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

public class AssetStorageModel
{
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? HostCreated { get; set; }
    public int? CreatedBy { get; set; }
}

public class WarehouseStorageModel
{
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? Description { get; set; }
    public string? FloorNumber { get; set; }
    public string? HouseNumber { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? HostCreated { get; set; }
    public int? CreatedBy { get; set; }
}

public class InventoryStorageModel
{
    // Reference to Asset (id in assets collection)
    public string? AssetId { get; set; }

    // Either RoomId (appartamento) or WarehouseId (magazzino)
    public string? RoomId { get; set; }
    public string? WarehouseId { get; set; }

    public int Quantity { get; set; }
    public int? MinQuantityDefault { get; set; }
    public bool? RequiredOnCheckin { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? HostCreated { get; set; }
    public int? CreatedBy { get; set; }
}

public record AssetDto(string? Id, string? Name, string? Code, string? Description, DateTime? CreatedAt);
public record WarehouseDto(string? Id, string? Name, string? Code, string? Description, string? FloorNumber, string? HouseNumber, DateTime? CreatedAt);
public record InventoryDto(string? Id, string? AssetId, string? RoomId, string? WarehouseId, int Quantity, int? MinQuantityDefault, bool? RequiredOnCheckin, DateTime? CreatedAt);
