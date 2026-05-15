using System.IO.Compression;
using System.Text.Json;

namespace HotelDruid.Migration;

/// <summary>
/// Builds a canonical export zip identical to the format produced by the PHP Exporter
/// and consumed by the Blazor ImportService.
/// Format: manifest.json + {canonical_table_name}.json per table
/// </summary>
public class ExportPackageBuilder
{
    private readonly JsonSerializerOptions _json = new() { WriteIndented = true };

    public async Task BuildAsync(
        IReadOnlyList<(string CanonicalTable, List<Dictionary<string, object?>> Rows)> tables,
        string outputPath)
    {
        var manifest = new
        {
            version = "1.0",
            source = "HotelDruid-migrate",
            exportedAt = DateTime.UtcNow.ToString("o"),
            tables = tables.Select(t => new { name = t.CanonicalTable, rowCount = t.Rows.Count }).ToList()
        };

        if (File.Exists(outputPath)) File.Delete(outputPath);

        using var archive = ZipFile.Open(outputPath, ZipArchiveMode.Create);

        // Write manifest
        var manifestEntry = archive.CreateEntry("manifest.json");
        using (var stream = manifestEntry.Open())
        {
            var manifestJson = JsonSerializer.Serialize(manifest, _json);
            await stream.WriteAsync(System.Text.Encoding.UTF8.GetBytes(manifestJson));
        }

        // Write each table
        foreach (var (canonicalTable, rows) in tables)
        {
            var tableData = new { table_name = canonicalTable, rows };
            var entry = archive.CreateEntry($"{canonicalTable}.json");
            using var s = entry.Open();
            var json = JsonSerializer.Serialize(tableData, _json);
            await s.WriteAsync(System.Text.Encoding.UTF8.GetBytes(json));
        }
    }
}

