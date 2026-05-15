using HotelDruid.Api.Models;
using HotelDruid.Shared;

namespace HotelDruid.Api.Services.ExportImport;

/// <summary>
/// Export/Import service interfaces
/// </summary>

public interface ICanonicalMapper
{
    /// <summary>
    /// Normalize table name to canonical form (liberal inbound)
    /// Accepts: "appartamenti", "apartments", "rooms", etc.
    /// Returns: canonical name for internal use
    /// </summary>
    string NormalizeTableName(string inputTableName);

    /// <summary>
    /// Get export table name (conservative outbound)
    /// Returns industry-standard name ("rooms")
    /// </summary>
    string GetExportTableName(string normalizedTableName);

    /// <summary>
    /// Get HotelDruid-compatible table name for backward compatibility
    /// Returns: Original HotelDruid name ("appartamenti")
    /// </summary>
    string GetHotelDruidTableName(string normalizedTableName);

    /// <summary>
    /// Normalize field name to canonical form
    /// Accepts: "idappartamenti", "room_id", "id", etc.
    /// </summary>
    string NormalizeFieldName(string inputFieldName);

    /// <summary>
    /// Convert .NET RoomDto objects to canonical JSON format (for export)
    /// </summary>
    CanonicalData ToCanonical(RoomDto[] rooms);

    /// <summary>
    /// Convert canonical JSON data to .NET RoomDto objects (for import)
    /// Handles flexible inbound naming
    /// </summary>
    RoomDto[] FromCanonical(CanonicalData data);

    // Assets / Inventory
    CanonicalData ToCanonicalAssets(AssetDto[] assets);
    AssetDto[] FromCanonicalAssets(CanonicalData data);

    CanonicalData ToCanonicalWarehouses(WarehouseDto[] warehouses);
    WarehouseDto[] FromCanonicalWarehouses(CanonicalData data);

    CanonicalData ToCanonicalInventory(InventoryDto[] inventory);
    InventoryDto[] FromCanonicalInventory(CanonicalData data);

    /// <summary>
    /// Get value from row with flexible field name handling
    /// Handles all aliases
    /// </summary>
    object? GetNormalizedValue(Dictionary<string, object?> row, string canonicalFieldName);
}

public interface IPackageBuilder
{
    /// <summary>
    /// Build HotelDruid-compatible package structure from canonical data
    /// </summary>
    Task<ExportPackage> BuildAsync(
        Dictionary<string, CanonicalData> data,
        ExportRequest options);
}

public interface IZipBuilder
{
    /// <summary>
    /// Create ZIP file from export package
    /// Returns path to created ZIP file
    /// </summary>
    Task<string> CreateZipAsync(ExportPackage package);

    /// <summary>
    /// List all exported ZIP files
    /// </summary>
    Task<List<ZipFileMetadata>> ListExportsAsync(int limit = 20, int offset = 0);

    /// <summary>
    /// Extract ZIP to temporary directory and return path
    /// </summary>
    Task<string> ExtractZipAsync(IFormFile zipFile);

    /// <summary>
    /// Get export ZIP file for download
    /// </summary>
    Task<(string FilePath, string FileName)> GetExportFileAsync(string exportId);
}

public interface IExportService
{
    /// <summary>
    /// Create complete export package from all APIs
    /// Returns export ID for tracking
    /// </summary>
    Task<string> CreateExportPackageAsync(ExportRequest? options = null);

    /// <summary>
    /// Get export status and download info
    /// </summary>
    Task<ExportStatusResponse> GetExportStatusAsync(string exportId);

    /// <summary>
    /// Download exported ZIP file
    /// </summary>
    Task<(Stream FileStream, string FileName)> DownloadExportAsync(string exportId);

    /// <summary>
    /// List all available exports
    /// </summary>
    Task<List<ExportStatusResponse>> ListExportsAsync(int limit = 20, int offset = 0);

    /// <summary>
    /// Cancel an export in progress
    /// </summary>
    Task<bool> CancelExportAsync(string exportId);
}

public interface IImportService
{
    /// <summary>
    /// Validate import package (ZIP file)
    /// Returns validation result with table preview
    /// </summary>
    Task<ImportValidationResponse> ValidatePackageAsync(IFormFile zipFile);

    /// <summary>
    /// Get preview of data that will be imported
    /// Shows sample rows and row counts
    /// </summary>
    Task<ImportValidationResponse> GetImportPreviewAsync(string packageId);

    /// <summary>
    /// Execute import - actually import data to APIs
    /// </summary>
    Task<ImportExecuteResponse> ExecuteImportAsync(
        string packageId,
        ImportExecuteRequest request);

    /// <summary>
    /// Get import status and results
    /// </summary>
    Task<ImportStatusResponse> GetImportStatusAsync(string importId);

    /// <summary>
    /// Cancel import in progress
    /// </summary>
    Task<bool> CancelImportAsync(string importId);
}

public interface IApiDistributor
{
    /// <summary>
    /// Distribute imported data to appropriate APIs
    /// For now, just handles rooms
    /// </summary>
    Task<DistributionResult> DistributeAsync(
        Dictionary<string, CanonicalData> data);
}

public record DistributionResult(
    int TotalImported,
    Dictionary<string, TableImportResult> ByTable,
    List<ImportError> Errors
);

