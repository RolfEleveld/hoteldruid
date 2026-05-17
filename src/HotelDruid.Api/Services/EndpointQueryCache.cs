using System.Collections.Concurrent;

namespace HotelDruid.Api.Services;

public sealed class EndpointQueryCache : IEndpointQueryCache
{
    private readonly ConcurrentDictionary<string, CacheEntry> _entries = new();
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, byte>> _tagIndex = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _keyLocks = new();

    private readonly ILogger<EndpointQueryCache> _logger;
    private readonly bool _enabled;
    private readonly TimeSpan _ttl;

    public EndpointQueryCache(ILogger<EndpointQueryCache> logger, bool enabled, int ttlSeconds)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _enabled = enabled;
        _ttl = TimeSpan.FromSeconds(Math.Max(1, ttlSeconds));
    }

    public async Task<T> GetOrCreateAsync<T>(string key, IReadOnlyCollection<string> tags, Func<Task<T>> factory)
    {
        if (!_enabled)
            return await factory();

        if (_entries.TryGetValue(key, out var hit) && hit.ExpiresAtUtc > DateTime.UtcNow)
        {
            _logger.LogDebug("Endpoint cache hit: {CacheKey}", key);
            return (T)hit.Value;
        }

        var gate = _keyLocks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
        await gate.WaitAsync();
        try
        {
            if (_entries.TryGetValue(key, out hit) && hit.ExpiresAtUtc > DateTime.UtcNow)
                return (T)hit.Value;

            var value = await factory();
            var expiresAt = DateTime.UtcNow.Add(_ttl);

            _entries[key] = new CacheEntry(value!, expiresAt, tags);
            foreach (var tag in tags)
            {
                var keys = _tagIndex.GetOrAdd(tag, _ => new ConcurrentDictionary<string, byte>(StringComparer.OrdinalIgnoreCase));
                keys[key] = 0;
            }

            _logger.LogDebug("Endpoint cache set: {CacheKey} (expires {ExpiresAtUtc})", key, expiresAt);
            return value;
        }
        finally
        {
            gate.Release();
        }
    }

    public void InvalidateTag(string tag)
    {
        if (!_enabled)
            return;

        if (!_tagIndex.TryRemove(tag, out var keys))
            return;

        foreach (var key in keys.Keys)
        {
            _entries.TryRemove(key, out _);
        }

        _logger.LogInformation("Endpoint cache invalidated by tag: {Tag}, keys: {Count}", tag, keys.Count);
    }

    private sealed record CacheEntry(object Value, DateTime ExpiresAtUtc, IReadOnlyCollection<string> Tags);
}
