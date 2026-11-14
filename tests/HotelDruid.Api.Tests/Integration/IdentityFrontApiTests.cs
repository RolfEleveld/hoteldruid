using System.Net;
using System.Net.Http.Json;
using HotelDruid.Api;
using HotelDruid.Api.Services;
using Microsoft.AspNetCore.Mvc.Testing;

namespace HotelDruid.Api.Tests.Integration;

[Collection("Sequential")]
public class IdentityFrontApiTests : IAsyncLifetime
{
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;
    private string _testDataRoot = null!;

    public Task InitializeAsync()
    {
        _testDataRoot = Path.Combine(Path.GetTempPath(), $"api-identity-front-{Guid.NewGuid()}");
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
    public async Task IdentityFront_IsAbsent_WhenTheDeploymentDoesNotNameKeycloak()
    {
        var response = await _client.GetAsync("/api/system/identity-front");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var status = await response.Content.ReadFromJsonAsync<IdentityFrontStatus>();
        Assert.NotNull(status);
        Assert.False(status.Configured);
        Assert.False(status.Reachable);
        Assert.False(status.IsFront);
    }
}
