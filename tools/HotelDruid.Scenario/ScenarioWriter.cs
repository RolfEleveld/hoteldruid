using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

public sealed record ScenarioRequest(DateOnly Start, DateOnly End, int Rooms, double Occupancy, int Seed);

public sealed record ScenarioResult(
    string HotelName,
    string DataRoot,
    DateOnly Start,
    DateOnly End,
    int Rooms,
    double TargetOccupancy,
    double Occupancy,
    int GuestBookings,
    int MaintenanceBlocks,
    int CancelledBookings,
    int Guests,
    int BabyCotStays,
    int AdjacentStays,
    decimal RoomRevenue,
    decimal OtherIncome,
    decimal OperatingExpense,
    int OccupiedNights,
    int SellableNights,
    int MaintenanceNights,
    long WriteMilliseconds);

public static class ScenarioWriter
{
    private const string HotelName = "Hotel Belvedere";
    private static readonly string[] Desk = ["Giulia Ferri", "Marco Keller", "Anna Novak", "Sophie Laurent"];
    private static readonly JsonSerializerOptions Json = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static ScenarioResult Write(string dataRoot, ScenarioRequest request)
    {
        if (request.End <= request.Start)
            throw new ArgumentException("End must be after start.");
        if (request.Rooms < 2 || request.Rooms % 2 != 0)
            throw new ArgumentException("Room count must be an even number of at least 2, split across two floors.");

        var started = DateTime.UtcNow;
        Wipe(dataRoot);
        var plan = Calibrate(request);
        var store = new Store(dataRoot);
        Persist(store, request, plan);
        store.Flush();
        var elapsed = (long)(DateTime.UtcNow - started).TotalMilliseconds;
        return new ScenarioResult(
            HotelName,
            Path.GetFullPath(dataRoot),
            request.Start,
            request.End,
            request.Rooms,
            request.Occupancy,
            plan.OccupiedNights == 0 ? 0 : (double)plan.OccupiedNights / plan.SellableNights,
            plan.Stays.Count(s => s.Kind == "guest"),
            plan.Maintenance.Count,
            plan.Cancelled,
            plan.Guests.Count,
            plan.BabyCots,
            plan.AdjacentPairs,
            plan.RoomRevenue,
            plan.OtherIncome,
            plan.Expenses,
            plan.OccupiedNights,
            plan.SellableNights,
            plan.MaintenanceNights,
            elapsed);
    }

    private static void Wipe(string dataRoot)
    {
        Directory.CreateDirectory(dataRoot);
        foreach (var name in new[] { "collections", "ledger", "bookings" })
        {
            var path = Path.Combine(dataRoot, name);
            if (Directory.Exists(path))
                Directory.Delete(path, recursive: true);
        }
    }

    private static Plan Calibrate(ScenarioRequest request)
    {
        Plan? best = null;
        var scale = 0.82;
        for (var attempt = 0; attempt < 8; attempt++)
        {
            var plan = Build(request, scale);
            best = best is null || Math.Abs(plan.Occupancy - request.Occupancy) < Math.Abs(best.Occupancy - request.Occupancy)
                ? plan
                : best;
            if (plan.SellableNights == 0)
                break;
            if (Math.Abs(plan.Occupancy - request.Occupancy) < 0.012)
                return plan;
            var ratio = request.Occupancy / Math.Max(0.05, plan.Occupancy);
            scale = Math.Clamp(scale * ratio, 0.25, 1.8);
        }

        return best ?? Build(request, 1);
    }

