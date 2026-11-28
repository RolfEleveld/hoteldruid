using System.ComponentModel;
using System.Text.Json.Nodes;
using ModelContextProtocol.Server;

namespace HotelDruid.Mcp;

[McpServerToolType]
public sealed class FinanceTools(HotelClient hotel)
{
    [McpServerTool(Name = "read_booking_account", Title = "Read a booking account", ReadOnly = true, Idempotent = true, Destructive = false, OpenWorld = false, UseStructuredContent = true)]
    [Description("Read what the stay costs, what has been paid, what is still a deposit, and the amount due, plus the ledger movements on that booking.")]
    public Task<HotelReply> Account(
        [Description("Booking id.")] string bookingId,
        CancellationToken cancellationToken = default)
        => HotelCall.Run(() => AccountCore(bookingId, cancellationToken));

    [McpServerTool(Name = "take_payment", Title = "Take a payment", ReadOnly = false, Idempotent = false, Destructive = false, OpenWorld = false, UseStructuredContent = true)]
    [Description("Record money received on a stay. Kind payment reduces what is due. Kind deposit is held until checkout applies it. Amount is positive, in the hotel currency.")]
    public Task<HotelReply> Pay(
        [Description("Booking id.")] string bookingId,
        [Description("Amount received, greater than zero.")] decimal amount,
        [Description("payment or deposit. Default payment.")] string? kind = null,
        [Description("What the money is for.")] string? description = null,
        [Description("Name to store as the person who took it.")] string? person = null,
        CancellationToken cancellationToken = default)
        => HotelCall.Run(() => Money(bookingId, amount, kind, description, person, refund: false, cancellationToken));

    [McpServerTool(Name = "refund_payment", Title = "Refund a guest", ReadOnly = false, Idempotent = false, Destructive = true, OpenWorld = false, UseStructuredContent = true)]
    [Description("Refund money on a stay. Amount is positive. Use this only when the person asked to refund that booking.")]
    public Task<HotelReply> Refund(
        [Description("Booking id.")] string bookingId,
        [Description("Amount to refund, greater than zero.")] decimal amount,
        [Description("Why the money is going back.")] string? description = null,
        [Description("Name to store as the person who refunded it.")] string? person = null,
        CancellationToken cancellationToken = default)
        => HotelCall.Run(() => Money(bookingId, amount, "refund", description, person, refund: true, cancellationToken));

    [McpServerTool(Name = "sell_at_desk", Title = "Sell at the desk", ReadOnly = false, Idempotent = false, Destructive = false, OpenWorld = false, UseStructuredContent = true)]
    [Description("Record a point-of-sale sale. With a booking id, the amount is charged to that in-house stay. Without one, it is a walk-up cash sale on the drawer.")]
    public Task<HotelReply> Sell(
        [Description("What was sold.")] string description,
        [Description("Amount, greater than zero.")] decimal amount,
        [Description("Booking id when the sale goes on a stay. The guest must already be checked in.")] string? bookingId = null,
        [Description("Person taking a walk-up cash sale.")] string? person = null,
        CancellationToken cancellationToken = default)
        => HotelCall.Run(() => SellCore(description, amount, bookingId, person, cancellationToken));

    [McpServerTool(Name = "record_expense", Title = "Record a hotel expense", ReadOnly = false, Idempotent = false, Destructive = false, OpenWorld = false, UseStructuredContent = true)]
    [Description("Record a purchase. Money leaves cash and becomes an operating expense. This is the purchase list, not the cash-drawer tape.")]
    public Task<HotelReply> Expense(
        [Description("Amount spent, greater than zero.")] decimal amount,
        [Description("What was bought.")] string description,
        [Description("Date yyyy-MM-dd. Omit for today.")] string? date = null,
        [Description("Cash register name or id. Omit for the first register.")] string? register = null,
        CancellationToken cancellationToken = default)
        => HotelCall.Run(() => ExpenseCore(amount, description, date, register, cancellationToken));

