#!/usr/bin/env pwsh
<#
.SYNOPSIS
    HotelDruid Deployment Configuration Helper
    
.DESCRIPTION
    Manages OneDrive detection, data folder suggestions, and persistent deployment settings
    stored in the user's personal AppData folder for easy lookup during updates/redeploys.
    
.NOTES
    This script:
    - Detects OneDrive installation
    - Suggests data folder locations (preferring OneDrive for backup)
    - Stores settings in: %APPDATA%\HotelDruid\deployment-settings.json
    - Retrieves settings on redeploy for consistent paths
#>

param(
    [Parameter(Mandatory=$false)]
    [string]$InstallDir,
    
    [Parameter(Mandatory=$false)]
    [string]$DataFolder,
    
    [switch]$Force,
    [switch]$ShowSettings,
    [switch]$ResetSettings
)

$ErrorActionPreference = 'Stop'

# ============================================================================
# CONFIGURATION
# ============================================================================

$AppDataBase = Join-Path $env:APPDATA 'HotelDruid'
$SettingsFile = Join-Path $AppDataBase 'deployment-settings.json'

$strings = @{
    en = @{
        DetectingEnvironment = 'Detecting deployment environment...'
        OneDriveFound = 'OneDrive installation detected: {0}'
        OneDriveNotFound = 'OneDrive not detected. Using standard folders.'
        SuggestedDataFolder = 'Suggested data folder: {0}'
        DataFolderExists = 'Data folder already exists: {0}'
        CreatingDataFolder = 'Creating data folder: {0}'
        DataFolderCreated = 'Data folder created successfully.'
        LoadingPreviousSettings = 'Loading previous deployment settings...'
        PreviousSettingsFound = 'Previous settings found:'
        PreviousInstallDir = '  Install Directory: {0}'
        PreviousDataFolder = '  Data Folder: {0}'
        SavedSettingsTo = 'Deployment settings saved to: {0}'
        ConfigFileLocation = 'Configuration file location: {0}'
        DataFolderPath = 'Data folder path: {0}'
        ConfirmProceed = 'Ready to proceed with deployment.'
        SettingsSummary = '=== Deployment Settings Summary ==='
        InstallDirectory = 'Install Directory'
        DataDirectory = 'Data Directory'
        SettingsStore = 'Settings Store'
        CreatedSettings = 'Created new deployment settings'
        UpdatedSettings = 'Updated deployment settings'
        ResetSettings = 'Settings have been reset'
    }
    it = @{
        DetectingEnvironment = 'Rilevamento ambiente di distribuzione...'
        OneDriveFound = 'Installazione OneDrive rilevata: {0}'
        OneDriveNotFound = 'OneDrive non rilevato. Utilizzo cartelle standard.'
        SuggestedDataFolder = 'Cartella dati suggerita: {0}'
        DataFolderExists = 'Cartella dati già esistente: {0}'
        CreatingDataFolder = 'Creazione cartella dati: {0}'
        DataFolderCreated = 'Cartella dati creata con successo.'
        LoadingPreviousSettings = 'Caricamento impostazioni distribuzione precedenti...'
        PreviousSettingsFound = 'Impostazioni precedenti trovate:'
        PreviousInstallDir = '  Directory di installazione: {0}'
        PreviousDataFolder = '  Cartella dati: {0}'
        SavedSettingsTo = 'Impostazioni distribuzione salvate in: {0}'
        ConfigFileLocation = 'Ubicazione file di configurazione: {0}'
        DataFolderPath = 'Percorso cartella dati: {0}'
        ConfirmProceed = 'Pronto per procedere con la distribuzione.'
        SettingsSummary = '=== Riepilogo impostazioni distribuzione ==='
        InstallDirectory = 'Directory di installazione'
        DataDirectory = 'Directory dati'
        SettingsStore = 'Archivio impostazioni'
        CreatedSettings = 'Nuove impostazioni di distribuzione create'
        UpdatedSettings = 'Impostazioni di distribuzione aggiornate'
        ResetSettings = 'Le impostazioni sono state ripristinate'
    }
    es = @{
        DetectingEnvironment = 'Detectando entorno de implementación...'
        OneDriveFound = 'Instalación de OneDrive detectada: {0}'
        OneDriveNotFound = 'OneDrive no detectado. Usando carpetas estándar.'
        SuggestedDataFolder = 'Carpeta de datos sugerida: {0}'
        DataFolderExists = 'Carpeta de datos ya existe: {0}'
        CreatingDataFolder = 'Creando carpeta de datos: {0}'
        DataFolderCreated = 'Carpeta de datos creada correctamente.'
        LoadingPreviousSettings = 'Cargando configuración de implementación anterior...'
        PreviousSettingsFound = 'Configuración anterior encontrada:'
        PreviousInstallDir = '  Directorio de instalación: {0}'
        PreviousDataFolder = '  Carpeta de datos: {0}'
        SavedSettingsTo = 'Configuración de implementación guardada en: {0}'
        ConfigFileLocation = 'Ubicación del archivo de configuración: {0}'
        DataFolderPath = 'Ruta de la carpeta de datos: {0}'
        ConfirmProceed = 'Listo para proceder con la implementación.'
        SettingsSummary = '=== Resumen de configuración de implementación ==='
        InstallDirectory = 'Directorio de instalación'
        DataDirectory = 'Directorio de datos'
        SettingsStore = 'Almacén de configuración'
        CreatedSettings = 'Nueva configuración de implementación creada'
        UpdatedSettings = 'Configuración de implementación actualizada'
        ResetSettings = 'La configuración se ha restablecido'
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

function Find-OneDrive {
    <#
    .SYNOPSIS
        Detect OneDrive installation(s) on the system
    .OUTPUTS
        PSObject with OneDrivePath and AccountName
    #>
    $oneDrives = @()
    
    # Check standard location: %USERPROFILE%\OneDrive
    $standardPath = Join-Path $env:USERPROFILE 'OneDrive'
    if (Test-Path $standardPath) {
        $oneDrives += @{
            Path = $standardPath
            Type = 'Personal'
        }
    }
    
    # Check for OneDrive for Business: %USERPROFILE%\OneDrive - <Company>
    $oneDriveParent = Split-Path $standardPath -Parent
    Get-ChildItem -Path $oneDriveParent -Directory | Where-Object { $_.Name -match '^OneDrive' -and $_.FullName -ne $standardPath } | ForEach-Object {
        $oneDrives += @{
            Path = $_.FullName
            Type = 'Business'
        }
    }
    
    return $oneDrives
}

function Get-PreviousSettings {
    <#
    .SYNOPSIS
        Load previous deployment settings from the settings file
    .OUTPUTS
        PSObject with previous settings or $null
    #>
    if (Test-Path $SettingsFile) {
        try {
            $settings = Get-Content $SettingsFile | ConvertFrom-Json
            return $settings
        } catch {
            Write-Warning "Failed to load settings file: $_"
            return $null
        }
    }
    return $null
}

function Save-DeploymentSettings {
    <#
    .SYNOPSIS
        Save deployment settings for future reference
    #>
    param(
        [Parameter(Mandatory=$true)]
        [string]$InstallDirectory,
        
        [Parameter(Mandatory=$true)]
        [string]$DataDirectory,
        
        [Parameter(Mandatory=$false)]
        [string]$OneDrivePath
    )
    
    # Create AppData folder if it doesn't exist
    if (-not (Test-Path $AppDataBase)) {
        New-Item -Path $AppDataBase -ItemType Directory -Force | Out-Null
    }
    
    $settings = @{
        Timestamp = (Get-Date).ToString('yyyy-MM-dd HH:mm:ss')
        InstallDirectory = $InstallDirectory
        DataDirectory = $DataDirectory
        OneDrivePath = $OneDrivePath
        Hostname = $env:COMPUTERNAME
        Username = $env:USERNAME
    }
    
    $settings | ConvertTo-Json | Set-Content $SettingsFile -Encoding UTF8
    Write-Host ($t.SavedSettingsTo -f $SettingsFile) -ForegroundColor Green
}

function New-ConfigFile {
    <#
    .SYNOPSIS
        Generate the hoteldruid-config.php file with detected paths
    #>
    param(
        [Parameter(Mandatory=$true)]
        [string]$DataPath,
        
        [Parameter(Mandatory=$true)]
        [string]$OutputPath
    )
    
    # Convert Windows path to PHP-friendly format
    $phpDataPath = $DataPath -replace '\\', '/' 
    
    $configContent = @"
<?php

##################################################################################
#    HOTELDRUID CONFIGURATION FILE
#    This file allows you to configure HotelDruid settings, including
#    the location of the dati folder (where all persistent data is stored).
#
#    IMPORTANT: Keep this file safe and do not delete it. Your data location
#    is configured here.
#
#    Generated: $(Get-Date)
#    Computer: $env:COMPUTERNAME
#    User: $env:USERNAME
##################################################################################

// Path to dati folder - change this to your desired location
// Use forward slashes (/) or double backslashes (\\) for Windows paths
// Examples:
//   Windows: "/Documents/HotelDruid" or "\\Documents\\HotelDruid"
//   Linux/Mac: "/home/username/hoteldruid-data"
//   Relative path: "../hoteldruid-data" (relative to the application directory)
//
// If this is empty or not set, the default "./dati" (relative to application) will be used
define('C_DATI_PATH_EXTERNAL', "$phpDataPath");

// Optional: You can also set other paths here if needed
// define('C_CARTELLA_DOC', "");
// define('C_CARTELLA_FILES_REALI', "");
// define('C_CARTELLA_UPLOAD_IMG', "");

?>
"@
    
    if (-not (Test-Path (Split-Path $OutputPath -Parent))) {
        New-Item -Path (Split-Path $OutputPath -Parent) -ItemType Directory -Force | Out-Null
    }
    
    $configContent | Set-Content $OutputPath -Encoding UTF8
}



function Show-SettingsSummary {
    <#
    .SYNOPSIS
        Display a formatted summary of deployment settings
    #>
    param(
        [string]$InstallDir,
        [string]$DataFolder,
        [string]$ConfigFile
    )
    
    Write-Host ""
    Write-Host $t.SettingsSummary -ForegroundColor Cyan
    Write-Host "-" * 50 -ForegroundColor Cyan
    Write-Host "$($t.InstallDirectory):`t$InstallDir" -ForegroundColor White
    Write-Host "$($t.DataDirectory):`t$DataFolder" -ForegroundColor White
    Write-Host "$($t.ConfigFileLocation):`t$ConfigFile" -ForegroundColor White
    Write-Host "$($t.SettingsStore):`t$SettingsFile" -ForegroundColor White
    Write-Host "-" * 50 -ForegroundColor Cyan
    Write-Host ""
}

# ============================================================================
# MAIN LOGIC
# ============================================================================

Write-Host ""
Write-Host $t.DetectingEnvironment -ForegroundColor Green

# Reset settings if requested
if ($ResetSettings) {
    if (Test-Path $SettingsFile) {
        Remove-Item $SettingsFile -Force
        Write-Host $t.ResetSettings -ForegroundColor Yellow
    }
    exit 0
}

# Show settings if requested
if ($ShowSettings) {
    $prevSettings = Get-PreviousSettings
    if ($prevSettings) {
        Write-Host ""
        Write-Host $t.PreviousSettingsFound -ForegroundColor Cyan
        Write-Host $prevSettings | ConvertTo-Json -Depth 5 | Out-Host
    } else {
        Write-Host "No previous settings found." -ForegroundColor Yellow
    }
    exit 0
}

# Detect OneDrive
$oneDrives = Find-OneDrive
$selectedOneDrive = $null
$suggestedDataFolder = $null

if ($oneDrives.Count -gt 0) {
    $selectedOneDrive = $oneDrives[0].Path
    Write-Host ($t.OneDriveFound -f $selectedOneDrive) -ForegroundColor Green
    
    # Suggest data folder in OneDrive
    $suggestedDataFolder = Join-Path $selectedOneDrive 'HotelDruid' 'hoteldruid' 'data'
    Write-Host ($t.SuggestedDataFolder -f $suggestedDataFolder) -ForegroundColor Cyan
} else {
    Write-Host $t.OneDriveNotFound -ForegroundColor Yellow
    
    # Fall back to Documents
    $documentsFolder = [Environment]::GetFolderPath('MyDocuments')
    $suggestedDataFolder = Join-Path $documentsFolder 'HotelDruid' 'hoteldruid' 'data'
    Write-Host ($t.SuggestedDataFolder -f $suggestedDataFolder) -ForegroundColor Cyan
}

# Load previous settings
Write-Host ""
Write-Host $t.LoadingPreviousSettings -ForegroundColor Yellow
$previousSettings = Get-PreviousSettings
if ($previousSettings) {
    Write-Host $t.PreviousSettingsFound -ForegroundColor Green
    Write-Host ($t.PreviousInstallDir -f $previousSettings.InstallDirectory)
    Write-Host ($t.PreviousDataFolder -f $previousSettings.DataDirectory)
    
    # Use previous settings if not overridden
    if (-not $InstallDir) { $InstallDir = $previousSettings.InstallDirectory }
    if (-not $DataFolder) { $DataFolder = $previousSettings.DataDirectory }
}

# Use provided or suggested values
if (-not $InstallDir) {
    $InstallDir = Join-Path $env:LOCALAPPDATA 'HotelDruid'
}

if (-not $DataFolder) {
    $DataFolder = $suggestedDataFolder
}

# Create data folder if it doesn't exist
if (-not (Test-Path $DataFolder)) {
    Write-Host ($t.CreatingDataFolder -f $DataFolder) -ForegroundColor Yellow
    New-Item -Path $DataFolder -ItemType Directory -Force | Out-Null
    Write-Host $t.DataFolderCreated -ForegroundColor Green
} else {
    Write-Host ($t.DataFolderExists -f $DataFolder) -ForegroundColor Gray
}

# Generate config file in the install directory
$configFile = Join-Path $InstallDir 'hoteldruid' 'hoteldruid-config.php'
New-ConfigFile -DataPath $DataFolder -OutputPath $configFile

# Note: phpdesktop runtime settings are no longer modified here.

# Save settings
Save-DeploymentSettings -InstallDirectory $InstallDir -DataDirectory $DataFolder -OneDrivePath $selectedOneDrive

# Display summary
Show-SettingsSummary -InstallDir $InstallDir -DataFolder $DataFolder -ConfigFile $configFile

Write-Host $t.ConfirmProceed -ForegroundColor Green
Write-Host ""

# Return settings as object for chaining with install script
$deploymentSettings = @{
    InstallDirectory = $InstallDir
    DataDirectory = $DataFolder
    ConfigFile = $configFile
    SettingsFile = $SettingsFile
    OneDrivePath = $selectedOneDrive
}

return $deploymentSettings
