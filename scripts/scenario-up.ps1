#!/usr/bin/env pwsh
param(
    [ValidateSet('internal-selfsigned-private','public-acme-keycloak','public-externalcert-keycloak')]
    [string]$Scenario = 'internal-selfsigned-private',
    [string[]]$ComposeFiles,
    [string]$EnvFile = '',
    [string]$ProjectName = 'hoteldroid',
    [switch]$Build
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

if (-not $ComposeFiles -or $ComposeFiles.Count -eq 0) {
    throw 'ComposeFiles is required. Provide one or more compose files for your chosen proxy/identity stack.'
}

$args = @('compose', '-p', $ProjectName)
foreach ($file in $ComposeFiles) {
    $args += @('-f', $file)
}

if ($EnvFile) {
    $args += @('--env-file', $EnvFile)
}

$args += @('up', '-d')
if ($Build) {
    $args += '--build'
}

Write-Host "Starting scenario '$Scenario' with compose files: $($ComposeFiles -join ', ')" -ForegroundColor Cyan
& docker @args
if ($LASTEXITCODE -ne 0) {
    throw "docker compose up failed (exit $LASTEXITCODE)"
}

Write-Host "Scenario '$Scenario' is up." -ForegroundColor Green
