<#
deploy-user.ps1

Per-user deployment script:
 - Extracts the package (creates package if missing)
 - Installs files into %LocalAppData%\HotelDroid (or provided path)
 - Creates a CurrentUser self-signed certificate for localhost and records its thumbprint
 - Creates a `run.bat` launcher and Start Menu shortcuts for Run and Uninstall
 - Registers an uninstall entry under HKCU so Apps & features shows the app for the current user

Usage:
  .\scripts\deploy-user.ps1
  .\scripts\deploy-user.ps1 -InstallDir "$env:LOCALAPPDATA\HotelDroid" -PackagePath "artifacts\hoteldroid-package.zip" -Force
#>

param(
    [string]$InstallDir = (Join-Path $env:LOCALAPPDATA 'HotelDroid'),
    [string]$PackagePath = 'artifacts\hoteldroid-package.zip',
    [switch]$Force
)

Set-StrictMode -Version Latest

function Ensure-Dir([string]$d) { if (-not (Test-Path $d)) { New-Item -ItemType Directory -Path $d | Out-Null } }

Write-Host "User deploy -> InstallDir: $InstallDir  Package: $PackagePath"

if (-not (Test-Path $PackagePath)) {
    Write-Host "Package not found; creating package first..."
    & "./scripts/pack-and-deploy.ps1" -PackageOnly -Force
    if (-not (Test-Path $PackagePath)) { throw "Failed to create package" }
}

if ((Test-Path $InstallDir) -and $Force) { Remove-Item -Recurse -Force $InstallDir }
Ensure-Dir $InstallDir

Write-Host "Extracting package to $InstallDir ..."
Add-Type -AssemblyName System.IO.Compression.FileSystem
[System.IO.Compression.ZipFile]::ExtractToDirectory((Resolve-Path $PackagePath).Path, $InstallDir)

# create or find a CurrentUser cert for 'localhost'
Write-Host "Looking for existing CurrentUser localhost certificate..."
function Find-LocalhostCert {
    $store = New-Object System.Security.Cryptography.X509Certificates.X509Store("My","CurrentUser")
    $store.Open([System.Security.Cryptography.X509Certificates.OpenFlags]::ReadOnly)
    try {
        $found = $store.Certificates | Where-Object { $_.Subject -like '*CN=localhost*' -or $_.DnsNameList -contains 'localhost' } | Sort-Object NotAfter -Descending
        if ($found -and $found.Count -gt 0) { return $found[0] }
    } finally { $store.Close() }
    return $null
}

$cert = Find-LocalhostCert
if ($null -eq $cert) {
    Write-Host "No CurrentUser localhost cert found. Creating self-signed certificate..."
    $cert = New-SelfSignedCertificate -DnsName 'localhost' -CertStoreLocation 'Cert:\CurrentUser\My' -FriendlyName 'HotelDroid Localhost' -NotAfter (Get-Date).AddYears(5)
    if ($null -eq $cert) { throw 'Failed to create certificate' }
}

$thumb = $cert.Thumbprint
Write-Host "Using certificate thumbprint: $thumb"

# Ensure the cert is trusted for the current user by importing into CurrentUser\Root
try {
    $raw = $cert.Export([System.Security.Cryptography.X509Certificates.X509ContentType]::Cert)
    $cer = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2 -ArgumentList (,$raw)
    $root = New-Object System.Security.Cryptography.X509Certificates.X509Store('Root','CurrentUser')
    $root.Open([System.Security.Cryptography.X509Certificates.OpenFlags]::ReadWrite)
    $exists = $root.Certificates | Where-Object { $_.Thumbprint -eq $cer.Thumbprint }
    if (-not $exists) { $root.Add($cer) }
    $root.Close()
    Write-Host "Imported certificate into CurrentUser\Root (trusted for current user)."
} catch {
    Write-Warning "Failed to import cert into CurrentUser\Root: $_"
}

# write install metadata so validators/uninstallers can reference the created cert
$meta = @{ thumbprint = $thumb; installDir = $InstallDir; installedAt = (Get-Date).ToString('o') }
$meta | ConvertTo-Json | Out-File -FilePath (Join-Path $InstallDir 'install-meta.json') -Encoding UTF8 -Force

# create run launcher (run.bat)
$runBat = Join-Path $InstallDir 'run.bat'
$runContent = @"
@echo off
set ASPNETCORE_Kestrel__Certificates__Default__Thumbprint=$thumb
cd /d "%~dp0\api"
if exist HotelDroid.Api.exe (
    start "HotelDroid API" /b .\HotelDroid.Api.exe
) else (
    start "HotelDroid API" /b dotnet HotelDroid.Api.dll --urls "https://localhost:5001;http://localhost:5000"
)
"@
$runContent | Out-File -FilePath $runBat -Encoding ASCII -Force

