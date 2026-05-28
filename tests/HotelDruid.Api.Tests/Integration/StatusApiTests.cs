using System.Net.Http.Json;
using HotelDruid.Api;
using Microsoft.AspNetCore.Mvc.Testing;

namespace HotelDruid.Api.Tests.Integration;

[Collection("Sequential")]
public class StatusApiTests : IAsyncLifetime
{
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;
    private string _testDataRoot = null!;

    public Task InitializeAsync()
    {
        _testDataRoot = Path.Combine(Path.GetTempPath(), $"api-status-tests-{Guid.NewGuid()}");
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
    public async Task Status_Returns_BaseVersion_And_BlazorVersionLabel()
    {
        var status = await _client.GetFromJsonAsync<StatusDto>("/api/status");

        Assert.NotNull(status);
        Assert.Equal("HotelDruid 3.0.7", status!.Version);
        Assert.Equal("HotelDruid 3.0.7. Blazor", status.VersionLabel);
        Assert.False(string.IsNullOrWhiteSpace(status.ActiveYear));
        Assert.False(string.IsNullOrWhiteSpace(status.User));
    }

    [Fact]
    public async Task Status_UsesLocalBuildPlaceholder_WhenNoCiBuildNumberExists()
    {
        Environment.SetEnvironmentVariable("BUILD_BUILDNUMBER", null);
        Environment.SetEnvironmentVariable("GITHUB_RUN_NUMBER", null);
        Environment.SetEnvironmentVariable("CI_PIPELINE_IID", null);
        Environment.SetEnvironmentVariable("HOTELDRUID_BUILD_NUMBER", null);

        var status = await _client.GetFromJsonAsync<StatusDto>("/api/status");

        Assert.NotNull(status);
        Assert.Equal("local", status!.BuildNumber);
    }

    private sealed class StatusDto
    {
        public string? ActiveYear { get; set; }
        public string? User { get; set; }
        public string? Version { get; set; }
        public string? VersionLabel { get; set; }
        public string? BuildNumber { get; set; }
    }
}
