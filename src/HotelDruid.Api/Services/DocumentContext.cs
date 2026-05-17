namespace HotelDruid.Api.Services;

/// <summary>
/// Complete data context assembled for a given booking before template rendering.
/// All lists are included so templates can iterate them with FOREACH markers.
/// </summary>
public sealed class DocumentContext
{
    // --- Hotel (from SystemConfiguration.Settings) ---
    public string HotelName { get; init; } = string.Empty;
    public string HotelAddress { get; init; } = string.Empty;
    public string HotelCity { get; init; } = string.Empty;
    public string HotelPhone { get; init; } = string.Empty;
    public string HotelEmail { get; init; } = string.Empty;
    public string HotelVatNumber { get; init; } = string.Empty;
    public string HotelCurrency { get; init; } = string.Empty;

    // --- Booking ---
    public string BookingId { get; init; } = string.Empty;
    public int BookingYear { get; init; }
    public string BookingArrivalDate { get; init; } = string.Empty;
    public string BookingDepartureDate { get; init; } = string.Empty;
    public int BookingNights { get; init; }
    public string BookingStatus { get; init; } = string.Empty;
    public string BookingNotes { get; init; } = string.Empty;

    // --- Primary client (booking holder) ---
    public string ClientLastName { get; init; } = string.Empty;
    public string ClientFirstName { get; init; } = string.Empty;
    public string ClientFullName { get; init; } = string.Empty;
    public string ClientTitle { get; init; } = string.Empty;
    public string ClientEmail { get; init; } = string.Empty;
    public string ClientPhone { get; init; } = string.Empty;
    public string ClientStreet { get; init; } = string.Empty;
    public string ClientStreetNumber { get; init; } = string.Empty;
    public string ClientCity { get; init; } = string.Empty;
    public string ClientPostalCode { get; init; } = string.Empty;
    public string ClientRegion { get; init; } = string.Empty;
    public string ClientNation { get; init; } = string.Empty;
    public string ClientTaxCode { get; init; } = string.Empty;
    public string ClientVatNumber { get; init; } = string.Empty;

    // --- Room ---
    public string RoomName { get; init; } = string.Empty;
    public string RoomFloor { get; init; } = string.Empty;
    public string RoomHouseNumber { get; init; } = string.Empty;
    public int RoomCapacity { get; init; }

    // --- Financial summary ---
    public string InvoiceTotalAmount { get; init; } = string.Empty;
    public string InvoiceCurrency { get; init; } = string.Empty;

    // --- Document meta ---
    public string DocumentToday { get; init; } = string.Empty;
    public string DocumentTemplateType { get; init; } = string.Empty;

    // --- Lists (expanded by FOREACH markers) ---
    public IReadOnlyList<GuestContextRow> Guests { get; init; } = [];
    public IReadOnlyList<CostContextRow> Costs { get; init; } = [];
    public IReadOnlyList<PaymentContextRow> Payments { get; init; } = [];
}

public sealed class GuestContextRow
{
    public int GuestNumber { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string Nationality { get; init; } = string.Empty;
    public string DateOfBirth { get; init; } = string.Empty;
}

public sealed class CostContextRow
{
    public string Description { get; init; } = string.Empty;
    public string Amount { get; init; } = string.Empty;
    public string TariffId { get; init; } = string.Empty;
}

public sealed class PaymentContextRow
{
    public string Amount { get; init; } = string.Empty;
    public string Date { get; init; } = string.Empty;
    public string Method { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
}
