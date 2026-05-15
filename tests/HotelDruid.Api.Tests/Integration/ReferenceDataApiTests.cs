using System.Net;
using System.Text;
using System.Text.Json;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using HotelDruid.Api;

namespace HotelDruid.Api.Tests.Integration;

[Collection("Sequential")]
public class ReferenceDataApiTests : IAsyncLifetime
{
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;
    private string _testDataRoot = null!;

    private class RefDto
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public string? Code2 { get; set; }
        public string? Code3 { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public async Task InitializeAsync()
    {
        _testDataRoot = Path.Combine(Path.GetTempPath(), $"api-refdata-tests-{Guid.NewGuid()}");
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
        {
            try { Directory.Delete(_testDataRoot, recursive: true); } catch { }
        }

        await Task.CompletedTask;
    }

    private async Task ClearCollectionAsync(string path)
    {
        var response = await _client.GetAsync(path);
        if (!response.IsSuccessStatusCode) return;
        var content = await response.Content.ReadAsStringAsync();
        var list = JsonSerializer.Deserialize<List<JsonElement>>(content);
        if (list == null) return;
        foreach (var item in list)
        {
            if (item.TryGetProperty("Id", out var idProp) || item.TryGetProperty("id", out idProp))
            {
                var id = idProp.GetString();
                if (!string.IsNullOrEmpty(id))
                    await _client.DeleteAsync($"{path}/{id}");
            }
        }
    }

    private StringContent Json(object obj) =>
        new(JsonSerializer.Serialize(obj), Encoding.UTF8, "application/json");

    // ─── Nations ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task Nation_Lifecycle_CreateGetListUpdateDelete()
    {
        await ClearCollectionAsync("/api/nations");

        var dto = new RefDto { Name = "TestNation", Code = "TN", Code2 = "TN2", Code3 = "TN3" };
        var createResp = await _client.PostAsync("/api/nations", Json(dto));
        Assert.Equal(HttpStatusCode.Created, createResp.StatusCode);
        var created = JsonSerializer.Deserialize<RefDto>(await createResp.Content.ReadAsStringAsync());
        Assert.NotNull(created);
        Assert.False(string.IsNullOrEmpty(created!.Id));

        // GetById
        var getResp = await _client.GetAsync($"/api/nations/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, getResp.StatusCode);

        // List
        var listResp = await _client.GetAsync("/api/nations");
        Assert.Equal(HttpStatusCode.OK, listResp.StatusCode);
        var list = JsonSerializer.Deserialize<List<RefDto>>(await listResp.Content.ReadAsStringAsync());
        Assert.NotNull(list);
        Assert.Contains(list!, n => n.Id == created.Id);

        // Update
        var updated = new RefDto { Name = "TestNation-Updated", Code = "TNU" };
        var putResp = await _client.PutAsync($"/api/nations/{created.Id}", Json(updated));
        Assert.Equal(HttpStatusCode.OK, putResp.StatusCode);
        var after = JsonSerializer.Deserialize<RefDto>(await putResp.Content.ReadAsStringAsync());
        Assert.Equal("TestNation-Updated", after?.Name);

        // Delete
        var delResp = await _client.DeleteAsync($"/api/nations/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, delResp.StatusCode);

        var getAfter = await _client.GetAsync($"/api/nations/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getAfter.StatusCode);
    }

    [Fact]
    public async Task Nation_Create_RequiresName()
    {
        var dto = new RefDto { Name = "" };
        var resp = await _client.PostAsync("/api/nations", Json(dto));
        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
    }

    // ─── Regions ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task Region_Lifecycle_CreateGetListUpdateDelete()
    {
        await ClearCollectionAsync("/api/regions");

        var dto = new RefDto { Name = "TestRegion", Code = "TR", Code2 = "TR2", Code3 = "TR3" };
        var createResp = await _client.PostAsync("/api/regions", Json(dto));
        Assert.Equal(HttpStatusCode.Created, createResp.StatusCode);
        var created = JsonSerializer.Deserialize<RefDto>(await createResp.Content.ReadAsStringAsync());
        Assert.NotNull(created);
        Assert.False(string.IsNullOrEmpty(created!.Id));

        // GetById
        var getResp = await _client.GetAsync($"/api/regions/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, getResp.StatusCode);

        // List
        var listResp = await _client.GetAsync("/api/regions");
        Assert.Equal(HttpStatusCode.OK, listResp.StatusCode);
        var list = JsonSerializer.Deserialize<List<RefDto>>(await listResp.Content.ReadAsStringAsync());
        Assert.NotNull(list);
        Assert.Contains(list!, r => r.Id == created.Id);

        // Update
        var updated = new RefDto { Name = "TestRegion-Updated", Code = "TRU" };
        var putResp = await _client.PutAsync($"/api/regions/{created.Id}", Json(updated));
        Assert.Equal(HttpStatusCode.OK, putResp.StatusCode);
        var after = JsonSerializer.Deserialize<RefDto>(await putResp.Content.ReadAsStringAsync());
        Assert.Equal("TestRegion-Updated", after?.Name);

        // Delete
        var delResp = await _client.DeleteAsync($"/api/regions/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, delResp.StatusCode);

        var getAfter = await _client.GetAsync($"/api/regions/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getAfter.StatusCode);
    }

    [Fact]
    public async Task Region_Create_RequiresName()
    {
        var dto = new RefDto { Name = "" };
        var resp = await _client.PostAsync("/api/regions", Json(dto));
        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
    }

    // ─── Cities ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task City_Lifecycle_CreateGetListUpdateDelete()
    {
        await ClearCollectionAsync("/api/cities");

        var dto = new RefDto { Name = "TestCity", Code = "TC", Code2 = "TC2", Code3 = "TC3" };
        var createResp = await _client.PostAsync("/api/cities", Json(dto));
        Assert.Equal(HttpStatusCode.Created, createResp.StatusCode);
        var created = JsonSerializer.Deserialize<RefDto>(await createResp.Content.ReadAsStringAsync());
        Assert.NotNull(created);
        Assert.False(string.IsNullOrEmpty(created!.Id));

        // GetById
        var getResp = await _client.GetAsync($"/api/cities/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, getResp.StatusCode);

        // List
        var listResp = await _client.GetAsync("/api/cities");
        Assert.Equal(HttpStatusCode.OK, listResp.StatusCode);
        var list = JsonSerializer.Deserialize<List<RefDto>>(await listResp.Content.ReadAsStringAsync());
        Assert.NotNull(list);
        Assert.Contains(list!, c => c.Id == created.Id);

        // Update
        var updated = new RefDto { Name = "TestCity-Updated", Code = "TCU" };
        var putResp = await _client.PutAsync($"/api/cities/{created.Id}", Json(updated));
        Assert.Equal(HttpStatusCode.OK, putResp.StatusCode);
        var after = JsonSerializer.Deserialize<RefDto>(await putResp.Content.ReadAsStringAsync());
        Assert.Equal("TestCity-Updated", after?.Name);

        // Delete
        var delResp = await _client.DeleteAsync($"/api/cities/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, delResp.StatusCode);

        var getAfter = await _client.GetAsync($"/api/cities/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getAfter.StatusCode);
    }

    [Fact]
    public async Task City_Create_RequiresName()
    {
        var dto = new RefDto { Name = "" };
        var resp = await _client.PostAsync("/api/cities", Json(dto));
        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
    }

    // ─── Identity Document Types ───────────────────────────────────────────────

    [Fact]
    public async Task IdentityDocumentType_Lifecycle_CreateGetListUpdateDelete()
    {
        await ClearCollectionAsync("/api/identity-document-types");

        var dto = new RefDto { Name = "Passport", Code = "PP", Code2 = "PP2", Code3 = "PP3" };
        var createResp = await _client.PostAsync("/api/identity-document-types", Json(dto));
        Assert.Equal(HttpStatusCode.Created, createResp.StatusCode);
        var created = JsonSerializer.Deserialize<RefDto>(await createResp.Content.ReadAsStringAsync());
        Assert.NotNull(created);
        Assert.False(string.IsNullOrEmpty(created!.Id));

        // GetById
        var getResp = await _client.GetAsync($"/api/identity-document-types/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, getResp.StatusCode);

        // List
        var listResp = await _client.GetAsync("/api/identity-document-types");
        Assert.Equal(HttpStatusCode.OK, listResp.StatusCode);
        var list = JsonSerializer.Deserialize<List<RefDto>>(await listResp.Content.ReadAsStringAsync());
        Assert.NotNull(list);
        Assert.Contains(list!, d => d.Id == created.Id);

        // Update
        var updated = new RefDto { Name = "Passport-Updated", Code = "PPU" };
        var putResp = await _client.PutAsync($"/api/identity-document-types/{created.Id}", Json(updated));
        Assert.Equal(HttpStatusCode.OK, putResp.StatusCode);
        var after = JsonSerializer.Deserialize<RefDto>(await putResp.Content.ReadAsStringAsync());
        Assert.Equal("Passport-Updated", after?.Name);

        // Delete
        var delResp = await _client.DeleteAsync($"/api/identity-document-types/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, delResp.StatusCode);

        var getAfter = await _client.GetAsync($"/api/identity-document-types/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getAfter.StatusCode);
    }

    [Fact]
    public async Task IdentityDocumentType_Create_RequiresName()
    {
        var dto = new RefDto { Name = "" };
        var resp = await _client.PostAsync("/api/identity-document-types", Json(dto));
        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
    }

    // ─── Family Relationships ──────────────────────────────────────────────────

    [Fact]
    public async Task FamilyRelationship_Lifecycle_CreateGetListUpdateDelete()
    {
        await ClearCollectionAsync("/api/family-relationships");

        var dto = new RefDto { Name = "Spouse", Code = "SP", Code2 = "SP2", Code3 = "SP3" };
        var createResp = await _client.PostAsync("/api/family-relationships", Json(dto));
        Assert.Equal(HttpStatusCode.Created, createResp.StatusCode);
        var created = JsonSerializer.Deserialize<RefDto>(await createResp.Content.ReadAsStringAsync());
        Assert.NotNull(created);
        Assert.False(string.IsNullOrEmpty(created!.Id));

        // GetById
        var getResp = await _client.GetAsync($"/api/family-relationships/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, getResp.StatusCode);

        // List
        var listResp = await _client.GetAsync("/api/family-relationships");
        Assert.Equal(HttpStatusCode.OK, listResp.StatusCode);
        var list = JsonSerializer.Deserialize<List<RefDto>>(await listResp.Content.ReadAsStringAsync());
        Assert.NotNull(list);
        Assert.Contains(list!, f => f.Id == created.Id);

        // Update
        var updated = new RefDto { Name = "Spouse-Updated", Code = "SPU" };
        var putResp = await _client.PutAsync($"/api/family-relationships/{created.Id}", Json(updated));
        Assert.Equal(HttpStatusCode.OK, putResp.StatusCode);
        var after = JsonSerializer.Deserialize<RefDto>(await putResp.Content.ReadAsStringAsync());
        Assert.Equal("Spouse-Updated", after?.Name);

        // Delete
        var delResp = await _client.DeleteAsync($"/api/family-relationships/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, delResp.StatusCode);

        var getAfter = await _client.GetAsync($"/api/family-relationships/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getAfter.StatusCode);
    }

    [Fact]
    public async Task FamilyRelationship_Create_RequiresName()
    {
        var dto = new RefDto { Name = "" };
        var resp = await _client.PostAsync("/api/family-relationships", Json(dto));
        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
    }
}

