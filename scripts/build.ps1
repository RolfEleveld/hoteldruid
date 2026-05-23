<#
.SYNOPSIS
  Stage 1 — Build and assemble HotelDruid for local development.

.DESCRIPTION
  Restores packages, publishes the Blazor client and the API, copies client files
  into the API wwwroot, then reads the static asset manifest to create non-fingerprinted
    file aliases. It also generates CycloneDX SBOM artifacts for the deployable
    projects. This lets UseStaticFiles() serve dotnet.js, dotnet.native.js, etc.
    without needing MapStaticAssets() or a slow double-publish.


    Artifacts land in:
        artifacts\client\  - raw Blazor client publish output
        artifacts\api\     - API publish output (with client embedded in wwwroot)
        artifacts\sbom\    - CycloneDX SBOM JSON files
        artifacts\HotelDruid-package.zip - deployable package (always created)


    Run this whenever you change source code. Follow with deploy.ps1 to start the app.

.EXAMPLE
  .\scripts\build.ps1              # Debug build
  .\scripts\build.ps1 -Clean      # Clean then build
#>

param(
    [ValidateSet('Debug','Release')]
    [string]$Configuration = 'Debug',
    [switch]$Clean
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$root      = Join-Path $PSScriptRoot '..'
$clientOut = Join-Path $root 'artifacts\client'
$apiOut    = Join-Path $root 'artifacts\api'
$sbomOut   = Join-Path $root 'artifacts\sbom'
$toolPath  = Join-Path $root '.tools\cyclonedx'
$toolExe   = Join-Path $toolPath 'dotnet-CycloneDX.exe'

function Ensure-Dir([string]$d) { if (-not (Test-Path $d)) { New-Item -ItemType Directory -Path $d | Out-Null } }

function Ensure-CycloneDxTool {
    Ensure-Dir $toolPath

    if (Test-Path $toolExe) {
        return
    }

    Write-Host "Installing CycloneDX .NET tool..." -ForegroundColor Cyan
    dotnet tool install --tool-path $toolPath cyclonedx --version 6.2.0 --ignore-failed-sources
    if ($LASTEXITCODE -ne 0) { throw "dotnet tool install failed for cyclonedx (exit $LASTEXITCODE)" }
}

function New-Sbom([string]$projectPath, [string]$fileName, [string]$componentName) {
    $arguments = @(
        $projectPath
        '--output', $sbomOut
        '--filename', $fileName
        '--output-format', 'Json'
        '--configuration', $Configuration
        '--disable-package-restore'
        '--include-project-references'
        '--exclude-test-projects'
        '--no-serial-number'
        '--set-name', $componentName
    )

    & $toolExe @arguments

    if ($LASTEXITCODE -ne 0) { throw "CycloneDX generation failed for $projectPath (exit $LASTEXITCODE)" }
}

# 0. Optional clean
if ($Clean) {
    Write-Host "Cleaning previous build outputs..." -ForegroundColor Cyan
    $packagePath = Join-Path $root 'artifacts\HotelDruid-package.zip'
    Remove-Item -Recurse -Force -ErrorAction SilentlyContinue $clientOut, $apiOut, $sbomOut
    if (Test-Path $packagePath) {
        Remove-Item -Force -ErrorAction SilentlyContinue $packagePath
    }

    Push-Location $root
    try {
        $cleanProjects = @(
            'src/HotelDruid.Shared/HotelDruid.Shared.csproj',
            'src/HotelDruid.Client/HotelDruid.Client.csproj',
            'src/HotelDruid.Api/HotelDruid.Api.csproj'
        )

        foreach ($project in $cleanProjects) {
            dotnet clean $project -c $Configuration --nologo
            if ($LASTEXITCODE -ne 0) { throw "dotnet clean failed for $project (exit $LASTEXITCODE)" }
        }
    } finally { Pop-Location }
}

# 1. Restore
Write-Host "`n[1/6] Restoring packages..." -ForegroundColor Cyan
Push-Location $root
try {
    # HotelDruid.slnx currently only contains folders, so restore concrete projects.
    $restoreProjects = @(
        'src/HotelDruid.Client/HotelDruid.Client.csproj',
        'src/HotelDruid.Api/HotelDruid.Api.csproj'
    )

    foreach ($project in $restoreProjects) {
        dotnet restore $project --nologo 2>&1 | Where-Object { $_ -notmatch 'up-to-date' }
        if ($LASTEXITCODE -ne 0) { throw "dotnet restore failed for $project (exit $LASTEXITCODE)" }
    }
} finally { Pop-Location }

# 2. Publish Client
Write-Host "`n[2/6] Publishing Blazor client..." -ForegroundColor Cyan
Push-Location $root
try {
    dotnet publish src/HotelDruid.Client -c $Configuration -o $clientOut --no-restore --nologo
    if ($LASTEXITCODE -ne 0) { throw "dotnet publish client failed (exit $LASTEXITCODE)" }
} finally { Pop-Location }

# 3. Publish API
Write-Host "`n[3/6] Publishing API..." -ForegroundColor Cyan
Push-Location $root
try {
    dotnet publish src/HotelDruid.Api -c $Configuration -o $apiOut --no-restore --nologo
    if ($LASTEXITCODE -ne 0) { throw "dotnet publish API failed (exit $LASTEXITCODE)" }
} finally { Pop-Location }

# 4. Embed client into API wwwroot + create non-fingerprinted aliases
Write-Host "`n[4/6] Embedding client into API wwwroot..." -ForegroundColor Cyan

$wwwroot   = Join-Path $apiOut 'wwwroot'
$clientWww = Join-Path $clientOut 'wwwroot'

Remove-Item -Recurse -Force -ErrorAction SilentlyContinue $wwwroot
Ensure-Dir $wwwroot

if (Test-Path $clientWww) {
    Copy-Item -Path (Join-Path $clientWww '*') -Destination $wwwroot -Recurse -Force
} else {
    Copy-Item -Path (Join-Path $clientOut '*') -Destination $wwwroot -Recurse -Force
}

if (-not (Test-Path (Join-Path $wwwroot 'index.html'))) {
    throw "index.html not found in $wwwroot. Check client publish output."
}

# Resolve blazor.webassembly#[.{fingerprint}].js token in index.html
$indexHtml = Join-Path $wwwroot 'index.html'
$content   = Get-Content $indexHtml -Raw
$frameworkDir = Join-Path $wwwroot '_framework'
$bwJs = Get-ChildItem $frameworkDir -Filter 'blazor.webassembly.*.js' |
    Where-Object { $_.Name -notmatch '\.(br|gz)$' } | Select-Object -First 1
if ($bwJs) {
    if ($bwJs.BaseName -match '^blazor\.webassembly\.(.+)$') { $fp = ".$($Matches[1])" } else { $fp = '' }
    $resolved = $content -replace '#\[\.{fingerprint}\]', $fp
    if ($resolved -ne $content) {
        Set-Content $indexHtml $resolved -NoNewline
        Write-Host "  index.html: blazor.webassembly$fp.js" -ForegroundColor DarkGray
    }
} else {
    Write-Warning "blazor.webassembly.*.js not found - fingerprint token not resolved."
}

# Read the static asset manifest to create non-fingerprinted file aliases.
# .NET 10 publishes only fingerprinted files (dotnet.abc123.js) but the bootstrapper
# requests the plain names (dotnet.js). We copy the real file under the plain name so
# UseStaticFiles() can serve it without MapStaticAssets().
$manifestPath = Join-Path $clientOut 'HotelDruid.Client.staticwebassets.endpoints.json'
if (Test-Path $manifestPath) {
    $manifest = Get-Content $manifestPath -Raw | ConvertFrom-Json

    $aliases = $manifest.Endpoints | Where-Object {
        $_.Route -and $_.AssetFile -and
        $_.Route -ne $_.AssetFile -and
        $_.Route -notmatch '\.[a-z0-9]{8,12}\.' -and
        $_.AssetFile -notmatch '\.(br|gz)$' -and
        (-not $_.Selectors -or $_.Selectors.Count -eq 0)
    }

    $aliasCount = 0
    foreach ($ep in $aliases) {
        $dest = Join-Path $wwwroot ($ep.Route -replace '/', '\')
        $src  = Join-Path $wwwroot ($ep.AssetFile -replace '/', '\')
        if ((Test-Path $src) -and -not (Test-Path $dest)) {
            Copy-Item $src $dest
            $aliasCount++
        }
    }
    Write-Host "  Created $aliasCount non-fingerprinted file aliases from manifest." -ForegroundColor DarkGray
} else {
    Write-Warning "Client manifest not found at $manifestPath - dotnet.js aliases NOT created."
}

# 5. Generate SBOM artifacts
Write-Host "`n[5/6] Generating SBOM artifacts..." -ForegroundColor Cyan
Ensure-Dir $sbomOut
Ensure-CycloneDxTool

New-Sbom -projectPath (Join-Path $root 'src/HotelDruid.Api/HotelDruid.Api.csproj') `
    -fileName 'HotelDruid.Api.sbom.cdx.json' `
    -componentName 'HotelDruid.Api'

New-Sbom -projectPath (Join-Path $root 'src/HotelDruid.Client/HotelDruid.Client.csproj') `
    -fileName 'HotelDruid.Client.sbom.cdx.json' `
    -componentName 'HotelDruid.Client'

Write-Host "   SBOM artifacts: $sbomOut" -ForegroundColor DarkGray

# 6. Create deployment package (zip)
Write-Host "`n[6/6] Creating deployment package..." -ForegroundColor Cyan
$packagePath = Join-Path $root 'artifacts\HotelDruid-package.zip'
if (Test-Path $packagePath) { Remove-Item $packagePath -Force }
Add-Type -AssemblyName System.IO.Compression.FileSystem
[System.IO.Compression.ZipFile]::CreateFromDirectory($apiOut, $packagePath)
Write-Host "Created deployment package: $packagePath" -ForegroundColor Green

Write-Host "`nBuild complete ($Configuration)." -ForegroundColor Green
Write-Host "   API artifacts : $apiOut"
Write-Host "   Client in     : $wwwroot"
Write-Host "   SBOM artifacts: $sbomOut"
Write-Host "`nRun the app with:"
Write-Host "   .\scripts\deploy.ps1" -ForegroundColor Yellow

