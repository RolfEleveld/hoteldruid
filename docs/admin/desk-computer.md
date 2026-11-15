# HotelDruid on a desk computer

This is the administrator walkthrough for putting HotelDruid on one Windows computer, checked in a Windows Sandbox on 15 November 2026.

The supported check is one command on the computer that has the built hotel:

```powershell
pwsh -NoProfile -File .\scripts\desktop\Start-Sandbox.ps1
```

Close any sandbox that is already open first. The command starts a new sandbox and maps the built hotel and the desktop scripts into it. Inside, one script installs Windows Package Manager, PowerShell 7, the .NET 10 host, and the ASP.NET Core 10 runtime, then installs HotelDruid when `C:\ProgramData\HotelDruid\install-meta.json` is absent. When that file is already there, the script only opens the desk. The setup log is `Desktop\HotelDruid-setup.log` inside the sandbox. The only click is the administrator prompt.

The sections below record the first sandbox, which had no mapped folder. Pasted commands failed there, and the ASP.NET Core installer alone left `C:\Program Files\dotnet` without `dotnet.exe`. That is why the script installs `Microsoft.DotNet.Runtime.10` and then `Microsoft.DotNet.AspNetCore.10`.

The package is `HotelDruid-desktop.zip`, 66.4 MB (117 MB once unpacked). It does not contain the .NET runtime. On a computer that does not already have ASP.NET Core 10, `HotelDruid.cmd Install` asks Windows Package Manager for `Microsoft.DotNet.Runtime.10` and `Microsoft.DotNet.AspNetCore.10`, and downloads those installers from Microsoft if the catalog does not answer. That adds about 29 MB for the host and 11 MB for the ASP.NET runtime.

An administrator unzips the package and runs `HotelDruid.cmd Install` once. That is the deployment. It keeps the hotel in `C:\ProgramData\HotelDruid`, allows the service account to read the program and to write only `data` and `logs`, and registers a startup task as Local Service. The public desktop and Start menu get HotelDruid. Employees open that icon, or a bookmark to `https://localhost:5001/`. If the desk is already running, the icon only opens the browser. Section 9 is that install, done from a flat folder in Downloads on a sandbox that already had .NET 10. The earlier sections are the first sandbox, where the startup task did not launch and the hotel was started by hand.

## 1. A clean computer

A new Windows Sandbox. The desktop has the Recycle Bin, Microsoft Edge, and the wallpaper note. There is no HotelDruid icon.

![A new Windows Sandbox desktop](images/01-clean-sandbox.jpg)

## 2. No runtime yet

The sandbox user is `WDAGUtilityAccount`, an administrator, on Windows 10.0.26200. `dotnet` is not a command. The desk package does not carry the runtime, so this computer cannot start the hotel until the ASP.NET Core 10 runtime is installed.

![dotnet is not recognized](images/02-no-dotnet.jpg)

## 3. No package installer either

`winget` is not a command on this sandbox. The runtime is installed from the ASP.NET Core 10 installer published by Microsoft. The short address `https://aka.ms/dotnet/10.0/aspnetcore-runtime-win-x64.exe` currently serves `aspnetcore-runtime-10.0.15-win-x64.exe`, about 11 MB. The .NET runtime alone is not enough. The hotel needs the ASP.NET Core runtime.

![winget is not recognized](images/03-no-winget.jpg)

## 4. PowerShell does not expand a Command Prompt path

PowerShell 7 is not installed. Windows PowerShell is. In that window `%USERPROFILE%` stays as those characters, so curl looks for a folder with that name and reports `Failed to open the file` and `curl: (23)`. The desktop path in PowerShell is `$env:USERPROFILE\Desktop`. `curl` is also a PowerShell alias, so the program to run is `curl.exe`.

![curl could not open the percent-style path](images/04-curl-userprofile.jpg)

## 5. The runtime installer

Pasting both lines at once in Windows PowerShell put the `&` inside the download path, and PowerShell rejected it. Run one line, wait until curl prints `100`, then run the next line. The file lands on the desktop and the setup window opens: Microsoft ASP.NET Core Runtime 10.0.15 (x64). Install accepts the runtime license.

![The ASP.NET Core 10.0.15 setup window](images/05-runtime-setup.jpg)

## 6. Runtime installed

Install finishes with "Installation Successfully Completed". Task Manager shows the ASP.NET Core Runtime installer while it is running. A PowerShell window opened afterwards from the Start menu still says `dotnet` is not recognized.

