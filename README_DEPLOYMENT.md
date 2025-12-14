# HotelDruid Enhanced Deployment System

> **Automatic OneDrive detection + Settings persistence + Zero configuration**

## 🎯 What This Does

This enhanced deployment system for HotelDruid:

1. ✅ **Detects OneDrive** and suggests data folder location
2. ✅ **Creates data folder** for automatic cloud backup  
3. ✅ **Generates configuration** file automatically
4. ✅ **Stores deployment settings** for easy redeploys
5. ✅ **Looks up previous settings** when you update
6. ✅ **Supports multiple languages** (EN/IT/ES)

## 🚀 Quick Start (One Command!)

```powershell
# First install OR update - same command!
.\install_release.ps1 -UseDeploymentConfig
```

That's it! Everything happens automatically:

- Detects your OneDrive
- Creates a data folder there (for automatic backup)
- Generates configuration file
- Saves settings for next time
- Installs HotelDruid

## 📁 What Gets Created

```text
C:\Users\<you>\OneDrive\HotelDruid\
└── hoteldruid\
    └── data\                    ← Your data here (synced to cloud!)

C:\Users\<you>\AppData\Roaming\HotelDruid\
└── deployment-settings.json     ← Settings stored here

<InstallDir>\hoteldruid\
└── hoteldruid-config.php        ← Configuration (auto-generated)
```

## 📚 Documentation

Choose your level:

| Document | Time | For You If... |
|----------|------|---|
| **AT_A_GLANCE.md** | 5 min | Want visual overview |
| **DEPLOYMENT_QUICK_REFERENCE.md** | 5 min | Need quick commands |
| **DEPLOYMENT_CONFIG_GUIDE.md** | 20 min | Want complete details |
| **DEPLOYMENT_WORKFLOWS.md** | 30 min | Planning team deployment |
| **INDEX.md** | 10 min | Need navigation |

## 🔧 What's Included

### Scripts (3)

- `deploy-hoteldruid-config.ps1` - Environment detection
- `install_release.ps1` - Installation (enhanced)
- `hoteldruid-settings-manager.ps1` - Settings utility

### Documentation (6 guides)

- AT_A_GLANCE.md - Visual summary
- DEPLOYMENT_QUICK_REFERENCE.md - Quick commands
- DEPLOYMENT_CONFIG_GUIDE.md - Complete guide
- DEPLOYMENT_WORKFLOWS.md - Automation examples
- IMPLEMENTATION_SUMMARY.md - Technical details
- INDEX.md - Navigation

### Auto-Generated

- `hoteldruid-config.php` - Configuration file
- `deployment-settings.json` - Settings storage

## 💡 Key Features

### Automatic OneDrive Detection

```powershell
✓ Finds personal OneDrive
✓ Detects business OneDrive
✓ Falls back to Documents if needed
→ Result: Data automatically backed up to cloud!
```

### Settings Persistence

```text
First Install:    Run installer → Settings saved
Later Updates:    Run same installer → Previous settings loaded
→ Result: No reconfiguration needed!
```

### Zero Configuration

```text
No manual paths
No config files to edit
No setup wizard
→ Just run command, everything works!
```

### Multi-Language

```powershell
# Auto-detects your language (EN/IT/ES)
# Or specify:
.\install_release.ps1 -UseDeploymentConfig -Language 'it'
```

## 📊 Usage Examples

### Basic Installation

```powershell
# Auto-everything
.\install_release.ps1 -UseDeploymentConfig
```

### With Desktop Shortcut

```powershell
.\install_release.ps1 -UseDeploymentConfig -CreateDesktopShortcut
```

### Custom Data Folder

```powershell
.\install_release.ps1 `
  -UseDeploymentConfig `
  -DataFolder 'D:\MyBackup\HotelDruid\data'
```

### Check Settings

```powershell
.\hoteldruid-settings-manager.ps1 -Action View
```

### Validate Everything

```powershell
.\hoteldruid-settings-manager.ps1 -Action Validate
```

### Backup Settings

```powershell
.\hoteldruid-settings-manager.ps1 -Action Backup
```

## 🎓 Where to Start?

### 🤔 "Just show me how to install"

→ Run: `.\install_release.ps1 -UseDeploymentConfig`  
→ Done!

### 📖 "I want to understand what's happening"

→ Read: [DEPLOYMENT_QUICK_REFERENCE.md](DEPLOYMENT_QUICK_REFERENCE.md)

### 🏢 "I need to deploy to my team"

→ Read: [DEPLOYMENT_WORKFLOWS.md](DEPLOYMENT_WORKFLOWS.md)

### 🔍 "I want all the details"

→ Read: [DEPLOYMENT_CONFIG_GUIDE.md](DEPLOYMENT_CONFIG_GUIDE.md)

### 🗺️ "I'm lost"

→ Read: [INDEX.md](INDEX.md)

## ✨ What Makes This Special

### For Users

✅ One-click install  
✅ Automatic cloud backup  
✅ Works in your language  
✅ Nothing to configure  

### For Administrators

✅ Enterprise deployment ready  
✅ Easy validation  
✅ Settings management  
✅ Backup/restore  

### For Teams

✅ 7 automation workflows  
✅ Multi-machine deployment  
✅ Scheduled updates  
✅ Disaster recovery  

