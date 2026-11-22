# HotelDruid

HotelDruid is a Blazor WebAssembly frontend with an ASP.NET Core API backend.

## Install a desk computer

On the computer that will run the hotel, open PowerShell and run the command published on the `blazor-latest` release. It has this shape, with the GitHub owner filled in:

```powershell
iex (irm https://github.com/<owner>/hotelDruid/releases/download/blazor-latest/install.ps1)
```

Approve the administrator prompt. The command downloads `HotelDruid-desktop.zip`, installs the ASP.NET Core 10 runtime when it is missing, copies the hotel to `C:\ProgramData\HotelDruid`, starts it at every boot, and puts HotelDruid on the desktop.

A computer with no network uses the same zip: unzip it and run `HotelDruid.cmd`.

The steps, the nightly backup, and removal are in [Deploy](docs/manual/chapters/01-deploy.md). A public address, a certificate, and sign-in are in the [deployment runbook](docs/deployment-runbook.md).

The same GitHub build attaches `HotelDruid.HotelDruid.yaml`, the winget manifest for that zip with its hash filled in. `winget install HotelDruid.HotelDruid` works after that file is accepted into the Windows Package Manager community repository. The source of the manifest is [deploy/winget/HotelDruid.HotelDruid.yaml](deploy/winget/HotelDruid.HotelDruid.yaml).

## Recent Updates (May 2026)

The latest implementation adds a staged configuration and assumption workflow to improve first-run usability and reduce manual maintenance.

### 1. System Configuration as First-Class Data

- System configuration is stored in the same object store model as other entities.
- API endpoints:
  - `GET /api/system/configuration`
  - `PUT /api/system/configuration`
  - `DELETE /api/system/configuration`
- Configuration is included in export/import workflows when present (`system_configuration`).

### 2. Setup Detection and Admin Setup Flow

- API setup state endpoint:
  - `GET /api/system/setup/status`
- Setup mode is triggered when:
  - configuration is missing and caller is admin/local
  - setup is explicitly requested (`?configure=true`)
- Client setup page:
  - `/setup`
  - includes baseline values such as default currency/year and fallback settings

### 3. Assumption Rule Engine and Async Self-Healing

- New rule engine handles valid year gaps for selected year-scoped data.
- Current self-healing scope:
  - periods (availability-supporting windows)
  - tariffs (pricing/supporting rates)
- Behavior:
  - first request for missing year returns an empty list
  - async background healing clones from configured source year/current default
  - subsequent requests receive generated data
- Important boundary:
  - invalid entities still return `404` (for example unknown room id)

### 4. Regression Coverage Added

- Unit and integration tests were added for:
  - configuration persistence and API behavior
  - setup-state detection
  - export/import inclusion of system configuration
  - async year-gap self-healing for periods and tariffs
  - invariant `404` behavior for non-existent entities

## Project Summary

- Frontend: `src/HotelDruid.Client`
- Backend: `src/HotelDruid.Api`
- Shared contracts: `src/HotelDruid.Shared`
- Migration tooling: `tools/HotelDruid.Migration`

## Play with a hotel

The hotel manual, including a fresh install and the demo used for training, is in [docs/manual](docs/manual/README.md). The desk install is [above](#install-a-desk-computer). Build the picture guide with `./scripts/manual.ps1`.

- Docker, from this repository: `docker compose up --build`, then open `http://localhost:8080/`.

## Build, Deploy, Test

PowerShell scripts use a convention-over-configuration workflow:

```powershell
# Build + publish + package
.\scripts\build.ps1
.\scripts\build.ps1 -Clean

# Deploy locally (default) or per-user install
.\scripts\deploy.ps1
.\scripts\deploy.ps1 -User

# Run all tests
.\scripts\test.ps1
.\scripts\test.ps1 -Clean

# Refresh the hotel manual from the running desk
.\scripts\manual.ps1
```

`-Clean` removes old outputs and runs a full clean/rebuild path before build or test execution.

See `scripts/README.md` for script parameters and details.

## Deployment Profiles (Short Form)

Hosted deployment guidance is provider-neutral. Administrators choose reverse proxy,
TLS, and identity settings through deployment profiles rather than application code.

- Reverse proxy: `nginx`, `caddy`, `traefik`, or platform ingress
- TLS: `self-signed` (private/internal), `acme`, or externally managed certs
- Identity: `keycloak-oidc` (recommended for public deployments) or enterprise SSO

Recommended scenarios:

1. Internal validation: self-signed + private network
2. Public production: ACME + Keycloak OIDC
3. Enterprise production: external cert + Keycloak OIDC

For scenario orchestration and run-books, see:

- `deploy/scenarios/README.md`
- `docs/deployment-runbook.md`

## Deployment Package (CI Artifacts)

On every push or pull request targeting the `blazor` branch, a deployment package is built automatically from `./scripts/build.ps1`.

For easier discovery on the repository main page, push builds also publish/update a Release asset under the `blazor-latest` tag.
You can download the package from either:
1. Releases (`blazor-latest`) on the repository home page
2. The Artifacts section in the matching GitHub Actions run

**Desk computer:** see [Install a desk computer](#install-a-desk-computer) and [Deploy](docs/manual/chapters/01-deploy.md).

**How to deploy a development copy:**
1. Download the build artifact from the matching GitHub Actions run, or build with `./scripts/build.ps1`.
2. Unzip the package.
3. Run the included `deploy.ps1` script.
4. Parameters are in `scripts/README.md`.

The deployment package includes:
- Build outputs produced under `artifacts/` (including `HotelDruid-package.zip`)
- The deployment script: `scripts/deploy.ps1`
- This `README.md` for reference

For advanced scenarios, see the full repository and documentation.