    private static Plan Build(ScenarioRequest request, double scale)
    {
        var rng = new Random(request.Seed);
        var days = request.End.DayNumber - request.Start.DayNumber;
        var rooms = BuildRooms(request.Rooms);
        var maint = new bool[rooms.Length, days];
        MarkMaintenance(request, rooms, maint, rng);
        var taken = new bool[rooms.Length, days];
        var stays = new List<Stay>();
        var guests = new List<Guest>();
        var babyCots = 0;
        var adjacent = 0;

        for (var r = 0; r < rooms.Length; r++)
        {
            var d = 0;
            while (d < days)
            {
                if (maint[r, d] || taken[r, d])
                {
                    d++;
                    continue;
                }

                var date = request.Start.AddDays(d);
                if (rng.NextDouble() > Math.Min(0.985, Season(date) * scale))
                {
                    d++;
                    continue;
                }

                var length = Math.Min(StayLength(rng), days - d);
                var end = d;
                while (end < d + length && !maint[r, end] && !taken[r, end])
                    end++;
                if (end == d)
                {
                    d++;
                    continue;
                }

                for (var i = d; i < end; i++)
                    taken[r, i] = true;

                var party = PartySize(rng, rooms[r].Capacity);
                var primary = Math.Max(1, party);
                var neighbor = -1;
                if (party >= 2 && rng.NextDouble() < (party >= 3 ? 0.55 : 0.18))
                    neighbor = FreeNeighbor(rooms, r, taken, maint, d, end);

                var here = neighbor >= 0 ? Math.Max(1, party / 2) : party;
                var there = neighbor >= 0 ? party - here : 0;
                if (neighbor >= 0 && there < 1)
                {
                    there = 1;
                    here = Math.Max(1, party - 1);
                }

                var primaryGuest = TakeGuest(rng, guests, primary: true);
                var companions = new List<Guest>();
                for (var g = 1; g < here; g++)
                    companions.Add(TakeCompanion(rng, guests, primaryGuest));

                var cot = (here is 2 or 3) && rng.NextDouble() < 0.16;
                if (cot)
                    babyCots++;
                stays.Add(MakeStay(rng, rooms[r], primaryGuest, companions, request.Start.AddDays(d), request.Start.AddDays(end), here, cot, neighbor >= 0 ? rooms[neighbor].Number : null));

                if (neighbor >= 0)
                {
                    for (var i = d; i < end; i++)
                        taken[neighbor, i] = true;
                    adjacent++;
                    var otherGuest = TakeGuest(rng, guests, primary: true);
                    var otherCompanions = new List<Guest>();
                    for (var g = 1; g < there; g++)
                        otherCompanions.Add(TakeCompanion(rng, guests, otherGuest));
                    var otherCot = (there is 2 or 3) && rng.NextDouble() < 0.16;
                    if (otherCot)
                        babyCots++;
                    var other = MakeStay(rng, rooms[neighbor], otherGuest, otherCompanions, request.Start.AddDays(d), request.Start.AddDays(end), there, otherCot, rooms[r].Number);
                    other.Linked = true;
                    stays[^1].Linked = true;
                    stays.Add(other);
                }

                d = end;
            }
        }

        var asOf = new DateOnly(2026, 11, 14);
        foreach (var stay in stays.Where(s => s.Arrival < asOf && s.Nights > 1 && rng.NextDouble() < 0.025))
        {
            for (var i = 1; i < stay.Nights; i++)
            {
                var index = stay.Arrival.AddDays(i).DayNumber - request.Start.DayNumber;
                var roomIndex = Array.FindIndex(rooms, room => room.Number == stay.RoomNumber);
                if (index >= 0 && index < days && roomIndex >= 0)
                    taken[roomIndex, index] = false;
            }

            stay.Nightly = [stay.Nightly[0]];
            stay.Departure = stay.Arrival.AddDays(1);
            stay.Status = "NoShow";
            stay.Extras.Clear();
            stay.Cot = false;
        }

        foreach (var stay in stays)
            Classify(stay, asOf, rng);

        var cancelled = new List<Stay>();
        var cancelCount = Math.Max(1, stays.Count / 12);
        for (var n = 0; n < cancelCount; n++)
        {
            var room = rooms[rng.Next(rooms.Length)];
            var offset = rng.Next(days - 1);
            var length = StayLength(rng);
            var arrival = request.Start.AddDays(offset);
            var departure = arrival.AddDays(Math.Min(length, Math.Max(1, days - offset)));
            var guest = TakeGuest(rng, guests, primary: true);
            cancelled.Add(new Stay
            {
                Id = NewId(),
                Kind = "cancelled",
                Status = "Cancelled",
                RoomNumber = room.Number,
                RoomId = "",
                Guest = guest,
                Arrival = arrival,
                Departure = departure,
                GuestCount = PartySize(rng, room.Capacity),
                Notes = "Cancelled by the guest before arrival."
            });
        }

        var maintStays = MaintenanceStays(request, rooms, maint);
        var occupied = 0;
        var sellable = 0;
        var blocked = 0;
        for (var r = 0; r < rooms.Length; r++)
        {
            for (var day = 0; day < days; day++)
            {
                if (maint[r, day])
                {
                    blocked++;
                    continue;
                }

                sellable++;
                if (taken[r, day])
                    occupied++;
            }
        }

        decimal roomRevenue = 0;
        decimal otherIncome = 0;
        foreach (var stay in stays)
        {
            stay.RoomRevenue = stay.Status == "NoShow"
                ? stay.Nightly[0]
                : stay.Nightly.Sum();
            stay.ExtraTotal = stay.Extras.Sum(e => e.Amount);
            if (stay.Status is "CheckedOut" or "CheckedIn" or "Confirmed" or "NoShow")
            {
                roomRevenue += stay.RoomRevenue;
                otherIncome += stay.ExtraTotal;
            }
        }

        var expenses = ExpenseTotal(request);
        return new Plan
        {
            Rooms = rooms,
            Guests = guests,
            Stays = stays,
            CancelledStays = cancelled,
            Maintenance = maintStays,
            OccupiedNights = occupied,
            SellableNights = sellable,
            MaintenanceNights = blocked,
            RoomRevenue = roomRevenue,
            OtherIncome = otherIncome,
            Expenses = expenses,
            BabyCots = stays.Count(s => s.Cot),
            AdjacentPairs = stays.Count(s => s.Linked) / 2,
            Cancelled = cancelled.Count
        };
    }

    private static void Classify(Stay stay, DateOnly asOf, Random rng)
    {
        if (stay.Status == "NoShow")
        {
            stay.Prepaid = 0;
            return;
        }

        if (stay.Departure <= asOf)
            stay.Status = "CheckedOut";
        else if (stay.Arrival < asOf)
            stay.Status = "CheckedIn";
        else
            stay.Status = "Confirmed";

        var depositChance = stay.Status == "Confirmed" ? 0.35 : 0.55;
        if (rng.NextDouble() < depositChance)
            stay.Prepaid = decimal.Round(stay.Nightly.Sum() * 0.30m, 2, MidpointRounding.AwayFromZero);
    }

    private static Stay MakeStay(Random rng, RoomSpec room, Guest guest, List<Guest> companions, DateOnly arrival, DateOnly departure, int guests, bool cot, string? adjacent)
    {
        var nights = departure.DayNumber - arrival.DayNumber;
        var nightly = new decimal[nights];
        for (var n = 0; n < nights; n++)
            nightly[n] = NightPrice(room.Capacity, arrival.AddDays(n));
        var extras = Extras(rng, guests, nights, arrival, cot);
        var notes = new List<string>();
        if (adjacent is not null)
            notes.Add($"Adjacent to room {adjacent}.");
        if (cot)
            notes.Add("Baby cot in the room.");
        if (companions.Count > 0)
            notes.Add("Travelling with " + string.Join(", ", companions.Select(c => c.First + " " + c.Last)) + ".");
        return new Stay
        {
            Id = NewId(),
            Kind = "guest",
            RoomNumber = room.Number,
            Guest = guest,
            Companions = companions,
            Arrival = arrival,
            Departure = departure,
            GuestCount = guests,
            Nightly = nightly,
            Extras = extras,
            Cot = cot,
            Notes = string.Join(" ", notes)
        };
    }

