<?php
/**
 * test-config-debug.php
 * 
 * Diagnostic script to debug configuration issues
 */
?>
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <title>Configuration Debug</title>
    <style>
        body { font-family: monospace; margin: 20px; background: #f5f5f5; }
        .container { background: white; padding: 20px; border-radius: 5px; }
        h2 { background: #333; color: white; padding: 10px; margin-top: 20px; }
        .info { background: #e3f2fd; padding: 10px; margin: 10px 0; border-left: 4px solid #2196F3; }
        .error { background: #ffebee; padding: 10px; margin: 10px 0; border-left: 4px solid #f44336; }
        .success { background: #e8f5e9; padding: 10px; margin: 10px 0; border-left: 4px solid #4CAF50; }
        code { background: #f0f0f0; padding: 2px 5px; }
    </style>
</head>
<body>

<div class="container">
    <h1>HotelDroid Configuration Debug</h1>

    <h2>1. Check Directory Paths</h2>
    <?php
    $current_dir = dirname(__FILE__);
    $script_name = basename(__FILE__);
    
    echo '<div class="info">';
    echo 'Current Directory: <code>' . htmlspecialchars($current_dir) . '</code><br>';
    echo 'Script Name: <code>' . htmlspecialchars($script_name) . '</code><br>';
    echo '</div>';
    
    // Try to find dati directory
    $dati_path = $current_dir . '/../../dati';
    $dati_path_real = realpath($dati_path);
    
    if ($dati_path_real) {
        echo '<div class="success">';
        echo 'dati Directory Found: <code>' . htmlspecialchars($dati_path_real) . '</code>';
        echo '</div>';
    } else {
        echo '<div class="error">';
        echo 'dati Directory NOT FOUND at: <code>' . htmlspecialchars($dati_path) . '</code>';
        echo '</div>';
    }
    ?>

    <h2>2. Check for dati_connessione.php</h2>
    <?php
    $conn_file = $dati_path . '/dati_connessione.php';
    
    if (file_exists($conn_file)) {
        echo '<div class="success">';
        echo 'dati_connessione.php FOUND at: <code>' . htmlspecialchars($conn_file) . '</code>';
        echo '</div>';
        
        // Try to read the file to see what's in it
        echo '<h3>File Contents (first 50 lines):</h3>';
        $lines = file($conn_file);
        echo '<pre style="background: #f0f0f0; padding: 10px; overflow-x: auto;">';
        for ($i = 0; $i < min(50, count($lines)); $i++) {
            $line = rtrim($lines[$i]);
            // Mask passwords
            if (stripos($line, 'password') !== false || stripos($line, 'pass') !== false) {
                $line = '*** PASSWORD LINE MASKED ***';
            }
            echo htmlspecialchars($line) . "\n";
        }
        echo '</pre>';
    } else {
        echo '<div class="error">';
        echo 'dati_connessione.php NOT FOUND at: <code>' . htmlspecialchars($conn_file) . '</code>';
        echo '</div>';
    }
    ?>

    <h2>3. Try Loading Configuration</h2>
    <?php
    // Try to load the configuration file
    if (file_exists($conn_file)) {
        echo '<div class="info">Attempting to include dati_connessione.php...</div>';
        
        // Suppress warnings
        @include_once($conn_file);
        
        echo '<h3>Variables Set After Include:</h3>';
        echo '<ul>';
        
        $vars_to_check = [
            'PHPR_DB_TYPE',
            'PHPR_DB_HOST',
            'PHPR_DB_PORT',
            'PHPR_DB_NAME',
            'PHPR_DB_USER',
            'PHPR_DB_PASS',
            'PHPR_TAB_PRE',
            'PHPR_LOAD_EXT'
        ];
        
        foreach ($vars_to_check as $var) {
            if (defined($var)) {
                $value = constant($var);
                if (stripos($var, 'PASS') !== false) {
                    $value = '*** MASKED ***';
                }
                echo '<li><strong>' . htmlspecialchars($var) . ':</strong> <code>' . htmlspecialchars($value) . '</code></li>';
            } else {
                echo '<li><strong>' . htmlspecialchars($var) . ':</strong> <span style="color: red;">NOT DEFINED</span></li>';
            }
        }
        
        echo '</ul>';
    }
    ?>

    <h2>4. Try to Connect to Database</h2>
    <?php
    if (defined('PHPR_DB_TYPE') && PHPR_DB_TYPE) {
        echo '<div class="info">';
        echo 'Database Type: <code>' . htmlspecialchars(PHPR_DB_TYPE) . '</code><br>';
        echo 'Host: <code>' . htmlspecialchars(PHPR_DB_HOST ?? 'NOT SET') . '</code><br>';
        echo 'Database: <code>' . htmlspecialchars(PHPR_DB_NAME ?? 'NOT SET') . '</code><br>';
        echo '</div>';
        
        // Try to connect based on database type
        if (PHPR_DB_TYPE === 'mysql' || PHPR_DB_TYPE === 'mysqli') {
            echo '<h3>Attempting MySQL Connection...</h3>';
            
            try {
                $conn = @mysqli_connect(
                    PHPR_DB_HOST,
                    PHPR_DB_USER,
                    PHPR_DB_PASS,
                    PHPR_DB_NAME,
                    PHPR_DB_PORT ?? 3306
                );
                
                if ($conn) {
                    echo '<div class="success">✓ Database Connection SUCCESSFUL</div>';
                    
                    // List tables
                    $result = mysqli_query($conn, "SHOW TABLES");
                    if ($result) {
                        $tables = [];
                        while ($row = mysqli_fetch_row($result)) {
                            $tables[] = $row[0];
                        }
                        
                        echo '<h3>Tables Found (' . count($tables) . '):</h3>';
                        echo '<div class="success">';
                        echo implode(', ', array_map('htmlspecialchars', array_slice($tables, 0, 10)));
                        if (count($tables) > 10) {
                            echo '... and ' . (count($tables) - 10) . ' more';
                        }
                        echo '</div>';
                    }
                    
                    mysqli_close($conn);
                } else {
                    echo '<div class="error">✗ Connection Failed: ' . htmlspecialchars(mysqli_connect_error()) . '</div>';
                }
            } catch (Exception $e) {
                echo '<div class="error">✗ Error: ' . htmlspecialchars($e->getMessage()) . '</div>';
            }
        } else {
            echo '<div class="info">Database type is ' . htmlspecialchars(PHPR_DB_TYPE) . ' - Connection test not implemented for this type</div>';
        }
    } else {
        echo '<div class="error">Cannot test connection: PHPR_DB_TYPE is not defined</div>';
    }
    ?>

    <h2>5. Check inizio.php</h2>
    <?php
    $inizio_file = $current_dir . '/inizio.php';
    
    if (file_exists($inizio_file)) {
        echo '<div class="success">inizio.php FOUND at: <code>' . htmlspecialchars($inizio_file) . '</code></div>';
    } else {
        echo '<div class="error">inizio.php NOT FOUND</div>';
    }
    ?>

</div>

</body>
</html>
