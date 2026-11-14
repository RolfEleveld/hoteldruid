using HotelDruid.Api.Models;
using HotelDruid.Shared;

namespace HotelDruid.Api.Services;

/// <summary>
/// Booking figures taken from the ledger. Room revenue, extras, deposits, and the
/// amount due are the buckets of the chart passed in. USALI is the default chart.
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
        => From(lines, AccountChart.Usali);

    public static BookingLedgerTotals From(IEnumerable<LedgerEntry> lines, AccountChart chart)
    {
        decimal stay = 0;
        decimal extra = 0;
        decimal deposits = 0;
        decimal due = 0;
        decimal payments = 0;
        decimal refunds = 0;

        foreach (var line in lines)
        {
            if (string.Equals(line.EntryType, ChartAdoptionLog.EntryType, StringComparison.OrdinalIgnoreCase))
                continue;
            if (line.AccountCode == chart.RoomRevenue)
                stay += line.Credit - line.Debit;
            else if (line.AccountCode == chart.OtherIncome)
                extra += line.Credit - line.Debit;
            else if (line.AccountCode == chart.GuestDeposits)
                deposits += line.Credit - line.Debit;
            else if (line.AccountCode == chart.GuestLedger)
            {
                due += line.Debit - line.Credit;
                if (Is(line, "booking_payment") && line.Credit > 0)
                    payments += line.Credit;
                if (Is(line, "booking_refund") && line.Debit > 0)
                    refunds += line.Debit;
            }
        }

        return new BookingLedgerTotals(stay, extra, stay + extra, payments, refunds, deposits, due);
    }

    private static bool Is(LedgerEntry line, string entryType)
        => string.Equals(line.EntryType, entryType, StringComparison.OrdinalIgnoreCase);
}
