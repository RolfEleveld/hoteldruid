# WARP.md

This file provides guidance to WARP (warp.dev) when working with code in this repository.

## Project Overview

HotelDruid is a hotel management system (property management system) written in PHP. This repository provides multiple deployment methods:
- Docker containerization with LAMP stack (Apache/PHP/MySQL)
- PHP Desktop standalone Windows application (with SQLite)
- Traditional LAMP server deployment

The codebase is a mature PHP application (version 3.0.7) released under AGPLv3, with ongoing modernization efforts to improve security, UI/UX, and code architecture.

## Development Commands

### Docker Development (Primary Method on Windows)

```powershell
# Start all containers (web server, MySQL, phpMyAdmin)
docker-compose up -d

# Stop all containers
docker-compose down

# View logs
docker-compose logs -f hoteldruid-web
docker-compose logs -f hoteldruid-db

# Rebuild after code changes
docker-compose build --no-cache
docker-compose up -d

# Access web container shell
docker-compose exec hoteldruid-web bash

# Check PHP modules
docker-compose exec hoteldruid-web php -m

# Restart specific service
docker-compose restart hoteldruid-web
```

### Database Operations

```powershell
# Access MySQL command line
docker-compose exec hoteldruid-db mysql -u hoteldruid_user -p hoteldruid

# Create database backup
docker-compose exec hoteldruid-db mysqldump -u root -p hoteldruid > backup.sql

# Restore database backup
docker-compose exec -T hoteldruid-db mysql -u root -p hoteldruid < backup.sql

# Reset database (WARNING: deletes all data)
docker-compose down -v
docker-compose up -d
```

### PHP Desktop (Standalone Windows Application)

```powershell
# One-time setup (downloads ~100MB)
.\setup-phpdesktop.ps1

# Start application
.\start-hoteldruid-desktop.ps1

# Package for distribution
.\package_release.ps1
```

### Access Points

- **HotelDruid Application**: http://localhost:8080
- **phpMyAdmin**: http://localhost:8081
- **MySQL**: localhost:3306

## Code Architecture

### Entry Point and Authentication Flow

- **`inizio.php`**: Main entry point, handles login/logout and session management
- Authentication flow:
  1. Check if database connection file exists (`dati/dati_connessione.php`)
  2. If not, show language selection and redirect to `creadb.php` for database setup
  3. If exists, proceed with login validation using enhanced security features
  4. After successful login, display main menu/dashboard

### Database Abstraction Layer

HotelDruid supports multiple database backends through a database abstraction system:

- **Configuration**: `dati/dati_connessione.php` sets `$PHPR_DB_TYPE` (mysql/mysqli/postgresql/sqlite)
- **Database-specific functions**: 
  - `includes/funzioni_mysql.php` / `funzioni_mysql_extra.php`
  - `includes/funzioni_mysqli.php` / `funzioni_mysqli_extra.php`
  - `includes/funzioni_postgresql.php` / `funzioni_postgresql_extra.php`
  - `includes/funzioni_sqlite.php` / `funzioni_sqlite_extra.php`
- **Core functions**: Defined in `includes/funzioni.php`
- **Key database functions**:
  - `esegui_query()` - Execute SQL queries (database-specific implementation)
  - `controlla_login()` - Authentication and session management
  - `risul_query()` / `numlin_query()` - Result handling
  - `lock_tabelle()` / `unlock_tabelle()` - Table locking for concurrent access

### Modernization Architecture

The codebase is undergoing modernization (see `MODERNIZATION_SUMMARY.md`). New patterns being introduced:

1. **Security Layer** (`includes/security.php`):
   - CSRF token generation and validation
   - Rate limiting for login attempts
   - Enhanced input validation and sanitization
   - Secure session configuration

2. **Template System** (`includes/template.php`):
   - Singleton pattern: `HotelDruidTemplate::getInstance()`
   - Separates PHP logic from HTML presentation
   - Templates stored in `includes/templates/{module}/{template}.php`
   - Example: `includes/templates/common/messages.php` for success/error/warning messages

