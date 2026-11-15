using System.Collections.Concurrent;
using System.Text.Json;
using HotelDruid.Api.Models;
using HotelDruid.Shared;
using HotelDruid.Shared.Configuration;

namespace HotelDruid.Api.Services.ExportImport;

/// <summary>
/// Orchestrates the export process - collects data from APIs and creates ZIP packages
/// </summary>
public class ExportService : IExportService
{
    private readonly ICanonicalMapper _canonicalMapper;
    private readonly IPackageBuilder _packageBuilder;
    private readonly IZipBuilder _zipBuilder;
    private readonly IKeyValueStore _store;
    private readonly ILedgerRepository? _ledger;
    private readonly ILogger<ExportService> _logger;
    
    // Shared across requests. A scoped service would forget an export as soon as the create request ended.
    private static readonly ConcurrentDictionary<string, ExportStatusTracker> ExportStatus = new();

    public ExportService(
        ICanonicalMapper canonicalMapper,
        IPackageBuilder packageBuilder,
        IZipBuilder zipBuilder,
        IKeyValueStore store,
        ILogger<ExportService> logger,
        ILedgerRepository? ledger = null)
    {
        _canonicalMapper = canonicalMapper;
        _packageBuilder = packageBuilder;
        _zipBuilder = zipBuilder;
        _store = store;
        _ledger = ledger;
        _logger = logger;
    }

    public async Task<string> CreateExportPackageAsync(ExportRequest? options = null)
    {
        var exportId = Guid.NewGuid().ToString();
        options ??= new ExportRequest();

        _logger.LogInformation("Starting export {ExportId}", exportId);

        // Track status
        var tracker = new ExportStatusTracker 
        { 
            ExportId = exportId,
            Status = "processing",
            CreatedAt = DateTime.UtcNow
        };
        ExportStatus[exportId] = tracker;
        await ExecuteExportAsync(exportId, options, tracker);
        return exportId;
    }

    public async Task<ExportStatusResponse> GetExportStatusAsync(string exportId)
    {
        if (!ExportStatus.TryGetValue(exportId, out var tracker))
        {
            _logger.LogWarning("Export {ExportId} not found", exportId);
            throw new KeyNotFoundException($"Export {exportId} not found");
        }

        return new ExportStatusResponse(
            ExportId: exportId,
            Status: tracker.Status,
            ProgressPercent: tracker.ProgressPercent,
            DownloadUrl: tracker.Status == "completed" 
                ? $"/api/export/{exportId}/download" 
                : null,
            FileSizeBytes: tracker.FileSizeBytes,
            CreatedAt: tracker.CreatedAt
        );
    }

    public async Task<(Stream FileStream, string FileName)> DownloadExportAsync(string exportId)
    {
        if (!ExportStatus.TryGetValue(exportId, out var tracker) || tracker.Status != "completed")
        {
            throw new InvalidOperationException($"Export {exportId} is not ready for download");
        }

        var (filePath, fileName) = await _zipBuilder.GetExportFileAsync(exportId);
        
        var stream = File.OpenRead(filePath);
        return (stream, fileName);
    }

    public async Task<List<ExportStatusResponse>> ListExportsAsync(int limit = 20, int offset = 0)
    {
        var allExports = await _zipBuilder.ListExportsAsync(limit, offset);
        
        return allExports.Select(f => new ExportStatusResponse(
            ExportId: ExtractExportIdFromFileName(f.FileName),
            Status: "completed",
            ProgressPercent: 100,
            DownloadUrl: $"/api/export/{ExtractExportIdFromFileName(f.FileName)}/download",
            FileSizeBytes: f.FileSizeBytes,
            CreatedAt: f.CreatedAt
        )).ToList();
    }

    public async Task<bool> CancelExportAsync(string exportId)
    {
        if (ExportStatus.TryGetValue(exportId, out var tracker))
        {
            if (tracker.Status == "processing")
            {
                tracker.Status = "cancelled";
                return true;
            }
        }
        return false;
    }

