<?php
/**
 * Message Display Component for crearegole.php
 * Shows success, error, and warning messages at the top of the page
 */
?>

<?php if (!empty($success_messages) or !empty($error_messages) or !empty($warning_messages)): ?>
<style>
.message-container {
    margin: 20px auto;
    max-width: 1200px;
    padding: 0 20px;
}

.message-box {
    padding: 15px 20px;
    margin-bottom: 15px;
    border-radius: 6px;
    box-shadow: 0 2px 4px rgba(0,0,0,0.1);
    display: flex;
    align-items: center;
    gap: 12px;
    font-size: 14px;
    line-height: 1.5;
}

.message-box.success {
    background: linear-gradient(135deg, #4CAF50 0%, #388E3C 100%);
    color: white;
    border-left: 4px solid #2E7D32;
}

.message-box.error {
    background: linear-gradient(135deg, #f44336 0%, #d32f2f 100%);
    color: white;
    border-left: 4px solid #c62828;
}

.message-box.warning {
    background: linear-gradient(135deg, #FF9800 0%, #F57C00 100%);
    color: white;
    border-left: 4px solid #E65100;
}

.message-icon {
    font-size: 20px;
    font-weight: bold;
    flex-shrink: 0;
}

.message-text {
    flex: 1;
}
</style>

<div class="message-container">
    <?php if (!empty($success_messages)): ?>
        <?php foreach ($success_messages as $message): ?>
            <div class="message-box success">
                <span class="message-icon">✓</span>
                <div class="message-text"><?php echo $message; ?></div>
            </div>
        <?php endforeach; ?>
    <?php endif; ?>
    
    <?php if (!empty($error_messages)): ?>
        <?php foreach ($error_messages as $message): ?>
            <div class="message-box error">
                <span class="message-icon">✕</span>
                <div class="message-text"><?php echo $message; ?></div>
            </div>
        <?php endforeach; ?>
    <?php endif; ?>
    
    <?php if (!empty($warning_messages)): ?>
        <?php foreach ($warning_messages as $message): ?>
            <div class="message-box warning">
                <span class="message-icon">⚠</span>
                <div class="message-text"><?php echo $message; ?></div>
            </div>
        <?php endforeach; ?>
    <?php endif; ?>
</div>
<?php endif; ?>
