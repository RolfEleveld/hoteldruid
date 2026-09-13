using System.Net;
using System.Net.Http.Json;
using HotelDruid.Api;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace HotelDruid.Api.Tests.Integration;

/// <summary>
/// Integration tests for the room-tariff resolution engine (/api/tariffs/resolve) and its
/// automatic wiring into booking create/update/check-in.
/// </summary>
[Collection("Sequential")]
public class TariffResolutionApiTests : IAsyncLifetime
{
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;
    private string _testDataRoot = null!;

    private record RoomDto(string? Id, string Name, int Capacity);

    private record TariffDto(
        string? Id, string Name, string? Description, string ApplicabilityMode,
        int? WeekdaysMask, int? MinNights, double PricePerNight, double? PricePerPerson,
        int Priority, bool IsActive, string TargetType, DateTime? CreatedAt, int? MinGuestCount = null);

    private record TariffAssignmentDto(string? Id, string? TariffId, string TargetType, string? TargetId, DateTime? CreatedAt);

    private record TariffSeasonDto(string? Id, string? TariffId, string? Name, DateOnly StartDate, DateOnly EndDate, string AdjustmentType, double Value, DateTime? CreatedAt);

    private record TariffResolutionResult(
        string TariffId, string TariffName, int Nights, double BasePricePerNight,
        double AdjustedPricePerNight, double TotalPrice, string? AppliedSeasonName, string ApplicabilityMode);

    private record BookingDto(
        string? Id, int Year, string? ClientId, string? RoomId, DateOnly? ArrivalDate, DateOnly? DepartureDate,
        string? Status, string? Notes, DateOnly? ActualArrivalDate, int? ActualGuestCount, double? PrepaidAmount,
        DateOnly? ActualDepartureDate, int? ExpectedGuestCount = null, string? TariffId = null);

    public async Task InitializeAsync()
    {
        _testDataRoot = Path.Combine(Path.GetTempPath(), $"api-tariff-resolution-tests-{Guid.NewGuid()}");
        Directory.CreateDirectory(_testDataRoot);
        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(b => b.UseSetting("DataRoot", _testDataRoot));
        _client = _factory.CreateClient();
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        _client?.Dispose();
        _factory?.Dispose();
        if (Directory.Exists(_testDataRoot)) { try { Directory.Delete(_testDataRoot, true); } catch { } }
        await Task.CompletedTask;
    }

    private async Task<string> CreateRoomAsync(string name, int capacity = 4)
    {
        var response = await _client.PostAsJsonAsync("/api/rooms", new RoomDto(null, name, capacity));
        response.EnsureSuccessStatusCode();
        var created = await response.Content.ReadFromJsonAsync<RoomDto>();
        return created!.Id!;
    }

    private async Task<string> CreateTariffAsync(string name, string mode, double pricePerNight,
        int? weekdaysMask = null, int? minNights = null, int? minGuestCount = null, int priority = 0, double? pricePerPerson = null)
    {
        var response = await _client.PostAsJsonAsync("/api/tariffs", new TariffDto(
            null, name, null, mode, weekdaysMask, minNights, pricePerNight, pricePerPerson, priority, true, "Room", null, minGuestCount));
        response.EnsureSuccessStatusCode();
        var created = await response.Content.ReadFromJsonAsync<TariffDto>();
        return created!.Id!;
    }

    private async Task AssignTariffToRoomAsync(string tariffId, string roomId)
    {
        var response = await _client.PostAsJsonAsync("/api/tariff-assignments", new TariffAssignmentDto(null, tariffId, "Room", roomId, null));
        response.EnsureSuccessStatusCode();
    }

    private static DateOnly NextSaturdayFrom(DateOnly start)
    {
        var date = start;
        while (date.DayOfWeek != DayOfWeek.Saturday) date = date.AddDays(1);
        return date;
    }

