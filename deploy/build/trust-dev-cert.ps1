<#
.SYNOPSIS
  Trust the current localhost development certificate in LocalMachine Root.

.DESCRIPTION
  Some Chromium sessions continue to show "Not secure" unless the selected
    localhost certificate is trusted machine-wide. This script prefers a localhost
    certificate in LocalMachine\My, then falls back to a trusted .NET development
    certificate or the latest localhost cert in CurrentUser\My, and copies its public cert to
  LocalMachine\Root.
#>

param(
    [string]$Thumbprint = ''
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Test-IsAdministrator {
    $principal = New-Object Security.Principal.WindowsPrincipal([Security.Principal.WindowsIdentity]::GetCurrent())
    return $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
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
        return @()
    }

    return @()
}

function Find-CurrentUserLocalhostCert {
    $store = New-Object System.Security.Cryptography.X509Certificates.X509Store('My', 'CurrentUser')
    $store.Open([System.Security.Cryptography.X509Certificates.OpenFlags]::ReadOnly)
    try {
        return $store.Certificates |
            Where-Object { $_.Subject -like '*CN=localhost*' -or $_.DnsNameList.Value -contains 'localhost' } |
            Where-Object { $_.NotAfter -gt (Get-Date) } |
            Sort-Object NotAfter -Descending |
            Select-Object -First 1
    }
    finally {
        $store.Close()
    }
}

function Find-LocalMachineLocalhostCert {
    $store = New-Object System.Security.Cryptography.X509Certificates.X509Store('My', 'LocalMachine')
    $store.Open([System.Security.Cryptography.X509Certificates.OpenFlags]::ReadOnly)
    try {
        return $store.Certificates |
            Where-Object { $_.Subject -like '*CN=localhost*' -or $_.DnsNameList.Value -contains 'localhost' } |
            Where-Object { $_.NotAfter -gt (Get-Date) -and $_.HasPrivateKey } |
            Sort-Object NotAfter -Descending |
            Select-Object -First 1
    }
    finally {
        $store.Close()
    }
}

function New-LocalMachineLocalhostCert {
    Write-Host 'No localhost certificate found in LocalMachine\\My. Creating one...' -ForegroundColor Cyan
    $createdCert = New-SelfSignedCertificate \
        -DnsName 'localhost' \
        -CertStoreLocation 'Cert:\LocalMachine\My' \
        -FriendlyName 'HotelDruid LocalMachine Localhost' \
        -NotAfter (Get-Date).AddYears(2)

    if (-not $createdCert) {
        throw 'Failed to create localhost certificate in LocalMachine\\My.'
    }

    return $createdCert
}

if (-not (Test-IsAdministrator)) {
    throw 'Run this script from an elevated PowerShell window.'
}

$selectedThumbprint = $Thumbprint
$certificate = $null

if ($selectedThumbprint) {
    $certificate = Get-ChildItem Cert:\LocalMachine\My -ErrorAction SilentlyContinue |
        Where-Object { $_.Thumbprint -eq $selectedThumbprint } |
        Select-Object -First 1

    if (-not $certificate) {
        $certificate = Get-ChildItem Cert:\CurrentUser\My -ErrorAction SilentlyContinue |
            Where-Object { $_.Thumbprint -eq $selectedThumbprint } |
            Select-Object -First 1
    }
}

if (-not $certificate) {
    $certificate = Find-LocalMachineLocalhostCert
}

if (-not $certificate) {
    $certificate = New-LocalMachineLocalhostCert
}

if (-not $certificate) {
    $trustedDevCert = Get-DotnetDevCertCandidates | Where-Object { $_.TrustLevel -eq 'Full' } | Select-Object -First 1
    if ($trustedDevCert) {
        $certificate = Get-ChildItem Cert:\CurrentUser\My -ErrorAction SilentlyContinue |
            Where-Object { $_.Thumbprint -eq $trustedDevCert.Thumbprint } |
            Select-Object -First 1
    }
}

$storeMy = New-Object System.Security.Cryptography.X509Certificates.X509Store('My', 'CurrentUser')
$storeMy.Open([System.Security.Cryptography.X509Certificates.OpenFlags]::ReadOnly)
try {
    if (-not $certificate) {
        $certificate = Find-CurrentUserLocalhostCert
    }

    if (-not $certificate) {
        throw 'No localhost certificate was found in LocalMachine\\My or CurrentUser\\My.'
    }

    $raw = $certificate.Export([System.Security.Cryptography.X509Certificates.X509ContentType]::Cert)
    $cer = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2 -ArgumentList (, $raw)

    $rootStore = New-Object System.Security.Cryptography.X509Certificates.X509Store('Root', 'LocalMachine')
    $rootStore.Open([System.Security.Cryptography.X509Certificates.OpenFlags]::ReadWrite)
    try {
        $existing = $rootStore.Certificates | Where-Object { $_.Thumbprint -eq $cer.Thumbprint }
        if (-not $existing) {
            $rootStore.Add($cer)
            Write-Host "Trusted certificate $($cer.Thumbprint) in LocalMachine\\Root." -ForegroundColor Green
        }
        else {
            Write-Host "Certificate $($cer.Thumbprint) is already trusted in LocalMachine\\Root." -ForegroundColor Green
        }
    }
    finally {
        $rootStore.Close()
    }
}
finally {
    $storeMy.Close()
}