# create uninstall script (per-user)
$uninstall = Join-Path $InstallDir 'uninstall-user.ps1'
# Use a literal here-string to avoid expanding variables at creation time; replace placeholders after
$uninstallContent = @'
param()
Write-Host 'Removing HotelDroid user installation...'
try {
    # remove cert from My and Root
    $thumb = '__THUMB__'
    $storeMy = New-Object System.Security.Cryptography.X509Certificates.X509Store('My','CurrentUser')
    $storeMy.Open([System.Security.Cryptography.X509Certificates.OpenFlags]::ReadWrite)
    $c = $storeMy.Certificates.Find([System.Security.Cryptography.X509Certificates.X509FindType]::FindByThumbprint, $thumb, $false)
    foreach ($cert in $c) { $storeMy.Remove($cert) }
    $storeMy.Close()
    try {
        $storeRoot = New-Object System.Security.Cryptography.X509Certificates.X509Store('Root','CurrentUser')
        $storeRoot.Open([System.Security.Cryptography.X509Certificates.OpenFlags]::ReadWrite)
        $c2 = $storeRoot.Certificates.Find([System.Security.Cryptography.X509Certificates.X509FindType]::FindByThumbprint, $thumb, $false)
        foreach ($cert2 in $c2) { $storeRoot.Remove($cert2) }
        $storeRoot.Close()
    } catch { }
} catch { Write-Warning "Failed to remove certificate: $_" }

try { Remove-Item -Recurse -Force -ErrorAction SilentlyContinue '__INSTALLDIR__' } catch {}

try { Remove-Item -Path "HKCU:\Software\Microsoft\Windows\CurrentVersion\Uninstall\HotelDroid_User" -ErrorAction SilentlyContinue } catch {}

Write-Host 'User uninstall complete.'
'@

# replace placeholders with actual values
$uninstallContent = $uninstallContent -replace '__THUMB__', $thumb
$uninstallContent = $uninstallContent -replace '__INSTALLDIR__', ($InstallDir -replace '\\','\\\\')
$uninstallContent | Out-File -FilePath $uninstall -Encoding UTF8 -Force

# create Start Menu shortcuts in user's profile
$startMenuDir = Join-Path $env:APPDATA 'Microsoft\Windows\Start Menu\Programs\HotelDroid'
Ensure-Dir $startMenuDir

# create Run shortcut (.lnk)
$wsh = New-Object -ComObject WScript.Shell
$lnkPath = Join-Path $startMenuDir 'Run HotelDroid.lnk'
$shortcut = $wsh.CreateShortcut($lnkPath)
$shortcut.TargetPath = $runBat
$shortcut.WorkingDirectory = $InstallDir
$shortcut.WindowStyle = 1
$shortcut.IconLocation = "%SystemRoot%\\system32\\shell32.dll, 1"
$shortcut.Save()

# create Uninstall shortcut
$unlnk = Join-Path $startMenuDir 'Uninstall HotelDroid.lnk'
$shortcut2 = $wsh.CreateShortcut($unlnk)
$shortcut2.TargetPath = 'powershell.exe'
$shortcut2.Arguments = "-NoProfile -ExecutionPolicy Bypass -File `"$uninstall`""
$shortcut2.WorkingDirectory = $InstallDir
$shortcut2.Save()

# register HKCU uninstall entry
try {
    $keyPath = 'HKCU:\Software\Microsoft\Windows\CurrentVersion\Uninstall\HotelDroid_User'
    New-Item -Path $keyPath -Force | Out-Null
    Set-ItemProperty -Path $keyPath -Name 'DisplayName' -Value 'HotelDroid (User)'
    Set-ItemProperty -Path $keyPath -Name 'DisplayVersion' -Value '0.1.0'
    Set-ItemProperty -Path $keyPath -Name 'Publisher' -Value 'HotelDroid Migration'
    Set-ItemProperty -Path $keyPath -Name 'InstallLocation' -Value $InstallDir
    Set-ItemProperty -Path $keyPath -Name 'UninstallString' -Value "powershell.exe -NoProfile -ExecutionPolicy Bypass -File `"$uninstall`""
    Set-ItemProperty -Path $keyPath -Name 'DisplayIcon' -Value (Join-Path $InstallDir 'api\HotelDroid.Api.dll')
    Write-Host 'Registered per-user uninstall entry (HKCU) visible in Apps & features for current user.'
} catch {
    Write-Warning 'Failed to register HKCU uninstall entry.'
}

Write-Host "User deployment done. Run from Start Menu -> 'Run HotelDroid' or execute: $runBat"
