using System.ComponentModel;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using ModelContextProtocol.Server;

namespace HotelDruid.Mcp;

[McpServerToolType]
public sealed class DiscoveryTools(HotelClient hotel)
{
    [McpServerTool(Name = "search_hotel", Title = "Search the hotel", ReadOnly = true, Idempotent = true, Destructive = false, OpenWorld = false, UseStructuredContent = true)]
    [Description("Find a guest, a room, a booking, or an outside message. Use this before creating anything. Match a surname, a room name such as 101, a booking id, or an external reference. Returns at most 20 matches and the ids later tools need.")]
    public Task<HotelReply> Search(
        [Description("Surname, room name, booking id, or the other party's booking number.")] string query,
        [Description("Arrival year to scan for booking numbers. Omit to use the year inside the query, or the current year.")] int? year = null,
        CancellationToken cancellationToken = default)
        => HotelCall.Run(() => SearchCore(query, year, cancellationToken));

    [McpServerTool(Name = "read_desk", Title = "Read the desk", ReadOnly = true, Idempotent = true, Destructive = false, OpenWorld = false, UseStructuredContent = true)]
    [Description("Read one day of the desk: arrivals, who is in house, departures, outside requests still waiting, and registration gaps. Omit the date for today.")]
    public Task<HotelReply> ReadDesk(
        [Description("Business date yyyy-MM-dd. Omit for today.")] string? date = null,
        CancellationToken cancellationToken = default)
        => HotelCall.Run(() => DeskCore(date, cancellationToken));

    [McpServerTool(Name = "find_availability", Title = "Find a free room", ReadOnly = true, Idempotent = true, Destructive = false, OpenWorld = false, UseStructuredContent = true)]
    [Description("List rooms free between arrival and departure, and quote a named room when one is given. A room name such as 101 is enough. Maintenance and overlapping stays are not free. Departure is the morning the guest leaves, so that date is free for the next arrival.")]
    public Task<HotelReply> FindAvailability(
        [Description("Arrival yyyy-MM-dd.")] string arrival,
        [Description("Departure yyyy-MM-dd, the morning the guest leaves.")] string departure,
        [Description("People who will sleep in the room. Default 1.")] int guests = 1,
        [Description("Optional room name or room id to quote.")] string? room = null,
        CancellationToken cancellationToken = default)
        => HotelCall.Run(() => AvailabilityCore(arrival, departure, guests, room, cancellationToken));

    private async Task<HotelReply> SearchCore(string query, int? year, CancellationToken cancellationToken)
    {
        var text = query?.Trim() ?? "";
        if (text.Length < 2)
            return new HotelReply(false, "Give at least two characters.", null, "Ask for a surname, a room, or a booking number.");

        var matches = new JsonArray();
        var scanYear = year ?? GuessYear(text) ?? DateTime.UtcNow.Year;

        if (text.Length is >= 20 and <= 32 && text.All(char.IsLetterOrDigit))
        {
            var booking = await hotel.BookingAsync(text, cancellationToken);
            if (booking is not null)
            {
                var names = await BookingView.RoomNamesAsync(hotel, cancellationToken);
                var roomId = HotelClient.JsonText(booking.Value, "RoomId");
                var guest = await BookingView.GuestNameAsync(hotel, HotelClient.JsonText(booking.Value, "ClientId"), cancellationToken);
                matches.Add(new JsonObject { ["kind"] = "booking", ["item"] = BookingView.Brief(booking.Value, guest, roomId is not null && names.TryGetValue(roomId, out var roomName) ? roomName : roomId) });
            }
        }

        var lastName = text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Last();
        foreach (var guest in await hotel.ArrayAsync($"api/clients?lastName={Uri.EscapeDataString(lastName)}", cancellationToken))
        {
            var first = HotelClient.JsonText(guest, "FirstName");
            var last = HotelClient.JsonText(guest, "LastName");
            var label = string.Join(' ', new[] { first, last }.Where(part => !string.IsNullOrWhiteSpace(part)));
            if (!label.Contains(text, StringComparison.OrdinalIgnoreCase) && !string.Equals(last, lastName, StringComparison.OrdinalIgnoreCase))
                continue;
            matches.Add(new JsonObject
            {
                ["kind"] = "guest",
                ["id"] = HotelClient.JsonText(guest, "Id"),
                ["name"] = label,
                ["email"] = HotelClient.JsonText(guest, "Email"),
                ["phone"] = HotelClient.JsonText(guest, "Phone"),
                ["nationality"] = HotelClient.JsonText(guest, "Nationality")
            });
            if (matches.Count >= 20)
                break;
        }

        foreach (var room in await hotel.ArrayAsync("api/rooms", cancellationToken))
        {
            var name = HotelClient.JsonText(room, "Name") ?? "";
            if (!name.Contains(text, StringComparison.OrdinalIgnoreCase) && !string.Equals(HotelClient.JsonText(room, "Id"), text, StringComparison.OrdinalIgnoreCase))
                continue;
            matches.Add(new JsonObject
            {
                ["kind"] = "room",
                ["id"] = HotelClient.JsonText(room, "Id"),
                ["name"] = name,
                ["capacity"] = HotelClient.JsonInt(room, "Capacity"),
                ["condition"] = HotelClient.JsonText(room, "Condition")
            });
        }

        foreach (var booking in await hotel.ArrayAsync($"api/bookings?year={scanYear}", cancellationToken))
        {
            var external = HotelClient.JsonText(booking, "ExternalReference");
            var id = HotelClient.JsonText(booking, "Id");
            if (external is null || !external.Contains(text, StringComparison.OrdinalIgnoreCase))
                continue;
            if (matches.Any(item => item?["kind"]?.GetValue<string>() == "booking" && item["item"]?["id"]?.GetValue<string>() == id))
                continue;
            matches.Add(new JsonObject { ["kind"] = "booking", ["item"] = BookingView.Brief(booking, null, null), ["matched"] = "externalReference" });
            if (matches.Count >= 20)
                break;
        }

        var shown = BookingView.Take(matches, 20, out var total);
        return new HotelReply(true, total == 0 ? $"Nothing matched {text}." : $"Found {total} match(es) for {text}.", new JsonObject { ["total"] = total, ["matches"] = shown }, total == 0 ? "Try the surname only, or find_availability for a room." : "Use the id with read_booking, read_guest, or create_reservation.");
    }

