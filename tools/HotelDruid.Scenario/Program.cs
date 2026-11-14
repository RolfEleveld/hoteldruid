if (args.Contains("--help") || args.Contains("-h"))
{
    Console.WriteLine("HotelDruid.Scenario --data-root <path> --replace [--start yyyy-MM-dd] [--end yyyy-MM-dd] [--rooms 20] [--occupancy 0.67] [--seed N] [--report <file>]");
    return;
}

string? dataRoot = null;
string? reportPath = null;
var start = new DateOnly(2022, 11, 14);
var end = new DateOnly(2026, 11, 14);
var rooms = 20;
var occupancy = 0.67;
var seed = 20261114;
var replace = false;

for (var i = 0; i < args.Length; i++)
{
    string Next() => i + 1 < args.Length ? args[++i] : throw new InvalidOperationException($"Missing value for {args[i]}");
    switch (args[i])
    {
        case "--data-root": dataRoot = Next(); break;
        case "--report": reportPath = Next(); break;
        case "--start": start = DateOnly.Parse(Next()); break;
        case "--end": end = DateOnly.Parse(Next()); break;
        case "--rooms": rooms = int.Parse(Next()); break;
        case "--occupancy": occupancy = double.Parse(Next(), System.Globalization.CultureInfo.InvariantCulture); break;
        case "--seed": seed = int.Parse(Next()); break;
        case "--replace": replace = true; break;
        default: throw new InvalidOperationException($"Unknown argument {args[i]}");
    }
}

if (string.IsNullOrWhiteSpace(dataRoot))
    throw new InvalidOperationException("Pass --data-root.");
if (!replace)
    throw new InvalidOperationException("Pass --replace to wipe collections, the ledger, and booking indexes in that data root before writing.");

var result = ScenarioWriter.Write(dataRoot, new ScenarioRequest(start, end, rooms, occupancy, seed));
var json = System.Text.Json.JsonSerializer.Serialize(result, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
Console.WriteLine(json);
if (!string.IsNullOrWhiteSpace(reportPath))
{
    Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(reportPath))!);
    File.WriteAllText(reportPath, json);
}
