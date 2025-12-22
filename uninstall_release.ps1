#!/usr/bin/env pwsh

<#
HotelDruid Uninstaller
Removes the installed desktop app, shortcuts, and cached settings.
Data folder removal is optional for safety.
Usage:
    pwsh -ExecutionPolicy Bypass -File .\uninstall_release.ps1
    pwsh -ExecutionPolicy Bypass -File .\uninstall_release.ps1 -Force
    pwsh -ExecutionPolicy Bypass -File .\uninstall_release.ps1 -RemoveDataFolder -Force
#>

[CmdletBinding()]
param(
    [string]$InstallDir = (Join-Path $env:LOCALAPPDATA 'HotelDruid'),
    [string]$DataFolder = '',
    [switch]$RemoveDataFolder,
    [switch]$RemoveSettings,
    [switch]$Silent,
    [switch]$Force
)

$ErrorActionPreference = 'Stop'

function Get-PreviousSettings {
    $settingsDir = Join-Path $env:APPDATA 'HotelDruid'
    $settingsFile = Join-Path $settingsDir 'deployment-settings.json'
    if (Test-Path -LiteralPath $settingsFile) {
        try { return Get-Content -LiteralPath $settingsFile -Raw | ConvertFrom-Json } catch { return $null }
    }
    return $null
}

function Stop-PhpDesktop {
    try {
        $processes = Get-Process -Name 'phpdesktop-chrome' -ErrorAction SilentlyContinue
        if ($processes) { $processes | Stop-Process -Force -ErrorAction SilentlyContinue; Start-Sleep -Milliseconds 400 }
    } catch {}
}

function Remove-PathSafe {
    param([string]$Path)
    if (-not $Path) { return }
    if (Test-Path -LiteralPath $Path) {
        Remove-Item -LiteralPath $Path -Recurse -Force -ErrorAction SilentlyContinue
    }
}

try {
    $settings = Get-PreviousSettings
    if ($settings) {
        if (-not $PSBoundParameters.ContainsKey('InstallDir') -or [string]::IsNullOrWhiteSpace($InstallDir)) { $InstallDir = $settings.InstallDirectory }
        if (-not $PSBoundParameters.ContainsKey('DataFolder') -or [string]::IsNullOrWhiteSpace($DataFolder)) { $DataFolder = $settings.DataDirectory }
    }
    if (-not $InstallDir) { $InstallDir = Join-Path $env:LOCALAPPDATA 'HotelDruid' }

    $settingsDir = Join-Path $env:APPDATA 'HotelDruid'
    $settingsFile = Join-Path $settingsDir 'deployment-settings.json'
    $validationLog = Join-Path $settingsDir 'install-validation.log'

    $startMenuShortcut = Join-Path $env:APPDATA 'Microsoft\Windows\Start Menu\Programs\HotelDruid.lnk'
    $desktopShortcut = Join-Path $env:USERPROFILE 'Desktop\HotelDruid.lnk'
    $startupShortcut = Join-Path $env:APPDATA 'Microsoft\Windows\Start Menu\Programs\Startup\HotelDruid.lnk'

    $items = @("Install directory: $InstallDir")
    if ($RemoveDataFolder) {
        $df = if ($DataFolder) { $DataFolder } else { '(not set)' }
        $items += "Data folder: $df"
    } else {
        $items += 'Data folder: will be preserved'
    }
    $items += 'Shortcuts: Start Menu, Desktop, Startup'
    if ($RemoveSettings -or $RemoveDataFolder) {
        $items += "Settings: $settingsFile"
    } else {
        $items += 'Settings: will be preserved'
    }

    Write-Host 'HotelDruid Uninstaller' -ForegroundColor Cyan
    Write-Host ''
    Write-Host 'Planned removals:' -ForegroundColor Yellow
    $items | ForEach-Object { Write-Host " - $_" -ForegroundColor Yellow }
    Write-Host ''

    if (-not ($Force -or $Silent)) {
        $answer = Read-Host 'Proceed? (y/N)'
        if ($answer -notin @('y','Y','yes','YES')) { Write-Host 'Cancelled.' -ForegroundColor Yellow; exit 0 }
    }

    Stop-PhpDesktop

    Remove-PathSafe -Path $startMenuShortcut
    Remove-PathSafe -Path $desktopShortcut
    Remove-PathSafe -Path $startupShortcut

    Remove-PathSafe -Path $InstallDir

    if ($RemoveDataFolder -and $DataFolder) {
        Remove-PathSafe -Path $DataFolder
    }

    if ($RemoveSettings -or $RemoveDataFolder) {
        Remove-PathSafe -Path $settingsFile
        Remove-PathSafe -Path $validationLog
        try {
            if (Test-Path -LiteralPath $settingsDir) {
                $remaining = Get-ChildItem -LiteralPath $settingsDir -ErrorAction SilentlyContinue
                if (-not $remaining) { Remove-Item -LiteralPath $settingsDir -Force -Recurse -ErrorAction SilentlyContinue }
            }
        } catch {}
    }

    Write-Host ''
    Write-Host 'Uninstall complete.' -ForegroundColor Green
    if (-not $RemoveDataFolder) {
        if ($DataFolder) { Write-Host "Data preserved at: $DataFolder" -ForegroundColor Green }
        else { Write-Host 'Data folder was not removed (path unknown).' -ForegroundColor Yellow }
    }
    if (-not ($RemoveSettings -or $RemoveDataFolder)) {
        Write-Host 'Settings preserved in roaming profile.' -ForegroundColor Green
    }
    exit 0
} catch {
    Write-Host "Uninstall failed: $_" -ForegroundColor Red
    exit 1
}
