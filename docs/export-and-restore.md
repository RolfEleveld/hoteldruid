# Export and restore

The installed desk can take a full export of the hotel that is running and repeat it on a schedule. The file is a zip. Another desk imports that zip into its own data folder. A second desk is the place to open a backup and decide what to carry back.

This is the desk's own package. A PHP MySQL or SQLite database is a different load, in [Import a PHP hotel](import-from-php.md).

## Where the zip is written

An export writes one zip on the desk that created it:

`C:\ProgramData\HotelDruid\exports\export_HotelDruid_yyyyMMdd_HHmmss_<eight characters>_v1.zip`

That folder is `Storage__ExportDirectory` in the installed service command. A copy-folder hotel does not set that name, so the API writes `artifacts\exports` under the process's current directory unless `Storage__ExportDirectory` is set.

There is no default folder for the scheduled copy. The first schedule has to name one. Until then, the zip above is the only export, and only when something calls the export API. Uninstall writes one more zip, `hoteldruid-export.zip`, under `%TEMP%\HotelDruid-uninstall-<stamp>\`.

## Schedule

On the installed desk:

```powershell
.\HotelDruid.cmd Install -BackupFolder "$env:OneDrive\HotelDruid-backups" -BackupAt 02:00
```

This registers the task `HotelDruid Backup`. At 02:00 every day it calls the export API at `https://localhost:5001/`, downloads the zip, and saves `hoteldruid-yyyyMMdd-HHmmss.zip` in the folder you named. Copies in that folder older than 30 days are removed. The task runs as the person who set it, and only while that person is signed in, so the folder can be OneDrive.

Run the same command again to change the schedule. The settings are:

| Setting | Default | What it changes |
| --- | --- | --- |
| `-BackupFolder` | none | Where `hoteldruid-yyyyMMdd-HHmmss.zip` is saved |
| `-BackupAt` | `02:00` | Time of day, as `HH:mm` |
| `-BackupIntervalDays` | `1` | Days between runs, from 1 to 30 |
| `-BackupKeepDays` | `30` | How long those scheduled zips are kept |

`.\HotelDruid.cmd Backup` writes one zip now, into the folder already saved in `C:\ProgramData\HotelDruid\backup.json`. The log is `C:\ProgramData\HotelDruid\logs\backup.log`.

`Backup-HotelDruid.ps1 -Destination <folder> -BaseUrl <desk>` is the same download for a desk that is not `https://localhost:5001/`. A copy folder or a container does not register the task. Put that command on a schedule if that hotel should export nightly.

## What is inside the zip

The scheduled file and the file under `exports` are the same package. A full export contains:

| Path in the zip | What it holds |
| --- | --- |
| `manifest.json` | When it was made, the HotelDruid version, and the source computer's name as a label |
| `data/tables/*.json` | The hotel tables: rooms, bookings, guests, tariffs, the ledger, and the rest of a full export |
| `schemas/tables/*.json` | The columns of those tables |
| `configs/export_metadata.json` | Table count, row count, and the options used |
| `configs/naming_info.json` | The table names in the package |
| `docs/EXPORT_INFO.txt` | A short description of the package |
| `docs/IMPORT_GUIDE.txt` | The import steps |

The computer name in the manifest is not a path. The other desk does not read the source computer's files.

## Restore onto a second desk

Start a second hotel with its own data folder and its own port, as in [Several hotels](several-hotels.md). Import the zip there. The live desk is left as it is, and the second desk is the one you look at.

On the second desk, `http://127.0.0.1:8081/` in this example:

1. `POST /api/import/validate` with the zip as the form file. The response includes `packageId` and the tables.
2. `GET /api/import/{packageId}/preview` lists each table, its row count, and its columns. Nothing is written.
3. `POST /api/import/{packageId}/execute` with `{ "packageId": "<packageId>", "dryRun": true }` counts the rows and still writes nothing.
4. The same execute with `"dryRun": false` writes every table in the zip into that desk's data folder.

Validate and execute have to reach the same running desk. It holds the checked package in memory. A restart means validating the zip again.

A repeat of the same zip writes the same rows again. The export names a room by its name, a client by name, a booking by client, room, and dates, and a charge or a guest by the booking it belongs to. Import updates the row with that name. It does not delete a row that the zip no longer contains. Changing the name, or the booking's client, room, or dates, inserts a new row and leaves the old one.

A ledger day that is in the zip replaces that whole day. A line added on the desk for that day after the export is not kept. Days absent from the zip stay. House settings are one record, and the zip replaces it.

## Carry back only some tables

Execute loads every table file that is in the zip. It has no list of tables to skip. To carry a subset onto the live desk, copy the zip and remove the `data/tables/<table>.json` files you do not want. Leave `manifest.json`. Validate and execute that copy on the live desk. The tables still in the zip are written. The tables you removed are not. The second desk, with the full zip imported, is where you decide which files those are.
