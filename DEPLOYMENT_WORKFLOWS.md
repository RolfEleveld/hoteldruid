# HotelDruid Deployment Automation Workflows

## Overview

This document provides ready-to-use PowerShell workflows and batch scripts for automating HotelDruid deployments in various scenarios.

---

## Workflow 1: Standard User Installation

**Scenario:** End-user installing HotelDruid for the first time

**PowerShell Script:**

```powershell
# Set-up parameters
$zipFile = 'hoteldruid-release.zip'
$language = 'en'  # Change to 'it' or 'es' as needed

# Run installation with auto-configuration
Write-Host "Starting HotelDruid Installation..." -ForegroundColor Green
& ".\install_release.ps1" `
    -ZipPath $zipFile `
    -UseDeploymentConfig `
    -Language $language `
    -CreateStartMenuShortcut `
    -LaunchAfterInstall $false

Write-Host "Installation complete! Check Start Menu for HotelDruid." -ForegroundColor Green
```

**Batch File Alternative:**

```batch
@echo off
REM Quick installation script
cd /d "%~dp0"
powershell -NoProfile -ExecutionPolicy Bypass -Command "& '.\install_release.ps1' -UseDeploymentConfig -CreateStartMenuShortcut"
pause
```

---

## Workflow 2: IT Administrator Deployment

**Scenario:** Deploying to multiple machines or managed environment

**PowerShell Script:**

```powershell
<#
    HotelDruid Enterprise Deployment Workflow
    Suitable for IT deployment to multiple machines
#>

param(
    [Parameter(Mandatory=$true)]
    [string]$ZipPath,
    
    [string]$InstallDir = (Join-Path $env:LOCALAPPDATA 'HotelDruid'),
    [string]$DataFolder = "",
    [string]$Language = 'en'
)

$ErrorActionPreference = 'Stop'

Write-Host "=== HotelDruid Enterprise Deployment ===" -ForegroundColor Cyan
Write-Host "Computer: $env:COMPUTERNAME" -ForegroundColor Yellow
Write-Host "User: $env:USERNAME" -ForegroundColor Yellow
Write-Host ""

try {
    # Run deployment config
    Write-Host "Step 1: Detecting environment..." -ForegroundColor Green
    $configScript = Join-Path (Split-Path $PSCommandPath -Parent) 'deploy-hoteldruid-config.ps1'
    $deploySettings = & $configScript
    
    # Run installation
    Write-Host "Step 2: Installing HotelDruid..." -ForegroundColor Green
    $installScript = Join-Path (Split-Path $PSCommandPath -Parent) 'install_release.ps1'
    & $installScript `
        -ZipPath $ZipPath `
        -UseDeploymentConfig `
        -Language $Language `
        -CreateStartMenuShortcut `
        -CreateDesktopShortcut
    
    # Verify installation
    Write-Host "Step 3: Verifying installation..." -ForegroundColor Green
    $verifyScript = Join-Path (Split-Path $PSCommandPath -Parent) 'hoteldruid-settings-manager.ps1'
    & $verifyScript -Action Validate
    
    Write-Host ""
    Write-Host "✓ Deployment completed successfully!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Installation details:" -ForegroundColor Cyan
    Write-Host "  Install Directory: $($deploySettings.InstallDirectory)" -ForegroundColor White
    Write-Host "  Data Directory: $($deploySettings.DataDirectory)" -ForegroundColor White
    Write-Host "  Settings File: $($deploySettings.SettingsFile)" -ForegroundColor White
    
} catch {
    Write-Host ""
    Write-Host "✗ Deployment failed!" -ForegroundColor Red
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}
```

---

## Workflow 3: Update Existing Installation

**Scenario:** Updating HotelDruid to new version while preserving data

**PowerShell Script:**

```powershell
<#
    HotelDruid Update Workflow
    Safely updates installation while preserving all data
#>

param(
    [Parameter(Mandatory=$true)]
    [string]$NewZipPath,
    
    [string]$Language = 'en'
)

$ErrorActionPreference = 'Stop'

Write-Host "=== HotelDruid Update Workflow ===" -ForegroundColor Cyan
Write-Host ""

try {
    # Step 1: Backup current settings
    Write-Host "Step 1: Backing up current settings..." -ForegroundColor Green
    $settingsManager = Join-Path (Split-Path $PSCommandPath -Parent) 'hoteldruid-settings-manager.ps1'
    & $settingsManager -Action Backup
    
    # Step 2: Validate current installation
    Write-Host "Step 2: Validating current installation..." -ForegroundColor Green
    & $settingsManager -Action Validate
    
    # Step 3: Install new version
    Write-Host "Step 3: Installing new version..." -ForegroundColor Green
    Write-Host "Note: Your data folder is preserved!" -ForegroundColor Yellow
    $installScript = Join-Path (Split-Path $PSCommandPath -Parent) 'install_release.ps1'
    & $installScript `
        -ZipPath $NewZipPath `
        -UseDeploymentConfig `
        -Language $Language
    
    # Step 4: Verify new installation
    Write-Host "Step 4: Verifying new installation..." -ForegroundColor Green
    & $settingsManager -Action Validate
    
    Write-Host ""
    Write-Host "✓ Update completed successfully!" -ForegroundColor Green
    Write-Host "Your data is safe and your settings have been preserved." -ForegroundColor Green
    
} catch {
    Write-Host ""
    Write-Host "✗ Update failed!" -ForegroundColor Red
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Write-Host "Your previous installation is unchanged." -ForegroundColor Yellow
    exit 1
}
```

---

## Workflow 4: Multi-Machine Deployment

**Scenario:** Deploying to multiple machines in a network

**PowerShell Script:**

```powershell
<#
    HotelDruid Multi-Machine Deployment
    Deploys to multiple computers via network
