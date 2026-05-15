using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using HotelDruid.Api.Models;

namespace HotelDruid.Api.Services.ExportImport;

/// <summary>
/// Builds HotelDruid-compatible package structure from canonical data
/// </summary>
public class PackageBuilder : IPackageBuilder
{
    private readonly ICanonicalMapper _canonicalMapper;
    private readonly ILogger<PackageBuilder> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true  // Allow case-insensitive deserialization
    };

    public PackageBuilder(
        ICanonicalMapper canonicalMapper,
        ILogger<PackageBuilder> logger)
    {
        _canonicalMapper = canonicalMapper;
        _logger = logger;
    }

    public async Task<ExportPackage> BuildAsync(
        Dictionary<string, CanonicalData> data,
        ExportRequest options)
    {
        _logger.LogInformation("Building export package with {TableCount} tables", data.Count);

        // Create manifest
        var manifest = CreateManifest(options);

        // Create data files
        var dataTableFiles = new List<TableFile>();
        foreach (var (_, tableData) in data)
        {
            var fileName = $"{tableData.TableName}.json";
            var content = JsonSerializer.Serialize(tableData, JsonOptions);
            dataTableFiles.Add(new TableFile(fileName, content));
            
            _logger.LogDebug("Created data table file: {FileName}", fileName);
        }

        // Create schema files
        var schemaFiles = new List<SchemaFile>();
        foreach (var (_, tableData) in data)
        {
            var fileName = $"{tableData.TableName}.json";
            var content = JsonSerializer.Serialize(tableData.Columns, JsonOptions);
            schemaFiles.Add(new SchemaFile(fileName, content));
            
            _logger.LogDebug("Created schema file: {FileName}", fileName);
        }

        // Create export metadata
        var metadata = CreateExportMetadata(manifest, data, options);
        var metadataContent = JsonSerializer.Serialize(metadata, JsonOptions);

        var configFiles = new List<ConfigFile>
        {
            new("export_metadata.json", metadataContent)
        };

        // Add naming info for transparency
        var namingInfo = new
        {
            source_names = data.Keys.ToList(),
            normalized_names = data.Keys.Select(k => _canonicalMapper.NormalizeTableName(k)).ToList(),
            export_names = data.Keys.Select(k => 
                _canonicalMapper.GetExportTableName(_canonicalMapper.NormalizeTableName(k))
            ).ToList()
        };
        configFiles.Add(
            new ConfigFile("naming_info.json", JsonSerializer.Serialize(namingInfo, JsonOptions))
        );

        // Create documentation files
        var docFiles = CreateDocumentation(manifest);

        var package = new ExportPackage(
            Manifest: manifest,
            DataTables: dataTableFiles,
            SchemaFiles: schemaFiles,
            ConfigFiles: configFiles,
            DocumentFiles: docFiles
        );

        _logger.LogInformation("Export package built successfully");
        return package;
    }

    private Manifest CreateManifest(ExportRequest options)
    {
        return new Manifest(
            ExportId: Guid.NewGuid().ToString(),
            ExportTimestamp: DateTime.UtcNow,
            SourceSystem: new SourceSystem(
                Application: "HotelDruid.API",
                Version: GetApplicationVersion(),
                DatabaseType: "SQLite"  // TODO: Make configurable
            ),
            SourceMachine: new SourceMachine(
                HostName: Environment.MachineName,
                SystemName: "HotelDruid Export",
                HostingSystem: $"ASP.NET Core {GetDotNetVersion()}",
                ExportedBy: "system"  // TODO: Get from user context
            ),
            ExportType: options.ExportType
        );
    }

    private ExportMetadata CreateExportMetadata(
        Manifest manifest,
        Dictionary<string, CanonicalData> data,
        ExportRequest options)
    {
        var totalRows = data.Values.Sum(t => t.RowCount);

        return new ExportMetadata(
            ExportId: manifest.ExportId,
            ExportTimestamp: manifest.ExportTimestamp,
            ExportDurationSeconds: 0,  // Would track actual duration
            ExportScope: new ExportScope(
                ExportType: options.ExportType,
                TotalTables: data.Count,
                TotalRows: totalRows,
                TotalConfigFiles: options.IncludeConfigs ? 1 : 0
            ),
            ExportOptions: new()
            {
                ["include_configs"] = options.IncludeConfigs,
                ["include_docs"] = options.IncludeDocs
            }
        );
    }

    private List<DocumentFile> CreateDocumentation(Manifest manifest)
    {
        var docs = new List<DocumentFile>();

        // EXPORT_INFO.txt
        var exportInfo = new StringBuilder();
        exportInfo.AppendLine("HotelDruid Export Package");
        exportInfo.AppendLine("=" + new string('=', 24));
        exportInfo.AppendLine();
        exportInfo.AppendLine($"Export ID: {manifest.ExportId}");
        exportInfo.AppendLine($"Created: {manifest.ExportTimestamp:O}");
        exportInfo.AppendLine($"Source System: {manifest.SourceSystem.Application} v{manifest.SourceSystem.Version}");
        exportInfo.AppendLine($"Database Type: {manifest.SourceSystem.DatabaseType}");
        exportInfo.AppendLine($"Format Version: {manifest.ExportFormatVersion}");
        exportInfo.AppendLine();
        exportInfo.AppendLine("Contents:");
        exportInfo.AppendLine("  - manifest.json: Package metadata and structure");
        exportInfo.AppendLine("  - data/tables/*.json: Table data in canonical JSON format");
        exportInfo.AppendLine("  - schemas/tables/*.json: Table schema definitions");
        exportInfo.AppendLine("  - configs/: Configuration files");
        exportInfo.AppendLine("  - docs/: Documentation and guides");

        docs.Add(new DocumentFile("EXPORT_INFO.txt", exportInfo.ToString()));

        // IMPORT_GUIDE.txt
        var importGuide = new StringBuilder();
        importGuide.AppendLine("How to Import This Package");
        importGuide.AppendLine("=" + new string('=', 24));
        importGuide.AppendLine();
        importGuide.AppendLine("1. Extract the ZIP file to a safe location");
        importGuide.AppendLine("2. Review manifest.json for source information");
        importGuide.AppendLine("3. Use the Import functionality in HotelDruid.API");
        importGuide.AppendLine("4. Select this package and review the data mapping");
        importGuide.AppendLine("5. Choose 'Dry-run' to preview, or 'Import' to proceed");
        importGuide.AppendLine();
        importGuide.AppendLine("Compatibility:");
        importGuide.AppendLine("  - This package uses standard naming (rooms, not appartamenti)");
        importGuide.AppendLine("  - Legacy HotelDruid instances can still import with mapping");
        importGuide.AppendLine("  - All data is in canonical JSON format for language-agnostic support");

        docs.Add(new DocumentFile("IMPORT_GUIDE.txt", importGuide.ToString()));

        return docs;
    }

    private static string GetApplicationVersion()
    {
        var version = System.Reflection.Assembly.GetExecutingAssembly()
            .GetName()
            .Version;
        return version?.ToString() ?? "1.0.0";
    }

    private static string GetDotNetVersion()
    {
        return System.Runtime.InteropServices.RuntimeInformation
            .FrameworkDescription
            .Replace(".NET ", "");
    }
}

