using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Bunit;
using HotelDroid.Client.Components.Inventory;
using HotelDroid.Client.Services;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotelDroid.Client.Tests.Integration.Components
{
    public class InventoryWidgetComponentTests : TestContext
    {
        private Mock<IInventoryApiService> _mockService;
        private Mock<ILanguageService> _mockLang;

        public InventoryWidgetComponentTests()
        {
            _mockService = new Mock<IInventoryApiService>();
            _mockLang = new Mock<ILanguageService>();
            _mockLang.Setup(x => x.GetText(It.IsAny<string>(), It.IsAny<string>())).Returns((string k, string d) => d);
            Services.AddScoped(_ => _mockService.Object);
            Services.AddScoped(_ => _mockLang.Object);

            _mockService.Setup(x => x.GetInventoryAsync()).Returns(Task.FromResult(new List<InventoryDto>
            {
                new("i1","asset-1","room-1","w1",10,null,null,null),
                new("i2","asset-2","room-2","w2",5,null,null,null)
            }));
        }

        [Fact]
        public void InventoryWidget_ShouldRender()
        {
            var comp = RenderComponent<InventoryWidget>();
            Assert.NotNull(comp.Instance);
        }

        [Fact]
        public async Task InventoryWidget_ShouldCallApi()
        {
            RenderComponent<InventoryWidget>();
            await Task.Delay(50);
            _mockService.Verify(x => x.GetInventoryAsync(), Times.Once);
        }
    }
}
