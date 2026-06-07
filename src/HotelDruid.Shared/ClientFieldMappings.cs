namespace HotelDruid.Shared;

public static class ClientFieldMappings
{
    private static readonly Dictionary<string, string> GenderMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["m"] = "male",
        ["male"] = "male",
        ["f"] = "female",
        ["female"] = "female",
        ["other"] = "diverse",
        ["diverse"] = "diverse",
        ["x"] = "diverse",
        ["nonbinary"] = "diverse",
        ["non-binary"] = "diverse"
    };

    private static readonly Dictionary<string, string> LanguageMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["it"] = "it",
        ["ita"] = "it",
        ["italiano"] = "it",
        ["es"] = "es",
        ["spa"] = "es",
        ["español"] = "es",
        ["espanol"] = "es",
        ["en"] = "en",
        ["eng"] = "en",
        ["english"] = "en"
    };

    public static string? GetCanonicalGender(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return null;
        }

        var normalized = raw.Trim();
        if (GenderMap.TryGetValue(normalized, out var canonical))
        {
            return canonical;
        }

        return string.Equals(normalized, "male", StringComparison.OrdinalIgnoreCase)
            || string.Equals(normalized, "female", StringComparison.OrdinalIgnoreCase)
            || string.Equals(normalized, "diverse", StringComparison.OrdinalIgnoreCase)
            ? normalized.ToLowerInvariant()
            : null;
    }

    public static string? GetCanonicalLanguage(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return null;
        }

        var normalized = raw.Trim();
        if (LanguageMap.TryGetValue(normalized, out var canonical))
        {
            return canonical;
        }

        return string.Equals(normalized, "it", StringComparison.OrdinalIgnoreCase)
            || string.Equals(normalized, "es", StringComparison.OrdinalIgnoreCase)
            || string.Equals(normalized, "en", StringComparison.OrdinalIgnoreCase)
            ? normalized.ToLowerInvariant()
            : null;
    }

    public static bool IsSupportedGender(string? raw)
        => GetCanonicalGender(raw) is not null;

    public static bool IsSupportedLanguage(string? raw)
        => GetCanonicalLanguage(raw) is not null;

    public static IReadOnlyList<string> SupportedGenderValues { get; } = new[] { "male", "female", "diverse" };
    public static IReadOnlyList<string> SupportedLanguageValues { get; } = new[] { "it", "es", "en" };
}
