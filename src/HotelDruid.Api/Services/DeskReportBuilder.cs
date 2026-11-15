using System.Globalization;
using HotelDruid.Api.Models;
using HotelDruid.Shared;
using HotelDruid.Shared.Configuration;

namespace HotelDruid.Api.Services;

/// <summary>
/// Builds the fixed desk and manager sheets. Store ids stay inside; the sheet speaks in room names and guest names.
/// Money still due is the quoted room charge minus the deposit recorded on the stay.
/// </summary>
public static class DeskReportBuilder
{
    public static async Task<PaperReportDto?> BuildAsync(string key, DateOnly date, IKeyValueStore store, ILedgerRepository ledger)
    {
        if (!Known(key))
            return null;

        var config = await FirstAsync<SystemConfiguration>(store, "system_configuration") ?? new SystemConfiguration();
        var settings = config.Settings ?? new Dictionary<string, string>();
        var chart = AccountChart.ResolveOrUsali(config.AccountingStandard, config.AccountCodes);
        var hotel = settings.GetValueOrDefault("HotelName") ?? "Hotel";
        var currency = config.DefaultCurrency ?? settings.GetValueOrDefault("Currency") ?? "EUR";
        var stayRate = ParseRate(settings.GetValueOrDefault("TaxRateStay")) ?? ParseRate(settings.GetValueOrDefault("VatRate"));
        var inclusive = !string.Equals(settings.GetValueOrDefault("TaxPricing"), "Exclusive", StringComparison.OrdinalIgnoreCase);

        if (string.Equals(key, "performance", StringComparison.OrdinalIgnoreCase))
        {
            var performance = await PerformanceReportBuilder.BuildAsync(store, ledger);
            return Performance(hotel, currency, performance);
        }

        var rooms = await RoomsAsync(store);
        var stays = await StaysAsync(store);
        var registers = await RegistersAsync(store);
        var lines = NeedsLines(key)
            ? await LinesAsync(ledger, Range(key, date).From, Range(key, date).To)
            : [];

        var facts = new DeskFacts(hotel, currency, stayRate, chart, rooms, stays, lines, registers, TaxInclusive: inclusive);
        return Build(key, date, facts);
    }

    public static PaperReportDto Build(string key, DateOnly date, DeskFacts facts)
    {
        var (from, to) = Range(key, date);
        var sections = key switch
        {
            "arrivals" => new[] { Arrivals(date, facts) },
            "in-house" => new[] { InHouse(date, facts) },
            "departures" => new[] { Departures(date, facts) },
            "room-status" => new[] { RoomStatus(date, facts) },
            "room-board" => new[] { RoomBoard(from, to, facts) },
            "cashier" => new[] { Movements(date, facts), CountSheet() },
            "flash" => new[] { Flash(date, facts), Movements(date, facts) },
            "forecast" => new[] { Forecast(date, facts) },
            "balances" => new[] { Balances(date, facts) },
            "tax" => new[] { Tax(from, to, facts) },
            "statistics" => new[] { Statistics(date, facts) },
            "performance" when facts.Performance is not null => Performance(facts.HotelName, facts.Currency, facts.Performance).Sections.ToArray(),
            _ => []
        };

        return new PaperReportDto(key, facts.HotelName, facts.Currency, from, to, sections);
    }

    public static bool Known(string key) => key is
        "arrivals" or "in-house" or "departures" or "room-status" or "room-board" or "cashier"
        or "flash" or "forecast" or "balances" or "performance" or "tax" or "statistics";

