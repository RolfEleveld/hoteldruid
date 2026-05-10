<#
.SYNOPSIS
  Create a development TLS certificate for `localhost` and optionally trust it for the current user or machine.

.DESCRIPTION
  This script creates a self-signed certificate for `localhost` (FriendlyName: "HotelDroid Dev Localhost")
  and exports a PFX and CER. It can import the CER into the CurrentUser\Root store (no elevation)
  or LocalMachine\Root (requires elevation) so browsers trust it.

  Usage examples:
    # Create under CurrentUser and trust for current user (no elevation)
    .\create-dev-cert.ps1 -Store CurrentUser -Trust

    # Create under LocalMachine and trust system-wide (requires elevated PowerShell)
    .\create-dev-cert.ps1 -Store LocalMachine -Trust -Force

.NOTES
  - Intended for development only. Do NOT use in production.
  - Requires PowerShell on Windows.
#>

param(
    [ValidateSet('CurrentUser','LocalMachine')]
    [string]$Store = 'CurrentUser',
    [string]$DnsName = 'localhost',
    [string]$FriendlyName = 'HotelDroid Dev Localhost',
    [int]$ValidYears = 1,
    [switch]$Trust,
    [switch]$Force
)

function Test-Admin {
    return ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
}

Write-Host "Creating dev cert for $DnsName in store: $Store"

# Check if cert already exists by friendly name
$existing = Get-ChildItem Cert:\$Store\My -ErrorAction SilentlyContinue | Where-Object FriendlyName -eq $FriendlyName
if ($existing -and -not $Force) {
    Write-Host "Certificate already exists. Thumbprint: $($existing[0].Thumbprint)"
    return $existing[0].Thumbprint
}

# Create self-signed certificate
$notAfter = (Get-Date).AddYears($ValidYears)
$cert = New-SelfSignedCertificate -DnsName $DnsName -CertStoreLocation "cert:\$Store\My" -FriendlyName $FriendlyName -NotAfter $notAfter

if (-not $cert) {
    Write-Error "Failed to create certificate."
    exit 1
}

Write-Host "Created cert thumbprint: $($cert.Thumbprint)"

# Export PFX and CER into scripts folder
$outDir = Join-Path $PSScriptRoot 'certs'
New-Item -ItemType Directory -Path $outDir -Force | Out-Null
$pfxPath = Join-Path $outDir "hoteldroid_$($cert.Thumbprint).pfx"
$cerPath = Join-Path $outDir "hoteldroid_$($cert.Thumbprint).cer"

$pfxPassword = ConvertTo-SecureString -String 'hoteldroid-dev' -Force -AsPlainText
Export-PfxCertificate -Cert $cert -FilePath $pfxPath -Password $pfxPassword -Force | Out-Null
Export-Certificate -Cert $cert -FilePath $cerPath -Force | Out-Null

Write-Host "Exported PFX: $pfxPath"
Write-Host "Exported CER: $cerPath"

if ($Trust) {
    if ($Store -eq 'LocalMachine' -and -not (Test-Admin)) {
        Write-Warning "Importing into LocalMachine\Root requires elevation. Re-run as Administrator or omit -Trust or use -Store CurrentUser."
    }
    else {
        $targetRoot = if ($Store -eq 'LocalMachine') { 'Cert:\LocalMachine\Root' } else { 'Cert:\CurrentUser\Root' }
        try {
            Import-Certificate -FilePath $cerPath -CertStoreLocation $targetRoot | Out-Null
            Write-Host "Imported certificate into $targetRoot (trusted)."
        } catch {
            Write-Warning "Failed to import into $targetRoot : $($_.Exception.Message)"
        }
    }
}

Write-Host "Done. Thumbprint: $($cert.Thumbprint)"
return $cert.Thumbprint
