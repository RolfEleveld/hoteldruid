<#
.SYNOPSIS
  Build a self-contained copy of the hotel, beside the framework-dependent package.

.DESCRIPTION
  Publishes the Blazor client once, then a self-contained API for each runtime,
  into artifacts\selfcontained. The daily build under artifacts\api is left
  alone. Each folder can be copied as it stands. Each zip is
  artifacts\HotelDruid-selfcontained-<runtime>.zip.

  The default runtimes are win-x64 and linux-x64. Pass -Runtime to build one.
  Windows packages include Start-HotelDruid.cmd. Linux packages include
  start-hoteldruid.sh. The desk listens at http://127.0.0.1:8080/. The hotel
  files are the data folder beside that command.

  ReadyToRun is on for win-* runtimes so the process reaches the port without
  a cold JIT of the startup path. A Linux runtime built on Windows stays JIT.
  The publish stays a directory, not one file, so the first start does not
  unpack an archive. linux-x64 is glibc (Ubuntu, Debian, Fedora, RHEL).

.EXAMPLE
  ./deploy/build/package-selfcontained.ps1
  ./deploy/build/package-selfcontained.ps1 -Runtime linux-x64
#>
param(
    [string[]]$Runtime = @('win-x64', 'linux-x64'),
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release'
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$runtimes = @($Runtime | ForEach-Object { $_ -split ',' } | ForEach-Object { $_.Trim() } | Where-Object { $_ })
if ($runtimes.Count -eq 0) { throw 'Pass at least one runtime, for example win-x64 or linux-x64.' }
foreach ($rid in $runtimes) {
    if ($rid -notmatch '^[A-Za-z0-9]+-[A-Za-z0-9]+$') {
        throw "Runtime must look like win-x64 or linux-x64. It was '$rid'."
    }
}

$root = Resolve-Path (Join-Path $PSScriptRoot '../..')
$work = Join-Path $root 'artifacts\selfcontained'
$clientOut = Join-Path $work 'client'
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

function Embed-Client([string]$ApiOut) {
    $wwwroot = Join-Path $ApiOut 'wwwroot'
    $clientWww = Join-Path $clientOut 'wwwroot'
    if (-not (Test-Path $clientWww)) { throw "Client wwwroot was not published at $clientWww." }
    if (Test-Path $wwwroot) { Remove-Item -Recurse -Force $wwwroot }
    Ensure-Dir $wwwroot
    Copy-Item -Path (Join-Path $clientWww '*') -Destination $wwwroot -Recurse -Force

    $indexHtml = Join-Path $wwwroot 'index.html'
    if (-not (Test-Path $indexHtml)) { throw 'index.html was not embedded.' }
    $content = Get-Content $indexHtml -Raw
    $frameworkDir = Join-Path $wwwroot '_framework'
    $bwJs = Get-ChildItem $frameworkDir -Filter 'blazor.webassembly.*.js' -ErrorAction SilentlyContinue |
        Where-Object { $_.Name -notmatch '\.(br|gz)$' } |
        Select-Object -First 1
    if ($bwJs -and $bwJs.BaseName -match '^blazor\.webassembly\.(.+)$') {
        $fp = ".$($Matches[1])"
        $resolved = $content -replace '#\[\.{fingerprint}\]', $fp
        if ($resolved -ne $content) {
            Set-Content $indexHtml $resolved -NoNewline
        }
    }

    $manifestPath = Join-Path $clientOut 'HotelDruid.Client.staticwebassets.endpoints.json'
    if (-not (Test-Path $manifestPath)) { throw "Client manifest was not found at $manifestPath." }
    $manifest = Get-Content $manifestPath -Raw | ConvertFrom-Json
    $aliases = @($manifest.Endpoints | Where-Object {
        $_.Route -and $_.AssetFile -and
        $_.Route -ne $_.AssetFile -and
        $_.Route -notmatch '\.[a-z0-9]{8,12}\.' -and
        $_.AssetFile -notmatch '\.(br|gz)$' -and
        (-not $_.Selectors -or $_.Selectors.Count -eq 0)
    })
    $aliasCount = 0
    foreach ($ep in $aliases) {
        $dest = Join-Path $wwwroot ($ep.Route -replace '/', '\')
        $src = Join-Path $wwwroot ($ep.AssetFile -replace '/', '\')
        if ((Test-Path $src) -and -not (Test-Path $dest)) {
            $destDir = Split-Path $dest -Parent
            Ensure-Dir $destDir
            Copy-Item $src $dest
            $aliasCount++
        }
    }
    Write-Host "  Linked $aliasCount plain asset names." -ForegroundColor DarkGray
    if (-not (Test-Path (Join-Path $wwwroot '_framework\dotnet.js'))) {
        throw 'dotnet.js was not linked into wwwroot.'
    }
}

Write-Host "Publishing the Blazor client ($Configuration)..." -ForegroundColor Cyan
if (Test-Path $clientOut) { Remove-Item -Recurse -Force $clientOut }
Push-Location $root
try {
    & dotnet publish src/HotelDruid.Client -c $Configuration -o $clientOut @props --nologo
    if ($LASTEXITCODE -ne 0) { throw "Client publish failed (exit $LASTEXITCODE)." }
} finally { Pop-Location }

function Set-LinuxZipModes([string]$ZipPath) {
    Add-Type -AssemblyName System.IO.Compression
    Add-Type -AssemblyName System.IO.Compression.FileSystem
    $names = @('HotelDruid.Api', 'start-hoteldruid.sh')
    # 0100755, little-endian, so the high word survives a signed int property.
    $modeBytes = [byte[]](0, 0, 0xED, 0x81)
    $signedMode = [BitConverter]::ToInt32($modeBytes, 0)
    $mode = [BitConverter]::ToUInt32($modeBytes, 0)
    $zip = [System.IO.Compression.ZipFile]::Open($ZipPath, [System.IO.Compression.ZipArchiveMode]::Update)
    try {
        foreach ($entry in $zip.Entries) {
            $leaf = [IO.Path]::GetFileName($entry.FullName.Replace('\', '/'))
            if ($names -notcontains $leaf) { continue }
            # Unix regular file, mode 0755, stored in the high 16 bits.
            $entry.ExternalAttributes = $signedMode
        }
    } finally {
        $zip.Dispose()
    }

    # The Windows zip writer records itself as the creating system. Linux unzip
    # reads the mode only when that field says Unix, so patch the central directory.
    $bytes = [IO.File]::ReadAllBytes($ZipPath)
    $ascii = [Text.Encoding]::ASCII
    $central = -1
    for ($i = $bytes.Length - 22; $i -ge 0; $i--) {
        if ($bytes[$i] -eq 0x50 -and $bytes[$i + 1] -eq 0x4B -and $bytes[$i + 2] -eq 0x05 -and $bytes[$i + 3] -eq 0x06) {
            $central = [int][BitConverter]::ToUInt32($bytes, $i + 16)
            break
        }
    }
    if ($central -lt 0) { throw "Zip central directory was not found in $ZipPath." }
    $i = $central
    while ($i -le $bytes.Length - 46 -and $bytes[$i] -eq 0x50 -and $bytes[$i + 1] -eq 0x4B -and $bytes[$i + 2] -eq 0x01 -and $bytes[$i + 3] -eq 0x02) {
        $nameLength = [int][BitConverter]::ToUInt16($bytes, $i + 28)
        $extraLength = [int][BitConverter]::ToUInt16($bytes, $i + 30)
        $commentLength = [int][BitConverter]::ToUInt16($bytes, $i + 32)
        $name = $ascii.GetString($bytes, $i + 46, $nameLength).Replace('\', '/')
        $leaf = [IO.Path]::GetFileName($name)
        if ($names -contains $leaf) {
            $bytes[$i + 5] = 3
            [BitConverter]::GetBytes($mode).CopyTo($bytes, $i + 38)
        }
        $i += 46 + $nameLength + $extraLength + $commentLength
    }
    [IO.File]::WriteAllBytes($ZipPath, $bytes)
}

foreach ($rid in $runtimes) {
Write-Host "Publishing the self-contained API ($rid)..." -ForegroundColor Cyan
$apiOut = Join-Path $work $rid
$apiProps = @($props)
if ($rid -like 'win-*') { $apiProps += '-p:PublishReadyToRun=true' }
if (Test-Path $apiOut) { Remove-Item -Recurse -Force $apiOut }
Push-Location $root
try {
    & dotnet publish src/HotelDruid.Api -c $Configuration -r $rid --self-contained true -o $apiOut @apiProps --nologo
    if ($LASTEXITCODE -ne 0) { throw "API publish failed for $rid (exit $LASTEXITCODE)." }
} finally { Pop-Location }

$exeName = if ($rid -like 'win-*') { 'HotelDruid.Api.exe' } else { 'HotelDruid.Api' }
if (-not (Test-Path (Join-Path $apiOut $exeName))) {
    throw "$exeName was not produced in $apiOut."
}

Write-Host 'Embedding the client...' -ForegroundColor Cyan
Embed-Client -ApiOut $apiOut

if ($rid -like 'win-*') {
    Copy-Item -Path (Join-Path $PSScriptRoot 'selfcontained\Start-HotelDruid.cmd') -Destination (Join-Path $apiOut 'Start-HotelDruid.cmd') -Force
    @'
HotelDruid copy package.

Unzip this folder, or copy it, onto the computer that will run the hotel.
Run Start-HotelDruid.cmd in this folder.

The runtime is already in the folder. Nothing else is installed.
The desk listens at http://127.0.0.1:8080/.
The hotel files are the data folder created next to Start-HotelDruid.cmd.
Close the window to stop the desk.

The other download, HotelDruid-desktop.zip, is the installed desk. It registers a startup task and uses https://localhost:5001/. This folder does not.
'@ | Set-Content -Path (Join-Path $apiOut 'START.txt') -Encoding utf8
} else {
    $sh = @'
#!/bin/sh
cd "$(dirname "$0")" || exit 1
mkdir -p data logs
export HOTELDRUID_DATAROOT="$PWD/data"
export HOTELDRUID_LOGDIR="$PWD/logs"
export ASPNETCORE_ENVIRONMENT=Production
export ASPNETCORE_URLS=http://127.0.0.1:8080
echo "HotelDruid is starting at http://127.0.0.1:8080/"
echo "The hotel files are in the data folder next to this script."
exec ./HotelDruid.Api
'@
    $shPath = Join-Path $apiOut 'start-hoteldruid.sh'
    [System.IO.File]::WriteAllText($shPath, $sh.Replace("`r`n", "`n"))
    @'
HotelDruid copy package.

Copy this folder onto a glibc Linux computer (Ubuntu, Debian, Fedora, or RHEL).
Run ./start-hoteldruid.sh in this folder.

The runtime is already in the folder.
The desk listens at http://127.0.0.1:8080/.
The hotel files are the data folder created next to the script.

If the shell says Permission denied, run:
  chmod +x start-hoteldruid.sh HotelDruid.Api
  ./start-hoteldruid.sh
'@ | Set-Content -Path (Join-Path $apiOut 'START.txt') -Encoding utf8
}

$package = Join-Path $root "artifacts\HotelDruid-selfcontained-$rid.zip"
if (Test-Path $package) { Remove-Item -Force $package }
Add-Type -AssemblyName System.IO.Compression.FileSystem
[System.IO.Compression.ZipFile]::CreateFromDirectory($apiOut, $package)
if ($rid -notlike 'win-*') { Set-LinuxZipModes -ZipPath $package }
$info = Get-Item $package
$mb = [Math]::Round($info.Length / 1MB, 1)
Write-Host "Created $package ($mb MB)" -ForegroundColor Green
Write-Host "Copy folder: $apiOut" -ForegroundColor Green
}
