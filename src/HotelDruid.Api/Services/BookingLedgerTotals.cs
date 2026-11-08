using HotelDruid.Api.Models;

namespace HotelDruid.Api.Services;

/// <summary>
/// Booking figures taken from the ledger. Room revenue is 4100, extras are 4900,
/// deposits still held are 2400, and the amount due is the net of 1200.
/// </summary>
public readonly record struct BookingLedgerTotals(
    decimal StayTotal,
    decimal ExtraTotal,
    decimal ChargesTotal,
    decimal PaymentsTotal,
    decimal RefundsTotal,
    decimal DepositsHeld,
    decimal TotalDue)
{
    public static BookingLedgerTotals From(IEnumerable<LedgerEntry> lines)
    {
        decimal stay = 0;
        decimal extra = 0;
        decimal deposits = 0;
        decimal due = 0;
        decimal payments = 0;
        decimal refunds = 0;

        foreach (var line in lines)
        {
            switch (line.AccountCode)
            {
                case HotelAccounts.RoomRevenue:
                    stay += line.Credit - line.Debit;
                    break;
                case HotelAccounts.OtherIncome:
                    extra += line.Credit - line.Debit;
                    break;
                case HotelAccounts.GuestDeposits:
                    deposits += line.Credit - line.Debit;
                    break;
                case HotelAccounts.GuestLedger:
                    due += line.Debit - line.Credit;
                    if (Is(line, "booking_payment") && line.Credit > 0)
                        payments += line.Credit;
                    if (Is(line, "booking_refund") && line.Debit > 0)
                        refunds += line.Debit;
                    break;
            }
        }

        return new BookingLedgerTotals(stay, extra, stay + extra, payments, refunds, deposits, due);
    }

    private static bool Is(LedgerEntry line, string entryType)
        => string.Equals(line.EntryType, entryType, StringComparison.OrdinalIgnoreCase);
}
