#!/usr/bin/env pwsh

<#
Build a customer-ready ZIP by:
  - downloading the latest phpdesktop release
  - extracting to a staging folder
  - copying your app (hoteldruid/) into staging
  - applying your phpdesktop/settings.json
  - pruning caches, sample www, and unnecessary locales
  - producing a smaller distributable archive

Usage:
  .\build_customer_package.ps1
  .\build_customer_package.ps1 -KeepAllLocales
  .\build_customer_package.ps1 -OutputZip .\out\HotelDruid-customer.zip
#>

[CmdletBinding()]
param(
	[string]$RepoRoot = $PSScriptRoot,
	[string]$OutputDir = (Join-Path $PSScriptRoot 'out'),
	[string]$OutputZip,
	[string]$WorkDir = (Join-Path $env:TEMP ("hoteldruid_pkg_{0}" -f (Get-Random))),
	[switch]$KeepAllLocales,
	[string[]]$KeepLocales = @('en-US', 'en-GB', 'it', 'es', 'es-419'),
	[switch]$SkipDownload,
	[string]$PhpDesktopZipPath,
	[switch]$NoPrune
)

$ErrorActionPreference = 'Stop'

function Get-LatestPhpDesktopAssetUrl {
	param(
		[string]$Repo = 'cztomczak/phpdesktop',
		[string]$AssetNameRegex = 'phpdesktop-chrome-.*-php-.*\.zip$'
	)

	$uri = "https://api.github.com/repos/$Repo/releases/latest"
	$headers = @{
		'User-Agent' = 'HotelDruid-Packager'
		'Accept'     = 'application/vnd.github+json'
	}

	$release = Invoke-RestMethod -Uri $uri -Headers $headers -Method GET
	if (-not $release -or -not $release.assets) {
		throw "GitHub API returned no assets for $Repo."
	}

	$asset = $release.assets | Where-Object { $_.name -match $AssetNameRegex } | Select-Object -First 1
	if (-not $asset) {
		$names = ($release.assets | ForEach-Object { $_.name }) -join ', '
		throw "Could not find a phpdesktop zip asset matching '$AssetNameRegex'. Assets: $names"
	}

	return [pscustomobject]@{
		Url     = $asset.browser_download_url
		Name    = $asset.name
		TagName = $release.tag_name
	}
}

function Ensure-EmptyDirectory {
	param([string]$Path)
	if (Test-Path -LiteralPath $Path) {
		Remove-Item -LiteralPath $Path -Recurse -Force
	}
	New-Item -Path $Path -ItemType Directory -Force | Out-Null
}

function Copy-Directory {
	param(
		[string]$Source,
		[string]$Destination
	)
	if (-not (Test-Path -LiteralPath $Source)) {
		throw "Source not found: $Source"
	}
	New-Item -Path $Destination -ItemType Directory -Force | Out-Null
	Copy-Item -Path (Join-Path $Source '*') -Destination $Destination -Recurse -Force
}

$hoteldruidDir = Join-Path $RepoRoot 'hoteldruid'
$repoPhpDesktopSettings = Join-Path $RepoRoot 'phpdesktop\settings.json'
$repoInstaller = Join-Path $RepoRoot 'install_release.ps1'
$repoLauncher = Join-Path $RepoRoot 'start-hoteldruid-desktop.ps1'

if (-not (Test-Path -LiteralPath $hoteldruidDir)) { throw "Missing folder: $hoteldruidDir" }
if (-not (Test-Path -LiteralPath $repoPhpDesktopSettings)) { throw "Missing file: $repoPhpDesktopSettings" }
if (-not (Test-Path -LiteralPath $repoInstaller)) { throw "Missing file: $repoInstaller" }
if (-not (Test-Path -LiteralPath $repoLauncher)) { throw "Missing file: $repoLauncher" }

Ensure-EmptyDirectory -Path $WorkDir

$downloadDir = Join-Path $WorkDir 'download'
$extractDir = Join-Path $WorkDir 'extract'
$stagingDir = Join-Path $WorkDir 'staging'
Ensure-EmptyDirectory -Path $downloadDir
Ensure-EmptyDirectory -Path $extractDir
Ensure-EmptyDirectory -Path $stagingDir

if (-not (Test-Path -LiteralPath $OutputDir)) {
	New-Item -Path $OutputDir -ItemType Directory -Force | Out-Null
}

if (-not $OutputZip) {
	$stamp = Get-Date -Format 'yyyyMMdd-HHmm'
	$OutputZip = Join-Path $OutputDir "HotelDruid-customer-$stamp.zip"
}

if (-not $SkipDownload) {
	$asset = Get-LatestPhpDesktopAssetUrl
	$PhpDesktopZipPath = Join-Path $downloadDir $asset.Name
	Write-Host "Downloading phpdesktop: $($asset.TagName) ($($asset.Name))" -ForegroundColor Cyan
	Invoke-WebRequest -Uri $asset.Url -OutFile $PhpDesktopZipPath -UseBasicParsing
} else {
	if (-not $PhpDesktopZipPath) { throw "-SkipDownload requires -PhpDesktopZipPath" }
	if (-not (Test-Path -LiteralPath $PhpDesktopZipPath)) { throw "Zip not found: $PhpDesktopZipPath" }
}

Write-Host "Extracting phpdesktop..." -ForegroundColor Cyan
Expand-Archive -Path $PhpDesktopZipPath -DestinationPath $extractDir -Force