    private static List<(DateOnly Date, decimal Amount, string Description)> ExpenseTotalSource(ScenarioRequest request)
    {
        var rows = new List<(DateOnly, decimal, string)>();
        for (var cursor = new DateOnly(request.Start.Year, request.Start.Month, 1); cursor < request.End; cursor = cursor.AddMonths(1))
        {
            var on = cursor.AddDays(Math.Min(5, DateTime.DaysInMonth(cursor.Year, cursor.Month) - 1));
            if (on < request.Start || on >= request.End)
                continue;
            var yearScale = 1m + 0.03m * (cursor.Year - 2023);
            var winter = cursor.Month is 11 or 12 or 1 or 2 or 3;
            rows.Add((on, decimal.Round((winter ? 2200m : cursor.Month is 6 or 7 or 8 ? 900m : 1400m) * yearScale, 2), "Electricity, heat, and water"));
            rows.Add((on, decimal.Round(1100m * yearScale, 2), "Breakfast provisions"));
            rows.Add((on, decimal.Round(480m * yearScale, 2), "Laundry and linen"));
            if (cursor.Month == 1)
                rows.Add((on, decimal.Round(6400m * yearScale, 2), "Renovation of rooms 101-105"));
            if (cursor.Month == 11)
                rows.Add((on, decimal.Round(8200m * yearScale, 2), "Renovation of rooms 206-210"));
        }

        return rows;
    }

    private static decimal ExpenseTotal(ScenarioRequest request) =>
        ExpenseTotalSource(request).Sum(row => row.Amount);

    private static List<Stay> MaintenanceStays(ScenarioRequest request, RoomSpec[] rooms, bool[,] maint)
    {
        var stays = new List<Stay>();
        var days = maint.GetLength(1);
        for (var r = 0; r < rooms.Length; r++)
        {
            var d = 0;
            while (d < days)
            {
                if (!maint[r, d])
                {
                    d++;
                    continue;
                }

                var start = d;
                while (d < days && maint[r, d])
                    d++;
                var arrival = request.Start.AddDays(start);
                var departure = request.Start.AddDays(d);
                var floor = rooms[r].Floor;
                stays.Add(new Stay
                {
                    Id = NewId(),
                    Kind = "maintenance",
                    Status = "Maintenance",
                    RoomNumber = rooms[r].Number,
                    Arrival = arrival,
                    Departure = departure,
                    GuestCount = 0,
                    Notes = d - start >= 7
                        ? $"Floor {floor} segment closed for renovation."
                        : $"Room {rooms[r].Number} closed for adjacent-room maintenance."
                });
            }
        }

        return stays;
    }

    private static void MarkMaintenance(ScenarioRequest request, RoomSpec[] rooms, bool[,] maint, Random rng)
    {
        void Block(string number, DateOnly from, DateOnly to)
        {
            var index = Array.FindIndex(rooms, room => room.Number == number);
            if (index < 0)
                return;
            for (var day = from; day < to; day = day.AddDays(1))
            {
                var offset = day.DayNumber - request.Start.DayNumber;
                if (offset >= 0 && offset < maint.GetLength(1))
                    maint[index, offset] = true;
            }
        }

        for (var year = request.Start.Year; year <= request.End.Year; year++)
        {
            for (var n = 101; n <= 105; n++)
                Block(n.ToString(CultureInfo.InvariantCulture), new DateOnly(year, 1, 16), new DateOnly(year, 1, 26));
            for (var n = 206; n <= 210; n++)
                Block(n.ToString(CultureInfo.InvariantCulture), new DateOnly(year, 11, 3), new DateOnly(year, 11, 17));
            foreach (var n in new[] { 108, 109, 110 })
                Block(n.ToString(CultureInfo.InvariantCulture), new DateOnly(year, 3, 8), new DateOnly(year, 3, 12));

            for (var extra = 0; extra < 8; extra++)
            {
                var room = rooms[rng.Next(rooms.Length)];
                var span = request.End.DayNumber - request.Start.DayNumber;
                var offset = rng.Next(Math.Max(1, span - 2));
                Block(room.Number, request.Start.AddDays(offset), request.Start.AddDays(offset + 2));
            }
        }
    }

    private static int FreeNeighbor(RoomSpec[] rooms, int room, bool[,] taken, bool[,] maint, int from, int to)
    {
        foreach (var candidate in rooms[room].Neighbors)
        {
            var index = Array.FindIndex(rooms, item => item.Number == candidate);
            if (index < 0)
                continue;
            var free = true;
            for (var day = from; day < to; day++)
            {
                if (taken[index, day] || maint[index, day])
                {
                    free = false;
                    break;
                }
            }

            if (free)
                return index;
        }

        return -1;
    }

    private static Guest TakeGuest(Random rng, List<Guest> guests, bool primary)
    {
        if (primary && guests.Count > 30 && rng.NextDouble() < 0.18)
            return guests[rng.Next(guests.Count)];

        var place = Places[rng.Next(Places.Length)];
        var female = rng.NextDouble() < 0.48;
        var firsts = female ? Female(place.Language) : Male(place.Language);
        var lasts = Last(place.Language);
        var guest = new Guest
        {
            Id = NewId(),
            First = firsts[rng.Next(firsts.Length)],
            Last = lasts[rng.Next(lasts.Length)],
            Female = female,
            Language = place.Language,
            Place = place,
            StreetNumber = (rng.Next(80) + 1).ToString(CultureInfo.InvariantCulture),
            Born = new DateOnly(rng.Next(1948, 2004), rng.Next(1, 13), rng.Next(1, 28)),
            EmailLocal = rng.Next(1000, 9999)
        };
        guests.Add(guest);
        return guest;
    }

    private static Guest TakeCompanion(Random rng, List<Guest> guests, Guest primary)
    {
        var female = rng.NextDouble() < 0.5;
        var firsts = female ? Female(primary.Language) : Male(primary.Language);
        var child = rng.NextDouble() < 0.35;
        var guest = new Guest
        {
            Id = NewId(),
            First = firsts[rng.Next(firsts.Length)],
            Last = rng.NextDouble() < 0.75 ? primary.Last : Last(primary.Language)[rng.Next(Last(primary.Language).Length)],
            Female = female,
            Language = primary.Language,
            Place = primary.Place,
            StreetNumber = primary.StreetNumber,
            Born = child
                ? new DateOnly(rng.Next(2016, 2024), rng.Next(1, 13), rng.Next(1, 28))
                : new DateOnly(rng.Next(1948, 2004), rng.Next(1, 13), rng.Next(1, 28)),
            EmailLocal = rng.Next(1000, 9999),
            CompanionOf = primary.Id
        };
        guests.Add(guest);
        return guest;
    }

