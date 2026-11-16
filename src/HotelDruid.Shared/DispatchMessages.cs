namespace HotelDruid.Shared;

/// <summary>
/// One row on the batch list. Email, WhatsApp, SMS, and post share this shape.
/// The hotel stores the row. It does not send it.
/// </summary>
public record DispatchMessageDto(
    string? Id,
    string? Direction,
    string? Channel,
    string? To,
    string? From,
    DateTime? RequestedAt,
    DateTime? DeliverBy,
    string? Subject,
    string? Body,
    string? Attachments,
    string? BookingId,
    int? Year,
    string? InvoiceNumber,
    string? Status,
    DateTime? RelayedAt,
    string? Eml,
    string? Kind);

public record DispatchDocumentRequest(string BookingId, int Year, string DocumentType, int TemplateNumber = 1);

public record DispatchRelayRequest(DateTime? RelayedAt);

public record DispatchEventDto(long Cursor, string Topic, string? MessageId, DateTime At);

public static class DispatchTopics
{
    public const string IncomingReceived = "messages.incoming.received";
    public const string BookingRequest = "messages.incoming.booking-request";
    public const string BookingConfirmed = "bookings.confirmed";
    public const string OutgoingReady = "messages.outgoing.ready";
    public const string OutgoingRelayed = "messages.outgoing.relayed";
}
