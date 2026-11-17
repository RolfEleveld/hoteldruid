using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Nodes;
using ModelContextProtocol.Server;

namespace HotelDruid.Mcp;

[McpServerToolType]
public sealed class ReservationTools(HotelClient hotel)
{
    [McpServerTool(Name = "read_booking", Title = "Read a booking", ReadOnly = true, Idempotent = true, Destructive = false, OpenWorld = false, UseStructuredContent = true)]
    [Description("Read one stay: dates, room, guest, source, external reference, price, invoice number, attached guests, and the money still open. Use the booking id from search_hotel or read_desk.")]
    public Task<HotelReply> Read(
        [Description("Booking id.")] string bookingId,
        CancellationToken cancellationToken = default)
        => HotelCall.Run(() => ReadCore(bookingId, cancellationToken));

    [McpServerTool(Name = "create_reservation", Title = "Create a reservation", ReadOnly = false, Idempotent = false, Destructive = false, OpenWorld = false, UseStructuredContent = true)]
    [Description("Create a reservation on a free room. Pass a room name such as 101. Source is who sent it (desk, phone, email, homepage, Booking.com, or another hotel). External reference is their booking number. Reuses one matching guest; if several people share the surname, it stops and lists them.")]
    public Task<HotelReply> Create(
        [Description("Guest surname. Required when guestId is omitted.")] string? lastName = null,
        [Description("Guest given name.")] string? firstName = null,
        [Description("Existing guest id. Omit to find or create the guest.")] string? guestId = null,
        [Description("Room name or room id.")] string? room = null,
        [Description("Arrival yyyy-MM-dd.")] string? arrival = null,
        [Description("Departure yyyy-MM-dd.")] string? departure = null,
        [Description("People sleeping in the room. Default 1.")] int guests = 1,
        [Description("Who sent the booking.")] string? source = null,
        [Description("The other party's booking number.")] string? externalReference = null,
        [Description("Email, used when a new guest is created.")] string? email = null,
        [Description("Phone, used when a new guest is created.")] string? phone = null,
        [Description("Note stored on the stay.")] string? notes = null,
        CancellationToken cancellationToken = default)
        => HotelCall.Run(() => CreateCore(lastName, firstName, guestId, room, arrival, departure, guests, source, externalReference, email, phone, notes, cancellationToken));

    [McpServerTool(Name = "change_reservation", Title = "Change a reservation", ReadOnly = false, Idempotent = true, Destructive = false, OpenWorld = false, UseStructuredContent = true)]
    [Description("Change dates, guest count, notes, source, or external reference on a stay that is not yet checked in. Omitted fields stay as they are. A room change for a guest already in house must use move_room.")]
    public Task<HotelReply> Change(
        [Description("Booking id.")] string bookingId,
        [Description("New room name or id.")] string? room = null,
        [Description("New arrival yyyy-MM-dd.")] string? arrival = null,
        [Description("New departure yyyy-MM-dd.")] string? departure = null,
        [Description("New expected guest count.")] int? guests = null,
        [Description("Replacement note. Omit to keep the current note.")] string? notes = null,
        [Description("Who sent the booking.")] string? source = null,
        [Description("The other party's booking number.")] string? externalReference = null,
        CancellationToken cancellationToken = default)
        => HotelCall.Run(() => ChangeCore(bookingId, room, arrival, departure, guests, notes, source, externalReference, cancellationToken));

    [McpServerTool(Name = "check_in", Title = "Check a guest in", ReadOnly = false, Idempotent = true, Destructive = false, OpenWorld = false, UseStructuredContent = true)]
    [Description("Mark the guest as in house. The arrival date must be before the booked departure. If more people arrive than were expected, the hotel may re-quote the stay.")]
    public Task<HotelReply> CheckIn(
        [Description("Booking id.")] string bookingId,
        [Description("Actual arrival yyyy-MM-dd.")] string arrival,
        [Description("People who actually arrived.")] int guestCount,
        CancellationToken cancellationToken = default)
        => HotelCall.Run(() => Act(bookingId, $"api/bookings/{{year}}/{{id}}/check-in", new { ActualArrivalDate = arrival, ActualGuestCount = guestCount }, cancellationToken));

    [McpServerTool(Name = "check_out", Title = "Check a guest out", ReadOnly = false, Idempotent = true, Destructive = false, OpenWorld = false, UseStructuredContent = true)]
    [Description("Mark the guest as departed. The guest must already be checked in, and the departure cannot be after the booked departure.")]
    public Task<HotelReply> CheckOut(
        [Description("Booking id.")] string bookingId,
        [Description("Actual departure yyyy-MM-dd.")] string departure,
        CancellationToken cancellationToken = default)
        => HotelCall.Run(() => Act(bookingId, "api/bookings/{year}/{id}/check-out", new { ActualDepartureDate = departure }, cancellationToken));

