# HotelDruid PHP Desktop Setup Guide

This guide explains how to set up and run HotelDruid as a standalone Windows application using PHP Desktop.

## Overview

PHP Desktop embeds PHP and a web browser (Chromium) into a single Windows executable, allowing HotelDruid to run without requiring:

- Apache web server
- MySQL database (uses SQLite instead)
- Separate PHP installation

## Prerequisites

- Windows 10 or Windows 11
- PowerShell 5.1 or later
- Internet connection (for initial download)

## Quick Start

### 1. Run Setup Script

Open PowerShell in the project root directory and run:

```powershell
.\setup-phpdesktop.ps1
```

This script will:

- Download PHP Desktop (if not already present)
- Extract and configure PHP Desktop
- Verify PHP extensions (sqlite3, session, json, mbstring)
- Create `server.php` router script
- Configure settings for HotelDruid

### 2. Start HotelDruid

After setup is complete, run:

```powershell
.\start-hoteldruid-desktop.ps1
```

Or double-click:

```cmd
phpdesktop\phpdesktop-chrome.exe
```

## Directory Structure

After setup, your directory structure will look like:

```text
hotelDroid/
├── phpdesktop/              # PHP Desktop application
│   ├── phpdesktop-chrome.exe
│   ├── php/
│   │   └── php.exe
│   └── settings.json
├── hoteldruid/              # HotelDruid application
│   ├── server.php          # Router script (created by setup)
│   ├── inizio.php          # Entry point
│   ├── includes/
│   ├── themes/
│   └── dati/               # Data directory (SQLite database)
├── setup-phpdesktop.ps1     # Setup script
└── start-hoteldruid-desktop.ps1  # Launcher script
```

## Configuration

### PHP Desktop Settings

Settings are stored in `phpdesktop/settings.json`. Key settings:

- **Application Name**: HotelDruid
- **Window Size**: 1280x800 (minimum 1024x600)
- **Web Server**: Listens on 127.0.0.1:8080
- **Document Root**: hoteldruid directory
- **Router Script**: server.php

### Database Configuration

HotelDruid will use SQLite by default. The database file will be stored in:

```
hoteldruid/dati/db_hoteldruid
```

## First Run

1. **Start the application** using the launcher script
2. **Create the database**:
   - The application will detect if no database exists
   - Follow the setup wizard to create the SQLite database
   - Configure rooms, pricing, etc.

## Troubleshooting

### PHP Desktop Not Found

If you see "PHP Desktop executable not found":

- Run `.\setup-phpdesktop.ps1` again
- Check that `phpdesktop/phpdesktop-chrome.exe` exists

### Missing PHP Extensions

If you see errors about missing PHP extensions:

- Check `phpdesktop/php/php.ini`
- Ensure extensions are enabled:
  - `extension=sqlite3`
  - `extension=session`
  - `extension=json`
  - `extension=mbstring`

### Port Already in Use

If port 8080 is already in use:

- Edit `phpdesktop/settings.json`
- Change the port in `web_server.listen_on[1]`
- Or close the application using port 8080

### Database Errors

If you see database connection errors:

- Ensure the `hoteldruid/dati/` directory is writable
- Check that SQLite3 extension is enabled
- Verify `dati/dati_connessione.php` exists and is configured for SQLite

## Manual Setup

If the automated setup doesn't work:

1. **Download PHP Desktop**:
   - Visit: https://github.com/cztomczak/phpdesktop/releases
   - Download: `phpdesktop-chrome-91.0-php-8.1.0-windows.zip`
   - Extract to `phpdesktop/` directory

2. **Configure settings.json**:
   - Copy `hoteldruid/phpdesktop-settings.json` to `phpdesktop/settings.json`
   - Adjust paths if needed

3. **Verify server.php exists**:
   - Should be in `hoteldruid/server.php`
   - If missing, copy from the template in this repository

## Development Mode

For development, you can also run HotelDruid using PHP's built-in server:

```powershell
cd hoteldruid
php -S localhost:8080 server.php
```

Then open `http://localhost:8080` in your browser.

## Security Notes

- PHP Desktop runs on `127.0.0.1` (localhost only) by default
- This prevents external network access
- For network access, change to `0.0.0.0` in settings.json (not recommended for production)

## Support

For issues or questions:

- Check the main HotelDruid documentation in `hoteldruid/doc/`
- Review PHP Desktop documentation: https://github.com/cztomczak/phpdesktop

## Next Steps

After getting the basic setup working:

1. Test all HotelDruid features with SQLite
2. Configure rooms, pricing, and other settings
3. Set up user accounts and permissions
4. Create backups of your SQLite database
