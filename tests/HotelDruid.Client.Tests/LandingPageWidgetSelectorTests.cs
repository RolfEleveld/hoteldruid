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
            return JsonResponse(new { activeYear = "2026", user = "admin", version = "HotelDruid 4.10.1", versionLabel = "HotelDruid 4.10.1. Blazor", buildNumber = "local" });
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

        if (method == HttpMethod.Get && path.Equals("/api/bookings/stays", StringComparison.OrdinalIgnoreCase))
        {
            var parsed = ParseBookings(bookings);
            var stays = GetCurrentStays(parsed, request.RequestUri?.Query);
            return JsonResponse(stays);
        }

        if (method == HttpMethod.Get && path.Equals("/api/bookings/reservations", StringComparison.OrdinalIgnoreCase))
        {
            var parsed = ParseBookings(bookings);
            var reservations = GetReservations(parsed, request.RequestUri?.Query);
            return JsonResponse(reservations);
        }

        if (method == HttpMethod.Get && path.Equals("/api/bookings/arrivals", StringComparison.OrdinalIgnoreCase))
        {
            var parsed = ParseBookings(bookings);
            var arrivals = GetArrivals(parsed, request.RequestUri?.Query);
            return JsonResponse(arrivals);
        }

        if (method == HttpMethod.Get && path.Equals("/api/bookings/departures", StringComparison.OrdinalIgnoreCase))
        {
            var parsed = ParseBookings(bookings);
            var departures = GetDepartures(parsed, request.RequestUri?.Query);
            return JsonResponse(departures);
        }

        if (method == HttpMethod.Put && path.StartsWith("/api/bookings/", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{}", Encoding.UTF8, "application/json")
            });
        }

        if (method == HttpMethod.Post
            && (path.Contains("/check-in", StringComparison.OrdinalIgnoreCase)
                || path.Contains("/check-out", StringComparison.OrdinalIgnoreCase)
                || path.Contains("/no-show", StringComparison.OrdinalIgnoreCase)))
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{}", Encoding.UTF8, "application/json")
            });
        }

        if (method == HttpMethod.Post && path.Contains("/cancel", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NoContent));
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

    private static List<MockBooking> ParseBookings(object[] sourceBookings)
    {
        var serialized = JsonSerializer.Serialize(sourceBookings);
        return JsonSerializer.Deserialize<List<MockBooking>>(serialized, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new List<MockBooking>();
    }

    private static object[] GetCurrentStays(IEnumerable<MockBooking> sourceBookings, string? query)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var year = TryReadYearQuery(query);
        return sourceBookings
            .Where(b => (!year.HasValue || b.Year == year.Value) && IsStay(b, today))
            .Select(b => new
            {
                b.Id,
                b.Year,
                b.ClientId,
                b.RoomId,
                b.ArrivalDate,
                b.DepartureDate,
                b.Status,
                b.ActualArrivalDate,
                b.ActualDepartureDate,
                b.ActualGuestCount,
                b.PrepaidAmount,
                b.Notes
            })
            .ToArray<object>();
    }

    private static object[] GetReservations(IEnumerable<MockBooking> sourceBookings, string? query)
    {
        var year = TryReadYearQuery(query);
        return sourceBookings
            .Where(b => (!year.HasValue || b.Year == year.Value) && IsReservation(b))
            .Select(ToApiShape)
            .ToArray<object>();
    }

    private static object[] GetArrivals(IEnumerable<MockBooking> sourceBookings, string? query)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var year = TryReadYearQuery(query);
        var lookAheadDays = TryReadIntQuery(query, "lookAheadDays") ?? 0;
        var latest = today.AddDays(Math.Max(0, lookAheadDays));

        return sourceBookings
            .Where(b => (!year.HasValue || b.Year == year.Value)
                && IsReservation(b)
                && TryParseDate(b.ArrivalDate).HasValue
                && today <= TryParseDate(b.ArrivalDate)!.Value
                && TryParseDate(b.ArrivalDate)!.Value <= latest)
            .Select(ToApiShape)
            .ToArray<object>();
    }

    private static object[] GetDepartures(IEnumerable<MockBooking> sourceBookings, string? query)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var year = TryReadYearQuery(query);

        return sourceBookings
            .Where(b => (!year.HasValue || b.Year == year.Value)
                && !string.IsNullOrWhiteSpace(b.RoomId)
                && !IsNonBlockingStatus(b.Status)
                && TryParseDate(b.ActualArrivalDate).HasValue
                && !TryParseDate(b.ActualDepartureDate).HasValue
                && TryParseDate(b.DepartureDate).HasValue
                && TryParseDate(b.DepartureDate)!.Value > TryParseDate(b.ActualArrivalDate)!.Value
                && TryParseDate(b.DepartureDate)!.Value == today)
            .Select(ToApiShape)
            .ToArray<object>();
    }

    private static bool IsReservation(MockBooking booking)
    {
        var arrival = TryParseDate(booking.ArrivalDate);
        var departure = TryParseDate(booking.DepartureDate);

        return !IsNonBlockingStatus(booking.Status)
            && !TryParseDate(booking.ActualArrivalDate).HasValue
            && !TryParseDate(booking.ActualDepartureDate).HasValue
            && arrival.HasValue
            && departure.HasValue
            && departure.Value > arrival.Value;
    }

    private static bool IsStay(MockBooking booking, DateOnly today)
    {
        var actualArrival = TryParseDate(booking.ActualArrivalDate);
        var departure = TryParseDate(booking.DepartureDate);

        return !string.IsNullOrWhiteSpace(booking.RoomId)
            && !IsNonBlockingStatus(booking.Status)
            && actualArrival.HasValue
            && !TryParseDate(booking.ActualDepartureDate).HasValue
            && departure.HasValue
            && departure.Value > actualArrival.Value
            && booking.ActualGuestCount.HasValue
            && booking.ActualGuestCount.Value > 0
            && actualArrival.Value <= today
            && today < departure.Value;
    }

    private static object ToApiShape(MockBooking booking)
    {
        return new
        {
            booking.Id,
            booking.Year,
            booking.ClientId,
            booking.RoomId,
            booking.ArrivalDate,
            booking.DepartureDate,
            booking.Status,
            booking.ActualArrivalDate,
            booking.ActualDepartureDate,
            booking.ActualGuestCount,
            booking.PrepaidAmount,
            booking.Notes
        };
    }

    private static bool IsNonBlockingStatus(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            return false;
        }

        return status.Contains("cancel", StringComparison.OrdinalIgnoreCase)
               || status.Contains("void", StringComparison.OrdinalIgnoreCase)
               || status.Contains("deleted", StringComparison.OrdinalIgnoreCase);
    }

    private static DateOnly? TryParseDate(string? value)
    {
        return DateOnly.TryParse(value, out var parsed) ? parsed : null;
    }

    private static int? TryReadYearQuery(string? query)
    {
        return TryReadIntQuery(query, "year");
    }

    private static int? TryReadIntQuery(string? query, string key)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return null;
        }

        var segments = query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries);
        foreach (var segment in segments)
        {
            var pair = segment.Split('=', 2);
            if (pair.Length != 2 || !pair[0].Equals(key, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (int.TryParse(Uri.UnescapeDataString(pair[1]), out var year))
            {
                return year;
            }
        }

        return null;
    }

    private sealed class MockBooking
    {
        public string? Id { get; set; }
        public int Year { get; set; }
        public string? ClientId { get; set; }
        public string? RoomId { get; set; }
        public string? ArrivalDate { get; set; }
        public string? DepartureDate { get; set; }
        public string? Status { get; set; }
        public string? Notes { get; set; }
        public string? ActualArrivalDate { get; set; }
        public string? ActualDepartureDate { get; set; }
        public int? ActualGuestCount { get; set; }
        public double? PrepaidAmount { get; set; }
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
        "Dashboard.Widget.VisibleCount.Tariffs",
        "Dashboard.Widget.VisibleCount.Reports",
        "Dashboard.Widget.VisibleCount.PointOfSale"
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
        "Dashboard.Widget.Menu.AddTariff",
        "Dashboard.Widget.Menu.AddReportTemplate",
        "Index.Feature.PointOfSale.Button"
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

        var inventoryApi = new Mock<IInventoryApiService>();
        inventoryApi.Setup(x => x.GetInventoryAsync()).ReturnsAsync(new List<InventoryDto>());
        Services.AddScoped(_ => inventoryApi.Object);

        var warehouseApi = new Mock<IWarehouseApiService>();
        warehouseApi.Setup(x => x.GetWarehousesAsync()).ReturnsAsync(new List<WarehouseDto>());
        Services.AddScoped(_ => warehouseApi.Object);

        var client = new HttpClient(new LandingPageMockHandler()) { BaseAddress = new Uri("http://localhost/") };
        Services.AddScoped(_ => client);

        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void WidgetMenus_ShouldShow_EntitySpecificVisibleHeader_AndAddAction()
    {
        var component = Render<IndexPage>();

        component.WaitForAssertion(() =>
        {
            var menuButtons = component.FindAll("button[title='Dashboard.Widget.Menu']");
            Assert.Equal(ExpectedAddActionKeys.Length, menuButtons.Count);
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

                if (i == CalendarWidgetMenuIndex)
                {
                    Assert.DoesNotContain("Dashboard.Widget.Menu.AddYear", items);
                    Assert.DoesNotContain("Dashboard.Widget.VisibleCount.Calendar", headers);
                }
                else
                {
                    Assert.Contains(ExpectedAddActionKeys[i], items);
                }

                if (i != CalendarWidgetMenuIndex)
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
        var component = Render<IndexPage>();

        component.WaitForAssertion(() =>
        {
            var menuButtons = component.FindAll("button[title='Dashboard.Widget.Menu']");
            Assert.Equal(ExpectedAddActionKeys.Length, menuButtons.Count);
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
        var component = Render<IndexPage>();

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
        var component = Render<IndexPage>();

        component.WaitForAssertion(() =>
        {
            var bookingRows = component.FindAll(".border-primary .list-group-item");
            Assert.NotEmpty(bookingRows);
            Assert.Contains(bookingRows, row => row.TextContent.Contains("Client, Five", StringComparison.Ordinal));
            Assert.Contains(bookingRows, row => row.TextContent.Contains("2026-06-13 · Room 5", StringComparison.Ordinal));
        });
    }

    [Fact]
    public void ReservationsWidget_RowMenuShouldOffer_ArrivedAndClientActions()
    {
        var component = Render<IndexPage>();

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
            Assert.Contains("Mark No-Show", items);
            Assert.Contains("Cancel Booking", items);
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

        var component = localContext.Render<IndexPage>();

        component.WaitForAssertion(() =>
        {
            var stayRows = component.FindAll(".border-danger .list-group-item");
            Assert.Single(stayRows);
            Assert.Contains("Room 1", stayRows[0].TextContent);
            Assert.DoesNotContain("Room 2", stayRows[0].TextContent);
        });
    }

    [Fact]
    public void StaysWidget_ShouldRender_BookingWithActualArrivalAsStay()
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
            new { id = "stay-1", year = 2026, clientId = "client-1", roomId = "room-1", arrivalDate = today.AddDays(-3).ToString("yyyy-MM-dd"), departureDate = today.AddDays(2).ToString("yyyy-MM-dd"), actualArrivalDate = today.AddDays(-3).ToString("yyyy-MM-dd"), actualGuestCount = 2, status = "Confirmed" },
            new { id = "res-1", year = 2026, clientId = "client-2", roomId = "room-1", arrivalDate = today.AddDays(3).ToString("yyyy-MM-dd"), departureDate = today.AddDays(5).ToString("yyyy-MM-dd"), status = "Confirmed" }
        ]))
        { BaseAddress = new Uri("http://localhost/") };
        localContext.Services.AddScoped(_ => client);
        localContext.JSInterop.Mode = JSRuntimeMode.Loose;

        var component = localContext.Render<IndexPage>();

        component.WaitForAssertion(() =>
        {
            var stayRows = component.FindAll(".border-danger .list-group-item");
            Assert.Single(stayRows);
            Assert.Contains("Room 1", stayRows[0].TextContent);
            Assert.Contains("Dashboard.Stays.ActiveBadge", component.Markup);
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

        var component = localContext.Render<IndexPage>();

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

            Assert.Contains("Check Out", items);
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

        var component = localContext.Render<IndexPage>();

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
            Assert.Contains("Mark No-Show", items);
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

        var component = localContext.Render<IndexPage>();

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

            Assert.Contains("Check Out", items);
        });
    }

    [Fact]
    public void RoomsWidget_MenuShouldOffer_NewReservationForSelectedRoom()
    {
        var component = Render<IndexPage>();

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

        var component = localContext.Render<IndexPage>();

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

        var component = localContext.Render<IndexPage>();

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

        var component = localContext.Render<IndexPage>();

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

        var component = localContext.Render<IndexPage>();

        component.WaitForAssertion(() =>
        {
            var roomRows = component.FindAll(".actionable-room-list .list-group-item");
            Assert.Equal(3, roomRows.Count);
        });
    }
}
