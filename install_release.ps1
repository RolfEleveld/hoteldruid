<#
Enhanced HotelDruid Installer
Downloads latest sources from GitHub and phpdesktop runtime
Extracts, configures, and deploys to target location
Supports cloud storage with data preservation on redeploys

Usage (interactive with language detection):
  .\install_release.ps1

Usage (with specific language):
  .\install_release.ps1 -Language es
  .\install_release.ps1 -Language en
  .\install_release.ps1 -Language it

Usage (with custom install directory):
  .\install_release.ps1 -Language es -InstallDir 'C:\HotelDruid'

Usage (custom data folder):
  .\install_release.ps1 -Language es -DataFolder 'C:\OneDrive\HotelDruid\data'

Usage (skip shortcuts):
  .\install_release.ps1 -Language es -CreateStartMenuShortcut:$false
#>

param(
    [string]$InstallDir = (Join-Path $env:LOCALAPPDATA 'HotelDruid'),
    [string]$DataFolder = "",
    [bool]$CreateStartMenuShortcut = $true,
    [bool]$CreateStartupShortcut = $false,
    [bool]$CreateDesktopShortcut = $false,
    [bool]$LaunchAfterInstall = $false,
    [ValidateSet('it', 'en', 'es')]
    [string]$Language,
    [string]$HoteldruidSource = '',
    [string]$GitHubRepo = 'hoteldruid/hoteldruid-community',
    [string]$GitHubBranch = 'main',
    [string]$WorkDir = (Join-Path $env:TEMP ("hoteldruid_install_{0}" -f (Get-Random)))
)

$ErrorActionPreference = 'Stop'

