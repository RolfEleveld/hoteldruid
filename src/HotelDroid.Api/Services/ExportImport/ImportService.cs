using System.Text.Json;
using System.Text.Json.Serialization;
using HotelDroid.Api.Models;

namespace HotelDroid.Api.Services.ExportImport;

/// <summary>
/// Orchestrates the import process - validates packages and distributes data
/// </summary>
public class ImportService : IImportService
{
    private readonly IZipBuilder _zipBuilder;
    private readonly ICanonicalMapper _canonicalMapper;
    private readonly IApiDistributor _apiDistributor;
    private readonly ILogger<ImportService> _logger;

    // JSON options matching PackageBuilder for consistency
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    // Track in-progress imports
    private readonly Dictionary<string, ImportStatusTracker> _importStatus = new();
    private readonly Dictionary<string, ImportPackageInfo> _validatedPackages = new();

    public ImportService(
        IZipBuilder zipBuilder,
        ICanonicalMapper canonicalMapper,
        IApiDistributor apiDistributor,
        ILogger<ImportService> logger)
    {
        _zipBuilder = zipBuilder;
        _canonicalMapper = canonicalMapper;
        _apiDistributor = apiDistributor;
        _logger = logger;
    }

    public async Task<ImportValidationResponse> ValidatePackageAsync(IFormFile zipFile)
    {
        var packageId = Guid.NewGuid().ToString();

        try
        {
            _logger.LogInformation("Validating package {PackageId}", packageId);

            // Extract ZIP
            var tempDir = await _zipBuilder.ExtractZipAsync(zipFile);

            // Read and validate manifest
            var manifestPath = Path.Combine(tempDir, "manifest.json");
            if (!File.Exists(manifestPath))
            {
                return new ImportValidationResponse(
                    Valid: false,
                    PackageId: packageId,
                    Manifest: null,
                    Tables: new(),
                    NameMapping: new(),
                    Errors: new() { "manifest.json not found in package" }
                );
            }

            var manifestJson = await File.ReadAllTextAsync(manifestPath);
            var manifest = JsonSerializer.Deserialize<Manifest>(manifestJson, JsonOptions);

            // Discover tables
            var tablesDir = Path.Combine(tempDir, "data", "tables");
            var tables = new List<TablePreview>();
            var errors = new List<string>();
            var nameMapping = new Dictionary<string, string>();

            if (Directory.Exists(tablesDir))
            {
                foreach (var tableFile in Directory.GetFiles(tablesDir, "*.json"))
                {
                    try
                    {
                        var tableJson = await File.ReadAllTextAsync(tableFile);
                        var table = JsonSerializer.Deserialize<CanonicalData>(tableJson, JsonOptions);

                        if (table != null)
                        {
                            var normalized = _canonicalMapper.NormalizeTableName(table.TableName);
                            var exportName = _canonicalMapper.GetExportTableName(normalized);

                            var preview = new TablePreview(
                                Name: exportName,
                                SourceNames: new() { table.TableName },
                                RowCount: table.RowCount,
                                Columns: table.Columns.Select(c => c.Name).ToList()
                            );

                            tables.Add(preview);

                            // Add to mapping info
                            nameMapping[table.TableName] = exportName;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error reading table file {File}", tableFile);
                        errors.Add($"Error reading table file: {Path.GetFileName(tableFile)}");
                    }
                }
            }
            else
            {
                errors.Add("data/tables directory not found in package");
            }

            // Store validated package info
            _validatedPackages[packageId] = new ImportPackageInfo(
                PackageId: packageId,
                FileName: zipFile.FileName,
                FileSizeBytes: zipFile.Length,
                Manifest: manifest,
                Tables: tables,
                UploadedAt: DateTime.UtcNow
            );

            // Clean up temp directory
            try { Directory.Delete(tempDir, true); } catch { }

            var isValid = !errors.Any();
            _logger.LogInformation("Package {PackageId} validation completed. Valid: {IsValid}", packageId, isValid);

            return new ImportValidationResponse(
                Valid: isValid,
                PackageId: packageId,
                Manifest: new ManifestInfo(
                    ExportTimestamp: manifest?.ExportTimestamp.ToString("O") ?? "unknown",
                    SourceSystem: manifest?.SourceSystem.Application ?? "unknown",
                    Version: manifest?.SourceSystem.Version
                ),
                Tables: tables,
                NameMapping: nameMapping,
                Errors: errors
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating package {PackageId}", packageId);
            return new ImportValidationResponse(
                Valid: false,
                PackageId: packageId,
                Manifest: null,
                Tables: new(),
                NameMapping: new(),
                Errors: new() { $"Validation error: {ex.Message}" }
            );
        }
    }

    public async Task<ImportValidationResponse> GetImportPreviewAsync(string packageId)
    {
        if (!_validatedPackages.TryGetValue(packageId, out var pkgInfo))
        {
            throw new KeyNotFoundException($"Package {packageId} not found");
        }

        var nameMapping = new Dictionary<string, string>();
        foreach (var table in pkgInfo.Tables)
        {
            foreach (var sourceName in table.SourceNames)
            {
                nameMapping[sourceName] = table.Name;
            }
        }

        return new ImportValidationResponse(
            Valid: true,
            PackageId: packageId,
            Manifest: new ManifestInfo(
                ExportTimestamp: pkgInfo.Manifest?.ExportTimestamp.ToString("O") ?? "unknown",
                SourceSystem: pkgInfo.Manifest?.SourceSystem.Application ?? "unknown",
                Version: pkgInfo.Manifest?.SourceSystem.Version
            ),
            Tables: pkgInfo.Tables,
            NameMapping: nameMapping,
            Errors: new()
        );
    }

    public async Task<ImportExecuteResponse> ExecuteImportAsync(
        string packageId,
        ImportExecuteRequest request)
    {
        var importId = Guid.NewGuid().ToString();

        if (!_validatedPackages.TryGetValue(packageId, out var pkgInfo))
        {
            throw new KeyNotFoundException($"Package {packageId} not found");
        }

        _logger.LogInformation("Executing import {ImportId} from package {PackageId}", importId, packageId);

        var tracker = new ImportStatusTracker
        {
            ImportId = importId,
            Status = "processing",
            CreatedAt = DateTime.UtcNow
        };
        _importStatus[importId] = tracker;

        // Run import in background
        _ = Task.Run(async () => await ExecuteImportInternalAsync(importId, pkgInfo, request, tracker));

        return new ImportExecuteResponse(
            ImportId: importId,
            Status: "processing",
            StatusUrl: $"/api/import/{importId}/status"
        );
    }

    public async Task<ImportStatusResponse> GetImportStatusAsync(string importId)
    {
        if (!_importStatus.TryGetValue(importId, out var tracker))
        {
            throw new KeyNotFoundException($"Import {importId} not found");
        }

        return new ImportStatusResponse(
            ImportId: importId,
            Status: tracker.Status,
            ProgressPercent: tracker.ProgressPercent,
            TablesProcessed: tracker.TablesProcessed,
            TotalRows: tracker.TotalRows,
            Errors: tracker.Errors,
            CompletedAt: tracker.CompletedAt
        );
    }

    public async Task<bool> CancelImportAsync(string importId)
    {
        if (_importStatus.TryGetValue(importId, out var tracker))
        {
            if (tracker.Status == "processing")
            {
                tracker.Status = "cancelled";
                return true;
            }
        }
        return false;
    }

    private async Task ExecuteImportInternalAsync(
        string importId,
        ImportPackageInfo pkgInfo,
        ImportExecuteRequest request,
        ImportStatusTracker tracker)
    {
        var tempDir = "";

        try
        {
            _logger.LogInformation("Starting import {ImportId}", importId);

            // Step 1: Re-extract package (since temp dir was deleted after validation)
            tracker.ProgressPercent = 10;
            
            // Create a temporary in-memory copy or re-extract from original
            // For now, assume package is accessible via pkgInfo
            tempDir = Path.Combine(Path.GetTempPath(), $"import_{importId}");
            Directory.CreateDirectory(tempDir);

            // Step 2: Load canonical data
            tracker.ProgressPercent = 30;
            var canonicalData = new Dictionary<string, CanonicalData>();
            var tablesDir = Path.Combine(tempDir, "data", "tables");

            if (Directory.Exists(tablesDir))
            {
                foreach (var tableFile in Directory.GetFiles(tablesDir, "*.json"))
                {
                    try
                    {
                        var tableJson = await File.ReadAllTextAsync(tableFile);
                        var table = JsonSerializer.Deserialize<CanonicalData>(tableJson);
                        if (table != null)
                        {
                            canonicalData[table.TableName] = table;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error loading table file {File}", tableFile);
                    }
                }
            }

            if (canonicalData.Count == 0)
            {
                throw new InvalidOperationException("No valid tables found in package");
            }

            // Step 3: Distribute to APIs
            tracker.ProgressPercent = 50;
            if (!request.DryRun)
            {
                var distributionResult = await _apiDistributor.DistributeAsync(canonicalData);

                tracker.TablesProcessed = distributionResult.ByTable.Count;
                tracker.TotalRows = distributionResult.TotalImported;

                foreach (var error in distributionResult.Errors)
                {
                    tracker.Errors.Add($"{error.TableName}: {error.ErrorMessage}");
                }
            }
            else
            {
                // Dry-run: just count what would be imported
                tracker.TablesProcessed = canonicalData.Count;
                tracker.TotalRows = canonicalData.Values.Sum(t => t.RowCount);
            }

            tracker.ProgressPercent = 100;
            tracker.Status = "completed";
            tracker.CompletedAt = DateTime.UtcNow;

            _logger.LogInformation(
                "Import {ImportId} completed. Tables: {Tables}, Rows: {Rows}",
                importId, tracker.TablesProcessed, tracker.TotalRows);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Import {ImportId} failed", importId);
            tracker.Status = "failed";
            tracker.Errors.Add($"Import failed: {ex.Message}");
        }
        finally
        {
            // Clean up temp directory
            try { if (Directory.Exists(tempDir)) Directory.Delete(tempDir, true); } catch { }
        }
    }

    private class ImportStatusTracker
    {
        public string ImportId { get; set; }
        public string Status { get; set; } = "pending";
        public int ProgressPercent { get; set; } = 0;
        public int TablesProcessed { get; set; } = 0;
        public int TotalRows { get; set; } = 0;
        public List<string> Errors { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
