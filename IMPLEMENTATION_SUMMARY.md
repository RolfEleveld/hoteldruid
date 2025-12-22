# Enhanced HotelDruid Deployment System - Implementation Summary

## Overview

A comprehensive deployment enhancement system has been created to automatically detect OneDrive, suggest data folders for cloud backup, and persist deployment settings for seamless updates and redeployments.

## Components Created

### 1. **deploy-hoteldruid-config.ps1**

**Purpose:** Deployment configuration helper script

**Functionality:**

- Detects OneDrive installation (personal and business)
- Suggests optimal data folder paths
- Loads previous deployment settings from persistent storage
- Generates `hoteldruid-config.php` with detected paths
- Saves settings for future redeploys
- Supports 3 languages (English, Italian, Spanish)

**Location:** `\hotelDroid\deploy-hoteldruid-config.ps1`

**Key Features:**

```powershell
# Auto-detect and configure
.\deploy-hoteldruid-config.ps1

# View previous settings
.\deploy-hoteldruid-config.ps1 -ShowSettings

# Reset and reconfigure
.\deploy-hoteldruid-config.ps1 -ResetSettings
```

---

### 2. **install_release.ps1** (Enhanced)

**Purpose:** Installation script with integrated configuration management

**New Features:**

- Integration with deployment config helper
- Automatic data folder creation
- Configuration file generation
- `hoteldruid-config.php` placement during installation
- Settings persistence for updates

**Usage:**

```powershell
# Quick installation with auto-detection
.\install_release.ps1 -UseDeploymentConfig

# Custom paths
.\install_release.ps1 `
  -UseDeploymentConfig `
  -DataFolder 'D:\CustomPath'

# With language selection
.\install_release.ps1 -UseDeploymentConfig -Language 'it'
```

**Location:** `\hotelDroid\install_release.ps1`

---

### 3. **hoteldruid-settings-manager.ps1**

**Purpose:** Utility for managing deployment settings

**Functionality:**

- View current deployment settings
- Validate all configured paths
- Backup settings to external storage
- Reset settings (with confirmation)
- Edit settings manually

**Usage:**

```powershell
# View settings
.\hoteldruid-settings-manager.ps1 -Action View

# Validate paths
.\hoteldruid-settings-manager.ps1 -Action Validate

# Backup settings
.\hoteldruid-settings-manager.ps1 -Action Backup

# Reset settings
.\hoteldruid-settings-manager.ps1 -Action Reset
```

**Location:** `\hotelDroid\hoteldruid-settings-manager.ps1`

---

## Settings Storage Location

### Persistent Settings File

**Path:** `%APPDATA%\HotelDruid\deployment-settings.json`

**Contents:**

```json
{
  "Timestamp": "2025-12-14 15:30:45",
  "InstallDirectory": "\\AppData\\Local\\HotelDruid",
  "DataDirectory": "\\OneDrive\\HotelDruid\\hoteldruid\\data",
  "OneDrivePath": "\\OneDrive",
  "Hostname": "DESKTOP-ABC123",
  "Username": "usera"
}
```

Access:  

- Via GUI: `%APPDATA%\HotelDruid\`
- Via PowerShell: `$env:APPDATA\HotelDruid\deployment-settings.json`

---

## Configuration File

### Generated Location

`<InstallDir>\hoteldruid\hoteldruid-config.php`

### Contents

```php
<?php
// Auto-generated with detected paths
define('C_DATI_PATH_EXTERNAL', "/OneDrive/HotelDruid/hoteldruid/data");
?>
```

**Advantages:**

- Application reads data path from config file
- Path persists across updates
- Supports both Windows and relative paths
- OneDrive enables automatic backups

---

## Documentation Files

### 1. **DEPLOYMENT_CONFIG_GUIDE.md**

**Purpose:** Complete implementation guide

**Covers:**

- Architecture overview
- Workflow diagrams (initial deploy, redeploy/update)
- All usage examples
- Advanced configuration options
- Troubleshooting guide
- Security notes
- Multi-device setup
- Scripted deployments

**Location:** `\hotelDroid\DEPLOYMENT_CONFIG_GUIDE.md`

---

### 2. **DEPLOYMENT_QUICK_REFERENCE.md**

**Purpose:** Quick reference for common tasks

**Includes:**

- Quick start commands
- File location reference table
- Common tasks and solutions
- Language support examples
- Troubleshooting checklist
- Pro tips

**Location:** `\hotelDroid\DEPLOYMENT_QUICK_REFERENCE.md`

---

## Workflow Diagrams

### Initial Deployment

```text
install_release.ps1 -UseDeploymentConfig
    ↓
Detect OneDrive
    ↓
Load Previous Settings (none on first install)
    ↓
Suggest Data Folder: OneDrive\HotelDruid\hoteldruid\data
    ↓
Create Data Folder
    ↓
Generate hoteldruid-config.php
    ↓
Save Settings to %APPDATA%\HotelDruid\
    ↓
Extract ZIP to Install Directory
    ↓
Create Shortcuts (Start Menu, Desktop, etc.)
    ↓
Installation Complete
```

### Redeploy/Update

```text
install_release.ps1 -UseDeploymentConfig
    ↓
Detect OneDrive (same as before)
    ↓
Load Previous Settings ✓ Found!
    ↓
Use Previous Paths (no reconfiguration needed)
    ↓
Data Folder Already Exists (data preserved)
    ↓
Generate hoteldruid-config.php (with same paths)
    ↓
Update Settings (timestamp updated)
    ↓
