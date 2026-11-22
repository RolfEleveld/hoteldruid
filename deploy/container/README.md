# Container

The GitHub build publishes one hotel image. It listens on port 8080 inside the container. The hotel files are written under `/data`. `HOTELDRUID_PORT` changes the host port. `HOTELDRUID_DATA` points `/data` at a folder. `HOTELDRUID_IMAGE` selects the image.

Every scenario below runs that same image. Keycloak and the proxy are the extra images a scenario adds beside it.

The runtime image is Alpine, which is the small image that can still call the health check.

## The image from the build

A push to `blazor` publishes:

- `ghcr.io/<owner>/hoteldruid:blazor`
- `ghcr.io/<owner>/hoteldruid:sha-<commit>`

Every run, including a pull request, also stores `hoteldruid-container-<run>.tar` with the Actions artifacts for 14 days. Load that file when the registry is not the source:

```powershell
docker load -i hoteldruid-container.tar
$env:HOTELDRUID_IMAGE = 'hoteldruid:local'
```

The package is private until its visibility is changed in the GitHub package settings.

## Plain

The hotel image only. No sign-in. From this repository, the image is built on the machine:

```powershell
docker compose -f deploy/container/compose/plain.yml up --build
```

Open `http://localhost:8080/`.

The published image, without a local build:

```powershell
$env:HOTELDRUID_IMAGE = 'ghcr.io/<owner>/hoteldruid:blazor'
docker compose -f deploy/container/compose/plain.yml up --pull always --no-build
```

Another host port, or a folder instead of a Docker volume:

```powershell
$env:HOTELDRUID_PORT = '9080'
$env:HOTELDRUID_DATA = 'C:\hoteldruid\data'
docker compose -f deploy/container/compose/plain.yml up --build
```

`docker compose -f deploy/container/compose/plain.yml down` stops it and keeps the data. Adding `-v` deletes the named volume.

## Keycloak

The hotel image, plus Keycloak and its database. The desk asks for a sign-in. Keycloak is on port 8081. The example values, including the admin password, are in [public.env](compose/public.env). Change that password before this stack is used.

```powershell
docker compose -f deploy/container/compose/plain.yml -f deploy/container/compose/keycloak.yml --env-file deploy/container/compose/public.env up --build
```

The realm file is [keycloak/hoteldruid-realm.json](keycloak/hoteldruid-realm.json). How to finish that setup is [docs/keycloak-setup.md](../../docs/keycloak-setup.md).

## Reverse proxy

The hotel image, plus nginx. The proxy publishes ports 80 and 443 and forwards to the hotel. Put the certificate at `deploy/container/compose/certs/fullchain.pem` and `privkey.pem`, or set `TLS_CERT_PATH` and `TLS_KEY_PATH`. The proxy expects those files. It does not request a certificate itself.

```powershell
docker compose -f deploy/container/compose/plain.yml -f deploy/container/compose/proxy.yml --env-file deploy/container/compose/public.env up --build
```

The hotel port from the plain file stays published as well.

## Public

The hotel image, the reverse proxy, and Keycloak. This is the stack for a public address. Use the published image by setting `HOTELDRUID_IMAGE` and replacing `--build` with `--pull always --no-build`.

```powershell
docker compose -f deploy/container/compose/plain.yml -f deploy/container/compose/proxy.yml -f deploy/container/compose/keycloak.yml --env-file deploy/container/compose/public.env up --build
```

The longer runbook is [docs/deployment-runbook.md](../../docs/deployment-runbook.md). The same stack can be started with [../build/scenario-up.ps1](../build/scenario-up.ps1).

## What the host chooses

The application does not pick the proxy or the certificate. The host does.

- Reverse proxy: the nginx file in this folder, or Caddy, Traefik, or the platform ingress
- Certificate: a private certificate, or one the host already has
- Sign-in: none for an internal desk, or Keycloak for a public address
