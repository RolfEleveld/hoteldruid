<#
.SYNOPSIS
  Install HotelDruid for everyone on this computer.

.DESCRIPTION
  When this file sits next to HotelDruid.Api.dll it installs that folder.
  When it is fetched on its own it downloads HotelDruid-desktop.zip from the
  blazor-latest release and installs that.

  The release build stamps the repository name over __GITHUB_REPOSITORY__.
  Until then, pass -Repository owner/name.

.EXAMPLE
  iex (irm https://github.com/owner/hotelDruid/releases/download/blazor-latest/install.ps1)
#>
param(
    [string]$Repository = '__GITHUB_REPOSITORY__',
    [string]$PackageUrl = ''
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
$ProgressPreference = 'SilentlyContinue'

function Test-PackageRoot([string]$Path) {
    if ([string]::IsNullOrWhiteSpace($Path)) { return $false }
    $dll = Join-Path $Path 'HotelDruid.Api.dll'
    $launcher = Join-Path $Path 'HotelDruid.ps1'
    return (Test-Path $dll) -and (Test-Path $launcher)
}

$source = $PSScriptRoot
if (Test-PackageRoot $source) {
    & (Join-Path $source 'HotelDruid.ps1') -Action Install
    return
}

if ([string]::IsNullOrWhiteSpace($PackageUrl)) {
    if ([string]::IsNullOrWhiteSpace($Repository) -or $Repository -eq '__GITHUB_REPOSITORY__') {
        throw @"
This installer does not know the GitHub repository yet.
Download HotelDruid-desktop.zip from the blazor-latest release, unzip it, and run HotelDruid.cmd Install.
Or pass -Repository owner/name.
"@
    }
    $PackageUrl = "https://github.com/$Repository/releases/download/blazor-latest/HotelDruid-desktop.zip"
}

$stage = Join-Path $env:TEMP ('HotelDruid-stage-' + [guid]::NewGuid().ToString('n'))
$app = Join-Path $stage 'app'
New-Item -ItemType Directory -Path $app -Force | Out-Null
$zip = Join-Path $stage 'HotelDruid-desktop.zip'

Write-Host "Downloading $PackageUrl" -ForegroundColor Cyan
Invoke-WebRequest -Uri $PackageUrl -OutFile $zip
Expand-Archive -Path $zip -DestinationPath $app -Force

if (-not (Test-PackageRoot $app)) {
    throw "The release package did not contain HotelDruid.Api.dll and HotelDruid.ps1."
}

try {
    & (Join-Path $app 'HotelDruid.ps1') -Action Install
}
finally {
    Remove-Item -Recurse -Force -ErrorAction SilentlyContinue $stage
}
