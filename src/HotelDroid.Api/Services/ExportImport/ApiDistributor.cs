using HotelDroid.Api.Models;
using HotelDroid.Shared;

namespace HotelDroid.Api.Services.ExportImport;

/// <summary>
/// Distributes imported data to appropriate API endpoints
/// Currently handles rooms; will be extended for other entities
/// </summary>
public class ApiDistributor : IApiDistributor
{
    private readonly IKeyValueStore _roomsStore;
    private readonly ICanonicalMapper _canonicalMapper;
    private readonly ILogger<ApiDistributor> _logger;

    public ApiDistributor(
        IKeyValueStore roomsStore,
        ICanonicalMapper canonicalMapper,
        ILogger<ApiDistributor> logger)
    {
        _roomsStore = roomsStore;
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
                        await _roomsStore.CreateAsync<RoomDto>("rooms", room.Name, room);
                        importedCount++;
                    }
                    catch (InvalidOperationException)
                    {
                        // Name already exists - update it instead
                        var existing = await _roomsStore.GetByNameAsync<RoomDto>("rooms", room.Name);
                        if (existing?.Id != null)
                        {
                            await _roomsStore.UpdateAsync<RoomDto>("rooms", existing.Id, room);
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
}
