<#
.SYNOPSIS
  Deploy HotelDroid locally, per-user, or to a target directory.

.DESCRIPTION
  Uses the package created by build.ps1. Supports three common flows:
  - Default: run locally from artifacts with a localhost dev certificate.
  - Per-user install: extract to LocalAppData, register uninstall, create shortcuts.
  - Target deployment: extract the package to a specified directory.

  The per-user uninstall contract matches validate-cleanup.ps1: it writes
  install-meta.json, removes the recorded localhost cert from CurrentUser\My and
  CurrentUser\Root, removes the HKCU uninstall entry, and removes the install dir.

.EXAMPLE
  ./deploy.ps1
  ./deploy.ps1 -Publish -OpenBrowser
  ./deploy.ps1 -User
  ./deploy.ps1 -Target "C:\Deploy\HotelDroid"
#>

param(
    [switch]$Publish,
    [string]$CertThumbprint = '',
    [int]$HttpPort = 5000,
    [int]$HttpsPort = 5001,
    [switch]$OpenBrowser,
    [string]$Target = '',
    [switch]$User,
    [switch]$Force
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$root = Join-Path $PSScriptRoot '..'
$packagePath = Join-Path $root 'artifacts\hoteldroid-package.zip'
$artifactsApi = Join-Path $root 'artifacts\api'

function Ensure-Dir([string]$Path) {
    if (-not (Test-Path $Path)) {
        New-Item -ItemType Directory -Path $Path -Force | Out-Null
    }
}

function Ensure-Package {
    if ($Publish -or -not (Test-Path $packagePath)) {
        & "$PSScriptRoot\build.ps1" -Configuration Release
    }

    if (-not (Test-Path $packagePath)) {
        throw "Deployment package not found at $packagePath. Run build.ps1 first."
    }
}

function Find-CurrentUserLocalhostCert {
    $store = New-Object System.Security.Cryptography.X509Certificates.X509Store('My', 'CurrentUser')
    $store.Open([System.Security.Cryptography.X509Certificates.OpenFlags]::ReadOnly)
    try {
        $found = $store.Certificates |
            Where-Object { $_.Subject -like '*CN=localhost*' -or $_.DnsNameList.Value -contains 'localhost' } |
            Where-Object { $_.NotAfter -gt (Get-Date) } |
            Sort-Object NotAfter -Descending
        if ($found -and $found.Count -gt 0) {
            return $found[0]
        }
    }
    finally {
        $store.Close()
    }

    return $null
}

function Trust-CertificateForCurrentUser([System.Security.Cryptography.X509Certificates.X509Certificate2]$Certificate) {
    $raw = $Certificate.Export([System.Security.Cryptography.X509Certificates.X509ContentType]::Cert)
    $cer = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2 -ArgumentList (, $raw)
    $rootStore = New-Object System.Security.Cryptography.X509Certificates.X509Store('Root', 'CurrentUser')
    $rootStore.Open([System.Security.Cryptography.X509Certificates.OpenFlags]::ReadWrite)
    try {
        $existing = $rootStore.Certificates | Where-Object { $_.Thumbprint -eq $cer.Thumbprint }
        if (-not $existing) {
            $rootStore.Add($cer)
        }
    }
    finally {
        $rootStore.Close()
    }
}

function Get-OrCreate-LocalhostThumbprint {
    if ($CertThumbprint) {
        return $CertThumbprint
    }

    $existingCert = Find-CurrentUserLocalhostCert
    if ($existingCert) {
        return $existingCert.Thumbprint
    }

    Write-Host 'No localhost dev certificate found. Creating one...' -ForegroundColor Cyan
    $createdCert = New-SelfSignedCertificate \
        -DnsName 'localhost' \
        -CertStoreLocation 'Cert:\CurrentUser\My' \
        -FriendlyName 'HotelDroid Dev Localhost' \
        -NotAfter (Get-Date).AddYears(1)

    if (-not $createdCert) {
        throw 'Failed to create localhost development certificate.'
    }

    Trust-CertificateForCurrentUser -Certificate $createdCert
    Write-Host "Created and trusted localhost certificate: $($createdCert.Thumbprint)" -ForegroundColor Green
    return $createdCert.Thumbprint
}

function Expand-PackageTo([string]$Destination) {
    if ((Test-Path $Destination) -and $Force) {
        Remove-Item -Recurse -Force $Destination
    }

    Ensure-Dir $Destination
    Add-Type -AssemblyName System.IO.Compression.FileSystem
    [System.IO.Compression.ZipFile]::ExtractToDirectory((Resolve-Path $packagePath).Path, $Destination, $true)
}

function New-UserUninstallScript([string]$InstallDir, [string]$Thumbprint) {
    $uninstallPath = Join-Path $InstallDir 'uninstall-user.ps1'
    $script = @'
param()

Write-Host "Removing HotelDroid user installation..."

try {
    $thumb = "__THUMB__"

    $storeMy = New-Object System.Security.Cryptography.X509Certificates.X509Store('My', 'CurrentUser')
    $storeMy.Open([System.Security.Cryptography.X509Certificates.OpenFlags]::ReadWrite)
    try {
        $matches = $storeMy.Certificates.Find([System.Security.Cryptography.X509Certificates.X509FindType]::FindByThumbprint, $thumb, $false)
        foreach ($cert in $matches) { $storeMy.Remove($cert) }
    }
    finally {
        $storeMy.Close()
    }

    $storeRoot = New-Object System.Security.Cryptography.X509Certificates.X509Store('Root', 'CurrentUser')
    $storeRoot.Open([System.Security.Cryptography.X509Certificates.OpenFlags]::ReadWrite)
    try {
        $matchesRoot = $storeRoot.Certificates.Find([System.Security.Cryptography.X509Certificates.X509FindType]::FindByThumbprint, $thumb, $false)
        foreach ($certRoot in $matchesRoot) { $storeRoot.Remove($certRoot) }
    }
    finally {
        $storeRoot.Close()
    }
}
catch {
    Write-Warning "Failed to remove certificate: $_"
}

try {
    Remove-Item -Path 'HKCU:\Software\Microsoft\Windows\CurrentVersion\Uninstall\HotelDroid_User' -ErrorAction SilentlyContinue
}
catch {
}

try {
    $startMenuDir = Join-Path $env:APPDATA 'Microsoft\Windows\Start Menu\Programs\HotelDroid'
    if (Test-Path $startMenuDir) {
        Remove-Item -Recurse -Force $startMenuDir
    }
}
catch {
}

try {
    Remove-Item -Recurse -Force -ErrorAction SilentlyContinue '__INSTALLDIR__'
}
catch {
}

Write-Host 'User uninstall complete.'
'@

    $script = $script -replace '__THUMB__', $Thumbprint
    $script = $script -replace '__INSTALLDIR__', ($InstallDir -replace '\\', '\\\\')
    Set-Content -Path $uninstallPath -Value $script -Encoding UTF8
    return $uninstallPath
}

function New-UserShortcuts([string]$InstallDir, [string]$RunBatPath, [string]$UninstallPath, [int]$LocalHttpsPort) {
    $startMenuDir = Join-Path $env:APPDATA 'Microsoft\Windows\Start Menu\Programs\HotelDroid'
    Ensure-Dir $startMenuDir

    $wsh = New-Object -ComObject WScript.Shell

    $runShortcutPath = Join-Path $startMenuDir 'Run HotelDroid.lnk'
    $runShortcut = $wsh.CreateShortcut($runShortcutPath)
    $runShortcut.TargetPath = $RunBatPath
    $runShortcut.WorkingDirectory = $InstallDir
    $runShortcut.WindowStyle = 1
    $runShortcut.IconLocation = "$env:SystemRoot\System32\shell32.dll,220"
    $runShortcut.Save()

    $openShortcutPath = Join-Path $startMenuDir 'Open HotelDroid.lnk'
    $openShortcut = $wsh.CreateShortcut($openShortcutPath)
    $openShortcut.TargetPath = 'powershell.exe'
    $openShortcut.Arguments = "-NoProfile -ExecutionPolicy Bypass -WindowStyle Hidden -Command `"Start-Process '$RunBatPath'; Start-Sleep -Seconds 2; Start-Process 'https://localhost:$LocalHttpsPort/'`""
    $openShortcut.WorkingDirectory = $InstallDir
    $openShortcut.IconLocation = "$env:SystemRoot\System32\shell32.dll,220"
    $openShortcut.Save()

    $uninstallShortcutPath = Join-Path $startMenuDir 'Uninstall HotelDroid.lnk'
    $uninstallShortcut = $wsh.CreateShortcut($uninstallShortcutPath)
    $uninstallShortcut.TargetPath = 'powershell.exe'
    $uninstallShortcut.Arguments = "-NoProfile -ExecutionPolicy Bypass -File `"$UninstallPath`""
    $uninstallShortcut.WorkingDirectory = $InstallDir
    $uninstallShortcut.IconLocation = "$env:SystemRoot\System32\shell32.dll,131"
    $uninstallShortcut.Save()
}

function Register-UserUninstall([string]$InstallDir, [string]$UninstallPath) {
    $keyPath = 'HKCU:\Software\Microsoft\Windows\CurrentVersion\Uninstall\HotelDroid_User'
    New-Item -Path $keyPath -Force | Out-Null
    Set-ItemProperty -Path $keyPath -Name 'DisplayName' -Value 'HotelDroid (User)'
    Set-ItemProperty -Path $keyPath -Name 'DisplayVersion' -Value '1.0.0'
    Set-ItemProperty -Path $keyPath -Name 'Publisher' -Value 'HotelDroid'
    Set-ItemProperty -Path $keyPath -Name 'InstallLocation' -Value $InstallDir
    Set-ItemProperty -Path $keyPath -Name 'UninstallString' -Value "powershell.exe -NoProfile -ExecutionPolicy Bypass -File `"$UninstallPath`""
    Set-ItemProperty -Path $keyPath -Name 'DisplayIcon' -Value (Join-Path $InstallDir 'HotelDroid.Api.dll')
}

function Invoke-UserDeployment {
    param([string]$InstallDir)

    Ensure-Package
    Expand-PackageTo -Destination $InstallDir

    $thumbprint = Get-OrCreate-LocalhostThumbprint
    $metadata = @{
        thumbprint = $thumbprint
        installDir = $InstallDir
        installedAt = (Get-Date).ToString('o')
    }
    $metadata | ConvertTo-Json | Set-Content -Path (Join-Path $InstallDir 'install-meta.json') -Encoding UTF8

    $runBatPath = Join-Path $InstallDir 'run.bat'
    $runBat = @"
@echo off
set ASPNETCORE_Kestrel__Certificates__Default__Store=My
set ASPNETCORE_Kestrel__Certificates__Default__Location=CurrentUser
set ASPNETCORE_Kestrel__Certificates__Default__Thumbprint=$thumbprint
cd /d "%~dp0"
start "HotelDroid API" /b dotnet HotelDroid.Api.dll --urls "http://*:$HttpPort;https://*:$HttpsPort"
"@
    Set-Content -Path $runBatPath -Value $runBat -Encoding ASCII

    $uninstallPath = New-UserUninstallScript -InstallDir $InstallDir -Thumbprint $thumbprint
    New-UserShortcuts -InstallDir $InstallDir -RunBatPath $runBatPath -UninstallPath $uninstallPath -LocalHttpsPort $HttpsPort
    Register-UserUninstall -InstallDir $InstallDir -UninstallPath $uninstallPath

    Write-Host "User deployment completed at $InstallDir" -ForegroundColor Green
    Write-Host "Start Menu entries were created under HotelDroid." -ForegroundColor Green
}

Ensure-Package

if ($User) {
    $installDir = if ($Target) { $Target } else { Join-Path $env:LOCALAPPDATA 'HotelDroid' }
    Invoke-UserDeployment -InstallDir $installDir
    return
}

if ($Target) {
    Write-Host "Deploying package to $Target" -ForegroundColor Cyan
    Expand-PackageTo -Destination $Target
    Write-Host "Package extracted to $Target" -ForegroundColor Green
    return
}

$CertThumbprint = Get-OrCreate-LocalhostThumbprint
$env:ASPNETCORE_Kestrel__Certificates__Default__Store = 'My'
$env:ASPNETCORE_Kestrel__Certificates__Default__Location = 'CurrentUser'
$env:ASPNETCORE_Kestrel__Certificates__Default__Thumbprint = $CertThumbprint

Push-Location $artifactsApi
try {
    Start-Process 'dotnet' "HotelDroid.Api.dll --urls=http://*:$HttpPort;https://*:$HttpsPort" -NoNewWindow
    if ($OpenBrowser) {
        Start-Process "https://localhost:$HttpsPort/"
    }

    Write-Host "API running at https://localhost:$HttpsPort/" -ForegroundColor Green
}
finally {
    Pop-Location
}