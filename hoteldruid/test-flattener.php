<?php
/**
 * test-flattener.php
 * 
 * TEST SCRIPT: Export and Flatten Database Tables to JSON
 * 
 * Purpose: Validate that DataFlattener correctly converts database tables to JSON
 * Usage: Access via http://localhost:8080/hoteldruid/test-flattener.php (while logged in as admin)
 */

// Start output buffering
ob_start();

// Set error reporting 
error_reporting(E_ALL);
ini_set('display_errors', 0);

// Load hoteldruid-config.php to get C_DATI_PATH_EXTERNAL
if (file_exists(__DIR__ . '/hoteldruid-config.php')) {
    include(__DIR__ . '/hoteldruid-config.php');
}

// Set up C_DATI_PATH the same way costanti.php does it
if (!defined('C_DATI_PATH')) {
    $dati_path = null;
    
    if (defined('C_DATI_PATH_EXTERNAL') && C_DATI_PATH_EXTERNAL !== "" && C_DATI_PATH_EXTERNAL !== null) {
        $dati_path = C_DATI_PATH_EXTERNAL;
    } else {
        $dati_path = __DIR__ . '/dati';
    }
    
    // Normalize the path
    $dati_path = rtrim(str_replace('\\', '/', $dati_path), '/');
    
    // Check for absolute vs relative paths
    if (!preg_match('/^[A-Za-z]:\/|^\/|^\\\\/', $dati_path)) {
        $dati_path = __DIR__ . '/' . $dati_path;
    }
    
    define('C_DATI_PATH', $dati_path);
}

// Now load the database configuration directly from dati_connessione.php
if (file_exists(C_DATI_PATH . '/dati_connessione.php')) {
    include(C_DATI_PATH . '/dati_connessione.php');
}

// If variables are loaded as globals (from include), bring them into scope
if (!isset($PHPR_DB_TYPE) && isset($GLOBALS['PHPR_DB_TYPE'])) {
    $PHPR_DB_TYPE = $GLOBALS['PHPR_DB_TYPE'];
}
if (!isset($PHPR_DB_NAME) && isset($GLOBALS['PHPR_DB_NAME'])) {
    $PHPR_DB_NAME = $GLOBALS['PHPR_DB_NAME'];
}
if (!isset($PHPR_DB_HOST) && isset($GLOBALS['PHPR_DB_HOST'])) {
    $PHPR_DB_HOST = $GLOBALS['PHPR_DB_HOST'];
}
if (!isset($PHPR_DB_PORT) && isset($GLOBALS['PHPR_DB_PORT'])) {
    $PHPR_DB_PORT = $GLOBALS['PHPR_DB_PORT'];
}
if (!isset($PHPR_DB_USER) && isset($GLOBALS['PHPR_DB_USER'])) {
    $PHPR_DB_USER = $GLOBALS['PHPR_DB_USER'];
}
if (!isset($PHPR_DB_PASS) && isset($GLOBALS['PHPR_DB_PASS'])) {
    $PHPR_DB_PASS = $GLOBALS['PHPR_DB_PASS'];
}
if (!isset($PHPR_TAB_PRE) && isset($GLOBALS['PHPR_TAB_PRE'])) {
    $PHPR_TAB_PRE = $GLOBALS['PHPR_TAB_PRE'];
} elseif (defined('PHPR_TAB_PRE')) {
    // PHPR_TAB_PRE is defined but might be empty
    $PHPR_TAB_PRE = constant('PHPR_TAB_PRE');
}
if (!isset($PHPR_LOAD_EXT) && isset($GLOBALS['PHPR_LOAD_EXT'])) {
    $PHPR_LOAD_EXT = $GLOBALS['PHPR_LOAD_EXT'];
}

// NOTE: PHPR_TAB_PRE is empty in the config! This is the actual table prefix configured in the database.
// An empty PHPR_TAB_PRE means tables have NO prefix (e.g., "clienti", "contratti" instead of "pre_clienti")

// Load the functions we need for database operations
if (!function_exists('esegui_query')) {
    if (file_exists(__DIR__ . '/includes/funzioni.php')) {
        include(__DIR__ . '/includes/funzioni.php');
    }
}

