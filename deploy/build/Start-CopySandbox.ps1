<#
.SYNOPSIS
  Open the self-contained Windows hotel in Windows Sandbox.

.DESCRIPTION
  Maps artifacts\selfcontained\win-x64 into a sandbox with networking off,
  copies it to a writable folder inside the sandbox, and starts Start-HotelDruid.cmd.
  The sandbox cannot download a runtime. Close an already open sandbox first.

.EXAMPLE
  ./deploy/build/Start-CopySandbox.ps1
#>
param(
    [string]$AppDir = '',
    [switch]$PrepareOnly
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Test-SandboxRunning {
    $names = @('WindowsSandbox', 'WindowsSandboxClient', 'WindowsSandboxServer', 'WindowsSandboxRemoteSession', 'vmmemWindowsSandbox')
    $running = @(Get-Process -Name $names -ErrorAction SilentlyContinue)
    return $running.Count -gt 0
}

$root = Resolve-Path (Join-Path $PSScriptRoot '../..')
if ([string]::IsNullOrWhiteSpace($AppDir)) {
    $AppDir = Join-Path $root 'artifacts\selfcontained\win-x64'
}
$AppDir = (Resolve-Path $AppDir).Path
$scriptDir = Join-Path $PSScriptRoot 'selfcontained'
$bootstrap = Join-Path $scriptDir 'sandbox-selfcontained.ps1'
$exe = Join-Path $AppDir 'HotelDruid.Api.exe'
$starter = Join-Path $AppDir 'Start-HotelDruid.cmd'
if (-not (Test-Path $exe)) { throw "HotelDruid.Api.exe was not found in $AppDir. Run package-selfcontained.ps1 first." }
if (-not (Test-Path $starter)) { throw "Start-HotelDruid.cmd was not found in $AppDir." }
if (-not (Test-Path $bootstrap)) { throw "sandbox-selfcontained.ps1 was not found." }
if (-not (Test-Path "$env:SystemRoot\System32\WindowsSandbox.exe")) { throw 'Windows Sandbox is not installed on this computer.' }

$resultDir = Join-Path ([System.IO.Path]::GetTempPath()) 'HotelDruid-copy-sandbox'
if (Test-Path $resultDir) { Remove-Item -Recurse -Force $resultDir }
New-Item -ItemType Directory -Path $resultDir | Out-Null

$wsbPath = Join-Path $resultDir 'HotelDruid-copy.wsb'
$command = "C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe -NoExit -NoProfile -ExecutionPolicy Bypass -File C:\HotelDruidScripts\sandbox-selfcontained.ps1 -Source C:\HotelDruidApp -ResultDir C:\HotelDruidResult"
$wsb = @"
<Configuration>
  <Networking>Disable</Networking>
  <MappedFolders>
    <MappedFolder>
      <HostFolder>$AppDir</HostFolder>
      <SandboxFolder>C:\HotelDruidApp</SandboxFolder>
      <ReadOnly>true</ReadOnly>
    </MappedFolder>
    <MappedFolder>
      <HostFolder>$scriptDir</HostFolder>
      <SandboxFolder>C:\HotelDruidScripts</SandboxFolder>
      <ReadOnly>true</ReadOnly>
    </MappedFolder>
    <MappedFolder>
      <HostFolder>$resultDir</HostFolder>
      <SandboxFolder>C:\HotelDruidResult</SandboxFolder>
      <ReadOnly>false</ReadOnly>
    </MappedFolder>
  </MappedFolders>
  <LogonCommand>
    <Command>$command</Command>
  </LogonCommand>
</Configuration>
"@
Set-Content -Path $wsbPath -Value $wsb -Encoding Unicode

Write-Host "Hotel:  $AppDir"
Write-Host "Result: $resultDir"
Write-Host "Sandbox file: $wsbPath"
if ($PrepareOnly) { return }

if (Test-SandboxRunning) {
    Write-Host 'A Windows Sandbox is already open. Close it, then run this script again.' -ForegroundColor Yellow
    exit 1
}

Write-Host 'Starting Windows Sandbox with networking off.' -ForegroundColor Cyan
Start-Process -FilePath "$env:SystemRoot\System32\WindowsSandbox.exe" -ArgumentList "`"$wsbPath`""
