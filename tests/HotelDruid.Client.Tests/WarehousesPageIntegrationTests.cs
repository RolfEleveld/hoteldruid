using Bunit;
using HotelDruid.Client.Pages;
using HotelDruid.Client.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace HotelDruid.Client.Tests.Integration.Pages;

public class WarehousesPageIntegrationTests : TestContext
{
    public WarehousesPageIntegrationTests()
    {
        Services.AddClientLocalizationTestSupport();

        var warehouses = new Mock<IWarehouseApiService>();
        warehouses.Setup(x => x.GetWarehousesAsync()).ReturnsAsync(new List<WarehouseDto>
        {
            new("wh-1", "101 storage", "wh-101-storage", "Storage shed", "1", "1", null)
        });

        var inventory = new Mock<IInventoryApiService>();
        inventory.Setup(x => x.GetInventoryAsync()).ReturnsAsync(new List<InventoryDto>
        {
            new("inv-1", "asset-1", null, "wh-1", 4, null, null, null),
            new("inv-2", "asset-2", null, "wh-other", 1, null, null, null)
        });

        var assets = new Mock<IAssetApiService>();
        assets.Setup(x => x.GetAssetsAsync()).ReturnsAsync(new List<AssetDto>
        {
            new("asset-1", "Bed Linen", "BL-1", null, null),
            new("asset-2", "Towels", "TW-1", null, null)
        });

        Services.AddScoped(_ => warehouses.Object);
        Services.AddScoped(_ => inventory.Object);
        Services.AddScoped(_ => assets.Object);
    }

    [Fact]
    public void WarehouseDetails_ListsInventoryLikeReservations()
    {
        var component = RenderComponent<Warehouses>();

        component.WaitForAssertion(() => Assert.Contains("101 storage", component.Markup));
        component.Find("button.list-group-item").Click();

        component.WaitForAssertion(() =>
        {
            Assert.Contains("Inventory in this warehouse", component.Markup);
            Assert.Contains("Bed Linen", component.Markup);
            Assert.Contains("Quantity: 4", component.Markup);
            Assert.DoesNotContain("Towels", component.Markup);
            Assert.Contains("/inventory?selectedId=inv-1", component.Markup);
            Assert.DoesNotContain(">Loading", component.Markup);
        });
    }
}
