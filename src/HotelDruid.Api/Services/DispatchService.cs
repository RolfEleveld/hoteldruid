using System.Globalization;
using System.Text.Json;
using HotelDruid.Api.Models;
using HotelDruid.Shared;
using HotelDruid.Shared.Configuration;

namespace HotelDruid.Api.Services;

public sealed class DispatchMessageStorage
{
    public string? Direction { get; set; }
    public string? Channel { get; set; }
    public string? To { get; set; }
    public string? From { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? DeliverBy { get; set; }
    public string? Subject { get; set; }
    public string? Body { get; set; }
    public string? Attachments { get; set; }
    public string? BookingId { get; set; }
    public int? Year { get; set; }
    public string? InvoiceNumber { get; set; }
    public string? Status { get; set; }
    public DateTime? RelayedAt { get; set; }
    public string? Eml { get; set; }
    public string? Kind { get; set; }
}

public sealed class DispatchEventStorage
{
    public long Cursor { get; set; }
    public string Topic { get; set; } = "";
    public string? MessageId { get; set; }
    public DateTime At { get; set; }
}

public sealed class DispatchCursorStorage
{
    public long Value { get; set; }
}

/// <summary>
/// The batch inbox and outbox. An integrator reads the cursor and writes the relay time.
/// The hotel does not call a channel.
/// </summary>
public sealed class DispatchService
{
    private readonly IKeyValueStore _store;
    private readonly ISystemConfigurationStore _config;
    private readonly IFinancialPostingService _ledger;
    private readonly IAccountChart _chart;
    private readonly ITariffResolutionService _tariffs;
    private readonly SemaphoreSlim _cursor = new(1, 1);

    public DispatchService(
        IKeyValueStore store,
        ISystemConfigurationStore config,
        IFinancialPostingService ledger,
        IAccountChart chart,
        ITariffResolutionService tariffs)
    {
        _store = store;
        _config = config;
        _ledger = ledger;
        _chart = chart;
        _tariffs = tariffs;
    }

    public async Task<DispatchMessageDto> EnqueueAsync(DispatchMessageStorage message, string topic)
    {
        message.RequestedAt = message.RequestedAt == default ? DateTime.UtcNow : message.RequestedAt;
        message.Status ??= message.Direction == "in" ? "review" : "ready";
        var id = await _store.CreateAsync("dispatch_messages", Guid.NewGuid().ToString("N"), message);
        await PublishAsync(topic, id);
        if (string.Equals(message.Kind, "booking-request", StringComparison.OrdinalIgnoreCase))
            await PublishAsync(DispatchTopics.BookingRequest, id);
        return ToDto(id, message);
    }

    public async Task<List<DispatchMessageDto>> ListAsync(string? status, string? channel, string? direction)
    {
        var index = await _store.GetIndexAsync("dispatch_messages");
        var rows = new List<DispatchMessageDto>();
        foreach (var entry in index)
        {
            var message = await _store.GetAsync<DispatchMessageStorage>("dispatch_messages", entry.Value);
            if (message is null)
                continue;
            if (status is not null && !string.Equals(message.Status, status, StringComparison.OrdinalIgnoreCase))
                continue;
            if (channel is not null && !string.Equals(message.Channel, channel, StringComparison.OrdinalIgnoreCase))
                continue;
            if (direction is not null && !string.Equals(message.Direction, direction, StringComparison.OrdinalIgnoreCase))
                continue;
            rows.Add(ToDto(entry.Value, message));
        }
        return rows.OrderBy(row => row.RequestedAt).ToList();
    }

    public async Task<DispatchMessageDto?> RelayAsync(string id, DateTime? relayedAt)
    {
        var message = await _store.GetAsync<DispatchMessageStorage>("dispatch_messages", id);
        if (message is null)
            return null;
        message.Status = "relayed";
        message.RelayedAt = relayedAt ?? DateTime.UtcNow;
        await _store.UpdateAsync("dispatch_messages", id, message);
        await PublishAsync(DispatchTopics.OutgoingRelayed, id);
        return ToDto(id, message);
    }

