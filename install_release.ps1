<#
Installer script to run on the target machine after copying the release zip.
It extracts the zip to a chosen install folder, detects OneDrive for optimal data storage,
creates a configuration file, and creates Start Menu/other shortcuts.

Usage (run as user):
  .\install_release.ps1 -ZipPath .\hoteldruid-release.zip -InstallDir 'C:\Program Files\HotelDruid'
  .\install_release.ps1 -UseDeploymentConfig  # Use detected OneDrive + previous settings
#>
param(
    [string]$ZipPath = (Join-Path (Get-Location) 'hoteldruid-release.zip'),
    [string]$InstallDir = (Join-Path $env:LOCALAPPDATA 'HotelDruid'),
    [string]$DataFolder = "",
    [bool]$CreateStartMenuShortcut = $true,
    [bool]$CreateStartupShortcut = $false,
    [bool]$CreateDesktopShortcut = $false,
    [bool]$LaunchAfterInstall = $false,
    [switch]$UseDeploymentConfig,
    [ValidateSet('it', 'en', 'es')]
    [string]$Language
)

function Get-Lang {
    param([string]$Explicit)
    if ($Explicit) { return $Explicit }
    try {
        $name = (Get-Culture).Name
        if ($name -match '^it') { return 'it' }
        if ($name -match '^es') { return 'es' }
    } catch { }
    return 'en'
}

$lang = Get-Lang -Explicit $Language

$strings = @{
    en = @{
        ZipNotFound = 'Zip file not found: {0}'
        Installing  = 'Installing from {0} to {1}'
        DetectedExe = 'Detected exe: {0}'
        NoExeWarn   = 'No executable found automatically in {0}.'
        PromptExe   = 'Enter full path to the exe (or press Enter to skip)'
        ExeNotFound = 'Provided exe path not found.'
        SmCreated   = 'Start Menu shortcut created: {0}'
        StartupCreated = 'Startup shortcut created: {0}'
        DesktopCreated = 'Desktop shortcut created: {0}'
        NoExeShortcut = 'No executable available to create shortcuts.'
        Finished    = 'Installation finished.'
        Launching   = 'Launching app...'
        AutoStartNote = 'If you need the app to auto-start for all users or create a service, run with elevated rights and adjust shortcut targets accordingly.'
    }
    it = @{
        ZipNotFound = 'File zip non trovato: {0}'
        Installing  = 'Installazione da {0} a {1}'
        DetectedExe = 'Eseguibile rilevato: {0}'
        NoExeWarn   = 'Nessun eseguibile trovato automaticamente in {0}.'
        PromptExe   = "Inserisci il percorso completo dell'eseguibile (o premi Invio per saltare)"
        ExeNotFound = 'Percorso eseguibile non trovato.'
        SmCreated   = 'Collegamento nel menu Start creato: {0}'
        StartupCreated = 'Collegamento in Esecuzione automatica creato: {0}'
        DesktopCreated = 'Collegamento sul Desktop creato: {0}'
        NoExeShortcut = 'Nessun eseguibile disponibile per creare i collegamenti.'
        Finished    = 'Installazione completata.'
        Launching   = "Avvio dell'applicazione..."
        AutoStartNote = 'Se serve l''avvio automatico per tutti gli utenti o un servizio, eseguire con diritti elevati e regolare i collegamenti di conseguenza.'
    }
    es = @{
        ZipNotFound = 'Archivo zip no encontrado: {0}'
        Installing  = 'Instalando desde {0} a {1}'
        DetectedExe = 'Ejecutable detectado: {0}'
        NoExeWarn   = 'No se encontró ningún ejecutable automáticamente en {0}.'
        PromptExe   = 'Introduzca la ruta completa del exe (o Enter para omitir)'
        ExeNotFound = 'La ruta del exe proporcionada no existe.'
        SmCreated   = 'Acceso directo en el Menú Inicio creado: {0}'
        StartupCreated = 'Acceso directo en Inicio creado: {0}'
        DesktopCreated = 'Acceso directo en el Escritorio creado: {0}'
        NoExeShortcut = 'No hay un ejecutable disponible para crear accesos directos.'
        Finished    = 'Instalación finalizada.'
        Launching   = 'Iniciando la aplicación...'
        AutoStartNote = 'Si necesita inicio automático para todos los usuarios o un servicio, ejecute con permisos elevados y ajuste los accesos directos según corresponda.'
    }
}

$t = $strings[$lang]

# ============================================================================
# DEPLOYMENT CONFIGURATION STAGE
# ============================================================================

if ($UseDeploymentConfig) {
    Write-Host ""
    Write-Host "=== Deployment Configuration Helper ===" -ForegroundColor Cyan
    Write-Host "Detecting OneDrive and previous settings..." -ForegroundColor Yellow
    
    $deployScriptPath = Join-Path (Split-Path $PSCommandPath -Parent) 'deploy-hoteldruid-config.ps1'
    
    if (Test-Path $deployScriptPath) {
        try {
            # Run deployment config helper
            $deploymentSettings = & $deployScriptPath
            
            if ($deploymentSettings) {
                $InstallDir = $deploymentSettings.InstallDirectory
                $DataFolder = $deploymentSettings.DataDirectory
                Write-Host ""
                Write-Host "Configuration prepared. Proceeding with installation..." -ForegroundColor Green
                Write-Host ""
            }
        } catch {
            Write-Warning "Deployment config helper failed: $_"
            Write-Host "Continuing with default settings..." -ForegroundColor Yellow
        }
    } else {
        Write-Warning "Deployment config helper script not found at: $deployScriptPath"
    }
}

