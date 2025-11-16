#!/usr/bin/env pwsh

# HotelDruid PHP Desktop Setup Script
# Downloads and configures PHP Desktop for Windows 10/11

Write-Host "=== HotelDruid PHP Desktop Setup ===" -ForegroundColor Green
Write-Host ""

$ErrorActionPreference = "Stop"

# Configuration
# Function to fetch the latest PHP Desktop release
function Get-LatestPhpDesktopRelease {
    $releasesUrl = "https://github.com/cztomczak/phpdesktop/releases"
    Write-Host "🌐 Fetching latest PHP Desktop release from: $releasesUrl" -ForegroundColor Yellow

    try {
        $html = Invoke-WebRequest -Uri $releasesUrl -UseBasicParsing
        $latestRelease = ($html.Links | Where-Object { $_.href -match "phpdesktop-chrome.*.zip" }) |
                         Select-Object -First 1

        if ($latestRelease -and $latestRelease.href) {
            $latestUrl = "https://github.com" + $latestRelease.href
            Write-Host "✅ Latest release found: $latestUrl" -ForegroundColor Green
            return $latestUrl
        } else {
            Write-Host "❌ Could not find a valid PHP Desktop release link." -ForegroundColor Red
            return $null
        }
    } catch {
        Write-Host "❌ Failed to fetch releases: $($_.Exception.Message)" -ForegroundColor Red
        return $null
    }
}

# Fetch the latest release URL dynamically
$phpDesktopUrl = Get-LatestPhpDesktopRelease
if (-not $phpDesktopUrl) {
    Write-Host "⚠️ Falling back to hardcoded URL." -ForegroundColor Yellow
    $phpDesktopUrl = "https://github.com/cztomczak/phpdesktop/releases/download/chrome-v130.1/phpdesktop-chrome-130.1-php-8.3.zip"
}

# Extract version from URL
$phpDesktopVersion = ($phpDesktopUrl -split "-")[1]
$phpDesktopDir = Join-Path $PSScriptRoot "phpdesktop"
$hoteldruidDir = Join-Path $PSScriptRoot "hoteldruid"
$phpDesktopZip = Join-Path $PSScriptRoot "phpdesktop.zip"

# Check if running on Windows
if ($PSVersionTable.PSVersion.Major -lt 5 -or -not $IsWindows) {
    if ($IsWindows -eq $null) {
        # PowerShell 5.1 or earlier - assume Windows
    } else {
        Write-Host "❌ This script is designed for Windows 10/11" -ForegroundColor Red
        exit 1
    }
}

Write-Host "📋 Configuration:" -ForegroundColor Cyan
Write-Host "   PHP Desktop Version: $phpDesktopVersion" -ForegroundColor White
Write-Host "   Installation Directory: $phpDesktopDir" -ForegroundColor White
Write-Host "   HotelDruid Directory: $hoteldruidDir" -ForegroundColor White
Write-Host ""

# Step 1: Check if HotelDruid directory exists
if (-not (Test-Path $hoteldruidDir)) {
    Write-Host "❌ HotelDruid directory not found: $hoteldruidDir" -ForegroundColor Red
    exit 1
}
Write-Host "✅ HotelDruid directory found" -ForegroundColor Green

# Step 2: Download PHP Desktop if not already present
if (Test-Path $phpDesktopDir) {
    Write-Host "📦 PHP Desktop directory already exists" -ForegroundColor Yellow
    Write-Host "   Skipping download. Delete '$phpDesktopDir' to re-download." -ForegroundColor Gray
} else {
    Write-Host "📥 Downloading PHP Desktop..." -ForegroundColor Yellow
    
    try {
        # Check if zip file already exists
        if (Test-Path $phpDesktopZip) {
            Write-Host "   Found existing zip file, using it..." -ForegroundColor Gray
        } else {
            Write-Host "   Downloading from: $phpDesktopUrl" -ForegroundColor Gray
            Invoke-WebRequest -Uri $phpDesktopUrl -OutFile $phpDesktopZip -UseBasicParsing
            Write-Host "✅ Download complete" -ForegroundColor Green
        }
        
        # Extract PHP Desktop
        Write-Host "📂 Extracting PHP Desktop..." -ForegroundColor Yellow
        Expand-Archive -Path $phpDesktopZip -DestinationPath $PSScriptRoot -Force
        Write-Host "✅ Extraction complete" -ForegroundColor Green
        
        # Waiting a bit to allow file system to ctach up
        Start-Sleep -Seconds 10
        
        # Find the extracted directory (it might have a version-specific name)
        $extractedDirs = Get-ChildItem -Path $PSScriptRoot -Directory | Where-Object { $_.Name -like "phpdesktop*" }
        if ($extractedDirs.Count -gt 0) {
            $extractedDir = $extractedDirs[0].FullName
            if ($extractedDir -ne $phpDesktopDir) {
                Rename-Item -Path $extractedDir -NewName "phpdesktop" -Force
            }
        }
        
        # Clean up zip file
        if (Test-Path $phpDesktopZip) {
            Remove-Item $phpDesktopZip -Force
            Write-Host "🧹 Cleaned up download file" -ForegroundColor Gray
        }
    } catch {
        Write-Host "❌ Failed to download PHP Desktop: $($_.Exception.Message)" -ForegroundColor Red
        Write-Host "   You can manually download from: https://github.com/cztomczak/phpdesktop/releases" -ForegroundColor Yellow
        exit 1
    }
}

