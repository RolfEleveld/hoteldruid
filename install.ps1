# Install HotelDruid via the latest GitHub release
$ErrorActionPreference = 'Stop'

# Repository information
$repo = 'rolfeleveld/hoteldruid'
$headers = @{ 'User-Agent' = 'HotelDruid Installer' }

# Prepare temporary folder for download and extraction
$tmp = Join-Path ([IO.Path]::GetTempPath()) ('hoteldruid_install_' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force -Path $tmp | Out-Null

# Helper to download URL to $zipPath
function Download-Zip($url, $outPath) {
	try {
		Invoke-WebRequest -Uri $url -OutFile $outPath -UseBasicParsing -Headers $headers -ErrorAction Stop
		return $true
	} catch {
		Write-Host "Download failed from $url : $_"
		return $false
	}
}

# 1) Try deterministic approach: follow releases/latest redirect to determine tag, then compute download URL
$zipPath = $null
$downloaded = $false
try {
	$resp = Invoke-WebRequest -Uri "https://github.com/$repo/releases/latest" -UseBasicParsing -Headers $headers -ErrorAction Stop
	$finalUri = $resp.BaseResponse.ResponseUri.AbsoluteUri
	if (-not $finalUri) { $finalUri = $resp.BaseResponse.RequestMessage.RequestUri.OriginalString }
	if ($finalUri -match '/releases/tag/(.+)$') { $tag = $Matches[1] }
} catch {
	Write-Host "Could not resolve latest release redirect: $_"
	$tag = $null
}

if ($tag) {
	$assetName = "Hoteldruid-PHPDesktop-$tag.zip"
	$zipPath = Join-Path $tmp $assetName
	$downloadUrl = "https://github.com/$repo/releases/download/$tag/$assetName"
	Write-Host "Attempting deterministic download: $assetName from $downloadUrl"
	$downloaded = Download-Zip $downloadUrl $zipPath
}

# 2) Fallback: parse releases page for an asset link if deterministic download failed
if (-not $downloaded) {
	try {
		if (-not $resp) { $resp = Invoke-WebRequest -Uri "https://github.com/$repo/releases/latest" -UseBasicParsing -Headers $headers -ErrorAction Stop }
		$link = $resp.Links | Where-Object { $_.href -match 'Hoteldruid-PHPDesktop-.*\.zip$' } | Select-Object -First 1
		if (-not $link) {
			# As a last resort search the raw HTML for .zip
			$href = ($resp.RawContent -split 'href="') | Where-Object { $_ -match 'Hoteldruid-PHPDesktop-.*\.zip' } | Select-Object -First 1
			if ($href) { $href = ($href -split '"')[0] }
		} else {
			$href = $link.href
		}
		if ($href) {
			if ($href -notmatch '^https?://') { $downloadUrl = "https://github.com$href" } else { $downloadUrl = $href }
			$assetName = Split-Path $downloadUrl -Leaf
			$zipPath = Join-Path $tmp $assetName
			Write-Host "Attempting fallback download from page link: $downloadUrl"
			$downloaded = Download-Zip $downloadUrl $zipPath
		}
	} catch {
		Write-Host "Failed to parse releases page: $_"
		$downloaded = $false
	}
}

if (-not $downloaded) { Write-Error 'Unable to obtain Hoteldruid release ZIP via deterministic URL or releases page parsing.'; exit 1 }

# Extract the zip file
Write-Host "Extracting $zipPath to $tmp"
Expand-Archive -LiteralPath $zipPath -DestinationPath $tmp -Force

# Run the install_release.ps1 script from the extracted files
$script = Join-Path $tmp 'install_release.ps1'
if (-not (Test-Path $script)) { Write-Error 'install_release.ps1 not found in archive'; exit 2 }

Write-Host "Running install_release.ps1 from $tmp"
& powershell -NoProfile -NoLogo -ExecutionPolicy Bypass -File $script