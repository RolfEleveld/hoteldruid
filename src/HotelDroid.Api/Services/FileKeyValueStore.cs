using System.Text.Json;
using System.Text.Json.Serialization;

namespace HotelDroid.Api.Services;

/// <summary>
/// File-based key-value store with in-process concurrency control.
/// 
/// Storage Model:
/// - Collections are directories under a data root
/// - Data files: {guid}.json (GUID-base32, 26 chars)
/// - Index file: _index.json (maps human-readable names to GUIDs)
/// - Concurrency: SemaphoreSlim per collection (serializes writes)
/// - Atomicity: Write to temp file, atomic rename
/// 
/// Example:
///   data/
///     collections/
///       rooms/
///         _index.json                    // { "Room-1": "a1b2c3d4e5...", ... }
///         a1b2c3d4e5f6g7h8i9j0k1l2.json // Actual room data
///         b2c3d4e5f6g7h8i9j0k1l2m3.json
/// </summary>
public class FileKeyValueStore : IKeyValueStore, IDisposable
{
    private readonly string _dataRoot;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ILogger<FileKeyValueStore> _logger;
    
    // Per-collection locks for write serialization
    private readonly Dictionary<string, SemaphoreSlim> _collectionLocks = new();
    private readonly object _lockDictionaryLock = new();

    private const string IndexFileName = "_index.json";
    private const string MetadataKey = "_metadata";
    private const string CollectionsDir = "collections";

