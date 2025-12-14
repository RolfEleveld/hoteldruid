# HotelDruid Enhanced Deployment System - Complete Index

## 📋 Documentation Map

### For Different User Types

#### 👤 End Users (First-Time Installation)

1. **Start here:** [DEPLOYMENT_QUICK_REFERENCE.md](DEPLOYMENT_QUICK_REFERENCE.md)
   - Quick start commands
   - Common tasks
   - Troubleshooting

2. **Installation command:**  

   ```powershell
   .\install_release.ps1 -UseDeploymentConfig
   ```

#### 👨‍💼 System Administrators

1. **Start here:** [DEPLOYMENT_CONFIG_GUIDE.md](DEPLOYMENT_CONFIG_GUIDE.md)
   - Complete architecture overview
   - Multi-machine deployment
   - Enterprise workflows

2. **Reference:** [DEPLOYMENT_WORKFLOWS.md](DEPLOYMENT_WORKFLOWS.md)
   - Ready-to-use PowerShell scripts
   - Batch deployment templates
   - Scheduled update workflows

#### 🔧 Developers/Maintainers

1. **Start here:** [IMPLEMENTATION_SUMMARY.md](IMPLEMENTATION_SUMMARY.md)
   - Technical implementation details
   - Component descriptions
   - Architecture principles

2. **Deep dive:** [DEPLOYMENT_CONFIG_GUIDE.md](DEPLOYMENT_CONFIG_GUIDE.md) - Architecture section

---

## 📁 Files Created/Modified

### Primary Scripts

| File | Purpose | Usage |
|------|---------|-------|
| **deploy-hoteldruid-config.ps1** | Environment detection & config generation | Internal (called by installer) |
| **install_release.ps1** | Installation script with config integration | `.\install_release.ps1 -UseDeploymentConfig` |
| **hoteldruid-settings-manager.ps1** | Settings viewing/management utility | `.\hoteldruid-settings-manager.ps1 -Action View` |

### Documentation

| File | Purpose | Audience |
|------|---------|----------|
| **DEPLOYMENT_QUICK_REFERENCE.md** | Quick commands and common tasks | End users |
| **DEPLOYMENT_CONFIG_GUIDE.md** | Complete guide with architecture | Admins & Developers |
| **DEPLOYMENT_WORKFLOWS.md** | Ready-to-use automation scripts | Admins & Teams |
| **IMPLEMENTATION_SUMMARY.md** | Technical implementation overview | Developers |
| **INDEX.md** (this file) | Navigation and reference | Everyone |

### Auto-Generated

| File | Location | Purpose |
|------|----------|---------|
| **hoteldruid-config.php** | `<InstallDir>\hoteldruid\` | Application configuration |
| **deployment-settings.json** | `%APPDATA%\HotelDruid\` | Settings persistence |

---

## 🚀 Quick Start Guide

### Installation (Any User)

```powershell
# One command - everything automatic
.\install_release.ps1 -UseDeploymentConfig
```

That's it! The script will:

- ✓ Detect OneDrive
- ✓ Suggest data folder
- ✓ Create configuration
- ✓ Save settings for future updates
- ✓ Create shortcuts

### Update (Any User)

```powershell
# Same command - data preserved automatically
.\install_release.ps1 -UseDeploymentConfig
```

### Check Settings (Any User)

```powershell
# View what's configured
.\hoteldruid-settings-manager.ps1 -Action View