    [Fact]
    public async Task Resolve_PicksRackRate_WhenOnlyAnyDayTariffAssigned()
    {
        var roomId = await CreateRoomAsync("TR-Rack");
        var tariffId = await CreateTariffAsync("Rack Rate", "AnyDay", 100.0);
        await AssignTariffToRoomAsync(tariffId, roomId);

        // A Wednesday arrival, 3 nights, avoids weekday-specific interference.
        var arrival = new DateOnly(2030, 5, 1);
        var departure = arrival.AddDays(3);

        var response = await _client.GetAsync($"/api/tariffs/resolve?roomId={roomId}&arrivalDate={arrival:yyyy-MM-dd}&departureDate={departure:yyyy-MM-dd}&guestCount=2");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<TariffResolutionResult>();

        Assert.Equal(tariffId, result!.TariffId);
        Assert.Equal(3, result.Nights);
        Assert.Equal(300.0, result.TotalPrice);
    }

    [Fact]
    public async Task Resolve_PrefersWeekdayTariff_OverAnyDay_WhenArrivalMatchesWeekday()
    {
        var roomId = await CreateRoomAsync("TR-Weekend");
        var rackId = await CreateTariffAsync("Rack Rate", "AnyDay", 100.0);
        var weekendId = await CreateTariffAsync("Weekend Rate", "Weekdays", 150.0, weekdaysMask: 32 /* Saturday */);
        await AssignTariffToRoomAsync(rackId, roomId);
        await AssignTariffToRoomAsync(weekendId, roomId);

        var saturday = NextSaturdayFrom(new DateOnly(2030, 5, 1));
        var departure = saturday.AddDays(2);

        var response = await _client.GetAsync($"/api/tariffs/resolve?roomId={roomId}&arrivalDate={saturday:yyyy-MM-dd}&departureDate={departure:yyyy-MM-dd}&guestCount=1");
        var result = await response.Content.ReadFromJsonAsync<TariffResolutionResult>();

        Assert.Equal(weekendId, result!.TariffId);
    }

    [Fact]
    public async Task Resolve_PrefersMinStayTariff_OverWeekdayAndAnyDay_WhenNightsThresholdMet()
    {
        var roomId = await CreateRoomAsync("TR-Week");
        var rackId = await CreateTariffAsync("Rack Rate", "AnyDay", 100.0);
        var weekId = await CreateTariffAsync("Week Rate", "MinStay", 70.0, minNights: 7);
        await AssignTariffToRoomAsync(rackId, roomId);
        await AssignTariffToRoomAsync(weekId, roomId);

        var arrival = new DateOnly(2030, 5, 1);
        var departure = arrival.AddDays(7);

        var response = await _client.GetAsync($"/api/tariffs/resolve?roomId={roomId}&arrivalDate={arrival:yyyy-MM-dd}&departureDate={departure:yyyy-MM-dd}&guestCount=1");
        var result = await response.Content.ReadFromJsonAsync<TariffResolutionResult>();

        Assert.Equal(weekId, result!.TariffId);
        Assert.Equal(490.0, result.TotalPrice);
    }

    [Fact]
    public async Task Resolve_PrefersMinGuestsTariff_OverAnyDay_WhenGuestThresholdMet()
    {
        var roomId = await CreateRoomAsync("TR-ExtraGuest");
        var rackId = await CreateTariffAsync("Rack Rate", "AnyDay", 100.0);
        var extraGuestId = await CreateTariffAsync("Extra Guest Surcharge", "MinGuests", 130.0, minGuestCount: 3);
        await AssignTariffToRoomAsync(rackId, roomId);
        await AssignTariffToRoomAsync(extraGuestId, roomId);

        var arrival = new DateOnly(2030, 5, 1);
        var departure = arrival.AddDays(2);

        var underThreshold = await _client.GetAsync($"/api/tariffs/resolve?roomId={roomId}&arrivalDate={arrival:yyyy-MM-dd}&departureDate={departure:yyyy-MM-dd}&guestCount=2");
        var underResult = await underThreshold.Content.ReadFromJsonAsync<TariffResolutionResult>();
        Assert.Equal(rackId, underResult!.TariffId);

        var atThreshold = await _client.GetAsync($"/api/tariffs/resolve?roomId={roomId}&arrivalDate={arrival:yyyy-MM-dd}&departureDate={departure:yyyy-MM-dd}&guestCount=3");
        var atResult = await atThreshold.Content.ReadFromJsonAsync<TariffResolutionResult>();
        Assert.Equal(extraGuestId, atResult!.TariffId);
    }

