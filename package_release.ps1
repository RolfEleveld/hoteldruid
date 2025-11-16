<#
Package the repo into a distributable ZIP and include an installer script.
Usage (run from repo root in PowerShell):
  .\package_release.ps1 -SourceDir (Get-Location) -OutputZip .\hoteldruid-release.zip
#>
param(
    [string]$SourceDir = (Get-Location).Path,
    [string]$OutputZip = "hoteldruid-release.zip",
    [string]$TempDir = "$env:TEMP\hoteldruid_pkg_$((Get-Random))",
    [string[]]$ExcludePatterns = @('.git','node_modules','$RECYCLE.BIN','windows\','Thumbs.db','*.zip','EasyPHP*.exe','vcredist_*.exe','phpdesktop*.zip')
)

Write-Host "Packaging repository from: $SourceDir"
Write-Host "Temporary staging: $TempDir"

if (-not (Test-Path -Path $SourceDir)) {
    Write-Error "Source directory not found: $SourceDir"
    exit 2
}

# Make temp dir
if (Test-Path -Path $TempDir) { Remove-Item -LiteralPath $TempDir -Recurse -Force }
New-Item -Path $TempDir -ItemType Directory -Force | Out-Null

# Copy files while excluding common VCS and node artefacts; keep directory structure
$files = Get-ChildItem -Path $SourceDir -Recurse -Force -File | Where-Object {
    $full = $_.FullName
    foreach ($pat in $ExcludePatterns) {
        if ($full -like "*$pat*") { return $false }
    }
    return $true
}

$filesCount = ($files | Measure-Object).Count
Write-Host "Copying $filesCount files..."

foreach ($f in $files) {
    $dest = Join-Path $TempDir ($f.FullName.Substring($SourceDir.Length).TrimStart('\','/'))
    $destDir = Split-Path $dest -Parent
    if (-not (Test-Path -Path $destDir)) { New-Item -Path $destDir -ItemType Directory -Force | Out-Null }
    Copy-Item -LiteralPath $f.FullName -Destination $dest -Force
}

# Ensure installer script is present (should be in repo). If not, warn.
$installerSrc = Join-Path $SourceDir 'install_release.ps1'
if (Test-Path $installerSrc) {
    Copy-Item -LiteralPath $installerSrc -Destination (Join-Path $TempDir 'install_release.ps1') -Force
} else {
    Write-Warning "No install_release.ps1 found in source; create it or edit package_release.ps1 to include one."
}

# Create zip
if (Test-Path -Path $OutputZip) { Remove-Item -LiteralPath $OutputZip -Force }
Write-Host "Compressing to $OutputZip..."
Compress-Archive -Path (Join-Path $TempDir '*') -DestinationPath $OutputZip -Force

# Cleanup
Write-Host "Cleaning up staging area..."
Remove-Item -LiteralPath $TempDir -Recurse -Force

Write-Host "Package created: $OutputZip"
Write-Host "Next: copy the zip to target machine and run 'install_release.ps1' (included in zip) to extract and install."