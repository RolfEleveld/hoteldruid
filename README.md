# HotelDroid - Containerized Windows Environment

HotelDroid is a Docker-based solution that provides a virtualized Windows environment for hotel management operations. This system leverages the powerful [dockur/windows](https://github.com/dockur/windows) container to deliver a full Windows desktop experience accessible via web browser or RDP connection.

## Features ✨

- **Full Windows 10 LTSC Environment**: Pre-configured with 25GB disk space and 2GB RAM
- **Web-based Access**: Connect through your browser at `http://localhost:8006`
- **RDP Support**: Traditional Remote Desktop access via port 3389
- **Automated Setup**: One-click deployment with the included PowerShell script
- **Persistent Storage**: Data persistence through Docker volumes
- **Spanish Localization**: Configured for Spanish region with US keyboard layout

## System Requirements

- Docker Desktop with Windows containers support
- At least 4GB available RAM (2GB allocated to container)
- 30GB free disk space (25GB for Windows + overhead)
- Windows 10/11 host system
- Virtualization enabled in BIOS

## Quick Start

### Starting the System

Run the automated startup script:

```powershell
.\start.ps1
```

This script will:

1. Build and start the Windows container using Docker Compose
2. Automatically configure the environment based on settings in `.env`
3. Run for up to 8 hours before auto-shutdown
4. Provide connection information once ready

### Accessing the Windows Environment

Once the container is running, you can access it in two ways:

#### Option 1: Web Browser (Recommended for setup)

- Navigate to: `http://localhost:8006`
- Use for initial configuration and light usage

#### Option 2: Remote Desktop (Recommended for regular use)

- Connect to: `localhost:3389`
- Username: `rolf`
- Password: `Pw12345!`
- Use any RDP client (built-in Windows RDP, Microsoft Remote Desktop app, etc.)

### Stopping the System

The system will automatically shut down after 8 hours, or you can manually stop it:

```powershell
docker-compose --file .\Docker.Windows.Container.compose.yml -p windows down
```

## Configuration

System configuration is managed through the `.env` file:

| Variable | Current Value | Description |
|----------|---------------|-------------|
| `Windows_Version` | `10l` | Windows 10 LTSC |
| `Disk_Size` | `25GB` | Virtual disk size |
| `Ram_Size` | `2GB` | Allocated memory |
| `Cpu_Cores` | `2` | CPU cores assigned |
| `Username` | `rolf` | Windows username |
| `Password` | `Pw12345!` | Windows password |
| `Region` | `es-es` | Spanish region |
| `Keyboard` | `US` | US keyboard layout |

To modify these settings, edit the `.env` file before starting the container.

## Troubleshooting

**Container won't start:**

- Ensure Docker Desktop is running
- Verify Windows containers are enabled in Docker Desktop
- Check that virtualization is enabled in BIOS

**Poor performance:**

- Increase `Ram_Size` in `.env` if your system has more available memory
- Consider upgrading to `Cpu_Cores=4` if you have a multi-core processor

**Connection issues:**

- Wait a few minutes after startup for Windows to fully initialize
- Check that ports 3389 and 8006 are not blocked by firewall
- Try refreshing the web browser or reconnecting RDP client

## File Structure

```text
hotelDroid/
├── start.ps1                              # Main startup script
├── Docker.Windows.Container.compose.yml   # Docker Compose configuration
├── .env                                   # Environment variables
├── windows/                               # Persistent storage volume
└── README.md                              # This file
```

## Data Persistence

All Windows data is stored in the `./windows` directory and persists between container restarts. This includes:

- User files and documents
- Installed applications
- Windows settings and configurations
- Registry changes

## Security Notes

⚠️ **Important Security Considerations:**

- Default credentials are included in this repository for development purposes
- Change the default password in `.env` before production use
- Ensure proper network security when exposing RDP ports
- Consider using VPN access for remote connections

## Support

For issues specific to the Windows container engine, refer to the [dockur/windows documentation](https://github.com/dockur/windows).

For HotelDroid-specific issues, please check the troubleshooting section above or contact the development team.

## Hotel Droid deployment

Deploy ```https://www.easyphp.org/save-easyphp-devserver-latest.php```
- Deploy ```vcredist_x86.exe``` from ```https://www.microsoft.com/en-us/download/details.aspx?id=30679``` for the MSVCR110.dll
Then copy the contents of the hoteldruid*.zip into ```C:\Program Files (x86)\EasyPHP-Devserver-17\data\localweb```