    [McpServerTool(Name = "move_room", Title = "Move a guest to another room", ReadOnly = false, Idempotent = false, Destructive = false, OpenWorld = false, UseStructuredContent = true)]
    [Description("Split a stay on the move date. Nights before that date stay in the current room. Later nights continue in the new room, and the price is split. The move date must leave at least one night in each room. Pass a room name such as 102.")]
    public Task<HotelReply> Move(
        [Description("Booking id of the stay the guest is in now.")] string bookingId,
        [Description("First night in the new room, yyyy-MM-dd.")] string fromDate,
        [Description("New room name or id.")] string room,
        [Description("Why the guest is moving.")] string? reason = null,
        CancellationToken cancellationToken = default)
        => HotelCall.Run(() => MoveCore(bookingId, fromDate, room, reason, cancellationToken));

    [McpServerTool(Name = "cancel_reservation", Title = "Cancel a reservation", ReadOnly = false, Idempotent = true, Destructive = true, OpenWorld = false, UseStructuredContent = true)]
    [Description("Cancel a stay that should not be kept. Use this only when the person asked to cancel that booking.")]
    public Task<HotelReply> Cancel(
        [Description("Booking id.")] string bookingId,
        CancellationToken cancellationToken = default)
        => HotelCall.Run(() => Act(bookingId, "api/bookings/{year}/{id}/cancel", new { }, cancellationToken));

    [McpServerTool(Name = "mark_no_show", Title = "Mark a no-show", ReadOnly = false, Idempotent = true, Destructive = true, OpenWorld = false, UseStructuredContent = true)]
    [Description("Mark a reservation as a no-show. A note is required. A guest who has already checked in cannot be a no-show.")]
    public Task<HotelReply> NoShow(
        [Description("Booking id.")] string bookingId,
        [Description("Why this is a no-show.")] string notes,
        CancellationToken cancellationToken = default)
        => HotelCall.Run(() => Act(bookingId, "api/bookings/{year}/{id}/no-show", new { Notes = notes }, cancellationToken));

    private async Task<HotelReply> ReadCore(string bookingId, CancellationToken cancellationToken)
    {
        var booking = await hotel.BookingAsync(bookingId, cancellationToken);
        if (booking is null)
            return new HotelReply(false, "That booking was not found.", null, "search_hotel for the surname or the external reference.");
        var year = HotelClient.JsonInt(booking.Value, "Year");
        var id = HotelClient.JsonText(booking.Value, "Id") ?? bookingId;
        var names = await BookingView.RoomNamesAsync(hotel, cancellationToken);
        var roomId = HotelClient.JsonText(booking.Value, "RoomId");
        var guest = await BookingView.GuestNameAsync(hotel, HotelClient.JsonText(booking.Value, "ClientId"), cancellationToken);
        var finance = await hotel.GetAsync($"api/bookings/{year}/{Uri.EscapeDataString(id)}/finance", cancellationToken);
        var attached = new JsonArray();
        foreach (var row in await hotel.ArrayAsync($"api/booking-guests?year={year}", cancellationToken))
        {
            if (!string.Equals(HotelClient.JsonText(row, "BookingId"), id, StringComparison.OrdinalIgnoreCase))
                continue;
            var otherId = HotelClient.JsonText(row, "ClientId");
            attached.Add(new JsonObject
            {
                ["id"] = HotelClient.JsonText(row, "Id"),
                ["guestId"] = otherId,
                ["name"] = await BookingView.GuestNameAsync(hotel, otherId, cancellationToken),
                ["guestNumber"] = HotelClient.JsonInt(row, "GuestNumber"),
                ["coveredByLead"] = HotelClient.JsonBool(row, "CoveredByLead"),
                ["cardState"] = HotelClient.JsonText(row, "CardState")
            });
        }

        return new HotelReply(true, $"{guest ?? "Guest"} in {RoomLabel(names, roomId)}, {HotelClient.JsonText(booking.Value, "Status")}.", new JsonObject
        {
            ["booking"] = BookingView.Brief(booking.Value, guest, RoomLabel(names, roomId)),
            ["account"] = finance is null ? null : JsonNode.Parse(finance.Value.GetRawText()),
            ["attachedGuests"] = attached
        }, "take_payment, issue_document, check_in, or attach_guest.");
    }

