<#
Enhanced HotelDruid Installer
Downloads latest sources from GitHub and phpdesktop runtime
Extracts, configures, and deploys to target location
Supports cloud storage with data preservation on redeploys

Usage (interactive with auto language detection from OS culture):
    .\install_release.ps1

Usage (override language if needed):
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
    [switch]$Quiet = $false,
    [ValidateSet('it', 'en', 'es')]
    [string]$Language,
    [string]$HoteldruidSource = '',
    [string]$GitHubRepo = 'RolfEleveld/hoteldruid',
    [string]$GitHubBranch = 'main',
    [string]$WorkDir = (Join-Path $env:TEMP ("hoteldruid_install_{0}" -f (Get-Random)))
)

# Setup of secure and default values for web requests
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
$PSDefaultParameterValues['Invoke-RestMethod:UserAgent'] = 'HotelDruid-Installer'


$ErrorActionPreference = 'Stop'
# ============================================================================
# VC++ REDISTRIBUTABLE (2015–2022 x64) HANDLING
# ============================================================================

function Set-UninstallRegistryReference([string]$InstallDir){
    # Where your app is installed
    $scriptPath = Join-Path $InstallDir "uninstall_release.ps1"

    # Registry uninstall key
    $regPath = "HKCU:\Software\Microsoft\Windows\CurrentVersion\Uninstall\HotelDruid.PHPDesktop"

    # Create the key
    New-Item -Path $regPath -Force | Out-Null

    # Set metadata
    Set-ItemProperty -Path $regPath -Name "DisplayName" -Value "HotelDruid PHPDesktop"
    Set-ItemProperty -Path $regPath -Name "Publisher" -Value "Rolf Eleveld"
    Set-ItemProperty -Path $regPath -Name "DisplayVersion" -Value "3.0.7"

    # IMPORTANT: wrap the PS1 in a PowerShell call
    $uninstallCmd = "powershell.exe -NoProfile -ExecutionPolicy Bypass -File `"$scriptPath`""

    Set-ItemProperty -Path $regPath -Name "UninstallString" -Value $uninstallCmd
    Set-ItemProperty -Path $regPath -Name "InstallLocation" -Value $InstallDir
    Set-ItemProperty -Path $regPath -Name "InstallDate" -Value (Get-Date -Format "yyyyMMdd")
    Set-ItemProperty -Path $regPath -Name "DisplayIcon" -Value (Join-Path $InstallDir "hoteldruid\icon.ico")
    Set-ItemProperty -Path $regPath -Name "EstimatedSize" -Value 1500000  # Size in KB (approx 150 MB)
    Write-Host "=================================================="
    Write-Host "Uninstall registry reference created at $regPath"
    Write-Host "=================================================="
}

function Test-VCRedistPresent {
    try {
        $sys = "$env:WINDIR\System32"
        $needed = @('vcruntime140.dll','vcruntime140_1.dll','msvcp140.dll')
        $dllsPresent = $true
        foreach ($dll in $needed) { if (-not (Test-Path (Join-Path $sys $dll))) { $dllsPresent = $false } }
        # Also check SysWOW64 for 32-bit scenarios, though we expect x64
        if (-not $dllsPresent) {
            $syswow = "$env:WINDIR\SysWOW64"
            $dllsPresent = $true
            foreach ($dll in $needed) { if (-not (Test-Path (Join-Path $syswow $dll))) { $dllsPresent = $false } }
        }

        $regPaths = @(
            'HKLM:\SOFTWARE\Microsoft\VisualStudio\14.0\VC\Runtimes\x64',
            'HKLM:\SOFTWARE\Microsoft\VisualStudio\15.0\VC\Runtimes\x64',
            'HKLM:\SOFTWARE\Microsoft\VisualStudio\16.0\VC\Runtimes\x64',
            'HKLM:\SOFTWARE\Microsoft\VisualStudio\17.0\VC\Runtimes\x64'
        )
        $regInstalled = $false
        foreach ($rp in $regPaths) {
            try {
                $v = Get-ItemProperty -Path $rp -ErrorAction Stop
                if ($null -ne $v -and $v.Installed -eq 1) { $regInstalled = $true; break }
            } catch {}
        }

        if ($dllsPresent -or $regInstalled) { return $true }
        return $false
    } catch { return $false }
}

function Get-VCRedistLocalPath {
    # Search repo root for VC++ installer
    $candidates = @(
        (Join-Path $PSScriptRoot 'vcredist_x64.exe'),
        (Join-Path $PSScriptRoot 'VC_redist.x64.exe'),
        'vcredist_x64.exe','VC_redist.x64.exe'
    )
    foreach ($c in $candidates) { if ($c -and (Test-Path -LiteralPath $c)) { return (Resolve-Path $c).Path } }
    return $null
}

function Download-VCRedist {
    param([string]$Destination)
    try {
        # Official Microsoft download URL (evergreen) often redirects to latest
        $url = 'https://aka.ms/vs/17/release/vc_redist.x64.exe'
        Invoke-WebRequest -Uri $url -OutFile $Destination -UseBasicParsing -TimeoutSec 300
        return $Destination
    } catch {
        throw "Failed to download VC++ Redistributable: $_"
    }
}

function Ensure-VCRedistInstalled {
    if (Test-VCRedistPresent) { return }
    Write-Host "VC++ Redistributable not detected. Installing..." -ForegroundColor Yellow
    $installer = Get-VCRedistLocalPath
    if (-not $installer) {
        $installer = Join-Path $env:TEMP "vc_redist.x64.exe"
        $installer = Download-VCRedist -Destination $installer
    }
    try {
        Write-Host "Running VC++ installer: $installer" -ForegroundColor Yellow
        # Elevate if not running as admin
        $isAdmin = ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
        $args = '/install /quiet /norestart'
        if ($isAdmin) {
            $proc = Start-Process -FilePath $installer -ArgumentList $args -Wait -PassThru -ErrorAction Stop
        } else {
            $proc = Start-Process -FilePath $installer -ArgumentList $args -Verb RunAs -Wait -PassThru -ErrorAction Stop
        }
        Write-Host "VC++ installer exit code: $($proc.ExitCode)" -ForegroundColor DarkGray
        Start-Sleep -Seconds 2
    } catch {
        throw "VC++ installation failed: $_"
    }
    if (-not (Test-VCRedistPresent)) {
        Write-Host "Primary install did not detect runtime; attempting repair..." -ForegroundColor Yellow
        try {
            $args2 = '/repair /norestart'
            $isAdmin = ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
            if ($isAdmin) {
                $proc2 = Start-Process -FilePath $installer -ArgumentList $args2 -Wait -PassThru -ErrorAction Stop
            } else {
                $proc2 = Start-Process -FilePath $installer -ArgumentList $args2 -Verb RunAs -Wait -PassThru -ErrorAction Stop
            }
            Write-Host "VC++ repair exit code: $($proc2.ExitCode)" -ForegroundColor DarkGray
            Start-Sleep -Seconds 2
        } catch {}
    }
    if (-not (Test-VCRedistPresent)) {
        # Check for pending reboot condition that may delay DLL registration
        try {
            $pending = (Get-ItemProperty -Path 'HKLM:\SYSTEM\CurrentControlSet\Control\Session Manager').PendingFileRenameOperations
            if ($pending) {
                Write-Host "VC++ installation may require a system restart to finalize." -ForegroundColor Yellow
            }
        } catch {}
        Write-Host "Warning: VC++ Redistributable still not detected after install." -ForegroundColor Yellow
        Write-Host "You may need to reboot the sandbox or run the installer manually: https://aka.ms/vs/17/release/vc_redist.x64.exe" -ForegroundColor Yellow
        # Proceed with rest of installation; PHP CGI may fail until runtime is present.
        return
    }
    Write-Host "VC++ Redistributable installed." -ForegroundColor Green
}

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
    return @{
        Name = 'phpdesktop-chrome-130.1-php-8.3.zip'
        Url  = 'https://github.com/cztomczak/phpdesktop/releases/download/chrome-v130.1/phpdesktop-chrome-130.1-php-8.3.zip'
        Tag  = 'chrome-130.1-php-8.3'
    }
}

## Removed unused helper functions with unapproved verbs to satisfy linter

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
    $settingsDir = Join-Path $env:APPDATA 'HotelDruid'
    $settingsFile = Join-Path $settingsDir 'deployment-settings.json'
    
    if (Test-Path -LiteralPath $settingsFile) {
        try {
            return Get-Content -LiteralPath $settingsFile -Raw | ConvertFrom-Json
        } catch {
            return $null
        }
    }
    return $null
}

function Save-DeploymentSettings {
    param([string]$InstallDir, [string]$DataFolder, [string]$OneDrivePath)
    
    $settingsDir = Join-Path $env:APPDATA 'HotelDruid'
    if (-not (Test-Path -LiteralPath $settingsDir)) {
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
    $settings | ConvertTo-Json | Set-Content -LiteralPath $settingsFile -Encoding UTF8
}

function New-ConfigFile {
    param([string]$InstallDir, [string]$DataFolder)
    
    $hoteldruidDir = Join-Path $InstallDir 'hoteldruid'
    if (-not (Test-Path -LiteralPath $hoteldruidDir)) {
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

function Merge-Objects {
    param(
        [Parameter(Mandatory=$true)]
        $Target,
        [Parameter(Mandatory=$true)]
        $Source
    )

    # Recursively merge Source into Target. Arrays are replaced, scalars are overwritten.
    if ($null -eq $Source) { return $Target }
    if ($null -eq $Target) { return $Source }

    # If both are custom objects, merge by properties
    if ($Target -is [pscustomobject] -and $Source -is [pscustomobject]) {
        foreach ($prop in $Source.PSObject.Properties) {
            $name = $prop.Name
            $srcVal = $prop.Value
            $tgtVal = $Target.PSObject.Properties[$name].Value

            if ($tgtVal -is [pscustomobject] -and $srcVal -is [pscustomobject]) {
                $merged = Merge-Objects -Target $tgtVal -Source $srcVal
                if ($Target.PSObject.Properties[$name]) {
                    $Target.PSObject.Properties[$name].Value = $merged
                } else {
                    $Target | Add-Member -NotePropertyName $name -NotePropertyValue $merged -Force
                }
            } elseif ($srcVal -is [System.Collections.IEnumerable] -and -not ($srcVal -is [string])) {
                # Replace arrays/lists entirely for determinism
                if ($Target.PSObject.Properties[$name]) {
                    $Target.PSObject.Properties[$name].Value = $srcVal
                } else {
                    $Target | Add-Member -NotePropertyName $name -NotePropertyValue $srcVal -Force
                }
            } else {
                # Overwrite scalar or add new property
                if ($Target.PSObject.Properties[$name]) {
                    $Target.PSObject.Properties[$name].Value = $srcVal
                } else {
                    $Target | Add-Member -NotePropertyName $name -NotePropertyValue $srcVal -Force
                }
            }
        }
        return $Target
    }

    # Fallback: if types differ, prefer Source
    return $Source
}

function Sanitize-PhpDesktopSettings {
    [Diagnostics.CodeAnalysis.SuppressMessageAttribute('PSUseApprovedVerbs','', Justification='Intentionally named for clarity; verb usage warnings are ignored per project policy')]
    param(
        [Parameter(Mandatory=$true)]
        $Settings
    )

    if ($null -eq $Settings) { return $Settings }

    # Clamp debugging.log_level to allowed values
    $allowedLogLevels = @('default','verbose','debug','info','warning','error','fatal')
    if ($Settings.debugging -and $Settings.debugging.PSObject.Properties['log_level']) {
        $lvl = $Settings.debugging.PSObject.Properties['log_level'].Value
        if ($null -eq $lvl -or ($allowedLogLevels -notcontains $lvl)) {
            $Settings.debugging.PSObject.Properties['log_level'].Value = 'info'
        }
    }

    # Ensure listen_on has a valid host and port (>0)
    if ($Settings.web_server -and $Settings.web_server.PSObject.Properties['listen_on']) {
        try {
            $arr = @($Settings.web_server.PSObject.Properties['listen_on'].Value)
            $currenthost = if ($arr.Count -ge 1 -and [string]::IsNullOrWhiteSpace($arr[0]) -eq $false) { $arr[0] } else { '127.0.0.1' }
            $port = if ($arr.Count -ge 2 -and [int]$arr[1] -gt 0) { [int]$arr[1] } else { 8080 }
            $Settings.web_server.PSObject.Properties['listen_on'].Value = @($currenthost, $port)
        } catch {
            $Settings.web_server.PSObject.Properties['listen_on'].Value = @('127.0.0.1', 8080)
        }
    }

    # Normalize web_server root key: ensure 'www_directory' is present (phpdesktop expects this)
    if ($Settings.web_server) {
        $ws = $Settings.web_server
        $hasDocRoot = $null -ne $ws.PSObject.Properties['document_root'] -and -not [string]::IsNullOrWhiteSpace($ws.document_root)
        $hasWwwDir  = $null -ne $ws.PSObject.Properties['www_directory'] -and -not [string]::IsNullOrWhiteSpace($ws.www_directory)
        if (-not $hasWwwDir -and $hasDocRoot) {
            $ws | Add-Member -NotePropertyName 'www_directory' -NotePropertyValue $ws.document_root -Force
        }
        # Do NOT remove www_directory; if both exist, prefer www_directory and drop document_root
        if ($hasDocRoot -and $hasWwwDir) {
            try { $ws.PSObject.Properties.Remove('document_root') } catch { }
        }
    }

    # Ensure application section has required basic metadata
    if ($Settings.application) {
        if (-not $Settings.application.PSObject.Properties['name'] -or [string]::IsNullOrWhiteSpace($Settings.application.name)) {
            $Settings.application | Add-Member -NotePropertyName 'name' -NotePropertyValue 'HotelDruid' -Force
        }
        if (-not $Settings.application.PSObject.Properties['version'] -or [string]::IsNullOrWhiteSpace($Settings.application.version)) {
            $Settings.application | Add-Member -NotePropertyName 'version' -NotePropertyValue '1.0.0' -Force
        }
    }

    # Remove unknown top-level keys to avoid strict parser issues
    $allowedTop = @('application','debugging','main_window','popup_window','web_server','chrome')
    $sanitized = [pscustomobject]@{}
    foreach ($k in $allowedTop) {
        if ($Settings.PSObject.Properties[$k]) {
            $sanitized | Add-Member -NotePropertyName $k -NotePropertyValue $Settings.PSObject.Properties[$k].Value -Force
        }
    }
    return $sanitized
}

function Update-PhpDesktopSettings {
    param(
        [string]$DataFolder,
        [string]$InstallDir,
        [string]$CustomSettingsPath
    )
    
    Write-Host $t['UpdatingPhpDesktop'] -ForegroundColor Yellow
    
    $phpDesktopSettingsPath = Join-Path $InstallDir 'phpdesktop'
    $phpDesktopSettingsPath = Join-Path $phpDesktopSettingsPath 'settings.json'
    
    if (-not (Test-Path -LiteralPath $phpDesktopSettingsPath)) {
        Write-Host $t['PhpDesktopUpdated'] -ForegroundColor Green
        return
    }
    
    try {
        # Load existing settings from phpdesktop
        $existingSettings = Get-Content -LiteralPath $phpDesktopSettingsPath -Raw | ConvertFrom-Json -ErrorAction Stop
        
        # Determine custom settings source in priority order
        $customSettingsFile = $null
        if ($CustomSettingsPath -and (Test-Path -LiteralPath $CustomSettingsPath)) {
            $customSettingsFile = $CustomSettingsPath
        } elseif ($PSScriptRoot -and (Test-Path (Join-Path $PSScriptRoot 'phpdesktop-custom-settings.json'))) {
            $customSettingsFile = Join-Path $PSScriptRoot 'phpdesktop-custom-settings.json'
        } elseif (Test-Path 'phpdesktop-custom-settings.json') {
            $customSettingsFile = Get-Item 'phpdesktop-custom-settings.json' | Select-Object -ExpandProperty FullName
        } elseif ($PSScriptRoot -and (Test-Path (Join-Path $PSScriptRoot 'phpdesktop\settings.json'))) {
            $customSettingsFile = Join-Path $PSScriptRoot 'phpdesktop\settings.json'
        } elseif ($MyInvocation.PSScriptRoot -and (Test-Path (Join-Path $MyInvocation.PSScriptRoot 'phpdesktop\settings.json'))) {
            $customSettingsFile = Join-Path $MyInvocation.PSScriptRoot 'phpdesktop\settings.json'
        }

        Write-Host "[DEBUG] PSScriptRoot=$PSScriptRoot" -ForegroundColor DarkGray
        Write-Host "[DEBUG] CustomSettingsFile=$customSettingsFile" -ForegroundColor DarkGray

        # Apply deep merge of custom settings if found
        if ($customSettingsFile -and (Test-Path -LiteralPath $customSettingsFile)) {
            Write-Host "[DEBUG] Loading custom settings from: $customSettingsFile" -ForegroundColor Green
            $customSettings = Get-Content -LiteralPath $customSettingsFile -Raw | ConvertFrom-Json -ErrorAction Stop
            # Sanitize source to avoid re-introducing deprecated keys and invalid values
            $customSettings = Sanitize-PhpDesktopSettings -Settings $customSettings
            $existingSettings = Merge-Objects -Target $existingSettings -Source $customSettings
        } else {
            Write-Host "[DEBUG] No custom settings file found" -ForegroundColor Yellow
        }

        # Enforce web root pointing to installed hoteldruid
        try {
            $absHotelDruid = Convert-Path (Join-Path $InstallDir 'hoteldruid') -ErrorAction Stop
            if (-not $existingSettings.web_server) {
                $existingSettings | Add-Member -NotePropertyName 'web_server' -NotePropertyValue ([pscustomobject]@{}) -Force
            }
            # Prefer standard key 'www_directory' (relative path with forward slashes)
            $wwwRelative = '../hoteldruid'
            if (-not $existingSettings.web_server.PSObject.Properties['www_directory']) {
                $existingSettings.web_server | Add-Member -NotePropertyName 'www_directory' -NotePropertyValue $wwwRelative -Force
            } else {
                $existingSettings.web_server.www_directory = $wwwRelative
            }
            # Remove non-standard 'document_root' to avoid schema issues
            try { $existingSettings.web_server.PSObject.Properties.Remove('document_root') } catch { }

            # Ensure index_files includes inizio.php first
            $existingSettings.web_server | Add-Member -NotePropertyName 'index_files' -NotePropertyValue @('inizio.php','index.html','index.php') -Force

            # Use relative CGI interpreter path for portability
            $existingSettings.web_server | Add-Member -NotePropertyName 'cgi_interpreter' -NotePropertyValue 'php/php-cgi.exe' -Force

            # Remove 404 handler if target file does not exist
            $pretty = Join-Path $absHotelDruid 'pretty-urls.php'
            if (-not (Test-Path -LiteralPath $pretty)) {
                try { $existingSettings.web_server.PSObject.Properties.Remove('404_handler') } catch { }
            }
        } catch { }

        # Sanitize invalid or unsupported values to prevent runtime parse errors
        $existingSettings = Sanitize-PhpDesktopSettings -Settings $existingSettings
        
        # Save merged settings - use absolute path to avoid null issues
        $absPath = Convert-Path -LiteralPath $phpDesktopSettingsPath -ErrorAction Stop
        $settingsJson = $existingSettings | ConvertTo-Json -Depth 10
        # Write as UTF-8 without BOM to avoid phpdesktop parser error
        $utf8NoBom = New-Object System.Text.UTF8Encoding($false)
        [System.IO.File]::WriteAllText($absPath, $settingsJson, $utf8NoBom)
        Write-Host $t['PhpDesktopUpdated'] -ForegroundColor Green
    } catch {
        Write-Host "Warning: Could not fully update phpdesktop settings: $_" -ForegroundColor Yellow
        Write-Host $t['PhpDesktopUpdated'] -ForegroundColor Green
    }
}

function Test-InstallConfiguration {
    param(
        [string]$InstallDir,
        [string]$DataFolder
    )

    $errors = @()

    # Validate phpdesktop settings.json
    $settingsPath = Join-Path (Join-Path $InstallDir 'phpdesktop') 'settings.json'
    if (-not (Test-Path -LiteralPath $settingsPath)) {
        $errors += "Missing phpdesktop settings.json at $settingsPath"
    } else {
        try {
            $settings = Get-Content -LiteralPath $settingsPath -Raw | ConvertFrom-Json -ErrorAction Stop
            if (-not $settings.web_server -or -not $settings.web_server.www_directory) {
                $errors += "phpdesktop settings missing web_server.www_directory"
            }
        } catch {
            $errors += "phpdesktop settings.json is invalid JSON: $_"
        }
    }

    # Validate hoteldruid entry file exists
    $wwwDir = Join-Path $InstallDir 'hoteldruid'
    $inizio = Join-Path $wwwDir 'inizio.php'
    if (-not (Test-Path -LiteralPath $inizio)) {
        $errors += "Missing inizio.php at $inizio"
    }

    # Validate external data folder exists and is writable
    if (-not (Test-Path -LiteralPath $DataFolder)) {
        $errors += "Data folder does not exist: $DataFolder"
    } else {
        $testFile = Join-Path $DataFolder ".__write_test__.tmp"
        try { "ok" | Set-Content -LiteralPath $testFile -ErrorAction Stop; Remove-Item -LiteralPath $testFile -Force -ErrorAction SilentlyContinue }
        catch { $errors += "Data folder is not writable: $DataFolder ($_ )" }
    }

    if ($errors.Count) {
        $report = ($errors -join "`n - ")
        Write-Host "Configuration validation failed:" -ForegroundColor Red
        Write-Host (" - {0}" -f $report) -ForegroundColor Red
        $logPath = Join-Path (Join-Path $env:APPDATA 'HotelDruid') 'install-validation.log'
        "Validation errors:`n$report" | Set-Content -LiteralPath $logPath -Encoding UTF8
        throw "Invalid configuration detected. See $logPath for details."
    } else {
        Write-Host "Configuration validation passed" -ForegroundColor Green
    }
}

