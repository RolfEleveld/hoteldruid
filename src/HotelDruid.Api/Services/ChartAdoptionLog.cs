using HotelDruid.Api.Models;
using HotelDruid.Shared;
using HotelDruid.Shared.Configuration;

namespace HotelDruid.Api.Services;

/// <summary>
/// Writes a zero-amount journal when setup adopts or changes the chart.
/// The lines carry the numbers in force from that moment. They do not move money.
/// </summary>
public static class ChartAdoptionLog
{
    public const string EntryType = "chart_of_accounts";

    public static bool AlreadyRecorded(SystemConfiguration? previous, AccountChart next)
    {
        if (previous is null || string.IsNullOrWhiteSpace(previous.AccountingStandard))
            return false;
        if (!string.Equals(previous.AccountingStandard.Trim(), next.StandardId, StringComparison.OrdinalIgnoreCase))
            return false;
        if (previous.AccountCodes is null)
            return false;

        foreach (var role in AccountRoles.All)
        {
            if (!previous.AccountCodes.TryGetValue(role, out var code))
                return false;
            if (!string.Equals(code?.Trim(), next.Codes[role], StringComparison.OrdinalIgnoreCase))
                return false;
        }

        return true;
    }

    public static string Describe(SystemConfiguration? previous, AccountChart next)
    {
        var nextName = NameOf(next.StandardId);
        var nextNumbers = Numbers(next);
        var hadExplicit = previous is not null && !string.IsNullOrWhiteSpace(previous.AccountingStandard);
        if (!hadExplicit)
            return $"Chart of accounts set to {nextName}. Journals from this entry forward use {nextNumbers}.";

        var from = AccountChart.ResolveOrUsali(previous!.AccountingStandard, previous.AccountCodes);
        return $"Chart of accounts changed from {NameOf(from.StandardId)} to {nextName}. Journals from this entry forward use {nextNumbers}. Earlier journals keep the numbers they were written with: {Numbers(from)}.";
    }

    public static async Task WriteAsync(ILedgerRepository ledger, SystemConfiguration? previous, AccountChart next, string? createdBy)
    {
        var note = Describe(previous, next);
        var journalId = Guid.NewGuid().ToString("N");
        var when = DateTime.UtcNow;
        foreach (var account in next.Accounts)
        {
            await ledger.AppendEntryAsync(when, new LedgerEntry
            {
                JournalId = journalId,
                Timestamp = when,
                EntryType = EntryType,
                AccountCode = account.Code,
                AccountGroup = account.Group,
                Debit = 0,
                Credit = 0,
                Amount = 0,
                SourceType = "system-configuration",
                SourceId = next.StandardId,
                CreatedBy = string.IsNullOrWhiteSpace(createdBy) ? null : createdBy,
                Notes = note,
                Status = "posted"
            });
        }
    }

    private static string NameOf(string standardId)
    {
        var standard = AccountChart.Standards.FirstOrDefault(item =>
            string.Equals(item.Id, standardId, StringComparison.OrdinalIgnoreCase));
        return standard?.Name ?? standardId;
    }

    private static string Numbers(AccountChart chart)
        => string.Join(", ", AccountRoles.All.Select(role => $"{role} {chart.Codes[role]}"));
}