    private async Task<HotelReply> CreateCore(string? lastName, string? firstName, string? guestId, string? room, string? arrival, string? departure, int guests, string? source, string? externalReference, string? email, string? phone, string? notes, CancellationToken cancellationToken)
    {
        var start = HotelClient.Date(arrival);
        var end = HotelClient.Date(departure);
        if (start is null || end is null || end <= start)
            return new HotelReply(false, "Arrival and departure must be yyyy-MM-dd, and departure must be after arrival.", null, "Ask for both dates.");
        if (string.IsNullOrWhiteSpace(room))
            return new HotelReply(false, "A room is required.", null, "find_availability, then pass the room name.");
        var resolved = await hotel.RoomAsync(room, cancellationToken);
        if (resolved is null)
            return new HotelReply(false, $"Room {room} is not in the hotel.", null, "find_availability lists the free rooms.");
        if (resolved.Value.Capacity > 0 && resolved.Value.Capacity < guests)
            return new HotelReply(false, $"Room {resolved.Value.Name} sleeps {resolved.Value.Capacity}, not {guests}.", null, "find_availability for a larger room.");

        var free = await new DiscoveryTools(hotel).FindAvailability(BookingView.Day(start.Value), BookingView.Day(end.Value), guests, resolved.Value.Name, cancellationToken);
        if (free.Data?["quoteNote"]?.GetValue<string>() is string note && note.Contains("not free", StringComparison.OrdinalIgnoreCase))
            return new HotelReply(false, note, free.Data, "Choose another room from find_availability.");

        var (id, createdGuest) = await ResolveGuest(guestId, lastName, firstName, email, phone, cancellationToken);
        if (id is null)
            return createdGuest;

        var created = await hotel.PostAsync("api/bookings", new
        {
            Year = start.Value.Year,
            ClientId = id,
            RoomId = resolved.Value.Id,
            ArrivalDate = BookingView.Day(start.Value),
            DepartureDate = BookingView.Day(end.Value),
            Status = "Confirmed",
            ExpectedGuestCount = guests,
            Source = Blank(source),
            ExternalReference = Blank(externalReference),
            Notes = Blank(notes)
        }, cancellationToken);
        if (created is null)
            return new HotelReply(false, "The hotel did not return the new booking.", null, "search_hotel for the guest.");

        var guestName = await BookingView.GuestNameAsync(hotel, id, cancellationToken);
        return new HotelReply(true, $"Reserved {resolved.Value.Name} for {guestName} from {BookingView.Day(start.Value)} to {BookingView.Day(end.Value)}.", new JsonObject
        {
            ["booking"] = BookingView.Brief(created.Value, guestName, resolved.Value.Name),
            ["guestCreated"] = createdGuest.Ok
        }, "read_booking for the price and the amount still due.");
    }