// Connect to database using the configuration we just loaded
if (!isset($numconnessione) && !empty($PHPR_DB_TYPE) && !empty($PHPR_DB_NAME)) {
    $db_type_file = __DIR__ . '/includes/funzioni_' . $PHPR_DB_TYPE . '.php';
    if (file_exists($db_type_file)) {
        include($db_type_file);
        
        // Attempt connection
        if (function_exists('connetti_db')) {
            $numconnessione = @connetti_db(
                $PHPR_DB_NAME,
                $PHPR_DB_HOST ?? 'localhost',
                $PHPR_DB_PORT ?? 3306,
                $PHPR_DB_USER ?? '',
                $PHPR_DB_PASS ?? '',
                $PHPR_LOAD_EXT ?? ''
            );
        }
    }
}

// Get user ID from session if available, or from query parameter for testing
if (!isset($id_utente)) {
    if (session_status() == PHP_SESSION_NONE) {
        @session_start();
    }
    $id_utente = $_SESSION['id_utente'] ?? null;
    
    // For local testing: allow admin parameter (only works on localhost)
    if (!$id_utente && isset($_GET['admin']) && $_GET['admin'] === 'test') {
        if ($_SERVER['REMOTE_ADDR'] === '127.0.0.1' || $_SERVER['REMOTE_ADDR'] === 'localhost' || $_SERVER['REMOTE_ADDR'] === '::1') {
            $id_utente = 1; // Set as admin for local testing
        }
    }
}

// Clear any existing output buffers and enable real-time flushing
while (ob_get_level() > 0) {
    @ob_end_flush();
}
ob_implicit_flush(true);
// Allow long-running operations during export
@set_time_limit(0);
@ignore_user_abort(true);
// Ensure we send HTML progressively
header('Content-Type: text/html; charset=utf-8');

