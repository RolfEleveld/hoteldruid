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
        Assert.Contains(invList!, i => i.AssetId == createdAsset.Id);

        // Cleanup
        await _client.DeleteAsync($"/api/inventory/{createdInv.Id}");
        await _client.DeleteAsync($"/api/warehouses/{createdWh.Id}");
        await _client.DeleteAsync($"/api/assets/{createdAsset.Id}");
    }
    // ========== Asset validation tests ==========

    [Fact]
    public async Task Asset_Create_MissingName_ReturnsBadRequest()
    {
        var r = await _client.PostAsync("/api/assets",
            new StringContent(JsonSerializer.Serialize(new AssetDto { Code = "X1", Description = "no name" }), Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.BadRequest, r.StatusCode);
    }

    [Fact]
    public async Task Asset_GetById_NotFound()
    {
        var r = await _client.GetAsync("/api/assets/nonexistent-id-99999");
        Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
    }

    // ========== Warehouse standalone lifecycle ==========

    [Fact]
    public async Task Warehouse_Create_GetById_Update_Delete_Lifecycle()
    {
        var wh = new WarehouseDto { Name = "WhTest-1", Code = "WH-T1", Description = "Test warehouse", FloorNumber = "2", HouseNumber = "5" };
        var createResp = await _client.PostAsync("/api/warehouses",
            new StringContent(JsonSerializer.Serialize(wh), Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.Created, createResp.StatusCode);
        var created = JsonSerializer.Deserialize<WarehouseDto>(await createResp.Content.ReadAsStringAsync());
        Assert.NotNull(created); Assert.False(string.IsNullOrEmpty(created!.Id));
        Assert.Equal("WhTest-1", created.Name);

        // Get by id
        var getResp = await _client.GetAsync($"/api/warehouses/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, getResp.StatusCode);
        var fetched = JsonSerializer.Deserialize<WarehouseDto>(await getResp.Content.ReadAsStringAsync());
        Assert.Equal(created.Id, fetched?.Id);

        // List
        var listResp = await _client.GetAsync("/api/warehouses");
        Assert.Equal(HttpStatusCode.OK, listResp.StatusCode);
        var list = JsonSerializer.Deserialize<List<WarehouseDto>>(await listResp.Content.ReadAsStringAsync());
        Assert.Contains(list!, w => w.Id == created.Id);

        // Update
        var updated = new WarehouseDto { Id = created.Id, Name = "WhTest-1-Updated", Code = "WH-T1U", FloorNumber = "3" };
        var putResp = await _client.PutAsync($"/api/warehouses/{created.Id}",
            new StringContent(JsonSerializer.Serialize(updated), Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.OK, putResp.StatusCode);
        var after = JsonSerializer.Deserialize<WarehouseDto>(await putResp.Content.ReadAsStringAsync());
        Assert.Equal("WhTest-1-Updated", after?.Name);

        // Delete
        var delResp = await _client.DeleteAsync($"/api/warehouses/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, delResp.StatusCode);

        // Verify deletion
        var getAfter = await _client.GetAsync($"/api/warehouses/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getAfter.StatusCode);
    }

    [Fact]
    public async Task Warehouse_Create_MissingName_ReturnsBadRequest()
    {
        var r = await _client.PostAsync("/api/warehouses",
            new StringContent(JsonSerializer.Serialize(new WarehouseDto { Code = "WH-NM" }), Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.BadRequest, r.StatusCode);
    }

    [Fact]
    public async Task Warehouse_GetById_NotFound()
    {
        var r = await _client.GetAsync("/api/warehouses/nonexistent-warehouse-99999");
        Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
    }

    // ========== Inventory additional tests ==========

    [Fact]
    public async Task Inventory_Update_Quantity_ReturnsOk()
    {
        await ClearInventoryAsync(); await ClearWarehousesAsync(); await ClearAssetsAsync();

        var assetResp = await _client.PostAsync("/api/assets",
            new StringContent(JsonSerializer.Serialize(new AssetDto { Name = "InvUpd-Asset", Code = "IUA-1" }), Encoding.UTF8, "application/json"));
        var asset = JsonSerializer.Deserialize<AssetDto>(await assetResp.Content.ReadAsStringAsync());

        var whResp = await _client.PostAsync("/api/warehouses",
            new StringContent(JsonSerializer.Serialize(new WarehouseDto { Name = "InvUpd-WH", Code = "IWH-1" }), Encoding.UTF8, "application/json"));
        var wh = JsonSerializer.Deserialize<WarehouseDto>(await whResp.Content.ReadAsStringAsync());

        var invResp = await _client.PostAsync("/api/inventory",
            new StringContent(JsonSerializer.Serialize(new InventoryDto { AssetId = asset!.Id, WarehouseId = wh!.Id, Quantity = 3 }), Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.Created, invResp.StatusCode);
        var inv = JsonSerializer.Deserialize<InventoryDto>(await invResp.Content.ReadAsStringAsync());

        // Update quantity
        var updatedInv = new InventoryDto { Id = inv!.Id, AssetId = asset.Id, WarehouseId = wh.Id, Quantity = 10, MinQuantityDefault = 2, RequiredOnCheckin = true };
        var putResp = await _client.PutAsync($"/api/inventory/{inv.Id}",
            new StringContent(JsonSerializer.Serialize(updatedInv), Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.OK, putResp.StatusCode);
        var afterUpdate = JsonSerializer.Deserialize<InventoryDto>(await putResp.Content.ReadAsStringAsync());
        Assert.Equal(10, afterUpdate?.Quantity);
        Assert.Equal(2, afterUpdate?.MinQuantityDefault);
        Assert.True(afterUpdate?.RequiredOnCheckin);

        // Cleanup
        await _client.DeleteAsync($"/api/inventory/{inv.Id}");
        await _client.DeleteAsync($"/api/warehouses/{wh.Id}");
        await _client.DeleteAsync($"/api/assets/{asset.Id}");
    }

    [Fact]
    public async Task Inventory_Delete_ReturnsNoContent()
    {
        await ClearInventoryAsync(); await ClearWarehousesAsync(); await ClearAssetsAsync();

        var assetResp = await _client.PostAsync("/api/assets",
            new StringContent(JsonSerializer.Serialize(new AssetDto { Name = "DelInv-Asset", Code = "DIA-1" }), Encoding.UTF8, "application/json"));
        var asset = JsonSerializer.Deserialize<AssetDto>(await assetResp.Content.ReadAsStringAsync());

        var whResp = await _client.PostAsync("/api/warehouses",
            new StringContent(JsonSerializer.Serialize(new WarehouseDto { Name = "DelInv-WH", Code = "DIW-1" }), Encoding.UTF8, "application/json"));
        var wh = JsonSerializer.Deserialize<WarehouseDto>(await whResp.Content.ReadAsStringAsync());

        var invResp = await _client.PostAsync("/api/inventory",
            new StringContent(JsonSerializer.Serialize(new InventoryDto { AssetId = asset!.Id, WarehouseId = wh!.Id, Quantity = 1 }), Encoding.UTF8, "application/json"));
        var inv = JsonSerializer.Deserialize<InventoryDto>(await invResp.Content.ReadAsStringAsync());

        var delResp = await _client.DeleteAsync($"/api/inventory/{inv!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, delResp.StatusCode);

        var getAfter = await _client.GetAsync($"/api/inventory/{inv.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getAfter.StatusCode);

        await _client.DeleteAsync($"/api/warehouses/{wh.Id}");
        await _client.DeleteAsync($"/api/assets/{asset.Id}");
    }

    [Fact]
    public async Task Inventory_List_FilteredByWarehouse()
    {
        await ClearInventoryAsync(); await ClearWarehousesAsync(); await ClearAssetsAsync();

        var assetResp = await _client.PostAsync("/api/assets",
            new StringContent(JsonSerializer.Serialize(new AssetDto { Name = "WH-Filter-Asset", Code = "WFA-1" }), Encoding.UTF8, "application/json"));
        var asset = JsonSerializer.Deserialize<AssetDto>(await assetResp.Content.ReadAsStringAsync());

        var wh1Resp = await _client.PostAsync("/api/warehouses",
            new StringContent(JsonSerializer.Serialize(new WarehouseDto { Name = "WH-Filter-A", Code = "WFA-1" }), Encoding.UTF8, "application/json"));
        var wh1 = JsonSerializer.Deserialize<WarehouseDto>(await wh1Resp.Content.ReadAsStringAsync());

        var wh2Resp = await _client.PostAsync("/api/warehouses",
            new StringContent(JsonSerializer.Serialize(new WarehouseDto { Name = "WH-Filter-B", Code = "WFB-1" }), Encoding.UTF8, "application/json"));
        var wh2 = JsonSerializer.Deserialize<WarehouseDto>(await wh2Resp.Content.ReadAsStringAsync());

        var inv1Resp = await _client.PostAsync("/api/inventory",
            new StringContent(JsonSerializer.Serialize(new InventoryDto { AssetId = asset!.Id, WarehouseId = wh1!.Id, Quantity = 2 }), Encoding.UTF8, "application/json"));
        var inv1 = JsonSerializer.Deserialize<InventoryDto>(await inv1Resp.Content.ReadAsStringAsync());

        var inv2Resp = await _client.PostAsync("/api/inventory",
            new StringContent(JsonSerializer.Serialize(new InventoryDto { AssetId = asset.Id, WarehouseId = wh2!.Id, Quantity = 4 }), Encoding.UTF8, "application/json"));
        var inv2 = JsonSerializer.Deserialize<InventoryDto>(await inv2Resp.Content.ReadAsStringAsync());

        var listByWh1 = await _client.GetAsync($"/api/inventory?warehouseId={wh1.Id}");
        Assert.Equal(HttpStatusCode.OK, listByWh1.StatusCode);
        var whList = JsonSerializer.Deserialize<List<InventoryDto>>(await listByWh1.Content.ReadAsStringAsync());
        Assert.NotNull(whList);
        Assert.Contains(whList!, i => i.WarehouseId == wh1.Id);
        Assert.DoesNotContain(whList!, i => i.WarehouseId == wh2.Id);

        // Cleanup
        await _client.DeleteAsync($"/api/inventory/{inv1!.Id}");
        await _client.DeleteAsync($"/api/inventory/{inv2!.Id}");
        await _client.DeleteAsync($"/api/warehouses/{wh1.Id}");
        await _client.DeleteAsync($"/api/warehouses/{wh2.Id}");
        await _client.DeleteAsync($"/api/assets/{asset.Id}");
    }

    [Fact]
    public async Task Inventory_GetById_NotFound()
    {
        var r = await _client.GetAsync("/api/inventory/nonexistent-inventory-99999");
        Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
    }
}