    private async Task<HotelReply> ChangeCore(string bookingId, string? room, string? arrival, string? departure, int? guests, string? notes, string? source, string? externalReference, CancellationToken cancellationToken)
    {
        var current = await hotel.BookingAsync(bookingId, cancellationToken);
        if (current is null)
            return new HotelReply(false, "That booking was not found.", null, "search_hotel first.");
        var status = HotelClient.JsonText(current.Value, "Status");
        if (string.Equals(status, "CheckedIn", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(room))
            return new HotelReply(false, "This guest is already in house. A room change from here would rewrite the whole stay.", null, "Use move_room with the first night in the new room.");

        var body = JsonNode.Parse(current.Value.GetRawText())!.AsObject();
        if (!string.IsNullOrWhiteSpace(room))
        {
            var resolved = await hotel.RoomAsync(room, cancellationToken);
            if (resolved is null)
                return new HotelReply(false, $"Room {room} is not in the hotel.", null, "find_availability.");
            body["RoomId"] = resolved.Value.Id;
        }
        if (HotelClient.Date(arrival) is DateOnly start)
            body["ArrivalDate"] = BookingView.Day(start);
        if (HotelClient.Date(departure) is DateOnly end)
            body["DepartureDate"] = BookingView.Day(end);
        if (guests is > 0)
            body["ExpectedGuestCount"] = guests;
        if (notes is not null)
            body["Notes"] = notes;
        if (source is not null)
            body["Source"] = Blank(source);
        if (externalReference is not null)
            body["ExternalReference"] = Blank(externalReference);
        if (body["ActualArrivalDate"] is null)
        {
            body.Remove("ActualGuestCount");
            body.Remove("ActualDepartureDate");
        }

        var year = HotelClient.JsonInt(current.Value, "Year");
        var id = HotelClient.JsonText(current.Value, "Id") ?? bookingId;
        var updated = await hotel.PutAsync($"api/bookings/{year}/{Uri.EscapeDataString(id)}", body, cancellationToken);
        return new HotelReply(true, "The reservation was updated.", updated is null ? null : new JsonObject { ["booking"] = BookingView.Brief(updated.Value, null, null) }, "read_booking to see the price after the change.");
    }

    private async Task<HotelReply> MoveCore(string bookingId, string fromDate, string room, string? reason, CancellationToken cancellationToken)
    {
        if (HotelClient.Date(fromDate) is null)
            return new HotelReply(false, "The move date must be yyyy-MM-dd.", null, "Pass the first night in the new room.");
        var resolved = await hotel.RoomAsync(room, cancellationToken);
        if (resolved is null)
            return new HotelReply(false, $"Room {room} is not in the hotel.", null, "find_availability for the nights after the move.");
        return await Act(bookingId, "api/bookings/{year}/{id}/move", new { FromDate = fromDate, RoomId = resolved.Value.Id, Reason = Blank(reason) }, cancellationToken);
    }

    private async Task<HotelReply> Act(string bookingId, string pathTemplate, object body, CancellationToken cancellationToken)
    {
        var booking = await hotel.BookingAsync(bookingId, cancellationToken);
        if (booking is null)
            return new HotelReply(false, "That booking was not found.", null, "search_hotel first.");
        var year = HotelClient.JsonInt(booking.Value, "Year");
        var id = HotelClient.JsonText(booking.Value, "Id") ?? bookingId;
        var path = pathTemplate.Replace("{year}", year.ToString(), StringComparison.Ordinal).Replace("{id}", Uri.EscapeDataString(id), StringComparison.Ordinal);
        var updated = await hotel.PostAsync(path, body, cancellationToken);
        var names = await BookingView.RoomNamesAsync(hotel, cancellationToken);
        var roomId = updated is null ? null : HotelClient.JsonText(updated.Value, "RoomId");
        return new HotelReply(true, "The stay was updated.", updated is null ? null : new JsonObject { ["booking"] = BookingView.Brief(updated.Value, null, roomId is not null && names.TryGetValue(roomId, out var name) ? name : roomId) }, "read_booking for the resulting stay.");
    }

    private async Task<(string? Id, HotelReply Reply)> ResolveGuest(string? guestId, string? lastName, string? firstName, string? email, string? phone, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(guestId))
            return (guestId.Trim(), new HotelReply(false, "Used the guest id that was passed.", null, null));

        if (string.IsNullOrWhiteSpace(lastName))
            return (null, new HotelReply(false, "A surname or an existing guest id is required.", null, "Ask for the surname."));

        var found = new JsonArray();
        foreach (var guest in await hotel.ArrayAsync($"api/clients?lastName={Uri.EscapeDataString(lastName.Trim())}", cancellationToken))
        {
            var first = HotelClient.JsonText(guest, "FirstName");
            if (!string.IsNullOrWhiteSpace(firstName) && !string.Equals(first, firstName.Trim(), StringComparison.OrdinalIgnoreCase))
                continue;
            found.Add(new JsonObject
            {
                ["id"] = HotelClient.JsonText(guest, "Id"),
                ["name"] = string.Join(' ', new[] { first, HotelClient.JsonText(guest, "LastName") }.Where(part => !string.IsNullOrWhiteSpace(part)))
            });
        }

        if (found.Count > 1)
            return (null, new HotelReply(false, $"Several guests are named {firstName} {lastName}.", new JsonObject { ["guests"] = found }, "Pass guestId for the right person, or a more specific given name."));
        if (found.Count == 1)
            return (found[0]!["id"]!.GetValue<string>(), new HotelReply(false, "Used the guest already on file.", null, null));

        var created = await hotel.PostAsync("api/clients", new { LastName = lastName.Trim(), FirstName = Blank(firstName), Email = Blank(email), Phone = Blank(phone) }, cancellationToken);
        var id = created is null ? null : HotelClient.JsonText(created.Value, "Id");
        return id is null
            ? (null, new HotelReply(false, "The guest could not be created.", null, "save_guest, then pass guestId."))
            : (id, new HotelReply(true, "Created a new guest.", null, null));
    }

    private static string? Blank(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string? RoomLabel(Dictionary<string, string> names, string? roomId)
        => roomId is not null && names.TryGetValue(roomId, out var name) ? name : roomId;
}
