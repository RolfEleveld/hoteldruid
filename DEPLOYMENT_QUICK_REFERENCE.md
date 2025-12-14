# HotelDruid Deployment Quick Reference

## 🚀 Quick Start

### First Time Installation

```powershell
.\install_release.ps1 -UseDeploymentConfig
```

✓ Auto-detects OneDrive  
✓ Creates config file  
✓ Stores settings for future updates

### Update Existing Installation

```powershell
.\install_release.ps1 -UseDeploymentConfig
```

✓ Loads previous paths automatically  
✓ Data preserved  
✓ Settings updated

## 📁 File Locations

| Component | Location |
|-----------|----------|
| **Settings** | `%APPDATA%\HotelDruid\deployment-settings.json` |
| **Config File** | `<InstallDir>\hoteldruid\hoteldruid-config.php` |
| **Data Folder** | `OneDrive\HotelDruid\hoteldruid\data` (or custom) |
| **Install Directory** | `%LOCALAPPDATA%\HotelDruid` (default) |

## 🛠️ Common Tasks

### View Current Settings

```powershell
.\hoteldruid-settings-manager.ps1 -Action View
```

### Validate Paths

```powershell
.\hoteldruid-settings-manager.ps1 -Action Validate
```

### Backup Settings

```powershell
.\hoteldruid-settings-manager.ps1 -Action Backup
```

### Reset Settings (Start Fresh)

```powershell
.\hoteldruid-settings-manager.ps1 -Action Reset
.\install_release.ps1 -UseDeploymentConfig
```

### Custom Installation

```powershell
.\install_release.ps1 `
  -InstallDir 'C:\Program Files\HotelDruid' `
  -DataFolder 'D:\MyBackup\HotelDruid\data' `
  -CreateDesktopShortcut
```

## 🌍 Language Support

```powershell
# Italian
.\install_release.ps1 -UseDeploymentConfig -Language 'it'

# Spanish
.\install_release.ps1 -UseDeploymentConfig -Language 'es'

# English (default)
.\install_release.ps1 -UseDeploymentConfig -Language 'en'
```

## 🔍 Troubleshooting

### OneDrive Not Detected?

- Install/enable OneDrive
- Use custom data folder: `-DataFolder 'C:\CustomPath'`

### Settings Not Found on Update?

```powershell
# Check what's stored
.\hoteldruid-settings-manager.ps1 -Action View

# Validate paths
.\hoteldruid-settings-manager.ps1 -Action Validate
```

### Configuration File Missing?

- Check install directory has `hoteldruid` folder
- Run with admin rights if permission denied
- Reinstall if file corrupted: `-Force` flag

## 📊 Deployment Settings JSON

**Location:** `%APPDATA%\HotelDruid\deployment-settings.json`

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

## 🎯 What Gets Stored/Restored

### Stored in Settings JSON

✓ Install directory  
✓ Data folder path  
✓ OneDrive location  
✓ Computer & user info  
✓ Timestamp  

### Preserved on Update

✓ Data files (in data folder)  
✓ Paths (loaded from settings)  
✓ OneDrive sync  

### Overwritten on Update

✗ Application files  
✗ Config paths (regenerated)  
✗ Shortcuts  

## 🔒 Security Notes

- Settings stored locally per user
- Data folder recommended on OneDrive for backup
- hoteldruid-config.php contains path information
- Don't commit to version control with user paths
- Use NTFS permissions on data folder

## 📚 Full Documentation

See `DEPLOYMENT_CONFIG_GUIDE.md` for:

- Complete architecture overview
- Detailed workflow diagrams
- Advanced configuration options
- Multi-device setup
- Scripted deployment examples
- Network path support

## 💡 Pro Tips

1. **Backup before major version updates**  

   ```powershell
   .\hoteldruid-settings-manager.ps1 -Action Backup
   ```

2. **Verify all paths after installation**  

   ```powershell
   .\hoteldruid-settings-manager.ps1 -Action Validate
   ```

3. **Store settings backup on cloud**  

   ```powershell
   .\hoteldruid-settings-manager.ps1 -Action Backup -BackupPath 'C:\Users\*\OneDrive\Backups'
   ```

4. **Install portable version**  

   ```powershell
   .\install_release.ps1 -InstallDir 'D:\Portable\HotelDruid'
   ```

## 🆘 Getting Help

Check these files for more info:

- `DEPLOYMENT_CONFIG_GUIDE.md` - Complete guide
- `hoteldruid-settings-manager.ps1` - Settings utility
- `deploy-hoteldruid-config.ps1` - Config detection
- `install_release.ps1` - Installation script

---

**Need more help?** Run scripts with `-?` flag for detailed help:

```powershell
.\install_release.ps1 -?
.\deploy-hoteldruid-config.ps1 -?
```
