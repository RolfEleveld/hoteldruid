using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace HotelDruid.Mcp;

internal static class BookingView
{
    public static JsonObject Brief(JsonElement booking, string? guest, string? roomName)
    {
        var arrival = HotelClient.JsonText(booking, "ArrivalDate");
        var room = roomName ?? HotelClient.JsonText(booking, "RoomId");
        return new JsonObject
        {
            ["id"] = HotelClient.JsonText(booking, "Id"),
            ["year"] = HotelClient.JsonInt(booking, "Year"),
            ["reference"] = string.IsNullOrWhiteSpace(arrival) ? room : $"{arrival} · {room}",
            ["room"] = room,
            ["roomId"] = HotelClient.JsonText(booking, "RoomId"),
            ["guest"] = guest,
            ["guestId"] = HotelClient.JsonText(booking, "ClientId"),
            ["arrival"] = arrival,
            ["departure"] = HotelClient.JsonText(booking, "DepartureDate"),
            ["status"] = HotelClient.JsonText(booking, "Status"),
            ["source"] = HotelClient.JsonText(booking, "Source"),
            ["externalReference"] = HotelClient.JsonText(booking, "ExternalReference"),
            ["expectedGuests"] = HotelClient.JsonInt(booking, "ExpectedGuestCount"),
            ["price"] = HotelClient.JsonRaw(booking, "TariffTotalPrice"),
            ["issuedInvoiceNumber"] = HotelClient.JsonText(booking, "IssuedInvoiceNumber")
        };
    }

    public static async Task<string?> GuestNameAsync(HotelClient hotel, string? guestId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(guestId))
            return null;
        try
        {
            var guest = await hotel.GetAsync($"api/clients/{Uri.EscapeDataString(guestId)}", cancellationToken);
            if (guest is null)
                return null;
            var first = HotelClient.JsonText(guest.Value, "FirstName");
            var last = HotelClient.JsonText(guest.Value, "LastName");
            return string.Join(' ', new[] { first, last }.Where(part => !string.IsNullOrWhiteSpace(part)));
        }
        catch (HotelCallException)
        {
            return null;
        }
    }

    public static async Task<Dictionary<string, string>> RoomNamesAsync(HotelClient hotel, CancellationToken cancellationToken)
    {
        var names = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var room in await hotel.ArrayAsync("api/rooms", cancellationToken))
        {
            var id = HotelClient.JsonText(room, "Id");
            if (id is not null)
                names[id] = HotelClient.JsonText(room, "Name") ?? id;
        }

        return names;
    }

    public static bool Overlaps(JsonElement booking, DateOnly arrival, DateOnly departure)
    {
        if (HotelClient.Closed(HotelClient.JsonText(booking, "Status")))
            return false;
        var start = HotelClient.Date(HotelClient.JsonText(booking, "ArrivalDate"));
        var end = HotelClient.Date(HotelClient.JsonText(booking, "DepartureDate"));
        return start is not null && end is not null && start.Value < departure && arrival < end.Value;
    }

    public static JsonArray Take(IEnumerable<JsonNode?> items, int limit, out int total)
    {
        var list = items.Where(item => item is not null).Cast<JsonNode>().ToList();
        total = list.Count;
        var array = new JsonArray();
        foreach (var item in list.Take(limit))
            array.Add(item.DeepClone());
        return array;
    }

    public static string Money(JsonElement element, string name)
        => element.TryGetProperty(name, out var value) ? value.ToString() : "0";

    public static string Day(DateOnly date) => date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
}
