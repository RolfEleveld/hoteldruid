namespace HotelDroid.Migration;

/// <summary>
/// Orchestrates the full migration: reads DB, maps names, builds export or calls API.
/// </summary>
public class MigrationRunner
{
    private readonly MigrationOptions _opts;

    public MigrationRunner(MigrationOptions opts) => _opts = opts;

    public async Task<MigrationReport> RunAsync()
    {
        using var db = DatabaseReader.Create(_opts);
        Console.WriteLine($"Opening {_opts.DbType} database...");
        await db.OpenAsync();

        var existingTables = (await db.GetExistingTablesAsync())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        Console.WriteLine($"  Found {existingTables.Count} tables in database");

        var tableReports = new List<TableReport>();
        var exportData = new List<(string CanonicalTable, List<Dictionary<string, object?>> Rows)>();

        foreach (var knownSource in LegacyTableMapper.KnownSourceTables(_opts.TablePrefix))
        {
            var canonical = LegacyTableMapper.MapTable(knownSource, _opts.TablePrefix);

            if (!existingTables.Contains(knownSource))
            {
                if (_opts.Verbose)
                    Console.WriteLine($"  [SKIP] {knownSource} — not found in DB");
                tableReports.Add(new TableReport(knownSource, canonical, 0, true, "Table not found"));
                continue;
            }

            // Inspect actual columns (handles older versions with missing fields)
            var actualColumns = await db.GetColumnsAsync(knownSource);
            if (_opts.Verbose)
                Console.WriteLine($"  [READ] {knownSource} ({actualColumns.Count} columns)");

            if (!_opts.DryRun)
            {
                var rows = await db.ReadTableAsync(knownSource, actualColumns);

                // Map column names to canonical
                var mappedRows = rows.Select(row =>
                    row.ToDictionary(
                        kv => LegacyTableMapper.MapColumn(kv.Key),
                        kv => kv.Value))
                    .ToList();

                exportData.Add((canonical, mappedRows));
                tableReports.Add(new TableReport(knownSource, canonical, mappedRows.Count, false, null));
                Console.WriteLine($"  [OK]   {knownSource,-30} -> {canonical,-30} {mappedRows.Count,6} rows");
            }
            else
            {
                tableReports.Add(new TableReport(knownSource, canonical, 0, false, null));
                Console.WriteLine($"  [DRY]  {knownSource,-30} -> {canonical}");
            }
        }

        var report = new MigrationReport(
            TablesFound: existingTables.Count,
            TablesRead: tableReports.Count(t => !t.Skipped),
            TablesSkipped: tableReports.Count(t => t.Skipped),
            TotalRows: tableReports.Sum(t => t.RowCount),
            Tables: tableReports);

        if (!_opts.DryRun)
        {
            if (_opts.Mode == "export")
            {
                Console.WriteLine($"\nBuilding export zip: {_opts.OutputFile}");
                var builder = new ExportPackageBuilder();
                await builder.BuildAsync(exportData, _opts.OutputFile);
                Console.WriteLine($"  ✓ Written: {_opts.OutputFile}");
            }
            else if (_opts.Mode == "import")
            {
                if (string.IsNullOrEmpty(_opts.ApiUrl))
                    throw new InvalidOperationException("--api-url is required for import mode");

                // Build temp zip then post
                var tempZip = Path.Combine(Path.GetTempPath(), $"hoteldroid-migrate-{Guid.NewGuid():N}.zip");
                try
                {
                    var builder = new ExportPackageBuilder();
                    await builder.BuildAsync(exportData, tempZip);
                    var importer = new ApiImporter(_opts.ApiUrl);
                    await importer.ImportAsync(tempZip, _opts.Verbose);
                }
                finally
                {
                    if (File.Exists(tempZip)) File.Delete(tempZip);
                }
            }
        }

        return report;
    }
}
