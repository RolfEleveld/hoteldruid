# Deploy

Everything used to put HotelDruid on a computer is in this folder.

| Folder | What it is |
| --- | --- |
| [build](build/README.md) | Scripts that build the package, install a development copy, run tests, and refresh the manual. |
| [container](container/README.md) | The hotel image, and the plain, Keycloak, proxy, and public scenarios. |
| [winget](winget/HotelDruid.HotelDruid.yaml) | The Windows Package Manager manifest. The GitHub build fills the zip hash. |

A desk computer does not need this folder. The install command is in the root [README](../README.md). That install opens `https://localhost:5001/` on the same computer.

## Other computers on the same network

This is the step after a single computer using localhost. A few other computers can open the desk by the desk computer's own name. There is no domain and no central certificate service.

The install, including the one-command install, copies this script to the desk computer:

`C:\ProgramData\HotelDruid\app\desk-certificate.ps1`

On the desk computer, after HotelDruid is installed, run this in an administrator PowerShell:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File "$env:ProgramData\HotelDruid\app\desk-certificate.ps1"
```

The script reads this computer's name. It issues a certificate for that name, trusts it on this computer, and writes the public certificate on the desktop. It also lets other computers reach the desk. From then on, the desktop icon on this computer opens `https://` plus the computer's name. `https://localhost:5001/` still opens the same desk.

Copy `C:\ProgramData\HotelDruid\app\desk-certificate.ps1` and the public certificate file from the desktop into one folder on each other computer. Run the same command there, with the path of that copied script. The script finds the certificate file beside itself and adds it to that computer's trusted list. Open the address the desk computer printed. The browser accepts it.

The other computers find the desk by its Windows name. A name typed into the hosts file on the desk computer is not seen by them. Use the computer's own name.
