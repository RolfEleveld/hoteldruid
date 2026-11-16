using System.Net;
using System.Text;
using System.Text.Json;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using HotelDruid.Api;

namespace HotelDruid.Api.Tests.Integration;

[Collection("Sequential")]
public class Layer5ApiTests : IAsyncLifetime
{
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;
    private string _testDataRoot = null!;
    private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

    private class MessageDto { public string? Id { get; set; } public string? MessageType { get; set; } public string? Status { get; set; } public string? Sender { get; set; } public string? Body { get; set; } public string? RecipientUserIds { get; set; } public string? CreatedAt { get; set; } public string? SeenAt { get; set; } }
    private class ContractTemplateDto { public string? Id { get; set; } public string? Type { get; set; } public int Number { get; set; } public string? Content { get; set; } }

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
}

