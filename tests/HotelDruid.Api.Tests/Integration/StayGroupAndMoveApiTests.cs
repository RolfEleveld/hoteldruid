using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using HotelDruid.Api;
using HotelDruid.Shared;
using HotelDruid.Shared.Configuration;
using Microsoft.AspNetCore.Mvc.Testing;

namespace HotelDruid.Api.Tests.Integration;

/// <summary>
/// A party is its own id. The same name later is a different party.
/// A move splits one stay into two linked bookings and carries the price.
/// </summary>
[Collection("Sequential")]
public class StayGroupAndMoveApiTests : IAsyncLifetime
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;
    private string _root = null!;

    public Task InitializeAsync()
    {
        _root = Path.Combine(Path.GetTempPath(), $"api-stay-group-move-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_root);
        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => builder.UseSetting("DataRoot", _root));
        _client = _factory.CreateClient();
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        _client.Dispose();
        _factory.Dispose();
        if (Directory.Exists(_root))
        {
            try { Directory.Delete(_root, true); } catch { }
        }

        return Task.CompletedTask;
    }

    [Fact]
    public async Task SameName_IsTwoParties_AndALaterArrivalStaysInThePartyItWasGiven()
    {
        var first = await CreateGroupAsync("Schneider");
        var second = await CreateGroupAsync("Schneider");
        Assert.NotEqual(first.Id, second.Id);

        var earlyRoom = await CreateRoomAsync("101");
        var laterRoom = await CreateRoomAsync("102");
        var otherRoom = await CreateRoomAsync("103");
        var early = await CreateStayAsync(earlyRoom, new DateOnly(2026, 11, 14), new DateOnly(2026, 11, 17), first.Id, "Schneider");
        var later = await CreateStayAsync(laterRoom, new DateOnly(2026, 11, 20), new DateOnly(2026, 11, 22), first.Id, "Schneider");
        var other = await CreateStayAsync(otherRoom, new DateOnly(2026, 11, 14), new DateOnly(2026, 11, 16), second.Id, "Schneider", "Incoming");

        var parties = await _client.GetFromJsonAsync<List<StayGroupDto>>("/api/stay-groups", Json) ?? [];
        var firstParty = Assert.Single(parties, party => party.Id == first.Id);
        var secondParty = Assert.Single(parties, party => party.Id == second.Id);
        Assert.Equal("Schneider", firstParty.Name);
        Assert.Equal("Schneider", secondParty.Name);
        Assert.Equal(2, firstParty.RoomCount);
        Assert.Equal(1, secondParty.RoomCount);
        Assert.Equal(new DateOnly(2026, 11, 14), firstParty.FirstArrival);
        Assert.Equal(new DateOnly(2026, 11, 22), firstParty.LastDeparture);
        Assert.Equal(new DateOnly(2026, 11, 14), secondParty.FirstArrival);
        Assert.Equal(new DateOnly(2026, 11, 16), secondParty.LastDeparture);

        var firstMembers = await ListGroupAsync(first.Id!);
        Assert.Equal(new[] { early, later }.OrderBy(id => id), firstMembers.Select(stay => stay.Id).OrderBy(id => id));
        var secondMembers = await ListGroupAsync(second.Id!);
        Assert.Equal(other, Assert.Single(secondMembers).Id);
    }

    [Fact]
    public async Task Confirm_OneParty_LeavesTheOtherPartyNamedTheSameWay()
    {
        var first = await CreateGroupAsync("Schneider");
        var second = await CreateGroupAsync("Schneider");
        var firstRoom = await CreateRoomAsync("201");
        var secondRoom = await CreateRoomAsync("202");
        var kept = await CreateStayAsync(firstRoom, new DateOnly(2026, 12, 1), new DateOnly(2026, 12, 3), first.Id, "Schneider", "Incoming");
        var untouched = await CreateStayAsync(secondRoom, new DateOnly(2026, 12, 1), new DateOnly(2026, 12, 4), second.Id, "Schneider", "Incoming");

        var confirm = await _client.PostAsync($"/api/bookings/groups/{Uri.EscapeDataString(first.Id!)}/confirm", null);
        Assert.Equal(HttpStatusCode.OK, confirm.StatusCode);

        var confirmed = await GetStayAsync(kept);
        var stillIncoming = await GetStayAsync(untouched);
        Assert.Equal("Confirmed", confirmed.Status);
        Assert.Equal("Incoming", stillIncoming.Status);
        Assert.Equal(second.Id, stillIncoming.GroupCode);
    }

    [Fact]
    public async Task BlankGroupName_IsRejected()
    {
        var response = await _client.PostAsJsonAsync("/api/stay-groups", new StayGroupRequest("  "), Json);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Move_OnALaterNight_SplitsTheStay_AndCarriesTheQuotedPrice()
    {
        var fromRoom = await CreateRoomAsync("301");
        var toRoom = await CreateRoomAsync("302");
        await CreateRackAsync(fromRoom);
        var stay = await CreateStayAsync(fromRoom, new DateOnly(2027, 6, 1), new DateOnly(2027, 6, 4), status: "Confirmed");
        var before = await GetStayAsync(stay);
        Assert.Equal(300, before.TariffTotalPrice);

        var moved = await MoveAsync(stay, 2027, new DateOnly(2027, 6, 2), toRoom, "Maintenance", "The corridor stays open.");
        Assert.Equal(HttpStatusCode.OK, moved.StatusCode);
        var continued = await ReadStayAsync(moved);
        var origin = await GetStayAsync(stay);

        Assert.Equal(new DateOnly(2027, 6, 2), continued.ArrivalDate);
        Assert.Equal(new DateOnly(2027, 6, 4), continued.DepartureDate);
        Assert.Equal(toRoom, continued.RoomId);
        Assert.Equal(stay, continued.ContinuedFromId);
        Assert.Equal("Confirmed", continued.Status);
        Assert.Equal(200, continued.TariffTotalPrice);
        Assert.Contains("Continued from 301 on 2027-06-02", continued.Notes);
        Assert.Contains("Maintenance", continued.Notes);
        Assert.Contains("The corridor stays open.", continued.Notes);

        Assert.Equal(new DateOnly(2027, 6, 2), origin.DepartureDate);
        Assert.Equal(continued.Id, origin.ContinuedToId);
        Assert.Equal("Confirmed", origin.Status);
        Assert.Null(origin.ActualDepartureDate);
        Assert.Equal(100, origin.TariffTotalPrice);
        Assert.Contains("Moved to 302 on 2027-06-02", origin.Notes);

        var again = await MoveAsync(stay, 2027, new DateOnly(2027, 6, 3), toRoom, "Relocate", null);
        Assert.Equal(HttpStatusCode.Conflict, again.StatusCode);
        Assert.Contains("already moved", await ErrorAsync(again), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Move_WhenTheGuestIsAlreadyInHouse_ChecksOutTheFirstRoomOnTheMoveNight()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var fromRoom = await CreateRoomAsync("401");
        var toRoom = await CreateRoomAsync("402");
        await CreateRackAsync(fromRoom);
        var stay = await CreateStayAsync(fromRoom, today.AddDays(-1), today.AddDays(2), status: "CheckedIn", arrived: true);

        var moved = await MoveAsync(stay, today.AddDays(-1).Year, today, toRoom, "Relocate", "Moving tonight.");
        Assert.Equal(HttpStatusCode.OK, moved.StatusCode);
        var continued = await ReadStayAsync(moved);
        var origin = await GetStayAsync(stay);

        Assert.Equal("CheckedOut", origin.Status);
        Assert.Equal(today, origin.ActualDepartureDate);
        Assert.Equal("CheckedIn", continued.Status);
        Assert.Equal(today, continued.ActualArrivalDate);
        Assert.Equal(today, continued.ArrivalDate);
    }

    [Fact]
    public async Task Move_RejectsANightThatDoesNotLeaveOneNightInEachRoom_AndARoomThatIsTaken()
    {
        var fromRoom = await CreateRoomAsync("501");
        var takenRoom = await CreateRoomAsync("502");
        var freeRoom = await CreateRoomAsync("503");
        var stay = await CreateStayAsync(fromRoom, new DateOnly(2027, 7, 1), new DateOnly(2027, 7, 4));
        await CreateStayAsync(takenRoom, new DateOnly(2027, 7, 2), new DateOnly(2027, 7, 5));

        var onArrival = await MoveAsync(stay, 2027, new DateOnly(2027, 7, 1), freeRoom, "Relocate", null);
        var onDeparture = await MoveAsync(stay, 2027, new DateOnly(2027, 7, 4), freeRoom, "Relocate", null);
        var sameRoom = await MoveAsync(stay, 2027, new DateOnly(2027, 7, 2), fromRoom, "Relocate", null);
        var taken = await MoveAsync(stay, 2027, new DateOnly(2027, 7, 2), takenRoom, "Relocate", null);

        Assert.Equal(HttpStatusCode.BadRequest, onArrival.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, onDeparture.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, sameRoom.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, taken.StatusCode);
        Assert.Contains("one night", await ErrorAsync(onArrival), StringComparison.OrdinalIgnoreCase);
        Assert.Contains("not free", await ErrorAsync(taken), StringComparison.OrdinalIgnoreCase);

        var origin = await GetStayAsync(stay);
        Assert.Null(origin.ContinuedToId);
        Assert.Equal(new DateOnly(2027, 7, 4), origin.DepartureDate);
    }

    [Fact]
    public async Task Move_KeepsTheParty_OnBothRooms()
    {
        var party = await CreateGroupAsync("Schneider");
        var fromRoom = await CreateRoomAsync("601");
        var toRoom = await CreateRoomAsync("602");
        var stay = await CreateStayAsync(fromRoom, new DateOnly(2027, 8, 1), new DateOnly(2027, 8, 4), party.Id, "Schneider");

        var moved = await MoveAsync(stay, 2027, new DateOnly(2027, 8, 3), toRoom, "Relocate", null);
        var continued = await ReadStayAsync(moved);
        Assert.Equal(party.Id, continued.GroupCode);
        Assert.Equal("Schneider", continued.GroupName);

        var members = await ListGroupAsync(party.Id!);
        Assert.Equal(2, members.Count);
        Assert.Contains(members, member => member.Id == stay);
        Assert.Contains(members, member => member.Id == continued.Id);
    }

    [Fact]
    public async Task Move_WithInclusiveTax_ChargesTheContinuedRoomTheSameAmountTheGuestWasQuoted()
    {
        await _client.PutAsJsonAsync("/api/system/configuration", new SystemConfiguration
        {
            Id = "system",
            AccountingStandard = "usali",
            DefaultCurrency = "EUR",
            Settings = new Dictionary<string, string>
            {
                ["TaxPricing"] = "Inclusive",
                ["TaxRateStay"] = "8.1"
            }
        }, Json);

        var fromRoom = await CreateRoomAsync("701");
        var toRoom = await CreateRoomAsync("702");
        await CreateRackAsync(fromRoom);
        var stay = await CreateStayAsync(fromRoom, new DateOnly(2027, 9, 1), new DateOnly(2027, 9, 4));
        var moved = await MoveAsync(stay, 2027, new DateOnly(2027, 9, 2), toRoom, "Relocate", null);
        Assert.Equal(HttpStatusCode.OK, moved.StatusCode);
        var continued = await ReadStayAsync(moved);

        var lines = await LedgerAsync(continued.Id!);
        var guestDebit = lines.Where(line => line.AccountCode == "1200").Sum(line => line.Debit - line.Credit);
        Assert.Equal(200m, guestDebit);
        Assert.Equal(200, continued.TariffTotalPrice);
        Assert.Contains(lines, line => line.AccountCode == "2300" && line.Credit > 0);
    }

    private async Task<StayGroupDto> CreateGroupAsync(string name)
    {
        var response = await _client.PostAsJsonAsync("/api/stay-groups", new StayGroupRequest(name), Json);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<StayGroupDto>(Json))!;
    }

    private async Task<string> CreateRoomAsync(string name)
    {
        var response = await _client.PostAsJsonAsync("/api/rooms", new { name, capacity = 2 }, Json);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var room = await response.Content.ReadFromJsonAsync<IdName>(Json);
        return room!.Id!;
    }

    private async Task CreateRackAsync(string roomId)
    {
        var tariff = await _client.PostAsJsonAsync("/api/tariffs", new
        {
            year = 2027,
            name = "Rack " + roomId,
            applicabilityMode = "AnyDay",
            pricePerNight = 100.0,
            isActive = true
        }, Json);
        Assert.Equal(HttpStatusCode.Created, tariff.StatusCode);
        var created = await tariff.Content.ReadFromJsonAsync<IdName>(Json);
        var assign = await _client.PostAsJsonAsync("/api/tariff-assignments", new
        {
            tariffId = created!.Id,
            targetType = "Room",
            targetId = roomId
        }, Json);
        Assert.Equal(HttpStatusCode.Created, assign.StatusCode);
    }

    private async Task<string> CreateStayAsync(
        string roomId,
        DateOnly arrival,
        DateOnly departure,
        string? groupId = null,
        string? groupName = null,
        string status = "Confirmed",
        bool arrived = false)
    {
        var response = await _client.PostAsJsonAsync("/api/bookings", new
        {
            year = arrival.Year,
            clientId = "guest-" + roomId,
            roomId,
            status,
            arrivalDate = arrival,
            departureDate = departure,
            actualArrivalDate = arrived ? arrival : (DateOnly?)null,
            actualGuestCount = arrived ? 1 : (int?)null,
            expectedGuestCount = 1,
            groupCode = groupId,
            groupName
        }, Json);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await ReadStayAsync(response)).Id!;
    }

    private Task<HttpResponseMessage> MoveAsync(string stayId, int year, DateOnly from, string roomId, string reason, string? note)
        => _client.PostAsJsonAsync($"/api/bookings/{year}/{stayId}/move", new BookingMoveRequest(from, roomId, reason, note), Json);

    private async Task<List<StayRow>> ListGroupAsync(string groupId)
    {
        return await _client.GetFromJsonAsync<List<StayRow>>($"/api/bookings?groupCode={Uri.EscapeDataString(groupId)}", Json) ?? [];
    }

    private async Task<StayRow> GetStayAsync(string id)
    {
        var stay = await _client.GetFromJsonAsync<StayRow>($"/api/bookings/by-id/{id}", Json);
        Assert.NotNull(stay);
        return stay;
    }

    private static async Task<StayRow> ReadStayAsync(HttpResponseMessage response)
    {
        var stay = await response.Content.ReadFromJsonAsync<StayRow>(Json);
        Assert.NotNull(stay);
        return stay;
    }

    private static async Task<string> ErrorAsync(HttpResponseMessage response)
    {
        var payload = await response.Content.ReadFromJsonAsync<ApiError>(Json);
        return payload?.Error ?? "";
    }

    private async Task<List<LedgerRow>> LedgerAsync(string bookingId)
    {
        var from = new DateOnly(2020, 1, 1);
        var to = new DateOnly(2030, 1, 1);
        return await _client.GetFromJsonAsync<List<LedgerRow>>(
            $"/api/ledger/lines?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}&bookingId={Uri.EscapeDataString(bookingId)}", Json) ?? [];
    }

    private sealed class IdName
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
    }

    private sealed class StayRow
    {
        public string? Id { get; set; }
        public string? RoomId { get; set; }
        public string? Status { get; set; }
        public DateOnly? ArrivalDate { get; set; }
        public DateOnly? DepartureDate { get; set; }
        public DateOnly? ActualArrivalDate { get; set; }
        public DateOnly? ActualDepartureDate { get; set; }
        public string? Notes { get; set; }
        public double? TariffTotalPrice { get; set; }
        public string? GroupCode { get; set; }
        public string? GroupName { get; set; }
        public string? ContinuedFromId { get; set; }
        public string? ContinuedToId { get; set; }
    }

    private sealed class LedgerRow
    {
        public string? AccountCode { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
    }

    private sealed class ApiError
    {
        public string? Error { get; set; }
    }
}
