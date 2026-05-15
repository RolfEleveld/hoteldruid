<#
.SYNOPSIS
  Stage 1 — Build and assemble HotelDroid for local development.

.DESCRIPTION
  Restores packages, publishes the Blazor client and the API, copies client files
  into the API wwwroot, then reads the static asset manifest to create non-fingerprinted
  file aliases. This lets UseStaticFiles() serve dotnet.js, dotnet.native.js, etc.
  without needing MapStaticAssets() or a slow double-publish.


    Artifacts land in:
        artifacts\client\  - raw Blazor client publish output
        artifacts\api\     - API publish output (with client embedded in wwwroot)
        artifacts\hoteldroid-package.zip - deployable package (always created)


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

function Ensure-Dir([string]$d) { if (-not (Test-Path $d)) { New-Item -ItemType Directory -Path $d | Out-Null } }
function Reset-ProjectIntermediates([string]$projectPath, [string]$configuration) {
    $projectDir = Join-Path $root $projectPath
    $objDir = Join-Path $projectDir (Join-Path 'obj' $configuration)
    $binDir = Join-Path $projectDir (Join-Path 'bin' $configuration)

    if (Test-Path $objDir) {
        Remove-Item -Recurse -Force -ErrorAction SilentlyContinue $objDir
    }

    if (Test-Path $binDir) {
        Remove-Item -Recurse -Force -ErrorAction SilentlyContinue $binDir
    }
}

# 0. Optional clean
if ($Clean) {
    Write-Host "Cleaning artifacts..." -ForegroundColor Cyan
    Remove-Item -Recurse -Force -ErrorAction SilentlyContinue $clientOut, $apiOut
}

# Clear stale intermediates before restore so publish can reuse the restored assets.
Reset-ProjectIntermediates 'src\HotelDroid.Shared' $Configuration
Reset-ProjectIntermediates 'src\HotelDroid.Client' $Configuration
Reset-ProjectIntermediates 'src\HotelDroid.Api' $Configuration

# 1. Restore
Write-Host "`n[1/4] Restoring packages..." -ForegroundColor Cyan
Push-Location $root
try {
    dotnet restore --nologo 2>&1 | Where-Object { $_ -notmatch 'up-to-date' }
    if ($LASTEXITCODE -ne 0) { throw "dotnet restore failed (exit $LASTEXITCODE)" }
} finally { Pop-Location }

# 2. Publish Client
Write-Host "`n[2/4] Publishing Blazor client..." -ForegroundColor Cyan
Push-Location $root
try {
    dotnet publish src/HotelDroid.Client -c $Configuration -o $clientOut --no-restore --nologo -q
    if ($LASTEXITCODE -ne 0) { throw "dotnet publish client failed (exit $LASTEXITCODE)" }
} finally { Pop-Location }

# 3. Publish API
Write-Host "`n[3/4] Publishing API..." -ForegroundColor Cyan
Push-Location $root
try {
    dotnet publish src/HotelDroid.Api -c $Configuration -o $apiOut --no-restore --nologo -q
    if ($LASTEXITCODE -ne 0) { throw "dotnet publish API failed (exit $LASTEXITCODE)" }
} finally { Pop-Location }

# 4. Embed client into API wwwroot + create non-fingerprinted aliases
Write-Host "`n[4/4] Embedding client into API wwwroot..." -ForegroundColor Cyan

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
$manifestPath = Join-Path $clientOut 'HotelDroid.Client.staticwebassets.endpoints.json'
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

Write-Host "`nBuild complete ($Configuration)." -ForegroundColor Green
Write-Host "   API artifacts : $apiOut"
Write-Host "   Client in     : $wwwroot"
Write-Host "`nRun the app with:"
Write-Host "   .\scripts\deploy.ps1" -ForegroundColor Yellow

# 5. Create deployment package (zip)
$packagePath = Join-Path $root 'artifacts\hoteldroid-package.zip'
if (Test-Path $packagePath) { Remove-Item $packagePath -Force }
Add-Type -AssemblyName System.IO.Compression.FileSystem
[System.IO.Compression.ZipFile]::CreateFromDirectory($apiOut, $packagePath)
Write-Host "Created deployment package: $packagePath" -ForegroundColor Green
