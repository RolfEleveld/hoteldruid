<#
.SYNOPSIS
  Start, install, or remove the HotelDruid desk on this computer.

.DESCRIPTION
  HotelDruid.cmd is the program a person opens. It checks that the desk answers,
  starts it when it is down, and then opens the browser.

  Install registers one startup task running as the machine account. The hotel
  files live in ProgramData, so every person who signs in on this computer shares
  that hotel. Uninstall writes the data export into the current user's Temp folder
  and leaves uninstall.json there. The next install reports that folder.

.EXAMPLE
  .\HotelDruid.cmd
  .\HotelDruid.cmd Install
  .\HotelDruid.cmd Uninstall
  .\HotelDruid.cmd Status
#>
param(
    [ValidateSet('Start', 'Install', 'Uninstall', 'Status', 'SelfTest')]
    [string]$Action = 'Start',
    [switch]$NoBrowser,
    [int]$HttpPort = 5000,
    [int]$HttpsPort = 5001
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$TaskName = 'HotelDruid'
$MachineRoot = Join-Path $env:ProgramData 'HotelDruid'
$MachineApp = Join-Path $MachineRoot 'app'
$MachineData = Join-Path $MachineRoot 'data'
$MachineLogs = Join-Path $MachineRoot 'logs'
$MetaPath = Join-Path $MachineRoot 'install-meta.json'
$UninstallKey = 'HKLM:\Software\Microsoft\Windows\CurrentVersion\Uninstall\HotelDruid'
$CertFriendlyName = 'HotelDruid LocalMachine Localhost'
$RuntimeDownload = 'https://dotnet.microsoft.com/download/dotnet/10.0'

function Test-IsAdministrator {
    $identity = [Security.Principal.WindowsIdentity]::GetCurrent()
    $principal = New-Object Security.Principal.WindowsPrincipal($identity)
    return $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
}

function Enter-Admin {
    if (Test-IsAdministrator) { return }
    $hostExe = "$env:SystemRoot\System32\WindowsPowerShell\v1.0\powershell.exe"
    $pwsh = Get-Command pwsh -ErrorAction SilentlyContinue
    if ($pwsh) { $hostExe = $pwsh.Source }
    $quoted = "-NoProfile -ExecutionPolicy Bypass -File `"$PSCommandPath`" -Action $Action"
    if ($NoBrowser) { $quoted += ' -NoBrowser' }
    Write-Host 'Administrator approval is required.' -ForegroundColor Yellow
    $elevated = Start-Process -FilePath $hostExe -ArgumentList $quoted -Verb RunAs -Wait -PassThru
    if ($elevated) { exit $elevated.ExitCode }
    exit 1
}

function Get-DotnetExe {
    $cmd = Get-Command dotnet -ErrorAction SilentlyContinue
    if ($cmd -and $cmd.Source) { return $cmd.Source }
    $candidate = Join-Path $env:ProgramFiles 'dotnet\dotnet.exe'
    if (Test-Path $candidate) { return $candidate }
    return $null
}

function Test-AspNetRuntime([string]$DotnetExe) {
    $runtimes = & $DotnetExe --list-runtimes 2>$null
    if (-not $runtimes) { return $false }
    return [bool]($runtimes | Where-Object { $_ -match 'Microsoft\.AspNetCore\.App 10\.' })
}

function Ensure-Dir([string]$Path) {
    if (-not (Test-Path $Path)) {
        New-Item -ItemType Directory -Path $Path -Force | Out-Null
    }
}

function Read-JsonFile([string]$Path) {
    if (-not (Test-Path $Path)) { return $null }
    return Get-Content -Path $Path -Raw | ConvertFrom-Json
}

function Get-DeskConfig {
    $meta = Read-JsonFile $MetaPath
    $https = $HttpsPort
    $http = $HttpPort
    $url = "https://localhost:$HttpsPort/"
    if ($meta) {
        if ($meta.PSObject.Properties.Name -contains 'httpsPort' -and $meta.httpsPort) { $https = [int]$meta.httpsPort }
        if ($meta.PSObject.Properties.Name -contains 'httpPort' -and $meta.httpPort) { $http = [int]$meta.httpPort }
        if ($meta.PSObject.Properties.Name -contains 'url' -and $meta.url) { $url = [string]$meta.url }
    }
    if ($url[-1] -ne '/') { $url = $url + '/' }
    [pscustomobject]@{
        Url = $url
        HealthUrl = ($url.TrimEnd('/') + '/health')
        HttpPort = $http
        HttpsPort = $https
        Meta = $meta
    }
}

function Test-MachineInstalled {
    return Test-Path $MetaPath
}

function Invoke-Curl {
    param([string[]]$ArgumentList)
    $hasPreference = Test-Path variable:PSNativeCommandUseErrorActionPreference
    $preference = $false
    if ($hasPreference) {
        $preference = $PSNativeCommandUseErrorActionPreference
        $PSNativeCommandUseErrorActionPreference = $false
    }
    try {
        return (& curl.exe @ArgumentList 2>$null)
    }
    finally {
        if ($hasPreference) { $PSNativeCommandUseErrorActionPreference = $preference }
    }
}

function Get-HealthCode([string]$HealthUrl) {
    if (-not (Get-Command curl.exe -ErrorAction SilentlyContinue)) {
        throw 'curl.exe was not found. It is part of Windows and is required to check the desk.'
    }
    $code = Invoke-Curl -ArgumentList @('-k', '-s', '-S', '-m', '4', '-o', 'NUL', '-w', '%{http_code}', $HealthUrl)
    if (-not $code) { return '000' }
    return ([string]$code).Trim()
}

function Get-UninstallMarkers {
    param([string[]]$Roots)
    $found = New-Object System.Collections.Generic.List[object]
    foreach ($root in $Roots) {
        if ([string]::IsNullOrWhiteSpace($root) -or -not (Test-Path $root)) { continue }
        $dirs = @(Get-ChildItem -Path $root -Directory -Filter 'HotelDruid-uninstall-*' -ErrorAction SilentlyContinue)
        foreach ($dir in $dirs) {
            $markerPath = Join-Path $dir.FullName 'uninstall.json'
            if (-not (Test-Path $markerPath)) { continue }
            try {
                $json = Get-Content -Path $markerPath -Raw | ConvertFrom-Json
            }
            catch {
                continue
            }
            $selfTest = $false
            if ($json.PSObject.Properties.Name -contains 'selfTest') { $selfTest = [bool]$json.selfTest }
            if ($selfTest) { continue }
            $at = ''
            $exportFile = ''
            if ($json.PSObject.Properties.Name -contains 'uninstalledAt') { $at = [string]$json.uninstalledAt }
            if ($json.PSObject.Properties.Name -contains 'exportFile') { $exportFile = [string]$json.exportFile }
            $found.Add([pscustomobject]@{
                Folder = $dir.FullName
                Marker = $markerPath
                At = $at
                ExportFile = $exportFile
            })
        }
    }
    return @($found | Sort-Object At -Descending)
}

function Get-UninstallSearchRoots {
    $roots = New-Object System.Collections.Generic.List[string]
    if ($env:TEMP) { $roots.Add($env:TEMP) }
    $windowsTemp = Join-Path $env:SystemRoot 'Temp'
    if (-not $roots.Contains($windowsTemp)) { $roots.Add($windowsTemp) }
    return $roots.ToArray()
}

function Show-PreviousUninstall {
    $markers = @(Get-UninstallMarkers -Roots (Get-UninstallSearchRoots))
    if ($markers.Count -eq 0) { return $null }
    $latest = $markers[0]
    Write-Host "A previous uninstall was found ($($markers.Count))." -ForegroundColor Yellow
    Write-Host "  When:   $($latest.At)"
    Write-Host "  Folder: $($latest.Folder)"
    if ($latest.ExportFile) { Write-Host "  Export: $($latest.ExportFile)" }
    Write-Host 'The new desk starts empty. Import that export from the desk when the previous hotel should come back.' -ForegroundColor Yellow
    return $latest
}

function Get-TaskState {
    try {
        $task = Get-ScheduledTask -TaskName $TaskName -ErrorAction Stop
        return [string]$task.State
    }
    catch {
        return 'NotInstalled'
    }
}

function Get-HotelDruidProcesses {
    $processes = @(Get-CimInstance Win32_Process -Filter "Name='dotnet.exe'" -ErrorAction SilentlyContinue)
    $matched = New-Object System.Collections.Generic.List[object]
    foreach ($process in $processes) {
        $cmd = [string]$process.CommandLine
        if ($cmd -and $cmd -match 'HotelDruid\.Api\.dll') { $matched.Add($process) }
    }
    return $matched.ToArray()
}

function Stop-HotelDruidAt([string]$AppDir) {
    $state = Get-TaskState
    if ($state -eq 'Running') {
        Write-Host 'Stopping the HotelDruid startup task...' -ForegroundColor Yellow
        Stop-ScheduledTask -TaskName $TaskName -ErrorAction SilentlyContinue
        $deadline = (Get-Date).AddSeconds(20)
        do {
            Start-Sleep -Seconds 1
            $state = Get-TaskState
        } while ($state -eq 'Running' -and (Get-Date) -lt $deadline)
    }

    $needle = [regex]::Escape($AppDir)
    foreach ($process in @(Get-HotelDruidProcesses)) {
        $cmd = [string]$process.CommandLine
        if ($cmd -match $needle) {
            Write-Host "Stopping process $($process.ProcessId)..." -ForegroundColor Yellow
            Stop-Process -Id $process.ProcessId -Force -ErrorAction SilentlyContinue
        }
    }
}

function Wait-Desk([string]$HealthUrl, [int]$Seconds) {
    $deadline = (Get-Date).AddSeconds($Seconds)
    do {
        $code = Get-HealthCode $HealthUrl
        if ($code -eq '200') { return $true }
        Start-Sleep -Seconds 1
    } while ((Get-Date) -lt $deadline)
    return $false
}

function Open-Desk([string]$Url) {
    if ($NoBrowser) { return }
    Start-Process $Url | Out-Null
}

function New-ServiceCommand {
    param(
        [string]$AppDir,
        [string]$DataDir,
        [string]$LogDir,
        [string]$DotnetExe,
        [string]$Thumbprint,
        [string]$CertLocation,
        [int]$LocalHttpPort,
        [int]$LocalHttpsPort
    )
    $cmdPath = Join-Path $AppDir 'run-service.cmd'
    $content = @"
@echo off
setlocal
set "HOTELDRUID_DATAROOT=$DataDir"
set "ASPNETCORE_ENVIRONMENT=Production"
set "ASPNETCORE_Kestrel__Certificates__Default__Store=My"
set "ASPNETCORE_Kestrel__Certificates__Default__Location=$CertLocation"
set "ASPNETCORE_Kestrel__Certificates__Default__Thumbprint=$Thumbprint"
cd /d "$AppDir"
if not exist "$LogDir" mkdir "$LogDir"
"$DotnetExe" HotelDruid.Api.dll --urls "http://localhost:$LocalHttpPort;https://localhost:$LocalHttpsPort" 1>> "$LogDir\api.log" 2>&1
"@
    Set-Content -Path $cmdPath -Value $content -Encoding ASCII
    return $cmdPath
}

function Grant-TaskStartToUsers {
    $service = New-Object -ComObject Schedule.Service
    $service.Connect()
    $task = $service.GetFolder('\').GetTask($TaskName)
    $sddl = [string]$task.GetSecurityDescriptor(0xF)
    $ace = '(A;;GRGX;;;BU)'
    if ($sddl.Contains($ace)) { return }
    if ($sddl.Contains('S:')) {
        $sddl = $sddl.Replace('S:', "$ace S:")
    }
    else {
        $sddl = $sddl.TrimEnd() + $ace
    }
    $task.SetSecurityDescriptor($sddl, 0)
}

function Register-DeskTask([string]$CommandPath) {
    # Task Scheduler starts the program directly. A .cmd file is not a program, so cmd.exe has to open it.
    $action = New-ScheduledTaskAction -Execute "$env:SystemRoot\System32\cmd.exe" -Argument "/c `"$CommandPath`"" -WorkingDirectory (Split-Path -Parent $CommandPath)
    $trigger = New-ScheduledTaskTrigger -AtStartup
    $principal = New-ScheduledTaskPrincipal -UserId 'NT AUTHORITY\LOCAL SERVICE' -LogonType ServiceAccount -RunLevel Limited
    $settings = New-ScheduledTaskSettingsSet `
        -AllowStartIfOnBatteries `
        -DontStopIfGoingOnBatteries `
        -StartWhenAvailable `
        -RestartCount 3 `
        -RestartInterval (New-TimeSpan -Minutes 1) `
        -MultipleInstances IgnoreNew
    Register-ScheduledTask -TaskName $TaskName -Action $action -Trigger $trigger -Principal $principal -Settings $settings -Force -Description 'Keeps the HotelDruid desk running for everyone on this computer.' | Out-Null

    $xml = Export-ScheduledTask -TaskName $TaskName
    $updated = $xml -replace '<ExecutionTimeLimit>.*?</ExecutionTimeLimit>', '<ExecutionTimeLimit>PT0S</ExecutionTimeLimit>'
    if ($updated -eq $xml) {
        $updated = $xml -replace '</Settings>', "<ExecutionTimeLimit>PT0S</ExecutionTimeLimit></Settings>"
    }
    Register-ScheduledTask -TaskName $TaskName -Xml $updated -Force | Out-Null
    try {
        Grant-TaskStartToUsers
    }
    catch {
        Write-Host "The startup task is registered. A person who is not an administrator may need an administrator to start it: $($_.Exception.Message)" -ForegroundColor Yellow
    }
}

function Test-LocalhostCertificate([System.Security.Cryptography.X509Certificates.X509Certificate2]$Certificate) {
    if (-not $Certificate.HasPrivateKey -or $Certificate.NotAfter -le (Get-Date)) { return $false }
    if ($Certificate.Subject -like '*CN=localhost*') { return $true }
    foreach ($name in @($Certificate.DnsNameList)) {
        $value = [string]$name
        if ($name.PSObject.Properties.Name -contains 'Unicode') { $value = [string]$name.Unicode }
        elseif ($name.PSObject.Properties.Name -contains 'Value') { $value = [string]$name.Value }
        if ($value -eq 'localhost') { return $true }
    }
    return $false
}

function Find-CertificateInStore([string]$StoreName, [string]$StoreLocation) {
    $store = New-Object System.Security.Cryptography.X509Certificates.X509Store($StoreName, $StoreLocation)
    $store.Open([System.Security.Cryptography.X509Certificates.OpenFlags]::ReadOnly)
    try {
        $found = @($store.Certificates | Where-Object { Test-LocalhostCertificate $_ } | Sort-Object NotAfter -Descending)
        if ($found.Count -gt 0) { return $found[0] }
    }
    finally {
        $store.Close()
    }
    return $null
}

function Grant-MachineKeyToSystem([System.Security.Cryptography.X509Certificates.X509Certificate2]$Certificate) {
    try {
    $rsa = [System.Security.Cryptography.X509Certificates.RSACertificateExtensions]::GetRSAPrivateKey($Certificate)
    $cng = $rsa -as [System.Security.Cryptography.RSACng]
    if (-not $cng) { return }
    $unique = $cng.Key.UniqueName
    $candidates = @(
        (Join-Path $env:ProgramData "Microsoft\Crypto\Keys\$unique"),
        (Join-Path $env:ProgramData "Microsoft\Crypto\RSA\MachineKeys\$unique")
    )
    $keyPath = $candidates | Where-Object { Test-Path $_ } | Select-Object -First 1
    if (-not $keyPath) { return }
    $acl = Get-Acl -Path $keyPath
    foreach ($sid in @('S-1-5-19', 'S-1-5-18')) {
        $account = New-Object System.Security.Principal.SecurityIdentifier $sid
        $rule = New-Object System.Security.AccessControl.FileSystemAccessRule($account, 'Read', 'Allow')
        $acl.AddAccessRule($rule)
    }
    Set-Acl -Path $keyPath -AclObject $acl
    }
    catch {
        Write-Host "The machine account may be unable to read the certificate key: $($_.Exception.Message)" -ForegroundColor Yellow
    }
}

function Trust-LocalMachineCertificate([System.Security.Cryptography.X509Certificates.X509Certificate2]$Certificate) {
    $raw = $Certificate.Export([System.Security.Cryptography.X509Certificates.X509ContentType]::Cert)
    $publicCert = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2 -ArgumentList (, $raw)
    $root = New-Object System.Security.Cryptography.X509Certificates.X509Store('Root', 'LocalMachine')
    $root.Open([System.Security.Cryptography.X509Certificates.OpenFlags]::ReadWrite)
    try {
        $existing = @($root.Certificates | Where-Object { $_.Thumbprint -eq $publicCert.Thumbprint })
        if ($existing.Count -eq 0) { $root.Add($publicCert) }
    }
    finally {
        $root.Close()
    }
}

function Get-OrCreateMachineCert {
    $existing = Find-CertificateInStore -StoreName 'My' -StoreLocation 'LocalMachine'
    if ($existing) {
        Trust-LocalMachineCertificate -Certificate $existing
        Grant-MachineKeyToSystem -Certificate $existing
        $created = $existing.FriendlyName -eq $CertFriendlyName
        return [pscustomobject]@{ Thumbprint = $existing.Thumbprint; Created = $created }
    }

    Write-Host 'Creating a localhost certificate for this computer...' -ForegroundColor Cyan
    $createdCert = New-SelfSignedCertificate `
        -DnsName 'localhost' `
        -CertStoreLocation 'Cert:\LocalMachine\My' `
        -FriendlyName $CertFriendlyName `
        -NotAfter (Get-Date).AddYears(5) `
        -KeyExportPolicy NonExportable `
        -KeyUsage DigitalSignature, KeyEncipherment
    Trust-LocalMachineCertificate -Certificate $createdCert
    Grant-MachineKeyToSystem -Certificate $createdCert
    return [pscustomobject]@{ Thumbprint = $createdCert.Thumbprint; Created = $true }
}

function Get-OrCreateUserCert {
    $existing = Find-CertificateInStore -StoreName 'My' -StoreLocation 'CurrentUser'
    if ($existing) { return $existing.Thumbprint }
    $createdCert = New-SelfSignedCertificate `
        -DnsName 'localhost' `
        -CertStoreLocation 'Cert:\CurrentUser\My' `
        -FriendlyName 'HotelDruid CurrentUser Localhost' `
        -NotAfter (Get-Date).AddYears(2) `
        -KeyUsage DigitalSignature, KeyEncipherment
    $raw = $createdCert.Export([System.Security.Cryptography.X509Certificates.X509ContentType]::Cert)
    $publicCert = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2 -ArgumentList (, $raw)
    $root = New-Object System.Security.Cryptography.X509Certificates.X509Store('Root', 'CurrentUser')
    $root.Open([System.Security.Cryptography.X509Certificates.OpenFlags]::ReadWrite)
    try { $root.Add($publicCert) }
    finally { $root.Close() }
    return $createdCert.Thumbprint
}

function Remove-MachineCert([string]$Thumbprint, [bool]$CreatedByHotelDruid) {
    if (-not $CreatedByHotelDruid -or [string]::IsNullOrWhiteSpace($Thumbprint)) { return }
    foreach ($storeName in @('My', 'Root')) {
        $store = New-Object System.Security.Cryptography.X509Certificates.X509Store($storeName, 'LocalMachine')
        $store.Open([System.Security.Cryptography.X509Certificates.OpenFlags]::ReadWrite)
        try {
            $matches = $store.Certificates.Find(
                [System.Security.Cryptography.X509Certificates.X509FindType]::FindByThumbprint,
                $Thumbprint,
                $false)
            foreach ($cert in $matches) {
                if ($storeName -eq 'My' -and $cert.FriendlyName -ne $CertFriendlyName) { continue }
                $store.Remove($cert)
            }
        }
        finally {
            $store.Close()
        }
    }
}

function Protect-ServiceFolder([string]$Path, [System.Security.AccessControl.FileSystemRights]$Rights) {
    Ensure-Dir $Path
    $acl = New-Object System.Security.AccessControl.DirectorySecurity
    $inherit = [System.Security.AccessControl.InheritanceFlags]'ContainerInherit, ObjectInherit'
    $none = [System.Security.AccessControl.PropagationFlags]::None
    $allow = [System.Security.AccessControl.AccessControlType]::Allow
    $admins = New-Object System.Security.Principal.SecurityIdentifier 'S-1-5-32-544'
    $service = New-Object System.Security.Principal.SecurityIdentifier 'S-1-5-19'
    $acl.SetAccessRuleProtection($true, $false)
    $acl.AddAccessRule((New-Object System.Security.AccessControl.FileSystemAccessRule($admins, 'FullControl', $inherit, $none, $allow)))
    $acl.AddAccessRule((New-Object System.Security.AccessControl.FileSystemAccessRule($service, $Rights, $inherit, $none, $allow)))
    Set-Acl -Path $Path -AclObject $acl
}

function Protect-AppDirectory([string]$Path) {
    Ensure-Dir $Path
    $acl = New-Object System.Security.AccessControl.DirectorySecurity
    $inherit = [System.Security.AccessControl.InheritanceFlags]'ContainerInherit, ObjectInherit'
    $none = [System.Security.AccessControl.PropagationFlags]::None
    $allow = [System.Security.AccessControl.AccessControlType]::Allow
    $admins = New-Object System.Security.Principal.SecurityIdentifier 'S-1-5-32-544'
    $users = New-Object System.Security.Principal.SecurityIdentifier 'S-1-5-32-545'
    $service = New-Object System.Security.Principal.SecurityIdentifier 'S-1-5-19'
    $acl.SetAccessRuleProtection($true, $false)
    $acl.AddAccessRule((New-Object System.Security.AccessControl.FileSystemAccessRule($admins, 'FullControl', $inherit, $none, $allow)))
    $acl.AddAccessRule((New-Object System.Security.AccessControl.FileSystemAccessRule($users, 'ReadAndExecute', $inherit, $none, $allow)))
    $acl.AddAccessRule((New-Object System.Security.AccessControl.FileSystemAccessRule($service, 'ReadAndExecute', $inherit, $none, $allow)))
    Set-Acl -Path $Path -AclObject $acl
}

function Copy-Package([string]$Source, [string]$Destination) {
    Ensure-Dir $Destination
    Get-ChildItem -Path $Destination -Force -ErrorAction SilentlyContinue | Remove-Item -Recurse -Force
    Ensure-Dir $Destination
    foreach ($item in @(Get-ChildItem -Path $Source -Force)) {
        if ($item.Name -eq 'data' -or $item.Name -eq 'logs') { continue }
        Copy-Item -Path $item.FullName -Destination (Join-Path $Destination $item.Name) -Recurse -Force
    }
    if (-not (Test-Path (Join-Path $Destination 'HotelDruid.Api.dll'))) {
        throw "HotelDruid.Api.dll was not copied to $Destination."
    }
    Protect-AppDirectory $Destination
}

function New-DeskShortcut([string]$ShortcutPath, [string]$TargetPath, [string]$Arguments, [int]$IconIndex) {
    $shell = New-Object -ComObject WScript.Shell
    $shortcut = $shell.CreateShortcut($ShortcutPath)
    $shortcut.TargetPath = $TargetPath
    $shortcut.Arguments = $Arguments
    $shortcut.WorkingDirectory = Split-Path -Parent $TargetPath
    $shortcut.IconLocation = "$env:SystemRoot\System32\shell32.dll,$IconIndex"
    $shortcut.Save()
}

function Register-UninstallEntry([string]$AppDir, [string]$Version) {
    $cmd = Join-Path $AppDir 'HotelDruid.cmd'
    New-Item -Path $UninstallKey -Force | Out-Null
    Set-ItemProperty -Path $UninstallKey -Name 'DisplayName' -Value 'HotelDruid'
    Set-ItemProperty -Path $UninstallKey -Name 'DisplayVersion' -Value $Version
    Set-ItemProperty -Path $UninstallKey -Name 'Publisher' -Value 'HotelDruid'
    Set-ItemProperty -Path $UninstallKey -Name 'InstallLocation' -Value $MachineRoot
    Set-ItemProperty -Path $UninstallKey -Name 'UninstallString' -Value "`"$cmd`" Uninstall"
    Set-ItemProperty -Path $UninstallKey -Name 'DisplayIcon' -Value (Join-Path $AppDir 'HotelDruid.Api.dll')
    Set-ItemProperty -Path $UninstallKey -Name 'NoModify' -Value 1 -Type DWord
    Set-ItemProperty -Path $UninstallKey -Name 'NoRepair' -Value 1 -Type DWord
}

function Get-PackageVersion([string]$AppDir) {
    $dll = Join-Path $AppDir 'HotelDruid.Api.dll'
    if (Test-Path $dll) {
        $info = [Diagnostics.FileVersionInfo]::GetVersionInfo($dll)
        if ($info.FileVersion) { return [string]$info.FileVersion }
        if ($info.ProductVersion) { return [string]$info.ProductVersion }
    }
    return '3.0.7'
}

function Test-ZipFile([string]$Path) {
    if (-not (Test-Path $Path)) { return $false }
    $stream = [System.IO.File]::OpenRead($Path)
    try {
        if ($stream.Length -lt 4) { return $false }
        $first = $stream.ReadByte()
        $second = $stream.ReadByte()
        return ($first -eq 0x50 -and $second -eq 0x4B)
    }
    finally {
        $stream.Close()
    }
}

function Save-DataExport {
    param(
        [string]$HealthUrl,
        [string]$BaseUrl,
        [string]$DataDir
    )
    $stamp = Get-Date -Format 'yyyyMMdd-HHmmss'
    $dest = Join-Path $env:TEMP "HotelDruid-uninstall-$stamp"
    Ensure-Dir $dest
    $exportFile = Join-Path $dest 'hoteldruid-export.zip'
    $exportError = ''
    $saved = $false

    $health = Get-HealthCode $HealthUrl
    if ($health -ne '200') {
        $state = Get-TaskState
        if ($state -eq 'Running') {
            Wait-Desk -HealthUrl $HealthUrl -Seconds 45 | Out-Null
        }
        elseif ($state -eq 'Ready') {
            Write-Host 'Starting the desk so the export can be taken...' -ForegroundColor Cyan
            Start-ScheduledTask -TaskName $TaskName -ErrorAction SilentlyContinue
            Wait-Desk -HealthUrl $HealthUrl -Seconds 45 | Out-Null
        }
    }

    $health = Get-HealthCode $HealthUrl
    if ($health -eq '200') {
        Write-Host 'Writing the data export...' -ForegroundColor Cyan
        $createBody = Join-Path $dest 'create.json'
        $code = Invoke-Curl -ArgumentList @('-k', '-s', '-S', '-m', '300', '-o', $createBody, '-w', '%{http_code}', '-X', 'POST', '-H', 'Content-Type: application/json', '-d', '{}', ($BaseUrl.TrimEnd('/') + '/api/export/create'))
        if (([string]$code).Trim() -eq '200' -and (Test-Path $createBody)) {
            try {
                $created = Get-Content -Path $createBody -Raw | ConvertFrom-Json
                $exportId = [string]$created.ExportId
                $status = [string]$created.Status
                $deadline = (Get-Date).AddMinutes(5)
                while ($status -ne 'completed' -and $status -ne 'failed' -and (Get-Date) -lt $deadline) {
                    Start-Sleep -Seconds 2
                    $statusBody = Join-Path $dest 'status.json'
                    Invoke-Curl -ArgumentList @('-k', '-s', '-S', '-m', '30', '-o', $statusBody, ($BaseUrl.TrimEnd('/') + "/api/export/$exportId/status")) | Out-Null
                    $statusJson = Get-Content -Path $statusBody -Raw | ConvertFrom-Json
                    $status = [string]$statusJson.Status
                }
                if ($status -eq 'completed') {
                    Invoke-Curl -ArgumentList @('-k', '-s', '-S', '-m', '300', '-o', $exportFile, ($BaseUrl.TrimEnd('/') + "/api/export/$exportId/download")) | Out-Null
                    $saved = Test-ZipFile $exportFile
                    if (-not $saved) { $exportError = 'The export download was not a zip file.' }
                }
                else {
                    $exportError = "Export status was '$status'."
                }
            }
            catch {
                $exportError = $_.Exception.Message
            }
        }
        else {
            $exportError = "Export request returned HTTP $code."
        }
    }
    else {
        $exportError = "The desk did not answer ($health), so the export API could not run."
    }

    $fallback = ''
    if (-not $saved -and (Test-Path $DataDir)) {
        $fileCount = @(Get-ChildItem -Path $DataDir -Recurse -File -ErrorAction SilentlyContinue).Count
        if ($fileCount -gt 0) {
            Write-Host 'The export did not complete. Copying the hotel folder instead...' -ForegroundColor Yellow
            $fallback = Join-Path $dest 'data'
            Ensure-Dir $fallback
            foreach ($dir in @(Get-ChildItem -Path $DataDir -Directory -Recurse -ErrorAction SilentlyContinue)) {
                $relative = $dir.FullName.Substring($DataDir.Length).TrimStart('\')
                if ($relative) { Ensure-Dir (Join-Path $fallback $relative) }
            }
            foreach ($file in @(Get-ChildItem -Path $DataDir -File -Recurse -ErrorAction SilentlyContinue)) {
                $relative = $file.FullName.Substring($DataDir.Length).TrimStart('\')
                $target = Join-Path $fallback $relative
                Ensure-Dir (Split-Path -Parent $target)
                Copy-Item -Path $file.FullName -Destination $target -Force
            }
            $copied = @(Get-ChildItem -Path $fallback -Recurse -File -ErrorAction SilentlyContinue).Count
            if ($copied -eq 0) {
                throw "The hotel data copy in $dest is empty. Uninstall stopped and the desk was left in place. $exportError"
            }
        }
    }
    elseif (-not $saved -and -not (Test-Path $DataDir)) {
        $exportError = 'There was no hotel folder to export.'
    }

    if (-not $saved -and -not $fallback -and (Test-Path $DataDir)) {
        $fileCount = @(Get-ChildItem -Path $DataDir -Recurse -File -ErrorAction SilentlyContinue).Count
        if ($fileCount -gt 0) {
            throw "Uninstall stopped because the hotel data was not saved. $exportError"
        }
    }

    $marker = [ordered]@{
        uninstalledAt = (Get-Date).ToString('o')
        user = [Environment]::UserName
        machine = [Environment]::MachineName
        dataDir = $DataDir
        exportFile = $(if ($saved) { $exportFile } else { '' })
        fallbackCopy = $fallback
        exportError = $exportError
        selfTest = $false
    }
    $marker | ConvertTo-Json | Set-Content -Path (Join-Path $dest 'uninstall.json') -Encoding UTF8
    if ($saved) {
        Write-Host "Export saved to $exportFile" -ForegroundColor Green
    }
    elseif ($fallback) {
        Write-Host "Hotel folder copied to $fallback" -ForegroundColor Green
    }
    else {
        Write-Host "No hotel data was stored. A marker was left at $dest" -ForegroundColor Yellow
    }
    return $dest
}

function Invoke-Start {
    $config = Get-DeskConfig
    $code = Get-HealthCode $config.HealthUrl
    if ($code -eq '200') {
        Write-Host "HotelDruid is running at $($config.Url)" -ForegroundColor Green
        Open-Desk $config.Url
        return 0
    }

    if (Test-MachineInstalled) {
        $state = Get-TaskState
        if ($state -eq 'NotInstalled') {
            Write-Host 'The desk is installed but the startup task is missing. Run HotelDruid.cmd Install.' -ForegroundColor Yellow
            return 1
        }
        Write-Host 'Starting the HotelDruid service...' -ForegroundColor Cyan
        try {
            Start-ScheduledTask -TaskName $TaskName
        }
        catch {
            Write-Host "The startup task could not be started: $($_.Exception.Message)" -ForegroundColor Yellow
            Write-Host "The log is $(Join-Path $MachineLogs 'api.log')"
            return 1
        }
    }
    else {
        $appDir = $PSScriptRoot
        if (-not (Test-Path (Join-Path $appDir 'HotelDruid.Api.dll'))) {
            Write-Host "HotelDruid is not installed and HotelDruid.Api.dll is not in $appDir." -ForegroundColor Yellow
            Write-Host 'Unzip HotelDruid-desktop.zip and run HotelDruid.cmd Install, or run HotelDruid.cmd from that folder.'
            return 1
        }
        $dotnet = Get-DotnetExe
        if (-not $dotnet -or -not (Test-AspNetRuntime $dotnet)) {
            Write-Host "The ASP.NET Core 10 runtime is required. Install it from $RuntimeDownload" -ForegroundColor Yellow
            return 1
        }
        $dataDir = Join-Path $appDir 'data'
        $logDir = Join-Path $appDir 'logs'
        Ensure-Dir $dataDir
        Ensure-Dir $logDir
        $thumbprint = Get-OrCreateUserCert
        Write-Host "Starting HotelDruid from $appDir" -ForegroundColor Cyan
        $cmdPath = New-ServiceCommand -AppDir $appDir -DataDir $dataDir -LogDir $logDir -DotnetExe $dotnet -Thumbprint $thumbprint -CertLocation 'CurrentUser' -LocalHttpPort $config.HttpPort -LocalHttpsPort $config.HttpsPort
        Start-Process -FilePath $cmdPath -WorkingDirectory $appDir -WindowStyle Hidden | Out-Null
    }

    Write-Host 'Waiting for the desk...' -ForegroundColor Cyan
    if (Wait-Desk -HealthUrl $config.HealthUrl -Seconds 45) {
        $config = Get-DeskConfig
        Write-Host "HotelDruid is running at $($config.Url)" -ForegroundColor Green
        Open-Desk $config.Url
        return 0
    }

    Write-Host 'The desk did not answer on /health within 45 seconds.' -ForegroundColor Yellow
    Write-Host "Log: $(Join-Path $MachineLogs 'api.log')"
    return 1
}

function Install-QuietRuntime([string]$Url, [string]$FileName) {
    $dest = Join-Path $env:TEMP $FileName
    Write-Host "Downloading $Url" -ForegroundColor Cyan
    $hasPreference = Test-Path variable:PSNativeCommandUseErrorActionPreference
    $preference = $false
    if ($hasPreference) {
        $preference = $PSNativeCommandUseErrorActionPreference
        $PSNativeCommandUseErrorActionPreference = $false
    }
    try {
        & curl.exe -L --fail --retry 3 -o $dest $Url
        $code = $LASTEXITCODE
    }
    finally {
        if ($hasPreference) { $PSNativeCommandUseErrorActionPreference = $preference }
    }
    if ($code -ne 0 -or -not (Test-Path $dest)) { throw "Could not download $Url" }
    $proc = Start-Process -FilePath $dest -ArgumentList '/install', '/quiet', '/norestart' -Wait -PassThru
    if ($proc.ExitCode -ne 0 -and $proc.ExitCode -ne 3010 -and $proc.ExitCode -ne 1638) {
        throw "The runtime installer returned $($proc.ExitCode)."
    }
}

function Install-RuntimeWithWinget([string]$Id) {
    $winget = Get-Command winget.exe -ErrorAction SilentlyContinue
    if (-not $winget) { return $false }
    $hasPreference = Test-Path variable:PSNativeCommandUseErrorActionPreference
    $preference = $false
    if ($hasPreference) {
        $preference = $PSNativeCommandUseErrorActionPreference
        $PSNativeCommandUseErrorActionPreference = $false
    }
    try {
        & $winget.Source install --id $Id -e --scope machine --accept-package-agreements --accept-source-agreements --disable-interactivity
        $code = $LASTEXITCODE
        if ($code -ne 0 -and $code -ne -1978335189) {
            & $winget.Source install --id $Id -e --accept-package-agreements --accept-source-agreements --disable-interactivity
            $code = $LASTEXITCODE
        }
    }
    finally {
        if ($hasPreference) { $PSNativeCommandUseErrorActionPreference = $preference }
    }
    return ($code -eq 0 -or $code -eq -1978335189)
}

function Ensure-AspNetRuntime {
    $dotnet = Join-Path $env:ProgramFiles 'dotnet\dotnet.exe'
    if ((Test-Path $dotnet) -and (Test-AspNetRuntime $dotnet)) { return $dotnet }
    Write-Host 'The ASP.NET Core 10 runtime is not installed. Installing it...' -ForegroundColor Cyan
    $runtimeOk = Install-RuntimeWithWinget 'Microsoft.DotNet.Runtime.10'
    $aspnetOk = Install-RuntimeWithWinget 'Microsoft.DotNet.AspNetCore.10'
    if ((-not $runtimeOk -or -not $aspnetOk) -and -not ((Test-Path $dotnet) -and (Test-AspNetRuntime $dotnet))) {
        Write-Host 'The package catalog did not install the runtime. Downloading it from Microsoft.' -ForegroundColor Yellow
        if (-not (Test-Path $dotnet)) {
            Install-QuietRuntime 'https://aka.ms/dotnet/10.0/dotnet-runtime-win-x64.exe' 'dotnet-runtime-win-x64.exe'
        }
        Install-QuietRuntime 'https://aka.ms/dotnet/10.0/aspnetcore-runtime-win-x64.exe' 'aspnetcore-runtime-win-x64.exe'
    }
    if (-not (Test-Path $dotnet) -or -not (Test-AspNetRuntime $dotnet)) {
        throw "The ASP.NET Core 10 runtime is not installed. Install it from $RuntimeDownload and run Install again."
    }
    return $dotnet
}

function Invoke-Install {
    $source = $PSScriptRoot
    $sourceIsApp = $false
    if (Test-Path $MachineApp) {
        $sourceIsApp = ((Resolve-Path $source).Path -eq (Resolve-Path $MachineApp).Path)
    }
    if (-not $sourceIsApp -and -not (Test-Path (Join-Path $source 'HotelDruid.Api.dll'))) {
        throw "HotelDruid.Api.dll was not found next to this installer ($source)."
    }

    $dotnet = Ensure-AspNetRuntime

    $previous = Show-PreviousUninstall
    $cert = Get-OrCreateMachineCert
    Ensure-Dir $MachineRoot
    Ensure-Dir $MachineLogs
    Protect-ServiceFolder -Path $MachineData -Rights 'Modify'
    Protect-ServiceFolder -Path $MachineLogs -Rights 'Modify'

    if (-not $sourceIsApp) {
        Stop-HotelDruidAt -AppDir $MachineApp
        Write-Host "Copying HotelDruid to $MachineApp" -ForegroundColor Cyan
        Copy-Package -Source $source -Destination $MachineApp
    }
    else {
        Protect-AppDirectory $MachineApp
    }

    $cmdPath = New-ServiceCommand -AppDir $MachineApp -DataDir $MachineData -LogDir $MachineLogs -DotnetExe $dotnet -Thumbprint $cert.Thumbprint -CertLocation 'LocalMachine' -LocalHttpPort $HttpPort -LocalHttpsPort $HttpsPort
    $version = Get-PackageVersion $MachineApp
    $meta = [ordered]@{
        mode = 'machine'
        appDir = $MachineApp
        dataDir = $MachineData
        logDir = $MachineLogs
        url = "https://localhost:$HttpsPort/"
        httpPort = $HttpPort
        httpsPort = $HttpsPort
        thumbprint = $cert.Thumbprint
        certCreatedByHotelDruid = [bool]$cert.Created
        installedAt = (Get-Date).ToString('o')
        version = $version
        previousUninstall = $null
    }
    if ($previous) {
        $meta.previousUninstall = [ordered]@{
            at = $previous.At
            folder = $previous.Folder
            exportFile = $previous.ExportFile
        }
        $meta.previousUninstall | ConvertTo-Json | Set-Content -Path (Join-Path $MachineData 'previous-uninstall.json') -Encoding UTF8
    }
    $meta | ConvertTo-Json -Depth 5 | Set-Content -Path $MetaPath -Encoding UTF8

    Register-DeskTask -CommandPath $cmdPath
    Register-UninstallEntry -AppDir $MachineApp -Version $version

    $startMenu = Join-Path $env:ProgramData 'Microsoft\Windows\Start Menu\Programs\HotelDruid'
    Ensure-Dir $startMenu
    $launcher = Join-Path $MachineApp 'HotelDruid.cmd'
    New-DeskShortcut -ShortcutPath (Join-Path $startMenu 'HotelDruid.lnk') -TargetPath $launcher -Arguments '' -IconIndex 220
    New-DeskShortcut -ShortcutPath (Join-Path $startMenu 'Uninstall HotelDruid.lnk') -TargetPath $launcher -Arguments 'Uninstall' -IconIndex 131
    $publicDesktop = Join-Path $env:Public 'Desktop'
    if (Test-Path $publicDesktop) {
        New-DeskShortcut -ShortcutPath (Join-Path $publicDesktop 'HotelDruid.lnk') -TargetPath $launcher -Arguments '' -IconIndex 220
    }

    $already = Get-HealthCode "https://localhost:$HttpsPort/health"
    if ($already -eq '200') {
        Write-Host "HotelDruid is installed for this computer. A desk is already answering at https://localhost:$HttpsPort/ so the new service was not started." -ForegroundColor Green
        Write-Host 'The startup task will take the port after that process stops, and at the next boot.'
    }
    else {
        Start-ScheduledTask -TaskName $TaskName
        if (Wait-Desk -HealthUrl "https://localhost:$HttpsPort/health" -Seconds 45) {
            Write-Host "HotelDruid is installed and running at https://localhost:$HttpsPort/" -ForegroundColor Green
        }
        else {
            Write-Host 'HotelDruid is installed. The desk did not answer yet.' -ForegroundColor Yellow
            $taskInfo = Get-ScheduledTaskInfo -TaskName $TaskName -ErrorAction SilentlyContinue
            if ($taskInfo) { Write-Host "Startup task result: $($taskInfo.LastTaskResult)" }
            Write-Host "Log: $(Join-Path $MachineLogs 'api.log')"
        }
    }
    Write-Host "Hotel data: $MachineData"
    Write-Host 'Open HotelDruid from the desktop or the Start menu.'
}

function Invoke-Uninstall {
    if (-not (Test-MachineInstalled) -and (Get-TaskState) -eq 'NotInstalled') {
        Write-Host 'HotelDruid is not installed on this computer.' -ForegroundColor Yellow
        return
    }

    $meta = Read-JsonFile $MetaPath
    $dataDir = $MachineData
    $thumb = ''
    $created = $false
    $url = 'https://localhost:5001/'
    if ($meta) {
        if ($meta.PSObject.Properties.Name -contains 'dataDir' -and $meta.dataDir) { $dataDir = [string]$meta.dataDir }
        if ($meta.PSObject.Properties.Name -contains 'thumbprint') { $thumb = [string]$meta.thumbprint }
        if ($meta.PSObject.Properties.Name -contains 'certCreatedByHotelDruid') { $created = [bool]$meta.certCreatedByHotelDruid }
        if ($meta.PSObject.Properties.Name -contains 'url' -and $meta.url) { $url = [string]$meta.url }
    }

    $savedTo = Save-DataExport -HealthUrl ($url.TrimEnd('/') + '/health') -BaseUrl $url -DataDir $dataDir

    Write-Host 'Removing the HotelDruid service...' -ForegroundColor Cyan
    Stop-HotelDruidAt -AppDir $MachineApp
    Unregister-ScheduledTask -TaskName $TaskName -Confirm:$false -ErrorAction SilentlyContinue

    Remove-MachineCert -Thumbprint $thumb -CreatedByHotelDruid $created

    $startMenu = Join-Path $env:ProgramData 'Microsoft\Windows\Start Menu\Programs\HotelDruid'
    if (Test-Path $startMenu) { Remove-Item -Recurse -Force $startMenu }
    $desktopLink = Join-Path (Join-Path $env:Public 'Desktop') 'HotelDruid.lnk'
    if (Test-Path $desktopLink) { Remove-Item -Force $desktopLink }
    if (Test-Path $UninstallKey) { Remove-Item -Path $UninstallKey -Recurse -Force }

    if (Test-Path $MachineRoot) { Remove-Item -Recurse -Force $MachineRoot }
    Write-Host "HotelDruid was removed. The export is in $savedTo" -ForegroundColor Green
}

function Show-Status {
    $config = Get-DeskConfig
    $code = Get-HealthCode $config.HealthUrl
    $state = Get-TaskState
    Write-Host "URL:      $($config.Url)"
    Write-Host "Health:   $code"
    Write-Host "Task:     $state"
    Write-Host "Data:     $MachineData"
    Write-Host "App:      $MachineApp"
    if (Test-MachineInstalled) { Write-Host "Installed: yes" } else { Write-Host "Installed: no" }
    Show-PreviousUninstall | Out-Null
}

function Invoke-SelfTest {
    $root = Join-Path $env:TEMP 'HotelDruid-selftest-root'
    $folder = Join-Path $root 'HotelDruid-uninstall-20000101-000000'
    $decoy = Join-Path $root 'HotelDruid-uninstall-selftest'
    if (Test-Path $root) { Remove-Item -Recurse -Force $root }
    Ensure-Dir $folder
    Ensure-Dir $decoy
    [ordered]@{
        uninstalledAt = '2000-01-01T00:00:00.0000000+00:00'
        exportFile = (Join-Path $folder 'hoteldruid-export.zip')
        selfTest = $false
    } | ConvertTo-Json | Set-Content -Path (Join-Path $folder 'uninstall.json') -Encoding UTF8
    [ordered]@{
        uninstalledAt = '2099-01-01T00:00:00.0000000+00:00'
        exportFile = ''
        selfTest = $true
    } | ConvertTo-Json | Set-Content -Path (Join-Path $decoy 'uninstall.json') -Encoding UTF8

    $markers = @(Get-UninstallMarkers -Roots @($root))
    if ($markers.Count -ne 1) { throw "Expected one uninstall marker, found $($markers.Count)." }
    $foundFolder = [System.IO.Path]::GetFullPath($markers[0].Folder)
    $expectedFolder = [System.IO.Path]::GetFullPath($folder)
    if ($foundFolder -ne $expectedFolder) { throw "The marker folder was $foundFolder." }
    if ($markers[0].ExportFile -notlike '*hoteldruid-export.zip') { throw 'The export path was not read.' }

    $empty = Join-Path $env:TEMP 'HotelDruid-selftest-empty'
    Ensure-Dir $empty
    $zip = Join-Path $empty 'empty.zip'
    Set-Content -Path $zip -Value 'not-a-zip' -Encoding ASCII
    if (Test-ZipFile $zip) { throw 'A text file was accepted as a zip.' }
    Remove-Item -Recurse -Force $root
    Remove-Item -Recurse -Force $empty
    Write-Host 'Self-test passed.' -ForegroundColor Green
}

try {
    switch ($Action) {
        'SelfTest' { Invoke-SelfTest }
        'Status' { Show-Status }
        'Install' { Enter-Admin; Invoke-Install }
        'Uninstall' { Enter-Admin; Invoke-Uninstall }
        default {
            $startCode = Invoke-Start
            if ($startCode -ne 0) {
                Write-Host 'Press Enter to close.'
                [void](Read-Host)
            }
            exit $startCode
        }
    }
}
catch {
    Write-Host $_.Exception.Message -ForegroundColor Red
    if ($Action -eq 'Start') {
        Write-Host 'Press Enter to close.'
        [void](Read-Host)
    }
    exit 1
}
