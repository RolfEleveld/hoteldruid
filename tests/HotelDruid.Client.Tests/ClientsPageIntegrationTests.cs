using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using HotelDruid.Client.Pages;
using HotelDruid.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace HotelDruid.Client.Tests.Integration.Pages;

public class ClientsPageIntegrationTests : TestContext
{
    private readonly Mock<IClientApiService> clientApi = new();
    private readonly Mock<IRoomApiService> roomApi = new();
    private readonly Mock<IBookingApiService> bookingApi = new();
    private readonly Mock<IBookingFinanceApiService> financeApi = new();

    public ClientsPageIntegrationTests()
    {
        Services.AddClientLocalizationTestSupport();

        var knownClients = new List<ClientDto>
        {
            new("c-3", "Brown", "Zoe"),
            new("c-1", "Adams", "Anna"),
            new("c-2", "Adams", "Ben")
        };
        clientApi.Setup(x => x.GetAllAsync(It.IsAny<string?>(), It.IsAny<DateOnly?>(), It.IsAny<DateOnly?>())).ReturnsAsync(knownClients);
        clientApi.Setup(x => x.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((string id) => knownClients.FirstOrDefault(client => string.Equals(client.Id, id, StringComparison.OrdinalIgnoreCase)));
        roomApi.Setup(x => x.GetRoomsAsync()).ReturnsAsync(new List<RoomDto>());
        bookingApi.Setup(x => x.ListAsync(It.IsAny<int?>(), It.IsAny<bool>(), It.IsAny<DateOnly?>(), It.IsAny<DateOnly?>(), It.IsAny<string?>()))
            .ReturnsAsync((int? year, bool includeDeleted, DateOnly? from, DateOnly? to, string? clientId) =>
                string.Equals(clientId, "c-1", StringComparison.OrdinalIgnoreCase)
                    ? new List<HotelDruid.Shared.BookingDto> { new("b-anna", 2026, "c-1", "room-1", new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 3), "Confirmed") }
                    : new List<HotelDruid.Shared.BookingDto>());
        financeApi.Setup(x => x.GetSummaryAsync(It.IsAny<int>(), It.IsAny<string>()))
            .ReturnsAsync((int year, string bookingId) => null);

        Services.AddScoped(_ => clientApi.Object);
        Services.AddScoped(_ => roomApi.Object);
        Services.AddScoped(_ => bookingApi.Object);
        Services.AddScoped(_ => financeApi.Object);
    }

    [Fact]
    public void ClientsPage_ShouldSortClientsAlphabeticallyByDefault()
    {
        var component = Render<Clients>();

        component.WaitForAssertion(() =>
        {
            component.FindAll(".list-group-item").Should().HaveCount(3);
        });

        var items = component.FindAll(".list-group-item");
        items[0].TextContent.Should().Contain("Adams, Anna");
        items[1].TextContent.Should().Contain("Adams, Ben");
        items[2].TextContent.Should().Contain("Brown, Zoe");

        var today = DateOnly.FromDateTime(DateTime.Today);
        var dates = component.FindAll("input[type='date']");
        dates[0].GetAttribute("value").Should().Be(today.ToString("yyyy-MM-dd"));
        dates[1].GetAttribute("value").Should().Be(today.ToString("yyyy-MM-dd"));
        clientApi.Verify(x => x.GetAllAsync(null, today, today), Times.AtLeastOnce);
    }

    [Fact]
    public void ClientsPage_NameFilter_ShouldNarrowTheLoadedClients()
    {
        var component = Render<Clients>();
        component.WaitForAssertion(() => component.FindAll(".list-group-item").Should().HaveCount(3));

        component.Find("input[type=text]").Input("Brown");

        component.WaitForAssertion(() =>
        {
            var items = component.FindAll(".list-group-item");
            items.Should().ContainSingle();
            items[0].TextContent.Should().Contain("Brown, Zoe");
        });
    }

