<#
.SYNOPSIS
  Inside Windows Sandbox, copy the self-contained hotel and start it.

.DESCRIPTION
  The mapped folder is read-only. This copies it to C:\HotelDruid, starts
  Start-HotelDruid.cmd, and writes the health result where the host can read it.
  It does not install a runtime.
#>
param(
    [string]$Source = 'C:\HotelDruidApp',
    [string]$ResultDir = 'C:\HotelDruidResult'
)

$ErrorActionPreference = 'Stop'
New-Item -ItemType Directory -Force -Path $ResultDir | Out-Null
$log = Join-Path $ResultDir 'setup.log'

function Write-Step([string]$Message) {
    $line = '{0}  {1}' -f (Get-Date -Format 'HH:mm:ss'), $Message
    Write-Host $line
    Add-Content -Path $log -Value $line -Encoding UTF8
}

try {
    $dest = 'C:\HotelDruid'
    Write-Step "Copying $Source to $dest"
    if (Test-Path $dest) { Remove-Item -Recurse -Force $dest }
    New-Item -ItemType Directory -Path $dest | Out-Null
    & robocopy.exe $Source $dest /E /NFL /NDL /NJH /NJS /nc /ns /np | Out-Null
    if ($LASTEXITCODE -ge 8) { throw "Copying the hotel failed (robocopy $LASTEXITCODE)." }

    $dotnet = Get-Command dotnet.exe -ErrorAction SilentlyContinue
    if ($dotnet) { Write-Step "dotnet is present at $($dotnet.Source). The copy package does not call it." }
    else { Write-Step 'dotnet is not installed.' }

    $cmd = Join-Path $dest 'Start-HotelDruid.cmd'
    if (-not (Test-Path $cmd)) { throw "Start-HotelDruid.cmd was not copied to $dest." }
    Write-Step 'Starting HotelDruid.Api.exe'
    Start-Process -FilePath $cmd -WorkingDirectory $dest

    $healthPath = Join-Path $ResultDir 'health.txt'
    $ok = $false
    for ($i = 0; $i -lt 60; $i++) {
        Start-Sleep -Seconds 1
        try {
            $response = Invoke-WebRequest -Uri 'http://127.0.0.1:8080/health' -UseBasicParsing -TimeoutSec 2
            if ($response.StatusCode -eq 200) {
                Set-Content -Path $healthPath -Value $response.Content -Encoding UTF8
                $ok = $true
                break
            }
        }
        catch { }
    }
    if (-not $ok) { throw 'The desk did not answer http://127.0.0.1:8080/health.' }

    Write-Step "Healthy: $((Get-Content -Path $healthPath -Raw).Trim())"
    Start-Process 'http://127.0.0.1:8080/'
    Set-Content -Path (Join-Path $ResultDir 'result.txt') -Value 'ok' -Encoding UTF8
    Write-Step 'The desk is open at http://127.0.0.1:8080/.'
}
catch {
    Write-Step $_.Exception.Message
    Set-Content -Path (Join-Path $ResultDir 'result.txt') -Value $_.Exception.Message -Encoding UTF8
    exit 1
}
