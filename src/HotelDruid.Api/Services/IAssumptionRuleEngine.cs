namespace HotelDruid.Api.Services;

public interface IAssumptionRuleEngine
{
    Task TriggerMissingYearHealingAsync(string entityType, int year);
}
