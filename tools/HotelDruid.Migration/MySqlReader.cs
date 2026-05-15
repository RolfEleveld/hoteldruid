using MySqlConnector;

namespace HotelDruid.Migration;

public class MySqlReader : DatabaseReader
{
    private MySqlConnection? _conn;

    public MySqlReader(MigrationOptions options) : base(options) { }

    public override async Task OpenAsync()
    {
        _conn = new MySqlConnection(Options.ConnectionString);
        await _conn.OpenAsync();
    }

    public override async Task<List<string>> GetExistingTablesAsync()
    {
        var tables = new List<string>();
        using var cmd = _conn!.CreateCommand();
        cmd.CommandText = "SHOW TABLES";
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync()) tables.Add(reader.GetString(0));
        return tables;
    }

    public override async Task<List<string>> GetColumnsAsync(string tableName)
    {
        var cols = new List<string>();
        using var cmd = _conn!.CreateCommand();
        cmd.CommandText = $"SHOW COLUMNS FROM `{tableName}`";
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync()) cols.Add(reader.GetString(0)); // Field column
        return cols;
    }

    public override async Task<List<Dictionary<string, object?>>> ReadTableAsync(string tableName, List<string> columns)
    {
        var rows = new List<Dictionary<string, object?>>();
        if (columns.Count == 0) return rows;
        var colList = string.Join(", ", columns.Select(c => $"`{c}`"));
        using var cmd = _conn!.CreateCommand();
        cmd.CommandText = $"SELECT {colList} FROM `{tableName}`";
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var row = new Dictionary<string, object?>();
            for (int i = 0; i < columns.Count; i++)
                row[columns[i]] = reader.IsDBNull(i) ? null : reader.GetValue(i);
            rows.Add(row);
        }
        return rows;
    }

    public override void Dispose() => _conn?.Dispose();
}