# Find extracted root containing phpdesktop-chrome.exe
$phpDesktopExe = Get-ChildItem -Path $extractDir -Recurse -File -Filter 'phpdesktop-chrome.exe' | Select-Object -First 1
if (-not $phpDesktopExe) {
	throw "Could not find phpdesktop-chrome.exe after extraction."
}
$phpDesktopRoot = Split-Path $phpDesktopExe.FullName -Parent

$stagedPhpDesktop = Join-Path $stagingDir 'phpdesktop'
$stagedHotelDruid = Join-Path $stagingDir 'hoteldruid'

Write-Host "Staging phpdesktop..." -ForegroundColor Cyan
Copy-Directory -Source $phpDesktopRoot -Destination $stagedPhpDesktop

Write-Host "Staging hoteldruid app..." -ForegroundColor Cyan
Copy-Directory -Source $hoteldruidDir -Destination $stagedHotelDruid

Write-Host "Applying phpdesktop settings.json..." -ForegroundColor Cyan
Copy-Item -LiteralPath $repoPhpDesktopSettings -Destination (Join-Path $stagedPhpDesktop 'settings.json') -Force

Write-Host "Adding installer + launcher..." -ForegroundColor Cyan
Copy-Item -LiteralPath $repoInstaller -Destination (Join-Path $stagingDir 'install_release.ps1') -Force
Copy-Item -LiteralPath $repoLauncher -Destination (Join-Path $stagingDir 'start-hoteldruid-desktop.ps1') -Force

$readmePath = Join-Path $stagingDir 'README.md'
$readme = @"
# HotelDruid Desktop (PHP Desktop)

This ZIP contains everything needed to run HotelDruid on Windows (phpdesktop + PHP + HotelDruid).

## Install (customer)

1. Extract this ZIP anywhere (e.g., `Downloads`).
2. Open the extracted folder.
3. Run the installer:

	 - Italian: `pwsh -ExecutionPolicy Bypass -File .\\install_release.ps1 -Language it`
	 - English: `pwsh -ExecutionPolicy Bypass -File .\\install_release.ps1 -Language en`
	 - Spanish: `pwsh -ExecutionPolicy Bypass -File .\\install_release.ps1 -Language es`

4. Start the app from the Start Menu shortcut **HotelDruid**.

## Optional

- Install to a specific folder:

	`pwsh -ExecutionPolicy Bypass -File .\\install_release.ps1 -Language it -InstallDir "C:\\HotelDruid"`

- Create a Desktop shortcut:

	`pwsh -ExecutionPolicy Bypass -File .\\install_release.ps1 -Language it -CreateDesktopShortcut:$true`

- Launch after install:

	`pwsh -ExecutionPolicy Bypass -File .\\install_release.ps1 -Language it -LaunchAfterInstall:$true`

## Developer / Sandbox test

From the source repo:

`pwsh -ExecutionPolicy Bypass -File .\\test_sandbox_install.ps1 -Language en`

This installs into `out\\sandbox\\installed` without creating shortcuts.
"@
Set-Content -Path $readmePath -Value $readme -Encoding UTF8

if (-not $NoPrune) {
	Write-Host "Pruning unnecessary files to reduce size..." -ForegroundColor Cyan

	# Remove runtime caches/logs and phpdesktop sample site
	foreach ($p in @(
		(Join-Path $stagedPhpDesktop 'webcache'),
		(Join-Path $stagedPhpDesktop 'debug.log'),
		(Join-Path $stagedPhpDesktop 'www')
	)) {
		if (Test-Path -LiteralPath $p) {
			Remove-Item -LiteralPath $p -Recurse -Force
		}
	}

	# Keep only selected locales (.pak files)
	if (-not $KeepAllLocales) {
		$localesDir = Join-Path $stagedPhpDesktop 'locales'
		if (Test-Path -LiteralPath $localesDir) {
			$keepSet = @{}
			foreach ($loc in $KeepLocales) { $keepSet["$loc.pak"] = $true }

			Get-ChildItem -Path $localesDir -File -Filter '*.pak' | ForEach-Object {
				if (-not $keepSet.ContainsKey($_.Name)) {
					Remove-Item -LiteralPath $_.FullName -Force
				}
			}
		}
	}

	# Prune PHP dev/debug binaries and docs (keep runtime + extensions)
	$phpDir = Join-Path $stagedPhpDesktop 'php'
	if (Test-Path -LiteralPath $phpDir) {
		$removeFiles = @(
			'deplister.exe',
			'php.ini-development',
			'php.ini-production',
			'news.txt',
			'readme-redist-bins.txt',
			'README.md',
			'snapshot.txt',
			'phar.phar.bat',
			'pharcommand.phar',
			'phpdbg.exe',
			'php8phpdbg.dll',
			'php8embed.lib'
		)

		foreach ($name in $removeFiles) {
			$candidate = Join-Path $phpDir $name
			if (Test-Path -LiteralPath $candidate) {
				Remove-Item -LiteralPath $candidate -Force
			}
		}
	}
}

if (Test-Path -LiteralPath $OutputZip) {
	Remove-Item -LiteralPath $OutputZip -Force
}

Write-Host "Creating zip: $OutputZip" -ForegroundColor Cyan
Compress-Archive -Path (Join-Path $stagingDir '*') -DestinationPath $OutputZip -Force

Write-Host "Done." -ForegroundColor Green
Write-Output $OutputZip