    private async Task ExecuteExportAsync(string exportId, ExportRequest options, ExportStatusTracker tracker)
    {
        try
        {
            _logger.LogInformation("Executing export {ExportId}", exportId);

            // Step 1: Collect data from stores
            tracker.ProgressPercent = 20;
            var rooms = await CollectRoomsDataAsync();
            var assets = await CollectAssetsAsync();
            var warehouses = await CollectWarehousesAsync();
            var inventory = await CollectInventoryAsync();
            var bookings = await CollectBookingsDataAsync();
            var systemConfigurations = await _store.ListAsync<SystemConfiguration>("system_configuration");
            var clients = await CollectDocumentsAsync("clients");
            var tariffs = await CollectDocumentsAsync("tariffs");
            var cashRegisters = await CollectDocumentsAsync("cash_registers");
            var years = await CollectDocumentsAsync("years");
            var bookingCosts = await CollectDocumentsAsync("booking_costs");
            var bookingGuests = await CollectDocumentsAsync("booking_guests");
            var expenses = await CollectDocumentsAsync("expenses");
            var cancelled = await CollectDocumentsAsync("cancelled_bookings");
            var ledgerRows = await CollectLedgerAsync();

            _logger.LogInformation("Collected {RoomCount} rooms, {AssetCount} assets, {WhCount} warehouses, {InvCount} inventory rows, {BookingCount} bookings, {ClientCount} clients, {LedgerCount} ledger lines",
                rooms.Length, assets.Length, warehouses.Length, inventory.Length, bookings.Length, clients.RowCount, ledgerRows.RowCount);

            // Step 2: Convert to canonical format
            tracker.ProgressPercent = 40;
            var canonicalData = new Dictionary<string, CanonicalData>
            {
                ["years"] = years,
                ["cash_registers"] = cashRegisters,
                ["clients"] = clients,
                ["rooms"] = _canonicalMapper.ToCanonical(rooms),
                ["assets"] = _canonicalMapper.ToCanonicalAssets(assets),
                ["warehouses"] = _canonicalMapper.ToCanonicalWarehouses(warehouses),
                ["tariffs"] = tariffs,
                ["inventory"] = _canonicalMapper.ToCanonicalInventory(inventory),
                ["bookings"] = _canonicalMapper.ToCanonicalBookings(bookings),
                ["booking_guests"] = bookingGuests,
                ["booking_costs"] = bookingCosts,
                ["cancelled_bookings"] = cancelled,
                ["expenses"] = expenses,
                ["ledger_entries"] = ledgerRows
            };

            if (systemConfigurations.Count > 0)
            {
                canonicalData["system_configuration"] = ToCanonicalSystemConfiguration(systemConfigurations);
            }

            CanonicalReferences.Apply(canonicalData);

            // Step 3: Build package
            tracker.ProgressPercent = 60;
            var package = await _packageBuilder.BuildAsync(canonicalData, options);

            // Step 4: Create ZIP
            tracker.ProgressPercent = 80;
            var zipPath = await _zipBuilder.CreateZipAsync(package);
            var namedPath = Path.Combine(Path.GetDirectoryName(zipPath)!, $"export_HotelDruid_{exportId}_v1.zip");
            if (!string.Equals(zipPath, namedPath, StringComparison.OrdinalIgnoreCase))
            {
                File.Move(zipPath, namedPath, overwrite: true);
                zipPath = namedPath;
            }
            var fileSize = new FileInfo(zipPath).Length;
            tracker.FileSizeBytes = fileSize;

            // Mark as completed
            tracker.ProgressPercent = 100;
            tracker.Status = "completed";
            
            _logger.LogInformation("Export {ExportId} completed successfully. ZIP size: {Size} bytes", 
                exportId, fileSize);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Export {ExportId} failed", exportId);
            tracker.Status = "failed";
            tracker.ErrorMessage = ex.Message;
        }
    }

