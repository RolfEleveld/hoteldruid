using HotelDruid.Api.Models;
using HotelDruid.Shared;
using HotelDruid.Shared.Configuration;
using System.Text.Json;

namespace HotelDruid.Api.Services.ExportImport;

/// <summary>
/// Distributes imported data to appropriate API endpoints
/// Currently handles rooms; will be extended for other entities
/// </summary>
public class ApiDistributor : IApiDistributor
{
    private readonly IKeyValueStore _store;
    private readonly ISystemConfigurationStore _systemConfigurationStore;
    private readonly ICanonicalMapper _canonicalMapper;
    private readonly IFinancialPostingService? _ledger;
    private readonly ILedgerRepository? _ledgerRepository;
    private readonly IBookingLedgerIndexRepository? _bookingLedgerIndex;
    private readonly ILogger<ApiDistributor> _logger;
    private readonly Dictionary<string, string> _ids = new(StringComparer.OrdinalIgnoreCase);

    public ApiDistributor(
        IKeyValueStore store,
        ISystemConfigurationStore systemConfigurationStore,
        ICanonicalMapper canonicalMapper,
        ILogger<ApiDistributor> logger,
        IFinancialPostingService? ledger = null,
        ILedgerRepository? ledgerRepository = null,
        IBookingLedgerIndexRepository? bookingLedgerIndex = null)
    {
        _store = store;
        _systemConfigurationStore = systemConfigurationStore;
        _canonicalMapper = canonicalMapper;
        _ledger = ledger;
        _ledgerRepository = ledgerRepository;
        _bookingLedgerIndex = bookingLedgerIndex;
        _logger = logger;
    }

