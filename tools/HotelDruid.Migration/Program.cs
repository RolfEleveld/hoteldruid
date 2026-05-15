using HotelDruid.Migration;

// ── Parse arguments ──────────────────────────────────────────────────────────
var opts = new MigrationOptions();
var args_list = args.ToList();

string? GetArg(string name)
{
    var i = args_list.IndexOf(name);
    return i >= 0 && i + 1 < args_list.Count ? args_list[i + 1] : null;
}
bool HasFlag(string name) => args_list.Contains(name);

if (HasFlag("--help") || HasFlag("-h") || args_list.Count == 0)
{
    Console.WriteLine("""
        HotelDruid-migrate — Legacy HotelDruid database migration tool

        Usage:
          HotelDruid-migrate --db-type mysql --connection "Server=...;Database=...;User=...;Password=..." [options]
          HotelDruid-migrate --db-type sqlite --connection "Data Source=/path/to/HotelDruid.db" [options]

        Options:
          --db-type       mysql | sqlite                      (required)
          --connection    ADO.NET connection string           (required)
          --prefix        table prefix, e.g. "hd_"           (default: "")
          --mode          export | import                     (default: export)
          --output        output zip filename                 (default: migration.zip)
          --api-url       base URL of HotelDruid API          (required for import mode)
          --dry-run       map tables without reading data
          --verbose       show detailed output
          --help          show this help

        Examples:
          # Export from MySQL to zip:
          HotelDruid-migrate --db-type mysql --connection "Server=localhost;Database=HotelDruid;User=root;Password=secret" --output my-export.zip

          # Import from SQLite directly to API:
          HotelDruid-migrate --db-type sqlite --connection "Data Source=HotelDruid.db" --mode import --api-url https://my-server/
        """);
    return 0;
}

opts.DbType           = GetArg("--db-type") ?? opts.DbType;
opts.ConnectionString = GetArg("--connection") ?? opts.ConnectionString;
opts.TablePrefix      = GetArg("--prefix") ?? opts.TablePrefix;
opts.Mode             = GetArg("--mode") ?? opts.Mode;
opts.OutputFile       = GetArg("--output") ?? opts.OutputFile;
opts.ApiUrl           = GetArg("--api-url") ?? opts.ApiUrl;
opts.DryRun           = HasFlag("--dry-run");
opts.Verbose          = HasFlag("--verbose");

if (string.IsNullOrEmpty(opts.ConnectionString))
{
    Console.Error.WriteLine("Error: --connection is required. Use --help for usage.");
    return 1;
}

// ── Run migration ─────────────────────────────────────────────────────────────
Console.WriteLine("HotelDruid Legacy Migration Tool");
Console.WriteLine($"  DB type : {opts.DbType}");
Console.WriteLine($"  Mode    : {opts.Mode}{(opts.DryRun ? " (dry-run)" : "")}");
if (opts.Mode == "export") Console.WriteLine($"  Output  : {opts.OutputFile}");
if (opts.Mode == "import") Console.WriteLine($"  API URL : {opts.ApiUrl}");
Console.WriteLine();

try
{
    var runner = new MigrationRunner(opts);
    var report = await runner.RunAsync();
    report.Print();
    return 0;
}
catch (Exception ex)
{
    Console.Error.WriteLine($"\nFatal error: {ex.Message}");
    if (opts.Verbose) Console.Error.WriteLine(ex.StackTrace);
    return 2;
}

