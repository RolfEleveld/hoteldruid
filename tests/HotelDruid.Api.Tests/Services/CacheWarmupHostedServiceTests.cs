using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HotelDruid.Api.Services.Tests;

public class CacheWarmupHostedServiceTests : IAsyncLifetime
{
    private string _tempDataRoot = null!;

    public Task InitializeAsync()
    {
        _tempDataRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempDataRoot);
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        if (Directory.Exists(_tempDataRoot))
        {
            Directory.Delete(_tempDataRoot, recursive: true);
        }

        return Task.CompletedTask;
    }

    [Fact]
    public async Task StartAsync_WhenEnabled_CompletesWarmupAndUpdatesState()
    {
        // Arrange
        var loggerStore = new Mock<ILogger<FileKeyValueStore>>();
        var loggerCache = new Mock<ILogger<CachedKeyValueStoreDecorator>>();
        var loggerWarmup = new Mock<ILogger<CacheWarmupHostedService>>();

        using (var seedStore = new FileKeyValueStore(_tempDataRoot, loggerStore.Object))
        {
            await seedStore.CreateAsync("rooms", "Room-1", new Dictionary<string, object> { ["name"] = "Room-1", ["capacity"] = 2 });
            await seedStore.CreateAsync("rooms", "Room-2", new Dictionary<string, object> { ["name"] = "Room-2", ["capacity"] = 3 });
        }

        var cfg = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["CacheWarmupEnabled"] = "true",
                ["CacheWarmupCollections"] = "rooms",
                ["CacheWarmupMaxDocumentsPerCollection"] = "10"
            })
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(cfg);
        services.AddSingleton<IKeyValueStore>(_ =>
        {
            var baseStore = new FileKeyValueStore(_tempDataRoot, loggerStore.Object);
            return new CachedKeyValueStoreDecorator(
                baseStore,
                loggerCache.Object,
                invalidationRootPath: _tempDataRoot,
                versionCheckIntervalMilliseconds: 0);
        });

        var provider = services.BuildServiceProvider();
        var state = new CacheWarmupState();
        var warmupService = new CacheWarmupHostedService(provider, cfg, state, loggerWarmup.Object);

        // Act
        await warmupService.StartAsync(CancellationToken.None);

        for (var i = 0; i < 40 && !state.IsCompleted; i++)
        {
            await Task.Delay(50);
        }

        await warmupService.StopAsync(CancellationToken.None);

        // Assert
        Assert.True(state.IsEnabled);
        Assert.True(state.IsCompleted);
        Assert.False(state.IsInProgress);
        Assert.NotNull(state.StartedAtUtc);
        Assert.NotNull(state.CompletedAtUtc);
        Assert.True(state.CollectionsProcessed >= 1);
        Assert.True(state.DocumentsWarmed >= 2);
        Assert.Null(state.LastError);
    }

    [Fact]
    public async Task StartAsync_WhenDisabled_MarksSkippedState()
    {
        // Arrange
        var cfg = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["CacheWarmupEnabled"] = "false"
            })
            .Build();

        var services = new ServiceCollection();
        var loggerStore = new Mock<ILogger<FileKeyValueStore>>();
        var loggerCache = new Mock<ILogger<CachedKeyValueStoreDecorator>>();
        var loggerWarmup = new Mock<ILogger<CacheWarmupHostedService>>();

        services.AddSingleton<IConfiguration>(cfg);
        services.AddSingleton<IKeyValueStore>(_ =>
        {
            var baseStore = new FileKeyValueStore(_tempDataRoot, loggerStore.Object);
            return new CachedKeyValueStoreDecorator(baseStore, loggerCache.Object);
        });

        var provider = services.BuildServiceProvider();
        var state = new CacheWarmupState();
        var warmupService = new CacheWarmupHostedService(provider, cfg, state, loggerWarmup.Object);

        // Act
        await warmupService.StartAsync(CancellationToken.None);

        for (var i = 0; i < 20 && !state.IsCompleted; i++)
        {
            await Task.Delay(25);
        }

        await warmupService.StopAsync(CancellationToken.None);

        // Assert
        Assert.False(state.IsEnabled);
        Assert.True(state.IsCompleted);
        Assert.False(state.IsInProgress);
        Assert.NotNull(state.StartedAtUtc);
        Assert.NotNull(state.CompletedAtUtc);
        Assert.Equal(0, state.CollectionsProcessed);
        Assert.Equal(0, state.DocumentsWarmed);
        Assert.Null(state.LastError);
    }
}
