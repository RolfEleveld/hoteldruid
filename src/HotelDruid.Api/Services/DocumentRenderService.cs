using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HotelDruid.Api.Models;
using HotelDruid.Shared;
using HotelDruid.Shared.Configuration;
using QRCoder;

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
    private readonly ILedgerRepository _ledger;
    private readonly IBookingLedgerIndexRepository _ledgerIndex;

    public DocumentRenderService(
        IKeyValueStore store,
        ISystemConfigurationStore configStore,
        ITariffResolutionService tariffs,
        ILedgerRepository ledger,
        IBookingLedgerIndexRepository ledgerIndex)
    {
        _store = store;
        _configStore = configStore;
        _ = tariffs;
        _ledger = ledger;
        _ledgerIndex = ledgerIndex;
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

        // 4. Charges and cash movements come from the booking's ledger.
        var ledgerLines = await BookingTransactionProjection.LoadAsync(_ledger, _ledgerIndex, bookingId, DateTime.MinValue, DateTime.MaxValue);
        var chartConfig = await _configStore.GetAsync();
        var chart = AccountChart.ResolveOrUsali(chartConfig?.AccountingStandard, chartConfig?.AccountCodes);
        var totals = BookingLedgerTotals.From(ledgerLines, chart);
        var payments = MovementRows(ledgerLines);

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

        var prepaidAmount = Convert.ToDecimal(booking.PrepaidAmount ?? 0);

        // 6. Hotel info from system configuration
        var config = await _configStore.GetAsync() ?? new SystemConfiguration();
        var settings = config.Settings ?? new Dictionary<string, string>();
        var currency = config.DefaultCurrency ?? settings.GetValueOrDefault("Currency", "EUR");
        var presented = PresentCharges(ledgerLines, chart, TaxPolicy.From(settings));

        // 7. Compute nights
        int nights = 0;
        if (booking.ArrivalDate.HasValue && booking.DepartureDate.HasValue)
            nights = (booking.DepartureDate.Value.ToDateTime(TimeOnly.MinValue)
                      - booking.ArrivalDate.Value.ToDateTime(TimeOnly.MinValue)).Days;

        var invoiceNumber = await InvoiceSeries.EnsureIssuedAsync(_store, _configStore, bookingId, booking, templateType);
        // The guest ledger still carries the full charges while a deposit sits on the deposit account.
        // The bill states what the guest still has to pay. Checkout applies the deposit and clears it, so it is not subtracted twice.
        var due = (totals.TotalDue - totals.DepositsHeld).ToString("F2", CultureInfo.InvariantCulture);

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
            BookingReference = BuildBookingReference(booking.ArrivalDate, room?.Name),
            BookingQrCode = BuildBookingQrCode(bookingId, year, booking.ArrivalDate, room?.Name),
            InvoiceNumber = invoiceNumber,
            InvoiceQrCode = BuildQr($"hoteldruid:invoice|{invoiceNumber}|{bookingId}|{year}"),
            PaymentQrCode = BuildQr($"hoteldruid:pay|{invoiceNumber}|{due}|{currency}|{bookingId}"),

            // Client
            ClientLastName = client?.LastName ?? string.Empty,
            ClientFirstName = client?.FirstName ?? string.Empty,
            ClientFullName = FormatFullName(client?.Title, client?.FirstName, client?.LastName),
            ClientTitle = client?.Title ?? string.Empty,
            ClientEmail = GetPrimaryEmail(client),
            ClientPhone = GetPrimaryPhone(client),
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
            InvoicePrepaidAmount = prepaidAmount.ToString("F2", CultureInfo.InvariantCulture),
            InvoicePaymentsTotal = totals.PaymentsTotal.ToString("F2", CultureInfo.InvariantCulture),
            InvoiceRefundsTotal = totals.RefundsTotal.ToString("F2", CultureInfo.InvariantCulture),
            InvoiceDepositsHeld = totals.DepositsHeld.ToString("F2", CultureInfo.InvariantCulture),
            InvoiceTotalDue = due,
            InvoiceTotalAmount = totals.ChargesTotal.ToString("F2", CultureInfo.InvariantCulture),
            InvoiceCurrency = currency,
            TaxNotice = presented.Notice,
            TaxNoticeDe = presented.NoticeDe,

            // Meta
            DocumentToday = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            DocumentTemplateType = templateType,

            // Lists
            Guests = guests,
            Costs = presented.Costs,
            Payments = payments,
            Taxes = presented.Recap,
            TaxLines = presented.Lines
        };
    }

    private readonly record struct ChargePresentation(
        List<CostContextRow> Costs,
        List<TaxLineContextRow> Lines,
        List<TaxContextRow> Recap,
        string Notice,
        string NoticeDe);

    private static ChargePresentation PresentCharges(IEnumerable<LedgerEntry> lines, AccountChart chart, TaxPolicy tax)
    {
        var costs = new List<CostContextRow>();
        var taxLines = new List<TaxLineContextRow>();
        var buckets = new Dictionary<(string Label, string Rate), (decimal Net, decimal Tax, decimal Gross)>();

        foreach (var group in lines.GroupBy(JournalKey))
        {
            var room = SumAccount(group, chart.RoomRevenue);
            var other = SumAccount(group, chart.OtherIncome);
            var postedTax = SumAccount(group, chart.OutputTax);
            var city = SumAccount(group, chart.CityTax);
            if (room == 0 && other == 0 && postedTax == 0 && city == 0)
                continue;

            var described = group.FirstOrDefault(line => !string.IsNullOrWhiteSpace(line.Notes)) ?? group.First();
            var description = described.Notes ?? string.Empty;
            var showNet = !tax.Inclusive && postedTax != 0;
            if (room != 0 || (postedTax != 0 && other == 0 && city == 0))
            {
                var gross = room + (other == 0 ? postedTax : 0m);
                costs.Add(Cost(description, showNet ? room : gross));
                AddRecap(buckets, "Stay / Übernachtung", tax.Stay, room, other == 0 ? postedTax : 0m, tax.Inclusive);
            }

            if (other != 0)
            {
                var gross = other + (room == 0 ? postedTax : 0m);
                costs.Add(Cost(description, showNet ? other : gross));
                AddRecap(buckets, "Other / Übrige", tax.Product ?? tax.Stay, other, room == 0 ? postedTax : 0m, tax.Inclusive);
            }

            if (city != 0)
            {
                costs.Add(Cost(string.IsNullOrWhiteSpace(description) ? "City tax / Ortstaxe" : description, city));
                AddCity(buckets, city);
            }

            if (showNet && postedTax != 0)
            {
                var rate = CloseRate(room + other, postedTax, tax.Stay);
                taxLines.Add(new TaxLineContextRow
                {
                    Label = $"VAT {rate} / MwSt. {rate}",
                    Amount = Money(postedTax)
                });
            }
        }

        var recap = tax.Inclusive
            ? buckets.Select(pair => new TaxContextRow
            {
                Label = pair.Key.Label,
                Rate = pair.Key.Rate,
                Net = Money(pair.Value.Net),
                Tax = Money(pair.Value.Tax),
                Gross = Money(pair.Value.Gross)
            }).ToList()
            : [];

        var hasTax = recap.Any(row => row.Tax != "0.00") || taxLines.Count > 0;
        var notice = hasTax && tax.Inclusive
            ? "Prices include tax at the rates below. The tax is not added again."
            : taxLines.Count > 0
                ? "Tax is added at the rates below."
                : string.Empty;
        var noticeDe = hasTax && tax.Inclusive
            ? "Die Preise enthalten die Steuer zu den Sätzen unten. Die Steuer wird nicht noch einmal hinzugerechnet."
            : taxLines.Count > 0
                ? "Die Steuer wird zu den Preisen hinzugerechnet."
                : string.Empty;
        return new ChargePresentation(costs, taxLines, recap, notice, noticeDe);
    }

    private static void AddRecap(
        Dictionary<(string Label, string Rate), (decimal Net, decimal Tax, decimal Gross)> buckets,
        string label,
        decimal? rate,
        decimal netOrGross,
        decimal postedTax,
        bool inclusive)
    {
        if (netOrGross == 0 && postedTax == 0)
            return;
        decimal net;
        decimal tax;
        decimal gross;
        string shown;
        if (postedTax != 0)
        {
            net = netOrGross;
            tax = postedTax;
            gross = net + tax;
            shown = CloseRate(net, tax, rate);
        }
        else if (inclusive && rate is > 0)
        {
            gross = netOrGross;
            var split = StayTax.Split(gross, rate, true);
            net = split.Net;
            tax = split.Tax;
            shown = RateText(rate.Value);
        }
        else
        {
            gross = netOrGross;
            net = netOrGross;
            tax = 0m;
            shown = "—";
        }

        var key = (label, shown);
        buckets.TryGetValue(key, out var current);
        buckets[key] = (current.Net + net, current.Tax + tax, current.Gross + gross);
    }

    private static void AddCity(Dictionary<(string Label, string Rate), (decimal Net, decimal Tax, decimal Gross)> buckets, decimal amount)
    {
        var key = ("City tax / Ortstaxe", "—");
        buckets.TryGetValue(key, out var current);
        buckets[key] = (current.Net + amount, current.Tax, current.Gross + amount);
    }

    private static string CloseRate(decimal net, decimal tax, decimal? expected)
    {
        if (net == 0)
            return "—";
        var computed = StayTax.Round(tax / net * 100m);
        if (expected is > 0 && Math.Abs(computed - expected.Value) < 0.06m)
            return RateText(expected.Value);
        return RateText(computed);
    }

    private static string RateText(decimal rate)
        => rate.ToString("0.##", CultureInfo.InvariantCulture) + "%";

    private static CostContextRow Cost(string description, decimal amount)
        => new()
        {
            Description = description,
            Amount = Money(amount),
            TariffId = string.Empty
        };

    private static string Money(decimal amount)
        => amount.ToString("F2", CultureInfo.InvariantCulture);

    private static decimal SumAccount(IEnumerable<LedgerEntry> lines, string account)
        => lines.Where(line => line.AccountCode == account).Sum(line => line.Credit - line.Debit);

    private static List<PaymentContextRow> MovementRows(IEnumerable<LedgerEntry> lines)
    {
        var rows = new List<PaymentContextRow>();
        foreach (var group in lines.GroupBy(JournalKey))
        {
            var ordered = group.ToList();
            var kind = ordered.Select(line => line.EntryType).FirstOrDefault(IsCashMovement);
            if (kind is null)
                continue;
            var amount = ordered.Max(line => line.Debit > 0 ? line.Debit : line.Credit);
            var signed = kind.Contains("refund", StringComparison.OrdinalIgnoreCase) ? -amount : amount;
            var described = ordered.FirstOrDefault(line => !string.IsNullOrWhiteSpace(line.Notes)) ?? ordered[0];
            rows.Add(new PaymentContextRow
            {
                Amount = signed.ToString("F2", CultureInfo.InvariantCulture),
                Date = described.Timestamp == default ? string.Empty : described.Timestamp.ToString("yyyy-MM-dd"),
                Method = kind,
                Description = described.Notes ?? kind,
                Type = kind
            });
        }
        return rows;
    }

    private static bool IsCashMovement(string? entryType)
    {
        return string.Equals(entryType, "booking_payment", StringComparison.OrdinalIgnoreCase)
            || string.Equals(entryType, "booking_deposit", StringComparison.OrdinalIgnoreCase)
            || string.Equals(entryType, "booking_prepaid", StringComparison.OrdinalIgnoreCase)
            || string.Equals(entryType, "booking_refund", StringComparison.OrdinalIgnoreCase);
    }

    private static string JournalKey(LedgerEntry line)
        => string.IsNullOrWhiteSpace(line.JournalId) ? line.Id ?? string.Empty : line.JournalId;

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
                "Taxes" => ExpandList(body, ctx.Taxes, ExpandTaxTokens),
                "TaxLines" => ExpandList(body, ctx.TaxLines, ExpandTaxLineTokens),
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
            .Replace("{{Booking.Reference}}", Encode(ctx.BookingReference))
            .Replace("{{Booking.QrCode}}", ctx.BookingQrCode)
            .Replace("{{Invoice.Number}}", Encode(ctx.InvoiceNumber))
            .Replace("{{Invoice.QrCode}}", ctx.InvoiceQrCode)
            .Replace("{{Payment.QrCode}}", ctx.PaymentQrCode)
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
            .Replace("{{Invoice.PrepaidAmount}}", Encode(ctx.InvoicePrepaidAmount))
            .Replace("{{Invoice.PaymentsTotal}}", Encode(ctx.InvoicePaymentsTotal))
            .Replace("{{Invoice.RefundsTotal}}", Encode(ctx.InvoiceRefundsTotal))
            .Replace("{{Invoice.DepositsHeld}}", Encode(ctx.InvoiceDepositsHeld))
            .Replace("{{Invoice.TotalDue}}", Encode(ctx.InvoiceTotalDue))
            .Replace("{{Room.Name}}", Encode(ctx.RoomName))
            .Replace("{{Room.Floor}}", Encode(ctx.RoomFloor))
            .Replace("{{Room.HouseNumber}}", Encode(ctx.RoomHouseNumber))
            .Replace("{{Room.Capacity}}", ctx.RoomCapacity.ToString())
            // Financial
            .Replace("{{Invoice.TotalAmount}}", Encode(ctx.InvoiceTotalAmount))
            .Replace("{{Invoice.Currency}}", Encode(ctx.InvoiceCurrency))
            .Replace("{{Tax.Notice}}", Encode(ctx.TaxNotice))
            .Replace("{{Tax.NoticeDe}}", Encode(ctx.TaxNoticeDe))
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

    private static string ExpandTaxTokens(string body, TaxContextRow row) =>
        body
            .Replace("{{Tax.Label}}", Encode(row.Label))
            .Replace("{{Tax.Rate}}", Encode(row.Rate))
            .Replace("{{Tax.Net}}", Encode(row.Net))
            .Replace("{{Tax.Tax}}", Encode(row.Tax))
            .Replace("{{Tax.Gross}}", Encode(row.Gross));

    private static string ExpandTaxLineTokens(string body, TaxLineContextRow row) =>
        body
            .Replace("{{TaxLine.Label}}", Encode(row.Label))
            .Replace("{{TaxLine.Amount}}", Encode(row.Amount));

    private static string ExpandPaymentTokens(string body, PaymentContextRow p) =>
        body
            .Replace("{{Payment.Amount}}", Encode(p.Amount))
            .Replace("{{Payment.Date}}", Encode(p.Date))
            .Replace("{{Payment.Method}}", Encode(p.Method))
            .Replace("{{Payment.Description}}", Encode(p.Description))
            .Replace("{{Payment.Type}}", Encode(p.Type));

    private static string BuildBookingReference(DateOnly? arrival, string? roomName)
    {
        var date = arrival?.ToString("yyyy-MM-dd") ?? string.Empty;
        var room = string.IsNullOrWhiteSpace(roomName) ? string.Empty : roomName.Trim();
        if (date.Length == 0) return room;
        if (room.Length == 0) return date;
        return $"{date} / {room}";
    }

    private static string BuildBookingQrCode(string bookingId, int year, DateOnly? arrival, string? roomName)
    {
        var payload = string.Join('|',
            "hoteldruid:booking",
            bookingId,
            year.ToString(CultureInfo.InvariantCulture),
            arrival?.ToString("yyyy-MM-dd") ?? string.Empty,
            roomName?.Trim() ?? string.Empty);
        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.M);
        var png = new PngByteQRCode(data).GetGraphic(4);
        return "data:image/png;base64," + Convert.ToBase64String(png);
    }

    private static string BuildQr(string payload)
    {
        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.M);
        var png = new PngByteQRCode(data).GetGraphic(4);
        return "data:image/png;base64," + Convert.ToBase64String(png);
    }

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
        if (ctx.HotelEmail.Length > 0)
            sb.AppendLine($"Reply-To: {ctx.HotelName} <{fromAddress}>");
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
            Email = GetPrimaryEmail(c),
            Phone = GetPrimaryPhone(c),
            Nationality = c?.Nationality ?? string.Empty,
            DateOfBirth = c?.DateOfBirth?.ToString("yyyy-MM-dd") ?? string.Empty
        };

    private static string GetPrimaryPhone(ClientStorageModel? client)
    {
        if (client is null)
            return string.Empty;

        var candidate = client.Phones?.FirstOrDefault(phone => !string.IsNullOrWhiteSpace(phone));
        if (!string.IsNullOrWhiteSpace(candidate))
            return candidate.Trim();

        return client.Phone ?? client.Phone2 ?? client.Phone3 ?? string.Empty;
    }

    private static string GetPrimaryEmail(ClientStorageModel? client)
    {
        if (client is null)
            return string.Empty;

        var candidate = client.Emails?.FirstOrDefault(email => !string.IsNullOrWhiteSpace(email));
        if (!string.IsNullOrWhiteSpace(candidate))
            return candidate.Trim();

        return client.Email ?? client.Email2 ?? client.Email3 ?? string.Empty;
    }

    private static string SafeFilename(string value) =>
        string.IsNullOrWhiteSpace(value)
            ? "document"
            : Regex.Replace(value, @"[^a-zA-Z0-9\-_]", "-");
}