    private static List<Extra> Extras(Random rng, int guests, int nights, DateOnly arrival, bool cot)
    {
        var list = new List<Extra>();
        if (rng.NextDouble() < 0.62)
            list.Add(new Extra("Breakfast", 14m * guests * nights));
        if (rng.NextDouble() < (arrival.Month is >= 6 and <= 8 ? 0.45 : 0.22))
            list.Add(new Extra("Parking", 18m * nights));
        if (rng.NextDouble() < 0.28)
            list.Add(new Extra("Minibar", 8m + rng.Next(0, 28)));
        if (arrival.Month is >= 4 and <= 10 && rng.NextDouble() < 0.18)
            list.Add(new Extra("Bicycle rental", 22m * Math.Min(nights, 1 + rng.Next(3))));
        if (rng.NextDouble() < 0.15)
            list.Add(new Extra("Half board dinner", 32m * guests * Math.Max(1, nights / 2)));
        if (rng.NextDouble() < 0.07)
            list.Add(new Extra("Late checkout", 25m));
        if (cot && rng.NextDouble() < 0.5)
            list.Add(new Extra("Baby cot", 10m));
        list.Add(new Extra("City tax", 2.50m * guests * nights));
        return list;
    }

    private static decimal NightPrice(int capacity, DateOnly date)
    {
        var baseRate = capacity switch
        {
            >= 4 => 132m,
            3 => 109m,
            _ => 89m
        };
        var year = 1m + 0.03m * (date.Year - 2023);
        var weekend = date.DayOfWeek is DayOfWeek.Friday or DayOfWeek.Saturday ? 16m : 0m;
        var season = 1m;
        if (date.Month == 12 && date.Day >= 20 || date.Month == 1 && date.Day <= 6)
            season = 1.40m;
        else if (date.Month == 6 && date.Day >= 15 || date.Month is 7 or 8)
            season = 1.28m;
        else if (date.Month is 1 or 2 or 3 && !(date.Month == 1 && date.Day <= 6) || date.Month == 11)
            season = 0.88m;
        return decimal.Round((baseRate + weekend) * year * season, 2, MidpointRounding.AwayFromZero);
    }

    private static double Season(DateOnly date)
    {
        if (date.Month == 12 && date.Day >= 20)
            return 0.94;
        if (date.Month == 1 && date.Day <= 6)
            return 0.90;
        return date.Month switch
        {
            1 => 0.40,
            2 => 0.46,
            3 => 0.52,
            4 => 0.66,
            5 => 0.74,
            6 => 0.88,
            7 => 0.96,
            8 => 0.97,
            9 => 0.84,
            10 => 0.60,
            11 => 0.42,
            _ => 0.50
        };
    }

    private static int StayLength(Random rng)
    {
        var roll = rng.NextDouble();
        if (roll < 0.16) return 1;
        if (roll < 0.42) return 2;
        if (roll < 0.66) return 3;
        if (roll < 0.80) return 4;
        if (roll < 0.90) return 5;
        if (roll < 0.96) return 7;
        return 10;
    }

    private static int PartySize(Random rng, int capacity)
    {
        var roll = rng.NextDouble();
        var size = capacity >= 4
            ? roll < 0.15 ? 2 : roll < 0.45 ? 3 : 4
            : capacity == 3
                ? roll < 0.2 ? 1 : roll < 0.7 ? 2 : 3
                : roll < 0.28 ? 1 : 2;
        return Math.Min(capacity, size);
    }

    private static RoomSpec[] BuildRooms(int count)
    {
        var perFloor = count / 2;
        var rooms = new List<RoomSpec>();
        for (var floor = 1; floor <= 2; floor++)
        {
            for (var n = 1; n <= perFloor; n++)
            {
                var number = (floor * 100 + n).ToString(CultureInfo.InvariantCulture);
                var capacity = n % 5 == 4 ? 4 : n % 5 == 3 ? 3 : 2;
                rooms.Add(new RoomSpec(number, floor.ToString(CultureInfo.InvariantCulture), capacity));
            }
        }

        for (var i = 0; i < rooms.Count; i++)
        {
            var floor = rooms[i].Floor;
            if (i > 0 && rooms[i - 1].Floor == floor)
                rooms[i].Neighbors.Add(rooms[i - 1].Number);
            if (i + 1 < rooms.Count && rooms[i + 1].Floor == floor)
                rooms[i].Neighbors.Add(rooms[i + 1].Number);
        }

        return rooms.ToArray();
    }

    private static void Persist(Store store, ScenarioRequest request, Plan plan)
    {
        var roomIds = plan.Rooms.ToDictionary(room => room.Number, _ => NewId());
        var tariffId = NewId();
        var cashId = NewId();
        var houseGuest = new Guest
        {
            Id = NewId(),
            First = "House",
            Last = "Maintenance",
            Language = "en",
            Place = Places[0],
            StreetNumber = "1",
            Born = new DateOnly(1980, 1, 1),
            EmailLocal = 1000
        };

        store.Put("system_configuration", "system", NewId(), new
        {
            id = "system",
            createdUtc = request.Start.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc),
            updatedUtc = DateTime.UtcNow,
            defaultCurrency = "EUR",
            defaultYear = 2026,
            accountingStandard = "usali",
            accountCodes = new Dictionary<string, string>
            {
                ["cash"] = "1000",
                ["guest-ledger"] = "1200",
                ["guest-deposits"] = "2400",
                ["room-revenue"] = "4100",
                ["other-income"] = "4900",
                ["operating-expense"] = "6100"
            },
            settings = new Dictionary<string, string>
            {
                ["HotelName"] = HotelName,
                ["PriceFallback"] = "LatestAvailableYear",
                ["ScenarioStart"] = request.Start.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                ["ScenarioEnd"] = request.End.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                ["ScenarioRooms"] = request.Rooms.ToString(CultureInfo.InvariantCulture),
                ["ScenarioTargetOccupancy"] = request.Occupancy.ToString("0.00", CultureInfo.InvariantCulture)
            }
        });

