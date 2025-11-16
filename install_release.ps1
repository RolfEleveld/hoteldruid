<#
Installer script to run on the target machine after copying the release zip.
It extracts the zip to a chosen install folder and creates a Start Menu shortcut and optional Startup shortcut.
Usage (run as user):
  .\install_release.ps1 -ZipPath .\hoteldruid-release.zip -InstallDir 'C:\Program Files\HotelDruid'
#>
param(
    [string]$ZipPath = (Join-Path (Get-Location) 'hoteldruid-release.zip'),
    [string]$InstallDir = "$env:ProgramFiles\HotelDruid",
    [switch]$CreateStartMenuShortcut = $true,
    [switch]$CreateStartupShortcut = $false
)

if (-not (Test-Path -Path $ZipPath)) {
    Write-Error "Zip file not found: $ZipPath"
    exit 2
}

Write-Host "Installing from $ZipPath to $InstallDir"

# Create destination
if (-not (Test-Path -Path $InstallDir)) {
    New-Item -Path $InstallDir -ItemType Directory -Force | Out-Null
}

# Expand
Expand-Archive -Path $ZipPath -DestinationPath $InstallDir -Force

# Try to detect executable
$exe = Get-ChildItem -Path $InstallDir -Filter *.exe -Recurse -File | Select-Object -First 1
if (-not $exe) {
    Write-Warning "No .exe found automatically in $InstallDir. Please provide the path to the executable file."
    $exePath = Read-Host "Enter full path to the exe (or press Enter to skip)"
    if ($exePath) {
        if (-not (Test-Path $exePath)) { Write-Error "Provided exe path not found." } else { $exe = Get-Item -LiteralPath $exePath }
    }
}

if ($exe) {
    $targetExe = $exe.FullName
    Write-Host "Detected exe: $targetExe"

    # Create a Start Menu shortcut for current user
    if ($CreateStartMenuShortcut) {
        $smpFolder = Join-Path $env:AppData 'Microsoft\Windows\Start Menu\Programs'
        $linkFolder = Join-Path $smpFolder 'HotelDruid'
        if (-not (Test-Path -Path $linkFolder)) { New-Item -Path $linkFolder -ItemType Directory -Force | Out-Null }
        $linkPath = Join-Path $linkFolder 'HotelDruid.lnk'
        $ws = New-Object -ComObject WScript.Shell
        $shortcut = $ws.CreateShortcut($linkPath)
        $shortcut.TargetPath = $targetExe
        $shortcut.WorkingDirectory = Split-Path $targetExe -Parent
        $shortcut.IconLocation = $targetExe
        $shortcut.Save()
        Write-Host "Start Menu shortcut created: $linkPath"
    }

    if ($CreateStartupShortcut) {
        $startupFolder = Join-Path $env:AppData 'Microsoft\Windows\Start Menu\Programs\Startup'
        $linkPath2 = Join-Path $startupFolder 'HotelDruid.lnk'
        $ws2 = New-Object -ComObject WScript.Shell
        $shortcut2 = $ws2.CreateShortcut($linkPath2)
        $shortcut2.TargetPath = $targetExe
        $shortcut2.WorkingDirectory = Split-Path $targetExe -Parent
        $shortcut2.IconLocation = $targetExe
        $shortcut2.Save()
        Write-Host "Startup shortcut created: $linkPath2"
    }
} else {
    Write-Warning "No executable available to create shortcuts. You can create them manually pointing to the appropriate exe inside $InstallDir."
}

Write-Host "Installation finished."
Write-Host "If you need the app to auto-start for all users or create a service, run with elevated rights and adjust shortcut targets accordingly."