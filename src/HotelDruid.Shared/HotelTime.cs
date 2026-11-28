namespace HotelDruid.Shared;

/// <summary>
/// The hotel's clock. Stored moments stay in UTC. The hotel zone decides the
/// business date and the time shown on the desk.
/// </summary>
public static class HotelTime
{
    public static string Resolve(string? configured)
    {
        if (!string.IsNullOrWhiteSpace(configured))
        {
            try
            {
                _ = TimeZoneInfo.FindSystemTimeZoneById(configured.Trim());
                return configured.Trim();
            }
            catch (TimeZoneNotFoundException)
            {
            }
            catch (InvalidTimeZoneException)
            {
            }
        }

        return TimeZoneInfo.Local.Id;
    }

    public static TimeZoneInfo Zone(string? configured) =>
        TimeZoneInfo.FindSystemTimeZoneById(Resolve(configured));

    public static DateTime Now(string? configured) =>
        TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Zone(configured));

    public static DateOnly Today(string? configured) =>
        DateOnly.FromDateTime(Now(configured));

    public static DateTime Display(DateTime stored, string? configured)
    {
        var utc = stored.Kind switch
        {
            DateTimeKind.Utc => stored,
            DateTimeKind.Local => stored.ToUniversalTime(),
            _ => DateTime.SpecifyKind(stored, DateTimeKind.Utc)
        };
        return TimeZoneInfo.ConvertTimeFromUtc(utc, Zone(configured));
    }
}
