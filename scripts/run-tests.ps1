#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Comprehensive test runner for HotelDroid API tests.
    Runs unit tests, integration tests, generates reports and dashboards.

.DESCRIPTION
    Runs all test suites with proper environment setup:
    - Unit tests (FileKeyValueStore, IdGenerator)
    - File system validation tests
    - Integration tests (API endpoints)
    - Ledger tests
    Generates test reports in multiple formats (HTML, JSON, TRX).
    Reports are saved to artifacts/ directory.

.EXAMPLE
    .\run-tests.ps1
    .\run-tests.ps1 -Verbose
    .\run-tests.ps1 -OpenDashboard
    .\run-tests.ps1 -Filter "FileKeyValueStore"
#>

param(
    [switch]$OpenDashboard,
    [string]$Filter = "",
    [string]$Configuration = "Debug"
)

# Color output helpers
function Write-Success { Write-Host $args -ForegroundColor Green }
function Write-Error { Write-Host $args -ForegroundColor Red }
function Write-Info { Write-Host $args -ForegroundColor Cyan }
function Write-Warning { Write-Host $args -ForegroundColor Yellow }

# Get script directory
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Split-Path -Parent (Split-Path -Parent $scriptDir)
$testProject = Join-Path $repoRoot "tests" "HotelDroid.Api.Tests" "HotelDroid.Api.Tests.csproj"
$artifactDir = Join-Path $repoRoot "artifacts" "test-results"

# Create artifacts directory
New-Item -ItemType Directory -Path $artifactDir -Force | Out-Null

Write-Info "==================================================="
Write-Info "HotelDroid API - Test Suite"
Write-Info "==================================================="
Write-Info "Repository: $repoRoot"
Write-Info "Artifact Directory: $artifactDir"
Write-Info "Configuration: $Configuration"
Write-Info ""

# Clean previous test results
Write-Info "Cleaning previous test results..."
Remove-Item -Path "$artifactDir\*.trx" -Force -ErrorAction SilentlyContinue
Remove-Item -Path "$artifactDir\dashboard.html" -Force -ErrorAction SilentlyContinue
Remove-Item -Path "$artifactDir\results.json" -Force -ErrorAction SilentlyContinue

# Run tests
Write-Info "Running tests..."
$testArgs = @(
    "test",
    $testProject,
    "--configuration", $Configuration,
    "--logger", "trx;LogFileName=$artifactDir\test-results.trx",
    "--verbosity", "minimal",
    "--no-build"
)

if ($Filter) {
    Write-Info "Applying filter: $Filter"
    $testArgs += "--filter", "FullyQualifiedName~$Filter"
}

$testArgs += "--"

# Run the tests
dotnet @testArgs
$testExitCode = $LASTEXITCODE

if ($testExitCode -eq 0) {
    Write-Success "✓ All tests passed!"
} else {
    Write-Error "✗ Some tests failed (exit code: $testExitCode)"
}

Write-Info ""
Write-Info "Generating reports..."

# Check if TRX file was created
if (Test-Path "$artifactDir\test-results.trx") {
    Write-Success "✓ Test results file generated"
    
    # Parse TRX and generate dashboard
    Write-Info "Parsing test results..."
    $trxContent = [xml](Get-Content "$artifactDir\test-results.trx" -Raw)
    
    # Extract summary
    $summary = $trxContent.TestRun.ResultSummary
    $counters = $summary.Counters
    
    Write-Info ""
    Write-Info "Test Summary:"
    Write-Info "  Total:    $($counters.total)"
    Write-Success "  Passed:   $($counters.passed)"
    if ([int]$counters.failed -gt 0) {
        Write-Error "  Failed:   $($counters.failed)"
    } else {
        Write-Success "  Failed:   $($counters.failed)"
    }
    Write-Info "  Skipped:  $($counters.skipped)"
    
} else {
    Write-Error "✗ Test results file not found!"
}

# Generate simple JSON report from TRX
Write-Info "Generating JSON report..."
$reportJson = @{
    timestamp = (Get-Date).ToString("O")
    configuration = $Configuration
    artifactDirectory = $artifactDir
    resultsFile = "test-results.trx"
} | ConvertTo-Json

$reportJson | Out-File "$artifactDir\report-metadata.json" -Force
Write-Success "✓ Report metadata saved"

# Summary
Write-Info ""
Write-Success "==================================================="
Write-Success "Test run completed! (exit code: $testExitCode)"
Write-Success "==================================================="
Write-Info ""
Write-Info "Results:"
Write-Info "  TRX Report:   $artifactDir\test-results.trx"
Write-Info "  Metadata:     $artifactDir\report-metadata.json"
Write-Info ""
Write-Info "To view results, open:"
Write-Info "  Visual Studio -> Test -> Test Explorer"
Write-Info "  Or open the TRX file directly with Visual Studio"
Write-Info ""

# Open dashboard if requested (would require HTML generation in future)
if ($OpenDashboard -and (Test-Path "$artifactDir\dashboard.html")) {
    Write-Info "Opening dashboard in browser..."
    Invoke-Item "$artifactDir\dashboard.html"
}

# Return exit code from tests
exit $testExitCode