        for (var year = request.Start.Year; year <= request.End.Year; year++)
            store.Put("years", year.ToString(CultureInfo.InvariantCulture), NewId(), new { periodType = "variable" });

        store.Put("cash_registers", "Reception", cashId, new { name = "Reception", createdAt = DateTime.UtcNow });
        store.Put("tariffs", "rack", tariffId, new
        {
            name = "Rack",
            description = "Current rack card. Historical stays keep the price stored on the booking.",
            applicabilityMode = "AnyDay",
            pricePerNight = 89d,
            priority = 1,
            isActive = true,
            targetType = "Room",
            createdAt = DateTime.UtcNow
        });

        foreach (var room in plan.Rooms)
        {
            store.Put("rooms", room.Number, roomIds[room.Number], new
            {
                name = room.Number,
                capacity = room.Capacity,
                floorNumber = room.Floor,
                houseNumber = "A",
                priority = int.Parse(room.Number, CultureInfo.InvariantCulture),
                hasBeds = "S",
                neighboringRooms = string.Join(",", room.Neighbors),
                comments = room.Floor == "1"
                    ? "First floor, street side."
                    : "Second floor, quieter side."
            });
        }

        foreach (var guest in plan.Guests.Append(houseGuest))
            store.Put("clients", "guest-" + guest.Id, guest.Id, GuestDocument(guest));

        var assetChair = NewId();
        var assetArm = NewId();
        var assetBike = NewId();
        var assetCot = NewId();
        store.Put("assets", "Side chair", assetChair, new { name = "Side chair", code = "CHAIR", description = "Guest-room side chair", createdAt = DateTime.UtcNow });
        store.Put("assets", "Armchair", assetArm, new { name = "Armchair", code = "ARMCHAIR", description = "Lounge armchair", createdAt = DateTime.UtcNow });
        store.Put("assets", "City bicycle", assetBike, new { name = "City bicycle", code = "BICYCLE", description = "Bicycle guests can hire", createdAt = DateTime.UtcNow });
        store.Put("assets", "Baby cot", assetCot, new { name = "Baby cot", code = "COT", description = "Cot for a family room", createdAt = DateTime.UtcNow });

        var linen = NewId();
        var sports = NewId();
        var furniture = NewId();
        store.Put("warehouses", "Linen store", linen, new { name = "Linen store", code = "LINEN", description = "Ground-floor linen and cots", floorNumber = "0", houseNumber = "A", createdAt = DateTime.UtcNow });
        store.Put("warehouses", "Sports store", sports, new { name = "Sports store", code = "SPORT", description = "Bicycles kept for hire", floorNumber = "0", houseNumber = "A", createdAt = DateTime.UtcNow });
        store.Put("warehouses", "Furniture store", furniture, new { name = "Furniture store", code = "FURN", description = "Spare chairs", floorNumber = "1", houseNumber = "A", createdAt = DateTime.UtcNow });

        foreach (var room in plan.Rooms)
            store.Put("inventory", "chair-" + room.Number, NewId(), new { assetId = assetChair, roomId = roomIds[room.Number], quantity = 1, minQuantityDefault = 1, requiredOnCheckin = true, createdAt = DateTime.UtcNow });
        store.Put("inventory", "chairs-spare", NewId(), new { assetId = assetChair, warehouseId = furniture, quantity = 16, minQuantityDefault = 8, createdAt = DateTime.UtcNow });
        var familyRooms = plan.Rooms.Where(room => room.Capacity >= 4).Take(4).ToArray();
        foreach (var room in familyRooms)
            store.Put("inventory", "arm-" + room.Number, NewId(), new { assetId = assetArm, roomId = roomIds[room.Number], quantity = 1, createdAt = DateTime.UtcNow });
        store.Put("inventory", "arm-spare", NewId(), new { assetId = assetArm, warehouseId = furniture, quantity = 4, createdAt = DateTime.UtcNow });
        store.Put("inventory", "bikes", NewId(), new { assetId = assetBike, warehouseId = sports, quantity = 12, minQuantityDefault = 4, createdAt = DateTime.UtcNow });

        var cotsInRooms = plan.Stays.Where(s => s.Cot && s.Status == "CheckedIn").Take(2).ToArray();
        foreach (var stay in cotsInRooms)
            store.Put("inventory", "cot-" + stay.RoomNumber, NewId(), new { assetId = assetCot, roomId = roomIds[stay.RoomNumber], quantity = 1, createdAt = DateTime.UtcNow });
        store.Put("inventory", "cots-store", NewId(), new { assetId = assetCot, warehouseId = linen, quantity = Math.Max(1, 6 - cotsInRooms.Length), minQuantityDefault = 2, createdAt = DateTime.UtcNow });

        var ledger = new Dictionary<DateOnly, List<object>>();
        var bookingIndex = new Dictionary<string, List<string>>();
        void Post(DateOnly date, string type, string notes, decimal amount, string debitAccount, string debitGroup, string creditAccount, string creditGroup, string? bookingId, string? sourceId, string createdBy, string? applied, string? method)
        {
            if (amount == 0)
                return;
            var journal = Guid.NewGuid().ToString("N");
            var when = date.ToDateTime(new TimeOnly(10, 0), DateTimeKind.Utc);
            if (!ledger.TryGetValue(date, out var lines))
                ledger[date] = lines = [];
            void Line(string account, string group, decimal debit, decimal credit)
            {
                var seq = lines.Count + 1;
                var id = $"ledger_{date:yyyyMMdd}_{seq:D6}";
                lines.Add(new
                {
                    id,
                    sequence = seq,
                    timestamp = when,
                    entryType = type,
                    bookingId,
                    journalId = journal,
                    accountCode = account,
                    accountGroup = group,
                    debit,
                    credit,
                    amount = debit - credit,
                    cashRegisterId = account == "1000" ? cashId : null,
                    sourceType = bookingId is null ? "expense" : "booking",
                    sourceId,
                    method,
                    status = "posted",
                    createdBy,
                    appliedTo = applied,
                    notes
                });
                if (bookingId is not null)
                {
                    if (!bookingIndex.TryGetValue(bookingId, out var ids))
                        bookingIndex[bookingId] = ids = [];
                    ids.Add(id);
                }
            }

            Line(debitAccount, debitGroup, amount, 0);
            Line(creditAccount, creditGroup, 0, amount);
        }

