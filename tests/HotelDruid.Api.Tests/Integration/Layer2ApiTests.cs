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

    private class ClientDto { public string? Id { get; set; } public string? LastName { get; set; } public string? FirstName { get; set; } public string? Email { get; set; } public string? Phone { get; set; } public string? City { get; set; } }
    private class ClientDataDto { public string? Id { get; set; } public string? ClientId { get; set; } public int? Number { get; set; } public string? Type { get; set; } public string? Text1 { get; set; } }
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
    public async Task Client_Delete_ReturnsNoContent()
    {
        var create = await _client.PostAsync("/api/clients", Json(new { LastName = "ToDelete" }));
        var created = await ReadAs<ClientDto>(create);
        var r = await _client.DeleteAsync($"/api/clients/{created!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, r.StatusCode);
        var get = await _client.GetAsync($"/api/clients/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, get.StatusCode);
    }

    // ========== ClientData ==========

    [Fact]
    public async Task ClientData_Create_ReturnsCreated()
    {
        var r = await _client.PostAsync("/api/client-data", Json(new { ClientId = "client1", Number = 1, Type = "telefono", Text1 = "0123456789" }));
        Assert.Equal(HttpStatusCode.Created, r.StatusCode);
        var dto = await ReadAs<ClientDataDto>(r);
        Assert.NotNull(dto?.Id);
        Assert.Equal("client1", dto.ClientId);
    }

    [Fact]
    public async Task ClientData_Create_ClientIdRequired_ReturnsBadRequest()
    {
        var r = await _client.PostAsync("/api/client-data", Json(new { ClientId = "", Number = 1, Type = "nota" }));
        Assert.Equal(HttpStatusCode.BadRequest, r.StatusCode);
    }

    [Fact]
    public async Task ClientData_GetByClientId_ReturnsList()
    {
        await _client.PostAsync("/api/client-data", Json(new { ClientId = "clientXYZ", Number = 1, Type = "email", Text1 = "a@b.com" }));
        await _client.PostAsync("/api/client-data", Json(new { ClientId = "clientXYZ", Number = 2, Type = "nota", Text1 = "Some note" }));
        var r = await _client.GetAsync("/api/client-data?clientId=clientXYZ");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var list = await ReadAs<List<ClientDataDto>>(r);
        Assert.Equal(2, list?.Count);
    }

    [Fact]
    public async Task ClientData_Update_ChangesText()
    {
        var create = await _client.PostAsync("/api/client-data", Json(new { ClientId = "clientUpd", Number = 1, Type = "nota", Text1 = "old" }));
        var created = await ReadAs<ClientDataDto>(create);
        var r = await _client.PutAsync($"/api/client-data/{created!.Id}", Json(new { ClientId = "clientUpd", Number = 1, Type = "nota", Text1 = "new" }));
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var dto = await ReadAs<ClientDataDto>(r);
        Assert.Equal("new", dto?.Text1);
    }

    [Fact]
    public async Task ClientData_Delete_ReturnsNoContent()
    {
        var create = await _client.PostAsync("/api/client-data", Json(new { ClientId = "clientDel", Number = 1, Type = "nota" }));
        var created = await ReadAs<ClientDataDto>(create);
        var r = await _client.DeleteAsync($"/api/client-data/{created!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, r.StatusCode);
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