function Find-PhpDesktopExe {
    param([string]$InstallDir)
    
    $exePath = Join-Path $InstallDir 'phpdesktop'
    $exePath = Join-Path $exePath 'phpdesktop-chrome.exe'
    if (Test-Path -LiteralPath $exePath) {
        return $exePath
    }
    
    $exePath = Join-Path $InstallDir 'phpdesktop'
    $exePath = Join-Path $exePath 'chrome'
    $exePath = Join-Path $exePath 'phpdesktop-chrome.exe'
    if (Test-Path -LiteralPath $exePath) {
        return $exePath
    }
    
    return $null
}

function New-Shortcut {
    param([string]$ShortcutPath, [string]$TargetPath, [string]$WorkingDir, [string]$IconPath)
    
    try {
        $ws = New-Object -ComObject WScript.Shell
        $shortcut = $ws.CreateShortcut($ShortcutPath)
        $shortcut.TargetPath = $TargetPath
        $shortcut.WorkingDirectory = $WorkingDir
        if ($IconPath) {
            $IconPath = (Resolve-Path -LiteralPath $IconPath).Path
            $shortcut.IconLocation = $IconPath
        }
        $shortcut.Save()
        return $true
    } catch {
        return $false
    }
}

function Stop-PhpDesktop {
    try {
        $processes = Get-Process -Name 'phpdesktop-chrome' -ErrorAction SilentlyContinue
        if ($processes) {
            Write-Host "Stopping phpdesktop..." -ForegroundColor Yellow
            $processes | Stop-Process -Force -ErrorAction SilentlyContinue
            Start-Sleep -Milliseconds 500
            Write-Host "[+] phpdesktop stopped" -ForegroundColor Green
        }
    } catch {
        # Silently continue if phpdesktop is not running
    }
}

