#!/usr/bin/env pwsh
<#
.SYNOPSIS
    HotelDruid Deployment Settings Manager
    
.DESCRIPTION
    Utility script to view, manage, and troubleshoot HotelDruid deployment settings
    stored in %APPDATA%\HotelDruid\deployment-settings.json
    
.EXAMPLE
    .\hoteldruid-settings-manager.ps1 -Action View
    .\hoteldruid-settings-manager.ps1 -Action Reset
    .\hoteldruid-settings-manager.ps1 -Action Validate
    .\hoteldruid-settings-manager.ps1 -Action Backup -BackupPath 'D:\Settings Backup'
#>

param(
    [ValidateSet('View', 'Reset', 'Validate', 'Backup', 'Edit')]
    [string]$Action = 'View',
    
    [string]$BackupPath,
    [string]$CustomInstallDir,
    [string]$CustomDataFolder,
    [switch]$Force
)

$ErrorActionPreference = 'Stop'

# Configuration
$AppDataBase = Join-Path $env:APPDATA 'HotelDruid'
$SettingsFile = Join-Path $AppDataBase 'deployment-settings.json'

# Localization
$strings = @{
    en = @{
        SettingsManager = '=== HotelDruid Deployment Settings Manager ==='
        NoSettings = 'No deployment settings found.'
        SettingsFile = 'Settings File'
        CurrentSettings = 'Current Deployment Settings'
        Timestamp = 'Last Updated'
        InstallDir = 'Install Directory'
        DataDir = 'Data Directory'
        OneDrive = 'OneDrive Path'
        Computer = 'Computer Name'
        User = 'User Name'
        PathExists = 'Exists'
        PathMissing = 'MISSING'
        ValidationResults = 'Validation Results'
        ValidationPassed = '✓ All paths are valid'
        ValidationFailed = '✗ Some paths are invalid or missing'
        ResettingSettings = 'Resetting deployment settings...'
        SettingsReset = 'Settings have been reset.'
        ConfirmReset = 'Are you sure you want to reset settings? (Y/n)'
        BackupCreated = 'Settings backed up to: {0}'
        EditingSettings = 'Opening settings file for editing...'
        SettingsSaved = 'Settings saved successfully.'
        InvalidJson = 'Settings file contains invalid JSON.'
        PathValid = '{0} - ✓ Valid'
        PathInvalid = '{0} - ✗ Invalid or missing'
        UnexpectedError = 'An unexpected error occurred: {0}'
    }
    it = @{
        SettingsManager = '=== Manager Impostazioni Distribuzione HotelDruid ==='
        NoSettings = 'Nessuna impostazione di distribuzione trovata.'
        SettingsFile = 'File Impostazioni'
        CurrentSettings = 'Impostazioni Distribuzione Attuali'
        Timestamp = 'Ultimo Aggiornamento'
        InstallDir = 'Directory Installazione'
        DataDir = 'Directory Dati'
        OneDrive = 'Percorso OneDrive'
        Computer = 'Nome Computer'
        User = 'Nome Utente'
        PathExists = 'Esiste'
        PathMissing = 'MANCANTE'
        ValidationResults = 'Risultati Validazione'
        ValidationPassed = '✓ Tutti i percorsi sono validi'
        ValidationFailed = '✗ Alcuni percorsi non sono validi o mancanti'
        ResettingSettings = 'Ripristino impostazioni di distribuzione...'
        SettingsReset = 'Le impostazioni sono state ripristinate.'
        ConfirmReset = 'Sei sicuro di voler ripristinare le impostazioni? (S/n)'
        BackupCreated = 'Impostazioni salvate in: {0}'
        EditingSettings = 'Apertura file impostazioni per la modifica...'
        SettingsSaved = 'Impostazioni salvate con successo.'
        InvalidJson = 'File impostazioni contiene JSON non valido.'
        PathValid = '{0} - ✓ Valido'
        PathInvalid = '{0} - ✗ Non valido o mancante'
        UnexpectedError = 'Si è verificato un errore imprevisto: {0}'
    }
    es = @{
        SettingsManager = '=== Administrador de Configuración de Implementación HotelDruid ==='
        NoSettings = 'No se encontró configuración de implementación.'
        SettingsFile = 'Archivo de Configuración'
        CurrentSettings = 'Configuración de Implementación Actual'
        Timestamp = 'Última Actualización'
        InstallDir = 'Directorio de Instalación'
        DataDir = 'Directorio de Datos'
        OneDrive = 'Ruta de OneDrive'
        Computer = 'Nombre de Equipo'
        User = 'Nombre de Usuario'
        PathExists = 'Existe'
        PathMissing = 'FALTA'
        ValidationResults = 'Resultados de Validación'
        ValidationPassed = '✓ Todas las rutas son válidas'
        ValidationFailed = '✗ Algunas rutas no son válidas o faltan'
        ResettingSettings = 'Restableciendo configuración de implementación...'
        SettingsReset = 'La configuración ha sido restablecida.'
        ConfirmReset = '¿Estás seguro de que deseas restablecer la configuración? (S/n)'
        BackupCreated = 'Configuración respaldada en: {0}'
        EditingSettings = 'Abriendo archivo de configuración para editar...'
        SettingsSaved = 'Configuración guardada correctamente.'
        InvalidJson = 'El archivo de configuración contiene JSON inválido.'
        PathValid = '{0} - ✓ Válido'
        PathInvalid = '{0} - ✗ Inválido o falta'
        UnexpectedError = 'Ocurrió un error inesperado: {0}'
    }
}

