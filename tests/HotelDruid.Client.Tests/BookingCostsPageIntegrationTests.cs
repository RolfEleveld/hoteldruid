using System.Collections.Generic;
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

public class BookingCostsPageIntegrationTests : TestContext
{
    private readonly Mock<IBookingCostApiService> bookingCostApi = new();
    private readonly Mock<IBookingApiService> bookingApi = new();
    private readonly Mock<IRoomApiService> roomApi = new();
    private readonly Mock<IActiveYearService> activeYearService = new();

    public BookingCostsPageIntegrationTests()
    {
        Services.AddClientLocalizationTestSupport();

        activeYearService.SetupGet(x => x.CurrentYear).Returns(2026);
        activeYearService.Setup(x => x.InitializeAsync()).Returns(Task.CompletedTask);
        activeYearService.Setup(x => x.SetActiveYearAsync(It.IsAny<int>(), It.IsAny<bool>())).Returns(Task.CompletedTask);

        bookingCostApi
            .Setup(x => x.ListAsync(It.IsAny<int?>()))
            .ReturnsAsync(new List<BookingCostClientDto>());

        bookingCostApi
            .Setup(x => x.CreateAsync(It.IsAny<BookingCostClientDto>()))
            .ReturnsAsync("cost-created");

        bookingApi.Setup(x => x.ListAsync(It.IsAny<int?>())).ReturnsAsync(new List<HotelDruid.Shared.BookingDto>
        {
            new("stay-42", 2026, null, "room-1", new DateOnly(2026, 5, 1), new DateOnly(2026, 5, 2)),
            new("stay-99", 2026, null, "room-1", new DateOnly(2026, 5, 3), new DateOnly(2026, 5, 4))
        });
        roomApi.Setup(x => x.GetRoomsAsync()).ReturnsAsync(new List<RoomDto>
        {
            new("room-1", "Room 1", 2, 1, "A")
        });

        Services.AddScoped(_ => bookingCostApi.Object);
        Services.AddScoped(_ => bookingApi.Object);
        Services.AddScoped(_ => roomApi.Object);
        Services.AddScoped(_ => activeYearService.Object);
    }

    [Fact]
    public void BookingCostsNewRoute_ShouldCreateCostForStayOnlyOnce_WhenSaveClickedRapidly()
    {
        var navigation = Services.GetRequiredService<NavigationManager>();
        navigation.NavigateTo("/booking-costs?bookingId=stay-42&year=2026&new=true");

        BookingCostClientDto? createdDto = null;
        var createGate = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);

        bookingCostApi
            .Setup(x => x.CreateAsync(It.IsAny<BookingCostClientDto>()))
            .Callback<BookingCostClientDto>(dto => createdDto = dto)
            .Returns(createGate.Task);

        var component = Render<BookingCosts>();

        component.WaitForAssertion(() =>
        {
            component.Find("button.btn.btn-primary.me-2");
        });

        var saveButton = component.Find("button.btn.btn-primary.me-2");
        saveButton.Click();
        saveButton.Click();

        bookingCostApi.Verify(x => x.CreateAsync(It.IsAny<BookingCostClientDto>()), Times.Once);

        createdDto.Should().NotBeNull();
        createdDto!.BookingId.Should().Be("stay-42");
        createdDto.Year.Should().Be(2026);

        createGate.SetResult("cost-created");

        component.WaitForAssertion(() =>
        {
            bookingCostApi.Verify(x => x.ListAsync(2026), Times.AtLeast(2));
        });
    }

    [Fact]
    public void BookingCostsNewRoute_ShouldDefaultYearFromActiveYear_WhenYearQueryMissing()
    {
        var navigation = Services.GetRequiredService<NavigationManager>();
        navigation.NavigateTo("/booking-costs?bookingId=stay-99&new=true");

        BookingCostClientDto? createdDto = null;

        bookingCostApi
            .Setup(x => x.CreateAsync(It.IsAny<BookingCostClientDto>()))
            .Callback<BookingCostClientDto>(dto => createdDto = dto)
            .ReturnsAsync("cost-created");

        var component = Render<BookingCosts>();

        component.WaitForAssertion(() =>
        {
            component.Find("button.btn.btn-primary.me-2").Click();
        });

        bookingCostApi.Verify(x => x.CreateAsync(It.IsAny<BookingCostClientDto>()), Times.Once);
        createdDto.Should().NotBeNull();
        createdDto!.BookingId.Should().Be("stay-99");
        createdDto.Year.Should().Be(2026);
    }
}
