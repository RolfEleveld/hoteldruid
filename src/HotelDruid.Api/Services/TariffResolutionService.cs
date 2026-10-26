using HotelDruid.Api.Models;

namespace HotelDruid.Api.Services;

/// <summary>Result of resolving which tariff applies to a room for a given stay and guest count.</summary>
public record TariffResolutionResult(
    string TariffId,
    string TariffName,
    int Nights,
    double BasePricePerNight,
    double AdjustedPricePerNight,
    double TotalPrice,
    string? AppliedSeasonName,
    string ApplicabilityMode,
    IReadOnlyList<TariffDailySegment>? DailySegments = null
);

/// <summary>One night in a multi-tariff stay.</summary>
public record TariffDailySegment(
    DateOnly Date,
    string TariffId,
    string TariffName,
    double Price
);

public interface ITariffResolutionService
{
    /// <summary>
    /// Picks the best-matching active tariff assigned to <paramref name="roomId"/> for the given stay,
    /// applies any overlapping seasonal adjustment, and computes the total price.
    /// Returns null when the room has no assigned/active tariffs or the dates are invalid.
    /// </summary>
    Task<TariffResolutionResult?> ResolveAsync(string roomId, DateOnly arrivalDate, DateOnly departureDate, int guestCount);
}

/// <summary>
/// Resolves the applicable room-rate tariff the same way the tariff management UI reasons about
/// applicability: rack rate (AnyDay) is the fallback, weekday-specific and minimum-stay tariffs take
/// precedence over it, and higher Priority wins among equally-specific candidates.
/// </summary>
public class TariffResolutionService : ITariffResolutionService
{
    private const string AnyDay = "AnyDay";
    private const string Weekdays = "Weekdays";
    private const string MinStay = "MinStay";
    private const string MinGuests = "MinGuests";

    private readonly IKeyValueStore _store;

    public TariffResolutionService(IKeyValueStore store)
    {
        _store = store;
    }

    public async Task<TariffResolutionResult?> ResolveAsync(string roomId, DateOnly arrivalDate, DateOnly departureDate, int guestCount)
    {
        if (string.IsNullOrWhiteSpace(roomId) || departureDate <= arrivalDate)
        {
            return null;
        }

        var nights = departureDate.DayNumber - arrivalDate.DayNumber;
        var effectiveGuestCount = Math.Max(1, guestCount);

        var assignmentIdx = await _store.GetIndexAsync("tariff_assignments");
        var assignedTariffIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var kvp in assignmentIdx)
        {
            var assignment = await _store.GetAsync<TariffAssignmentStorageModel>("tariff_assignments", kvp.Value);
            if (assignment is not null
                && string.Equals(assignment.TargetType, "Room", StringComparison.OrdinalIgnoreCase)
                && string.Equals(assignment.TargetId, roomId, StringComparison.OrdinalIgnoreCase)
                && !string.IsNullOrWhiteSpace(assignment.TariffId))
            {
                assignedTariffIds.Add(assignment.TariffId!);
            }
        }

        if (assignedTariffIds.Count == 0)
        {
            return null;
        }

        var weekdayBit = arrivalDate.DayOfWeek switch
        {
            DayOfWeek.Monday => 1,
            DayOfWeek.Tuesday => 2,
            DayOfWeek.Wednesday => 4,
            DayOfWeek.Thursday => 8,
            DayOfWeek.Friday => 16,
            DayOfWeek.Saturday => 32,
            DayOfWeek.Sunday => 64,
            _ => 0
        };

        TariffStorageModel? bestTariff = null;
        string? bestTariffId = null;
        var bestSpecificity = -1; // AnyDay=0, Weekdays=1, MinGuests=2, MinStay=3 (higher wins ties on Priority)

        foreach (var tariffId in assignedTariffIds)
        {
            var tariff = await _store.GetAsync<TariffStorageModel>("tariffs", tariffId);
            if (tariff is null || !tariff.IsActive)
            {
                continue;
            }

            var specificity = tariff.ApplicabilityMode switch
            {
                MinStay when nights >= (tariff.MinNights ?? int.MaxValue) => 3,
                MinGuests when effectiveGuestCount >= (tariff.MinGuestCount ?? int.MaxValue) => 2,
                Weekdays when ((tariff.WeekdaysMask ?? 0) & weekdayBit) != 0 => 1,
                AnyDay => 0,
                _ => -1 // Doesn't apply to this stay.
            };

            if (specificity < 0)
            {
                continue;
            }

            if (specificity > bestSpecificity
                || (specificity == bestSpecificity && (bestTariff is null || tariff.Priority > bestTariff.Priority)))
            {
                bestSpecificity = specificity;
                bestTariff = tariff;
                bestTariffId = tariffId;
            }
        }

        if (bestTariff is null || bestTariffId is null)
        {
            return null;
        }

        var pricePerNight = bestTariff.PricePerNight;
        string? appliedSeasonName = null;

        var seasonIdx = await _store.GetIndexAsync("tariff_seasons");
        foreach (var kvp in seasonIdx)
        {
            var season = await _store.GetAsync<TariffSeasonStorageModel>("tariff_seasons", kvp.Value);
            if (season is null || !string.Equals(season.TariffId, bestTariffId, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (arrivalDate < season.StartDate || arrivalDate > season.EndDate)
            {
                continue;
            }

            pricePerNight = season.AdjustmentType switch
            {
                "FixedOverride" => season.Value,
                "AmountAdjustment" => pricePerNight + season.Value,
                _ => pricePerNight * (1 + season.Value / 100.0) // PercentageAdjustment
            };
            appliedSeasonName = season.Name;
            break;
        }

        var extraGuests = Math.Max(0, effectiveGuestCount - 1);
        var perPersonPerNight = bestTariff.PricePerPerson ?? 0;
        var totalPrice = (nights * pricePerNight) + (nights * extraGuests * perPersonPerNight);

        // Build daily segments only if the stay is long enough or we want to show breakdown
        List<TariffDailySegment>? daily = null;
        if (nights > 1)
        {
            daily = new List<TariffDailySegment>();
            for (int i = 0; i < nights; i++)
            {
                var nightDate = arrivalDate.AddDays(i);
                // For simplicity we reuse the same best tariff for all nights in this basic version.
                // A full implementation would re-evaluate per night.
                daily.Add(new TariffDailySegment(
                    nightDate,
                    bestTariffId,
                    bestTariff.Name ?? string.Empty,
                    pricePerNight + (extraGuests * perPersonPerNight)));
            }
        }

        return new TariffResolutionResult(
            bestTariffId,
            bestTariff.Name ?? string.Empty,
            nights,
            bestTariff.PricePerNight,
            pricePerNight,
            totalPrice,
            appliedSeasonName,
            bestTariff.ApplicabilityMode ?? AnyDay,
            daily);
    }
}