    public FileKeyValueStore(string dataRoot, ILogger<FileKeyValueStore> logger)
    {
        _dataRoot = dataRoot ?? throw new ArgumentNullException(nameof(dataRoot));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        // Ensure data root exists
        Directory.CreateDirectory(_dataRoot);

        // Configure JSON serialization
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    /// <inheritdoc />
    public async Task<T?> GetAsync<T>(string collectionName, string id) where T : class
    {
        ValidateCollectionName(collectionName);
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("ID cannot be empty", nameof(id));

        var filePath = GetDocumentPath(collectionName, id);
        
        if (!File.Exists(filePath))
        {
            _logger.LogWarning("Document not found: {Collection}/{Id}", collectionName, id);
            return null;
        }

        try
        {
            var json = await File.ReadAllTextAsync(filePath);
            var document = JsonSerializer.Deserialize<T>(json, _jsonOptions);
            return document;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading document: {Collection}/{Id}", collectionName, id);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<T?> GetByNameAsync<T>(string collectionName, string name) where T : class
    {
        ValidateCollectionName(collectionName);
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        var index = await GetIndexAsync(collectionName);
        if (!index.TryGetValue(name, out var id))
        {
            _logger.LogWarning("Document not found by name: {Collection}/{Name}", collectionName, name);
            return null;
        }

        return await GetAsync<T>(collectionName, id);
    }

    /// <inheritdoc />
    public async Task<List<T>> ListAsync<T>(string collectionName) where T : class
    {
        ValidateCollectionName(collectionName);
        
        var collectionPath = GetCollectionPath(collectionName);
        if (!Directory.Exists(collectionPath))
        {
            _logger.LogInformation("Collection does not exist: {Collection}", collectionName);
            return new List<T>();
        }

        var documents = new List<T>();
        var files = Directory.GetFiles(collectionPath, "*.json")
            .Where(f => Path.GetFileName(f) != IndexFileName)
            .OrderBy(f => f);

        foreach (var file in files)
        {
            try
            {
                var json = await File.ReadAllTextAsync(file);
                var document = JsonSerializer.Deserialize<T>(json, _jsonOptions);
                if (document != null)
                    documents.Add(document);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading document: {File}", file);
                // Continue loading other documents
            }
        }

        _logger.LogInformation("Listed {Count} documents from {Collection}", documents.Count, collectionName);
        return documents;
    }

    /// <inheritdoc />
    public async Task<string> CreateAsync<T>(string collectionName, string name, T document) where T : class
    {
        ValidateCollectionName(collectionName);
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));
        if (document == null)
            throw new ArgumentNullException(nameof(document));

        var lockObj = GetCollectionLock(collectionName);
        
        await lockObj.WaitAsync();
        try
        {
            // Check if name already exists in index
            var index = await LoadIndexAsync(collectionName);
            if (index.ContainsKey(name))
            {
                var ex = new InvalidOperationException($"Document '{name}' already exists in collection '{collectionName}'");
                _logger.LogError(ex, "Create failed: duplicate name");
                throw ex;
            }

            // Generate ID and create document
            var id = IdGenerator.GenerateId();
            var filePath = GetDocumentPath(collectionName, id);
            
            // Ensure collection directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

            // Write document with atomic pattern
            await AtomicWriteAsync(filePath, JsonSerializer.Serialize(document, _jsonOptions));

            // Update index
            index[name] = id;
            await SaveIndexAsync(collectionName, index);

            _logger.LogInformation("Created document: {Collection}/{Name} -> {Id}", collectionName, name, id);
            return id;
        }
        finally
        {
            lockObj.Release();
        }
    }

    /// <inheritdoc />
    public async Task UpdateAsync<T>(string collectionName, string id, T document) where T : class
    {
        ValidateCollectionName(collectionName);
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("ID cannot be empty", nameof(id));
        if (document == null)
            throw new ArgumentNullException(nameof(document));

        var lockObj = GetCollectionLock(collectionName);
        
        await lockObj.WaitAsync();
        try
        {
            var filePath = GetDocumentPath(collectionName, id);
            
            if (!File.Exists(filePath))
            {
                var ex = new FileNotFoundException($"Document not found: {collectionName}/{id}");
                _logger.LogError(ex, "Update failed: document not found");
                throw ex;
            }

            await AtomicWriteAsync(filePath, JsonSerializer.Serialize(document, _jsonOptions));
            _logger.LogInformation("Updated document: {Collection}/{Id}", collectionName, id);
        }
        finally
        {
            lockObj.Release();
        }
    }

    /// <inheritdoc />
    public async Task DeleteAsync(string collectionName, string id)
    {
        ValidateCollectionName(collectionName);
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("ID cannot be empty", nameof(id));

        var lockObj = GetCollectionLock(collectionName);
        
        await lockObj.WaitAsync();
        try
        {
            var filePath = GetDocumentPath(collectionName, id);
            
            if (!File.Exists(filePath))
            {
                var ex = new FileNotFoundException($"Document not found: {collectionName}/{id}");
                _logger.LogError(ex, "Delete failed: document not found");
                throw ex;
            }

            File.Delete(filePath);

            // Remove from index
            var index = await LoadIndexAsync(collectionName);
            var nameToRemove = index.FirstOrDefault(x => x.Value == id).Key;
            if (nameToRemove != null)
            {
                index.Remove(nameToRemove);
                await SaveIndexAsync(collectionName, index);
            }

            _logger.LogInformation("Deleted document: {Collection}/{Id}", collectionName, id);
        }
        finally
        {
            lockObj.Release();
        }
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(string collectionName, string id)
    {
        ValidateCollectionName(collectionName);
        if (string.IsNullOrWhiteSpace(id))
            return false;

        var filePath = GetDocumentPath(collectionName, id);
        return await Task.FromResult(File.Exists(filePath));
    }

    /// <inheritdoc />
    public async Task DeleteCollectionAsync(string collectionName)
    {
        ValidateCollectionName(collectionName);

        var lockObj = GetCollectionLock(collectionName);
        
        await lockObj.WaitAsync();
        try
        {
            var collectionPath = GetCollectionPath(collectionName);
            
            if (Directory.Exists(collectionPath))
            {
                Directory.Delete(collectionPath, recursive: true);
                _logger.LogInformation("Deleted collection: {Collection}", collectionName);
            }
        }
        finally
        {
            lockObj.Release();
        }
    }

    /// <inheritdoc />
    public async Task<int> RebuildIndexAsync(string collectionName)
    {
        ValidateCollectionName(collectionName);

        var lockObj = GetCollectionLock(collectionName);
        
        await lockObj.WaitAsync();
        try
        {
            var collectionPath = GetCollectionPath(collectionName);
            var index = new Dictionary<string, string>();

            if (!Directory.Exists(collectionPath))
            {
                _logger.LogInformation("Collection does not exist, creating index: {Collection}", collectionName);
                await SaveIndexAsync(collectionName, index);
                return 0;
            }

            // Scan all JSON files except index
            var dataFiles = Directory.GetFiles(collectionPath, "*.json")
                .Where(f => Path.GetFileName(f) != IndexFileName)
                .OrderBy(f => f);

            foreach (var file in dataFiles)
            {
                try
                {
                    var json = await File.ReadAllTextAsync(file);
                    
                    // Try to extract 'name' field from JSON for index mapping
                    using (var doc = JsonDocument.Parse(json))
                    {
                        if (doc.RootElement.TryGetProperty("name", out var nameElement) && 
                            nameElement.ValueKind == JsonValueKind.String)
                        {
                            var name = nameElement.GetString();
                            var id = Path.GetFileNameWithoutExtension(file);
                            
                            if (!string.IsNullOrEmpty(name))
                            {
                                index[name] = id;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Could not parse document for index rebuild: {File}", file);
                    // Continue with other files
                }
            }

            // Save rebuilt index
            await SaveIndexAsync(collectionName, index);
            _logger.LogInformation("Rebuilt index for {Collection}: {Count} entries", collectionName, index.Count);
            return index.Count;
        }
        finally
        {
            lockObj.Release();
        }
    }

    /// <inheritdoc />
    public async Task<Dictionary<string, string>> GetIndexAsync(string collectionName)
    {
        ValidateCollectionName(collectionName);
        return await LoadIndexAsync(collectionName);
    }

    /// <summary>
    /// Load the index file for a collection. Returns empty dict if collection doesn't exist.
    /// </summary>
    private async Task<Dictionary<string, string>> LoadIndexAsync(string collectionName)
    {
        var indexPath = GetIndexPath(collectionName);
        
        if (!File.Exists(indexPath))
        {
            return new Dictionary<string, string>();
        }

        try
        {
            var json = await File.ReadAllTextAsync(indexPath);
            var index = JsonSerializer.Deserialize<Dictionary<string, string>>(json, _jsonOptions) 
                ?? new Dictionary<string, string>();
            
            // Remove metadata key if present
            index.Remove(MetadataKey);
            return index;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading index: {Collection}", collectionName);
            return new Dictionary<string, string>();
        }
    }

    /// <summary>
    /// Save the index file for a collection.
    /// </summary>
    private async Task SaveIndexAsync(string collectionName, Dictionary<string, string> index)
    {
        var indexPath = GetIndexPath(collectionName);
        
        // Ensure collection directory exists
        Directory.CreateDirectory(Path.GetDirectoryName(indexPath)!);

        // Add metadata
        var indexWithMetadata = new Dictionary<string, string>(index)
        {
            {
                MetadataKey,
                JsonSerializer.Serialize(new
                {
                    version = "1",
                    lastUpdated = DateTime.UtcNow.ToString("O"),
                    entryCount = index.Count
                }, _jsonOptions)
            }
        };

        await AtomicWriteAsync(indexPath, JsonSerializer.Serialize(indexWithMetadata, _jsonOptions));
    }

    /// <summary>
    /// Write content to a file atomically using temp file + rename pattern.
    /// Guarantees writes are all-or-nothing even if process crashes.
    /// </summary>
    private async Task AtomicWriteAsync(string filePath, string content)
    {
        var tempPath = filePath + ".tmp";
        
        try
        {
            // Write to temp file
            await File.WriteAllTextAsync(tempPath, content);
            
            // Atomic rename (OS guarantees atomicity)
            File.Move(tempPath, filePath, overwrite: true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error writing file: {FilePath}", filePath);
            
            // Cleanup temp file if it exists
            try { File.Delete(tempPath); }
            catch { /* ignore cleanup errors */ }
            
            throw;
        }
    }

    /// <summary>
    /// Get or create a SemaphoreSlim for a collection.
    /// Serializes writes to a collection.
    /// </summary>
    private SemaphoreSlim GetCollectionLock(string collectionName)
    {
        lock (_lockDictionaryLock)
        {
            if (!_collectionLocks.TryGetValue(collectionName, out var lockObj))
            {
                lockObj = new SemaphoreSlim(1, 1);
                _collectionLocks[collectionName] = lockObj;
            }
            return lockObj;
        }
    }

    /// <summary>
    /// Get the full filesystem path for a collection directory.
    /// </summary>
    private string GetCollectionPath(string collectionName)
    {
        ValidateCollectionName(collectionName);
        return Path.Combine(_dataRoot, CollectionsDir, collectionName);
    }

    /// <summary>
    /// Get the full filesystem path for a document file.
    /// </summary>
    private string GetDocumentPath(string collectionName, string id)
    {
        return Path.Combine(GetCollectionPath(collectionName), $"{id}.json");
    }

    /// <summary>
    /// Get the full filesystem path for the index file.
    /// </summary>
    private string GetIndexPath(string collectionName)
    {
        return Path.Combine(GetCollectionPath(collectionName), IndexFileName);
    }

    /// <summary>
    /// Validate collection name (alphanumeric + underscore, no path traversal).
    /// </summary>
    private static void ValidateCollectionName(string collectionName)
    {
        if (string.IsNullOrWhiteSpace(collectionName))
            throw new ArgumentException("Collection name cannot be empty", nameof(collectionName));

        if (collectionName.Contains("..") || collectionName.Contains("/") || collectionName.Contains("\\"))
            throw new ArgumentException("Invalid collection name (path traversal detected)", nameof(collectionName));

        if (!System.Text.RegularExpressions.Regex.IsMatch(collectionName, @"^[a-zA-Z0-9_]+$"))
            throw new ArgumentException("Collection name must be alphanumeric (underscore allowed)", nameof(collectionName));
    }

    public void Dispose()
    {
        lock (_lockDictionaryLock)
        {
            foreach (var lockObj in _collectionLocks.Values)
            {
                lockObj?.Dispose();
            }
            _collectionLocks.Clear();
        }
    }
}
