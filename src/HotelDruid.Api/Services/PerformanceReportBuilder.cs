using System.Diagnostics;
using System.Globalization;
using HotelDruid.Api.Models;
using HotelDruid.Shared;
using HotelDruid.Shared.Configuration;

namespace HotelDruid.Api.Services;

public static class PerformanceReportBuilder
{
    public static async Task<PerformanceReportDto> BuildAsync(IKeyValueStore store, ILedgerRepository ledger)
    {
        var watch = Stopwatch.StartNew();
        var config = await FirstAsync<SystemConfiguration>(store, "system_configuration");
        var settings = config?.Settings ?? new Dictionary<string, string>();
        var rooms = await RoomsAsync(store);
        var bookings = await BookingsAsync(store);
        var lines = await ledger.ReadAllAsync();

        var arrivals = bookings.Where(b => b.Arrival.HasValue).Select(b => b.Arrival!.Value).ToList();
        var departures = bookings.Where(b => b.Departure.HasValue).Select(b => b.Departure!.Value).ToList();
        var from = ParseDate(settings.GetValueOrDefault("ScenarioStart"))
            ?? (arrivals.Count == 0 ? DateOnly.FromDateTime(DateTime.UtcNow) : arrivals.Min());
        var to = ParseDate(settings.GetValueOrDefault("ScenarioEnd"))
            ?? (departures.Count == 0 ? from.AddDays(1) : departures.Max());
        if (to <= from)
            to = from.AddDays(1);

        var days = to.DayNumber - from.DayNumber;
        var occupied = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var blocked = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var revenueByRoom = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
        var occupiedByYear = new Dictionary<int, int>();
        var blockedByYear = new Dictionary<int, int>();
        var nights = new List<NightPoint>();
        var roomRevenueLines = new List<RevenuePoint>();
        var guestBookings = 0;

        foreach (var room in rooms)
        {
            occupied[room.Id] = 0;
            blocked[room.Id] = 0;
            revenueByRoom[room.Id] = 0;
        }

        foreach (var booking in bookings)
        {
            if (!booking.Arrival.HasValue || !booking.Departure.HasValue || string.IsNullOrWhiteSpace(booking.RoomId))
                continue;
            var maintenance = string.Equals(booking.Status, "Maintenance", StringComparison.OrdinalIgnoreCase);
            if (!maintenance)
                guestBookings++;
            for (var day = booking.Arrival.Value; day < booking.Departure.Value; day = day.AddDays(1))
            {
                if (day < from || day >= to)
                    continue;
                if (maintenance)
                {
                    blocked[booking.RoomId] = blocked.GetValueOrDefault(booking.RoomId) + 1;
                    blockedByYear[day.Year] = blockedByYear.GetValueOrDefault(day.Year) + 1;
                }
                else
                {
                    occupied[booking.RoomId] = occupied.GetValueOrDefault(booking.RoomId) + 1;
                    occupiedByYear[day.Year] = occupiedByYear.GetValueOrDefault(day.Year) + 1;
                }

                nights.Add(new NightPoint(booking.RoomId, day, maintenance));
            }
        }

        var roomRevenueByYear = new Dictionary<int, decimal>();
        var otherByYear = new Dictionary<int, decimal>();
        var expenseByYear = new Dictionary<int, decimal>();
        decimal roomRevenue = 0, otherIncome = 0, expenses = 0;
        var bookingRoom = bookings
            .Where(b => !string.IsNullOrWhiteSpace(b.Id))
            .ToDictionary(b => b.Id!, b => b.RoomId, StringComparer.OrdinalIgnoreCase);

        foreach (var line in lines)
        {
            var year = line.Timestamp.Year;
            if (IsRevenue(line, "booking_stay_charge"))
            {
                var amount = line.Credit - line.Debit;
                roomRevenue += amount;
                roomRevenueByYear[year] = roomRevenueByYear.GetValueOrDefault(year) + amount;
                if (!string.IsNullOrWhiteSpace(line.BookingId) && bookingRoom.TryGetValue(line.BookingId, out var roomId) && !string.IsNullOrWhiteSpace(roomId))
                {
                    revenueByRoom[roomId] = revenueByRoom.GetValueOrDefault(roomId) + amount;
                    roomRevenueLines.Add(new RevenuePoint(roomId, DateOnly.FromDateTime(line.Timestamp), amount));
                }
            }
            else if (IsRevenue(line, "booking_extra_charge"))
            {
                var amount = line.Credit - line.Debit;
                otherIncome += amount;
                otherByYear[year] = otherByYear.GetValueOrDefault(year) + amount;
            }
            else if (string.Equals(line.EntryType, "hotel_expense", StringComparison.OrdinalIgnoreCase)
                && string.Equals(line.AccountGroup, "expense", StringComparison.OrdinalIgnoreCase))
            {
                var amount = line.Debit - line.Credit;
                expenses += amount;
                expenseByYear[year] = expenseByYear.GetValueOrDefault(year) + amount;
            }
        }

        var maintenanceNights = blocked.Values.Sum();
        var occupiedNights = occupied.Values.Sum();
        var sellable = rooms.Count * days - maintenanceNights;
        var years = new List<PerformanceYearDto>();
        for (var year = from.Year; year <= to.AddDays(-1).Year; year++)
        {
            var yearStart = year == from.Year ? from : new DateOnly(year, 1, 1);
            var yearEnd = year == to.Year ? to : new DateOnly(year + 1, 1, 1);
            var yearDays = Math.Max(0, yearEnd.DayNumber - yearStart.DayNumber);
            var yearBlocked = blockedByYear.GetValueOrDefault(year);
            var yearSellable = rooms.Count * yearDays - yearBlocked;
            var yearOccupied = occupiedByYear.GetValueOrDefault(year);
            var yearRoom = roomRevenueByYear.GetValueOrDefault(year);
            years.Add(new PerformanceYearDto(
                year,
                yearRoom,
                otherByYear.GetValueOrDefault(year),
                expenseByYear.GetValueOrDefault(year),
                yearOccupied,
                yearSellable,
                yearBlocked,
                Rate(yearOccupied, yearSellable),
                Rate(yearRoom, yearOccupied),
                Rate(yearRoom, yearSellable)));
        }

        var roomRows = rooms
            .Select(room =>
            {
                var sold = occupied.GetValueOrDefault(room.Id);
                var closed = blocked.GetValueOrDefault(room.Id);
                var available = days - closed;
                var earned = revenueByRoom.GetValueOrDefault(room.Id);
                return new RoomProductivityDto(room.Name, sold, available, closed, earned, Rate(sold, available), Rate(earned, sold));
            })
            .OrderBy(room => room.RoomName, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var roomYears = BuildRoomYears(rooms, from, to, nights, roomRevenueLines);
        var actions = BuildActions(rooms, from, to, years, roomYears, nights, roomRevenueLines);

        watch.Stop();
        return new PerformanceReportDto(
            settings.GetValueOrDefault("HotelName") ?? "Hotel",
            from,
            to,
            rooms.Count,
            Rate(occupiedNights, sellable),
            roomRevenue,
            otherIncome,
            expenses,
            occupiedNights,
            sellable,
            maintenanceNights,
            guestBookings,
            Rate(roomRevenue, occupiedNights),
            Rate(roomRevenue, sellable),
            watch.ElapsedMilliseconds,
            years,
            roomRows,
            roomYears,
            actions);
    }

    private static List<RoomYearDto> BuildRoomYears(
        List<RoomInfo> rooms,
        DateOnly from,
        DateOnly to,
        List<NightPoint> nights,
        List<RevenuePoint> revenue)
    {
        var rows = new List<RoomYearDto>();
        var lastYear = to.AddDays(-1).Year;
        for (var year = from.Year; year <= lastYear; year++)
        {
            var (start, end) = YearBounds(year, from, to);
            var priorStart = start.AddYears(-1);
            var priorEnd = end.AddYears(-1);
            var comparable = priorStart >= from && priorEnd <= to;
            foreach (var room in rooms.OrderBy(room => room.Name, StringComparer.OrdinalIgnoreCase))
            {
                var current = RoomWindow(room.Id, start, end, nights, revenue);
                decimal? occupancyChange = null;
                decimal? adrChange = null;
                decimal? revParChange = null;
                if (comparable)
                {
                    var prior = RoomWindow(room.Id, priorStart, priorEnd, nights, revenue);
                    occupancyChange = current.Occupancy - prior.Occupancy;
                    adrChange = current.Adr - prior.Adr;
                    revParChange = current.RevPar - prior.RevPar;
                }

                rows.Add(new RoomYearDto(
                    room.Name,
                    year,
                    current.Occupied,
                    current.Sellable,
                    current.Closed,
                    current.Revenue,
                    current.Occupancy,
                    current.Adr,
                    current.RevPar,
                    occupancyChange,
                    adrChange,
                    revParChange));
            }
        }

        return rows;
    }

    private static List<PerformanceActionDto> BuildActions(
        List<RoomInfo> rooms,
        DateOnly from,
        DateOnly to,
        List<PerformanceYearDto> years,
        List<RoomYearDto> roomYears,
        List<NightPoint> nights,
        List<RevenuePoint> revenue)
    {
        var actions = new List<PerformanceActionDto>();
        if (years.Count == 0 || rooms.Count == 0)
            return actions;

        var focus = years[^1];
        var (start, end) = YearBounds(focus.Year, from, to);
        var priorStart = start.AddYears(-1);
        var priorEnd = end.AddYears(-1);
        var comparable = priorStart >= from && priorEnd <= to;
        if (comparable)
        {
            var current = HotelWindow(start, end, rooms.Count, nights, revenue);
            var prior = HotelWindow(priorStart, priorEnd, rooms.Count, nights, revenue);
            var occupancyUp = current.Occupancy >= prior.Occupancy + 0.005m;
            var occupancyDown = current.Occupancy <= prior.Occupancy - 0.005m;
            var rateUp = current.Adr >= prior.Adr + 0.50m;
            var rateDown = current.Adr <= prior.Adr - 0.50m;
            var code = occupancyUp && rateUp ? "pace-ahead"
                : occupancyDown && rateDown ? "pace-behind"
                : occupancyUp && rateDown ? "pace-volume"
                : rateUp ? "pace-rate"
                : "pace-flat";
            actions.Add(new PerformanceActionDto(
                code, null, focus.Year, focus.Year - 1,
                current.Occupancy, current.Adr, prior.Occupancy, prior.Adr, null));
        }

        foreach (var room in roomYears
            .Where(room => room.Year == focus.Year && room.Occupancy >= focus.Occupancy + 0.03m && focus.Adr > 0 && room.Adr <= focus.Adr * 0.92m)
            .OrderBy(room => room.Adr)
            .Take(2))
        {
            actions.Add(new PerformanceActionDto(
                "raise-rate", room.RoomName, focus.Year, 0,
                room.Occupancy, room.Adr, focus.Occupancy, focus.Adr, null));
        }

        foreach (var room in roomYears
            .Where(room => room.Year == focus.Year && room.OccupancyChange is <= -0.03m && room.RevParChange is < 0)
            .OrderBy(room => room.RevParChange)
            .Take(2))
        {
            actions.Add(new PerformanceActionDto(
                "lost-demand", room.RoomName, focus.Year, focus.Year - 1,
                room.OccupancyChange ?? 0, room.Adr, room.RevParChange ?? 0, 0, null));
        }

        AddWeekday(actions, focus, start, end, rooms.Count, nights);
        AddWorks(actions, focus, start, end, rooms.Count, nights);
        AddCapacity(actions, rooms, focus.Year, start, end, nights, revenue);
        return actions;
    }

    private static void AddWeekday(
        List<PerformanceActionDto> actions,
        PerformanceYearDto focus,
        DateOnly start,
        DateOnly end,
        int roomCount,
        List<NightPoint> nights)
    {
        var occupied = new int[7];
        var sellable = new int[7];
        var closedByDay = new Dictionary<DateOnly, int>();
        foreach (var night in nights)
        {
            if (night.Day < start || night.Day >= end)
                continue;
            if (night.Closed)
                closedByDay[night.Day] = closedByDay.GetValueOrDefault(night.Day) + 1;
            else
                occupied[(int)night.Day.DayOfWeek]++;
        }

        for (var day = start; day < end; day = day.AddDays(1))
            sellable[(int)day.DayOfWeek] += roomCount - closedByDay.GetValueOrDefault(day);

        var worst = -1;
        var worstOccupancy = 1m;
        for (var index = 0; index < 7; index++)
        {
            if (sellable[index] < roomCount * 4)
                continue;
            var occupancy = Rate(occupied[index], sellable[index]);
            if (occupancy < worstOccupancy)
            {
                worstOccupancy = occupancy;
                worst = index;
            }
        }

        if (worst >= 0 && focus.Occupancy - worstOccupancy >= 0.08m)
        {
            actions.Add(new PerformanceActionDto(
                "weekday", null, focus.Year, 0,
                worstOccupancy, 0, focus.Occupancy, 0, worst.ToString(CultureInfo.InvariantCulture)));
        }
    }

    private static void AddWorks(
        List<PerformanceActionDto> actions,
        PerformanceYearDto focus,
        DateOnly start,
        DateOnly end,
        int roomCount,
        List<NightPoint> nights)
    {
        var occupied = new int[13];
        var closed = new int[13];
        var days = new int[13];
        for (var day = start; day < end; day = day.AddDays(1))
            days[day.Month]++;

        foreach (var night in nights)
        {
            if (night.Day < start || night.Day >= end)
                continue;
            if (night.Closed)
                closed[night.Day.Month]++;
            else
                occupied[night.Day.Month]++;
        }

        var busiestClosure = 0;
        var closedNights = 0;
        for (var month = 1; month <= 12; month++)
        {
            if (closed[month] > closedNights)
            {
                closedNights = closed[month];
                busiestClosure = month;
            }
        }

        if (busiestClosure == 0 || closedNights < 40)
            return;

        var monthSellable = roomCount * days[busiestClosure] - closed[busiestClosure];
        var monthOccupancy = Rate(occupied[busiestClosure], monthSellable);
        var code = monthOccupancy <= focus.Occupancy - 0.05m ? "works-quiet"
            : monthOccupancy >= focus.Occupancy + 0.02m ? "works-busy"
            : null;
        if (code is null)
            return;

        actions.Add(new PerformanceActionDto(
            code, null, focus.Year, 0,
            monthOccupancy, 0, focus.Occupancy, 0, busiestClosure.ToString(CultureInfo.InvariantCulture)));
    }

    private static void AddCapacity(
        List<PerformanceActionDto> actions,
        List<RoomInfo> rooms,
        int year,
        DateOnly start,
        DateOnly end,
        List<NightPoint> nights,
        List<RevenuePoint> revenue)
    {
        var doubles = rooms.Where(room => room.Capacity == 2).ToList();
        var larger = rooms.Where(room => room.Capacity >= 3).GroupBy(room => room.Capacity).OrderByDescending(group => group.Key).FirstOrDefault();
        if (doubles.Count == 0 || larger is null || larger.Count() == 0)
            return;

        var small = GroupWindow(doubles, start, end, nights, revenue);
        var family = GroupWindow(larger.ToList(), start, end, nights, revenue);
        if (small.Adr <= 0 || family.Adr < small.Adr * 1.15m)
            return;

        var code = family.Occupancy <= small.Occupancy - 0.08m ? "premium-high" : "protect-premium";
        if (code == "protect-premium" && Math.Abs(family.Occupancy - small.Occupancy) > 0.05m)
            return;

        actions.Add(new PerformanceActionDto(
            code, null, year, 0,
            family.Adr, family.Occupancy, small.Adr, small.Occupancy,
            larger.Key.ToString(CultureInfo.InvariantCulture)));
    }

    private static (DateOnly Start, DateOnly End) YearBounds(int year, DateOnly from, DateOnly to)
    {
        var start = year == from.Year ? from : new DateOnly(year, 1, 1);
        var end = year == to.Year ? to : new DateOnly(year + 1, 1, 1);
        return (start, end);
    }

    private static WindowStats RoomWindow(
        string roomId,
        DateOnly start,
        DateOnly end,
        List<NightPoint> nights,
        List<RevenuePoint> revenue)
    {
        var occupied = 0;
        var closed = 0;
        foreach (var night in nights)
        {
            if (!string.Equals(night.RoomId, roomId, StringComparison.OrdinalIgnoreCase) || night.Day < start || night.Day >= end)
                continue;
            if (night.Closed)
                closed++;
            else
                occupied++;
        }

        decimal money = 0;
        foreach (var line in revenue)
        {
            if (string.Equals(line.RoomId, roomId, StringComparison.OrdinalIgnoreCase) && line.Day >= start && line.Day < end)
                money += line.Amount;
        }

        var days = Math.Max(0, end.DayNumber - start.DayNumber);
        var sellable = days - closed;
        return new WindowStats(occupied, sellable, closed, money, Rate(occupied, sellable), Rate(money, occupied), Rate(money, sellable));
    }

    private static WindowStats HotelWindow(
        DateOnly start,
        DateOnly end,
        int roomCount,
        List<NightPoint> nights,
        List<RevenuePoint> revenue)
    {
        var occupied = 0;
        var closed = 0;
        foreach (var night in nights)
        {
            if (night.Day < start || night.Day >= end)
                continue;
            if (night.Closed)
                closed++;
            else
                occupied++;
        }

        decimal money = 0;
        foreach (var line in revenue)
        {
            if (line.Day >= start && line.Day < end)
                money += line.Amount;
        }

        var sellable = roomCount * Math.Max(0, end.DayNumber - start.DayNumber) - closed;
        return new WindowStats(occupied, sellable, closed, money, Rate(occupied, sellable), Rate(money, occupied), Rate(money, sellable));
    }

    private static WindowStats GroupWindow(
        List<RoomInfo> group,
        DateOnly start,
        DateOnly end,
        List<NightPoint> nights,
        List<RevenuePoint> revenue)
    {
        var ids = new HashSet<string>(group.Select(room => room.Id), StringComparer.OrdinalIgnoreCase);
        var occupied = 0;
        var closed = 0;
        foreach (var night in nights)
        {
            if (!ids.Contains(night.RoomId) || night.Day < start || night.Day >= end)
                continue;
            if (night.Closed)
                closed++;
            else
                occupied++;
        }

        decimal money = 0;
        foreach (var line in revenue)
        {
            if (ids.Contains(line.RoomId) && line.Day >= start && line.Day < end)
                money += line.Amount;
        }

        var sellable = group.Count * Math.Max(0, end.DayNumber - start.DayNumber) - closed;
        return new WindowStats(occupied, sellable, closed, money, Rate(occupied, sellable), Rate(money, occupied), Rate(money, sellable));
    }

    private static bool IsRevenue(LedgerEntry line, string entryType) =>
        string.Equals(line.EntryType, entryType, StringComparison.OrdinalIgnoreCase)
        && string.Equals(line.AccountGroup, "revenue", StringComparison.OrdinalIgnoreCase);

    private static decimal Rate(decimal numerator, decimal denominator) =>
        denominator == 0 ? 0 : decimal.Round(numerator / denominator, 4, MidpointRounding.AwayFromZero);

    private static decimal Rate(int numerator, int denominator) =>
        denominator == 0 ? 0 : decimal.Round((decimal)numerator / denominator, 4, MidpointRounding.AwayFromZero);

    private static DateOnly? ParseDate(string? text) =>
        DateOnly.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date) ? date : null;

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

