using System.IO.Compression;
using HotelDruid.Api.Models;

namespace HotelDruid.Api.Services.ExportImport;

/// <summary>
/// Creates and manages ZIP files for export packages
/// </summary>
public class ZipBuilder : IZipBuilder
{
    private readonly string _exportDirectory;
    private readonly string _importDirectory;
    private readonly ILogger<ZipBuilder> _logger;

    public ZipBuilder(
        ILogger<ZipBuilder> logger,
        IConfiguration configuration)
    {
        _logger = logger;
        
        // Get storage paths from configuration
        _exportDirectory = configuration["Storage:ExportDirectory"]
            ?? Path.Combine(Directory.GetCurrentDirectory(), "artifacts", "exports");
        
        _importDirectory = configuration["Storage:ImportDirectory"]
            ?? Path.Combine(Directory.GetCurrentDirectory(), "artifacts", "imports");

        // Ensure directories exist
        Directory.CreateDirectory(_exportDirectory);
        Directory.CreateDirectory(Path.Combine(_importDirectory, "temp"));
        Directory.CreateDirectory(Path.Combine(_importDirectory, "completed"));

        _logger.LogInformation("ZipBuilder initialized - Export dir: {ExportDir}, Import dir: {ImportDir}",
            _exportDirectory, _importDirectory);
    }

    public async Task<string> CreateZipAsync(ExportPackage package)
    {
        var timestamp = package.Manifest.ExportTimestamp.ToString("yyyyMMdd_HHmmss");
        var unique = Guid.NewGuid().ToString().Substring(0, 8);  // First 8 chars of GUID for uniqueness
        var fileName = $"export_HotelDruid_{timestamp}_{unique}_v1.zip";
        var zipPath = Path.Combine(_exportDirectory, fileName);

        try
        {
            _logger.LogInformation("Creating ZIP file: {FileName}", fileName);

            using (var archive = ZipFile.Open(zipPath, ZipArchiveMode.Create))
            {
                // Add manifest.json
                AddJsonEntry(archive, "manifest.json", package.Manifest);

                // Add data files
                foreach (var table in package.DataTables)
                {
                    AddTextEntry(archive, $"data/tables/{table.FileName}", table.Content);
                }

                // Add schema files
                foreach (var schema in package.SchemaFiles)
                {
                    AddTextEntry(archive, $"schemas/tables/{schema.FileName}", schema.Content);
                }

                // Add config files
                foreach (var config in package.ConfigFiles)
                {
                    AddTextEntry(archive, $"configs/{config.FileName}", config.Content);
                }

                // Add documentation files
                foreach (var doc in package.DocumentFiles)
                {
                    AddTextEntry(archive, $"docs/{doc.FileName}", doc.Content);
                }
            }

            _logger.LogInformation("ZIP file created successfully: {FilePath}", zipPath);
            return zipPath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating ZIP file: {FileName}", fileName);
            
            // Clean up failed file
            try { File.Delete(zipPath); } catch { }
            
            throw;
        }
    }

    public async Task<List<ZipFileMetadata>> ListExportsAsync(int limit = 20, int offset = 0)
    {
        try
        {
            var files = Directory.GetFiles(_exportDirectory, "*.zip")
                .OrderByDescending(f => File.GetLastWriteTimeUtc(f))
                .Skip(offset)
                .Take(limit)
                .Select(path => new ZipFileMetadata(
                    FileName: Path.GetFileName(path),
                    FileSizeBytes: new FileInfo(path).Length,
                    CreatedAt: File.GetLastWriteTimeUtc(path)
                ))
                .ToList();

            _logger.LogInformation("Listed {Count} exports", files.Count);
            return files;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing exports");
            throw;
        }
    }

    public async Task<string> ExtractZipAsync(IFormFile zipFile)
    {
        if (zipFile == null || zipFile.Length == 0)
            throw new ArgumentException("ZIP file is empty or null");

        var tempDir = Path.Combine(_importDirectory, "temp", $"import_{Guid.NewGuid()}");
        
        try
        {
            _logger.LogInformation("Extracting ZIP file to: {TempDir}", tempDir);
            Directory.CreateDirectory(tempDir);

            using (var stream = zipFile.OpenReadStream())
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Read))
            {
                archive.ExtractToDirectory(tempDir);
            }

            _logger.LogInformation("ZIP file extracted successfully");
            return tempDir;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting ZIP file");
            
            // Clean up failed extraction
            try { Directory.Delete(tempDir, true); } catch { }
            
            throw;
        }
    }

    public async Task<(string FilePath, string FileName)> GetExportFileAsync(string exportId)
    {
        try
        {
            var files = Directory.GetFiles(_exportDirectory, "*.zip")
                .Where(f => Path.GetFileName(f).Contains(exportId))
                .FirstOrDefault();

            if (files == null)
                throw new FileNotFoundException($"Export file not found for ID: {exportId}");

            return (files, Path.GetFileName(files));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting export file for ID: {ExportId}", exportId);
            throw;
        }
    }

    private void AddJsonEntry<T>(ZipArchive archive, string entryName, T obj)
    {
        var entry = archive.CreateEntry(entryName);
        using (var stream = entry.Open())
        using (var writer = new StreamWriter(stream))
        {
            var json = System.Text.Json.JsonSerializer.Serialize(obj, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });
            writer.Write(json);
        }
    }

    private void AddTextEntry(ZipArchive archive, string entryName, string content)
    {
        var entry = archive.CreateEntry(entryName);
        using (var stream = entry.Open())
        using (var writer = new StreamWriter(stream))
        {
            writer.Write(content);
        }
    }
}

