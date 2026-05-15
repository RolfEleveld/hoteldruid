using HotelDruid.Api.Models;
using HotelDruid.Api.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HotelDruid.Api.Tests.Services;

/// <summary>
/// Unit tests for BookingTransactionRepository.
/// Uses a temp directory — verifies both in-memory behavior and files on disk.
/// </summary>
public class BookingTransactionRepositoryTests : IAsyncLifetime
{
    private string _tempDataRoot = null!;
    private BookingTransactionRepository _repo = null!;
    private Mock<ILogger<BookingTransactionRepository>> _loggerMock = null!;

    private const string BookingId = "booking-test-001";

    public Task InitializeAsync()
    {
        _tempDataRoot = Path.Combine(Path.GetTempPath(), $"HotelDruid-txn-tests-{Guid.NewGuid()}");
        Directory.CreateDirectory(_tempDataRoot);
        _loggerMock = new Mock<ILogger<BookingTransactionRepository>>();
        _repo = new BookingTransactionRepository(_tempDataRoot, _loggerMock.Object);
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        _repo.Dispose();
        if (Directory.Exists(_tempDataRoot))
        {
            try { Directory.Delete(_tempDataRoot, recursive: true); }
            catch { /* ignore test cleanup errors */ }
        }
        return Task.CompletedTask;
    }

    // ─── AppendTransactionAsync ──────────────────────────────────────────────

    [Fact]
    public async Task AppendTransactionAsync_FirstTransaction_GetsSequence1()
    {
        var txn = new BookingTransaction { Type = "checkin_charge", Amount = 150m };
        var result = await _repo.AppendTransactionAsync(BookingId, txn);
        Assert.Equal(1, result.Sequence);
    }

    [Fact]
    public async Task AppendTransactionAsync_MultipleTransactions_SequencesAreIncremental()
    {
        var t1 = await _repo.AppendTransactionAsync(BookingId,
            new BookingTransaction { Type = "checkin_charge", Amount = 150m });
        var t2 = await _repo.AppendTransactionAsync(BookingId,
            new BookingTransaction { Type = "additional_charge", Amount = 20m });
        var t3 = await _repo.AppendTransactionAsync(BookingId,
            new BookingTransaction { Type = "payment", Amount = 170m });

        Assert.Equal(1, t1.Sequence);
        Assert.Equal(2, t2.Sequence);
        Assert.Equal(3, t3.Sequence);
    }

    [Fact]
    public async Task AppendTransactionAsync_SetsBookingIdOnResult()
    {
        var txn = new BookingTransaction { Type = "payment", Amount = 50m };
        var result = await _repo.AppendTransactionAsync(BookingId, txn);
        Assert.Equal(BookingId, result.BookingId);
    }