    private static async Task<List<RoomInfo>> RoomsAsync(IKeyValueStore store)
    {
        var index = await store.GetIndexAsync("rooms");
        var rooms = new List<RoomInfo>();
        foreach (var (name, id) in index)
        {
            var room = await store.GetAsync<RoomStorageModel>("rooms", id);
            rooms.Add(new RoomInfo(id, room?.Name ?? name, room?.Capacity ?? 0));
        }

        return rooms;
    }

    private static async Task<List<BookingRow>> BookingsAsync(IKeyValueStore store)
    {
        var index = await store.GetIndexAsync("bookings");
        var rows = new List<BookingRow>();
        foreach (var (_, id) in index)
        {
            var booking = await store.GetAsync<BookingStorageModel>("bookings", id);
            if (booking is null || booking.DeletedAt.HasValue)
                continue;
            rows.Add(new BookingRow(id, booking.RoomId, booking.ArrivalDate, booking.DepartureDate, booking.Status));
        }

        return rows;
    }

    private sealed record BookingRow(string Id, string? RoomId, DateOnly? Arrival, DateOnly? Departure, string? Status);
    private sealed record RoomInfo(string Id, string Name, int Capacity);
    private readonly record struct NightPoint(string RoomId, DateOnly Day, bool Closed);
    private readonly record struct RevenuePoint(string RoomId, DateOnly Day, decimal Amount);
    private readonly record struct WindowStats(int Occupied, int Sellable, int Closed, decimal Revenue, decimal Occupancy, decimal Adr, decimal RevPar);
}
