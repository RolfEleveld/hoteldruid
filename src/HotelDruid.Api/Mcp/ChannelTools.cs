using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Nodes;
using ModelContextProtocol.Server;

namespace HotelDruid.Mcp;

[McpServerToolType]
public sealed class ChannelTools(HotelClient hotel)
{
    [McpServerTool(Name = "submit_channel_request", Title = "Submit an outside booking request", ReadOnly = false, Idempotent = false, Destructive = false, OpenWorld = false, UseStructuredContent = true)]
    [Description("Place an OTA or partner-hotel request on the inbox. This does not create the stay. The body stored for the clerk is the name, dates, room, and guest count. Confirm it with confirm_channel_request. The hotel stores no channel password.")]
    public Task<HotelReply> Submit(
        [Description("Surname on the outside booking.")] string lastName,
        [Description("Arrival yyyy-MM-dd.")] string arrival,
        [Description("Departure yyyy-MM-dd.")] string departure,
        [Description("Room name or room id they asked for.")] string room,
        [Description("Given name.")] string? firstName = null,
        [Description("Guest email. The confirmation is addressed here.")] string? email = null,
        [Description("People on the request. Default 1.")] int guests = 1,
        [Description("Channel: email, booking.com, or the partner hotel's name. Default email.")] string? channel = null,
        [Description("Who sent it. Default is the channel.")] string? from = null,
        [Description("Their booking number. Stored on the message. confirm writes the message id onto the stay as the external reference.")] string? externalReference = null,
        CancellationToken cancellationToken = default)
        => HotelCall.Run(() => SubmitCore(lastName, arrival, departure, room, firstName, email, guests, channel, from, externalReference, cancellationToken));

    [McpServerTool(Name = "list_messages", Title = "List messages", ReadOnly = true, Idempotent = true, Destructive = false, OpenWorld = false, UseStructuredContent = true)]
    [Description("List the batch inbox and outbox. Direction is in or out. Status for an inbox request is often review or confirmed. Status for an outgoing row is ready, then relayed. The hotel does not send these.")]
    public Task<HotelReply> List(
        [Description("in or out. Omit for both.")] string? direction = null,
        [Description("Status filter, such as ready or confirmed.")] string? status = null,
        [Description("How many rows to return. Default 20.")] int limit = 20,
        CancellationToken cancellationToken = default)
        => HotelCall.Run(() => ListCore(direction, status, limit, cancellationToken));

    [McpServerTool(Name = "confirm_channel_request", Title = "Confirm an outside booking", ReadOnly = false, Idempotent = true, Destructive = false, OpenWorld = false, UseStructuredContent = true)]
    [Description("Accept one inbox request into an ordinary booking. The request must already carry a surname, dates, and a room. Source becomes dispatch. External reference becomes this message id. A confirmation is queued on the outbox. The hotel still does not send it.")]
    public Task<HotelReply> Confirm(
        [Description("Inbox message id from submit_channel_request or list_messages.")] string messageId,
        CancellationToken cancellationToken = default)
        => HotelCall.Run(async () =>
        {
            var message = await hotel.PostAsync($"api/dispatch/{Uri.EscapeDataString(messageId)}/accept", new { }, cancellationToken);
            if (message is null)
                return new HotelReply(false, "The request was not found.", null, "list_messages direction in.");
            var bookingId = HotelClient.JsonText(message.Value, "BookingId");
            return new HotelReply(true, bookingId is null ? "The request was not turned into a booking." : $"Confirmed as booking {bookingId}. A confirmation is waiting on the outbox.", new JsonObject { ["message"] = Project(message.Value) }, "read_booking for the new stay. mark_message_relayed only after a tool has actually sent the confirmation.");
        });

    [McpServerTool(Name = "process_channel_inbox", Title = "Process structured inbox messages", ReadOnly = false, Idempotent = true, Destructive = false, OpenWorld = false, UseStructuredContent = true)]
    [Description("Run the hotel's structured-inbox pass. Messages become bookings only when an administrator has turned automation on. Otherwise they stay for confirm_channel_request. This tool does not turn automation on.")]
    public Task<HotelReply> Process(CancellationToken cancellationToken = default)
        => HotelCall.Run(async () =>
        {
            var result = await hotel.PostAsync("api/dispatch/process", new { }, cancellationToken);
            return new HotelReply(true, "The inbox pass finished.", result is null ? null : JsonNode.Parse(result.Value.GetRawText()), "list_messages direction in to see what is still waiting.");
        });

