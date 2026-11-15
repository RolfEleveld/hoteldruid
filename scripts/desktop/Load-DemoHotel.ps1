<#
.SYNOPSIS
  Save the current desk, then load a Belvedere demo that crosses today.

.DESCRIPTION
  Calculates the range from this computer's clock: five years of history and six
  months of reservations ahead. Writes an export of the current hotel into the
  current user's Temp folder, then replaces C:\ProgramData\HotelDruid\data.
  The export path is printed at the end. The live hotel on another computer is
  not used. Run this in an administrator PowerShell on the desk that should
  receive the demo.
#>
param(
    [string]$DataRoot = $(Join-Path $env:ProgramData 'HotelDruid\data'),
    [int]$Rooms = 20,
    [double]$Occupancy = 0.67,
    [int]$Seed = 20261114,
    [string]$BaseUrl = 'https://localhost:5001/'
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
$TaskName = 'HotelDruid'

function Test-IsAdministrator {
    $identity = [Security.Principal.WindowsIdentity]::GetCurrent()
    $principal = New-Object Security.Principal.WindowsPrincipal($identity)
    return $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
}

if (-not (Test-IsAdministrator)) {
    Write-Host 'Administrator approval is required.' -ForegroundColor Yellow
    $launch = "-NoProfile -ExecutionPolicy Bypass -File `"$PSCommandPath`""
    $elevated = Start-Process -FilePath "$env:SystemRoot\System32\WindowsPowerShell\v1.0\powershell.exe" -ArgumentList $launch -Verb RunAs -Wait -PassThru
    if ($elevated) { exit $elevated.ExitCode }
    exit 1
}

function Get-HealthCode([string]$Url) {
    $hasPreference = Test-Path variable:PSNativeCommandUseErrorActionPreference
    $preference = $false
    if ($hasPreference) {
        $preference = $PSNativeCommandUseErrorActionPreference
        $PSNativeCommandUseErrorActionPreference = $false
    }
    try {
        $code = & curl.exe -k -s -o NUL -w '%{http_code}' --max-time 5 $Url
        if (-not $code) { return '000' }
        return ([string]$code).Trim()
    }
    catch {
        return '000'
    }
    finally {
        if ($hasPreference) { $PSNativeCommandUseErrorActionPreference = $preference }
    }
}

function Invoke-Curl {
    param([string[]]$ArgumentList)
    $hasPreference = Test-Path variable:PSNativeCommandUseErrorActionPreference
    $preference = $false
    if ($hasPreference) {
        $preference = $PSNativeCommandUseErrorActionPreference
        $PSNativeCommandUseErrorActionPreference = $false
    }
    try {
        return (& curl.exe @ArgumentList)
    }
    finally {
        if ($hasPreference) { $PSNativeCommandUseErrorActionPreference = $preference }
    }
}

function Test-ZipFile([string]$Path) {
    if (-not (Test-Path $Path)) { return $false }
    $stream = [System.IO.File]::OpenRead($Path)
    try {
        if ($stream.Length -lt 4) { return $false }
        $first = $stream.ReadByte()
        $second = $stream.ReadByte()
        return ($first -eq 0x50 -and $second -eq 0x4B)
    }
    finally {
        $stream.Close()
    }
}

$today = (Get-Date).Date
$start = $today.AddYears(-5).ToString('yyyy-MM-dd')
$end = $today.AddMonths(6).ToString('yyyy-MM-dd')
$scenario = Join-Path $PSScriptRoot 'scenario\HotelDruid.Scenario.dll'
$dotnet = Join-Path $env:ProgramFiles 'dotnet\dotnet.exe'
if (-not (Test-Path $dotnet)) { throw "dotnet.exe was not found at $dotnet." }
if (-not (Test-Path $scenario)) { throw "HotelDruid.Scenario.dll was not found at $scenario." }

Write-Host "Demo range: $start through $end ($Rooms rooms, occupancy $Occupancy, seed $Seed)" -ForegroundColor Cyan

$healthUrl = $BaseUrl.TrimEnd('/') + '/health'
$stamp = Get-Date -Format 'yyyyMMdd-HHmmss'
$dest = Join-Path $env:TEMP "HotelDruid-demo-backup-$stamp"
New-Item -ItemType Directory -Path $dest -Force | Out-Null
$exportFile = Join-Path $dest 'hoteldruid-export.zip'
$health = Get-HealthCode $healthUrl
if ($health -ne '200') {
    throw "The desk did not answer at $healthUrl (HTTP $health). The current hotel was left in place."
}

Write-Host 'Writing the data export...' -ForegroundColor Cyan
$savedPath = ''
$fallback = ''
$exportError = ''
$createBody = Join-Path $dest 'create.json'
$code = Invoke-Curl -ArgumentList @('-k', '-s', '-S', '-m', '300', '-o', $createBody, '-w', '%{http_code}', '-X', 'POST', '-H', 'Content-Type: application/json', '-d', '{}', ($BaseUrl.TrimEnd('/') + '/api/export/create'))
if (([string]$code).Trim() -eq '200' -and (Test-Path $createBody)) {
    $created = Get-Content -Path $createBody -Raw | ConvertFrom-Json
    $exportId = [string]$created.ExportId
    $status = [string]$created.Status
    $deadline = (Get-Date).AddMinutes(5)
    while ($status -ne 'completed' -and $status -ne 'failed' -and (Get-Date) -lt $deadline) {
        Start-Sleep -Seconds 2
        $statusBody = Join-Path $dest 'status.json'
        Invoke-Curl -ArgumentList @('-k', '-s', '-S', '-m', '30', '-o', $statusBody, ($BaseUrl.TrimEnd('/') + "/api/export/$exportId/status")) | Out-Null
        $statusJson = Get-Content -Path $statusBody -Raw | ConvertFrom-Json
        $status = [string]$statusJson.Status
    }
    if ($status -eq 'completed') {
        Invoke-Curl -ArgumentList @('-k', '-s', '-S', '-m', '300', '-o', $exportFile, ($BaseUrl.TrimEnd('/') + "/api/export/$exportId/download")) | Out-Null
        if (Test-ZipFile $exportFile) { $savedPath = $exportFile }
        else { $exportError = 'The export download was not a zip file.' }
    }
    else {
        $exportError = "Export status was '$status'."
    }
}
else {
    $exportError = "Export request returned HTTP $code."
}

if (-not $savedPath) {
    $log = Join-Path $env:ProgramData 'HotelDruid\logs\api.log'
    if (Test-Path $log) {
        $reason = @(Get-Content -Path $log -Tail 60 | Where-Object { $_ -match 'Exception|denied|Export .+ failed' } | Select-Object -Last 1)
        if ($reason.Count -eq 1) { $exportError = ($exportError + ' ' + $reason[0].Trim()).Trim() }
    }
    Write-Host "The export API did not produce a zip. $exportError" -ForegroundColor Yellow
    if (-not (Test-Path $DataRoot)) {
        throw "There is no hotel folder to copy. The current hotel was left in place."
    }
    Write-Host 'Copying the hotel folder instead...' -ForegroundColor Yellow
    $fallback = Join-Path $dest 'data'
    New-Item -ItemType Directory -Path $fallback -Force | Out-Null
    Copy-Item -Path (Join-Path $DataRoot '*') -Destination $fallback -Recurse -Force
    $copied = @(Get-ChildItem -Path $fallback -Recurse -File -ErrorAction SilentlyContinue).Count
    $sourceCount = @(Get-ChildItem -Path $DataRoot -Recurse -File -ErrorAction SilentlyContinue).Count
    if ($sourceCount -gt 0 -and $copied -eq 0) {
        throw "The hotel folder copy is empty. The current hotel was left in place."
    }
    $savedPath = $fallback
}

$note = [ordered]@{
    exportedAt = (Get-Date).ToString('o')
    user = [Environment]::UserName
    machine = [Environment]::MachineName
    dataDir = $DataRoot
    exportFile = $(if ($savedPath -like '*.zip') { $savedPath } else { '' })
    fallbackCopy = $fallback
    exportError = $exportError
    start = $start
    end = $end
    rooms = $Rooms
    occupancy = $Occupancy
    seed = $Seed
}
$note | ConvertTo-Json | Set-Content -Path (Join-Path $dest 'demo-backup.json') -Encoding UTF8
Write-Host "Export: $savedPath" -ForegroundColor Green

Write-Host 'Stopping the HotelDruid service...' -ForegroundColor Yellow
Stop-ScheduledTask -TaskName $TaskName -ErrorAction SilentlyContinue
$stopped = (Get-Date).AddSeconds(20)
do {
    $state = (Get-ScheduledTask -TaskName $TaskName -ErrorAction SilentlyContinue).State
    if ($state -ne 'Running') { break }
    Start-Sleep -Seconds 1
} while ((Get-Date) -lt $stopped)

$appDir = Join-Path $env:ProgramData 'HotelDruid\app'
$parents = @(Get-CimInstance Win32_Process -Filter "Name = 'cmd.exe'" | Where-Object { [string]$_.CommandLine -like "*$appDir*" })
foreach ($parent in $parents) {
    Get-CimInstance Win32_Process | Where-Object { $_.ParentProcessId -eq $parent.ProcessId -and [string]$_.CommandLine -match 'HotelDruid\.Api\.dll' } | ForEach-Object {
        Stop-Process -Id $_.ProcessId -Force -ErrorAction SilentlyContinue
    }
    Stop-Process -Id $parent.ProcessId -Force -ErrorAction SilentlyContinue
}
Start-Sleep -Seconds 2

$replaced = $false
try {
    if ((Get-HealthCode $healthUrl) -eq '200') {
        throw 'The desk is still answering, so the data was not replaced.'
    }
    Write-Host "Replacing $DataRoot" -ForegroundColor Cyan
    $occupancyText = $Occupancy.ToString([System.Globalization.CultureInfo]::InvariantCulture)
    $hasPreference = Test-Path variable:PSNativeCommandUseErrorActionPreference
    $preference = $false
    if ($hasPreference) {
        $preference = $PSNativeCommandUseErrorActionPreference
        $PSNativeCommandUseErrorActionPreference = $false
    }
    try {
        & $dotnet $scenario --data-root $DataRoot --replace --start $start --end $end --rooms $Rooms --occupancy $occupancyText --seed $Seed
        if ($LASTEXITCODE -ne 0) { throw "The demo generator exited with code $LASTEXITCODE." }
    }
    finally {
        if ($hasPreference) { $PSNativeCommandUseErrorActionPreference = $preference }
    }
    $replaced = $true
}
finally {
    Write-Host 'Starting the HotelDruid service...' -ForegroundColor Cyan
    Start-ScheduledTask -TaskName $TaskName
}

$ready = $false
$waitUntil = (Get-Date).AddSeconds(45)
do {
    if ((Get-HealthCode $healthUrl) -eq '200') { $ready = $true; break }
    Start-Sleep -Seconds 1
} while ((Get-Date) -lt $waitUntil)

if ($ready) {
    Write-Host "HotelDruid is running at $($BaseUrl.TrimEnd('/'))/" -ForegroundColor Green
}
else {
    Write-Host 'The demo was written. The desk did not answer yet.' -ForegroundColor Yellow
    Write-Host "Log: $(Join-Path $env:ProgramData 'HotelDruid\logs\api.log')"
}
Write-Host "Export: $savedPath"
if (-not $replaced) { exit 1 }
