using System.Globalization;
using System.Text.Json;
using HotelDruid.Api.Models;

namespace HotelDruid.Api.Services.ExportImport;

/// <summary>
/// Reads a PHP HotelDruid export table. The exporter writes snake_case names.
/// </summary>
public static class PhpExportJson
{
    private static readonly JsonSerializerOptions Snake = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    private static readonly JsonSerializerOptions Camel = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static Manifest? ReadManifest(string json)
    {
        var manifest = TryManifest(json, Snake);
        if (!string.IsNullOrWhiteSpace(manifest?.ExportId))
            return manifest;
        return TryManifest(json, Camel) ?? manifest;
    }

    public static CanonicalData? ReadTable(string json, string? fallbackName = null)
    {
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;
        if (root.ValueKind != JsonValueKind.Object)
            return null;

        var name = StringValue(root, "table_name") ?? StringValue(root, "tableName") ?? fallbackName;
        if (string.IsNullOrWhiteSpace(name))
            return null;

        var columns = new List<ColumnDefinition>();
        if (root.TryGetProperty("columns", out var columnArray) && columnArray.ValueKind == JsonValueKind.Array)
        {
            foreach (var column in columnArray.EnumerateArray())
            {
                var columnName = StringValue(column, "name");
                if (string.IsNullOrWhiteSpace(columnName))
                    continue;
                columns.Add(new ColumnDefinition(
                    columnName,
                    StringValue(column, "type") ?? "text",
                    BoolValue(column, "nullable") ?? true));
            }
        }

        var rows = new List<Dictionary<string, object?>>();
        if (root.TryGetProperty("rows", out var rowArray) && rowArray.ValueKind == JsonValueKind.Array)
        {
            foreach (var row in rowArray.EnumerateArray())
            {
                if (row.ValueKind != JsonValueKind.Object)
                    continue;
                var mapped = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
                foreach (var property in row.EnumerateObject())
                    mapped[property.Name] = ClrValue(property.Value);
                rows.Add(mapped);
            }
        }

        return new CanonicalData(name, rows.Count, rows, columns);
    }

    private static Manifest? TryManifest(string json, JsonSerializerOptions options)
    {
        try
        {
            return JsonSerializer.Deserialize<Manifest>(json, options);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static string? StringValue(JsonElement element, string name)
    {
        if (!element.TryGetProperty(name, out var value))
            return null;
        return value.ValueKind == JsonValueKind.String ? value.GetString() : null;
    }

    private static bool? BoolValue(JsonElement element, string name)
    {
        if (!element.TryGetProperty(name, out var value))
            return null;
        return value.ValueKind switch
        {
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            _ => null
        };
    }

    private static object? ClrValue(JsonElement value)
    {
        return value.ValueKind switch
        {
            JsonValueKind.Null or JsonValueKind.Undefined => null,
            JsonValueKind.String => value.GetString(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Number when value.TryGetInt64(out var whole) => whole,
            JsonValueKind.Number => value.GetDouble(),
            _ => value.GetRawText()
        };
    }

    public static string? Text(IReadOnlyDictionary<string, object?> row, params string[] keys)
    {
        var value = First(row, keys);
        if (value is null)
            return null;
        var text = Convert.ToString(value, CultureInfo.InvariantCulture);
        return string.IsNullOrWhiteSpace(text) ? null : text;
    }

    public static double? Number(IReadOnlyDictionary<string, object?> row, params string[] keys)
    {
        var value = First(row, keys);
        return value switch
        {
            null => null,
            double number => number,
            float number => number,
            decimal number => (double)number,
            long number => number,
            int number => number,
            _ => double.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed) ? parsed : null
        };
    }

    public static int? Whole(IReadOnlyDictionary<string, object?> row, params string[] keys)
    {
        var number = Number(row, keys);
        return number.HasValue ? (int)number.Value : null;
    }

    public static DateTime? Timestamp(IReadOnlyDictionary<string, object?> row, params string[] keys)
    {
        var text = Text(row, keys);
        if (text is null)
            return null;
        return DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var parsed) ? parsed : null;
    }

    public static DateOnly DayOfYear(int year, int dayId)
    {
        return new DateOnly(year, 1, 1).AddDays(dayId - 1);
    }

    private static object? First(IReadOnlyDictionary<string, object?> row, params string[] keys)
    {
        foreach (var key in keys)
        {
            if (row.TryGetValue(key, out var value) && value is not null)
                return value;
        }
        return null;
    }
}
