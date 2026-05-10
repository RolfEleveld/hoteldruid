using System.Net;
using System.Text;
using System.Text.Json;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using HotelDroid.Api;

namespace HotelDroid.Api.Tests.Integration;

[Collection("Sequential")]
public class Layer1ApiTests : IAsyncLifetime
{
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;
    private string _testDataRoot = null!;

    private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

    private class CashRegisterDto { public string? Id { get; set; } public string? Name { get; set; } public string? Status { get; set; } public string? Code { get; set; } public string? Description { get; set; } public DateTime? CreatedAt { get; set; } }
    private class UserGroupDto { public string? Id { get; set; } public string? Name { get; set; } }
    private class UserDto { public string? Id { get; set; } public string? Username { get; set; } public string? PasswordType { get; set; } public DateTime? CreatedAt { get; set; } }
    private class SettingDto { public string? Key { get; set; } public int? UserId { get; set; } public string? StringValue { get; set; } public int? NumericValue { get; set; } }

    public async Task InitializeAsync()
    {
        _testDataRoot = Path.Combine(Path.GetTempPath(), $"api-layer1-tests-{Guid.NewGuid()}");
        Directory.CreateDirectory(_testDataRoot);

        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => builder.UseSetting("DataRoot", _testDataRoot));

        _client = _factory.CreateClient();
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        _client?.Dispose();
        _factory?.Dispose();
        if (Directory.Exists(_testDataRoot))
            try { Directory.Delete(_testDataRoot, recursive: true); } catch { }
        await Task.CompletedTask;
    }

    private StringContent Json(object o) =>
        new(JsonSerializer.Serialize(o), Encoding.UTF8, "application/json");

    private async Task<T?> ReadAs<T>(HttpResponseMessage r) =>
        JsonSerializer.Deserialize<T>(await r.Content.ReadAsStringAsync(), _json);

    // =========================================================
    // Cash Registers
    // =========================================================

    [Fact]
    public async Task CashRegister_Create_ReturnsCreated()
    {
        var r = await _client.PostAsync("/api/cash-registers",
            Json(new { Name = "Main Till", Status = "open", Code = "CR01", Description = "Reception till" }));
        Assert.Equal(HttpStatusCode.Created, r.StatusCode);
        var dto = await ReadAs<CashRegisterDto>(r);
        Assert.NotNull(dto?.Id);
        Assert.Equal("Main Till", dto.Name);
        Assert.Equal("open", dto.Status);
    }

    [Fact]
    public async Task CashRegister_Get_ReturnsOk()
    {
        var create = await _client.PostAsync("/api/cash-registers",
            Json(new { Name = "Till Get Test", Status = "open" }));
        var created = await ReadAs<CashRegisterDto>(create);

        var r = await _client.GetAsync($"/api/cash-registers/{created!.Id}");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var dto = await ReadAs<CashRegisterDto>(r);
        Assert.Equal(created.Id, dto?.Id);
        Assert.Equal("Till Get Test", dto?.Name);
    }

    [Fact]
    public async Task CashRegister_List_ContainsCreated()
    {
        await _client.PostAsync("/api/cash-registers",
            Json(new { Name = "Till List A", Status = "open" }));

        var r = await _client.GetAsync("/api/cash-registers");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var list = await ReadAs<List<CashRegisterDto>>(r);
        Assert.Contains(list!, x => x.Name == "Till List A");
    }

    [Fact]
    public async Task CashRegister_Update_ChangesStatus()
    {
        var create = await _client.PostAsync("/api/cash-registers",
            Json(new { Name = "Till Update", Status = "open" }));
        var created = await ReadAs<CashRegisterDto>(create);

        var r = await _client.PutAsync($"/api/cash-registers/{created!.Id}",
            Json(new { Name = "Till Update", Status = "closed" }));
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var dto = await ReadAs<CashRegisterDto>(r);
        Assert.Equal("closed", dto?.Status);
    }

    [Fact]
    public async Task CashRegister_Delete_ReturnsNoContent()
    {
        var create = await _client.PostAsync("/api/cash-registers",
            Json(new { Name = "Till Delete", Status = "open" }));
        var created = await ReadAs<CashRegisterDto>(create);

        var r = await _client.DeleteAsync($"/api/cash-registers/{created!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, r.StatusCode);

        var get = await _client.GetAsync($"/api/cash-registers/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, get.StatusCode);
    }

    [Fact]
    public async Task CashRegister_Create_ValidationError_ReturnsBadRequest()
    {
        var r = await _client.PostAsync("/api/cash-registers",
            Json(new { Name = "" }));
        Assert.Equal(HttpStatusCode.BadRequest, r.StatusCode);
    }

    [Fact]
    public async Task CashRegister_Create_Duplicate_ReturnsConflict()
    {
        await _client.PostAsync("/api/cash-registers",
            Json(new { Name = "Till Dup", Status = "open" }));
        var r = await _client.PostAsync("/api/cash-registers",
            Json(new { Name = "Till Dup", Status = "closed" }));
        Assert.Equal(HttpStatusCode.Conflict, r.StatusCode);
    }

    // =========================================================
    // User Groups
    // =========================================================

    [Fact]
    public async Task UserGroup_Create_ReturnsCreated()
    {
        var r = await _client.PostAsync("/api/user-groups",
            Json(new { Name = "Admins" }));
        Assert.Equal(HttpStatusCode.Created, r.StatusCode);
        var dto = await ReadAs<UserGroupDto>(r);
        Assert.NotNull(dto?.Id);
        Assert.Equal("Admins", dto.Name);
    }

    [Fact]
    public async Task UserGroup_Get_ReturnsOk()
    {
        var create = await _client.PostAsync("/api/user-groups",
            Json(new { Name = "Receptionists" }));
        var created = await ReadAs<UserGroupDto>(create);

        var r = await _client.GetAsync($"/api/user-groups/{created!.Id}");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var dto = await ReadAs<UserGroupDto>(r);
        Assert.Equal("Receptionists", dto?.Name);
    }

    [Fact]
    public async Task UserGroup_List_ContainsCreated()
    {
        await _client.PostAsync("/api/user-groups",
            Json(new { Name = "Housekeeping" }));
        var r = await _client.GetAsync("/api/user-groups");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var list = await ReadAs<List<UserGroupDto>>(r);
        Assert.Contains(list!, x => x.Name == "Housekeeping");
    }

    [Fact]
    public async Task UserGroup_Update_ChangesName()
    {
        var create = await _client.PostAsync("/api/user-groups",
            Json(new { Name = "OldGroup" }));
        var created = await ReadAs<UserGroupDto>(create);

        var r = await _client.PutAsync($"/api/user-groups/{created!.Id}",
            Json(new { Name = "NewGroup" }));
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var dto = await ReadAs<UserGroupDto>(r);
        Assert.Equal("NewGroup", dto?.Name);
    }

    [Fact]
    public async Task UserGroup_Delete_ReturnsNoContent()
    {
        var create = await _client.PostAsync("/api/user-groups",
            Json(new { Name = "ToDeleteGroup" }));
        var created = await ReadAs<UserGroupDto>(create);

        var r = await _client.DeleteAsync($"/api/user-groups/{created!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, r.StatusCode);
    }

    [Fact]
    public async Task UserGroup_Create_EmptyName_ReturnsBadRequest()
    {
        var r = await _client.PostAsync("/api/user-groups",
            Json(new { Name = "" }));
        Assert.Equal(HttpStatusCode.BadRequest, r.StatusCode);
    }

    // =========================================================
    // Users
    // =========================================================

    [Fact]
    public async Task User_Create_ReturnsCreated_WithoutPassword()
    {
        var r = await _client.PostAsync("/api/users",
            Json(new { Username = "alice", Password = "secret123", PasswordType = "n" }));
        Assert.Equal(HttpStatusCode.Created, r.StatusCode);
        var dto = await ReadAs<UserDto>(r);
        Assert.NotNull(dto?.Id);
        Assert.Equal("alice", dto.Username);
        // Password must never be in the response JSON
        var raw = await r.Content.ReadAsStringAsync();
        Assert.DoesNotContain("secret123", raw, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("PasswordHash", raw, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Salt", raw, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task User_Get_DoesNotReturnPassword()
    {
        var create = await _client.PostAsync("/api/users",
            Json(new { Username = "bob", Password = "p@ssword", PasswordType = "n" }));
        var created = await ReadAs<UserDto>(create);

        var r = await _client.GetAsync($"/api/users/{created!.Id}");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var raw = await r.Content.ReadAsStringAsync();
        Assert.DoesNotContain("PasswordHash", raw, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Salt", raw, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task User_List_DoesNotReturnPasswords()
    {
        await _client.PostAsync("/api/users",
            Json(new { Username = "charlie", Password = "pwd", PasswordType = "n" }));
        var r = await _client.GetAsync("/api/users");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var raw = await r.Content.ReadAsStringAsync();
        Assert.DoesNotContain("PasswordHash", raw, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Salt", raw, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task User_ChangePassword_ReturnsOk()
    {
        var create = await _client.PostAsync("/api/users",
            Json(new { Username = "dave", Password = "oldpass", PasswordType = "n" }));
        var created = await ReadAs<UserDto>(create);

        var r = await _client.PostAsync($"/api/users/{created!.Id}/change-password",
            Json(new { Password = "newpass456" }));
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
    }

    [Fact]
    public async Task User_Create_EmptyUsername_ReturnsBadRequest()
    {
        var r = await _client.PostAsync("/api/users",
            Json(new { Username = "", Password = "abc" }));
        Assert.Equal(HttpStatusCode.BadRequest, r.StatusCode);
    }

    [Fact]
    public async Task User_Delete_ReturnsNoContent()
    {
        var create = await _client.PostAsync("/api/users",
            Json(new { Username = "eve", Password = "x" }));
        var created = await ReadAs<UserDto>(create);

        var r = await _client.DeleteAsync($"/api/users/{created!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, r.StatusCode);
    }

    // =========================================================
    // Settings
    // =========================================================

    [Fact]
    public async Task Setting_Upsert_Create_ReturnsCreated()
    {
        var r = await _client.PostAsync("/api/settings",
            Json(new { Key = "theme", UserId = 1, StringValue = "dark", NumericValue = (int?)null }));
        Assert.True(r.StatusCode == HttpStatusCode.Created || r.StatusCode == HttpStatusCode.OK);
        var dto = await ReadAs<SettingDto>(r);
        Assert.Equal("theme", dto?.Key);
        Assert.Equal("dark", dto?.StringValue);
    }

    [Fact]
    public async Task Setting_GetByUserAndKey_ReturnsOk()
    {
        await _client.PostAsync("/api/settings",
            Json(new { Key = "lang", UserId = 2, StringValue = "en" }));

        var r = await _client.GetAsync("/api/settings/2/lang");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var dto = await ReadAs<SettingDto>(r);
        Assert.Equal("lang", dto?.Key);
        Assert.Equal(2, dto?.UserId);
        Assert.Equal("en", dto?.StringValue);
    }

    [Fact]
    public async Task Setting_ListByUser_FiltersCorrectly()
    {
        await _client.PostAsync("/api/settings",
            Json(new { Key = "color", UserId = 3, StringValue = "blue" }));
        await _client.PostAsync("/api/settings",
            Json(new { Key = "font", UserId = 3, StringValue = "Arial" }));
        await _client.PostAsync("/api/settings",
            Json(new { Key = "size", UserId = 99, StringValue = "large" }));

        var r = await _client.GetAsync("/api/settings?userId=3");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var list = await ReadAs<List<SettingDto>>(r);
        Assert.All(list!, s => Assert.Equal(3, s.UserId));
        Assert.Contains(list!, s => s.Key == "color");
        Assert.Contains(list!, s => s.Key == "font");
        Assert.DoesNotContain(list!, s => s.UserId == 99);
    }

    [Fact]
    public async Task Setting_Update_ChangesValue()
    {
        await _client.PostAsync("/api/settings",
            Json(new { Key = "page_size", UserId = 4, NumericValue = 10 }));

        var r = await _client.PutAsync("/api/settings/4/page_size",
            Json(new { Key = "page_size", UserId = 4, NumericValue = 25 }));
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var dto = await ReadAs<SettingDto>(r);
        Assert.Equal(25, dto?.NumericValue);
    }

    [Fact]
    public async Task Setting_Delete_ReturnsNoContent()
    {
        await _client.PostAsync("/api/settings",
            Json(new { Key = "to_delete", UserId = 5, StringValue = "bye" }));

        var r = await _client.DeleteAsync("/api/settings/5/to_delete");
        Assert.Equal(HttpStatusCode.NoContent, r.StatusCode);

        var get = await _client.GetAsync("/api/settings/5/to_delete");
        Assert.Equal(HttpStatusCode.NotFound, get.StatusCode);
    }

    [Fact]
    public async Task Setting_Create_EmptyKey_ReturnsBadRequest()
    {
        var r = await _client.PostAsync("/api/settings",
            Json(new { Key = "", UserId = 1, StringValue = "x" }));
        Assert.Equal(HttpStatusCode.BadRequest, r.StatusCode);
    }
}
