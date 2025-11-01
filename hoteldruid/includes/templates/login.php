<!DOCTYPE html>
<html lang="<?php echo htmlspecialchars($language ?? 'en'); ?>">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta name="robots" content="noindex, nofollow">
    <title><?php echo htmlspecialchars($title ?? 'HotelDruid Login'); ?></title>
    
    <!-- Modern CSS Framework -->
    <link rel="stylesheet" href="./includes/modern.css">
    
    <!-- Preload critical resources -->
    <link rel="preconnect" href="https://fonts.googleapis.com">
    
    <!-- Security headers -->
    <meta http-equiv="X-Content-Type-Options" content="nosniff">
    <meta http-equiv="X-Frame-Options" content="DENY">
    <meta http-equiv="X-XSS-Protection" content="1; mode=block">
    
    <style>
        /* Additional custom styles if needed */
        .login-logo {
            max-width: 120px;
            height: auto;
            margin-bottom: var(--space-4);
        }
    </style>
</head>
<body>
    <div class="login-container">
        <div class="login-card">
            <div class="login-header">
                <?php if (!empty($logo_url)): ?>
                    <img src="<?php echo htmlspecialchars($logo_url); ?>" alt="Logo" class="login-logo">
                <?php endif; ?>
                
                <h1 class="login-title"><?php echo htmlspecialchars($page_title ?? 'HotelDruid'); ?></h1>
                
                <?php if (!empty($subtitle)): ?>
                    <p class="login-subtitle"><?php echo htmlspecialchars($subtitle); ?></p>
                <?php endif; ?>
            </div>
            
            <div class="login-body">
                <?php if (!empty($errors)): ?>
                    <div class="alert alert-error">
                        <?php if (is_array($errors)): ?>
                            <ul style="margin: 0; padding-left: 1.5rem;">
                                <?php foreach ($errors as $error): ?>
                                    <li><?php echo htmlspecialchars($error); ?></li>
                                <?php endforeach; ?>
                            </ul>
                        <?php else: ?>
                            <?php echo htmlspecialchars($errors); ?>
                        <?php endif; ?>
                    </div>
                <?php endif; ?>
                
                <?php if (!empty($success_message)): ?>
                    <div class="alert alert-success">
                        <?php echo htmlspecialchars($success_message); ?>
                    </div>
                <?php endif; ?>
                
                <form method="post" action="<?php echo htmlspecialchars($form_action ?? 'inizio.php'); ?>" class="login-form" novalidate>
                    <?php echo csrf_token_input(); ?>
                    
                    <?php if (!empty($hidden_fields)): ?>
                        <?php foreach ($hidden_fields as $name => $value): ?>
                            <input type="hidden" name="<?php echo htmlspecialchars($name); ?>" value="<?php echo htmlspecialchars($value); ?>">
                        <?php endforeach; ?>
                    <?php endif; ?>
                    
                    <div class="form-group">
                        <label for="nome_utente_phpr" class="form-label">
                            <?php echo htmlspecialchars($username_label ?? 'Username'); ?>
                            <span class="text-red-500">*</span>
                        </label>
                        <input 
                            type="text" 
                            id="nome_utente_phpr" 
                            name="nome_utente_phpr" 
                            class="form-input" 
                            value="<?php echo htmlspecialchars($username_value ?? ''); ?>"
                            required 
                            autocomplete="username"
                            aria-describedby="username-help"
                        >
                        <div id="username-help" class="sr-only">Enter your username</div>
                    </div>
                    
                    <div class="form-group">
                        <label for="password_phpr" class="form-label">
                            <?php echo htmlspecialchars($password_label ?? 'Password'); ?>
                            <span class="text-red-500">*</span>
                        </label>
                        <input 
                            type="password" 
                            id="password_phpr" 
                            name="password_phpr" 
                            class="form-input" 
                            required 
                            autocomplete="current-password"
                            autocorrect="off" 
                            autocapitalize="off"
                            aria-describedby="password-help"
                        >
                        <div id="password-help" class="sr-only">Enter your password</div>
                    </div>
                    
                    <?php if (!empty($additional_fields)): ?>
                        <?php echo $additional_fields; ?>
                    <?php endif; ?>
                    
                    <button type="submit" class="btn btn-primary w-full btn-lg" id="login-button">
                        <span id="login-text"><?php echo htmlspecialchars($login_button_text ?? 'Login'); ?></span>
                    </button>
                    
                    <?php if (!empty($forgot_password_url)): ?>
                        <div class="text-center mt-4">
                            <a href="<?php echo htmlspecialchars($forgot_password_url); ?>" class="text-primary-600 hover:text-primary-500">
                                <?php echo htmlspecialchars($forgot_password_text ?? 'Forgot your password?'); ?>
                            </a>
                        </div>
                    <?php endif; ?>
                </form>
            </div>
            
            <?php if (!empty($footer_content)): ?>
                <div class="card-footer text-center">
                    <?php echo $footer_content; ?>
                </div>
            <?php endif; ?>
        </div>
    </div>
    
    <script>
        // Enhanced form validation and UX
        document.addEventListener('DOMContentLoaded', function() {
            const form = document.querySelector('.login-form');
            const loginButton = document.getElementById('login-button');
            const loginText = document.getElementById('login-text');
            
            if (form && loginButton) {
                form.addEventListener('submit', function(e) {
                    // Basic client-side validation
                    const username = document.getElementById('nome_utente_phpr').value.trim();
                    const password = document.getElementById('password_phpr').value;
                    
                    if (!username || !password) {
                        e.preventDefault();
                        showError('<?php echo htmlspecialchars($validation_required ?? 'Please fill in all required fields'); ?>');
                        return false;
                    }
                    
                    // Show loading state
                    loginButton.classList.add('loading');
                    loginButton.disabled = true;
                    loginText.textContent = '<?php echo htmlspecialchars($login_loading_text ?? 'Logging in...'); ?>';
                });
            }
            
            // Auto-focus username field
            const usernameField = document.getElementById('nome_utente_phpr');
            if (usernameField && !usernameField.value) {
                usernameField.focus();
            }
        });
        
        function showError(message) {
            // Remove existing alerts
            const existingAlert = document.querySelector('.alert');
            if (existingAlert) {
                existingAlert.remove();
            }
            
            // Create new alert
            const alert = document.createElement('div');
            alert.className = 'alert alert-error';
            alert.textContent = message;
            
            // Insert at the beginning of form
            const form = document.querySelector('.login-form');
            form.insertBefore(alert, form.firstChild);
            
            // Auto-scroll to alert
            alert.scrollIntoView({ behavior: 'smooth', block: 'center' });
        }
        
        // Improve accessibility
        document.addEventListener('keydown', function(e) {
            // Allow Escape to clear form
            if (e.key === 'Escape') {
                const usernameField = document.getElementById('nome_utente_phpr');
                const passwordField = document.getElementById('password_phpr');
                
                if (document.activeElement === usernameField || document.activeElement === passwordField) {
                    usernameField.value = '';
                    passwordField.value = '';
                    usernameField.focus();
                }
            }
        });
    </script>
</body>
</html>