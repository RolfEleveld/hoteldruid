using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using HotelDruid.Api.Models;
using HotelDruid.Shared;
using HotelDruid.Shared.Configuration;

namespace HotelDruid.Api.Services;

/// <summary>
/// Assembles the DocumentContext from the key-value store and renders HTML templates.
///
/// Template syntax:
///   Scalar tokens  : {{Namespace.Field}}
///   Repeating rows : <!-- FOREACH:ListName -->...{{Row.Field}}...<!-- ENDFOREACH:ListName -->
///
/// Supported list names: Guests, Costs, Payments
/// </summary>
public sealed class DocumentRenderService : IDocumentRenderService
{
    private static readonly Regex ForeachPattern = new(
        @"<!--\s*FOREACH:(?<list>\w+)\s*-->(?<body>.*?)<!--\s*ENDFOREACH:\k<list>\s*-->",
        RegexOptions.Singleline | RegexOptions.Compiled);

    private readonly IKeyValueStore _store;
    private readonly ISystemConfigurationStore _configStore;

    public DocumentRenderService(IKeyValueStore store, ISystemConfigurationStore configStore)
    {
        _store = store;
        _configStore = configStore;
    }

    // -----------------------------------------------------------------------
    // Context assembly
    // -----------------------------------------------------------------------

