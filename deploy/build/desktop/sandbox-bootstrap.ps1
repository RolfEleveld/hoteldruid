<#
.SYNOPSIS
  Prepare a clean Windows computer and install HotelDruid when it is not already there.

.DESCRIPTION
  Runs inside the sandbox started by Start-Sandbox.ps1. The hotel files are already
  on disk through a mapped folder. This script installs Windows Package Manager,
  PowerShell 7, the .NET 10 host, and the ASP.NET Core 10 runtime, then installs
  the hotel. A second run on a machine that already has the hotel only opens it.
#>
param(
    [string]$AppDir = 'C:\HotelDruidApp',
    [string]$DesktopDir = 'C:\HotelDruidDesktop'
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$LogPath = Join-Path $env:USERPROFILE 'Desktop\HotelDruid-setup.log'

function Write-Step([string]$Message) {
    $line = '{0}  {1}' -f (Get-Date -Format 'HH:mm:ss'), $Message
    Write-Host $line
    Add-Content -Path $LogPath -Value $line -Encoding UTF8
}

function Test-IsAdministrator {
    $identity = [Security.Principal.WindowsIdentity]::GetCurrent()
    $principal = New-Object Security.Principal.WindowsPrincipal($identity)
    return $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
}

function Enter-Admin {
    if (Test-IsAdministrator) { return }
    Write-Step 'Approve the administrator prompt. That is the only click.'
    $arg = "-NoProfile -ExecutionPolicy Bypass -File `"$PSCommandPath`" -AppDir `"$AppDir`" -DesktopDir `"$DesktopDir`""
    $proc = Start-Process -FilePath "$env:SystemRoot\System32\WindowsPowerShell\v1.0\powershell.exe" -ArgumentList $arg -Verb RunAs -Wait -PassThru
    if ($proc) { exit $proc.ExitCode }
    exit 1
}

function Refresh-Path {
    $machine = [Environment]::GetEnvironmentVariable('Path', 'Machine')
    $user = [Environment]::GetEnvironmentVariable('Path', 'User')
    $env:Path = "$machine;$user"
}

function Get-WinGetExe {
    $command = Get-Command winget.exe -ErrorAction SilentlyContinue
    if ($command -and $command.Source -and (Test-Path $command.Source)) { return $command.Source }
    $candidates = @(
        (Join-Path $env:LOCALAPPDATA 'Microsoft\WindowsApps\winget.exe')
    )
    $apps = 'C:\Program Files\WindowsApps'
    if (Test-Path $apps) {
        $found = @(Get-ChildItem -Path $apps -Filter winget.exe -Recurse -ErrorAction SilentlyContinue | Select-Object -First 1)
        if ($found.Count -gt 0) { return $found[0].FullName }
    }
    foreach ($candidate in $candidates) {
        if (Test-Path $candidate) { return $candidate }
    }
    return $null
}

function Ensure-WinGet {
    $existing = Get-WinGetExe
    if ($existing) { return $existing }

    Write-Step 'Registering Windows Package Manager from this Windows image'
    try {
        Add-AppxPackage -RegisterByFamilyName -MainPackage Microsoft.DesktopAppInstaller_8wekyb3d8bbwe
    }
    catch {
        Write-Step 'This Windows image did not add its own package manager. The setup continues with the winget program if it is already present.'
    }
    Refresh-Path
    $existing = Get-WinGetExe
    if ($existing) { return $existing }

    Write-Step 'Installing Windows Package Manager from Microsoft'
    [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
    Install-PackageProvider -Name NuGet -MinimumVersion 2.8.5.201 -Force | Out-Null
    if ((Get-PSRepository -Name PSGallery).InstallationPolicy -ne 'Trusted') {
        Set-PSRepository -Name PSGallery -InstallationPolicy Trusted
    }
    Install-Module -Name Microsoft.WinGet.Client -Force -AllowClobber
    Repair-WinGetPackageManager -AllUsers
    Refresh-Path
    $existing = Get-WinGetExe
    if (-not $existing) { throw 'Windows Package Manager is still not available.' }
    return $existing
}

function Save-Download([string]$Url, [string]$Destination) {
    Write-Step "Downloading $Url"
    $hasPreference = Test-Path variable:PSNativeCommandUseErrorActionPreference
    $preference = $false
    if ($hasPreference) {
        $preference = $PSNativeCommandUseErrorActionPreference
        $PSNativeCommandUseErrorActionPreference = $false
    }
    try {
        & curl.exe -L --fail --retry 3 -o $Destination $Url
        $code = $LASTEXITCODE
    }
    finally {
        if ($hasPreference) { $PSNativeCommandUseErrorActionPreference = $preference }
    }
    if ($code -ne 0 -or -not (Test-Path $Destination)) { throw "Download failed: $Url" }
    $mb = [math]::Round((Get-Item $Destination).Length / 1MB, 1)
    Write-Step "Saved $mb MB to $Destination"
}

function Install-QuietExe([string]$Path) {
    Write-Step "Installing $Path"
    $proc = Start-Process -FilePath $Path -ArgumentList '/install', '/quiet', '/norestart' -Wait -PassThru
    if ($proc.ExitCode -ne 0 -and $proc.ExitCode -ne 3010 -and $proc.ExitCode -ne 1638) {
        throw "Installer exit $($proc.ExitCode): $Path"
    }
}

function Install-PowerShell7 {
    $pwsh = 'C:\Program Files\PowerShell\7\pwsh.exe'
    if (Test-Path $pwsh) {
        Write-Step 'PowerShell 7 is already installed.'
        return
    }
    $zip = Join-Path $env:TEMP 'PowerShell-7.7.0-win-x64.zip'
    Save-Download 'https://github.com/PowerShell/PowerShell/releases/download/v7.7.0/PowerShell-7.7.0-win-x64.zip' $zip
    $dest = 'C:\Program Files\PowerShell\7'
    if (Test-Path $dest) { Remove-Item -Recurse -Force $dest }
    Write-Step "Expanding PowerShell 7 to $dest"
    Expand-Archive -Path $zip -DestinationPath $dest -Force
    if (-not (Test-Path $pwsh)) { throw 'pwsh.exe was not in the PowerShell package.' }
    $machine = [Environment]::GetEnvironmentVariable('Path', 'Machine')
    if ($machine -notlike "*$dest*") {
        [Environment]::SetEnvironmentVariable('Path', "$machine;$dest", 'Machine')
    }
    Refresh-Path
    Write-Step (& $pwsh -NoProfile -Command '$PSVersionTable.PSVersion.ToString()')
}

function Install-DotNet10 {
    $dotnet = Join-Path $env:ProgramFiles 'dotnet\dotnet.exe'
    $runtimeZip = Join-Path $env:TEMP 'dotnet-runtime-win-x64.exe'
    $aspnetZip = Join-Path $env:TEMP 'aspnetcore-runtime-win-x64.exe'
    if (-not (Test-Path $dotnet)) {
        Save-Download 'https://aka.ms/dotnet/10.0/dotnet-runtime-win-x64.exe' $runtimeZip
        Install-QuietExe $runtimeZip
    }
    else {
        Write-Step 'The .NET host is already installed.'
    }
    Save-Download 'https://aka.ms/dotnet/10.0/aspnetcore-runtime-win-x64.exe' $aspnetZip
    Install-QuietExe $aspnetZip
    Refresh-Path
}

function Test-HotelInstalled {
    return Test-Path 'C:\ProgramData\HotelDruid\install-meta.json'
}

try {
    Start-Sleep -Seconds 5
    Enter-Admin
    if (-not (Test-Path $DesktopDir)) { $DesktopDir = $AppDir }
    New-Item -ItemType File -Path $LogPath -Force | Out-Null
    Write-Step "Hotel files: $AppDir"
    Write-Step "Scripts: $DesktopDir"

    if (Test-HotelInstalled) {
        Write-Step 'HotelDruid is already installed. Opening it.'
        & (Join-Path 'C:\ProgramData\HotelDruid\app' 'HotelDruid.ps1') -Action Start
        Write-Step 'Done.'
        Read-Host 'Press Enter to close'
        exit 0
    }

    if (-not (Test-Path (Join-Path $AppDir 'HotelDruid.Api.dll'))) {
        throw "HotelDruid.Api.dll was not found in $AppDir."
    }

    $winget = Ensure-WinGet
    Write-Step "Package manager: $winget"
    Write-Step 'The package catalog returned a Rest API internal error on the machine-wide install, so PowerShell 7 and .NET are installed from Microsoft directly.'
    Install-PowerShell7
    Install-DotNet10

    $dotnet = Join-Path $env:ProgramFiles 'dotnet\dotnet.exe'
    if (-not (Test-Path $dotnet)) { throw "The .NET host was not installed at $dotnet." }
    $runtimes = & $dotnet --list-runtimes
    $runtimes | ForEach-Object { Write-Step $_ }
    if (-not ($runtimes | Where-Object { $_ -match 'Microsoft\.AspNetCore\.App 10\.' })) {
        throw 'Microsoft.AspNetCore.App 10 is not installed.'
    }

    $stage = 'C:\HotelDruidStage'
    if (Test-Path $stage) { Remove-Item -Recurse -Force $stage }
    New-Item -ItemType Directory -Path $stage | Out-Null
    Write-Step 'Copying the hotel into the sandbox'
    $hasPreference = Test-Path variable:PSNativeCommandUseErrorActionPreference
    if ($hasPreference) { $PSNativeCommandUseErrorActionPreference = $false }
    & robocopy $AppDir $stage /E /NFL /NDL /NJH /NJS /nc /ns /np /XD data logs | Out-Null
    if ($LASTEXITCODE -ge 8) { throw "Copying the hotel failed (robocopy $LASTEXITCODE)." }
    Copy-Item -Path (Join-Path $DesktopDir '*') -Destination $stage -Force
    if (-not (Test-Path (Join-Path $stage 'HotelDruid.ps1'))) {
        throw 'HotelDruid.ps1 was not copied next to the hotel.'
    }

    Write-Step 'Installing HotelDruid for this computer'
    & (Join-Path $stage 'HotelDruid.ps1') -Action Install
    Write-Step 'HotelDruid is installed.'
    Read-Host 'Press Enter to close'
    exit 0
}
catch {
    Write-Step $_.Exception.Message
    Write-Host $_.Exception.Message -ForegroundColor Red
    Write-Host "Log: $LogPath"
    Read-Host 'Press Enter to close'
    exit 1
}