    [McpServerTool(Name = "record_drawer", Title = "Record drawer cash", ReadOnly = false, Idempotent = false, Destructive = false, OpenWorld = false, UseStructuredContent = true)]
    [Description("Record petty cash that is not a guest payment and not a purchase-list expense. Direction in is other income. Direction out is operating expense. Do not use this for a booking payment; use take_payment.")]
    public Task<HotelReply> Drawer(
        [Description("Amount, greater than zero.")] decimal amount,
        [Description("in or out.")] string direction,
        [Description("What the cash was.")] string description,
        [Description("Date yyyy-MM-dd. Omit for today.")] string? date = null,
        [Description("Person named on the row.")] string? person = null,
        CancellationToken cancellationToken = default)
        => HotelCall.Run(() => DrawerCore(amount, direction, description, date, person, cancellationToken));

    [McpServerTool(Name = "issue_document", Title = "Issue a document", ReadOnly = false, Idempotent = true, Destructive = false, OpenWorld = false, UseStructuredContent = true)]
    [Description("Render an invoice, receipt, or confirmation for a stay and put it on the outbox. An invoice takes the next number in that year's series, printed as 2026-00042, and later calls reuse it. The body is the document. The hotel does not email it.")]
    public Task<HotelReply> Issue(
        [Description("Booking id.")] string bookingId,
        [Description("invoice, rechnung, receipt, or confirmation. Default invoice.")] string? documentType = null,
        [Description("Template number. Default 1.")] int templateNumber = 1,
        CancellationToken cancellationToken = default)
        => HotelCall.Run(() => IssueCore(bookingId, documentType, templateNumber, cancellationToken));

    [McpServerTool(Name = "read_ledger", Title = "Read the ledger", ReadOnly = true, Idempotent = true, Destructive = false, OpenWorld = false, UseStructuredContent = true)]
    [Description("Read general-ledger lines between two dates, or the lines of one booking. Returns at most 40 lines plus the count. Hotel cash has an empty booking id.")]
    public Task<HotelReply> Ledger(
        [Description("From date yyyy-MM-dd.")] string from,
        [Description("To date yyyy-MM-dd.")] string to,
        [Description("Booking id. Omit for the whole hotel in that range.")] string? bookingId = null,
        CancellationToken cancellationToken = default)
        => HotelCall.Run(() => LedgerCore(from, to, bookingId, cancellationToken));

    private async Task<HotelReply> AccountCore(string bookingId, CancellationToken cancellationToken)
    {
        var booking = await hotel.BookingAsync(bookingId, cancellationToken);
        if (booking is null)
            return new HotelReply(false, "That booking was not found.", null, "search_hotel first.");
        var year = HotelClient.JsonInt(booking.Value, "Year");
        var id = HotelClient.JsonText(booking.Value, "Id") ?? bookingId;
        var finance = await hotel.GetAsync($"api/bookings/{year}/{Uri.EscapeDataString(id)}/finance", cancellationToken);
        var movements = await hotel.GetAsync($"api/bookings/{year}/{Uri.EscapeDataString(id)}/transactions", cancellationToken);
        var due = finance is null ? "" : BookingView.Money(finance.Value, "TotalDue");
        return new HotelReply(true, $"Amount due {due}.", new JsonObject
        {
            ["account"] = finance is null ? null : JsonNode.Parse(finance.Value.GetRawText()),
            ["movements"] = movements is null ? null : JsonNode.Parse(movements.Value.GetRawText()),
            ["issuedInvoiceNumber"] = HotelClient.JsonText(booking.Value, "IssuedInvoiceNumber")
        }, "take_payment, or issue_document when the person wants the invoice.");
    }

    private async Task<HotelReply> Money(string bookingId, decimal amount, string? kind, string? description, string? person, bool refund, CancellationToken cancellationToken)
    {
        if (amount <= 0)
            return new HotelReply(false, "The amount must be greater than zero.", null, "Ask for the amount.");
        var type = refund ? "refund" : string.Equals(kind, "deposit", StringComparison.OrdinalIgnoreCase) ? "deposit" : "payment";
        var booking = await hotel.BookingAsync(bookingId, cancellationToken);
        if (booking is null)
            return new HotelReply(false, "That booking was not found.", null, "search_hotel first.");
        var year = HotelClient.JsonInt(booking.Value, "Year");
        var id = HotelClient.JsonText(booking.Value, "Id") ?? bookingId;
        var applied = type == "deposit" ? "deposit" : "stay_balance";
        await hotel.PostAsync($"api/bookings/{year}/{Uri.EscapeDataString(id)}/transactions", new
        {
            Type = type,
            Amount = amount,
            Description = string.IsNullOrWhiteSpace(description) ? type : description.Trim(),
            AppliedTo = applied,
            CreatedBy = string.IsNullOrWhiteSpace(person) ? null : person.Trim()
        }, cancellationToken);
        return await AccountCore(id, cancellationToken);
    }

