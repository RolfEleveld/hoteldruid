using HotelDruid.Api.Models;
using HotelDruid.Shared;

namespace HotelDruid.Api.Services;

/// <summary>
/// Reads booking payments, deposits, and refunds from the global ledger.
/// One journal is one business event. The per-booking list is this projection.
/// </summary>
public static class BookingTransactionProjection
{
    public readonly record struct Totals(decimal Payments, decimal Refunds, decimal Deposits);

    /// <summary>
    /// Index first, then a date-range scan for bookings that have no index yet.
    /// </summary>
    public static async Task<List<LedgerEntry>> LoadAsync(
        ILedgerRepository ledger,
        IBookingLedgerIndexRepository index,
        string bookingId,
        DateTime fallbackFrom,
        DateTime fallbackTo)
    {
        var entryIds = await index.GetEntryIdsAsync(bookingId);
        if (entryIds.Count > 0)
            return await ledger.GetEntriesByIdsAsync(entryIds);

        return await ledger.GetEntriesForBookingAsync(bookingId, fallbackFrom, fallbackTo);
    }

    /// <summary>
    /// One row per booking-transaction journal, ordered by time, numbered 1..n.
    /// </summary>
    public static List<BookingTransactionDto> Project(IEnumerable<LedgerEntry> lines)
        => Materialize(lines).Select(row => row.Dto).ToList();

    /// <summary>
    /// The projected row for one journal, using the same sequence the list would assign.
    /// </summary>
    public static BookingTransactionDto? ProjectJournal(IEnumerable<LedgerEntry> lines, string? journalId)
    {
        if (string.IsNullOrWhiteSpace(journalId))
            return null;

        return Materialize(lines)
            .FirstOrDefault(row => string.Equals(row.JournalId, journalId, StringComparison.Ordinal))
            ?.Dto;
    }

    public static Totals Sum(IEnumerable<BookingTransactionDto> transactions)
    {
        decimal payments = 0;
        decimal refunds = 0;
        decimal deposits = 0;

        foreach (var transaction in transactions)
        {
            var type = transaction.Type ?? string.Empty;
            var appliedTo = transaction.AppliedTo ?? string.Empty;
            var isRefund = type.Contains("refund", StringComparison.OrdinalIgnoreCase);
            var isDeposit = type.Contains("deposit", StringComparison.OrdinalIgnoreCase)
                || string.Equals(appliedTo, "deposit", StringComparison.OrdinalIgnoreCase);

            if (isRefund)
            {
                refunds += transaction.Amount;
                if (isDeposit)
                    deposits -= transaction.Amount;
                continue;
            }

            if (isDeposit)
            {
                deposits += transaction.Amount;
                continue;
            }

            if (type.Contains("payment", StringComparison.OrdinalIgnoreCase)
                || type.Contains("charge", StringComparison.OrdinalIgnoreCase)
                || type.Contains("prepaid", StringComparison.OrdinalIgnoreCase))
            {
                payments += transaction.Amount;
            }
        }

        return new Totals(payments, refunds, deposits);
    }

    private static List<ProjectedRow> Materialize(IEnumerable<LedgerEntry> lines)
    {
        var groups = lines
            .Where(line => string.Equals(line.SourceType, "booking-transaction", StringComparison.OrdinalIgnoreCase))
            .GroupBy(GroupKey)
            .Select(group =>
            {
                var ordered = group
                    .OrderByDescending(line => line.Debit)
                    .ThenBy(line => line.Id, StringComparer.Ordinal)
                    .ToList();
                var primary = ordered[0];
                return new
                {
                    JournalId = string.IsNullOrWhiteSpace(primary.JournalId) ? null : primary.JournalId,
                    Timestamp = ordered.Min(line => line.Timestamp),
                    SortKey = group.Key,
                    BookingId = FirstNonEmpty(ordered.Select(line => line.BookingId)),
                    Type = DisplayType(primary.EntryType),
                    Amount = EventAmount(ordered),
                    Description = primary.Notes,
                    AppliedTo = FirstNonEmpty(ordered.Select(line => line.AppliedTo)),
                    CreatedBy = FirstNonEmpty(ordered.Select(line => line.CreatedBy))
                };
            })
            .OrderBy(row => row.Timestamp)
            .ThenBy(row => row.SortKey, StringComparer.Ordinal)
            .ToList();

        var projected = new List<ProjectedRow>(groups.Count);
        for (var i = 0; i < groups.Count; i++)
        {
            var row = groups[i];
            projected.Add(new ProjectedRow(
                row.JournalId,
                new BookingTransactionDto(
                    row.BookingId,
                    i + 1,
                    row.Timestamp,
                    row.Type,
                    row.Amount,
                    row.Description,
                    row.AppliedTo,
                    row.CreatedBy)));
        }

        return projected;
    }

    private static string GroupKey(LedgerEntry line)
    {
        if (!string.IsNullOrWhiteSpace(line.JournalId))
            return line.JournalId!;

        if (!string.IsNullOrWhiteSpace(line.Id))
            return line.Id!;

        return $"{line.Timestamp:O}|{line.SourceId}|{line.EntryType}";
    }

    private static decimal EventAmount(IReadOnlyList<LedgerEntry> lines)
    {
        decimal amount = 0;
        foreach (var line in lines)
        {
            var side = line.Debit > 0 ? line.Debit : line.Credit;
            if (side > amount)
                amount = side;
        }

        return amount;
    }

    private static string? DisplayType(string? entryType)
    {
        if (string.IsNullOrWhiteSpace(entryType))
            return entryType;

        const string prefix = "booking_";
        return entryType.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
            ? entryType[prefix.Length..]
            : entryType;
    }

    private static string? FirstNonEmpty(IEnumerable<string?> values)
        => values.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value));

    private sealed record ProjectedRow(string? JournalId, BookingTransactionDto Dto);
}
