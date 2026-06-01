using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Bunit;
using HotelDruid.Client.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using IndexPage = HotelDruid.Client.Pages.Index;

namespace HotelDruid.Client.Tests.Integration.Pages;

internal sealed class LandingPageMockHandler : HttpMessageHandler
{
    private readonly object[] bookings;
    private readonly Dictionary<string, string> settings;
    private readonly bool failClientsRequest;

    public LandingPageMockHandler(object[]? bookings = null, Dictionary<string, string>? settings = null, bool failClientsRequest = false)
    {
        this.bookings = bookings ??
        [
            new { id = "1", year = 2026, clientId = "client-1", roomId = "room-1", arrivalDate = "2026-06-01", departureDate = "2026-06-03", status = "Confirmed" },
            new { id = "2", year = 2026, clientId = "client-2", roomId = "room-2", arrivalDate = "2026-06-04", departureDate = "2026-06-06", status = "Pending" },
            new { id = "3", year = 2026, clientId = "client-3", roomId = "room-3", arrivalDate = "2026-06-07", departureDate = "2026-06-09", status = "Confirmed" },
            new { id = "4", year = 2026, clientId = "client-4", roomId = "room-4", arrivalDate = "2026-06-10", departureDate = "2026-06-12", status = "Pending" },
            new { id = "5", year = 2026, clientId = "client-5", roomId = "room-5", arrivalDate = "2026-06-13", departureDate = "2026-06-15", status = "Confirmed" }
        ];

        this.settings = settings ?? new Dictionary<string, string>();
        this.failClientsRequest = failClientsRequest;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var path = request.RequestUri?.AbsolutePath ?? string.Empty;
        var method = request.Method;

        if (method == HttpMethod.Get && path.Equals("/api/system/setup/status", StringComparison.OrdinalIgnoreCase))
        {
            return JsonResponse(new { requiresSetup = false, reason = "configured" });
        }

        if (method == HttpMethod.Get && path.Equals("/api/status", StringComparison.OrdinalIgnoreCase))
        {
            return JsonResponse(new { activeYear = "2026", user = "admin", version = "HotelDruid 3.0.7", versionLabel = "HotelDruid 3.0.7. Blazor", buildNumber = "local" });
        }

        if (method == HttpMethod.Get && path.Equals("/api/clients", StringComparison.OrdinalIgnoreCase))
        {
            if (failClientsRequest)
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("{\"error\":\"simulated failure\"}", Encoding.UTF8, "application/json")
                });
            }

            return JsonResponse(new[]
            {
                new { id = "client-1", lastName = "Client", firstName = "One" },
                new { id = "client-2", lastName = "Client", firstName = "Two" },
                new { id = "client-3", lastName = "Client", firstName = "Three" },
                new { id = "client-4", lastName = "Client", firstName = "Four" },
                new { id = "client-5", lastName = "Client", firstName = "Five" }
            });
        }

        if (method == HttpMethod.Get && path.Equals("/api/bookings", StringComparison.OrdinalIgnoreCase))
        {
            return JsonResponse(bookings);
        }

        if (method == HttpMethod.Get && path.Equals("/api/system/configuration", StringComparison.OrdinalIgnoreCase))
        {
            return JsonResponse(new
            {
                id = "system",
                defaultCurrency = "EUR",
                defaultYear = 2026,
                settings = settings
            });
        }

        if (method == HttpMethod.Put && path.Equals("/api/system/configuration", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{}", Encoding.UTF8, "application/json")
            });
        }

        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
    }

    private static Task<HttpResponseMessage> JsonResponse(object payload)
    {
        var json = JsonSerializer.Serialize(payload);
        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        });
    }
}

public class LandingPageWidgetSelectorTests : TestContext
{
    private const int CalendarWidgetMenuIndex = 6;

