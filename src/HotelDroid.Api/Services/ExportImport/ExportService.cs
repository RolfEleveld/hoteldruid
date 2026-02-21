using HotelDroid.Api.Models;

namespace HotelDroid.Api.Services.ExportImport;

/// <summary>
/// Orchestrates the export process - collects data from APIs and creates ZIP packages
/// </summary>
public class ExportService : IExportService
{
    private readonly ICanonicalMapper _canonicalMapper;
    private readonly IPackageBuilder _packageBuilder;
    private readonly IZipBuilder _zipBuilder;
    private readonly IKeyValueStore _roomsStore;
    private readonly ILogger<ExportService> _logger;
    
    // Track in-progress exports
    private readonly Dictionary<string, ExportStatusTracker> _exportStatus = new();

    public ExportService(
        ICanonicalMapper canonicalMapper,
        IPackageBuilder packageBuilder,
        IZipBuilder zipBuilder,
        IKeyValueStore roomsStore,
        ILogger<ExportService> logger)
    {
        _canonicalMapper = canonicalMapper;
        _packageBuilder = packageBuilder;
        _zipBuilder = zipBuilder;
        _roomsStore = roomsStore;
        _logger = logger;
    }

    public async Task<string> CreateExportPackageAsync(ExportRequest options = null)
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
        _exportStatus[exportId] = tracker;

        // Run export in background
        _ = Task.Run(async () => await ExecuteExportAsync(exportId, options, tracker));

        return exportId;
    }

    public async Task<ExportStatusResponse> GetExportStatusAsync(string exportId)
    {
        if (!_exportStatus.TryGetValue(exportId, out var tracker))
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
        if (!_exportStatus.TryGetValue(exportId, out var tracker) || tracker.Status != "completed")
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
        if (_exportStatus.TryGetValue(exportId, out var tracker))
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

            // Step 1: Collect data from RoomsAPI (currently just from store)
            tracker.ProgressPercent = 20;
            var rooms = await CollectRoomsDataAsync();
            _logger.LogInformation("Collected {Count} rooms", rooms.Length);

            if (rooms.Length == 0)
            {
                _logger.LogWarning("No rooms found for export");
            }

            // Step 2: Convert to canonical format
            tracker.ProgressPercent = 40;
            var canonicalData = new Dictionary<string, CanonicalData>
            {
                ["rooms"] = _canonicalMapper.ToCanonical(rooms)
            };

            // Step 3: Build package
            tracker.ProgressPercent = 60;
            var package = await _packageBuilder.BuildAsync(canonicalData, options);

            // Step 4: Create ZIP
            tracker.ProgressPercent = 80;
            var zipPath = await _zipBuilder.CreateZipAsync(package);
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
            // Get all rooms from the file store
            var rooms = await _roomsStore.ListAsync<RoomDto>("rooms");
            return rooms.ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error collecting rooms data");
            throw;
        }
    }

    private string ExtractExportIdFromFileName(string fileName)
    {
        // Format: export_hoteldruid_20260221_103000_v1.zip
        // Extract timestamp as ID or use a different strategy
        return fileName.Replace("export_hoteldruid_", "").Replace("_v1.zip", "");
    }

    private class ExportStatusTracker
    {
        public string ExportId { get; set; }
        public string Status { get; set; } = "pending"; // pending, processing, completed, failed, cancelled
        public int ProgressPercent { get; set; } = 0;
        public long? FileSizeBytes { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
