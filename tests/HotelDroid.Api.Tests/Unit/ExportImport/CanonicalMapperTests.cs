using HotelDroid.Api.Models;
using HotelDroid.Api.Services.ExportImport;
using Microsoft.Extensions.Logging;
using Xunit;

namespace HotelDroid.Api.Tests.Unit.ExportImport;

/// <summary>
/// Unit tests for CanonicalMapper
/// Tests flexible inbound naming and refined outbound naming
/// </summary>
public class CanonicalMapperTests
{
    private readonly ICanonicalMapper _mapper;
    private readonly ILogger<CanonicalMapper> _logger;

    public CanonicalMapperTests()
    {
        _logger = LoggingHelper.CreateLogger<CanonicalMapper>();
        _mapper = new CanonicalMapper(_logger);
    }

    [Theory]
    [InlineData("appartamenti", "rooms")]
    [InlineData("apartments", "rooms")]
    [InlineData("rooms", "rooms")]
    [InlineData("Apartments", "rooms")]
    [InlineData("APPARTAMENTI", "rooms")]
    public void NormalizeTableName_AcceptsAllVariants(string input, string expected)
    {
        // Act
        var result = _mapper.NormalizeTableName(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("idappartamenti", "id")]
    [InlineData("room_id", "id")]
    [InlineData("id", "id")]
    [InlineData("numpiano", "floor_number")]
    [InlineData("floor", "floor_number")]
    [InlineData("floor_number", "floor_number")]
    [InlineData("maxoccupanti", "capacity")]
    [InlineData("max_occupancy", "capacity")]
    [InlineData("capacity", "capacity")]
    public void NormalizeFieldName_AcceptsAllVariants(string input, string expected)
    {
        // Act
        var result = _mapper.NormalizeFieldName(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetExportTableName_ReturnsStandardName()
    {
        // Act
        var result = _mapper.GetExportTableName("appartamenti");

        // Assert
        Assert.Equal("rooms", result);
    }

    [Fact]
    public void GetHotelDroidTableName_ReturnsCompatibilityName()
    {
        // Act
        var result = _mapper.GetHotelDroidTableName("rooms");

        // Assert
        Assert.Equal("appartamenti", result);
    }

    [Fact]
    public void ToCanonical_ConvertsRoomDtoArray()
    {
        // Arrange
        var rooms = new[]
        {
            new RoomDto("room1", "Room 1", 2, "1", "101", null, null, "S", "room2", "Corner"),
            new RoomDto("room2", "Room 2", 4, "1", "102", 1, null, "S", "room1,room3", null)
        };

        // Act
        var result = _mapper.ToCanonical(rooms);

        // Assert
        Assert.Equal("rooms", result.TableName);
        Assert.Equal(2, result.RowCount);
        Assert.NotEmpty(result.Columns);
        Assert.Equal(2, result.Rows.Count);
        Assert.Equal("room1", result.Rows[0]["id"]);
        Assert.Equal("Room 1", result.Rows[0]["name"]);
        Assert.Equal(2, result.Rows[0]["capacity"]);
    }

    [Fact]
    public void FromCanonical_ConvertsCanonicalDataToRoomDto()
    {
        // Arrange
        var canonicalData = new CanonicalData(
            TableName: "rooms",
            RowCount: 1,
            Rows: new()
            {
                new()
                {
                    ["id"] = "room1",
                    ["name"] = "Room 1",
                    ["capacity"] = 2,
                    ["floor_number"] = "1",
                    ["house_number"] = "101"
                }
            },
            Columns: new()
        );

        // Act
        var result = _mapper.FromCanonical(canonicalData);

        // Assert
        Assert.Single(result);
        Assert.Equal("room1", result[0].Id);
        Assert.Equal("Room 1", result[0].Name);
        Assert.Equal(2, result[0].Capacity);
    }

    [Fact]
    public void FromCanonical_AcceptsApartmentiTableName()
    {
        // Arrange - use hoteldruid Italian name
        var canonicalData = new CanonicalData(
            TableName: "appartamenti",
            RowCount: 1,
            Rows: new()
            {
                new()
                {
                    ["idappartamenti"] = "app1",
                    ["maxoccupanti"] = 2,
                    ["numpiano"] = "1"
                }
            },
            Columns: new()
        );

        // Act
        var result = _mapper.FromCanonical(canonicalData);

        // Assert
        Assert.Single(result);
        Assert.Equal("app1", result[0].Id);
        Assert.Equal(2, result[0].Capacity);
        Assert.Equal("1", result[0].FloorNumber);
    }

    [Fact]
    public void FromCanonical_HandlesFlexibleFieldNames()
    {
        // Arrange - mix of naming conventions
        var canonicalData = new CanonicalData(
            TableName: "rooms",
            RowCount: 1,
            Rows: new()
            {
                new()
                {
                    ["room_id"] = "r1",               // English variant
                    ["name"] = "Room 1",
                    ["max_occupancy"] = 3,           // English variant
                    ["floor"] = "2",                 // English variant
                    ["neighbors"] = "r2",            // English variant
                    ["notes"] = "Nice view"           // English variant
                }
            },
            Columns: new()
        );

        // Act
        var result = _mapper.FromCanonical(canonicalData);

        // Assert
        Assert.Single(result);
        Assert.Equal("r1", result[0].Id);
        Assert.Equal(3, result[0].Capacity);
        Assert.Equal("2", result[0].FloorNumber);
        Assert.Equal("r2", result[0].NeighboringRooms);
        Assert.Equal("Nice view", result[0].Comments);
    }

    [Fact]
    public void GetNormalizedValue_FindsValueByCanonicalName()
    {
        // Arrange
        var row = new Dictionary<string, object?> { ["id"] = "room1" };

        // Act
        var result = _mapper.GetNormalizedValue(row, "id");

        // Assert
        Assert.Equal("room1", result);
    }

    [Fact]
    public void GetNormalizedValue_FindsValueByAlias()
    {
        // Arrange
        var row = new Dictionary<string, object?> { ["idappartamenti"] = "app1" };

        // Act
        var result = _mapper.GetNormalizedValue(row, "id");

        // Assert
        Assert.Equal("app1", result);
    }

    [Fact]
    public void RoundTrip_PreservesAllFields()
    {
        // Arrange
        var originalRooms = new[]
        {
            new RoomDto(
                Id: "room1",
                Name: "Room 1",
                Capacity: 2,
                FloorNumber: "1",
                HouseNumber: "101",
                Priority: 1,
                SecondaryPriority: 2,
                HasBeds: "S",
                NeighboringRooms: "room2",
                Comments: "Corner room"
            )
        };

        // Act
        var canonical = _mapper.ToCanonical(originalRooms);
        var reconstructed = _mapper.FromCanonical(canonical);

        // Assert
        Assert.Single(reconstructed);
        Assert.Equal("room1", reconstructed[0].Id);
        Assert.Equal("Room 1", reconstructed[0].Name);
        Assert.Equal(2, reconstructed[0].Capacity);
        Assert.Equal("1", reconstructed[0].FloorNumber);
        Assert.Equal("101", reconstructed[0].HouseNumber);
        Assert.Equal(1, reconstructed[0].Priority);
        Assert.Equal(2, reconstructed[0].SecondaryPriority);
        Assert.Equal("S", reconstructed[0].HasBeds);
        Assert.Equal("room2", reconstructed[0].NeighboringRooms);
        Assert.Equal("Corner room", reconstructed[0].Comments);
    }
}
