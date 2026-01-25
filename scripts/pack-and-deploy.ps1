<#
pack-and-deploy.ps1

Creates a deployment package for HotelDroid and optionally deploys it to a Windows machine.

Steps performed:
 1. prepare (dotnet restore)
 2. build solution (dotnet build Release)
 3. publish client & api (dotnet publish)
 4. create zip deployment package (artifacts\hoteldroid-package.zip)
 5. optionally deploy package to an install folder, create Start Menu link, registry uninstall entry, and uninstall script

Usage examples:
  # create package only
  .\scripts\pack-and-deploy.ps1 -PackageOnly

  # create package and deploy to Program Files (requires elevation to write registry HKLM)
  .\scripts\pack-and-deploy.ps1 -Deploy -InstallDir "C:\Program Files\HotelDroid"

Parameters:
  -InstallDir: target installation folder (default: C:\Program Files\HotelDroid)
  -PackagePath: path for created zip package (default: artifacts\hoteldroid-package.zip)
  -Deploy: switch to extract package to InstallDir and register Start Menu + Uninstall
  -PackageOnly: switch to only create package and skip deployment
  -Force: overwrite existing package/install

Notes:
  - Deploying will attempt to create an uninstall registry entry under HKLM; run in elevated shell when deploying system-wide.
  - The script copies `scripts\deploy-api-local.ps1` into the package and sets the default Start Menu link to open https://localhost:5001/.
#>

param(
    [string]$InstallDir = "C:\Program Files\HotelDroid",
    [string]$PackagePath = "artifacts\hoteldroid-package.zip",
    [switch]$Deploy,
    [switch]$PackageOnly,
    [switch]$Force
)

Set-StrictMode -Version Latest

function Ensure-Dir([string]$d) { if (-not (Test-Path $d)) { New-Item -ItemType Directory -Path $d | Out-Null } }

Write-Host "1) Preparing: restore packages..."
dotnet restore
if ($LASTEXITCODE -ne 0) { throw "dotnet restore failed (exit code $LASTEXITCODE)" }

Write-Host "2) Building solution (Release)..."
dotnet build -c Release
if ($LASTEXITCODE -ne 0) { throw "dotnet build failed (exit code $LASTEXITCODE)" }

Write-Host "3) Publishing client and API..."
$clientOut = Join-Path $PSScriptRoot "..\artifacts\client"
$apiOut = Join-Path $PSScriptRoot "..\artifacts\api"
Remove-Item -Recurse -Force -ErrorAction SilentlyContinue $clientOut, $apiOut

# Ensure third-party client libraries are restored via LibMan (if libman is available)
Write-Host "-> Restoring client libraries with LibMan (required)..."
$libman = Get-Command libman -ErrorAction SilentlyContinue
if ($null -eq $libman) {
    Write-Host "LibMan CLI not found. Installing Microsoft.Web.LibraryManager.Cli as a user tool..."
    dotnet tool install -g Microsoft.Web.LibraryManager.Cli
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Failed to install LibMan CLI (dotnet tool install failed)."
        throw "LibMan CLI installation failed"
    }
    # Ensure user tools folder is on PATH for the current session
    $dotnetTools = Join-Path $env:USERPROFILE ".dotnet\tools"
    if (-not ($env:PATH -split ';' | Where-Object { $_ -eq $dotnetTools })) {
        $env:PATH = $env:PATH + ";" + $dotnetTools
    }
    $libman = Get-Command libman -ErrorAction SilentlyContinue
    if ($null -eq $libman) {
        Write-Error "LibMan CLI was installed but not found on PATH. Ensure '$dotnetTools' is on your PATH and re-run the script."
        throw "LibMan CLI not available after install"
    }
}

Push-Location (Join-Path $PSScriptRoot "..\src\HotelDroid.Api")
try {
    libman restore
    if ($LASTEXITCODE -ne 0) { throw "libman restore failed with exit code $LASTEXITCODE" }
} catch {
    Pop-Location
    Write-Error "libman restore failed: $_"
    throw "libman restore failed"
} finally { if ((Get-Location).Path -ne (Join-Path $PSScriptRoot "..\src\HotelDroid.Api")) { Pop-Location } }

# Publish Blazor client (precompress on publish)
dotnet publish src/HotelDroid.Client -c Release -o $clientOut -p:BlazorEnableCompression=true
if ($LASTEXITCODE -ne 0) { throw "publish client failed (exit code $LASTEXITCODE)" }

# Publish API as a single-file executable for Windows x64 so deployment is a single host exe
# Ensure any running instance of the published API is stopped so the publish can overwrite the exe
try { Stop-Process -Name HotelDroid.Api -Force -ErrorAction SilentlyContinue } catch {}
Remove-Item -Force -ErrorAction SilentlyContinue (Join-Path $apiOut 'HotelDroid.Api.exe')
dotnet publish src/HotelDroid.Api -c Release -r win-x64 -p:PublishSingleFile=true -p:PublishTrimmed=false -o $apiOut
if ($LASTEXITCODE -ne 0) { throw "publish api failed (exit code $LASTEXITCODE)" }