    [McpServerTool(Name = "queue_message", Title = "Queue a message", ReadOnly = false, Idempotent = false, Destructive = false, OpenWorld = false, UseStructuredContent = true)]
    [Description("Put an email, WhatsApp, SMS, or post row on the outbox. Email is inline: do not attach the invoice. The hotel stores the row and does not send it. Use issue_document when the row should be a rendered invoice or confirmation.")]
    public Task<HotelReply> Queue(
        [Description("Who receives it.")] string to,
        [Description("Subject line.")] string subject,
        [Description("Inline body.")] string body,
        [Description("email, whatsapp, sms, or post. Default email.")] string? channel = null,
        [Description("Sender address or desk name.")] string? from = null,
        [Description("Booking id this message belongs to.")] string? bookingId = null,
        [Description("confirmation, invoice, or a short kind.")] string? kind = null,
        CancellationToken cancellationToken = default)
        => HotelCall.Run(() => QueueCore(to, subject, body, channel, from, bookingId, kind, cancellationToken));

    [McpServerTool(Name = "mark_message_relayed", Title = "Mark a message relayed", ReadOnly = false, Idempotent = true, Destructive = false, OpenWorld = false, UseStructuredContent = true)]
    [Description("Write the moment an integration tool actually handed the message to the channel. Call this only after the send has happened. The hotel itself does not send.")]
    public Task<HotelReply> Relay(
        [Description("Message id.")] string messageId,
        [Description("Relay time in ISO-8601. Omit for now.")] string? relayedAt = null,
        CancellationToken cancellationToken = default)
        => HotelCall.Run(async () =>
        {
            DateTime? when = null;
            if (!string.IsNullOrWhiteSpace(relayedAt))
            {
                if (!DateTime.TryParse(relayedAt, out var parsed))
                    return new HotelReply(false, "relayedAt must be a date and time.", null, "Omit it to use the current time.");
                when = parsed.ToUniversalTime();
            }

            var updated = await hotel.PostAsync($"api/dispatch/{Uri.EscapeDataString(messageId)}/relayed", new { RelayedAt = when }, cancellationToken);
            return new HotelReply(true, "The message is marked relayed.", updated is null ? null : new JsonObject { ["message"] = Project(updated.Value) }, "list_messages status ready for anything still waiting.");
        });

    private async Task<HotelReply> SubmitCore(string lastName, string arrival, string departure, string room, string? firstName, string? email, int guests, string? channel, string? from, string? externalReference, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(lastName))
            return new HotelReply(false, "A surname is required.", null, "Ask for the guest surname on the outside booking.");
        if (HotelClient.Date(arrival) is null || HotelClient.Date(departure) is null)
            return new HotelReply(false, "Arrival and departure must be yyyy-MM-dd.", null, "Ask for both dates.");
        var resolved = await hotel.RoomAsync(room, cancellationToken);
        if (resolved is null)
            return new HotelReply(false, $"Room {room} is not in the hotel.", null, "find_availability, then submit again with a room that exists.");

