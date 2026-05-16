#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Run all HotelDruid tests (unit, integration).
.DESCRIPTION
    Runs all test projects and generates reports in artifacts/test-results.
.EXAMPLE
    ./test.ps1
    ./test.ps1 -OpenDashboard
    ./test.ps1 -Filter "FileKeyValueStore"
#>
param(
    [switch]$OpenDashboard,
    [switch]$Clean,
    [string]$Filter = "",
    [string]$Configuration = "Debug"
)

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Join-Path $scriptDir '..'
$artifactDir = Join-Path $repoRoot 'artifacts' 'test-results'
New-Item -ItemType Directory -Path $artifactDir -Force | Out-Null

Write-Host "Running all tests..."
$testProjects = @(
    'tests/HotelDruid.Api.Tests/HotelDruid.Api.Tests.csproj',
    'tests/HotelDruid.Client.Tests/HotelDruid.Client.Tests.csproj',
    'tests/HotelDruid.Migration.Tests/HotelDruid.Migration.Tests.csproj'
)

if ($Clean) {
    Write-Host "Cleaning previous test outputs..." -ForegroundColor Cyan
    Remove-Item -Recurse -Force -ErrorAction SilentlyContinue $artifactDir
    New-Item -ItemType Directory -Path $artifactDir -Force | Out-Null

    Push-Location $repoRoot
    try {
        foreach ($proj in $testProjects) {
            dotnet clean $proj -c $Configuration --nologo
            if ($LASTEXITCODE -ne 0) { throw "dotnet clean failed for $proj (exit $LASTEXITCODE)" }
        }
    } finally { Pop-Location }
}

foreach ($proj in $testProjects) {
    $testArgs = @(
        'test',
        $proj,
        '-c', $Configuration,
        '--logger', "trx;LogFileName=$artifactDir/$(Split-Path $proj -Leaf).trx",
        '--results-directory', $artifactDir
    )

    if ($Filter) {
        $testArgs += @('--filter', $Filter)
    }

    dotnet @testArgs
    if ($LASTEXITCODE -ne 0) { throw "dotnet test failed for $proj (exit $LASTEXITCODE)" }
}
Write-Host "Test results in $artifactDir"
if ($OpenDashboard) {
    # Optionally open a dashboard/report if available
    Write-Host "(Dashboard opening not implemented)"
}
