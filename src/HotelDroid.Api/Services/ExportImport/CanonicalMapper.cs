using System.Text.Json;
using HotelDroid.Api.Models;
using HotelDroid.Shared;

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
            // Rooms
            ["appartamenti"] = "rooms",
            ["apartments"] = "rooms",
            ["rooms"] = "rooms",
            ["appartment"] = "rooms",
            ["apartment"] = "rooms",

            // Assets / Inventory (italian → canonical)
            ["beniinventario"] = "assets",
            ["beniinventario"] = "assets",
            ["magazzini"] = "warehouses",
            ["relinventario"] = "inventory",

            // English forms
            ["assets"] = "assets",
            ["warehouses"] = "warehouses",
            ["inventory"] = "inventory"
        },
        FieldAliases: new()
        {
            // Hoteldruid Italian → Canonical (rooms)
            ["idappartamenti"] = "id",
            ["numpiano"] = "floor_number",
            ["maxoccupanti"] = "capacity",
            ["numcasa"] = "house_number",
            ["app_vicini"] = "neighboring_rooms",
            ["priorita"] = "priority",
            ["priorita2"] = "secondary_priority",
            ["letto"] = "has_beds",
            ["commento"] = "comments",

            // Rooms - English variants
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
            ["name"] = "name",

            // Assets (italian)
            ["idbeniinventario"] = "id",
            ["nome_bene"] = "name",
            ["codice_bene"] = "code",
            ["descrizione_bene"] = "description",
            ["datainserimento"] = "created_at",
            ["hostinserimento"] = "host_created",
            ["utente_inserimento"] = "created_by",

            // Warehouses (italian)
            ["idmagazzini"] = "id",
            ["nome_magazzino"] = "name",
            ["codice_magazzino"] = "code",
            ["descrizione_magazzino"] = "description",
            ["numpiano"] = "floor_number",
            ["numcasa"] = "house_number",

            // Inventory (italian)
            ["idbeneinventario"] = "asset_id",
            ["idappartamento"] = "room_id",
            ["idmagazzino"] = "warehouse_id",
            ["quantita"] = "quantity",
            ["quantita_min_predef"] = "min_quantity_default",
            ["richiesto_checkin"] = "required_on_checkin"
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
    private const string AssetsTableName = "assets";
    private const string AssetsHoteldruidName = "beniinventario";
    private const string WarehousesTableName = "warehouses";
    private const string WarehousesHoteldruidName = "magazzini";
    private const string InventoryTableName = "inventory";
    private const string InventoryHoteldruidName = "relinventario";

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
            "assets" => AssetsTableName,
            "warehouses" => WarehousesTableName,
            "inventory" => InventoryTableName,
            _ => table
        };
    }

    public string GetHotelDroidTableName(string normalizedTableName)
    {
        var table = NormalizeTableName(normalizedTableName);
        return table switch
        {
            "rooms" => HotelDroidTableName,
            "assets" => AssetsHoteldruidName,
            "warehouses" => WarehousesHoteldruidName,
            "inventory" => InventoryHoteldruidName,
            _ => table
        };
    }

    // --- Assets / Warehouses / Inventory canonicalization ---

    public CanonicalData ToCanonicalAssets(AssetDto[] assets)
    {
        var columns = new List<ColumnDefinition>
        {
            new("id", "string", false, "Asset identifier"),
            new("name", "string", false, "Asset name"),
            new("code", "string", true, "Asset code"),
            new("description", "string", true, "Description"),
            new("created_at", "string", true, "Created timestamp")
        };

        var rows = assets.Select(a => new Dictionary<string, object?>
        {
            ["id"] = a.Id,
            ["name"] = a.Name,
            ["code"] = a.Code,
            ["description"] = a.Description,
            ["created_at"] = a.CreatedAt?.ToString("O")
        }).ToList();

        return new CanonicalData(TableName: AssetsTableName, RowCount: assets.Length, Rows: rows, Columns: columns);
    }

    public AssetDto[] FromCanonicalAssets(CanonicalData data)
    {
        var normalized = NormalizeTableName(data.TableName);
        if (normalized != "assets") throw new InvalidOperationException($"Cannot map table '{data.TableName}' to assets.");

        var results = new List<AssetDto>();
        foreach (var row in data.Rows)
        {
            var id = GetNormalizedValue(row, "id")?.ToString();
            var name = GetNormalizedValue(row, "name")?.ToString() ?? "";
            var code = GetNormalizedValue(row, "code")?.ToString();
            var description = GetNormalizedValue(row, "description")?.ToString();
            var created = DateTime.TryParse(GetNormalizedValue(row, "created_at")?.ToString(), out var dt) ? dt : (DateTime?)null;

            results.Add(new AssetDto(id, name, code, description, created));
        }

        return results.ToArray();
    }

    public CanonicalData ToCanonicalWarehouses(WarehouseDto[] warehouses)
    {
        var columns = new List<ColumnDefinition>
        {
            new("id", "string", false, "Warehouse identifier"),
            new("name", "string", false, "Warehouse name"),
            new("code", "string", true, "Code"),
            new("description", "string", true, "Description"),
            new("floor_number", "string", true, "Floor"),
            new("house_number", "string", true, "House number")
        };

        var rows = warehouses.Select(w => new Dictionary<string, object?>
        {
            ["id"] = w.Id,
            ["name"] = w.Name,
            ["code"] = w.Code,
            ["description"] = w.Description,
            ["floor_number"] = w.FloorNumber,
            ["house_number"] = w.HouseNumber
        }).ToList();

        return new CanonicalData(TableName: WarehousesTableName, RowCount: warehouses.Length, Rows: rows, Columns: columns);
    }

    public WarehouseDto[] FromCanonicalWarehouses(CanonicalData data)
    {
        var normalized = NormalizeTableName(data.TableName);
        if (normalized != "warehouses") throw new InvalidOperationException($"Cannot map table '{data.TableName}' to warehouses.");

        var results = new List<WarehouseDto>();
        foreach (var row in data.Rows)
        {
            var id = GetNormalizedValue(row, "id")?.ToString();
            var name = GetNormalizedValue(row, "name")?.ToString() ?? "";
            var code = GetNormalizedValue(row, "code")?.ToString();
            var desc = GetNormalizedValue(row, "description")?.ToString();
            var floor = GetNormalizedValue(row, "floor_number")?.ToString();
            var house = GetNormalizedValue(row, "house_number")?.ToString();

            results.Add(new WarehouseDto(id, name, code, desc, floor, house, null));
        }

        return results.ToArray();
    }

    public CanonicalData ToCanonicalInventory(InventoryDto[] inventory)
    {
        var columns = new List<ColumnDefinition>
        {
            new("id", "string", false, "Inventory identifier"),
            new("asset_id", "string", false, "Asset reference id"),
            new("room_id", "string", true, "Room id (optional)"),
            new("warehouse_id", "string", true, "Warehouse id (optional)"),
            new("quantity", "integer", false, "Quantity"),
            new("min_quantity_default", "integer", true, "Min default quantity"),
            new("required_on_checkin", "boolean", true, "Required on check-in")
        };

        var rows = inventory.Select(i => new Dictionary<string, object?>
        {
            ["id"] = i.Id,
            ["asset_id"] = i.AssetId,
            ["room_id"] = i.RoomId,
            ["warehouse_id"] = i.WarehouseId,
            ["quantity"] = i.Quantity,
            ["min_quantity_default"] = i.MinQuantityDefault,
            ["required_on_checkin"] = i.RequiredOnCheckin
        }).ToList();

        return new CanonicalData(TableName: InventoryTableName, RowCount: inventory.Length, Rows: rows, Columns: columns);
    }

    public InventoryDto[] FromCanonicalInventory(CanonicalData data)
    {
        var normalized = NormalizeTableName(data.TableName);
        if (normalized != "inventory") throw new InvalidOperationException($"Cannot map table '{data.TableName}' to inventory.");

        var results = new List<InventoryDto>();
        foreach (var row in data.Rows)
        {
            var id = GetNormalizedValue(row, "id")?.ToString();
            var aid = GetNormalizedValue(row, "asset_id")?.ToString();
            var rid = GetNormalizedValue(row, "room_id")?.ToString();
            var wid = GetNormalizedValue(row, "warehouse_id")?.ToString();
            var qty = int.TryParse(GetNormalizedValue(row, "quantity")?.ToString(), out var q) ? q : 0;
            var minq = int.TryParse(GetNormalizedValue(row, "min_quantity_default")?.ToString(), out var mq) ? mq : (int?)null;
            var req = bool.TryParse(GetNormalizedValue(row, "required_on_checkin")?.ToString(), out var b) ? b : (bool?)null;

            results.Add(new InventoryDto(id, aid, rid, wid, qty, minq, req, null));
        }

        return results.ToArray();
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
