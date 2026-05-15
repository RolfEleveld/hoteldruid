namespace HotelDruid.Migration;

public class MigrationOptions
{
    public string DbType { get; set; } = "mysql";          // "mysql" or "sqlite"
    public string ConnectionString { get; set; } = "";
    public string TablePrefix { get; set; } = "";           // e.g. "hd_" if tables are prefixed
    public string Mode { get; set; } = "export";            // "export" or "import"
    public string OutputFile { get; set; } = "migration.zip";
    public string ApiUrl { get; set; } = "";
    public bool DryRun { get; set; } = false;
    public bool Verbose { get; set; } = false;
}