## 🔒 Security & Backup

- Data stored on OneDrive = automatic encryption & backup
- Settings stored locally in your user folder (secure)
- Version history available via OneDrive
- Easy to backup settings separately
- No credentials stored anywhere

## 🆘 Troubleshooting

### "OneDrive not found"

```powershell
# Use custom path
.\install_release.ps1 `
  -UseDeploymentConfig `
  -DataFolder 'C:\CustomPath'
```

### "Settings not loading"

```powershell
# Check what's stored
.\hoteldruid-settings-manager.ps1 -Action View

# Or validate everything
.\hoteldruid-settings-manager.ps1 -Action Validate
```

### "Need to reset"

```powershell
# Start completely fresh
.\hoteldruid-settings-manager.ps1 -Action Reset
.\install_release.ps1 -UseDeploymentConfig
```

## 📊 What Gets Stored

**File:** `%APPDATA%\HotelDruid\deployment-settings.json`

```json
{
  "Timestamp": "2025-12-14 15:30:45",
  "InstallDirectory": "C:\\Users\\you\\AppData\\Local\\HotelDruid",
  "DataDirectory": "C:\\Users\\you\\OneDrive\\HotelDruid\\hoteldruid\\data",
  "OneDrivePath": "C:\\Users\\you\\OneDrive",
  "Hostname": "YOUR-COMPUTER",
  "Username": "you"
}
```

**Why?** So next update loads same settings automatically!

## 📈 On Update

```text
You: .\install_release.ps1 -UseDeploymentConfig
     (6 months later with new version)
     
System:
  1. Detects OneDrive (same as before)
  2. Loads previous settings ✓
  3. Uses same data folder (data preserved!)
  4. Regenerates config.php
  5. Extracts new version
  
Result: Data intact, everything works, no reconfiguration!
```

## 🎯 Key Commands

```powershell
# Install/Update
.\install_release.ps1 -UseDeploymentConfig

# Check settings
.\hoteldruid-settings-manager.ps1 -Action View

# Validate paths
.\hoteldruid-settings-manager.ps1 -Action Validate

# Backup settings
.\hoteldruid-settings-manager.ps1 -Action Backup

# Reset to start fresh
.\hoteldruid-settings-manager.ps1 -Action Reset

# Get help
.\install_release.ps1 -?
```

## 💾 Before & After

### Before (Old Way)

```text
1. Manually choose install directory
2. Manually choose data folder
3. Edit configuration files
4. Backup data manually
5. Update: Reconfigure everything again
```

### After (New Way)

```text
1. Run one command
2. Everything automatic
3. Settings saved
4. Data auto-backed-up to OneDrive
5. Update: Settings loaded automatically, done!
```

## 🚀 Get Started Now

```powershell
# Navigate to hoteldruid folder
cd C:\Path\To\HotelDruid

# Run the installer
.\install_release.ps1 -UseDeploymentConfig

# That's it!
```

## 📞 Need Help?

| What | Do This |
|------|---------|
| Quick commands | Read DEPLOYMENT_QUICK_REFERENCE.md |
| How it works | Read DEPLOYMENT_CONFIG_GUIDE.md |
| Team deployment | Read DEPLOYMENT_WORKFLOWS.md |
| Navigation | Read INDEX.md |
| Check settings | Run `.\hoteldruid-settings-manager.ps1 -Action View` |
| Validate install | Run `.\hoteldruid-settings-manager.ps1 -Action Validate` |

## ℹ️ System Requirements

- Windows 7 or later
- PowerShell 5.0 or later
- OneDrive (optional, but recommended)
- 100 MB disk space

## 🎓 Learning Resources

All documentation is included! Here's the suggested reading order:

1. **This file** - Overview (you are here)
2. **QUICK_START.ps1** - Interactive quick reference
3. **DEPLOYMENT_QUICK_REFERENCE.md** - Common commands
4. **DEPLOYMENT_CONFIG_GUIDE.md** - Complete details
5. **DEPLOYMENT_WORKFLOWS.md** - Automation examples
6. **INDEX.md** - Full navigation

## ✅ Post-Installation Checklist

After running the installer:

- [ ] Start Menu shortcut created?
- [ ] Check: `.\hoteldruid-settings-manager.ps1 -Action View`
- [ ] Validate paths: `.\hoteldruid-settings-manager.ps1 -Action Validate`
- [ ] Application launches correctly?
- [ ] Data folder exists on OneDrive?
- [ ] Config file created in install directory?

## 📝 Version Information

- **System:** HotelDruid Enhanced Deployment v1.0
- **Released:** December 14, 2025
- **Compatibility:** Windows 7+ / PowerShell 5.0+
- **Languages:** English, Italian, Spanish
- **Dependencies:** None (uses built-in Windows features only)

## 🎁 What You Get

✓ 1,600+ lines of production code  
✓ 2,500+ lines of documentation  
✓ 7 complete automation workflows  
✓ 25+ code examples  
✓ 3 language support  
✓ Zero external dependencies  
✓ Enterprise-grade quality  

## 🚀 Ready?

```powershell
.\install_release.ps1 -UseDeploymentConfig
```

---

**Questions?** Start with [INDEX.md](INDEX.md) for navigation, or run:

```powershell
.\QUICK_START.ps1
```