    private async Task<HotelReply> SellCore(string description, decimal amount, string? bookingId, string? person, CancellationToken cancellationToken)
    {
        if (amount <= 0 || string.IsNullOrWhiteSpace(description))
            return new HotelReply(false, "A description and an amount greater than zero are required.", null, "Say what was sold and the price.");
        string? id = null;
        var year = (await hotel.TodayAsync(cancellationToken)).Year;
        if (!string.IsNullOrWhiteSpace(bookingId))
        {
            var booking = await hotel.BookingAsync(bookingId, cancellationToken);
            if (booking is null)
                return new HotelReply(false, "That booking was not found.", null, "Omit the booking for a walk-up sale.");
            id = HotelClient.JsonText(booking.Value, "Id");
            year = HotelClient.JsonInt(booking.Value, "Year");
        }

        var sale = await hotel.PostAsync("api/pos-sales", new
        {
            Year = year,
            Description = description.Trim(),
            Amount = amount,
            BookingId = id,
            Person = string.IsNullOrWhiteSpace(person) ? null : person.Trim(),
            Date = BookingView.Day(await hotel.TodayAsync(cancellationToken))
        }, cancellationToken);
        return new HotelReply(true, id is null ? "Walk-up sale recorded on the drawer." : "Sale charged to the stay.", sale is null ? null : new JsonObject { ["sale"] = JsonNode.Parse(sale.Value.GetRawText()) }, id is null ? "read_ledger for today if you need the cash line." : "read_booking_account for the new amount due.");
    }

    private async Task<HotelReply> ExpenseCore(decimal amount, string description, string? date, string? register, CancellationToken cancellationToken)
    {
        if (amount <= 0 || string.IsNullOrWhiteSpace(description))
            return new HotelReply(false, "A description and an amount greater than zero are required.", null, "Say what was bought.");
        var day = string.IsNullOrWhiteSpace(date) ? await hotel.TodayAsync(cancellationToken) : HotelClient.Date(date);
        if (day is null)
            return new HotelReply(false, "The date must be yyyy-MM-dd.", null, "Omit the date for today.");
        var registerId = await hotel.RegisterIdAsync(register, cancellationToken);
        if (registerId is null)
            return new HotelReply(false, "No cash register matches that name.", null, "Omit register to use the first one, or create the register on the desk.");
        var row = await hotel.PostAsync("api/expenses", new
        {
            Year = day.Value.Year,
            Amount = amount,
            Description = description.Trim(),
            Date = BookingView.Day(day.Value),
            CashRegisterId = registerId
        }, cancellationToken);
        return new HotelReply(true, $"Expense of {amount} recorded on {BookingView.Day(day.Value)}.", row is null ? null : new JsonObject { ["expense"] = JsonNode.Parse(row.Value.GetRawText()) }, "read_ledger if you need the cash and expense accounts.");
    }

    private async Task<HotelReply> DrawerCore(decimal amount, string direction, string description, string? date, string? person, CancellationToken cancellationToken)
    {
        if (amount <= 0 || string.IsNullOrWhiteSpace(description))
            return new HotelReply(false, "A description and an amount greater than zero are required.", null, "Say what the cash was.");
        var outgoing = direction.Equals("out", StringComparison.OrdinalIgnoreCase) || direction.Equals("expense", StringComparison.OrdinalIgnoreCase);
        var incoming = direction.Equals("in", StringComparison.OrdinalIgnoreCase) || direction.Equals("income", StringComparison.OrdinalIgnoreCase);
        if (!outgoing && !incoming)
            return new HotelReply(false, "Direction must be in or out.", null, "in is other income. out is petty cash spent.");
        var day = string.IsNullOrWhiteSpace(date) ? await hotel.TodayAsync(cancellationToken) : HotelClient.Date(date);
        if (day is null)
            return new HotelReply(false, "The date must be yyyy-MM-dd.", null, "Omit the date for today.");
        var registerId = await hotel.RegisterIdAsync(null, cancellationToken);
        var row = await hotel.PostAsync("api/money-history", new
        {
            Year = day.Value.Year,
            Amount = amount,
            Type = outgoing ? "expense" : "income",
            Description = description.Trim(),
            Date = BookingView.Day(day.Value),
            Person = string.IsNullOrWhiteSpace(person) ? null : person.Trim(),
            CashRegisterId = registerId
        }, cancellationToken);
        return new HotelReply(true, outgoing ? "Petty cash out recorded." : "Petty cash in recorded.", row is null ? null : new JsonObject { ["drawer"] = JsonNode.Parse(row.Value.GetRawText()) }, "A booking payment belongs on take_payment, not here.");
    }