# Step 3: Verify PHP Desktop structure
$phpExe = Join-Path $phpDesktopDir "php\php.exe"
$settingsIni = Join-Path $phpDesktopDir "settings.json"

if (-not (Test-Path $phpExe)) {
    Write-Host "❌ PHP executable not found: $phpExe" -ForegroundColor Red
    Write-Host "   PHP Desktop may not have extracted correctly" -ForegroundColor Yellow
    exit 1
}
Write-Host "✅ PHP Desktop structure verified" -ForegroundColor Green

# Step 4: Check PHP extensions
Write-Host "🔍 Checking PHP extensions..." -ForegroundColor Yellow
$phpCheck = & $phpExe -m
$requiredExtensions = @("sqlite3", "session", "json", "mbstring")

$missingExtensions = @()
foreach ($ext in $requiredExtensions) {
    if ($phpCheck -notmatch $ext) {
        $missingExtensions += $ext
    }
}

if ($missingExtensions.Count -gt 0) {
    Write-Host "⚠️  Missing PHP extensions: $($missingExtensions -join ', ')" -ForegroundColor Yellow
    Write-Host "   These may need to be enabled in php.ini" -ForegroundColor Gray
} else {
    Write-Host "✅ All required PHP extensions are available" -ForegroundColor Green
}

# Step 5: Configure PHP Desktop settings
Write-Host "⚙️  Configuring PHP Desktop..." -ForegroundColor Yellow

$settingsPath = Join-Path $phpDesktopDir "settings.json"
if (-not (Test-Path $settingsPath)) {
    Write-Host "   Creating settings.json..." -ForegroundColor Gray
    
    $settings = @{
        "application" = @{
            "name" = "HotelDruid"
            "version" = "3.0.7"
            "main_window" = @{
                "title" = "HotelDruid - Hotel Management System"
                "icon" = Join-Path $hoteldruidDir "img\favicon.ico"
                "width" = 1920
                "height" = 1080
                "min_width" = 1024
                "min_height" = 600
                "maximized" = $false
                "resizable" = $true
            }
        }
        "php" = @{
            "php_version" = "8.1"
            "extensions" = @("sqlite3", "session", "json", "mbstring", "pdo", "pdo_sqlite")
        }
        "web_server" = @{
            "listen_on" = @("127.0.0.1", "8080")
            "document_root" = $hoteldruidDir
            "index_files" = @("inizio.php", "index.html", "index.php")
        }
    }
    
    $settings | ConvertTo-Json -Depth 10 | Set-Content $settingsPath -Encoding UTF8
    Write-Host "✅ Settings file created" -ForegroundColor Green
} else {
    Write-Host "   Settings file already exists, skipping..." -ForegroundColor Gray
}

