namespace HotelDroid.Api.Models;

public record ClientDto(int Id, string Name, string? Email, string? Phone);
public record BookingDto(int Id, int ClientId, string ArrivalDate, string DepartureDate, string Status);

/// <summary>
/// Room/Apartment Data Transfer Object.
/// Maps to hoteldruid 'appartamenti' table.
/// 
/// Property Mapping (hoteldruid database → API):
/// - Name ← idappartamenti (PRIMARY KEY: Room identifier, e.g., "101", "Suite-A")
/// - FloorNumber ← numpiano (Physical floor, e.g., "1", "2", "Ground")
/// - Capacity ← maxoccupanti (Max occupants as INTEGER, e.g., 2, 4)
/// - HouseNumber ← numcasa (House/building section, e.g., "A", "B1")
/// - Priority ← priorita (Booking priority INTEGER, lower=higher priority)
/// - SecondaryPriority ← priorita2 (Bed assignment priority)
/// - HasBeds ← letto (VARCHAR(1), "S"=yes, "N"=no)
/// - NeighboringRooms ← app_vicini (Comma-separated IDs of adjacent rooms)
/// - Comments ← commento (Interior/metadata notes)
/// </summary>
public record RoomDto(
    string? Id, 
    string Name, 
    int Capacity, 
    string? FloorNumber = null,
    string? HouseNumber = null,
    int? Priority = null,
    int? SecondaryPriority = null,
    string? HasBeds = null,
    string? NeighboringRooms = null,
    string? Comments = null
);

/// <summary>
/// Internal storage model for rooms.
/// Explicit field definitions to avoid JSON deserialization defaults.
/// </summary>
public class RoomStorageModel
{
    public string? Name { get; set; }
    public int? Capacity { get; set; }
    public string? FloorNumber { get; set; }
    public string? HouseNumber { get; set; }
    public int? Priority { get; set; }
    public int? SecondaryPriority { get; set; }
    public string? HasBeds { get; set; }
    public string? NeighboringRooms { get; set; }
    public string? Comments { get; set; }
}
