using HotelDruid.Migration;
using Microsoft.Data.Sqlite;
using Xunit;

namespace HotelDruid.Migration.Tests;

/// <summary>
/// Integration tests using file-based SQLite — no external DB needed.
/// Simulates an older HotelDruid installation with partial schema (missing columns).
/// </summary>
public class SqliteReaderTests
{
    [Fact]
    public async Task SqliteReader_GetExistingTables_ReturnsTables()
    {
        var tempDb = Path.Combine(Path.GetTempPath(), $"test-{Guid.NewGuid():N}.db");
        try
        {
            // Seed DB
            {
                using var seed = new SqliteConnection($"Data Source={tempDb}");
                seed.Open();
                using var cmd = seed.CreateCommand();
                cmd.CommandText = "CREATE TABLE appartamenti (id INTEGER PRIMARY KEY, nome TEXT)";
                cmd.ExecuteNonQuery();
            }

            var opts = new MigrationOptions
            {
                DbType = "sqlite",
                ConnectionString = $"Data Source={tempDb}"
            };

            List<string> tables;
            {
                using var reader = (SqliteReader)DatabaseReader.Create(opts);
                await reader.OpenAsync();
                tables = await reader.GetExistingTablesAsync();
                // reader disposes here, releasing file lock
            }

            Assert.Contains("appartamenti", tables);
        }
        finally
        {
            SqliteConnection.ClearAllPools();
            if (File.Exists(tempDb)) File.Delete(tempDb);
        }
    }

    [Fact]
    public async Task SqliteReader_ReadsPartialSchema_MissingColumnsSkipped()
    {
        var tempDb = Path.Combine(Path.GetTempPath(), $"test-partial-{Guid.NewGuid():N}.db");
        try
        {
            // Create DB with partial schema (older version — beds column absent)
            {
                using var seed = new SqliteConnection($"Data Source={tempDb}");
                seed.Open();
                using var cmd = seed.CreateCommand();
                cmd.CommandText = """
                    CREATE TABLE appartamenti (
                        idappartamento INTEGER PRIMARY KEY,
                        nomerappartamento TEXT
                    );
                    INSERT INTO appartamenti VALUES (1, 'Room One');
                    INSERT INTO appartamenti VALUES (2, 'Room Two');
                    """;
                cmd.ExecuteNonQuery();
            }

            var opts = new MigrationOptions
            {
                DbType = "sqlite",
                ConnectionString = $"Data Source={tempDb}"
            };

            List<string> cols;
            List<Dictionary<string, object?>> rows;
            {
                using var reader = (SqliteReader)DatabaseReader.Create(opts);
                await reader.OpenAsync();
                cols = await reader.GetColumnsAsync("appartamenti");
                rows = await reader.ReadTableAsync("appartamenti", cols);
            }

            Assert.Equal(2, cols.Count);
            Assert.Contains("idappartamento", cols);
            Assert.Contains("nomerappartamento", cols);
            // "postiletto" (beds) is absent — that''s expected for older versions

            Assert.Equal(2, rows.Count);
            Assert.Equal("Room One", rows[0]["nomerappartamento"]?.ToString());
        }
        finally
        {
            SqliteConnection.ClearAllPools();
            if (File.Exists(tempDb)) File.Delete(tempDb);
        }
    }
}

