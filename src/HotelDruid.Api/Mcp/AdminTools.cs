using System.ComponentModel;
using System.Text.Json.Nodes;
using ModelContextProtocol.Server;

namespace HotelDruid.Mcp;

[McpServerToolType]
public sealed class AdminTools(HotelClient hotel)
{
    [McpServerTool(Name = "read_hotel", Title = "Read the hotel", ReadOnly = true, Idempotent = true, Destructive = false, OpenWorld = false, UseStructuredContent = true)]
    [Description("Read how this hotel is set: name, currency, tax, chart, registration standard, whether inbox automation is on, and whether Keycloak is the front. It does not change anything.")]
    public Task<HotelReply> ReadHotel(CancellationToken cancellationToken = default)
        => HotelCall.Run(() => Profile(cancellationToken));

    [McpServerTool(Name = "read_registration", Title = "Read the guest file", ReadOnly = true, Idempotent = true, Destructive = false, OpenWorld = false, UseStructuredContent = true)]
    [Description("Read the official guest-file gaps for one date under the hotel's registration standard. A gap is a missing name or a missing required field. Signing the night does not clear it.")]
    public Task<HotelReply> Registration(
        [Description("Date yyyy-MM-dd. Omit for today.")] string? date = null,
        CancellationToken cancellationToken = default)
        => HotelCall.Run(async () =>
        {
            var day = string.IsNullOrWhiteSpace(date) ? DateOnly.FromDateTime(DateTime.UtcNow) : HotelClient.Date(date);
            if (day is null)
                return new HotelReply(false, "The date must be yyyy-MM-dd.", null, "Omit it for today.");
            var report = await hotel.GetAsync($"api/guest-files?date={BookingView.Day(day.Value)}", cancellationToken);
            var ready = report is not null && HotelClient.JsonBool(report.Value, "Ready");
            return new HotelReply(true, ready ? "The guest file has no gaps for that day." : "The guest file has gaps.", report is null ? null : JsonNode.Parse(report.Value.GetRawText()), "save_guest and attach_guest for a missing person or a missing field.");
        });

    [McpServerTool(Name = "read_night", Title = "Read the night audit", ReadOnly = true, Idempotent = true, Destructive = false, OpenWorld = false, UseStructuredContent = true)]
    [Description("Read one business day: whether it is signed, the cash and room figures, and the guest-file gaps. Signing means the desk has seen the day. This tool does not sign it.")]
    public Task<HotelReply> Night(
        [Description("Business date yyyy-MM-dd.")] string date,
        CancellationToken cancellationToken = default)
        => HotelCall.Run(async () =>
        {
            var day = HotelClient.Date(date);
            if (day is null)
                return new HotelReply(false, "The date must be yyyy-MM-dd.", null, "Name the business date.");
            var audit = await hotel.GetAsync($"api/night-audits/day?date={BookingView.Day(day.Value)}", cancellationToken);
            var signed = audit is not null && HotelClient.JsonBool(audit.Value, "Signed");
            return new HotelReply(true, signed ? "That night is signed." : "That night is not signed.", audit is null ? null : JsonNode.Parse(audit.Value.GetRawText()), "sign_night only when the person named this date and asked to close it.");
        });

