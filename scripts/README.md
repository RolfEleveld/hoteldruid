# HotelDruid Scripts

This folder contains all automation scripts for building, deploying, and testing HotelDruid. Each script is single-purpose and follows convention over configuration—most common scenarios require no parameters.

## Scripts Overview

- **build.ps1**: Build and assemble the Blazor client and API for local development. Always produces a deployable package (`artifacts/HotelDruid-package.zip`).
- **deploy.ps1**: Deploy the package locally, as a per-user install (`-User`), or to a specified target (`-Target <path>`). For per-user installs it creates or trusts the localhost certificate, writes install metadata, creates Start Menu shortcuts, and generates the uninstall script and registry entry.
- **test.ps1**: Run all unit and integration tests, generate reports.
- **scenario-up.ps1**: Bring up a selected deployment scenario with admin-provided compose files.
- **scenario-test.ps1**: Run health and optional auth-gate checks for a deployed scenario.
- **scenario-down.ps1**: Tear down a scenario stack with optional volume/orphan cleanup.

## Usage

- **Build**: `./build.ps1` (optionally add `-Clean` or `-Configuration Release`)
- **Deploy locally**: `./deploy.ps1` (optionally add `-Publish`, `-CertThumbprint`, `-OpenBrowser`)
- **Install for current user**: `./deploy.ps1 -User` (optionally add `-Target <path>` and `-Force`)
- **Deploy to custom/remote**: `./deploy.ps1 -Target "C:\DeployPath"`
- **Test**: `./test.ps1` (optionally add `-OpenDashboard` or `-Filter`)
- **Scenario up**: `./scenario-up.ps1 -Scenario public-acme-keycloak -ComposeFiles @("docker-compose.yml", "deploy/compose/proxy.yml")`
- **Scenario test**: `./scenario-test.ps1 -BaseUrl "https://hotel.example.com" -RequireAuth`
- **Scenario down**: `./scenario-down.ps1 -ComposeFiles @("docker-compose.yml", "deploy/compose/proxy.yml") -RemoveVolumes`

## Conventions

- Scripts assume the most common scenario by default.
- Each script is single-purpose: one for build, one for deploy, one for test.
- Parameters are optional and only needed for advanced scenarios.

## Notes

- All scripts require PowerShell 7+.
- For production or distribution, use `build.ps1` then `deploy.ps1 -Target <path>`.
- For local development, use `build.ps1` then `deploy.ps1`.
- For running tests, use `test.ps1`.

---

If you need to customize or extend, see comments at the top of each script for details.

