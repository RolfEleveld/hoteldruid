<?php
/**
 * HotelDruid Data Path Configuration Checker
 * 
 * This script helps verify that your data path configuration is correct.
 * Run this from the command line or via browser to check your setup.
 */

// Start output buffering for browser formatting
if (php_sapi_name() !== 'cli') {
    ob_start();
    echo "<!DOCTYPE html><html><head><title>HotelDruid Data Path Check</title></head><body>";
    echo "<h1>HotelDruid Data Path Configuration Check</h1>";
    echo "<pre>";
}

// Include the constants file to get the configured path
include(__DIR__ . '/costanti.php');

echo "=== HotelDruid Data Path Configuration Check ===\n\n";

// Check if config file exists
$config_file = __DIR__ . '/hoteldruid-config.php';
if (file_exists($config_file)) {
    echo "✓ Configuration file found: hoteldruid-config.php\n";
    include($config_file);
    
    if (defined('C_DATI_PATH_EXTERNAL')) {
        echo "✓ C_DATI_PATH_EXTERNAL is defined\n";
        echo "  Configured path: " . C_DATI_PATH_EXTERNAL . "\n";
    } else {
        echo "⚠ C_DATI_PATH_EXTERNAL is not defined in config file\n";
    }
} else {
    echo "⚠ Configuration file not found: hoteldruid-config.php\n";
    echo "  Using default path: ./dati\n";
}

echo "\n";

// Check the actual path being used
if (defined('C_DATI_PATH')) {
    echo "✓ C_DATI_PATH is defined\n";
    echo "  Active data path: " . C_DATI_PATH . "\n";
    
    // Resolve to absolute path for display
    $absolute_path = realpath(C_DATI_PATH);
    if ($absolute_path === false) {
        // Path doesn't exist yet, show what it would be
        if (preg_match('/^[A-Za-z]:\/|^\/|^\\\\/', C_DATI_PATH)) {
            $absolute_path = C_DATI_PATH . " (absolute path, not yet created)";
        } else {
            $absolute_path = realpath(__DIR__) . '/' . C_DATI_PATH . " (relative path, not yet created)";
        }
    }
    echo "  Resolved absolute path: " . $absolute_path . "\n";
    
    // Check if directory exists
    if (is_dir(C_DATI_PATH)) {
        echo "✓ Data directory exists\n";
        
        // Check permissions
        if (is_writable(C_DATI_PATH)) {
            echo "✓ Data directory is writable\n";
        } else {
            echo "✗ Data directory is NOT writable\n";
            echo "  You may need to adjust file permissions\n";
        }
    } else {
        echo "⚠ Data directory does not exist yet\n";
        echo "  It will be created on first run\n";
        
        // Check if parent directory is writable
        $parent_dir = dirname(C_DATI_PATH);
        if (is_dir($parent_dir)) {
            if (is_writable($parent_dir)) {
                echo "✓ Parent directory is writable (directory can be created)\n";
            } else {
                echo "✗ Parent directory is NOT writable (cannot create directory)\n";
            }
        }
    }
} else {
    echo "✗ C_DATI_PATH is not defined - this should not happen!\n";
}

echo "\n=== Check Complete ===\n";

// Close HTML formatting if running in browser
if (php_sapi_name() !== 'cli') {
    echo "</pre>";
    echo "</body></html>";
}

?>

