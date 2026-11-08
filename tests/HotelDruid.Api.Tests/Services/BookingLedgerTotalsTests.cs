using HotelDruid.Api.Models;
using HotelDruid.Api.Services;

namespace HotelDruid.Api.Tests.Services;

public class BookingLedgerTotalsTests
{
    [Fact]
    public void From_RollsRoomExtrasDepositsAndGuestLedger()
    {
        var lines = new[]
        {
            Pair("stay", "booking", "booking_stay_charge", HotelAccounts.GuestLedger, HotelAccounts.RoomRevenue, 300m, "Stay"),
            Pair("extra", "booking-cost", "booking_extra_charge", HotelAccounts.GuestLedger, HotelAccounts.OtherIncome, 25m, "Breakfast"),
            Pair("deposit", "booking", "booking_prepaid", HotelAccounts.Cash, HotelAccounts.GuestDeposits, 40m, "Prepaid"),
            Pair("payment", "booking-payment", "booking_payment", HotelAccounts.Cash, HotelAccounts.GuestLedger, 30m, "Card"),
            Pair("refund", "booking-transaction", "booking_refund", HotelAccounts.GuestLedger, HotelAccounts.Cash, 5m, "Refund")
        }.SelectMany(pair => pair);

        var totals = BookingLedgerTotals.From(lines);

        Assert.Equal(300m, totals.StayTotal);
        Assert.Equal(25m, totals.ExtraTotal);
        Assert.Equal(325m, totals.ChargesTotal);
        Assert.Equal(40m, totals.DepositsHeld);
        Assert.Equal(30m, totals.PaymentsTotal);
        Assert.Equal(5m, totals.RefundsTotal);
        Assert.Equal(300m, totals.TotalDue);
    }

    [Fact]
    public void Project_IncludesStayExtrasAndImportedPayments()
    {
        var lines = new[]
        {
            Pair("stay", "booking", "booking_stay_charge", HotelAccounts.GuestLedger, HotelAccounts.RoomRevenue, 300m, "Stay"),
            Pair("pay", "booking-payment", "booking_payment", HotelAccounts.Cash, HotelAccounts.GuestLedger, 80m, "Imported"),
            Pair("petty", "money-history", "petty_cash_expense", HotelAccounts.OperatingExpense, HotelAccounts.Cash, 4m, "Coffee")
        }.SelectMany(pair => pair);

        var rows = BookingTransactionProjection.Project(lines);

        Assert.Equal(2, rows.Count);
        Assert.Contains(rows, row => row.Type == "stay_charge" && row.Amount == 300m);
        Assert.Contains(rows, row => row.Type == "payment" && row.Amount == 80m);
    }

    private static IEnumerable<LedgerEntry> Pair(string journalId, string sourceType, string entryType, string debitAccount, string creditAccount, decimal amount, string notes)
    {
        yield return new LedgerEntry { JournalId = journalId, SourceType = sourceType, EntryType = entryType, AccountCode = debitAccount, Debit = amount, Notes = notes, BookingId = "b1" };
        yield return new LedgerEntry { JournalId = journalId, SourceType = sourceType, EntryType = entryType, AccountCode = creditAccount, Credit = amount, Notes = notes, BookingId = "b1" };
    }
}
