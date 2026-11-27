# Build

These scripts build, package, test, and install HotelDruid. Run them from the repository root as `./deploy/build/<script>`. Each script is single-purpose. Most need no parameters.

## Scripts Overview

- **build.ps1**: Build and assemble the Blazor client and API for local development. Always produces a deployable package (`artifacts/HotelDruid-package.zip`) and CycloneDX SBOM JSON artifacts under `artifacts/sbom/`.
- **deploy.ps1**: Deploy the package locally, as a per-user install (`-User`), or to a specified target (`-Target <path>`). It creates a localhost certificate for the current user and trusts it. `trust-dev-cert.ps1`, from an administrator PowerShell, copies that public certificate into the machine trusted root store when the browser still says the address is not secure. A per-user install also writes install metadata, creates Start Menu shortcuts, and generates the uninstall script and registry entry.
- **desk-certificate.ps1**: An extra step after a localhost install. The build copies it into the desk package. After install, including the one-command install, it is `C:\ProgramData\HotelDruid\app\desk-certificate.ps1`. The steps are in [../README.md](../README.md).
- **desktop/HotelDruid.cmd**: The desk program. `Install` is the one administrator setup: it installs ASP.NET Core 10 when it is missing, copies the hotel to `ProgramData\HotelDruid`, creates a localhost certificate in the machine personal store, adds the public certificate to the machine trusted root store, and registers a startup task as Local Service with write access only to the data and log folders. Opening HotelDruid afterwards checks that the desk answers, starts it when it is down, and opens the browser. `Uninstall` writes the data export into the current user's Temp folder.
- **desktop/Start-Sandbox.ps1**: Starts a Windows Sandbox with the built hotel mapped in. The sandbox installs Windows Package Manager, PowerShell 7, and the .NET 10 host and ASP.NET runtime, then installs HotelDruid when it is not already installed. Close an open sandbox first.
- **Start-CopySandbox.ps1**: Starts a Windows Sandbox for the self-contained Windows folder. Networking is off. The sandbox copies `artifacts/selfcontained/win-x64` and runs `Start-HotelDruid.cmd`. Close an open sandbox first.
- **desktop/Backup-HotelDruid.ps1**: Calls the export API and saves the zip. `HotelDruid.cmd Install -BackupFolder` schedules it nightly. The steps are in the hotel manual, `docs/manual`.
- **desktop/Load-DemoHotel.ps1**: On an installed desk, exports the current hotel into the current user's Temp folder, then replaces the data folder with Hotel Belvedere. The range is five years before today through six months after today, 20 rooms, occupancy 0.67. The last line of output is the export path.
- **package-desktop.ps1**: Builds `artifacts/HotelDruid-desktop.zip` from a Release publish under `artifacts/play`. It does not publish over `artifacts/api`. The GitHub build of `blazor` publishes that zip and `install.ps1`. On a desk computer: `iex (irm https://github.com/RolfEleveld/hoteldruid/releases/download/blazor-latest/install.ps1)`.
- **package-selfcontained.ps1**: Builds `artifacts/HotelDruid-selfcontained-win-x64.zip` and `artifacts/HotelDruid-selfcontained-linux-x64.zip`. The runtime is inside each folder. Windows starts with `Start-HotelDruid.cmd`. Linux starts with `./start-hoteldruid.sh`. Both listen at `http://127.0.0.1:8080/` and keep the hotel in `data` beside the command. It does not publish over `artifacts/api`. `-Runtime win-x64` or `-Runtime linux-x64` builds one of them. The Linux zip is glibc, not Alpine.
- **manual.ps1**: Refresh the manual pictures from a running desk and write `docs/manual/HotelDruid.html` and `docs/manual/out/HotelDruid.epub`. Pictures listed in `docs/manual/images/hotel.txt` are left for the hotel.
- **test.ps1**: Run all unit and integration tests, generate reports.
- **scenario-up.ps1**: Bring up a selected deployment scenario with admin-provided compose files.
- **scenario-test.ps1**: Run health and optional auth-gate checks for a deployed scenario.
- **scenario-down.ps1**: Tear down a scenario stack with optional volume/orphan cleanup.

## Usage

- **Build**: `./build.ps1` (optionally add `-Clean` or `-Configuration Release`)
- **Deploy locally**: `./deploy.ps1` (optionally add `-Publish`, `-CertThumbprint`, `-OpenBrowser`)
- **Install for current user**: `./deploy.ps1 -User` (optionally add `-Target <path>` and `-Force`)
- **Deploy to custom/remote**: `./deploy.ps1 -Target "C:\DeployPath"`
- **Test**: `./test.ps1` (optionally add `-Clean`, `-OpenDashboard`, or `-Filter`)
- **Manual**: `./manual.ps1` (optionally `-Build` only, or `-Capture -BaseUrl https://localhost:5001`)
- **Scenario up**: `./scenario-up.ps1 -Scenario public-acme-keycloak -ComposeFiles @("deploy/container/compose/plain.yml", "deploy/container/compose/proxy.yml", "deploy/container/compose/keycloak.yml") -EnvFile deploy/container/compose/public.env`
- **Scenario test**: `./scenario-test.ps1 -BaseUrl "https://hotel.example.com" -RequireAuth`
- **Scenario down**: `./scenario-down.ps1 -ComposeFiles @("deploy/container/compose/plain.yml", "deploy/container/compose/proxy.yml", "deploy/container/compose/keycloak.yml") -RemoveVolumes`

## Conventions

- Scripts assume the most common scenario by default.
- Each script is single-purpose: one for build, one for deploy, one for test.
- Parameters are optional and only needed for advanced scenarios.

## Notes

- All scripts require PowerShell 7+.
- `build.ps1 -Clean` removes prior build artifacts, runs `dotnet clean` on source projects, and then performs a full rebuild/package.
- `build.ps1` also generates CycloneDX SBOM files for `HotelDruid.Api` and `HotelDruid.Client` under `artifacts/sbom/`.
- `test.ps1 -Clean` removes previous test result artifacts, runs `dotnet clean` on test projects, and then rebuilds/runs all tests.
- For production or distribution, use `build.ps1` then `deploy.ps1 -Target <path>`.
- For local development, use `build.ps1` then `deploy.ps1`.
- For running tests, use `test.ps1`.

---

If you need to customize or extend, see comments at the top of each script for details.

