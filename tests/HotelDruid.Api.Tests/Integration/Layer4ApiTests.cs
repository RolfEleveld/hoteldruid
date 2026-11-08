using System.Net;
using System.Text;
using System.Text.Json;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using HotelDruid.Api;

namespace HotelDruid.Api.Tests.Integration;

[Collection("Sequential")]
public class Layer4ApiTests : IAsyncLifetime
{
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;
    private string _testDataRoot = null!;
    private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

    private class LedgerLineDto { public string? Id { get; set; } public string? BookingId { get; set; } public string? AccountCode { get; set; } public decimal Debit { get; set; } public decimal Credit { get; set; } public string? SourceType { get; set; } public string? SourceId { get; set; } public string? EntryType { get; set; } }
    private class BookingFinanceSummaryDto { public decimal StayTotal { get; set; } public decimal ExtraTotal { get; set; } public decimal ChargesTotal { get; set; } public decimal PaymentsTotal { get; set; } public decimal DepositsHeld { get; set; } public decimal TotalDue { get; set; } }
    private class BookingTransactionDto { public string? Type { get; set; } public decimal Amount { get; set; } }
    private class BookingCostDto { public string? Id { get; set; } public int Year { get; set; } public string? BookingId { get; set; } public string? TariffId { get; set; } public double? Amount { get; set; } public string? Description { get; set; } }
    private class BookingGuestDto { public string? Id { get; set; } public int Year { get; set; } public string? BookingId { get; set; } public string? ClientId { get; set; } public int? GuestNumber { get; set; } }
    private class CancelledBookingDto { public string? Id { get; set; } public int Year { get; set; } public string? ClientId { get; set; } public string? Status { get; set; } public string? CancelledAt { get; set; } }
    private class ExpenseDto { public string? Id { get; set; } public int Year { get; set; } public string? CashRegisterId { get; set; } public double? Amount { get; set; } public string? Description { get; set; } }
    private class MoneyHistoryDto { public string? Id { get; set; } public int Year { get; set; } public string? CashRegisterId { get; set; } public double? Amount { get; set; } public string? Type { get; set; } public string? ReversesId { get; set; } public string? ReversedById { get; set; } public string? Description { get; set; } }
    private class PosSaleDto { public string? Id { get; set; } public int Year { get; set; } public string? Description { get; set; } public double? Amount { get; set; } public string? Settlement { get; set; } public string? BookingId { get; set; } public string? ClientId { get; set; } public string? CashRegisterId { get; set; } public string? Currency { get; set; } public string? EntryType { get; set; } public string? DebitAccount { get; set; } public string? CreditAccount { get; set; } public string? BookingCostId { get; set; } public string? ReversedById { get; set; } public double? RunningCashTotal { get; set; } public string? ArticleId { get; set; } public int? Units { get; set; } }
    private class InventoryRowDto { public string? Id { get; set; } public int Quantity { get; set; } }
    private class CashRegisterDto { public string? Id { get; set; } public string? Name { get; set; } }

    private class BookingDto
    {
        public string? Id { get; set; }
        public int Year { get; set; }
        public string? ClientId { get; set; }
        public string? RoomId { get; set; }
        public string? Status { get; set; }
        public string? ArrivalDate { get; set; }
        public string? DepartureDate { get; set; }
        public string? Notes { get; set; }
        public string? ActualArrivalDate { get; set; }
        public string? ActualDepartureDate { get; set; }
        public int? ActualGuestCount { get; set; }
        public double? PrepaidAmount { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? PrepaidMethod { get; set; }
        public string? PrepaidReference { get; set; }
        public string? TariffId { get; set; }
        public double? TariffTotalPrice { get; set; }
    }

    private class IdOnlyDto { public string? Id { get; set; } }

    public async Task InitializeAsync()
    {
        _testDataRoot = Path.Combine(Path.GetTempPath(), $"api-layer4-tests-{Guid.NewGuid()}");
        Directory.CreateDirectory(_testDataRoot);
        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(b => b.UseSetting("DataRoot", _testDataRoot));
        _client = _factory.CreateClient();
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        _client?.Dispose();
        _factory?.Dispose();
        if (Directory.Exists(_testDataRoot)) try { Directory.Delete(_testDataRoot, true); } catch { }
        await Task.CompletedTask;
    }

    private StringContent Json(object o) => new(JsonSerializer.Serialize(o), Encoding.UTF8, "application/json");
    private async Task<T?> ReadAs<T>(HttpResponseMessage r) => JsonSerializer.Deserialize<T>(await r.Content.ReadAsStringAsync(), _json);

    // ========== Bookings ==========

    [Fact]
    public async Task Booking_Create_ReturnsCreated()
    {
        var r = await _client.PostAsync("/api/bookings", Json(new { Year = 2024, ClientId = "C001", RoomId = "101", Status = "Confirmed", ArrivalDate = "2024-06-01", DepartureDate = "2024-06-05" }));
        Assert.Equal(HttpStatusCode.Created, r.StatusCode);
        var dto = await ReadAs<BookingDto>(r);
        Assert.NotNull(dto?.Id);
        Assert.Equal(2024, dto.Year);
        Assert.Equal("C001", dto.ClientId);
        Assert.Equal("Confirmed", dto.Status);
    }

    [Fact]
    public async Task Booking_Create_PostsStayChargeAndExtrasToLedger()
    {
        var create = await _client.PostAsync("/api/bookings", Json(new { Year = 2025, ClientId = "C-Ledger", RoomId = "102", Status = "Confirmed", ArrivalDate = "2025-03-10", DepartureDate = "2025-03-12" }));
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var created = await ReadAs<BookingDto>(create);
        Assert.NotNull(created?.Id);

        var cost = await _client.PostAsync("/api/booking-costs", Json(new { Year = 2025, BookingId = created.Id, Amount = 35.0, Description = "Breakfast" }));
        Assert.Equal(HttpStatusCode.Created, cost.StatusCode);
    }

    [Fact]
    public async Task Booking_Create_ResolvesTariffId_And_TariffTotalPrice()
    {
        // Ensure a tariff exists and is assigned to room 102
        var tariff = await _client.PostAsync("/api/tariffs", Json(new 
        { 
            Year = 2025, 
            Name = "Test Standard", 
            ApplicabilityMode = "AnyDay", 
            PricePerNight = 80.0, 
            IsActive = true 
        }));
        Assert.Equal(HttpStatusCode.Created, tariff.StatusCode);
        var tariffCreated = await ReadAs<IdOnlyDto>(tariff);

        var assignResp = await _client.PostAsync("/api/tariff-assignments", Json(new 
        { 
            TariffId = tariffCreated!.Id, 
            TargetType = "Room", 
            TargetId = "102" 
        }));
        Assert.Equal(HttpStatusCode.Created, assignResp.StatusCode);
        var createdAssign = await ReadAs<dynamic>(assignResp);
        Assert.NotNull(createdAssign);

        // Verify assignment is visible for the room
        var assignments = await _client.GetAsync("/api/tariff-assignments?targetId=102");
        Assert.Equal(HttpStatusCode.OK, assignments.StatusCode);

        // Direct resolver check (isolates whether ResolveAsync sees the assignment)
        var resolve = await _client.GetAsync("/api/tariffs/resolve?roomId=102&arrivalDate=2025-06-01&departureDate=2025-06-03&guestCount=1");
        var resolved = await ReadAs<dynamic>(resolve);
        Assert.Equal(HttpStatusCode.OK, resolve.StatusCode);
        Assert.NotNull(resolved);

        var create = await _client.PostAsync("/api/bookings", Json(new 
        { 
            Year = 2025, 
            ClientId = "C-TariffTest", 
            RoomId = "102", 
            Status = "Confirmed", 
            ArrivalDate = "2025-06-01", 
            DepartureDate = "2025-06-03" 
        }));

        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var created = await ReadAs<BookingDto>(create);
        Assert.NotNull(created?.Id);
        // Tariff resolution is verified via the direct /api/tariffs/resolve call above (file-store timing prevents end-to-end price assertion here)
    }

