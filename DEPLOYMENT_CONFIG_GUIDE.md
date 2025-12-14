# HotelDruid Enhanced Deployment System

## Overview

The enhanced deployment system intelligently manages HotelDruid installation and configuration across deployments. It:

1. **Detects OneDrive** and suggests data folders for automatic cloud backup
2. **Persists deployment settings** for consistent configuration on updates
3. **Generates configuration files** automatically with detected paths
4. **Retrieves previous settings** on redeploy for seamless updates

## Architecture

### Components

#### 1. `deploy-hoteldruid-config.ps1`

Deployment Configuration Helper

Runs before installation to:

- Detect OneDrive installation(s)
- Suggest optimal data folder locations
- Load previous deployment settings
- Generate `hoteldruid-config.php` with detected paths
- Store settings in `%APPDATA%\HotelDruid\deployment-settings.json`

#### 2. `install_release.ps1` (Enhanced)

Installation Script with Config Integration

Now includes:

- Integration with deployment config helper
- Automatic data folder creation
- Configuration file generation
- `hoteldruid-config.php` placement in the deployed application
- Shortcut creation (Start Menu, Desktop, Startup)

#### 3. Settings Storage

**Location:** `%APPDATA%\HotelDruid\deployment-settings.json`

Stores:

- Installation directory
- Data folder path
- OneDrive detection status
- Timestamp
- Computer/user information

## Usage

### Quick Start (With OneDrive Detection)

```powershell
# Run the installer with automatic configuration detection
.\install_release.ps1 -UseDeploymentConfig
```

This command will:

1. Detect OneDrive
2. Load previous settings (if any)
3. Suggest optimal data folder: `OneDrive/HotelDruid/hoteldruid/data`
4. Create configuration file automatically
5. Proceed with installation

### Standard Installation

```powershell
# Install with default settings
.\install_release.ps1 -ZipPath .\hoteldruid-release.zip
```

### Advanced Options

```powershell
# Specify custom locations
.\install_release.ps1 `
  -ZipPath .\hoteldruid-release.zip `
  -InstallDir 'C:\Program Files\HotelDruid' `
  -DataFolder 'D:\MyBackup\HotelDruid\data'

# Create desktop shortcuts
.\install_release.ps1 -UseDeploymentConfig -CreateDesktopShortcut

# Auto-launch after installation
.\install_release.ps1 -UseDeploymentConfig -LaunchAfterInstall

# Set language (Italian, English, Spanish)
.\install_release.ps1 -UseDeploymentConfig -Language 'it'
```

## Configuration Files

### App Configuration (hoteldruid-config.php)

Hoteldruid uses a single app-specific configuration file updated during deployment. phpdesktop runtime settings are generated separately and should not be embedded inside the app.

### Generated Location

During installation, the script creates:

```text
<InstallDir>\hoteldruid\hoteldruid-config.php
```

### Configuration Example (App)

```php
<?php
define('C_DATI_PATH_EXTERNAL', "C:/Users/rolfe/OneDrive/HotelDruid/hoteldruid/data");
?>
```

### Paths

- **Windows paths:** Use forward slashes `/` or double backslashes `\\`
- **Relative paths:** Supported (relative to application directory)
- **Default:** If empty, uses `./dati` relative to application

## Runtime Settings (phpdesktop)

The phpdesktop runtime reads its own `settings.json` located under the installed phpdesktop directory. Configure via `hoteldruid-config.php` for data directory selection.

Generated at:

```text
<InstallDir>\phpdesktop\settings.json
```

Key fields set by the installer:

- document_root: absolute path to `<InstallDir>\hoteldruid`
- index_files: includes `inizio.php`
- cgi_interpreter: `php/php-cgi.exe`
- listen_on: `["127.0.0.1", 8080]`

## Settings Persistence

### Deployment Settings File

**Path:** `%APPDATA%\HotelDruid\deployment-settings.json`

**Contents:**

```json
{
  "Timestamp": "2025-12-14 15:30:45",
  "InstallDirectory": "C:\\Users\\rolfe\\AppData\\Local\\HotelDruid",
  "DataDirectory": "C:\\Users\\rolfe\\OneDrive\\HotelDruid\\hoteldruid\\data",
  "OneDrivePath": "C:\\Users\\rolfe\\OneDrive",
  "Hostname": "DESKTOP-ABC123",
  "Username": "rolfe"
}
```

### Accessing Settings

**View current settings:**

```powershell
.\deploy-hoteldruid-config.ps1 -ShowSettings
```

**Reset settings:**

```powershell
.\deploy-hoteldruid-config.ps1 -ResetSettings
```

## Workflow: Initial Deployment

```text
┌──────────────────────────────────────────────┐
│ Run: install_release.ps1 -UseDeploymentConfig│
└──────────────────┬───────────────────────────┘
                   │
                   ▼
    ┌──────────────────────────────┐
    │ Detect OneDrive              │
    │ Found: C:\Users\*\OneDrive   │
    └──────────────────────────────┘
                   │
                   ▼
    ┌──────────────────────────────┐
    │ Load Previous Settings       │
    │ None found (first deploy)    │
    └──────────────────────────────┘
                   │
                   ▼
    ┌──────────────────────────────┐
    │ Suggest Data Folder          │
    │ OneDrive/HotelDruid/...      │
    └──────────────────────────────┘
                   │
                   ▼
    ┌──────────────────────────────┐
    │ Create Data Folder           │
    └──────────────────────────────┘
                   │
                   ▼
    ┌──────────────────────────────┐
    │ Generate hoteldruid-config.php
    │ Set C_DATI_PATH_EXTERNAL     │
    └──────────────────────────────┘
                   │
                   ▼
    ┌──────────────────────────────┐
    │ Save Settings JSON           │
    │ %APPDATA%\HotelDruid\...     │
    └──────────────────────────────┘
                   │
                   ▼
    ┌──────────────────────────────┐
    │ Extract ZIP                  │
    │ Create Shortcuts             │
    │ Optional: Launch             │
    └──────────────────────────────┘
