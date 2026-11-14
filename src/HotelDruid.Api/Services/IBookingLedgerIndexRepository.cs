namespace HotelDruid.Api.Services;

/// <summary>
/// Maintains a lightweight, append-only list of ledger entry IDs that belong to a booking.
/// Stored as _ledger_index.json under data/bookings/{bookingId}/.
/// This is a projection helper — the ledger remains the single source of truth.
/// </summary>
public interface IBookingLedgerIndexRepository
{
    /// <summary>
    /// Ids in append order, or null when this booking has not been indexed yet.
    /// </summary>
    Task<List<string>?> ReadIfPresentAsync(string bookingId);

    /// <summary>
    /// Returns the stored ids. When the file is missing, <paramref name="build"/> runs once and those ids are stored.
    /// </summary>
    Task<List<string>> GetOrBuildAsync(string bookingId, Func<Task<IReadOnlyList<string>>> build);

    /// <summary>
    /// Adds ids when the index already exists. A missing index stays missing so the next read can record the whole history.
    /// </summary>
    Task AppendIfPresentAsync(string bookingId, IReadOnlyCollection<string> entryIds);

    /// <summary>Replaces the booking's ledger index, creating it when missing.</summary>
    Task ReplaceAsync(string bookingId, IReadOnlyList<string> entryIds);
}