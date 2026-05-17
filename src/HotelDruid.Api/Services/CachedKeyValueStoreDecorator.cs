using System.Collections.Concurrent;

namespace HotelDruid.Api.Services;

/// <summary>
/// Caching decorator for IKeyValueStore with collection-level invalidation.
///
/// Design:
/// - Caches indexes (name→id mapping) per collection.
/// - Caches documents per collection+id.
/// - Invalidates entire collection cache on Create/Update/Delete/RebuildIndex/DeleteCollection.
/// - Thread-safe via ConcurrentDictionary.
///
/// Memory management:
/// - Bounded by MaxCachedCollections (default 20) to prevent unbounded growth.
/// - Optional sliding expiration (configurable, default 5 minutes).
/// - LRU eviction when collection count exceeds limit.
///
/// Performance characteristics:
/// - GetAsync: O(1) in-memory after first fetch (vs file I/O baseline).
/// - ListAsync: O(1) index lookup + O(n) document deserialization from memory (vs n+1 file I/O).
/// - GetIndexAsync: O(1) in-memory (vs file read).
/// - CreateAsync/UpdateAsync/DeleteAsync: O(1) cache invalidation + baseline write time.
/// </summary>
public class CachedKeyValueStoreDecorator : IKeyValueStore
{
    private readonly IKeyValueStore _inner;
    private readonly ILogger<CachedKeyValueStoreDecorator> _logger;

    // Per-collection caches
    // Key: collection name
    // Value: CacheEntry containing index + documents + timestamp
    private readonly ConcurrentDictionary<string, CacheEntry> _cache = new();

    private readonly int _maxCachedCollections;
    private readonly TimeSpan _cacheExpiration;
    private readonly string? _versionStampDirectory;
    private readonly TimeSpan _versionCheckInterval;

    public CachedKeyValueStoreDecorator(
        IKeyValueStore inner,
        ILogger<CachedKeyValueStoreDecorator> logger,
        int maxCachedCollections = 20,
        int cacheExpirationMinutes = 5,
        string? invalidationRootPath = null,
        int versionCheckIntervalMilliseconds = 250)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _maxCachedCollections = Math.Max(1, maxCachedCollections);
        _cacheExpiration = TimeSpan.FromMinutes(Math.Max(1, cacheExpirationMinutes));

        if (!string.IsNullOrWhiteSpace(invalidationRootPath))
        {
            _versionStampDirectory = Path.Combine(invalidationRootPath, ".cache-invalidation");
            Directory.CreateDirectory(_versionStampDirectory);
        }

