using System.ComponentModel;
using ModelContextProtocol.Server;

namespace HotelDruid.Mcp;

[McpServerPromptType]
public static class HotelPrompts
{
    [McpServerPrompt(Name = "front_desk_morning", Title = "Open the desk")]
    [Description("Read today and say who is arriving, who is in house, who is leaving, and which outside requests are waiting.")]
    public static string Morning()
        => "Call read_desk with no date. Summarise arrivals, in-house guests, departures, and waiting channel requests in plain sentences. Do not change any stay.";

    [McpServerPrompt(Name = "take_a_reservation", Title = "Take a reservation")]
    [Description("Find a free room and create the reservation.")]
    public static string Reserve(
        [Description("Arrival yyyy-MM-dd.")] string arrival,
        [Description("Departure yyyy-MM-dd.")] string departure,
        [Description("Surname.")] string lastName,
        [Description("Given name.")] string? firstName = null,
        [Description("People in the room.")] int guests = 1,
        [Description("Who sent it, such as phone or Booking.com.")] string? source = null)
        => $"Call find_availability for {arrival} to {departure} for {guests} people. Offer the free rooms. When one is chosen, call create_reservation for {firstName} {lastName}, source {source ?? "desk"}. Then read_booking and tell the price and the amount due.";

    [McpServerPrompt(Name = "accept_an_ota_booking", Title = "Accept an outside booking")]
    [Description("Put a channel request on the inbox and confirm it into a stay.")]
    public static string AcceptOta(
        [Description("Surname.")] string lastName,
        [Description("Arrival yyyy-MM-dd.")] string arrival,
        [Description("Departure yyyy-MM-dd.")] string departure,
        [Description("Room name.")] string room,
        [Description("Their booking number.")] string? externalReference = null,
        [Description("Guest email.")] string? email = null)
        => $"Call find_availability for room {room} from {arrival} to {departure}. If it is free, call submit_channel_request for {lastName}, then confirm_channel_request. If they sent a number ({externalReference ?? "none yet"}), call change_reservation so external reference is that number and source is the channel. Read the booking and say the confirmation is queued, not sent.";

    [McpServerPrompt(Name = "guest_details", Title = "Record guest details")]
    [Description("Save the fields copied from an identity document and attach the person to a stay.")]
    public static string GuestDetails(
        [Description("Booking id.")] string bookingId,
        [Description("Surname.")] string lastName,
        [Description("Given name.")] string firstName)
        => $"Call read_booking {bookingId}. Call save_guest for {firstName} {lastName} with the document fields the person provides. If this is an extra person in the room, call attach_guest. Then read_registration for the arrival date and say which gaps remain. Do not store a scan.";

    [McpServerPrompt(Name = "take_payment_and_invoice", Title = "Take payment and issue an invoice")]
    [Description("Record a payment and queue the invoice.")]
    public static string PayAndInvoice(
        [Description("Booking id.")] string bookingId,
        [Description("Amount received.")] decimal amount)
        => $"Call read_booking_account for {bookingId}. If the amount due is at least {amount}, call take_payment. Then issue_document of type invoice. Tell the invoice number, the new amount due, and that the invoice is queued and not sent.";

    [McpServerPrompt(Name = "record_a_purchase", Title = "Record a purchase")]
    [Description("Record a hotel expense against cash.")]
    public static string Purchase(
        [Description("What was bought.")] string description,
        [Description("Amount.")] decimal amount,
        [Description("Date yyyy-MM-dd, or empty for today.")] string? date = null)
        => $"Call record_expense for {amount} on {description}, date {date ?? "today"}. Then read_ledger for that date and point at the operating-expense and cash lines. Do not use record_drawer for a purchase.";

    [McpServerPrompt(Name = "close_a_business_day", Title = "Review a business day")]
    [Description("Read the night. Sign only if the person has already asked to close that exact date.")]
    public static string CloseDay(
        [Description("Business date yyyy-MM-dd.")] string date)
        => $"Call read_night for {date}, read_registration for {date}, and read_report cashier for {date}. Summarise cash, room revenue, amount still due, and guest-file gaps. Do not call sign_night unless the person has explicitly asked to close {date}.";
}