    public async Task<DocumentContext?> BuildContextAsync(string bookingId, int year, string templateType)
    {
        // 1. Load booking
        var booking = await _store.GetAsync<BookingStorageModel>("bookings", bookingId);
        if (booking is null)
            return null;

        // 2. Load primary client
        ClientStorageModel? client = null;
        if (!string.IsNullOrWhiteSpace(booking.ClientId))
            client = await _store.GetAsync<ClientStorageModel>("clients", booking.ClientId);

        // 3. Load room
        RoomStorageModel? room = null;
        if (!string.IsNullOrWhiteSpace(booking.RoomId))
            room = await _store.GetAsync<RoomStorageModel>("rooms", booking.RoomId);

        // 4. Load booking costs
        var costsIdx = await _store.GetIndexAsync("booking_costs");
        var costs = new List<CostContextRow>();
        double totalAmount = 0;
        foreach (var kvp in costsIdx)
        {
            var bc = await _store.GetAsync<BookingCostStorageModel>("booking_costs", kvp.Value);
            if (bc is null || bc.BookingId != bookingId) continue;
            var amount = bc.Amount ?? 0;
            totalAmount += amount;
            costs.Add(new CostContextRow
            {
                Description = bc.Description ?? string.Empty,
                Amount = amount.ToString("F2", CultureInfo.InvariantCulture),
                TariffId = bc.TariffId ?? string.Empty
            });
        }

        // 5. Load additional guests
        var guestsIdx = await _store.GetIndexAsync("booking_guests");
        var guests = new List<GuestContextRow>();
        foreach (var kvp in guestsIdx)
        {
            var bg = await _store.GetAsync<BookingGuestStorageModel>("booking_guests", kvp.Value);
            if (bg is null || bg.BookingId != bookingId) continue;
            ClientStorageModel? guestClient = null;
            if (!string.IsNullOrWhiteSpace(bg.ClientId))
                guestClient = await _store.GetAsync<ClientStorageModel>("clients", bg.ClientId);
            guests.Add(BuildGuestRow(bg.GuestNumber ?? 0, guestClient));
        }
        guests.Sort((a, b) => a.GuestNumber.CompareTo(b.GuestNumber));

        // 6. Load money history (payments) — filtered by year; MoneyHistory has no BookingId link
        var moneyIdx = await _store.GetIndexAsync("money_history");
        var payments = new List<PaymentContextRow>();
        foreach (var kvp in moneyIdx)
        {
            var mh = await _store.GetAsync<MoneyHistoryStorageModel>("money_history", kvp.Value);
            if (mh is null || mh.Year != year) continue;
            payments.Add(new PaymentContextRow
            {
                Amount = (mh.Amount ?? 0).ToString("F2", CultureInfo.InvariantCulture),
                Date = mh.Date?.ToString("yyyy-MM-dd") ?? string.Empty,
                Method = mh.Type ?? string.Empty,
                Description = mh.Description ?? string.Empty,
                Type = mh.Type ?? string.Empty
            });
        }

        // 7. Hotel info from system configuration
        var config = await _configStore.GetAsync() ?? new SystemConfiguration();
        var settings = config.Settings ?? new Dictionary<string, string>();
        var currency = config.DefaultCurrency ?? settings.GetValueOrDefault("Currency", "EUR");

        // 8. Compute nights
        int nights = 0;
        if (booking.ArrivalDate.HasValue && booking.DepartureDate.HasValue)
            nights = (booking.DepartureDate.Value.ToDateTime(TimeOnly.MinValue)
                      - booking.ArrivalDate.Value.ToDateTime(TimeOnly.MinValue)).Days;

        return new DocumentContext
        {
            // Hotel
            HotelName = settings.GetValueOrDefault("HotelName", string.Empty),
            HotelAddress = settings.GetValueOrDefault("HotelAddress", string.Empty),
            HotelCity = settings.GetValueOrDefault("HotelCity", string.Empty),
            HotelPhone = settings.GetValueOrDefault("HotelPhone", string.Empty),
            HotelEmail = settings.GetValueOrDefault("HotelEmail", string.Empty),
            HotelVatNumber = settings.GetValueOrDefault("HotelVatNumber", string.Empty),
            HotelCurrency = currency,

            // Booking
            BookingId = bookingId,
            BookingYear = year,
            BookingArrivalDate = booking.ArrivalDate?.ToString("yyyy-MM-dd") ?? string.Empty,
            BookingDepartureDate = booking.DepartureDate?.ToString("yyyy-MM-dd") ?? string.Empty,
            BookingNights = nights,
            BookingStatus = booking.Status ?? string.Empty,
            BookingNotes = booking.Notes ?? string.Empty,

            // Client
            ClientLastName = client?.LastName ?? string.Empty,
            ClientFirstName = client?.FirstName ?? string.Empty,
            ClientFullName = FormatFullName(client?.Title, client?.FirstName, client?.LastName),
            ClientTitle = client?.Title ?? string.Empty,
            ClientEmail = client?.Email ?? string.Empty,
            ClientPhone = client?.Phone ?? string.Empty,
            ClientStreet = client?.Street ?? string.Empty,
            ClientStreetNumber = client?.StreetNumber ?? string.Empty,
            ClientCity = client?.City ?? string.Empty,
            ClientPostalCode = client?.PostalCode ?? string.Empty,
            ClientRegion = client?.Region ?? string.Empty,
            ClientNation = client?.Nation ?? string.Empty,
            ClientTaxCode = client?.TaxCode ?? string.Empty,
            ClientVatNumber = client?.VatNumber ?? string.Empty,

            // Room
            RoomName = room?.Name ?? string.Empty,
            RoomFloor = room?.FloorNumber ?? string.Empty,
            RoomHouseNumber = room?.HouseNumber ?? string.Empty,
            RoomCapacity = room?.Capacity ?? 0,

            // Financial
            InvoiceTotalAmount = totalAmount.ToString("F2", CultureInfo.InvariantCulture),
            InvoiceCurrency = currency,

            // Meta
            DocumentToday = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            DocumentTemplateType = templateType,

            // Lists
            Guests = guests,
            Costs = costs,
            Payments = payments
        };
    }

    // -----------------------------------------------------------------------
    // Rendering
    // -----------------------------------------------------------------------

    public string RenderHtml(string templateContent, DocumentContext ctx)
    {
        // First expand FOREACH blocks
        var result = ForeachPattern.Replace(templateContent, match =>
        {
            var listName = match.Groups["list"].Value;
            var body = match.Groups["body"].Value;
            return listName switch
            {
                "Guests" => ExpandList(body, ctx.Guests, ExpandGuestTokens),
                "Costs" => ExpandList(body, ctx.Costs, ExpandCostTokens),
                "Payments" => ExpandList(body, ctx.Payments, ExpandPaymentTokens),
                _ => string.Empty
            };
        });

        // Then substitute scalar tokens
        result = ApplyScalarTokens(result, ctx);
        return result;
    }