        _versionCheckInterval = versionCheckIntervalMilliseconds <= 0
            ? TimeSpan.Zero
            : TimeSpan.FromMilliseconds(versionCheckIntervalMilliseconds);
    }

    /// <summary>
    /// Get a document by ID from cache (or load and cache).
    /// </summary>
    public async Task<T?> GetAsync<T>(string collectionName, string id) where T : class
    {
        var entry = GetOrCreateCacheEntry(collectionName);
        await EnsureCollectionFreshAsync(collectionName, entry);

        if (entry.DocumentCache.TryGetValue(id, out var cachedJson))
        {
            _logger.LogDebug("Cache hit: {Collection}/{Id}", collectionName, id);
            return DeserializeJson<T>(cachedJson);
        }

        _logger.LogDebug("Cache miss: {Collection}/{Id}", collectionName, id);
        var document = await _inner.GetAsync<T>(collectionName, id);
        
        if (document != null)
        {
            var json = SerializeJson(document);
            entry.DocumentCache.TryAdd(id, json);
        }

        return document;
    }

    /// <summary>
    /// Get a document by name via cached index.
    /// </summary>
    public async Task<T?> GetByNameAsync<T>(string collectionName, string name) where T : class
    {
        var index = await GetIndexAsync(collectionName);
        if (!index.TryGetValue(name, out var id))
        {
            _logger.LogDebug("Cache miss (name not in index): {Collection}/{Name}", collectionName, name);
            return null;
        }

        return await GetAsync<T>(collectionName, id);
    }

    /// <summary>
    /// List all documents: get index from cache, load all docs (with caching).
    /// </summary>
    public async Task<List<T>> ListAsync<T>(string collectionName) where T : class
    {
        var index = await GetIndexAsync(collectionName);
        var result = new List<T>();

        foreach (var id in index.Values)
        {
            var doc = await GetAsync<T>(collectionName, id);
            if (doc != null)
                result.Add(doc);
        }

        return result;
    }

    /// <summary>
    /// Create invalidates the collection cache and delegates to inner store.
    /// </summary>
    public async Task<string> CreateAsync<T>(string collectionName, string name, T document) where T : class
    {
        var id = await _inner.CreateAsync(collectionName, name, document);
        await SignalCollectionInvalidationAsync(collectionName);
        InvalidateCollectionCache(collectionName);
        _logger.LogInformation("Cache invalidated (Create): {Collection}", collectionName);
        return id;
    }

    /// <summary>
    /// Update invalidates the collection cache and delegates to inner store.
    /// </summary>
    public async Task UpdateAsync<T>(string collectionName, string id, T document) where T : class
    {
        await _inner.UpdateAsync(collectionName, id, document);
        await SignalCollectionInvalidationAsync(collectionName);
        InvalidateCollectionCache(collectionName);
        _logger.LogInformation("Cache invalidated (Update): {Collection}/{Id}", collectionName, id);
    }

    /// <summary>
    /// Delete invalidates the collection cache and delegates to inner store.
    /// </summary>
    public async Task DeleteAsync(string collectionName, string id)
    {
        await _inner.DeleteAsync(collectionName, id);
        await SignalCollectionInvalidationAsync(collectionName);
        InvalidateCollectionCache(collectionName);
        _logger.LogInformation("Cache invalidated (Delete): {Collection}/{Id}", collectionName, id);
    }

    /// <summary>
    /// Check existence: try cache first, fallback to inner store.
    /// </summary>
    public async Task<bool> ExistsAsync(string collectionName, string id)
    {
        var entry = GetOrCreateCacheEntry(collectionName);
        await EnsureCollectionFreshAsync(collectionName, entry);

        if (entry.DocumentCache.ContainsKey(id))
        {
            _logger.LogDebug("Cache hit (exists): {Collection}/{Id}", collectionName, id);
            return true;
        }

        return await _inner.ExistsAsync(collectionName, id);
    }

    /// <summary>
    /// Delete collection invalidates the collection cache and delegates to inner store.
    /// </summary>
    public async Task DeleteCollectionAsync(string collectionName)
    {
        await _inner.DeleteCollectionAsync(collectionName);
        await SignalCollectionInvalidationAsync(collectionName);
        InvalidateCollectionCache(collectionName);
        _logger.LogInformation("Cache invalidated (DeleteCollection): {Collection}", collectionName);
    }

    /// <summary>
    /// Rebuild index invalidates the collection cache and delegates to inner store.
    /// </summary>
    public async Task<int> RebuildIndexAsync(string collectionName)
    {
        var count = await _inner.RebuildIndexAsync(collectionName);
        await SignalCollectionInvalidationAsync(collectionName);
        InvalidateCollectionCache(collectionName);
        _logger.LogInformation("Cache invalidated (RebuildIndex): {Collection}", collectionName);
        return count;
    }

    /// <summary>
    /// Get index from cache (or load and cache).
    /// </summary>
    public async Task<Dictionary<string, string>> GetIndexAsync(string collectionName)
    {
        var entry = GetOrCreateCacheEntry(collectionName);
        await EnsureCollectionFreshAsync(collectionName, entry);
        
        // Return a copy to prevent external mutations
        if (entry.IndexCache != null && !IsExpired(entry.Timestamp))
        {
            _logger.LogDebug("Cache hit: {Collection} index ({Count} entries)", collectionName, entry.IndexCache.Count);
            return new Dictionary<string, string>(entry.IndexCache);
        }

        _logger.LogDebug("Cache miss: {Collection} index", collectionName);
        var index = await _inner.GetIndexAsync(collectionName);
        
        entry.IndexCache = new Dictionary<string, string>(index);
        entry.Timestamp = DateTime.UtcNow;

        return new Dictionary<string, string>(index);
    }

    // ─── Private helpers ────────────────────────────────────────────────────

    /// <summary>
    /// Get or create a cache entry for a collection.
    /// Implements simple LRU eviction by collection count.
    /// </summary>
    private CacheEntry GetOrCreateCacheEntry(string collectionName)
    {
        if (_cache.TryGetValue(collectionName, out var entry))
            return entry;

        // Evict oldest entry if cache is full
        if (_cache.Count >= _maxCachedCollections)
        {
            var oldest = _cache
                .OrderBy(x => x.Value.Timestamp)
                .FirstOrDefault();

            if (!string.IsNullOrEmpty(oldest.Key))
            {
                _cache.TryRemove(oldest.Key, out _);
                _logger.LogWarning("Evicted cache entry for {Collection} (LRU limit {Max})", oldest.Key, _maxCachedCollections);
            }
        }

        var newEntry = new CacheEntry();
        _cache.TryAdd(collectionName, newEntry);
        return newEntry;
    }

    /// <summary>
    /// Invalidate all cached data for a collection.
    /// </summary>
    private void InvalidateCollectionCache(string collectionName)
    {
        if (_cache.TryGetValue(collectionName, out var entry))
        {
            entry.IndexCache = null;
            entry.DocumentCache.Clear();
            entry.Timestamp = DateTime.MinValue; // Mark as expired
            entry.LastVersionCheckUtc = DateTime.MinValue;
        }
    }

    /// <summary>
    /// Check the shared version stamp for cross-instance invalidation.
    /// </summary>
    private async Task EnsureCollectionFreshAsync(string collectionName, CacheEntry entry)
    {
        if (_versionStampDirectory == null)
            return;

        var now = DateTime.UtcNow;
        if (_versionCheckInterval > TimeSpan.Zero && now - entry.LastVersionCheckUtc < _versionCheckInterval)
            return;

        var currentStamp = await ReadCollectionVersionStampAsync(collectionName);
        var hasCachedData = entry.IndexCache != null || !entry.DocumentCache.IsEmpty;

        if (hasCachedData && !string.Equals(entry.VersionStamp, currentStamp, StringComparison.Ordinal))
        {
            entry.IndexCache = null;
            entry.DocumentCache.Clear();
            entry.Timestamp = DateTime.MinValue;
            _logger.LogInformation("Cache invalidated (VersionStamp): {Collection}", collectionName);
        }

        entry.VersionStamp = currentStamp;
        entry.LastVersionCheckUtc = now;
    }

    /// <summary>
    /// Signal a collection mutation to other instances via version stamp update.
    /// </summary>
    private async Task SignalCollectionInvalidationAsync(string collectionName)
    {
        if (_versionStampDirectory == null)
            return;

        var stampPath = GetVersionStampPath(collectionName);
        var stampValue = Guid.NewGuid().ToString("N");
        await File.WriteAllTextAsync(stampPath, stampValue);
    }

    /// <summary>
    /// Read the current collection version stamp, if present.
    /// </summary>
    private async Task<string?> ReadCollectionVersionStampAsync(string collectionName)
    {
        if (_versionStampDirectory == null)
            return null;

        var stampPath = GetVersionStampPath(collectionName);
        if (!File.Exists(stampPath))
            return null;

        return await File.ReadAllTextAsync(stampPath);
    }

    private string GetVersionStampPath(string collectionName)
    {
        return Path.Combine(_versionStampDirectory!, $"{collectionName}.stamp");
    }

    /// <summary>
    /// Check if cache entry has expired.
    /// </summary>
    private bool IsExpired(DateTime timestamp)
    {
        return DateTime.UtcNow - timestamp > _cacheExpiration;
    }

    /// <summary>
    /// Serialize object to JSON for caching.
    /// </summary>
    private static string SerializeJson<T>(T obj) where T : class
    {
        return System.Text.Json.JsonSerializer.Serialize(obj);
    }

    /// <summary>
    /// Deserialize JSON from cache.
    /// </summary>
    private static T? DeserializeJson<T>(string json) where T : class
    {
        return System.Text.Json.JsonSerializer.Deserialize<T>(json);
    }

    /// <summary>
    /// Per-collection cache state.
    /// </summary>
    private class CacheEntry
    {
        /// <summary>
        /// Cached index (name → id mapping). Null if not yet loaded or expired.
        /// </summary>
        public Dictionary<string, string>? IndexCache { get; set; }

        /// <summary>
        /// Cached documents (id → JSON string).
        /// </summary>
        public ConcurrentDictionary<string, string> DocumentCache { get; } = new();

        /// <summary>
        /// When this cache entry was last populated.
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Last observed shared version stamp for this collection.
        /// </summary>
        public string? VersionStamp { get; set; }

        /// <summary>
        /// Last time the version stamp was checked.
        /// </summary>
        public DateTime LastVersionCheckUtc { get; set; } = DateTime.MinValue;
    }
}
