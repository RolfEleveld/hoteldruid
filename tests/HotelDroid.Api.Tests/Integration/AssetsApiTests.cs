using System.Net;
using System.Text;
using System.Text.Json;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using HotelDroid.Api;

namespace HotelDroid.Api.Tests.Integration;

[Collection("Sequential")]
public class AssetsApiTests : IAsyncLifetime
{
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;
    private string _testDataRoot = null!;

    private class AssetDto
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    private class WarehouseDto
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
        public string? FloorNumber { get; set; }
        public string? HouseNumber { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    private class InventoryDto
    {
        public string? Id { get; set; }
        public string? AssetId { get; set; }
        public string? RoomId { get; set; }
        public string? WarehouseId { get; set; }
        public int Quantity { get; set; }
        public int? MinQuantityDefault { get; set; }
        public bool? RequiredOnCheckin { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public async Task InitializeAsync()
    {
        _testDataRoot = Path.Combine(Path.GetTempPath(), $"api-assets-tests-{Guid.NewGuid()}");
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
        // try to deserialize as list of dynamic objects
        var list = JsonSerializer.Deserialize<List<JsonElement>>(content);
        if (list == null) return;
        foreach (var item in list)
        {
            if (item.TryGetProperty("id", out var idProp))
            {
                var id = idProp.GetString();
                if (!string.IsNullOrEmpty(id))
                {
                    await _client.DeleteAsync($"{path}/{id}");
                }
            }
        }
    }

    private Task ClearAssetsAsync() => ClearCollectionAsync("/api/assets");
    private Task ClearWarehousesAsync() => ClearCollectionAsync("/api/warehouses");
    private Task ClearInventoryAsync() => ClearCollectionAsync("/api/inventory");

    [Fact]
    public async Task CreateAsset_GetList_Update_Delete_Lifecycle()
    {
        await ClearInventoryAsync();
        await ClearWarehousesAsync();
        await ClearAssetsAsync();

        var asset = new AssetDto { Name = "TestAsset-1", Code = "TA-1", Description = "An asset" };
        var json = JsonSerializer.Serialize(asset);
        var createResp = await _client.PostAsync("/api/assets", new StringContent(json, Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.Created, createResp.StatusCode);
        var created = JsonSerializer.Deserialize<AssetDto>(await createResp.Content.ReadAsStringAsync());
        Assert.NotNull(created); Assert.False(string.IsNullOrEmpty(created!.Id));

        // Get by id
        var getResp = await _client.GetAsync($"/api/assets/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, getResp.StatusCode);

        // List
        var listResp = await _client.GetAsync("/api/assets");
        Assert.Equal(HttpStatusCode.OK, listResp.StatusCode);
        var list = JsonSerializer.Deserialize<List<AssetDto>>(await listResp.Content.ReadAsStringAsync());
        Assert.NotNull(list); Assert.Contains(list!, a => a.Id == created.Id);

        // Update
        var updated = new AssetDto { Id = created.Id, Name = "TestAsset-1-Updated", Code = "TA-1U", Description = "Updated" };
        var put = await _client.PutAsync($"/api/assets/{created.Id}", new StringContent(JsonSerializer.Serialize(updated), Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.OK, put.StatusCode);

        var after = JsonSerializer.Deserialize<AssetDto>(await put.Content.ReadAsStringAsync());
        Assert.Equal("TestAsset-1-Updated", after?.Name);

        // Delete
        var del = await _client.DeleteAsync($"/api/assets/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, del.StatusCode);

        // Verify deletion
        var getAfter = await _client.GetAsync($"/api/assets/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getAfter.StatusCode);
    }

    [Fact]
    public async Task CreateWarehouse_And_Inventory_Link()
    {
        await ClearInventoryAsync();
        await ClearWarehousesAsync();
        await ClearAssetsAsync();

        // Create asset
        var asset = new AssetDto { Name = "InvAsset-1", Code = "IA-1", Description = "Inventory asset" };
        var assetResp = await _client.PostAsync("/api/assets", new StringContent(JsonSerializer.Serialize(asset), Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.Created, assetResp.StatusCode);
        var createdAsset = JsonSerializer.Deserialize<AssetDto>(await assetResp.Content.ReadAsStringAsync());
        Assert.NotNull(createdAsset);

        // Create warehouse
        var wh = new WarehouseDto { Name = "MainStore", Code = "WS-1", Description = "Main warehouse", FloorNumber = "1" };
        var whResp = await _client.PostAsync("/api/warehouses", new StringContent(JsonSerializer.Serialize(wh), Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.Created, whResp.StatusCode);
        var createdWh = JsonSerializer.Deserialize<WarehouseDto>(await whResp.Content.ReadAsStringAsync());
        Assert.NotNull(createdWh);

        // Create inventory linking asset and warehouse
        var inv = new InventoryDto { AssetId = createdAsset!.Id, WarehouseId = createdWh!.Id, Quantity = 5, MinQuantityDefault = 1, RequiredOnCheckin = false };
        var invResp = await _client.PostAsync("/api/inventory", new StringContent(JsonSerializer.Serialize(inv), Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.Created, invResp.StatusCode);
        var createdInv = JsonSerializer.Deserialize<InventoryDto>(await invResp.Content.ReadAsStringAsync());
        Assert.NotNull(createdInv);

        // Get inventory by id
        var getInv = await _client.GetAsync($"/api/inventory/{createdInv!.Id}");
        Assert.Equal(HttpStatusCode.OK, getInv.StatusCode);

        // List inventory filtered by assetId
        var listByAsset = await _client.GetAsync($"/api/inventory?assetId={createdAsset.Id}");
        Assert.Equal(HttpStatusCode.OK, listByAsset.StatusCode);
        var invList = JsonSerializer.Deserialize<List<InventoryDto>>(await listByAsset.Content.ReadAsStringAsync());
        Assert.NotNull(invList);
        Assert.True(invList!.Any(i => i.AssetId == createdAsset.Id));

        // Cleanup
        await _client.DeleteAsync($"/api/inventory/{createdInv.Id}");
        await _client.DeleteAsync($"/api/warehouses/{createdWh.Id}");
        await _client.DeleteAsync($"/api/assets/{createdAsset.Id}");
    }
}
