using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace HotelDruid.Api.Services.Tests;

[Trait("Category", "Performance")]
public class EndpointQueryCachePerformanceTests
{
    private readonly ITestOutputHelper _output;

    public EndpointQueryCachePerformanceTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task Comparison_EndpointQueryCache_ColdVsWarm()
    {
        var logger = new Mock<ILogger<EndpointQueryCache>>();
        var cache = new EndpointQueryCache(logger.Object, enabled: true, ttlSeconds: 60);

        const int iterations = 40;

        var swCold = Stopwatch.StartNew();
        for (var i = 0; i < iterations; i++)
        {
            cache.InvalidateTag("rooms");
            _ = await cache.GetOrCreateAsync("rooms:list", new[] { "rooms" }, async () =>
            {
                await Task.Delay(2);
                return new List<int> { 1, 2, 3 };
            });
        }
        swCold.Stop();

        cache.InvalidateTag("rooms");
        _ = await cache.GetOrCreateAsync("rooms:list", new[] { "rooms" }, async () =>
        {
            await Task.Delay(2);
            return new List<int> { 1, 2, 3 };
        });

        var swWarm = Stopwatch.StartNew();
        for (var i = 0; i < iterations; i++)
        {
            _ = await cache.GetOrCreateAsync("rooms:list", new[] { "rooms" }, async () =>
            {
                await Task.Delay(2);
                return new List<int> { 1, 2, 3 };
            });
        }
        swWarm.Stop();

        var coldAvg = swCold.Elapsed.TotalMilliseconds / iterations;
        var warmAvg = swWarm.Elapsed.TotalMilliseconds / iterations;
        var improvement = ((coldAvg - warmAvg) / coldAvg) * 100;

        _output.WriteLine($"Endpoint query cache cold avg: {coldAvg:F3}ms");
        _output.WriteLine($"Endpoint query cache warm avg: {warmAvg:F3}ms");
        _output.WriteLine($"Improvement: {improvement:F1}%");

        Assert.True(warmAvg < coldAvg, $"Warm endpoint cache should be faster (warm={warmAvg:F3}, cold={coldAvg:F3})");
    }
}