    public (string content, string contentType, string filename) ApplyFormat(
        string renderedHtml, DocumentContext ctx, DocumentFormat format)
    {
        var safeType = SafeFilename(ctx.DocumentTemplateType);
        var safeId = SafeFilename(ctx.BookingId);

        return format switch
        {
            DocumentFormat.Print => (InjectPrintScript(renderedHtml), "text/html; charset=utf-8",
                $"{safeType}-{safeId}.html"),

            DocumentFormat.Doc => (renderedHtml, "application/msword",
                $"{safeType}-{safeId}.doc"),

            DocumentFormat.Eml => (BuildEml(renderedHtml, ctx), "message/rfc822",
                $"{safeType}-{safeId}.eml"),

            _ => (renderedHtml, "text/html; charset=utf-8", $"{safeType}-{safeId}.html")
        };
    }

    // -----------------------------------------------------------------------
    // Token substitution helpers
    // -----------------------------------------------------------------------

    private static string ApplyScalarTokens(string html, DocumentContext ctx)
    {
        return html
            // Hotel
            .Replace("{{Hotel.Name}}", Encode(ctx.HotelName))
            .Replace("{{Hotel.Address}}", Encode(ctx.HotelAddress))
            .Replace("{{Hotel.City}}", Encode(ctx.HotelCity))
            .Replace("{{Hotel.Phone}}", Encode(ctx.HotelPhone))
            .Replace("{{Hotel.Email}}", Encode(ctx.HotelEmail))
            .Replace("{{Hotel.VatNumber}}", Encode(ctx.HotelVatNumber))
            .Replace("{{Hotel.Currency}}", Encode(ctx.HotelCurrency))
            // Booking
            .Replace("{{Booking.Id}}", Encode(ctx.BookingId))
            .Replace("{{Booking.Year}}", ctx.BookingYear.ToString())
            .Replace("{{Booking.ArrivalDate}}", Encode(ctx.BookingArrivalDate))
            .Replace("{{Booking.DepartureDate}}", Encode(ctx.BookingDepartureDate))
            .Replace("{{Booking.Nights}}", ctx.BookingNights.ToString())
            .Replace("{{Booking.Status}}", Encode(ctx.BookingStatus))
            .Replace("{{Booking.Notes}}", Encode(ctx.BookingNotes))
            // Client
            .Replace("{{Client.LastName}}", Encode(ctx.ClientLastName))
            .Replace("{{Client.FirstName}}", Encode(ctx.ClientFirstName))
            .Replace("{{Client.FullName}}", Encode(ctx.ClientFullName))
            .Replace("{{Client.Title}}", Encode(ctx.ClientTitle))
            .Replace("{{Client.Email}}", Encode(ctx.ClientEmail))
            .Replace("{{Client.Phone}}", Encode(ctx.ClientPhone))
            .Replace("{{Client.Street}}", Encode(ctx.ClientStreet))
            .Replace("{{Client.StreetNumber}}", Encode(ctx.ClientStreetNumber))
            .Replace("{{Client.City}}", Encode(ctx.ClientCity))
            .Replace("{{Client.PostalCode}}", Encode(ctx.ClientPostalCode))
            .Replace("{{Client.Region}}", Encode(ctx.ClientRegion))
            .Replace("{{Client.Nation}}", Encode(ctx.ClientNation))
            .Replace("{{Client.TaxCode}}", Encode(ctx.ClientTaxCode))
            .Replace("{{Client.VatNumber}}", Encode(ctx.ClientVatNumber))
            // Room
            .Replace("{{Room.Name}}", Encode(ctx.RoomName))
            .Replace("{{Room.Floor}}", Encode(ctx.RoomFloor))
            .Replace("{{Room.HouseNumber}}", Encode(ctx.RoomHouseNumber))
            .Replace("{{Room.Capacity}}", ctx.RoomCapacity.ToString())
            // Financial
            .Replace("{{Invoice.TotalAmount}}", Encode(ctx.InvoiceTotalAmount))
            .Replace("{{Invoice.Currency}}", Encode(ctx.InvoiceCurrency))
            // Meta
            .Replace("{{Document.Today}}", Encode(ctx.DocumentToday))
            .Replace("{{Document.TemplateType}}", Encode(ctx.DocumentTemplateType));
    }

    private static string ExpandList<T>(string bodyTemplate, IReadOnlyList<T> rows,
        Func<string, T, string> expander)
    {
        if (rows.Count == 0) return string.Empty;
        var sb = new StringBuilder();
        foreach (var row in rows)
            sb.Append(expander(bodyTemplate, row));
        return sb.ToString();
    }

