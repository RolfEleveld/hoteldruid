# HotelDruid Standalone Executable Migration Plan

## Overview

This document outlines the steps to migrate HotelDruid from a traditional Apache + MySQL/PHP setup to a standalone executable that runs like Plex Server - a single executable that starts a web server and can be accessed via browser, without requiring separate MySQL or Apache installations.

## Current Architecture Analysis

### Current Setup

- **Web Server**: Apache (with .htaccess files for security)
- **Database**: MySQL/PostgreSQL/SQLite (SQLite already supported!)
- **Language**: PHP (traditional PHP application)
- **Entry Point**: `inizio.php`
- **Data Directory**: `./dati/` (contains database connection info and SQLite files)

### Key Findings

1. ✅ **SQLite Support Already Exists**: The application has full SQLite support via `includes/funzioni_sqlite.php`
2. ✅ **No Framework Dependencies**: Pure PHP, no Composer dependencies
3. ✅ **Simple Entry Point**: `inizio.php` is the main entry point
4. ⚠️ **Apache-Specific**: `.htaccess` files used for security (denying direct PHP access in includes/themes)
5. ✅ **Portable Data**: Database connection stored in `dati/dati_connessione.php`

## Migration Strategy

### Option 1: PHP Built-in Server (Recommended for Development/Testing)

- **Pros**: Simple, no additional dependencies, built into PHP
- **Cons**: Not production-grade, single-threaded, limited features
- **Use Case**: Quick prototype, development, small deployments

### Option 2: Standalone Web Server (Recommended for Production)

- **Pros**: Production-ready, better performance, more features
- **Options**:
  - **PHP-FPM + Nginx** (bundled)
  - **RoadRunner** (PHP application server)
  - **Swoole** (async PHP server)
  - **ReactPHP** (event-driven PHP)
- **Use Case**: Production deployments, better performance

### Option 3: Electron + PHP (Desktop-like Experience)

- **Pros**: Native desktop app feel, system tray integration
- **Cons**: Larger bundle size, more complex
- **Use Case**: Desktop application feel

## Recommended Approach: PHP Built-in Server + Executable Wrapper

### Phase 1: Core Migration (SQLite + PHP Built-in Server)

#### 1.1 Database Migration

- [ ] **Verify SQLite Support**
  - Confirm SQLite3 extension is available in PHP
  - Test database creation with SQLite
  - Verify all queries work with SQLite syntax

- [ ] **Update Default Configuration**
  - Modify `includes/costanti.php` to default to SQLite when available
  - Ensure `C_CREADB_TIPODB` defaults to "sqlite"
  - Update `creadb.php` to prefer SQLite

- [ ] **Data Directory Structure**
  - Ensure `dati/` directory is writable
  - SQLite database files will be stored as `dati/db_hoteldruid`
  - Verify path handling works with relative paths

#### 1.2 Web Server Replacement

- [ ] **Replace Apache with PHP Built-in Server**
  - Create `server.php` router script to handle:
    - Security (deny direct access to includes/themes PHP files)
    - Static file serving (CSS, JS, images)
    - PHP file execution
    - URL rewriting if needed

- [ ] **Security Implementation**
  - Replicate `.htaccess` security rules in router
  - Block direct access to:
    - `includes/*.php` (except CSS/JS)
    - `themes/*/php/*.php`
  - Allow:
    - Root PHP files
    - Static assets (CSS, JS, images)

- [ ] **Static File Handling**
  - Serve CSS, JS, images directly
  - Set proper MIME types
  - Handle caching headers

#### 1.3 Configuration Management

- [ ] **Port Configuration**
  - Default port: 8080 (configurable)
  - Allow port override via config file or environment variable
  - Check if port is available, suggest alternative if busy

- [ ] **Host Configuration**
  - Default: localhost (127.0.0.1)
  - Option to bind to all interfaces (0.0.0.0) for network access
  - Security warning for network access

- [ ] **Data Directory Path**
  - Use relative paths from executable location
  - Support portable installation (USB drive, etc.)
  - Handle Windows vs Linux path separators

### Phase 2: Executable Wrapper

#### 2.1 Windows Executable (.exe)

- [ ] **Create Launcher Script**
  - Batch file or PowerShell script that:
    - Checks for PHP installation
    - Starts PHP built-in server
    - Opens browser automatically
    - Handles graceful shutdown

