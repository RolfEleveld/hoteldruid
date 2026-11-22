# Deploy

A new computer gets one program and one data folder. Employees open the desktop icon, or a bookmark.

## Windows desk

On the desk computer, in PowerShell, run the install command from the HotelDruid release. The repository name is filled in on that release:

```powershell
iex (irm https://github.com/<owner>/hotelDruid/releases/download/blazor-latest/install.ps1)
```

Approve the administrator prompt. That downloads `HotelDruid-desktop.zip`, installs the ASP.NET Core 10 runtime when it is missing, copies the program to `C:\ProgramData\HotelDruid\app`, keeps the hotel in `data` and the log in `logs`, and starts it at every boot. Open the desktop icon. The first page is [House settings](03-house.md).

The same zip can be copied onto a computer that has no internet. Unzip it and run `HotelDruid.cmd`.

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
