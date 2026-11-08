using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using HotelDruid.Shared;

namespace HotelDruid.Client.Services
{
    /// <summary>
    /// The visible name of a booking is its arrival date and room.
    /// The booking id stays in the address, where the pages already select by it.
    /// </summary>
    public static class BookingReference
    {
        public static string Label(DateOnly? arrival, string? roomName)
        {
            var date = arrival?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            var room = string.IsNullOrWhiteSpace(roomName) ? null : roomName.Trim();
            if (date is null)
            {
                return room ?? string.Empty;
            }

            return room is null ? date : $"{date} · {room}";
        }

        public static string Href(string? bookingId)
            => string.IsNullOrWhiteSpace(bookingId)
                ? "/bookings"
                : $"/bookings?selectedId={Uri.EscapeDataString(bookingId)}";
    }

    public sealed class BookingCatalog
    {
        private readonly Dictionary<string, BookingDto> bookingsById;
        private readonly Dictionary<string, string> roomNames;

        private BookingCatalog(IReadOnlyList<BookingDto> bookings, Dictionary<string, BookingDto> bookingsById, Dictionary<string, string> roomNames)
        {
            Bookings = bookings;
            this.bookingsById = bookingsById;
            this.roomNames = roomNames;
        }

        public IReadOnlyList<BookingDto> Bookings { get; }

        public static BookingCatalog From(IEnumerable<BookingDto>? bookings, IEnumerable<RoomDto>? rooms)
        {
            var roomNames = (rooms ?? Array.Empty<RoomDto>())
                .Where(room => !string.IsNullOrWhiteSpace(room.Id) && !string.IsNullOrWhiteSpace(room.Name))
                .GroupBy(room => room.Id!, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(group => group.Key, group => group.First().Name!, StringComparer.OrdinalIgnoreCase);

            var bookingList = (bookings ?? Array.Empty<BookingDto>())
                .Where(booking => !string.IsNullOrWhiteSpace(booking.Id))
                .GroupBy(booking => booking.Id!, StringComparer.OrdinalIgnoreCase)
                .Select(group => group.First())
                .OrderByDescending(booking => booking.ArrivalDate ?? DateOnly.MinValue)
                .ThenBy(booking => roomNames.TryGetValue(booking.RoomId ?? string.Empty, out var name) ? name : string.Empty, StringComparer.OrdinalIgnoreCase)
                .ToList();

            var bookingsById = bookingList.ToDictionary(booking => booking.Id!, StringComparer.OrdinalIgnoreCase);
            return new BookingCatalog(bookingList, bookingsById, roomNames);
        }

        public string Label(string? bookingId)
            => bookingsById.TryGetValue(bookingId ?? string.Empty, out var booking) ? Label(booking) : string.Empty;

        public string Label(BookingDto? booking)
            => booking is null
                ? string.Empty
                : BookingReference.Label(booking.ArrivalDate, RoomName(booking.RoomId));

        public string? RoomName(string? roomId)
            => !string.IsNullOrWhiteSpace(roomId) && roomNames.TryGetValue(roomId, out var name) ? name : null;
    }
}
