# HotelDruid

HotelDruid 4.10.2 is updated work from the original HotelDruid. The desk is Blazor. The hotel is an ASP.NET program. The people at the desk use a browser. The hotel itself is a set of files on that computer.

## Description

The desk is a Blazor WebAssembly client. The program behind it is an ASP.NET Core API. Both are .NET. The API keeps the rooms, the stays, the guests, and the ledger in a file store. A desk install and a container run that same API. The client is served from it.

## Where it came from

HotelDruid was written by Marco Maria Francesco De Santis (`marco@digitaldruid.net`). The original notices run from 2001. Version 3.0.7 is still on the `main` branch, in `hoteldruid/`. His own line for it, in `hoteldruid/doc/README.italiano`, is a program for hotel rooms and for weekly and daily apartment lets. The pages are in Italian, English, and Spanish, and the program is published at [www.hoteldruid.com](http://www.hoteldruid.com). It keeps the hotel in PostgreSQL, MySQL, or SQLite.

This solution is that program, now at version 4.10.2. Version 4.10.1 was the first release of this line on .NET 10. The desk runs in Blazor. The hotel runs in ASP.NET, and it is stored as files. Version 4.10.2 adds the self-contained Windows and Linux copies beside the desk install.

The original thanks Stan Khodjaev of icojam.com for the Marmalade, Blue Bits, and Blueberry icons, which he released into the public domain.

## License

Thank you, Marco Maria Francesco De Santis, for HotelDruid, and for releasing it under the GNU Affero General Public License, version 3.

This is updated work from that program. On 22 November 2026 I published version 4.10.1, the first release of this line on .NET 10. Version 4.10.2 is that program with the desk install, the Windows copy folder, and the Linux copy folder. The desk runs in Blazor and the hotel runs in ASP.NET. I comply with that license, and I distribute this solution with it. The terms are in [LICENSE](LICENSE), and they cover this program as they cover the original. A later version applies only when you accept it, as the original program says.

## How to deploy

### Desktop

On the computer that will run the hotel, in PowerShell:

```powershell
iex (irm https://github.com/RolfEleveld/hoteldruid/releases/download/blazor-latest/install.ps1)
```

Approve the administrator prompt. The command downloads one zip, installs the ASP.NET Core 10 runtime when it is missing, copies the hotel to `C:\ProgramData\HotelDruid`, and starts it at every boot. The desktop icon opens the desk.

Windows leaves PowerShell script files disabled. The install command is downloaded text, so that setting does not block the command itself, and the installer starts the hotel script with a bypass for that run. If the window says `HotelDruid.ps1` cannot be loaded because running scripts is disabled, run this in the same window and then run the install command again:

```powershell
Set-ExecutionPolicy -Scope Process Bypass -Force
```

That change lasts only for the open window.

The install creates a certificate for `localhost`. The private key stays in the machine certificate store. The public certificate is added to the machine's trusted root store. The desk address is `https://localhost:5001/`.

A computer with no network unzips `HotelDruid-desktop.zip` and runs `HotelDruid.cmd`.

### Copy folder

The same release also carries two copy folders. The runtime is already inside each one. Nothing else is installed. The desk listens at `http://127.0.0.1:8080/`. The hotel is the `data` folder next to the start command.

- `HotelDruid-selfcontained-win-x64.zip`. Unzip it and run `Start-HotelDruid.cmd`. Close the window to stop it.
- `HotelDruid-selfcontained-linux-x64.zip`. This is for glibc Linux (Ubuntu, Debian, Fedora, or RHEL). Unzip it and run `./start-hoteldruid.sh`.

Build both from the repository:

```powershell
./deploy/build/package-selfcontained.ps1
```

The folders are `artifacts/selfcontained/win-x64` and `artifacts/selfcontained/linux-x64`. `build.ps1` is unchanged and still produces the smaller package that the install command downloads.

Backup, removal, and the first setup are in [Deploy](docs/manual/chapters/01-deploy.md).

### Container

The GitHub build publishes one hotel image, `ghcr.io/<owner>/hoteldruid:blazor`. Each scenario runs that image. The commands are in [deploy/container](deploy/container/README.md).

Plain, on this computer, built from the repository:

```powershell
docker compose -f deploy/container/compose/plain.yml up --build
```

Open `http://localhost:8080/`. The hotel is the `/data` mount. `HOTELDRUID_PORT` changes the host port. `HOTELDRUID_DATA` points `/data` at a folder.

Keycloak adds sign-in. The reverse proxy publishes ports 80 and 443. The public stack is both of those together. A certificate and the sign-in finish are in the [deployment runbook](docs/deployment-runbook.md).

## How to get started

Build a development copy and start it:

```powershell
./deploy/build/build.ps1
./deploy/build/deploy.ps1
```

The desk listens at `https://localhost:5001/`. The command creates a localhost certificate for the current user and trusts it. If the browser still says the address is not secure, run `./deploy/build/trust-dev-cert.ps1` once from an administrator PowerShell. That copies the public certificate into the machine's trusted root store. The first page asks for the hotel name, the currency, and the chart of accounts.

## How to set up the demo

On an installed desk, in an administrator PowerShell, run `Load-DemoHotel.ps1` from the installed folder. It is also in [deploy/build/desktop](deploy/build/desktop/Load-DemoHotel.ps1). The current hotel is exported first. The last line of the window is that path. Reload the browser. The demo is Hotel Belvedere. The details are in [Demo hotel](docs/manual/chapters/02-demo.md).

## Manual

The picture guide for the desk is [docs/manual](docs/manual/README.md). Open [HotelDruid.html](docs/manual/HotelDruid.html). Refresh it with `./deploy/build/manual.ps1`.

## Source tree

| Path | What it is |
| --- | --- |
| [src](src/README.md) | The client, the API, and the shared contracts. |
| [deploy](deploy/README.md) | Build scripts, the container, and the winget manifest. |
| [docs](docs/README.md) | The hotel manual and the notes for people who host the program. |
| [tests](tests) | API, client, and migration tests. Run them with `./deploy/build/test.ps1`. |
| [tools/HotelDruid.Migration](tools/HotelDruid.Migration/README.md) | Import from a PHP HotelDruid export. |

## CI artefacts

A push or a pull request to `blazor` runs [the build](.github/workflows/build-deployment-package.yml), which calls `./deploy/build/build.ps1`.

A push also updates the `blazor-latest` release:

- `install.ps1`, the desktop command above, with the repository name filled in
- `HotelDruid-desktop.zip`, the package that command downloads
- `HotelDruid-selfcontained-win-x64.zip`, the Windows copy folder. Unzip it and run `Start-HotelDruid.cmd`
- `HotelDruid-selfcontained-linux-x64.zip`, the Linux copy folder. Unzip it and run `./start-hoteldruid.sh`
- `HotelDruid.HotelDruid.yaml`, the winget manifest with the desktop zip hash filled in
- the two SBOM files

`winget install HotelDruid.HotelDruid` works after that manifest is accepted into the Windows Package Manager community repository. The source is [deploy/winget/HotelDruid.HotelDruid.yaml](deploy/winget/HotelDruid.HotelDruid.yaml).

The GitHub Actions run also keeps these artifacts for 14 days.

- `hoteldruid-deployment-package-<run>` is the development copy. Start it with `deploy.ps1`. Parameters are in [deploy/build](deploy/build/README.md).
- `hoteldruid-selfcontained-win-x64-<run>` is the Windows copy folder. Unzip it and run `Start-HotelDruid.cmd`.
- `hoteldruid-selfcontained-linux-x64-<run>` is the Linux copy folder. Unzip it and run `./start-hoteldruid.sh`.
- `hoteldruid-container-<run>` is the hotel image as a file. `docker load -i hoteldruid-container.tar` loads it. The same image is published to `ghcr.io/<owner>/hoteldruid:blazor` on a push, which is the address a host uses. The scenarios that add Keycloak or the proxy are in [deploy/container](deploy/container/README.md).