    private async Task<HotelReply> DeskCore(string? date, CancellationToken cancellationToken)
    {
        var day = string.IsNullOrWhiteSpace(date) ? await hotel.TodayAsync(cancellationToken) : HotelClient.Date(date);
        if (day is null)
            return new HotelReply(false, "The date must be yyyy-MM-dd.", null, "Pass a date such as 2026-11-16, or omit it.");

        var names = await BookingView.RoomNamesAsync(hotel, cancellationToken);
        var stays = await hotel.ArrayAsync($"api/bookings?from={BookingView.Day(day.Value)}&to={BookingView.Day(day.Value)}", cancellationToken);
        async Task<JsonArray> Rows(Func<JsonElement, bool> predicate)
        {
            var rows = new JsonArray();
            foreach (var stay in stays.Where(predicate))
            {
                var roomId = HotelClient.JsonText(stay, "RoomId");
                var guest = await BookingView.GuestNameAsync(hotel, HotelClient.JsonText(stay, "ClientId"), cancellationToken);
                rows.Add(BookingView.Brief(stay, guest, roomId is not null && names.TryGetValue(roomId, out var name) ? name : roomId));
                if (rows.Count >= 30)
                    break;
            }
            return rows;
        }

        var arrivals = await Rows(stay => HotelClient.JsonText(stay, "ArrivalDate") == BookingView.Day(day.Value) && !HotelClient.Closed(HotelClient.JsonText(stay, "Status")));
        var departures = await Rows(stay => HotelClient.JsonText(stay, "DepartureDate") == BookingView.Day(day.Value) && !HotelClient.Closed(HotelClient.JsonText(stay, "Status")));
        var inHouse = await Rows(stay =>
        {
            var status = HotelClient.JsonText(stay, "Status");
            var arrival = HotelClient.Date(HotelClient.JsonText(stay, "ArrivalDate"));
            var departure = HotelClient.Date(HotelClient.JsonText(stay, "DepartureDate"));
            return !HotelClient.Closed(status) && arrival is not null && departure is not null && arrival.Value <= day && day < departure.Value;
        });

        var waiting = new JsonArray();
        foreach (var message in await hotel.ArrayAsync("api/dispatch?direction=in", cancellationToken))
        {
            var status = HotelClient.JsonText(message, "Status");
            if (status is "confirmed" or "relayed")
                continue;
            waiting.Add(new JsonObject
            {
                ["id"] = HotelClient.JsonText(message, "Id"),
                ["channel"] = HotelClient.JsonText(message, "Channel"),
                ["from"] = HotelClient.JsonText(message, "From"),
                ["subject"] = HotelClient.JsonText(message, "Subject"),
                ["status"] = status
            });
            if (waiting.Count >= 20)
                break;
        }

        var files = await hotel.GetAsync($"api/guest-files?date={BookingView.Day(day.Value)}", cancellationToken);
        return new HotelReply(true, $"{BookingView.Day(day.Value)}: {arrivals.Count} arriving, {inHouse.Count} in house, {departures.Count} leaving, {waiting.Count} outside request(s) waiting.", new JsonObject
        {
            ["date"] = BookingView.Day(day.Value),
            ["arrivals"] = arrivals,
            ["inHouse"] = inHouse,
            ["departures"] = departures,
            ["channelWaiting"] = waiting,
            ["registration"] = files is null ? null : JsonNode.Parse(files.Value.GetRawText())
        }, "Open one stay with read_booking, or confirm a waiting request with confirm_channel_request.");
    }

