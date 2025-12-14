#!/usr/bin/env pwsh

<#
Build a minimal customer package containing only the installer script and launcher.
The installer downloads the latest phpdesktop and hoteldruid sources from public repos.

This approach:
  - Reduces package size dramatically (from 169MB to <1KB)
  - Always installs the latest versions from GitHub
  - Customer just extracts and runs the installer
  - Data is preserved on updates/redeploys

Usage:
  .\build_customer_package.ps1
  .\build_customer_package.ps1 -OutputZip .\out\HotelDruid-slim.zip
#>

[CmdletBinding()]
param(
	[string]$RepoRoot = $PSScriptRoot,
	[string]$OutputDir = (Join-Path $PSScriptRoot 'out'),
	[string]$OutputZip
)

$ErrorActionPreference = 'Stop'

# ============================================================================
# VALIDATION
# ============================================================================

$repoInstaller = Join-Path $RepoRoot 'install_release.ps1'
$repoLauncher = Join-Path $RepoRoot 'start-hoteldruid-desktop.ps1'

if (-not (Test-Path -LiteralPath $repoInstaller)) {
	throw "Missing file: $repoInstaller"
}
if (-not (Test-Path -LiteralPath $repoLauncher)) {
	throw "Missing file: $repoLauncher"
}

if (-not (Test-Path -LiteralPath $OutputDir)) {
	New-Item -Path $OutputDir -ItemType Directory -Force | Out-Null
}

if (-not $OutputZip) {
	$stamp = Get-Date -Format 'yyyyMMdd-HHmm'
	$OutputZip = Join-Path $OutputDir "HotelDruid-slim-$stamp.zip"
}

# ============================================================================
# CREATE MINIMAL PACKAGE
# ============================================================================

Write-Host "Building minimal customer package..." -ForegroundColor Cyan

$tempStaging = Join-Path $env:TEMP ("hoteldruid_pkg_{0}" -f (Get-Random))
if (Test-Path $tempStaging) {
	Remove-Item -Path $tempStaging -Recurse -Force
}
New-Item -Path $tempStaging -ItemType Directory -Force | Out-Null

# Copy installer and launcher
Write-Host "Adding installer and launcher scripts..." -ForegroundColor Cyan
Copy-Item -LiteralPath $repoInstaller -Destination (Join-Path $tempStaging 'install_release.ps1') -Force
Copy-Item -LiteralPath $repoLauncher -Destination (Join-Path $tempStaging 'start-hoteldruid-desktop.ps1') -Force

# Create comprehensive README
$readmePath = Join-Path $tempStaging 'README.md'
$readme = @"
# HotelDruid Desktop Edition

Lightweight, portable HotelDruid installation for Windows using PHP Desktop.

## What's Included

- **install_release.ps1** - Smart installer that downloads and installs the latest versions
- **start-hoteldruid-desktop.ps1** - Launcher script
- **README.md** - This file

## Installation (3 Steps)

### 1. Extract the ZIP

Extract this ZIP file anywhere on your computer (e.g., Desktop, Documents, Downloads folder).

### 2. Open PowerShell

Navigate to the extracted folder and open **PowerShell** as a regular user (no admin needed):

- Right-click in the folder
- Select "Open in Terminal" or "Open PowerShell window here"

### 3. Run the Installer

Choose your language and run the installer:

**English:**
``powershell
pwsh -ExecutionPolicy Bypass -File .\install_release.ps1 -Language en
``

**Italian (Italiano):**
``powershell
pwsh -ExecutionPolicy Bypass -File .\install_release.ps1 -Language it
``

**Spanish (Español):**
``powershell
pwsh -ExecutionPolicy Bypass -File .\install_release.ps1 -Language es
``

## What the Installer Does

1. **Downloads** the latest phpdesktop runtime from GitHub
2. **Downloads** the latest HotelDruid application from GitHub
3. **Extracts** both packages
4. **Detects** OneDrive (if installed) and creates a data folder for cloud backup
5. **Installs** everything to your AppData folder
6. **Creates** a Start Menu shortcut
7. **Preserves** your data on future updates

## After Installation

- Find **HotelDruid** in your Windows Start Menu and click to launch
- The first launch may take a moment while it initializes

## Update/Reinstall

To update to the latest version:

