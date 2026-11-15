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
            if (line.AccountCode == chart.GuestDeposits)
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

        foreach (var group in lines.Where(line => !string.Equals(line.EntryType, ChartAdoptionLog.EntryType, StringComparison.OrdinalIgnoreCase)).GroupBy(JournalKey))
        {
            var room = Sum(group, chart.RoomRevenue);
            var other = Sum(group, chart.OtherIncome);
            var tax = Sum(group, chart.OutputTax);
            var city = Sum(group, chart.CityTax);
            if (room != 0)
                stay += room + (other == 0 ? tax : 0m);
            if (other != 0)
                extra += other + tax;
            else if (room == 0)
                extra += tax;
            extra += city;
        }

        return new BookingLedgerTotals(stay, extra, stay + extra, payments, refunds, deposits, due);
    }

    private static bool Is(LedgerEntry line, string entryType)
        => string.Equals(line.EntryType, entryType, StringComparison.OrdinalIgnoreCase);

    private static decimal Sum(IEnumerable<LedgerEntry> lines, string account)
        => lines.Where(line => line.AccountCode == account).Sum(line => line.Credit - line.Debit);

    private static string JournalKey(LedgerEntry line)
        => string.IsNullOrWhiteSpace(line.JournalId) ? line.Id ?? string.Empty : line.JournalId;
}