    private async Task<RoomDto[]> CollectRoomsDataAsync()
    {
        try
        {
            var index = await _store.GetIndexAsync("rooms");
            var rooms = new List<RoomDto>();
            foreach (var (_, id) in index)
            {
                var storage = await _store.GetAsync<RoomStorageModel>("rooms", id);
                if (storage is null || string.IsNullOrWhiteSpace(storage.Name))
                    continue;
                rooms.Add(new RoomDto(
                    id,
                    storage.Name,
                    storage.Capacity ?? 0,
                    storage.FloorNumber,
                    storage.HouseNumber,
                    storage.Priority,
                    storage.SecondaryPriority,
                    storage.HasBeds,
                    storage.NeighboringRooms,
                    storage.Comments));
            }

            return rooms.ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error collecting rooms data");
            throw;
        }
    }

    private async Task<AssetDto[]> CollectAssetsAsync()
    {
        var index = await _store.GetIndexAsync("assets");
        var rows = new List<AssetDto>();
        foreach (var (_, id) in index)
        {
            var storage = await _store.GetAsync<AssetStorageModel>("assets", id);
            if (storage is null)
                continue;
            rows.Add(new AssetDto(id, storage.Name, storage.Code, storage.Description, storage.CreatedAt));
        }

        return rows.ToArray();
    }

    private async Task<WarehouseDto[]> CollectWarehousesAsync()
    {
        var index = await _store.GetIndexAsync("warehouses");
        var rows = new List<WarehouseDto>();
        foreach (var (_, id) in index)
        {
            var storage = await _store.GetAsync<WarehouseStorageModel>("warehouses", id);
            if (storage is null)
                continue;
            rows.Add(new WarehouseDto(id, storage.Name, storage.Code, storage.Description, storage.FloorNumber, storage.HouseNumber, storage.CreatedAt));
        }

        return rows.ToArray();
    }

    private async Task<InventoryDto[]> CollectInventoryAsync()
    {
        var index = await _store.GetIndexAsync("inventory");
        var rows = new List<InventoryDto>();
        foreach (var (_, id) in index)
        {
            var storage = await _store.GetAsync<InventoryStorageModel>("inventory", id);
            if (storage is null)
                continue;
            rows.Add(new InventoryDto(id, storage.AssetId, storage.RoomId, storage.WarehouseId, storage.Quantity, storage.MinQuantityDefault, storage.RequiredOnCheckin, storage.CreatedAt));
        }

        return rows.ToArray();
    }

    private async Task<CanonicalData> CollectDocumentsAsync(string collection)
    {
        var index = await _store.GetIndexAsync(collection);
        var rows = new List<Dictionary<string, object?>>();
        foreach (var (name, id) in index)
        {
            var document = await _store.GetAsync<Dictionary<string, JsonElement>>(collection, id);
            if (document is null)
                continue;
            var row = new Dictionary<string, object?>
            {
                ["blazorId"] = id,
                ["indexName"] = name
            };
            foreach (var (key, value) in document)
                row[key] = value.ValueKind == JsonValueKind.Null ? null : value;
            rows.Add(row);
        }

        var columns = new List<ColumnDefinition>
        {
            new("blazorId", "string", false, "Store id"),
            new("indexName", "string", false, "Store index name")
        };
        return new CanonicalData(collection, rows.Count, rows, columns);
    }

    private async Task<CanonicalData> CollectLedgerAsync()
    {
        var rows = new List<Dictionary<string, object?>>();
        if (_ledger is not null)
        {
            foreach (var entry in await _ledger.ReadAllAsync())
            {
                rows.Add(new Dictionary<string, object?>
                {
                    ["blazorId"] = entry.Id,
                    ["id"] = entry.Id,
                    ["sequence"] = entry.Sequence,
                    ["timestamp"] = entry.Timestamp,
                    ["entryType"] = entry.EntryType,
                    ["bookingId"] = entry.BookingId,
                    ["journalId"] = entry.JournalId,
                    ["accountCode"] = entry.AccountCode,
                    ["accountGroup"] = entry.AccountGroup,
                    ["debit"] = entry.Debit,
                    ["credit"] = entry.Credit,
                    ["amount"] = entry.Amount,
                    ["cashRegisterId"] = entry.CashRegisterId,
                    ["payerName"] = entry.PayerName,
                    ["sourceType"] = entry.SourceType,
                    ["sourceId"] = entry.SourceId,
                    ["method"] = entry.Method,
                    ["status"] = entry.Status,
                    ["createdBy"] = entry.CreatedBy,
                    ["appliedTo"] = entry.AppliedTo,
                    ["notes"] = entry.Notes
                });
            }
        }

        return new CanonicalData("ledger_entries", rows.Count, rows, [new ColumnDefinition("blazorId", "string", false, "Ledger line id")]);
    }

