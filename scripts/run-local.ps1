<#
.SYNOPSIS
  Stage 2 — Launch HotelDroid locally with trusted HTTPS.

.DESCRIPTION
  Finds or creates a CurrentUser self-signed certificate for localhost, trusts it
  in CurrentUser\Root (no elevation required), sets the Kestrel certificate env-vars,
  then starts the published API which serves the embedded Blazor WASM client.

  Run scripts\build.ps1 at least once before this script to produce the artifacts.

  URL   HTTP  : http://localhost:5000
        HTTPS : https://localhost:5001  (trusted, no browser warning)

.EXAMPLE
  # Start after a build
  .\scripts\run-local.ps1

  # Start from source (dotnet run, skips publish step — fastest for iteration)
  .\scripts\run-local.ps1 -FromSource

  # Provide a specific certificate thumbprint (skip auto-detect/create)
  .\scripts\run-local.ps1 -CertThumbprint "ABCDEF1234..."

  # Open the browser automatically
  .\scripts\run-local.ps1 -OpenBrowser
#>

param(
    [string]$CertThumbprint = '',
    [string]$ApiProject     = 'src/HotelDroid.Api',
    [int]$HttpPort           = 5000,
    [int]$HttpsPort          = 5001,
    [switch]$FromSource,
    [switch]$OpenBrowser
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$root       = Join-Path $PSScriptRoot '..'
$artifactsApi = Join-Path $root 'artifacts\api'

# ── 1. Certificate — find or create ──────────────────────────────────────────
function Find-CurrentUserLocalhostCert {
    $store = New-Object System.Security.Cryptography.X509Certificates.X509Store("My","CurrentUser")
    $store.Open([System.Security.Cryptography.X509Certificates.OpenFlags]::ReadOnly)
    try {
        $found = $store.Certificates |
            Where-Object { $_.Subject -like '*CN=localhost*' -or $_.DnsNameList.Value -contains 'localhost' } |
            Where-Object { $_.NotAfter -gt (Get-Date) } |
            Sort-Object NotAfter -Descending
        if ($found -and $found.Count -gt 0) { return $found[0] }
    } finally { $store.Close() }
    return $null
}

function Trust-Cert([System.Security.Cryptography.X509Certificates.X509Certificate2]$cert) {
    $raw = $cert.Export([System.Security.Cryptography.X509Certificates.X509ContentType]::Cert)
    $cerObj = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2 -ArgumentList (,$raw)
    $root = New-Object System.Security.Cryptography.X509Certificates.X509Store('Root','CurrentUser')
    $root.Open([System.Security.Cryptography.X509Certificates.OpenFlags]::ReadWrite)
    try {
        $already = $root.Certificates | Where-Object { $_.Thumbprint -eq $cerObj.Thumbprint }
        if (-not $already) {
            $root.Add($cerObj)
            Write-Host "Certificate imported into CurrentUser\Root (trusted, no elevation needed)." -ForegroundColor Green
        } else {
            Write-Host "Certificate already trusted in CurrentUser\Root." -ForegroundColor Green
        }
    } finally { $root.Close() }
}

if ($CertThumbprint) {
    Write-Host "Using provided thumbprint: $CertThumbprint"
    $cert = Get-ChildItem Cert:\CurrentUser\My -ErrorAction SilentlyContinue |
            Where-Object { $_.Thumbprint -eq $CertThumbprint } |
            Select-Object -First 1
    if (-not $cert) {
        $cert = Get-ChildItem Cert:\LocalMachine\My -ErrorAction SilentlyContinue |
                Where-Object { $_.Thumbprint -eq $CertThumbprint } |
                Select-Object -First 1
    }
    if (-not $cert) { throw "Certificate $CertThumbprint not found in CurrentUser\My or LocalMachine\My." }
} else {
    Write-Host "Locating existing CurrentUser localhost certificate..." -ForegroundColor Cyan

    # First try the dedicated create-dev-cert.ps1 (idempotent, exports PFX)
    $createDevCert = Join-Path $PSScriptRoot 'create-dev-cert.ps1'
    if (Test-Path $createDevCert) {
        $thumb = & $createDevCert -Store CurrentUser -Trust 2>&1
        $thumb = ($thumb | Where-Object { $_ -match '^[0-9A-Fa-f]{40}$' } | Select-Object -Last 1)
        if ($thumb) {
            $CertThumbprint = $thumb.Trim()
            $cert = Get-ChildItem Cert:\CurrentUser\My | Where-Object { $_.Thumbprint -eq $CertThumbprint } | Select-Object -First 1
            Write-Host "Dev cert ready. Thumbprint: $CertThumbprint" -ForegroundColor Green
        }
    }

    # Fall back: find any valid CurrentUser localhost cert
    if (-not $cert) {
        $cert = Find-CurrentUserLocalhostCert
        if ($cert) {
            $CertThumbprint = $cert.Thumbprint
            Write-Host "Found existing cert. Thumbprint: $CertThumbprint" -ForegroundColor Green
        }
    }

    # Last resort: create one inline
    if (-not $cert) {
        Write-Host "No localhost certificate found. Creating self-signed certificate..." -ForegroundColor Yellow
        $cert = New-SelfSignedCertificate `
            -DnsName 'localhost' `
            -CertStoreLocation 'Cert:\CurrentUser\My' `
            -FriendlyName 'HotelDroid Localhost' `
            -NotAfter (Get-Date).AddYears(1)
        if (-not $cert) { throw "Failed to create self-signed certificate." }
        $CertThumbprint = $cert.Thumbprint
        Write-Host "Created cert. Thumbprint: $CertThumbprint" -ForegroundColor Green
    }
}

# Ensure the cert is trusted for the current user
Trust-Cert $cert

# ── 2. Set Kestrel environment variables ──────────────────────────────────────
$env:ASPNETCORE_Kestrel__Certificates__Default__Store       = "My"
$env:ASPNETCORE_Kestrel__Certificates__Default__Thumbprint  = $CertThumbprint
Write-Host "Kestrel cert env-vars set (thumbprint: $CertThumbprint)." -ForegroundColor Cyan

$urls = "http://localhost:$HttpPort;https://localhost:$HttpsPort"

# ── 3. Open browser (always, after a short delay so the server is ready) ─────
$appUrl = "https://localhost:$HttpsPort"
Start-Job -ScriptBlock {
    param($u) Start-Sleep -Seconds 4; Start-Process $u
} -ArgumentList $appUrl | Out-Null

# ── 4. Start the app ──────────────────────────────────────────────────────────
Write-Host "`n>> Starting HotelDroid..." -ForegroundColor Green
Write-Host "   HTTP  -> http://localhost:$HttpPort"
Write-Host "   HTTPS -> https://localhost:$HttpsPort (trusted)  <-- browser will open here"
Write-Host "   Press Ctrl+C to stop.`n"

Push-Location $root
try {
    if ($FromSource) {
        # The API project references the Client with ReferenceOutputAssembly=false.
        # dotnet run will include the client's static web assets automatically.
        # No manual client publish/copy step needed.
        Write-Host "Mode: dotnet run (from source)" -ForegroundColor DarkCyan
        Push-Location (Join-Path $root $ApiProject)
        try { dotnet run --urls $urls }
        finally { Pop-Location }
    } else {
        $apiDll = Join-Path $artifactsApi 'HotelDroid.Api.dll'
        if (-not (Test-Path $apiDll)) {
            Write-Warning "Artifact not found: $apiDll"
            Write-Host "Tip: run '.\scripts\build.ps1' first, or use -FromSource to rebuild client and run from source."
            throw "No artifact found. Run build.ps1 first."
        }
        # Run from the artifact directory so ASP.NET sets the content root there
        # (and finds wwwroot/index.html for Blazor WASM).
        Write-Host "Mode: published artifact" -ForegroundColor DarkCyan
        Push-Location $artifactsApi
        try { dotnet .\HotelDroid.Api.dll }
        finally { Pop-Location }
    }
} finally { Pop-Location }
