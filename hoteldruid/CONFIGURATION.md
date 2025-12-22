# Configuration

HotelDruid allows you to store all persistent data (database, configuration, uploads) in a location outside the application directory. This is useful for:

- **Easy Updates**: Update the application without overwriting your data
- **Backup Management**: Keep data in a known, separate location
- **Portable Installations**: Move the application while keeping data in place
- **Multiple Installations**: Run different versions with different data directories

Primary application configuration is via `hoteldruid-config.php`.

- Set `C_DATI_PATH_EXTERNAL` to the absolute or relative path of your data directory.
- If unset, the application defaults to `./dati` relative to the app.

## Path Formats & Examples

Set `C_DATI_PATH_EXTERNAL` to your desired location:

```php
define('C_DATI_PATH_EXTERNAL', "/Documents/HotelDruid");
```

Path format guidelines:

- Windows: Use forward slashes (`/`) or double backslashes (`\\`)
  - `/Documents/HotelDruid`
  - `\\Documents\\HotelDruid`
- Linux/Mac: Use forward slashes
  - `/home/username/hoteldruid-data`
- Relative paths: Relative to the application directory
  - `../hoteldruid-data`

## Setup Notes

- Ensure the directory exists and is writable. HotelDruid creates subdirectories on first run.
- Migrating from default `./dati`:
  1. Stop HotelDruid
  2. Copy `dati` folder to new location
  3. Update `hoteldruid-config.php`
  4. Restart and verify

## Configuration Priority

1. External config file (`hoteldruid-config.php`)
2. Default `./dati` (relative to the application directory)

## Security Notes

- `hoteldruid-config.php` stores configuration only, not sensitive data.
- Data lives in your `dati` directory; ensure proper read/write permissions.

## Troubleshooting

- Permission errors: verify directory exists and is writable; adjust ownership if needed.
- Path not found: check typos; prefer absolute paths; use forward slashes on Windows.
- Data not persisting: confirm consistent path; avoid trailing slashes.

## Multiple Installations

```php
// Installation 1
define('C_DATI_PATH_EXTERNAL', "/Documents/HotelDruid-Production");

// Installation 2
define('C_DATI_PATH_EXTERNAL', "/Documents/HotelDruid-Testing");
```

Reference:

- Runtime `phpdesktop/settings.json` is installer-generated; do not embed or edit it in the app.