![ASP.NET Core 10.0.15 installation completed](images/06-runtime-installed.jpg)

![A new PowerShell still cannot see dotnet](images/07-dotnet-not-on-path.jpg)

`C:\Program Files\dotnet` exists, and it contains `shared` and `swidtag` only. `dotnet.exe` is not in that folder, so the full path `C:\Program Files\dotnet\dotnet.exe` is not a command either. The ASP.NET Core installer placed the shared runtime and did not place the `dotnet` program next to it.

![Program Files\dotnet has shared and swidtag, and no dotnet.exe](images/08-dotnet-folder-without-host.jpg)

## 7. The sandbox opens with the hotel mapped in

`Start-Sandbox.ps1` shares two folders. They do not appear as desktop icons. In the sandbox they are ordinary folders on `C:`.

| On this computer | Inside the sandbox |
| --- | --- |
| `C:\Users\RolfEleveld\git\BaJoMeRo\hotelDruid\artifacts\api` | `C:\HotelDruidApp` |
| `C:\Users\RolfEleveld\git\BaJoMeRo\hotelDruid\scripts\desktop` | `C:\HotelDruidDesktop` |

The first sandbox start left `HotelDruid-setup.log` on the sandbox desktop and no setup window. The logon command had started, and its window was not kept on screen. The script now opens a window titled HotelDruid setup and waits until the desktop is up before the administrator prompt.

![Sandbox desktop with the setup log and no setup window](images/09-sandbox-log-on-desktop.jpg)

The setup log confirms both folders. `C:\HotelDruidApp` is the built hotel and `C:\HotelDruidDesktop` is the script. Registering package manager from the Windows image failed, and the winget program that was already there returned `-1978335173`, which is `APPINSTALLER_CLI_ERROR_RESTAPI_INTERNAL_ERROR`. The package catalog refused the machine-wide install. The script no longer uses that catalog for PowerShell 7 or .NET. It downloads those installers from Microsoft.

![Setup log showing the mapped folders and the winget catalog error](images/10-setup-log-winget-rest.jpg)

## 8. One run installs the machine, then the desk does not answer

The second sandbox keeps a window titled HotelDruid setup. It registers package manager from Microsoft, expands PowerShell 7.7.0 into `C:\Program Files\PowerShell\7`, and installs the .NET 10.0.15 host and the ASP.NET Core 10.0.15 runtime. The log then lists `Microsoft.NETCore.App 10.0.15` and `Microsoft.AspNetCore.App 10.0.15`, copies the hotel, creates the localhost certificate, and records the install.

The last health check is `curl: (7) Failed to connect to localhost:5001`. Task Manager at that moment has no `dotnet` process. `C:\ProgramData\HotelDruid\logs` is empty, so the service program never started. The startup task was pointed at `run-service.cmd` itself. Task Scheduler starts a program directly, and a `.cmd` file is not a program, so the task exits before the log can be created. The task now starts `cmd.exe /c` with that file.

![The setup window opens](images/11-setup-window.jpg)

![Package manager installing](images/12-package-manager.jpg)

![PowerShell 7 expanded into Program Files](images/14-powershell-expanded.jpg)

![The .NET 10 host downloading and installing](images/15-dotnet-installing.jpg)

![Install finished and localhost:5001 did not answer](images/16-installed-desk-not-answering.jpg)

![The service log folder is empty](images/17-empty-service-log.jpg)

Double-clicking `C:\ProgramData\HotelDruid\app\run-service.cmd` starts the hotel. The log then says it is listening on `http://localhost:5000` and `https://localhost:5001`, with content root `C:\ProgramData\HotelDruid\app`. Leave that command window open. Closing it stops the hotel. The startup task, once it launches the same file through `cmd.exe`, keeps the hotel running without that window.

![The hotel is listening on localhost 5000 and 5001](images/18-service-listening.jpg)

Edge opens `https://localhost:5001`. A new data folder has no system configuration, so the hotel asks for that once. After it is accepted, the desk is an empty hotel: no reservations, arrivals, stays, departures, received bookings, or clients. The calendar shows the months around the sandbox date, November 2026 through January 2027.

![An empty HotelDruid desk in the sandbox](images/19-empty-welcome.jpg)

## 9. Unzip the package and run Install

This is the deployment an administrator does on a desk. The sandbox already had the .NET 10 host and the ASP.NET Core 10 runtime, so Install did not download them. The published hotel and `HotelDruid.cmd` were copied into one flat folder, `C:\Users\WDAGUtilityAccount\Downloads\HotelDruid`. `HotelDruid.cmd` and `HotelDruid.Api.dll` sit next to each other. That is the shape of the unzipped zip.

