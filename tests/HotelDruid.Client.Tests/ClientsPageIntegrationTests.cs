using System.Collections.Generic;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using HotelDruid.Client.Pages;
using HotelDruid.Client.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace HotelDruid.Client.Tests.Integration.Pages;

public class ClientsPageIntegrationTests : TestContext
{
    private readonly Mock<IClientApiService> clientApi = new();

    public ClientsPageIntegrationTests()
    {
        Services.AddClientLocalizationTestSupport();

        clientApi.Setup(x => x.GetAllAsync(It.IsAny<string?>())).ReturnsAsync(new List<ClientDto>
        {
            new("c-3", "Brown", "Zoe"),
            new("c-1", "Adams", "Anna"),
            new("c-2", "Adams", "Ben")
        });

        Services.AddScoped(_ => clientApi.Object);
    }

    [Fact]
    public void ClientsPage_ShouldSortClientsAlphabeticallyByDefault()
    {
        var component = RenderComponent<Clients>();

        component.WaitForAssertion(() =>
        {
            component.FindAll(".list-group-item").Should().HaveCount(3);
        });

        var items = component.FindAll(".list-group-item");
        items[0].TextContent.Should().Contain("Adams, Anna");
        items[1].TextContent.Should().Contain("Adams, Ben");
        items[2].TextContent.Should().Contain("Brown, Zoe");
    }
}