    private static PaperSectionDto Arrivals(DateOnly day, DeskFacts facts)
    {
        var rows = GuestStays(facts)
            .Where(stay => stay.Arrival == day)
            .OrderBy(stay => RoomLabel(facts, stay.RoomId), StringComparer.OrdinalIgnoreCase)
            .Select(stay => Row(RoomLabel(facts, stay.RoomId), stay.Guest, stay.Guests.ToString(CultureInfo.InvariantCulture), Nights(stay).ToString(CultureInfo.InvariantCulture), Money(stay.Quoted), Money(Due(stay)), stay.Notes ?? ""))
            .ToList();
        return Section("reports.section.arrivals",
            ["reports.col.room", "reports.col.guest", "reports.col.guests", "reports.col.nights", "reports.col.rate", "reports.col.due", "reports.col.notes"],
            ["text", "text", "number", "number", "money", "money", "text"],
            rows,
            Total("reports.total.rooms", rows.Count.ToString(CultureInfo.InvariantCulture), "number"),
            Total("reports.total.due", Money(GuestStays(facts).Where(stay => stay.Arrival == day).Sum(Due)), "money"));
    }

    private static PaperSectionDto InHouse(DateOnly day, DeskFacts facts)
    {
        var stays = GuestStays(facts).Where(stay => Covers(stay, day)).OrderBy(stay => RoomLabel(facts, stay.RoomId), StringComparer.OrdinalIgnoreCase).ToList();
        var rows = stays.Select(stay => Row(RoomLabel(facts, stay.RoomId), stay.Guest, stay.Guests.ToString(CultureInfo.InvariantCulture), stay.Departure?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? "", Money(Due(stay)), stay.Notes ?? "")).ToList();
        return Section("reports.section.inhouse",
            ["reports.col.room", "reports.col.guest", "reports.col.guests", "reports.col.departure", "reports.col.due", "reports.col.notes"],
            ["text", "text", "number", "date", "money", "text"],
            rows,
            Total("reports.total.rooms", rows.Count.ToString(CultureInfo.InvariantCulture), "number"),
            Total("reports.total.due", Money(stays.Sum(Due)), "money"));
    }

    private static PaperSectionDto Departures(DateOnly day, DeskFacts facts)
    {
        var stays = GuestStays(facts).Where(stay => stay.Departure == day).OrderBy(stay => RoomLabel(facts, stay.RoomId), StringComparer.OrdinalIgnoreCase).ToList();
        var rows = stays.Select(stay => Row(RoomLabel(facts, stay.RoomId), stay.Guest, Money(Due(stay)), Money(stay.Prepaid), "")).ToList();
        return Section("reports.section.departures",
            ["reports.col.room", "reports.col.guest", "reports.col.due", "reports.col.deposit", "reports.col.checked"],
            ["text", "text", "money", "money", "check"],
            rows,
            Total("reports.total.due", Money(stays.Sum(Due)), "money"),
            Total("reports.total.deposit", Money(stays.Sum(stay => stay.Prepaid)), "money"));
    }

    private static PaperSectionDto RoomStatus(DateOnly day, DeskFacts facts)
    {
        var rows = new List<PaperRowDto>();
        foreach (var room in facts.Rooms.OrderBy(room => RoomLabel(facts, room.Id), StringComparer.OrdinalIgnoreCase))
        {
            var here = facts.Stays.Where(stay => string.Equals(stay.RoomId, room.Id, StringComparison.OrdinalIgnoreCase) && Covers(stay, day) && !Cancelled(stay)).ToList();
            var leaving = facts.Stays.Where(stay => string.Equals(stay.RoomId, room.Id, StringComparison.OrdinalIgnoreCase) && stay.Departure == day && !Cancelled(stay) && !Maintenance(stay)).ToList();
            var arriving = GuestStays(facts).Where(stay => string.Equals(stay.RoomId, room.Id, StringComparison.OrdinalIgnoreCase) && stay.Arrival == day).ToList();
            var repair = here.Any(Maintenance);
            var occupied = here.Any(stay => !Maintenance(stay));
            var work = repair ? "reports.work.repair"
                : leaving.Count > 0 && arriving.Count > 0 ? "reports.work.turn"
                : leaving.Count > 0 ? "reports.work.departure"
                : arriving.Count > 0 ? "reports.work.arrival"
                : occupied ? "reports.work.stayover"
                : "reports.work.vacant";
            var condition = repair ? "reports.condition.outoforder"
                : occupied || leaving.Count > 0 ? "reports.condition.occupied"
                : "reports.condition.vacant";
            var guest = string.Join(", ", here.Concat(leaving).Concat(arriving).Select(stay => stay.Guest).Where(name => name.Length > 0).Distinct(StringComparer.OrdinalIgnoreCase));
            rows.Add(Row(RoomLabel(facts, room.Id), condition, guest, work, "", "", "", ""));
        }

        return Section("reports.section.rooms",
            ["reports.col.room", "reports.col.condition", "reports.col.guest", "reports.col.work", "reports.col.clean", "reports.col.repair", "reports.col.ready", "reports.col.checked"],
            ["text", "key", "text", "key", "check", "check", "check", "check"],
            rows,
            Total("reports.total.rooms", rows.Count.ToString(CultureInfo.InvariantCulture), "number"));
    }

