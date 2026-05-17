using System.Net;
using System.Net.Http.Json;
using HotelDruid.Api;
using HotelDruid.Shared.Configuration;
using Microsoft.AspNetCore.Mvc.Testing;

namespace HotelDruid.Api.Tests.Integration;

[Collection("Sequential")]
public class SystemSetupApiTests : IAsyncLifetime
{
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;
    private string _testDataRoot = null!;

    public Task InitializeAsync()
    {
        _testDataRoot = Path.Combine(Path.GetTempPath(), $"api-system-setup-tests-{Guid.NewGuid()}");
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

    [Fact]
    public async Task SetupStatus_RequiresSetup_WhenConfigurationMissing_AndAdmin()
    {
        var response = await _client.GetFromJsonAsync<SetupStatusDto>("/api/system/setup/status?adminMode=true");
        Assert.NotNull(response);
        Assert.True(response!.RequiresSetup);
        Assert.Equal("missing-configuration", response.Reason);
    }

    [Fact]
    public async Task SetupStatus_DoesNotRequireSetup_WhenConfigurationExists()
    {
        var save = await _client.PutAsJsonAsync("/api/system/configuration", new SystemConfiguration
        {
            Id = "system",
            DefaultCurrency = "EUR",
            DefaultYear = 2026
        });
        Assert.Equal(HttpStatusCode.OK, save.StatusCode);

        var response = await _client.GetFromJsonAsync<SetupStatusDto>("/api/system/setup/status?adminMode=true");
        Assert.NotNull(response);
        Assert.False(response!.RequiresSetup);
        Assert.Equal("configured", response.Reason);
    }

    [Fact]
    public async Task SetupStatus_RequiresSetup_WhenConfigureIsExplicitlyRequested()
    {
        var save = await _client.PutAsJsonAsync("/api/system/configuration", new SystemConfiguration
        {
            Id = "system",
            DefaultCurrency = "EUR",
            DefaultYear = 2026
        });
        Assert.Equal(HttpStatusCode.OK, save.StatusCode);

        var response = await _client.GetFromJsonAsync<SetupStatusDto>("/api/system/setup/status?adminMode=true&configure=true");
        Assert.NotNull(response);
        Assert.True(response!.RequiresSetup);
        Assert.Equal("explicit-configure", response.Reason);
    }

    private sealed class SetupStatusDto
    {
        public bool RequiresSetup { get; set; }
        public string? Reason { get; set; }
    }
}
