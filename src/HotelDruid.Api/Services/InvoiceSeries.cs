using System.Globalization;
using HotelDruid.Api.Models;
using HotelDruid.Shared.Configuration;

namespace HotelDruid.Api.Services;

/// <summary>
/// Guest-facing invoice numbers are one series per year, printed as 2026-00042.
/// An imported number stays on the ledger lines. A print made here takes the next number.
/// </summary>
public static class InvoiceSeries
{
    public static async Task<string> EnsureIssuedAsync(
        IKeyValueStore store,
        ISystemConfigurationStore configStore,
        string bookingId,
        BookingStorageModel booking,
        string templateType)
    {
        if (!IsInvoice(templateType))
            return booking.IssuedInvoiceNumber ?? booking.PrintedInvoiceNumber ?? "";

        if (!string.IsNullOrWhiteSpace(booking.IssuedInvoiceNumber))
            return booking.IssuedInvoiceNumber;

        var config = await configStore.GetAsync() ?? new SystemConfiguration();
        config.InvoiceSequence ??= new Dictionary<string, int>();
        var year = booking.Year > 0 ? booking.Year : DateTime.UtcNow.Year;
        var key = year.ToString(CultureInfo.InvariantCulture);
        config.InvoiceSequence.TryGetValue(key, out var last);
        last++;
        config.InvoiceSequence[key] = last;
        config.UpdatedUtc = DateTime.UtcNow;
        await configStore.SaveAsync(config);

        booking.IssuedInvoiceNumber = $"{key}-{last:00000}";
        await store.UpdateAsync("bookings", bookingId, booking);
        return booking.IssuedInvoiceNumber;
    }

    public static bool IsInvoice(string? templateType)
    {
        var type = templateType?.Trim().ToLowerInvariant();
        return type is "invoice" or "rechnung";
    }
}
