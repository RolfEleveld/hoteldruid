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

    public static IReadOnlyList<ChartAccountDto> Chart => AccountChart.Usali.Accounts;
}

public interface IFinancialPostingService
{
    Task<LedgerEntry> PostBalancedAsync(DateTime date, string entryType, string description, decimal amount, string debitAccount, string debitGroup, string creditAccount, string creditGroup, string? bookingId = null, string? cashRegisterId = null, string? payerName = null, string? sourceType = null, string? sourceId = null, string? createdBy = null, string? appliedTo = null);
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
        string? appliedTo = null)
    {
        if (amount == 0) throw new ArgumentException("Amount must be non-zero");

        if (string.IsNullOrWhiteSpace(createdBy))
            createdBy = RequestActor.KnownName(_httpContextAccessor?.HttpContext);

        var journalId = Guid.NewGuid().ToString("N");
        var when = date == default ? DateTime.UtcNow : date;
        var magnitude = Math.Abs(amount);
        var applied = string.IsNullOrWhiteSpace(appliedTo) ? null : appliedTo.Trim();

        var debitEntry = Line(journalId, when, entryType, description, bookingId, cashRegisterId, payerName, sourceType, sourceId, createdBy, applied, debitAccount, debitGroup, magnitude, 0);
        var creditEntry = Line(journalId, when, entryType, description, bookingId, cashRegisterId, payerName, sourceType, sourceId, createdBy, applied, creditAccount, creditGroup, 0, magnitude);

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
        decimal credit) => new()
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
        Status = "posted"
    };
}