# ============================================================================
# LANGUAGE STRINGS
# ============================================================================

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
        Title                   = 'HotelDruid Installation'
        Downloading             = 'Downloading {0}...'
        DownloadFailed          = 'Download failed: {0}'
        Extracting              = 'Extracting {0}...'
        ExtractionFailed        = 'Extraction failed: {0}'
        Installing              = 'Installing HotelDruid to {0}'
        OneDriveDetected        = 'OneDrive detected at: {0}'
        OneDriveSuggested       = 'Suggested data folder: {0}'
        CreatingDataFolder      = 'Creating data folder...'
        DataFolderCreated       = 'Data folder created: {0}'
        CopyingFiles            = 'Copying application files...'
        FileCopied              = 'Files copied to: {0}'
        CreatingConfig          = 'Creating configuration files...'
        ConfigCreated           = 'Configuration created at: {0}'
        UpdatingPhpDesktop      = 'Updating phpdesktop settings...'
        PhpDesktopUpdated       = 'phpdesktop settings updated'
        SavingSettings          = 'Saving installation settings...'
        SettingsSaved           = 'Installation settings saved for future updates'
        SearchingExe            = 'Searching for phpdesktop executable...'
        ExeFound                = 'Executable found: {0}'
        ExeNotFound             = 'No executable found in installation'
        CreatingShortcuts       = 'Creating shortcuts...'
        SmCreated               = 'Start Menu shortcut created: {0}'
        DesktopCreated          = 'Desktop shortcut created: {0}'
        StartupCreated          = 'Startup shortcut created: {0}'
        Finished                = 'Installation finished successfully'
        Launching               = 'Launching application...'
        LoadingPrevious         = 'Loading previous installation settings...'
        CleaningUp              = 'Cleaning up temporary files...'
        Error                   = 'Error: {0}'
    }
    it = @{
        Title                   = 'Installazione HotelDruid'
        Downloading             = 'Download di {0}...'
        DownloadFailed          = 'Download fallito: {0}'
        Extracting              = 'Estrazione di {0}...'
        ExtractionFailed        = 'Estrazione fallita: {0}'
        Installing              = 'Installazione di HotelDruid in {0}'
        OneDriveDetected        = 'OneDrive rilevato in: {0}'
        OneDriveSuggested       = 'Cartella dati suggerita: {0}'
        CreatingDataFolder      = 'Creazione cartella dati...'
        DataFolderCreated       = 'Cartella dati creata: {0}'
        CopyingFiles            = 'Copia dei file dell''applicazione...'
        FileCopied              = 'File copiati in: {0}'
        CreatingConfig          = 'Creazione file di configurazione...'
        ConfigCreated           = 'Configurazione creata in: {0}'
        UpdatingPhpDesktop      = 'Aggiornamento impostazioni phpdesktop...'
        PhpDesktopUpdated       = 'Impostazioni phpdesktop aggiornate'
        SavingSettings          = 'Salvataggio impostazioni installazione...'
        SettingsSaved           = 'Impostazioni di installazione salvate per aggiornamenti futuri'
        SearchingExe            = 'Ricerca eseguibile phpdesktop...'
        ExeFound                = 'Eseguibile trovato: {0}'
        ExeNotFound             = 'Nessun eseguibile trovato nell''installazione'
        CreatingShortcuts       = 'Creazione collegamenti...'
        SmCreated               = 'Collegamento nel menu Start creato: {0}'
        DesktopCreated          = 'Collegamento sul Desktop creato: {0}'
        StartupCreated          = 'Collegamento in Esecuzione automatica creato: {0}'
        Finished                = 'Installazione completata con successo'
        Launching               = "Avvio dell'applicazione..."
        LoadingPrevious         = 'Caricamento impostazioni di installazione precedente...'
        CleaningUp              = 'Pulizia dei file temporanei...'
        Error                   = 'Errore: {0}'
    }
    es = @{
        Title                   = 'Instalación de HotelDruid'
        Downloading             = 'Descargando {0}...'
        DownloadFailed          = 'La descarga falló: {0}'
        Extracting              = 'Extrayendo {0}...'
        ExtractionFailed        = 'La extracción falló: {0}'
        Installing              = 'Instalando HotelDruid en {0}'
        OneDriveDetected        = 'OneDrive detectado en: {0}'
        OneDriveSuggested       = 'Carpeta de datos sugerida: {0}'
        CreatingDataFolder      = 'Creando carpeta de datos...'
        DataFolderCreated       = 'Carpeta de datos creada: {0}'
        CopyingFiles            = 'Copiando archivos de la aplicación...'
        FileCopied              = 'Archivos copiados en: {0}'
        CreatingConfig          = 'Creando archivos de configuración...'
        ConfigCreated           = 'Configuración creada en: {0}'
        UpdatingPhpDesktop      = 'Actualizando configuración de phpdesktop...'
        PhpDesktopUpdated       = 'Configuración de phpdesktop actualizada'
        SavingSettings          = 'Guardando configuración de instalación...'
        SettingsSaved           = 'Configuración de instalación guardada para futuras actualizaciones'
        SearchingExe            = 'Buscando ejecutable de phpdesktop...'
        ExeFound                = 'Ejecutable encontrado: {0}'
        ExeNotFound             = 'No se encontró ningún ejecutable en la instalación'
        CreatingShortcuts       = 'Creando accesos directos...'
        SmCreated               = 'Acceso directo en el Menú Inicio creado: {0}'
        DesktopCreated          = 'Acceso directo en el Escritorio creado: {0}'
        StartupCreated          = 'Acceso directo de Inicio creado: {0}'
        Finished                = 'Instalación completada exitosamente'
        Launching               = 'Iniciando la aplicación...'
        LoadingPrevious         = 'Cargando configuración de instalación anterior...'
        CleaningUp              = 'Limpiando archivos temporales...'
        Error                   = 'Error: {0}'
    }
}

$t = $strings[$lang]

# ============================================================================
# HELPER FUNCTIONS
# ============================================================================

function Get-LatestReleaseAssetUrl {
    param(
        [string]$Repo = 'cztomczak/phpdesktop',
        [string]$AssetNameRegex = 'phpdesktop-chrome-.*-php-.*\.zip$'
    )
    
    try {
        $uri = "https://api.github.com/repos/$Repo/releases/latest"
        $headers = @{
            'User-Agent' = 'HotelDruid-Installer'
            'Accept'     = 'application/vnd.github+json'
        }
        
        $release = Invoke-RestMethod -Uri $uri -Headers $headers -Method GET -TimeoutSec 30
        if (-not $release -or -not $release.assets) {
            throw "GitHub API returned no assets"
        }
        
        $asset = $release.assets | Where-Object { $_.name -match $AssetNameRegex } | Select-Object -First 1
        if (-not $asset) {
            throw "No matching asset found"
        }
        
        return @{
            Name = $asset.name
            Url  = $asset.browser_download_url
            Tag  = $release.tag_name
        }
    } catch {
        throw "Failed to fetch latest release: $_"
    }
}

