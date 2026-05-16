# HotelDruid

HotelDruid is a Blazor WebAssembly frontend with an ASP.NET Core API backend.

## Project Summary

- Frontend: `src/HotelDruid.Client`
- Backend: `src/HotelDruid.Api`
- Shared contracts: `src/HotelDruid.Shared`
- Migration tooling: `tools/HotelDruid.Migration`

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

On every push or pull request targeting the `blazor` branch, a deployment package is built automatically from `./scripts/build.ps1` and attached as a downloadable artifact in the GitHub Actions run (see the Artifacts section in the Actions tab).

**How to deploy:**
1. Download the latest deployment package artifact from the GitHub Actions run for the `blazor` branch.
2. Unzip the package.
3. Run the included `deploy.ps1` script (see usage below).
4. For more details, see the rest of this README and the `scripts/README.md`.

The deployment package includes:
- Build outputs produced under `artifacts/` (including `HotelDruid-package.zip`)
- The deployment script: `scripts/deploy.ps1`
- This `README.md` for reference

For advanced scenarios, see the full repository and documentation.


