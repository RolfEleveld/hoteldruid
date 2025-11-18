# HotelDruid PHP Desktop Launcher
# Starts HotelDruid in PHP Desktop application

$phpDesktopExe = Join-Path $PSScriptRoot "phpdesktop\phpdesktop-chrome.exe"
$hoteldruidDir = Join-Path $PSScriptRoot "hoteldruid"

if (-not (Test-Path $phpDesktopExe)) {
    Write-Host "❌ PHP Desktop executable not found: $phpDesktopExe" -ForegroundColor Red
    Write-Host "   Please run setup-phpdesktop.ps1 first" -ForegroundColor Yellow
    exit 1
}

if (-not (Test-Path $hoteldruidDir)) {
    Write-Host "❌ HotelDruid directory not found: $hoteldruidDir" -ForegroundColor Red
    exit 1
}

Write-Host "🚀 Starting HotelDruid..." -ForegroundColor Green
Start-Process -FilePath $phpDesktopExe -WorkingDirectory $hoteldruidDir
