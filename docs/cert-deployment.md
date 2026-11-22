# Localhost certificate

The desk is served at `https://localhost:5001/`. Windows needs a certificate for that name, and the browser needs the public certificate in a trusted root store.

## Installed desk

`HotelDruid.cmd Install` does this while it has administrator approval.

- It creates a certificate named `HotelDruid LocalMachine Localhost` for `localhost` when that computer does not already have one.
- The private key stays in `LocalMachine\My`.
- The public certificate is added to `LocalMachine\Root`, the machine trusted root store.
- The service account is allowed to read the private key.
- The desk uses that certificate. Removing an install that created the certificate removes it from both stores.

## Other computers

After the desk is installed, the network step is `C:\ProgramData\HotelDruid\app\desk-certificate.ps1`. The steps are in [deploy/README.md](../deploy/README.md).

## Development copy

`./deploy/build/deploy.ps1` creates a localhost certificate for the current user and trusts it in that user's root store.

If Chromium still says the address is not secure, run this once from an administrator PowerShell:

```powershell
./deploy/build/trust-dev-cert.ps1
```

That copies the public certificate into `LocalMachine\Root`. Restart the browser after it finishes.