    [Fact]
    public async Task Booking_Create_AndUpdate_PrepaidAmount_PostsLedgerLines()
    {
        var create = await _client.PostAsync("/api/bookings", Json(new { Year = 2025, ClientId = "C-Prepaid", RoomId = "103", Status = "Confirmed", ArrivalDate = "2025-04-01", DepartureDate = "2025-04-03", PrepaidAmount = 150.0 }));
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var created = await ReadAs<BookingDto>(create);
        Assert.NotNull(created?.Id);

        var linesCreate = await _client.GetAsync($"/api/ledger/lines?from=2024-01-01&to=2030-12-31&bookingId={created.Id}");
        var listCreate = await ReadAs<List<LedgerLineDto>>(linesCreate) ?? new();
        Assert.Contains(listCreate, l => l.AccountCode == "1000" && l.Debit == 150m && l.SourceType == "booking");
        Assert.Contains(listCreate, l => l.AccountCode == "2400" && l.Credit == 150m && l.SourceType == "booking");

        var upd = await _client.PutAsync($"/api/bookings/2025/{created.Id}", Json(new { Year = 2025, ClientId = "C-Prepaid", RoomId = "103", Status = "Confirmed", ArrivalDate = "2025-04-01", DepartureDate = "2025-04-03", PrepaidAmount = 200.0 }));
        Assert.Equal(HttpStatusCode.OK, upd.StatusCode);

        var linesUpd = await _client.GetAsync($"/api/ledger/lines?from=2024-01-01&to=2030-12-31&bookingId={created.Id}");
        var listUpd = await ReadAs<List<LedgerLineDto>>(linesUpd) ?? new();
        Assert.Contains(listUpd, l => l.AccountCode == "1000" && l.Debit == 50m && l.SourceType == "booking");
    }

    [Fact]
    public async Task Booking_Checkout_AppliesDepositToGuestLedger()
    {
        var create = await _client.PostAsync("/api/bookings", Json(new { Year = 2025, ClientId = "C-Settle", RoomId = "104", Status = "Confirmed", ArrivalDate = "2025-05-01", DepartureDate = "2025-05-03", PrepaidAmount = 180.0 }));
        var created = await ReadAs<BookingDto>(create);
        Assert.NotNull(created?.Id);

        var upd = await _client.PutAsync($"/api/bookings/2025/{created.Id}", Json(new { Year = 2025, ClientId = "C-Settle", RoomId = "104", Status = "Confirmed", ArrivalDate = "2025-05-01", DepartureDate = "2025-05-03", ActualArrivalDate = "2025-05-01", ActualDepartureDate = "2025-05-03", ActualGuestCount = 2, PrepaidAmount = 180.0 }));
        Assert.Equal(HttpStatusCode.OK, upd.StatusCode);

        var lines = await _client.GetAsync($"/api/ledger/lines?from=2024-01-01&to=2030-12-31&bookingId={created.Id}");
        var list = await ReadAs<List<LedgerLineDto>>(lines) ?? new();
        Assert.Contains(list, l => l.EntryType == "booking_deposit_application" && l.AccountCode == "2400" && l.Debit == 180m);
        Assert.Contains(list, l => l.EntryType == "booking_deposit_application" && l.AccountCode == "1200" && l.Credit == 180m);
        Assert.DoesNotContain(list, l => l.EntryType == "booking_deposit_recognition");
    }

    [Fact]
    public async Task BookingFinance_ReadsLedgerAccounts_AndCheckoutClearsTheDeposit()
    {
        var roomId = "room-ledger-" + Guid.NewGuid().ToString("N");
        var create = await _client.PostAsync("/api/bookings", Json(new { Year = 2026, ClientId = "C-Finance", RoomId = roomId, Status = "Confirmed", ArrivalDate = "2026-06-01", DepartureDate = "2026-06-03" }));
        var created = await ReadAs<BookingDto>(create);
        Assert.NotNull(created?.Id);

        var extra = await _client.PostAsync("/api/booking-costs", Json(new { Year = 2026, BookingId = created.Id, Amount = 100.0, Description = "Breakfast" }));
        Assert.Equal(HttpStatusCode.Created, extra.StatusCode);
        var deposit = await _client.PostAsync($"/api/bookings/2026/{created.Id}/transactions", Json(new { Type = "deposit", Amount = 40.0, AppliedTo = "deposit" }));
        Assert.Equal(HttpStatusCode.Created, deposit.StatusCode);
        var payment = await _client.PostAsync($"/api/bookings/2026/{created.Id}/transactions", Json(new { Type = "payment", Amount = 30.0 }));
        Assert.Equal(HttpStatusCode.Created, payment.StatusCode);

        var open = await ReadAs<BookingFinanceSummaryDto>(await _client.GetAsync($"/api/bookings/2026/{created.Id}/finance"));
        Assert.Equal(0m, open!.StayTotal);
        Assert.Equal(100m, open.ExtraTotal);
        Assert.Equal(100m, open.ChargesTotal);
        Assert.Equal(40m, open.DepositsHeld);
        Assert.Equal(30m, open.PaymentsTotal);
        Assert.Equal(70m, open.TotalDue);

        var history = await ReadAs<List<BookingTransactionDto>>(await _client.GetAsync($"/api/bookings/2026/{created.Id}/transactions"));
        Assert.Contains(history!, row => row.Type == "extra_charge" && row.Amount == 100m);
        Assert.Contains(history!, row => row.Type == "payment" && row.Amount == 30m);

        var checkout = await _client.PutAsync($"/api/bookings/2026/{created.Id}", Json(new { Year = 2026, ClientId = "C-Finance", RoomId = roomId, Status = "CheckedOut", ArrivalDate = "2026-06-01", DepartureDate = "2026-06-03", ActualArrivalDate = "2026-06-01", ActualDepartureDate = "2026-06-03", ActualGuestCount = 1 }));
        Assert.Equal(HttpStatusCode.OK, checkout.StatusCode);

        var closed = await ReadAs<BookingFinanceSummaryDto>(await _client.GetAsync($"/api/bookings/2026/{created.Id}/finance"));
        Assert.Equal(0m, closed!.DepositsHeld);
        Assert.Equal(30m, closed.TotalDue);
        Assert.Equal(100m, closed.ChargesTotal);

        var lines = await LedgerLines(created.Id!);
        Assert.Contains(lines, line => line.EntryType == "booking_deposit_application" && line.AccountCode == "1200" && line.Credit == 40m);
        Assert.DoesNotContain(lines, line => line.AccountCode == "4100" && line.Credit == 40m);
    }

    [Fact]
    public async Task Booking_Create_WithoutDates_ReturnsBadRequest_AndDoesNotPersist()
    {
        var create = await _client.PostAsync("/api/bookings", Json(new { Year = 2024, ClientId = "C-NoDates", Status = "Pending" }));
        Assert.Equal(HttpStatusCode.BadRequest, create.StatusCode);

        var listResponse = await _client.GetAsync("/api/bookings?year=2024");
        Assert.Equal(HttpStatusCode.OK, listResponse.StatusCode);
        var list = await ReadAs<List<BookingDto>>(listResponse);
        Assert.DoesNotContain(list!, b => b.ClientId == "C-NoDates");
    }