# ============================================================================
# MAIN INSTALLATION
# ============================================================================

Write-Host ""
Write-Host "=== $($t['Title']) ===" -ForegroundColor Cyan
Write-Host ""

try {
    # Ensure VC++ runtime is present for php-cgi
    Ensure-VCRedistInstalled
    # Stop phpdesktop if it's running
    Stop-PhpDesktop
    
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
    
    # Download phpdesktop and HotelDruid sources with optimized settings
    Write-Host ""
    Write-Host "Step 1-2: Downloading required files..." -ForegroundColor Cyan
    
    $phpDesktopInfo = Get-LatestReleaseAssetUrl
    $phpDesktopZip = Join-Path $WorkDir $phpDesktopInfo.Name
    
    $hoteldruidZipUrl = $null
    $hoteldruidZip = $null
    $hoteldruidSourcePath = $null
    
    if ($HoteldruidSource -and (Test-Path $HoteldruidSource)) {
        Write-Host "Using local HotelDruid source" -ForegroundColor Yellow
        $hoteldruidSourcePath = $HoteldruidSource
    } else {
        $hoteldruidZipUrl = "https://github.com/$GitHubRepo/archive/refs/heads/$GitHubBranch.zip"
        $hoteldruidZip = Join-Path $WorkDir "hoteldruid-source.zip"
    }
    
    # Download with optimizations: faster timeouts, no progress bar overhead
    try {
        Write-Host ($t['Downloading'] -f "phpdesktop runtime") -ForegroundColor Yellow
        $ProgressPreference = 'SilentlyContinue'
        Invoke-WebRequest -Uri $phpDesktopInfo.Url -OutFile $phpDesktopZip -UseBasicParsing -TimeoutSec 600 -MaximumRedirection 5
        $ProgressPreference = 'Continue'
        Write-Host "[+] phpdesktop downloaded" -ForegroundColor Green
    } catch {
        throw ($t['DownloadFailed'] -f $_)
    }
    
    if ($hoteldruidZipUrl) {
        try {
            Write-Host ($t['Downloading'] -f "HotelDruid sources") -ForegroundColor Yellow
            $ProgressPreference = 'SilentlyContinue'
            Invoke-WebRequest -Uri $hoteldruidZipUrl -OutFile $hoteldruidZip -UseBasicParsing -TimeoutSec 600 -MaximumRedirection 5
            $ProgressPreference = 'Continue'
            Write-Host "[+] HotelDruid sources downloaded" -ForegroundColor Green
        } catch {
            throw ($t['DownloadFailed'] -f $_)
        }
    }
    
    # Extract packages (optimized - suppress progress for speed)
    Write-Host ""
    Write-Host "Step 3: Extracting packages..." -ForegroundColor Cyan
    $phpDesktopExtractDir = Join-Path $WorkDir 'phpdesktop-extracted'
    $hoteldruidExtractDir = Join-Path $WorkDir 'hoteldruid-extracted'
    
    # Clean and prepare directories
    if (Test-Path $phpDesktopExtractDir) {
        Remove-Item -Path $phpDesktopExtractDir -Recurse -Force
    }
    if (Test-Path $hoteldruidExtractDir) {
        Remove-Item -Path $hoteldruidExtractDir -Recurse -Force
    }
    New-Item -ItemType Directory -Path $phpDesktopExtractDir -Force | Out-Null
    New-Item -ItemType Directory -Path $hoteldruidExtractDir -Force | Out-Null
    
    # Extract with progress suppressed (faster)
    $ProgressPreference = 'SilentlyContinue'
    
    Write-Host ($t['Extracting'] -f (Split-Path $phpDesktopZip -Leaf)) -ForegroundColor Yellow
    Expand-Archive -Path $phpDesktopZip -DestinationPath $phpDesktopExtractDir -Force
    Write-Host "[+] phpdesktop extracted" -ForegroundColor Green
    
    if ($hoteldruidZip) {
        Write-Host ($t['Extracting'] -f (Split-Path $hoteldruidZip -Leaf)) -ForegroundColor Yellow
        Expand-Archive -Path $hoteldruidZip -DestinationPath $hoteldruidExtractDir -Force
        Write-Host "[+] HotelDruid sources extracted" -ForegroundColor Green
    } else {
        Write-Host "Copying local HotelDruid source..." -ForegroundColor Yellow
        Copy-Item -Path $hoteldruidSourcePath -Destination $hoteldruidExtractDir -Recurse -Force
        Write-Host "[+] HotelDruid sources copied" -ForegroundColor Green
    }
    
    $ProgressPreference = 'Continue'
    
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
        # For GitHub ZIP, look for extracted directory (e.g., hoteldruid-main)
        $repoRootDir = Get-ChildItem -Path $hoteldruidExtractDir -Directory | Select-Object -First 1
        if (-not $repoRootDir) {
            throw "Could not find extracted repository directory from GitHub"
        }
        
        # Look for hoteldruid subfolder within the extracted repo
        $hoteldruidSubfolder = Join-Path $repoRootDir.FullName 'hoteldruid'
        if (-not (Test-Path -LiteralPath $hoteldruidSubfolder)) {
            throw "Could not find 'hoteldruid' subfolder in extracted repository at $hoteldruidSubfolder"
        }
        
        $hoteldruidSourceDir = Get-Item -Path $hoteldruidSubfolder
        # Try to locate repo-provided phpdesktop settings for non-standard overrides
        $repoPhpDesktopSettings = Join-Path $repoRootDir.FullName 'phpdesktop'
        $repoPhpDesktopSettings = Join-Path $repoPhpDesktopSettings 'settings.json'
    }
    
    # Detect OneDrive
    Write-Host ""
    Write-Host "Step 4: Configuring data storage" -ForegroundColor Cyan
    $oneDrivePath = Find-OneDrive
    
    if ($oneDrivePath) {
        Write-Host ($t['OneDriveDetected'] -f $oneDrivePath) -ForegroundColor Green
        if ($DataFolder -eq "") {
            # Align with application expectations: use hoteldruid/dati directory name
            $DataFolder = Join-Path $oneDrivePath 'HotelDruid'
            $DataFolder = Join-Path $DataFolder 'dati'
            Write-Host ($t['OneDriveSuggested'] -f $DataFolder) -ForegroundColor Green
        }
    } else {
        Write-Host "OneDrive not detected, using local Documents folder" -ForegroundColor Yellow
        if ($DataFolder -eq "") {
            $DataFolder = Join-Path $env:USERPROFILE 'Documents'
            $DataFolder = Join-Path $DataFolder 'HotelDruid'
            $DataFolder = Join-Path $DataFolder 'dati'
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

    # Copy uninstaller if present to InstallDirectory
    $uninstallerSource = Join-Path $PSScriptRoot 'uninstall_release.ps1'
    if (Test-Path -LiteralPath $uninstallerSource) {
        $uninstallerDest = Join-Path $InstallDir 'uninstall_release.ps1'
        Copy-Item -LiteralPath $uninstallerSource -Destination $uninstallerDest -Force
        Write-Host "Uninstaller script copied to installation directory" -ForegroundColor Green
        Set-UninstallRegistryReference -InstallDir $InstallDir
    }
    
    # Copy phpdesktop
    Write-Host $t['CopyingFiles'] -ForegroundColor Yellow
    $destPhpDesktop = Join-Path $InstallDir 'phpdesktop'
    if (Test-Path $destPhpDesktop) {
        Remove-Item -Path $destPhpDesktop -Recurse -Force
    }
    Copy-Item -Path $phpDesktopRoot.FullName -Destination $destPhpDesktop -Recurse -Force

    # If package contains VC++ runtime DLLs, place them next to php-cgi.exe to allow PHP to start without system runtime
    try {
        $dllStage = $WorkDir
        # In minimal package, DLLs may be at root next to installer
        $dllStageAlt = Join-Path (Split-Path $PSCommandPath -Parent) 'vc_runtime_dlls'
        $dllSourceDir = $null
        if (Test-Path -LiteralPath $dllStage) { $dllSourceDir = $dllStage }
        elseif (Test-Path -LiteralPath $dllStageAlt) { $dllSourceDir = $dllStageAlt }
        if ($dllSourceDir) {
            $phpDir = Join-Path $destPhpDesktop 'php'
            Get-ChildItem -Path $dllSourceDir -Filter '*.dll' -File | ForEach-Object {
                Copy-Item -LiteralPath $_.FullName -Destination $phpDir -Force
            }
            Write-Host "Placed VC++ runtime DLLs next to php-cgi.exe" -ForegroundColor Green
        }
    } catch {
        Write-Host "Warning: failed to place VC++ DLLs: $_" -ForegroundColor Yellow
    }
    
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
    $configPath = New-ConfigFile -InstallDir $InstallDir -DataFolder $DataFolder.Replace('\','/')
    Write-Host ($t['ConfigCreated'] -f $configPath) -ForegroundColor Green
    
    # Update phpdesktop settings
    # Pass repo phpdesktop settings path when available for idempotent merge of non-standard overrides
    if ($repoPhpDesktopSettings -and (Test-Path -LiteralPath $repoPhpDesktopSettings)) {
        Update-PhpDesktopSettings -DataFolder $DataFolder -InstallDir $InstallDir -CustomSettingsPath $repoPhpDesktopSettings
    } else {
        Update-PhpDesktopSettings -DataFolder $DataFolder -InstallDir $InstallDir
    }
    
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
    $iconPath = Join-Path $InstallDir 'hoteldruid\icon.ico'
    
    if ($exePath) {
        Write-Host ($t['ExeFound'] -f $exePath) -ForegroundColor Green
        
        Write-Host $t['CreatingShortcuts'] -ForegroundColor Yellow
        
        if ($CreateStartMenuShortcut) {
            $smPath = Join-Path $env:APPDATA 'Microsoft\Windows\Start Menu\Programs'
            $smPath = Join-Path $smPath 'HotelDruid-phpdesktop.lnk'
            if (New-Shortcut -ShortcutPath $smPath -TargetPath $exePath -WorkingDir (Split-Path $exePath -Parent) -IconPath $iconPath) {
                Write-Host ($t['SmCreated'] -f $smPath) -ForegroundColor Green
            }
        }
        
        if ($CreateDesktopShortcut) {
            $desktopPath = Join-Path $env:USERPROFILE 'Desktop' 'HotelDruid-phpdesktop.lnk'
            if (New-Shortcut -ShortcutPath $desktopPath -TargetPath $exePath -WorkingDir (Split-Path $exePath -Parent) -IconPath $iconPath) {
                Write-Host ($t['DesktopCreated'] -f $desktopPath) -ForegroundColor Green
            }
        }
        
        if ($CreateStartupShortcut) {
            $startupPath = Join-Path $env:APPDATA 'Microsoft\Windows\Start Menu\Programs\Startup'
            $startupPath = Join-Path $startupPath 'HotelDruid-phpdesktop.lnk'
            if (New-Shortcut -ShortcutPath $startupPath -TargetPath $exePath -WorkingDir (Split-Path $exePath -Parent) -IconPath $iconPath) {
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
    
    # Post-install validation
    try { Test-InstallConfiguration -InstallDir $InstallDir -DataFolder $DataFolder } catch { Write-Host ($t['Error'] -f $_) -ForegroundColor Red; exit 1 }

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
