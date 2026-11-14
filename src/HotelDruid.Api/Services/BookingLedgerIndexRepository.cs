using System.Text.Json;

namespace HotelDruid.Api.Services;

/// <summary>
/// File-backed implementation of IBookingLedgerIndexRepository.
/// Stores a simple JSON array of ledger entry IDs under the booking folder.
/// </summary>
public class BookingLedgerIndexRepository : IBookingLedgerIndexRepository
{
    private readonly string _dataRoot;
    private readonly ILogger<BookingLedgerIndexRepository> _logger;
    private readonly Dictionary<string, SemaphoreSlim> _locks = new(StringComparer.OrdinalIgnoreCase);
    private readonly object _lockDictionaryLock = new();
    private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

    public BookingLedgerIndexRepository(string dataRoot, ILogger<BookingLedgerIndexRepository> logger)
    {
        _dataRoot = dataRoot;
        _logger = logger;
    }

    private string GetIndexPath(string bookingId)
        => Path.Combine(_dataRoot, "bookings", bookingId, "_ledger_index.json");

    public async Task<List<string>?> ReadIfPresentAsync(string bookingId)
    {
        if (string.IsNullOrWhiteSpace(bookingId))
            return new();

        var gate = GetLock(bookingId);
        await gate.WaitAsync();
        try
        {
            return await ReadUnlockedAsync(bookingId);
        }
        finally
        {
            gate.Release();
        }
    }

    public async Task<List<string>> GetOrBuildAsync(string bookingId, Func<Task<IReadOnlyList<string>>> build)
    {
        if (string.IsNullOrWhiteSpace(bookingId))
            return new();

        var gate = GetLock(bookingId);
        await gate.WaitAsync();
        try
        {
            var existing = await ReadUnlockedAsync(bookingId);
            if (existing is not null)
                return existing;

            var built = await build();
            var ids = built
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Distinct(StringComparer.Ordinal)
                .ToList();
            await WriteUnlockedAsync(bookingId, ids);
            return ids;
        }
        finally
        {
            gate.Release();
        }
    }

    public async Task AppendIfPresentAsync(string bookingId, IReadOnlyCollection<string> entryIds)
    {
        if (string.IsNullOrWhiteSpace(bookingId) || entryIds is null || entryIds.Count == 0)
            return;

        var gate = GetLock(bookingId);
        await gate.WaitAsync();
        try
        {
            var existing = await ReadUnlockedAsync(bookingId);
            if (existing is null)
                return;

            var changed = false;
            foreach (var entryId in entryIds)
            {
                if (string.IsNullOrWhiteSpace(entryId) || existing.Contains(entryId))
                    continue;
                existing.Add(entryId);
                changed = true;
            }

            if (changed)
                await WriteUnlockedAsync(bookingId, existing);
        }
        finally
        {
            gate.Release();
        }
    }

    public async Task ReplaceAsync(string bookingId, IReadOnlyList<string> entryIds)
    {
        if (string.IsNullOrWhiteSpace(bookingId))
            return;

        var gate = GetLock(bookingId);
        await gate.WaitAsync();
        try
        {
            var ids = entryIds
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Distinct(StringComparer.Ordinal)
                .ToList();
            await WriteUnlockedAsync(bookingId, ids);
        }
        finally
        {
            gate.Release();
        }
    }

    private async Task<List<string>?> ReadUnlockedAsync(string bookingId)
    {
        var path = GetIndexPath(bookingId);
        if (!File.Exists(path))
            return null;

        try
        {
            var json = await File.ReadAllTextAsync(path);
            return JsonSerializer.Deserialize<List<string>>(json, _json) ?? new();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to read ledger index for booking {BookingId}", bookingId);
            return new();
        }
    }

    private async Task WriteUnlockedAsync(string bookingId, IReadOnlyList<string> ids)
    {
        var path = GetIndexPath(bookingId);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        var json = JsonSerializer.Serialize(ids, _json);
        var tempPath = path + ".tmp";
        await File.WriteAllTextAsync(tempPath, json);
        File.Move(tempPath, path, overwrite: true);
    }

    private SemaphoreSlim GetLock(string bookingId)
    {
        lock (_lockDictionaryLock)
        {
            if (!_locks.TryGetValue(bookingId, out var gate))
            {
                gate = new SemaphoreSlim(1, 1);
                _locks[bookingId] = gate;
            }

            return gate;
        }
    }
}