    [Fact]
    public void ClientsPage_Load_ShouldAskForClientsInTheSelectedDates()
    {
        var component = Render<Clients>();
        component.WaitForAssertion(() => component.FindAll("input[type='date']").Should().HaveCount(2));

        component.FindAll("input[type='date']")[0].Change("2026-05-01");
        component.FindAll("input[type='date']")[1].Change("2026-05-03");
        component.FindAll("button").First(button => button.TextContent.Contains("Load")).Click();

        component.WaitForAssertion(() =>
            clientApi.Verify(x => x.GetAllAsync(null, new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 3)), Times.AtLeastOnce));
    }

    [Fact]
    public void ClientsPage_SelectedId_ShouldHighlightThatClientAndUseTheStayDates()
    {
        var navigation = Services.GetRequiredService<NavigationManager>();
        navigation.NavigateTo("/clients?selectedId=c-1");

        var component = Render<Clients>();

        component.WaitForAssertion(() =>
        {
            var selected = component.FindAll(".list-group-item.active");
            selected.Should().ContainSingle();
            selected[0].TextContent.Should().Contain("Adams, Anna");
        });

        var dates = component.FindAll("input[type='date']");
        dates[0].GetAttribute("value").Should().Be("2026-05-01");
        dates[1].GetAttribute("value").Should().Be("2026-05-03");
        clientApi.Verify(x => x.GetAllAsync(null, new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 3)), Times.AtLeastOnce);
    }

    [Fact]
    public void ClientsPage_AddPhoneAndEmail_ExpandsContactFields()
    {
        var component = Render<Clients>();
        component.FindAll("button").First(b => b.TextContent.Contains("Add Client")).Click();
        component.FindAll("button").First(b => b.TextContent.Contains("Add contacts")).Click();

        var initialPhoneCount = component.FindAll(".client-section-contact input[type=tel]").Count;
        var initialEmailCount = component.FindAll(".client-section-contact input[type=email]").Count;

        component.FindAll("button").First(b => b.TextContent.Contains("Add phone")).Click();
        component.FindAll("button").First(b => b.TextContent.Contains("Add email")).Click();

        component.FindAll(".client-section-contact input[type=tel]").Count.Should().Be(initialPhoneCount + 1);
        component.FindAll(".client-section-contact input[type=email]").Count.Should().Be(initialEmailCount + 1);

        component.FindAll("button").First(b => b.TextContent.Contains("Add phone")).Click();
        component.FindAll("button").First(b => b.TextContent.Contains("Add email")).Click();

        component.FindAll(".client-section-contact input[type=tel]").Count.Should().Be(initialPhoneCount + 2);
        component.FindAll(".client-section-contact input[type=email]").Count.Should().Be(initialEmailCount + 2);
    }

    [Fact]
    public void ClientsPage_RemovePhoneAndEmail_UpdatesContactFields()
    {
        var component = Render<Clients>();
        component.FindAll("button").First(b => b.TextContent.Contains("Add Client")).Click();
        component.FindAll("button").First(b => b.TextContent.Contains("Add contacts")).Click();

        var addPhone = component.FindAll("button").First(b => b.TextContent.Contains("Add phone"));
        var addEmail = component.FindAll("button").First(b => b.TextContent.Contains("Add email"));

        addPhone.Click();
        addPhone.Click();
        addEmail.Click();
        addEmail.Click();

        var phoneInputs = component.FindAll(".client-section-contact input[type=tel]");
        phoneInputs.Should().HaveCount(3);

        var emailInputs = component.FindAll(".client-section-contact input[type=email]");
        emailInputs.Should().HaveCount(3);

        phoneInputs[0].NextElementSibling!.Click();
        component.FindAll(".client-section-contact input[type=tel]").Count.Should().Be(2);

        emailInputs = component.FindAll(".client-section-contact input[type=email]");
        emailInputs[0].NextElementSibling!.Click();
        component.FindAll(".client-section-contact input[type=email]").Count.Should().Be(2);
    }
}
