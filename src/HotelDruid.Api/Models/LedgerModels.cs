namespace HotelDruid.Api.Models;

/// <summary>
/// Immutable ledger entry representing a single financial or booking event.
/// Stored as _seq_NNN.json under the daily ledger partition.
/// </summary>
public class LedgerEntry
{
    /// <summary>Unique entry ID (GUID-base32 or ledger_date_seq format).</summary>
    public string? Id { get; set; }

    /// <summary>Auto-assigned sequence number within the day partition (1-based).</summary>
    public int Sequence { get; set; }

    /// <summary>UTC timestamp of the event.</summary>
    public DateTime Timestamp { get; set; }

    /// <summary>Entry type: "payment", "booking_offer", "checkin_charge", "refund", etc.</summary>
    public string? EntryType { get; set; }

    /// <summary>Associated booking GUID-base32 ID (if applicable).</summary>
    public string? BookingId { get; set; }

    /// <summary>Monetary amount for this entry.</summary>
    public decimal? Amount { get; set; }

    /// <summary>Payment method: "cash", "card", "transfer", etc.</summary>
    public string? Method { get; set; }

    /// <summary>Status: "confirmed", "pending", "cancelled", "completed".</summary>
    public string? Status { get; set; }

    /// <summary>Staff member or system that created this entry.</summary>
    public string? CreatedBy { get; set; }

    /// <summary>Free-form notes or additional context.</summary>
    public string? Notes { get; set; }
}

/// <summary>
/// Daily snapshot of the ledger state at midnight.
/// Stored as _snapshot.json under the daily ledger partition.
/// The snapshot captures all entries up to its SequenceAt; incremental
/// _seq_NNN.json files cover entries with Sequence > SequenceAt.
/// </summary>
public class LedgerSnapshot
{
    /// <summary>The date this snapshot covers (UTC date only).</summary>
    public DateTime Date { get; set; }

    /// <summary>UTC timestamp when this snapshot was created.</summary>
    public DateTime Timestamp { get; set; }

    /// <summary>All consolidated entries captured in this snapshot.</summary>
    public List<LedgerEntry> Entries { get; set; } = new();

    /// <summary>Total balance for the day at snapshot time.</summary>
    public decimal TotalDayBalance { get; set; }

    /// <summary>The highest sequence number included in this snapshot (0 = empty).</summary>
    public int SequenceAt { get; set; }
}