    [McpServerTool(Name = "sign_night", Title = "Sign the night audit", ReadOnly = false, Idempotent = true, Destructive = true, OpenWorld = false, UseStructuredContent = true)]
    [Description("Sign one business date that is not in the future. This means the desk has seen and accepted the day. It does not mean guests have paid. Pass confirm true only when the person named that exact date and asked to close it. The hotel refuses a future date.")]
    public Task<HotelReply> Sign(
        [Description("Business date yyyy-MM-dd.")] string date,
        [Description("Must be true. False does not sign.")] bool confirm,
        [Description("The cash agrees with the drawer.")] bool cashAgrees = false,
        [Description("The balances were reviewed.")] bool balancesReviewed = false,
        [Description("The rooms were seen.")] bool roomsSeen = false,
        [Description("Desk name that signs the day. Required when confirm is true.")] string? person = null,
        [Description("Note stored with the signature.")] string? note = null,
        CancellationToken cancellationToken = default)
        => HotelCall.Run(async () =>
        {
            if (!confirm)
                return new HotelReply(false, "Not signed. confirm was false.", null, "Ask the person before passing confirm true.");
            if (string.IsNullOrWhiteSpace(person))
                return new HotelReply(false, "A desk name is required before the day can be signed.", null, "Pass the name the clerk is working under.");
            var day = HotelClient.Date(date);
            if (day is null)
                return new HotelReply(false, "The date must be yyyy-MM-dd.", null, "Name the business date.");
            var audit = await hotel.PostAsync($"api/night-audits/day?date={BookingView.Day(day.Value)}", new
            {
                CashAgrees = cashAgrees,
                BalancesReviewed = balancesReviewed,
                RoomsSeen = roomsSeen,
                Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim()
            }, cancellationToken, new Dictionary<string, string> { [HotelDruid.Shared.DeskUserNames.HeaderName] = person.Trim() });
            return new HotelReply(true, $"{BookingView.Day(day.Value)} is signed. That records the day as seen, not as paid.", audit is null ? null : JsonNode.Parse(audit.Value.GetRawText()), "read_night to show the signed figures.");
        });

    [McpServerTool(Name = "read_report", Title = "Read a desk report", ReadOnly = true, Idempotent = true, Destructive = false, OpenWorld = false, UseStructuredContent = true)]
    [Description("Read a desk or manager sheet. Keys: arrivals, in-house, departures, room-status, room-board, cashier, flash, forecast, balances, performance, tax, statistics. Returns the totals and at most 25 rows of each section.")]
    public Task<HotelReply> Report(
        [Description("Report key.")] string key,
        [Description("Date yyyy-MM-dd. Omit for today.")] string? date = null,
        CancellationToken cancellationToken = default)
        => HotelCall.Run(() => ReportCore(key, date, cancellationToken));

    [McpServerTool(Name = "set_message_automation", Title = "Set inbox automation", ReadOnly = false, Idempotent = true, Destructive = true, OpenWorld = false, UseStructuredContent = true)]
    [Description("Turn structured-inbox automation on or off. On means process_channel_inbox may create bookings from complete requests. Off means a person confirms each one. Every other hotel setting is written back unchanged.")]
    public Task<HotelReply> Automation(
        [Description("True to let the inbox pass create bookings. False to keep a person in the loop.")] bool enabled,
        CancellationToken cancellationToken = default)
        => HotelCall.Run(() => AutomationCore(enabled, cancellationToken));

    private async Task<HotelReply> Profile(CancellationToken cancellationToken)
    {
        JsonNode? config = null;
        try
        {
            var body = await hotel.GetAsync("api/system/configuration", cancellationToken);
            if (body is not null)
                config = JsonNode.Parse(body.Value.GetRawText());
        }
        catch (HotelCallException ex) when (ex.Status == 404)
        {
            config = null;
        }

        var identity = await hotel.GetAsync("api/system/identity-front", cancellationToken);
        var setup = await hotel.GetAsync("api/system/setup/status", cancellationToken);
        var accounts = await hotel.ArrayAsync("api/ledger/accounts", cancellationToken);
        var chart = new JsonArray();
        foreach (var account in accounts)
        {
            chart.Add(new JsonObject
            {
                ["code"] = HotelClient.JsonText(account, "Code"),
                ["name"] = HotelClient.JsonText(account, "Name"),
                ["role"] = HotelClient.JsonText(account, "Role")
            });
        }

        var settings = config?["Settings"] as JsonObject;
        return new HotelReply(true, "Hotel profile.", new JsonObject
        {
            ["hotelName"] = settings?["HotelName"]?.GetValue<string>(),
            ["currency"] = config?["DefaultCurrency"]?.GetValue<string>() ?? settings?["Currency"]?.GetValue<string>(),
            ["accountingStandard"] = config?["AccountingStandard"]?.GetValue<string>(),
            ["registrationStandard"] = config?["RegistrationStandard"]?.GetValue<string>(),
            ["messageAutomation"] = config?["MessageAutomation"]?.GetValue<bool>() ?? false,
            ["taxPricing"] = settings?["TaxPricing"]?.GetValue<string>(),
            ["taxRateStay"] = settings?["TaxRateStay"]?.GetValue<string>(),
            ["cityTax"] = settings?["CityTaxAmount"]?.GetValue<string>(),
            ["accounts"] = chart,
            ["identity"] = identity is null ? null : JsonNode.Parse(identity.Value.GetRawText()),
            ["setup"] = setup is null ? null : JsonNode.Parse(setup.Value.GetRawText())
        }, "find_availability or read_desk to work the day.");
    }

