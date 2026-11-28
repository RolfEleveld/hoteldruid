# Import a PHP hotel

A PHP HotelDruid on MySQL or SQLite can be read in one command and loaded into a running desk. The desk's own nightly zip is a different package, in [Export and restore](export-and-restore.md). The tool is [tools/HotelDruid.Migration](../tools/HotelDruid.Migration/README.md). It reads the known hotel tables: rooms, clients, bookings, guests, tariffs, cash, and the other tables in that map. A table an older database does not have is skipped. The PHP database is only read.

The desk that receives the hotel must already be running. The load writes into that desk's data folder. A second surname, `cognome2`, is joined onto the one surname field. There is no split back into two surname fields.

One command reads the database, builds the package, validates it, and executes the import:

```powershell
dotnet run --project tools/HotelDruid.Migration -- --db-type mysql --connection "Server=localhost;Database=HotelDruid;User=root;Password=secret" --prefix hd_ --mode import --api-url http://127.0.0.1:8080/
```

`--prefix` is the table prefix the PHP hotel uses. `hd_` is the usual one. Leave `--prefix` out when the tables are named `prenotazioni` with no prefix.

A SQLite file uses the same command with `--db-type sqlite` and `--connection "Data Source=/path/to/HotelDruid.db"`.

`--mode export` stops after writing `migration.zip`. That zip is the package the desk import accepts, so the load can be reviewed before it is applied. `--dry-run` lists the tables it would read and does not read the rows.

PostgreSQL is not a reader. A PHP hotel stored there is dumped to MySQL or SQLite first.
