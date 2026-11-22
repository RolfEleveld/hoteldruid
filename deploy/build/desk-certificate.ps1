<#
.SYNOPSIS
  Let other computers on this network open the desk by this computer's name.

.DESCRIPTION
  After install the script is C:\ProgramData\HotelDruid\app\desk-certificate.ps1.
  Run it there with no certificate file beside it. The script reads this
  computer's name, issues a certificate for that name, trusts it here, and
  writes the public certificate on the desktop.

  Copy this script and that public certificate into one folder on each other
  computer, and run the script there. It finds the certificate file and adds
  it to that computer's trusted list.
#>

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$FriendlyName = 'HotelDruid'
$MachineRoot = Join-Path $env:ProgramData 'HotelDruid'
$MetaPath = Join-Path $MachineRoot 'install-meta.json'
$TaskName = 'HotelDruid'

function Test-IsAdministrator {
    $identity = [Security.Principal.WindowsIdentity]::GetCurrent()
    $principal = New-Object Security.Principal.WindowsPrincipal($identity)
    return $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
}

if (-not (Test-IsAdministrator)) {
    Write-Host 'Administrator approval is required.' -ForegroundColor Yellow
    $launch = "-NoProfile -ExecutionPolicy Bypass -File `"$PSCommandPath`""
    $elevated = Start-Process -FilePath "$env:SystemRoot\System32\WindowsPowerShell\v1.0\powershell.exe" -ArgumentList $launch -Verb RunAs -Wait -PassThru
    if ($elevated) { exit $elevated.ExitCode }
    exit 1
}

function Get-CertificateNames([System.Security.Cryptography.X509Certificates.X509Certificate2]$Certificate) {
    $names = New-Object System.Collections.Generic.List[string]
    foreach ($entry in @($Certificate.DnsNameList)) {
        $value = [string]$entry
        if ($entry.PSObject.Properties.Name -contains 'Unicode') { $value = [string]$entry.Unicode }
        if ($value) { $names.Add($value) }
    }
    return $names
}

function Import-PublicCertificate([string]$Path) {
    $certificate = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2 $Path
    $store = New-Object System.Security.Cryptography.X509Certificates.X509Store('Root', 'LocalMachine')
    $store.Open([System.Security.Cryptography.X509Certificates.OpenFlags]::ReadWrite)
    try {
        $existing = @($store.Certificates | Where-Object { $_.Thumbprint -eq $certificate.Thumbprint })
        if ($existing.Count -eq 0) { $store.Add($certificate) }
    }
    finally {
        $store.Close()
    }
    $names = @(Get-CertificateNames $certificate)
    Write-Host "Trusted $($certificate.Subject)." -ForegroundColor Green
    if ($names.Count -gt 0) { Write-Host "Open https://$($names[0]):5001/" }
}

function Find-DeskCertificate([string]$ComputerName) {
    $store = New-Object System.Security.Cryptography.X509Certificates.X509Store('My', 'LocalMachine')
    $store.Open([System.Security.Cryptography.X509Certificates.OpenFlags]::ReadOnly)
    try {
        $found = @($store.Certificates | Where-Object {
            $_.HasPrivateKey -and $_.NotAfter -gt (Get-Date) -and $_.FriendlyName -eq $FriendlyName -and ((Get-CertificateNames $_) -contains $ComputerName)
        } | Sort-Object NotAfter -Descending)
        if ($found.Count -gt 0) { return $found[0] }
    }
    finally {
        $store.Close()
    }
    return $null
}

function Grant-MachineKeyToService([System.Security.Cryptography.X509Certificates.X509Certificate2]$Certificate) {
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
        Write-Host "The desk service may be unable to read the certificate key: $($_.Exception.Message)" -ForegroundColor Yellow
    }
}

function Trust-OnThisComputer([System.Security.Cryptography.X509Certificates.X509Certificate2]$Certificate) {
    $raw = $Certificate.Export([System.Security.Cryptography.X509Certificates.X509ContentType]::Cert)
    $publicCert = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2 -ArgumentList (, $raw)
    $store = New-Object System.Security.Cryptography.X509Certificates.X509Store('Root', 'LocalMachine')
    $store.Open([System.Security.Cryptography.X509Certificates.OpenFlags]::ReadWrite)
    try {
        $existing = @($store.Certificates | Where-Object { $_.Thumbprint -eq $publicCert.Thumbprint })
        if ($existing.Count -eq 0) { $store.Add($publicCert) }
    }
    finally {
        $store.Close()
    }
}

function Publish-DeskCertificate([string]$ComputerName) {
    $names = New-Object System.Collections.Generic.List[string]
    $names.Add($ComputerName)
    $names.Add('localhost')
    $suffix = [string][System.Net.NetworkInformation.IPGlobalProperties]::GetIPGlobalProperties().DomainName
    if ($suffix) { $names.Add("$ComputerName.$suffix") }

    $existing = Find-DeskCertificate $ComputerName
    if ($existing) {
        $certificate = $existing
    }
    else {
        Write-Host "Creating a certificate for $ComputerName..." -ForegroundColor Cyan
        $certificate = New-SelfSignedCertificate `
            -DnsName $names.ToArray() `
            -CertStoreLocation 'Cert:\LocalMachine\My' `
            -FriendlyName $FriendlyName `
            -NotAfter (Get-Date).AddYears(5) `
            -KeyExportPolicy NonExportable `
            -KeyUsage DigitalSignature, KeyEncipherment `
            -TextExtension @('2.5.29.37={text}1.3.6.1.5.5.7.3.1')
    }

    Trust-OnThisComputer $certificate
    Grant-MachineKeyToService $certificate

    $desktop = [Environment]::GetFolderPath('Desktop')
    $publicPath = Join-Path $desktop "$ComputerName.cer"
    $raw = $certificate.Export([System.Security.Cryptography.X509Certificates.X509ContentType]::Cert)
    [System.IO.File]::WriteAllBytes($publicPath, $raw)
    return [pscustomobject]@{ Certificate = $certificate; PublicFile = $publicPath; Created = (-not $existing) }
}

