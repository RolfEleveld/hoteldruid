<#
.SYNOPSIS
  Deploy HotelDruid locally, per-user, or to a target directory.

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
  ./deploy.ps1 -Target "C:\Deploy\HotelDruid"
#>

param(
    [switch]$Publish,
    [string]$CertThumbprint = '',
    [int]$HttpPort = 5000,
    [int]$HttpsPort = 5001,
    [switch]$OpenBrowser,
    [string]$Target = '',
    [switch]$User,
    [switch]$Force,
    [switch]$SkipSmokeTest
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$root = Join-Path $PSScriptRoot '..'
$packagePath = Join-Path $root 'artifacts\HotelDruid-package.zip'
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

function Test-IsAdministrator {
    $principal = New-Object Security.Principal.WindowsPrincipal([Security.Principal.WindowsIdentity]::GetCurrent())
    return $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
}

function Test-CertificateTrustedInRootStore([string]$Thumbprint, [string]$StoreLocation) {
    $store = New-Object System.Security.Cryptography.X509Certificates.X509Store('Root', $StoreLocation)
    $store.Open([System.Security.Cryptography.X509Certificates.OpenFlags]::ReadOnly)
    try {
        return @($store.Certificates | Where-Object { $_.Thumbprint -eq $Thumbprint }).Count -gt 0
    }
    finally {
        $store.Close()
    }
}

function Stop-RunningApiHost {
    $apiDll = Join-Path $artifactsApi 'HotelDruid.Api.dll'
    $apiOutPattern = [regex]::Escape($artifactsApi)
    $apiDllPattern = [regex]::Escape($apiDll)
    $apiEntryPattern = [regex]::Escape('HotelDruid.Api.dll')

    $dotnetProcesses = Get-CimInstance Win32_Process -Filter "Name='dotnet.exe'" -ErrorAction SilentlyContinue |
        Where-Object {
            $cmd = $_.CommandLine
            $cmd -and ($cmd -match $apiOutPattern -or $cmd -match $apiDllPattern -or $cmd -match $apiEntryPattern)
        }

    if (-not $dotnetProcesses) {
        return
    }

    Write-Host 'Stopping running API process(es)...' -ForegroundColor Yellow
    foreach ($process in $dotnetProcesses) {
        try {
            Stop-Process -Id $process.ProcessId -Force -ErrorAction Stop
            Wait-Process -Id $process.ProcessId -ErrorAction SilentlyContinue
            Write-Host "  Stopped dotnet process $($process.ProcessId)" -ForegroundColor DarkGray
        }
        catch {
            Write-Warning "Unable to stop dotnet process $($process.ProcessId): $($_.Exception.Message)"
        }
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

function Find-LocalMachineLocalhostCert {
    $store = New-Object System.Security.Cryptography.X509Certificates.X509Store('My', 'LocalMachine')
    $store.Open([System.Security.Cryptography.X509Certificates.OpenFlags]::ReadOnly)
    try {
        $found = $store.Certificates |
            Where-Object { $_.Subject -like '*CN=localhost*' -or $_.DnsNameList.Value -contains 'localhost' } |
            Where-Object { $_.NotAfter -gt (Get-Date) -and $_.HasPrivateKey } |
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

function Trust-CertificateForLocalMachine([System.Security.Cryptography.X509Certificates.X509Certificate2]$Certificate) {
    $raw = $Certificate.Export([System.Security.Cryptography.X509Certificates.X509ContentType]::Cert)
    $cer = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2 -ArgumentList (, $raw)
    $rootStore = New-Object System.Security.Cryptography.X509Certificates.X509Store('Root', 'LocalMachine')
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

function New-LocalMachineLocalhostCert {
    if (-not (Test-IsAdministrator)) {
        return $null
    }

    Write-Host 'No localhost certificate found in LocalMachine\\My. Creating one...' -ForegroundColor Cyan
    $createdCert = New-SelfSignedCertificate \
        -DnsName 'localhost' \
        -CertStoreLocation 'Cert:\LocalMachine\My' \
        -FriendlyName 'HotelDruid LocalMachine Localhost' \
        -NotAfter (Get-Date).AddYears(2)

    if (-not $createdCert) {
        throw 'Failed to create localhost certificate in LocalMachine\\My.'
    }

    Trust-CertificateForLocalMachine -Certificate $createdCert
    Write-Host "Created and trusted LocalMachine localhost certificate: $($createdCert.Thumbprint)" -ForegroundColor Green
    return $createdCert
}

function Get-DotnetDevCertCandidates {
    $dotnet = Get-Command dotnet -ErrorAction SilentlyContinue
    if (-not $dotnet) {
        return @()
    }

    try {
        $json = & $dotnet.Source dev-certs https --check-trust-machine-readable 2>$null
        if (-not $json) {
            return @()
        }

        $parsed = $json | ConvertFrom-Json
        if ($parsed -is [System.Array]) {
            return $parsed
        }

        if ($parsed) {
            return @($parsed)
        }
    }
    catch {
        Write-Verbose "Unable to inspect .NET HTTPS development certificates: $_"
    }

    return @()
}

function Get-OrTrust-DotnetDevCertThumbprint {
    $candidates = Get-DotnetDevCertCandidates
    $trusted = $candidates | Where-Object { $_.TrustLevel -eq 'Full' } | Select-Object -First 1
    if ($trusted) {
        return $trusted.Thumbprint
    }

    $dotnet = Get-Command dotnet -ErrorAction SilentlyContinue
    if (-not $dotnet) {
        return $null
    }

    Write-Host '.NET HTTPS development certificate is not trusted. Trusting it for the current user...' -ForegroundColor Cyan
    try {
        & $dotnet.Source dev-certs https --trust | Out-Host
    }
    catch {
        Write-Warning "Failed to trust the .NET HTTPS development certificate: $_"
        return $null
    }

    $trusted = Get-DotnetDevCertCandidates | Where-Object { $_.TrustLevel -eq 'Full' } | Select-Object -First 1
    if ($trusted) {
        Write-Host "Using trusted .NET HTTPS development certificate: $($trusted.Thumbprint)" -ForegroundColor Green
        return $trusted.Thumbprint
    }

    return $null
}

function Get-OrCreate-LocalhostThumbprint {
    if ($CertThumbprint) {
        $providedMachine = Get-ChildItem Cert:\LocalMachine\My -ErrorAction SilentlyContinue | Where-Object { $_.Thumbprint -eq $CertThumbprint } | Select-Object -First 1
        if ($providedMachine -and $providedMachine.HasPrivateKey) {
            Write-Host "Using certificate $CertThumbprint from LocalMachine\\My." -ForegroundColor Green
            return $CertThumbprint
        }

        $providedUser = Get-ChildItem Cert:\CurrentUser\My -ErrorAction SilentlyContinue | Where-Object { $_.Thumbprint -eq $CertThumbprint } | Select-Object -First 1
        if ($providedUser) {
            Trust-CertificateForCurrentUser -Certificate $providedUser
            Write-Host "Using certificate $CertThumbprint from CurrentUser\\My." -ForegroundColor Green
            return $CertThumbprint
        }

        throw "Certificate thumbprint $CertThumbprint was not found in LocalMachine\\My or CurrentUser\\My."
    }

    if (Test-IsAdministrator) {
        $existingMachineCert = Find-LocalMachineLocalhostCert
        if ($existingMachineCert) {
            Trust-CertificateForLocalMachine -Certificate $existingMachineCert
            Write-Host "Using localhost certificate from LocalMachine\\My: $($existingMachineCert.Thumbprint)" -ForegroundColor Green
            return $existingMachineCert.Thumbprint
        }

        $newMachineCert = New-LocalMachineLocalhostCert
        if ($newMachineCert) {
            return $newMachineCert.Thumbprint
        }
    }

    $dotnetDevCertThumbprint = Get-OrTrust-DotnetDevCertThumbprint
    if ($dotnetDevCertThumbprint) {
        return $dotnetDevCertThumbprint
    }

    $existingCert = Find-CurrentUserLocalhostCert
    if ($existingCert) {
        Trust-CertificateForCurrentUser -Certificate $existingCert
        return $existingCert.Thumbprint
    }

    Write-Host 'No localhost dev certificate found. Creating one...' -ForegroundColor Cyan
    $createdCert = New-SelfSignedCertificate \
        -DnsName 'localhost' \
        -CertStoreLocation 'Cert:\CurrentUser\My' \
        -FriendlyName 'HotelDruid Dev Localhost' \
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

Write-Host "Removing HotelDruid user installation..."

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
    Remove-Item -Path 'HKCU:\Software\Microsoft\Windows\CurrentVersion\Uninstall\HotelDruid_User' -ErrorAction SilentlyContinue
}
catch {
}

try {
    $startMenuDir = Join-Path $env:APPDATA 'Microsoft\Windows\Start Menu\Programs\HotelDruid'
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
    $startMenuDir = Join-Path $env:APPDATA 'Microsoft\Windows\Start Menu\Programs\HotelDruid'
    Ensure-Dir $startMenuDir

    $wsh = New-Object -ComObject WScript.Shell

    $runShortcutPath = Join-Path $startMenuDir 'Run HotelDruid.lnk'
    $runShortcut = $wsh.CreateShortcut($runShortcutPath)
    $runShortcut.TargetPath = $RunBatPath
    $runShortcut.WorkingDirectory = $InstallDir
    $runShortcut.WindowStyle = 1
    $runShortcut.IconLocation = "$env:SystemRoot\System32\shell32.dll,220"
    $runShortcut.Save()

    $openShortcutPath = Join-Path $startMenuDir 'Open HotelDruid.lnk'
    $openShortcut = $wsh.CreateShortcut($openShortcutPath)
    $openShortcut.TargetPath = 'powershell.exe'
    $openShortcut.Arguments = "-NoProfile -ExecutionPolicy Bypass -WindowStyle Hidden -Command `"Start-Process '$RunBatPath'; Start-Sleep -Seconds 2; Start-Process 'https://localhost:$LocalHttpsPort/'`""
    $openShortcut.WorkingDirectory = $InstallDir
    $openShortcut.IconLocation = "$env:SystemRoot\System32\shell32.dll,220"
    $openShortcut.Save()

    $uninstallShortcutPath = Join-Path $startMenuDir 'Uninstall HotelDruid.lnk'
    $uninstallShortcut = $wsh.CreateShortcut($uninstallShortcutPath)
    $uninstallShortcut.TargetPath = 'powershell.exe'
    $uninstallShortcut.Arguments = "-NoProfile -ExecutionPolicy Bypass -File `"$UninstallPath`""
    $uninstallShortcut.WorkingDirectory = $InstallDir
    $uninstallShortcut.IconLocation = "$env:SystemRoot\System32\shell32.dll,131"
    $uninstallShortcut.Save()
}

function Register-UserUninstall([string]$InstallDir, [string]$UninstallPath) {
    $keyPath = 'HKCU:\Software\Microsoft\Windows\CurrentVersion\Uninstall\HotelDruid_User'
    New-Item -Path $keyPath -Force | Out-Null
    Set-ItemProperty -Path $keyPath -Name 'DisplayName' -Value 'HotelDruid (User)'
    Set-ItemProperty -Path $keyPath -Name 'DisplayVersion' -Value '1.0.0'
    Set-ItemProperty -Path $keyPath -Name 'Publisher' -Value 'HotelDruid'
    Set-ItemProperty -Path $keyPath -Name 'InstallLocation' -Value $InstallDir
    Set-ItemProperty -Path $keyPath -Name 'UninstallString' -Value "powershell.exe -NoProfile -ExecutionPolicy Bypass -File `"$UninstallPath`""
    Set-ItemProperty -Path $keyPath -Name 'DisplayIcon' -Value (Join-Path $InstallDir 'HotelDruid.Api.dll')
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
start "HotelDruid API" /b dotnet HotelDruid.Api.dll --urls "http://*:$HttpPort;https://*:$HttpsPort"
"@
    Set-Content -Path $runBatPath -Value $runBat -Encoding ASCII

    $uninstallPath = New-UserUninstallScript -InstallDir $InstallDir -Thumbprint $thumbprint
    New-UserShortcuts -InstallDir $InstallDir -RunBatPath $runBatPath -UninstallPath $uninstallPath -LocalHttpsPort $HttpsPort
    Register-UserUninstall -InstallDir $InstallDir -UninstallPath $uninstallPath

    Write-Host "User deployment completed at $InstallDir" -ForegroundColor Green
    Write-Host "Start Menu entries were created under HotelDruid." -ForegroundColor Green
}

Ensure-Package

if ($User) {
    $installDir = if ($Target) { $Target } else { Join-Path $env:LOCALAPPDATA 'HotelDruid' }
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

$isAdmin = Test-IsAdministrator
$trustedInLocalMachineRoot = $false
if ($isAdmin) {
    $trustedInLocalMachineRoot = Test-CertificateTrustedInRootStore -Thumbprint $CertThumbprint -StoreLocation 'LocalMachine'
}

if (-not $trustedInLocalMachineRoot) {
    Write-Warning "Certificate $CertThumbprint is trusted for the current user, but not in LocalMachine Root. If Chromium still shows 'Not secure', run .\\scripts\\trust-dev-cert.ps1 once from an elevated PowerShell window."
}

Push-Location $artifactsApi
try {
    Stop-RunningApiHost
    Start-Process 'dotnet' "HotelDruid.Api.dll --urls=http://*:$HttpPort;https://*:$HttpsPort" -NoNewWindow

    if (-not $SkipSmokeTest) {
        & "$PSScriptRoot\scenario-test.ps1" -BaseUrl "https://localhost:$HttpsPort"
        if ($LASTEXITCODE -ne 0) {
            throw "Post-deploy smoke test failed (exit $LASTEXITCODE)."
        }
    }

    if ($OpenBrowser) {
        Start-Process "https://localhost:$HttpsPort/"
    }

    Write-Host "API running at https://localhost:$HttpsPort/" -ForegroundColor Green
}
finally {
    Pop-Location
}