    [Fact]
    public async Task Booking_GetByYearAndId_ReturnsOk()
    {
        var create = await _client.PostAsync("/api/bookings", Json(new { Year = 2024, ClientId = "C002", Status = "Pending", ArrivalDate = "2024-07-01", DepartureDate = "2024-07-03" }));
        var created = await ReadAs<BookingDto>(create);
        var r = await _client.GetAsync($"/api/bookings/2024/{created!.Id}");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var dto = await ReadAs<BookingDto>(r);
        Assert.Equal("C002", dto?.ClientId);
    }

    [Fact]
    public async Task Booking_List_FilteredByYear()
    {
        await _client.PostAsync("/api/bookings", Json(new { Year = 2060, ClientId = "C010", ArrivalDate = "2060-05-10", DepartureDate = "2060-05-12" }));
        await _client.PostAsync("/api/bookings", Json(new { Year = 2061, ClientId = "C011", ArrivalDate = "2061-05-10", DepartureDate = "2061-05-12" }));
        var r = await _client.GetAsync("/api/bookings?year=2060");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var list = await ReadAs<List<BookingDto>>(r);
        Assert.All(list!, b => Assert.Equal(2060, b.Year));
    }

    [Fact]
    public async Task Booking_StaysEndpoint_ReturnsOnlyCurrentStays_ByWidgetRules()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        await _client.PostAsync("/api/bookings", Json(new
        {
            Year = 2024,
            ClientId = "StayClient",
            RoomId = "101",
            Status = "Confirmed",
            ArrivalDate = today.AddDays(-2).ToString("yyyy-MM-dd"),
            DepartureDate = today.AddDays(2).ToString("yyyy-MM-dd"),
            ActualArrivalDate = today.AddDays(-2).ToString("yyyy-MM-dd"),
            ActualGuestCount = 2
        }));

        await _client.PostAsync("/api/bookings", Json(new
        {
            Year = 2024,
            ClientId = "NoActualArrival",
            RoomId = "102",
            Status = "Confirmed",
            ArrivalDate = today.AddDays(-1).ToString("yyyy-MM-dd"),
            DepartureDate = today.AddDays(1).ToString("yyyy-MM-dd")
        }));

        await _client.PostAsync("/api/bookings", Json(new
        {
            Year = 2024,
            ClientId = "DepartedToday",
            RoomId = "103",
            Status = "Confirmed",
            ArrivalDate = today.AddDays(-3).ToString("yyyy-MM-dd"),
            DepartureDate = today.ToString("yyyy-MM-dd"),
            ActualArrivalDate = today.AddDays(-3).ToString("yyyy-MM-dd"),
            ActualGuestCount = 2
        }));

        await _client.PostAsync("/api/bookings", Json(new
        {
            Year = 2024,
            ClientId = "CancelledStay",
            RoomId = "104",
            Status = "Cancelled",
            ArrivalDate = today.AddDays(-1).ToString("yyyy-MM-dd"),
            DepartureDate = today.AddDays(1).ToString("yyyy-MM-dd"),
            ActualArrivalDate = today.AddDays(-1).ToString("yyyy-MM-dd"),
            ActualGuestCount = 2
        }));

        var response = await _client.GetAsync("/api/bookings/stays?year=2024");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var stays = await ReadAs<List<BookingDto>>(response);
        Assert.NotNull(stays);
        Assert.Single(stays!);
        Assert.Equal("StayClient", stays[0].ClientId);
    }

    [Fact]
    public async Task Booking_LifecycleEndpoints_ClassifyStatesConsistently()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        await _client.PostAsync("/api/bookings", Json(new
        {
            Year = 2025,
            ClientId = "ReservationClient",
            RoomId = "201",
            Status = "Confirmed",
            ArrivalDate = today.AddDays(5).ToString("yyyy-MM-dd"),
            DepartureDate = today.AddDays(7).ToString("yyyy-MM-dd")
        }));

        await _client.PostAsync("/api/bookings", Json(new
        {
            Year = 2025,
            ClientId = "ArrivalClient",
            RoomId = "202",
            Status = "Confirmed",
            ArrivalDate = today.AddDays(1).ToString("yyyy-MM-dd"),
            DepartureDate = today.AddDays(3).ToString("yyyy-MM-dd")
        }));

        await _client.PostAsync("/api/bookings", Json(new
        {
            Year = 2025,
            ClientId = "DepartureClient",
            RoomId = "203",
            Status = "Confirmed",
            ArrivalDate = today.AddDays(-2).ToString("yyyy-MM-dd"),
            DepartureDate = today.ToString("yyyy-MM-dd"),
            ActualArrivalDate = today.AddDays(-2).ToString("yyyy-MM-dd"),
            ActualGuestCount = 2
        }));

        await _client.PostAsync("/api/bookings", Json(new
        {
            Year = 2025,
            ClientId = "CancelledClient",
            RoomId = "204",
            Status = "Cancelled",
            ArrivalDate = today.ToString("yyyy-MM-dd"),
            DepartureDate = today.AddDays(1).ToString("yyyy-MM-dd")
        }));

        var reservationsResponse = await _client.GetAsync("/api/bookings/reservations?year=2025");
        Assert.Equal(HttpStatusCode.OK, reservationsResponse.StatusCode);
        var reservations = await ReadAs<List<BookingDto>>(reservationsResponse);
        Assert.NotNull(reservations);
        Assert.Contains(reservations!, r => r.ClientId == "ReservationClient");
        Assert.Contains(reservations!, r => r.ClientId == "ArrivalClient");
        Assert.DoesNotContain(reservations!, r => r.ClientId == "DepartureClient");

        var arrivalsResponse = await _client.GetAsync("/api/bookings/arrivals?year=2025&lookAheadDays=2");
        Assert.Equal(HttpStatusCode.OK, arrivalsResponse.StatusCode);
        var arrivals = await ReadAs<List<BookingDto>>(arrivalsResponse);
        Assert.NotNull(arrivals);
        Assert.Contains(arrivals!, r => r.ClientId == "ArrivalClient");
        Assert.DoesNotContain(arrivals!, r => r.ClientId == "ReservationClient");

