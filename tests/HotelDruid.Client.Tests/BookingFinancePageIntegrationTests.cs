using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using HotelDruid.Client.Pages;
using HotelDruid.Client.Services;
using HotelDruid.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace HotelDruid.Client.Tests.Integration.Pages;

public class BookingFinancePageIntegrationTests : TestContext
{
    private readonly Mock<IBookingApiService> bookingApi = new();
    private readonly Mock<IBookingFinanceApiService> financeApi = new();
    private readonly Mock<IClientApiService> clientApi = new();
    private readonly Mock<IRoomApiService> roomApi = new();

    public BookingFinancePageIntegrationTests()
    {
        Services.AddClientLocalizationTestSupport();
        JSInterop.Mode = JSRuntimeMode.Loose;

        clientApi.Setup(x => x.GetAllAsync(It.IsAny<string?>())).ReturnsAsync(new List<HotelDruid.Client.Services.ClientDto>
        {
            new("guest-1", "Doe", "John"),
            new("guest-2", "Rossi", "Maria")
        });
        roomApi.Setup(x => x.GetRoomsAsync()).ReturnsAsync(new List<HotelDruid.Client.Services.RoomDto>
        {
            new("room-1", "101", 2, 1, "A"),
            new("room-2", "202", 2, 2, "A")
        });
        bookingApi.Setup(x => x.ListAsync(It.IsAny<int?>())).ReturnsAsync(new List<BookingDto>
        {
            new("stay-1", 2026, "guest-1", "room-1", new DateOnly(2026, 10, 11), new DateOnly(2026, 10, 12), PrepaidAmount: 100),
            new("stay-2", 2026, "guest-2", "room-2", new DateOnly(2026, 9, 3), new DateOnly(2026, 9, 4))
        });
        bookingApi.Setup(x => x.GetAsync(2026, "stay-1")).ReturnsAsync(
            new BookingDto("stay-1", 2026, "guest-1", "room-1", new DateOnly(2026, 10, 11), new DateOnly(2026, 10, 12), PrepaidAmount: 100));
        financeApi.Setup(x => x.GetSummaryAsync(2026, "stay-1")).ReturnsAsync(new BookingFinanceSummaryDto(
            "stay-1", 2026, 96, 0, 96, 100, 0, 0, 0, -4, "EUR", null, "Weekend", 1));
        financeApi.Setup(x => x.ListTransactionsAsync(2026, "stay-1")).ReturnsAsync(new List<BookingTransactionDto>());

        Services.AddScoped(_ => bookingApi.Object);
        Services.AddScoped(_ => financeApi.Object);
        Services.AddScoped(_ => clientApi.Object);
        Services.AddScoped(_ => roomApi.Object);
    }

    [Fact]
    public void BookingFinance_WithoutFilter_ListsEveryBooking()
    {
        var component = RenderComponent<BookingFinance>();

        component.WaitForAssertion(() =>
        {
            var items = component.FindAll("a.list-group-item");
            items.Should().HaveCount(2);
            component.Markup.Should().Contain("John Doe");
            component.Markup.Should().Contain("Maria Rossi");
        });
    }

    [Fact]
    public void BookingFinance_GuestFilter_ShowsOnlyThatGuest()
    {
        var navigation = Services.GetRequiredService<NavigationManager>();
        navigation.NavigateTo("/booking-finance?clientId=guest-1");

        var component = RenderComponent<BookingFinance>();

        component.WaitForAssertion(() =>
        {
            var items = component.FindAll("a.list-group-item");
            items.Should().HaveCount(1);
            items[0].TextContent.Should().Contain("John Doe");
            items[0].TextContent.Should().NotContain("Maria Rossi");
        });
    }

    [Fact]
    public void BookingFinance_UnknownGuestFilter_SaysNoMatch()
    {
        var navigation = Services.GetRequiredService<NavigationManager>();
        navigation.NavigateTo("/booking-finance?clientId=missing");

        var component = RenderComponent<BookingFinance>();

        component.WaitForAssertion(() =>
        {
            component.FindAll("a.list-group-item").Should().BeEmpty();
            component.Markup.Should().Contain("No bookings found for this guest.");
        });
    }

    [Fact]
    public void BookingFinance_SelectedBooking_ShowsReservationEvenWithoutCashMovements()
    {
        var navigation = Services.GetRequiredService<NavigationManager>();
        navigation.NavigateTo("/booking-finance?selectedId=stay-1&year=2026");

        var component = RenderComponent<BookingFinance>();

        component.WaitForAssertion(() =>
        {
            component.FindAll("a.list-group-item").Should().HaveCount(2);
            component.Markup.Should().Contain("Reservation");
            component.Markup.Should().Contain("96.00 EUR");
            component.Markup.Should().Contain("Prepaid");
            component.Markup.Should().NotContain("No transactions on this booking yet.");
        });
    }
}
