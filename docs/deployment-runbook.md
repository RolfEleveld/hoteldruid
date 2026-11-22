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
./deploy/build/scenario-up.ps1 -Scenario public-acme-keycloak -ComposeFiles @("deploy/container/compose/plain.yml", "deploy/container/compose/proxy.yml", "deploy/container/compose/keycloak.yml") -EnvFile "deploy/container/compose/public.env" -Build
```

## 4. Confirm the data root and the identity front

The API reads `HOTELDRUID_DATAROOT`. On Linux that name is case-sensitive. The image and `deploy/container/compose/plain.yml` set it to `/data`, which is the persisted volume.

Keycloak is optional. The API reports a front only when `IDENTITY_PROVIDER=keycloak` and a GET of `{IDENTITY_ISSUER}/.well-known/openid-configuration` succeeds from the API container. `IDENTITY_ISSUER` is that internal address (`http://keycloak:8080/realms/HotelDruid`). `IDENTITY_ADMIN_CONSOLE_URL` is the address the browser opens. When the probe succeeds, the HotelDruid menu shows Identity and that page links to the Keycloak admin console. When the probe fails, or the provider is `none`, the Identity menu stays hidden. HotelDruid does not store Keycloak admin credentials and does not keep a second login directory.

After the realm in `docs/keycloak-setup.md` exists, `GET /api/system/identity-front` should return `isFront: true`. `scenario-test.ps1` does not perform that probe.

## 5. Validate Environment

```powershell
./deploy/build/scenario-test.ps1 -BaseUrl "https://hotel.example.com" -RequireAuth -ProtectedPath "/"
```

Checks include:

- API health endpoint availability
- TLS endpoint reachability
- Optional auth gating for protected endpoints

## 6. Tear Down Environment

```powershell
./deploy/build/scenario-down.ps1 -ComposeFiles @("deploy/container/compose/plain.yml", "deploy/container/compose/proxy.yml", "deploy/container/compose/keycloak.yml") -RemoveVolumes
```

## 7. Security Baseline

- Public deployments must require authentication.
- Use Keycloak (or equivalent OIDC provider) for centralized authN/authZ.
- Store secrets in a secret manager, not plaintext env files.
- Enforce least-privilege access for deployment accounts.

## 8. CI/CD Integration Guidance

- Run scenario-up in pre-deploy integration stage.
- Run scenario-test as smoke and policy gate.
- Run scenario-down after validation or on rollback.
- Keep scenario scripts and compose overlays versioned with the repository.

