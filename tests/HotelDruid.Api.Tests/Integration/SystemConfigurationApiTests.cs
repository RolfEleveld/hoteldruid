using System.Net;
using System.Text;
using System.Text.Json;
using HotelDruid.Api;
using HotelDruid.Shared.Configuration;
using Microsoft.AspNetCore.Mvc.Testing;

namespace HotelDruid.Api.Tests.Integration;

[Collection("Sequential")]
public class SystemConfigurationApiTests : IAsyncLifetime
{
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;
    private string _testDataRoot = null!;

    public Task InitializeAsync()
    {
        _testDataRoot = Path.Combine(Path.GetTempPath(), $"api-system-config-tests-{Guid.NewGuid()}");
        Directory.CreateDirectory(_testDataRoot);

        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => builder.UseSetting("DataRoot", _testDataRoot));

        _client = _factory.CreateClient();
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        _client.Dispose();
        _factory.Dispose();

        if (Directory.Exists(_testDataRoot))
        {
            try { Directory.Delete(_testDataRoot, recursive: true); } catch { }
        }

        return Task.CompletedTask;
    }

    private static StringContent JsonContent<T>(T value)
    {
        return new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");
    }

    [Fact]
    public async Task Get_Returns_NotFound_When_No_Configuration_Exists()
    {
        var response = await _client.GetAsync("/api/system/configuration");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Put_Then_Get_RoundTrips_Configuration()
    {
        var input = new SystemConfiguration
        {
            Id = "system",
            DefaultCurrency = "USD",
            DefaultYear = 2026,
            Settings = new Dictionary<string, string> { ["PricingFallback"] = "CurrentYear" }
        };

        var putResponse = await _client.PutAsync("/api/system/configuration", JsonContent(input));
        Assert.Equal(HttpStatusCode.OK, putResponse.StatusCode);

        var getResponse = await _client.GetAsync("/api/system/configuration");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var payload = await getResponse.Content.ReadAsStringAsync();
        var loaded = JsonSerializer.Deserialize<SystemConfiguration>(payload, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(loaded);
        Assert.Equal("USD", loaded!.DefaultCurrency);
        Assert.Equal(2026, loaded.DefaultYear);
        Assert.NotNull(loaded.Settings);
        Assert.Equal("CurrentYear", loaded.Settings!["PricingFallback"]);
    }

    [Fact]
    public async Task Delete_Removes_Configuration()
    {
        var input = new SystemConfiguration
        {
            Id = "system",
            DefaultCurrency = "EUR",
            DefaultYear = 2025
        };

        var putResponse = await _client.PutAsync("/api/system/configuration", JsonContent(input));
        Assert.Equal(HttpStatusCode.OK, putResponse.StatusCode);

        var deleteResponse = await _client.DeleteAsync("/api/system/configuration");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await _client.GetAsync("/api/system/configuration");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }
}
