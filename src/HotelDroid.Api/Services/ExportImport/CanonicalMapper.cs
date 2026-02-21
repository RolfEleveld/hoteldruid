using System.Text.Json;
using HotelDroid.Api.Models;

namespace HotelDroid.Api.Services.ExportImport;

/// <summary>
/// Maps between .NET objects and hoteldruid canonical JSON format
/// Implements: Inward Liberal, Outward Conservative
/// - Accepts any reasonable table/field names on input
/// - Normalizes to canonical form internally
/// - Exports with industry-standard refined names
/// </summary>
public class CanonicalMapper : ICanonicalMapper
{
    private readonly NamingConfiguration _naming;
    private readonly ILogger<CanonicalMapper> _logger;

    // Default naming configuration
    private static readonly NamingConfiguration DefaultNaming = new(
        TableAliases: new()
        {
            ["appartamenti"] = "rooms",
            ["apartments"] = "rooms",
            ["rooms"] = "rooms",
            ["appartment"] = "rooms",
            ["apartment"] = "rooms"
        },
        FieldAliases: new()
        {
            // Hoteldruid Italian → Canonical
            ["idappartamenti"] = "id",
            ["numpiano"] = "floor_number",
            ["maxoccupanti"] = "capacity",
            ["numcasa"] = "house_number",
            ["app_vicini"] = "neighboring_rooms",
            ["priorita"] = "priority",
            ["priorita2"] = "secondary_priority",
            ["letto"] = "has_beds",
            ["commento"] = "comments",
            
            // English variants
            ["room_id"] = "id",
            ["floor"] = "floor_number",
            ["max_occupancy"] = "capacity",
            ["house_id"] = "house_number",
            ["neighbors"] = "neighboring_rooms",
            ["notes"] = "comments",
            ["beds"] = "has_beds",
            
            // API names (pass-through)
            ["id"] = "id",
            ["floor_number"] = "floor_number",
            ["capacity"] = "capacity",
            ["house_number"] = "house_number",
            ["neighboring_rooms"] = "neighboring_rooms",
            ["priority"] = "priority",
            ["secondary_priority"] = "secondary_priority",
            ["has_beds"] = "has_beds",
            ["comments"] = "comments",
            ["name"] = "name"
        },
        ExportNames: new ExportNamingConfig(
            TableName: "rooms",  // Standard export name
            Fields: new()
            {
                ["id"] = "id",
                ["floor_number"] = "floor_number",
                ["capacity"] = "capacity",
                ["house_number"] = "house_number",
                ["neighboring_rooms"] = "neighboring_rooms",
                ["priority"] = "priority",
                ["secondary_priority"] = "secondary_priority",
                ["has_beds"] = "has_beds",
                ["comments"] = "comments",
                ["name"] = "name"
            }
        )
    );

    private const string StandardTableName = "rooms";
    private const string HotelDroidTableName = "appartamenti";

    public CanonicalMapper(ILogger<CanonicalMapper> logger, NamingConfiguration? naming = null)
    {
        _logger = logger;
        _naming = naming ?? DefaultNaming;
    }

    public string NormalizeTableName(string inputTableName)
    {
        var lower = inputTableName?.ToLowerInvariant().Trim() ?? "";
        
        if (_naming.TableAliases.TryGetValue(lower, out var canonical))
        {
            _logger.LogDebug("Normalized table '{Input}' to '{Canonical}'", inputTableName, canonical);
            return canonical;
        }

        _logger.LogWarning("Unknown table name '{Input}', passing through", inputTableName);
        return lower;
    }

    public string GetExportTableName(string normalizedTableName)
    {
        var table = NormalizeTableName(normalizedTableName);
        return table switch
        {
            "rooms" => StandardTableName,
            _ => table
        };
    }

    public string GetHotelDroidTableName(string normalizedTableName)
    {
        var table = NormalizeTableName(normalizedTableName);
        return table switch
        {
            "rooms" => HotelDroidTableName,
            _ => table
        };
    }

    public string NormalizeFieldName(string inputFieldName)
    {
        var lower = inputFieldName?.ToLowerInvariant().Trim() ?? "";
        
        if (_naming.FieldAliases.TryGetValue(lower, out var canonical))
        {
            return canonical;
        }

        return lower;
    }