3. **Inline Message System** (`INLINE_MESSAGE_SYSTEM.md`):
   - User feedback without page redirects
   - Three message arrays: `$success_messages`, `$error_messages`, `$warning_messages`
   - Display with: `HotelDruidTemplate::getInstance()->display('common/messages', get_defined_vars())`

4. **Menu Generation** (`includes/menu_generator.php`):
   - Privilege-based menu generation
   - Modern responsive design with hamburger navigation
   - CSS framework: `includes/modern.css`

### Directory Structure

```
hoteldruid/
├── inizio.php              # Main entry point / login page
├── costanti.php            # Application constants
├── creadb.php              # Database creation wizard
├── dati/                   # Data directory (writable, contains config and SQLite DB)
│   ├── dati_connessione.php   # Database connection settings
│   ├── db_hoteldruid          # SQLite database file (if using SQLite)
│   ├── lingua.php             # Language preference
│   └── tema.php               # Theme settings
├── includes/
│   ├── funzioni.php           # Core utility functions
│   ├── funzioni_{db}.php      # Database-specific implementations
│   ├── security.php           # Modern security enhancements (CSRF, rate limiting)
│   ├── template.php           # Template engine for separation of concerns
│   ├── menu_generator.php     # Menu generation logic
│   ├── modern.css             # Modern responsive CSS framework
│   ├── head.php / foot.php    # Page header/footer
│   ├── templates/             # Template files organized by module
│   │   ├── common/            # Shared templates (messages, etc.)
│   │   ├── clienti/           # Client management templates
│   │   ├── crearegole/        # Rules creation templates
│   │   └── ...
│   └── lang/{language}/       # Localization files (Italian default, English, etc.)
├── {module}.php            # Feature modules (clienti.php, costi.php, etc.)
└── themes/                 # UI themes
```

### Multi-Database Support Pattern

When working with database code:
- Always use `esegui_query()` instead of direct database calls
- Connection info stored in global variables: `$PHPR_DB_TYPE`, `$PHPR_DB_HOST`, `$PHPR_DB_NAME`, etc.
- Database type determined at runtime from `dati/dati_connessione.php`
- Current setup uses SQLite by default (check `$PHPR_DB_TYPE` in `dati/dati_connessione.php`)

### Global Variables and Conventions

- `$PHPR_TAB_PRE`: Table prefix (usually empty string)
- `$id_utente`: Currently logged-in user ID
- `$id_sessione`: Session identifier
- `$anno`: Current working year
- `$lingua_mex`: User language (default "ita")
- `$pag`: Current page name (e.g., "inizio.php")
- `$numconnessione`: Database connection resource

### Translation System

- Function: `mex($message, $page)` - Returns translated message
- Translation files: `includes/lang/{language}/{page}`
- Default language: Italian (`ita`)
- Language preference stored in `dati/lingua.php`

## Key Development Patterns

### When Modifying Pages with Forms

Follow the inline message system pattern:

1. **Initialize message arrays** at the top of the file:
   ```php
   $success_messages = array();
   $error_messages = array();
   $warning_messages = array();
   ```

2. **Process form submissions** and add messages to arrays:
   ```php
   if (!empty($inserisci)) {
       if (empty($required_field)) {
           $error_messages[] = mex("Campo obbligatorio mancante", $pag);
       } else {
           // Process the form
           esegui_query("INSERT INTO ...");
           $success_messages[] = mex("Dati salvati con successo", $pag);
       }
       $show_main_form = true;  // Stay on same page
   }
   ```

3. **Display messages** after including head.php:
   ```php
   include("./includes/head.php");
   require_once("./includes/template.php");
   if (!empty($success_messages) or !empty($error_messages) or !empty($warning_messages)) {
       HotelDruidTemplate::getInstance()->display('common/messages', get_defined_vars());
   }
   ```

### CSRF Protection

When creating forms that modify data:

1. **Include security.php** at the top
2. **Add CSRF token to form**:
   ```php
   $csrf_token = generate_csrf_token();
   echo "<input type='hidden' name='csrf_token' value='$csrf_token'>";
   ```
3. **Validate on submission**:
   ```php
   if (!validate_csrf_token($_POST['csrf_token'] ?? '')) {
       $error_messages[] = "Invalid security token";
   }
   ```

### Rate Limiting

