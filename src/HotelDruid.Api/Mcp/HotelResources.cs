using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;

namespace HotelDruid.Mcp;

[McpServerResourceType]
public sealed class HotelResources(HotelClient hotel)
{
    public const string Guide = """
        HotelDruid is the system of record. This server is the desk.

        Search before you create. A room is spoken by its name, such as 101. Pass that name. The year of a booking is the arrival year.
        Source is who sent the stay: desk, phone, email, homepage, Booking.com, or another hotel. External reference is their booking number.
        An outside request is submit_channel_request, then confirm_channel_request. Confirming writes the message id onto the stay as the external reference.
        The hotel queues email, WhatsApp, SMS, and post. It does not send them. issue_document puts an invoice or confirmation on that queue. The invoice number is one series per year.
        take_payment records guest money. record_expense is a purchase. record_drawer is petty cash that is not a guest payment.
        save_guest stores the fields the clerk copied from an identity document. The hotel does not store a scan.
        sign_night means the desk has seen that business date. It does not mean the guests have paid. Do not sign unless the person named the date and asked to close it.
        """;

    [McpServerResource(UriTemplate = "hotel://guide", Name = "guide", Title = "How to run the hotel", MimeType = "text/plain")]
    [Description("The operating rules for this hotel server.")]
    public static string OperatingGuide() => Guide;

    [McpServerResource(UriTemplate = "hotel://desk/today", Name = "today", Title = "Today on the desk", MimeType = "application/json")]
    [Description("Arrivals, in-house guests, departures, waiting outside requests, and registration gaps for today.")]
    public async Task<string> Today(CancellationToken cancellationToken)
    {
        var reply = await new DiscoveryTools(hotel).ReadDesk(null, cancellationToken);
        return JsonSerializer.Serialize(reply);
    }

    [McpServerResource(UriTemplate = "hotel://rooms", Name = "rooms", Title = "Rooms", MimeType = "application/json")]
    [Description("Every room: name, id, capacity, floor, and condition.")]
    public async Task<string> Rooms(CancellationToken cancellationToken)
    {
        var rooms = await hotel.ArrayAsync("api/rooms", cancellationToken);
        return JsonSerializer.Serialize(rooms.Select(room => new
        {
            id = HotelClient.JsonText(room, "Id"),
            name = HotelClient.JsonText(room, "Name"),
            capacity = HotelClient.JsonInt(room, "Capacity"),
            floor = HotelClient.JsonText(room, "FloorNumber"),
            condition = HotelClient.JsonText(room, "Condition")
        }));
    }

    [McpServerResource(UriTemplate = "hotel://accounts", Name = "accounts", Title = "Chart of accounts", MimeType = "application/json")]
    [Description("The hotel chart: code, name, and the role each account plays.")]
    public async Task<string> Accounts(CancellationToken cancellationToken)
    {
        var accounts = await hotel.ArrayAsync("api/ledger/accounts", cancellationToken);
        return JsonSerializer.Serialize(accounts.Select(account => new
        {
            code = HotelClient.JsonText(account, "Code"),
            name = HotelClient.JsonText(account, "Name"),
            role = HotelClient.JsonText(account, "Role")
        }));
    }

    [McpServerResource(UriTemplate = "hotel://booking/{id}", Name = "booking", Title = "One booking", MimeType = "application/json")]
    [Description("One stay, its account, and the people attached to it.")]
    public async Task<string> Booking(string id, CancellationToken cancellationToken)
    {
        var reply = await new ReservationTools(hotel).Read(id, cancellationToken);
        return JsonSerializer.Serialize(reply);
    }

    [McpServerResource(UriTemplate = "hotel://guest/{id}", Name = "guest", Title = "One guest", MimeType = "application/json")]
    [Description("One guest record.")]
    public async Task<string> Guest(string id, CancellationToken cancellationToken)
    {
        var reply = await new GuestTools(hotel).Read(id, cancellationToken);
        return JsonSerializer.Serialize(reply);
    }
}
