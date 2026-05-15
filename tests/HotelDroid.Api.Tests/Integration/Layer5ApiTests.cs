using System.Net;
using System.Text;
using System.Text.Json;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using HotelDroid.Api;

namespace HotelDroid.Api.Tests.Integration;

[Collection("Sequential")]
public class Layer5ApiTests : IAsyncLifetime
{
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;
    private string _testDataRoot = null!;
    private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

    private class MessageDto { public string? Id { get; set; } public string? MessageType { get; set; } public string? Status { get; set; } public string? Sender { get; set; } public string? Body { get; set; } public string? RecipientUserIds { get; set; } public string? CreatedAt { get; set; } public string? SeenAt { get; set; } }
    private class ContractTemplateDto { public string? Id { get; set; } public string? Type { get; set; } public int Number { get; set; } public string? Content { get; set; } }
    private class ExternalIntegrationDto { public string? Id { get; set; } public string? IntegrationName { get; set; } public string? IdType { get; set; } public int? LocalId { get; set; } public string? RemoteId1 { get; set; } public int? Year { get; set; } public string? CreatedAt { get; set; } }
    private class SessionDto { public string? SessionId { get; set; } public int? UserId { get; set; } public string? IpAddress { get; set; } public string? ConnectionType { get; set; } public string? LastAccess { get; set; } }

