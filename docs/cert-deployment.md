# Certificate deployment (development & reproducible steps)

This document describes reproducible steps to create, export, and trust a development TLS certificate for `localhost`, and options for deploying the certificate on other developer machines or CI systems.

## Goals
- Produce a reproducible certificate named `HotelDroid Dev Localhost`.
- Make it trusted by the browser for the current user (no admin required) or system-wide (requires admin).
- Provide a PFX/CER export for use in CI or containerized deployments.

## Files added
- `scripts/create-dev-cert.ps1` — PowerShell helper to create and optionally trust the cert and export PFX/CER.

## Windows: Developer machines (recommended)

1. Open PowerShell (normal user) and run:

```powershell
# create and trust for current user (no admin required)
.\scripts\create-dev-cert.ps1 -Store CurrentUser -Trust
```

2. After the script reports the thumbprint, run your local deploy (example):

```powershell
# stop any dotnet processes
Stop-Process -Name dotnet -Force -ErrorAction SilentlyContinue

# run deploy in background so terminal doesn't block (uses script/deploy-api-local.ps1)
Start-Process -FilePath pwsh -ArgumentList "-NoProfile","-Command","& { . '$PWD\scripts\deploy-api-local.ps1' -CertThumbprint '<thumb>' }" -WindowStyle Hidden
```

3. Restart your browser and open `https://localhost:5001`.

Notes:
- If you want system-wide trust for all users, run the create script with `-Store LocalMachine -Trust` from an elevated PowerShell (Admin).
- `scripts/create-dev-cert.ps1` exports `certs/hoteldroid_<thumb>.pfx` and `.cer` in the repo for reuse.

## Linux / macOS or cross-platform CI

1. Create a certificate and key using OpenSSL (example):

```bash
openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
  -keyout localhost.key -out localhost.crt -subj "/CN=localhost"

# create pfx for Kestrel (optional)
openssl pkcs12 -export -out hoteldroid_localhost.pfx -inkey localhost.key -in localhost.crt -passout pass:changeit
```

2. Configure Kestrel / ASP.NET Core to use the PFX via environment variables in CI or container:

```powershell
# Windows PowerShell (example)
$env:ASPNETCORE_Kestrel__Certificates__Default__Path = 'C:\path\to\hoteldroid_localhost.pfx'
$env:ASPNETCORE_Kestrel__Certificates__Default__Password = 'changeit'

# Linux / Bash (example)
export ASPNETCORE_Kestrel__Certificates__Default__Path=/app/certs/hoteldroid_localhost.pfx
export ASPNETCORE_Kestrel__Certificates__Default__Password=changeit
```

3. In CI, store the PFX file and password as protected secrets and set the environment variables before `dotnet run` or `dotnet publish`.

## Containers (Docker)

- Copy the PFX into the container image or mount at runtime.
- Set `ASPNETCORE_Kestrel__Certificates__Default__Path` and `...__Password` to point to the PFX inside the container.

## Security notes
- Development self-signed certs are for local dev only. Don't use them in production.
- Protect PFX files and passwords in CI (use secrets managers).

## Troubleshooting
- Browser still shows `NET::ERR_CERT_AUTHORITY_INVALID`: Ensure the CER was imported to the appropriate Trusted Root store (`CurrentUser\Root` for the current user or `LocalMachine\Root` for system-wide). Restart the browser after import.
- Server still presents a different cert: ensure environment variables `ASPNETCORE_Kestrel__Certificates__Default__Store` and `ASPNETCORE_Kestrel__Certificates__Default__Thumbprint` are set (the `deploy-api-local.ps1` script does this when provided a thumbprint).

## Next steps for automation
- Add a CI job that creates or retrieves a PFX and sets secrets (certificate+password) for ephemeral test deployments.
