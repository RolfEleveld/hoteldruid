using System.Net;
using System.Text;
using HotelDruid.Api.Services;
using Microsoft.Extensions.Configuration;

namespace HotelDruid.Api.Tests.Services;

public class IdentityFrontProbeTests
{
    [Fact]
    public async Task Unconfigured_IsNotAFront_AndDoesNotCallTheNetwork()
    {
        var handler = new RecordingHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));
        var probe = CreateProbe(handler, new Dictionary<string, string?>());

        var status = await probe.GetStatusAsync();

        Assert.False(status.Configured);
        Assert.False(status.Reachable);
        Assert.False(status.IsFront);
        Assert.Equal(0, handler.Calls);
    }

    [Fact]
    public async Task KeycloakDiscovery_MakesItTheFront_AndReadsTheRealm()
    {
        var handler = new RecordingHandler(request =>
        {
            Assert.Equal(
                "http://keycloak:8080/realms/HotelDruid/.well-known/openid-configuration",
                request.RequestUri!.ToString());
            return Json(HttpStatusCode.OK, """{"issuer":"http://keycloak:8080/realms/HotelDruid"}""");
        });
        var probe = CreateProbe(handler, new Dictionary<string, string?>
        {
            ["Identity:Provider"] = "keycloak",
            ["Identity:Issuer"] = "http://keycloak:8080/realms/HotelDruid/",
            ["Identity:AdminConsoleUrl"] = "https://auth.example.com/admin/HotelDruid/console/"
        });

        var status = await probe.GetStatusAsync();

        Assert.True(status.Configured);
        Assert.True(status.Reachable);
        Assert.True(status.IsFront);
        Assert.Equal("HotelDruid", status.Realm);
        Assert.Equal("http://keycloak:8080/realms/HotelDruid", status.Issuer);
        Assert.Equal("https://auth.example.com/admin/HotelDruid/console", status.AdminConsoleUrl);
        Assert.Equal("https://auth.example.com/realms/HotelDruid/account", status.AccountUrl);
        Assert.Equal(1, handler.Calls);
    }

    [Fact]
    public async Task UnreachableKeycloak_StaysConfigured_AndIsNotTheFront()
    {
        var handler = new RecordingHandler(_ => Json(HttpStatusCode.ServiceUnavailable, "down"));
        var probe = CreateProbe(handler, new Dictionary<string, string?>
        {
            ["Identity:Provider"] = "keycloak",
            ["Identity:Issuer"] = "http://keycloak:8080/realms/HotelDruid"
        });

        var status = await probe.GetStatusAsync();

        Assert.True(status.Configured);
        Assert.False(status.Reachable);
        Assert.False(status.IsFront);
        Assert.Equal("HotelDruid", status.Realm);
    }

    [Fact]
    public async Task OtherProvider_IsNotTreatedAsKeycloak()
    {
        var handler = new RecordingHandler(_ => Json(HttpStatusCode.OK, """{"issuer":"https://login.example.com"}"""));
        var probe = CreateProbe(handler, new Dictionary<string, string?>
        {
            ["Identity:Provider"] = "entra",
            ["Identity:Issuer"] = "https://login.example.com/tenant"
        });

        var status = await probe.GetStatusAsync();

        Assert.False(status.Configured);
        Assert.False(status.IsFront);
        Assert.Equal(0, handler.Calls);
    }

    private static IdentityFrontProbe CreateProbe(HttpMessageHandler handler, Dictionary<string, string?> settings)
    {
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(settings).Build();
        return new IdentityFrontProbe(configuration, new HttpClient(handler));
    }

    private static HttpResponseMessage Json(HttpStatusCode status, string body)
        => new(status) { Content = new StringContent(body, Encoding.UTF8, "application/json") };

    private sealed class RecordingHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _respond;

        public RecordingHandler(Func<HttpRequestMessage, HttpResponseMessage> respond) => _respond = respond;

        public int Calls { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Calls++;
            return Task.FromResult(_respond(request));
        }
    }
}
