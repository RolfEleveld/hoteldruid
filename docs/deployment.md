# Deploy HotelDruid

HotelDruid runs as one program on a Windows desk, or as one container. Both keep the hotel in a single data folder. The first visit asks for the hotel name, the currency, and the chart of accounts. After that, people open the desk in a browser.

The Windows package is `HotelDruid-desktop.zip`. It does not contain the .NET runtime. On a computer that does not already have ASP.NET Core 10, install downloads it. That is about 29 MB for the host and 11 MB for the ASP.NET runtime. The screenshot walkthrough of a clean Windows install and a demo load is [HotelDruid on a desk computer](admin/desk-computer.md).

## Windows desk

Unzip `HotelDruid-desktop.zip` into one folder. `HotelDruid.cmd` and `HotelDruid.Api.dll` sit next to each other.

Open PowerShell as administrator, move into that folder, and install:

```powershell
cd C:\Users\You\Downloads\HotelDruid
.\HotelDruid.cmd Install
```

Approve the administrator prompt. Install does five things:

1. If ASP.NET Core 10 is missing, it installs `Microsoft.DotNet.Runtime.10` and `Microsoft.DotNet.AspNetCore.10` with Windows Package Manager. If that catalog does not answer, it downloads the two installers from Microsoft.
2. It copies the hotel to `C:\ProgramData\HotelDruid\app`.
3. It keeps the hotel data in `C:\ProgramData\HotelDruid\data` and the log in `C:\ProgramData\HotelDruid\logs`. The service account can write only those two folders.
4. It registers a startup task named HotelDruid. The task runs as Local Service at every boot.
5. It puts HotelDruid on the public desktop and in the Start menu, then starts the desk.

The window ends with `HotelDruid is installed and running at https://localhost:5001/`. Close that window. The task keeps the hotel running. There is no command window to leave open.

Employees open the desktop icon, or a bookmark to `https://localhost:5001/`. If the desk is already running, the icon only opens the browser. A new data folder asks for the system setup once, then for the name that this browser writes on ledger lines.

### Load a demo hotel

`Load-DemoHotel.ps1` fills the desk with Hotel Belvedere so someone can click through arrivals, stays, departures, and history. From an administrator PowerShell in the unzipped folder, after Install:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\Load-DemoHotel.ps1
```

The script reads this computer's clock. History starts five years before today and reservations run until six months after today. The hotel has 20 rooms and about 67 percent occupancy. The same seed produces the same pattern. Stays that left on or before today are checked out, stays that began before today and are still open are checked in, and arrivals today or later stay confirmed.

It saves the current hotel first, under `%TEMP%\HotelDruid-demo-backup-<time>\`. The last line of the window is that path. Then it stops the service, replaces `C:\ProgramData\HotelDruid\data`, and starts the service again. On the sandbox run of 15 November 2026 the file write itself took 3.1 seconds and the reloaded desk showed Hotel Belvedere, with arrivals and departures on that day.

Reload the browser after the script finishes. The page that was already open still shows the previous hotel until it is reloaded.

### Remove the desk

```powershell
.\HotelDruid.cmd Uninstall
```

Uninstall writes an export into the current user's Temp folder and then removes the program, the startup task, and the desktop icon.

## Docker

The container build uses the repository. From the folder that contains `docker-compose.yml`:

```powershell
docker compose up --build
```

Open `http://localhost:8080/`. The hotel data is the Docker volume `HotelDruid_data`, mounted at `/data` inside the container. The container listens on port 8080. Stopping the stack leaves that volume in place:

```powershell
docker compose down
```

`docker compose down -v` deletes the volume and the hotel with it.

The image runs as the container user `app`. That user can write `/data`. A health check calls `http://localhost:8080/health`.

Keycloak, a reverse proxy, and a public certificate are a separate step. The scenario scripts and the compose overlays for that are in [Deployment scenarios](../deploy/scenarios/README.md) and the [deployment runbook](deployment-runbook.md).
