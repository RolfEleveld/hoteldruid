using Microsoft.Extensions.Logging;
using Moq;

namespace HotelDruid.Api.Services.Tests;

public class EndpointQueryCacheTests
{
    [Fact]
    public async Task GetOrCreateAsync_ReturnsCachedValue_ForSameKey()
    {
        var logger = new Mock<ILogger<EndpointQueryCache>>();
        var cache = new EndpointQueryCache(logger.Object, enabled: true, ttlSeconds: 60);

        var callCount = 0;
        var value1 = await cache.GetOrCreateAsync("rooms:list", new[] { "rooms" }, async () =>
        {
            callCount++;
            await Task.Delay(1);
            return "first";
        });

        var value2 = await cache.GetOrCreateAsync("rooms:list", new[] { "rooms" }, async () =>
        {
            callCount++;
            await Task.Delay(1);
            return "second";
        });

        Assert.Equal("first", value1);
        Assert.Equal("first", value2);
        Assert.Equal(1, callCount);
    }

    [Fact]
    public async Task InvalidateTag_ForcesRecompute()
    {
        var logger = new Mock<ILogger<EndpointQueryCache>>();
        var cache = new EndpointQueryCache(logger.Object, enabled: true, ttlSeconds: 60);

        var callCount = 0;
        _ = await cache.GetOrCreateAsync("bookings:list:2026", new[] { "bookings" }, () =>
        {
            callCount++;
            return Task.FromResult(callCount);
        });

        cache.InvalidateTag("bookings");

        var value = await cache.GetOrCreateAsync("bookings:list:2026", new[] { "bookings" }, () =>
        {
            callCount++;
            return Task.FromResult(callCount);
        });

        Assert.Equal(2, value);
        Assert.Equal(2, callCount);
    }

    [Fact]
    public async Task DisabledCache_AlwaysInvokesFactory()
    {
        var logger = new Mock<ILogger<EndpointQueryCache>>();
        var cache = new EndpointQueryCache(logger.Object, enabled: false, ttlSeconds: 60);

        var callCount = 0;

        _ = await cache.GetOrCreateAsync("rooms:list", new[] { "rooms" }, () =>
        {
            callCount++;
            return Task.FromResult(callCount);
        });

        _ = await cache.GetOrCreateAsync("rooms:list", new[] { "rooms" }, () =>
        {
            callCount++;
            return Task.FromResult(callCount);
        });

        Assert.Equal(2, callCount);
    }
}
