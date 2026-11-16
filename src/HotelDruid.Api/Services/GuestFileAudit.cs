using HotelDruid.Api.Models;
using HotelDruid.Shared;
using HotelDruid.Shared.Configuration;

namespace HotelDruid.Api.Services;

public sealed record GuestFileReport(bool Ready, IReadOnlyList<string> Gaps, string StandardId);

/// <summary>
/// Names and fields the chosen registration standard expects for the stays of one day.
/// A short count or an empty required field is a line on the night audit and, when the
/// standard produces a digital file, on that report.
/// </summary>
public static class GuestFileAudit
{
    public static async Task<GuestFileReport> CollectAsync(DateOnly date, IKeyValueStore store, ISystemConfigurationStore configStore)
    {
        var config = await configStore.GetAsync();
        var profile = GuestRegistration.Resolve(config?.RegistrationStandard, config?.RegistrationCustom);
        if (string.Equals(profile.Id, GuestRegistration.NoneId, StringComparison.OrdinalIgnoreCase)
            || (!profile.ShowOnNightAudit && !profile.DigitalReport))
        {
            return new GuestFileReport(true, [], profile.Id);
        }

        var gaps = new List<string>();
        var bookings = await store.GetIndexAsync("bookings");
        var guests = await LoadGuestsAsync(store);
        var rooms = await store.GetIndexAsync("rooms");
        var roomNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var room in rooms)
        {
            var stored = await store.GetAsync<RoomStorageModel>("rooms", room.Value);
            if (stored is not null)
                roomNames[room.Value] = stored.Name ?? room.Value;
        }

        foreach (var entry in bookings)
        {
            var booking = await store.GetAsync<BookingStorageModel>("bookings", entry.Value);
            if (booking is null || booking.DeletedAt.HasValue)
                continue;
            if (string.Equals(booking.Status, "Cancelled", StringComparison.OrdinalIgnoreCase))
                continue;
            var arrival = booking.ArrivalDate ?? booking.ActualArrivalDate;
            var departure = booking.DepartureDate ?? booking.ActualDepartureDate;
            if (arrival is null || departure is null || date < arrival || date >= departure)
                continue;

            var lead = await LoadFactsAsync(store, booking.ClientId, covered: false);
            var attached = new List<GuestFacts>();
            foreach (var guest in guests.Where(item => string.Equals(item.BookingId, entry.Value, StringComparison.OrdinalIgnoreCase)))
            {
                if (string.Equals(guest.ClientId, booking.ClientId, StringComparison.OrdinalIgnoreCase))
                    continue;
                var facts = await LoadFactsAsync(store, guest.ClientId, guest.CoveredByLead);
                if (facts is not null)
                    attached.Add(facts);
            }

            var room = booking.RoomId is not null && roomNames.TryGetValue(booking.RoomId, out var name) ? name : booking.RoomId ?? entry.Value;
            var stay = new StayFacts
            {
                Label = $"Room {room}",
                Arrived = booking.ActualArrivalDate.HasValue && booking.ActualArrivalDate.Value <= date,
                ExpectedGuests = booking.ExpectedGuestCount,
                ActualGuests = booking.ActualGuestCount,
                EntryTime = booking.EntryTime,
                Lead = lead,
                Attached = attached
            };
            gaps.AddRange(GuestRegistration.Gaps(profile, stay));
        }

        return new GuestFileReport(gaps.Count == 0, gaps, profile.Id);
    }

    private static async Task<List<BookingGuestStorageModel>> LoadGuestsAsync(IKeyValueStore store)
    {
        var index = await store.GetIndexAsync("booking_guests");
        var rows = new List<BookingGuestStorageModel>();
        foreach (var entry in index)
        {
            var guest = await store.GetAsync<BookingGuestStorageModel>("booking_guests", entry.Value);
            if (guest is not null)
                rows.Add(guest);
        }
        return rows;
    }

    private static async Task<GuestFacts?> LoadFactsAsync(IKeyValueStore store, string? clientId, bool covered)
    {
        if (string.IsNullOrWhiteSpace(clientId))
            return null;
        var client = await store.GetAsync<ClientStorageModel>("clients", clientId);
        if (client is null)
            return null;
        return new GuestFacts
        {
            LastName = client.LastName,
            FirstName = client.FirstName,
            Gender = client.Gender,
            DateOfBirth = client.DateOfBirth,
            BirthCity = client.BirthCity,
            BirthNation = client.BirthNation,
            Nationality = client.Nationality,
            SecondNationality = client.SecondNationality,
            Street = client.Street,
            City = client.City,
            Nation = client.Nation,
            Phone = client.Phone,
            Email = client.Email,
            DocumentType = client.DocumentType,
            DocumentNumber = client.DocumentNumber,
            DocumentCity = client.DocumentCity,
            DocumentNation = client.DocumentNation,
            CoveredByLead = covered
        };
    }
}
