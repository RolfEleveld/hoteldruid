# HotelDruid Desktop Deployment (phpdesktop primary)

This is the single deployment guide for the Windows phpdesktop build. It covers packaging, install/update/uninstall, data location, and where to look for scale-out/server options.

## When to use this guide

- Default path: single-machine Windows desktop installs using phpdesktop.
- For multi-user or server scale-out, start here for terminology, then jump to the Docker/LAMP guidance in [README.md](README.md).

## Release package and CI

- Local build: `./build_deployment_package.ps1` creates the minimal ZIP with installer + uninstaller + launcher.
- CI build: [.github/workflows/release.yml](.github/workflows/release.yml) produces `hoteldruid-PHPDesktop-<version>.zip` on `main` pushes.
- Contents: [install_release.ps1](install_release.ps1), [uninstall_release.ps1](uninstall_release.ps1), [start-hoteldruid-desktop.ps1](start-hoteldruid-desktop.ps1), README.

## Install / update (desktop)

1) Download/unzip the release ZIP.
2) Run (auto language detection, data preserved):
``powershell
pwsh -ExecutionPolicy Bypass -File .\install_release.ps1
``
3) Optional flags: `-InstallDir`, `-DataFolder`, `-CreateDesktopShortcut`, `-LaunchAfterInstall`, `-Language en|it|es`.
4) Silent/scripted (winget-friendly):
``powershell
pwsh -ExecutionPolicy Bypass -File .\install_release.ps1 -Silent
``
Re-running the installer updates in place and keeps data/settings.

## Uninstall

- Keep data/settings (default):
``powershell
pwsh -ExecutionPolicy Bypass -File .\uninstall_release.ps1 -Silent
``
- Remove everything (app + data + roaming settings):
``powershell
pwsh -ExecutionPolicy Bypass -File .\uninstall_release.ps1 -RemoveDataFolder -RemoveSettings -Silent
``

## Data and settings

- Install path (default): `%LOCALAPPDATA%/HotelDruid`
- Data folder: OneDrive `HotelDruid/dati` when available; otherwise `Documents/HotelDruid/dati` or a custom `-DataFolder`.
- Settings cache: `%APPDATA%/HotelDruid/deployment-settings.json` (used for upgrades; remove with `-RemoveSettings`).

## Scale-out / server option (when you outgrow a single desktop)

- Use the Docker/LAMP deployment documented in [README.md](README.md) for multi-user, centralized DB, backups, and remote access.
- Basics: run `./start-containers.ps1` or `docker-compose up -d`, then access `http://localhost:8080` (see the Docker quick start section in the root README).
- Keep desktop and server data separate; migrate by exporting from the desktop data folder and importing into the server MySQL instance.

## Operational notes

- VC++ runtime: installer auto-installs or uses bundled DLLs when available.
- Shortcuts: Start Menu by default; desktop/startup optional via flags.
- Logs/validation: installer writes validation results to `%APPDATA%/HotelDruid/install-validation.log` on failure.

## Support checklist

- Install/Update: `pwsh -ExecutionPolicy Bypass -File .\install_release.ps1`
- Uninstall (keep data): `pwsh -ExecutionPolicy Bypass -File .\uninstall_release.ps1 -Silent`
- Uninstall (wipe): `pwsh -ExecutionPolicy Bypass -File .\uninstall_release.ps1 -RemoveDataFolder -RemoveSettings -Silent`
- Build package: `./build_deployment_package.ps1`

All older deployment docs are consolidated here to keep one source of truth for the phpdesktop path.