    private static PaperSectionDto Movements(DateOnly day, DeskFacts facts)
    {
        var rows = facts.Lines
            .Where(line => line.Day == day && line.AccountCode == facts.Chart.Cash && Movement(line) is not null)
            .OrderBy(line => line.EntryType, StringComparer.OrdinalIgnoreCase)
            .Select(line => Row(Register(facts, line.CashRegisterId), Movement(line)!, line.Notes ?? "", Money(line.Debit - line.Credit)))
            .ToList();
        var taken = facts.Lines.Where(line => line.Day == day && line.AccountCode == facts.Chart.Cash && Movement(line) is not null).Sum(line => line.Debit - line.Credit);
        return Section("reports.section.movements",
            ["reports.col.register", "reports.col.movement", "reports.col.detail", "reports.col.amount"],
            ["text", "key", "text", "money"],
            rows,
            Total("reports.total.posted", Money(taken), "money"));
    }

    private static PaperSectionDto CountSheet() => Section("reports.section.count",
        ["reports.col.item", "reports.col.amount"],
        ["key", "check"],
        [Row("reports.count.opening", ""), Row("reports.count.counted", "")]);

    private static PaperSectionDto Flash(DateOnly day, DeskFacts facts)
    {
        var occupied = GuestStays(facts).Count(stay => Covers(stay, day));
        var closed = facts.Stays.Count(stay => Maintenance(stay) && Covers(stay, day));
        var sellable = Math.Max(0, facts.Rooms.Count - closed);
        var roomRevenue = facts.Lines.Where(line => line.Day == day && line.AccountCode == facts.Chart.RoomRevenue).Sum(line => line.Credit - line.Debit);
        var extras = facts.Lines.Where(line => line.Day == day && line.AccountCode == facts.Chart.OtherIncome).Sum(line => line.Credit - line.Debit);
        var taken = facts.Lines.Where(line => line.Day == day && line.AccountCode == facts.Chart.Cash && Movement(line) is not null).Sum(line => line.Debit - line.Credit);
        var due = GuestStays(facts).Where(stay => stay.Departure == day).Sum(Due);
        return Section("reports.section.flash",
            ["reports.col.item", "reports.col.amount"],
            ["key", "text"],
            [
                Row("reports.col.sold", occupied.ToString(CultureInfo.InvariantCulture)),
                Row("reports.col.occupancy", Percent(Rate(occupied, sellable))),
                Row("reports.col.revenue", WithCurrency(facts, roomRevenue)),
                Row("reports.col.other", WithCurrency(facts, extras)),
                Row("reports.total.taken", WithCurrency(facts, taken)),
                Row("reports.total.due", WithCurrency(facts, due))
            ]);
    }