function Download-File {
    param([string]$Url, [string]$OutputPath)
    
    try {
        Write-Host ($t['Downloading'] -f (Split-Path $OutputPath -Leaf)) -ForegroundColor Yellow
        Invoke-WebRequest -Uri $Url -OutFile $OutputPath -UseBasicParsing -TimeoutSec 300
    } catch {
        throw ($t['DownloadFailed'] -f $_)
    }
}

function Extract-ZipArchive {
    param([string]$ZipPath, [string]$DestinationPath)
    
    try {
        Write-Host ($t['Extracting'] -f (Split-Path $ZipPath -Leaf)) -ForegroundColor Yellow
        if (Test-Path $DestinationPath) {
            Remove-Item -Path $DestinationPath -Recurse -Force
        }
        New-Item -ItemType Directory -Path $DestinationPath -Force | Out-Null
        Expand-Archive -Path $ZipPath -DestinationPath $DestinationPath -Force
    } catch {
        throw ($t['ExtractionFailed'] -f $_)
    }
}

function Find-OneDrive {
    <# Try personal OneDrive first, then business #>
    $personalPath = Join-Path $env:USERPROFILE 'OneDrive'
    if (Test-Path $personalPath) { return $personalPath }
    
    try {
        $businessPath = Get-ItemProperty -Path 'HKCU:\Software\Microsoft\OneDrive\Accounts\Business1' -Name 'UserFolder' -ErrorAction SilentlyContinue | Select-Object -ExpandProperty 'UserFolder'
        if ($businessPath -and (Test-Path $businessPath)) { return $businessPath }
    } catch { }
    
    return $null
}

function Get-PreviousSettings {
    $settingsFile = Join-Path $env:APPDATA 'HotelDruid' 'deployment-settings.json'
    
    if (Test-Path $settingsFile) {
        try {
            return Get-Content $settingsFile -Raw | ConvertFrom-Json
        } catch {
            return $null
        }
    }
    return $null
}

function Save-DeploymentSettings {
    param([string]$InstallDir, [string]$DataFolder, [string]$OneDrivePath)
    
    $settingsDir = Join-Path $env:APPDATA 'HotelDruid'
    if (-not (Test-Path $settingsDir)) {
        New-Item -ItemType Directory -Path $settingsDir -Force | Out-Null
    }
    
    $settings = @{
        InstallDirectory = $InstallDir
        DataDirectory    = $DataFolder
        OneDrivePath     = $OneDrivePath
        DeploymentDate   = (Get-Date -Format 'yyyy-MM-dd HH:mm:ss')
        ComputerName     = $env:COMPUTERNAME
        UserName         = $env:USERNAME
    }
    
    $settingsFile = Join-Path $settingsDir 'deployment-settings.json'
    $settings | ConvertTo-Json | Set-Content $settingsFile -Encoding UTF8
}

function New-ConfigFile {
    param([string]$InstallDir, [string]$DataFolder)
    
    $hoteldruidDir = Join-Path $InstallDir 'hoteldruid'
    if (-not (Test-Path $hoteldruidDir)) {
        New-Item -ItemType Directory -Path $hoteldruidDir -Force | Out-Null
    }
    
    # Convert Windows path to forward slashes for JSON compatibility
    $dataFolderJson = $DataFolder -replace '\\', '/'
    
    $configPath = Join-Path $hoteldruidDir 'hoteldruid-config.php'
    
    $configContent = @"
<?php
/**
 * HotelDruid Configuration File
 * Auto-generated on $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')
 * Computer: $env:COMPUTERNAME
 * User: $env:USERNAME
 */

// External data directory (cloud storage for backup)
define('C_DATI_PATH_EXTERNAL', "$dataFolderJson");
?>
"@
    
    Set-Content -Path $configPath -Value $configContent -Encoding UTF8
    return $configPath
}

