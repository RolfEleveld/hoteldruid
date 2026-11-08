using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using HotelDruid.Api;
using Microsoft.AspNetCore.Mvc.Testing;

namespace HotelDruid.Api.Tests.Integration;

/// <summary>
/// HTTP checks for the ledger rules that already exist:
/// a payment is a balanced pair, booking id is a filter, a deposit credits
/// liability 2400 and does not touch room revenue, and petty cash / walk-up
/// income leave the booking reference empty.
/// </summary>
[Collection("Sequential")]
public class LedgerApiTests : IAsyncLifetime
{
    private const string Cash = "1000";
    private const string GuestLedger = "1200";
    private const string GuestDeposits = "2400";
    private const string RoomRevenue = "4100";
    private const string OtherIncome = "4900";
    private const string OperatingExpense = "6100";

    private static readonly DateOnly PostDate = new(2026, 5, 15);
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;
    private string _testDataRoot = null!;

    public Task InitializeAsync()
    {
        _testDataRoot = Path.Combine(Path.GetTempPath(), $"api-ledger-tests-{Guid.NewGuid()}");
        Directory.CreateDirectory(_testDataRoot);

        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => builder.UseSetting("DataRoot", _testDataRoot));
        _client = _factory.CreateClient();
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        _client.Dispose();
        _factory.Dispose();
        if (Directory.Exists(_testDataRoot))
        {
            try { Directory.Delete(_testDataRoot, recursive: true); } catch { }
        }

