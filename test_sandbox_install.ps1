#!/usr/bin/env pwsh

<#
Builds a customer ZIP and installs it into a sandbox folder for testing.

Usage:
	.\test_sandbox_install.ps1
	.\test_sandbox_install.ps1 -Language it -Launch
#>

[CmdletBinding()]
param(
		[ValidateSet('it', 'en', 'es')]
		[string]$Language,
		[string]$SandboxRoot = (Join-Path $PSScriptRoot 'out\sandbox'),
		[switch]$Launch
)

$ErrorActionPreference = 'Stop'

New-Item -Path $SandboxRoot -ItemType Directory -Force | Out-Null

Write-Host "Building customer package..." -ForegroundColor Cyan
$zipPath = & (Join-Path $PSScriptRoot 'build_customer_package.ps1')
if (-not $zipPath -or -not (Test-Path -LiteralPath $zipPath)) {
		throw "Build did not produce a zip path."
}

$extractDir = Join-Path $SandboxRoot 'extracted'
if (Test-Path -LiteralPath $extractDir) { Remove-Item -LiteralPath $extractDir -Recurse -Force }
New-Item -Path $extractDir -ItemType Directory -Force | Out-Null

Write-Host "Extracting to sandbox..." -ForegroundColor Cyan
Expand-Archive -Path $zipPath -DestinationPath $extractDir -Force

$installDir = Join-Path $SandboxRoot 'installed'

Write-Host "Running installer into: $installDir" -ForegroundColor Cyan
$installer = Join-Path $extractDir 'install_release.ps1'
if (-not (Test-Path -LiteralPath $installer)) { throw "Missing installer in extracted package." }

$installerParams = @{
	ZipPath               = $zipPath
	InstallDir            = $installDir
	CreateStartMenuShortcut = $false
	CreateStartupShortcut = $false
	CreateDesktopShortcut = $false
}
if ($Language) { $installerParams.Language = $Language }
if ($Launch) { $installerParams.LaunchAfterInstall = $true }

& $installer @installerParams

Write-Host "Sandbox install complete." -ForegroundColor Green