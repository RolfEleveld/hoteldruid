# Backup and logs

The nightly backup is one API call. A scheduled task asks the desk for an export and saves the zip. The hotel process does not mount OneDrive or a network share. The task runs as the person who set it up, and only while that person is signed in, so their OneDrive folder or a mapped drive is available.

## Schedule the export

During install, or any time afterwards:

```powershell
.\HotelDruid.cmd Install -BackupFolder "$env:OneDrive\HotelDruid-backups" -BackupAt 02:00
```

`-BackupFolder` is the directory that receives `hoteldruid-yyyyMMdd-HHmmss.zip`. `-BackupAt` is the local time, `HH:mm`. `-BackupIntervalDays` defaults to 1. `-BackupKeepDays` defaults to 30 and applies to the zips and to the daily log files.

To change the time, the folder, or how often it runs, run the same command again with the new values. To take one export without waiting for the clock:

```powershell
.\HotelDruid.cmd Backup
```

That uses the folder already saved in `C:\ProgramData\HotelDruid\backup.json`. Passing `-BackupFolder` again replaces the schedule and then takes an export immediately.

The task name is `HotelDruid Backup`. Uninstall removes it.

What the task runs is `Backup-HotelDruid.ps1`. That script calls `POST /api/export/create` on `https://localhost:5001`, waits until the export is completed, and downloads `/api/export/{id}/download`. The service account writes the working zip under `C:\ProgramData\HotelDruid\exports`. The task then copies it into the backup folder. A missed night, because nobody was signed in, runs when that person next signs in.

## Logs

The desk writes one application file per calendar day:

`C:\ProgramData\HotelDruid\logs\api-yyyyMMdd.log`

A process that stays up past midnight continues in the new day's file. `HOTELDRUID_LOGDIR` selects that directory. The service command sets it.

`api-startup.log` in the same directory is the process console. It receives lines written before the daily logger is attached, including a failure to start. It is not split by day.

`backup.log` is one line for each export attempt, success or failure.

The nightly export deletes `api-*.log` and `hoteldruid-*.zip` older than `-BackupKeepDays`. It leaves `api-startup.log` and `backup.log`. Delete `api-startup.log` only while the desk is stopped. `backup.log` can be deleted at any time; the next export starts it again.

The container writes the same daily files when `HOTELDRUID_LOGDIR` is set. The compose file does not set it, so a container keeps the ordinary console log of the runtime.