# Step 6: Create server.php router if it doesn't exist
$serverPhp = Join-Path $hoteldruidDir "server.php"
if (-not (Test-Path $serverPhp)) {
    Write-Host "📝 Creating server.php router..." -ForegroundColor Yellow
    
    $serverPhpContent = @'
<?php
// Router for PHP built-in server
// Handles security and routing for HotelDruid

$requestUri = $_SERVER['REQUEST_URI'];
$requestPath = parse_url($requestUri, PHP_URL_PATH);
$filePath = __DIR__ . $requestPath;

// Security: Block direct access to includes/themes PHP files
if (preg_match('#/(includes|themes/[^/]+/php)/.*\.php$#', $requestPath)) {
    // Allow CSS/JS files
    if (preg_match('#\.(css|js)$#', $requestPath)) {
        return false; // Let PHP server handle it
    }
    http_response_code(403);
    die('Access denied');
}

// Serve static files
if (file_exists($filePath) && !is_dir($filePath)) {
    return false; // Let PHP server handle static files
}

// Route to index or inizio.php
if ($requestPath === '/' || $requestPath === '') {
    $_SERVER['SCRIPT_NAME'] = '/inizio.php';
    require __DIR__ . '/inizio.php';
    return true;
}

// Route PHP files
if (file_exists($filePath) && is_file($filePath)) {
    return false; // Let PHP server execute it
}

// 404
http_response_code(404);
die('File not found');
'@
    
    Set-Content -Path $serverPhp -Value $serverPhpContent -Encoding UTF8
    Write-Host "✅ Router script created" -ForegroundColor Green
} else {
    Write-Host "   Router script already exists, skipping..." -ForegroundColor Gray
}

# Step 7: Update costanti.php to prefer SQLite
Write-Host "⚙️  Updating configuration for SQLite..." -ForegroundColor Yellow
$costantiPhp = Join-Path $hoteldruidDir "includes\costanti.php"
if (Test-Path $costantiPhp) {
    $costantiContent = Get-Content $costantiPhp -Raw
    
    # Check if SQLite is already the default
    if ($costantiContent -match "C_CREADB_TIPODB.*sqlite") {
        Write-Host "   SQLite is already configured as default" -ForegroundColor Gray
    } else {
        Write-Host "   Note: SQLite will be auto-selected if SQLite3 class is available" -ForegroundColor Gray
    }
} else {
    Write-Host "⚠️  costanti.php not found, skipping..." -ForegroundColor Yellow
}

# Step 8: Create launcher script
Write-Host "📝 Creating launcher script..." -ForegroundColor Yellow
$launcherPath = Join-Path $phpDesktopDir "hoteldruid.exe"
$phpDesktopExe = Join-Path $phpDesktopDir "phpdesktop-chrome.exe"

if (Test-Path $phpDesktopExe) {
    # Create a shortcut/launcher
    $launcherScript = Join-Path $PSScriptRoot "start-hoteldruid-desktop.ps1"
    $launcherContent = @"
# HotelDruid PHP Desktop Launcher
# Starts HotelDruid in PHP Desktop application

`$phpDesktopExe = Join-Path `$PSScriptRoot "phpdesktop\phpdesktop-chrome.exe"
`$hoteldruidDir = Join-Path `$PSScriptRoot "hoteldruid"

if (-not (Test-Path `$phpDesktopExe)) {
    Write-Host "❌ PHP Desktop executable not found: `$phpDesktopExe" -ForegroundColor Red
    Write-Host "   Please run setup-phpdesktop.ps1 first" -ForegroundColor Yellow
    exit 1
}

if (-not (Test-Path `$hoteldruidDir)) {
    Write-Host "❌ HotelDruid directory not found: `$hoteldruidDir" -ForegroundColor Red
    exit 1
}

Write-Host "🚀 Starting HotelDruid..." -ForegroundColor Green
Start-Process -FilePath `$phpDesktopExe -WorkingDirectory `$hoteldruidDir
"@
    
    Set-Content -Path $launcherScript -Value $launcherContent -Encoding UTF8
    Write-Host "✅ Launcher script created: start-hoteldruid-desktop.ps1" -ForegroundColor Green
} else {
    Write-Host "⚠️  PHP Desktop executable not found, skipping launcher creation" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "=== Setup Complete ===" -ForegroundColor Green
Write-Host ""
Write-Host "📋 Next Steps:" -ForegroundColor Cyan
Write-Host "   1. Run: .\start-hoteldruid-desktop.ps1" -ForegroundColor White
Write-Host "   2. Or double-click: phpdesktop\phpdesktop-chrome.exe" -ForegroundColor White
Write-Host ""
Write-Host "📁 Important Files:" -ForegroundColor Cyan
Write-Host "   - PHP Desktop: $phpDesktopDir" -ForegroundColor White
Write-Host "   - HotelDruid: $hoteldruidDir" -ForegroundColor White
Write-Host "   - Settings: $settingsPath" -ForegroundColor White
Write-Host ""
Write-Host "✨ HotelDruid is ready to run!" -ForegroundColor Green