    private static string ExpandGuestTokens(string body, GuestContextRow g) =>
        body
            .Replace("{{Guest.GuestNumber}}", g.GuestNumber.ToString())
            .Replace("{{Guest.FullName}}", Encode(g.FullName))
            .Replace("{{Guest.LastName}}", Encode(g.LastName))
            .Replace("{{Guest.FirstName}}", Encode(g.FirstName))
            .Replace("{{Guest.Email}}", Encode(g.Email))
            .Replace("{{Guest.Phone}}", Encode(g.Phone))
            .Replace("{{Guest.Nationality}}", Encode(g.Nationality))
            .Replace("{{Guest.DateOfBirth}}", Encode(g.DateOfBirth));

    private static string ExpandCostTokens(string body, CostContextRow c) =>
        body
            .Replace("{{Cost.Description}}", Encode(c.Description))
            .Replace("{{Cost.Amount}}", Encode(c.Amount))
            .Replace("{{Cost.TariffId}}", Encode(c.TariffId));

    private static string ExpandPaymentTokens(string body, PaymentContextRow p) =>
        body
            .Replace("{{Payment.Amount}}", Encode(p.Amount))
            .Replace("{{Payment.Date}}", Encode(p.Date))
            .Replace("{{Payment.Method}}", Encode(p.Method))
            .Replace("{{Payment.Description}}", Encode(p.Description))
            .Replace("{{Payment.Type}}", Encode(p.Type));

    // -----------------------------------------------------------------------
    // Format builders
    // -----------------------------------------------------------------------

    private static string InjectPrintScript(string html)
    {
        const string script = "<script>window.addEventListener('load',function(){window.print();});</script>";
        var idx = html.LastIndexOf("</body>", StringComparison.OrdinalIgnoreCase);
        return idx >= 0
            ? html.Insert(idx, script)
            : html + script;
    }

    private static string BuildEml(string htmlBody, DocumentContext ctx)
    {
        var subject = $"Booking {ctx.BookingId} – {ctx.DocumentTemplateType.Replace('-', ' ')}";
        var toAddress = ctx.ClientEmail;
        var fromAddress = ctx.HotelEmail.Length > 0 ? ctx.HotelEmail : "noreply@hoteldruid.local";
        var boundary = $"=_{Guid.NewGuid():N}";

        var sb = new StringBuilder();
        sb.AppendLine($"MIME-Version: 1.0");
        sb.AppendLine($"Date: {DateTime.UtcNow:R}");
        sb.AppendLine($"From: {ctx.HotelName} <{fromAddress}>");
        if (!string.IsNullOrWhiteSpace(toAddress))
            sb.AppendLine($"To: {ctx.ClientFullName} <{toAddress}>");
        sb.AppendLine($"Subject: {subject}");
        sb.AppendLine($"Content-Type: multipart/alternative; boundary=\"{boundary}\"");
        sb.AppendLine();
        sb.AppendLine($"--{boundary}");
        sb.AppendLine("Content-Type: text/html; charset=utf-8");
        sb.AppendLine("Content-Transfer-Encoding: base64");
        sb.AppendLine();
        sb.AppendLine(Convert.ToBase64String(Encoding.UTF8.GetBytes(htmlBody)));
        sb.AppendLine($"--{boundary}--");
        return sb.ToString();
    }

    // -----------------------------------------------------------------------
    // Private utilities
    // -----------------------------------------------------------------------

    private static string Encode(string value) =>
        System.Net.WebUtility.HtmlEncode(value);

    private static string FormatFullName(string? title, string? first, string? last)
    {
        var parts = new[] { title, first, last }.Where(p => !string.IsNullOrWhiteSpace(p));
        return string.Join(" ", parts);
    }

    private static GuestContextRow BuildGuestRow(int number, ClientStorageModel? c) =>
        new()
        {
            GuestNumber = number,
            LastName = c?.LastName ?? string.Empty,
            FirstName = c?.FirstName ?? string.Empty,
            FullName = FormatFullName(c?.Title, c?.FirstName, c?.LastName),
            Email = c?.Email ?? string.Empty,
            Phone = c?.Phone ?? string.Empty,
            Nationality = c?.Nationality ?? string.Empty,
            DateOfBirth = c?.DateOfBirth?.ToString("yyyy-MM-dd") ?? string.Empty
        };

    private static string SafeFilename(string value) =>
        string.IsNullOrWhiteSpace(value)
            ? "document"
            : Regex.Replace(value, @"[^a-zA-Z0-9\-_]", "-");
}
