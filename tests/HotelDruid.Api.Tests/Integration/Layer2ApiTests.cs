using System.Net;
using System.Text;
using System.Text.Json;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using HotelDruid.Api;

namespace HotelDruid.Api.Tests.Integration;

[Collection("Sequential")]
public class Layer2ApiTests : IAsyncLifetime
{
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;
    private string _testDataRoot = null!;
    private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

    private class ClientDto
    {
        public string? Id { get; set; }
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? Email { get; set; }
        public string? Email2 { get; set; }
        public string? Email3 { get; set; }
        public List<string>? Emails { get; set; }
        public string? Phone { get; set; }
        public string? Phone2 { get; set; }
        public string? Phone3 { get; set; }
        public List<string>? Phones { get; set; }
        public string? City { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? DocumentExpiry { get; set; }
        public string? DocumentType { get; set; }
        public string? Nationality { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
    private class UserPrivilegeDto { public string? Id { get; set; } public int? UserId { get; set; } public int? Year { get; set; } public string? AllowedRules { get; set; } public string? BookingInsertPriv { get; set; } }
    private class UserRelationDto { public string? Id { get; set; } public int? UserId { get; set; } public int? NationId { get; set; } public int? RegionId { get; set; } public int? CityId { get; set; } }
    private class GroupMembershipDto { public string? Id { get; set; } public int? UserId { get; set; } public int? GroupId { get; set; } }

    public async Task InitializeAsync()
    {
        _testDataRoot = Path.Combine(Path.GetTempPath(), $"api-layer2-tests-{Guid.NewGuid()}");
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

    // ========== Clients ==========

    [Fact]
    public async Task Client_Create_ReturnsCreated()
    {
        var r = await _client.PostAsync("/api/clients", Json(new { LastName = "Rossi", FirstName = "Mario", Email = "mario@example.com", Phone = "0123456" }));
        Assert.Equal(HttpStatusCode.Created, r.StatusCode);
        var dto = await ReadAs<ClientDto>(r);
        Assert.NotNull(dto?.Id);
        Assert.Equal("Rossi", dto.LastName);
    }

    [Fact]
    public async Task Client_Create_WithPhoneAndEmailArrays_ReturnsArraysAndLegacyFields()
    {
        var create = await _client.PostAsync("/api/clients", Json(new
        {
            LastName = "Neri",
            FirstName = "Luca",
            Phones = new[] { "111", "222" },
            Emails = new[] { "first@example.com", "second@example.com" }
        }));

        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var dto = await ReadAs<ClientDto>(create);

        Assert.NotNull(dto?.Id);
        Assert.Equal(new[] { "111", "222" }, dto.Phones);
        Assert.Equal(new[] { "first@example.com", "second@example.com" }, dto.Emails);
        Assert.Equal("111", dto.Phone);
        Assert.Equal("222", dto.Phone2);
        Assert.Equal("first@example.com", dto.Email);
        Assert.Equal("second@example.com", dto.Email2);
    }

    [Fact]
    public async Task Client_Create_WithLegacyPhoneAndEmail_PopulatesArraysOnResponse()
    {
        var create = await _client.PostAsync("/api/clients", Json(new
        {
            LastName = "Legacy",
            Phone = "111",
            Phone2 = "222",
            Email = "first@example.com",
            Email2 = "second@example.com"
        }));

        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var dto = await ReadAs<ClientDto>(create);

        Assert.NotNull(dto?.Id);
        Assert.Equal(new[] { "111", "222" }, dto.Phones);
        Assert.Equal(new[] { "first@example.com", "second@example.com" }, dto.Emails);
    }

    [Fact]
    public async Task Client_Create_LastNameRequired_ReturnsBadRequest()
    {
        var r = await _client.PostAsync("/api/clients", Json(new { LastName = "", FirstName = "Mario" }));
        Assert.Equal(HttpStatusCode.BadRequest, r.StatusCode);
    }

    [Fact]
    public async Task Client_GetById_ReturnsOk()
    {
        var create = await _client.PostAsync("/api/clients", Json(new { LastName = "Ferrari", FirstName = "Luca" }));
        var created = await ReadAs<ClientDto>(create);
        var r = await _client.GetAsync($"/api/clients/{created!.Id}");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var dto = await ReadAs<ClientDto>(r);
        Assert.Equal("Ferrari", dto?.LastName);
    }

    [Fact]
    public async Task Client_List_ContainsCreated()
    {
        await _client.PostAsync("/api/clients", Json(new { LastName = "Bianchi" }));
        var r = await _client.GetAsync("/api/clients");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var list = await ReadAs<List<ClientDto>>(r);
        Assert.Contains(list!, x => x.LastName == "Bianchi");
    }

    [Fact]
    public async Task Client_Update_ChangesFields()
    {
        var create = await _client.PostAsync("/api/clients", Json(new { LastName = "Verdi", Phone = "111" }));
        var created = await ReadAs<ClientDto>(create);
        var r = await _client.PutAsync($"/api/clients/{created!.Id}", Json(new { LastName = "Verdi", Phone = "999" }));
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var dto = await ReadAs<ClientDto>(r);
        Assert.Equal("999", dto?.Phone);
    }

    [Fact]
    public async Task Client_List_CanFilterByLastName()
    {
        await _client.PostAsync("/api/clients", Json(new { LastName = "Rossi", FirstName = "Anna" }));
        await _client.PostAsync("/api/clients", Json(new { LastName = "Verdi", FirstName = "Luca" }));

        var r = await _client.GetAsync("/api/clients?lastName=ross");

        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var list = await ReadAs<List<ClientDto>>(r);
        Assert.Single(list!);
        Assert.Equal("Rossi", list[0].LastName);
    }

    [Fact]
    public async Task Client_List_ByStayRange_ReturnsGuestsArrivingStayingOrCheckedOut()
    {
        var arriving = await ReadAs<ClientDto>(await _client.PostAsync("/api/clients", Json(new { LastName = "ArrivingGuest" })));
        var staying = await ReadAs<ClientDto>(await _client.PostAsync("/api/clients", Json(new { LastName = "StayingGuest" })));
        var departed = await ReadAs<ClientDto>(await _client.PostAsync("/api/clients", Json(new { LastName = "DepartedGuest" })));
        var other = await ReadAs<ClientDto>(await _client.PostAsync("/api/clients", Json(new { LastName = "OtherGuest" })));

        await _client.PostAsync("/api/bookings", Json(new { Year = 2072, ClientId = arriving!.Id, ArrivalDate = "2072-04-10", DepartureDate = "2072-04-12", Status = "Confirmed" }));
        await _client.PostAsync("/api/bookings", Json(new { Year = 2072, ClientId = staying!.Id, ArrivalDate = "2072-04-08", DepartureDate = "2072-04-15", Status = "CheckedIn", ActualArrivalDate = "2072-04-08", ActualGuestCount = 1 }));
        await _client.PostAsync("/api/bookings", Json(new { Year = 2072, ClientId = departed!.Id, ArrivalDate = "2072-04-08", DepartureDate = "2072-04-10", Status = "CheckedOut", ActualArrivalDate = "2072-04-08", ActualGuestCount = 1, ActualDepartureDate = "2072-04-10" }));
        await _client.PostAsync("/api/bookings", Json(new { Year = 2072, ClientId = other!.Id, ArrivalDate = "2072-09-01", DepartureDate = "2072-09-03", Status = "Confirmed" }));

        var response = await _client.GetAsync("/api/clients?from=2072-04-10&to=2072-04-10");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var list = await ReadAs<List<ClientDto>>(response);
        Assert.Contains(list!, client => client.LastName == "ArrivingGuest");
        Assert.Contains(list!, client => client.LastName == "StayingGuest");
        Assert.Contains(list!, client => client.LastName == "DepartedGuest");
        Assert.DoesNotContain(list!, client => client.LastName == "OtherGuest");
    }

    [Fact]
    public async Task Client_Create_AndGet_RoundTripsExtendedFields()
    {
        var birthDate = new DateTime(1988, 4, 2, 0, 0, 0, DateTimeKind.Utc);
        var expiryDate = new DateTime(2031, 7, 14, 0, 0, 0, DateTimeKind.Utc);
        var create = await _client.PostAsync("/api/clients", Json(new
        {
            LastName = "Conti",
            FirstName = "Sara",
            DateOfBirth = birthDate,
            DocumentExpiry = expiryDate,
            DocumentType = "Passport",
            Phone3 = "+39-333-0003",
            Email3 = "pec@example.com",
            Nationality = "IT"
        }));
        var created = await ReadAs<ClientDto>(create);

        var r = await _client.GetAsync($"/api/clients/{created!.Id}");

        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var dto = await ReadAs<ClientDto>(r);
        Assert.Equal(birthDate, dto?.DateOfBirth);
        Assert.Equal(expiryDate, dto?.DocumentExpiry);
        Assert.Equal("Passport", dto?.DocumentType);
        Assert.Equal("+39-333-0003", dto?.Phone3);
        Assert.Equal("pec@example.com", dto?.Email3);
        Assert.Equal("IT", dto?.Nationality);
    }

    [Fact]
    public async Task Client_Update_PreservesCreatedAt_WhenOmitted()
    {
        var create = await _client.PostAsync("/api/clients", Json(new { LastName = "Audit", FirstName = "Trail" }));
        var created = await ReadAs<ClientDto>(create);
        Assert.NotNull(created);
        Assert.NotNull(created!.CreatedAt);

        var update = await _client.PutAsync($"/api/clients/{created.Id}", Json(new { LastName = "Audit", FirstName = "Trail Updated" }));

        Assert.Equal(HttpStatusCode.OK, update.StatusCode);
        var updated = await ReadAs<ClientDto>(update);
        Assert.Equal(created.CreatedAt, updated?.CreatedAt);
    }

    [Fact]
    public async Task Client_Delete_ReturnsNoContent()
    {
        var create = await _client.PostAsync("/api/clients", Json(new { LastName = "ToDelete" }));
        var created = await ReadAs<ClientDto>(create);
        var r = await _client.DeleteAsync($"/api/clients/{created!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, r.StatusCode);
        var get = await _client.GetAsync($"/api/clients/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, get.StatusCode);
    }

    // ========== UserPrivileges ==========

    [Fact]
    public async Task UserPrivilege_Create_ReturnsCreated()
    {
        var r = await _client.PostAsync("/api/user-privileges", Json(new { UserId = 10, Year = 2025, AllowedRules = "all", BookingInsertPriv = "yes" }));
        Assert.Equal(HttpStatusCode.Created, r.StatusCode);
        var dto = await ReadAs<UserPrivilegeDto>(r);
        Assert.NotNull(dto?.Id);
        Assert.Equal(10, dto.UserId);
        Assert.Equal(2025, dto.Year);
    }

    [Fact]
    public async Task UserPrivilege_GetByUserAndYear_ReturnsOk()
    {
        await _client.PostAsync("/api/user-privileges", Json(new { UserId = 20, Year = 2024, AllowedRules = "none" }));
        var r = await _client.GetAsync("/api/user-privileges/20/2024");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var dto = await ReadAs<UserPrivilegeDto>(r);
        Assert.Equal(20, dto?.UserId);
        Assert.Equal(2024, dto?.Year);
    }

    [Fact]
    public async Task UserPrivilege_ListByUser_FiltersCorrectly()
    {
        await _client.PostAsync("/api/user-privileges", Json(new { UserId = 30, Year = 2023 }));
        await _client.PostAsync("/api/user-privileges", Json(new { UserId = 30, Year = 2024 }));
        await _client.PostAsync("/api/user-privileges", Json(new { UserId = 99, Year = 2024 }));
        var r = await _client.GetAsync("/api/user-privileges?userId=30");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var list = await ReadAs<List<UserPrivilegeDto>>(r);
        Assert.All(list!, p => Assert.Equal(30, p.UserId));
        Assert.Equal(2, list!.Count);
    }

    [Fact]
    public async Task UserPrivilege_Delete_ReturnsNoContent()
    {
        await _client.PostAsync("/api/user-privileges", Json(new { UserId = 40, Year = 2025 }));
        var r = await _client.DeleteAsync("/api/user-privileges/40/2025");
        Assert.Equal(HttpStatusCode.NoContent, r.StatusCode);
        var get = await _client.GetAsync("/api/user-privileges/40/2025");
        Assert.Equal(HttpStatusCode.NotFound, get.StatusCode);
    }

    // ========== UserRelations ==========

    [Fact]
    public async Task UserRelation_Create_ReturnsCreated()
    {
        var r = await _client.PostAsync("/api/user-relations", Json(new { UserId = 100, NationId = 1, RegionId = 2, CityId = 3 }));
        Assert.Equal(HttpStatusCode.Created, r.StatusCode);
        var dto = await ReadAs<UserRelationDto>(r);
        Assert.NotNull(dto?.Id);
        Assert.Equal(100, dto.UserId);
    }

    [Fact]
    public async Task UserRelation_GetByUser_ReturnsOk()
    {
        await _client.PostAsync("/api/user-relations", Json(new { UserId = 200, NationId = 5 }));
        var r = await _client.GetAsync("/api/user-relations/200");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var dto = await ReadAs<UserRelationDto>(r);
        Assert.Equal(200, dto?.UserId);
        Assert.Equal(5, dto?.NationId);
    }

    [Fact]
    public async Task UserRelation_Update_ChangesNation()
    {
        await _client.PostAsync("/api/user-relations", Json(new { UserId = 300, NationId = 1 }));
        var r = await _client.PutAsync("/api/user-relations/300", Json(new { UserId = 300, NationId = 7 }));
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var dto = await ReadAs<UserRelationDto>(r);
        Assert.Equal(7, dto?.NationId);
    }

    [Fact]
    public async Task UserRelation_Delete_ReturnsNoContent()
    {
        await _client.PostAsync("/api/user-relations", Json(new { UserId = 400 }));
        var r = await _client.DeleteAsync("/api/user-relations/400");
        Assert.Equal(HttpStatusCode.NoContent, r.StatusCode);
    }

    // ========== GroupMemberships ==========

    [Fact]
    public async Task GroupMembership_Create_ReturnsCreated()
    {
        var r = await _client.PostAsync("/api/group-memberships", Json(new { UserId = 1000, GroupId = 5 }));
        Assert.Equal(HttpStatusCode.Created, r.StatusCode);
        var dto = await ReadAs<GroupMembershipDto>(r);
        Assert.NotNull(dto?.Id);
        Assert.Equal(1000, dto.UserId);
        Assert.Equal(5, dto.GroupId);
    }

    [Fact]
    public async Task GroupMembership_ListByUser_FiltersCorrectly()
    {
        await _client.PostAsync("/api/group-memberships", Json(new { UserId = 2000, GroupId = 1 }));
        await _client.PostAsync("/api/group-memberships", Json(new { UserId = 2000, GroupId = 2 }));
        await _client.PostAsync("/api/group-memberships", Json(new { UserId = 9999, GroupId = 1 }));
        var r = await _client.GetAsync("/api/group-memberships?userId=2000");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var list = await ReadAs<List<GroupMembershipDto>>(r);
        Assert.All(list!, m => Assert.Equal(2000, m.UserId));
        Assert.Equal(2, list!.Count);
    }

    [Fact]
    public async Task GroupMembership_Delete_ReturnsNoContent()
    {
        await _client.PostAsync("/api/group-memberships", Json(new { UserId = 3000, GroupId = 10 }));
        var r = await _client.DeleteAsync("/api/group-memberships/3000/10");
        Assert.Equal(HttpStatusCode.NoContent, r.StatusCode);
    }
}

