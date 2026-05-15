using HotelDruid.Api.Models;

namespace HotelDruid.Api.Services;

/// <summary>
/// Repository for the global immutable financial/booking ledger.
///
/// Storage layout:
///   data/ledger/{year:D4}/{month:D2}/{day:D2}/
///     _snapshot.json       – daily state at midnight (sequenceAt = last included seq)
///     _seq_001.json        – first incremental entry after the snapshot
///     _seq_002.json        – second incremental entry, etc.
/// </summary>
public interface ILedgerRepository
{
    /// <summary>Get the daily snapshot for a given UTC date, or null if none exists.</summary>
    Task<LedgerSnapshot?> GetSnapshotAsync(DateTime date);

    /// <summary>Persist (create or replace) the daily snapshot for a given UTC date.</summary>
    Task SaveSnapshotAsync(DateTime date, LedgerSnapshot snapshot);

    /// <summary>
    /// Append an entry to the ledger for the given UTC date.
    /// Auto-assigns the next sequence number and persists the entry atomically.
    /// </summary>
    /// <returns>The entry with Sequence populated.</returns>
    Task<LedgerEntry> AppendEntryAsync(DateTime date, LedgerEntry entry);

    /// <summary>Get all incremental entries after <paramref name="fromSequence"/> for the given date.</summary>
    Task<List<LedgerEntry>> GetEntriesSinceSequenceAsync(DateTime date, int fromSequence);

    /// <summary>Get all incremental entry files for the given date (does not include snapshot).</summary>
    Task<List<LedgerEntry>> GetEntriesForDateAsync(DateTime date);

    /// <summary>
    /// Get all entries for a specific booking across a date range (inclusive).
    /// Searches both snapshot and incremental entries.
    /// </summary>
    Task<List<LedgerEntry>> GetEntriesForBookingAsync(string bookingId, DateTime from, DateTime to);

    /// <summary>
    /// Consolidate: load the snapshot then apply all incremental entries on top.
    /// Returns an ordered list of all entries for the day.
    /// </summary>
    Task<List<LedgerEntry>> GetConsolidatedAsync(DateTime date);
}

