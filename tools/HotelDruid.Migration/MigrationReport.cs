namespace HotelDruid.Migration;

public record MigrationReport(
    int TablesFound,
    int TablesRead,
    int TablesSkipped,
    int TotalRows,
    List<TableReport> Tables)
{
    public void Print()
    {
        Console.WriteLine();
        Console.WriteLine("=== Migration Report ===");
        Console.WriteLine($"  Tables found in DB : {TablesFound}");
        Console.WriteLine($"  Tables read        : {TablesRead}");
        Console.WriteLine($"  Tables skipped     : {TablesSkipped}");
        Console.WriteLine($"  Total rows exported: {TotalRows}");
        Console.WriteLine();
        foreach (var t in Tables)
            Console.WriteLine($"  [{(t.Skipped ? "SKIP" : "OK  ")}] {t.SourceTable,-30} -> {t.CanonicalTable,-30} {t.RowCount,6} rows");
        Console.WriteLine("========================");
    }
}

public record TableReport(
    string SourceTable,
    string CanonicalTable,
    int RowCount,
    bool Skipped,
    string? SkipReason);

