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

    /// <summary>Associated booking ID. Empty for hotel-level postings such as rent, payroll, or petty cash.</summary>
    public string? BookingId { get; set; }

    /// <summary>Shared id for the balanced lines of one business event.</summary>
    public string? JournalId { get; set; }

    /// <summary>USALI/GAAP account code. The booking reference is a dimension, not the account.</summary>
    public string? AccountCode { get; set; }

    /// <summary>Account group: asset, liability, equity, revenue, or expense.</summary>
    public string? AccountGroup { get; set; }

    /// <summary>Debit amount. A journal balances when the sum of debits equals the sum of credits.</summary>
    public decimal Debit { get; set; }

    /// <summary>Credit amount.</summary>
    public decimal Credit { get; set; }

    /// <summary>Cash register that received or paid this line. Empty when the line is not a cash movement.</summary>
    public string? CashRegisterId { get; set; }

    /// <summary>Guest or payer when the movement is not tied to a staying booking.</summary>
    public string? PayerName { get; set; }

    /// <summary>Source document, such as money-history, expense, booking-cost, or booking-transaction.</summary>
    public string? SourceType { get; set; }

    /// <summary>Id of the source document that produced this line.</summary>
    public string? SourceId { get; set; }

    /// <summary>Monetary amount for this entry. Positive amounts are inflows; expenses are stored negative.</summary>
    public decimal? Amount { get; set; }

    /// <summary>Payment method: "cash", "card", "transfer", etc.</summary>
    public string? Method { get; set; }

    /// <summary>Status: "confirmed", "pending", "cancelled", "completed".</summary>
    public string? Status { get; set; }

    /// <summary>Staff member or system that created this entry.</summary>
    public string? CreatedBy { get; set; }

    /// <summary>Balance this booking movement applies to: stay_balance, deposit, or extras. Empty for hotel-level lines.</summary>
    public string? AppliedTo { get; set; }

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