    public CanonicalData ToCanonical(RoomDto[] rooms)
    {
        // Build column list
        var columns = new List<ColumnDefinition>
        {
            new("id", "string", false, "Room identifier"),
            new("name", "string", false, "Room name"),
            new("capacity", "integer", false, "Maximum occupants"),
            new("floor_number", "string", true, "Floor number"),
            new("house_number", "string", true, "House/apartment number"),
            new("priority", "integer", true, "Priority ordering (primary)"),
            new("secondary_priority", "integer", true, "Priority ordering (secondary)"),
            new("has_beds", "string", true, "Has beds (S/N)"),
            new("neighboring_rooms", "string", true, "Comma-separated neighbor room IDs"),
            new("comments", "string", true, "Additional comments")
        };

        // Convert rows to canonical format
        var rows = rooms.Select(r => new Dictionary<string, object?>
        {
            ["id"] = r.Id,
            ["name"] = r.Name,
            ["capacity"] = r.Capacity,
            ["floor_number"] = r.FloorNumber,
            ["house_number"] = r.HouseNumber,
            ["priority"] = r.Priority,
            ["secondary_priority"] = r.SecondaryPriority,
            ["has_beds"] = r.HasBeds,
            ["neighboring_rooms"] = r.NeighboringRooms,
            ["comments"] = r.Comments
        }).ToList();

        return new CanonicalData(
            TableName: StandardTableName,
            RowCount: rooms.Length,
            Rows: rows,
            Columns: columns
        );
    }

    public RoomDto[] FromCanonical(CanonicalData data)
    {
        // Validate table name
        var normalizedTable = NormalizeTableName(data.TableName);
        if (normalizedTable != "rooms")
        {
            throw new InvalidOperationException(
                $"Cannot map table '{data.TableName}' to rooms API. Expected 'rooms', 'apartments', or 'appartamenti'.");
        }

        var results = new List<RoomDto>();

        foreach (var row in data.Rows)
        {
            try
            {
                var roomDto = new RoomDto(
                    Id: GetNormalizedValue(row, "id")?.ToString(),
                    Name: GetNormalizedValue(row, "name")?.ToString() ?? "",
                    Capacity: int.TryParse(GetNormalizedValue(row, "capacity")?.ToString(), out var cap) ? cap : 0,
                    FloorNumber: GetNormalizedValue(row, "floor_number")?.ToString(),
                    HouseNumber: GetNormalizedValue(row, "house_number")?.ToString(),
                    Priority: int.TryParse(GetNormalizedValue(row, "priority")?.ToString(), out var p) ? p : null,
                    SecondaryPriority: int.TryParse(GetNormalizedValue(row, "secondary_priority")?.ToString(), out var p2) ? p2 : null,
                    HasBeds: GetNormalizedValue(row, "has_beds")?.ToString(),
                    NeighboringRooms: GetNormalizedValue(row, "neighboring_rooms")?.ToString(),
                    Comments: GetNormalizedValue(row, "comments")?.ToString()
                );
                
                results.Add(roomDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error mapping row to RoomDto");
                throw;
            }
        }

        return results.ToArray();
    }

    public object? GetNormalizedValue(Dictionary<string, object?> row, string canonicalFieldName)
    {
        if (row == null)
            return null;

        // Try exact canonical name first (case-insensitive)
        var lowerCanonical = canonicalFieldName?.ToLowerInvariant() ?? "";
        
        foreach (var (key, value) in row)
        {
            if (key.Equals(lowerCanonical, StringComparison.OrdinalIgnoreCase))
                return value;
        }

        // Try to find via alias mapping (reverse lookup)
        foreach (var (alias, canonical) in _naming.FieldAliases)
        {
            if (canonical == lowerCanonical)
            {
                // Check for this alias in the row (case-insensitive)
                foreach (var (key, value) in row)
                {
                    if (key.Equals(alias, StringComparison.OrdinalIgnoreCase))
                        return value;
                }
            }
        }

        _logger.LogDebug("Field '{Field}' not found in row", canonicalFieldName);
        return null;
    }

    /// <summary>
    /// Build metadata info showing source names vs. export names
    /// </summary>
    public Dictionary<string, string> GetNamingInfo(string sourceTableName)
    {
        var normalized = NormalizeTableName(sourceTableName);
        
        return new()
        {
            ["source_name"] = sourceTableName,
            ["normalized_to"] = normalized,
            ["export_name"] = GetExportTableName(normalized),
            ["hoteldruid_compat"] = GetHotelDroidTableName(normalized)
        };
    }
}
