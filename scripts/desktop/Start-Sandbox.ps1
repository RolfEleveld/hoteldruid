<#
.SYNOPSIS
  Start a Windows Sandbox with the built hotel mapped in, and let it deploy itself.

.DESCRIPTION
  Writes a sandbox configuration that maps the built application and these scripts,
  then starts Windows Sandbox. Inside the sandbox, sandbox-bootstrap.ps1 installs
  Windows Package Manager, PowerShell 7, the .NET 10 host, and the ASP.NET Core 10
  runtime, and installs HotelDruid when it is not already installed.

  Close an already open sandbox first. A running sandbox cannot gain this folder.

.EXAMPLE
  pwsh -NoProfile -File .\scripts\desktop\Start-Sandbox.ps1
#>
param(
    [string]$AppDir = '',
    [string]$DesktopDir = '',
    [switch]$PrepareOnly
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Test-SandboxRunning {
    $names = @('WindowsSandbox', 'WindowsSandboxClient', 'WindowsSandboxServer', 'WindowsSandboxRemoteSession', 'vmmemWindowsSandbox')
    $running = @(Get-Process -Name $names -ErrorAction SilentlyContinue)
    return $running.Count -gt 0
}

$here = $PSScriptRoot
if ([string]::IsNullOrWhiteSpace($DesktopDir)) { $DesktopDir = $here }
if ([string]::IsNullOrWhiteSpace($AppDir)) {
    $beside = Join-Path $here 'HotelDruid.Api.dll'
    if (Test-Path $beside) {
        $AppDir = $here
    }
    else {
        $AppDir = Join-Path (Join-Path (Split-Path -Parent $here) '..') 'artifacts\api'
    }
}

$AppDir = (Resolve-Path $AppDir).Path
$DesktopDir = (Resolve-Path $DesktopDir).Path
$dll = Join-Path $AppDir 'HotelDruid.Api.dll'
$bootstrap = Join-Path $DesktopDir 'sandbox-bootstrap.ps1'
if (-not (Test-Path $dll)) { throw "HotelDruid.Api.dll was not found in $AppDir. Build the package, or pass -AppDir." }
if (-not (Test-Path $bootstrap)) { throw "sandbox-bootstrap.ps1 was not found in $DesktopDir." }
if (-not (Test-Path "$env:SystemRoot\System32\WindowsSandbox.exe")) { throw 'Windows Sandbox is not installed on this computer.' }

$sameFolder = ($AppDir -eq $DesktopDir)
$sandboxApp = 'C:\HotelDruidApp'
$sandboxDesktop = $(if ($sameFolder) { 'C:\HotelDruidApp' } else { 'C:\HotelDruidDesktop' })

$wsbDir = Join-Path ([System.IO.Path]::GetTempPath()) 'HotelDruid-sandbox'
New-Item -ItemType Directory -Path $wsbDir -Force | Out-Null
$wsbPath = Join-Path $wsbDir 'HotelDruid.wsb'

$folders = @"
    <MappedFolder>
      <HostFolder>$AppDir</HostFolder>
      <SandboxFolder>$sandboxApp</SandboxFolder>
      <ReadOnly>true</ReadOnly>
    </MappedFolder>
"@
if (-not $sameFolder) {
    $folders += @"

    <MappedFolder>
      <HostFolder>$DesktopDir</HostFolder>
      <SandboxFolder>$sandboxDesktop</SandboxFolder>
      <ReadOnly>true</ReadOnly>
    </MappedFolder>
"@
}

$command = "cmd.exe /c start `"HotelDruid setup`" C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe -NoExit -NoProfile -ExecutionPolicy Bypass -File $sandboxDesktop\sandbox-bootstrap.ps1 -AppDir $sandboxApp -DesktopDir $sandboxDesktop"
$wsb = @"
<Configuration>
  <Networking>Enable</Networking>
  <MappedFolders>
$folders
  </MappedFolders>
  <LogonCommand>
    <Command>$command</Command>
  </LogonCommand>
</Configuration>
"@
Set-Content -Path $wsbPath -Value $wsb -Encoding Unicode

Write-Host "Hotel:  $AppDir"
Write-Host "Scripts: $DesktopDir"
Write-Host "Sandbox file: $wsbPath"
if ($PrepareOnly) { return }

if (Test-SandboxRunning) {
    Write-Host 'A Windows Sandbox is already open. Close it, then run this script again. The open sandbox has no deploy folder.' -ForegroundColor Yellow
    exit 1
}

Write-Host 'Starting Windows Sandbox. Approve the one administrator prompt inside it.' -ForegroundColor Cyan
Start-Process -FilePath "$env:SystemRoot\System32\WindowsSandbox.exe" -ArgumentList "`"$wsbPath`""
