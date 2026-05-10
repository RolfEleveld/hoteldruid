using HotelDroid.Shared;

namespace HotelDroid.Api.Services;

/// <summary>
/// Repository for managing Room entities.
/// Wraps IKeyValueStore with Room-specific validation and mapping.
/// </summary>
public interface IRoomRepository
{
    /// <summary>Get a room by its GUID-base32 ID.</summary>
    Task<RoomDto?> GetByIdAsync(string id);

    /// <summary>Get a room by its human-readable name (e.g., "Room-101").</summary>
    Task<RoomDto?> GetByNameAsync(string name);

    /// <summary>
    /// Create a new room.
    /// </summary>
    /// <returns>The generated GUID-base32 ID.</returns>
    /// <exception cref="InvalidOperationException">If a room with the same name already exists.</exception>
    /// <exception cref="ArgumentException">If the room data fails validation.</exception>
    Task<string> CreateAsync(RoomDto room);

    /// <summary>
    /// Update an existing room by ID.
    /// </summary>
    /// <exception cref="KeyNotFoundException">If no room with the given ID exists.</exception>
    /// <exception cref="ArgumentException">If the room data fails validation.</exception>
    Task UpdateAsync(string id, RoomDto room);

    /// <summary>
    /// Delete a room by ID.
    /// </summary>
    /// <exception cref="KeyNotFoundException">If no room with the given ID exists.</exception>
    Task DeleteAsync(string id);

    /// <summary>List all rooms.</summary>
    Task<List<RoomDto>> ListAsync();

    /// <summary>Rebuild the name → ID index by scanning all data files.</summary>
    Task<int> RebuildIndexAsync();
}
