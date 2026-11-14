using System.Reflection;
using AngleSharp.Dom;
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
        var component = Render<Inventory>();

        component.WaitForAssertion(() =>
        {
            Assert.Contains("Bed Linen", component.Markup);
            Assert.Contains("Main Store", component.Markup);
        });

        // Room and warehouse selectors live in the edit form, which opens after an item is selected.
        component.Find("button.list-group-item").Click();

        component.WaitForAssertion(() =>
        {
            Assert.Contains("Select warehouse...", component.Markup);
            Assert.NotNull(component.Find("select.form-select"));
        });
    }

    [Fact]
    public void InventoryPage_ShouldRenderInventoryListUsingLinkedNames()
    {
        var component = Render<Inventory>();

        component.WaitForAssertion(() =>
        {
            var item = component.Find("button.list-group-item");
            Assert.Contains("Bed Linen", item.TextContent);
            Assert.Contains("Main Store", item.TextContent);
            Assert.Contains("5", item.TextContent);
        });

        component.Find("button.list-group-item").Click();

        component.WaitForAssertion(() =>
        {
            Assert.Contains("Bed Linen - Main Store (5)", component.Markup);
        });
    }

    [Fact]
    public void InventoryPage_SelectWarehouse_ShouldNotClearSelectedRoom()
    {
        var component = Render<Inventory>();

        component.WaitForAssertion(() => Assert.Contains("Bed Linen", component.Markup));
        component.Find("button.list-group-item").Click();

        component.WaitForAssertion(() => Assert.NotNull(RoomSelect(component)));
        RoomSelect(component).Change(new ChangeEventArgs { Value = "room-1" });

        component.WaitForAssertion(() => Assert.Equal("room-1", GetEditValue(component, "RoomId")));
        component.Find("input[placeholder='Select warehouse...']").Input("Main");

        component.WaitForAssertion(() => Assert.NotNull(component.Find(".list-group.mt-2 button.list-group-item")));
        component.Find(".list-group.mt-2 button.list-group-item").Click();

        component.WaitForAssertion(() =>
        {
            Assert.Equal("room-1", GetEditValue(component, "RoomId"));
            Assert.Equal("wh-1", GetEditValue(component, "WarehouseId"));
            Assert.Contains("101", RoomSelect(component).TextContent);
            Assert.Contains("Main Store", component.Markup);
            Assert.Contains("Clear", component.Markup);
        });
    }

    private static AngleSharp.Dom.IElement RoomSelect(IRenderedComponent<Inventory> component)
        => component.FindAll("select.form-select")[1];

    private static string? GetEditValue(IRenderedComponent<Inventory> component, string propertyName)
    {
        var edit = typeof(Inventory).GetField("edit", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(component.Instance);
        Assert.NotNull(edit);
        return edit!.GetType().GetProperty(propertyName)?.GetValue(edit) as string;
    }

    [Fact]
    public void InventoryPage_SelectRoom_ShouldNotClearSelectedWarehouse()
    {
        var component = Render<Inventory>();

        component.WaitForAssertion(() => Assert.Contains("Bed Linen", component.Markup));
        component.Find("button.list-group-item").Click();

        component.WaitForAssertion(() => Assert.NotNull(component.Find("input[placeholder='Select warehouse...']")));
        component.Find("input[placeholder='Select warehouse...']").Input("Main");

        component.WaitForAssertion(() => Assert.NotNull(component.Find(".list-group button.list-group-item")));
        component.Find(".list-group button.list-group-item").Click();

        component.WaitForAssertion(() => Assert.Contains("Clear", component.Markup));
        RoomSelect(component).Change(new ChangeEventArgs { Value = "room-1" });

        component.WaitForAssertion(() =>
        {
            Assert.Equal("room-1", GetEditValue(component, "RoomId"));
            Assert.Equal("wh-1", GetEditValue(component, "WarehouseId"));
            Assert.Contains("Main Store", component.Markup);
            Assert.Contains("Clear", component.Markup);
        });
    }

    [Fact]
    public void InventoryPage_WithSelectedId_ShouldShowDetailsImmediately()
    {
        var navigation = Services.GetRequiredService<NavigationManager>();
        navigation.NavigateTo("/inventory?selectedId=inv-1");

        var component = Render<Inventory>();

        component.WaitForAssertion(() =>
        {
            Assert.Contains("inv-1", component.Markup);
            Assert.Contains("Bed Linen", component.Markup);
            Assert.Contains("Main Store", component.Markup);
        });
    }
}
