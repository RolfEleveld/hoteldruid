<?php

##################################################################################
#    HOTELDRUID
#    Router for PHP built-in server
#    Handles security and routing for HotelDruid
##################################################################################

$requestUri = $_SERVER['REQUEST_URI'];
$requestPath = parse_url($requestUri, PHP_URL_PATH);
$filePath = __DIR__ . $requestPath;

// Security: Block direct access to includes/themes PHP files
if (preg_match('#/(includes|themes/[^/]+/php)/.*\.php$#', $requestPath)) {
    // Allow CSS/JS files
    if (preg_match('#\.(css|js)$#', $requestPath)) {
        return false; // Let PHP server handle it
    }
    http_response_code(403);
    die('Access denied');
}

// Serve static files (CSS, JS, images, etc.)
if (file_exists($filePath) && !is_dir($filePath)) {
    // Set appropriate MIME types
    $extension = strtolower(pathinfo($filePath, PATHINFO_EXTENSION));
    $mimeTypes = array(
        'css' => 'text/css',
        'js' => 'application/javascript',
        'json' => 'application/json',
        'png' => 'image/png',
        'jpg' => 'image/jpeg',
        'jpeg' => 'image/jpeg',
        'gif' => 'image/gif',
        'ico' => 'image/x-icon',
        'svg' => 'image/svg+xml',
        'html' => 'text/html',
        'txt' => 'text/plain'
    );
    
    if (isset($mimeTypes[$extension])) {
        header('Content-Type: ' . $mimeTypes[$extension]);
    }
    
    return false; // Let PHP server handle static files
}

// Route to index or inizio.php
if ($requestPath === '/' || $requestPath === '') {
    $_SERVER['SCRIPT_NAME'] = '/inizio.php';
    require __DIR__ . '/inizio.php';
    return true;
}

// Route PHP files
if (file_exists($filePath) && is_file($filePath) && preg_match('#\.php$#', $filePath)) {
    return false; // Let PHP server execute it
}

// 404
http_response_code(404);
die('File not found');

