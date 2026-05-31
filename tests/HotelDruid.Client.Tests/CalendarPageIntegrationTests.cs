using System.Net;
using System.Text;
using System.Text.Json;
using Bunit;
using FluentAssertions;
using HotelDruid.Client.Pages;
using HotelDruid.Client.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using CalendarPage = HotelDruid.Client.Pages.Calendar;

namespace HotelDruid.Client.Tests.Integration.Pages;

internal sealed class CalendarPageMockHandler : HttpMessageHandler
{
    private readonly object[] bookings;

    public CalendarPageMockHandler(object[] bookings)
    {
        this.bookings = bookings;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var path = request.RequestUri?.AbsolutePath ?? string.Empty;

        if (request.Method == HttpMethod.Get && path.Equals("/api/rooms", StringComparison.OrdinalIgnoreCase))
        {
            return JsonResponse(new[]
            {
                new { id = "room-1", name = "Room 101", capacity = 2, floorNumber = "1", houseNumber = "A" },
                new { id = "room-2", name = "Room 102", capacity = 3, floorNumber = "1", houseNumber = "B" }
            });
        }

        if (request.Method == HttpMethod.Get && path.Equals("/api/clients", StringComparison.OrdinalIgnoreCase))
        {
            return JsonResponse(new[]
            {
                new { id = "client-1", lastName = "Doe", firstName = "Jane" },
                new { id = "client-2", lastName = "Smith", firstName = "Bob" }
            });
        }

        if (request.Method == HttpMethod.Get && path.Equals("/api/bookings", StringComparison.OrdinalIgnoreCase))
        {
            return JsonResponse(bookings);
        }

        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
    }

    private static Task<HttpResponseMessage> JsonResponse(object payload)
    {
        var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        });
    }
}

public class CalendarPageIntegrationTests : TestContext
{
    private readonly Mock<IActiveYearService> activeYearService = new();

    public CalendarPageIntegrationTests()
    {
        Services.AddClientLocalizationTestSupport();
        activeYearService.SetupGet(x => x.CurrentYear).Returns(DateTime.UtcNow.Year);
        activeYearService.Setup(x => x.InitializeAsync()).Returns(Task.CompletedTask);
        activeYearService.Setup(x => x.SetActiveYearAsync(It.IsAny<int>(), It.IsAny<bool>())).Returns(Task.CompletedTask);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var handler = new CalendarPageMockHandler(
        [
            new { id = "stay-1", year = today.Year, clientId = "client-1", roomId = "room-1", arrivalDate = today.AddDays(-1).ToString("yyyy-MM-dd"), departureDate = today.AddDays(2).ToString("yyyy-MM-dd"), actualArrivalDate = today.AddDays(-1).ToString("yyyy-MM-dd"), actualGuestCount = 2, status = "Confirmed" },
            new { id = "res-1", year = today.Year, clientId = "client-2", roomId = "room-2", arrivalDate = today.ToString("yyyy-MM-dd"), departureDate = today.AddDays(3).ToString("yyyy-MM-dd"), status = "Confirmed" }
            ,new { id = "res-2", year = today.Year, clientId = "client-1", roomId = "room-1", arrivalDate = today.AddDays(5).ToString("yyyy-MM-dd"), departureDate = today.AddDays(8).ToString("yyyy-MM-dd"), status = "Confirmed" }
        ]);

        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost/")
        };

