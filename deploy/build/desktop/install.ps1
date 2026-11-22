<#
.SYNOPSIS
  Install the HotelDruid desk on this computer.

.DESCRIPTION
  Fetched on its own, this downloads HotelDruid-desktop.zip from the
  blazor-latest release and installs it. The release build stamps the
  repository name over __GITHUB_REPOSITORY__.

  When the file already sits next to HotelDruid.Api.dll, that folder is installed
  and nothing is downloaded.

.EXAMPLE
  iex (irm https://github.com/owner/hotelDruid/releases/download/blazor-latest/install.ps1)

.EXAMPLE
  powershell -NoProfile -ExecutionPolicy Bypass -File .\install.ps1 -Repository owner/name
#>
Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
$ProgressPreference = 'SilentlyContinue'

$repository = '__GITHUB_REPOSITORY__'
$packageUrl = ''
for ($i = 0; $i -lt $args.Count; $i++) {
    if ($args[$i] -eq '-Repository' -and ($i + 1) -lt $args.Count) { $repository = [string]$args[$i + 1] }
    if ($args[$i] -eq '-PackageUrl' -and ($i + 1) -lt $args.Count) { $packageUrl = [string]$args[$i + 1] }
}

function Test-PackageRoot([string]$Path) {
    if ([string]::IsNullOrWhiteSpace($Path)) { return $false }
    return (Test-Path (Join-Path $Path 'HotelDruid.Api.dll')) -and (Test-Path (Join-Path $Path 'HotelDruid.ps1'))
}

$source = $PSScriptRoot
if (Test-PackageRoot $source) {
    & (Join-Path $source 'HotelDruid.ps1') -Action Install
    $code = 0
    if (Get-Variable -Name LASTEXITCODE -Scope Global -ErrorAction SilentlyContinue) { $code = [int]$global:LASTEXITCODE }
    if ($code -ne 0) { exit $code }
    return
}

if ([string]::IsNullOrWhiteSpace($packageUrl)) {
    if ([string]::IsNullOrWhiteSpace($repository) -or $repository -eq '__GITHUB_REPOSITORY__') {
        throw "This installer does not know the GitHub repository yet. Download HotelDruid-desktop.zip, unzip it, and run HotelDruid.cmd."
    }
    $packageUrl = "https://github.com/$repository/releases/download/blazor-latest/HotelDruid-desktop.zip"
}

$stage = Join-Path $env:TEMP ('HotelDruid-stage-' + [guid]::NewGuid().ToString('n'))
$app = Join-Path $stage 'app'
New-Item -ItemType Directory -Path $app -Force | Out-Null
$zip = Join-Path $stage 'HotelDruid-desktop.zip'

Write-Host "Downloading $packageUrl" -ForegroundColor Cyan
Invoke-WebRequest -Uri $packageUrl -OutFile $zip
Expand-Archive -Path $zip -DestinationPath $app -Force
if (-not (Test-PackageRoot $app)) {
    throw 'The release package did not contain HotelDruid.Api.dll and HotelDruid.ps1.'
}

try {
    & (Join-Path $app 'HotelDruid.ps1') -Action Install
    $code = 0
    if (Get-Variable -Name LASTEXITCODE -Scope Global -ErrorAction SilentlyContinue) { $code = [int]$global:LASTEXITCODE }
    if ($code -ne 0) { throw "Install failed ($code)." }
}
finally {
    Remove-Item -Recurse -Force -ErrorAction SilentlyContinue $stage
}
