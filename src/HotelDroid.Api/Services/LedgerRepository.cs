using System.Text.Json;
using System.Text.Json.Serialization;
using HotelDroid.Api.Models;

namespace HotelDroid.Api.Services;

/// <summary>
/// File-backed implementation of ILedgerRepository.
///
/// Concurrency: one SemaphoreSlim per date-partition (keyed by "yyyy-MM-dd").
/// Atomicity: writes to a .tmp file then renames over the target (atomic on NTFS/ext4).
///
/// Sequence numbering:
///   Scans existing _seq_*.json files in the partition directory to find the
///   current highest sequence, then increments by 1.  The scan is done while
///   holding the partition lock so no two concurrent callers can pick the same
///   sequence.
/// </summary>
public class LedgerRepository : ILedgerRepository, IDisposable
{
    private const string SnapshotFileName = "_snapshot.json";
    private const string SeqFilePrefix = "_seq_";
    private const string SeqFileSuffix = ".json";

    private readonly string _dataRoot;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ILogger<LedgerRepository> _logger;

    private readonly Dictionary<string, SemaphoreSlim> _partitionLocks = new();
    private readonly object _lockDictionaryLock = new();
    private bool _disposed;

    public LedgerRepository(string dataRoot, ILogger<LedgerRepository> logger)
    {
        _dataRoot = dataRoot ?? throw new ArgumentNullException(nameof(dataRoot));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    /// <inheritdoc />
    public async Task<LedgerSnapshot?> GetSnapshotAsync(DateTime date)
    {
        var path = GetSnapshotPath(date);
        if (!File.Exists(path))
            return null;

        var json = await File.ReadAllTextAsync(path);
        return JsonSerializer.Deserialize<LedgerSnapshot>(json, _jsonOptions);
    }

    /// <inheritdoc />
    public async Task SaveSnapshotAsync(DateTime date, LedgerSnapshot snapshot)
    {
        if (snapshot is null) throw new ArgumentNullException(nameof(snapshot));

        var partitionDir = GetPartitionDir(date);
        Directory.CreateDirectory(partitionDir);

        var sem = GetLock(date);
        await sem.WaitAsync();
        try
        {
            await AtomicWriteAsync(GetSnapshotPath(date), snapshot);
        }
        finally
        {
            sem.Release();
        }
    }

    /// <inheritdoc />
    public async Task<LedgerEntry> AppendEntryAsync(DateTime date, LedgerEntry entry)
    {
        if (entry is null) throw new ArgumentNullException(nameof(entry));

        var partitionDir = GetPartitionDir(date);
        Directory.CreateDirectory(partitionDir);

        var sem = GetLock(date);
        await sem.WaitAsync();
        try
        {
            var nextSeq = GetNextSequenceFromDisk(partitionDir);
            entry.Sequence = nextSeq;

            if (string.IsNullOrEmpty(entry.Id))
                entry.Id = $"ledger_{date:yyyyMMdd}_{nextSeq:D6}";

            if (entry.Timestamp == default)
                entry.Timestamp = DateTime.UtcNow;

            var filePath = GetSeqPath(partitionDir, nextSeq);
            await AtomicWriteAsync(filePath, entry);

            _logger.LogInformation(
                "Ledger entry appended: date={Date} seq={Seq} type={Type}",
                date.ToString("yyyy-MM-dd"), nextSeq, entry.EntryType);

            return entry;
        }
        finally
        {
            sem.Release();
        }
    }

    /// <inheritdoc />
    public async Task<List<LedgerEntry>> GetEntriesSinceSequenceAsync(DateTime date, int fromSequence)
    {
        var partitionDir = GetPartitionDir(date);
        if (!Directory.Exists(partitionDir))
            return new List<LedgerEntry>();

        var entries = new List<LedgerEntry>();
        foreach (var file in GetSortedSeqFiles(partitionDir))
        {
            var entry = await ReadEntryAsync(file);
            if (entry is not null && entry.Sequence > fromSequence)
                entries.Add(entry);
        }

        return entries;
    }

    /// <inheritdoc />
    public async Task<List<LedgerEntry>> GetEntriesForDateAsync(DateTime date)
    {
        var partitionDir = GetPartitionDir(date);
        if (!Directory.Exists(partitionDir))
            return new List<LedgerEntry>();

        var entries = new List<LedgerEntry>();
        foreach (var file in GetSortedSeqFiles(partitionDir))
        {
            var entry = await ReadEntryAsync(file);
            if (entry is not null)
                entries.Add(entry);
        }

        return entries;
    }

    /// <inheritdoc />
    public async Task<List<LedgerEntry>> GetEntriesForBookingAsync(
        string bookingId, DateTime from, DateTime to)
    {
        if (string.IsNullOrWhiteSpace(bookingId))
            throw new ArgumentException("Booking ID cannot be empty", nameof(bookingId));

        var result = new List<LedgerEntry>();

        for (var date = from.Date; date <= to.Date; date = date.AddDays(1))
        {
            var consolidated = await GetConsolidatedAsync(date);
            result.AddRange(consolidated.Where(e => e.BookingId == bookingId));
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<List<LedgerEntry>> GetConsolidatedAsync(DateTime date)
    {
        var result = new List<LedgerEntry>();

        // 1. Load snapshot entries (if any)
        var snapshot = await GetSnapshotAsync(date);
        if (snapshot is not null)
            result.AddRange(snapshot.Entries);

        // 2. Append incremental entries after the snapshot's sequenceAt
        var snapshotSeq = snapshot?.SequenceAt ?? 0;
        var incremental = await GetEntriesSinceSequenceAsync(date, snapshotSeq);
        result.AddRange(incremental);

        // Return ordered by sequence
        return result.OrderBy(e => e.Sequence).ToList();
    }

    // ─── IDisposable ────────────────────────────────────────────────────────

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        lock (_lockDictionaryLock)
        {
            foreach (var sem in _partitionLocks.Values)
                sem.Dispose();
            _partitionLocks.Clear();
        }
    }

    // ─── Private helpers ────────────────────────────────────────────────────

    private string GetPartitionDir(DateTime date) =>
        Path.Combine(_dataRoot, "ledger",
            date.Year.ToString("D4"),
            date.Month.ToString("D2"),
            date.Day.ToString("D2"));

    private string GetSnapshotPath(DateTime date) =>
        Path.Combine(GetPartitionDir(date), SnapshotFileName);

    private static string GetSeqPath(string partitionDir, int seq) =>
        Path.Combine(partitionDir, $"{SeqFilePrefix}{seq:D6}{SeqFileSuffix}");

    private static IEnumerable<string> GetSortedSeqFiles(string partitionDir) =>
        Directory.EnumerateFiles(partitionDir, $"{SeqFilePrefix}*{SeqFileSuffix}")
                 .OrderBy(f => f);

    private static int GetNextSequenceFromDisk(string partitionDir)
    {
        var highest = 0;
        foreach (var file in Directory.EnumerateFiles(
            partitionDir, $"{SeqFilePrefix}*{SeqFileSuffix}"))
        {
            var name = Path.GetFileNameWithoutExtension(file);
            var seqStr = name[SeqFilePrefix.Length..];
            if (int.TryParse(seqStr, out var n) && n > highest)
                highest = n;
        }
        return highest + 1;
    }

    private SemaphoreSlim GetLock(DateTime date)
    {
        var key = date.ToString("yyyy-MM-dd");
        lock (_lockDictionaryLock)
        {
            if (!_partitionLocks.TryGetValue(key, out var sem))
            {
                sem = new SemaphoreSlim(1, 1);
                _partitionLocks[key] = sem;
            }
            return sem;
        }
    }

    private async Task AtomicWriteAsync<T>(string targetPath, T value)
    {
        var tempPath = targetPath + ".tmp";
        var json = JsonSerializer.Serialize(value, _jsonOptions);
        await File.WriteAllTextAsync(tempPath, json);
        File.Move(tempPath, targetPath, overwrite: true);
    }

    private async Task<LedgerEntry?> ReadEntryAsync(string filePath)
    {
        try
        {
            var json = await File.ReadAllTextAsync(filePath);
            return JsonSerializer.Deserialize<LedgerEntry>(json, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to read ledger entry from {File}", filePath);
            return null;
        }
    }
}
