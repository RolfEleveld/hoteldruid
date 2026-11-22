<#
.SYNOPSIS
  Build the Windows desk zip without publishing over the local hotel.

.DESCRIPTION
  Publishes a Release client, then a Release API, into artifacts\play.
  The running hotel under artifacts\api is left alone. The zip is
  artifacts\HotelDruid-desktop.zip and includes the desk installer, the demo
  loader, and DEPLOYMENT.md.
#>
param(
    [string]$Configuration = 'Release'
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$root = Resolve-Path (Join-Path $PSScriptRoot '../..')
$play = Join-Path $root 'artifacts\play'
$clientOut = Join-Path $play 'client'
$apiOut = Join-Path $play 'api'
$props = @('-p:DebugType=none', '-p:DebugSymbols=false')
$pipelineBuild = [string]$env:HOTELDRUID_BUILD_NUMBER
if (-not [string]::IsNullOrWhiteSpace($pipelineBuild)) {
    if ($pipelineBuild -notmatch '^\d+$') { throw "HOTELDRUID_BUILD_NUMBER must be a whole number. It was '$pipelineBuild'." }
    [xml]$versionXml = Get-Content -Path (Join-Path $root 'Directory.Build.props')
    $baseVersion = [string]$versionXml.Project.PropertyGroup.Version
    if ([string]::IsNullOrWhiteSpace($baseVersion)) { throw 'Directory.Build.props has no Version.' }
    $props += "-p:InformationalVersion=$baseVersion.$pipelineBuild"
    $props += '-p:IncludeSourceRevisionInInformationalVersion=false'
    if ([int64]$pipelineBuild -le 65535) {
        $props += "-p:FileVersion=$baseVersion.$pipelineBuild"
    }
}

function Ensure-Dir([string]$Path) {
    if (-not (Test-Path $Path)) { New-Item -ItemType Directory -Path $Path -Force | Out-Null }
}

Write-Host 'Publishing the Blazor client...' -ForegroundColor Cyan
if (Test-Path $clientOut) { Remove-Item -Recurse -Force $clientOut }
Push-Location $root
try {
    & dotnet publish src/HotelDruid.Client -c $Configuration -o $clientOut @props --nologo
    if ($LASTEXITCODE -ne 0) { throw "Client publish failed (exit $LASTEXITCODE)." }
} finally { Pop-Location }

Write-Host 'Publishing the API...' -ForegroundColor Cyan
if (Test-Path $apiOut) { Remove-Item -Recurse -Force $apiOut }
Push-Location $root
try {
    & dotnet publish src/HotelDruid.Api -c $Configuration -o $apiOut @props --nologo
    if ($LASTEXITCODE -ne 0) { throw "API publish failed (exit $LASTEXITCODE)." }
} finally { Pop-Location }

Write-Host 'Embedding the client...' -ForegroundColor Cyan
$wwwroot = Join-Path $apiOut 'wwwroot'
$clientWww = Join-Path $clientOut 'wwwroot'
if (Test-Path $wwwroot) { Remove-Item -Recurse -Force $wwwroot }
Ensure-Dir $wwwroot
Copy-Item -Path (Join-Path $clientWww '*') -Destination $wwwroot -Recurse -Force
& python --version 2>$null | Out-Null
$python = Get-Command python -ErrorAction SilentlyContinue
if (-not $python) { $python = Get-Command python3 -ErrorAction SilentlyContinue }
if (-not $python) { throw 'Python is required to link the Blazor file names.' }
& $python.Source (Join-Path $PSScriptRoot 'docker\link-assets.py') $clientOut
if ($LASTEXITCODE -ne 0) { throw "Linking client assets failed (exit $LASTEXITCODE)." }
Copy-Item -Path (Join-Path $clientWww '*') -Destination $wwwroot -Recurse -Force
if (-not (Test-Path (Join-Path $wwwroot 'index.html'))) { throw 'index.html was not embedded.' }
if (-not (Test-Path (Join-Path $wwwroot '_framework\dotnet.js'))) { throw 'dotnet.js was not linked.' }

Write-Host 'Publishing the demo generator...' -ForegroundColor Cyan
$scenarioOut = Join-Path $apiOut 'scenario'
& dotnet publish (Join-Path $root 'tools\HotelDruid.Scenario\HotelDruid.Scenario.csproj') -c $Configuration -o $scenarioOut @props --nologo
if ($LASTEXITCODE -ne 0) { throw "Scenario publish failed (exit $LASTEXITCODE)." }

$desktop = Join-Path $PSScriptRoot 'desktop'
foreach ($name in @('HotelDruid.cmd', 'HotelDruid.ps1', 'install.ps1', 'Load-DemoHotel.ps1', 'Backup-HotelDruid.ps1')) {
    Copy-Item -Path (Join-Path $desktop $name) -Destination (Join-Path $apiOut $name) -Force
}
Copy-Item -Path (Join-Path $PSScriptRoot 'desk-certificate.ps1') -Destination (Join-Path $apiOut 'desk-certificate.ps1') -Force
@'
HotelDruid desk package.

Run HotelDruid.cmd in this folder.
Approve the administrator prompt. The desk is installed for this computer and starts at every boot.

The same install from the published release, without copying this folder first:

  iex (irm https://github.com/RolfEleveld/hoteldruid/releases/download/blazor-latest/install.ps1)

After that install, the desk is at C:\ProgramData\HotelDruid\app.
Other computers on the network use C:\ProgramData\HotelDruid\app\desk-certificate.ps1.
'@ | Set-Content -Path (Join-Path $apiOut 'INSTALL.txt') -Encoding utf8

$package = Join-Path $root 'artifacts\HotelDruid-desktop.zip'
if (Test-Path $package) { Remove-Item -Force $package }
Add-Type -AssemblyName System.IO.Compression.FileSystem
[System.IO.Compression.ZipFile]::CreateFromDirectory($apiOut, $package)
$info = Get-Item $package
$mb = [Math]::Round($info.Length / 1MB, 1)
Write-Host "Created $package ($mb MB)" -ForegroundColor Green
