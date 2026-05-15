# HotelDroid Migration Tool

A standalone .NET CLI tool to migrate data from a legacy HotelDruid (PHP)
MySQL or SQLite database into the HotelDroid Blazor application.

## Modes

| Mode | Description |
|------|-------------|
| `export` (default) | Reads the legacy DB and produces a canonical `.zip` file for manual upload via the Blazor admin UI |
| `import` | Reads the legacy DB and pushes data directly to a running HotelDroid API |

## Usage

### Export to zip (for manual upload)

```bash
dotnet run --project tools/HotelDroid.Migration -- \
  --db-type mysql \
  --connection "Server=localhost;Database=hoteldruid;User=root;Password=secret" \
  --output legacy-export.zip
  ```


Then upload legacy-export.zip via the Admin → Import page in the Blazor UI.

Import directly to API  

```bash
dotnet run --project tools/HotelDroid.Migration -- \
  --db-type sqlite \
  --connection "Data Source=/path/to/hoteldruid.db" \
  --mode import \
  --api-url https://your-server/
```

***All options***  

|Option|	Default	|Description|
|-|-|-|
|--db-type|	(required)	|mysql or sqlite|
|--connection|	(required)	|ADO.NET connection string|
|--prefix|	""	|Table name prefix, e.g. hd_|
|--mode|	export	|export or import|
|--output|	migration.zip	|Output zip filename (export mode)|
|--api-url|		|Base URL of HotelDroid API (import mode)|
|--dry-run|		|Map tables without reading data|
|--verbose|		|Show detailed output|

***Notes***  

Missing columns in older HotelDruid versions are gracefully skipped — no errors.
The exported zip format matches the existing PHP Exporter and Blazor ImportService.
Tests run as part of the standard dotnet test hotelDroid.sln (no external DB needed).