    public async Task<List<DispatchEventDto>> EventsAsync(long after)
    {
        var index = await _store.GetIndexAsync("dispatch_events");
        var rows = new List<DispatchEventDto>();
        foreach (var entry in index)
        {
            var stored = await _store.GetAsync<DispatchEventStorage>("dispatch_events", entry.Value);
            if (stored is null || stored.Cursor <= after)
                continue;
            rows.Add(new DispatchEventDto(stored.Cursor, stored.Topic, stored.MessageId, stored.At));
        }
        return rows.OrderBy(row => row.Cursor).ToList();
    }

    public async Task<DispatchMessageDto> EnqueueDocumentAsync(DocumentContext context, string html, string eml, string documentType)
    {
        var channel = "email";
        var footer = QrFooter(context);
        var body = html.Contains("hoteldruid:pay", StringComparison.Ordinal) ? html : html + footer;
        var message = new DispatchMessageStorage
        {
            Direction = "out",
            Channel = channel,
            To = context.ClientEmail,
            From = context.HotelEmail,
            DeliverBy = DateTime.UtcNow,
            Subject = $"{documentType} {context.InvoiceNumber} {context.BookingReference}".Trim(),
            Body = body,
            BookingId = context.BookingId,
            Year = context.BookingYear,
            InvoiceNumber = context.InvoiceNumber,
            Kind = documentType,
            Eml = eml,
            Status = "ready"
        };
        return await EnqueueAsync(message, DispatchTopics.OutgoingReady);
    }

    public async Task<(DispatchMessageDto? Message, string? Error)> AcceptAsync(string id)
    {
        var message = await _store.GetAsync<DispatchMessageStorage>("dispatch_messages", id);
        if (message is null)
            return (null, "missing");
        if (!string.Equals(message.Direction, "in", StringComparison.OrdinalIgnoreCase))
            return (ToDto(id, message), "not-incoming");

        BookingRequest? request;
        try
        {
            request = JsonSerializer.Deserialize<BookingRequest>(message.Body ?? "", new JsonSerializerOptions(JsonSerializerDefaults.Web));
        }
        catch (JsonException)
        {
            request = null;
        }

        if (request is null || string.IsNullOrWhiteSpace(request.LastName) || request.Arrival is null || request.Departure is null || string.IsNullOrWhiteSpace(request.RoomId))
            return (ToDto(id, message), "waiting");

        var arrival = request.Arrival.Value;
        var year = arrival.Year;
        var client = new ClientStorageModel
        {
            LastName = request.LastName.Trim(),
            FirstName = request.FirstName?.Trim(),
            Email = request.Email ?? message.From,
            CreatedAt = DateTime.UtcNow
        };
        var clientId = await _store.CreateAsync("clients", $"{client.LastName}_{Guid.NewGuid():N}", client);
        var booking = new BookingStorageModel
        {
            Year = year,
            ClientId = clientId,
            RoomId = request.RoomId,
            ArrivalDate = arrival,
            DepartureDate = request.Departure,
            Status = "Confirmed",
            ExpectedGuestCount = request.Guests ?? 1,
            ActualGuestCount = request.Guests,
            Source = "dispatch",
            ExternalReference = id,
            Notes = "Accepted from the inbox."
        };
        await ResolveTariffAsync(booking);
        var bookingId = await _store.CreateAsync("bookings", $"{year}_{Guid.NewGuid():N}", booking);
        await PostStayAsync(bookingId, booking);

        message.Status = "confirmed";
        message.BookingId = bookingId;
        message.Year = year;
        await _store.UpdateAsync("dispatch_messages", id, message);
        await PublishAsync(DispatchTopics.BookingConfirmed, id);

        var confirmation = new DispatchMessageStorage
        {
            Direction = "out",
            Channel = string.IsNullOrWhiteSpace(message.Channel) ? "email" : message.Channel,
            To = client.Email ?? message.From,
            From = message.To,
            Subject = $"Booking {bookingId}",
            Body = $"Your stay from {arrival:yyyy-MM-dd} is confirmed. Booking {bookingId}.",
            BookingId = bookingId,
            Year = year,
            Kind = "confirmation",
            Status = "ready"
        };
        await EnqueueAsync(confirmation, DispatchTopics.OutgoingReady);
        return (ToDto(id, message), null);
    }

