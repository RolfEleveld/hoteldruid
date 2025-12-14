# build_customer_package.md

This document describes how to build and test the customer-ready **Windows Desktop** package (phpdesktop + PHP + HotelDruid), and how the customer installs it.

## Goals

- Always package a **fresh phpdesktop download** (latest release) so the customer gets up-to-date binaries.
- Produce a **smaller ZIP** by pruning caches, sample content, and unnecessary locales.
- Provide a **multilingual installer** (Italian, English, Spanish).
- Provide a **repeatable sandbox test** on your machine.

## Build the customer ZIP

From the repo root:

```powershell
pwsh -ExecutionPolicy Bypass -File .\build_customer_package.ps1
```

Output:

- Creates a ZIP under `out\` named like `HotelDruid-customer-YYYYMMDD-HHMM.zip`.

### Build options

- Keep all Chromium locales (bigger package):

```powershell
pwsh -ExecutionPolicy Bypass -File .\build_customer_package.ps1 -KeepAllLocales
```

- Keep only specific locales (default list is already set to `en-US`, `en-GB`, `it`, `es`, `es-419`). You can override it:

```powershell
pwsh -ExecutionPolicy Bypass -File .\build_customer_package.ps1 -KeepLocales @('it','en-US','es')
```

- Skip download and use a local phpdesktop ZIP (useful for repeatable builds / offline builds):

```powershell
pwsh -ExecutionPolicy Bypass -File .\build_customer_package.ps1 -SkipDownload -PhpDesktopZipPath C:\path\to\phpdesktop.zip
```

- Disable pruning entirely (largest output; best for debugging):

```powershell
pwsh -ExecutionPolicy Bypass -File .\build_customer_package.ps1 -NoPrune
```

## What goes into the ZIP

The resulting ZIP includes:

- `phpdesktop/` (downloaded fresh)
- `hoteldruid/` (your app)
- `install_release.ps1` (installer)
- `start-hoteldruid-desktop.ps1` (launcher)
- `README.md` (customer installation instructions)

### What gets pruned (for smaller size)

By default, the build script removes:

- `phpdesktop/webcache/`
- `phpdesktop/debug.log`
- `phpdesktop/www/` (phpdesktop sample site)
- most `phpdesktop/locales/*.pak` (keeps only the configured list)
- some PHP dev/debug docs/binaries under `phpdesktop/php/` (keeps runtime + extensions)

## Customer installation

The customer should:

1. Extract the ZIP.
2. Run the installer.

Examples (choose language):

```powershell
# Italian
pwsh -ExecutionPolicy Bypass -File .\install_release.ps1 -Language it

# English
pwsh -ExecutionPolicy Bypass -File .\install_release.ps1 -Language en

# Spanish
pwsh -ExecutionPolicy Bypass -File .\install_release.ps1 -Language es
```

Notes:

- If `-Language` is omitted, the installer tries to auto-detect from Windows culture (defaults to English).
- The installer defaults to a per-user install folder under `LOCALAPPDATA` to avoid admin rights.

### Optional installer flags

```powershell
# Install to a specific directory
pwsh -ExecutionPolicy Bypass -File .\install_release.ps1 -Language it -InstallDir "C:\HotelDruid"

# Create a Desktop shortcut
pwsh -ExecutionPolicy Bypass -File .\install_release.ps1 -Language it -CreateDesktopShortcut:$true

# Launch after installing
pwsh -ExecutionPolicy Bypass -File .\install_release.ps1 -Language it -LaunchAfterInstall:$true
```

## Sandbox testing (recommended)

To verify the ZIP and installer end-to-end on your machine:

```powershell
pwsh -ExecutionPolicy Bypass -File .\test_sandbox_install.ps1 -Language en
```

This:

- builds a fresh ZIP
- extracts it under `out\sandbox\extracted`
- installs it under `out\sandbox\installed`

To test launching immediately:

```powershell
pwsh -ExecutionPolicy Bypass -File .\test_sandbox_install.ps1 -Language en -Launch
```

## Troubleshooting

- If download fails, re-run later or use `-SkipDownload -PhpDesktopZipPath ...`.
- If the package is still too large, tighten the `-KeepLocales` list.
- If the customer needs an all-users install location (e.g., `Program Files`), run PowerShell as Administrator and set `-InstallDir` accordingly.
