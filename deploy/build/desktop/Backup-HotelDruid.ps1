<#
.SYNOPSIS
  Ask the running desk for an export and save the zip.

.DESCRIPTION
  Calls POST /api/export/create, downloads the zip, and writes it to -Destination.
  Deletes export zips and daily api-yyyyMMdd.log files older than -KeepDays.
#>
param(
    [Parameter(Mandatory = $true)]
    [string]$Destination,
    [string]$BaseUrl = 'https://localhost:5001/',
    [int]$KeepDays = 30,
    [string]$LogDir = $(Join-Path $env:ProgramData 'HotelDruid\logs')
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Get-HealthCode([string]$Url) {
    try {
        $code = & curl.exe -k -s -o NUL -w '%{http_code}' --max-time 5 $Url
        if (-not $code) { return '000' }
        return ([string]$code).Trim()
    }
    catch {
        return '000'
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
        return ($stream.ReadByte() -eq 0x50 -and $stream.ReadByte() -eq 0x4B)
    }
    finally {
        $stream.Close()
    }
}

function Write-BackupLog([string]$Message) {
    if (-not (Test-Path $LogDir)) { New-Item -ItemType Directory -Path $LogDir -Force | Out-Null }
    $line = '{0} {1}' -f (Get-Date).ToString('o'), $Message
    Add-Content -Path (Join-Path $LogDir 'backup.log') -Value $line -Encoding UTF8
    Write-Host $line
}

function Remove-OlderThan([string]$Folder, [string]$Filter, [datetime]$Cutoff) {
    if (-not (Test-Path $Folder)) { return }
    Get-ChildItem -Path $Folder -Filter $Filter -File -ErrorAction SilentlyContinue | ForEach-Object {
        if ($_.LastWriteTime -lt $Cutoff) {
            Remove-Item -Force $_.FullName
            Write-BackupLog "Pruned $($_.FullName)"
        }
    }
}

try {
    if (-not (Test-Path $Destination)) {
        New-Item -ItemType Directory -Path $Destination -Force | Out-Null
    }
    $healthUrl = $BaseUrl.TrimEnd('/') + '/health'
    $health = Get-HealthCode $healthUrl
    if ($health -ne '200') {
        throw "The desk did not answer at $healthUrl (HTTP $health)."
    }

    $stamp = Get-Date -Format 'yyyyMMdd-HHmmss'
    $work = Join-Path $env:TEMP "HotelDruid-backup-$stamp"
    New-Item -ItemType Directory -Path $work -Force | Out-Null
    $createBody = Join-Path $work 'create.json'
    $code = Invoke-Curl -ArgumentList @('-k', '-s', '-S', '-m', '300', '-o', $createBody, '-w', '%{http_code}', '-X', 'POST', '-H', 'Content-Type: application/json', '-d', '{}', ($BaseUrl.TrimEnd('/') + '/api/export/create'))
    if (([string]$code).Trim() -ne '200' -or -not (Test-Path $createBody)) {
        throw "Export request returned HTTP $code."
    }
    $created = Get-Content -Path $createBody -Raw | ConvertFrom-Json
    $exportId = [string]$created.ExportId
    $status = [string]$created.Status
    $deadline = (Get-Date).AddMinutes(10)
    while ($status -ne 'completed' -and $status -ne 'failed' -and (Get-Date) -lt $deadline) {
        Start-Sleep -Seconds 2
        $statusBody = Join-Path $work 'status.json'
        Invoke-Curl -ArgumentList @('-k', '-s', '-S', '-m', '30', '-o', $statusBody, ($BaseUrl.TrimEnd('/') + "/api/export/$exportId/status")) | Out-Null
        $status = [string](Get-Content -Path $statusBody -Raw | ConvertFrom-Json).Status
    }
    if ($status -ne 'completed') {
        throw "Export status was '$status'."
    }

    $zip = Join-Path $Destination "hoteldruid-$stamp.zip"
    Invoke-Curl -ArgumentList @('-k', '-s', '-S', '-m', '300', '-o', $zip, ($BaseUrl.TrimEnd('/') + "/api/export/$exportId/download")) | Out-Null
    if (-not (Test-ZipFile $zip)) {
        if (Test-Path $zip) { Remove-Item -Force $zip }
        throw 'The export download was not a zip file.'
    }

    $cutoff = (Get-Date).AddDays(-1 * [Math]::Abs($KeepDays))
    Remove-OlderThan -Folder $Destination -Filter 'hoteldruid-*.zip' -Cutoff $cutoff
    Remove-OlderThan -Folder $LogDir -Filter 'api-*.log' -Cutoff $cutoff
    Write-BackupLog "Export: $zip"
    Remove-Item -Recurse -Force $work -ErrorAction SilentlyContinue
    exit 0
}
catch {
    Write-BackupLog $_.Exception.Message
    exit 1
}
