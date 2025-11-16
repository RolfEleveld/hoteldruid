# HotelDruid PHP Desktop - Quick Start

## Setup (One-Time)

```powershell
# Run from the project root directory (hotelDroid/)
.\setup-phpdesktop.ps1
```

This will:

- Download PHP Desktop (~100MB)
- Configure it for HotelDruid
- Set up the router script
- Verify PHP extensions

## Run HotelDruid

```powershell
.\start-hoteldruid-desktop.ps1
```

Or double-click: `phpdesktop\phpdesktop-chrome.exe`

## First Time Setup

1. Application opens in a window
2. If no database exists, you'll see the setup wizard
3. Choose SQLite (default if available)
4. Configure your hotel/rooms
5. Start using HotelDruid!

## Troubleshooting

**"PHP Desktop not found"**
→ Run `.\setup-phpdesktop.ps1` again

**"Port 8080 in use"**
→ Edit `phpdesktop/settings.json`, change port number

**Database errors**
→ Check that `hoteldruid/dati/` directory is writable

## Files Created

- `phpdesktop/` - PHP Desktop application
- `hoteldruid/server.php` - Router script
- `phpdesktop/settings.json` - Configuration

## Next Steps

See `README-PHPDESKTOP.md` for detailed documentation.
