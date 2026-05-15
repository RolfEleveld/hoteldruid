using System.Text.Json;
using System.Text.Json.Serialization;
using HotelDruid.Api.Models;

namespace HotelDruid.Api.Services;

/// <summary>
/// File-backed implementation of IBookingTransactionRepository.
///
/// Concurrency: one SemaphoreSlim per bookingId.
/// Atomicity: writes to a .tmp file then renames over the target.
///
/// Sequence numbering:
///   The _index.json file tracks NextSequence.  When appending a transaction:
///   1. Acquire the booking lock
///   2. Read (or initialise) the index
///   3. Write the _seq_NNNNNN.json file
///   4. Increment NextSequence in the index
///   All three steps happen under the same lock.
/// </summary>
public class BookingTransactionRepository : IBookingTransactionRepository, IDisposable
{
    private const string IndexFileName = "_index.json";
    private const string SeqFilePrefix = "_seq_";
    private const string SeqFileSuffix = ".json";

    private readonly string _dataRoot;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ILogger<BookingTransactionRepository> _logger;

    private readonly Dictionary<string, SemaphoreSlim> _bookingLocks = new();
    private readonly object _lockDictionaryLock = new();
    private bool _disposed;

    public BookingTransactionRepository(string dataRoot, ILogger<BookingTransactionRepository> logger)
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
    public async Task<BookingTransaction> AppendTransactionAsync(
        string bookingId, BookingTransaction transaction)
    {
        ValidateBookingId(bookingId);
        if (transaction is null) throw new ArgumentNullException(nameof(transaction));

        var txnDir = GetTransactionDir(bookingId);
        Directory.CreateDirectory(txnDir);

        var sem = GetLock(bookingId);
        await sem.WaitAsync();
        try
        {
            // Load or initialise index
            var index = await ReadIndexAsync(bookingId, txnDir);
            var seq = index.NextSequence;

            // Populate transaction
            transaction.BookingId = bookingId;
            transaction.Sequence = seq;
            if (transaction.Timestamp == default)
                transaction.Timestamp = DateTime.UtcNow;

            // Write transaction file
            var txnPath = GetSeqPath(txnDir, seq);
            await AtomicWriteAsync(txnPath, transaction);

            // Update index
            index.NextSequence = seq + 1;
            index.LastUpdated = DateTime.UtcNow;
            await AtomicWriteAsync(GetIndexPath(txnDir), index);

            _logger.LogInformation(
                "Booking transaction appended: booking={BookingId} seq={Seq} type={Type}",
                bookingId, seq, transaction.Type);

            return transaction;
        }
        finally
        {
            sem.Release();
        }
    }

    /// <inheritdoc />
    public async Task<List<BookingTransaction>> GetTransactionsAsync(string bookingId)
    {
        ValidateBookingId(bookingId);

        var txnDir = GetTransactionDir(bookingId);
        if (!Directory.Exists(txnDir))
            return new List<BookingTransaction>();

        var transactions = new List<BookingTransaction>();
        foreach (var file in GetSortedSeqFiles(txnDir))
        {
            var txn = await ReadTransactionAsync(file);
            if (txn is not null)
                transactions.Add(txn);
        }

        return transactions.OrderBy(t => t.Sequence).ToList();
    }

    /// <inheritdoc />
    public async Task<int> GetNextSequenceAsync(string bookingId)
    {
        ValidateBookingId(bookingId);

        var txnDir = GetTransactionDir(bookingId);
        if (!Directory.Exists(txnDir))
            return 1;

        var index = await ReadIndexAsync(bookingId, txnDir);
        return index.NextSequence;
    }

    // ─── IDisposable ────────────────────────────────────────────────────────

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        lock (_lockDictionaryLock)
        {
            foreach (var sem in _bookingLocks.Values)
                sem.Dispose();
            _bookingLocks.Clear();
        }
    }

    // ─── Private helpers ────────────────────────────────────────────────────

    private string GetTransactionDir(string bookingId) =>
        Path.Combine(_dataRoot, "bookings", $"{bookingId}_transactions");

    private static string GetIndexPath(string txnDir) =>
        Path.Combine(txnDir, IndexFileName);

    private static string GetSeqPath(string txnDir, int seq) =>
        Path.Combine(txnDir, $"{SeqFilePrefix}{seq:D6}{SeqFileSuffix}");

    private static IEnumerable<string> GetSortedSeqFiles(string txnDir) =>
        Directory.EnumerateFiles(txnDir, $"{SeqFilePrefix}*{SeqFileSuffix}")
                 .OrderBy(f => f);

    private SemaphoreSlim GetLock(string bookingId)
    {
        lock (_lockDictionaryLock)
        {
            if (!_bookingLocks.TryGetValue(bookingId, out var sem))
            {
                sem = new SemaphoreSlim(1, 1);
                _bookingLocks[bookingId] = sem;
            }
            return sem;
        }
    }

    private async Task<BookingTransactionIndex> ReadIndexAsync(string bookingId, string txnDir)
    {
        var indexPath = GetIndexPath(txnDir);
        if (!File.Exists(indexPath))
        {
            return new BookingTransactionIndex
            {
                BookingId = bookingId,
                NextSequence = 1,
                LastUpdated = DateTime.UtcNow
            };
        }

        var json = await File.ReadAllTextAsync(indexPath);
        return JsonSerializer.Deserialize<BookingTransactionIndex>(json, _jsonOptions)
               ?? new BookingTransactionIndex { BookingId = bookingId, NextSequence = 1 };
    }

    private async Task AtomicWriteAsync<T>(string targetPath, T value)
    {
        var tempPath = targetPath + ".tmp";
        var json = JsonSerializer.Serialize(value, _jsonOptions);
        await File.WriteAllTextAsync(tempPath, json);
        File.Move(tempPath, targetPath, overwrite: true);
    }

    private async Task<BookingTransaction?> ReadTransactionAsync(string filePath)
    {
        try
        {
            var json = await File.ReadAllTextAsync(filePath);
            return JsonSerializer.Deserialize<BookingTransaction>(json, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to read booking transaction from {File}", filePath);
            return null;
        }
    }

    private static void ValidateBookingId(string bookingId)
    {
        if (string.IsNullOrWhiteSpace(bookingId))
            throw new ArgumentException("Booking ID cannot be empty", nameof(bookingId));
    }
}

