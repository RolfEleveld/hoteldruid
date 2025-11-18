# HotelDruid Standalone Executable - Action List

## Quick Summary

Migrate HotelDruid from Apache + MySQL to a standalone executable that runs like Plex Server, using SQLite and PHP's built-in web server.

## Key Advantages

- ✅ SQLite support already exists in the codebase
- ✅ No framework dependencies - pure PHP
- ✅ Simple entry point (`inizio.php`)
- ✅ Portable data directory

## Critical Actions

### 1. Database Migration to SQLite

- [ ] **Verify SQLite works**: Test database creation and all queries
- [ ] **Update defaults**: Ensure `includes/costanti.php` defaults to SQLite
- [ ] **Test all features**: Verify booking, clients, pricing, etc. work with SQLite
- [ ] **Handle SQL differences**: Fix any MySQL-specific SQL syntax
- [ ] **Data migration tool**: Create script to convert existing MySQL data to SQLite (if needed)

### 2. Replace Apache with PHP Built-in Server

- [ ] **Create router script** (`server.php`):
  - Block direct access to `includes/*.php` and `themes/*/php/*.php`
  - Allow CSS/JS/images
  - Route requests to appropriate PHP files
  - Handle 404 errors
- [ ] **Test security**: Verify unauthorized file access is blocked
- [ ] **Test static files**: CSS, JS, images serve correctly
- [ ] **Test PHP execution**: All PHP pages work correctly

### 3. Create Launcher Scripts

- [ ] **Windows launcher** (`start_hoteldruid.bat`):
  - Check for PHP installation
  - Start PHP server on port 8080
  - Auto-open browser to http://localhost:8080
  - Handle graceful shutdown
- [ ] **Linux/Mac launcher** (`start_hoteldruid.sh`):
  - Same functionality as Windows version
  - Make executable (`chmod +x`)
- [ ] **Configuration**: Allow port/host customization

### 4. Package as Standalone Executable

- [ ] **Choose packaging method**:
  - Option A: PHP Desktop (Windows) - embeds PHP in executable
  - Option B: Go launcher (cross-platform) - small binary that runs PHP
  - Option C: Electron + PHP (cross-platform) - desktop app feel
  - Option D: Installer with bundled PHP (NSIS/Inno Setup for Windows)
- [ ] **Bundle PHP runtime**: Include PHP 8.1+ with required extensions:
  - sqlite3 (critical)
  - session
  - json
  - mbstring
- [ ] **Test executable**: Verify it runs without PHP pre-installed
- [ ] **Cross-platform**: Create Windows (.exe), Linux (AppImage/binary), Mac (.app)

### 5. Enhanced Features (Optional)

- [ ] **System tray integration**: Minimize to tray, right-click menu
- [ ] **Configuration UI**: Settings window for port, host, data directory
- [ ] **Auto-start browser**: Detect and open default browser
- [ ] **Logging**: Server logs, error logs, access logs
- [ ] **Auto-backup**: Schedule SQLite database backups

### 6. Production Enhancements (Optional)

- [ ] **Better web server**: Replace PHP built-in server with:
  - RoadRunner (PHP application server)
  - Nginx + PHP-FPM (lightweight bundle)
  - Swoole (async PHP server)
- [ ] **Performance optimization**: Caching, connection pooling
- [ ] **HTTPS support**: Add SSL/TLS for secure connections

## Implementation Priority

### Phase 1: Core Functionality (Week 1-2)

1. Create `server.php` router
2. Test SQLite with all features
3. Create launcher scripts
4. Test end-to-end

### Phase 2: Executable Packaging (Week 3-4)

1. Choose packaging method
2. Bundle PHP runtime
3. Create executable
4. Test on clean systems

### Phase 3: Polish (Week 5+)

1. System tray integration
2. Configuration UI
3. Documentation
4. User testing

## Files to Create/Modify

### New Files

- `server.php` - Router for PHP built-in server
- `start_hoteldruid.bat` - Windows launcher
- `start_hoteldruid.sh` - Linux/Mac launcher
- `config.ini` - Configuration file (optional)
- `MIGRATION_PLAN.md` - Detailed plan (this file)

### Files to Modify

- `includes/costanti.php` - Ensure SQLite is default
- `creadb.php` - Prefer SQLite in database creation

### Files to Test

- All PHP files - Verify they work with SQLite
- Database queries - Check for MySQL-specific syntax
- File paths - Ensure relative paths work

## Testing Checklist

### Database

- [ ] SQLite database creates successfully
- [ ] All tables create correctly
- [ ] Data inserts/updates/deletes work
- [ ] Queries return correct results
- [ ] Transactions work
- [ ] Concurrent access handled (if applicable)

### Web Server

- [ ] Router blocks unauthorized access
- [ ] Static files (CSS, JS, images) serve correctly
- [ ] PHP files execute correctly
- [ ] URLs route correctly
- [ ] 404 errors handled properly
- [ ] Security rules enforced

### Launcher

- [ ] Detects PHP installation
- [ ] Starts server on correct port
- [ ] Opens browser automatically
- [ ] Handles port conflicts
- [ ] Graceful shutdown works
- [ ] Multiple instances don't conflict

### Executable

- [ ] Runs without PHP pre-installed
- [ ] Bundled PHP works correctly
- [ ] All extensions available
- [ ] Data directory accessible
- [ ] Portable (works from USB drive)
- [ ] Cross-platform compatibility

## Known Challenges

1. **SQL Syntax Differences**
   - MySQL: `LIKE` with `%` wildcards
   - SQLite: `GLOB` with `*` wildcards (already handled in `funzioni_sqlite.php`)
   - Date functions may differ
   - String functions may differ

2. **File Paths**
   - Windows vs Linux path separators
   - Relative vs absolute paths
   - Portable installation paths

3. **PHP Extensions**
   - Ensure all required extensions are bundled
   - SQLite3 is critical
   - Session support needed

4. **Performance**
   - PHP built-in server is single-threaded
   - SQLite has write locks
   - May need upgrade path for production

## Success Criteria

✅ Single executable file (or installer)  
✅ No MySQL/Apache installation required  
✅ Runs on Windows, Linux, Mac  
✅ Opens browser automatically  
✅ All HotelDruid features work  
✅ Data persists in SQLite database  
✅ Portable (works from USB drive)  
✅ Easy to use (double-click to start)  

## Next Immediate Steps

1. **Create `server.php` router** - Start with this, test with PHP built-in server
2. **Test SQLite** - Create a test database, verify all features work
3. **Create launcher script** - Make it easy to start the server
4. **Test end-to-end** - Verify everything works together
5. **Package executable** - Once core functionality works

## Resources

- PHP Built-in Server: https://www.php.net/manual/en/features.commandline.webserver.php
- SQLite PHP: https://www.php.net/manual/en/book.sqlite3.php
- PHP Desktop: https://github.com/cztomczak/phpdesktop
- RoadRunner: https://roadrunner.dev/