    public async Task<int> ProcessReadyAsync()
    {
        var config = await _config.GetAsync();
        if (config is null || !config.MessageAutomation)
            return 0;
        var waiting = await ListAsync("review", null, "in");
        var done = 0;
        foreach (var message in waiting)
        {
            if (message.Id is null)
                continue;
            var (_, error) = await AcceptAsync(message.Id);
            if (error is null)
                done++;
        }
        return done;
    }

    private async Task ResolveTariffAsync(BookingStorageModel booking)
    {
        if (string.IsNullOrWhiteSpace(booking.RoomId) || booking.ArrivalDate is null || booking.DepartureDate is null)
            return;
        var guests = booking.ActualGuestCount ?? booking.ExpectedGuestCount ?? 1;
        var result = await _tariffs.ResolveAsync(booking.RoomId, booking.ArrivalDate.Value, booking.DepartureDate.Value, guests);
        booking.TariffId = result?.TariffId;
        booking.TariffTotalPrice = result?.TotalPrice;
    }

    private async Task PostStayAsync(string bookingId, BookingStorageModel booking)
    {
        var total = Convert.ToDecimal(booking.TariffTotalPrice ?? 0);
        if (total <= 0)
            return;
        var chart = await _chart.CurrentAsync();
        await _ledger.PostBalancedAsync(
            DateTime.UtcNow,
            "booking_stay_charge",
            "Stay charge from an accepted inbox request",
            total,
            chart.GuestLedger,
            "asset",
            chart.RoomRevenue,
            "revenue",
            bookingId: bookingId,
            sourceType: "booking",
            sourceId: bookingId);
    }

    private async Task PublishAsync(string topic, string messageId)
    {
        await _cursor.WaitAsync();
        try
        {
            var index = await _store.GetIndexAsync("dispatch_cursor");
            DispatchCursorStorage stored;
            string cursorId;
            if (!index.TryGetValue("cursor", out cursorId!))
            {
                stored = new DispatchCursorStorage { Value = 1 };
                cursorId = await _store.CreateAsync("dispatch_cursor", "cursor", stored);
            }
            else
            {
                stored = await _store.GetAsync<DispatchCursorStorage>("dispatch_cursor", cursorId) ?? new DispatchCursorStorage();
                stored.Value++;
                await _store.UpdateAsync("dispatch_cursor", cursorId, stored);
            }
            var ev = new DispatchEventStorage
            {
                Cursor = stored.Value,
                Topic = topic,
                MessageId = messageId,
                At = DateTime.UtcNow
            };
            await _store.CreateAsync("dispatch_events", stored.Value.ToString("D12", CultureInfo.InvariantCulture), ev);
        }
        finally
        {
            _cursor.Release();
        }
    }

    private static string QrFooter(DocumentContext context)
    {
        var parts = new List<string>();
        if (!string.IsNullOrWhiteSpace(context.PaymentQrCode))
            parts.Add($"<p>Payment</p><img alt=\"payment\" src=\"{context.PaymentQrCode}\" />");
        if (!string.IsNullOrWhiteSpace(context.InvoiceQrCode))
            parts.Add($"<p>Invoice</p><img alt=\"invoice\" src=\"{context.InvoiceQrCode}\" />");
        if (!string.IsNullOrWhiteSpace(context.BookingQrCode))
            parts.Add($"<p>Booking</p><img alt=\"booking\" src=\"{context.BookingQrCode}\" />");
        return parts.Count == 0 ? "" : "<div>" + string.Join("", parts) + "</div>";
    }

    public static DispatchMessageDto ToDto(string id, DispatchMessageStorage message) => new(
        id,
        message.Direction,
        message.Channel,
        message.To,
        message.From,
        message.RequestedAt,
        message.DeliverBy,
        message.Subject,
        message.Body,
        message.Attachments,
        message.BookingId,
        message.Year,
        message.InvoiceNumber,
        message.Status,
        message.RelayedAt,
        message.Eml,
        message.Kind);

    private sealed class BookingRequest
    {
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? Email { get; set; }
        public DateOnly? Arrival { get; set; }
        public DateOnly? Departure { get; set; }
        public string? RoomId { get; set; }
        public int? Guests { get; set; }
    }
}