function Update-PhpDesktopSettings {
    param([string]$DataFolder, [string]$InstallDir)
    
    $phpDesktopSettingsPath = Join-Path $InstallDir 'phpdesktop' 'settings.json'
    
    if (-not (Test-Path $phpDesktopSettingsPath)) {
        return
    }
    
    try {
        Write-Host $t['UpdatingPhpDesktop'] -ForegroundColor Yellow
        $settings = Get-Content $phpDesktopSettingsPath -Raw | ConvertFrom-Json
        
        # Convert Windows path to forward slashes
        $dataFolderJson = $DataFolder -replace '\\', '/'
        
        # Ensure hoteldruid section exists
        if ($null -eq $settings.hoteldruid) {
            $settings | Add-Member -NotePropertyName 'hoteldruid' -NotePropertyValue @{}
        }
        
        # Update data_path
        $settings.hoteldruid.data_path = $dataFolderJson
        
        # Save updated settings
        $settings | ConvertTo-Json -Depth 10 | Set-Content $phpDesktopSettingsPath -Encoding UTF8
        Write-Host $t['PhpDesktopUpdated'] -ForegroundColor Green
    } catch {
        Write-Host "Warning: Could not update phpdesktop settings: $_" -ForegroundColor Yellow
    }
}

function Find-PhpDesktopExe {
    param([string]$InstallDir)
    
    $candidates = @(
        Join-Path $InstallDir 'phpdesktop' 'phpdesktop-chrome.exe',
        Join-Path $InstallDir 'phpdesktop' 'chrome' 'phpdesktop-chrome.exe'
    )
    
    foreach ($candidate in $candidates) {
        if (Test-Path $candidate) {
            return (Resolve-Path $candidate).Path
        }
    }
    
    return $null
}

function Create-Shortcut {
    param([string]$ShortcutPath, [string]$TargetPath, [string]$WorkingDir)
    
    try {
        $ws = New-Object -ComObject WScript.Shell
        $shortcut = $ws.CreateShortcut($ShortcutPath)
        $shortcut.TargetPath = $TargetPath
        $shortcut.WorkingDirectory = $WorkingDir
        $shortcut.Save()
        return $true
    } catch {
        return $false
    }
}

# ============================================================================
# MAIN INSTALLATION
# ============================================================================

Write-Host ""
Write-Host "=== $($t['Title']) ===" -ForegroundColor Cyan
Write-Host ""

