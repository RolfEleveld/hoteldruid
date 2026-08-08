using System.Collections.Generic;
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

public class BookingsPageIntegrationTests : TestContext
{
    private readonly Mock<IBookingApiService> bookingApi = new();
    private readonly Mock<IClientApiService> clientApi = new();
    private readonly Mock<IRoomApiService> roomApi = new();
    private readonly Mock<IActiveYearService> activeYearService = new();

    public BookingsPageIntegrationTests()
    {
        Services.AddClientLocalizationTestSupport();
        activeYearService.SetupGet(x => x.CurrentYear).Returns(2026);
        activeYearService.Setup(x => x.InitializeAsync()).Returns(Task.CompletedTask);
        activeYearService.Setup(x => x.SetActiveYearAsync(It.IsAny<int>(), It.IsAny<bool>())).Returns(Task.CompletedTask);

        bookingApi.Setup(x => x.ListAsync(It.IsAny<int?>())).ReturnsAsync(new List<BookingDto>
        {
            new("b-2", 2026, "client-2", "room-2", new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 3), "Pending", "Second booking"),
            new("b-1", 2026, "client-1", "room-1", new DateOnly(2026, 5, 10), new DateOnly(2026, 5, 12), "Confirmed", "First booking")
        });

        bookingApi.Setup(x => x.ListStaysAsync(It.IsAny<int?>())).ReturnsAsync(new List<BookingDto>
        {
            new("stay-1", 2026, "client-1", "room-1", new DateOnly(2026, 8, 6), new DateOnly(2026, 8, 10), "Confirmed", "Checked in", new DateOnly(2026, 8, 6), 2)
        });

        bookingApi.Setup(x => x.ListReservationsAsync(It.IsAny<int?>())).ReturnsAsync(new List<BookingDto>
        {
            new("res-1", 2026, "client-2", "room-2", new DateOnly(2026, 8, 11), new DateOnly(2026, 8, 14), "Confirmed", "Reservation")
        });

        bookingApi.Setup(x => x.ListArrivalsAsync(It.IsAny<int?>(), It.IsAny<int?>())).ReturnsAsync(new List<BookingDto>
        {
            new("arr-1", 2026, "client-2", "room-2", new DateOnly(2026, 8, 8), new DateOnly(2026, 8, 10), "Confirmed", "Arrival expected")
        });

        bookingApi.Setup(x => x.ListDeparturesAsync(It.IsAny<int?>())).ReturnsAsync(new List<BookingDto>
        {
            new("dep-1", 2026, "client-1", "room-1", new DateOnly(2026, 8, 6), new DateOnly(2026, 8, 8), "Confirmed", "Departure", new DateOnly(2026, 8, 6), 2, null, new DateOnly(2026, 8, 8))
        });

        clientApi.Setup(x => x.GetAllAsync(It.IsAny<string?>())).ReturnsAsync(new List<HotelDruid.Client.Services.ClientDto>
        {
            new("client-1", "Adams", "Anna"),
            new("client-2", "Brown", "Ben")
        });
        roomApi.Setup(x => x.GetRoomsAsync()).ReturnsAsync(new List<HotelDruid.Client.Services.RoomDto>
        {
            new HotelDruid.Client.Services.RoomDto("room-77", "Room 77", 2, 1, "A", 1, 0, true),
            new HotelDruid.Client.Services.RoomDto("room-1", "Room 1", 2, 1, "A", 1, 0, true),
            new HotelDruid.Client.Services.RoomDto("room-2", "Room 2", 2, 1, "A", 2, 0, true)
        });

