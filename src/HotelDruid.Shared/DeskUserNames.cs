namespace HotelDruid.Shared;

/// <summary>
/// A desk name stored by the browser and sent on API calls.
/// It is a label on journals, not a login.
/// </summary>
public static class DeskUserNames
{
    public const string HeaderName = "X-HotelDruid-Desk-User";
    public const string StorageKey = "hoteldruid.deskUser";

    public static string? Sanitize(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return null;

        var value = raw.Trim();
        if (value.Length is < 1 or > 80 || !char.IsLetterOrDigit(value[0]))
            return null;

        foreach (var character in value)
        {
            if (!char.IsLetterOrDigit(character) && character is not ' ' and not '.' and not '\'' and not '-' and not '_')
                return null;
        }

        return value;
    }
}
