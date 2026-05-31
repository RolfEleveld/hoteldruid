using System.Net.Http;
using AngleSharp.Dom;
using Bunit;
using HotelDruid.Client.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using IndexPage = HotelDruid.Client.Pages.Index;

namespace HotelDruid.Client.Tests.Integration.Pages;

public class DashboardBusinessFlowIntegrationTests
{
    private static TestContext CreateContext(object[] bookings, Dictionary<string, string>? settings = null)
    {
        var context = new TestContext();
        context.Services.AddClientLocalizationTestSupport();

        var roomApi = new Mock<IRoomApiService>();
        roomApi.Setup(x => x.GetRoomsAsync()).ReturnsAsync(new List<RoomDto>
        {
            new("room-1", "Room 1", 2, 1, "A", 1, 0, true),
            new("room-2", "Room 2", 1, 1, "A", 2, 0, true),
            new("room-3", "Room 3", 4, 2, "B", 3, 0, true),
            new("room-4", "Room 4", 2, 2, "B", 4, 0, true),
            new("room-5", "Room 5", 2, 3, "C", 5, 0, true)
        });
        context.Services.AddScoped(_ => roomApi.Object);

        var http = new HttpClient(new LandingPageMockHandler(bookings, settings))
        {
            BaseAddress = new Uri("http://localhost/")
        };
        context.Services.AddScoped(_ => http);
        context.JSInterop.Mode = JSRuntimeMode.Loose;

        return context;
    }

    private static IRenderedFragment RenderDashboard(TestContext context) => context.RenderComponent<IndexPage>();

    private static IElement GetWidgetCard(IRenderedFragment component, string titleKey)
    {
        return component.FindAll(".dashboard-widget")
            .First(card => card.TextContent.Contains(titleKey, StringComparison.Ordinal));
    }

    [Fact]
    public void Dashboard_ShouldRender_BusinessFlowWidgets_InExpectedOrder()
    {
        using var context = CreateContext(
        [
            new { id = "r-1", year = 2026, clientId = "client-1", roomId = "room-1", arrivalDate = "2026-06-01", departureDate = "2026-06-03", status = "Confirmed" }
        ]);

        var component = RenderDashboard(context);

        component.WaitForAssertion(() =>
        {
            var titles = component.FindAll(".dashboard-widget .card-header h5")
                .Select(x => x.TextContent.Trim())
                .ToList();

            var reservationsIndex = titles.FindIndex(text => text.Contains("Dashboard.Reservations.Title", StringComparison.Ordinal) || text.Contains("Index.Feature.Reservations.Title", StringComparison.Ordinal));
            var arrivalsIndex = titles.FindIndex(text => text.Contains("Dashboard.Arrivals.Title", StringComparison.Ordinal));
            var staysIndex = titles.FindIndex(text => text.Contains("Dashboard.Stays.Title", StringComparison.Ordinal));
            var departuresIndex = titles.FindIndex(text => text.Contains("Dashboard.Departures.Title", StringComparison.Ordinal));

            Assert.True(reservationsIndex >= 0);
            Assert.True(arrivalsIndex > reservationsIndex);
            Assert.True(staysIndex > arrivalsIndex);
            Assert.True(departuresIndex > staysIndex);
        });
    }