    private async Task<HotelReply> AvailabilityCore(string arrival, string departure, int guests, string? room, CancellationToken cancellationToken)
    {
        var start = HotelClient.Date(arrival);
        var end = HotelClient.Date(departure);
        if (start is null || end is null || end <= start)
            return new HotelReply(false, "Arrival and departure must be yyyy-MM-dd, and departure must be after arrival.", null, "Ask for both dates.");
        if (guests < 1)
            return new HotelReply(false, "Guest count must be at least 1.", null, "Pass the number of people sleeping in the room.");

        var names = await BookingView.RoomNamesAsync(hotel, cancellationToken);
        var taken = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var stay in await hotel.ArrayAsync($"api/bookings?from={BookingView.Day(start.Value)}&to={BookingView.Day(end.Value.AddDays(-1))}", cancellationToken))
        {
            if (!BookingView.Overlaps(stay, start.Value, end.Value))
                continue;
            var roomId = HotelClient.JsonText(stay, "RoomId");
            if (roomId is not null)
                taken.Add(roomId);
        }

        var free = new JsonArray();
        foreach (var item in await hotel.ArrayAsync("api/rooms", cancellationToken))
        {
            var id = HotelClient.JsonText(item, "Id");
            var condition = HotelClient.JsonText(item, "Condition");
            var capacity = HotelClient.JsonInt(item, "Capacity");
            if (id is null || capacity < guests)
                continue;
            if (taken.Contains(id) || (condition?.Contains("maintenance", StringComparison.OrdinalIgnoreCase) ?? false))
                continue;
            free.Add(new JsonObject
            {
                ["id"] = id,
                ["name"] = HotelClient.JsonText(item, "Name"),
                ["capacity"] = capacity,
                ["floor"] = HotelClient.JsonText(item, "FloorNumber")
            });
        }

        JsonNode? quote = null;
        string? quoteNote = null;
        if (!string.IsNullOrWhiteSpace(room))
        {
            var resolved = await hotel.RoomAsync(room, cancellationToken);
            if (resolved is null)
                quoteNote = $"Room {room} is not in the hotel.";
            else if (taken.Contains(resolved.Value.Id) || (resolved.Value.Condition?.Contains("maintenance", StringComparison.OrdinalIgnoreCase) ?? false))
                quoteNote = $"Room {resolved.Value.Name} is not free for those nights.";
            else
            {
                try
                {
                    var priced = await hotel.GetAsync($"api/tariffs/resolve?roomId={Uri.EscapeDataString(resolved.Value.Id)}&arrivalDate={BookingView.Day(start.Value)}&departureDate={BookingView.Day(end.Value)}&guestCount={guests}", cancellationToken);
                    quote = priced is null ? null : JsonNode.Parse(priced.Value.GetRawText());
                }
                catch (HotelCallException ex) when (ex.Status == 404)
                {
                    quoteNote = $"Room {resolved.Value.Name} is free. No tariff is assigned for these dates, so the stay would be stored without a quoted price.";
                }
            }
        }

        return new HotelReply(true, $"{free.Count} room(s) free from {BookingView.Day(start.Value)} to {BookingView.Day(end.Value)} for {guests}.", new JsonObject
        {
            ["arrival"] = BookingView.Day(start.Value),
            ["departure"] = BookingView.Day(end.Value),
            ["guests"] = guests,
            ["free"] = free,
            ["quote"] = quote,
            ["quoteNote"] = quoteNote
        }, "create_reservation uses the room name, the dates, and the guest.");
    }

    private static int? GuessYear(string text)
    {
        foreach (var token in text.Split([' ', '-', '/'], StringSplitOptions.RemoveEmptyEntries))
        {
            if (token.Length == 4 && int.TryParse(token, out var year) && year is >= 2000 and <= 2100)
                return year;
        }

        return null;
    }
}
