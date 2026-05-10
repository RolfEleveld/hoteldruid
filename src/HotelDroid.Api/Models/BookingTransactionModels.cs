namespace HotelDroid.Api.Models;

/// <summary>
/// Immutable per-booking transaction record.
/// Stored as _seq_NNN.json under data/bookings/{bookingId}_transactions/.
/// </summary>
public class BookingTransaction
{
    /// <summary>GUID-base32 ID of the booking this transaction belongs to.</summary>
    public string? BookingId { get; set; }

    /// <summary>Auto-assigned sequence number for this booking (1-based).</summary>
    public int Sequence { get; set; }

    /// <summary>UTC timestamp of the transaction.</summary>
    public DateTime Timestamp { get; set; }

    /// <summary>Transaction type: "checkin_charge", "additional_charge", "payment", "refund".</summary>
    public string? Type { get; set; }

    /// <summary>Monetary amount for this transaction.</summary>
    public decimal Amount { get; set; }

    /// <summary>Human-readable description of the charge or payment.</summary>
    public string? Description { get; set; }

    /// <summary>Balance target: "stay_balance", "deposit", "extras".</summary>
    public string? AppliedTo { get; set; }

    /// <summary>Staff member or system that created this transaction.</summary>
    public string? CreatedBy { get; set; }
}

/// <summary>
/// Index file stored at data/bookings/{bookingId}_transactions/_index.json.
/// Tracks the next available sequence number to avoid file-scan on every append.
/// </summary>
public class BookingTransactionIndex
{
    /// <summary>The booking this index belongs to.</summary>
    public string? BookingId { get; set; }

    /// <summary>The next sequence number to use when appending a transaction.</summary>
    public int NextSequence { get; set; } = 1;

    /// <summary>UTC timestamp of the last update.</summary>
    public DateTime LastUpdated { get; set; }
}
