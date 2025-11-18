<?php

##################################################################################
#    HOTELDRUID SECURITY ENHANCEMENTS
#    Modern security improvements for login and session management
##################################################################################

/**
 * Generate CSRF token for forms
 */
function generate_csrf_token() {
    if (!isset($_SESSION['csrf_token'])) {
        $_SESSION['csrf_token'] = bin2hex(random_bytes(32));
    }
    return $_SESSION['csrf_token'];
}

/**
 * Validate CSRF token
 */
function validate_csrf_token($token) {
    return isset($_SESSION['csrf_token']) && hash_equals($_SESSION['csrf_token'], $token);
}

/**
 * Enhanced input sanitization
 */
function sanitize_input($input, $type = 'string') {
    $input = trim($input);
    
    switch ($type) {
        case 'email':
            return filter_var($input, FILTER_SANITIZE_EMAIL);
        case 'int':
            return filter_var($input, FILTER_SANITIZE_NUMBER_INT);
        case 'url':
            return filter_var($input, FILTER_SANITIZE_URL);
        case 'string':
        default:
            return htmlspecialchars($input, ENT_QUOTES, 'UTF-8');
    }
}

/**
 * Rate limiting for login attempts
 */
function check_rate_limit($ip, $username = '') {
    global $PHPR_TAB_PRE, $numconnessione;
    
    $key = 'login_attempts_' . md5($ip . $username);
    $max_attempts = 5;
    $time_window = 300; // 5 minutes
    
    if (!isset($numconnessione) || !$numconnessione) {
        include(C_DATI_PATH."/dati_connessione.php");
        include("./includes/funzioni_$PHPR_DB_TYPE.php");
        $numconnessione = connetti_db($PHPR_DB_NAME,$PHPR_DB_HOST,$PHPR_DB_PORT,$PHPR_DB_USER,$PHPR_DB_PASS,$PHPR_LOAD_EXT);
    }
    
    $current_time = time();
    $time_limit = date("Y-m-d H:i:s", $current_time - $time_window);
    
    // Clean old attempts
    esegui_query("DELETE FROM $PHPR_TAB_PRE"."transazioni WHERE tipo_transazione = 'rate_limit' AND ultimo_accesso < '$time_limit'");
    
    // Count recent attempts
    $attempts = esegui_query("SELECT COUNT(*) as count FROM $PHPR_TAB_PRE"."transazioni WHERE tipo_transazione = 'rate_limit' AND dati_transazione1 = '$key'");
    $count = risul_query($attempts, 0, 'count');
    
    if ($count >= $max_attempts) {
        return false;
    }
    
    return true;
}

/**
 * Log rate limit attempt
 */
function log_rate_limit_attempt($ip, $username = '') {
    global $PHPR_TAB_PRE;
    
    $key = 'login_attempts_' . md5($ip . $username);
    $current_time = date("Y-m-d H:i:s", time() + (C_DIFF_ORE * 3600));
    $id_transazione = crea_id_sessione("", $PHPR_TAB_PRE."versioni", 8);
    
    esegui_query("INSERT INTO $PHPR_TAB_PRE"."transazioni (idtransazioni, tipo_transazione, dati_transazione1, ultimo_accesso) VALUES ('$id_transazione', 'rate_limit', '$key', '$current_time')");
}

/**
 * Enhanced password validation
 */
function validate_password_strength($password) {
    $errors = array();
    
    if (strlen($password) < 8) {
        $errors[] = "Password must be at least 8 characters long";
    }
    
    if (!preg_match('/[A-Z]/', $password)) {
        $errors[] = "Password must contain at least one uppercase letter";
    }
    
    if (!preg_match('/[a-z]/', $password)) {
        $errors[] = "Password must contain at least one lowercase letter";
    }
    
    if (!preg_match('/[0-9]/', $password)) {
        $errors[] = "Password must contain at least one number";
    }
    
    return $errors;
}

/**
 * Secure session configuration
 */
function configure_secure_session() {
    // Configure secure session settings
    ini_set('session.cookie_httponly', 1);
    ini_set('session.cookie_secure', isset($_SERVER['HTTPS']));
    ini_set('session.use_only_cookies', 1);
    ini_set('session.cookie_samesite', 'Strict');
    ini_set('session.gc_maxlifetime', 1800); // 30 minutes
}

/**
 * Generate secure random string
 */
function generate_secure_random($length = 32) {
    if (function_exists('random_bytes')) {
        return bin2hex(random_bytes($length / 2));
    } else {
        // Fallback for older PHP versions
        return crea_val_casuale($length);
    }
}

/**
 * Enhanced login validation with security features
 */
function enhanced_login_validation($username, $password, $csrf_token) {
    $errors = array();
    
    // Validate CSRF token
    if (!validate_csrf_token($csrf_token)) {
        $errors[] = "Invalid security token";
    }
    
    // Check rate limiting
    $ip = $_SERVER['REMOTE_ADDR'] ?? '';
    if (!check_rate_limit($ip, $username)) {
        $errors[] = "Too many login attempts. Please try again later.";
    }
    
    // Basic input validation
    if (empty($username) || empty($password)) {
        $errors[] = "Username and password are required";
    }
    
    // Sanitize inputs
    $username = sanitize_input($username);
    $password = sanitize_input($password);
    
    if (!empty($errors)) {
        log_rate_limit_attempt($ip, $username);
    }
    
    return array(
        'valid' => empty($errors),
        'errors' => $errors,
        'username' => $username,
        'password' => $password
    );
}

?>