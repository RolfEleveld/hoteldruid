# HotelDruid Installer - Latest Package Information

**Date:** December 14, 2025
**Latest Package:** `HotelDruid-slim-20251214-0419.zip` (0.01 MB)

## Recent Fixes

### 1. GitHub Repository Corrected

- **Previous:** `hoteldruid/hoteldruid-community` (non-existent, returned 404)
- **Current:** `RolfEleveld/hoteldruid` (your fork)
- **Branch:** `main`

### 2. HotelDruid Subfolder Extraction

The installer now correctly:

- Downloads your repository from GitHub
- Extracts the ZIP file
- Finds the `hoteldruid/` subfolder inside the extracted repo
- Uses only those files for installation

### 3. File Encoding Cleaned

- Saved with UTF-8 encoding (no BOM)
- Ensures compatibility across all Windows environments
- Fixes any potential character encoding issues

## If You're Getting Syntax Errors

If you see errors like:

```text
The Try statement is missing its Catch or Finally block.
The string is missing the terminator: "
```

**Solution:** You likely have an older cached package. Please:

1. **Delete old packages** from your Downloads folder
2. **Download the latest:** `HotelDruid-slim-20251214-0419.zip` or newer
3. **Clear browser cache** to avoid downloading old versions
4. **Extract fresh** and test again

## Testing Instructions

To verify the package works on your system:

```powershell
# Extract the ZIP file
Expand-Archive -Path HotelDruid-slim-20251214-0419.zip -DestinationPath C:\test_install

# Change to extracted directory
cd C:\test_install

# Run installer (choose your language)
pwsh -ExecutionPolicy Bypass -File .\install_release.ps1 -Language es
pwsh -ExecutionPolicy Bypass -File .\install_release.ps1 -Language en
pwsh -ExecutionPolicy Bypass -File .\install_release.ps1 -Language it
```

## Package Contents

- `install_release.ps1` - Smart installer (~10 KB)
- `start-hoteldruid-desktop.ps1` - Launcher script (~1 KB)
- `README.md` - Installation guide

## What the Installer Does

1. Downloads latest phpdesktop runtime from GitHub
2. Downloads latest HotelDruid from your fork (`RolfEleveld/hoteldruid`)
3. Extracts and configures everything
4. Detects OneDrive for cloud backup
5. Creates Start Menu shortcut
6. Preserves data on future updates

## Supported Languages

- English (`-Language en`)
- Italian (`-Language it`)
- Spanish (`-Language es`)

## Need Help?

If you continue to experience issues:

1. Check that you have PowerShell 5.0+: `$PSVersionTable.PSVersion`
2. Ensure internet connection for downloads
3. Try running from a fresh extraction
4. If still failing, contact support with the full error message

---

**All packages are now using the correct GitHub repository and proper file encoding.**
