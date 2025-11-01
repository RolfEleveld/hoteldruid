<?php
/**
 * Template system for HotelDruid
 * Separates presentation from logic
 */

class HotelDruidTemplate {
    private static $instance = null;
    private $templateDir;
    private $variables = array();
    
    private function __construct($templateDir = './includes/templates/') {
        $this->templateDir = $templateDir;
    }
    
    public static function getInstance($templateDir = './includes/templates/') {
        if (self::$instance === null) {
            self::$instance = new self($templateDir);
        }
        return self::$instance;
    }
    
    public function __clone() {
        // Prevent cloning of singleton
        throw new Exception('Cannot clone singleton instance');
    }
    
    public function __wakeup() {
        // Prevent unserialization of singleton
        throw new Exception('Cannot unserialize singleton instance');
    }
    
    public function assign($key, $value) {
        $this->variables[$key] = $value;
    }
    
    public function render($template, $variables = array()) {
        // Merge additional variables
        $allVariables = array_merge($this->variables, $variables);
        
        // Extract variables into current scope
        extract($allVariables, EXTR_SKIP);
        
        // Start output buffering
        ob_start();
        
        // Include template file
        $templateFile = $this->templateDir . $template . '.php';
        if (file_exists($templateFile)) {
            include $templateFile;
        } else {
            throw new Exception("Template file not found: $templateFile");
        }
        
        // Return rendered content
        return ob_get_clean();
    }
    
    public function display($template, $variables = array()) {
        echo $this->render($template, $variables);
    }
    
    /**
     * Alias for render method to maintain compatibility
     */
    public function renderTemplate($template, $variables = array()) {
        return $this->render($template, $variables);
    }
    
    /**
     * Generate CSRF token input field
     */
    public function getCsrfTokenInput() {
        // Check if CSRF function exists, otherwise return empty
        if (function_exists('csrf_token_input')) {
            return csrf_token_input();
        }
        // Fallback: generate basic hidden field
        if (session_status() == PHP_SESSION_NONE) {
            session_start();
        }
        if (!isset($_SESSION['csrf_token'])) {
            $_SESSION['csrf_token'] = bin2hex(random_bytes(32));
        }
        return '<input type="hidden" name="csrf_token" value="' . htmlspecialchars($_SESSION['csrf_token']) . '">';
    }
}

/**
 * Generate secure CSRF token input
 */
function csrf_token_input() {
    $token = generate_csrf_token();
    return '<input type="hidden" name="csrf_token" value="' . htmlspecialchars($token) . '">';
}

/**
 * Generate alert HTML
 */
function render_alert($message, $type = 'error') {
    $alertClass = 'alert alert-' . $type;
    return '<div class="' . $alertClass . '">' . htmlspecialchars($message) . '</div>';
}

/**
 * Render form field with label and validation
 */
function render_form_field($name, $label, $type = 'text', $value = '', $required = false, $errors = array()) {
    $hasError = isset($errors[$name]);
    $errorClass = $hasError ? ' border-red-500' : '';
    $requiredAttr = $required ? ' required' : '';
    $ariaInvalid = $hasError ? ' aria-invalid="true"' : '';
    
    $html = '<div class="form-group">';
    $html .= '<label for="' . $name . '" class="form-label">' . htmlspecialchars($label);
    if ($required) {
        $html .= ' <span class="text-red-500">*</span>';
    }
    $html .= '</label>';
    
    $html .= '<input type="' . $type . '" id="' . $name . '" name="' . $name . '" 
              class="form-input' . $errorClass . '" 
              value="' . htmlspecialchars($value) . '"' . $requiredAttr . $ariaInvalid;
    
    if ($type === 'password') {
        $html .= ' autocorrect="off" autocapitalize="off"';
    }
    
    $html .= '>';
    
    if ($hasError) {
        $html .= '<div class="text-red-600 text-sm mt-1">' . htmlspecialchars($errors[$name]) . '</div>';
    }
    
    $html .= '</div>';
    
    return $html;
}

/**
 * Render navigation menu items
 */
function render_nav_menu($menuItems, $currentPage = '') {
    $html = '';
    foreach ($menuItems as $item) {
        $activeClass = ($item['page'] === $currentPage) ? ' active' : '';
        $html .= '<a href="' . htmlspecialchars($item['url']) . '" class="nav-menu-item' . $activeClass . '">';
        $html .= htmlspecialchars($item['title']);
        $html .= '</a>';
    }
    return $html;
}

/**
 * Render main menu grid items
 */
function render_menu_grid($menuItems) {
    $html = '<div class="menu-grid">';
    
    foreach ($menuItems as $item) {
        $html .= '<a href="' . htmlspecialchars($item['url']) . '" class="menu-item">';
        
        if (!empty($item['icon'])) {
            $html .= '<div class="menu-item-icon">';
            $html .= $item['icon']; // This can be an SVG or icon font class
            $html .= '</div>';
        }
        
        $html .= '<h3 class="menu-item-title">' . htmlspecialchars($item['title']) . '</h3>';
        
        if (!empty($item['description'])) {
            $html .= '<p class="menu-item-description">' . htmlspecialchars($item['description']) . '</p>';
        }
        
        $html .= '</a>';
    }
    
    $html .= '</div>';
    return $html;
}

?>