    public async Task InitializeAsync()
    {
        _testDataRoot = Path.Combine(Path.GetTempPath(), $"api-layer5-tests-{Guid.NewGuid()}");
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

    // ========== Messages ==========

    [Fact]
    public async Task Message_Create_ReturnsCreated()
    {
        var r = await _client.PostAsync("/api/messages", Json(new { Sender = "alice", Body = "Hello world", MessageType = "internal" }));
        Assert.Equal(HttpStatusCode.Created, r.StatusCode);
        var dto = await ReadAs<MessageDto>(r);
        Assert.NotNull(dto?.Id);
        Assert.Equal("alice", dto.Sender);
        Assert.Equal("Hello world", dto.Body);
        Assert.Equal("unread", dto.Status);
    }

    [Fact]
    public async Task Message_Create_MissingSender_ReturnsBadRequest()
    {
        var r = await _client.PostAsync("/api/messages", Json(new { Body = "No sender" }));
        Assert.Equal(HttpStatusCode.BadRequest, r.StatusCode);
    }

    [Fact]
    public async Task Message_GetById_ReturnsOk()
    {
        var create = await _client.PostAsync("/api/messages", Json(new { Sender = "bob", Body = "Test message" }));
        var created = await ReadAs<MessageDto>(create);
        var r = await _client.GetAsync($"/api/messages/{created!.Id}");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var dto = await ReadAs<MessageDto>(r);
        Assert.Equal("bob", dto?.Sender);
    }

    [Fact]
    public async Task Message_List_FilteredByStatus()
    {
        await _client.PostAsync("/api/messages", Json(new { Sender = "s1", Body = "msg1" }));
        await _client.PostAsync("/api/messages", Json(new { Sender = "s2", Body = "msg2" }));
        var r = await _client.GetAsync("/api/messages?status=unread");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var list = await ReadAs<List<MessageDto>>(r);
        Assert.All(list!, m => Assert.Equal("unread", m.Status));
    }

    [Fact]
    public async Task Message_MarkRead_UpdatesStatus()
    {
        var create = await _client.PostAsync("/api/messages", Json(new { Sender = "carol", Body = "Read me" }));
        var created = await ReadAs<MessageDto>(create);
        var r = await _client.PutAsync($"/api/messages/{created!.Id}/read", null);
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var dto = await ReadAs<MessageDto>(r);
        Assert.Equal("read", dto?.Status);
        Assert.NotNull(dto?.SeenAt);
    }

    [Fact]
    public async Task Message_Archive_UpdatesStatus()
    {
        var create = await _client.PostAsync("/api/messages", Json(new { Sender = "dave", Body = "Archive me" }));
        var created = await ReadAs<MessageDto>(create);
        var r = await _client.PutAsync($"/api/messages/{created!.Id}/archive", null);
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var dto = await ReadAs<MessageDto>(r);
        Assert.Equal("archived", dto?.Status);
    }

    [Fact]
    public async Task Message_Delete_ReturnsNoContent()
    {
        var create = await _client.PostAsync("/api/messages", Json(new { Sender = "eve", Body = "Delete me" }));
        var created = await ReadAs<MessageDto>(create);
        var r = await _client.DeleteAsync($"/api/messages/{created!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, r.StatusCode);
        var get = await _client.GetAsync($"/api/messages/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, get.StatusCode);
    }

    // ========== Contract Templates ==========

    [Fact]
    public async Task ContractTemplate_Create_ReturnsCreated()
    {
        var r = await _client.PostAsync("/api/contract-templates", Json(new { Type = "standard", Content = "This is a standard contract." }));
        Assert.Equal(HttpStatusCode.Created, r.StatusCode);
        var dto = await ReadAs<ContractTemplateDto>(r);
        Assert.NotNull(dto?.Id);
        Assert.Equal("standard", dto.Type);
        Assert.Equal(1, dto.Number);
        Assert.Equal("This is a standard contract.", dto.Content);
    }

    [Fact]
    public async Task ContractTemplate_Create_AutoIncrementsNumber()
    {
        await _client.PostAsync("/api/contract-templates", Json(new { Type = "premium", Content = "Premium v1" }));
        var r2 = await _client.PostAsync("/api/contract-templates", Json(new { Type = "premium", Content = "Premium v2" }));
        Assert.Equal(HttpStatusCode.Created, r2.StatusCode);
        var dto2 = await ReadAs<ContractTemplateDto>(r2);
        Assert.Equal(2, dto2?.Number);
    }

    [Fact]
    public async Task ContractTemplate_GetByTypeAndNumber_ReturnsOk()
    {
        var create = await _client.PostAsync("/api/contract-templates", Json(new { Type = "basic", Content = "Basic contract" }));
        var created = await ReadAs<ContractTemplateDto>(create);
        var r = await _client.GetAsync($"/api/contract-templates/{created!.Type}/{created.Number}");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var dto = await ReadAs<ContractTemplateDto>(r);
        Assert.Equal("Basic contract", dto?.Content);
    }

    [Fact]
    public async Task ContractTemplate_List_FilteredByType()
    {
        await _client.PostAsync("/api/contract-templates", Json(new { Type = "typeA", Content = "A1" }));
        await _client.PostAsync("/api/contract-templates", Json(new { Type = "typeB", Content = "B1" }));
        var r = await _client.GetAsync("/api/contract-templates?type=typeA");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var list = await ReadAs<List<ContractTemplateDto>>(r);
        Assert.All(list!, ct => Assert.Equal("typeA", ct.Type));
    }

    [Fact]
    public async Task ContractTemplate_Update_ChangesContent()
    {
        var create = await _client.PostAsync("/api/contract-templates", Json(new { Type = "updatable", Content = "Old content" }));
        var created = await ReadAs<ContractTemplateDto>(create);
        var r = await _client.PutAsync($"/api/contract-templates/{created!.Type}/{created.Number}", Json(new { Type = "updatable", Content = "New content" }));
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var dto = await ReadAs<ContractTemplateDto>(r);
        Assert.Equal("New content", dto?.Content);
    }

    [Fact]
    public async Task ContractTemplate_Delete_ReturnsNoContent()
    {
        var create = await _client.PostAsync("/api/contract-templates", Json(new { Type = "deletable", Content = "To be deleted" }));
        var created = await ReadAs<ContractTemplateDto>(create);
        var r = await _client.DeleteAsync($"/api/contract-templates/{created!.Type}/{created.Number}");
        Assert.Equal(HttpStatusCode.NoContent, r.StatusCode);
        var get = await _client.GetAsync($"/api/contract-templates/{created.Type}/{created.Number}");
        Assert.Equal(HttpStatusCode.NotFound, get.StatusCode);
    }

    // ========== External Integrations ==========

    [Fact]
    public async Task ExternalIntegration_Create_ReturnsCreated()
    {
        var r = await _client.PostAsync("/api/external-integrations", Json(new { IntegrationName = "Booking.com", Year = 2024 }));
        Assert.Equal(HttpStatusCode.Created, r.StatusCode);
        var dto = await ReadAs<ExternalIntegrationDto>(r);
        Assert.NotNull(dto?.Id);
        Assert.Equal("Booking.com", dto.IntegrationName);
        Assert.Equal(2024, dto.Year);
    }

    [Fact]
    public async Task ExternalIntegration_Create_MissingName_ReturnsBadRequest()
    {
        var r = await _client.PostAsync("/api/external-integrations", Json(new { Year = 2024 }));
        Assert.Equal(HttpStatusCode.BadRequest, r.StatusCode);
    }

    [Fact]
    public async Task ExternalIntegration_GetById_ReturnsOk()
    {
        var create = await _client.PostAsync("/api/external-integrations", Json(new { IntegrationName = "Expedia", Year = 2023 }));
        var created = await ReadAs<ExternalIntegrationDto>(create);
        var r = await _client.GetAsync($"/api/external-integrations/{created!.Id}");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var dto = await ReadAs<ExternalIntegrationDto>(r);
        Assert.Equal("Expedia", dto?.IntegrationName);
    }

    [Fact]
    public async Task ExternalIntegration_List_FilteredByYear()
    {
        await _client.PostAsync("/api/external-integrations", Json(new { IntegrationName = "IntA", Year = 2050 }));
        await _client.PostAsync("/api/external-integrations", Json(new { IntegrationName = "IntB", Year = 2051 }));
        var r = await _client.GetAsync("/api/external-integrations?year=2050");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var list = await ReadAs<List<ExternalIntegrationDto>>(r);
        Assert.All(list!, ei => Assert.Equal(2050, ei.Year));
    }

    [Fact]
    public async Task ExternalIntegration_Update_ChangesName()
    {
        var create = await _client.PostAsync("/api/external-integrations", Json(new { IntegrationName = "OldName", Year = 2024 }));
        var created = await ReadAs<ExternalIntegrationDto>(create);
        var r = await _client.PutAsync($"/api/external-integrations/{created!.Id}", Json(new { IntegrationName = "NewName", Year = 2024 }));
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var dto = await ReadAs<ExternalIntegrationDto>(r);
        Assert.Equal("NewName", dto?.IntegrationName);
    }

    [Fact]
    public async Task ExternalIntegration_Delete_ReturnsNoContent()
    {
        var create = await _client.PostAsync("/api/external-integrations", Json(new { IntegrationName = "ToDelete", Year = 2024 }));
        var created = await ReadAs<ExternalIntegrationDto>(create);
        var r = await _client.DeleteAsync($"/api/external-integrations/{created!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, r.StatusCode);
        var get = await _client.GetAsync($"/api/external-integrations/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, get.StatusCode);
    }

    // ========== Sessions ==========

    [Fact]
    public async Task Session_Create_ReturnsCreated()
    {
        var r = await _client.PostAsync("/api/sessions", Json(new { SessionId = "sess_abc123", UserId = 42, IpAddress = "192.168.1.1" }));
        Assert.Equal(HttpStatusCode.Created, r.StatusCode);
        var dto = await ReadAs<SessionDto>(r);
        Assert.NotNull(dto?.SessionId);
        Assert.Equal(42, dto.UserId);
        Assert.Equal("192.168.1.1", dto.IpAddress);
    }

    [Fact]
    public async Task Session_Create_MissingUserId_ReturnsBadRequest()
    {
        var r = await _client.PostAsync("/api/sessions", Json(new { SessionId = "sess_nouser" }));
        Assert.Equal(HttpStatusCode.BadRequest, r.StatusCode);
    }

    [Fact]
    public async Task Session_GetBySessionId_ReturnsOk()
    {
        var create = await _client.PostAsync("/api/sessions", Json(new { SessionId = "sess_get_test", UserId = 10 }));
        var created = await ReadAs<SessionDto>(create);
        var r = await _client.GetAsync($"/api/sessions/{created!.SessionId}");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var dto = await ReadAs<SessionDto>(r);
        Assert.Equal(10, dto?.UserId);
    }

    [Fact]
    public async Task Session_List_FilteredByUserId()
    {
        await _client.PostAsync("/api/sessions", Json(new { SessionId = "sess_u99_a", UserId = 99 }));
        await _client.PostAsync("/api/sessions", Json(new { SessionId = "sess_u100_a", UserId = 100 }));
        var r = await _client.GetAsync("/api/sessions?userId=99");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var list = await ReadAs<List<SessionDto>>(r);
        Assert.All(list!, s => Assert.Equal(99, s.UserId));
    }

    [Fact]
    public async Task Session_Touch_UpdatesLastAccess()
    {
        await _client.PostAsync("/api/sessions", Json(new { SessionId = "sess_touch_me", UserId = 7 }));
        await Task.Delay(10);
        var r = await _client.PutAsync("/api/sessions/sess_touch_me/touch", null);
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var dto = await ReadAs<SessionDto>(r);
        Assert.NotNull(dto?.LastAccess);
    }

    [Fact]
    public async Task Session_Delete_ReturnsNoContent()
    {
        await _client.PostAsync("/api/sessions", Json(new { SessionId = "sess_del_me", UserId = 5 }));
        var r = await _client.DeleteAsync("/api/sessions/sess_del_me");
        Assert.Equal(HttpStatusCode.NoContent, r.StatusCode);
        var get = await _client.GetAsync("/api/sessions/sess_del_me");
        Assert.Equal(HttpStatusCode.NotFound, get.StatusCode);
    }

    [Fact]
    public async Task Session_DeleteForUser_RemovesAllUserSessions()
    {
        await _client.PostAsync("/api/sessions", Json(new { SessionId = "sess_u77_1", UserId = 77 }));
        await _client.PostAsync("/api/sessions", Json(new { SessionId = "sess_u77_2", UserId = 77 }));
        await _client.PostAsync("/api/sessions", Json(new { SessionId = "sess_u78_1", UserId = 78 }));
        var r = await _client.DeleteAsync("/api/sessions?userId=77");
        Assert.Equal(HttpStatusCode.NoContent, r.StatusCode);
        var remaining = await _client.GetAsync("/api/sessions?userId=77");
        var list = await ReadAs<List<SessionDto>>(remaining);
        Assert.Empty(list!);
        var others = await _client.GetAsync("/api/sessions?userId=78");
        var otherList = await ReadAs<List<SessionDto>>(others);
        Assert.NotEmpty(otherList!);
    }
}