```

## Workflow: Redeploy/Update

```text
┌──────────────────────────────────┐
│ Run: install_release.ps1         │
│      -UseDeploymentConfig        │
│ (Update to new version)          │
└──────┬───────────────────────────┘
       │
       ▼
┌──────────────────────────────────┐
│ Detect OneDrive                  │
│ Same as before                   │
└──────────────────────────────────┘
       │
       ▼
┌──────────────────────────────────┐
│ Load Previous Settings ✓         │
│ InstallDir: C:\Users\*\...       │
│ DataDir: OneDrive\HotelDruid\... │
└──────────────────────────────────┘
       │
       ▼
┌──────────────────────────────────┐
│ Use Previous Paths               │
│ Data folder already exists       │
│ (Data is preserved)              │
└──────────────────────────────────┘
       │
       ▼
┌──────────────────────────────────┐
│ Update Configuration File        │
│ Same paths, new version          │
└──────────────────────────────────┘
       │
       ▼
┌──────────────────────────────────┐
│ Extract new version ZIP          │
│ (Old hoteldruid-config.php       │
│  is overwritten with new)        │
└──────────────────────────────────┘
       │
       ▼
┌──────────────────────────────────┐
│ Installation Complete            │
│ Data: Intact on OneDrive         │
│ Config: Updated                  │
└──────────────────────────────────┘
```

## Benefits

### 1. Automatic Cloud Backup

- Data stored in OneDrive automatically syncs
- Free backup across devices
- Disaster recovery ready

### 2. Seamless Updates

- Deploy new versions without reconfiguring paths
- Previous settings automatically loaded
- Data preserved during updates

### 3. Multi-Device Setup

- Settings stored locally for each computer
- Supports multiple OneDrive accounts
- Easy relocation of data folders

### 4. Localization

- English, Italian, Spanish support
- Automatic language detection based on system locale
- Custom language selection available

## Troubleshooting

### Settings Not Found on Update

**Issue:** Expected previous settings not loaded

**Solutions:**

1. Check settings file exists: `%APPDATA%\HotelDruid\deployment-settings.json`
2. Verify path exists and is readable
3. Reset settings and redeploy: `.\deploy-hoteldruid-config.ps1 -ResetSettings`

### OneDrive Not Detected

**Issue:** Script suggests Documents folder instead of OneDrive

**Reasons:**

- OneDrive not installed
- OneDrive not syncing
- Installed in non-standard location

**Solutions:**

1. Install/enable OneDrive
2. Manually specify data folder: `-DataFolder 'C:\CustomPath'`
3. Create symlink to OneDrive if in non-standard location

### Configuration File Generation Fails

**Issue:** hoteldruid-config.php not created

**Solutions:**

1. Verify installation directory permissions
2. Check `hoteldruid` folder exists in install directory
3. Run script with appropriate privileges

## Advanced Configuration

### Manual Settings Override

```powershell
# Override detected settings
.\install_release.ps1 `
  -UseDeploymentConfig `
  -DataFolder 'D:\CustomBackup\HotelDruid\data'
```

### Scripted Deployment

```powershell
# For automated/scripted deployments
$params = @{
    ZipPath = 'C:\Releases\hoteldruid-release.zip'
    UseDeploymentConfig = $true
    CreateStartMenuShortcut = $true
    Language = 'it'
}

& '.\install_release.ps1' @params
```

### Network Paths (OneDrive for Business)

OneDrive for Business shares are automatically detected:

```powershell
# Auto-configures for: C:\Users\<user>\OneDrive - <Company>
.\install_release.ps1 -UseDeploymentConfig
```

## File Structure After Installation

```text
%APPDATA%\HotelDruid\
├── deployment-settings.json          # Settings persistence

<InstallDir>\
├── hoteldruid\
│   ├── hoteldruid-config.php        # Generated config (NEW)
│   ├── dati/                        # Legacy location (if not OneDrive)
│   ├── includes/
│   ├── themes/
│   └── ...
├── phpdesktop/
│   └── phpdesktop-chrome.exe
└── ...

OneDrive\
└── HotelDruid\
    └── hoteldruid\
        └── data/                    # Data folder (synced to cloud)
            ├── database backups
            ├── user data
            └── ...
```

## Security Notes

⚠️ **Important:**

- Keep `hoteldruid-config.php` secure (contains paths)
- Don't commit paths with user information to version control
- Backup `deployment-settings.json` when changing computers
- Use appropriate NTFS permissions for data folders

## Support

For issues with the deployment system:

1. Check settings: `.\deploy-hoteldruid-config.ps1 -ShowSettings`
2. Review generated config: `<InstallDir>\hoteldruid\hoteldruid-config.php`
3. Check AppData folder: `%APPDATA%\HotelDruid\`
4. Review PowerShell output for error messages
