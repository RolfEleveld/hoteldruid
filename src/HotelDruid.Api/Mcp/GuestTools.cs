using System.ComponentModel;
using System.Text.Json.Nodes;
using ModelContextProtocol.Server;

namespace HotelDruid.Mcp;

[McpServerToolType]
public sealed class GuestTools(HotelClient hotel)
{
    [McpServerTool(Name = "read_guest", Title = "Read a guest", ReadOnly = true, Idempotent = true, Destructive = false, OpenWorld = false, UseStructuredContent = true)]
    [Description("Read the guest record: name, contact, nationality, birth, identity document, tax numbers, and address. The hotel does not store a scan of the document.")]
    public Task<HotelReply> Read(
        [Description("Guest id.")] string guestId,
        CancellationToken cancellationToken = default)
        => HotelCall.Run(async () =>
        {
            var guest = await hotel.GetAsync($"api/clients/{Uri.EscapeDataString(guestId)}", cancellationToken);
            if (guest is null)
                return new HotelReply(false, "That guest was not found.", null, "search_hotel by surname.");
            return new HotelReply(true, GuestLabel(guest.Value), new JsonObject { ["guest"] = JsonNode.Parse(guest.Value.GetRawText()) }, "save_guest to correct a field. attach_guest to put this person on a stay.");
        });

    [McpServerTool(Name = "save_guest", Title = "Save a guest", ReadOnly = false, Idempotent = true, Destructive = false, OpenWorld = false, UseStructuredContent = true)]
    [Description("Create a guest, or update the guest id that is passed. Omitted fields on an update stay as they are. Use this for the name, contact, address, birth, nationality, document, and tax numbers the clerk copied from the document.")]
    public Task<HotelReply> Save(
        [Description("Existing guest id. Omit to create.")] string? guestId = null,
        [Description("Surname.")] string? lastName = null,
        [Description("Given name.")] string? firstName = null,
        [Description("Email.")] string? email = null,
        [Description("Phone.")] string? phone = null,
        [Description("Nationality.")] string? nationality = null,
        [Description("Second nationality, when the registration standard asks for it.")] string? secondNationality = null,
        [Description("Gender.")] string? gender = null,
        [Description("Date of birth yyyy-MM-dd.")] string? dateOfBirth = null,
        [Description("Place of birth.")] string? birthCity = null,
        [Description("Document type, as free text.")] string? documentType = null,
        [Description("Document number.")] string? documentNumber = null,
        [Description("Document expiry yyyy-MM-dd.")] string? documentExpiry = null,
        [Description("Place of issue.")] string? documentCity = null,
        [Description("Street.")] string? street = null,
        [Description("City.")] string? city = null,
        [Description("Postal code.")] string? postalCode = null,
        [Description("Country of the address.")] string? nation = null,
        [Description("Italian codice fiscale, when the invoice needs it.")] string? taxCode = null,
        [Description("VAT number, when the invoice needs it.")] string? vatNumber = null,
        [Description("Free-text note.")] string? notes = null,
        CancellationToken cancellationToken = default)
        => HotelCall.Run(() => SaveCore(guestId, lastName, firstName, email, phone, nationality, secondNationality, gender, dateOfBirth, birthCity, documentType, documentNumber, documentExpiry, documentCity, street, city, postalCode, nation, taxCode, vatNumber, notes, cancellationToken));

    [McpServerTool(Name = "attach_guest", Title = "Attach a guest to a stay", ReadOnly = false, Idempotent = false, Destructive = false, OpenWorld = false, UseStructuredContent = true)]
    [Description("Add another named person to a booking. The booking year is the arrival year. Covered by lead means a child or group member who does not need a full card, where the registration standard allows that. Card state is printed, checked, or filed.")]
    public Task<HotelReply> Attach(
        [Description("Booking id.")] string bookingId,
        [Description("Existing guest id. Omit to create from the name.")] string? guestId = null,
        [Description("Surname, when guestId is omitted.")] string? lastName = null,
        [Description("Given name.")] string? firstName = null,
        [Description("1 is the lead's companion number. Default 2.")] int guestNumber = 2,
        [Description("True when this person is covered by the lead guest.")] bool coveredByLead = false,
        [Description("printed, checked, or filed. Omit until the paper card moves.")] string? cardState = null,
        CancellationToken cancellationToken = default)
        => HotelCall.Run(() => AttachCore(bookingId, guestId, lastName, firstName, guestNumber, coveredByLead, cardState, cancellationToken));