    public async Task<DistributionResult> DistributeAsync(
        Dictionary<string, CanonicalData> data)
    {
        var results = new Dictionary<string, TableImportResult>();
        var errors = new List<ImportError>();
        var totalImported = 0;
        var phpTables = new List<PhpTable>();
        _ids.Clear();

        foreach (var (sourceTableName, tableData) in data.OrderBy(pair => ImportRank(_canonicalMapper.NormalizeTableName(string.IsNullOrWhiteSpace(pair.Value.TableName) ? pair.Key : pair.Value.TableName))))
        {
            try
            {
                var sourceName = string.IsNullOrWhiteSpace(tableData.TableName) ? sourceTableName : tableData.TableName;
                var normalizedTable = _canonicalMapper.NormalizeTableName(sourceName);
                var year = _canonicalMapper.GetTableYear(sourceName);
                if (normalizedTable == "expenses" && year != null)
                    normalizedTable = "cash_journal";
                if (PhpHotelImporter.Handles(normalizedTable, year, tableData))
                {
                    phpTables.Add(new PhpTable(sourceName, normalizedTable, year, tableData));
                    continue;
                }

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
                    case "clients":
                        var clientResult = await ImportStoredAsync<ClientStorageModel>("clients", tableData, rememberId: true);
                        results[normalizedTable] = clientResult;
                        if (clientResult.Success) totalImported += clientResult.ImportedCount;
                        else if (clientResult.ErrorMessage != null) errors.Add(new ImportError(normalizedTable, clientResult.ErrorMessage));
                        break;
                    case "cash_registers":
                        var registerResult = await ImportStoredAsync<CashRegisterStorageModel>("cash_registers", tableData, rememberId: true);
                        results[normalizedTable] = registerResult;
                        if (registerResult.Success) totalImported += registerResult.ImportedCount;
                        else if (registerResult.ErrorMessage != null) errors.Add(new ImportError(normalizedTable, registerResult.ErrorMessage));
                        break;
                    case "tariffs":
                        var tariffResult = await ImportStoredAsync<TariffStorageModel>("tariffs", tableData, rememberId: true);
                        results[normalizedTable] = tariffResult;
                        if (tariffResult.Success) totalImported += tariffResult.ImportedCount;
                        else if (tariffResult.ErrorMessage != null) errors.Add(new ImportError(normalizedTable, tariffResult.ErrorMessage));
                        break;
                    case "years":
                        var yearResult = await ImportStoredAsync<YearStorageModel>("years", tableData, rememberId: false);
                        results[normalizedTable] = yearResult;
                        if (yearResult.Success) totalImported += yearResult.ImportedCount;
                        else if (yearResult.ErrorMessage != null) errors.Add(new ImportError(normalizedTable, yearResult.ErrorMessage));
                        break;
                    case "booking_costs":
                        var costResult = await ImportStoredAsync<BookingCostStorageModel>("booking_costs", tableData, rememberId: false, rewriteBooking: true);
                        results[normalizedTable] = costResult;
                        if (costResult.Success) totalImported += costResult.ImportedCount;
                        else if (costResult.ErrorMessage != null) errors.Add(new ImportError(normalizedTable, costResult.ErrorMessage));
                        break;
                    case "booking_guests":
                        var guestResult = await ImportStoredAsync<BookingGuestStorageModel>("booking_guests", tableData, rememberId: false, rewriteBooking: true);
                        results[normalizedTable] = guestResult;
                        if (guestResult.Success) totalImported += guestResult.ImportedCount;
                        else if (guestResult.ErrorMessage != null) errors.Add(new ImportError(normalizedTable, guestResult.ErrorMessage));
                        break;
                    case "expenses":
                        var expenseResult = await ImportStoredAsync<ExpenseStorageModel>("expenses", tableData, rememberId: false, rewriteCash: true);
                        results[normalizedTable] = expenseResult;
                        if (expenseResult.Success) totalImported += expenseResult.ImportedCount;
                        else if (expenseResult.ErrorMessage != null) errors.Add(new ImportError(normalizedTable, expenseResult.ErrorMessage));
                        break;
                    case "cancelled_bookings":
                        var cancelledResult = await ImportStoredAsync<CancelledBookingStorageModel>("cancelled_bookings", tableData, rememberId: false, rewriteBooking: true, rewriteClient: true, rewriteRoom: true);
                        results[normalizedTable] = cancelledResult;
                        if (cancelledResult.Success) totalImported += cancelledResult.ImportedCount;
                        else if (cancelledResult.ErrorMessage != null) errors.Add(new ImportError(normalizedTable, cancelledResult.ErrorMessage));
                        break;
                    case "ledger_entries":
                        var ledgerResult = await ImportLedgerAsync(tableData);
                        results[normalizedTable] = ledgerResult;
                        if (ledgerResult.Success) totalImported += ledgerResult.ImportedCount;
                        else if (ledgerResult.ErrorMessage != null) errors.Add(new ImportError(normalizedTable, ledgerResult.ErrorMessage));
                        break;
                    case "bookings":
                        var bookingResult = await ImportBookingsAsync(tableData);
                        results[normalizedTable] = bookingResult;
                        if (bookingResult.Success) totalImported += bookingResult.ImportedCount;
                        else if (bookingResult.ErrorMessage != null) errors.Add(new ImportError(normalizedTable, bookingResult.ErrorMessage));
                        break;
                    case "system_configuration":
                        var configResult = await ImportSystemConfigurationAsync(tableData);
                        results[normalizedTable] = configResult;
                        if (configResult.Success) totalImported += configResult.ImportedCount;
                        else if (configResult.ErrorMessage != null) errors.Add(new ImportError(normalizedTable, configResult.ErrorMessage));
                        break;

                    default:
                        _logger.LogInformation("Skipping table {TableName}; it is not one of the Blazor books.", sourceTableName);
                        results[sourceTableName] = new TableImportResult(sourceTableName, 0, true);
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error distributing table {TableName}", sourceTableName);
                errors.Add(new ImportError(sourceTableName, ex.Message));
            }
        }

        if (phpTables.Count > 0)
        {
            var chartConfig = await _systemConfigurationStore.GetAsync();
            var chart = AccountChart.ResolveOrUsali(chartConfig?.AccountingStandard, chartConfig?.AccountCodes);
            var phpResult = await new PhpHotelImporter(_store, _ledger, _logger, chart).ImportAsync(phpTables);
            totalImported += phpResult.TotalImported;
            foreach (var (name, result) in phpResult.ByTable)
                results[name] = result;
            errors.AddRange(phpResult.Errors);
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
                        var newId = await _store.CreateAsync<RoomDto>("rooms", room.Name, room);
                        Remember(room.Id, newId);
                        importedCount++;
                    }
                    catch (InvalidOperationException)
                    {
                        // Name already exists - update it instead
                        var existing = await _store.GetByNameAsync<RoomDto>("rooms", room.Name);
                        if (existing?.Id != null)
                        {
                            await _store.UpdateAsync<RoomDto>("rooms", existing.Id, room);
                            Remember(room.Id, existing.Id);
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
                    var newId = await _store.CreateAsync<AssetDto>("assets", asset.Name, asset);
                    Remember(asset.Id, newId);
                    imported++;
                }
                catch (InvalidOperationException)
                {
                    var existing = await _store.GetByNameAsync<AssetDto>("assets", asset.Name);
                    if (existing?.Id != null) { await _store.UpdateAsync<AssetDto>("assets", existing.Id, asset); Remember(asset.Id, existing.Id); imported++; }
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
                    var newId = await _store.CreateAsync<WarehouseDto>("warehouses", w.Name, w);
                    Remember(w.Id, newId);
                    imported++;
                }
                catch (InvalidOperationException)
                {
                    var existing = await _store.GetByNameAsync<WarehouseDto>("warehouses", w.Name);
                    if (existing?.Id != null) { await _store.UpdateAsync<WarehouseDto>("warehouses", existing.Id, w); Remember(w.Id, existing.Id); imported++; }
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
                var stored = inv with
                {
                    AssetId = Remap(inv.AssetId),
                    RoomId = Remap(inv.RoomId),
                    WarehouseId = Remap(inv.WarehouseId)
                };
                var key = string.IsNullOrWhiteSpace(inv.Id) ? $"inv-{stored.AssetId}-{stored.RoomId}-{stored.WarehouseId}-{imported}" : "inv-" + inv.Id;
                try
                {
                    await _store.CreateAsync<InventoryDto>("inventory", key, stored);
                    imported++;
                }
                catch (InvalidOperationException)
                {
                    var existing = await _store.GetByNameAsync<InventoryDto>("inventory", key);
                    if (existing?.Id != null) { await _store.UpdateAsync<InventoryDto>("inventory", existing.Id, stored); imported++; }
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

    private async Task<TableImportResult> ImportBookingsAsync(CanonicalData tableData)
    {
        try
        {
            _logger.LogInformation("Importing {Count} bookings", tableData.RowCount);
            var bookings = _canonicalMapper.FromCanonicalBookings(tableData);
            var imported = 0;

            foreach (var booking in bookings)
            {
                if (booking.Year <= 0 || string.IsNullOrWhiteSpace(booking.ClientId) || !booking.ArrivalDate.HasValue || !booking.DepartureDate.HasValue)
                {
                    _logger.LogWarning("Skipping invalid booking row. Year={Year}, ClientId={ClientId}", booking.Year, booking.ClientId);
                    continue;
                }

                var logicalKey = !string.IsNullOrWhiteSpace(booking.Id)
                    ? $"{booking.Year}_{booking.Id}"
                    : $"{booking.Year}_{Guid.NewGuid():N}";

                var storage = new BookingStorageModel
                {
                    Year = booking.Year,
                    ClientId = Remap(booking.ClientId),
                    RoomId = Remap(booking.RoomId),
                    ArrivalDate = booking.ArrivalDate,
                    DepartureDate = booking.DepartureDate,
                    Status = booking.Status,
                    Notes = booking.Notes,
                    ActualArrivalDate = booking.ActualArrivalDate,
                    ActualDepartureDate = booking.ActualDepartureDate,
                    ActualGuestCount = booking.ActualGuestCount,
                    PrepaidAmount = booking.PrepaidAmount,
                    PrepaidMethod = booking.PrepaidMethod,
                    ExpectedGuestCount = booking.ExpectedGuestCount,
                    TariffId = Remap(booking.TariffId),
                    TariffTotalPrice = booking.TariffTotalPrice
                };

                try
                {
                    var newId = await _store.CreateAsync<BookingStorageModel>("bookings", logicalKey, storage);
                    Remember(booking.Id, newId);
                    imported++;
                }
                catch (InvalidOperationException)
                {
                    var index = await _store.GetIndexAsync("bookings");
                    if (index.TryGetValue(logicalKey, out var existingId))
                    {
                        await _store.UpdateAsync("bookings", existingId, storage);
                        Remember(booking.Id, existingId);
                        imported++;
                    }
                }
            }

            return new TableImportResult(TableName: "bookings", ImportedCount: imported, Success: true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing bookings");
            return new TableImportResult(TableName: "bookings", ImportedCount: 0, Success: false, ErrorMessage: ex.Message);
        }
    }

    private async Task<TableImportResult> ImportSystemConfigurationAsync(CanonicalData tableData)
    {
        try
        {
            if (tableData.RowCount <= 0 || tableData.Rows.Count == 0)
            {
                return new TableImportResult(TableName: "system_configuration", ImportedCount: 0, Success: true);
            }

            var firstRow = tableData.Rows[0];
            var json = JsonSerializer.Serialize(firstRow);
            var imported = JsonSerializer.Deserialize<SystemConfiguration>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (imported is null)
            {
                return new TableImportResult(
                    TableName: "system_configuration",
                    ImportedCount: 0,
                    Success: false,
                    ErrorMessage: "Failed to deserialize system configuration row.");
            }

            await _systemConfigurationStore.SaveAsync(imported);

            return new TableImportResult(TableName: "system_configuration", ImportedCount: 1, Success: true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing system configuration");
            return new TableImportResult(
                TableName: "system_configuration",
                ImportedCount: 0,
                Success: false,
                ErrorMessage: ex.Message);
        }
    }

    private static int ImportRank(string table) => table switch
    {
        "system_configuration" => 0,
        "years" or "cash_registers" or "clients" or "rooms" or "assets" or "warehouses" or "tariffs" => 1,
        "inventory" => 2,
        "bookings" or "cancelled_bookings" => 3,
        "booking_costs" or "booking_guests" or "expenses" => 4,
        "ledger_entries" => 5,
        _ => 6
    };

    private async Task<TableImportResult> ImportStoredAsync<T>(
        string collection,
        CanonicalData data,
        bool rememberId,
        bool rewriteBooking = false,
        bool rewriteClient = false,
        bool rewriteRoom = false,
        bool rewriteCash = false) where T : class
    {
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var imported = 0;
        foreach (var row in data.Rows)
        {
            var oldId = Text(row, "blazorId") ?? Text(row, "id");
            var indexName = Text(row, "indexName") ?? oldId ?? Guid.NewGuid().ToString("N");
            var model = JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(row), options);
            if (model is null)
                continue;
            if (rewriteBooking)
                SetString(model, "BookingId", Remap(ReadString(model, "BookingId")));
            if (rewriteClient)
                SetString(model, "ClientId", Remap(ReadString(model, "ClientId")));
            if (rewriteRoom)
                SetString(model, "RoomId", Remap(ReadString(model, "RoomId")));
            if (rewriteCash)
                SetString(model, "CashRegisterId", Remap(ReadString(model, "CashRegisterId")));

            try
            {
                var newId = await _store.CreateAsync(collection, indexName, model);
                if (rememberId)
                    Remember(oldId, newId);
                imported++;
            }
            catch (InvalidOperationException)
            {
                var index = await _store.GetIndexAsync(collection);
                if (index.TryGetValue(indexName, out var existingId))
                {
                    await _store.UpdateAsync(collection, existingId, model);
                    if (rememberId)
                        Remember(oldId, existingId);
                    imported++;
                }
            }
        }

        return new TableImportResult(collection, imported, true);
    }

    private async Task<TableImportResult> ImportLedgerAsync(CanonicalData data)
    {
        if (_ledgerRepository is null)
            return new TableImportResult("ledger_entries", 0, false, "Ledger repository is not available.");

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var byDate = new Dictionary<DateOnly, List<LedgerEntry>>();
        foreach (var row in data.Rows)
        {
            var entry = JsonSerializer.Deserialize<LedgerEntry>(JsonSerializer.Serialize(row), options);
            if (entry is null || entry.Timestamp == default)
                continue;
            entry.BookingId = string.IsNullOrWhiteSpace(entry.BookingId) ? null : Remap(entry.BookingId);
            entry.CashRegisterId = string.IsNullOrWhiteSpace(entry.CashRegisterId) ? null : Remap(entry.CashRegisterId);
            if (string.Equals(entry.SourceType, "booking", StringComparison.OrdinalIgnoreCase))
                entry.SourceId = Remap(entry.SourceId);
            var day = DateOnly.FromDateTime(entry.Timestamp);
            if (!byDate.TryGetValue(day, out var list))
                byDate[day] = list = [];
            list.Add(entry);
        }

        var indexes = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        var count = 0;
        foreach (var (day, list) in byDate)
        {
            var ordered = list.OrderBy(entry => entry.Sequence).ToList();
            for (var i = 0; i < ordered.Count; i++)
            {
                ordered[i].Sequence = i + 1;
                ordered[i].Id = $"ledger_{day:yyyyMMdd}_{i + 1:D6}";
                if (!string.IsNullOrWhiteSpace(ordered[i].BookingId))
                {
                    if (!indexes.TryGetValue(ordered[i].BookingId!, out var ids))
                        indexes[ordered[i].BookingId!] = ids = [];
                    ids.Add(ordered[i].Id!);
                }

                count++;
            }

            await _ledgerRepository.SaveSnapshotAsync(day.ToDateTime(TimeOnly.MinValue), new LedgerSnapshot
            {
                Date = day.ToDateTime(TimeOnly.MinValue),
                Timestamp = DateTime.UtcNow,
                Entries = ordered,
                SequenceAt = ordered.Count
            });
        }

        if (_bookingLedgerIndex is not null)
        {
            foreach (var (bookingId, ids) in indexes)
                await _bookingLedgerIndex.ReplaceAsync(bookingId, ids);
        }

        return new TableImportResult("ledger_entries", count, true);
    }

    private void Remember(string? oldId, string newId)
    {
        if (!string.IsNullOrWhiteSpace(oldId))
            _ids[oldId] = newId;
    }

    private string? Remap(string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return id;
        return _ids.TryGetValue(id, out var mapped) ? mapped : id;
    }

    private static string? Text(Dictionary<string, object?> row, string key)
    {
        if (!row.TryGetValue(key, out var value) || value is null)
            return null;
        if (value is JsonElement element)
            return element.ValueKind == JsonValueKind.String ? element.GetString() : element.ToString();
        return value.ToString();
    }

    private static string? ReadString(object model, string propertyName)
    {
        var property = model.GetType().GetProperty(propertyName);
        return property?.GetValue(model) as string;
    }

    private static void SetString(object model, string propertyName, string? value)
    {
        model.GetType().GetProperty(propertyName)?.SetValue(model, value);
    }
}