- [ ] **Package PHP Runtime**
  - Bundle PHP portable version (PHP 8.1+)
  - Include required extensions:
    - SQLite3
    - Session
    - JSON
    - mbstring
    - Other dependencies
  - Use tools like:
    - **PHP Desktop** (embed PHP in executable)
    - **WinBinder** (Windows GUI wrapper)
    - **NSIS/Inno Setup** (installer with bundled PHP)

- [ ] **Create Executable**
  - Use tools like:
    - **php-desktop** (Electron-like for PHP)
    - **WinBinder** + PHP
    - **AutoIt** script compiler
    - **Go** binary that embeds PHP
    - **C# WPF** launcher that runs PHP process

#### 2.2 Linux/Mac Executable

- [ ] **Create Shell Script Launcher**
  - Bash script that:
    - Checks for PHP installation
    - Downloads/bundles PHP if needed
    - Starts server
    - Opens browser

- [ ] **Package as AppImage (Linux)**
  - Self-contained application bundle
  - Includes PHP runtime
  - Portable across Linux distributions

- [ ] **Package as DMG (macOS)**
  - Application bundle with embedded PHP
  - Native macOS experience

### Phase 3: Enhanced Features

#### 3.1 System Tray Integration

- [ ] **Windows System Tray**
  - Minimize to tray
  - Right-click menu:
    - Open in browser
    - Stop server
    - Settings
    - Exit

- [ ] **Linux/Mac System Tray**
  - Similar functionality
  - Use platform-specific APIs

#### 3.2 Auto-Start Browser

- [ ] **Browser Detection**
  - Detect default browser
  - Open `http://localhost:8080` automatically
  - Handle browser already open scenarios

#### 3.3 Configuration UI

- [ ] **Settings Window**
  - Port configuration
  - Host binding (localhost vs network)
  - Data directory location
  - Auto-start browser option
  - Save settings to config file

#### 3.4 Logging

- [ ] **Server Logs**
  - Log file location
  - Log rotation
  - Error logging
  - Access logging (optional)

### Phase 4: Production Enhancements

#### 4.1 Better Web Server (Optional Upgrade)

- [ ] **RoadRunner Integration**
  - Replace PHP built-in server with RoadRunner
  - Better performance
  - Production-ready
  - Requires Go runtime (can be bundled)

- [ ] **Nginx + PHP-FPM**
  - Bundle lightweight Nginx
  - PHP-FPM for PHP execution
  - Better for production workloads

#### 4.2 Database Migration Tool

- [ ] **MySQL to SQLite Converter**
  - Script to export MySQL data
  - Import to SQLite
  - Verify data integrity
  - Handle schema differences

#### 4.3 Backup Integration

- [ ] **Automated Backups**
  - Schedule SQLite database backups
  - Store in configurable location
  - Retention policy

## Implementation Steps (Detailed)

### Step 1: Create Router Script (`server.php`)

```php
<?php
// Router for PHP built-in server
// Handles security and routing

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
```

### Step 2: Create Launcher Scripts

#### Windows (`start_hoteldruid.bat`)

```batch
@echo off
setlocal

REM Check for PHP
where php >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo PHP not found. Please install PHP 8.1 or later.
    pause
    exit /b 1
)

REM Set port
set PORT=8080

REM Start server
echo Starting HotelDruid on http://localhost:%PORT%
start "" "http://localhost:%PORT%"
php -S localhost:%PORT% -t . server.php

pause
```

#### Linux/Mac (`start_hoteldruid.sh`)

```bash
#!/bin/bash

# Check for PHP
if ! command -v php &> /dev/null; then
    echo "PHP not found. Please install PHP 8.1 or later."
    exit 1
fi

# Set port
PORT=8080

# Start server
echo "Starting HotelDruid on http://localhost:$PORT"
xdg-open "http://localhost:$PORT" 2>/dev/null || open "http://localhost:$PORT" 2>/dev/null
php -S localhost:$PORT -t . server.php
```

### Step 3: Update Configuration for SQLite Default

Modify `includes/costanti.php` to prefer SQLite:

- Already defaults to SQLite if SQLite3 class exists
- Ensure this is the primary choice

### Step 4: Create Executable Wrapper

#### Option A: Using PHP Desktop (Windows)

- Download php-desktop
- Configure to use HotelDruid
- Package as executable

#### Option B: Using Go Launcher (Cross-platform)

