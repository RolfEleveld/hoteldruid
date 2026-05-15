using HotelDruid.Api.Models;

namespace HotelDruid.Api.Services;

/// <summary>
/// Repository for per-booking transaction sequences.
///
/// Storage layout:
///   data/bookings/{bookingId}_transactions/
///     _index.json       – tracks NextSequence, BookingId, LastUpdated
///     _seq_000001.json  – first transaction
///     _seq_000002.json  – second transaction, etc.
/// </summary>
public interface IBookingTransactionRepository
{
    /// <summary>
    /// Append a new transaction to a booking's sequence.
    /// Auto-assigns the next sequence number.
    /// </summary>
    /// <returns>The transaction with Sequence and Timestamp populated.</returns>
    Task<BookingTransaction> AppendTransactionAsync(string bookingId, BookingTransaction transaction);

    /// <summary>Get all transactions for a booking, ordered by sequence number.</summary>
    Task<List<BookingTransaction>> GetTransactionsAsync(string bookingId);

    /// <summary>Get the next sequence number that will be used for a booking (without consuming it).</summary>
    Task<int> GetNextSequenceAsync(string bookingId);
}

