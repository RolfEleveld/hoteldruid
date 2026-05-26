#!/usr/bin/env pwsh
param(
    [string]$BaseUrl = 'https://localhost:5001',
    [string]$HealthPath = '/health',
    [switch]$RequireAuth,
    [string]$ProtectedPath = '/',
    [int]$StartupTimeoutSeconds = 60,
    [int]$PollIntervalSeconds = 2
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Resolve-AssetUrl {
    param(
        [Uri]$RootUri,
        [string]$AssetPath
    )

    if ([string]::IsNullOrWhiteSpace($AssetPath)) {
        return $null
    }

    $trimmed = $AssetPath.Trim()
    if ($trimmed.StartsWith('#') -or $trimmed.StartsWith('data:')) {
        return $null
    }

    if ($trimmed.StartsWith('http://') -or $trimmed.StartsWith('https://')) {
        return $null
    }

    if ($trimmed.StartsWith('//')) {
        return $null
    }

    return [Uri]::new($RootUri, $trimmed)
}

$rootUri = [Uri]::new($BaseUrl)
$healthUrl = "$BaseUrl$HealthPath"
$deadline = (Get-Date).AddSeconds($StartupTimeoutSeconds)
$healthResponse = $null

Write-Host "Waiting for health endpoint: $healthUrl" -ForegroundColor Cyan
while ((Get-Date) -lt $deadline) {
    try {
        $healthResponse = Invoke-WebRequest -Uri $healthUrl -Method Get -SkipCertificateCheck -TimeoutSec $PollIntervalSeconds
        if ($healthResponse.StatusCode -eq 200) {
            break
        }
    }
    catch {
        # App may still be starting.
    }

    Start-Sleep -Seconds $PollIntervalSeconds
}

if ($null -eq $healthResponse -or $healthResponse.StatusCode -ne 200) {
    throw "Health check failed for $healthUrl within $StartupTimeoutSeconds seconds."
}
Write-Host 'Health endpoint is healthy.' -ForegroundColor Green

$rootPageUrl = [Uri]::new($rootUri, '/')
Write-Host "Checking app shell: $rootPageUrl" -ForegroundColor Cyan
$rootPageResponse = Invoke-WebRequest -Uri $rootPageUrl -Method Get -SkipCertificateCheck
if ($rootPageResponse.StatusCode -ne 200) {
    throw "Root page check failed with status $($rootPageResponse.StatusCode)"
}

$rootHtml = $rootPageResponse.Content
if ($rootHtml -notmatch '<div\s+id="app"') {
    throw 'Root page does not contain the Blazor app host container.'
}

$scriptMatch = [System.Text.RegularExpressions.Regex]::Match($rootHtml, '<script\s+src="([^"]*blazor\.webassembly[^"]*\.js)"')
if (-not $scriptMatch.Success) {
    throw 'Root page is missing the Blazor bootstrap script reference.'
}

$bootstrapScriptPath = $scriptMatch.Groups[1].Value
if ($bootstrapScriptPath.Contains('#[')) {
    throw "Blazor bootstrap script still contains unresolved fingerprint token: $bootstrapScriptPath"
}

$localAssetMatches = [System.Text.RegularExpressions.Regex]::Matches($rootHtml, '(?:href|src)="([^"]+)"')
$localAssetUrls = New-Object System.Collections.Generic.HashSet[string]([System.StringComparer]::OrdinalIgnoreCase)
foreach ($match in $localAssetMatches) {
    $assetRef = $match.Groups[1].Value
    $assetUrl = Resolve-AssetUrl -RootUri $rootUri -AssetPath $assetRef
    if ($null -ne $assetUrl) {
        [void]$localAssetUrls.Add($assetUrl.AbsoluteUri)
    }
}

Write-Host ("Checking {0} local static assets..." -f $localAssetUrls.Count) -ForegroundColor Cyan
foreach ($asset in $localAssetUrls) {
    $assetResponse = Invoke-WebRequest -Uri $asset -Method Get -SkipCertificateCheck
    if ($assetResponse.StatusCode -ne 200) {
        throw "Static asset check failed for $asset with status $($assetResponse.StatusCode)"
    }
}

Write-Host 'App shell and static assets are responsive.' -ForegroundColor Green

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
