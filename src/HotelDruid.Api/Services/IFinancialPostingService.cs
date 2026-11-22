using HotelDruid.Api.Models;
using HotelDruid.Shared;

namespace HotelDruid.Api.Services;

public static class HotelAccounts
{
    public const string Cash = "1000";
    public const string GuestDeposits = "2400";
    public const string GuestLedger = "1200";
    public const string RoomRevenue = "4100";
    public const string OtherIncome = "4900";
    public const string OperatingExpense = "6100";
    public const string OutputTax = "2300";
    public const string CityTax = "2500";

    public static IReadOnlyList<ChartAccountDto> Chart => AccountChart.Usali.Accounts;
}

public interface IFinancialPostingService
{
    Task<LedgerEntry> PostBalancedAsync(DateTime date, string entryType, string description, decimal amount, string debitAccount, string debitGroup, string creditAccount, string creditGroup, string? bookingId = null, string? cashRegisterId = null, string? payerName = null, string? sourceType = null, string? sourceId = null, string? createdBy = null, string? appliedTo = null, string? printedInvoiceNumber = null, string? method = null, string? reference = null);

    /// <summary>
    /// Charges a guest or the cash drawer. The quoted amount is what was asked.
    /// Inclusive tax stays inside that amount. Exclusive tax is added. Output tax is posted apart from revenue.
    /// Returns the amount the guest or the drawer was charged.
    /// </summary>
    Task<decimal> PostGuestChargeAsync(DateTime date, string entryType, string description, decimal quoted, decimal? rate, bool inclusive, string debitAccount, string debitGroup, string revenueAccount, string revenueGroup, string outputTaxAccount, string? bookingId = null, string? cashRegisterId = null, string? payerName = null, string? sourceType = null, string? sourceId = null, string? createdBy = null);

    /// <summary>
    /// Reverses a guest or cash charge. Revenue and output tax are debited. The guest or the drawer is credited the gross amount.
    /// </summary>
    Task<decimal> RelieveGuestChargeAsync(DateTime date, string entryType, string description, decimal quoted, decimal? rate, bool inclusive, string creditAccount, string creditGroup, string revenueAccount, string revenueGroup, string outputTaxAccount, string? bookingId = null, string? cashRegisterId = null, string? payerName = null, string? sourceType = null, string? sourceId = null, string? createdBy = null);

    /// <summary>Moves one balance from one booking to another. Cash does not move.</summary>
    Task PostTransferAsync(DateTime date, string entryType, string description, decimal amount, string account, string group, string debitBookingId, string creditBookingId, string? sourceType = null, string? sourceId = null, string? createdBy = null);

    /// <summary>True when this booking source already posted output tax.</summary>
    Task<bool> SourceHasOutputTaxAsync(string bookingId, string sourceId, string outputTaxAccount);

    /// <summary>True when a cash sale posted on this day already carried output tax.</summary>
    Task<bool> SourceHasOutputTaxOnDateAsync(DateTime day, string sourceId, string outputTaxAccount);

    /// <summary>Posts the opposite of every line with this source on that day. Returns false when nothing was found.</summary>
    Task<bool> FlipSourceOnDateAsync(DateTime day, string sourceId, string entryType, string description, string? cashRegisterId = null, string? payerName = null, string? sourceType = null, string? sourceIdForReversal = null);
}

/// <summary>
/// Posts one business event as two balanced lines in the global ledger.
/// BookingId is a dimension. Hotel purchases and petty cash leave it empty.
/// </summary>
public class FinancialPostingService : IFinancialPostingService
{
    private readonly ILedgerRepository _ledger;
    private readonly IBookingLedgerIndexRepository? _bookingIndex;
    private readonly IHttpContextAccessor? _httpContextAccessor;