function Get-Lang {
    try {
        $name = (Get-Culture).Name
        if ($name -match '^it') { return 'it' }
        if ($name -match '^es') { return 'es' }
    } catch { }
    return 'en'
}

$lang = Get-Lang
$t = $strings[$lang]

# ============================================================================
# FUNCTIONS
# ============================================================================

function View-Settings {
    Write-Host ""
    Write-Host $t.SettingsManager -ForegroundColor Cyan
    Write-Host "-" * 70 -ForegroundColor Cyan
    Write-Host "$($t.SettingsFile): $SettingsFile" -ForegroundColor Yellow
    
    if (-not (Test-Path $SettingsFile)) {
        Write-Host ""
        Write-Host $t.NoSettings -ForegroundColor Yellow
        Write-Host ""
        return
    }
    
    try {
        $settings = Get-Content $SettingsFile | ConvertFrom-Json
        
        Write-Host ""
        Write-Host $t.CurrentSettings -ForegroundColor Green
        Write-Host "-" * 70 -ForegroundColor Cyan
        Write-Host "$($t.Timestamp): $($settings.Timestamp)" -ForegroundColor White
        Write-Host ""
        Write-Host "$($t.InstallDir): " -ForegroundColor White -NoNewline
        Write-Host $settings.InstallDirectory -ForegroundColor Cyan
        Write-Host ""
        Write-Host "$($t.DataDir): " -ForegroundColor White -NoNewline
        Write-Host $settings.DataDirectory -ForegroundColor Cyan
        
        if ($settings.OneDrivePath) {
            Write-Host ""
            Write-Host "$($t.OneDrive): " -ForegroundColor White -NoNewline
            Write-Host $settings.OneDrivePath -ForegroundColor Cyan
        }
        
        Write-Host ""
        Write-Host "$($t.Computer): $($settings.Hostname)" -ForegroundColor Gray
        Write-Host "$($t.User): $($settings.Username)" -ForegroundColor Gray
        Write-Host ""
        Write-Host "-" * 70 -ForegroundColor Cyan
        
    } catch {
        Write-Host ""
        Write-Host $t.InvalidJson -ForegroundColor Red
        Write-Host $_.Exception.Message -ForegroundColor Red
        Write-Host ""
    }
}

