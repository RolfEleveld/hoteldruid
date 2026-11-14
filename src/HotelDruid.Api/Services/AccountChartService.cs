using HotelDruid.Shared;

namespace HotelDruid.Api.Services;

public interface IAccountChart
{
    Task<AccountChart> CurrentAsync();
}

/// <summary>
/// Reads the chart chosen in setup. A missing choice is USALI.
/// </summary>
public sealed class AccountChartService : IAccountChart
{
    private readonly ISystemConfigurationStore _configuration;

    public AccountChartService(ISystemConfigurationStore configuration)
    {
        _configuration = configuration;
    }

    public async Task<AccountChart> CurrentAsync()
    {
        var config = await _configuration.GetAsync();
        return AccountChart.ResolveOrUsali(config?.AccountingStandard, config?.AccountCodes);
    }
}
