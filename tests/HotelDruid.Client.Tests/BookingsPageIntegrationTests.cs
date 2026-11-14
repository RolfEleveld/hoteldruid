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
    private readonly Mock<IBookingFinanceApiService> financeApi = new();
    private readonly List<BookingDto> sampleBookings;

    public BookingsPageIntegrationTests()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        sampleBookings = new List<BookingDto>
        {
            new("b-0", 2026, "client-1", "room-1", new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 3), "Confirmed", "Older booking"),
            new("b-2", 2026, "client-2", "room-2", new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 3), "Pending", "Second booking"),
            new("b-1", 2026, "client-1", "room-1", new DateOnly(2026, 5, 10), new DateOnly(2026, 5, 12), "Confirmed", "First booking"),
            new("b-today", today.Year, "client-1", "room-1", today, today.AddDays(2), "Confirmed", "Today stay")
        };

        Services.AddClientLocalizationTestSupport();
        activeYearService.SetupGet(x => x.CurrentYear).Returns(2026);
        activeYearService.Setup(x => x.InitializeAsync()).Returns(Task.CompletedTask);
        activeYearService.Setup(x => x.SetActiveYearAsync(It.IsAny<int>(), It.IsAny<bool>())).Returns(Task.CompletedTask);

        bookingApi.Setup(x => x.ListAsync(It.IsAny<int?>(), It.IsAny<bool>(), It.IsAny<DateOnly?>(), It.IsAny<DateOnly?>(), It.IsAny<string?>()))
            .ReturnsAsync((int? year, bool includeDeleted, DateOnly? from, DateOnly? to, string? clientId) =>
                FilterBookings(year, from, to, clientId));
        bookingApi.Setup(x => x.GetByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((string id) => sampleBookings.FirstOrDefault(booking => string.Equals(booking.Id, id, StringComparison.OrdinalIgnoreCase)));

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
            new("dep-1", 2026, "client-1", "room-1", new DateOnly(2026, 8, 6), new DateOnly(2026, 8, 8), "Confirmed", "Departure", new DateOnly(2026, 8, 6), 2, null, null, null, new DateOnly(2026, 8, 8), null, null, null, null, null)
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

        financeApi.Setup(x => x.GetSummaryAsync(It.IsAny<int>(), It.IsAny<string>()))
            .ReturnsAsync((int year, string bookingId) => new BookingFinanceSummaryDto(
                bookingId, year, 96m, 0m, 96m, 0m, 0m, 0m, 0m, 96m, "EUR", null, null, 1));
        financeApi.Setup(x => x.GetDocumentUrl(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns((int year, string bookingId, string documentType, string format) =>
                $"/api/documents/render/{documentType}/1?bookingId={bookingId}&year={year}&format={format}");

        Services.AddScoped(_ => bookingApi.Object);
        Services.AddScoped(_ => clientApi.Object);
        Services.AddScoped(_ => roomApi.Object);
        Services.AddScoped(_ => activeYearService.Object);
        Services.AddScoped(_ => financeApi.Object);
    }

    [Fact]
    public void BookingsPage_ShouldOpenOnTheDeskDateWindow()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var component = Render<Bookings>();

        component.WaitForAssertion(() =>
        {
            var items = component.FindAll(".list-group-item");
            items.Should().HaveCount(BookingsInRange(today, today).Count);
            items[0].TextContent.Should().Contain("Adams Anna");
            items[0].TextContent.Should().Contain($"{today:yyyy-MM-dd} · Room 1");
        });

        var dates = component.FindAll("input[type='date']");
        dates[0].GetAttribute("value").Should().Be(today.ToString("yyyy-MM-dd"));
        dates[1].GetAttribute("value").Should().Be(today.ToString("yyyy-MM-dd"));
        bookingApi.Verify(x => x.ListAsync(null, false, today, today, null), Times.AtLeastOnce);

        component.FindAll(".list-group-item")[0].Click();

        component.WaitForAssertion(() =>
        {
            component.FindAll("input[type='date']").Should().HaveCount(6);
        });

        component.Markup.Should().Contain("Actual Checkout Date");
        component.Markup.Should().Contain("Guest Details");
        component.Markup.Should().Contain("Other Bookings");
        component.Markup.Should().Contain("/clients?selectedId=client-1");
    }

    [Fact]
    public void BookingsPage_Load_ShouldAskTheBackendForTheSelectedDates()
    {
        var component = Render<Bookings>();
        component.WaitForAssertion(() => component.FindAll(".list-group-item").Should().NotBeEmpty());

        component.FindAll("input[type='date']")[0].Change("2026-05-01");
        component.FindAll("input[type='date']")[1].Change("2026-06-30");
        component.FindAll("button").First(button => button.TextContent.Contains("Load")).Click();

        component.WaitForAssertion(() =>
        {
            var items = component.FindAll(".list-group-item");
            items.Should().HaveCount(3);
            items[0].TextContent.Should().Contain("2026-05-01 · Room 1");
            items[1].TextContent.Should().Contain("2026-05-10 · Room 1");
            items[2].TextContent.Should().Contain("2026-06-01 · Room 2");
        });

        bookingApi.Verify(x => x.ListAsync(null, false, new DateOnly(2026, 5, 1), new DateOnly(2026, 6, 30), null), Times.AtLeastOnce);
    }

    [Fact]
    public void BookingsPage_UrlDates_ShouldLoadThatRangeFromTheBackend()
    {
        var navigation = Services.GetRequiredService<NavigationManager>();
        navigation.NavigateTo("/bookings?from=2026-05-01&to=2026-06-30");

        var component = Render<Bookings>();

        component.WaitForAssertion(() =>
        {
            var items = component.FindAll(".list-group-item");
            items.Should().HaveCount(3);
            items[0].TextContent.Should().Contain("2026-05-01 · Room 1");
        });

        bookingApi.Verify(x => x.ListAsync(null, false, new DateOnly(2026, 5, 1), new DateOnly(2026, 6, 30), null), Times.AtLeastOnce);
        bookingApi.Verify(x => x.ListStaysAsync(It.IsAny<int?>()), Times.Never);
    }

    [Fact]
    public void BookingsPage_SelectedId_ShouldHighlightThatStayAndUseItsDates()
    {
        var navigation = Services.GetRequiredService<NavigationManager>();
        navigation.NavigateTo("/bookings?selectedId=b-0");

        var component = Render<Bookings>();

        component.WaitForAssertion(() =>
        {
            var selected = component.FindAll(".list-group-item.active");
            selected.Should().ContainSingle();
            selected[0].TextContent.Should().Contain("Adams Anna");
            selected[0].TextContent.Should().Contain("2026-05-01 · Room 1");
        });

        var dates = component.FindAll("input[type='date']");
        dates[0].GetAttribute("value").Should().Be("2026-05-01");
        dates[1].GetAttribute("value").Should().Be("2026-05-03");
        bookingApi.Verify(x => x.ListAsync(null, false, new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 3), null), Times.AtLeastOnce);
        bookingApi.Verify(x => x.ListAsync(null, false, DateOnly.FromDateTime(DateTime.Today), DateOnly.FromDateTime(DateTime.Today), null), Times.Never);
    }

    [Fact]
    public void BookingsNewRoute_ShouldPrefillRoom_WhenRoomIdQueryIsProvided()
    {
        var navigation = Services.GetRequiredService<NavigationManager>();
        navigation.NavigateTo("/bookings/new?roomId=room-77");

        var component = Render<Bookings>();

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

        var component = Render<Bookings>();

        component.WaitForAssertion(() =>
        {
            var items = component.FindAll(".list-group-item");
            items.Should().HaveCount(1);
            items[0].TextContent.Should().Contain("Adams Anna");
            items[0].TextContent.Should().Contain("2026-08-06 · Room 1");
        });

        bookingApi.Verify(x => x.ListStaysAsync(null), Times.AtLeastOnce);
    }

    [Fact]
    public void BookingsPage_ViewArrivals_ShouldUseArrivalsApiForLeftColumn()
    {
        var navigation = Services.GetRequiredService<NavigationManager>();
        navigation.NavigateTo("/bookings?view=arrivals&lookAheadDays=2");

        var component = Render<Bookings>();

        component.WaitForAssertion(() =>
        {
            var items = component.FindAll(".list-group-item");
            items.Should().HaveCount(1);
            items[0].TextContent.Should().Contain("Brown Ben");
            items[0].TextContent.Should().Contain("2026-08-08 · Room 2");
        });

        bookingApi.Verify(x => x.ListArrivalsAsync(null, 2), Times.AtLeastOnce);
    }

    [Fact]
    public void BookingsPage_ViewDepartures_ShouldUseDeparturesApiForLeftColumn()
    {
        var navigation = Services.GetRequiredService<NavigationManager>();
        navigation.NavigateTo("/bookings?view=departures");

        var component = Render<Bookings>();

        component.WaitForAssertion(() =>
        {
            var items = component.FindAll(".list-group-item");
            items.Should().HaveCount(1);
            items[0].TextContent.Should().Contain("Adams Anna");
            items[0].TextContent.Should().Contain("2026-08-06 · Room 1");
        });

        bookingApi.Verify(x => x.ListDeparturesAsync(null), Times.AtLeastOnce);
    }

    [Fact]
    public void BookingsPage_ViewReservations_ShouldUseReservationsApiForLeftColumn()
    {
        var navigation = Services.GetRequiredService<NavigationManager>();
        navigation.NavigateTo("/bookings?view=reservations");

        var component = Render<Bookings>();

        component.WaitForAssertion(() =>
        {
            var items = component.FindAll(".list-group-item");
            items.Should().HaveCount(1);
            items[0].TextContent.Should().Contain("Brown Ben");
            items[0].TextContent.Should().Contain("2026-08-11 · Room 2");
        });

        bookingApi.Verify(x => x.ListReservationsAsync(null), Times.AtLeastOnce);
    }

    [Fact]
    public void BookingsPage_ViewArrivals_ShouldShow_Arrived_And_NoShow_Actions()
    {
        var navigation = Services.GetRequiredService<NavigationManager>();
        navigation.NavigateTo("/bookings?view=arrivals&lookAheadDays=2");

        var component = Render<Bookings>();

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
        var component = Render<Bookings>();

        component.WaitForAssertion(() =>
        {
            component.FindAll(".list-group-item").Should().HaveCount(BookingsInRange(DateOnly.FromDateTime(DateTime.Today), DateOnly.FromDateTime(DateTime.Today)).Count);
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

        var component = Render<Bookings>();

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

    [Fact]
    public void BookingsPage_TotalDue_ShouldOpenFinanceHistory()
    {
        var navigation = Services.GetRequiredService<NavigationManager>();
        var component = Render<Bookings>();
        var today = DateOnly.FromDateTime(DateTime.Today);
        var opened = BookingsInRange(today, today)[0];

        component.WaitForAssertion(() => component.FindAll(".list-group-item").Should().NotBeEmpty());
        component.FindAll(".list-group-item")[0].Click();

        component.WaitForAssertion(() => component.Markup.Should().Contain("Total due"));
        component.Find("button.form-control").Click();

        navigation.Uri.Should().Contain($"/booking-finance?selectedId={opened.Id}&year={opened.Year}");
        component.Markup.Should().Contain("Invoice");
    }

    private List<BookingDto> BookingsInRange(DateOnly from, DateOnly to)
        => FilterBookings(null, from, to, null)
            .OrderBy(booking => booking.ArrivalDate ?? DateOnly.MinValue)
            .ThenBy(booking => booking.Id, StringComparer.OrdinalIgnoreCase)
            .ToList();

    private List<BookingDto> FilterBookings(int? year, DateOnly? from, DateOnly? to, string? clientId)
    {
        IEnumerable<BookingDto> result = sampleBookings;
        if (!string.IsNullOrWhiteSpace(clientId))
        {
            result = result.Where(booking => string.Equals(booking.ClientId, clientId, StringComparison.OrdinalIgnoreCase));
        }
        else if (from.HasValue && to.HasValue)
        {
            result = result.Where(booking => StayOverlaps(booking, from.Value, to.Value));
        }
        else if (year.HasValue)
        {
            result = result.Where(booking => booking.Year == year.Value);
        }

        return result.ToList();
    }

    private static bool StayOverlaps(BookingDto booking, DateOnly from, DateOnly to)
    {
        var arrival = booking.ArrivalDate ?? booking.ActualArrivalDate;
        if (arrival is null)
        {
            return booking.Year >= from.Year && booking.Year <= to.Year;
        }

        var departure = booking.DepartureDate ?? booking.ActualDepartureDate ?? arrival.Value;
        return arrival.Value <= to && departure >= from;
    }
}