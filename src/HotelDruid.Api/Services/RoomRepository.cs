using HotelDruid.Api.Models;
using HotelDruid.Shared;

namespace HotelDruid.Api.Services;

/// <summary>
/// File-backed implementation of IRoomRepository.
/// Delegates persistence to IKeyValueStore and enforces Room business rules.
///
/// Validation rules:
///   - Name is required and must be unique
///   - Capacity must be between 1 and 100 (inclusive)
///   - HasBeds must be null, "S" (yes), or "N" (no) when provided
/// </summary>
public class RoomRepository : IRoomRepository
{
    private const string Collection = "rooms";
    private const int MaxCapacity = 100;
    private const int MinCapacity = 1;

    private readonly IKeyValueStore _store;
    private readonly ILogger<RoomRepository> _logger;

    public RoomRepository(IKeyValueStore store, ILogger<RoomRepository> logger)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<RoomDto?> GetByIdAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Room ID cannot be empty", nameof(id));

        var model = await _store.GetAsync<RoomStorageModel>(Collection, id);
        if (model is null)
            return null;

        return ToDto(id, model);
    }

    /// <inheritdoc />
    public async Task<RoomDto?> GetByNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Room name cannot be empty", nameof(name));

        var index = await _store.GetIndexAsync(Collection);
        if (!index.TryGetValue(name, out var id))
            return null;

        var model = await _store.GetAsync<RoomStorageModel>(Collection, id);
        if (model is null)
            return null;

        return ToDto(id, model);
    }

    /// <inheritdoc />
    public async Task<string> CreateAsync(RoomDto room)
    {
        ValidateRoom(room);

        var model = ToStorageModel(room);

        try
        {
            var id = await _store.CreateAsync(Collection, room.Name, model);
            _logger.LogInformation("Room created: {Name} ({Id})", room.Name, id);
            return id;
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
        {
            throw new InvalidOperationException($"A room named '{room.Name}' already exists.", ex);
        }
    }

    /// <inheritdoc />
    public async Task UpdateAsync(string id, RoomDto room)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Room ID cannot be empty", nameof(id));

        ValidateRoom(room);

        var exists = await _store.ExistsAsync(Collection, id);
        if (!exists)
            throw new KeyNotFoundException($"Room with ID '{id}' not found.");

        var model = ToStorageModel(room);
        await _store.UpdateAsync(Collection, id, model);
        _logger.LogInformation("Room updated: {Id} ({Name})", id, room.Name);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Room ID cannot be empty", nameof(id));

        var exists = await _store.ExistsAsync(Collection, id);
        if (!exists)
            throw new KeyNotFoundException($"Room with ID '{id}' not found.");

        await _store.DeleteAsync(Collection, id);
        _logger.LogInformation("Room deleted: {Id}", id);
    }

    /// <inheritdoc />
    public async Task<List<RoomDto>> ListAsync()
    {
        var models = await _store.ListAsync<RoomStorageModel>(Collection);
        var index = await _store.GetIndexAsync(Collection);

        // Build reverse index: name → id
        var nameToId = index.ToDictionary(kv => kv.Key, kv => kv.Value);

        var result = new List<RoomDto>(models.Count);
        foreach (var model in models)
        {
            var name = model.Name ?? string.Empty;
            var id = nameToId.TryGetValue(name, out var foundId) ? foundId : string.Empty;
            result.Add(ToDto(id, model));
        }

        return result;
    }

    /// <inheritdoc />
    public async Task<int> RebuildIndexAsync()
    {
        return await _store.RebuildIndexAsync(Collection);
    }

    // ─── Private helpers ────────────────────────────────────────────────────

    private static void ValidateRoom(RoomDto room)
    {
        if (room is null)
            throw new ArgumentNullException(nameof(room));
        if (string.IsNullOrWhiteSpace(room.Name))
            throw new ArgumentException("Room name is required.", nameof(room));
        if (room.Capacity < MinCapacity || room.Capacity > MaxCapacity)
            throw new ArgumentException(
                $"Capacity must be between {MinCapacity} and {MaxCapacity}.", nameof(room));
        if (room.HasBeds is not null && room.HasBeds != "S" && room.HasBeds != "N")
            throw new ArgumentException(
                "HasBeds must be null, \"S\" (yes), or \"N\" (no).", nameof(room));
    }

    private static RoomStorageModel ToStorageModel(RoomDto room) => new()
    {
        Name = room.Name,
        Capacity = room.Capacity,
        FloorNumber = room.FloorNumber,
        HouseNumber = room.HouseNumber,
        Priority = room.Priority,
        SecondaryPriority = room.SecondaryPriority,
        HasBeds = room.HasBeds,
        NeighboringRooms = room.NeighboringRooms,
        Comments = room.Comments
    };

    private static RoomDto ToDto(string id, RoomStorageModel model) => new(
        id,
        model.Name ?? string.Empty,
        model.Capacity ?? 0,
        model.FloorNumber,
        model.HouseNumber,
        model.Priority,
        model.SecondaryPriority,
        model.HasBeds,
        model.NeighboringRooms,
        model.Comments
    );
}