// Now check if we have what we need
// PHPR_TAB_PRE can be empty string, which is valid - it just means no prefix
if (empty($PHPR_DB_TYPE) || !isset($numconnessione)) {
    ?>
    <!DOCTYPE html>
    <html>
    <head>
        <meta charset="utf-8">
        <title>Configuration Error</title>
        <style>
            body { font-family: Arial, sans-serif; margin: 40px; background: #f5f5f5; }
            .container { background: white; padding: 30px; border-radius: 5px; box-shadow: 0 2px 5px rgba(0,0,0,0.1); max-width: 900px; margin: 0 auto; }
            h2 { color: #ff9800; }
            p { color: #666; }
            .code { background: #f0f0f0; padding: 10px; border-radius: 3px; font-family: monospace; }
            table { border-collapse: collapse; width: 100%; margin: 15px 0; }
            th, td { border: 1px solid #ddd; padding: 10px; text-align: left; }
            th { background: #f0f0f0; }
            .debug-box { background: #f9f9f9; padding: 15px; margin: 15px 0; border-radius: 4px; border: 1px solid #ddd; }
        </style>
    </head>
    <body>
        <div class="container">
            <h2>⚠️ Configuration Error</h2>
            <p>Cannot initialize database configuration. Missing variables:</p>
            <div class="code">
                PHPR_DB_TYPE: <?php echo empty($PHPR_DB_TYPE) ? '<strong style="color: red;">NOT SET</strong>' : htmlspecialchars($PHPR_DB_TYPE); ?><br>
                PHPR_TAB_PRE: <?php echo empty($PHPR_TAB_PRE) ? '<strong style="color: red;">NOT SET</strong>' : htmlspecialchars($PHPR_TAB_PRE); ?><br>
                numconnessione: <?php echo !isset($numconnessione) ? '<strong style="color: red;">NOT SET</strong>' : 'Set'; ?><br>
                id_utente: <?php echo !isset($id_utente) ? '<strong style="color: red;">NOT SET</strong>' : htmlspecialchars($id_utente); ?>
            </div>
            
            <h3>Detailed Debug Information</h3>
            
            <div class="debug-box">
                <h4>Step 1: Paths</h4>
                C_DATI_PATH: <code><?php echo defined('C_DATI_PATH') ? htmlspecialchars(C_DATI_PATH) : 'NOT DEFINED'; ?></code><br>
                Path exists: <?php echo (defined('C_DATI_PATH') && is_dir(C_DATI_PATH)) ? '<span style="color: green;">✓ YES</span>' : '<span style="color: red;">✗ NO</span>'; ?><br>
                
                <?php
                $config_file = defined('C_DATI_PATH') ? C_DATI_PATH . '/dati_connessione.php' : 'NOT DETERMINABLE';
                ?>
                Config file path: <code><?php echo htmlspecialchars($config_file); ?></code><br>
                Config file exists: <?php echo file_exists($config_file) ? '<span style="color: green;">✓ YES</span>' : '<span style="color: red;">✗ NO</span>'; ?><br>
            </div>
            
            <div class="debug-box">
                <h4>Step 2: Loaded Variables</h4>
                <table>
                    <tr>
                        <th>Variable/Constant</th>
                        <th>Status</th>
                        <th>Value</th>
                    </tr>
                    <?php
                    $checks = [
                        'PHPR_DB_TYPE' => ['type' => 'constant', 'mask' => false],
                        'PHPR_DB_NAME' => ['type' => 'constant', 'mask' => false],
                        'PHPR_DB_HOST' => ['type' => 'constant', 'mask' => false],
                        'PHPR_DB_PORT' => ['type' => 'constant', 'mask' => false],
                        'PHPR_DB_USER' => ['type' => 'constant', 'mask' => false],
                        'PHPR_DB_PASS' => ['type' => 'constant', 'mask' => true],
                        'PHPR_TAB_PRE' => ['type' => 'constant', 'mask' => false],
                        'PHPR_LOAD_EXT' => ['type' => 'constant', 'mask' => false],
                    ];
                    
                    foreach ($checks as $name => $info) {
                        echo '<tr>';
                        echo '<td><code>' . htmlspecialchars($name) . '</code></td>';
                        
                        if (defined($name)) {
                            $value = constant($name);
                            if ($info['mask']) {
                                $value = '*** MASKED ***';
                            }
                            echo '<td style="color: green;">✓ Defined</td>';
                            echo '<td><code>' . htmlspecialchars(is_array($value) ? json_encode($value) : (is_null($value) ? 'NULL' : $value)) . '</code></td>';
                        } elseif (isset($GLOBALS[$name])) {
                            echo '<td style="color: blue;">Global Variable</td>';
                            echo '<td><code>' . htmlspecialchars($GLOBALS[$name]) . '</code></td>';
                        } else {
                            echo '<td style="color: red;">✗ NOT SET</td>';
                            echo '<td>-</td>';
                        }
                        echo '</tr>';
                    }
                    ?>
                </table>
            </div>
            
            <?php
            if (file_exists($config_file)) {
                echo '<div class="debug-box">';
                echo '<h4>Step 3: Config File Contents (First 40 lines)</h4>';
                $lines = file($config_file);
                echo '<pre style="background: #fff; border: 1px solid #ddd; padding: 10px; overflow-x: auto; font-size: 11px; max-height: 400px; overflow-y: auto;">';
                for ($i = 0; $i < min(40, count($lines)); $i++) {
                    $line = rtrim($lines[$i]);
                    // Mask passwords and sensitive info
                    if (preg_match('/(password|pass|user|key|secret)/i', $line)) {
                        $line = '*** SENSITIVE DATA MASKED ***';
                    }
                    echo htmlspecialchars($line) . "\n";
                }
                echo '</pre>';
                echo '</div>';
            }
            ?>
            
            <p style="margin-top: 30px; color: #999;">Please check the configuration above and ensure the dati folder is correctly located.</p>
        </div>
    </body>
    </html>
    <?php
    exit;
}

// Security check: Only allow admin user or local test access
if (!isset($id_utente) || ($id_utente != 1 && !($_SERVER['REMOTE_ADDR'] === '127.0.0.1' && isset($_GET['admin'])))) {
    ?>
    <!DOCTYPE html>
    <html>
    <head>
        <meta charset="utf-8">
        <title>Access Denied</title>
        <style>
            body { font-family: Arial, sans-serif; margin: 40px; background: #f5f5f5; }
            .container { background: white; padding: 30px; border-radius: 5px; box-shadow: 0 2px 5px rgba(0,0,0,0.1); max-width: 600px; margin: 0 auto; }
            h2 { color: #f44336; }
            p { color: #666; }
            .hint { background: #f0f0f0; padding: 10px; border-radius: 3px; margin-top: 20px; }
        </style>
    </head>
    <body>
        <div class="container">
            <h2>❌ Access Denied</h2>
            <p>Only administrator (User ID=1) can run this test.</p>
            <p><strong>Current User ID:</strong> <?php echo isset($id_utente) ? htmlspecialchars($id_utente) : 'NOT SET'; ?></p>
            <p>Please log in as the administrator and try again.</p>
            
            <div class="hint">
                <p><strong>Testing locally?</strong> You can add <code>?admin=test</code> to the URL:</p>
                <p style="font-family: monospace; background: white; padding: 10px; border: 1px solid #ddd;">
                    http://localhost:8080/hoteldruid/test-flattener.php?admin=test
                </p>
            </div>
        </div>
    </body>
    </html>
    <?php
    exit;
}

// Include the DataFlattener class
require_once('./export-import/lib/DataFlattener.php');

?>
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <title>Test: Data Flattener</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            margin: 20px;
            background-color: #f5f5f5;
        }
        .container {
            max-width: 1200px;
            margin: 0 auto;
            background: white;
            padding: 20px;
            border-radius: 5px;
            box-shadow: 0 2px 5px rgba(0,0,0,0.1);
        }
        h1 {
            color: #333;
            border-bottom: 3px solid #4CAF50;
            padding-bottom: 10px;
        }
        h2 {
            color: #555;
            margin-top: 30px;
        }
        .info-box {
            background-color: #e8f5e9;
            border-left: 4px solid #4CAF50;
            padding: 15px;
            margin: 15px 0;
        }
        .warning-box {
            background-color: #fff3e0;
            border-left: 4px solid #ff9800;
            padding: 15px;
            margin: 15px 0;
        }
        .error-box {
            background-color: #ffebee;
            border-left: 4px solid #f44336;
            padding: 15px;
            margin: 15px 0;
        }
        .json-output {
            background-color: #f9f9f9;
            border: 1px solid #ddd;
            padding: 15px;
            margin: 15px 0;
            overflow-x: auto;
            border-radius: 4px;
            font-family: 'Courier New', monospace;
            font-size: 12px;
            line-height: 1.4;
        }
        .table-stats {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 15px;
            margin: 20px 0;
        }
        .stat-card {
            background: #f5f5f5;
            padding: 15px;
            border-radius: 4px;
            border-left: 4px solid #2196F3;
        }
        .stat-card h3 {
            margin: 0 0 10px 0;
            color: #2196F3;
            font-size: 14px;
        }
        .stat-card .number {
            font-size: 24px;
            font-weight: bold;
            color: #333;
        }
        .controls {
            margin: 20px 0;
        }
        .controls label {
            margin-right: 20px;
            display: inline-block;
        }
        .controls input[type="checkbox"] {
            margin-right: 5px;
        }
        button {
            background-color: #4CAF50;
            color: white;
            padding: 10px 20px;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            font-size: 14px;
        }
        button:hover {
            background-color: #45a049;
        }
        .table-list {
            margin: 20px 0;
        }
        .table-list li {
            padding: 5px;
            list-style: none;
        }
        .table-list li:before {
            content: "✓ ";
            color: #4CAF50;
            font-weight: bold;
            margin-right: 10px;
        }
        .record-preview {
            background-color: #f0f0f0;
            padding: 10px;
            margin: 10px 0;
            border-radius: 3px;
            font-size: 12px;
        }
    </style>
</head>
<body>

<div class="container">
    <h1>Data Flattener Test - Validation Tool</h1>
    
    <div class="info-box">
        <strong>Purpose:</strong> This tool exports your HotelDroid database tables in flattened JSON format.
        You can review the exported structure and compare it with the actual database data to validate
        that the flattening process works correctly.
    </div>

    <?php
    
    // Show detailed debug information about configuration loading
    echo '<h2>Detailed Configuration Debug</h2>';
    
    echo '<div style="background: #f9f9f9; padding: 15px; margin: 15px 0; border-radius: 4px;">';
    echo '<h3>Step 1: Paths</h3>';
    echo 'C_DATI_PATH: <code>' . (defined('C_DATI_PATH') ? htmlspecialchars(C_DATI_PATH) : 'NOT DEFINED') . '</code><br>';
    echo 'File exists: ' . (defined('C_DATI_PATH') && file_exists(C_DATI_PATH) ? '<span style="color: green;">✓ YES</span>' : '<span style="color: red;">✗ NO</span>') . '<br>';
    
    $config_file = defined('C_DATI_PATH') ? C_DATI_PATH . '/dati_connessione.php' : 'NOT DETERMINABLE';
    echo 'Config file: <code>' . htmlspecialchars($config_file) . '</code><br>';
    echo 'Config file exists: ' . (file_exists($config_file) ? '<span style="color: green;">✓ YES</span>' : '<span style="color: red;">✗ NO</span>') . '<br>';
    echo '</div>';
    
    echo '<div style="background: #f9f9f9; padding: 15px; margin: 15px 0; border-radius: 4px;">';
    echo '<h3>Step 2: Loaded Variables After Including dati_connessione.php</h3>';
    echo '<table border="1" cellpadding="10">';
    echo '<tr><th>Variable/Constant</th><th>Type</th><th>Value</th></tr>';
    
    $checks = [
        'PHPR_DB_TYPE' => 'constant',
        'PHPR_DB_NAME' => 'constant',
        'PHPR_DB_HOST' => 'constant',
        'PHPR_DB_PORT' => 'constant',
        'PHPR_DB_USER' => 'constant',
        'PHPR_DB_PASS' => 'constant',
        'PHPR_TAB_PRE' => 'constant',
        'PHPR_LOAD_EXT' => 'constant',
    ];
    
    foreach ($checks as $name => $type) {
        echo '<tr>';
        echo '<td><code>' . htmlspecialchars($name) . '</code></td>';
        
        if (defined($name)) {
            $value = constant($name);
            if (stripos($name, 'PASS') !== false) {
                $value = '*** MASKED ***';
            }
            echo '<td>Constant</td>';
            echo '<td><code>' . htmlspecialchars(is_array($value) ? json_encode($value) : $value) . '</code></td>';
        } elseif (isset(${'GLOBALS'}[$name])) {
            echo '<td>Global Variable</td>';
            echo '<td><code>' . htmlspecialchars(${'GLOBALS'}[$name]) . '</code></td>';
        } else {
            echo '<td colspan="2"><span style="color: red;">NOT SET</span></td>';
        }
        echo '</tr>';
    }
    
    echo '</table>';
    echo '</div>';
    
    echo '<div style="background: #f9f9f9; padding: 15px; margin: 15px 0; border-radius: 4px;">';
    echo '<h3>Step 3: File Contents (First 30 lines)</h3>';
    if (file_exists($config_file)) {
        $lines = file($config_file, FILE_SKIP_EMPTY_LINES);
        echo '<pre style="background: #fff; border: 1px solid #ddd; padding: 10px; overflow-x: auto; font-size: 12px;">';
        for ($i = 0; $i < min(30, count($lines)); $i++) {
            $line = rtrim($lines[$i]);
            // Mask passwords
            if (stripos($line, 'password') !== false || stripos($line, 'pass') !== false) {
                $line = '*** PASSWORD MASKED ***';
            }
            echo htmlspecialchars($line) . "\n";
        }
        echo '</pre>';
    } else {
        echo '<span style="color: red;">Config file not found!</span>';
    }
    echo '</div>';
    
    // Now create the flattener instance and process the data
        try {
            // Provide a lightweight logger that echoes progress, flushes output, and writes to a logfile
            $logfile = (defined('C_DATI_PATH') ? C_DATI_PATH : sys_get_temp_dir()) . '/hoteldruid_flatten.log';
            // Clear previous logfile for this run
            @file_put_contents($logfile, "--- Flatten run: " . date('c') . " ---\n");

            $logger = new class($logfile) {
                private $file;
                public function __construct($file) {
                    $this->file = $file;
                }
                private function writeLog($level, $msg) {
                    $line = '[' . date('Y-m-d H:i:s') . '] ' . $level . ': ' . $msg . PHP_EOL;
                    @file_put_contents($this->file, $line, FILE_APPEND | LOCK_EX);
                }
                public function info($msg) {
                    echo '<div class="info-box">Progress: ' . htmlspecialchars($msg) . '</div>';
                    $this->writeLog('INFO', $msg);
                    if (ob_get_length()) { @ob_flush(); }
                    @flush();
                }
                public function error($msg) {
                    echo '<div class="error-box">Error: ' . htmlspecialchars($msg) . '</div>';
                    $this->writeLog('ERROR', $msg);
                    if (ob_get_length()) { @ob_flush(); }
                    @flush();
                }
            };
            // Expose logfile path to the page for convenience
            echo '<div class="info-box">Log file: <code>' . htmlspecialchars($logfile) . '</code></div>';
            if (ob_get_length()) { @ob_flush(); }
            @flush();

            $flattener = new DataFlattener($numconnessione, $PHPR_TAB_PRE ?? '', $PHPR_DB_TYPE, $logger);

    ?>

            <div class="info-box" id="db-config">
            <strong>Database Configuration:</strong><br>
            Database Type (PHPR_DB_TYPE): <code><?php echo (defined('PHPR_DB_TYPE') ? htmlspecialchars(PHPR_DB_TYPE) : 'NOT DEFINED'); ?></code><br>
            Table Prefix (PHPR_TAB_PRE): <code><?php echo (defined('PHPR_TAB_PRE') ? htmlspecialchars(PHPR_TAB_PRE) : (isset($PHPR_TAB_PRE) ? htmlspecialchars($PHPR_TAB_PRE) : 'NOT SET')); ?></code><br>
            Database Name (PHPR_DB_NAME): <code><?php echo (defined('PHPR_DB_NAME') ? htmlspecialchars(PHPR_DB_NAME) : 'NOT DEFINED'); ?></code><br>
            Data Path (C_DATI_PATH): <code><?php echo (defined('C_DATI_PATH') ? htmlspecialchars(C_DATI_PATH) : 'NOT DEFINED'); ?></code><br>
            Connection Status: <strong><?php echo (isset($numconnessione) && $numconnessione ? 'Connected' : 'NOT CONNECTED'); ?></strong>
            </div>
        
        <?php
            // Start flattening process (single run)
            // First, list discovered tables so the user sees a checklist immediately
            $tables_to_process = $flattener->getAllTables();
            echo '<h2>Tables to Process (' . count($tables_to_process) . ')</h2>';
            echo '<ul class="table-list" id="tables-checklist">';
            foreach ($tables_to_process as $tname) {
                echo '<li><label><input type="checkbox" disabled> <code>' . htmlspecialchars($tname) . '</code></label></li>';
            }
            echo '</ul>';
            if (ob_get_length()) { @ob_flush(); }
            @flush();

            echo '<h2>Flattening Database...</h2>';
            echo '<div class="info-box">Processing all tables to JSON format. This may take a moment...</div>';
            if (ob_get_length()) { @ob_flush(); }
            @flush();

            // Temporary: enable error display and convert errors to exceptions to capture fatal issues
            ini_set('display_errors', 1);
            set_error_handler(function($errno, $errstr, $errfile, $errline) {
                throw new \ErrorException($errstr, 0, $errno, $errfile, $errline);
            });
            register_shutdown_function(function() {
                $err = error_get_last();
                if ($err) {
                    echo '<div class="error-box"><strong>Shutdown Error:</strong> ' . htmlspecialchars($err['message']) . ' in ' . htmlspecialchars($err['file']) . ' on line ' . htmlspecialchars($err['line']) . '</div>';
                    if (ob_get_length()) { @ob_flush(); }
                    @flush();
                }
            });

            echo '<div class="info-box">About to call flattenDatabase()</div>';
            if (ob_get_length()) { @ob_flush(); }
            @flush();

            $flattened_data = $flattener->flattenDatabase();

            echo '<div class="info-box">flattenDatabase() returned</div>';
            if (ob_get_length()) { @ob_flush(); }
            @flush();
        
        if (empty($flattened_data)) {
            echo '<div class="error-box"><strong>Error:</strong> No data was flattened. Check that tables exist in the database.</div>';
        } else {
            // Show statistics
            echo '<h2>Flattening Results</h2>';
            echo '<div class="table-stats">';
            
            $total_tables = count($flattened_data);
            $total_records = 0;
            foreach ($flattened_data as $table_name => $table_data) {
                if (isset($table_data['row_count'])) {
                    $total_records += $table_data['row_count'];
                }
            }
            
            echo '<div class="stat-card">';
            echo '<h3>Total Tables</h3>';
            echo '<div class="number">' . $total_tables . '</div>';
            echo '</div>';
            
            echo '<div class="stat-card">';
            echo '<h3>Total Records</h3>';
            echo '<div class="number">' . $total_records . '</div>';
            echo '</div>';
            
            echo '</div>';
            
            // Show table list
            echo '<h2>Tables Processed (' . $total_tables . ')</h2>';
            echo '<ul class="table-list">';
            foreach ($flattened_data as $table_name => $table_data) {
                $count = isset($table_data['row_count']) ? $table_data['row_count'] : 0;
                echo '<li><code>' . htmlspecialchars($table_name) . '</code> (' . $count . ' records)</li>';
            }
            echo '</ul>';
            
            // Show JSON output
            echo '<h2>Flattened JSON Data</h2>';
            echo '<div class="warning-box">';
            echo '<strong>Instructions for Validation:</strong>';
            echo '<ol>';
            echo '<li>Review the JSON structure below</li>';
            echo '<li>Compare sample records with the actual database using your HotelDroid interface</li>';
            echo '<li>Verify that all fields are present and values are correct</li>';
            echo '<li>Check that relationships between tables are properly represented</li>';
            echo '<li>Note any missing or unexpected fields</li>';
            echo '</ol>';
            echo '</div>';
            
            // Pretty-print JSON
            $json_output = json_encode($flattened_data, JSON_PRETTY_PRINT | JSON_UNESCAPED_UNICODE);
            echo '<div class="json-output">';
            echo '<pre>' . htmlspecialchars($json_output) . '</pre>';
            echo '</div>';
            
            // Offer download option
            echo '<h2>Export Options</h2>';
            echo '<div class="info-box">';
            echo '<p><strong>Save the JSON for comparison:</strong></p>';
            
            // Create a downloadable JSON file
            $timestamp = date('Y-m-d_H-i-s');
            
            echo '<a href="?download=1" style="display: inline-block; background-color: #2196F3; color: white; padding: 10px 20px; text-decoration: none; border-radius: 4px; margin: 10px 0;">⬇ Download JSON File</a>';
            echo '<br><small>File will be saved as: <code>hoteldruid-flatten-test-' . htmlspecialchars($timestamp) . '.json</code></small>';
            echo '</div>';
            
            // Show table-by-table breakdown
            echo '<h2>Per-Table Details</h2>';
            foreach ($flattened_data as $table_name => $table_data) {
                $record_count = isset($table_data['row_count']) ? $table_data['row_count'] : 0;
                echo '<div style="margin: 20px 0; border: 1px solid #ddd; padding: 15px; border-radius: 4px;">';
                echo '<h3 style="margin-top: 0;">Table: <code>' . htmlspecialchars($table_name) . '</code></h3>';
                echo '<p><strong>Records:</strong> ' . $record_count . '</p>';
                
                if (isset($table_data['columns']) && is_array($table_data['columns'])) {
                    echo '<p><strong>Columns:</strong> ' . count($table_data['columns']) . '</p>';
                    echo '<div class="record-preview">';
                    echo '<strong>Field Names:</strong><br>';
                    $col_names = array();
                    foreach ($table_data['columns'] as $col_info) {
                        if (is_array($col_info) && isset($col_info['name'])) {
                            $col_names[] = htmlspecialchars($col_info['name']);
                        }
                    }
                    echo implode(', ', $col_names);
                    echo '</div>';
                }
                
                if (isset($table_data['rows']) && is_array($table_data['rows']) && count($table_data['rows']) > 0) {
                    echo '<p><strong>Sample Record (first record):</strong></p>';
                    $first_record = reset($table_data['rows']);
                    echo '<div class="record-preview">';
                    if (is_array($first_record)) {
                        foreach ($first_record as $field => $value) {
                            $display_value = is_string($value) ? htmlspecialchars(substr($value, 0, 100)) : htmlspecialchars(json_encode($value));
                            echo '<div><strong>' . htmlspecialchars($field) . ':</strong> ' . $display_value . '</div>';
                        }
                    }
                    echo '</div>';
                }
                echo '</div>';
            }
        }
        } catch (Exception $e) {
            echo '<div class="error-box">';
            echo '<strong>Error during flattening:</strong><br>';
            echo htmlspecialchars($e->getMessage());
            echo '<br><br><strong>Stack Trace:</strong><br>';
            echo '<pre>' . htmlspecialchars($e->getTraceAsString()) . '</pre>';
            echo '</div>';
        }