    [Fact]
    public void Dashboard_ShouldClassifyBookings_AcrossReservationsArrivalsStaysDepartures()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        using var context = CreateContext(
        [
            new { id = "res-1", year = 2026, clientId = "client-1", roomId = "room-1", arrivalDate = today.AddDays(3).ToString("yyyy-MM-dd"), departureDate = today.AddDays(6).ToString("yyyy-MM-dd"), status = "Confirmed" },
            new { id = "arr-1", year = 2026, clientId = "client-2", roomId = "room-2", arrivalDate = today.ToString("yyyy-MM-dd"), departureDate = today.AddDays(2).ToString("yyyy-MM-dd"), status = "Confirmed" },
            new { id = "stay-1", year = 2026, clientId = "client-3", roomId = "room-3", arrivalDate = today.AddDays(-2).ToString("yyyy-MM-dd"), departureDate = today.AddDays(2).ToString("yyyy-MM-dd"), actualArrivalDate = today.AddDays(-2).ToString("yyyy-MM-dd"), actualGuestCount = 2, status = "Confirmed" },
            new { id = "dep-1", year = 2026, clientId = "client-4", roomId = "room-4", arrivalDate = today.AddDays(-3).ToString("yyyy-MM-dd"), departureDate = today.ToString("yyyy-MM-dd"), actualArrivalDate = today.AddDays(-3).ToString("yyyy-MM-dd"), actualGuestCount = 2, status = "Confirmed" },
            new { id = "cancelled", year = 2026, clientId = "client-5", roomId = "room-5", arrivalDate = today.ToString("yyyy-MM-dd"), departureDate = today.AddDays(1).ToString("yyyy-MM-dd"), status = "Cancelled" }
        ],
        new Dictionary<string, string>
        {
            ["DashboardArrivalsLookAheadDays"] = "0"
        });

        var component = RenderDashboard(context);