try {
    # Create work directory
    if (-not (Test-Path $WorkDir)) {
        New-Item -ItemType Directory -Path $WorkDir -Force | Out-Null
    }
    
    # Check for previous settings
    $previousSettings = Get-PreviousSettings
    if ($previousSettings) {
        Write-Host $t['LoadingPrevious'] -ForegroundColor Yellow
        $InstallDir = $previousSettings.InstallDirectory
        if ($DataFolder -eq "") {
            $DataFolder = $previousSettings.DataDirectory
        }
    }
    
    # Download phpdesktop
    Write-Host ""
    Write-Host "Step 1: Downloading phpdesktop runtime" -ForegroundColor Cyan
    $phpDesktopInfo = Get-LatestReleaseAssetUrl
    $phpDesktopZip = Join-Path $WorkDir $phpDesktopInfo.Name
    Download-File -Url $phpDesktopInfo.Url -OutputPath $phpDesktopZip
    
    # Download HotelDruid sources from GitHub or use local
    Write-Host ""
    Write-Host "Step 2: Getting HotelDruid sources" -ForegroundColor Cyan
    
    if ($HoteldruidSource -and (Test-Path $HoteldruidSource)) {
        Write-Host "Using local HotelDruid source: $HoteldruidSource" -ForegroundColor Yellow
        $hoteldruidZip = $null
        $hoteldruidSourcePath = $HoteldruidSource
    } else {
        Write-Host "Downloading HotelDruid from GitHub..." -ForegroundColor Yellow
        $hoteldruidZipUrl = "https://github.com/$GitHubRepo/archive/refs/heads/$GitHubBranch.zip"
        $hoteldruidZip = Join-Path $WorkDir "hoteldruid-source.zip"
        Download-File -Url $hoteldruidZipUrl -OutputPath $hoteldruidZip
        $hoteldruidSourcePath = $null
    }
    
    # Extract both
    Write-Host ""
    Write-Host "Step 3: Extracting packages" -ForegroundColor Cyan
    $phpDesktopExtractDir = Join-Path $WorkDir 'phpdesktop-extracted'
    $hoteldruidExtractDir = Join-Path $WorkDir 'hoteldruid-extracted'
    
    Extract-ZipArchive -ZipPath $phpDesktopZip -DestinationPath $phpDesktopExtractDir
    
    if ($hoteldruidZip) {
        Extract-ZipArchive -ZipPath $hoteldruidZip -DestinationPath $hoteldruidExtractDir
    } else {
        # Copy local source directly
        if (Test-Path $hoteldruidExtractDir) {
            Remove-Item -Path $hoteldruidExtractDir -Recurse -Force
        }
        New-Item -ItemType Directory -Path $hoteldruidExtractDir -Force | Out-Null
        Copy-Item -Path $hoteldruidSourcePath -Destination $hoteldruidExtractDir -Recurse -Force
    }
    
    # Find phpdesktop root (may be in subdirectory)
    $phpDesktopRoot = Get-ChildItem -Path $phpDesktopExtractDir -Directory -Filter 'phpdesktop-*' | Select-Object -First 1
    if (-not $phpDesktopRoot) {
        $phpDesktopRoot = Get-ChildItem -Path $phpDesktopExtractDir -File -Filter 'phpdesktop-chrome.exe' | Select-Object -First 1
        if ($phpDesktopRoot) {
            $phpDesktopRoot = $phpDesktopRoot.Directory
        }
    }
    
    if (-not $phpDesktopRoot) {
        throw "Could not find phpdesktop root after extraction"
    }
    
    # Find hoteldruid root (may be in subdirectory or use direct path if local)
    if ($hoteldruidSourcePath) {
        # For local sources, use directly
        $hoteldruidSourceDir = Get-Item -Path $hoteldruidSourcePath
    } else {
        # For GitHub ZIP, look for extracted directory
        $hoteldruidSourceDir = Get-ChildItem -Path $hoteldruidExtractDir -Directory -Filter 'hoteldruid-*' | Select-Object -First 1
        if (-not $hoteldruidSourceDir) {
            throw "Could not find hoteldruid source after extraction from GitHub"
        }
    }
    
    # Detect OneDrive
    Write-Host ""
    Write-Host "Step 4: Configuring data storage" -ForegroundColor Cyan
    $oneDrivePath = Find-OneDrive
    
    if ($oneDrivePath) {
        Write-Host ($t['OneDriveDetected'] -f $oneDrivePath) -ForegroundColor Green
        if ($DataFolder -eq "") {
            $DataFolder = Join-Path $oneDrivePath 'HotelDruid' 'hoteldruid' 'data'
            Write-Host ($t['OneDriveSuggested'] -f $DataFolder) -ForegroundColor Green
        }
    } else {
        Write-Host "OneDrive not detected, using local Documents folder" -ForegroundColor Yellow
        if ($DataFolder -eq "") {
            $DataFolder = Join-Path $env:USERPROFILE 'Documents' 'HotelDruid' 'hoteldruid' 'data'
        }
    }
    
    # Create data folder
    Write-Host $t['CreatingDataFolder'] -ForegroundColor Yellow
    if (-not (Test-Path $DataFolder)) {
        New-Item -ItemType Directory -Path $DataFolder -Force | Out-Null
    }
    Write-Host ($t['DataFolderCreated'] -f $DataFolder) -ForegroundColor Green
    
    # Create install directory
    Write-Host ""
    Write-Host "Step 5: Installing application" -ForegroundColor Cyan
    Write-Host ($t['Installing'] -f $InstallDir) -ForegroundColor Cyan
    
    if (-not (Test-Path $InstallDir)) {
        New-Item -ItemType Directory -Path $InstallDir -Force | Out-Null
    }
    
    # Copy phpdesktop
    Write-Host $t['CopyingFiles'] -ForegroundColor Yellow
    $destPhpDesktop = Join-Path $InstallDir 'phpdesktop'
    if (Test-Path $destPhpDesktop) {
        Remove-Item -Path $destPhpDesktop -Recurse -Force
    }
    Copy-Item -Path $phpDesktopRoot.FullName -Destination $destPhpDesktop -Recurse -Force
    
    # Copy hoteldruid
    $destHotelDruid = Join-Path $InstallDir 'hoteldruid'
    if (Test-Path $destHotelDruid) {
        Remove-Item -Path $destHotelDruid -Recurse -Force
    }
    Copy-Item -Path $hoteldruidSourceDir.FullName -Destination $destHotelDruid -Recurse -Force
    
    Write-Host ($t['FileCopied'] -f $InstallDir) -ForegroundColor Green
    
    # Create configuration
    Write-Host ""
    Write-Host "Step 6: Creating configuration" -ForegroundColor Cyan
    Write-Host $t['CreatingConfig'] -ForegroundColor Yellow
    $configPath = New-ConfigFile -InstallDir $InstallDir -DataFolder $DataFolder
    Write-Host ($t['ConfigCreated'] -f $configPath) -ForegroundColor Green
    
    # Update phpdesktop settings
    Update-PhpDesktopSettings -DataFolder $DataFolder -InstallDir $InstallDir
    
    # Save settings for future deploys
    Write-Host ""
    Write-Host $t['SavingSettings'] -ForegroundColor Yellow
    Save-DeploymentSettings -InstallDir $InstallDir -DataFolder $DataFolder -OneDrivePath $oneDrivePath
    Write-Host $t['SettingsSaved'] -ForegroundColor Green
    
    # Create shortcuts
    Write-Host ""
    Write-Host "Step 7: Creating shortcuts" -ForegroundColor Cyan
    Write-Host $t['SearchingExe'] -ForegroundColor Yellow
    
    $exePath = Find-PhpDesktopExe -InstallDir $InstallDir
    
    if ($exePath) {
        Write-Host ($t['ExeFound'] -f $exePath) -ForegroundColor Green
        
        Write-Host $t['CreatingShortcuts'] -ForegroundColor Yellow
        
        if ($CreateStartMenuShortcut) {
            $smPath = Join-Path $env:APPDATA 'Microsoft\Windows\Start Menu\Programs' 'HotelDruid.lnk'
            if (Create-Shortcut -ShortcutPath $smPath -TargetPath $exePath -WorkingDir (Split-Path $exePath -Parent)) {
                Write-Host ($t['SmCreated'] -f $smPath) -ForegroundColor Green
            }
        }
        
        if ($CreateDesktopShortcut) {
            $desktopPath = Join-Path $env:USERPROFILE 'Desktop' 'HotelDruid.lnk'
            if (Create-Shortcut -ShortcutPath $desktopPath -TargetPath $exePath -WorkingDir (Split-Path $exePath -Parent)) {
                Write-Host ($t['DesktopCreated'] -f $desktopPath) -ForegroundColor Green
            }
        }
        
        if ($CreateStartupShortcut) {
            $startupPath = Join-Path $env:APPDATA 'Microsoft\Windows\Start Menu\Programs\Startup' 'HotelDruid.lnk'
            if (Create-Shortcut -ShortcutPath $startupPath -TargetPath $exePath -WorkingDir (Split-Path $exePath -Parent)) {
                Write-Host ($t['StartupCreated'] -f $startupPath) -ForegroundColor Green
            }
        }
    } else {
        Write-Host $t['ExeNotFound'] -ForegroundColor Yellow
    }
    
    # Cleanup
    Write-Host ""
    Write-Host $t['CleaningUp'] -ForegroundColor Yellow
    if (Test-Path $WorkDir) {
        Remove-Item -Path $WorkDir -Recurse -Force -ErrorAction SilentlyContinue
    }
    
    Write-Host ""
    Write-Host $t['Finished'] -ForegroundColor Green
    Write-Host ""
    
    if ($LaunchAfterInstall -and $exePath) {
        Write-Host $t['Launching'] -ForegroundColor Yellow
        & $exePath
    }
    
} catch {
    Write-Host ""
    Write-Host ($t['Error'] -f $_) -ForegroundColor Red
    Write-Host ""
    exit 1
}