# Copy published client files into the API wwwroot so API serves the Blazor app from the same origin
$wwwroot = Join-Path $apiOut 'wwwroot'
Remove-Item -Recurse -Force -ErrorAction SilentlyContinue $wwwroot
New-Item -ItemType Directory -Path $wwwroot | Out-Null

# The Blazor publish output commonly places client files under a nested 'wwwroot' folder
# e.g. artifacts/client/wwwroot. Prefer copying the contents of that folder when present.
$clientWww = Join-Path $clientOut 'wwwroot'
if (Test-Path $clientWww) {
    Copy-Item -Path (Join-Path $clientWww '*') -Destination $wwwroot -Recurse -Force
} else {
    Copy-Item -Path (Join-Path $clientOut '*') -Destination $wwwroot -Recurse -Force
}

# Ensure client assets actually live under the API's wwwroot directory.
# Some publish layouts accidentally place files at the API root; move them
# into wwwroot so runtime static file middleware works reliably after deploy.
if (-not (Test-Path (Join-Path $wwwroot 'index.html'))) {
    Write-Host "Index not found under $wwwroot — searching for stray client files..."
    $foundIndex = Get-ChildItem -Path $apiOut -Filter 'index.html' -Recurse -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($null -ne $foundIndex) {
        Write-Host "Moving stray index.html from $($foundIndex.DirectoryName) to $wwwroot"
        Ensure-Dir $wwwroot
        Move-Item -Path $foundIndex.FullName -Destination (Join-Path $wwwroot 'index.html') -Force
    }

    # Move any top-level _framework folder into wwwroot as well
    $rootFramework = Join-Path $apiOut '_framework'
    if (Test-Path $rootFramework -PathType Container) {
        Write-Host "Moving stray _framework from $rootFramework to $wwwroot\_framework"
        Ensure-Dir (Join-Path $wwwroot '_framework')
        Copy-Item -Path (Join-Path $rootFramework '*') -Destination (Join-Path $wwwroot '_framework') -Recurse -Force
        Remove-Item -Recurse -Force $rootFramework -ErrorAction SilentlyContinue
    }
}

# Some runtimes expect ICU payloads under _framework/icu. Duplicate the ICU files there to satisfy lookups.
$frameworkDir = Join-Path $wwwroot '_framework'
$icuDir = Join-Path $frameworkDir 'icu'
if (Test-Path $frameworkDir -PathType Container) {
    Ensure-Dir $icuDir
    Get-ChildItem -Path $frameworkDir -Filter 'icudt_*.dat*' -File -ErrorAction SilentlyContinue | ForEach-Object {
        Copy-Item -Path $_.FullName -Destination (Join-Path $icuDir $_.Name) -Force
    }
}

# Final sanity check: fail early if client entrypoint missing
if (-not (Test-Path (Join-Path $wwwroot 'index.html'))) {
    Write-Error "Client index.html not found under $wwwroot after copy/move — aborting package creation."
    throw "Client files missing from API wwwroot"
}

# Ensure a run launcher is present in the API root to start the single exe with an optional cert thumbprint
$runBatPath = Join-Path $apiOut 'run.bat'
$runContent = @"
@echo off
set ASPNETCORE_Kestrel__Certificates__Default__Thumbprint=%ASPNETCORE_Kestrel__Certificates__Default__Thumbprint%
cd /d "%~dp0"
if exist HotelDroid.Api.exe (
    start "HotelDroid API" /b .\HotelDroid.Api.exe
) else (
    start "HotelDroid API" /b dotnet HotelDroid.Api.dll
)
"@
$runContent | Out-File -FilePath $runBatPath -Encoding ASCII -Force

Write-Host "4) Creating deployment package..."
Ensure-Dir (Join-Path $PSScriptRoot "..\artifacts")
if ((Test-Path $PackagePath) -and $Force) {
    Write-Host "Removing existing package: $PackagePath"
    Remove-Item $PackagePath -Force
}
elseif ((Test-Path $PackagePath) -and (-not $Force)) {
    Write-Host "Package exists. Use -Force to overwrite. Aborting."
    Remove-Item -Recurse -Force $tmp -ErrorAction SilentlyContinue
    exit 1
}

$tmp = Join-Path $env:TEMP "hoteldroid_package_$(Get-Random)"
Remove-Item -Recurse -Force -ErrorAction SilentlyContinue $tmp
Ensure-Dir $tmp

# copy published outputs (preserve folder names)
$clientDest = Join-Path $tmp "client"
$apiDest = Join-Path $tmp "api"
Ensure-Dir $clientDest
Ensure-Dir $apiDest
Copy-Item -Path $clientOut -Destination $tmp -Recurse -Force
Copy-Item -Path $apiOut -Destination $tmp -Recurse -Force