        return Task.CompletedTask;
    }

    [Fact]
    public async Task Payment_PostsBalancedJournal_AndFiltersByBooking()
    {
        var booking = await CreateBookingAsync("guest-a");
        var other = await CreateBookingAsync("guest-b");

        var created = await _client.PostAsJsonAsync(
            $"/api/bookings/2026/{booking}/transactions",
            new
            {
                type = "payment",
                amount = 80m,
                description = "Card payment",
                timestamp = new DateTime(2026, 5, 15, 10, 0, 0, DateTimeKind.Utc)
            },
            JsonOptions);
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);

        var lines = await GetLinesAsync(bookingId: booking);
        Assert.Equal(2, lines.Count);
        Assert.Single(lines.Select(line => line.JournalId).Distinct());
        Assert.Equal(lines.Sum(line => line.Debit), lines.Sum(line => line.Credit));
        Assert.Contains(lines, line => line.AccountCode == Cash && line.Debit == 80m && line.Credit == 0m);
        Assert.Contains(lines, line => line.AccountCode == GuestLedger && line.Credit == 80m && line.Debit == 0m);
        Assert.All(lines, line => Assert.Equal(booking, line.BookingId));
        Assert.DoesNotContain(lines, line => line.AccountCode == RoomRevenue);

        var otherLines = await GetLinesAsync(bookingId: other);
        Assert.Empty(otherLines);
    }

    [Fact]
    public async Task Deposit_CreditsLiability_AndDoesNotReduceRoomRevenue()
    {
        var booking = await CreateBookingAsync("guest-deposit");

        var created = await _client.PostAsJsonAsync(
            $"/api/bookings/2026/{booking}/transactions",
            new
            {
                type = "deposit",
                amount = 50m,
                description = "Arrival deposit",
                appliedTo = "deposit",
                timestamp = new DateTime(2026, 5, 15, 11, 0, 0, DateTimeKind.Utc)
            },
            JsonOptions);
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);

        var lines = await GetLinesAsync(bookingId: booking);
        Assert.Equal(2, lines.Count);
        Assert.Equal(lines.Sum(line => line.Debit), lines.Sum(line => line.Credit));
        Assert.Contains(lines, line => line.AccountCode == Cash && line.Debit == 50m);
        Assert.Contains(lines, line => line.AccountCode == GuestDeposits && line.AccountGroup == "liability" && line.Credit == 50m);
        Assert.Equal(0m, lines.Where(line => line.AccountCode == RoomRevenue).Sum(line => line.Credit - line.Debit));
        Assert.DoesNotContain(lines, line => line.AccountCode == RoomRevenue);
    }

    [Fact]
    public async Task PettyCash_LeavesBookingReferenceEmpty_AndAppearsOnlyAsUnassigned()
    {
        var booking = await CreateBookingAsync("guest-nearby");
        var marker = $"coffee-{Guid.NewGuid():N}";

        var created = await _client.PostAsJsonAsync(
            "/api/money-history",
            new
            {
                year = 2026,
                cashRegisterId = "CR1",
                amount = -4.0,
                type = "expense",
                description = marker,
                date = PostDate
            },
            JsonOptions);
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);

        var unassigned = (await GetLinesAsync(unassigned: true))
            .Where(line => line.Description == marker)
            .ToList();
        Assert.Equal(2, unassigned.Count);
        Assert.Equal(unassigned.Sum(line => line.Debit), unassigned.Sum(line => line.Credit));
        Assert.All(unassigned, line => Assert.True(string.IsNullOrWhiteSpace(line.BookingId)));
        Assert.Contains(unassigned, line => line.AccountCode == OperatingExpense && line.Debit == 4m);
        Assert.Contains(unassigned, line => line.AccountCode == Cash && line.Credit == 4m);
        Assert.Contains(unassigned, line => line.EntryType == "petty_cash_expense" && line.CashRegisterId == "CR1");

        var bookingLines = await GetLinesAsync(bookingId: booking);
        Assert.DoesNotContain(bookingLines, line => line.Description == marker);
    }

    [Fact]
    public async Task NonStayingPayment_CreditsOtherIncome_NotRoomRevenue()
    {
        var marker = $"walk-up-{Guid.NewGuid():N}";

        var created = await _client.PostAsJsonAsync(
            "/api/money-history",
            new
            {
                year = 2026,
                cashRegisterId = "CR2",
                amount = 25.0,
                type = "income",
                description = marker,
                date = PostDate
            },
            JsonOptions);
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);

        var lines = (await GetLinesAsync(unassigned: true))
            .Where(line => line.Description == marker)
            .ToList();
        Assert.Equal(2, lines.Count);
        Assert.Equal(lines.Sum(line => line.Debit), lines.Sum(line => line.Credit));
        Assert.All(lines, line => Assert.True(string.IsNullOrWhiteSpace(line.BookingId)));
        Assert.Contains(lines, line => line.AccountCode == Cash && line.Debit == 25m);
        Assert.Contains(lines, line => line.AccountCode == OtherIncome && line.AccountGroup == "revenue" && line.Credit == 25m);
        Assert.DoesNotContain(lines, line => line.AccountCode == RoomRevenue);
        Assert.Contains(lines, line => line.EntryType == "petty_cash_income");
    }

    [Fact]
    public async Task BookingTransactions_ProjectOneRowPerJournal_AndDoNotWriteASecondBook()
    {
        var booking = await CreateBookingAsync("guest-projection");

        var payment = await _client.PostAsJsonAsync(
            $"/api/bookings/2026/{booking}/transactions",
            new
            {
                type = "payment",
                amount = 80m,
                description = "Card payment",
                appliedTo = "stay_balance",
                createdBy = "clerk",
                timestamp = new DateTime(2026, 5, 15, 10, 0, 0, DateTimeKind.Utc)
            },
            JsonOptions);
        Assert.Equal(HttpStatusCode.Created, payment.StatusCode);
        var createdPayment = await payment.Content.ReadFromJsonAsync<BookingTransactionRow>(JsonOptions);
        Assert.NotNull(createdPayment);
        Assert.Equal(1, createdPayment.Sequence);
        Assert.Equal("payment", createdPayment.Type);
        Assert.Equal(80m, createdPayment.Amount);
        Assert.Equal("Card payment", createdPayment.Description);
        Assert.Equal("stay_balance", createdPayment.AppliedTo);
        Assert.Equal("clerk", createdPayment.CreatedBy);

        var refund = await _client.PostAsJsonAsync(
            $"/api/bookings/2026/{booking}/transactions",
            new
            {
                type = "refund",
                amount = 4m,
                description = "Overpayment",
                appliedTo = "stay_balance",
                createdBy = "clerk",
                timestamp = new DateTime(2026, 5, 15, 11, 0, 0, DateTimeKind.Utc)
            },
            JsonOptions);
        Assert.Equal(HttpStatusCode.Created, refund.StatusCode);
        var createdRefund = await refund.Content.ReadFromJsonAsync<BookingTransactionRow>(JsonOptions);
        Assert.NotNull(createdRefund);
        Assert.Equal(2, createdRefund.Sequence);
        Assert.Equal("refund", createdRefund.Type);
        Assert.Equal(4m, createdRefund.Amount);
        Assert.Equal("stay_balance", createdRefund.AppliedTo);
        Assert.Equal("clerk", createdRefund.CreatedBy);

        var secondBook = Path.Combine(_testDataRoot, "bookings", $"{booking}_transactions");
        Assert.False(Directory.Exists(secondBook));

        var listResponse = await _client.GetAsync($"/api/bookings/2026/{Uri.EscapeDataString(booking)}/transactions");
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);
        var list = await listResponse.Content.ReadFromJsonAsync<List<BookingTransactionRow>>(JsonOptions) ?? [];
        Assert.Equal(2, list.Count);
        Assert.Equal(new[] { 1, 2 }, list.Select(row => row.Sequence).OrderBy(sequence => sequence).ToArray());
        Assert.Contains(list, row => row.Type == "payment" && row.Amount == 80m && row.AppliedTo == "stay_balance" && row.CreatedBy == "clerk");
        Assert.Contains(list, row => row.Type == "refund" && row.Amount == 4m && row.AppliedTo == "stay_balance" && row.CreatedBy == "clerk");

        var paymentLines = (await GetLinesAsync(bookingId: booking))
            .Where(line => line.Description == "Card payment")
            .ToList();
        Assert.Equal(2, paymentLines.Count);
        Assert.Single(paymentLines.Select(line => line.JournalId).Distinct());
        Assert.Equal(paymentLines.Sum(line => line.Debit), paymentLines.Sum(line => line.Credit));

        var financeResponse = await _client.GetAsync($"/api/bookings/2026/{Uri.EscapeDataString(booking)}/finance");
        Assert.Equal(HttpStatusCode.OK, financeResponse.StatusCode);
        var finance = await financeResponse.Content.ReadFromJsonAsync<FinanceSummary>(JsonOptions);
        Assert.NotNull(finance);
        Assert.Equal(80m, finance.PaymentsTotal);
        Assert.Equal(4m, finance.RefundsTotal);
    }

    private async Task<string> CreateBookingAsync(string clientId)
    {
        var response = await _client.PostAsJsonAsync(
            "/api/bookings",
            new
            {
                year = 2026,
                clientId,
                status = "Confirmed",
                arrivalDate = "2026-05-15",
                departureDate = "2026-05-17"
            },
            JsonOptions);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var booking = await response.Content.ReadFromJsonAsync<BookingCreated>(JsonOptions);
        Assert.False(string.IsNullOrWhiteSpace(booking?.Id));
        return booking!.Id!;
    }

    private async Task<List<LedgerLine>> GetLinesAsync(string? bookingId = null, bool unassigned = false)
    {
        var query = $"/api/ledger/lines?from={PostDate:yyyy-MM-dd}&to={PostDate:yyyy-MM-dd}";
        if (!string.IsNullOrWhiteSpace(bookingId))
            query += $"&bookingId={Uri.EscapeDataString(bookingId)}";
        if (unassigned)
            query += "&unassigned=true";

        var response = await _client.GetAsync(query);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return await response.Content.ReadFromJsonAsync<List<LedgerLine>>(JsonOptions) ?? [];
    }

    private sealed class BookingCreated
    {
        public string? Id { get; set; }
    }

    private sealed class LedgerLine
    {
        public string? JournalId { get; set; }
        public string? EntryType { get; set; }
        public string? BookingId { get; set; }
        public string? AccountCode { get; set; }
        public string? AccountGroup { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string? CashRegisterId { get; set; }
        public string? Description { get; set; }
    }

    private sealed class BookingTransactionRow
    {
        public int Sequence { get; set; }
        public string? Type { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public string? AppliedTo { get; set; }
        public string? CreatedBy { get; set; }
    }

    private sealed class FinanceSummary
    {
        public decimal PaymentsTotal { get; set; }
        public decimal RefundsTotal { get; set; }
    }
}