        Post(request.Start, "chart_of_accounts", "Chart of accounts set to USALI. Journals from this entry forward use 1000, 1200, 2400, 4100, 4900, 6100.", 0, "1000", "asset", "1000", "asset", null, null, Desk[0], null, null);

        var desk = 0;
        foreach (var stay in plan.Stays)
        {
            var who = Desk[desk++ % Desk.Length];
            var roomId = roomIds[stay.RoomNumber];
            var guestId = stay.Guest?.Id ?? houseGuest.Id;
            store.Put("bookings", $"{stay.Arrival.Year}_{stay.Id}", stay.Id, new
            {
                year = stay.Arrival.Year,
                clientId = guestId,
                roomId,
                arrivalDate = stay.Arrival,
                departureDate = stay.Departure,
                status = stay.Status,
                notes = string.IsNullOrWhiteSpace(stay.Notes) ? null : stay.Notes,
                actualArrivalDate = stay.Status is "CheckedIn" or "CheckedOut" ? stay.Arrival : (DateOnly?)null,
                actualDepartureDate = stay.Status == "CheckedOut" ? stay.Departure : (DateOnly?)null,
                actualGuestCount = stay.Status is "CheckedIn" or "CheckedOut" ? stay.GuestCount : (int?)null,
                prepaidAmount = stay.Prepaid > 0 ? (double)stay.Prepaid : (double?)null,
                prepaidMethod = stay.Prepaid > 0 ? "transfer" : null,
                expectedGuestCount = stay.GuestCount,
                tariffId,
                tariffTotalPrice = (double)stay.RoomRevenue
            });

            var order = 1;
            foreach (var companion in stay.Companions)
            {
                store.Put("booking_guests", $"{stay.Arrival.Year}_{NewId()}", NewId(), new
                {
                    year = stay.Arrival.Year,
                    bookingId = stay.Id,
                    clientId = companion.Id,
                    guestNumber = ++order
                });
            }

            if (stay.Guest is not null)
            {
                store.Put("booking_guests", $"{stay.Arrival.Year}_{NewId()}", NewId(), new
                {
                    year = stay.Arrival.Year,
                    bookingId = stay.Id,
                    clientId = stay.Guest.Id,
                    guestNumber = 1
                });
            }

            foreach (var extra in stay.Extras)
            {
                store.Put("booking_costs", $"{stay.Arrival.Year}_{NewId()}", NewId(), new
                {
                    year = stay.Arrival.Year,
                    bookingId = stay.Id,
                    amount = (double)extra.Amount,
                    description = extra.Description
                });
            }

            if (stay.Kind != "guest")
                continue;

            var chargeDate = stay.Arrival;
            Post(chargeDate, "booking_stay_charge", "Room charge", stay.RoomRevenue, "1200", "asset", "4100", "revenue", stay.Id, stay.Id, who, "stay_balance", null);
            if (stay.ExtraTotal > 0)
                Post(chargeDate, "booking_extra_charge", "Extras", stay.ExtraTotal, "1200", "asset", "4900", "revenue", stay.Id, stay.Id, who, "extras", null);

            if (stay.Prepaid > 0)
            {
                var depositDate = stay.Arrival.AddDays(-21);
                if (depositDate < request.Start)
                    depositDate = request.Start;
                Post(depositDate, "booking_prepaid", "Deposit", stay.Prepaid, "1000", "asset", "2400", "liability", stay.Id, stay.Id, who, "deposit", "transfer");
            }

            if (stay.Status is "CheckedOut" or "NoShow")
            {
                var due = stay.RoomRevenue + stay.ExtraTotal;
                if (stay.Prepaid > 0)
                {
                    Post(stay.Departure, "booking_deposit_application", "Deposit applied", stay.Prepaid, "2400", "liability", "1200", "asset", stay.Id, stay.Id, who, "stay_balance", null);
                    due -= stay.Prepaid;
                }

                if (due > 0)
                {
                    var method = rngMethod(stay.Id);
                    Post(stay.Departure, "booking_payment", "Guest settlement", due, "1000", "asset", "1200", "asset", stay.Id, stay.Id, who, "stay_balance", method);
                }
            }
        }

        foreach (var stay in plan.CancelledStays)
        {
            var guestId = stay.Guest!.Id;
            store.Put("cancelled_bookings", $"{stay.Arrival.Year}_{stay.Id}", stay.Id, new
            {
                year = stay.Arrival.Year,
                clientId = guestId,
                roomId = roomIds[stay.RoomNumber],
                arrivalDate = stay.Arrival,
                departureDate = stay.Departure,
                status = "Cancelled",
                cancelledAt = stay.Arrival.AddDays(-4).ToDateTime(new TimeOnly(9, 0), DateTimeKind.Utc),
                notes = stay.Notes,
                bookingId = stay.Id
            });
        }

        foreach (var stay in plan.Maintenance)
        {
            store.Put("bookings", $"{stay.Arrival.Year}_{stay.Id}", stay.Id, new
            {
                year = stay.Arrival.Year,
                clientId = houseGuest.Id,
                roomId = roomIds[stay.RoomNumber],
                arrivalDate = stay.Arrival,
                departureDate = stay.Departure,
                status = "Maintenance",
                notes = stay.Notes,
                expectedGuestCount = 0,
                tariffTotalPrice = 0d
            });
        }

