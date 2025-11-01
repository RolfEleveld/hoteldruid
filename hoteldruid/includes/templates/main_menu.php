<!DOCTYPE html>
<html lang="<?php echo htmlspecialchars($language ?? 'en'); ?>">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title><?php echo htmlspecialchars($title ?? 'HotelDruid'); ?></title>
    
    <!-- Modern CSS Framework -->
    <link rel="stylesheet" href="./base.css">
    <link rel="stylesheet" href="./includes/modern.css">
    
    <!-- Theme CSS if available -->
    <?php if (!empty($theme_css)): ?>
        <link rel="stylesheet" href="<?php echo htmlspecialchars($theme_css); ?>">
    <?php endif; ?>
    
    <!-- Custom CSS -->
    <?php if (!empty($custom_css)): ?>
        <link rel="stylesheet" href="<?php echo htmlspecialchars($custom_css); ?>">
    <?php endif; ?>
    
    <meta name="robots" content="noindex, nofollow">
    <meta http-equiv="X-Content-Type-Options" content="nosniff">
    <meta http-equiv="X-Frame-Options" content="SAMEORIGIN">
</head>
<body>
    <!-- Navigation Header -->
    <nav class="nav-header">
        <div class="nav-container">
            <a href="<?php echo htmlspecialchars($home_url ?? './inizio.php'); ?>" class="nav-logo">
                <?php if (!empty($logo_url)): ?>
                    <img src="<?php echo htmlspecialchars($logo_url); ?>" alt="Logo" style="height: 32px; margin-right: 0.5rem;">
                <?php endif; ?>
                <?php echo htmlspecialchars($site_name ?? 'HotelDruid'); ?>
            </a>
            
            <!-- Desktop Navigation -->
            <div class="nav-menu" id="nav-menu">
                <?php if (!empty($nav_items)): ?>
                    <?php foreach ($nav_items as $item): ?>
                        <a href="<?php echo htmlspecialchars($item['url']); ?>" 
                           class="nav-menu-item<?php echo (!empty($item['active']) ? ' active' : ''); ?>">
                            <?php echo htmlspecialchars($item['title']); ?>
                        </a>
                    <?php endforeach; ?>
                <?php endif; ?>
                
                <?php if (!empty($user_name)): ?>
                    <div class="nav-menu-item">
                        <?php echo htmlspecialchars($user_label ?? 'User'); ?>: <?php echo htmlspecialchars($user_name); ?>
                    </div>
                    <a href="<?php echo htmlspecialchars($logout_url ?? './inizio.php?logout=SI'); ?>" class="nav-menu-item">
                        <?php echo htmlspecialchars($logout_label ?? 'Logout'); ?>
                    </a>
                <?php endif; ?>
            </div>
            
            <!-- Mobile Menu Toggle -->
            <div class="nav-toggle" id="nav-toggle">
                <span></span>
                <span></span>
                <span></span>
            </div>
        </div>
    </nav>
    
    <!-- Main Content -->
    <main class="container">
        <?php if (!empty($page_header)): ?>
            <header style="text-align: center; margin: 2rem 0;">
                <h1 style="font-size: 2.5rem; font-weight: 700; color: var(--gray-900); margin-bottom: 0.5rem;">
                    <?php echo htmlspecialchars($page_header); ?>
                </h1>
                <?php if (!empty($page_subtitle)): ?>
                    <p style="color: var(--gray-600); font-size: 1.1rem;">
                        <?php echo htmlspecialchars($page_subtitle); ?>
                    </p>
                <?php endif; ?>
            </header>
        <?php endif; ?>
        
        <?php if (!empty($alerts)): ?>
            <?php foreach ($alerts as $alert): ?>
                <div class="alert alert-<?php echo htmlspecialchars($alert['type']); ?>">
                    <?php echo htmlspecialchars($alert['message']); ?>
                </div>
            <?php endforeach; ?>
        <?php endif; ?>
        
        <?php if (!empty($form_aggiorna_sub)): ?>
            <?php echo $form_aggiorna_sub; ?>
        <?php endif; ?>
        
        <!-- Main Menu Grid -->
        <?php if (!empty($menu_sections)): ?>
            <?php foreach ($menu_sections as $section): ?>
                <?php if (!empty($section['title'])): ?>
                    <h2 style="font-size: 1.5rem; font-weight: 600; margin: 2rem 0 1rem; color: var(--gray-800);">
                        <?php echo htmlspecialchars($section['title']); ?>
                    </h2>
                <?php endif; ?>
                
                <div class="menu-grid">
                    <?php foreach ($section['items'] as $item): ?>
                        <?php if (!empty($item['url'])): ?>
                            <a href="<?php echo htmlspecialchars($item['url']); ?>" class="menu-item">
                        <?php else: ?>
                            <div class="menu-item" style="cursor: default; opacity: 0.6;">
                        <?php endif; ?>
                        
                            <?php if (!empty($item['icon'])): ?>
                                <div class="menu-item-icon">
                                    <?php echo $item['icon']; ?>
                                </div>
                            <?php endif; ?>
                            
                            <h3 class="menu-item-title">
                                <?php echo htmlspecialchars($item['title']); ?>
                            </h3>
                            
                            <?php if (!empty($item['description'])): ?>
                                <p class="menu-item-description">
                                    <?php echo htmlspecialchars($item['description']); ?>
                                </p>
                            <?php endif; ?>
                            
                        <?php if (!empty($item['url'])): ?>
                            </a>
                        <?php else: ?>
                            </div>
                        <?php endif; ?>
                    <?php endforeach; ?>
                </div>
            <?php endforeach; ?>
        <?php endif; ?>
        
        <!-- Year Selection -->
        <?php if (!empty($show_year_selector)): ?>
            <div class="card" style="margin-top: 2rem;">
                <div class="card-body text-center">
                    <h3 style="margin-bottom: 1rem;"><?php echo htmlspecialchars($year_selector_title ?? 'Select Year'); ?></h3>
                    <form method="post" action="inizio.php" style="display: inline-flex; align-items: center; gap: 1rem;">
                        <input type="hidden" name="id_sessione" value="<?php echo htmlspecialchars($id_sessione ?? ''); ?>">
                        <?php echo csrf_token_input(); ?>
                        
                        <label for="anno" class="form-label" style="margin: 0;">
                            <?php echo htmlspecialchars($year_label ?? 'Year'); ?>:
                        </label>
                        <input type="text" 
                               id="anno"
                               name="anno" 
                               size="4" 
                               maxlength="4" 
                               value="<?php echo htmlspecialchars($default_year ?? ''); ?>"
                               class="form-input"
                               style="width: 80px;">
                        <button type="submit" class="btn btn-primary">
                            <?php echo htmlspecialchars($go_label ?? 'Go'); ?>
                        </button>
                    </form>
                    
                    <?php if (!empty($year_note)): ?>
                        <p style="margin-top: 1rem; color: var(--gray-600); font-size: 0.875rem;">
                            <?php echo htmlspecialchars($year_note); ?>
                        </p>
                    <?php endif; ?>
                </div>
            </div>
        <?php endif; ?>
    </main>
    
    <!-- Footer -->
    <?php if (!empty($show_footer)): ?>
        <footer style="margin-top: 4rem; padding: 2rem 0; text-align: center; background-color: var(--gray-100); color: var(--gray-600);">
            <div class="container">
                <?php if (!empty($footer_content)): ?>
                    <?php echo $footer_content; ?>
                <?php else: ?>
                    <p style="margin: 0; font-size: 0.875rem;">
                        Website <a href="./mostra_sorgente.php" style="color: var(--gray-600);">engine code</a> is copyright © by DigitalDruid.Net.<br>
                        <a href="http://www.hoteldruid.com" style="color: var(--gray-600);">HotelDruid</a> is free software released under the GNU/AGPL.
                    </p>
                <?php endif; ?>
            </div>
        </footer>
    <?php endif; ?>
    
    <!-- JavaScript -->
    <script>
        // Mobile navigation toggle
        document.addEventListener('DOMContentLoaded', function() {
            const navToggle = document.getElementById('nav-toggle');
            const navMenu = document.getElementById('nav-menu');
            
            if (navToggle && navMenu) {
                navToggle.addEventListener('click', function() {
                    navToggle.classList.toggle('active');
                    navMenu.classList.toggle('active');
                });
                
                // Close menu when clicking on a link
                const navLinks = navMenu.querySelectorAll('.nav-menu-item');
                navLinks.forEach(link => {
                    link.addEventListener('click', function() {
                        navToggle.classList.remove('active');
                        navMenu.classList.remove('active');
                    });
                });
                
                // Close menu when clicking outside
                document.addEventListener('click', function(e) {
                    if (!navToggle.contains(e.target) && !navMenu.contains(e.target)) {
                        navToggle.classList.remove('active');
                        navMenu.classList.remove('active');
                    }
                });
            }
            
            // Enhance form interactions
            const forms = document.querySelectorAll('form');
            forms.forEach(form => {
                form.addEventListener('submit', function(e) {
                    const submitButton = form.querySelector('button[type="submit"]');
                    if (submitButton) {
                        submitButton.classList.add('loading');
                        submitButton.disabled = true;
                        
                        // Re-enable after 5 seconds as fallback
                        setTimeout(() => {
                            submitButton.classList.remove('loading');
                            submitButton.disabled = false;
                        }, 5000);
                    }
                });
            });
            
            // Add keyboard navigation for menu items
            const menuItems = document.querySelectorAll('.menu-item');
            menuItems.forEach((item, index) => {
                item.addEventListener('keydown', function(e) {
                    let targetIndex;
                    
                    switch(e.key) {
                        case 'ArrowRight':
                        case 'ArrowDown':
                            e.preventDefault();
                            targetIndex = (index + 1) % menuItems.length;
                            menuItems[targetIndex].focus();
                            break;
                        case 'ArrowLeft':
                        case 'ArrowUp':
                            e.preventDefault();
                            targetIndex = (index - 1 + menuItems.length) % menuItems.length;
                            menuItems[targetIndex].focus();
                            break;
                        case 'Home':
                            e.preventDefault();
                            menuItems[0].focus();
                            break;
                        case 'End':
                            e.preventDefault();
                            menuItems[menuItems.length - 1].focus();
                            break;
                    }
                });
                
                // Make menu items focusable
                if (item.tagName.toLowerCase() === 'a') {
                    item.setAttribute('tabindex', '0');
                }
            });
        });
        
        // Add smooth scroll behavior
        document.documentElement.style.scrollBehavior = 'smooth';
        
        // Enhance accessibility
        window.addEventListener('keydown', function(e) {
            // Skip to main content with Ctrl+/
            if ((e.ctrlKey || e.metaKey) && e.key === '/') {
                e.preventDefault();
                const main = document.querySelector('main');
                if (main) {
                    main.focus();
                    main.scrollIntoView();
                }
            }
        });
    </script>
    
    <?php if (!empty($additional_scripts)): ?>
        <?php echo $additional_scripts; ?>
    <?php endif; ?>
</body>
</html>