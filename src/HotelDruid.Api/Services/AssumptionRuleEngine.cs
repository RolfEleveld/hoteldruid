using HotelDruid.Api.Models;

namespace HotelDruid.Api.Services;

public class AssumptionRuleEngine : IAssumptionRuleEngine
{
    private readonly ILogger<AssumptionRuleEngine> _logger;

    public AssumptionRuleEngine(
        IKeyValueStore store,
        ISystemConfigurationStore configurationStore,
        ILogger<AssumptionRuleEngine> logger)
    {
        _ = store;
        _ = configurationStore;
        _logger = logger;
    }

    public Task TriggerMissingYearHealingAsync(string entityType, int year)
    {
        _logger.LogDebug("Did not copy {EntityType} into {Year}. A price applies only on its own dates.", entityType, year);
        return Task.CompletedTask;
    }
}
