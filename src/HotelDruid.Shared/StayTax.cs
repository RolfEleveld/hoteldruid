using System.Globalization;

namespace HotelDruid.Shared;

/// <summary>
/// How a quoted amount splits into what the guest owes, the net revenue, and the output tax.
/// Inclusive prices already contain the tax. Exclusive prices have the tax added.
/// City tax is outside the value-added tax and is not part of this split.
/// </summary>
public static class StayTax
{
    public static decimal Round(decimal value)
        => decimal.Round(value, 2, MidpointRounding.AwayFromZero);

    public static (decimal Guest, decimal Net, decimal Tax) Split(decimal quoted, decimal? rate, bool inclusive)
    {
        var amount = Round(Math.Abs(quoted));
        if (amount == 0 || rate is null or <= 0)
            return (amount, amount, 0m);

        if (inclusive)
        {
            var included = Round(amount * rate.Value / (100m + rate.Value));
            if (included > amount)
                included = amount;
            return (amount, amount - included, included);
        }

        var added = Round(amount * rate.Value / 100m);
        return (amount + added, amount, added);
    }

    /// <summary>The share of a total that belongs to part of the nights. The other share is the remainder.</summary>
    public static decimal Portion(decimal total, int partNights, int allNights)
    {
        var amount = Round(total);
        if (partNights <= 0 || allNights <= 0)
            return 0m;
        if (partNights >= allNights)
            return amount;
        return Round(amount * partNights / allNights);
    }

    public static decimal CityTax(int nights, int persons, string? basis, decimal amount)
    {
        if (nights <= 0 || amount <= 0)
            return 0m;
        if (string.Equals(basis, "RoomNight", StringComparison.OrdinalIgnoreCase))
            return Round(amount * nights);
        if (string.Equals(basis, "PersonNight", StringComparison.OrdinalIgnoreCase))
            return Round(amount * nights * Math.Max(persons, 1));
        return 0m;
    }
}

/// <summary>The hotel's tax rates. A missing rate means that kind is not taxed.</summary>
public sealed record TaxPolicy(
    bool Inclusive,
    decimal? Stay,
    decimal? Food,
    decimal? Service,
    decimal? Product,
    string CityBasis,
    decimal CityAmount)
{
    public static TaxPolicy From(IReadOnlyDictionary<string, string>? settings)
    {
        var source = settings ?? new Dictionary<string, string>();
        var inclusive = !string.Equals(source.GetValueOrDefault("TaxPricing"), "Exclusive", StringComparison.OrdinalIgnoreCase);
        var stay = Parse(source, "TaxRateStay") ?? Parse(source, "VatRate");
        var basis = source.GetValueOrDefault("CityTaxBasis") ?? "None";
        if (basis is not ("RoomNight" or "PersonNight"))
            basis = "None";
        return new TaxPolicy(
            inclusive,
            stay,
            Parse(source, "TaxRateFood"),
            Parse(source, "TaxRateService"),
            Parse(source, "TaxRateProduct"),
            basis,
            Parse(source, "CityTaxAmount") ?? 0m);
    }

    public decimal? RateFor(string? category, double? ownRate)
    {
        if (ownRate is > 0)
            return (decimal)ownRate.Value;

        var text = category ?? string.Empty;
        if (Contains(text, "food") || Contains(text, "breakfast") || Contains(text, "restaurant") || Contains(text, "bar"))
            return Food ?? Product ?? Stay;
        if (Contains(text, "service"))
            return Service ?? Product ?? Stay;
        return Product ?? Stay;
    }

    private static bool Contains(string text, string word)
        => text.Contains(word, StringComparison.OrdinalIgnoreCase);

    private static decimal? Parse(IReadOnlyDictionary<string, string> settings, string key)
    {
        if (!settings.TryGetValue(key, out var text) || string.IsNullOrWhiteSpace(text))
            return null;
        return decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out var rate) && rate > 0
            ? rate
            : null;
    }
}
