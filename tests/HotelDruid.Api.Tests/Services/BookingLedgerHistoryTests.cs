using HotelDruid.Api.Models;
using HotelDruid.Api.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HotelDruid.Api.Tests.Services;

public class BookingLedgerHistoryTests : IAsyncLifetime
{
    private string _tempDataRoot = null!;
    private LedgerRepository _repo = null!;
    private BookingLedgerIndexRepository _index = null!;
    private FinancialPostingService _posting = null!;

    private static readonly DateTime Day = new(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc);

    public Task InitializeAsync()
    {
        _tempDataRoot = Path.Combine(Path.GetTempPath(), $"HotelDruid-ledger-index-{Guid.NewGuid()}");
        Directory.CreateDirectory(_tempDataRoot);
        _repo = new LedgerRepository(_tempDataRoot, new Mock<ILogger<LedgerRepository>>().Object);
        _index = new BookingLedgerIndexRepository(_tempDataRoot, new Mock<ILogger<BookingLedgerIndexRepository>>().Object);
        _posting = new FinancialPostingService(_repo, _index);
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

    [Fact]
    public async Task FirstRead_RecordsHistory_LaterJournalIsAppended()
    {
        await PostAsync(Day, "b1", "booking_stay_charge", 80m);
        await PostAsync(Day.AddDays(10), "b1", "booking_payment", 30m);
        await PostAsync(Day.AddDays(10), "b2", "booking_payment", 5m);

        Assert.Null(await _index.ReadIfPresentAsync("b1"));

        var first = await BookingTransactionProjection.LoadAsync(_repo, _index, "b1", DateTime.MinValue, DateTime.MaxValue);
        Assert.Equal(4, first.Count);
        Assert.Equal(4, (await _index.ReadIfPresentAsync("b1"))!.Count);
        Assert.DoesNotContain(first, line => line.BookingId == "b2");

        var second = await BookingTransactionProjection.LoadAsync(_repo, _index, "b1", DateTime.MinValue, DateTime.MaxValue);
        Assert.Equal(4, second.Count);

        await PostAsync(Day.AddDays(11), "b1", "booking_deposit", 20m);
        var third = await BookingTransactionProjection.LoadAsync(_repo, _index, "b1", DateTime.MinValue, DateTime.MaxValue);
        Assert.Equal(6, third.Count);
        Assert.Equal(6, (await _index.ReadIfPresentAsync("b1"))!.Count);
    }

    [Fact]
    public async Task EmptyRead_WritesAnEmptyIndex_NextJournalIsAppended()
    {
        var first = await BookingTransactionProjection.LoadAsync(_repo, _index, "empty", DateTime.MinValue, DateTime.MaxValue);
        Assert.Empty(first);
        Assert.Empty((await _index.ReadIfPresentAsync("empty"))!);

        await PostAsync(Day, "empty", "booking_payment", 15m);

        var stored = await _index.ReadIfPresentAsync("empty");
        Assert.Equal(2, stored!.Count);

        var second = await BookingTransactionProjection.LoadAsync(_repo, _index, "empty", DateTime.MinValue, DateTime.MaxValue);
        Assert.Equal(2, second.Count);
        Assert.All(second, line => Assert.Equal("empty", line.BookingId));
    }

    private Task PostAsync(DateTime when, string bookingId, string entryType, decimal amount)
        => _posting.PostBalancedAsync(
            when,
            entryType,
            entryType,
            amount,
            HotelAccounts.Cash,
            "asset",
            HotelAccounts.GuestLedger,
            "asset",
            bookingId: bookingId,
            sourceType: "booking-transaction",
            sourceId: Guid.NewGuid().ToString("N"));
}