# Validate paths
.\hoteldruid-settings-manager.ps1 -Action Validate
```

---

## 📚 Documentation by Task

### "How do I...?"

#### Install HotelDruid

→ [DEPLOYMENT_QUICK_REFERENCE.md](DEPLOYMENT_QUICK_REFERENCE.md#-quick-start) - Quick Start

#### Update HotelDruid

→ [DEPLOYMENT_WORKFLOWS.md](DEPLOYMENT_WORKFLOWS.md#workflow-3-update-existing-installation)

#### Check my settings

→ [DEPLOYMENT_QUICK_REFERENCE.md](DEPLOYMENT_QUICK_REFERENCE.md#-common-tasks) - Common Tasks

#### Deploy to multiple machines

→ [DEPLOYMENT_WORKFLOWS.md](DEPLOYMENT_WORKFLOWS.md#workflow-4-multi-machine-deployment)

#### Set up OneDrive backup

→ [DEPLOYMENT_CONFIG_GUIDE.md](DEPLOYMENT_CONFIG_GUIDE.md#benefits) - Benefits section

#### Recover from data loss

→ [DEPLOYMENT_WORKFLOWS.md](DEPLOYMENT_WORKFLOWS.md#workflow-5-disaster-recovery)

#### Understand how it works

→ [IMPLEMENTATION_SUMMARY.md](IMPLEMENTATION_SUMMARY.md)

#### Automate updates

→ [DEPLOYMENT_WORKFLOWS.md](DEPLOYMENT_WORKFLOWS.md#workflow-6-scheduled-updates)

#### Troubleshoot issues

→ [DEPLOYMENT_CONFIG_GUIDE.md](DEPLOYMENT_CONFIG_GUIDE.md#troubleshooting)

---

## 🎯 Feature Highlights

### ✨ Automatic OneDrive Detection

- Finds personal and business OneDrive
- Suggests optimal data location
- Enables automatic cloud backup
- See: [DEPLOYMENT_CONFIG_GUIDE.md](DEPLOYMENT_CONFIG_GUIDE.md#1-onedrive-detection)

### 💾 Settings Persistence

- Stores settings in `%APPDATA%\HotelDruid\`
- Loads on redeploy
- Easy to backup/restore
- See: [DEPLOYMENT_CONFIG_GUIDE.md](DEPLOYMENT_CONFIG_GUIDE.md#settings-persistence)

### ⚙️ Automatic Configuration

- Generates `hoteldruid-config.php`
- No manual path editing
- Supports Windows and relative paths
- See: [DEPLOYMENT_CONFIG_GUIDE.md](DEPLOYMENT_CONFIG_GUIDE.md#configuration-file)

### 🌍 Multi-Language Support

- English, Italian, Spanish
- Auto-detects system locale
- Manual selection available
- See: [DEPLOYMENT_QUICK_REFERENCE.md](DEPLOYMENT_QUICK_REFERENCE.md#-language-support)

---

## 📊 Settings Storage Structure

```text
%APPDATA%\HotelDruid\
├── deployment-settings.json       ← Persistent settings

Example content:
{
  "Timestamp": "2025-12-14 15:30:45",
  "InstallDirectory": "C:\\Users\\rolfe\\AppData\\Local\\HotelDruid",
  "DataDirectory": "C:\\Users\\rolfe\\OneDrive\\HotelDruid\\hoteldruid\\data",
  "OneDrivePath": "C:\\Users\\rolfe\\OneDrive",
  "Hostname": "DESKTOP-ABC123",
  "Username": "rolfe"
}
```

**Access via PowerShell:**

```powershell
$settingsFile = Join-Path $env:APPDATA 'HotelDruid\deployment-settings.json'
Get-Content $settingsFile | ConvertFrom-Json
```

---

## 🔄 Workflow Diagrams

### Initial Deployment

```text
Run: install_release.ps1 -UseDeploymentConfig
            ↓
Detect OneDrive
            ↓
Suggest Data Folder
            ↓
Generate Configuration
            ↓
Save Settings
            ↓
Extract & Install
            ↓
✓ Complete
```

### Subsequent Updates

```text
Run: install_release.ps1 -UseDeploymentConfig
            ↓
Load Previous Settings
            ↓
Use Same Paths (data preserved)
            ↓
Update Configuration
            ↓
Extract New Version
            ↓
✓ Complete (data intact)
```

---

## 🔐 Security & Backup

### Data Protection

- OneDrive provides encrypted storage
- Settings stored in user's AppData
- NTFS permissions recommended
- See: [DEPLOYMENT_CONFIG_GUIDE.md](DEPLOYMENT_CONFIG_GUIDE.md#security-notes)

### Backup & Recovery

- Settings can be backed up separately
- Data folders preserved on updates
- Disaster recovery workflow available
- See: [DEPLOYMENT_WORKFLOWS.md](DEPLOYMENT_WORKFLOWS.md#workflow-5-disaster-recovery)

---

## 🛠️ Utility Commands

### Settings Manager

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

### Direct Access to Settings

```powershell
# View settings file location
$settingsPath = Join-Path $env:APPDATA 'HotelDruid\deployment-settings.json'
notepad $settingsPath

# View config file location
$configPath = Join-Path $env:LOCALAPPDATA 'HotelDruid\hoteldruid\hoteldruid-config.php'
notepad $configPath
```

---

## 📞 Getting Help

### Issue Troubleshooting

1. Check: [DEPLOYMENT_QUICK_REFERENCE.md](DEPLOYMENT_QUICK_REFERENCE.md#-troubleshooting) - Troubleshooting section
2. Review: [DEPLOYMENT_CONFIG_GUIDE.md](DEPLOYMENT_CONFIG_GUIDE.md#troubleshooting) - Extended troubleshooting
3. Validate:  

   ```powershell
   .\hoteldruid-settings-manager.ps1 -Action Validate
   ```

### Detailed Information

```powershell
# View all settings
.\hoteldruid-settings-manager.ps1 -Action View