    private async Task<HotelReply> SaveCore(string? guestId, string? lastName, string? firstName, string? email, string? phone, string? nationality, string? secondNationality, string? gender, string? dateOfBirth, string? birthCity, string? documentType, string? documentNumber, string? documentExpiry, string? documentCity, string? street, string? city, string? postalCode, string? nation, string? taxCode, string? vatNumber, string? notes, CancellationToken cancellationToken)
    {
        if (dateOfBirth is not null && HotelClient.Date(dateOfBirth) is null)
            return new HotelReply(false, "Date of birth must be yyyy-MM-dd.", null, "Pass the date, or omit it.");
        if (documentExpiry is not null && HotelClient.Date(documentExpiry) is null)
            return new HotelReply(false, "Document expiry must be yyyy-MM-dd.", null, "Pass the date, or omit it.");

        JsonObject body;
        if (string.IsNullOrWhiteSpace(guestId))
        {
            if (string.IsNullOrWhiteSpace(lastName))
                return new HotelReply(false, "A new guest needs a surname.", null, "Ask for the surname.");
            body = new JsonObject { ["LastName"] = lastName.Trim() };
        }
        else
        {
            var current = await hotel.GetAsync($"api/clients/{Uri.EscapeDataString(guestId)}", cancellationToken);
            if (current is null)
                return new HotelReply(false, "That guest was not found.", null, "search_hotel by surname.");
            body = JsonNode.Parse(current.Value.GetRawText())!.AsObject();
        }

        Set(body, "LastName", lastName);
        Set(body, "FirstName", firstName);
        Set(body, "Email", email);
        Set(body, "Phone", phone);
        Set(body, "Nationality", nationality);
        Set(body, "SecondNationality", secondNationality);
        Set(body, "Gender", gender);
        Set(body, "DateOfBirth", dateOfBirth);
        Set(body, "BirthCity", birthCity);
        Set(body, "DocumentType", documentType);
        Set(body, "DocumentNumber", documentNumber);
        Set(body, "DocumentExpiry", documentExpiry);
        Set(body, "DocumentCity", documentCity);
        Set(body, "Street", street);
        Set(body, "City", city);
        Set(body, "PostalCode", postalCode);
        Set(body, "Nation", nation);
        Set(body, "TaxCode", taxCode);
        Set(body, "VatNumber", vatNumber);
        Set(body, "Notes", notes);

        var saved = string.IsNullOrWhiteSpace(guestId)
            ? await hotel.PostAsync("api/clients", body, cancellationToken)
            : await hotel.PutAsync($"api/clients/{Uri.EscapeDataString(guestId)}", body, cancellationToken);
        if (saved is null)
            return new HotelReply(false, "The hotel did not return the guest.", null, "search_hotel by surname.");
        return new HotelReply(true, string.IsNullOrWhiteSpace(guestId) ? "Guest created." : "Guest updated.", new JsonObject { ["guest"] = JsonNode.Parse(saved.Value.GetRawText()) }, "attach_guest when this person shares a room.");
    }

    private async Task<HotelReply> AttachCore(string bookingId, string? guestId, string? lastName, string? firstName, int guestNumber, bool coveredByLead, string? cardState, CancellationToken cancellationToken)
    {
        var booking = await hotel.BookingAsync(bookingId, cancellationToken);
        if (booking is null)
            return new HotelReply(false, "That booking was not found.", null, "search_hotel first.");
        var id = guestId;
        if (string.IsNullOrWhiteSpace(id))
        {
            if (string.IsNullOrWhiteSpace(lastName))
                return new HotelReply(false, "A surname or a guest id is required.", null, "read_guest or pass the surname.");
            var created = await hotel.PostAsync("api/clients", new { LastName = lastName.Trim(), FirstName = string.IsNullOrWhiteSpace(firstName) ? null : firstName.Trim() }, cancellationToken);
            id = created is null ? null : HotelClient.JsonText(created.Value, "Id");
            if (id is null)
                return new HotelReply(false, "The guest could not be created.", null, "save_guest first.");
        }

        var year = HotelClient.JsonInt(booking.Value, "Year");
        var stayId = HotelClient.JsonText(booking.Value, "Id") ?? bookingId;
        var row = await hotel.PostAsync("api/booking-guests", new
        {
            Year = year,
            BookingId = stayId,
            ClientId = id,
            GuestNumber = guestNumber,
            CoveredByLead = coveredByLead,
            CardState = string.IsNullOrWhiteSpace(cardState) ? null : cardState.Trim()
        }, cancellationToken);
        return new HotelReply(true, "The person is attached to the stay.", row is null ? null : new JsonObject { ["bookingGuest"] = JsonNode.Parse(row.Value.GetRawText()) }, "read_registration for the day's gaps after the names are complete.");
    }

    private static void Set(JsonObject body, string name, string? value)
    {
        if (value is not null)
            body[name] = value;
    }

    private static string GuestLabel(System.Text.Json.JsonElement guest)
    {
        var name = string.Join(' ', new[] { HotelClient.JsonText(guest, "FirstName"), HotelClient.JsonText(guest, "LastName") }.Where(part => !string.IsNullOrWhiteSpace(part)));
        return string.IsNullOrWhiteSpace(name) ? "Guest" : name;
    }
}