1. Extract a new copy of this ZIP (or download from https://github.com/adrianomacedo/hoteldruid)
2. Run the installer again with the same language
3. Your data is automatically preserved in the OneDrive folder

Example for update:
``powershell
pwsh -ExecutionPolicy Bypass -File .\install_release.ps1 -Language en
``

The installer will remember your previous location and data folder, keeping everything intact.

## Advanced Options

### Custom Installation Folder

Install to a specific location:

``powershell
pwsh -ExecutionPolicy Bypass -File .\install_release.ps1 -Language en -InstallDir "C:\MyApps\HotelDruid"
``

### Custom Data Folder

Use a specific data folder (must exist or use a path that can be created):

``powershell
pwsh -ExecutionPolicy Bypass -File .\install_release.ps1 -Language en -DataFolder "C:\MyData\HotelDruid"
``

### Additional Shortcuts

Create desktop or startup shortcuts:

``powershell
pwsh -ExecutionPolicy Bypass -File .\install_release.ps1 -Language en -CreateDesktopShortcut:\$true -CreateStartupShortcut:\$true
``

### Launch Immediately

Launch the application right after installation completes:

``powershell
pwsh -ExecutionPolicy Bypass -File .\install_release.ps1 -Language en -LaunchAfterInstall:\$true
``

## System Requirements

- Windows 10 or later
- PowerShell 5.0 or later (built into Windows)
- Internet connection (for initial download)
- ~200-300 MB free disk space

## Troubleshooting

**"PowerShell execution policy" error?**

Run:
``powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
``

Then run the installer again.

**Missing Start Menu shortcut?**

Run the installer again with the same language.

**OneDrive not detected?**

The installer will use your Documents folder instead. You can manually move your data folder to OneDrive later.

**Want to reinstall or move to a different location?**

Run the installer again and specify a new `-InstallDir` path.

## Data Locations

After installation, your data is stored in:

- **Installation folder:** `%LOCALAPPDATA%\HotelDruid` (usually `C:\Users\YourName\AppData\Local\HotelDruid`)
- **Data folder:** `C:\Users\YourName\OneDrive\HotelDruid\hoteldruid\data` (if OneDrive detected)
- **Settings:** `%APPDATA%\HotelDruid\deployment-settings.json`

## Support

For HotelDruid issues or updates, visit:
https://github.com/adrianomacedo/hoteldruid

## License

HotelDruid is released under the GNU Affero General Public License v3.

---

**Version:** Slim Distribution  
**Built:** $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')
"@

Set-Content -Path $readmePath -Value $readme -Encoding UTF8

# ============================================================================
# CREATE ZIP ARCHIVE
# ============================================================================

if (Test-Path -LiteralPath $OutputZip) {
	Remove-Item -LiteralPath $OutputZip -Force
}

Write-Host "Creating slim package ZIP: $OutputZip" -ForegroundColor Cyan
Compress-Archive -Path (Join-Path $tempStaging '*') -DestinationPath $OutputZip -Force

# ============================================================================
# CLEANUP AND REPORT
# ============================================================================

Remove-Item -Path $tempStaging -Recurse -Force -ErrorAction SilentlyContinue

$zipSize = (Get-Item $OutputZip).Length
$zipSizeMB = [math]::Round($zipSize / 1MB, 2)

Write-Host ""
Write-Host "✅ Package created successfully!" -ForegroundColor Green
Write-Host ""
Write-Host "Package: $OutputZip" -ForegroundColor Green
Write-Host "Size: $zipSizeMB MB" -ForegroundColor Green
Write-Host ""
Write-Host "Files included:" -ForegroundColor Yellow
Write-Host "  - install_release.ps1 (Smart installer, ~10 KB)"
Write-Host "  - start-hoteldruid-desktop.ps1 (Launcher, ~1 KB)"
Write-Host "  - README.md (Installation guide)"
Write-Host ""
Write-Host "Customer experience:" -ForegroundColor Yellow
Write-Host "  1. Extract the ZIP"
Write-Host "  2. Run: pwsh -ExecutionPolicy Bypass -File .\install_release.ps1 -Language es"
Write-Host "  3. Installer downloads latest phpdesktop + HotelDruid"
Write-Host "  4. Application launches"
Write-Host ""
Write-Host "On redeploy: Customer data is preserved in OneDrive" -ForegroundColor Yellow
Write-Host ""

Write-Output $OutputZip