        var expenseWho = Desk[0];
        foreach (var expense in ExpenseTotalSource(request))
        {
            var id = NewId();
            store.Put("expenses", $"{expense.Date.Year}_{id}", id, new
            {
                year = expense.Date.Year,
                cashRegisterId = cashId,
                amount = (double)expense.Amount,
                description = expense.Description,
                date = expense.Date
            });
            Post(expense.Date, "hotel_expense", expense.Description, expense.Amount, "6100", "expense", "1000", "asset", null, id, expenseWho, null, "transfer");
        }

        foreach (var (date, lines) in ledger)
        {
            var dir = Path.Combine(store.Root, "ledger", date.Year.ToString("D4"), date.Month.ToString("D2"), date.Day.ToString("D2"));
            Directory.CreateDirectory(dir);
            var snapshot = new
            {
                date = date.ToDateTime(TimeOnly.MinValue),
                timestamp = date.ToDateTime(new TimeOnly(23, 0), DateTimeKind.Utc),
                entries = lines,
                totalDayBalance = 0m,
                sequenceAt = lines.Count
            };
            File.WriteAllText(Path.Combine(dir, "_snapshot.json"), JsonSerializer.Serialize(snapshot, Json));
        }

        foreach (var (bookingId, ids) in bookingIndex)
        {
            var path = Path.Combine(store.Root, "bookings", bookingId, "_ledger_index.json");
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllText(path, JsonSerializer.Serialize(ids));
        }