function Update-InstalledDesk([string]$Thumbprint, [bool]$Created) {
    if (-not (Test-Path $MetaPath)) {
        Write-Host 'HotelDruid is not installed on this computer yet. Install it, then run this script again.' -ForegroundColor Yellow
        return
    }

    $meta = Get-Content -Path $MetaPath -Raw | ConvertFrom-Json
    $httpsPort = 5001
    $httpPort = 5000
    if ($meta.PSObject.Properties.Name -contains 'httpsPort' -and $meta.httpsPort) { $httpsPort = [int]$meta.httpsPort }
    if ($meta.PSObject.Properties.Name -contains 'httpPort' -and $meta.httpPort) { $httpPort = [int]$meta.httpPort }
    $appDir = Join-Path $MachineRoot 'app'
    if ($meta.PSObject.Properties.Name -contains 'appDir' -and $meta.appDir) { $appDir = [string]$meta.appDir }
    $commandPath = Join-Path $appDir 'run-service.cmd'
    if (-not (Test-Path $commandPath)) {
        Write-Host "The installed desk has no startup command at $commandPath." -ForegroundColor Yellow
        return
    }

    $content = Get-Content -Path $commandPath -Raw
    $content = $content -replace 'ASPNETCORE_Kestrel__Certificates__Default__Thumbprint=[^\r\n]*', "ASPNETCORE_Kestrel__Certificates__Default__Thumbprint=$Thumbprint"
    $content = $content -replace '--urls "[^"]*"', "--urls `"https://+:$httpsPort;http://+:$httpPort`""
    Set-Content -Path $commandPath -Value $content -Encoding ASCII

    $meta.thumbprint = $Thumbprint
    $meta.certCreatedByHotelDruid = $Created -or (($meta.PSObject.Properties.Name -contains 'certCreatedByHotelDruid') -and [bool]$meta.certCreatedByHotelDruid)
    $meta.url = "https://$($env:COMPUTERNAME):$httpsPort/"
    $meta | ConvertTo-Json -Depth 5 | Set-Content -Path $MetaPath -Encoding UTF8

    $ruleName = 'HotelDruid'
    $rule = Get-NetFirewallRule -DisplayName $ruleName -ErrorAction SilentlyContinue
    if (-not $rule) {
        New-NetFirewallRule -DisplayName $ruleName -Direction Inbound -Action Allow -Protocol TCP -LocalPort $httpsPort | Out-Null
    }

    $task = Get-ScheduledTask -TaskName $TaskName -ErrorAction SilentlyContinue
    if ($task) {
        if ([string]$task.State -eq 'Running') { Stop-ScheduledTask -TaskName $TaskName -ErrorAction SilentlyContinue }
        $processes = @(Get-CimInstance Win32_Process -Filter "Name='dotnet.exe'" -ErrorAction SilentlyContinue)
        foreach ($process in $processes) {
            $cmd = [string]$process.CommandLine
            if ($cmd -and $cmd.Contains('HotelDruid.Api.dll') -and $cmd.Contains($appDir)) {
                Stop-Process -Id $process.ProcessId -Force -ErrorAction SilentlyContinue
            }
        }
        Start-ScheduledTask -TaskName $TaskName
    }

    Write-Host "Other computers can open https://$($env:COMPUTERNAME):$httpsPort/" -ForegroundColor Green
}

$computerName = $env:COMPUTERNAME
$publicFiles = @(Get-ChildItem -Path $PSScriptRoot -Filter '*.cer' -File -ErrorAction SilentlyContinue)
$forThisComputer = @($publicFiles | Where-Object {
    $parsed = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2 $_.FullName
    (Get-CertificateNames $parsed) -contains $computerName
})

if ($publicFiles.Count -gt 0 -and $forThisComputer.Count -eq 0) {
    foreach ($file in $publicFiles) { Import-PublicCertificate $file.FullName }
    return
}

$published = Publish-DeskCertificate $computerName
Write-Host "Public certificate: $($published.PublicFile)" -ForegroundColor Green
Write-Host 'Copy this script and that file into one folder on each other computer, then run the script there.'
Update-InstalledDesk -Thumbprint $published.Certificate.Thumbprint -Created ([bool]$published.Created)
