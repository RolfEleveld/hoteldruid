#!/usr/bin/env pwsh
param(
    [string]$BaseUrl = 'https://localhost',
    [string]$HealthPath = '/health',
    [switch]$RequireAuth,
    [string]$ProtectedPath = '/'
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$healthUrl = "$BaseUrl$HealthPath"
Write-Host "Checking health endpoint: $healthUrl" -ForegroundColor Cyan
$healthResponse = Invoke-WebRequest -Uri $healthUrl -Method Get -SkipCertificateCheck
if ($healthResponse.StatusCode -ne 200) {
    throw "Health check failed with status $($healthResponse.StatusCode)"
}
Write-Host 'Health endpoint is healthy.' -ForegroundColor Green

if ($RequireAuth) {
    $protectedUrl = "$BaseUrl$ProtectedPath"
    Write-Host "Checking auth gate on: $protectedUrl" -ForegroundColor Cyan
    try {
        $response = Invoke-WebRequest -Uri $protectedUrl -Method Get -MaximumRedirection 0 -SkipCertificateCheck -ErrorAction Stop
        if ($response.StatusCode -eq 200) {
            throw 'Protected endpoint returned 200 without auth; expected redirect or unauthorized response.'
        }
    }
    catch {
        $webResponse = $_.Exception.Response
        if ($null -eq $webResponse) {
            throw
        }

        $statusCode = [int]$webResponse.StatusCode
        if (($statusCode -ne 401) -and ($statusCode -ne 403) -and ($statusCode -ne 302)) {
            throw "Unexpected auth response code: $statusCode"
        }
    }

    Write-Host 'Auth gate behavior looks valid.' -ForegroundColor Green
}

Write-Host 'Scenario tests completed successfully.' -ForegroundColor Green
