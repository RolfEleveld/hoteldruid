<?php
// Stream export packages through HTTP so browsers trigger download instead of opening file:// links.
set_time_limit(0);
header('X-Content-Type-Options: nosniff');

$encoded = $_GET['p'] ?? '';
if ($encoded === '') {
    http_response_code(400);
    exit('missing parameter');
}

$path = base64_decode(strtr($encoded, '-_', '+/'), true);
if ($path === false || $path === '') {
    http_response_code(400);
    exit('invalid parameter');
}

$real = realpath($path);
if ($real === false || !is_file($real)) {
    http_response_code(404);
    exit('not found');
}

// Allow only known export locations
$allowedBases = array();
$hoteldruidRoot = realpath(__DIR__ . '/..');

// Load constants to resolve C_DATI_PATH if available
@include_once(__DIR__ . '/../costanti.php');
if (defined('C_DATI_PATH') && C_DATI_PATH) {
    $base = realpath(rtrim(C_DATI_PATH, '/\\') . '/export-import/packages');
    if ($base) $allowedBases[] = $base;
    $base = realpath(rtrim(C_DATI_PATH, '/\\') . '/export-runs');
    if ($base) $allowedBases[] = $base;
}
$base = realpath($hoteldruidRoot . '/export-import/packages');
if ($base) $allowedBases[] = $base;
$base = realpath($hoteldruidRoot . '/dati/export-runs');
if ($base) $allowedBases[] = $base;

$authorized = false;
foreach ($allowedBases as $base) {
    if (strpos($real, $base) === 0) {
        $authorized = true;
        break;
    }
}

if (!$authorized) {
    http_response_code(403);
    exit('forbidden');
}

$filename = basename($real);
header('Content-Type: application/zip');
header('Content-Disposition: attachment; filename="' . $filename . '"');
header('Content-Length: ' . filesize($real));

$fp = fopen($real, 'rb');
if ($fp) {
    while (!feof($fp)) {
        echo fread($fp, 8192);
        flush();
    }
    fclose($fp);
}
exit;
