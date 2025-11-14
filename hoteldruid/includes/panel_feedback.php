<?php

/**
 * Panel Feedback System - Common Architecture
 * 
 * This file provides a standardized approach for inline panel feedback messages
 * across HotelDruid system, eliminating intermediate confirmation pages.
 * 
 * USAGE PATTERN:
 * 1. Initialize message arrays at top of page file
 * 2. In processing section: call panel_feedback_add() to add messages
 * 3. Display panel with panel_feedback_display_check() in template
 * 4. No redirects, messages display inline in active panel
 * 
 * @author HotelDruid Development Team
 * @version 1.0
 */

/**
 * Initialize panel feedback system message arrays
 * Call this at the top of any page file that needs panel feedback
 * 
 * @global array $success_messages Success feedback messages
 * @global array $error_messages Error feedback messages  
 * @global array $warning_messages Warning feedback messages
 * @global string $active_panel Currently active panel ID for message display
 */
function panel_feedback_init() {
    global $success_messages, $error_messages, $warning_messages, $active_panel;
    
    if (!isset($success_messages)) $success_messages = array();
    if (!isset($error_messages)) $error_messages = array();
    if (!isset($warning_messages)) $warning_messages = array();
    if (!isset($active_panel)) $active_panel = '';
}

/**
 * Add a success message to display in the specified panel
 * 
 * @param string $message The success message text
 * @param string $panel_id The panel ID where message should display
 */
function panel_feedback_success($message, $panel_id = '') {
    global $success_messages, $active_panel;
    
    if (!isset($success_messages)) $success_messages = array();
    $success_messages[] = $message;
    
    if ($panel_id) {
        $active_panel = $panel_id;
    }
}

/**
 * Add an error message to display in the specified panel
 * 
 * @param string $message The error message text
 * @param string $panel_id The panel ID where message should display
 */
function panel_feedback_error($message, $panel_id = '') {
    global $error_messages, $active_panel;
    
    if (!isset($error_messages)) $error_messages = array();
    $error_messages[] = $message;
    
    if ($panel_id) {
        $active_panel = $panel_id;
    }
}

/**
 * Add a warning message to display in the specified panel
 * 
 * @param string $message The warning message text
 * @param string $panel_id The panel ID where message should display
 */
function panel_feedback_warning($message, $panel_id = '') {
    global $warning_messages, $active_panel;
    
    if (!isset($warning_messages)) $warning_messages = array();
    $warning_messages[] = $message;
    
    if ($panel_id) {
        $active_panel = $panel_id;
    }
}

/**
 * Check if current panel should display messages
 * Use this in panel templates to conditionally show message area
 * 
 * @param string $panel_id The ID of this panel
 * @return bool True if messages should display in this panel
 */
function panel_feedback_display_check($panel_id) {
    global $active_panel;
    return (isset($active_panel) && $active_panel === $panel_id);
}

/**
 * Display all feedback messages using common template
 * Call this within panel templates where messages should appear
 * 
 * @param string $panel_id The ID of this panel
 * @param array $vars Template variables (use get_defined_vars())
 */
function panel_feedback_display($panel_id, $vars = array()) {
    global $active_panel;
    
    if (panel_feedback_display_check($panel_id)) {
        if (class_exists('HotelDruidTemplate')) {
            $template = HotelDruidTemplate::getInstance();
            $template->display('common/messages', $vars);
        }
    }
}

/**
 * Clear all feedback messages
 * Useful when you want to reset state after display
 */
function panel_feedback_clear() {
    global $success_messages, $error_messages, $warning_messages, $active_panel;
    
    $success_messages = array();
    $error_messages = array();
    $warning_messages = array();
    $active_panel = '';
}

/**
 * Get current active panel ID
 * 
 * @return string Current active panel ID
 */
function panel_feedback_get_active_panel() {
    global $active_panel;
    return isset($active_panel) ? $active_panel : '';
}

/**
 * Set active panel ID
 * 
 * @param string $panel_id Panel ID to set as active
 */
function panel_feedback_set_active_panel($panel_id) {
    global $active_panel;
    $active_panel = $panel_id;
}

/**
 * Check if any messages exist
 * 
 * @return bool True if any message arrays contain messages
 */
function panel_feedback_has_messages() {
    global $success_messages, $error_messages, $warning_messages;
    
    return (!empty($success_messages) || !empty($error_messages) || !empty($warning_messages));
}

/**
 * Get all messages as an associative array
 * 
 * @return array Associative array with 'success', 'error', 'warning' keys
 */
function panel_feedback_get_messages() {
    global $success_messages, $error_messages, $warning_messages;
    
    return array(
        'success' => isset($success_messages) ? $success_messages : array(),
        'error' => isset($error_messages) ? $error_messages : array(),
        'warning' => isset($warning_messages) ? $warning_messages : array()
    );
}

?>