        Services.AddScoped(_ => bookingApi.Object);
        Services.AddScoped(_ => clientApi.Object);
        Services.AddScoped(_ => roomApi.Object);
        Services.AddScoped(_ => activeYearService.Object);
    }

    [Fact]
    public void BookingsPage_ShouldRenderBookingsAndDateInputs()
    {
        var component = RenderComponent<Bookings>();

        component.WaitForAssertion(() =>
        {
            var items = component.FindAll(".list-group-item");
            items.Should().HaveCount(2);
        });

        var itemsInOrder = component.FindAll(".list-group-item");
        itemsInOrder[0].TextContent.Should().Contain("2026 — Adams Anna");
        itemsInOrder[0].TextContent.Should().Contain("2026-05-10 → 2026-05-12");
        itemsInOrder[1].TextContent.Should().Contain("2026 — Brown Ben");

        itemsInOrder[0].Click();

        component.WaitForAssertion(() =>
        {
            component.FindAll("input[type='date']").Should().HaveCount(4);
        });

        component.Markup.Should().Contain("Actual Checkout Date");
    }

    [Fact]
    public void BookingsNewRoute_ShouldPrefillRoom_WhenRoomIdQueryIsProvided()
    {
        var navigation = Services.GetRequiredService<NavigationManager>();
        navigation.NavigateTo("/bookings/new?roomId=room-77");

        var component = RenderComponent<Bookings>();

        component.WaitForAssertion(() =>
        {
            component.Markup.Should().Contain("Room 77");
            component.Markup.Should().NotContain("room-77");
        });
    }

    [Fact]
    public void BookingsPage_ViewStays_ShouldUseStaysApiForLeftColumn()
    {
        var navigation = Services.GetRequiredService<NavigationManager>();
        navigation.NavigateTo("/bookings?view=stays");

        var component = RenderComponent<Bookings>();

        component.WaitForAssertion(() =>
        {
            var items = component.FindAll(".list-group-item");
            items.Should().HaveCount(1);
            items[0].TextContent.Should().Contain("2026 — Adams Anna");
            items[0].TextContent.Should().Contain("2026-08-06 → 2026-08-10");
        });

        bookingApi.Verify(x => x.ListStaysAsync(2026), Times.AtLeastOnce);
    }

    [Fact]
    public void BookingsPage_ViewArrivals_ShouldUseArrivalsApiForLeftColumn()
    {
        var navigation = Services.GetRequiredService<NavigationManager>();
        navigation.NavigateTo("/bookings?view=arrivals&lookAheadDays=2");

        var component = RenderComponent<Bookings>();

        component.WaitForAssertion(() =>
        {
            var items = component.FindAll(".list-group-item");
            items.Should().HaveCount(1);
            items[0].TextContent.Should().Contain("2026 — Brown Ben");
        });

        bookingApi.Verify(x => x.ListArrivalsAsync(2026, 2), Times.AtLeastOnce);
    }

    [Fact]
    public void BookingsPage_ViewDepartures_ShouldUseDeparturesApiForLeftColumn()
    {
        var navigation = Services.GetRequiredService<NavigationManager>();
        navigation.NavigateTo("/bookings?view=departures");

        var component = RenderComponent<Bookings>();

        component.WaitForAssertion(() =>
        {
            var items = component.FindAll(".list-group-item");
            items.Should().HaveCount(1);
            items[0].TextContent.Should().Contain("2026 — Adams Anna");
        });

        bookingApi.Verify(x => x.ListDeparturesAsync(2026), Times.AtLeastOnce);
    }

    [Fact]
    public void BookingsPage_ViewReservations_ShouldUseReservationsApiForLeftColumn()
    {
        var navigation = Services.GetRequiredService<NavigationManager>();
        navigation.NavigateTo("/bookings?view=reservations");

        var component = RenderComponent<Bookings>();

        component.WaitForAssertion(() =>
        {
            var items = component.FindAll(".list-group-item");
            items.Should().HaveCount(1);
            items[0].TextContent.Should().Contain("2026 — Brown Ben");
        });

        bookingApi.Verify(x => x.ListReservationsAsync(2026), Times.AtLeastOnce);
    }

    [Fact]
    public void BookingsPage_ViewArrivals_ShouldShow_Arrived_And_NoShow_Actions()
    {
        var navigation = Services.GetRequiredService<NavigationManager>();
        navigation.NavigateTo("/bookings?view=arrivals&lookAheadDays=2");

        var component = RenderComponent<Bookings>();

        component.WaitForAssertion(() =>
        {
            var items = component.FindAll(".list-group-item");
            items.Should().HaveCount(1);
        });

        component.FindAll(".list-group-item")[0].Click();

        component.WaitForAssertion(() =>
        {
            component.Markup.Should().Contain("Arrived");
            component.Markup.Should().Contain("Mark No-Show");
            component.Markup.Should().Contain("Cancel Booking");
            component.Markup.Should().NotContain("Delete");
        });
    }

    [Fact]
    public void BookingsPage_DefaultView_PreArrivalBooking_ShouldShow_Arrived_Action_And_No_Delete()
    {
        var component = RenderComponent<Bookings>();

        component.WaitForAssertion(() =>
        {
            var items = component.FindAll(".list-group-item");
            items.Should().HaveCount(2);
        });

        component.FindAll(".list-group-item")[0].Click();

        component.WaitForAssertion(() =>
        {
            component.Markup.Should().Contain("Arrived");
            component.Markup.Should().Contain("Mark No-Show");
            component.Markup.Should().Contain("Cancel Booking");
            component.Markup.Should().NotContain("Delete");
        });
    }

    [Fact]
    public void BookingsPage_ViewStays_ShouldShow_CheckOut_Action_OnlyForStageTransition()
    {
        var navigation = Services.GetRequiredService<NavigationManager>();
        navigation.NavigateTo("/bookings?view=stays");

        var component = RenderComponent<Bookings>();

        component.WaitForAssertion(() =>
        {
            var items = component.FindAll(".list-group-item");
            items.Should().HaveCount(1);
        });

        component.FindAll(".list-group-item")[0].Click();

        component.WaitForAssertion(() =>
        {
            component.Markup.Should().Contain("Check Out");
            component.Markup.Should().NotContain("Mark No-Show");
            component.Markup.Should().NotContain("Cancel Booking");
        });
    }
}