    private static readonly string[] ExpectedVisibleCountHeaderKeys =
    [
        "Dashboard.Widget.VisibleCount.Reservations",
        "Dashboard.Widget.VisibleCount.Arrivals",
        "Dashboard.Widget.VisibleCount.Stays",
        "Dashboard.Widget.VisibleCount.Departures",
        "Dashboard.Widget.VisibleCount.Clients",
        "Dashboard.Widget.VisibleCount.Rooms",
        "Dashboard.Widget.VisibleCount.Inventory",
        "Dashboard.Widget.VisibleCount.Warehouses",
        "Dashboard.Widget.VisibleCount.Reports"
    ];

    private static readonly string[] ExpectedAddActionKeys =
    [
        "Index.QuickAction.NewReservation",
        "Index.QuickAction.NewReservation",
        "Index.QuickAction.NewStay",
        "Index.QuickAction.NewReservation",
        "Index.QuickAction.NewClient",
        "DashboardRooms.Menu.AddRoom",
        "Dashboard.Widget.Menu.AddYear",
        "Dashboard.Widget.Menu.AddInventoryItem",
        "Dashboard.Widget.Menu.AddWarehouse",
        "Dashboard.Widget.Menu.AddReportTemplate"
    ];

    public LandingPageWidgetSelectorTests()
    {
        Services.AddClientLocalizationTestSupport();

        var roomApi = new Mock<IRoomApiService>();
        roomApi.Setup(x => x.GetRoomsAsync()).ReturnsAsync(new List<RoomDto>
        {
            new("room-1", "Room 1", 2, 1, "A", 1, 0, true),
            new("room-2", "Room 2", 2, 1, "A", 2, 0, true),
            new("room-3", "Room 3", 2, 1, "A", 3, 0, true),
            new("room-4", "Room 4", 2, 1, "A", 4, 0, true),
            new("room-5", "Room 5", 2, 1, "A", 5, 0, true),
            new("room-6", "Room 6", 2, 1, "A", 6, 0, true),
            new("room-7", "Room 7", 2, 1, "A", 7, 0, true),
            new("room-8", "Room 8", 2, 1, "A", 8, 0, true)
        });
        Services.AddScoped(_ => roomApi.Object);

        var client = new HttpClient(new LandingPageMockHandler()) { BaseAddress = new Uri("http://localhost/") };
        Services.AddScoped(_ => client);

        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void WidgetMenus_ShouldShow_EntitySpecificVisibleHeader_AndAddAction()
    {
        var component = RenderComponent<IndexPage>();

        component.WaitForAssertion(() =>
        {
            var menuButtons = component.FindAll("button[title='Dashboard.Widget.Menu']");
            Assert.Equal(10, menuButtons.Count);
        });

        for (var i = 0; i < ExpectedAddActionKeys.Length; i++)
        {
            var buttons = component.FindAll("button[title='Dashboard.Widget.Menu']");
            buttons[i].Click();

            component.WaitForAssertion(() =>
            {
                var headers = component.FindAll(".dropdown-menu.show .dropdown-header")
                    .Select(x => x.TextContent.Trim())
                    .ToList();

                var items = component.FindAll(".dropdown-menu.show .dropdown-item")
                    .Select(x => x.TextContent.Trim())
                    .ToList();

                Assert.Contains(ExpectedAddActionKeys[i], items);

                if (i == CalendarWidgetMenuIndex)
                {
                    Assert.DoesNotContain("Dashboard.Widget.VisibleCount.Calendar", headers);
                }
                else
                {
                    var expectedHeaderIndex = i > CalendarWidgetMenuIndex ? i - 1 : i;
                    Assert.Contains(ExpectedVisibleCountHeaderKeys[expectedHeaderIndex], headers);
                }
            });
        }
    }

    [Fact]
    public void WidgetMenus_ShouldExpose_ConsistentVisibleCountOptions_OnAllWidgets()
    {
        var component = RenderComponent<IndexPage>();

        component.WaitForAssertion(() =>
        {
            var menuButtons = component.FindAll("button[title='Dashboard.Widget.Menu']");
            Assert.Equal(10, menuButtons.Count);
        });

        var buttons = component.FindAll("button[title='Dashboard.Widget.Menu']");
        for (var i = 0; i < buttons.Count; i++)
        {
            buttons = component.FindAll("button[title='Dashboard.Widget.Menu']");
            buttons[i].Click();

            component.WaitForAssertion(() =>
            {
                var items = component.FindAll(".dropdown-menu.show .dropdown-item")
                    .Select(x => x.TextContent.Trim())
                    .ToList();

                Assert.Contains("3", items);
                Assert.Contains("5", items);
                Assert.Contains("7", items);
                Assert.Contains("10", items);
                Assert.Contains("15", items);
            });
        }
    }

    [Fact]
    public void RoomsWidget_ShouldRespect_SelectedVisibleCountOption()
    {
        var component = RenderComponent<IndexPage>();

        component.WaitForAssertion(() =>
        {
            var roomRows = component.FindAll(".actionable-room-list .list-group-item");
            Assert.Equal(3, roomRows.Count);
        });

        var menuButtons = component.FindAll("button[title='Dashboard.Widget.Menu']");
        menuButtons[5].Click();

        component.WaitForAssertion(() =>
        {
            var option = component.FindAll(".dropdown-menu.show .dropdown-item")
                .First(x => x.TextContent.Trim() == "7");
            option.Click();
        });

        component.WaitForAssertion(() =>
        {
            var roomRows = component.FindAll(".actionable-room-list .list-group-item");
            Assert.Equal(7, roomRows.Count);
        });
    }

    [Fact]
    public void ReservationsWidget_ShouldRenderBookings_FromApiPayloadWithStringClientIds()
    {
        var component = RenderComponent<IndexPage>();

        component.WaitForAssertion(() =>
        {
            var bookingRows = component.FindAll(".border-primary .list-group-item");
            Assert.NotEmpty(bookingRows);
            Assert.Contains(bookingRows, row => row.TextContent.Contains("Client, Five", StringComparison.Ordinal));
            Assert.Contains(bookingRows, row => row.TextContent.Contains("2026-06-13 -> 2026-06-15", StringComparison.Ordinal));
        });
    }

    [Fact]
    public void ReservationsWidget_RowMenuShouldOffer_ArrivedAndClientActions()
    {
        var component = RenderComponent<IndexPage>();

        component.WaitForAssertion(() =>
        {
            var menuButtons = component.FindAll("button[title='Dashboard.Reservations.RowMenu']");
            Assert.NotEmpty(menuButtons);
        });

        var reservationMenuButton = component.FindAll("button[title='Dashboard.Reservations.RowMenu']")[0];
        reservationMenuButton.Click();

        component.WaitForAssertion(() =>
        {
            var items = component.FindAll(".border-primary .dropdown-menu.show .dropdown-item")
                .Select(x => x.TextContent.Trim())
                .ToList();

            Assert.Contains("Dashboard.Reservations.Menu.Arrived", items);
            Assert.Contains("Dashboard.Stays.Action.OpenClient", items);
        });
    }

    [Fact]
    public void StaysWidget_ShouldShowOnly_CurrentOccupiedRooms()
    {
        using var localContext = new TestContext();
        localContext.Services.AddClientLocalizationTestSupport();

        var roomApi = new Mock<IRoomApiService>();
        roomApi.Setup(x => x.GetRoomsAsync()).ReturnsAsync(new List<RoomDto>
        {
            new("room-1", "Room 1", 2, 1, "A", 1, 0, true),
            new("room-2", "Room 2", 2, 1, "A", 2, 0, true),
            new("room-3", "Room 3", 2, 1, "A", 3, 0, true)
        });
        localContext.Services.AddScoped(_ => roomApi.Object);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var client = new HttpClient(new LandingPageMockHandler(
        [
            new { id = "b-1", year = 2026, clientId = "client-1", roomId = "room-1", arrivalDate = today.AddDays(-1).ToString("yyyy-MM-dd"), departureDate = today.AddDays(2).ToString("yyyy-MM-dd"), actualArrivalDate = today.AddDays(-1).ToString("yyyy-MM-dd"), actualGuestCount = 2, status = "Confirmed" },
            new { id = "b-2", year = 2026, clientId = "client-2", roomId = "room-2", arrivalDate = today.AddDays(3).ToString("yyyy-MM-dd"), departureDate = today.AddDays(5).ToString("yyyy-MM-dd"), status = "Confirmed" },
            new { id = "b-3", year = 2026, clientId = "client-3", roomId = "room-3", arrivalDate = today.AddDays(-2).ToString("yyyy-MM-dd"), departureDate = today.AddDays(1).ToString("yyyy-MM-dd"), actualArrivalDate = today.AddDays(-2).ToString("yyyy-MM-dd"), actualGuestCount = 1, status = "Cancelled" }
        ]))
        { BaseAddress = new Uri("http://localhost/") };
        localContext.Services.AddScoped(_ => client);
        localContext.JSInterop.Mode = JSRuntimeMode.Loose;

        var component = localContext.RenderComponent<IndexPage>();

        component.WaitForAssertion(() =>
        {
            var stayRows = component.FindAll(".border-danger .list-group-item");
            Assert.Single(stayRows);
            Assert.Contains("Room 1", stayRows[0].TextContent);
            Assert.DoesNotContain("Room 2", stayRows[0].TextContent);
        });
    }

    [Fact]
    public void StaysWidget_RowMenuShouldOffer_StandardStayActions()
    {
        using var localContext = new TestContext();
        localContext.Services.AddClientLocalizationTestSupport();

        var roomApi = new Mock<IRoomApiService>();
        roomApi.Setup(x => x.GetRoomsAsync()).ReturnsAsync(new List<RoomDto>
        {
            new("room-1", "Room 1", 2, 1, "A", 1, 0, true)
        });
        localContext.Services.AddScoped(_ => roomApi.Object);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var client = new HttpClient(new LandingPageMockHandler(
        [
            new { id = "b-1", year = 2026, clientId = "client-1", roomId = "room-1", arrivalDate = today.AddDays(-1).ToString("yyyy-MM-dd"), departureDate = today.AddDays(2).ToString("yyyy-MM-dd"), actualArrivalDate = today.AddDays(-1).ToString("yyyy-MM-dd"), actualGuestCount = 2, status = "Confirmed" }
        ]))
        { BaseAddress = new Uri("http://localhost/") };
        localContext.Services.AddScoped(_ => client);
        localContext.JSInterop.Mode = JSRuntimeMode.Loose;

        var component = localContext.RenderComponent<IndexPage>();

        component.WaitForAssertion(() =>
        {
            var menuButtons = component.FindAll("button[title='Dashboard.Stays.RowMenu']");
            Assert.NotEmpty(menuButtons);
        });

        component.FindAll("button[title='Dashboard.Stays.RowMenu']")[0].Click();

        component.WaitForAssertion(() =>
        {
            var items = component.FindAll(".border-danger .dropdown-menu.show .dropdown-item")
                .Select(x => x.TextContent.Trim())
                .ToList();

            Assert.Contains("Dashboard.Stays.Action.OpenReservation", items);
            Assert.Contains("Dashboard.Stays.Action.AddCosts", items);
            Assert.Contains("Dashboard.Stays.Action.AddGuests", items);
            Assert.Contains("Dashboard.Stays.Action.OpenClient", items);
        });
    }

    [Fact]
    public void ArrivalsWidget_RowMenuShouldOffer_ArrivalActions()
    {
        using var localContext = new TestContext();
        localContext.Services.AddClientLocalizationTestSupport();

        var roomApi = new Mock<IRoomApiService>();
        roomApi.Setup(x => x.GetRoomsAsync()).ReturnsAsync(new List<RoomDto>
        {
            new("room-1", "Room 1", 2, 1, "A", 1, 0, true)
        });
        localContext.Services.AddScoped(_ => roomApi.Object);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var client = new HttpClient(new LandingPageMockHandler(
        [
            new { id = "b-1", year = 2026, clientId = "client-1", roomId = "room-1", arrivalDate = today.ToString("yyyy-MM-dd"), departureDate = today.AddDays(2).ToString("yyyy-MM-dd"), status = "Confirmed" }
        ]))
        { BaseAddress = new Uri("http://localhost/") };
        localContext.Services.AddScoped(_ => client);
        localContext.JSInterop.Mode = JSRuntimeMode.Loose;

        var component = localContext.RenderComponent<IndexPage>();

        component.WaitForAssertion(() =>
        {
            var menuButtons = component.FindAll("button[title='Dashboard.Arrivals.RowMenu']");
            Assert.NotEmpty(menuButtons);
        });

        component.FindAll("button[title='Dashboard.Arrivals.RowMenu']")[0].Click();

        component.WaitForAssertion(() =>
        {
            var items = component.FindAll(".border-secondary .dropdown-menu.show .dropdown-item")
                .Select(x => x.TextContent.Trim())
                .ToList();

            Assert.Contains("Dashboard.Reservations.Menu.Arrived", items);
            Assert.Contains("Dashboard.Stays.Action.OpenClient", items);
        });
    }

    [Fact]
    public void DeparturesWidget_ShouldShowOnly_TodaysDeparturesAndMenuActions()
    {
        using var localContext = new TestContext();
        localContext.Services.AddClientLocalizationTestSupport();

        var roomApi = new Mock<IRoomApiService>();
        roomApi.Setup(x => x.GetRoomsAsync()).ReturnsAsync(new List<RoomDto>
        {
            new("room-1", "Room 1", 2, 1, "A", 1, 0, true),
            new("room-2", "Room 2", 2, 1, "A", 2, 0, true)
        });
        localContext.Services.AddScoped(_ => roomApi.Object);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var client = new HttpClient(new LandingPageMockHandler(
        [
            new { id = "b-1", year = 2026, clientId = "client-1", roomId = "room-1", arrivalDate = today.AddDays(-2).ToString("yyyy-MM-dd"), departureDate = today.ToString("yyyy-MM-dd"), actualArrivalDate = today.AddDays(-2).ToString("yyyy-MM-dd"), actualGuestCount = 2, status = "Confirmed" },
            new { id = "b-2", year = 2026, clientId = "client-2", roomId = "room-2", arrivalDate = today.AddDays(-1).ToString("yyyy-MM-dd"), departureDate = today.AddDays(1).ToString("yyyy-MM-dd"), actualArrivalDate = today.AddDays(-1).ToString("yyyy-MM-dd"), actualGuestCount = 1, status = "Confirmed" }
        ]))
        { BaseAddress = new Uri("http://localhost/") };
        localContext.Services.AddScoped(_ => client);
        localContext.JSInterop.Mode = JSRuntimeMode.Loose;

        var component = localContext.RenderComponent<IndexPage>();

        component.WaitForAssertion(() =>
        {
            var departureRows = component.FindAll(".border-dark .list-group-item");
            Assert.Single(departureRows);
            Assert.Contains("Room 1", departureRows[0].TextContent);
        });

        component.FindAll("button[title='Dashboard.Departures.RowMenu']")[0].Click();

        component.WaitForAssertion(() =>
        {
            var items = component.FindAll(".border-dark .dropdown-menu.show .dropdown-item")
                .Select(x => x.TextContent.Trim())
                .ToList();

            Assert.Contains("Dashboard.Stays.Action.OpenReservation", items);
            Assert.Contains("Dashboard.Stays.Action.AddCosts", items);
            Assert.Contains("Dashboard.Stays.Action.OpenClient", items);
        });
    }

    [Fact]
    public void RoomsWidget_MenuShouldOffer_NewReservationForSelectedRoom()
    {
        var component = RenderComponent<IndexPage>();

        component.WaitForAssertion(() =>
        {
            var roomRows = component.FindAll(".actionable-room-list .list-group-item");
            Assert.NotEmpty(roomRows);
        });

        var menuButtons = component.FindAll("button[title='DashboardRooms.RoomMenu']");
        menuButtons[0].Click();

        component.WaitForAssertion(() =>
        {
            var items = component.FindAll(".actionable-room-list .dropdown-menu.show .dropdown-item")
                .Select(x => x.TextContent.Trim())
                .ToList();

            Assert.Contains("DashboardRooms.Menu.NewReservation", items);
        });
    }

    [Fact]
    public void RoomsWidget_ShouldRenderRooms_WhenBookingsHaveNoRoomOrDates()
    {
        using var localContext = new TestContext();
        localContext.Services.AddClientLocalizationTestSupport();

        var roomApi = new Mock<IRoomApiService>();
        roomApi.Setup(x => x.GetRoomsAsync()).ReturnsAsync(new List<RoomDto>
        {
            new("room-1", "Room 1", 2, 1, "A", 1, 0, false),
            new("room-2", "Room 2", 2, 1, "A", 2, 0, false),
            new("room-3", "Room 3", 2, 1, "A", 3, 0, true),
            new("room-4", "Room 4", 2, 1, "A", 4, 0, false),
            new("room-5", "Room 5", 2, 1, "A", 5, 0, true),
            new("room-6", "Room 6", 2, 1, "A", 6, 0, false)
        });
        localContext.Services.AddScoped(_ => roomApi.Object);

        var client = new HttpClient(new LandingPageMockHandler(
        [
            new { id = "b-1", year = 2026, clientId = "client-1", status = "Confirmed", roomId = (string?)null, arrivalDate = (string?)null, departureDate = (string?)null },
            new { id = "b-2", year = 2026, clientId = "client-2", status = "Confirmed", roomId = "", arrivalDate = "", departureDate = "" }
        ]))
        { BaseAddress = new Uri("http://localhost/") };
        localContext.Services.AddScoped(_ => client);
        localContext.JSInterop.Mode = JSRuntimeMode.Loose;

        var component = localContext.RenderComponent<IndexPage>();

        component.WaitForAssertion(() =>
        {
            var roomRows = component.FindAll(".actionable-room-list .list-group-item");
            Assert.Equal(3, roomRows.Count);
            var text = component.Markup;
            Assert.Contains("Room 1", text);
            Assert.Contains("Room 2", text);
            Assert.Contains("Room 3", text);
        });
    }

    [Fact]
    public void RoomsWidget_ShouldHideRoom_WhenActiveReservationHasRoomAndDates()
    {
        using var localContext = new TestContext();
        localContext.Services.AddClientLocalizationTestSupport();

        var roomApi = new Mock<IRoomApiService>();
        roomApi.Setup(x => x.GetRoomsAsync()).ReturnsAsync(new List<RoomDto>
        {
            new("room-1", "Room 1", 2, 1, "A", 1, 0, true),
            new("room-2", "Room 2", 2, 1, "A", 2, 0, true),
            new("room-3", "Room 3", 2, 1, "A", 3, 0, true),
            new("room-4", "Room 4", 2, 1, "A", 4, 0, true)
        });
        localContext.Services.AddScoped(_ => roomApi.Object);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var arrival = today.AddDays(-1).ToString("yyyy-MM-dd");
        var departure = today.AddDays(1).ToString("yyyy-MM-dd");

        var client = new HttpClient(new LandingPageMockHandler(
        [
            new { id = "b-1", year = 2026, clientId = "client-1", status = "Confirmed", roomId = "room-1", arrivalDate = arrival, departureDate = departure, actualArrivalDate = arrival, actualGuestCount = 2 },
            new { id = "b-2", year = 2026, clientId = "client-2", status = "Confirmed", roomId = (string?)null, arrivalDate = (string?)null, departureDate = (string?)null }
        ]))
        { BaseAddress = new Uri("http://localhost/") };
        localContext.Services.AddScoped(_ => client);
        localContext.JSInterop.Mode = JSRuntimeMode.Loose;

        var component = localContext.RenderComponent<IndexPage>();

        component.WaitForAssertion(() =>
        {
            var roomRows = component.FindAll(".actionable-room-list .list-group-item");
            Assert.DoesNotContain(roomRows, row => row.TextContent.Contains("Room 1", StringComparison.Ordinal));
            Assert.Contains(roomRows, row => row.TextContent.Contains("Room 2", StringComparison.Ordinal));
            Assert.Contains(roomRows, row => row.TextContent.Contains("Room 3", StringComparison.Ordinal));
            Assert.Contains(roomRows, row => row.TextContent.Contains("Room 4", StringComparison.Ordinal));
        });
    }

    [Fact]
    public void RoomsWidget_ShouldRefresh_WhenIncludeWithoutBedsCheckboxChanges()
    {
        using var localContext = new TestContext();
        localContext.Services.AddClientLocalizationTestSupport();

        var roomApi = new Mock<IRoomApiService>();
        roomApi.Setup(x => x.GetRoomsAsync()).ReturnsAsync(new List<RoomDto>
        {
            new("room-1", "Room 1", 2, 1, "A", 1, 0, false),
            new("room-2", "Room 2", 2, 1, "A", 2, 0, false),
            new("room-3", "Room 3", 2, 1, "A", 3, 0, false)
        });
        localContext.Services.AddScoped(_ => roomApi.Object);

        var systemSettings = new Dictionary<string, string>
        {
            ["DashboardRoomsIncludeWithoutBedsDefault"] = "false"
        };

        var client = new HttpClient(new LandingPageMockHandler(settings: systemSettings))
        { BaseAddress = new Uri("http://localhost/") };
        localContext.Services.AddScoped(_ => client);
        localContext.JSInterop.Mode = JSRuntimeMode.Loose;

        var component = localContext.RenderComponent<IndexPage>();

        component.WaitForAssertion(() =>
        {
            var empty = component.Markup;
            Assert.Contains("DashboardRooms.FilterEmpty", empty);
        });

        var checkbox = component.Find("#rooms-show-without-beds");
        checkbox.Change("true");

        component.WaitForAssertion(() =>
        {
            var roomRows = component.FindAll(".actionable-room-list .list-group-item");
            Assert.Equal(3, roomRows.Count);
        });
    }

    [Fact]
    public void RoomsWidget_ShouldRender_WhenClientsRequestFails()
    {
        using var localContext = new TestContext();
        localContext.Services.AddClientLocalizationTestSupport();

        var roomApi = new Mock<IRoomApiService>();
        roomApi.Setup(x => x.GetRoomsAsync()).ReturnsAsync(new List<RoomDto>
        {
            new("room-1", "Room 1", 2, 1, "A", 1, 0, true),
            new("room-2", "Room 2", 2, 1, "A", 2, 0, true),
            new("room-3", "Room 3", 2, 1, "A", 3, 0, true)
        });
        localContext.Services.AddScoped(_ => roomApi.Object);

        var client = new HttpClient(new LandingPageMockHandler(failClientsRequest: true))
        { BaseAddress = new Uri("http://localhost/") };
        localContext.Services.AddScoped(_ => client);
        localContext.JSInterop.Mode = JSRuntimeMode.Loose;

        var component = localContext.RenderComponent<IndexPage>();

        component.WaitForAssertion(() =>
        {
            var roomRows = component.FindAll(".actionable-room-list .list-group-item");
            Assert.Equal(3, roomRows.Count);
        });
    }
}