    private static PaperSectionDto Forecast(DateOnly start, DeskFacts facts)
    {
        var rows = new List<PaperRowDto>();
        for (var day = start; day < start.AddDays(14); day = day.AddDays(1))
        {
            var occupied = GuestStays(facts).Where(stay => Covers(stay, day)).ToList();
            var closed = facts.Stays.Count(stay => Maintenance(stay) && Covers(stay, day));
            var sellable = Math.Max(0, facts.Rooms.Count - closed);
            var arrivals = GuestStays(facts).Count(stay => stay.Arrival == day);
            var departures = GuestStays(facts).Count(stay => stay.Departure == day);
            var revenue = occupied.Sum(stay => NightShare(stay));
            rows.Add(Row(
                day.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                arrivals.ToString(CultureInfo.InvariantCulture),
                departures.ToString(CultureInfo.InvariantCulture),
                occupied.Count.ToString(CultureInfo.InvariantCulture),
                Percent(Rate(occupied.Count, sellable)),
                Money(revenue)));
        }

        return Section("reports.section.forecast",
            ["reports.col.date", "reports.col.arrivals", "reports.col.departures", "reports.col.occupied", "reports.col.occupancy", "reports.col.revenue"],
            ["date", "number", "number", "number", "text", "money"],
            rows);
    }

    private static PaperSectionDto Balances(DateOnly day, DeskFacts facts)
    {
        var until = day.AddDays(14);
        var stays = GuestStays(facts)
            .Where(stay => stay.Departure >= day && (Covers(stay, day) || stay.Arrival == day || stay.Arrival <= until) && (Due(stay) != 0 || stay.Prepaid != 0))
            .OrderBy(stay => stay.Arrival)
            .ThenBy(stay => RoomLabel(facts, stay.RoomId), StringComparer.OrdinalIgnoreCase)
            .ToList();
        var rows = stays.Select(stay => Row(
            RoomLabel(facts, stay.RoomId),
            stay.Guest,
            stay.Arrival?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? "",
            stay.Departure?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? "",
            Money(stay.Quoted),
            Money(stay.Prepaid),
            Money(Due(stay)))).ToList();
        return Section("reports.section.balances",
            ["reports.col.room", "reports.col.guest", "reports.col.date", "reports.col.departure", "reports.col.rate", "reports.col.deposit", "reports.col.due"],
            ["text", "text", "date", "date", "money", "money", "money"],
            rows,
            Total("reports.total.deposit", Money(stays.Sum(stay => stay.Prepaid)), "money"),
            Total("reports.total.due", Money(stays.Sum(Due)), "money"));
    }

    private static PaperSectionDto Tax(DateOnly from, DateOnly to, DeskFacts facts)
    {
        var grouped = facts.Lines
            .Where(line => line.Day >= from && line.Day <= to && (line.AccountCode == facts.Chart.RoomRevenue || line.AccountCode == facts.Chart.OtherIncome))
            .GroupBy(line => (line.Day, Rooms: line.AccountCode == facts.Chart.RoomRevenue))
            .Select(group => (group.Key.Day, group.Key.Rooms, Amount: group.Sum(line => line.Credit - line.Debit)))
            .Where(row => row.Amount != 0)
            .OrderBy(row => row.Day)
            .ToList();
        var rows = grouped.Select(row => Row(
            row.Day.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            row.Rooms ? "reports.account.rooms" : "reports.account.other",
            Money(row.Amount))).ToList();
        var postedJournals = facts.Lines
            .Where(line => line.AccountCode == facts.Chart.OutputTax && line.Credit != line.Debit)
            .Select(line => line.JournalId ?? string.Empty)
            .ToHashSet(StringComparer.Ordinal);
        var basis = facts.Lines
            .Where(line => line.Day >= from && line.Day <= to
                && (line.AccountCode == facts.Chart.RoomRevenue || line.AccountCode == facts.Chart.OtherIncome)
                && !postedJournals.Contains(line.JournalId ?? string.Empty))
            .Sum(line => line.Credit - line.Debit);
        var postedTax = facts.Lines
            .Where(line => line.Day >= from && line.Day <= to && line.AccountCode == facts.Chart.OutputTax)
            .Sum(line => line.Credit - line.Debit);
        var city = facts.Lines
            .Where(line => line.Day >= from && line.Day <= to && line.AccountCode == facts.Chart.CityTax)
            .Sum(line => line.Credit - line.Debit);
        if (city != 0)
            rows.Add(Row(to.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), "reports.account.city", Money(city)));
        var totals = new List<PaperTotalDto> { Total("reports.total.base", Money(grouped.Sum(row => row.Amount) + city), "money") };
        decimal output = postedTax;
        if (facts.VatRate is decimal rate && rate > 0 && basis != 0)
        {
            output += facts.TaxInclusive
                ? decimal.Round(basis * rate / (100m + rate), 2, MidpointRounding.AwayFromZero)
                : decimal.Round(basis * rate / 100m, 2, MidpointRounding.AwayFromZero);
        }
        if (output != 0)
            totals.Add(Total(facts.TaxInclusive ? "reports.total.output" : "reports.total.added", Money(output), "money"));