![The unzipped package in Downloads](images/20-unzipped-downloads.jpg)

PowerShell was started from the Start menu with Run as administrator.

![Run Windows PowerShell as administrator](images/21-powershell-as-administrator.jpg)

From that window:

```powershell
cd C:\Users\WDAGUtilityAccount\Downloads\HotelDruid
.\HotelDruid.cmd Install
```

![The elevated window, changed into the unzipped folder](images/22-install-command.jpg)

Install copied the hotel to `C:\ProgramData\HotelDruid\app`, started the Local Service task, and reported the desk running. The hotel data folder is `C:\ProgramData\HotelDruid\data`. The elevated window can be closed. The task keeps the hotel running.

![Install finished and the desk is running on localhost 5001](images/23-install-running.jpg)

Edge opened `https://localhost:5001`. A new data folder has no system configuration, so the hotel asks for it once. The storage location shown is `C:\ProgramData\HotelDruid\data`.

![First-run system setup](images/24-first-setup.jpg)

The chart was set to Switzerland (Kontenrahmen KMU) and saved.

![Configuration saved](images/25-configuration-saved.jpg)

The browser profile name was set to Hotel Admin and saved. That name belongs to this browser. Each person who signs in on the computer keeps their own name.

![Profile name saved](images/26-profile-saved.jpg)

Install also placed HotelDruid on the public desktop. The shortcut points at `C:\ProgramData\HotelDruid\app\HotelDruid.cmd`. Double-clicking it opens the browser at `https://localhost:5001`. The desk was already running, so the icon did not start a second copy. The hotel is empty: no reservations, arrivals, stays, departures, received bookings, or clients. The calendar shows November 2026 through January 2027.

![The HotelDruid icon on the sandbox desktop](images/27-desktop-icon.jpg)

![The desk opened from the desktop icon](images/28-desk-from-icon.jpg)

## 10. Load the demo hotel

`Load-DemoHotel.ps1` is the demo load for a desk that is already installed. It reads this computer's clock. History starts five years before today and reservations run until six months after today. The hotel is Hotel Belvedere: 20 rooms, about 67 percent occupied, with the same seed each time. Stays that left on or before today are checked out, stays that began before today and are still open are checked in, and arrivals today or later stay confirmed.

The script exports the current hotel first. The zip goes to a new `HotelDruid-demo-backup-<time>` folder in the current user's Temp directory, and the last line of the window is that path. On this sandbox the export API returned `failed` and the hotel was left in place. The script then copies the data folder into that same Temp directory and continues. Only after that copy exists does it stop the service and replace `C:\ProgramData\HotelDruid\data`. Starting the task brings the demo desk back at `https://localhost:5001/`.

In the sandbox, from the administrator PowerShell:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File C:\HotelDruidDesktop\Load-DemoHotel.ps1
```

The generator is `C:\HotelDruidDesktop\scenario\HotelDruid.Scenario.dll`, mapped from this repository. It is not inside the 66.4 MB hotel zip. The command replaces the sandbox hotel. The hotel on the computer that hosts the sandbox is not the data root this script uses.

The run on 15 November 2026 wrote Hotel Belvedere for 2021-11-15 through 2027-05-15 in 3,058 milliseconds. That clock starts when the folder is cleared and stops when the last file is written. The result was 20 rooms, 8,836 guest stays, 725 cancellations, 133 maintenance blocks, 16,123 guests, and 29,758 occupied nights out of 43,260 nights that could be sold. Occupancy landed at about 69 percent. The zip export was refused before that write: Local Service cannot create a file under `C:\ProgramData\HotelDruid\app\artifacts\exports`. The folder copy is `C:\Users\WDAGUtilityAccount\AppData\Local\Temp\HotelDruid-demo-backup-20261115-232051\data`. The browser window that was already open still showed the empty desk. Reloading `https://localhost:5001/` opens Hotel Belvedere. The desk shows 8,967 bookings, 3 arrivals for 15 November 2026 who have not checked in, 3 occupied rooms, 2 departures due today, and 14,324 clients. Rooms 101, 103, and 104 are on the first floor of house A. Reservations still ahead include stays in May 2027.

![Demo hotel written in 3058 milliseconds](images/29-demo-written.jpg)

![Hotel Belvedere after reloading the desk](images/30-demo-desk.png)
