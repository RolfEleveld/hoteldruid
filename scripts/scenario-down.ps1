#!/usr/bin/env pwsh
param(
    [string[]]$ComposeFiles,
    [string]$EnvFile = '',
    [string]$ProjectName = 'hoteldroid',
    [switch]$RemoveVolumes,
    [switch]$RemoveOrphans
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

if (-not $ComposeFiles -or $ComposeFiles.Count -eq 0) {
    throw 'ComposeFiles is required. Provide the same compose files used for scenario-up.'
}

$args = @('compose', '-p', $ProjectName)
foreach ($file in $ComposeFiles) {
    $args += @('-f', $file)
}

if ($EnvFile) {
    $args += @('--env-file', $EnvFile)
}

$args += @('down')
if ($RemoveVolumes) {
    $args += '--volumes'
}
if ($RemoveOrphans) {
    $args += '--remove-orphans'
}

Write-Host "Tearing down compose project '$ProjectName'." -ForegroundColor Cyan
& docker @args
if ($LASTEXITCODE -ne 0) {
    throw "docker compose down failed (exit $LASTEXITCODE)"
}

Write-Host 'Scenario environment is down.' -ForegroundColor Green
