# Deployment Runbook

This runbook describes a deployment process for containerized HotelDruid.

## 1. Select a Scenario

Choose one scenario based on exposure and trust model:

1. Internal validation: `internal-selfsigned-private`
2. Public production: `public-acme-keycloak`
3. Enterprise production: `public-externalcert-keycloak`

## 2. Provide Deployment Inputs

Provide the following deployment artifacts:

- Compose files for API, reverse proxy/ingress, and identity provider
- Environment file with domain names, secrets references, and ports
- Storage/volume mappings for API data and identity state

## 3. Bring Up Environment

```powershell
./scripts/scenario-up.ps1 -Scenario public-acme-keycloak -ComposeFiles @("docker-compose.yml", "deploy/compose/my-proxy.yml", "deploy/compose/keycloak.yml") -EnvFile "deploy/scenarios/public-acme-keycloak.env" -Build
```

## 4. Validate Environment

```powershell
./scripts/scenario-test.ps1 -BaseUrl "https://hotel.example.com" -RequireAuth -ProtectedPath "/"
```

Checks include:

- API health endpoint availability
- TLS endpoint reachability
- Optional auth gating for protected endpoints

## 5. Tear Down Environment

```powershell
./scripts/scenario-down.ps1 -ComposeFiles @("docker-compose.yml", "deploy/compose/my-proxy.yml", "deploy/compose/keycloak.yml") -RemoveVolumes
```

## 6. Security Baseline

- Public deployments must require authentication.
- Use Keycloak (or equivalent OIDC provider) for centralized authN/authZ.
- Store secrets in a secret manager, not plaintext env files.
- Enforce least-privilege access for deployment accounts.

## 7. CI/CD Integration Guidance

- Run scenario-up in pre-deploy integration stage.
- Run scenario-test as smoke and policy gate.
- Run scenario-down after validation or on rollback.
- Keep scenario scripts and compose overlays versioned with the repository.