    private async Task<HotelReply> IssueCore(string bookingId, string? documentType, int templateNumber, CancellationToken cancellationToken)
    {
        var booking = await hotel.BookingAsync(bookingId, cancellationToken);
        if (booking is null)
            return new HotelReply(false, "That booking was not found.", null, "search_hotel first.");
        var type = string.IsNullOrWhiteSpace(documentType) ? "invoice" : documentType.Trim();
        var year = HotelClient.JsonInt(booking.Value, "Year");
        var id = HotelClient.JsonText(booking.Value, "Id") ?? bookingId;
        var queued = await hotel.PostAsync("api/dispatch/from-document", new
        {
            BookingId = id,
            Year = year,
            DocumentType = type,
            TemplateNumber = templateNumber < 1 ? 1 : templateNumber
        }, cancellationToken);
        var fresh = await hotel.BookingAsync(id, cancellationToken);
        var number = fresh is null ? null : HotelClient.JsonText(fresh.Value, "IssuedInvoiceNumber");
        return new HotelReply(true, number is null ? $"{type} queued. No invoice number was issued for this document type." : $"Invoice {number} queued. The hotel did not send it.", new JsonObject
        {
            ["invoiceNumber"] = number,
            ["message"] = queued is null ? null : JsonNode.Parse(queued.Value.GetRawText())
        }, "mark_message_relayed after a tool sends it. read_booking_account for the figures.");
    }

    private async Task<HotelReply> LedgerCore(string from, string to, string? bookingId, CancellationToken cancellationToken)
    {
        var start = HotelClient.Date(from);
        var end = HotelClient.Date(to);
        if (start is null || end is null || end < start)
            return new HotelReply(false, "From and to must be yyyy-MM-dd, and to must not be before from.", null, "Pass both dates.");
        var path = $"api/ledger/lines?from={BookingView.Day(start.Value)}&to={BookingView.Day(end.Value)}";
        if (!string.IsNullOrWhiteSpace(bookingId))
        {
            var booking = await hotel.BookingAsync(bookingId, cancellationToken);
            if (booking is null)
                return new HotelReply(false, "That booking was not found.", null, "Omit bookingId for the whole hotel.");
            path += "&bookingId=" + Uri.EscapeDataString(HotelClient.JsonText(booking.Value, "Id") ?? bookingId);
        }

        var lines = await hotel.ArrayAsync(path, cancellationToken);
        var shown = new JsonArray();
        foreach (var line in lines.Take(40))
        {
            shown.Add(new JsonObject
            {
                ["timestamp"] = HotelClient.JsonText(line, "Timestamp"),
                ["type"] = HotelClient.JsonText(line, "EntryType"),
                ["account"] = HotelClient.JsonText(line, "AccountCode"),
                ["debit"] = HotelClient.JsonRaw(line, "Debit"),
                ["credit"] = HotelClient.JsonRaw(line, "Credit"),
                ["bookingId"] = HotelClient.JsonText(line, "BookingId"),
                ["description"] = HotelClient.JsonText(line, "Description"),
                ["invoiceNumber"] = HotelClient.JsonText(line, "PrintedInvoiceNumber")
            });
        }

        return new HotelReply(true, $"{lines.Count} ledger line(s) from {BookingView.Day(start.Value)} to {BookingView.Day(end.Value)}.", new JsonObject { ["total"] = lines.Count, ["lines"] = shown }, lines.Count > 40 ? "Narrow the dates or pass a booking id. Only 40 lines were returned." : "read_booking_account for one stay's balance.");
    }
}