For authentication or sensitive operations:
```php
if (!check_rate_limit($identifier)) {
    $error_messages[] = "Too many attempts. Please try again later.";
}
```

## Configuration Files

### Docker Environment (.env)

Copy `.env.example` to `.env` and modify:
- `DB_PASSWORD` / `DB_ROOT_PASSWORD` - Change in production
- `PHP_MEMORY_LIMIT`, `PHP_UPLOAD_MAX_FILESIZE` - Adjust for needs
- `SESSION_SECRET`, `CSRF_SECRET` - Must change in production

### Database Connection (hoteldruid/dati/dati_connessione.php)

```php
$PHPR_DB_TYPE = "sqlite";  // or "mysql", "mysqli", "postgresql"
$PHPR_DB_NAME = "hoteldruid";
$PHPR_DB_HOST = "localhost";  // or "hoteldruid-db" for Docker
$PHPR_DB_PORT = "3306";
$PHPR_DB_USER = "root";
$PHPR_DB_PASS = "password";
```

## Testing Approach

No automated test suite exists. Manual testing recommended:

1. **Test with different database backends** (SQLite, MySQL, PostgreSQL)
2. **Test form submissions** - verify inline messages appear correctly
3. **Test CSRF protection** - verify forms reject invalid tokens
4. **Test rate limiting** - verify excessive login attempts are blocked
5. **Test responsive design** - verify on mobile devices (hamburger menu, touch targets)

## Important Notes

### Legacy Code Coexistence

- The codebase mixes old and new patterns - modernization is in progress
- Old pages may use direct `echo` for messages instead of message arrays
- Not all pages use the template system yet
- When modifying a page, consider modernizing it (see `INLINE_MESSAGE_SYSTEM.md`)

### Session Management

- Sessions are configured with secure settings in `inizio.php`
- Session lifetime: 30 minutes (1800 seconds)
- HttpOnly, Secure (if HTTPS), SameSite=Strict cookies

### File Permissions

- `dati/` directory must be writable (777 in Docker, appropriate permissions otherwise)
- Contains database file (SQLite), configuration, and cached data

### Windows Development

This project is developed on Windows with PowerShell. When writing shell scripts:
- Use PowerShell syntax (`.ps1` files)
- Provide bash alternatives (`.sh` files) for cross-platform compatibility
- Use `Get-ChildItem` instead of `ls`, `docker-compose` works the same

### Multi-Language Support

- All user-facing strings should use `mex($message, $pag)`
- Default language is Italian
- English and other languages available in `includes/lang/`
- Page-specific translations in separate files

## Common Development Tasks

### Adding a New Page

1. Create PHP file in root (e.g., `newfeature.php`)
2. Include standard headers:
   ```php
   $pag = "newfeature.php";
   $titolo = "New Feature";
   include("./costanti.php");
   include_once("./includes/funzioni.php");
   ```
3. Initialize message arrays if form handling needed
4. Check authentication: `$id_utente = controlla_login(...)`
5. Include head, display messages, implement feature, include foot

### Adding a New Template

1. Create template file: `includes/templates/{module}/{name}.php`
2. Extract variables using `extract($variables)`
3. Use pure HTML/CSS with minimal PHP
4. Display with: `HotelDruidTemplate::getInstance()->display('{module}/{name}', $data)`

### Modifying CSS

- **Legacy styles**: `base.css` in root
- **Modern styles**: `includes/modern.css`
- **Theme-specific**: `themes/{theme}/css/`
- Prefer using `modern.css` classes for new features

### Database Schema Changes

1. Modify appropriate `mysql/init/` SQL scripts if using Docker
2. Consider migration for existing installations
3. Test with all supported database types (MySQL, SQLite, PostgreSQL)
4. Update database initialization in `creadb.php`

## References

- Main README: `README.md` - Comprehensive Docker setup guide
- PHP Desktop: `README-PHPDESKTOP.md` and `QUICKSTART-PHPDESKTOP.md`
- Modernization details: `MODERNIZATION_SUMMARY.md`
- Message system: `INLINE_MESSAGE_SYSTEM.md`
- License: AGPLv3 - see `hoteldruid/COPYING`