    public FinancialPostingService(ILedgerRepository ledger, IBookingLedgerIndexRepository? bookingIndex = null, IHttpContextAccessor? httpContextAccessor = null)
    {
        _ledger = ledger;
        _bookingIndex = bookingIndex;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<LedgerEntry> PostBalancedAsync(
        DateTime date,
        string entryType,
        string description,
        decimal amount,
        string debitAccount,
        string debitGroup,
        string creditAccount,
        string creditGroup,
        string? bookingId = null,
        string? cashRegisterId = null,
        string? payerName = null,
        string? sourceType = null,
        string? sourceId = null,
        string? createdBy = null,
        string? appliedTo = null,
        string? printedInvoiceNumber = null,
        string? method = null,
        string? reference = null)
    {
        if (amount == 0) throw new ArgumentException("Amount must be non-zero");

        if (string.IsNullOrWhiteSpace(createdBy))
            createdBy = RequestActor.KnownName(_httpContextAccessor?.HttpContext);

        var journalId = Guid.NewGuid().ToString("N");
        var when = date == default ? DateTime.UtcNow : date;
        var magnitude = Math.Abs(amount);
        var applied = string.IsNullOrWhiteSpace(appliedTo) ? null : appliedTo.Trim();

        var debitEntry = Line(journalId, when, entryType, description, bookingId, cashRegisterId, payerName, sourceType, sourceId, createdBy, applied, debitAccount, debitGroup, magnitude, 0, printedInvoiceNumber, method, reference);
        var creditEntry = Line(journalId, when, entryType, description, bookingId, cashRegisterId, payerName, sourceType, sourceId, createdBy, applied, creditAccount, creditGroup, 0, magnitude, printedInvoiceNumber, method, reference);

        await _ledger.AppendEntryAsync(when, debitEntry);
        await _ledger.AppendEntryAsync(when, creditEntry);

        if (_bookingIndex is not null && !string.IsNullOrWhiteSpace(bookingId))
        {
            var ids = new[] { debitEntry.Id, creditEntry.Id }
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Select(id => id!)
                .ToList();
            await _bookingIndex.AppendIfPresentAsync(bookingId, ids);
        }

        return debitEntry;
    }

    public async Task<decimal> PostGuestChargeAsync(
        DateTime date,
        string entryType,
        string description,
        decimal quoted,
        decimal? rate,
        bool inclusive,
        string debitAccount,
        string debitGroup,
        string revenueAccount,
        string revenueGroup,
        string outputTaxAccount,
        string? bookingId = null,
        string? cashRegisterId = null,
        string? payerName = null,
        string? sourceType = null,
        string? sourceId = null,
        string? createdBy = null)
    {
        var (guest, net, tax) = StayTax.Split(quoted, rate, inclusive);
        if (guest == 0)
            return 0m;
        if (tax == 0 || string.IsNullOrWhiteSpace(outputTaxAccount))
        {
            await PostBalancedAsync(date, entryType, description, guest, debitAccount, debitGroup, revenueAccount, revenueGroup, bookingId, cashRegisterId, payerName, sourceType, sourceId, createdBy);
            return guest;
        }

        if (string.IsNullOrWhiteSpace(createdBy))
            createdBy = RequestActor.KnownName(_httpContextAccessor?.HttpContext);

        var journalId = Guid.NewGuid().ToString("N");
        var when = date == default ? DateTime.UtcNow : date;
        var lines = new List<LedgerEntry>
        {
            Line(journalId, when, entryType, description, bookingId, cashRegisterId, payerName, sourceType, sourceId, createdBy, null, debitAccount, debitGroup, guest, 0)
        };
        if (net != 0)
            lines.Add(Line(journalId, when, entryType, description, bookingId, cashRegisterId, payerName, sourceType, sourceId, createdBy, null, revenueAccount, revenueGroup, 0, net));
        lines.Add(Line(journalId, when, entryType, description, bookingId, cashRegisterId, payerName, sourceType, sourceId, createdBy, null, outputTaxAccount, "liability", 0, tax));
        await AppendAsync(when, bookingId, lines);
        return guest;
    }

    public async Task<decimal> RelieveGuestChargeAsync(
        DateTime date,
        string entryType,
        string description,
        decimal quoted,
        decimal? rate,
        bool inclusive,
        string creditAccount,
        string creditGroup,
        string revenueAccount,
        string revenueGroup,
        string outputTaxAccount,
        string? bookingId = null,
        string? cashRegisterId = null,
        string? payerName = null,
        string? sourceType = null,
        string? sourceId = null,
        string? createdBy = null)
    {
        var (guest, net, tax) = StayTax.Split(quoted, rate, inclusive);
        if (guest == 0)
            return 0m;
        if (tax == 0 || string.IsNullOrWhiteSpace(outputTaxAccount))
        {
            await PostBalancedAsync(date, entryType, description, guest, revenueAccount, revenueGroup, creditAccount, creditGroup, bookingId, cashRegisterId, payerName, sourceType, sourceId, createdBy);
            return guest;
        }

        if (string.IsNullOrWhiteSpace(createdBy))
            createdBy = RequestActor.KnownName(_httpContextAccessor?.HttpContext);

        var journalId = Guid.NewGuid().ToString("N");
        var when = date == default ? DateTime.UtcNow : date;
        var lines = new List<LedgerEntry>();
        if (net != 0)
            lines.Add(Line(journalId, when, entryType, description, bookingId, cashRegisterId, payerName, sourceType, sourceId, createdBy, null, revenueAccount, revenueGroup, net, 0));
        lines.Add(Line(journalId, when, entryType, description, bookingId, cashRegisterId, payerName, sourceType, sourceId, createdBy, null, outputTaxAccount, "liability", tax, 0));
        lines.Add(Line(journalId, when, entryType, description, bookingId, cashRegisterId, payerName, sourceType, sourceId, createdBy, null, creditAccount, creditGroup, 0, guest));
        await AppendAsync(when, bookingId, lines);
        return guest;
    }

    public async Task PostTransferAsync(
        DateTime date,
        string entryType,
        string description,
        decimal amount,
        string account,
        string group,
        string debitBookingId,
        string creditBookingId,
        string? sourceType = null,
        string? sourceId = null,
        string? createdBy = null)
    {
        if (amount == 0)
            throw new ArgumentException("Amount must be non-zero");
        if (string.IsNullOrWhiteSpace(createdBy))
            createdBy = RequestActor.KnownName(_httpContextAccessor?.HttpContext);

        var journalId = Guid.NewGuid().ToString("N");
        var when = date == default ? DateTime.UtcNow : date;
        var magnitude = Math.Abs(amount);
        var debit = Line(journalId, when, entryType, description, debitBookingId, null, null, sourceType, sourceId, createdBy, null, account, group, magnitude, 0);
        var credit = Line(journalId, when, entryType, description, creditBookingId, null, null, sourceType, sourceId, createdBy, null, account, group, 0, magnitude);
        await _ledger.AppendEntryAsync(when, debit);
        await _ledger.AppendEntryAsync(when, credit);
        if (_bookingIndex is not null)
        {
            if (!string.IsNullOrWhiteSpace(debit.Id))
                await _bookingIndex.AppendIfPresentAsync(debitBookingId, [debit.Id]);
            if (!string.IsNullOrWhiteSpace(credit.Id))
                await _bookingIndex.AppendIfPresentAsync(creditBookingId, [credit.Id]);
        }
    }

    public async Task<bool> SourceHasOutputTaxAsync(string bookingId, string sourceId, string outputTaxAccount)
    {
        if (string.IsNullOrWhiteSpace(bookingId) || string.IsNullOrWhiteSpace(sourceId))
            return false;
        var lines = await _ledger.GetEntriesForBookingAsync(bookingId, DateTime.MinValue, DateTime.MaxValue);
        return lines.Any(line =>
            string.Equals(line.SourceId, sourceId, StringComparison.OrdinalIgnoreCase)
            && string.Equals(line.AccountCode, outputTaxAccount, StringComparison.OrdinalIgnoreCase)
            && line.Credit != line.Debit);
    }

    public async Task<bool> FlipSourceOnDateAsync(
        DateTime day,
        string sourceId,
        string entryType,
        string description,
        string? cashRegisterId = null,
        string? payerName = null,
        string? sourceType = null,
        string? sourceIdForReversal = null)
    {
        var found = (await _ledger.GetConsolidatedAsync(day))
            .Where(line => string.Equals(line.SourceId, sourceId, StringComparison.OrdinalIgnoreCase))
            .ToList();
        if (found.Count == 0)
            return false;

        var createdBy = RequestActor.KnownName(_httpContextAccessor?.HttpContext);
        var journalId = Guid.NewGuid().ToString("N");
        var when = DateTime.UtcNow;
        var lines = new List<LedgerEntry>();
        foreach (var account in found.GroupBy(line => line.AccountCode ?? string.Empty))
        {
            var net = account.Sum(line => line.Debit - line.Credit);
            if (net == 0 || string.IsNullOrWhiteSpace(account.Key))
                continue;
            var sample = account.First();
            var debit = net < 0 ? -net : 0;
            var credit = net > 0 ? net : 0;
            lines.Add(Line(journalId, when, entryType, description, sample.BookingId, cashRegisterId ?? sample.CashRegisterId, payerName, sourceType, sourceIdForReversal, createdBy, null, account.Key, sample.AccountGroup ?? string.Empty, debit, credit));
        }

        if (lines.Count == 0)
            return false;
        foreach (var line in lines)
            await _ledger.AppendEntryAsync(when, line);
        if (_bookingIndex is not null)
        {
            foreach (var bookingGroup in lines.Where(line => !string.IsNullOrWhiteSpace(line.BookingId)).GroupBy(line => line.BookingId!))
            {
                var ids = bookingGroup.Select(line => line.Id).Where(id => !string.IsNullOrWhiteSpace(id)).Select(id => id!).ToList();
                if (ids.Count > 0)
                    await _bookingIndex.AppendIfPresentAsync(bookingGroup.Key, ids);
            }
        }

        return true;
    }

    public async Task<bool> SourceHasOutputTaxOnDateAsync(DateTime day, string sourceId, string outputTaxAccount)
    {
        if (string.IsNullOrWhiteSpace(sourceId))
            return false;
        var lines = await _ledger.GetConsolidatedAsync(day);
        return lines.Any(line =>
            string.Equals(line.SourceId, sourceId, StringComparison.OrdinalIgnoreCase)
            && string.Equals(line.AccountCode, outputTaxAccount, StringComparison.OrdinalIgnoreCase)
            && line.Credit != line.Debit);
    }

    private async Task AppendAsync(DateTime when, string? bookingId, IReadOnlyList<LedgerEntry> lines)
    {
        foreach (var line in lines)
            await _ledger.AppendEntryAsync(when, line);

        if (_bookingIndex is null || string.IsNullOrWhiteSpace(bookingId))
            return;

        var ids = lines
            .Select(line => line.Id)
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Select(id => id!)
            .ToList();
        if (ids.Count > 0)
            await _bookingIndex.AppendIfPresentAsync(bookingId, ids);
    }

    private static LedgerEntry Line(
        string journalId,
        DateTime when,
        string entryType,
        string description,
        string? bookingId,
        string? cashRegisterId,
        string? payerName,
        string? sourceType,
        string? sourceId,
        string? createdBy,
        string? appliedTo,
        string accountCode,
        string accountGroup,
        decimal debit,
        decimal credit,
        string? printedInvoiceNumber = null,
        string? method = null,
        string? reference = null) => new()
    {
        JournalId = journalId,
        Timestamp = when,
        EntryType = entryType,
        BookingId = string.IsNullOrWhiteSpace(bookingId) ? null : bookingId,
        AccountCode = accountCode,
        AccountGroup = accountGroup,
        Debit = debit,
        Credit = credit,
        Amount = debit - credit,
        CashRegisterId = cashRegisterId,
        PayerName = payerName,
        SourceType = sourceType,
        SourceId = sourceId,
        CreatedBy = createdBy,
        AppliedTo = appliedTo,
        Notes = description,
        Method = string.IsNullOrWhiteSpace(method) ? null : method.Trim(),
        Reference = string.IsNullOrWhiteSpace(reference) ? null : reference.Trim(),
        Status = "posted",
        PrintedInvoiceNumber = string.IsNullOrWhiteSpace(printedInvoiceNumber) ? null : printedInvoiceNumber.Trim()
    };
}
