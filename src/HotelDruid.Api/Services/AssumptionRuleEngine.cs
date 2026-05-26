using HotelDruid.Api.Models;

namespace HotelDruid.Api.Services;

public class AssumptionRuleEngine : IAssumptionRuleEngine
{
    private readonly IKeyValueStore _store;
    private readonly ISystemConfigurationStore _configurationStore;
    private readonly ILogger<AssumptionRuleEngine> _logger;

    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public AssumptionRuleEngine(
        IKeyValueStore store,
        ISystemConfigurationStore configurationStore,
        ILogger<AssumptionRuleEngine> logger)
    {
        _store = store;
        _configurationStore = configurationStore;
        _logger = logger;
    }

    public async Task TriggerMissingYearHealingAsync(string entityType, int year)
    {
        if (year < 1900 || year > 2200)
        {
            return;
        }

        await _semaphore.WaitAsync();
        try
        {
            if (entityType.Equals("tariffs", StringComparison.OrdinalIgnoreCase))
            {
                await HealTariffsAsync(year);
                return;
            }

            if (entityType.Equals("periods", StringComparison.OrdinalIgnoreCase))
            {
                await HealPeriodsAsync(year);
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task HealTariffsAsync(int year)
    {
        var target = await _store.ListAsync<TariffStorageModel>("tariffs");
        if (target.Any(t => t.Year == year))
        {
            return;
        }

        var sourceYear = await ResolveSourceYearAsync(year, "TariffFallbackSourceYear");
        var source = target.Where(t => t.Year == sourceYear).ToList();
        if (source.Count == 0)
        {
            _logger.LogInformation("No source tariffs found for fallback year {SourceYear}", sourceYear);
            return;
        }

        foreach (var tariff in source)
        {
            var key = $"{year}_{Guid.NewGuid():N}";
            var clone = new TariffStorageModel
            {
                Year = year,
                ExtraCostName = tariff.ExtraCostName,
                CostType = tariff.CostType,
                BaseValue = tariff.BaseValue,
                PercentageValue = tariff.PercentageValue,
                TaxPercentage = tariff.TaxPercentage,
                Category = tariff.Category,
                NumberLimit = tariff.NumberLimit
            };

            await _store.CreateAsync("tariffs", key, clone);
        }

        _logger.LogInformation("Healed {Count} missing tariffs for year {Year} using source year {SourceYear}", source.Count, year, sourceYear);
    }

    private async Task HealPeriodsAsync(int year)
    {
        var target = await _store.ListAsync<PeriodStorageModel>("periods");
        if (target.Any(p => p.Year == year))
        {
            return;
        }

        var sourceYear = await ResolveSourceYearAsync(year, "AvailabilityFallbackSourceYear");
        var source = target.Where(p => p.Year == sourceYear).ToList();
        if (source.Count == 0)
        {
            _logger.LogInformation("No source periods found for fallback year {SourceYear}", sourceYear);
            return;
        }

        foreach (var period in source)
        {
            var key = $"{year}_{Guid.NewGuid():N}";
            var clone = new PeriodStorageModel
            {
                Year = year,
                StartDate = period.StartDate,
                EndDate = period.EndDate,
                Tariff1 = period.Tariff1,
                Tariff1PerPerson = period.Tariff1PerPerson,
                Tariff2 = period.Tariff2,
                Tariff2PerPerson = period.Tariff2PerPerson,
                Tariff3 = period.Tariff3,
                Tariff3PerPerson = period.Tariff3PerPerson,
                Tariff4 = period.Tariff4,
                Tariff4PerPerson = period.Tariff4PerPerson,
                Tariff5 = period.Tariff5,
                Tariff5PerPerson = period.Tariff5PerPerson,
                Tariff6 = period.Tariff6,
                Tariff6PerPerson = period.Tariff6PerPerson,
                Tariff7 = period.Tariff7,
                Tariff7PerPerson = period.Tariff7PerPerson,
                Tariff8 = period.Tariff8,
                Tariff8PerPerson = period.Tariff8PerPerson,
                Tariff9 = period.Tariff9,
                Tariff9PerPerson = period.Tariff9PerPerson,
                Tariff10 = period.Tariff10,
                Tariff10PerPerson = period.Tariff10PerPerson,
                Tariff11 = period.Tariff11,
                Tariff11PerPerson = period.Tariff11PerPerson,
                Tariff12 = period.Tariff12,
                Tariff12PerPerson = period.Tariff12PerPerson
            };

            await _store.CreateAsync("periods", key, clone);
        }

        _logger.LogInformation("Healed {Count} missing periods for year {Year} using source year {SourceYear}", source.Count, year, sourceYear);
    }

    private async Task<int> ResolveSourceYearAsync(int requestedYear, string explicitSettingKey)
    {
        var config = await _configurationStore.GetAsync();
        var currentYear = DateTime.UtcNow.Year;

        if (config?.Settings != null
            && config.Settings.TryGetValue(explicitSettingKey, out var configuredValue)
            && int.TryParse(configuredValue, out var configuredYear)
            && configuredYear > 1900
            && configuredYear < 2200
            && configuredYear != requestedYear)
        {
            return configuredYear;
        }

        if (explicitSettingKey.Equals("TariffFallbackSourceYear", StringComparison.Ordinal)
            && config?.Settings != null
            && config.Settings.TryGetValue("PriceFallback", out var pricingFallbackRule)
            && !string.IsNullOrWhiteSpace(pricingFallbackRule))
        {
            var normalizedRule = pricingFallbackRule.Trim();

            if (normalizedRule.Equals("DefaultYear", StringComparison.OrdinalIgnoreCase)
                && config.DefaultYear is int configuredDefaultYear
                && configuredDefaultYear > 1900
                && configuredDefaultYear < 2200
                && configuredDefaultYear != requestedYear)
            {
                return configuredDefaultYear;
            }

            if (normalizedRule.Equals("PreviousYear", StringComparison.OrdinalIgnoreCase)
                && requestedYear - 1 > 1900)
            {
                return requestedYear - 1;
            }

            // CurrentYear is the default behavior for pricing fallback.
            if (currentYear != requestedYear)
            {
                return currentYear;
            }
        }

        if (config?.DefaultYear is int defaultYear && defaultYear != requestedYear)
        {
            return defaultYear;
        }

        if (currentYear != requestedYear)
        {
            return currentYear;
        }

        return requestedYear - 1;
    }
}