        Services.AddSingleton(httpClient);
        Services.AddScoped<IBookingApiService, BookingApiService>();
        Services.AddScoped<IClientApiService, ClientApiService>();
        Services.AddScoped<IRoomApiService, RoomApiService>();
        Services.AddScoped(_ => activeYearService.Object);
    }

    [Fact]
    public void CalendarPage_ShouldRenderMonthListAndColorCodedOccupancy()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var expectedDayCount = DateTime.DaysInMonth(today.Year, today.Month);
        var component = RenderComponent<CalendarPage>();

        component.WaitForAssertion(() =>
        {
            component.FindAll(".calendar-month-item").Should().HaveCount(12);
            component.FindAll(".calendar-day-header").Should().HaveCount(expectedDayCount);
            component.Markup.Should().Contain($"repeat({expectedDayCount}, minmax(0, 1fr))");
            component.FindAll(".calendar-day-header .day-name").Should().BeEmpty();
            component.FindAll(".calendar-day-cell.is-occupied").Should().NotBeEmpty();
            component.FindAll(".calendar-day-cell.is-reserved").Should().NotBeEmpty();
        });

        component.Markup.Should().Contain("Months");
        component.Markup.Should().Contain("Day Details");
        component.Markup.Should().Contain("room-1");
        component.Markup.Should().Contain("room-2");
        component.FindAll(".calendar-room-cell").Should().OnlyContain(cell => !cell.TextContent.Contains("Room 101", StringComparison.OrdinalIgnoreCase));

        var monthStart = new DateOnly(today.Year, today.Month, 1);
        var saturday = Enumerable.Range(0, expectedDayCount)
            .Select(offset => monthStart.AddDays(offset))
            .FirstOrDefault(day => day.DayOfWeek == DayOfWeek.Saturday);
        var sunday = Enumerable.Range(0, expectedDayCount)
            .Select(offset => monthStart.AddDays(offset))
            .FirstOrDefault(day => day.DayOfWeek == DayOfWeek.Sunday);

        if (saturday != default)
        {
            component.FindAll(".calendar-day-header")
                .Single(cell => cell.TextContent.Trim() == saturday.Day.ToString())
                .ClassList.Should().Contain("is-weekend-saturday");
        }

        if (sunday != default)
        {
            component.FindAll(".calendar-day-header")
                .Single(cell => cell.TextContent.Trim() == sunday.Day.ToString())
                .ClassList.Should().Contain("is-weekend-sunday");
        }
    }

    [Fact]
    public void CalendarPage_ShouldShowSelectedRoomDetails_WhenRoomNumberIsClicked()
    {
        var component = RenderComponent<CalendarPage>();

        component.WaitForAssertion(() =>
        {
            component.FindAll(".calendar-day-cell.is-occupied").Should().NotBeEmpty();
        });

        component.FindAll(".calendar-room-cell")
            .Single(cell => cell.TextContent.Trim() == "room-1")
            .Click();

        component.WaitForAssertion(() =>
        {
            component.Markup.Should().Contain("Room Details");
            component.Markup.Should().Contain("room-1");
            component.FindAll(".calendar-detail-room-entry").Should().HaveCount(2);
            component.Markup.Should().Contain("Doe, Jane");
            component.Markup.Should().Contain("Stay");
        });
    }

    [Fact]
    public void CalendarPage_ShouldShowSelectedDayDetails_WhenDayHeaderIsClicked()
    {
        var component = RenderComponent<CalendarPage>();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        component.FindAll(".calendar-day-header")
            .Single(cell => cell.TextContent.Trim() == today.Day.ToString())
            .Click();

        component.WaitForAssertion(() =>
        {
            component.Markup.Should().Contain("Day Details");
            component.Markup.Should().Contain(today.ToString("dddd, dd MMMM yyyy"));
            component.FindAll(".calendar-detail-day-entry").Should().HaveCount(2);
            component.Markup.Should().Contain("Room 101");
            component.Markup.Should().Contain("Room 102");
        });
    }

    [Fact]
    public void CalendarPage_ShouldShowReservationRecord_WhenOccupiedCellIsClicked()
    {
        var component = RenderComponent<CalendarPage>();

        component.WaitForAssertion(() =>
        {
            component.FindAll(".calendar-day-cell.is-occupied").Should().NotBeEmpty();
        });

        component.FindAll(".calendar-day-cell.is-occupied").First().Click();

        component.WaitForAssertion(() =>
        {
            component.Markup.Should().Contain("Reservation Record");
            component.Markup.Should().Contain("Open reservation");
            component.Markup.Should().Contain("Room 101");
            component.Markup.Should().Contain("Doe, Jane");
        });
    }
}