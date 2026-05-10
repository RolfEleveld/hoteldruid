using System.Net;
using System.Text;
using System.Text.Json;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using HotelDroid.Api;

namespace HotelDroid.Api.Tests.Integration;

[Collection("Sequential")]
public class Layer4ApiTests : IAsyncLifetime
{
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;
    private string _testDataRoot = null!;
    private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

    private class BookingDto { public string? Id { get; set; } public int Year { get; set; } public string? ClientId { get; set; } public string? RoomId { get; set; } public string? Status { get; set; } public string? ArrivalDate { get; set; } public string? DepartureDate { get; set; } }
    private class BookingCostDto { public string? Id { get; set; } public int Year { get; set; } public string? BookingId { get; set; } public string? TariffId { get; set; } public double? Amount { get; set; } public string? Description { get; set; } }
    private class BookingGuestDto { public string? Id { get; set; } public int Year { get; set; } public string? BookingId { get; set; } public string? ClientId { get; set; } public int? GuestNumber { get; set; } }
    private class CancelledBookingDto { public string? Id { get; set; } public int Year { get; set; } public string? ClientId { get; set; } public string? Status { get; set; } public string? CancelledAt { get; set; } }
    private class ExpenseDto { public string? Id { get; set; } public int Year { get; set; } public string? CashRegisterId { get; set; } public double? Amount { get; set; } public string? Description { get; set; } }
    private class MoneyHistoryDto { public string? Id { get; set; } public int Year { get; set; } public string? CashRegisterId { get; set; } public double? Amount { get; set; } public string? Type { get; set; } }

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
        var r = await _client.PostAsync("/api/bookings", Json(new { Year = 2024, ClientId = "C001", RoomId = "101", Status = "Confirmed" }));
        Assert.Equal(HttpStatusCode.Created, r.StatusCode);
        var dto = await ReadAs<BookingDto>(r);
        Assert.NotNull(dto?.Id);
        Assert.Equal(2024, dto.Year);
        Assert.Equal("C001", dto.ClientId);
        Assert.Equal("Confirmed", dto.Status);
    }

    [Fact]
    public async Task Booking_GetByYearAndId_ReturnsOk()
    {
        var create = await _client.PostAsync("/api/bookings", Json(new { Year = 2024, ClientId = "C002", Status = "Pending" }));
        var created = await ReadAs<BookingDto>(create);
        var r = await _client.GetAsync($"/api/bookings/2024/{created!.Id}");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var dto = await ReadAs<BookingDto>(r);
        Assert.Equal("C002", dto?.ClientId);
    }

    [Fact]
    public async Task Booking_List_FilteredByYear()
    {
        await _client.PostAsync("/api/bookings", Json(new { Year = 2060, ClientId = "C010" }));
        await _client.PostAsync("/api/bookings", Json(new { Year = 2061, ClientId = "C011" }));
        var r = await _client.GetAsync("/api/bookings?year=2060");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var list = await ReadAs<List<BookingDto>>(r);
        Assert.All(list!, b => Assert.Equal(2060, b.Year));
    }

    [Fact]
    public async Task Booking_Update_ChangesFields()
    {
        var create = await _client.PostAsync("/api/bookings", Json(new { Year = 2024, ClientId = "C003", Status = "Pending" }));
        var created = await ReadAs<BookingDto>(create);
        var r = await _client.PutAsync($"/api/bookings/2024/{created!.Id}", Json(new { Year = 2024, ClientId = "C003", Status = "Confirmed" }));
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var dto = await ReadAs<BookingDto>(r);
        Assert.Equal("Confirmed", dto?.Status);
    }

    [Fact]
    public async Task Booking_Delete_ReturnsNoContent()
    {
        var create = await _client.PostAsync("/api/bookings", Json(new { Year = 2024, ClientId = "C004" }));
        var created = await ReadAs<BookingDto>(create);
        var r = await _client.DeleteAsync($"/api/bookings/2024/{created!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, r.StatusCode);
        var get = await _client.GetAsync($"/api/bookings/2024/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, get.StatusCode);
    }

    [Fact]
    public async Task Booking_Cancel_MovesToCancelledBookings()
    {
        var create = await _client.PostAsync("/api/bookings", Json(new { Year = 2024, ClientId = "C005", Status = "Confirmed" }));
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
        var create = await _client.PostAsync("/api/bookings", Json(new { Year = 2024, ClientId = "C999", Status = "Confirmed" }));
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
    public async Task Expense_Delete_ReturnsNoContent()
    {
        var create = await _client.PostAsync("/api/expenses", Json(new { Year = 2024, CashRegisterId = "CR6", Amount = 25.0 }));
        var created = await ReadAs<ExpenseDto>(create);
        var r = await _client.DeleteAsync($"/api/expenses/2024/{created!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, r.StatusCode);
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
    public async Task MoneyHistory_Delete_ReturnsNoContent()
    {
        var create = await _client.PostAsync("/api/money-history", Json(new { Year = 2024, CashRegisterId = "CR20", Amount = 50.0, Type = "income" }));
        var created = await ReadAs<MoneyHistoryDto>(create);
        var r = await _client.DeleteAsync($"/api/money-history/2024/{created!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, r.StatusCode);
    }
}
