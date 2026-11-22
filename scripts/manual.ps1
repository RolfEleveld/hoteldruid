<#
.SYNOPSIS
  Refresh the hotel manual pictures and build the HTML guide and the epub.

.DESCRIPTION
  Reads docs/manual/chapters and docs/manual/catalog.json.
  -Capture saves a picture of each catalog page from a running desk.
  Pictures named in docs/manual/images/hotel.txt are left as the hotel saved them.
  -Build writes docs/manual/out/index.html and docs/manual/out/HotelDruid.epub.

.EXAMPLE
  ./scripts/manual.ps1
  ./scripts/manual.ps1 -Build
  ./scripts/manual.ps1 -Capture -BaseUrl https://localhost:5001
#>
param(
    [string]$BaseUrl = 'https://localhost:5001',
    [switch]$Capture,
    [switch]$Build,
    [switch]$Force
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

if (-not $Capture -and -not $Build) {
    $Capture = $true
    $Build = $true
}

$root = Resolve-Path (Join-Path $PSScriptRoot '..')
$manual = Join-Path $root 'docs\manual'
$chapterDir = Join-Path $manual 'chapters'
$imageDir = Join-Path $manual 'images'
$outDir = Join-Path $manual 'out'
$catalogPath = Join-Path $manual 'catalog.json'
$keepPath = Join-Path $imageDir 'hotel.txt'

function Get-Slug([string]$Text) {
    $slug = $Text.ToLowerInvariant() -replace '[^a-z0-9]+', '-'
    return $slug.Trim('-')
}

function Get-ChapterId([string]$FileName) {
    $stem = [IO.Path]::GetFileNameWithoutExtension($FileName)
    return ($stem -replace '^\d+-', '')
}

function Get-KeptImages {
    if (-not (Test-Path $keepPath)) { return @() }
    return @(Get-Content $keepPath | ForEach-Object { $_.Trim() } | Where-Object { $_ -and -not $_.StartsWith('#') })
}

function Get-Edge {
    $candidates = @(
        "$env:ProgramFiles\Microsoft\Edge\Application\msedge.exe",
        "${env:ProgramFiles(x86)}\Microsoft\Edge\Application\msedge.exe"
    )
    return $candidates | Where-Object { $_ -and (Test-Path $_) } | Select-Object -First 1
}

function Get-SampleStay([string]$Url) {
    $from = Get-Date -Format 'yyyy-MM-dd'
    $to = (Get-Date).AddDays(120).ToString('yyyy-MM-dd')
    $list = "$($Url.TrimEnd('/'))/api/bookings?from=$from&to=$to"
    $rows = Invoke-RestMethod -Uri $list -SkipCertificateCheck
    $confirmed = @($rows | Where-Object { $_.Status -eq 'Confirmed' -and $_.Id })
    if ($confirmed.Count -gt 0) { return $confirmed[0] }
    $any = @($rows | Where-Object { $_.Id -and $_.Status -ne 'Cancelled' -and $_.Status -ne 'CheckedOut' })
    if ($any.Count -gt 0) { return $any[0] }
    return $null
}

$script:CdpId = 0

function Send-Cdp($Socket, [string]$Method, $Params) {
    $script:CdpId++
    $payload = @{ id = $script:CdpId; method = $Method }
    if ($null -ne $Params) { $payload.params = $Params }
    $bytes = [Text.Encoding]::UTF8.GetBytes(($payload | ConvertTo-Json -Compress -Depth 8))
    $send = $Socket.SendAsync([ArraySegment[byte]]::new($bytes), [Net.WebSockets.WebSocketMessageType]::Text, $true, [Threading.CancellationToken]::None)
    $send.Wait()
    return $script:CdpId
}

function Receive-Cdp($Socket, [int]$Id) {
    $buffer = New-Object byte[] 262144
    while ($true) {
        $stream = New-Object IO.MemoryStream
        do {
            $wait = [Threading.CancellationTokenSource]::new(20000)
        try { $received = $Socket.ReceiveAsync([ArraySegment[byte]]::new($buffer), $wait.Token).GetAwaiter().GetResult() }
        catch { throw 'The browser did not answer during capture.' }
            if ($received.MessageType -eq [Net.WebSockets.WebSocketMessageType]::Close) { throw 'The browser closed the capture connection.' }
            $stream.Write($buffer, 0, $received.Count)
        } while (-not $received.EndOfMessage)
        $text = [Text.Encoding]::UTF8.GetString($stream.ToArray())
        $message = $text | ConvertFrom-Json
        if ($message.PSObject.Properties.Name -contains 'id' -and [int]$message.id -eq $Id) { return $message }
    }
}

function Invoke-Cdp($Socket, [string]$Method, $Params) {
    $id = Send-Cdp $Socket $Method $Params
    return Receive-Cdp $Socket $id
}

function Wait-Desk([Net.WebSockets.ClientWebSocket]$Socket, [string]$ExpectedPath) {
    for ($attempt = 0; $attempt -lt 30; $attempt++) {
        Start-Sleep -Milliseconds 400
        $probe = Invoke-Cdp $Socket 'Runtime.evaluate' @{
            expression = 'JSON.stringify({p:location.pathname+location.search,n:document.body?document.body.innerText.length:0})'
            returnByValue = $true
        }
        $raw = [string]$probe.result.result.value
        if ([string]::IsNullOrWhiteSpace($raw)) { continue }
        $state = $raw | ConvertFrom-Json
        $here = [string]$state.p
        $matchesPath = [string]::IsNullOrWhiteSpace($ExpectedPath) -or $here.StartsWith($ExpectedPath)
        if ($matchesPath -and [int]$state.n -gt 500) {
            Start-Sleep -Milliseconds 2500
            return $true
        }
    }
    return $false
}

function Save-DeskPictures($Socket, $Shots, $Stay, $Kept) {
    Invoke-Cdp $Socket 'Page.navigate' @{ url = "$($BaseUrl.TrimEnd('/'))/" } | Out-Null
    if (-not (Wait-Desk $Socket '/')) { Write-Warning 'The desk did not finish opening.' }
    foreach ($shot in $Shots) {
        $file = [string]$shot.file
        if (-not $Force -and ($Kept -contains $file)) {
            Write-Host "Keep $file"
            continue
        }
        $path = [string]$shot.path
        if ($path -match '\{id\}' -or $path -match '\{year\}') {
            if (-not $Stay) {
                Write-Warning "Skip $file. No sample stay was found."
                continue
            }
            $path = $path.Replace('{year}', [string]$Stay.Year).Replace('{id}', [string]$Stay.Id)
        }
        $url = "$($BaseUrl.TrimEnd('/'))$path"
        Write-Host "Capture $file"
        $literal = $url.Replace('\', '\\').Replace("'", "\'")
        Invoke-Cdp $Socket 'Runtime.evaluate' @{
            expression = "(()=>{const a=document.createElement('a');a.href='$literal';document.body.appendChild(a);a.click();a.remove();})()"
            returnByValue = $true
        } | Out-Null
        $expected = ([Uri]$url).PathAndQuery
        if ($expected.Length -gt 1) { $expected = $expected.TrimEnd('/') }
        if (-not (Wait-Desk $Socket $expected)) { Write-Warning "$file was still loading." }
        $shotResult = Invoke-Cdp $Socket 'Page.captureScreenshot' @{ format = 'png' }
        $bytes = [Convert]::FromBase64String([string]$shotResult.result.data)
        [IO.File]::WriteAllBytes((Join-Path $imageDir $file), $bytes)
    }
}

function Update-Pictures {
    $edge = Get-Edge
    if (-not $edge) { throw 'Microsoft Edge was not found. Capture needs Edge.' }
    try {
        Invoke-WebRequest -Uri "$($BaseUrl.TrimEnd('/'))/health" -SkipCertificateCheck -UseBasicParsing -TimeoutSec 10 | Out-Null
    }
    catch {
        Write-Warning "Desk is not answering at $BaseUrl. Pictures were not refreshed."
        return
    }

    $stay = Get-SampleStay $BaseUrl
    $kept = Get-KeptImages
    $shots = Get-Content $catalogPath -Raw | ConvertFrom-Json
    New-Item -ItemType Directory -Force -Path $imageDir | Out-Null

    $profile = Join-Path $env:TEMP 'hd-manual-edge'
    if (Test-Path $profile) { Remove-Item $profile -Recurse -Force -ErrorAction SilentlyContinue }
    $browser = Start-Process -FilePath $edge -ArgumentList @(
        '--headless=new',
        '--disable-gpu',
        '--hide-scrollbars',
        '--ignore-certificate-errors',
        '--window-size=1440,1200',
        "--user-data-dir=$profile",
        '--remote-debugging-port=9223',
        'about:blank'
    ) -PassThru -WindowStyle Hidden
    $socket = $null
    try {
        $version = $null
        for ($attempt = 0; $attempt -lt 40 -and -not $version; $attempt++) {
            Start-Sleep -Milliseconds 250
            try { $version = Invoke-RestMethod -Uri 'http://127.0.0.1:9223/json/version' -TimeoutSec 2 } catch { }
        }
        if (-not $version) { throw 'Edge did not open a capture port.' }
        $page = @((Invoke-RestMethod -Uri 'http://127.0.0.1:9223/json/list') | Where-Object { $_.type -eq 'page' })[0]
        $socket = [Net.WebSockets.ClientWebSocket]::new()
        $wsUrl = [string](@($page.webSocketDebuggerUrl)[0])
        $socket.ConnectAsync([Uri]::new($wsUrl), [Threading.CancellationToken]::None).GetAwaiter().GetResult() | Out-Null
        Invoke-Cdp $socket 'Page.enable' $null | Out-Null
        Invoke-Cdp $socket 'Runtime.enable' $null | Out-Null
        Invoke-Cdp $socket 'Emulation.setDeviceMetricsOverride' @{ width = 1440; height = 1100; deviceScaleFactor = 1; mobile = $false } | Out-Null
        Save-DeskPictures $socket $shots $stay $kept
    }
    finally {
        if ($socket -and $socket.State -eq [Net.WebSockets.WebSocketState]::Open) {
            $socket.CloseAsync([Net.WebSockets.WebSocketCloseStatus]::NormalClosure, 'done', [Threading.CancellationToken]::None).Wait(1000) | Out-Null
        }
        if ($browser -and -not $browser.HasExited) { Stop-Process -Id $browser.Id -Force -ErrorAction SilentlyContinue }
    }
}

function Convert-Inline {
    param([string]$Text, [string]$Mode, [hashtable]$Chapters)
    $safe = $Text.Replace('&', '&amp;').Replace('<', '&lt;').Replace('>', '&gt;')
    $safe = $safe -replace '`([^`]+)`', '<code>$1</code>'
    $safe = $safe -replace '\*\*([^*]+)\*\*', '<strong>$1</strong>'
    $safe = $safe -replace '!\[([^\]]*)\]\(([^)]+)\)', {
        $alt = $_.Groups[1].Value
        $src = [IO.Path]::GetFileName($_.Groups[2].Value)
        if ($Mode -eq 'epub') { "<img src=""images/$src"" alt=""$alt""/>" } else { "<img src=""images/$src"" alt=""$alt""/>" }
    }
    $safe = $safe -replace '\[([^\]]+)\]\(([^)]+)\)', {
        $label = $_.Groups[1].Value
        $href = $_.Groups[2].Value
        $id = Get-ChapterId ([IO.Path]::GetFileName($href))
        if ($href.EndsWith('.md') -and $Chapters.ContainsKey($id)) {
            if ($Mode -eq 'epub') { "<a href=""$id.xhtml"">$label</a>" } else { "<a href=""#$id"">$label</a>" }
        }
        elseif ($Mode -eq 'epub') { $label }
        else { "<a href=""$href"">$label</a>" }
    }
    return $safe
}

function Convert-Chapter {
    param([string]$Markdown, [string]$ChapterId, [string]$Mode, [hashtable]$Chapters)
    $lines = $Markdown -replace "`r", '' -split "`n"
    $parts = New-Object System.Collections.Generic.List[string]
    $i = 0
    while ($i -lt $lines.Count) {
        $line = $lines[$i]
        if ($line -match '^```') {
            $i++
            $code = New-Object System.Collections.Generic.List[string]
            while ($i -lt $lines.Count -and $lines[$i] -notmatch '^```') {
                $code.Add(($lines[$i].Replace('&', '&amp;').Replace('<', '&lt;').Replace('>', '&gt;')))
                $i++
            }
            $parts.Add('<pre><code>' + ($code -join "`n") + '</code></pre>')
            $i++
            continue
        }
        if ($line -match '^\|' -and ($i + 1) -lt $lines.Count -and $lines[$i + 1] -match '^\|\s*-') {
            $header = $line.Trim('|').Split('|') | ForEach-Object { $_.Trim() }
            $i += 2
            $rows = New-Object System.Collections.Generic.List[string]
            $headHtml = ($header | ForEach-Object { '<th>' + (Convert-Inline $_ $Mode $Chapters) + '</th>' }) -join ''
            $rows.Add("<tr>$headHtml</tr>")
            while ($i -lt $lines.Count -and $lines[$i] -match '^\|') {
                $cells = $lines[$i].Trim('|').Split('|') | ForEach-Object { $_.Trim() }
                $cellHtml = ($cells | ForEach-Object { '<td>' + (Convert-Inline $_ $Mode $Chapters) + '</td>' }) -join ''
                $rows.Add("<tr>$cellHtml</tr>")
                $i++
            }
            $parts.Add('<table>' + ($rows -join '') + '</table>')
            continue
        }
        if ($line -match '^(#{1,3})\s+(.+)$') {
            $level = $Matches[1].Length
            $title = $Matches[2].Trim()
            $id = if ($level -eq 1) { $ChapterId } else { "$ChapterId-$(Get-Slug $title)" }
            $parts.Add("<h$level id=""$id"">" + (Convert-Inline $title $Mode $Chapters) + "</h$level>")
            $i++
            continue
        }
        if ($line -match '^\s*[-*]\s+' -or $line -match '^\s*\d+\.\s+') {
            $ordered = $line -match '^\s*\d+\.\s+'
            $tag = if ($ordered) { 'ol' } else { 'ul' }
            $items = New-Object System.Collections.Generic.List[string]
            while ($i -lt $lines.Count -and ($lines[$i] -match '^\s*[-*]\s+' -or $lines[$i] -match '^\s*\d+\.\s+')) {
                $item = $lines[$i] -replace '^\s*[-*]\s+', '' -replace '^\s*\d+\.\s+', ''
                $items.Add('<li>' + (Convert-Inline $item.Trim() $Mode $Chapters) + '</li>')
                $i++
            }
            $parts.Add("<$tag>" + ($items -join '') + "</$tag>")
            continue
        }
        if ([string]::IsNullOrWhiteSpace($line)) { $i++; continue }
        $para = New-Object System.Collections.Generic.List[string]
        while ($i -lt $lines.Count -and -not [string]::IsNullOrWhiteSpace($lines[$i]) -and $lines[$i] -notmatch '^(#{1,3}\s|\||```|\s*[-*]\s|\s*\d+\.\s)') {
            $para.Add($lines[$i].Trim())
            $i++
        }
        $parts.Add('<p>' + (Convert-Inline ($para -join ' ') $Mode $Chapters) + '</p>')
    }
    return ($parts -join "`n")
}

function Build-Manual {
    $files = @(Get-ChildItem $chapterDir -Filter '*.md' | Sort-Object Name)
    if ($files.Count -eq 0) { throw "No chapters in $chapterDir" }
    $chapters = @{}
    foreach ($file in $files) {
        $chapters[(Get-ChapterId $file.Name)] = $file
    }

    $sections = New-Object System.Collections.Generic.List[string]
    $nav = New-Object System.Collections.Generic.List[string]
    $xhtml = @{}
    $titles = @{}
    foreach ($file in $files) {
        $id = Get-ChapterId $file.Name
        $raw = Get-Content $file.FullName -Raw
        $titleLine = ($raw -split "`n" | Where-Object { $_ -match '^#\s+' } | Select-Object -First 1)
        $title = if ($titleLine) { ($titleLine -replace '^#\s+', '').Trim() } else { $id }
        $titles[$id] = $title
        $nav.Add("<a href=""#$id"">$title</a>")
        $sections.Add((Convert-Chapter $raw $id 'html' $chapters))
        $xhtml[$id] = Convert-Chapter $raw $id 'epub' $chapters
    }

    New-Item -ItemType Directory -Force -Path $outDir | Out-Null
    $style = @'
body{font:16px/1.45 "Segoe UI",sans-serif;margin:0;color:#1c1c1c;background:#fff}
nav{position:sticky;top:0;background:#fff;border-bottom:1px solid #ccc;padding:8px 16px;display:flex;flex-wrap:wrap;gap:8px 14px}
nav a{color:#163a5f;text-decoration:none}
main{max-width:880px;margin:0 auto;padding:8px 16px 48px}
h1{margin-top:2rem;border-top:1px solid #ddd;padding-top:1rem}
img{max-width:100%;height:auto;border:1px solid #ddd}
table{border-collapse:collapse;width:100%;margin:12px 0}
td,th{border:1px solid #ccc;padding:4px 8px;vertical-align:top;text-align:left}
code,pre{font-family:Consolas,monospace}
pre{background:#f6f6f6;padding:8px;overflow:auto}
'@
    $html = @"
<!DOCTYPE html>
<html lang="en">
<head>
<meta charset="utf-8"/>
<title>HotelDruid manual</title>
<style>
$style
</style>
</head>
<body>
<nav>
$($nav -join '')
</nav>
<main>
$($sections -join "`n")
</main>
</body>
</html>
"@
    $htmlPath = Join-Path $manual 'HotelDruid.html'
    [IO.File]::WriteAllText($htmlPath, $html, [Text.UTF8Encoding]::new($false))

    $epubPath = Join-Path $outDir 'HotelDruid.epub'
    if (Test-Path $epubPath) { Remove-Item $epubPath -Force }
    Add-Type -AssemblyName System.IO.Compression
    Add-Type -AssemblyName System.IO.Compression.FileSystem
    $zip = [IO.Compression.ZipFile]::Open($epubPath, [IO.Compression.ZipArchiveMode]::Create)
    try {
        Add-ZipText $zip 'mimetype' 'application/epub+zip' ([IO.Compression.CompressionLevel]::NoCompression)
        Add-ZipText $zip 'META-INF/container.xml' @'
<?xml version="1.0" encoding="UTF-8"?>
<container version="1.0" xmlns="urn:oasis:names:tc:opendocument:xmlns:container">
<rootfiles><rootfile full-path="OEBPS/content.opf" media-type="application/oebps-package+xml"/></rootfiles>
</container>
'@ ([IO.Compression.CompressionLevel]::Optimal)

        $manifest = New-Object System.Collections.Generic.List[string]
        $spine = New-Object System.Collections.Generic.List[string]
        $navPoints = New-Object System.Collections.Generic.List[string]
        $play = 1
        $epubCss = 'body{font-family:serif;line-height:1.45}img{max-width:100%;height:auto}table{border-collapse:collapse;width:100%}td,th{border:1px solid #999;padding:4px;vertical-align:top}'
        Add-ZipText $zip 'OEBPS/style.css' $epubCss ([IO.Compression.CompressionLevel]::Optimal)
        $manifest.Add('<item id="css" href="style.css" media-type="text/css"/>')
        $manifest.Add('<item id="nav" href="nav.xhtml" media-type="application/xhtml+xml" properties="nav"/>')

        $navItems = New-Object System.Collections.Generic.List[string]
        foreach ($file in $files) {
            $id = Get-ChapterId $file.Name
            $body = $xhtml[$id]
            $page = @"
<?xml version="1.0" encoding="UTF-8"?>
<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="en">
<head><title>$([System.Net.WebUtility]::HtmlEncode($titles[$id]))</title><link rel="stylesheet" type="text/css" href="style.css"/></head>
<body>
$body
</body>
</html>
"@
            Add-ZipText $zip "OEBPS/$id.xhtml" $page ([IO.Compression.CompressionLevel]::Optimal)
            $manifest.Add("<item id=""$id"" href=""$id.xhtml"" media-type=""application/xhtml+xml""/>")
            $spine.Add("<itemref idref=""$id""/>")
            $navItems.Add("<li><a href=""$id.xhtml"">$([System.Net.WebUtility]::HtmlEncode($titles[$id]))</a></li>")
            $navPoints.Add("<navPoint id=""nav-$id"" playOrder=""$play""><navLabel><text>$([System.Net.WebUtility]::HtmlEncode($titles[$id]))</text></navLabel><content src=""$id.xhtml""/></navPoint>")
            $play++
        }

        $used = New-Object System.Collections.Generic.HashSet[string]
        foreach ($file in $files) {
            foreach ($match in [regex]::Matches((Get-Content $file.FullName -Raw), '!\[[^\]]*\]\(([^)]+)\)')) {
                $name = [IO.Path]::GetFileName($match.Groups[1].Value)
                if (-not $used.Add($name)) { continue }
                $source = Join-Path $imageDir $name
                if (Test-Path $source) {
                    Add-ZipFile $zip "OEBPS/images/$name" $source
                    $manifest.Add("<item id=""img-$($name.Replace('.', '-'))"" href=""images/$name"" media-type=""image/png""/>")
                }
            }
        }

        $navDoc = @"
<?xml version="1.0" encoding="UTF-8"?>
<html xmlns="http://www.w3.org/1999/xhtml" xmlns:epub="http://www.idpf.org/2007/ops" xml:lang="en">
<head><title>Contents</title></head>
<body><nav epub:type="toc"><h1>Contents</h1><ol>
$($navItems -join '')
</ol></nav></body></html>
"@
        Add-ZipText $zip 'OEBPS/nav.xhtml' $navDoc ([IO.Compression.CompressionLevel]::Optimal)
        $ncx = @"
<?xml version="1.0" encoding="UTF-8"?>
<ncx xmlns="http://www.daisy.org/z3986/2005/ncx/" version="2005-1">
<head><meta name="dtb:uid" content="urn:uuid:hoteldruid-manual"/></head>
<docTitle><text>HotelDruid manual</text></docTitle>
<navMap>
$($navPoints -join '')
</navMap>
</ncx>
"@
        Add-ZipText $zip 'OEBPS/toc.ncx' $ncx ([IO.Compression.CompressionLevel]::Optimal)
        $manifest.Add('<item id="ncx" href="toc.ncx" media-type="application/x-dtbncx+xml"/>')
        $opf = @"
<?xml version="1.0" encoding="UTF-8"?>
<package xmlns="http://www.idpf.org/2007/opf" unique-identifier="uid" version="3.0">
<metadata xmlns:dc="http://purl.org/dc/elements/1.1/">
<dc:identifier id="uid">urn:uuid:hoteldruid-manual</dc:identifier>
<dc:title>HotelDruid manual</dc:title>
<dc:language>en</dc:language>
<meta property="dcterms:modified">$([DateTime]::UtcNow.ToString('yyyy-MM-ddTHH:mm:ssZ'))</meta>
</metadata>
<manifest>
$($manifest -join "`n")
</manifest>
<spine toc="ncx">
$($spine -join "`n")
</spine>
</package>
"@
        Add-ZipText $zip 'OEBPS/content.opf' $opf ([IO.Compression.CompressionLevel]::Optimal)
    }
    finally {
        $zip.Dispose()
    }
    Write-Host "Wrote $htmlPath"
    Write-Host "Wrote $epubPath"
}

function Add-ZipText($Zip, [string]$Name, [string]$Text, $Level) {
    $entry = $Zip.CreateEntry($Name, $Level)
    $stream = $entry.Open()
    try {
        $bytes = [Text.UTF8Encoding]::new($false).GetBytes($Text)
        $stream.Write($bytes, 0, $bytes.Length)
    }
    finally { $stream.Dispose() }
}

function Add-ZipFile($Zip, [string]$Name, [string]$Path) {
    $entry = $Zip.CreateEntry($Name, [IO.Compression.CompressionLevel]::Optimal)
    $input = [IO.File]::OpenRead($Path)
    $stream = $entry.Open()
    try { $input.CopyTo($stream) }
    finally { $stream.Dispose(); $input.Dispose() }
}

if ($Capture) { Update-Pictures }
if ($Build) { Build-Manual }
