# Deploy

A new computer gets one program and one data folder. Employees open the desktop icon, or a bookmark.

## Windows desk

1. Unzip `HotelDruid-desktop.zip`. `HotelDruid.cmd` sits next to `HotelDruid.Api.dll`.
2. In an administrator PowerShell, in that folder: `.\HotelDruid.cmd Install`
3. Install adds the ASP.NET Core 10 runtime when it is missing, copies the program to `C:\ProgramData\HotelDruid\app`, keeps the hotel in `data` and the log in `logs`, and starts it at every boot.
4. Open the desktop icon. The first page is [House settings](03-house.md).

The address is `https://localhost:5001/`.

## Nightly copy

```powershell
.\HotelDruid.cmd Install -BackupFolder "$env:OneDrive\HotelDruid-backups" -BackupAt 02:00
```

Run the same command again to change the folder or the time. `.\HotelDruid.cmd Backup` takes one copy now.

The task runs as the person who set it, and only while that person is signed in, so the folder can be OneDrive. Without this, the only copy is the data folder on that computer.

Logs are `C:\ProgramData\HotelDruid\logs\api-yyyyMMdd.log`. Copies and daily logs older than 30 days are removed. `api-startup.log` is the start-up console and is kept.

## Remove

`.\HotelDruid.cmd Uninstall` writes an export into the current user's Temp folder, then removes the program, the task, and the icon.

## Container

From the folder that contains `docker-compose.yml`: `docker compose up --build`. Open `http://localhost:8080/`. The hotel is the volume `HotelDruid_data`. `docker compose down -v` deletes that volume.

A public address, a certificate, and sign-in are a separate step. See the [deployment runbook](../../deployment-runbook.md) and [Keycloak](../../keycloak-setup.md).

Next: [Demo hotel](02-demo.md).