# include scripts and README
Ensure-Dir (Join-Path $tmp "scripts")
Copy-Item -Path scripts\* -Destination (Join-Path $tmp "scripts") -Recurse -Force
Copy-Item -Path README.md -Destination $tmp -Force -ErrorAction SilentlyContinue

Add-Type -AssemblyName System.IO.Compression.FileSystem
[System.IO.Compression.ZipFile]::CreateFromDirectory($tmp, $PackagePath)
Write-Host "Package created: $PackagePath"

if ($PackageOnly) { Remove-Item -Recurse -Force $tmp; Write-Host "Package-only mode - done."; exit 0 }

if ($Deploy) {
    Write-Host "5) Deploying package to '$InstallDir'..."
    if (-not (Test-Path $PackagePath)) { throw "Package not found: $PackagePath" }

    # create install dir
    if ((Test-Path $InstallDir) -and $Force) { Remove-Item -Recurse -Force $InstallDir }
    Ensure-Dir $InstallDir

    # extract package into install dir
    Add-Type -AssemblyName System.IO.Compression.FileSystem
    [System.IO.Compression.ZipFile]::ExtractToDirectory($PackagePath, $InstallDir)

    # create uninstall script inside install dir
    $uninstallPath = Join-Path $InstallDir "uninstall.ps1"
    $uninstallContent = @"
param()
Write-Host 'Stopping HotelDroid (no services to stop by default).'
Start-Sleep -Seconds 1
Write-Host 'Removing installation files...'
Remove-Item -Recurse -Force -ErrorAction SilentlyContinue '$InstallDir'

# remove Start Menu link
$startMenu = Join-Path $env:ProgramData 'Microsoft\Windows\Start Menu\Programs\HotelDroid'
if (Test-Path $startMenu) { Remove-Item -Recurse -Force -ErrorAction SilentlyContinue $startMenu }

# remove uninstall registry entry
try {
    Remove-Item -Path 'HKLM:\Software\Microsoft\Windows\CurrentVersion\Uninstall\HotelDroid' -ErrorAction SilentlyContinue
} catch {}

Write-Host 'Uninstall complete.'
"@
    $uninstallContent | Out-File -FilePath $uninstallPath -Encoding UTF8 -Force

    # create Start Menu internet shortcut (opens app URL)
    $startMenuDir = Join-Path $env:ProgramData 'Microsoft\Windows\Start Menu\Programs\HotelDroid'
    Ensure-Dir $startMenuDir
    $urlFile = Join-Path $startMenuDir 'HotelDruid.url'
    $urlContent = @"
[InternetShortcut]
URL=https://localhost:5001/
IconFile=%SystemRoot%\system32\shell32.dll
IconIndex=1
"@
    $urlContent | Out-File -FilePath $urlFile -Encoding ASCII -Force

    # write uninstall registry entry (Apps & features)
    $displayName = 'HotelDroid'
    $publisher = 'HotelDroid Migration'
    $displayVersion = '0.1.0'
    $uninstallString = "powershell.exe -ExecutionPolicy Bypass -File '$uninstallPath'"

    try {
        New-Item -Path 'HKLM:\Software\Microsoft\Windows\CurrentVersion\Uninstall\HotelDroid' -Force | Out-Null
        Set-ItemProperty -Path 'HKLM:\Software\Microsoft\Windows\CurrentVersion\Uninstall\HotelDroid' -Name 'DisplayName' -Value $displayName
        Set-ItemProperty -Path 'HKLM:\Software\Microsoft\Windows\CurrentVersion\Uninstall\HotelDroid' -Name 'DisplayVersion' -Value $displayVersion
        Set-ItemProperty -Path 'HKLM:\Software\Microsoft\Windows\CurrentVersion\Uninstall\HotelDroid' -Name 'Publisher' -Value $publisher
        Set-ItemProperty -Path 'HKLM:\Software\Microsoft\Windows\CurrentVersion\Uninstall\HotelDroid' -Name 'InstallLocation' -Value $InstallDir
        Set-ItemProperty -Path 'HKLM:\Software\Microsoft\Windows\CurrentVersion\Uninstall\HotelDroid' -Name 'UninstallString' -Value $uninstallString
        Set-ItemProperty -Path 'HKLM:\Software\Microsoft\Windows\CurrentVersion\Uninstall\HotelDroid' -Name 'DisplayIcon' -Value "$InstallDir\api\HotelDroid.Api.dll"
        Write-Host 'Registered application in Apps & features (HKLM).'
    } catch {
        Write-Warning 'Failed to write HKLM uninstall key. Run script elevated to register in Apps & features.'
    }

    Write-Host "Deployment complete. Installed to: $InstallDir"
    Write-Host "Use '$uninstallPath' to remove the installation, or uninstall via Apps & features if registry entry was created."
}

Remove-Item -Recurse -Force $tmp