    private async Task<HotelReply> ReportCore(string key, string? date, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(key))
            return new HotelReply(false, "A report key is required.", null, "Use arrivals, in-house, departures, cashier, or tax.");
        var path = $"api/reports/desk/{Uri.EscapeDataString(key.Trim())}";
        if (HotelClient.Date(date) is DateOnly day)
            path += "?date=" + BookingView.Day(day);
        else if (!string.IsNullOrWhiteSpace(date))
            return new HotelReply(false, "The date must be yyyy-MM-dd.", null, "Omit it for today.");

        var report = await hotel.GetAsync(path, cancellationToken);
        if (report is null)
            return new HotelReply(false, "The report was empty.", null, "Check the key.");
        var sections = new JsonArray();
        if (report.Value.TryGetProperty("Sections", out var rawSections))
        {
            foreach (var section in rawSections.EnumerateArray())
            {
                var rows = new JsonArray();
                if (section.TryGetProperty("Rows", out var rawRows))
                {
                    foreach (var row in rawRows.EnumerateArray())
                    {
                        if (rows.Count >= 25)
                            break;
                        rows.Add(JsonNode.Parse(row.GetRawText()));
                    }
                }

                var totals = section.TryGetProperty("Totals", out var rawTotals) ? JsonNode.Parse(rawTotals.GetRawText()) : null;
                sections.Add(new JsonObject
                {
                    ["heading"] = HotelClient.JsonText(section, "HeadingKey"),
                    ["rows"] = rows,
                    ["rowCount"] = section.TryGetProperty("Rows", out var count) ? count.GetArrayLength() : 0,
                    ["totals"] = totals
                });
            }
        }

        return new HotelReply(true, $"Report {key}.", new JsonObject
        {
            ["key"] = HotelClient.JsonText(report.Value, "Key"),
            ["hotel"] = HotelClient.JsonText(report.Value, "HotelName"),
            ["currency"] = HotelClient.JsonText(report.Value, "Currency"),
            ["from"] = HotelClient.JsonText(report.Value, "From"),
            ["to"] = HotelClient.JsonText(report.Value, "To"),
            ["sections"] = sections
        }, "read_booking for a row you need to act on.");
    }

    private async Task<HotelReply> AutomationCore(bool enabled, CancellationToken cancellationToken)
    {
        var current = await hotel.GetAsync("api/system/configuration", cancellationToken);
        if (current is null)
            return new HotelReply(false, "The hotel has no configuration to update.", null, "Finish setup on the desk before changing automation.");
        var body = JsonNode.Parse(current.Value.GetRawText())!.AsObject();
        body["MessageAutomation"] = enabled;
        var saved = await hotel.PutAsync("api/system/configuration", body, cancellationToken);
        return new HotelReply(true, enabled ? "Inbox automation is on. A complete request can become a booking when the inbox is processed." : "Inbox automation is off. A person confirms each request.", saved is null ? null : new JsonObject { ["messageAutomation"] = enabled }, enabled ? "process_channel_inbox for the waiting requests." : "confirm_channel_request for each request.");
    }
}
