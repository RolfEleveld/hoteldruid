# Configuring External Data Directory

HotelDruid allows you to store all persistent data (database, configuration, uploads) in a location outside the application directory. This is useful for:

- **Easy Updates**: Update the application without overwriting your data
- **Backup Management**: Keep data in a known, separate location
- **Portable Installations**: Move the application while keeping data in place
- **Multiple Installations**: Run different versions with different data directories

## Quick Setup

### For PHP Desktop (Standalone Executable)

1. **Edit the configuration file**: Open `phpdesktop-settings.json` in the `hoteldruid` directory

2. **Set your data path**: Change the `data_path` value in the `hoteldruid` section:

   ```json
   "hoteldruid": {
     "data_path": "C:/Users/rolfe/Documents/HotelDruid"
   }
   ```

### For Web Server Deployment

1. **Edit the configuration file**: Open `hoteldruid-config.php` in the root directory of HotelDruid

2. **Set your data path**: Change the `C_DATI_PATH_EXTERNAL` value to your desired location:

   ```php
   define('C_DATI_PATH_EXTERNAL', "C:/Users/rolfe/Documents/HotelDruid");
   ```

3. **Path Format**:
   - **Windows**: Use forward slashes (`/`) or double backslashes (`\\`)
     - `C:/Users/rolfe/Documents/HotelDruid`
     - `C:\\Users\\rolfe\\Documents\\HotelDruid`
   - **Linux/Mac**: Use forward slashes
     - `/home/username/hoteldruid-data`
   - **Relative paths**: Relative to the application directory
     - `../hoteldruid-data`

4. **Create the directory**: Make sure the directory exists and is writable. HotelDruid will create the necessary subdirectories on first run.

5. **Migrate existing data** (if needed): If you have an existing `dati` folder, copy its contents to your new location.

## Example Configuration

```php
// Windows - Absolute path
define('C_DATI_PATH_EXTERNAL', "C:/Users/rolfe/Documents/HotelDruid");

// Windows - User Documents folder (recommended)
define('C_DATI_PATH_EXTERNAL', getenv('USERPROFILE') . "/Documents/HotelDruid");

// Linux/Mac - Home directory
define('C_DATI_PATH_EXTERNAL', getenv('HOME') . "/hoteldruid-data");

// Relative path - One level up from application
define('C_DATI_PATH_EXTERNAL', "../hoteldruid-data");
```

## Configuration Priority

HotelDruid checks for the data path in this order:
1. **PHP Desktop settings** (`phpdesktop-settings.json` - for standalone executable)
2. **External config file** (`hoteldruid-config.php` - for web server deployments)
3. **Default** (`./dati` - relative to the application directory)

## Default Behavior

If neither configuration file specifies a data path, HotelDruid will use the default location: `./dati` (relative to the application directory).

## Security Notes

- The `hoteldruid-config.php` file contains configuration only, not sensitive data
- Your actual data (database, etc.) is stored in the `dati` folder at your specified location
- Make sure the data directory has proper file permissions (read/write for the web server user)

## Troubleshooting

### Permission Errors

If you see "I don't have write permissions on dati folder":
- Check that the directory exists
- Verify the application has write permissions to the directory
- On Windows, make sure the folder isn't read-only
- On Linux/Mac, you may need to adjust ownership: `chown -R www-data:www-data /path/to/data`

### Path Not Found

- Verify the path is correct (check for typos)
- Use absolute paths if relative paths aren't working
- Make sure forward slashes are used (even on Windows)

### Data Not Persisting

- Ensure the path points to the same location every time
- Check that the directory is writable
- Verify the path doesn't have trailing slashes (they're automatically removed)

## Migration from Default Location

If you're moving from the default `./dati` location:

1. Stop HotelDruid
2. Copy the entire `dati` folder to your new location
3. Update `hoteldruid-config.php` with the new path
4. Restart HotelDruid
5. Verify everything works, then you can delete the old `dati` folder

## Multiple Installations

You can run multiple HotelDruid installations with different data directories:

```php
// Installation 1
define('C_DATI_PATH_EXTERNAL', "C:/Users/rolfe/Documents/HotelDruid-Production");

// Installation 2 (different config file or different installation)
define('C_DATI_PATH_EXTERNAL', "C:/Users/rolfe/Documents/HotelDruid-Testing");
```