function Validate-Settings {
    Write-Host ""
    Write-Host $t.SettingsManager -ForegroundColor Cyan
    Write-Host ""
    
    if (-not (Test-Path $SettingsFile)) {
        Write-Host $t.NoSettings -ForegroundColor Yellow
        Write-Host ""
        return $false
    }
    
    try {
        $settings = Get-Content $SettingsFile | ConvertFrom-Json
        
        Write-Host $t.ValidationResults -ForegroundColor Green
        Write-Host "-" * 70 -ForegroundColor Cyan
        
        $allValid = $true
        
        # Check Install Directory
        if (Test-Path $settings.InstallDirectory) {
            Write-Host ($t.PathValid -f $settings.InstallDirectory) -ForegroundColor Green
        } else {
            Write-Host ($t.PathInvalid -f $settings.InstallDirectory) -ForegroundColor Red
            $allValid = $false
        }
        
        # Check Data Directory
        if (Test-Path $settings.DataDirectory) {
            Write-Host ($t.PathValid -f $settings.DataDirectory) -ForegroundColor Green
        } else {
            Write-Host ($t.PathInvalid -f $settings.DataDirectory) -ForegroundColor Red
            $allValid = $false
        }
        
        # Check OneDrive if set
        if ($settings.OneDrivePath -and (Test-Path $settings.OneDrivePath)) {
            Write-Host ($t.PathValid -f $settings.OneDrivePath) -ForegroundColor Green
        } elseif ($settings.OneDrivePath) {
            Write-Host ($t.PathInvalid -f $settings.OneDrivePath) -ForegroundColor Yellow
        }
        
        # Check config file
        $configFile = Join-Path $settings.InstallDirectory 'hoteldruid' 'hoteldruid-config.php'
        if (Test-Path $configFile) {
            Write-Host ($t.PathValid -f "Config file: $configFile") -ForegroundColor Green
        } else {
            Write-Host ($t.PathInvalid -f "Config file: $configFile") -ForegroundColor Yellow
            $allValid = $false
        }
        
        Write-Host ""
        Write-Host "-" * 70 -ForegroundColor Cyan
        Write-Host ""
        
        if ($allValid) {
            Write-Host $t.ValidationPassed -ForegroundColor Green
        } else {
            Write-Host $t.ValidationFailed -ForegroundColor Yellow
        }
        Write-Host ""
        
        return $allValid
        
    } catch {
        Write-Host $t.InvalidJson -ForegroundColor Red
        Write-Host $_.Exception.Message -ForegroundColor Red
        Write-Host ""
        return $false
    }
}

function Reset-Settings {
    Write-Host ""
    if (-not $Force) {
        $response = Read-Host $t.ConfirmReset
        if ($response -notmatch '^[Yy]?$') {
            Write-Host "Aborted." -ForegroundColor Yellow
            return
        }
    }
    
    Write-Host $t.ResettingSettings -ForegroundColor Yellow
    
    if (Test-Path $SettingsFile) {
        Remove-Item $SettingsFile -Force
    }
    
    Write-Host $t.SettingsReset -ForegroundColor Green
    Write-Host ""
}

function Backup-Settings {
    if (-not $BackupPath) {
        $BackupPath = [Environment]::GetFolderPath('Desktop')
    }
    
    if (-not (Test-Path $BackupPath)) {
        New-Item -Path $BackupPath -ItemType Directory -Force | Out-Null
    }
    
    $backupFile = Join-Path $BackupPath "hoteldruid-settings-$(Get-Date -Format 'yyyyMMdd-HHmmss').json"
    
    Copy-Item -Path $SettingsFile -Destination $backupFile -Force
    
    Write-Host ""
    Write-Host ($t.BackupCreated -f $backupFile) -ForegroundColor Green
    Write-Host ""
}

function Edit-Settings {
    if (-not (Test-Path $SettingsFile)) {
        Write-Host ""
        Write-Host $t.NoSettings -ForegroundColor Yellow
        Write-Host ""
        return
    }
    
    Write-Host ""
    Write-Host $t.EditingSettings -ForegroundColor Yellow
    
    # Open with default JSON editor
    & notepad $SettingsFile
    
    Write-Host $t.SettingsSaved -ForegroundColor Green
    Write-Host ""
}

# ============================================================================
# MAIN
# ============================================================================

try {
    switch ($Action) {
        'View' { View-Settings }
        'Validate' { Validate-Settings | Out-Null }
        'Reset' { Reset-Settings }
        'Backup' { Backup-Settings }
        'Edit' { Edit-Settings }
        default { View-Settings }
    }
} catch {
    Write-Host ""
    Write-Host ($t.UnexpectedError -f $_.Exception.Message) -ForegroundColor Red
    Write-Host ""
    exit 1
}