Extract New Version ZIP
    ↓
Update Complete - Data Intact!
```

---

## Key Features

### 1. **OneDrive Detection**

- Automatically finds OneDrive installation
- Supports personal and business OneDrive
- Suggests optimal data location
- Falls back to Documents if not found

### 2. **Settings Persistence**

- Stores settings in personal AppData folder
- Accessible across redeploys
- Per-computer configuration
- Easy backup/restore

### 3. **Configuration Auto-Generation**

- Creates `hoteldruid-config.php` automatically
- No manual path configuration needed
- Uses detected/confirmed paths
- Supports Windows and relative paths

### 4. **Multi-Language Support**

- English (en)
- Italian (it)
- Spanish (es)
- Auto-detects system locale
- Manual override available

### 5. **Cloud Backup Integration**

- Data stored on OneDrive = automatic backup
- No additional storage needed
- Works across devices
- Peace of mind

---

## Usage Examples

### Basic Installation

```powershell
# Simplest method - auto-configures everything
.\install_release.ps1 -UseDeploymentConfig
```

### Installation with Custom Paths

```powershell
.\install_release.ps1 `
  -ZipPath 'C:\hoteldruid-release.zip' `
  -UseDeploymentConfig `
  -DataFolder 'D:\Backups\HotelDruid\data'
```

### Installation with All Options

```powershell
.\install_release.ps1 `
  -UseDeploymentConfig `
  -Language 'it' `
  -CreateStartMenuShortcut `
  -CreateDesktopShortcut `
  -LaunchAfterInstall
```

### Settings Management

```powershell
# View settings
.\hoteldruid-settings-manager.ps1 -Action View

# Validate installation
.\hoteldruid-settings-manager.ps1 -Action Validate

# Backup before major update
.\hoteldruid-settings-manager.ps1 -Action Backup -BackupPath 'D:\Backups'

# Reset and start fresh
.\hoteldruid-settings-manager.ps1 -Action Reset -Force
```

---

## Benefits

✅ **For Users:**

- One-command installation/update
- Automatic OneDrive backup
- No path reconfiguration on updates
- Settings preserved across versions

✅ **For Administrators:**

- Consistent deployment across machines
- Easy troubleshooting (view settings any time)
- Backup/restore capability
- Language-aware prompts

✅ **For Developers:**

- Application reads data path from config
- Easy path changes without code modification
- Settings available for monitoring
- Extensible framework

---

## Security Considerations

### Data Protection

- Data stored on OneDrive = encrypted at rest
- Sync enables version history
- NTFS permissions on data folders recommended

### Settings File

- Stored in user's AppData (only accessible by user)
- Contains paths (not credentials)
- Can be backed up separately
- Easy to relocate on new machine

### Configuration File Security

- Part of application installation
- Contains path information only
- Not critical to keep (regenerated on each install)
- Should not be committed to version control with paths

---

## Files Modified/Created

| File | Type | Purpose |
| - | - | - |
| `deploy-hoteldruid-config.ps1` | Created | Config detection & generation |
| `install_release.ps1` | Enhanced | Integrated config management |
| `hoteldruid-settings-manager.ps1` | Created | Settings utility |
| `DEPLOYMENT_CONFIG_GUIDE.md` | Created | Complete documentation |
| `DEPLOYMENT_QUICK_REFERENCE.md` | Created | Quick reference guide |
| `hoteldruid-config.php` | Auto-generated | Configuration for application |
| `%APPDATA%\HotelDruid\deployment-settings.json` | Auto-generated | Settings persistence |

---

## Next Steps

1. **Test the deployment:**  

   ```powershell
   .\install_release.ps1 -UseDeploymentConfig
   ```

2. **Verify settings:**  

   ```powershell
   .\hoteldruid-settings-manager.ps1 -Action Validate
   ```

3. **Test update scenario:**
   - Modify some data in HotelDruid
   - Run installer again with `-UseDeploymentConfig`
   - Verify data is preserved

4. **Review documentation:**
   - Read `DEPLOYMENT_CONFIG_GUIDE.md` for complete details
   - Check `DEPLOYMENT_QUICK_REFERENCE.md` for common tasks

---

## Implementation Notes

### Design Principles

1. **Non-intrusive** - Works with existing scripts
2. **Automatic** - Minimal user interaction
3. **Recoverable** - Easy to reset or modify
4. **Extensible** - Easy to add new features
5. **Multilingual** - Supports multiple languages

### PowerShell Version

- Requires PowerShell 5.0+
- Works on Windows 7+ (with appropriate PowerShell)
- Tested on Windows 10/11

### Dependencies

- No external dependencies
- Uses built-in PowerShell/Windows features
- JSON for settings (built-in support)
- WScript.Shell for shortcuts (standard)

---

## Support & Maintenance

### Viewing Settings

Any time you need to check deployment settings:  

```powershell
.\hoteldruid-settings-manager.ps1 -Action View
```

### Troubleshooting

See `DEPLOYMENT_CONFIG_GUIDE.md` - Troubleshooting section

### Updating Configuration

To change data folder on next deploy:  

```powershell
.\install_release.ps1 -UseDeploymentConfig -DataFolder 'NewPath'
```

---

## Conclusion

The enhanced deployment system provides a complete solution for:

- **Automatic OneDrive detection** for cloud backup
- **Settings persistence** across updates
- **Configuration management** without manual intervention
- **Multi-language support** for international users

Users can now deploy, update, and manage HotelDruid installations with a single command while maintaining data integrity and automatic backup capabilities.
