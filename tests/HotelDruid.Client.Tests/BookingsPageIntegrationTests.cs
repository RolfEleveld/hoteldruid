using System.Collections.Generic;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using HotelDruid.Client.Pages;
using HotelDruid.Client.Services;
using HotelDruid.Shared;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace HotelDruid.Client.Tests.Integration.Pages;

public class BookingsPageIntegrationTests : TestContext
{
    private readonly Mock<IBookingApiService> bookingApi = new();
    private readonly Mock<IActiveYearService> activeYearService = new();

    public BookingsPageIntegrationTests()
    {
        Services.AddClientLocalizationTestSupport();
        activeYearService.SetupGet(x => x.CurrentYear).Returns(2026);
        activeYearService.Setup(x => x.InitializeAsync()).Returns(Task.CompletedTask);
        activeYearService.Setup(x => x.SetActiveYearAsync(It.IsAny<int>(), It.IsAny<bool>())).Returns(Task.CompletedTask);

        bookingApi.Setup(x => x.ListAsync(It.IsAny<int?>())).ReturnsAsync(new List<BookingDto>
        {
            new("b-1", 2026, "client-1", "room-1", new DateOnly(2026, 5, 10), new DateOnly(2026, 5, 12), "Confirmed", "First booking"),
            new("b-2", 2026, "client-2", "room-2", new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 3), "Pending", "Second booking")
        });

        Services.AddScoped(_ => bookingApi.Object);
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

        component.Markup.Should().Contain("2026 — client-1");
        component.Markup.Should().Contain("2026-05-10 → 2026-05-12");

        component.Find("button.btn.btn-sm.btn-primary.mb-2").Click();

        component.WaitForAssertion(() =>
        {
            component.FindAll("input[type='date']").Should().HaveCount(2);
        });
    }
}