    [Fact]
    public async Task Resolve_AppliesPercentageSeasonalAdjustment()
    {
        var roomId = await CreateRoomAsync("TR-Season");
        var tariffId = await CreateTariffAsync("Rack Rate", "AnyDay", 100.0);
        await AssignTariffToRoomAsync(tariffId, roomId);

        var arrival = new DateOnly(2030, 7, 15);
        var departure = arrival.AddDays(2);

        var seasonResponse = await _client.PostAsJsonAsync("/api/tariff-seasons", new TariffSeasonDto(
            null, tariffId, "High Season", new DateOnly(2030, 7, 1), new DateOnly(2030, 8, 31), "PercentageAdjustment", 20.0, null));
        seasonResponse.EnsureSuccessStatusCode();

        var response = await _client.GetAsync($"/api/tariffs/resolve?roomId={roomId}&arrivalDate={arrival:yyyy-MM-dd}&departureDate={departure:yyyy-MM-dd}&guestCount=1");
        var result = await response.Content.ReadFromJsonAsync<TariffResolutionResult>();

        Assert.Equal(120.0, result!.AdjustedPricePerNight);
        Assert.Equal(240.0, result.TotalPrice);
        Assert.Equal("High Season", result.AppliedSeasonName);
    }

    [Fact]
    public async Task Resolve_ReturnsNotFound_WhenNoTariffAssignedToRoom()
    {
        var roomId = await CreateRoomAsync("TR-NoTariff");
        var arrival = new DateOnly(2030, 5, 1);
        var departure = arrival.AddDays(2);

        var response = await _client.GetAsync($"/api/tariffs/resolve?roomId={roomId}&arrivalDate={arrival:yyyy-MM-dd}&departureDate={departure:yyyy-MM-dd}&guestCount=1");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateBooking_AutoResolvesTariffId_FromExpectedGuestCount()
    {
        var roomId = await CreateRoomAsync("TR-Book");
        var tariffId = await CreateTariffAsync("Rack Rate", "AnyDay", 90.0);
        await AssignTariffToRoomAsync(tariffId, roomId);

        var arrival = new DateOnly(2031, 3, 10);
        var departure = arrival.AddDays(2);

        var createResponse = await _client.PostAsJsonAsync("/api/bookings", new BookingDto(
            null, 2031, "guest-1", roomId, arrival, departure, "Confirmed", null, null, null, null, null, ExpectedGuestCount: 2));
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<BookingDto>();

        Assert.Equal(tariffId, created!.TariffId);
    }

    [Fact]
    public async Task CheckIn_CorrectsTariff_WhenActualGuestsExceedExpected_AndRuleEnabledByDefault()
    {
        var roomId = await CreateRoomAsync("TR-CheckInCorrect");
        var rackId = await CreateTariffAsync("Rack Rate", "AnyDay", 90.0);
        var extraGuestId = await CreateTariffAsync("Extra Guest Surcharge", "MinGuests", 130.0, minGuestCount: 4);
        await AssignTariffToRoomAsync(rackId, roomId);
        await AssignTariffToRoomAsync(extraGuestId, roomId);

        var arrival = new DateOnly(2031, 4, 10);
        var departure = arrival.AddDays(2);

        var createResponse = await _client.PostAsJsonAsync("/api/bookings", new BookingDto(
            null, 2031, "guest-2", roomId, arrival, departure, "Confirmed", null, null, null, null, null, ExpectedGuestCount: 2));
        var created = await createResponse.Content.ReadFromJsonAsync<BookingDto>();
        Assert.Equal(rackId, created!.TariffId);

        var checkInResponse = await _client.PostAsJsonAsync($"/api/bookings/2031/{created.Id}/check-in", new
        {
            ActualArrivalDate = arrival,
            ActualGuestCount = 4,
            Status = "CheckedIn"
        });
        Assert.Equal(HttpStatusCode.OK, checkInResponse.StatusCode);
        var checkedIn = await checkInResponse.Content.ReadFromJsonAsync<BookingDto>();

        Assert.Equal(extraGuestId, checkedIn!.TariffId);
    }

    [Fact]
    public async Task CheckIn_DoesNotCorrectTariff_WhenAutoCorrectRuleDisabledInSettings()
    {
        var roomId = await CreateRoomAsync("TR-CheckInRuleOff");
        var rackId = await CreateTariffAsync("Rack Rate", "AnyDay", 90.0);
        var extraGuestId = await CreateTariffAsync("Extra Guest Surcharge", "MinGuests", 130.0, minGuestCount: 4);
        await AssignTariffToRoomAsync(rackId, roomId);
        await AssignTariffToRoomAsync(extraGuestId, roomId);

        // Persist the business rule toggle as disabled.
        var configResponse = await _client.PutAsJsonAsync("/api/system/configuration", new
        {
            Id = "system",
            Settings = new Dictionary<string, string> { ["TariffAutoCorrectOnCheckIn"] = "false" }
        });
        Assert.Equal(HttpStatusCode.OK, configResponse.StatusCode);

        var arrival = new DateOnly(2031, 5, 10);
        var departure = arrival.AddDays(2);

        var createResponse = await _client.PostAsJsonAsync("/api/bookings", new BookingDto(
            null, 2031, "guest-3", roomId, arrival, departure, "Confirmed", null, null, null, null, null, ExpectedGuestCount: 2));
        var created = await createResponse.Content.ReadFromJsonAsync<BookingDto>();
        Assert.Equal(rackId, created!.TariffId);

        var checkInResponse = await _client.PostAsJsonAsync($"/api/bookings/2031/{created.Id}/check-in", new
        {
            ActualArrivalDate = arrival,
            ActualGuestCount = 4,
            Status = "CheckedIn"
        });
        var checkedIn = await checkInResponse.Content.ReadFromJsonAsync<BookingDto>();

        Assert.Equal(rackId, checkedIn!.TariffId);
    }

    [Fact]
    public async Task CheckIn_DoesNotRecompute_WhenActualGuestCountNotGreaterThanExpected()
    {
        var roomId = await CreateRoomAsync("TR-CheckInSameGuests");
        var rackId = await CreateTariffAsync("Rack Rate", "AnyDay", 90.0);
        var extraGuestId = await CreateTariffAsync("Extra Guest Surcharge", "MinGuests", 130.0, minGuestCount: 2);
        await AssignTariffToRoomAsync(rackId, roomId);
        await AssignTariffToRoomAsync(extraGuestId, roomId);

        var arrival = new DateOnly(2031, 6, 10);
        var departure = arrival.AddDays(2);

        // ExpectedGuestCount already meets the MinGuests threshold, so the resolved tariff at
        // creation is already the surcharge one; checking in with the same guest count must not
        // "correct" anything because actual is not greater than expected.
        var createResponse = await _client.PostAsJsonAsync("/api/bookings", new BookingDto(
            null, 2031, "guest-4", roomId, arrival, departure, "Confirmed", null, null, null, null, null, ExpectedGuestCount: 2));
        var created = await createResponse.Content.ReadFromJsonAsync<BookingDto>();
        Assert.Equal(extraGuestId, created!.TariffId);

        var checkInResponse = await _client.PostAsJsonAsync($"/api/bookings/2031/{created.Id}/check-in", new
        {
            ActualArrivalDate = arrival,
            ActualGuestCount = 2,
            Status = "CheckedIn"
        });
        var checkedIn = await checkInResponse.Content.ReadFromJsonAsync<BookingDto>();

        Assert.Equal(extraGuestId, checkedIn!.TariffId);
    }
}