        var payload = new JsonObject
        {
            ["lastName"] = lastName.Trim(),
            ["firstName"] = string.IsNullOrWhiteSpace(firstName) ? null : firstName.Trim(),
            ["email"] = string.IsNullOrWhiteSpace(email) ? null : email.Trim(),
            ["arrival"] = arrival,
            ["departure"] = departure,
            ["roomId"] = resolved.Value.Id,
            ["guests"] = guests < 1 ? 1 : guests
        };
        var name = string.IsNullOrWhiteSpace(channel) ? "email" : channel.Trim();
        var created = await hotel.PostAsync("api/dispatch", new
        {
            Direction = "in",
            Channel = name,
            From = string.IsNullOrWhiteSpace(from) ? name : from.Trim(),
            To = "desk",
            Subject = string.IsNullOrWhiteSpace(externalReference) ? $"Request for {resolved.Value.Name}" : $"Request {externalReference.Trim()}",
            Body = payload.ToJsonString(),
            Kind = "booking-request",
            Status = "review"
        }, cancellationToken);
        if (created is null)
            return new HotelReply(false, "The request was not stored.", null, "list_messages direction in.");
        return new HotelReply(true, $"Request stored for {lastName.Trim()}, room {resolved.Value.Name}. It is not a booking until it is confirmed.", new JsonObject
        {
            ["message"] = Project(created.Value),
            ["room"] = resolved.Value.Name,
            ["externalReferenceOnSubject"] = string.IsNullOrWhiteSpace(externalReference) ? null : externalReference.Trim()
        }, "confirm_channel_request with this message id. After confirm, write their number with change_reservation if it is not already the message id.");
    }

    private async Task<HotelReply> ListCore(string? direction, string? status, int limit, CancellationToken cancellationToken)
    {
        var query = new List<string>();
        if (!string.IsNullOrWhiteSpace(direction))
            query.Add("direction=" + Uri.EscapeDataString(direction.Trim()));
        if (!string.IsNullOrWhiteSpace(status))
            query.Add("status=" + Uri.EscapeDataString(status.Trim()));
        var path = "api/dispatch" + (query.Count == 0 ? "" : "?" + string.Join('&', query));
        var rows = new JsonArray();
        var count = 0;
        foreach (var message in await hotel.ArrayAsync(path, cancellationToken))
        {
            count++;
            if (rows.Count >= Math.Clamp(limit, 1, 50))
                continue;
            rows.Add(Project(message));
        }

        return new HotelReply(true, $"{count} message(s).", new JsonObject { ["total"] = count, ["messages"] = rows }, "confirm_channel_request for an inbox request. mark_message_relayed after a real send.");
    }

    private async Task<HotelReply> QueueCore(string to, string subject, string body, string? channel, string? from, string? bookingId, string? kind, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(to) || string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(body))
            return new HotelReply(false, "To, subject, and body are required.", null, "Write the message inline.");
        int? year = null;
        if (!string.IsNullOrWhiteSpace(bookingId))
        {
            var booking = await hotel.BookingAsync(bookingId, cancellationToken);
            if (booking is null)
                return new HotelReply(false, "That booking was not found.", null, "search_hotel, or omit bookingId.");
            year = HotelClient.JsonInt(booking.Value, "Year");
            bookingId = HotelClient.JsonText(booking.Value, "Id");
        }

        var name = string.IsNullOrWhiteSpace(channel) ? "email" : channel.Trim();
        var created = await hotel.PostAsync("api/dispatch", new
        {
            Direction = "out",
            Channel = name,
            To = to.Trim(),
            From = string.IsNullOrWhiteSpace(from) ? "desk" : from.Trim(),
            Subject = subject.Trim(),
            Body = body,
            BookingId = bookingId,
            Year = year,
            Kind = string.IsNullOrWhiteSpace(kind) ? null : kind.Trim(),
            Status = "ready"
        }, cancellationToken);
        return new HotelReply(true, "The message is ready for a relay tool. The hotel did not send it.", created is null ? null : new JsonObject { ["message"] = Project(created.Value) }, "mark_message_relayed after the tool sends it.");
    }

    private static JsonObject Project(JsonElement message)
    {
        var body = HotelClient.JsonText(message, "Body");
        return new JsonObject
        {
            ["id"] = HotelClient.JsonText(message, "Id"),
            ["direction"] = HotelClient.JsonText(message, "Direction"),
            ["channel"] = HotelClient.JsonText(message, "Channel"),
            ["status"] = HotelClient.JsonText(message, "Status"),
            ["from"] = HotelClient.JsonText(message, "From"),
            ["to"] = HotelClient.JsonText(message, "To"),
            ["subject"] = HotelClient.JsonText(message, "Subject"),
            ["kind"] = HotelClient.JsonText(message, "Kind"),
            ["bookingId"] = HotelClient.JsonText(message, "BookingId"),
            ["year"] = HotelClient.JsonInt(message, "Year"),
            ["invoiceNumber"] = HotelClient.JsonText(message, "InvoiceNumber"),
            ["relayedAt"] = HotelClient.JsonText(message, "RelayedAt"),
            ["body"] = body is null || body.Length <= 240 ? body : body[..240]
        };
    }
}
