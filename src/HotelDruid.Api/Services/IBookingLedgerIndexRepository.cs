namespace HotelDruid.Api.Services;

/// <summary>
/// Maintains a lightweight, append-only list of ledger entry IDs that belong to a booking.
/// Stored as _ledger_index.json under data/bookings/{bookingId}/.
/// This is a projection helper — the ledger remains the single source of truth.
/// </summary>
public interface IBookingLedgerIndexRepository
{
    /// <summary>
    /// Appends a ledger entry ID to the booking's index (idempotent).
    /// </summary>
    Task AppendEntryIdAsync(string bookingId, string ledgerEntryId);

    /// <summary>
    /// Returns all ledger entry IDs recorded for the booking (in append order).
    /// </summary>
    Task<List<string>> GetEntryIdsAsync(string bookingId);
}