#>

param(
    [Parameter(Mandatory=$true)]
    [string[]]$ComputerNames,
    
    [Parameter(Mandatory=$true)]
    [string]$ZipPath,
    
    [string]$Language = 'en'
)

$ErrorActionPreference = 'Continue'

Write-Host "=== HotelDruid Multi-Machine Deployment ===" -ForegroundColor Cyan
Write-Host "Deploying to $($ComputerNames.Count) computer(s)..." -ForegroundColor Yellow
Write-Host ""

$deploymentLog = @()

foreach ($computer in $ComputerNames) {
    Write-Host "Deploying to: $computer" -ForegroundColor Cyan
    
    try {
        # Create deployment session
        $session = New-PSSession -ComputerName $computer -ErrorAction Stop
        
        # Copy deployment files
        Copy-Item `
            -Path @(
                '.\install_release.ps1',
                '.\deploy-hoteldruid-config.ps1',
                '.\hoteldruid-settings-manager.ps1',
                $ZipPath
            ) `
            -Destination 'C:\Temp\HotelDruid' `
            -ToSession $session `
            -Force
        
        # Run installation
        $result = Invoke-Command -Session $session -ScriptBlock {
            param($zip, $lang)
            Set-Location 'C:\Temp\HotelDruid'
            & '.\install_release.ps1' `
                -ZipPath $zip `
                -UseDeploymentConfig `
                -Language $lang
        } -ArgumentList (Split-Path $ZipPath -Leaf), $Language
        
        # Cleanup
        Remove-PSSession $session
        
        $deploymentLog += @{
            Computer = $computer
            Status = 'Success'
            Timestamp = Get-Date
        }
        
        Write-Host "  ✓ Deployment successful" -ForegroundColor Green
        
    } catch {
        $deploymentLog += @{
            Computer = $computer
            Status = 'Failed'
            Error = $_.Exception.Message
            Timestamp = Get-Date
        }
        
        Write-Host "  ✗ Deployment failed: $($_.Exception.Message)" -ForegroundColor Red
    }
    
    Write-Host ""
}

# Summary
Write-Host "=== Deployment Summary ===" -ForegroundColor Cyan
$successful = $deploymentLog | Where-Object { $_.Status -eq 'Success' } | Measure-Object | Select-Object -ExpandProperty Count
$failed = $deploymentLog | Where-Object { $_.Status -eq 'Failed' } | Measure-Object | Select-Object -ExpandProperty Count

Write-Host "Successful: $successful" -ForegroundColor Green
Write-Host "Failed: $failed" -ForegroundColor $(if ($failed -gt 0) { 'Red' } else { 'Green' })

# Export detailed log
$deploymentLog | Export-Csv -Path "deployment-log-$(Get-Date -Format 'yyyyMMdd-HHmmss').csv" -NoTypeInformation
Write-Host "Log exported to CSV file" -ForegroundColor Yellow
```

---

## Workflow 5: Disaster Recovery

**Scenario:** Recovering from data loss or corruption

**PowerShell Script:**

```powershell
<#
    HotelDruid Disaster Recovery Workflow
    Recovers installation and data from backup
#>

param(
    [Parameter(Mandatory=$true)]
    [string]$SettingsBackupFile,
    
    [Parameter(Mandatory=$true)]
    [string]$DataBackupFolder,
    
    [string]$ZipPath
)

$ErrorActionPreference = 'Stop'

Write-Host "=== HotelDruid Disaster Recovery ===" -ForegroundColor Cyan
Write-Host ""

try {
    # Step 1: Verify backups exist
    Write-Host "Step 1: Verifying backups..." -ForegroundColor Green
    if (-not (Test-Path $SettingsBackupFile)) {
        throw "Settings backup not found: $SettingsBackupFile"
    }
    if (-not (Test-Path $DataBackupFolder)) {
        throw "Data backup not found: $DataBackupFolder"
    }
    Write-Host "  ✓ Backups verified" -ForegroundColor Green
    
    # Step 2: Reset current installation
    Write-Host "Step 2: Resetting current installation..." -ForegroundColor Green
    $settingsManager = Join-Path (Split-Path $PSCommandPath -Parent) 'hoteldruid-settings-manager.ps1'
    & $settingsManager -Action Reset -Force
    Write-Host "  ✓ Installation reset" -ForegroundColor Green
    
    # Step 3: Restore data
    Write-Host "Step 3: Restoring data..." -ForegroundColor Green
    $recoverySettings = Get-Content $SettingsBackupFile | ConvertFrom-Json
    $dataFolder = $recoverySettings.DataDirectory
    
    if (-not (Test-Path $dataFolder)) {
        New-Item -Path $dataFolder -ItemType Directory -Force | Out-Null
    }
    
    Copy-Item -Path "$DataBackupFolder\*" -Destination $dataFolder -Recurse -Force
    Write-Host "  ✓ Data restored to: $dataFolder" -ForegroundColor Green
    
    # Step 4: Reinstall application (if ZIP provided)
    if ($ZipPath -and (Test-Path $ZipPath)) {
        Write-Host "Step 4: Reinstalling application..." -ForegroundColor Green
        $installScript = Join-Path (Split-Path $PSCommandPath -Parent) 'install_release.ps1'
        & $installScript `
            -ZipPath $ZipPath `
            -UseDeploymentConfig `
            -DataFolder $dataFolder
        Write-Host "  ✓ Application reinstalled" -ForegroundColor Green
    }
    
    # Step 5: Verify recovery
    Write-Host "Step 5: Verifying recovery..." -ForegroundColor Green
    & $settingsManager -Action Validate
    
    Write-Host ""
    Write-Host "✓ Disaster recovery completed successfully!" -ForegroundColor Green
    Write-Host "Your installation and data have been restored." -ForegroundColor Green
    
} catch {
    Write-Host ""
    Write-Host "✗ Recovery failed!" -ForegroundColor Red
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}
```

---

## Workflow 6: Scheduled Updates

**Scenario:** Automated updates on a schedule (e.g., nightly)

**PowerShell Script (Task Scheduler):**

```powershell
<#
    HotelDruid Scheduled Update
    Run via Windows Task Scheduler
    
    Create a scheduled task:
    - Trigger: Daily at 2:00 AM
    - Action: powershell.exe
    - Arguments: -NoProfile -ExecutionPolicy Bypass -File "C:\path\to\scheduled-update.ps1"
#>

param(
    [string]$ZipPath = "C:\Updates\hoteldruid-latest.zip",
    [string]$LogPath = "C:\Logs\hoteldruid-update.log"
)

$ErrorActionPreference = 'Continue'
$LogFile = Join-Path $LogPath "update-$(Get-Date -Format 'yyyyMMdd-HHmmss').log"

# Ensure log directory exists
if (-not (Test-Path $LogPath)) {
    New-Item -Path $LogPath -ItemType Directory -Force | Out-Null
}

# Start logging
$logContent = @()
$logContent += "=== HotelDruid Scheduled Update ==="
$logContent += "Started: $(Get-Date)"
$logContent += "Computer: $env:COMPUTERNAME"
$logContent += "User: $env:USERNAME"
$logContent += ""

try {
    if (-not (Test-Path $ZipPath)) {
        throw "Update ZIP not found: $ZipPath"
    }
    
    $logContent += "Installing update from: $ZipPath"
    
    # Get the installation script path
    $installScript = Join-Path "C:\Program Files\HotelDruid" "install_release.ps1"
    
    if (-not (Test-Path $installScript)) {
        $installScript = Join-Path $env:LOCALAPPDATA "HotelDruid\install_release.ps1"
    }
    
    # Run update
    $output = & $installScript `
        -ZipPath $ZipPath `
        -UseDeploymentConfig 2>&1
    
    $logContent += $output -join "`r`n"
    $logContent += ""
    $logContent += "✓ Update completed successfully"
    $logContent += "Completed: $(Get-Date)"
    
} catch {
    $logContent += "✗ Update failed"
    $logContent += "Error: $($_.Exception.Message)"
    $logContent += "Failed: $(Get-Date)"
}

# Write log
$logContent | Set-Content $LogFile -Encoding UTF8

# Optional: Send email notification
# $emailParams = @{
#     To = "admin@example.com"
#     From = "hoteldruid-updates@example.com"
#     Subject = "HotelDruid Update - $env:COMPUTERNAME"
#     Body = $logContent -join "`n"
#     SmtpServer = "smtp.example.com"
# }
# Send-MailMessage @emailParams
```

---

## Workflow 7: Custom Data Folder Deployment

**Scenario:** Deploying with custom data location (network share, external drive, etc.)

**PowerShell Script:**

```powershell
<#
    HotelDruid Custom Data Folder Deployment
    Suitable for shared/networked data storage
#>

param(
    [Parameter(Mandatory=$true)]
    [string]$ZipPath,
    
    [Parameter(Mandatory=$true)]
    [string]$DataFolder,
    
    [string]$InstallDir = (Join-Path $env:LOCALAPPDATA 'HotelDruid'),
    [string]$Language = 'en'
)

$ErrorActionPreference = 'Stop'

Write-Host "=== HotelDruid Custom Data Deployment ===" -ForegroundColor Cyan
Write-Host ""

try {
    # Validate data folder
    Write-Host "Validating data folder access..." -ForegroundColor Green
    if (-not (Test-Path $DataFolder)) {
        Write-Host "Creating data folder: $DataFolder" -ForegroundColor Yellow
        New-Item -Path $DataFolder -ItemType Directory -Force -ErrorAction Stop | Out-Null
    }
    
    # Test write access
    $testFile = Join-Path $DataFolder '.test'
    try {
        "test" | Set-Content $testFile -ErrorAction Stop
        Remove-Item $testFile -ErrorAction SilentlyContinue
    } catch {
        throw "Write access denied to data folder: $DataFolder"
    }
    
    Write-Host "  ✓ Data folder is accessible" -ForegroundColor Green
    Write-Host "  Path: $DataFolder" -ForegroundColor White
    Write-Host ""
    
    # Run installation
    Write-Host "Installing HotelDruid..." -ForegroundColor Green
    $installScript = Join-Path (Split-Path $PSCommandPath -Parent) 'install_release.ps1'
    & $installScript `
        -ZipPath $ZipPath `
        -InstallDir $InstallDir `
        -DataFolder $DataFolder `
        -Language $Language `
        -CreateStartMenuShortcut
    
    Write-Host ""
    Write-Host "✓ Installation completed successfully!" -ForegroundColor Green
    Write-Host "Install Directory: $InstallDir" -ForegroundColor White
    Write-Host "Data Directory: $DataFolder" -ForegroundColor White
    
} catch {
    Write-Host ""
    Write-Host "✗ Installation failed!" -ForegroundColor Red
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}
```

---

## Quick Workflow Reference

| Scenario | Script | Command |
|----------|--------|---------|
| **First Install** | install_release.ps1 | `.\install_release.ps1 -UseDeploymentConfig` |
| **Update** | install_release.ps1 | `.\install_release.ps1 -UseDeploymentConfig` |
| **Custom Paths** | install_release.ps1 | `.\install_release.ps1 -UseDeploymentConfig -DataFolder 'C:\Path'` |
| **View Settings** | hoteldruid-settings-manager.ps1 | `.\hoteldruid-settings-manager.ps1 -Action View` |
| **Validate Install** | hoteldruid-settings-manager.ps1 | `.\hoteldruid-settings-manager.ps1 -Action Validate` |
| **Backup Settings** | hoteldruid-settings-manager.ps1 | `.\hoteldruid-settings-manager.ps1 -Action Backup` |
| **Reset Settings** | hoteldruid-settings-manager.ps1 | `.\hoteldruid-settings-manager.ps1 -Action Reset` |

---

## Choosing a Workflow

1. **Standard User** → Use Workflow 1
2. **IT Admin** → Use Workflow 2
3. **Regular Updates** → Use Workflow 3
4. **Multiple Machines** → Use Workflow 4
5. **Recovery** → Use Workflow 5
6. **Automated Updates** → Use Workflow 6
7. **Custom Storage** → Use Workflow 7

---

## Tips for Success

✅ Always backup before major updates  
✅ Validate settings after installation  
✅ Test on one machine before deployment  
✅ Keep update ZIPs in accessible location  
✅ Monitor scheduled tasks if using automation  
✅ Review logs for troubleshooting  
✅ Document custom paths in team wiki  

---

## Emergency Contacts

- **Urgent Issue?** Check `DEPLOYMENT_CONFIG_GUIDE.md` Troubleshooting section
- **Need Help?** Run `.\hoteldruid-settings-manager.ps1 -Action View` to see current state
- **Lost Data?** Look for backups in `%APPDATA%\HotelDruid\`
