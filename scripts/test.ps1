#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Run all HotelDroid tests (unit, integration).
.DESCRIPTION
    Runs all test projects and generates reports in artifacts/test-results.
.EXAMPLE
    ./test.ps1
    ./test.ps1 -OpenDashboard
    ./test.ps1 -Filter "FileKeyValueStore"
#>
param(
    [switch]$OpenDashboard,
    [string]$Filter = "",
    [string]$Configuration = "Debug"
)

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Join-Path $scriptDir '..'
$artifactDir = Join-Path $repoRoot 'artifacts' 'test-results'
New-Item -ItemType Directory -Path $artifactDir -Force | Out-Null

Write-Host "Running all tests..."
$testProjects = @(
    'tests/HotelDroid.Api.Tests/HotelDroid.Api.Tests.csproj',
    'tests/HotelDroid.Client.Tests/HotelDroid.Client.Tests.csproj',
    'tests/HotelDroid.Migration.Tests/HotelDroid.Migration.Tests.csproj'
)
foreach ($proj in $testProjects) {
    dotnet test $proj -c $Configuration --logger "trx;LogFileName=$artifactDir/$(Split-Path $proj -Leaf).trx" --results-directory $artifactDir --no-build
}
Write-Host "Test results in $artifactDir"
if ($OpenDashboard) {
    # Optionally open a dashboard/report if available
    Write-Host "(Dashboard opening not implemented)"
}