        return Section("reports.section.tax",
            ["reports.col.date", "reports.col.account", "reports.col.amount"],
            ["date", "key", "money"],
            rows,
            totals.ToArray());
    }

    private static PaperSectionDto Statistics(DateOnly day, DeskFacts facts)
    {
        var rows = GuestStays(facts)
            .Where(stay => stay.Arrival == day)
            .OrderBy(stay => RoomLabel(facts, stay.RoomId), StringComparer.OrdinalIgnoreCase)
            .Select(stay => Row(
                RoomLabel(facts, stay.RoomId),
                stay.Guest,
                stay.Nationality ?? "",
                stay.DocumentType ?? "",
                stay.DocumentNumber ?? "",
                stay.Born?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? "",
                Nights(stay).ToString(CultureInfo.InvariantCulture)))
            .ToList();
        return Section("reports.section.statistics",
            ["reports.col.room", "reports.col.guest", "reports.col.nationality", "reports.col.document", "reports.col.number", "reports.col.born", "reports.col.nights"],
            ["text", "text", "text", "text", "text", "date", "number"],
            rows,
            Total("reports.total.rooms", rows.Count.ToString(CultureInfo.InvariantCulture), "number"));
    }

    private static PaperReportDto Performance(string hotel, string currency, PerformanceReportDto report)
    {
        var summary = Section("reports.section.performance",
            ["reports.col.item", "reports.col.amount"],
            ["key", "text"],
            [
                Row("reports.col.occupancy", Percent(report.Occupancy)),
                Row("reports.col.adr", Money(report.Adr) + " " + currency),
                Row("reports.col.revpar", Money(report.RevPar) + " " + currency),
                Row("reports.col.revenue", Money(report.RoomRevenue) + " " + currency),
                Row("reports.col.other", Money(report.OtherIncome) + " " + currency),
                Row("reports.col.expense", Money(report.OperatingExpense) + " " + currency)
            ]);
        var years = Section("reports.section.years",
            ["reports.col.year", "reports.col.occupancy", "reports.col.adr", "reports.col.revpar", "reports.col.revenue", "reports.col.other", "reports.col.expense"],
            ["number", "text", "money", "money", "money", "money", "money"],
            report.Years.Select(year => Row(
                year.Year.ToString(CultureInfo.InvariantCulture),
                Percent(year.Occupancy),
                Money(year.Adr),
                Money(year.RevPar),
                Money(year.RoomRevenue),
                Money(year.OtherIncome),
                Money(year.OperatingExpense))).ToList());
        var rooms = Section("reports.section.roomsEarned",
            ["reports.col.room", "reports.col.sold", "reports.col.sellable", "reports.col.occupancy", "reports.col.revenue", "reports.col.adr"],
            ["text", "number", "number", "text", "money", "money"],
            report.Rooms.Select(room => Row(
                room.RoomName,
                room.OccupiedNights.ToString(CultureInfo.InvariantCulture),
                room.SellableNights.ToString(CultureInfo.InvariantCulture),
                Percent(room.Occupancy),
                Money(room.RoomRevenue),
                Money(room.Adr))).ToList());
        return new PaperReportDto("performance", hotel, currency, report.From, report.To, [summary, years, rooms]);
    }

    private static PaperSectionDto RoomBoard(DateOnly from, DateOnly to, DeskFacts facts)
    {
        var days = new List<DateOnly>();
        for (var day = from; day <= to; day = day.AddDays(1))
            days.Add(day);

        var columns = new List<string> { "reports.col.room" };
        columns.AddRange(days.Select(day => day.Day.ToString(CultureInfo.InvariantCulture)));
        var kinds = new List<string> { "text" };
        kinds.AddRange(days.Select(_ => "band"));

        var rows = facts.Rooms
            .OrderBy(room => room.House, StringComparer.OrdinalIgnoreCase)
            .ThenBy(room => room.Name, StringComparer.OrdinalIgnoreCase)
            .Select(room =>
            {
                var bands = days.Select(day => Band(room, Covering(facts, room.Id, day))).ToList();
                var cells = new List<string> { RoomLabel(facts, room.Id) };
                var spans = new List<int> { 1 };
                for (var i = 0; i < bands.Count;)
                {
                    var span = 1;
                    while (i + span < bands.Count && string.Equals(bands[i + span], bands[i], StringComparison.Ordinal))
                        span++;
                    cells.Add(bands[i]);
                    spans.Add(span);
                    i += span;
                }

                return new PaperRowDto(cells, spans);
            })
            .ToList();

        return new PaperSectionDto("reports.roomBoard.section", columns, kinds, rows, Array.Empty<PaperTotalDto>());
    }

    private static DeskStay? Covering(DeskFacts facts, string roomId, DateOnly day)
    {
        DeskStay? guest = null;
        DeskStay? block = null;
        foreach (var stay in facts.Stays)
        {
            if (!string.Equals(stay.RoomId, roomId, StringComparison.OrdinalIgnoreCase) || !Covers(stay, day) || Cancelled(stay))
                continue;
            if (Maintenance(stay))
                block ??= stay;
            else
                guest ??= stay;
        }

        return guest ?? block;
    }

    private static string Band(DeskRoom room, DeskStay? stay)
    {
        if (stay is not null && Maintenance(stay))
            return "maintenance";
        if (stay is not null)
            return "occupied|" + stay.Guest + "|" + stay.Id;
        var condition = (room.Condition ?? "").Trim().ToLowerInvariant();
        return condition is "dirty" or "blocked" or "maintenance" ? condition : "available";
    }

    private static (DateOnly From, DateOnly To) Range(string key, DateOnly date) => key switch
    {
        "forecast" => (date, date.AddDays(13)),
        "tax" or "room-board" => (new DateOnly(date.Year, date.Month, 1), new DateOnly(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month))),
        _ => (date, date)
    };

    private static bool NeedsLines(string key) => key is "cashier" or "flash" or "tax";

    private static IEnumerable<DeskStay> GuestStays(DeskFacts facts) => facts.Stays.Where(stay => !Cancelled(stay) && !Maintenance(stay));

    private static bool Covers(DeskStay stay, DateOnly day) => stay.Arrival is not null && stay.Departure is not null && stay.Arrival <= day && stay.Departure > day;

    private static bool Cancelled(DeskStay stay) =>
        stay.Status is not null && (stay.Status.Equals("Cancelled", StringComparison.OrdinalIgnoreCase) || stay.Status.Equals("Void", StringComparison.OrdinalIgnoreCase) || stay.Status.Equals("Deleted", StringComparison.OrdinalIgnoreCase));

    private static bool Maintenance(DeskStay stay) => string.Equals(stay.Status, "Maintenance", StringComparison.OrdinalIgnoreCase);

    private static int Nights(DeskStay stay) => stay.Arrival is null || stay.Departure is null ? 0 : Math.Max(0, stay.Departure.Value.DayNumber - stay.Arrival.Value.DayNumber);

    private static decimal Due(DeskStay stay) => stay.Quoted - stay.Prepaid;

    private static decimal NightShare(DeskStay stay)
    {
        var nights = Nights(stay);
        return nights == 0 ? 0 : decimal.Round(stay.Quoted / nights, 2, MidpointRounding.AwayFromZero);
    }

    private static string? Movement(DeskLine line) => line.EntryType?.ToLowerInvariant() switch
    {
        "booking_payment" => "reports.move.payment",
        "booking_deposit" => "reports.move.deposit",
        "booking_prepaid" => "reports.move.prepaid",
        "booking_refund" => "reports.move.refund",
        "hotel_expense" => "reports.move.expense",
        _ => null
    };

    private static string Register(DeskFacts facts, string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return "reports.register.desk";
        return facts.Registers.TryGetValue(id, out var name) && !string.IsNullOrWhiteSpace(name) ? name : id;
    }

    private static string RoomLabel(DeskFacts facts, string? roomId)
    {
        var room = facts.Rooms.FirstOrDefault(item => string.Equals(item.Id, roomId, StringComparison.OrdinalIgnoreCase));
        if (room is null)
            return roomId ?? "";
        var repeated = facts.Rooms.Count(item => string.Equals(item.Name, room.Name, StringComparison.OrdinalIgnoreCase)) > 1;
        return repeated && !string.IsNullOrWhiteSpace(room.House) ? $"{room.House}/{room.Name}" : room.Name;
    }

    private static decimal Rate(int occupied, int sellable) => sellable == 0 ? 0 : decimal.Round((decimal)occupied / sellable, 4, MidpointRounding.AwayFromZero);

    private static string Money(decimal amount) => amount.ToString("0.00", CultureInfo.InvariantCulture);

    private static string WithCurrency(DeskFacts facts, decimal amount) => Money(amount) + " " + facts.Currency;

    private static string Percent(decimal rate) => (rate * 100m).ToString("0.0", CultureInfo.InvariantCulture) + "%";

    private static decimal? ParseRate(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return null;
        var cleaned = text.Trim().TrimEnd('%');
        return decimal.TryParse(cleaned, NumberStyles.Number, CultureInfo.InvariantCulture, out var rate) ? rate : null;
    }

    private static PaperSectionDto Section(string heading, string[] columns, string[] kinds, List<PaperRowDto> rows, params PaperTotalDto[] totals)
        => new(heading, columns, kinds, rows, totals);

    private static PaperRowDto Row(params string[] cells) => new(cells);

    private static PaperTotalDto Total(string label, string value, string kind) => new(label, value, kind);

    private static async Task<List<DeskRoom>> RoomsAsync(IKeyValueStore store)
    {
        var index = await store.GetIndexAsync("rooms");
        var rooms = new List<DeskRoom>();
        foreach (var (name, id) in index)
        {
            var room = await store.GetAsync<RoomStorageModel>("rooms", id);
            rooms.Add(new DeskRoom(id, room?.Name ?? name, room?.HouseNumber, room?.FloorNumber, room?.Condition));
        }

        return rooms;
    }

    private static async Task<List<DeskStay>> StaysAsync(IKeyValueStore store)
    {
        var index = await store.GetIndexAsync("bookings");
        var pending = new List<(string Id, BookingStorageModel Booking)>();
        foreach (var (_, id) in index)
        {
            var booking = await store.GetAsync<BookingStorageModel>("bookings", id);
            if (booking is null || booking.DeletedAt.HasValue || !booking.ArrivalDate.HasValue || !booking.DepartureDate.HasValue)
                continue;
            pending.Add((id, booking));
        }

        var guests = new Dictionary<string, ClientStorageModel>(StringComparer.OrdinalIgnoreCase);
        foreach (var clientId in pending.Select(item => item.Booking.ClientId).Where(id => !string.IsNullOrWhiteSpace(id)).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            var client = await store.GetAsync<ClientStorageModel>("clients", clientId!);
            if (client is not null)
                guests[clientId!] = client;
        }

        return pending.Select(item =>
        {
            guests.TryGetValue(item.Booking.ClientId ?? "", out var client);
            var guest = client is null
                ? item.Booking.ClientId ?? ""
                : string.Join(", ", new[] { client.LastName, client.FirstName }.Where(part => !string.IsNullOrWhiteSpace(part)));
            return new DeskStay(
                item.Id,
                item.Booking.RoomId,
                item.Booking.ClientId,
                guest,
                item.Booking.ArrivalDate,
                item.Booking.DepartureDate,
                item.Booking.Status,
                item.Booking.ActualGuestCount ?? item.Booking.ExpectedGuestCount ?? 1,
                Convert.ToDecimal(item.Booking.TariffTotalPrice ?? 0),
                Convert.ToDecimal(item.Booking.PrepaidAmount ?? 0),
                item.Booking.Notes,
                client?.Nationality ?? client?.Nation,
                client?.DocumentType,
                client?.DocumentNumber,
                client?.DateOfBirth);
        }).ToList();
    }

    private static async Task<Dictionary<string, string>> RegistersAsync(IKeyValueStore store)
    {
        var index = await store.GetIndexAsync("cash_registers");
        var names = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var (name, id) in index)
        {
            var register = await store.GetAsync<CashRegisterStorageModel>("cash_registers", id);
            names[id] = register?.Name ?? name;
        }

        return names;
    }

    private static async Task<List<DeskLine>> LinesAsync(ILedgerRepository ledger, DateOnly from, DateOnly to)
    {
        var lines = new List<DeskLine>();
        for (var day = from; day <= to; day = day.AddDays(1))
        {
            foreach (var entry in await ledger.GetConsolidatedAsync(day.ToDateTime(TimeOnly.MinValue)))
            {
                lines.Add(new DeskLine(
                    day,
                    entry.EntryType,
                    entry.AccountCode,
                    entry.AccountGroup,
                    entry.Debit,
                    entry.Credit,
                    entry.CashRegisterId,
                    entry.Method,
                    entry.Notes,
                    entry.BookingId,
                    entry.JournalId));
            }
        }

        return lines;
    }

    private static async Task<T?> FirstAsync<T>(IKeyValueStore store, string collection) where T : class
    {
        var index = await store.GetIndexAsync(collection);
        foreach (var (_, id) in index)
        {
            var item = await store.GetAsync<T>(collection, id);
            if (item is not null)
                return item;
        }

        return null;
    }
}

public sealed record DeskFacts(
    string HotelName,
    string Currency,
    decimal? VatRate,
    AccountChart Chart,
    IReadOnlyList<DeskRoom> Rooms,
    IReadOnlyList<DeskStay> Stays,
    IReadOnlyList<DeskLine> Lines,
    IReadOnlyDictionary<string, string> Registers,
    PerformanceReportDto? Performance = null,
    bool TaxInclusive = true);

public sealed record DeskRoom(string Id, string Name, string? House, string? Floor, string? Condition = null);

public sealed record DeskStay(
    string Id,
    string? RoomId,
    string? ClientId,
    string Guest,
    DateOnly? Arrival,
    DateOnly? Departure,
    string? Status,
    int Guests,
    decimal Quoted,
    decimal Prepaid,
    string? Notes,
    string? Nationality,
    string? DocumentType,
    string? DocumentNumber,
    DateTime? Born);

public sealed record DeskLine(
    DateOnly Day,
    string? EntryType,
    string? AccountCode,
    string? AccountGroup,
    decimal Debit,
    decimal Credit,
    string? CashRegisterId,
    string? Method,
    string? Notes,
    string? BookingId,
    string? JournalId = null);
