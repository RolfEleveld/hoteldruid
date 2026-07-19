using Bunit;
using HotelDruid.Client.Pages;
using HotelDruid.Client.Services;
using HotelDruid.Client.Tests;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace HotelDruid.Client.Tests.Integration.Pages;

public class InventoryPageIntegrationTests : TestContext
{
    private readonly Mock<IInventoryApiService> _inventoryApi = new();
    private readonly Mock<IAssetApiService> _assetApi = new();
    private readonly Mock<IWarehouseApiService> _warehouseApi = new();
    private readonly Mock<IRoomApiService> _roomApi = new();

    public InventoryPageIntegrationTests()
    {
        Services.AddClientLocalizationTestSupport();

        _inventoryApi.Setup(x => x.GetInventoryAsync()).ReturnsAsync(new List<InventoryDto>
        {
            new("inv-1", "asset-1", null, "wh-1", 5, null, null, null)
        });

        _assetApi.Setup(x => x.GetAssetsAsync()).ReturnsAsync(new List<AssetDto>
        {
            new("asset-1", "Bed Linen", "BL-1", "Linen set", null)
        });

        _warehouseApi.Setup(x => x.GetWarehousesAsync()).ReturnsAsync(new List<WarehouseDto>
        {
            new("wh-1", "Main Store", "MS-1", "Primary warehouse", "1", "A", null)
        });

        _roomApi.Setup(x => x.GetRoomsAsync()).ReturnsAsync(new List<RoomDto>
        {
            new("room-1", "101", 2, 1, "A")
        });

        Services.AddScoped(_ => _inventoryApi.Object);
        Services.AddScoped(_ => _assetApi.Object);
        Services.AddScoped(_ => _warehouseApi.Object);
        Services.AddScoped(_ => _roomApi.Object);
    }

    [Fact]
    public void InventoryPage_ShouldLoadWarehouseAndAssetSelectors()
    {
        var component = RenderComponent<Inventory>();

        component.WaitForAssertion(() =>
        {
            Assert.Contains("Main Store", component.Markup);
            Assert.Contains("Bed Linen", component.Markup);
            Assert.Contains("Select warehouse...", component.Markup);
        });
    }

    [Fact]
    public void InventoryPage_ShouldRenderInventoryListUsingLinkedNames()
    {
        var component = RenderComponent<Inventory>();

        component.WaitForAssertion(() =>
        {
            Assert.Contains("Bed Linen - Main Store (5)", component.Markup);
        });
    }

    [Fact]
    public void InventoryPage_SelectWarehouse_ShouldNotClearSelectedRoom()
    {
        var component = RenderComponent<Inventory>();

        component.WaitForAssertion(() =>
        {
            var roomSelect = component.Find("select.form-select");
            roomSelect.Change("room-1");

            var warehouseInput = component.Find("input[placeholder='Select warehouse...']");
            warehouseInput.Input("Main");

            var warehouseOption = component.Find(".list-group button.list-group-item");
            warehouseOption.Click();

            var selectedRoomOption = component.Find("select.form-select option:checked");
            Assert.Equal("room-1", selectedRoomOption.GetAttribute("value"));
            Assert.Contains("101", selectedRoomOption.TextContent);
        });
    }

    [Fact]
    public void InventoryPage_SelectRoom_ShouldNotClearSelectedWarehouse()
    {
        var component = RenderComponent<Inventory>();

        component.WaitForAssertion(() =>
        {
            var warehouseInput = component.Find("input[placeholder='Select warehouse...']");
            warehouseInput.Input("Main");

            var warehouseOption = component.Find(".list-group button.list-group-item");
            warehouseOption.Click();

            var roomSelect = component.Find("select.form-select");
            roomSelect.Change("room-1");

            Assert.Contains("Main Store", component.Markup);
            Assert.Contains("Clear", component.Markup);
        });
    }

    [Fact]
    public void InventoryPage_WithSelectedId_ShouldShowDetailsImmediately()
    {
        var navigation = Services.GetRequiredService<NavigationManager>();
        navigation.NavigateTo("/inventory?selectedId=inv-1");

        var component = RenderComponent<Inventory>();

        component.WaitForAssertion(() =>
        {
            Assert.Contains("inv-1", component.Markup);
            Assert.Contains("Bed Linen", component.Markup);
            Assert.Contains("Main Store", component.Markup);
        });
    }
}