# ============================================================================
# INSTALLATION STAGE
# ============================================================================

if (-not (Test-Path -Path $ZipPath)) {
    Write-Error ($t.ZipNotFound -f $ZipPath)
    exit 2
}

Write-Host ($t.Installing -f $ZipPath, $InstallDir)

# Create destination
if (-not (Test-Path -Path $InstallDir)) {
    New-Item -Path $InstallDir -ItemType Directory -Force | Out-Null
}

# Expand
Expand-Archive -Path $ZipPath -DestinationPath $InstallDir -Force

# ============================================================================
# CONFIGURATION FILE GENERATION
# ============================================================================

# If DataFolder is still empty, use default or OneDrive path
if (-not $DataFolder) {
    $oneDrivePath = Join-Path $env:USERPROFILE 'OneDrive'
    if (Test-Path $oneDrivePath) {
        $DataFolder = Join-Path $oneDrivePath 'HotelDruid' 'hoteldruid' 'data'
    } else {
        $DataFolder = Join-Path $InstallDir 'hoteldruid' 'dati'
    }
}

# Create data folder if it doesn't exist
if (-not (Test-Path $DataFolder)) {
    New-Item -Path $DataFolder -ItemType Directory -Force | Out-Null
}

# Generate configuration file
$configPath = Join-Path $InstallDir 'hoteldruid' 'hoteldruid-config.php'
$phpDataPath = $DataFolder -replace '\\', '/'

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
//   Windows: "C:/Users/rolfe/Documents/HotelDruid" or "C:\\Users\\rolfe\\Documents\\HotelDruid"
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

$configContent | Set-Content $configPath -Encoding UTF8
Write-Host "Configuration file created: $configPath" -ForegroundColor Green

# ============================================================================
# SHORTCUTS CREATION
# ============================================================================

# Try to detect executable: prefer phpdesktop\phpdesktop-chrome.exe
$preferredExe = Join-Path $InstallDir 'phpdesktop\phpdesktop-chrome.exe'
$exe = $null
if (Test-Path -LiteralPath $preferredExe) {
    $exe = Get-Item -LiteralPath $preferredExe
} else {
    $exe = Get-ChildItem -Path $InstallDir -Filter *.exe -Recurse -File | Select-Object -First 1
}
if (-not $exe) {
    Write-Warning ($t.NoExeWarn -f $InstallDir)
    $exePath = Read-Host $t.PromptExe
    if ($exePath) {
        if (-not (Test-Path $exePath)) { Write-Error $t.ExeNotFound } else { $exe = Get-Item -LiteralPath $exePath }
    }
}

if ($exe) {
    $targetExe = $exe.FullName
    Write-Host ($t.DetectedExe -f $targetExe)

    $iconCandidate = Join-Path $InstallDir 'hoteldruid\img\favicon.ico'
    $iconLocation = $targetExe
    if (Test-Path -LiteralPath $iconCandidate) { $iconLocation = $iconCandidate }

    $workingDir = Split-Path $targetExe -Parent

    # Create a Start Menu shortcut for current user
    if ($CreateStartMenuShortcut) {
        $smpFolder = Join-Path $env:AppData 'Microsoft\Windows\Start Menu\Programs'
        $linkFolder = Join-Path $smpFolder 'HotelDruid'
        if (-not (Test-Path -Path $linkFolder)) { New-Item -Path $linkFolder -ItemType Directory -Force | Out-Null }
        $linkPath = Join-Path $linkFolder 'HotelDruid.lnk'
        $ws = New-Object -ComObject WScript.Shell
        $shortcut = $ws.CreateShortcut($linkPath)
        $shortcut.TargetPath = $targetExe
        $shortcut.WorkingDirectory = $workingDir
        $shortcut.IconLocation = $iconLocation
        $shortcut.Save()
        Write-Host ($t.SmCreated -f $linkPath)
    }

    if ($CreateStartupShortcut) {
        $startupFolder = Join-Path $env:AppData 'Microsoft\Windows\Start Menu\Programs\Startup'
        $linkPath2 = Join-Path $startupFolder 'HotelDruid.lnk'
        $ws2 = New-Object -ComObject WScript.Shell
        $shortcut2 = $ws2.CreateShortcut($linkPath2)
        $shortcut2.TargetPath = $targetExe
        $shortcut2.WorkingDirectory = $workingDir
        $shortcut2.IconLocation = $iconLocation
        $shortcut2.Save()
        Write-Host ($t.StartupCreated -f $linkPath2)
    }

    if ($CreateDesktopShortcut) {
        $desktopFolder = [Environment]::GetFolderPath('DesktopDirectory')
        $linkPath3 = Join-Path $desktopFolder 'HotelDruid.lnk'
        $ws3 = New-Object -ComObject WScript.Shell
        $shortcut3 = $ws3.CreateShortcut($linkPath3)
        $shortcut3.TargetPath = $targetExe
        $shortcut3.WorkingDirectory = $workingDir
        $shortcut3.IconLocation = $iconLocation
        $shortcut3.Save()
        Write-Host ($t.DesktopCreated -f $linkPath3)
    }

    if ($LaunchAfterInstall) {
        Write-Host $t.Launching
        Start-Process -FilePath $targetExe -WorkingDirectory $workingDir
    }
} else {
    Write-Warning $t.NoExeShortcut
}

Write-Host $t.Finished
Write-Host $t.AutoStartNote