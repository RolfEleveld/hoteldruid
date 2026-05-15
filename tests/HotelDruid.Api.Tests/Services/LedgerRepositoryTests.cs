using HotelDruid.Api.Models;
using HotelDruid.Api.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HotelDruid.Api.Tests.Services;

/// <summary>
/// Unit tests for LedgerRepository.
/// Uses a temp directory — verifies both in-memory behavior and files on disk.
/// </summary>
public class LedgerRepositoryTests : IAsyncLifetime
{
    private string _tempDataRoot = null!;
    private LedgerRepository _repo = null!;
    private Mock<ILogger<LedgerRepository>> _loggerMock = null!;

    // Fixed test date so all tests use a predictable partition
    private static readonly DateTime TestDate = new DateTime(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc);

    public Task InitializeAsync()
    {
        _tempDataRoot = Path.Combine(Path.GetTempPath(), $"HotelDruid-ledger-tests-{Guid.NewGuid()}");
        Directory.CreateDirectory(_tempDataRoot);
        _loggerMock = new Mock<ILogger<LedgerRepository>>();
        _repo = new LedgerRepository(_tempDataRoot, _loggerMock.Object);
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

    // ─── Snapshot ───────────────────────────────────────────────────────────

    [Fact]
    public async Task GetSnapshotAsync_NoFile_ReturnsNull()
    {
        var result = await _repo.GetSnapshotAsync(TestDate);
        Assert.Null(result);
    }

    [Fact]
    public async Task SaveSnapshotAsync_ThenGetSnapshot_RoundTrips()
    {
        var snapshot = new LedgerSnapshot
        {
            Date = TestDate,
            Timestamp = TestDate,
            TotalDayBalance = 250.00m,
            SequenceAt = 0,
            Entries = new List<LedgerEntry>()
        };

        await _repo.SaveSnapshotAsync(TestDate, snapshot);
        var loaded = await _repo.GetSnapshotAsync(TestDate);

        Assert.NotNull(loaded);
        Assert.Equal(250.00m, loaded!.TotalDayBalance);
        Assert.Equal(0, loaded.SequenceAt);
    }

    [Fact]
    public async Task SaveSnapshotAsync_CreatesFileOnDisk()
    {
        var snapshot = new LedgerSnapshot { Date = TestDate, Timestamp = TestDate };
        await _repo.SaveSnapshotAsync(TestDate, snapshot);

        var expectedPath = Path.Combine(
            _tempDataRoot, "ledger", "2026", "05", "01", "_snapshot.json");
        Assert.True(File.Exists(expectedPath), $"Snapshot file not found: {expectedPath}");
    }

    [Fact]
    public async Task SaveSnapshotAsync_Overwrite_UpdatesExisting()
    {
        var snap1 = new LedgerSnapshot { Date = TestDate, Timestamp = TestDate, TotalDayBalance = 100m };
        await _repo.SaveSnapshotAsync(TestDate, snap1);

        var snap2 = new LedgerSnapshot { Date = TestDate, Timestamp = TestDate, TotalDayBalance = 999m };
        await _repo.SaveSnapshotAsync(TestDate, snap2);

        var loaded = await _repo.GetSnapshotAsync(TestDate);
        Assert.Equal(999m, loaded!.TotalDayBalance);
    }

    // ─── AppendEntryAsync ────────────────────────────────────────────────────

    [Fact]
    public async Task AppendEntryAsync_FirstEntry_GetsSequence1()
    {
        var entry = new LedgerEntry { EntryType = "payment", Amount = 50m };
        var result = await _repo.AppendEntryAsync(TestDate, entry);
        Assert.Equal(1, result.Sequence);
    }

    [Fact]
    public async Task AppendEntryAsync_MultipleEntries_SequencesAreIncremental()
    {
        var e1 = await _repo.AppendEntryAsync(TestDate, new LedgerEntry { EntryType = "payment", Amount = 10m });
        var e2 = await _repo.AppendEntryAsync(TestDate, new LedgerEntry { EntryType = "payment", Amount = 20m });
        var e3 = await _repo.AppendEntryAsync(TestDate, new LedgerEntry { EntryType = "refund", Amount = 5m });

        Assert.Equal(1, e1.Sequence);
        Assert.Equal(2, e2.Sequence);
        Assert.Equal(3, e3.Sequence);
    }

    [Fact]
    public async Task AppendEntryAsync_AutoAssignsId()
    {
        var entry = new LedgerEntry { EntryType = "payment", Amount = 30m };
        var result = await _repo.AppendEntryAsync(TestDate, entry);
        Assert.NotEmpty(result.Id!);
    }

    [Fact]
    public async Task AppendEntryAsync_AutoAssignsTimestamp()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);
        var entry = new LedgerEntry { EntryType = "payment" };
        var result = await _repo.AppendEntryAsync(TestDate, entry);
        Assert.True(result.Timestamp >= before);
    }

    [Fact]
    public async Task AppendEntryAsync_CreatesSeqFileOnDisk()
    {
        var entry = new LedgerEntry { EntryType = "payment" };
        await _repo.AppendEntryAsync(TestDate, entry);

        var expectedPath = Path.Combine(
            _tempDataRoot, "ledger", "2026", "05", "01", "_seq_000001.json");
        Assert.True(File.Exists(expectedPath), $"Seq file not found: {expectedPath}");
    }

    [Fact]
    public async Task AppendEntryAsync_PreservesExistingId()
    {
        var entry = new LedgerEntry { Id = "my-custom-id", EntryType = "payment" };
        var result = await _repo.AppendEntryAsync(TestDate, entry);
        Assert.Equal("my-custom-id", result.Id);
    }

    // ─── GetEntriesSinceSequenceAsync ────────────────────────────────────────

    [Fact]
    public async Task GetEntriesSinceSequenceAsync_AfterSeq0_ReturnsAll()
    {
        await _repo.AppendEntryAsync(TestDate, new LedgerEntry { EntryType = "a" });
        await _repo.AppendEntryAsync(TestDate, new LedgerEntry { EntryType = "b" });
        await _repo.AppendEntryAsync(TestDate, new LedgerEntry { EntryType = "c" });

        var entries = await _repo.GetEntriesSinceSequenceAsync(TestDate, 0);
        Assert.Equal(3, entries.Count);
    }

    [Fact]
    public async Task GetEntriesSinceSequenceAsync_AfterSeq2_ReturnsTail()
    {
        await _repo.AppendEntryAsync(TestDate, new LedgerEntry { EntryType = "a" });
        await _repo.AppendEntryAsync(TestDate, new LedgerEntry { EntryType = "b" });
        await _repo.AppendEntryAsync(TestDate, new LedgerEntry { EntryType = "c" });

        var entries = await _repo.GetEntriesSinceSequenceAsync(TestDate, 2);
        Assert.Single(entries);
        Assert.Equal(3, entries[0].Sequence);
    }

    [Fact]
    public async Task GetEntriesSinceSequenceAsync_NoPartition_ReturnsEmpty()
    {
        var entries = await _repo.GetEntriesSinceSequenceAsync(
            new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc), 0);
        Assert.Empty(entries);
    }

    // ─── GetEntriesForDateAsync ──────────────────────────────────────────────

    [Fact]
    public async Task GetEntriesForDateAsync_ReturnsAllEntriesForDate()
    {
        await _repo.AppendEntryAsync(TestDate, new LedgerEntry { EntryType = "x" });
        await _repo.AppendEntryAsync(TestDate, new LedgerEntry { EntryType = "y" });

        var entries = await _repo.GetEntriesForDateAsync(TestDate);
        Assert.Equal(2, entries.Count);
    }

    [Fact]
    public async Task GetEntriesForDateAsync_NoData_ReturnsEmpty()
    {
        var entries = await _repo.GetEntriesForDateAsync(
            new DateTime(2020, 6, 1, 0, 0, 0, DateTimeKind.Utc));
        Assert.Empty(entries);
    }

    // ─── GetEntriesForBookingAsync ───────────────────────────────────────────

    [Fact]
    public async Task GetEntriesForBookingAsync_MatchesBookingId()
    {
        await _repo.AppendEntryAsync(TestDate, new LedgerEntry { EntryType = "a", BookingId = "booking-1" });
        await _repo.AppendEntryAsync(TestDate, new LedgerEntry { EntryType = "b", BookingId = "booking-2" });
        await _repo.AppendEntryAsync(TestDate, new LedgerEntry { EntryType = "c", BookingId = "booking-1" });

        var entries = await _repo.GetEntriesForBookingAsync("booking-1", TestDate, TestDate);

        Assert.Equal(2, entries.Count);
        Assert.All(entries, e => Assert.Equal("booking-1", e.BookingId));
    }

    [Fact]
    public async Task GetEntriesForBookingAsync_SpansMultipleDays_ReturnsAll()
    {
        var day1 = TestDate;
        var day2 = TestDate.AddDays(1);

        await _repo.AppendEntryAsync(day1, new LedgerEntry { EntryType = "pay", BookingId = "booking-X" });
        await _repo.AppendEntryAsync(day2, new LedgerEntry { EntryType = "pay", BookingId = "booking-X" });

        var entries = await _repo.GetEntriesForBookingAsync("booking-X", day1, day2);
        Assert.Equal(2, entries.Count);
    }

    // ─── GetConsolidatedAsync ────────────────────────────────────────────────

    [Fact]
    public async Task GetConsolidatedAsync_NoData_ReturnsEmpty()
    {
        var result = await _repo.GetConsolidatedAsync(
            new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetConsolidatedAsync_SnapshotPlusIncrementals_ReturnsMerged()
    {
        // Seed two entries, then create a snapshot capturing both
        var e1 = await _repo.AppendEntryAsync(TestDate, new LedgerEntry { EntryType = "initial" });
        var e2 = await _repo.AppendEntryAsync(TestDate, new LedgerEntry { EntryType = "initial2" });

        var snapshot = new LedgerSnapshot
        {
            Date = TestDate,
            Timestamp = TestDate,
            SequenceAt = 2,
            Entries = new List<LedgerEntry> { e1, e2 },
            TotalDayBalance = 0
        };
        await _repo.SaveSnapshotAsync(TestDate, snapshot);

        // Append one more entry after the snapshot
        var e3 = await _repo.AppendEntryAsync(TestDate, new LedgerEntry { EntryType = "post-snapshot" });

        var result = await _repo.GetConsolidatedAsync(TestDate);

        // Should contain snapshot entries + the new incremental entry (e3 only, since e1/e2 are in snapshot)
        // e3 has sequence 3 which is > snapshot.SequenceAt (2), so it's included from incremental
        // snapshot has e1 and e2
        Assert.Equal(3, result.Count);
        Assert.Equal(1, result[0].Sequence);
        Assert.Equal(2, result[1].Sequence);
        Assert.Equal(3, result[2].Sequence);
    }

    [Fact]
    public async Task GetConsolidatedAsync_OnlyIncrementals_ReturnsAll()
    {
        await _repo.AppendEntryAsync(TestDate, new LedgerEntry { EntryType = "inc1" });
        await _repo.AppendEntryAsync(TestDate, new LedgerEntry { EntryType = "inc2" });

        var result = await _repo.GetConsolidatedAsync(TestDate);

        Assert.Equal(2, result.Count);
        Assert.Equal(1, result[0].Sequence);
        Assert.Equal(2, result[1].Sequence);
    }

    // ─── Concurrent appends ──────────────────────────────────────────────────

    [Fact]
    public async Task AppendEntryAsync_ConcurrentAppends_UniqueSequences()
    {
        const int taskCount = 10;
        var tasks = Enumerable.Range(0, taskCount)
            .Select(_ => _repo.AppendEntryAsync(TestDate, new LedgerEntry { EntryType = "concurrent" }))
            .ToList();

        var results = await Task.WhenAll(tasks);

        var sequences = results.Select(r => r.Sequence).ToList();
        Assert.Equal(taskCount, sequences.Distinct().Count());
        Assert.Equal(taskCount, sequences.Max());
    }
}

