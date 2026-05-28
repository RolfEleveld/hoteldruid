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
            return JsonResponse(new[]
            {
                new { id = 1, name = "Client 1" },
                new { id = 2, name = "Client 2" },
                new { id = 3, name = "Client 3" },
                new { id = 4, name = "Client 4" },
                new { id = 5, name = "Client 5" }
            });
        }

        if (method == HttpMethod.Get && path.Equals("/api/bookings", StringComparison.OrdinalIgnoreCase))
        {
            return JsonResponse(new[]
            {
                new { id = 1, clientId = 1, status = "Confirmed" },
                new { id = 2, clientId = 2, status = "Pending" },
                new { id = 3, clientId = 3, status = "Confirmed" },
                new { id = 4, clientId = 4, status = "Pending" },
                new { id = 5, clientId = 5, status = "Confirmed" }
            });
        }

        if (method == HttpMethod.Get && path.Equals("/api/system/configuration", StringComparison.OrdinalIgnoreCase))
        {
            return JsonResponse(new
            {
                id = "system",
                defaultCurrency = "EUR",
                defaultYear = 2026,
                settings = new Dictionary<string, string>()
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
    public void WidgetMenus_ShouldExpose_ConsistentVisibleCountOptions_OnAllWidgets()
    {
        var component = RenderComponent<IndexPage>();

        component.WaitForAssertion(() =>
        {
            var menuButtons = component.FindAll("button[title='Dashboard.Widget.Menu']");
            Assert.Equal(7, menuButtons.Count);
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
        menuButtons[2].Click();

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
}
