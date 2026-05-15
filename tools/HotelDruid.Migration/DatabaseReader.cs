namespace HotelDruid.Migration;

/// <summary>
/// Reads all HotelDruid tables from a database.
/// Inspects the actual schema before reading — missing columns in older versions
/// are silently skipped (returned as null in the row dictionary).
/// </summary>
public abstract class DatabaseReader : IDisposable
{
    protected readonly MigrationOptions Options;

    protected DatabaseReader(MigrationOptions options) => Options = options;

    public abstract Task OpenAsync();
    public abstract Task<List<string>> GetExistingTablesAsync();
    public abstract Task<List<string>> GetColumnsAsync(string tableName);
    public abstract Task<List<Dictionary<string, object?>>> ReadTableAsync(string tableName, List<string> columns);
    public abstract void Dispose();

    public static DatabaseReader Create(MigrationOptions options) => options.DbType.ToLower() switch
    {
        "sqlite" => new SqliteReader(options),
        "mysql"  => new MySqlReader(options),
        _        => throw new ArgumentException($"Unknown db-type '{options.DbType}'. Use 'mysql' or 'sqlite'.")
    };
}