        component.WaitForAssertion(() =>
        {
            var reservations = GetWidgetCard(component, "Index.Feature.Reservations.Title");
            var arrivals = GetWidgetCard(component, "Dashboard.Arrivals.Title");
            var stays = GetWidgetCard(component, "Dashboard.Stays.Title");
            var departures = GetWidgetCard(component, "Dashboard.Departures.Title");

            Assert.Contains("Client, One", reservations.TextContent);
            Assert.DoesNotContain("Client, Three", reservations.TextContent);
            Assert.DoesNotContain("Client, Four", reservations.TextContent);
            Assert.Contains("Client, Two", arrivals.TextContent);
            Assert.Contains("Client, Three", stays.TextContent);
            Assert.Contains("Client, Four", departures.TextContent);
            Assert.DoesNotContain("Client, Five", arrivals.TextContent);
        });
    }

    [Fact]
    public void Dashboard_RowMenus_ShouldExpose_LifecycleBusinessActions()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        using var context = CreateContext(
        [
            new { id = "res-1", year = 2026, clientId = "client-1", roomId = "room-1", arrivalDate = today.AddDays(2).ToString("yyyy-MM-dd"), departureDate = today.AddDays(5).ToString("yyyy-MM-dd"), status = "Confirmed" },
            new { id = "arr-1", year = 2026, clientId = "client-2", roomId = "room-2", arrivalDate = today.ToString("yyyy-MM-dd"), departureDate = today.AddDays(2).ToString("yyyy-MM-dd"), status = "Confirmed" },
            new { id = "stay-1", year = 2026, clientId = "client-3", roomId = "room-3", arrivalDate = today.AddDays(-2).ToString("yyyy-MM-dd"), departureDate = today.AddDays(3).ToString("yyyy-MM-dd"), actualArrivalDate = today.AddDays(-2).ToString("yyyy-MM-dd"), actualGuestCount = 1, status = "Confirmed" },
            new { id = "dep-1", year = 2026, clientId = "client-4", roomId = "room-4", arrivalDate = today.AddDays(-3).ToString("yyyy-MM-dd"), departureDate = today.ToString("yyyy-MM-dd"), actualArrivalDate = today.AddDays(-3).ToString("yyyy-MM-dd"), actualGuestCount = 2, status = "Confirmed" }
        ]);

        var component = RenderDashboard(context);

        component.WaitForAssertion(() =>
        {
            Assert.NotEmpty(component.FindAll("button[title='Dashboard.Reservations.RowMenu']"));
            Assert.NotEmpty(component.FindAll("button[title='Dashboard.Arrivals.RowMenu']"));
            Assert.NotEmpty(component.FindAll("button[title='Dashboard.Stays.RowMenu']"));
            Assert.NotEmpty(component.FindAll("button[title='Dashboard.Departures.RowMenu']"));
        });

        component.FindAll("button[title='Dashboard.Reservations.RowMenu']")[0].Click();
        component.WaitForAssertion(() =>
        {
            var items = component.FindAll(".border-primary .dropdown-menu.show .dropdown-item").Select(x => x.TextContent.Trim()).ToList();
            Assert.Contains("Dashboard.Reservations.Menu.Arrived", items);
            Assert.Contains("Dashboard.Stays.Action.OpenClient", items);
        });

        component.FindAll("button[title='Dashboard.Arrivals.RowMenu']")[0].Click();
        component.WaitForAssertion(() =>
        {
            var items = component.FindAll(".border-secondary .dropdown-menu.show .dropdown-item").Select(x => x.TextContent.Trim()).ToList();
            Assert.Contains("Dashboard.Reservations.Menu.Arrived", items);
            Assert.Contains("Dashboard.Stays.Action.AddGuests", items);
        });

        component.FindAll("button[title='Dashboard.Stays.RowMenu']")[0].Click();
        component.WaitForAssertion(() =>
        {
            var items = component.FindAll(".border-danger .dropdown-menu.show .dropdown-item").Select(x => x.TextContent.Trim()).ToList();
            Assert.Contains("Dashboard.Stays.Action.AddCosts", items);
            Assert.Contains("Dashboard.Stays.Action.OpenClient", items);
        });

        component.FindAll("button[title='Dashboard.Departures.RowMenu']")[0].Click();
        component.WaitForAssertion(() =>
        {
            var items = component.FindAll(".border-dark .dropdown-menu.show .dropdown-item").Select(x => x.TextContent.Trim()).ToList();
            Assert.Contains("Dashboard.Stays.Action.OpenReservation", items);
            Assert.Contains("Dashboard.Stays.Action.AddCosts", items);
        });
    }

    [Fact]
    public void Dashboard_VisualStructure_ShouldKeepSettingsAndSubmenusAligned()
    {
        using var context = CreateContext(
        [
            new { id = "r-1", year = 2026, clientId = "client-1", roomId = "room-1", arrivalDate = "2026-06-01", departureDate = "2026-06-03", status = "Confirmed" }
        ]);

        var component = RenderDashboard(context);

        component.Find("button[title='Dashboard.Settings.Menu']").Click();
        component.WaitForAssertion(() =>
        {
            var settingsDropdown = component.FindAll(".dashboard-dropdown.dashboard-dropdown-start.show");
            Assert.NotEmpty(settingsDropdown);
        });

        component.FindAll("button[title='Dashboard.Widget.Menu']")[0].Click();
        component.WaitForAssertion(() =>
        {
            var widgetDropdowns = component.FindAll(".dashboard-dropdown.dropdown-menu-end.show");
            Assert.NotEmpty(widgetDropdowns);
        });

        component.FindAll("button[title='Dashboard.Reservations.RowMenu']")[0].Click();
        component.WaitForAssertion(() =>
        {
            var rowDropdowns = component.FindAll(".border-primary .dashboard-dropdown.dropdown-menu-end.show");
            Assert.NotEmpty(rowDropdowns);
        });
    }

    [Fact]
    public void Dashboard_ArrivalsLookAheadSetting_ShouldControlPendingArrivalsWindow()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        using var context = CreateContext(
        [
            new { id = "arr-tomorrow", year = 2026, clientId = "client-2", roomId = "room-2", arrivalDate = today.AddDays(1).ToString("yyyy-MM-dd"), departureDate = today.AddDays(3).ToString("yyyy-MM-dd"), status = "Confirmed" }
        ],
        new Dictionary<string, string>
        {
            ["DashboardArrivalsLookAheadDays"] = "1"
        });

        var component = RenderDashboard(context);

        component.WaitForAssertion(() =>
        {
            var arrivals = GetWidgetCard(component, "Dashboard.Arrivals.Title");
            Assert.Contains("Client, Two", arrivals.TextContent);
        });
    }
}
