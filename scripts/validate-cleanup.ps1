<#
validate-cleanup.ps1
Runs per-user uninstall script and reports whether install dir, HKCU uninstall key, and localhost cert were removed.
#>


$inst = Join-Path $env:LOCALAPPDATA 'HotelDroid'
Write-Host "InstallDir: $inst"

# capture before state of localhost cert thumbprints
$store = New-Object System.Security.Cryptography.X509Certificates.X509Store('My','CurrentUser')
$store.Open([System.Security.Cryptography.X509Certificates.OpenFlags]::ReadOnly)
$before = $store.Certificates | Where-Object { $_.Subject -like '*CN=localhost*' } | Select-Object -ExpandProperty Thumbprint
$store.Close()
Write-Host "Found $($before.Count) localhost cert(s) before uninstall."

# read recorded thumbprint from install-meta.json if present
$metaFile = Join-Path $inst 'install-meta.json'
$recordedThumb = $null
if (Test-Path $metaFile) {
    try { $recorded = Get-Content $metaFile | ConvertFrom-Json; $recordedThumb = $recorded.thumbprint } catch {}
}
if ($recordedThumb) { Write-Host "Recorded created cert thumbprint: $recordedThumb" } else { Write-Host 'No recorded thumbprint found.' }

if (Test-Path (Join-Path $inst 'uninstall-user.ps1')) {
    Write-Host 'Found uninstall script - executing it now...'
    & (Join-Path $inst 'uninstall-user.ps1')
} else {
    Write-Host 'uninstall-user.ps1 not found'
}

if (Test-Path $inst) { Write-Host 'INSTALL_DIR_EXISTS' } else { Write-Host 'INSTALL_DIR_REMOVED' }

if (Test-Path 'HKCU:\Software\Microsoft\Windows\CurrentVersion\Uninstall\HotelDroid_User') { Write-Host 'HKCU_UNINSTALL_PRESENT' } else { Write-Host 'HKCU_UNINSTALL_REMOVED' }

# capture after state and analyze
$store = New-Object System.Security.Cryptography.X509Certificates.X509Store('My','CurrentUser')
$store.Open([System.Security.Cryptography.X509Certificates.OpenFlags]::ReadOnly)
$after = $store.Certificates | Where-Object { $_.Subject -like '*CN=localhost*' } | Select-Object -ExpandProperty Thumbprint
$store.Close()
Write-Host "Found $($after.Count) localhost cert(s) after uninstall."

if ($recordedThumb) {
    if ($before -contains $recordedThumb -and -not ($after -contains $recordedThumb)) {
        Write-Host "Recorded certificate $recordedThumb was removed successfully."
    } else {
        Write-Warning "Recorded certificate $recordedThumb still present or was not found before uninstall."
    }
} else {
    Write-Host 'No recorded cert to validate removal.'
}

Write-Host 'validate-cleanup.ps1 completed.'
