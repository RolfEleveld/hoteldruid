# Install HotelDruid via the latest GitHub release
$ErrorActionPreference = 'Stop'
# Repository information
$repo = 'rolfeleveld/hoteldruid'
# Get the latest release information from GitHub API
$release = Invoke-RestMethod "https://api.github.com/repos/$repo/releases/latest"
# Find the asset matching the desired pattern
$asset = $release.assets | Where-Object { $_.name -like 'Hoteldruid-PHPDesktop-*.zip' } | Select-Object -First 1
# Exit if no matching asset is found
if (-not $asset) { Write-Error 'No matching release found for PHP Desktop HotelDruid on GitHub'; exit 1 }
# Download the zip asset into temporary location
$downloadUrl = $asset.browser_download_url
Write-Host "Downloading $($asset.name) from $downloadUrl"
$tmp = Join-Path ([IO.Path]::GetTempPath()) ('hoteldruid_install_' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force -Path $tmp | Out-Null
$zipPath = Join-Path $tmp $asset.name
Invoke-WebRequest -Uri $downloadUrl -OutFile $zipPath -UseBasicParsing -ErrorAction Stop

# Extract the zip file
Write-Host "Extracting $zipPath to $tmp"
Expand-Archive -LiteralPath $zipPath -DestinationPath $tmp -Force

# Run the install_release.ps1 script from the extracted files
$script = Join-Path $tmp 'install_release.ps1'
if (-not (Test-Path $script)) { Write-Error 'install_release.ps1 not found in archive'; exit 2 }

Write-Host "Running install_release.ps1 from $tmp"
& powershell -NoProfile -NoLogo -ExecutionPolicy Bypass -File $script