    private async Task<BookingDto[]> CollectBookingsDataAsync()
    {
        try
        {
            var index = await _store.GetIndexAsync("bookings");
            var bookings = new List<BookingDto>();

            foreach (var kvp in index)
            {
                var storage = await _store.GetAsync<BookingStorageModel>("bookings", kvp.Value);
                if (storage is null)
                {
                    continue;
                }

                bookings.Add(new BookingDto(
                    Id: kvp.Value,
                    Year: storage.Year,
                    ClientId: storage.ClientId,
                    RoomId: storage.RoomId,
                    ArrivalDate: storage.ArrivalDate,
                    DepartureDate: storage.DepartureDate,
                    Status: storage.Status,
                    Notes: storage.Notes,
                    ActualArrivalDate: storage.ActualArrivalDate,
                    ActualGuestCount: storage.ActualGuestCount,
                    PrepaidAmount: storage.PrepaidAmount,
                    PrepaidMethod: storage.PrepaidMethod,
                    ActualDepartureDate: storage.ActualDepartureDate,
                    ExpectedGuestCount: storage.ExpectedGuestCount,
                    TariffId: storage.TariffId,
                    TariffTotalPrice: storage.TariffTotalPrice));
            }

            return bookings.ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error collecting bookings data");
            throw;
        }
    }

    private static CanonicalData ToCanonicalSystemConfiguration(List<SystemConfiguration> configurations)
    {
        var columns = new List<ColumnDefinition>
        {
            new("id", "string", false, "Configuration key"),
            new("createdUtc", "string", true, "Created timestamp UTC"),
            new("updatedUtc", "string", true, "Updated timestamp UTC"),
            new("defaultCurrency", "string", true, "Default currency"),
            new("defaultYear", "number", true, "Default year"),
            new("accountingStandard", "string", true, "Chart of accounts standard"),
            new("accountCodes", "object", true, "Account codes for the six ledger buckets"),
            new("adminEmails", "array", true, "Configured administrator emails"),
            new("settings", "object", true, "Additional settings dictionary")
        };

        var rows = configurations.Select(c => new Dictionary<string, object?>
        {
            ["id"] = c.Id,
            ["createdUtc"] = c.CreatedUtc.ToString("O"),
            ["updatedUtc"] = c.UpdatedUtc.ToString("O"),
            ["defaultCurrency"] = c.DefaultCurrency,
            ["defaultYear"] = c.DefaultYear,
            ["accountingStandard"] = c.AccountingStandard,
            ["accountCodes"] = c.AccountCodes,
            ["adminEmails"] = c.AdminEmails,
            ["settings"] = c.Settings
        }).ToList();

        return new CanonicalData(
            TableName: "system_configuration",
            RowCount: rows.Count,
            Rows: rows,
            Columns: columns);
    }

    private string ExtractExportIdFromFileName(string fileName)
    {
        // Format: export_HotelDruid_20260221_103000_v1.zip
        // Extract timestamp as ID or use a different strategy
        return fileName.Replace("export_HotelDruid_", "").Replace("_v1.zip", "");
    }

    private class ExportStatusTracker
    {
        public string ExportId { get; set; } = string.Empty;
        public string Status { get; set; } = "pending"; // pending, processing, completed, failed, cancelled
        public int ProgressPercent { get; set; } = 0;
        public long? FileSizeBytes { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