```go
package main

import (
    "os"
    "os/exec"
    "path/filepath"
    "runtime"
)

func main() {
    // Get executable directory
    exePath, _ := os.Executable()
    exeDir := filepath.Dir(exePath)
    
    // Find PHP
    phpPath := findPHP()
    if phpPath == "" {
        panic("PHP not found")
    }
    
    // Start server
    cmd := exec.Command(phpPath, "-S", "localhost:8080", "-t", exeDir, "server.php")
    cmd.Dir = exeDir
    cmd.Run()
}

func findPHP() string {
    // Check common locations
    // Return path to PHP executable
}
```

#### Option C: Using Electron + PHP (Cross-platform)

- Use electron-php
- Create Electron app that runs PHP server
- Package as native executable

## Testing Checklist

- [ ] SQLite database creation works
- [ ] All application features work with SQLite
- [ ] Router script blocks unauthorized access
- [ ] Static files (CSS, JS, images) serve correctly
- [ ] PHP files execute correctly
- [ ] Port configuration works
- [ ] Browser auto-opens correctly
- [ ] Graceful shutdown works
- [ ] Data persistence works (SQLite file)
- [ ] Multiple instances don't conflict
- [ ] Network access works (if enabled)
- [ ] Windows executable runs without PHP installed
- [ ] Linux/Mac executable runs without PHP installed

## File Structure After Migration

```text
hoteldruid/
├── server.php              # Router for PHP built-in server
├── start_hoteldruid.bat     # Windows launcher
├── start_hoteldruid.sh      # Linux/Mac launcher
├── hoteldruid.exe           # Windows executable (optional)
├── hoteldruid               # Linux executable (optional)
├── config.ini               # Configuration file (optional)
├── inizio.php               # Entry point (existing)
├── costanti.php             # Constants (existing)
├── includes/                # (existing)
├── themes/                  # (existing)
├── img/                     # (existing)
└── dati/                    # Data directory (existing)
    ├── db_hoteldruid        # SQLite database
    └── dati_connessione.php # Connection config
```

## Dependencies to Bundle

### PHP Extensions Required

- sqlite3
- session
- json
- mbstring
- pdo (if used)
- openssl (if HTTPS needed)
- curl (if external APIs used)

### PHP Version

- Minimum: PHP 7.4
- Recommended: PHP 8.1+
- Tested with: PHP 8.2

## Security Considerations

1. **Default to localhost only** - Don't expose to network by default
2. **No authentication by default** - Warn users about security
3. **File permissions** - Ensure data directory is protected
4. **SQLite file permissions** - Protect database file
5. **HTTPS** - Consider adding HTTPS support for production

## Performance Considerations

1. **PHP Built-in Server Limitations**
   - Single-threaded
   - Not for high concurrency
   - Suitable for single-user or small team use

2. **SQLite Limitations**
   - File-based, good for single-user
   - Can handle moderate concurrent reads
   - Write locks may be an issue with many users
   - Consider migration path to better server if needed

## Migration Path from Existing Installation

1. **Export MySQL Data** (if using MySQL)
   - Use HotelDruid backup feature
   - Or direct MySQL export

2. **Import to SQLite**
   - Use migration script
   - Or manual import via HotelDruid interface

3. **Copy Data Directory**
   - Copy `dati/` directory
   - Update `dati_connessione.php` for SQLite

4. **Test Thoroughly**
   - Verify all data imported correctly
   - Test all features
   - Check for SQL syntax differences

## Next Steps

1. **Start with Phase 1** - Get SQLite + PHP built-in server working
2. **Test thoroughly** - Ensure all features work
3. **Create launcher scripts** - Make it easy to start
4. **Package executable** - Create standalone executable
5. **Add enhancements** - System tray, auto-start, etc.

## Tools and Resources

- **PHP Desktop**: https://github.com/cztomczak/phpdesktop
- **RoadRunner**: https://roadrunner.dev/
- **Electron PHP**: https://github.com/electron-php
- **PHP Portable**: https://windows.php.net/download/
- **AppImage**: https://appimage.org/
- **NSIS**: https://nsis.sourceforge.io/ (Windows installer)
- **Inno Setup**: https://jrsoftware.org/isinfo.php (Windows installer)

## Estimated Timeline

- **Phase 1** (Core Migration): 1-2 weeks
- **Phase 2** (Executable Wrapper): 1-2 weeks  
- **Phase 3** (Enhanced Features): 1 week
- **Phase 4** (Production Enhancements): 2-3 weeks (optional)

**Total**: 4-8 weeks for complete solution