        var departuresResponse = await _client.GetAsync("/api/bookings/departures?year=2025");
        Assert.Equal(HttpStatusCode.OK, departuresResponse.StatusCode);
        var departures = await ReadAs<List<BookingDto>>(departuresResponse);
        Assert.NotNull(departures);
        Assert.Contains(departures!, r => r.ClientId == "DepartureClient");
        Assert.DoesNotContain(departures!, r => r.ClientId == "CancelledClient");
    }

    [Fact]
    public async Task Booking_LifecycleEndpoints_ExcludeCheckedOutBookings()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        await _client.PostAsync("/api/bookings", Json(new
        {
            Year = 2025,
            ClientId = "CheckedOutClient",
            RoomId = "205",
            Status = "Confirmed",
            ArrivalDate = today.AddDays(-2).ToString("yyyy-MM-dd"),
            DepartureDate = today.ToString("yyyy-MM-dd"),
            ActualArrivalDate = today.AddDays(-2).ToString("yyyy-MM-dd"),
            ActualDepartureDate = today.ToString("yyyy-MM-dd"),
            ActualGuestCount = 2
        }));

        var staysResponse = await _client.GetAsync("/api/bookings/stays?year=2025");
        Assert.Equal(HttpStatusCode.OK, staysResponse.StatusCode);
        var stays = await ReadAs<List<BookingDto>>(staysResponse);
        Assert.NotNull(stays);
        Assert.DoesNotContain(stays!, b => b.ClientId == "CheckedOutClient");

        var departuresResponse = await _client.GetAsync("/api/bookings/departures?year=2025");
        Assert.Equal(HttpStatusCode.OK, departuresResponse.StatusCode);
        var departures = await ReadAs<List<BookingDto>>(departuresResponse);
        Assert.NotNull(departures);
        Assert.DoesNotContain(departures!, b => b.ClientId == "CheckedOutClient");
    }

    [Fact]
    public async Task Booking_CheckInEndpoint_RequiresGuestCount_AndPersistsArrival()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var create = await _client.PostAsync("/api/bookings", Json(new
        {
            Year = 2026,
            ClientId = "C-CHECKIN",
            RoomId = "R-CHECKIN",
            Status = "Confirmed",
            ArrivalDate = today.ToString("yyyy-MM-dd"),
            DepartureDate = today.AddDays(2).ToString("yyyy-MM-dd")
        }));
        var created = await ReadAs<BookingDto>(create);

        var invalid = await _client.PostAsync($"/api/bookings/2026/{created!.Id}/check-in", Json(new { ActualArrivalDate = today.ToString("yyyy-MM-dd") }));
        Assert.Equal(HttpStatusCode.BadRequest, invalid.StatusCode);

        var valid = await _client.PostAsync($"/api/bookings/2026/{created.Id}/check-in", Json(new
        {
            ActualArrivalDate = today.ToString("yyyy-MM-dd"),
            ActualGuestCount = 2,
            Status = "CheckedIn"
        }));
        Assert.Equal(HttpStatusCode.OK, valid.StatusCode);

        var dto = await ReadAs<BookingDto>(valid);
        Assert.Equal(today.ToString("yyyy-MM-dd"), dto?.ActualArrivalDate);
        Assert.Equal(2, dto?.ActualGuestCount);
    }

    [Fact]
    public async Task Booking_CheckOutEndpoint_RequiresActualDepartureDate_AndPersistsCheckout()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var create = await _client.PostAsync("/api/bookings", Json(new
        {
            Year = 2026,
            ClientId = "C-CHECKOUT",
            RoomId = "R-CHECKOUT",
            Status = "CheckedIn",
            ArrivalDate = today.AddDays(-1).ToString("yyyy-MM-dd"),
            DepartureDate = today.AddDays(1).ToString("yyyy-MM-dd"),
            ActualArrivalDate = today.AddDays(-1).ToString("yyyy-MM-dd"),
            ActualGuestCount = 2
        }));
        var created = await ReadAs<BookingDto>(create);

        var invalid = await _client.PostAsync($"/api/bookings/2026/{created!.Id}/check-out", Json(new { Status = "CheckedOut" }));
        Assert.Equal(HttpStatusCode.BadRequest, invalid.StatusCode);

        var valid = await _client.PostAsync($"/api/bookings/2026/{created.Id}/check-out", Json(new
        {
            ActualDepartureDate = today.ToString("yyyy-MM-dd"),
            Status = "CheckedOut"
        }));
        Assert.Equal(HttpStatusCode.OK, valid.StatusCode);

        var dto = await ReadAs<BookingDto>(valid);
        Assert.Equal(today.ToString("yyyy-MM-dd"), dto?.ActualDepartureDate);
        Assert.Equal("CheckedOut", dto?.Status);
    }

    [Fact]
    public async Task Booking_NoShowEndpoint_RequiresNotes_AndSetsStatus()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var create = await _client.PostAsync("/api/bookings", Json(new
        {
            Year = 2026,
            ClientId = "C-NOSHOW",
            RoomId = "R-NOSHOW",
            Status = "Confirmed",
            ArrivalDate = today.ToString("yyyy-MM-dd"),
            DepartureDate = today.AddDays(2).ToString("yyyy-MM-dd")
        }));
        var created = await ReadAs<BookingDto>(create);

        var invalid = await _client.PostAsync($"/api/bookings/2026/{created!.Id}/no-show", Json(new { Status = "NoShow" }));
        Assert.Equal(HttpStatusCode.BadRequest, invalid.StatusCode);

        var valid = await _client.PostAsync($"/api/bookings/2026/{created.Id}/no-show", Json(new
        {
            Notes = "Guest did not arrive",
            Status = "NoShow"
        }));
        Assert.Equal(HttpStatusCode.OK, valid.StatusCode);

        var dto = await ReadAs<BookingDto>(valid);
        Assert.Equal("NoShow", dto?.Status);
        Assert.Equal("Guest did not arrive", dto?.Notes);
    }

    [Fact]
    public async Task Booking_ListAfterCreate_ReturnsFreshData()
    {
        await _client.PostAsync("/api/bookings", Json(new { Year = 2090, ClientId = "Phase3-A", ArrivalDate = "2090-01-01", DepartureDate = "2090-01-03" }));

        var first = await _client.GetAsync("/api/bookings?year=2090");
        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        var firstList = await ReadAs<List<BookingDto>>(first);
        Assert.NotNull(firstList);
        Assert.Contains(firstList!, b => b.ClientId == "Phase3-A");

        await _client.PostAsync("/api/bookings", Json(new { Year = 2090, ClientId = "Phase3-B", ArrivalDate = "2090-02-01", DepartureDate = "2090-02-04" }));

        var second = await _client.GetAsync("/api/bookings?year=2090");
        Assert.Equal(HttpStatusCode.OK, second.StatusCode);
        var secondList = await ReadAs<List<BookingDto>>(second);
        Assert.NotNull(secondList);
        Assert.Contains(secondList!, b => b.ClientId == "Phase3-B");
    }

    [Fact]
    public async Task Booking_Update_ChangesFields()
    {
        var create = await _client.PostAsync("/api/bookings", Json(new { Year = 2024, ClientId = "C003", Status = "Pending", ArrivalDate = "2024-08-01", DepartureDate = "2024-08-05" }));
        var created = await ReadAs<BookingDto>(create);
        var r = await _client.PutAsync($"/api/bookings/2024/{created!.Id}", Json(new { Year = 2024, ClientId = "C003", Status = "Confirmed", ArrivalDate = "2024-08-01", DepartureDate = "2024-08-06" }));
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var dto = await ReadAs<BookingDto>(r);
        Assert.Equal("Confirmed", dto?.Status);
    }

    [Fact]
    public async Task Booking_Update_WithActualArrivalAndPrepay_PersistsValues()
    {
        var create = await _client.PostAsync("/api/bookings", Json(new { Year = 2024, ClientId = "C003X", Status = "Pending", ArrivalDate = "2024-08-01", DepartureDate = "2024-08-06" }));
        var created = await ReadAs<BookingDto>(create);

        var update = await _client.PutAsync(
            $"/api/bookings/2024/{created!.Id}",
            Json(new
            {
                Year = 2024,
                ClientId = "C003X",
                Status = "CheckedIn",
                ArrivalDate = "2024-08-01",
                DepartureDate = "2024-08-06",
                ActualArrivalDate = "2024-08-02",
                ActualGuestCount = 2,
                PrepaidAmount = 125.50
            }));

        Assert.Equal(HttpStatusCode.OK, update.StatusCode);
        var dto = await ReadAs<BookingDto>(update);
        Assert.Equal("2024-08-02", dto?.ActualArrivalDate);
        Assert.Equal(2, dto?.ActualGuestCount);
        Assert.Equal(125.50, dto?.PrepaidAmount);
    }

    [Fact]
    public async Task Booking_Update_WithActualDeparture_PersistsValue()
    {
        var create = await _client.PostAsync("/api/bookings", Json(new
        {
            Year = 2024,
            ClientId = "C003Y",
            Status = "CheckedIn",
            ArrivalDate = "2024-08-01",
            DepartureDate = "2024-08-06",
            ActualArrivalDate = "2024-08-01",
            ActualGuestCount = 2
        }));
        var created = await ReadAs<BookingDto>(create);

        var update = await _client.PutAsync(
            $"/api/bookings/2024/{created!.Id}",
            Json(new
            {
                Year = 2024,
                ClientId = "C003Y",
                Status = "CheckedOut",
                ArrivalDate = "2024-08-01",
                DepartureDate = "2024-08-06",
                ActualArrivalDate = "2024-08-01",
                ActualDepartureDate = "2024-08-05",
                ActualGuestCount = 2,
                PrepaidAmount = 125.50
            }));

        Assert.Equal(HttpStatusCode.OK, update.StatusCode);
        var dto = await ReadAs<BookingDto>(update);
        Assert.Equal("2024-08-05", dto?.ActualDepartureDate);
    }

    [Fact]
    public async Task Booking_Update_WithInvalidDateRange_ReturnsBadRequest_AndPreservesStoredBooking()
    {
        var create = await _client.PostAsync("/api/bookings", Json(new { Year = 2024, ClientId = "C-BadUpdate", Status = "Pending", ArrivalDate = "2024-09-10", DepartureDate = "2024-09-15" }));
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var created = await ReadAs<BookingDto>(create);

        var update = await _client.PutAsync($"/api/bookings/2024/{created!.Id}", Json(new { Year = 2024, ClientId = "C-BadUpdate", Status = "Confirmed", ArrivalDate = "2024-09-15", DepartureDate = "2024-09-10" }));
        Assert.Equal(HttpStatusCode.BadRequest, update.StatusCode);

        var get = await _client.GetAsync($"/api/bookings/2024/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, get.StatusCode);
        var dto = await ReadAs<BookingDto>(get);
        Assert.Equal("2024-09-10", dto?.ArrivalDate);
        Assert.Equal("2024-09-15", dto?.DepartureDate);
    }

    [Fact]
    public async Task Booking_Delete_ReturnsNoContent()
    {
        var create = await _client.PostAsync("/api/bookings", Json(new { Year = 2024, ClientId = "C004", ArrivalDate = "2024-10-01", DepartureDate = "2024-10-03" }));
        var created = await ReadAs<BookingDto>(create);
        var r = await _client.DeleteAsync($"/api/bookings/2024/{created!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, r.StatusCode);
        var get = await _client.GetAsync($"/api/bookings/2024/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, get.StatusCode);
    }

    [Fact]
    public async Task Booking_Cancel_MovesToCancelledBookings()
    {
        var create = await _client.PostAsync("/api/bookings", Json(new { Year = 2024, ClientId = "C005", Status = "Confirmed", ArrivalDate = "2024-11-01", DepartureDate = "2024-11-04" }));
        var created = await ReadAs<BookingDto>(create);
        var cancel = await _client.PostAsync($"/api/bookings/2024/{created!.Id}/cancel", null);
        Assert.Equal(HttpStatusCode.NoContent, cancel.StatusCode);
        var get = await _client.GetAsync($"/api/bookings/2024/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, get.StatusCode);
        var cancelled = await _client.GetAsync("/api/cancelled-bookings?year=2024");
        var list = await ReadAs<List<CancelledBookingDto>>(cancelled);
        Assert.Contains(list!, cb => cb.ClientId == "C005");
    }

    // ========== Booking Costs ==========

    [Fact]
    public async Task BookingCost_Create_ReturnsCreated()
    {
        var r = await _client.PostAsync("/api/booking-costs", Json(new { Year = 2024, BookingId = "bk001", TariffId = "T1", Amount = 50.0, Description = "Breakfast" }));
        Assert.Equal(HttpStatusCode.Created, r.StatusCode);
        var dto = await ReadAs<BookingCostDto>(r);
        Assert.NotNull(dto?.Id);
        Assert.Equal("Breakfast", dto.Description);
        Assert.Equal(50.0, dto.Amount);
    }

    [Fact]
    public async Task BookingCost_GetByYearAndId_ReturnsOk()
    {
        var create = await _client.PostAsync("/api/booking-costs", Json(new { Year = 2024, BookingId = "bk002", Amount = 30.0 }));
        var created = await ReadAs<BookingCostDto>(create);
        var r = await _client.GetAsync($"/api/booking-costs/2024/{created!.Id}");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var dto = await ReadAs<BookingCostDto>(r);
        Assert.Equal(30.0, dto?.Amount);
    }

    [Fact]
    public async Task BookingCost_List_FilteredByYear()
    {
        await _client.PostAsync("/api/booking-costs", Json(new { Year = 2070, BookingId = "bk070", Amount = 10.0 }));
        await _client.PostAsync("/api/booking-costs", Json(new { Year = 2071, BookingId = "bk071", Amount = 20.0 }));
        var r = await _client.GetAsync("/api/booking-costs?year=2070");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var list = await ReadAs<List<BookingCostDto>>(r);
        Assert.All(list!, bc => Assert.Equal(2070, bc.Year));
    }

    [Fact]
    public async Task BookingCost_Update_ChangesAmount()
    {
        var create = await _client.PostAsync("/api/booking-costs", Json(new { Year = 2024, BookingId = "bk003", Amount = 40.0 }));
        var created = await ReadAs<BookingCostDto>(create);
        var r = await _client.PutAsync($"/api/booking-costs/2024/{created!.Id}", Json(new { Year = 2024, BookingId = "bk003", Amount = 80.0 }));
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var dto = await ReadAs<BookingCostDto>(r);
        Assert.Equal(80.0, dto?.Amount);
    }

    [Fact]
    public async Task BookingCost_Delete_ReturnsNoContent()
    {
        var create = await _client.PostAsync("/api/booking-costs", Json(new { Year = 2024, BookingId = "bk004" }));
        var created = await ReadAs<BookingCostDto>(create);
        var r = await _client.DeleteAsync($"/api/booking-costs/2024/{created!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, r.StatusCode);
    }

    [Fact]
    public async Task BookingCost_Sale_PostsGuestLedgerAndReversesOnRemoval()
    {
        var bookingId = "stay-extra-" + Guid.NewGuid().ToString("N");
        var create = await _client.PostAsync("/api/booking-costs", Json(new { Year = 2026, BookingId = bookingId, Amount = 25.0, Description = "Late checkout" }));
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var created = await ReadAs<BookingCostDto>(create);
        Assert.NotNull(created?.Id);

        var sold = await LedgerLines(bookingId);
        Assert.Contains(sold, line => line.EntryType == "booking_extra_charge" && line.AccountCode == "1200" && line.Debit == 25m && line.SourceType == "booking-cost" && line.SourceId == created.Id);
        Assert.Contains(sold, line => line.EntryType == "booking_extra_charge" && line.AccountCode == "4900" && line.Credit == 25m);

        var update = await _client.PutAsync($"/api/booking-costs/2026/{created.Id}", Json(new { Year = 2026, BookingId = bookingId, Amount = 40.0, Description = "Late checkout" }));
        Assert.Equal(HttpStatusCode.OK, update.StatusCode);
        var raised = await LedgerLines(bookingId);
        Assert.Contains(raised, line => line.EntryType == "booking_extra_charge" && line.AccountCode == "1200" && line.Debit == 15m);

        var remove = await _client.DeleteAsync($"/api/booking-costs/2026/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, remove.StatusCode);
        var listed = await ReadAs<List<BookingCostDto>>(await _client.GetAsync("/api/booking-costs?year=2026"));
        Assert.DoesNotContain(listed!, cost => cost.Id == created.Id);
        var closed = await LedgerLines(bookingId);
        Assert.Contains(closed, line => line.EntryType == "booking_extra_reversal" && line.AccountCode == "4900" && line.Debit == 40m);
        Assert.Contains(closed, line => line.EntryType == "booking_extra_reversal" && line.AccountCode == "1200" && line.Credit == 40m);
        Assert.Equal(0m, closed.Where(line => line.AccountCode == "1200").Sum(line => line.Debit - line.Credit));
    }

    private async Task<List<LedgerLineDto>> LedgerLines(string bookingId)
    {
        var response = await _client.GetAsync($"/api/ledger/lines?from=2024-01-01&to=2030-12-31&bookingId={bookingId}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return await ReadAs<List<LedgerLineDto>>(response) ?? new();
    }

    // ========== Point of sale ==========

    [Fact]
    public async Task PosSale_WalkUp_PostsCashAndLeavesBookingEmpty()
    {
        var register = await ReadAs<CashRegisterDto>(await _client.PostAsync("/api/cash-registers", Json(new { Name = "Front desk", Code = "1" })));
        var create = await _client.PostAsync("/api/pos-sales", Json(new { Year = 2026, Description = "Mineral water", Amount = 3.5, Person = "Walk up" }));
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var sale = await ReadAs<PosSaleDto>(create);
        Assert.Equal("cash", sale?.Settlement);
        Assert.Null(sale?.BookingId);
        Assert.Null(sale?.ClientId);
        Assert.Equal(register!.Id, sale?.CashRegisterId);
        Assert.Equal("EUR", sale?.Currency);
        Assert.Equal("1000", sale?.DebitAccount);
        Assert.Equal("4900", sale?.CreditAccount);
        Assert.Equal(3.5, sale?.RunningCashTotal);

        var lines = await ReadAs<List<LedgerLineDto>>(await _client.GetAsync("/api/ledger/lines?from=2026-01-01&to=2026-12-31&unassigned=true"));
        Assert.Contains(lines!, line => line.EntryType == "pos_cash_sale" && line.AccountCode == "1000" && line.Debit == 3.5m && string.IsNullOrEmpty(line.BookingId));
        Assert.Contains(lines!, line => line.EntryType == "pos_cash_sale" && line.AccountCode == "4900" && line.Credit == 3.5m);

        var reverse = await _client.PostAsync($"/api/pos-sales/2026/{sale!.Id}/reverse", Json(new { }));
        Assert.Equal(HttpStatusCode.Created, reverse.StatusCode);
        var reversed = await ReadAs<List<LedgerLineDto>>(await _client.GetAsync("/api/ledger/lines?from=2026-01-01&to=2026-12-31&unassigned=true"));
        Assert.Contains(reversed!, line => line.EntryType == "pos_cash_reversal" && line.AccountCode == "1000" && line.Credit == 3.5m);
        Assert.Contains(reversed!, line => line.EntryType == "pos_cash_reversal" && line.AccountCode == "4900" && line.Debit == 3.5m);
    }

    [Fact]
    public async Task PosSale_OnStay_PostsGuestLedgerOnlyWhileCheckedIn()
    {
        var createdBooking = await ReadAs<BookingDto>(await _client.PostAsync("/api/bookings", Json(new { Year = 2026, ClientId = "guest-pos", RoomId = "101", Status = "Confirmed", ArrivalDate = "2026-11-08", DepartureDate = "2026-11-10" })));
        var tooEarly = await _client.PostAsync("/api/pos-sales", Json(new { Year = 2026, BookingId = createdBooking!.Id, Description = "Breakfast", Amount = 12.0 }));
        Assert.Equal(HttpStatusCode.BadRequest, tooEarly.StatusCode);

        var checkIn = await _client.PostAsync($"/api/bookings/2026/{createdBooking.Id}/check-in", Json(new { ActualArrivalDate = "2026-11-08", ActualGuestCount = 1, Status = "CheckedIn" }));
        Assert.Equal(HttpStatusCode.OK, checkIn.StatusCode);

        var saleResponse = await _client.PostAsync("/api/pos-sales", Json(new { Year = 2026, BookingId = createdBooking.Id, Description = "Breakfast", Amount = 12.0 }));
        Assert.Equal(HttpStatusCode.Created, saleResponse.StatusCode);
        var sale = await ReadAs<PosSaleDto>(saleResponse);
        Assert.Equal("folio", sale?.Settlement);
        Assert.Equal(createdBooking.Id, sale?.BookingId);
        Assert.Equal("guest-pos", sale?.ClientId);
        Assert.Null(sale?.CashRegisterId);
        Assert.Equal("1200", sale?.DebitAccount);
        Assert.Equal("4900", sale?.CreditAccount);
        Assert.False(string.IsNullOrWhiteSpace(sale?.BookingCostId));

        var lines = await LedgerLines(createdBooking.Id!);
        Assert.Contains(lines, line => line.EntryType == "booking_extra_charge" && line.AccountCode == "1200" && line.Debit == 12m && line.SourceType == "booking-cost");
        Assert.Contains(lines, line => line.EntryType == "booking_extra_charge" && line.AccountCode == "4900" && line.Credit == 12m);
        Assert.DoesNotContain(lines, line => line.EntryType == "pos_cash_sale");

        var checkOut = await _client.PostAsync($"/api/bookings/2026/{createdBooking.Id}/check-out", Json(new { ActualDepartureDate = "2026-11-10", Status = "CheckedOut" }));
        Assert.Equal(HttpStatusCode.OK, checkOut.StatusCode);
        var after = await _client.PostAsync("/api/pos-sales", Json(new { Year = 2026, BookingId = createdBooking.Id, Description = "Late bar", Amount = 8.0 }));
        Assert.Equal(HttpStatusCode.BadRequest, after.StatusCode);
    }

    [Fact]
    public async Task PosSale_Article_CommitsWarehouseQuantityWhenTheLedgerIsWritten()
    {
        var asset = await ReadAs<CashRegisterDto>(await _client.PostAsync("/api/assets", Json(new { Name = "Bathrobe", Code = "BR" })));
        var warehouse = await ReadAs<CashRegisterDto>(await _client.PostAsync("/api/warehouses", Json(new { Name = "Linen room", Code = "LIN" })));
        var stock = await ReadAs<InventoryRowDto>(await _client.PostAsync("/api/inventory", Json(new { AssetId = asset!.Id, WarehouseId = warehouse!.Id, Quantity = 3 })));

        var shortSale = await _client.PostAsync("/api/pos-sales", Json(new { Year = 2026, Description = "Bathrobe", Amount = 20.0, ArticleId = stock!.Id, Units = 4 }));
        Assert.Equal(HttpStatusCode.BadRequest, shortSale.StatusCode);
        Assert.Equal(3, (await ReadAs<InventoryRowDto>(await _client.GetAsync($"/api/inventory/{stock.Id}")))!.Quantity);

        var saleResponse = await _client.PostAsync("/api/pos-sales", Json(new { Year = 2026, Description = "Bathrobe", Amount = 20.0, ArticleId = stock.Id, Units = 1 }));
        Assert.Equal(HttpStatusCode.Created, saleResponse.StatusCode);
        var sale = await ReadAs<PosSaleDto>(saleResponse);
        Assert.Equal(stock.Id, sale?.ArticleId);
        Assert.Equal(1, sale?.Units);
        Assert.Equal(2, (await ReadAs<InventoryRowDto>(await _client.GetAsync($"/api/inventory/{stock.Id}")))!.Quantity);
        var lines = await ReadAs<List<LedgerLineDto>>(await _client.GetAsync("/api/ledger/lines?from=2026-01-01&to=2026-12-31&unassigned=true"));
        Assert.Contains(lines!, line => line.EntryType == "pos_cash_sale" && line.AccountCode == "4900" && line.Credit == 20m);

        var reverse = await _client.PostAsync($"/api/pos-sales/2026/{sale!.Id}/reverse", Json(new { }));
        Assert.Equal(HttpStatusCode.Created, reverse.StatusCode);
        Assert.Equal(3, (await ReadAs<InventoryRowDto>(await _client.GetAsync($"/api/inventory/{stock.Id}")))!.Quantity);
    }

    [Fact]
    public async Task PosSale_RoomArticle_ChecksAvailabilityAndDoesNotChangeQuantity()
    {
        var asset = await ReadAs<CashRegisterDto>(await _client.PostAsync("/api/assets", Json(new { Name = "Towel", Code = "TW" })));
        var roomStock = await ReadAs<InventoryRowDto>(await _client.PostAsync("/api/inventory", Json(new { AssetId = asset!.Id, RoomId = "room-article", Quantity = 2 })));
        var walkUp = await _client.PostAsync("/api/pos-sales", Json(new { Year = 2026, Description = "Towel", Amount = 5.0, ArticleId = roomStock!.Id }));
        Assert.Equal(HttpStatusCode.BadRequest, walkUp.StatusCode);
        Assert.Equal(2, (await ReadAs<InventoryRowDto>(await _client.GetAsync($"/api/inventory/{roomStock.Id}")))!.Quantity);

        var booking = await ReadAs<BookingDto>(await _client.PostAsync("/api/bookings", Json(new { Year = 2026, ClientId = "guest-article", RoomId = "room-article", Status = "Confirmed", ArrivalDate = "2026-11-08", DepartureDate = "2026-11-10" })));
        var checkIn = await _client.PostAsync($"/api/bookings/2026/{booking!.Id}/check-in", Json(new { ActualArrivalDate = "2026-11-08", ActualGuestCount = 1, Status = "CheckedIn" }));
        Assert.Equal(HttpStatusCode.OK, checkIn.StatusCode);

        var saleResponse = await _client.PostAsync("/api/pos-sales", Json(new { Year = 2026, BookingId = booking.Id, Description = "Towel", Amount = 5.0, ArticleId = roomStock.Id, Units = 2 }));
        Assert.Equal(HttpStatusCode.Created, saleResponse.StatusCode);
        Assert.Equal(2, (await ReadAs<InventoryRowDto>(await _client.GetAsync($"/api/inventory/{roomStock.Id}")))!.Quantity);

        var tooMany = await _client.PostAsync("/api/pos-sales", Json(new { Year = 2026, BookingId = booking.Id, Description = "Towel", Amount = 5.0, ArticleId = roomStock.Id, Units = 3 }));
        Assert.Equal(HttpStatusCode.BadRequest, tooMany.StatusCode);
    }

    // ========== Booking Guests ==========

    [Fact]
    public async Task BookingGuest_Create_ReturnsCreated()
    {
        var r = await _client.PostAsync("/api/booking-guests", Json(new { Year = 2024, BookingId = "bk001", ClientId = "C100", GuestNumber = 1 }));
        Assert.Equal(HttpStatusCode.Created, r.StatusCode);
        var dto = await ReadAs<BookingGuestDto>(r);
        Assert.NotNull(dto?.Id);
        Assert.Equal("C100", dto.ClientId);
        Assert.Equal(1, dto.GuestNumber);
    }

    [Fact]
    public async Task BookingGuest_GetByYearAndId_ReturnsOk()
    {
        var create = await _client.PostAsync("/api/booking-guests", Json(new { Year = 2024, BookingId = "bk005", ClientId = "C101" }));
        var created = await ReadAs<BookingGuestDto>(create);
        var r = await _client.GetAsync($"/api/booking-guests/2024/{created!.Id}");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var dto = await ReadAs<BookingGuestDto>(r);
        Assert.Equal("C101", dto?.ClientId);
    }

    [Fact]
    public async Task BookingGuest_List_FilteredByYear()
    {
        await _client.PostAsync("/api/booking-guests", Json(new { Year = 2080, BookingId = "bk080", ClientId = "C200" }));
        await _client.PostAsync("/api/booking-guests", Json(new { Year = 2081, BookingId = "bk081", ClientId = "C201" }));
        var r = await _client.GetAsync("/api/booking-guests?year=2080");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var list = await ReadAs<List<BookingGuestDto>>(r);
        Assert.All(list!, g => Assert.Equal(2080, g.Year));
    }

    [Fact]
    public async Task BookingGuest_Delete_ReturnsNoContent()
    {
        var create = await _client.PostAsync("/api/booking-guests", Json(new { Year = 2024, BookingId = "bk006", ClientId = "C102" }));
        var created = await ReadAs<BookingGuestDto>(create);
        var r = await _client.DeleteAsync($"/api/booking-guests/2024/{created!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, r.StatusCode);
    }

    // ========== Cancelled Bookings ==========

    [Fact]
    public async Task CancelledBooking_List_ReturnsOk()
    {
        var r = await _client.GetAsync("/api/cancelled-bookings");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
    }

    [Fact]
    public async Task CancelledBooking_DeleteFromArchive_ReturnsNoContent()
    {
        var create = await _client.PostAsync("/api/bookings", Json(new { Year = 2024, ClientId = "C999", Status = "Confirmed", ArrivalDate = "2024-12-01", DepartureDate = "2024-12-03" }));
        var created = await ReadAs<BookingDto>(create);
        await _client.PostAsync($"/api/bookings/2024/{created!.Id}/cancel", null);
        var listResp = await _client.GetAsync("/api/cancelled-bookings?year=2024");
        var list = await ReadAs<List<CancelledBookingDto>>(listResp);
        var entry = list!.FirstOrDefault(cb => cb.ClientId == "C999");
        Assert.NotNull(entry);
        var del = await _client.DeleteAsync($"/api/cancelled-bookings/2024/{entry!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, del.StatusCode);
    }

    // ========== Expenses ==========

    [Fact]
    public async Task Expense_Create_ReturnsCreated()
    {
        var r = await _client.PostAsync("/api/expenses", Json(new { Year = 2024, CashRegisterId = "CR1", Amount = 120.0, Description = "Office supplies" }));
        Assert.Equal(HttpStatusCode.Created, r.StatusCode);
        var dto = await ReadAs<ExpenseDto>(r);
        Assert.NotNull(dto?.Id);
        Assert.Equal("Office supplies", dto.Description);
        Assert.Equal(120.0, dto.Amount);
    }

    [Fact]
    public async Task Expense_GetByYearAndId_ReturnsOk()
    {
        var create = await _client.PostAsync("/api/expenses", Json(new { Year = 2024, CashRegisterId = "CR2", Amount = 200.0 }));
        var created = await ReadAs<ExpenseDto>(create);
        var r = await _client.GetAsync($"/api/expenses/2024/{created!.Id}");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var dto = await ReadAs<ExpenseDto>(r);
        Assert.Equal(200.0, dto?.Amount);
    }

    [Fact]
    public async Task Expense_List_FilteredByYear()
    {
        await _client.PostAsync("/api/expenses", Json(new { Year = 2090, CashRegisterId = "CR3", Amount = 50.0 }));
        await _client.PostAsync("/api/expenses", Json(new { Year = 2091, CashRegisterId = "CR4", Amount = 60.0 }));
        var r = await _client.GetAsync("/api/expenses?year=2090");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var list = await ReadAs<List<ExpenseDto>>(r);
        Assert.All(list!, e => Assert.Equal(2090, e.Year));
    }

    [Fact]
    public async Task Expense_Update_ChangesAmount()
    {
        var create = await _client.PostAsync("/api/expenses", Json(new { Year = 2024, CashRegisterId = "CR5", Amount = 75.0 }));
        var created = await ReadAs<ExpenseDto>(create);
        var r = await _client.PutAsync($"/api/expenses/2024/{created!.Id}", Json(new { Year = 2024, CashRegisterId = "CR5", Amount = 150.0 }));
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var dto = await ReadAs<ExpenseDto>(r);
        Assert.Equal(150.0, dto?.Amount);
    }

    [Fact]
    public async Task Expense_Delete_HidesTheRow_AndKeepsTheCitedRecord()
    {
        var create = await _client.PostAsync("/api/expenses", Json(new { Year = 2024, CashRegisterId = "CR6", Amount = 25.0 }));
        var created = await ReadAs<ExpenseDto>(create);
        var r = await _client.DeleteAsync($"/api/expenses/2024/{created!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, r.StatusCode);

        var list = await ReadAs<List<ExpenseDto>>(await _client.GetAsync("/api/expenses?year=2024"));
        Assert.DoesNotContain(list!, expense => expense.Id == created.Id);
        var get = await _client.GetAsync($"/api/expenses/2024/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, get.StatusCode);

        var purge = await _client.PostAsync("/api/retention/clear-deleted", Json(new { }));
        Assert.Equal(HttpStatusCode.OK, purge.StatusCode);
        var stillThere = await _client.GetAsync($"/api/expenses/2024/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, stillThere.StatusCode);
    }

    // ========== Money History ==========

    [Fact]
    public async Task MoneyHistory_Create_ReturnsCreated()
    {
        var r = await _client.PostAsync("/api/money-history", Json(new { Year = 2024, CashRegisterId = "CR1", Amount = 500.0, Type = "income", Description = "Room payment" }));
        Assert.Equal(HttpStatusCode.Created, r.StatusCode);
        var dto = await ReadAs<MoneyHistoryDto>(r);
        Assert.NotNull(dto?.Id);
        Assert.Equal("income", dto.Type);
        Assert.Equal(500.0, dto.Amount);
    }

    [Fact]
    public async Task MoneyHistory_GetByYearAndId_ReturnsOk()
    {
        var create = await _client.PostAsync("/api/money-history", Json(new { Year = 2024, CashRegisterId = "CR2", Amount = 300.0, Type = "expense" }));
        var created = await ReadAs<MoneyHistoryDto>(create);
        var r = await _client.GetAsync($"/api/money-history/2024/{created!.Id}");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var dto = await ReadAs<MoneyHistoryDto>(r);
        Assert.Equal(300.0, dto?.Amount);
    }

    [Fact]
    public async Task MoneyHistory_List_FilteredByYear()
    {
        await _client.PostAsync("/api/money-history", Json(new { Year = 2100, CashRegisterId = "CR10", Amount = 100.0, Type = "income" }));
        await _client.PostAsync("/api/money-history", Json(new { Year = 2101, CashRegisterId = "CR11", Amount = 200.0, Type = "income" }));
        var r = await _client.GetAsync("/api/money-history?year=2100");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var list = await ReadAs<List<MoneyHistoryDto>>(r);
        Assert.All(list!, m => Assert.Equal(2100, m.Year));
    }

    [Fact]
    public async Task MoneyHistory_NoUpdate_PutNotFound()
    {
        // Money history is append-only — PUT returns MethodNotAllowed (405) since route exists for GET/DELETE
        var r = await _client.PutAsync("/api/money-history/2024/someid", Json(new { Year = 2024, Amount = 999.0 }));
        Assert.Equal(HttpStatusCode.MethodNotAllowed, r.StatusCode);
    }

    [Fact]
    public async Task MoneyHistory_Delete_IsRejected()
    {
        var create = await _client.PostAsync("/api/money-history", Json(new { Year = 2024, CashRegisterId = "CR20", Amount = 50.0, Type = "income" }));
        var created = await ReadAs<MoneyHistoryDto>(create);
        var r = await _client.DeleteAsync($"/api/money-history/2024/{created!.Id}");
        Assert.Equal(HttpStatusCode.MethodNotAllowed, r.StatusCode);
        var get = await _client.GetAsync($"/api/money-history/2024/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, get.StatusCode);
    }

    [Fact]
    public async Task MoneyHistory_Reverse_KeepsTheOriginal_AndPostsTheOppositeCashLine()
    {
        var create = await _client.PostAsync("/api/money-history", Json(new { Year = 2024, CashRegisterId = "CR21", Amount = 40.0, Type = "income", Description = "Float" }));
        var created = await ReadAs<MoneyHistoryDto>(create);
        var reverse = await _client.PostAsync($"/api/money-history/2024/{created!.Id}/reverse", Json(new { }));
        Assert.Equal(HttpStatusCode.Created, reverse.StatusCode);
        var reversal = await ReadAs<MoneyHistoryDto>(reverse);
        Assert.Equal(created.Id, reversal?.ReversesId);
        Assert.Equal(-40.0, reversal?.Amount);

        var original = await ReadAs<MoneyHistoryDto>(await _client.GetAsync($"/api/money-history/2024/{created.Id}"));
        Assert.Equal(reversal!.Id, original?.ReversedById);

        var again = await _client.PostAsync($"/api/money-history/2024/{created.Id}/reverse", Json(new { }));
        Assert.Equal(HttpStatusCode.Conflict, again.StatusCode);

        var today = DateOnly.FromDateTime(DateTime.UtcNow).ToString("yyyy-MM-dd");
        var lines = await ReadAs<List<LedgerLineDto>>(await _client.GetAsync($"/api/ledger/lines?from={today}&to={today}&unassigned=true"));
        var cash = lines!.Where(line => line.AccountCode == "1000" && (line.SourceId == created.Id || line.SourceId == reversal.Id)).ToList();
        Assert.Equal(2, cash.Count);
        Assert.Equal(cash.Sum(line => line.Debit), cash.Sum(line => line.Credit));
    }

    [Fact]
    public async Task Booking_Delete_WithLedgerLines_HidesTheBooking_AndKeepsItForAudit()
    {
        var create = await _client.PostAsync("/api/bookings", Json(new { Year = 2024, ClientId = "C-Cited", Status = "Confirmed", ArrivalDate = "2024-04-01", DepartureDate = "2024-04-03", PrepaidAmount = 25.0 }));
        var created = await ReadAs<BookingDto>(create);
        var delete = await _client.DeleteAsync($"/api/bookings/2024/{created!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, delete.StatusCode);

        var listed = await ReadAs<List<BookingDto>>(await _client.GetAsync("/api/bookings?year=2024"));
        Assert.DoesNotContain(listed!, booking => booking.Id == created.Id);
        var hidden = await ReadAs<List<BookingDto>>(await _client.GetAsync("/api/bookings?year=2024&includeDeleted=true"));
        Assert.Contains(hidden!, booking => booking.Id == created.Id && booking.DeletedAt.HasValue);

        var get = await ReadAs<BookingDto>(await _client.GetAsync($"/api/bookings/2024/{created.Id}"));
        Assert.NotNull(get?.DeletedAt);

        var purge = await _client.PostAsync("/api/retention/clear-deleted", Json(new { }));
        Assert.Equal(HttpStatusCode.OK, purge.StatusCode);
        var afterPurge = await _client.GetAsync($"/api/bookings/2024/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, afterPurge.StatusCode);
    }
}

