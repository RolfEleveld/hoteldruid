# Compose Overlay Templates

This folder contains starter compose overlays for scenario scripts.

These files are templates. Administrators should adapt them to their environment,
network topology, certificate automation, and identity policies.

Included templates:

- `my-proxy.yml`: reverse proxy overlay (NGINX example)
- `keycloak.yml`: identity provider overlay (Keycloak + PostgreSQL example)
- `nginx/default.conf`: proxy config mounted by `my-proxy.yml`

Notes:

- For public environments, do not deploy without authentication.
- ACME issuance/renewal is intentionally not hardcoded here; integrate your
  organization's chosen automation and bind resulting certificate files using env vars.
