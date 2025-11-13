<?php
/**
 * Generic Message Display Template
 * 
 * Displays success, error, and warning messages at the top of any page.
 * 
 * Usage:
 * 1. Initialize message arrays at the start of your PHP file:
 *    $success_messages = array();
 *    $error_messages = array();
 *    $warning_messages = array();
 * 
 * 2. Add messages during processing:
 *    $success_messages[] = "Operation completed successfully";
 *    $error_messages[] = "An error occurred";
 *    $warning_messages[] = "Please review this warning";
 * 
 * 3. Display messages (after head.php):
 *    HotelDruidTemplate::getInstance()->display('common/messages', get_defined_vars());
 */

if (!empty($success_messages) || !empty($error_messages) || !empty($warning_messages)): ?>
<style>
.message-container {
    max-width: 1200px;
    margin: 20px auto;
    padding: 0 20px;
}

.message-box {
    padding: 15px 20px;
    margin-bottom: 15px;
    border-radius: 8px;
    border-left: 5px solid;
    box-shadow: 0 2px 8px rgba(0,0,0,0.1);
    display: flex;
    align-items: flex-start;
    gap: 12px;
    animation: slideIn 0.3s ease-out;
}

@keyframes slideIn {
    from {
        opacity: 0;
        transform: translateY(-10px);
    }
    to {
        opacity: 1;
        transform: translateY(0);
    }
}

.message-box.success {
    background: linear-gradient(135deg, #d4edda 0%, #c3e6cb 100%);
    border-left-color: #28a745;
    color: #155724;
}

.message-box.error {
    background: linear-gradient(135deg, #f8d7da 0%, #f5c6cb 100%);
    border-left-color: #dc3545;
    color: #721c24;
}

.message-box.warning {
    background: linear-gradient(135deg, #fff3cd 0%, #ffeaa7 100%);
    border-left-color: #ffc107;
    color: #856404;
}

.message-icon {
    font-size: 24px;
    font-weight: bold;
    flex-shrink: 0;
    line-height: 1;
}

.message-content {
    flex: 1;
    line-height: 1.5;
}

.message-content ul {
    margin: 5px 0;
    padding-left: 20px;
}

.message-content li {
    margin: 3px 0;
}
</style>

<div class="message-container">
    <?php if (!empty($success_messages)): ?>
        <?php foreach ($success_messages as $message): ?>
            <div class="message-box success">
                <div class="message-icon">✓</div>
                <div class="message-content"><?php echo $message; ?></div>
            </div>
        <?php endforeach; ?>
    <?php endif; ?>

    <?php if (!empty($error_messages)): ?>
        <?php foreach ($error_messages as $message): ?>
            <div class="message-box error">
                <div class="message-icon">✕</div>
                <div class="message-content"><?php echo $message; ?></div>
            </div>
        <?php endforeach; ?>
    <?php endif; ?>

    <?php if (!empty($warning_messages)): ?>
        <?php foreach ($warning_messages as $message): ?>
            <div class="message-box warning">
                <div class="message-icon">⚠</div>
                <div class="message-content"><?php echo $message; ?></div>
            </div>
        <?php endforeach; ?>
    <?php endif; ?>
</div>
<?php endif; ?>
