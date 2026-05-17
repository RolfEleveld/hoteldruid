using System.Globalization;

namespace HotelDruid.Api.Services;

public sealed class CacheWarmupHostedService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly CacheWarmupState _state;
    private readonly ILogger<CacheWarmupHostedService> _logger;

    public CacheWarmupHostedService(
        IServiceProvider serviceProvider,
        IConfiguration configuration,
        CacheWarmupState state,
        ILogger<CacheWarmupHostedService> logger)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _state = state;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var enabled = _configuration.GetValue<bool>("CacheWarmupEnabled", false);
        if (!enabled)
        {
            _state.MarkSkipped();
            return;
        }

        _state.MarkStarted();

        try
        {
            await Task.Yield();

            using var scope = _serviceProvider.CreateScope();
            var store = scope.ServiceProvider.GetRequiredService<IKeyValueStore>();

            var configuredCollections = _configuration["CacheWarmupCollections"];
            var collections = ParseCollections(configuredCollections);
            if (collections.Count == 0)
            {
                collections.Add("rooms");
                collections.Add("bookings");
            }

            var maxDocsPerCollection = Math.Max(0, _configuration.GetValue<int>("CacheWarmupMaxDocumentsPerCollection", 200));

            foreach (var collection in collections)
            {
                if (stoppingToken.IsCancellationRequested)
                    break;

                var index = await store.GetIndexAsync(collection);
                var ids = maxDocsPerCollection == 0
                    ? index.Values
                    : index.Values.Take(maxDocsPerCollection);

                var warmedForCollection = 0;
                foreach (var id in ids)
                {
                    if (stoppingToken.IsCancellationRequested)
                        break;

                    _ = await store.GetAsync<Dictionary<string, object>>(collection, id);
                    warmedForCollection++;
                }

                _state.AddCollectionResult(warmedForCollection);
                _logger.LogInformation(
                    "Warm-up collection {Collection}: index={IndexCount}, documentsWarmed={DocumentsWarmed}, limit={Limit}",
                    collection,
                    index.Count,
                    warmedForCollection,
                    maxDocsPerCollection.ToString(CultureInfo.InvariantCulture));
            }

            _state.MarkCompleted();
            _logger.LogInformation("Cache warm-up completed: collections={Collections}, documents={Documents}", _state.CollectionsProcessed, _state.DocumentsWarmed);
        }
        catch (Exception ex)
        {
            _state.MarkFailed(ex);
            _logger.LogError(ex, "Cache warm-up failed");
        }
    }

    private static List<string> ParseCollections(string? configured)
    {
        if (string.IsNullOrWhiteSpace(configured))
            return new List<string>();

        return configured
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}
