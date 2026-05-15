# Deployment Scenarios (Neutral)

This folder defines provider-neutral deployment scenarios and environment contracts.

## Goals

- Keep deployment strategy outside app code.
- Allow administrators to choose reverse proxy, TLS, and identity provider.
- Support repeatable up/test/down workflows in scripts.

## Scenario Catalog

1. `internal-selfsigned-private`
- Intended for private/internal environments.
- TLS may use self-signed certificates.
- Auth requirements are organization policy driven.

2. `public-acme-keycloak`
- Intended for internet-exposed environments.
- TLS via ACME automation.
- Authentication and authorization via Keycloak OIDC.

3. `public-externalcert-keycloak`
- Intended for enterprise/public environments with externally managed certs.
- Authentication and authorization via Keycloak OIDC.

## How to Use

Use the scenario scripts in `scripts/`:

```powershell
# Bring a scenario up
./scripts/scenario-up.ps1 -Scenario public-acme-keycloak -ComposeFiles @("docker-compose.yml", "deploy/compose/my-proxy.yml", "deploy/compose/keycloak.yml") -EnvFile "deploy/scenarios/public-acme-keycloak.env"

# Run checks
./scripts/scenario-test.ps1 -BaseUrl "https://hotel.example.com" -RequireAuth

# Tear down
./scripts/scenario-down.ps1 -ComposeFiles @("docker-compose.yml", "deploy/compose/my-proxy.yml", "deploy/compose/keycloak.yml") -RemoveVolumes
```

Included starter templates:

- `deploy/compose/my-proxy.yml`
- `deploy/compose/keycloak.yml`
- `deploy/scenarios/public-acme-keycloak.env`

Customize these templates before production use.

## Notes

- Compose file selection is intentionally admin-provided.
- This avoids forcing a specific proxy or ACME implementation.
- Public scenarios should not be configured for anonymous access.
