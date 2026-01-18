<#
Deploy API locally and optionally bind a LocalMachine certificate for Kestrel.

Usage examples:
  # Run the API without publishing, attempt to auto-find a 'localhost' cert
  .\scripts\deploy-api-local.ps1

  # Run and publish the API, providing a specific certificate thumbprint
  .\scripts\deploy-api-local.ps1 -Publish -CertThumbprint "ABCDEF123..."

Notes:
  - The script sets environment variables `ASPNETCORE_Kestrel__Certificates__Default__Store`
    and `ASPNETCORE_Kestrel__Certificates__Default__Thumbprint` so Kestrel (Program.cs)
    can pick the certificate from the LocalMachine\My store.
  - If no certificate is found/provided the API will run on HTTP only.
#>

param(
    [string]$ApiProject = "src/HotelDroid.Api",
    [int]$HttpPort = 5000,
    [int]$HttpsPort = 5001,
    [string]$CertThumbprint = $null,
    [switch]$Publish
)

function Get-MachineCertThumbprint {
    param(
        [string]$SubjectHint = "localhost"
    )
    try {
        $store = Get-ChildItem Cert:\LocalMachine\My -ErrorAction Stop
        $found = $store | Where-Object { $_.Subject -like "*$SubjectHint*" } | Sort-Object NotAfter -Descending
        if ($found -and $found.Count -gt 0) { return $found[0].Thumbprint }
    } catch {
        return $null
    }
    return $null
}

if (-not $CertThumbprint) {
    Write-Host "No certificate thumbprint provided; attempting to locate a LocalMachine cert for 'localhost'..."
    $CertThumbprint = Get-MachineCertThumbprint -SubjectHint "localhost"
    if (-not $CertThumbprint) {
        Write-Host "No 'localhost' cert found; trying machine name..."
        $CertThumbprint = Get-MachineCertThumbprint -SubjectHint $env:COMPUTERNAME
    }
}

if ($CertThumbprint) {
    Write-Host "Using certificate thumbprint: $CertThumbprint"
    $env:ASPNETCORE_Kestrel__Certificates__Default__Store = "My"
    $env:ASPNETCORE_Kestrel__Certificates__Default__Thumbprint = $CertThumbprint
} else {
    Write-Warning "No certificate thumbprint found or provided. API will run without HTTPS."
    Remove-Item Env:ASPNETCORE_Kestrel__Certificates__Default__Store -ErrorAction SilentlyContinue
    Remove-Item Env:ASPNETCORE_Kestrel__Certificates__Default__Thumbprint -ErrorAction SilentlyContinue
}

# Publish if requested
if ($Publish) {
    Write-Host "Publishing API project '$ApiProject'..."
    dotnet publish $ApiProject -c Release -o "$PSScriptRoot\publish\api"
    $exePath = Join-Path "$PSScriptRoot\publish\api" "HotelDroid.Api.dll"
}

$urls = "http://localhost:$HttpPort;http://127.0.0.1:$HttpPort"
if ($CertThumbprint) { $urls += ";https://localhost:$HttpsPort;https://127.0.0.1:$HttpsPort" }

if ($Publish) {
    Write-Host "Starting published API from $exePath with URLs: $urls"
    dotnet $exePath --urls $urls
} else {
    Write-Host "Running API project $ApiProject with URLs: $urls"
    dotnet run --project $ApiProject --urls $urls
}
