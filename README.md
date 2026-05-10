# HotelDruid — Blazor WebAssembly + ASP.NET Core migration

This repository contains the migration of HotelDruid toward a Blazor WebAssembly frontend with a thin ASP.NET Core API backend. The legacy PHP application is retained under `hoteldruid/` for reference and incremental migration.

Summary

- Frontend: Blazor WebAssembly (`src/HotelDroid.Client`).
- Backend: ASP.NET Core minimal API (`src/HotelDroid.Api`).
- Persistence plan: file-backed key-value store (collections as directories, documents as JSON).
- Packaging & deployment: PowerShell scripts under `scripts/` produce a per-user package and provide local HTTPS dev helpers.

Quick Start — developer

Prerequisites

- .NET 8/10 SDK installed (use the version in `global.json` if present).
- PowerShell (Windows) or pwsh on other platforms for the included scripts.

Build, package and run locally

```powershell
# 1) Create a package (build + publish + zip)
.\scripts\pack-and-deploy.ps1 -PackageOnly -Force

# 2) Per-user install (no admin required)
.\scripts\deploy-user.ps1 -Force

# 3) Run the API locally (the script binds HTTPS using a dev cert if available)
.\scripts\deploy-api-local.ps1

# 4) Validate uninstall/cleanup
.\scripts\validate-cleanup.ps1
```

Notes

- `deploy-user.ps1` extracts the package into `%LocalAppData%\HotelDroid`, creates a CurrentUser dev certificate (if needed), writes `install-meta.json`, and registers an HKCU uninstall entry.
- To perform a system-wide install use `pack-and-deploy.ps1 -Deploy` and run as Administrator (this writes HKLM uninstall entries).

Project layout (root-level)

```
src/
  ├─ HotelDroid.Api/         # ASP.NET Core minimal API
  ├─ HotelDroid.Client/      # Blazor WebAssembly client
  └─ HotelDroid.Shared/      # DTOs + shared models
hoteldruid/                  # Legacy PHP application (kept for reference)
scripts/                     # Packaging, deploy and cert helper scripts
artifacts/                   # Build outputs and packages (ignored by default)
BLazor-migration-tracker.md  # Migration tracker and status
README.md                    # This file (updated for Blazor+API)
```

Contributing & committing

- Keep build artifacts out of source control. Add `artifacts/`, `bin/`, and `obj/` to `.gitignore` before committing new code.
- Prefer PRs that implement a single feature (API + matching Blazor UI).

Development tips

- Run the API and Blazor client together by publishing the client to the API `wwwroot` (scripts automate this in the packaging step).
- For local HTTPS testing the scripts create a CurrentUser `localhost` cert and bind Kestrel when a thumbprint is provided.

Troubleshooting

- If a dev cert exists but is not trusted, run: `.\scripts\create-dev-cert.ps1 -TrustCurrentUser`
- If an install leaves artifacts behind, run: `.\scripts\validate-cleanup.ps1`

More information

- Migration status and next steps: `BLazor-migration-tracker.md`.
- Packaging and deploy scripts: `scripts/pack-and-deploy.ps1`, `scripts/deploy-user.ps1`, `scripts/deploy-api-local.ps1`.

If you want, I can now:

- Add `artifacts/` to `.gitignore` and commit the change.
- Scaffold the KV store library and basic `clients`/`bookings` endpoints and run a local build.

