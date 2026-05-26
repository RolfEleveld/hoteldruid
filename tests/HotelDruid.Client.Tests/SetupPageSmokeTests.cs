using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using HotelDruid.Client.Pages;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HotelDruid.Client.Tests.Integration.Pages;

internal sealed class SetupPageMockHandler : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var path = request.RequestUri?.AbsolutePath ?? string.Empty;

        if (request.Method == HttpMethod.Get && path.Equals("/api/system/storage", System.StringComparison.OrdinalIgnoreCase))
        {
            const string json = "{\"dataRoot\":\"C:\\\\data\\\\hoteldruid\",\"source\":\"env:HOTELDRUID_DATAROOT\",\"environmentVariable\":\"HOTELDRUID_DATAROOT\",\"configurationKey\":\"DataRoot\"}";
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            });
        }

        if (request.Method == HttpMethod.Get && path.Equals("/api/system/configuration", System.StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
        }

        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
    }
}

public class SetupPageSmokeTests : TestContext
{
    public SetupPageSmokeTests()
    {
        Services.AddClientLocalizationTestSupport();
        var client = new HttpClient(new SetupPageMockHandler()) { BaseAddress = new System.Uri("http://localhost/") };
        Services.AddScoped(_ => client);
    }

    [Fact]
    public void SetupPage_ShouldShowUpdatedCurrencyStorageAndFallbackChoices()
    {
        var component = RenderComponent<Setup>();

        component.WaitForAssertion(() =>
        {
            component.Markup.Should().Contain("Setup.SystemCurrency.Label");
            component.Markup.Should().Contain("Setup.Storage.Label");
            component.Markup.Should().Contain("HOTELDRUID_DATAROOT");
            component.Markup.Should().Contain("Setup.Fallback.Label");
            component.Markup.Should().Contain("value=\"LatestAvailableYear\"");
            component.Markup.Should().Contain("value=\"NoSuggestion\"");
        });
    }
}