# Get help on any script
.\install_release.ps1 -?
.\deploy-hoteldruid-config.ps1 -?
.\hoteldruid-settings-manager.ps1 -?
```

---

## 🎓 Learning Path

### New to HotelDruid Deployment?

1. Read: [DEPLOYMENT_QUICK_REFERENCE.md](DEPLOYMENT_QUICK_REFERENCE.md) (5 min)
2. Try: `.\install_release.ps1 -UseDeploymentConfig` (2 min)
3. Check: `.\hoteldruid-settings-manager.ps1 -Action View` (1 min)

### Planning Enterprise Deployment?

1. Read: [DEPLOYMENT_CONFIG_GUIDE.md](DEPLOYMENT_CONFIG_GUIDE.md) (15 min)
2. Review: [DEPLOYMENT_WORKFLOWS.md](DEPLOYMENT_WORKFLOWS.md) (10 min)
3. Adapt: [DEPLOYMENT_WORKFLOWS.md - Workflow 2](DEPLOYMENT_WORKFLOWS.md#workflow-2-it-administrator-deployment) (5 min)

### Automating Updates?

1. Read: [DEPLOYMENT_WORKFLOWS.md - Workflow 6](DEPLOYMENT_WORKFLOWS.md#workflow-6-scheduled-updates) (10 min)
2. Adapt: Schedule in Task Scheduler (5 min)
3. Test: Run manually first (5 min)

---

## 🔑 Key Concepts

### OneDrive Detection

The system automatically finds OneDrive and suggests storing data there for automatic cloud backup.

### Settings Persistence

Deployment settings are stored locally so that updates use the same paths without reconfiguration.

### Configuration Auto-Generation

The `hoteldruid-config.php` file is automatically created with the detected or confirmed paths.

### Non-Intrusive Design

All features are optional and the system falls back gracefully if OneDrive isn't found.

---

## 📈 Version & Updates

**System Version:** 1.0  
**Last Updated:** December 14, 2025  
**Compatibility:** Windows 7+ with PowerShell 5.0+  
**Language Support:** English, Italian, Spanish  

---

## 📋 Checklist: Post-Installation

After installation, verify:

- [ ] Start Menu shortcut created and working
- [ ] Settings saved to `%APPDATA%\HotelDruid\deployment-settings.json`
- [ ] Configuration file at `<InstallDir>\hoteldruid\hoteldruid-config.php`
- [ ] Data folder created and accessible
- [ ] OneDrive sync active (if using OneDrive location)

Run:

```powershell
.\hoteldruid-settings-manager.ps1 -Action Validate
```

---

## 🎯 Next Steps

1. **Ready to install?**  
   → Run: `.\install_release.ps1 -UseDeploymentConfig`

2. **Need details?**  
   → Read: [DEPLOYMENT_CONFIG_GUIDE.md](DEPLOYMENT_CONFIG_GUIDE.md)

3. **Planning deployment?**  
   → Check: [DEPLOYMENT_WORKFLOWS.md](DEPLOYMENT_WORKFLOWS.md)

4. **Got questions?**  
   → See: [DEPLOYMENT_QUICK_REFERENCE.md - Troubleshooting](DEPLOYMENT_QUICK_REFERENCE.md#-troubleshooting)

---

## 📄 Complete File List

**Scripts:**

- `deploy-hoteldruid-config.ps1` - Configuration detection
- `install_release.ps1` - Installation with config
- `hoteldruid-settings-manager.ps1` - Settings management

**Documentation:**

- `DEPLOYMENT_QUICK_REFERENCE.md` - Quick start guide
- `DEPLOYMENT_CONFIG_GUIDE.md` - Complete guide
- `DEPLOYMENT_WORKFLOWS.md` - Automation scripts
- `IMPLEMENTATION_SUMMARY.md` - Technical overview
- `INDEX.md` (this file) - Navigation guide

**Auto-Generated:**

- `hoteldruid-config.php` - Application configuration
- `deployment-settings.json` - Settings storage

---

**Welcome to HotelDruid Enhanced Deployment System!**  
Choose where to go next from the list above. 👆
