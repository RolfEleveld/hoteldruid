using HotelDroid.Api.Models;
using HotelDroid.Shared;

namespace HotelDroid.Api.Services.ExportImport;

/// <summary>
/// Distributes imported data to appropriate API endpoints
/// Currently handles rooms; will be extended for other entities
/// </summary>
public class ApiDistributor : IApiDistributor
{
    private readonly IKeyValueStore _store;
    private readonly ICanonicalMapper _canonicalMapper;
    private readonly ILogger<ApiDistributor> _logger;

    public ApiDistributor(
        IKeyValueStore store,
        ICanonicalMapper canonicalMapper,
        ILogger<ApiDistributor> logger)
    {
        _store = store;
        _canonicalMapper = canonicalMapper;
        _logger = logger;
    }

    public async Task<DistributionResult> DistributeAsync(
        Dictionary<string, CanonicalData> data)
    {
        var results = new Dictionary<string, TableImportResult>();
        var errors = new List<ImportError>();
        var totalImported = 0;

        foreach (var (sourceTableName, tableData) in data)
        {
            try
            {
                var normalizedTable = _canonicalMapper.NormalizeTableName(sourceTableName);

                switch (normalizedTable)
                {
                    case "rooms":
                        var roomResult = await ImportRoomsAsync(tableData);
                        results[normalizedTable] = roomResult;
                        if (roomResult.Success)
                            totalImported += roomResult.ImportedCount;
                        else if (roomResult.ErrorMessage != null)
                            errors.Add(new ImportError(normalizedTable, roomResult.ErrorMessage));
                        break;
                    case "assets":
                        var assetsResult = await ImportAssetsAsync(tableData);
                        results[normalizedTable] = assetsResult;
                        if (assetsResult.Success) totalImported += assetsResult.ImportedCount;
                        else if (assetsResult.ErrorMessage != null) errors.Add(new ImportError(normalizedTable, assetsResult.ErrorMessage));
                        break;
                    case "warehouses":
                        var whResult = await ImportWarehousesAsync(tableData);
                        results[normalizedTable] = whResult;
                        if (whResult.Success) totalImported += whResult.ImportedCount;
                        else if (whResult.ErrorMessage != null) errors.Add(new ImportError(normalizedTable, whResult.ErrorMessage));
                        break;
                    case "inventory":
                        var invResult = await ImportInventoryAsync(tableData);
                        results[normalizedTable] = invResult;
                        if (invResult.Success) totalImported += invResult.ImportedCount;
                        else if (invResult.ErrorMessage != null) errors.Add(new ImportError(normalizedTable, invResult.ErrorMessage));
                        break;

                    default:
                        _logger.LogWarning("Cannot import unknown table: {TableName}", sourceTableName);
                        errors.Add(new ImportError(sourceTableName, 
                            $"Unknown table type '{sourceTableName}'. Only 'rooms' is currently supported."));
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error distributing table {TableName}", sourceTableName);
                errors.Add(new ImportError(sourceTableName, ex.Message));
            }
        }

        return new DistributionResult(
            TotalImported: totalImported,
            ByTable: results,
            Errors: errors
        );
    }

    private async Task<TableImportResult> ImportRoomsAsync(CanonicalData tableData)
    {
        try
        {
            _logger.LogInformation("Importing {Count} rooms", tableData.RowCount);

            // Convert canonical data to RoomDto objects
            var rooms = _canonicalMapper.FromCanonical(tableData);

            // Store each room
            var importedCount = 0;
            
            foreach (var room in rooms)
            {
                try
                {
                    if (string.IsNullOrEmpty(room.Name))
                    {
                        _logger.LogWarning("Skipping room without name");
                        continue;
                    }

                    // Create new room with name as the index key
                    // This will auto-generate a storage ID
                    try
                    {
                        await _store.CreateAsync<RoomDto>("rooms", room.Name, room);
                        importedCount++;
                    }
                    catch (InvalidOperationException)
                    {
                        // Name already exists - update it instead
                        var existing = await _store.GetByNameAsync<RoomDto>("rooms", room.Name);
                        if (existing?.Id != null)
                        {
                            await _store.UpdateAsync<RoomDto>("rooms", existing.Id, room);
                            importedCount++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error storing room {RoomName}", room.Name);
                    throw;
                }
            }

            _logger.LogInformation("Successfully imported {Count} rooms", importedCount);

            return new TableImportResult(
                TableName: "rooms",
                ImportedCount: importedCount,
                Success: true
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing rooms table");
            return new TableImportResult(
                TableName: "rooms",
                ImportedCount: 0,
                Success: false,
                ErrorMessage: ex.Message
            );
        }
    }

    private async Task<TableImportResult> ImportAssetsAsync(CanonicalData tableData)
    {
        try
        {
            _logger.LogInformation("Importing {Count} assets", tableData.RowCount);
            var assets = _canonicalMapper.FromCanonicalAssets(tableData);
            var imported = 0;
            foreach (var asset in assets)
            {
                if (string.IsNullOrEmpty(asset.Name)) { _logger.LogWarning("Skipping asset without name"); continue; }
                try
                {
                    await _store.CreateAsync<AssetDto>("assets", asset.Name, asset);
                    imported++;
                }
                catch (InvalidOperationException)
                {
                    var existing = await _store.GetByNameAsync<AssetDto>("assets", asset.Name);
                    if (existing?.Id != null) { await _store.UpdateAsync<AssetDto>("assets", existing.Id, asset); imported++; }
                }
            }

            return new TableImportResult(TableName: "assets", ImportedCount: imported, Success: true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing assets");
            return new TableImportResult(TableName: "assets", ImportedCount: 0, Success: false, ErrorMessage: ex.Message);
        }
    }

    private async Task<TableImportResult> ImportWarehousesAsync(CanonicalData tableData)
    {
        try
        {
            _logger.LogInformation("Importing {Count} warehouses", tableData.RowCount);
            var items = _canonicalMapper.FromCanonicalWarehouses(tableData);
            var imported = 0;
            foreach (var w in items)
            {
                if (string.IsNullOrEmpty(w.Name)) { _logger.LogWarning("Skipping warehouse without name"); continue; }
                try
                {
                    await _store.CreateAsync<WarehouseDto>("warehouses", w.Name, w);
                    imported++;
                }
                catch (InvalidOperationException)
                {
                    var existing = await _store.GetByNameAsync<WarehouseDto>("warehouses", w.Name);
                    if (existing?.Id != null) { await _store.UpdateAsync<WarehouseDto>("warehouses", existing.Id, w); imported++; }
                }
            }

            return new TableImportResult(TableName: "warehouses", ImportedCount: imported, Success: true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing warehouses");
            return new TableImportResult(TableName: "warehouses", ImportedCount: 0, Success: false, ErrorMessage: ex.Message);
        }
    }

    private async Task<TableImportResult> ImportInventoryAsync(CanonicalData tableData)
    {
        try
        {
            _logger.LogInformation("Importing {Count} inventory rows", tableData.RowCount);
            var items = _canonicalMapper.FromCanonicalInventory(tableData);
            var imported = 0;
            foreach (var inv in items)
            {
                if (string.IsNullOrEmpty(inv.AssetId)) { _logger.LogWarning("Skipping inventory without asset reference"); continue; }
                try
                {
                    // Use asset id as logical index when creating
                    await _store.CreateAsync<InventoryDto>("inventory", inv.AssetId ?? Guid.NewGuid().ToString(), inv);
                    imported++;
                }
                catch (InvalidOperationException)
                {
                    // try to update existing by assetId
                    var existing = await _store.GetByNameAsync<InventoryDto>("inventory", inv.AssetId ?? "");
                    if (existing?.Id != null) { await _store.UpdateAsync<InventoryDto>("inventory", existing.Id, inv); imported++; }
                }
            }

            return new TableImportResult(TableName: "inventory", ImportedCount: imported, Success: true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing inventory");
            return new TableImportResult(TableName: "inventory", ImportedCount: 0, Success: false, ErrorMessage: ex.Message);
        }
    }
}
