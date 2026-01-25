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
dotnet publish src/HotelDroid.Client -c Release -o $clientOut
if ($LASTEXITCODE -ne 0) { throw "publish client failed (exit code $LASTEXITCODE)" }
dotnet publish src/HotelDroid.Api -c Release -o $apiOut
if ($LASTEXITCODE -ne 0) { throw "publish api failed (exit code $LASTEXITCODE)" }

Write-Host "4) Creating deployment package..."
Ensure-Dir (Join-Path $PSScriptRoot "..\artifacts")
if ((Test-Path $PackagePath) -and $Force) { Remove-Item $PackagePath -Force }
if ((Test-Path $PackagePath) -and (-not $Force)) { Write-Host "Package exists. Use -Force to overwrite." }

$tmp = Join-Path $env:TEMP "hoteldroid_package_$(Get-Random)"
Remove-Item -Recurse -Force -ErrorAction SilentlyContinue $tmp
Ensure-Dir $tmp

# copy published outputs
Copy-Item -Path (Join-Path $clientOut "*") -Destination (Join-Path $tmp "client") -Recurse -Force
Copy-Item -Path (Join-Path $apiOut "*") -Destination (Join-Path $tmp "api") -Recurse -Force

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