        static string rngMethod(string id) => (id[0] % 5) switch
        {
            0 => "cash",
            1 => "transfer",
            _ => "card"
        };
    }

    private static object GuestDocument(Guest guest) => new
    {
        lastName = guest.Last,
        firstName = guest.First,
        gender = guest.Female ? "F" : "M",
        language = guest.Language,
        dateOfBirth = guest.Born.ToDateTime(TimeOnly.MinValue),
        birthCity = guest.Place.City,
        birthNation = guest.Place.Country,
        nationality = guest.Place.Country,
        nation = guest.Place.Country,
        region = guest.Place.Region,
        city = guest.Place.City,
        street = guest.Place.Street,
        streetNumber = guest.StreetNumber,
        postalCode = guest.Place.Postal,
        phone = guest.Place.PhonePrefix + guest.EmailLocal.ToString(CultureInfo.InvariantCulture),
        email = $"{guest.First}.{guest.Last}.{guest.EmailLocal}@example.com".Replace(" ", "", StringComparison.Ordinal).ToLowerInvariant(),
        notes = guest.CompanionOf is null ? null : "Companion on a family stay.",
        createdAt = DateTime.UtcNow
    };

    private static string NewId()
    {
        const string alphabet = "abcdefghijklmnopqrstuvwxyz234567";
        var bytes = Guid.NewGuid().ToByteArray();
        var buffer = 0;
        var size = 0;
        var result = new char[26];
        var n = 0;
        foreach (var b in bytes)
        {
            buffer = (buffer << 8) | b;
            size += 8;
            while (size >= 5)
            {
                size -= 5;
                result[n++] = alphabet[(buffer >> size) & 31];
            }
        }

        if (size > 0)
            result[n++] = alphabet[(buffer << (5 - size)) & 31];
        return new string(result, 0, n);
    }

    private static string[] Female(string language) => language switch
    {
        "de" => ["Anna", "Lena", "Klara", "Mia", "Greta"],
        "fr" => ["Camille", "Louise", "Chloé", "Manon", "Inès"],
        "es" => ["Lucía", "Marta", "Sofía", "Elena", "Nuria"],
        "nl" => ["Sanne", "Fleur", "Noor", "Eva", "Lotte"],
        "pl" => ["Zofia", "Hanna", "Maja", "Aleksandra", "Natalia"],
        "pt" => ["Matilde", "Beatriz", "Inês", "Carolina", "Rita"],
        "sv" => ["Astrid", "Elsa", "Linnea", "Maja", "Freja"],
        "en" => ["Emma", "Olivia", "Aoife", "Grace", "Hannah"],
        _ => ["Giulia", "Chiara", "Francesca", "Elena", "Martina"]
    };

    private static string[] Male(string language) => language switch
    {
        "de" => ["Lukas", "Jonas", "Felix", "Paul", "Theo"],
        "fr" => ["Louis", "Hugo", "Arthur", "Jules", "Adam"],
        "es" => ["Hugo", "Martín", "Pablo", "Daniel", "Javier"],
        "nl" => ["Daan", "Sem", "Luuk", "Bram", "Finn"],
        "pl" => ["Jan", "Antoni", "Aleksander", "Jakub", "Filip"],
        "pt" => ["João", "Miguel", "Francisco", "Duarte", "Tiago"],
        "sv" => ["Erik", "Oscar", "Noah", "Liam", "Axel"],
        "en" => ["Oliver", "George", "Jack", "Harry", "Sean"],
        _ => ["Luca", "Marco", "Andrea", "Matteo", "Davide"]
    };

    private static string[] Last(string language) => language switch
    {
        "de" => ["Keller", "Wagner", "Bauer", "Schneider", "Hofmann"],
        "fr" => ["Laurent", "Moreau", "Bernard", "Petit", "Rousseau"],
        "es" => ["García", "López", "Martín", "Sánchez", "Navarro"],
        "nl" => ["de Vries", "Jansen", "Bakker", "Visser", "Smit"],
        "pl" => ["Nowak", "Kowalski", "Wiśniewski", "Wójcik", "Kamiński"],
        "pt" => ["Silva", "Santos", "Ferreira", "Costa", "Oliveira"],
        "sv" => ["Lindberg", "Berg", "Nyström", "Eriksson", "Holm"],
        "en" => ["Murphy", "Walsh", "Clarke", "Hughes", "Walker"],
        _ => ["Rossi", "Bianchi", "Ferrari", "Esposito", "Romano"]
    };

    private static readonly Place[] Places =
    [
        new("Italy", "it", "Verona", "Veneto", "37121", "Via Mazzini", "+39"),
        new("Italy", "it", "Bologna", "Emilia-Romagna", "40121", "Via Indipendenza", "+39"),
        new("Italy", "it", "Firenze", "Toscana", "50122", "Via dei Calzaiuoli", "+39"),
        new("Germany", "de", "München", "Bayern", "80331", "Sendlinger Straße", "+49"),
        new("Germany", "de", "Hamburg", "Hamburg", "20095", "Mönckebergstraße", "+49"),
        new("Germany", "de", "Köln", "Nordrhein-Westfalen", "50667", "Schildergasse", "+49"),
        new("France", "fr", "Lyon", "Auvergne-Rhône-Alpes", "69002", "Rue de la République", "+33"),
        new("France", "fr", "Bordeaux", "Nouvelle-Aquitaine", "33000", "Rue Sainte-Catherine", "+33"),
        new("Spain", "es", "Barcelona", "Catalunya", "08002", "Carrer de Ferran", "+34"),
        new("Spain", "es", "Valencia", "Comunitat Valenciana", "46002", "Carrer de Sant Vicent", "+34"),
        new("Netherlands", "nl", "Utrecht", "Utrecht", "3511", "Oudegracht", "+31"),
        new("Netherlands", "nl", "Amsterdam", "Noord-Holland", "1012", "Keizersgracht", "+31"),
        new("Switzerland", "de", "Zürich", "Zürich", "8001", "Bahnhofstrasse", "+41"),
        new("Switzerland", "fr", "Genève", "Genève", "1204", "Rue du Rhône", "+41"),
        new("Austria", "de", "Salzburg", "Salzburg", "5020", "Getreidegasse", "+43"),
        new("Belgium", "nl", "Antwerpen", "Antwerpen", "2000", "Meir", "+32"),
        new("Belgium", "fr", "Liège", "Liège", "4000", "Rue Pont d'Avroy", "+32"),
        new("Poland", "pl", "Kraków", "Małopolska", "31-008", "Floriańska", "+48"),
        new("Czechia", "en", "Praha", "Praha", "110 00", "Celetná", "+420"),
        new("Sweden", "sv", "Göteborg", "Västra Götaland", "411 03", "Kungsgatan", "+46"),
        new("Denmark", "en", "Aarhus", "Midtjylland", "8000", "Søndergade", "+45"),
        new("Portugal", "pt", "Porto", "Porto", "4000-322", "Rua de Santa Catarina", "+351"),
        new("Ireland", "en", "Galway", "Connacht", "H91", "Shop Street", "+353"),
        new("United Kingdom", "en", "York", "North Yorkshire", "YO1", "Stonegate", "+44")
    ];

    private sealed class Store(string root)
    {
        private readonly Dictionary<string, Dictionary<string, string>> _index = new(StringComparer.Ordinal);
        public string Root { get; } = root;

        public void Put(string collection, string name, string id, object document)
        {
            var dir = Path.Combine(Root, "collections", collection);
            Directory.CreateDirectory(dir);
            File.WriteAllText(Path.Combine(dir, id + ".json"), JsonSerializer.Serialize(document, Json));
            if (!_index.TryGetValue(collection, out var index))
                _index[collection] = index = new Dictionary<string, string>(StringComparer.Ordinal);
            index[name] = id;
        }

        public void Flush()
        {
            foreach (var (collection, index) in _index)
            {
                var withMeta = new Dictionary<string, string>(index, StringComparer.Ordinal)
                {
                    ["_metadata"] = JsonSerializer.Serialize(new
                    {
                        version = "1",
                        lastUpdated = DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture),
                        entryCount = index.Count
                    })
                };
                var path = Path.Combine(Root, "collections", collection, "_index.json");
                File.WriteAllText(path, JsonSerializer.Serialize(withMeta, Json));
            }
        }
    }

    private sealed class Plan
    {
        public RoomSpec[] Rooms { get; init; } = [];
        public List<Guest> Guests { get; init; } = [];
        public List<Stay> Stays { get; init; } = [];
        public List<Stay> CancelledStays { get; init; } = [];
        public List<Stay> Maintenance { get; init; } = [];
        public int OccupiedNights { get; init; }
        public int SellableNights { get; init; }
        public int MaintenanceNights { get; init; }
        public decimal RoomRevenue { get; init; }
        public decimal OtherIncome { get; init; }
        public decimal Expenses { get; init; }
        public int BabyCots { get; init; }
        public int AdjacentPairs { get; init; }
        public int Cancelled { get; init; }
        public double Occupancy => SellableNights == 0 ? 0 : (double)OccupiedNights / SellableNights;
    }

    private sealed class RoomSpec(string number, string floor, int capacity)
    {
        public string Number { get; } = number;
        public string Floor { get; } = floor;
        public int Capacity { get; } = capacity;
        public List<string> Neighbors { get; } = [];
    }

    private sealed class Guest
    {
        public string Id { get; init; } = "";
        public string First { get; init; } = "";
        public string Last { get; init; } = "";
        public bool Female { get; init; }
        public string Language { get; init; } = "";
        public Place Place { get; init; } = Places[0];
        public string StreetNumber { get; init; } = "";
        public DateOnly Born { get; init; }
        public int EmailLocal { get; init; }
        public string? CompanionOf { get; init; }
    }

    private sealed record Place(string Country, string Language, string City, string Region, string Postal, string Street, string PhonePrefix);

    private sealed class Stay
    {
        public string Id { get; init; } = "";
        public string Kind { get; init; } = "guest";
        public string Status { get; set; } = "";
        public string RoomNumber { get; init; } = "";
        public string RoomId { get; init; } = "";
        public Guest? Guest { get; init; }
        public List<Guest> Companions { get; init; } = [];
        public DateOnly Arrival { get; init; }
        public DateOnly Departure { get; set; }
        public int GuestCount { get; init; }
        public decimal[] Nightly { get; set; } = [];
        public List<Extra> Extras { get; init; } = [];
        public bool Cot { get; set; }
        public bool Linked { get; set; }
        public decimal Prepaid { get; set; }
        public decimal RoomRevenue { get; set; }
        public decimal ExtraTotal { get; set; }
        public string Notes { get; init; } = "";
        public int Nights => Math.Max(0, Departure.DayNumber - Arrival.DayNumber);
    }

    private sealed record Extra(string Description, decimal Amount);
}
