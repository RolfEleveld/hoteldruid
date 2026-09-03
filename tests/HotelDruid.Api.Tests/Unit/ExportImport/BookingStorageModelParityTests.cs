using System.Reflection;
using HotelDruid.Api.Models;
using HotelDruid.Api.Services.ExportImport;
using HotelDruid.Shared;
using Xunit;

namespace HotelDruid.Api.Tests.Unit.ExportImport;

/// <summary>
/// Regression tests guarding the booking contract.
/// The build once broke because <see cref="BookingDto"/> gained ActualDepartureDate
/// while <see cref="BookingStorageModel"/> did not, breaking export/import.
/// These tests fail fast if the DTO and the persisted model drift apart again.
/// </summary>
public class BookingStorageModelParityTests
{
    [Fact]
    public void BookingDto_Properties_ExistOnStorageModel()
    {
        var dtoProperties = typeof(BookingDto)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(p => p.Name)
            .ToHashSet(StringComparer.Ordinal);

        var storageProperties = typeof(BookingStorageModel)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(p => p.Name)
            .ToHashSet(StringComparer.Ordinal);

        var missing = dtoProperties
            .Where(name => !string.Equals(name, nameof(BookingDto.Id), StringComparison.Ordinal))
            .Where(name => !storageProperties.Contains(name))
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToList();

        Assert.True(missing.Count == 0,
            "BookingDto properties missing from BookingStorageModel: " + string.Join(", ", missing));
    }

    [Fact]
    public void ToCanonicalBookings_IncludesActualDepartureDate()
    {
        var mapper = new CanonicalMapper(LoggingHelper.CreateLogger<CanonicalMapper>());
        var booking = new BookingDto(
            Id: "b1",
            Year: 2026,
            ClientId: "c1",
            RoomId: "r1",
            ArrivalDate: new DateOnly(2026, 8, 1),
            DepartureDate: new DateOnly(2026, 8, 5),
            Status: "CheckedOut",
            Notes: null,
            ActualArrivalDate: new DateOnly(2026, 8, 1),
            ActualGuestCount: 2,
            PrepaidAmount: 50.0,
            ActualDepartureDate: new DateOnly(2026, 8, 4));

        var canonical = mapper.ToCanonicalBookings(new[] { booking });

        Assert.Equal("2026-08-04", canonical.Rows[0]["actual_departure_date"]);
        Assert.Contains(canonical.Columns, c => c.Name == "actual_departure_date");
    }

    [Fact]
    public void CanonicalBookings_RoundTrip_PreservesActualDepartureDate()
    {
        var mapper = new CanonicalMapper(LoggingHelper.CreateLogger<CanonicalMapper>());
        var booking = new BookingDto(
            Id: "b1",
            Year: 2026,
            ClientId: "c1",
            RoomId: "r1",
            ArrivalDate: new DateOnly(2026, 8, 1),
            DepartureDate: new DateOnly(2026, 8, 5),
            Status: "CheckedOut",
            Notes: "n",
            ActualArrivalDate: new DateOnly(2026, 8, 1),
            ActualGuestCount: 2,
            PrepaidAmount: 50.0,
            ActualDepartureDate: new DateOnly(2026, 8, 4));

        var roundTripped = mapper.FromCanonicalBookings(mapper.ToCanonicalBookings(new[] { booking }));

        Assert.Single(roundTripped);
        Assert.Equal(booking.ActualDepartureDate, roundTripped[0].ActualDepartureDate);
    }

    [Fact]
    public void FromCanonicalBookings_MissingActualDepartureDate_RemainsNull()
    {
        var mapper = new CanonicalMapper(LoggingHelper.CreateLogger<CanonicalMapper>());
        var legacy = new CanonicalData(
            TableName: "bookings",
            RowCount: 1,
            Rows: new List<Dictionary<string, object?>>
            {
                new()
                {
                    ["id"] = "b1",
                    ["year"] = 2024,
                    ["client_id"] = "c1",
                    ["arrival_date"] = "2024-08-01",
                    ["departure_date"] = "2024-08-05"
                }
            },
            Columns: new List<ColumnDefinition>());

        var result = mapper.FromCanonicalBookings(legacy);

        Assert.Single(result);
        Assert.Null(result[0].ActualDepartureDate);
    }
}
