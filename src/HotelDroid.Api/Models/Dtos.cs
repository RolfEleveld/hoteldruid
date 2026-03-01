using HotelDroid.Shared;

namespace HotelDroid.Api.Models;

// DTOs are now defined in HotelDroid.Shared.Dtos.cs for shared use between Client and API

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
