namespace HotelDroid.Api.Models;

/// <summary>
/// Models for export/import functionality
/// </summary>

// ==================== Request/Response Models ====================

public record ExportRequest(
    bool IncludeConfigs = true,
    bool IncludeDocs = true,
    string ExportType = "full"
);

public record ExportResponse(
    string ExportId,
    string Status,
    string StatusUrl,
    int EstimatedSeconds = 5
);

public record ExportStatusResponse(
    string ExportId,
    string Status,
    int ProgressPercent,
    string? DownloadUrl,
    long? FileSizeBytes,
    DateTime CreatedAt
);

public record ImportValidationRequest(
    string PackageId,
    bool DryRun = false
);

public record ImportValidationResponse(
    bool Valid,
    string PackageId,
    ManifestInfo? Manifest,
    List<TablePreview> Tables,
    Dictionary<string, string> NameMapping,
    List<string> Errors
);

public record TablePreview(
    string Name,
    List<string> SourceNames,
    int RowCount,
    List<string> Columns
);

public record ManifestInfo(
    string ExportTimestamp,
    string SourceSystem,
    string? Version
);

public record ImportExecuteRequest(
    string PackageId,
    bool DryRun = false,
    Dictionary<string, object>? MappingOverrides = null
);

public record ImportExecuteResponse(
    string ImportId,
    string Status,
    string StatusUrl
);

public record ImportStatusResponse(
    string ImportId,
    string Status,
    int ProgressPercent,
    int TablesProcessed,
    int TotalRows,
    List<string> Errors,
    DateTime? CompletedAt
);

// ==================== Canonical Data Models ====================

public record CanonicalData(
    string TableName,
    int RowCount,
    List<Dictionary<string, object?>> Rows,
    List<ColumnDefinition> Columns
);

public record ColumnDefinition(
    string Name,
    string Type,
    bool Nullable,
    string? Description = null
);

public record CanonicalRow(Dictionary<string, object?> Data);

// ==================== Package Models ====================

public record ExportPackage(
    Manifest Manifest,
    List<TableFile> DataTables,
    List<SchemaFile> SchemaFiles,
    List<ConfigFile> ConfigFiles,
    List<DocumentFile> DocumentFiles
);

public record Manifest(
    string ExportId,
    DateTime ExportTimestamp,
    SourceSystem SourceSystem,
    SourceMachine SourceMachine,
    string ExportFormatVersion = "1.0.0",
    string ExportType = "full"
);

public record SourceSystem(
    string Application,
    string Version,
    string DatabaseType
);

public record SourceMachine(
    string HostName,
    string SystemName,
    string HostingSystem,
    string ExportedBy
);

public record TableFile(
    string FileName,
    string Content
);

public record SchemaFile(
    string FileName,
    string Content
);

public record ConfigFile(
    string FileName,
    string Content
);

public record DocumentFile(
    string FileName,
    string Content
);

public record ExportMetadata(
    string ExportId,
    DateTime ExportTimestamp,
    int ExportDurationSeconds,
    ExportScope ExportScope,
    Dictionary<string, bool> ExportOptions
);

public record ExportScope(
    string ExportType,
    int TotalTables,
    int TotalRows,
    int TotalConfigFiles
);

// ==================== Import Models ====================

public record ImportPackageInfo(
    string PackageId,
    string FileName,
    long FileSizeBytes,
    Manifest? Manifest,
    List<TablePreview> Tables,
    DateTime UploadedAt
);

public record ImportExecutionResult(
    int TablesProcessed,
    int TotalRowsImported,
    Dictionary<string, TableImportResult> ByTable,
    List<ImportError> Errors
);

public record TableImportResult(
    string TableName,
    int ImportedCount,
    bool Success,
    string? ErrorMessage = null
);

public record ImportError(
    string TableName,
    string ErrorMessage,
    int? RowNumber = null
);

// ==================== Naming Models ====================

public record NamingConfiguration(
    Dictionary<string, string> TableAliases,
    Dictionary<string, string> FieldAliases,
    ExportNamingConfig ExportNames
);

public record ExportNamingConfig(
    string TableName,
    Dictionary<string, string> Fields
);

// ==================== Format Models ====================

public record ZipFileMetadata(
    string FileName,
    long FileSizeBytes,
    DateTime CreatedAt
);
