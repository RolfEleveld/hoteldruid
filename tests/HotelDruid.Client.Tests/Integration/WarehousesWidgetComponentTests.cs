using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Bunit;
using HotelDruid.Client.Components.Warehouses;
using HotelDruid.Client.Services;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using HotelDruid.Client.Tests;

namespace HotelDruid.Client.Tests.Integration.Components
{
    public class WarehousesWidgetComponentTests : TestContext
    {
        private Mock<IWarehouseApiService> _mockService;

        public WarehousesWidgetComponentTests()
        {
            Services.AddClientLocalizationTestSupport();
            _mockService = new Mock<IWarehouseApiService>();
            Services.AddScoped(_ => _mockService.Object);

            _mockService.Setup(x => x.GetWarehousesAsync()).Returns(Task.FromResult(new List<WarehouseDto>
            {
                new("w1","Main Store","M1","Main storage","1","A",null),
                new("w2","Backroom","B1","Backroom storage","0","B",null)
            }));
        }

        [Fact]
        public void WarehousesWidget_ShouldRender()
        {
            var comp = RenderComponent<WarehousesWidget>();
            Assert.NotNull(comp.Instance);
        }

        [Fact]
        public async Task WarehousesWidget_ShouldCallApi()
        {
            RenderComponent<WarehousesWidget>();
            await Task.Delay(50);
            _mockService.Verify(x => x.GetWarehousesAsync(), Times.Once);
        }
    }
}