    [Fact]
    public async Task AppendTransactionAsync_AutoAssignsTimestamp()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);
        var txn = new BookingTransaction { Type = "payment" };
        var result = await _repo.AppendTransactionAsync(BookingId, txn);
        Assert.True(result.Timestamp >= before);
    }

    [Fact]
    public async Task AppendTransactionAsync_PreservesExistingTimestamp()
    {
        var fixedTime = new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc);
        var txn = new BookingTransaction { Type = "payment", Timestamp = fixedTime };
        var result = await _repo.AppendTransactionAsync(BookingId, txn);
        Assert.Equal(fixedTime, result.Timestamp);
    }

    [Fact]
    public async Task AppendTransactionAsync_CreatesSeqFileOnDisk()
    {
        var txn = new BookingTransaction { Type = "checkin_charge", Amount = 100m };
        await _repo.AppendTransactionAsync(BookingId, txn);

        var expectedPath = Path.Combine(
            _tempDataRoot, "bookings", $"{BookingId}_transactions", "_seq_000001.json");
        Assert.True(File.Exists(expectedPath), $"Transaction file not found: {expectedPath}");
    }

    [Fact]
    public async Task AppendTransactionAsync_CreatesIndexFileOnDisk()
    {
        await _repo.AppendTransactionAsync(BookingId, new BookingTransaction { Type = "payment" });

        var indexPath = Path.Combine(
            _tempDataRoot, "bookings", $"{BookingId}_transactions", "_index.json");
        Assert.True(File.Exists(indexPath), $"Index file not found: {indexPath}");
    }

    [Fact]
    public async Task AppendTransactionAsync_EmptyBookingId_ThrowsArgumentException()
    {
        var txn = new BookingTransaction { Type = "payment" };
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _repo.AppendTransactionAsync(string.Empty, txn));
    }

    [Fact]
    public async Task AppendTransactionAsync_NullTransaction_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _repo.AppendTransactionAsync(BookingId, null!));
    }

    // ─── GetTransactionsAsync ────────────────────────────────────────────────

    [Fact]
    public async Task GetTransactionsAsync_NoTransactions_ReturnsEmpty()
    {
        var result = await _repo.GetTransactionsAsync("nonexistent-booking");
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetTransactionsAsync_ReturnsAllTransactionsOrdered()
    {
        await _repo.AppendTransactionAsync(BookingId, new BookingTransaction { Type = "checkin_charge", Amount = 100m });
        await _repo.AppendTransactionAsync(BookingId, new BookingTransaction { Type = "payment", Amount = 100m });
        await _repo.AppendTransactionAsync(BookingId, new BookingTransaction { Type = "refund", Amount = 10m });

        var result = await _repo.GetTransactionsAsync(BookingId);

        Assert.Equal(3, result.Count);
        Assert.Equal(1, result[0].Sequence);
        Assert.Equal(2, result[1].Sequence);
        Assert.Equal(3, result[2].Sequence);
    }

    [Fact]
    public async Task GetTransactionsAsync_RoundTripsAllFields()
    {
        var txn = new BookingTransaction
        {
            Type = "additional_charge",
            Amount = 35.50m,
            Description = "Mini-bar",
            AppliedTo = "extras",
            CreatedBy = "staff@hotel.com"
        };
        await _repo.AppendTransactionAsync(BookingId, txn);

        var result = await _repo.GetTransactionsAsync(BookingId);

        Assert.Single(result);
        var loaded = result[0];
        Assert.Equal("additional_charge", loaded.Type);
        Assert.Equal(35.50m, loaded.Amount);
        Assert.Equal("Mini-bar", loaded.Description);
        Assert.Equal("extras", loaded.AppliedTo);
        Assert.Equal("staff@hotel.com", loaded.CreatedBy);
    }

    // ─── GetNextSequenceAsync ────────────────────────────────────────────────

    [Fact]
    public async Task GetNextSequenceAsync_NoTransactions_Returns1()
    {
        var next = await _repo.GetNextSequenceAsync("new-booking-id");
        Assert.Equal(1, next);
    }

    [Fact]
    public async Task GetNextSequenceAsync_AfterAppend_Increments()
    {
        await _repo.AppendTransactionAsync(BookingId, new BookingTransaction { Type = "payment" });
        await _repo.AppendTransactionAsync(BookingId, new BookingTransaction { Type = "payment" });

        var next = await _repo.GetNextSequenceAsync(BookingId);
        Assert.Equal(3, next);
    }

    // ─── Multiple bookings isolation ────────────────────────────────────────

    [Fact]
    public async Task Transactions_DifferentBookings_AreIsolated()
    {
        await _repo.AppendTransactionAsync("booking-A", new BookingTransaction { Type = "payment", Amount = 100m });
        await _repo.AppendTransactionAsync("booking-A", new BookingTransaction { Type = "payment", Amount = 50m });
        await _repo.AppendTransactionAsync("booking-B", new BookingTransaction { Type = "checkin_charge", Amount = 200m });

        var txnsA = await _repo.GetTransactionsAsync("booking-A");
        var txnsB = await _repo.GetTransactionsAsync("booking-B");

        Assert.Equal(2, txnsA.Count);
        Assert.Single(txnsB);
        Assert.All(txnsA, t => Assert.Equal("booking-A", t.BookingId));
        Assert.All(txnsB, t => Assert.Equal("booking-B", t.BookingId));
    }

    [Fact]
    public async Task Transactions_DifferentBookings_SequencesStartAt1Each()
    {
        var tA1 = await _repo.AppendTransactionAsync("booking-C", new BookingTransaction { Type = "payment" });
        var tB1 = await _repo.AppendTransactionAsync("booking-D", new BookingTransaction { Type = "payment" });

        Assert.Equal(1, tA1.Sequence);
        Assert.Equal(1, tB1.Sequence);
    }

    // ─── Concurrent appends ──────────────────────────────────────────────────

    [Fact]
    public async Task AppendTransactionAsync_ConcurrentAppends_UniqueSequences()
    {
        const int taskCount = 10;
        var tasks = Enumerable.Range(0, taskCount)
            .Select(_ => _repo.AppendTransactionAsync(
                BookingId, new BookingTransaction { Type = "concurrent" }))
            .ToList();

        var results = await Task.WhenAll(tasks);

        var sequences = results.Select(r => r.Sequence).ToList();
        Assert.Equal(taskCount, sequences.Distinct().Count());
        Assert.Equal(taskCount, sequences.Max());
    }
}

