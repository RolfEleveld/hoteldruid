using HotelDroid.Api.Models;
using HotelDroid.Api.Services;
using HotelDroid.Shared;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HotelDroid.Api.Tests.Services;

/// <summary>
/// Unit tests for RoomRepository.
/// Uses MockFileKeyValueStore (already defined in Helpers.cs) so no disk I/O.
/// </summary>
public class RoomRepositoryTests
{
    private readonly RoomRepository _repo;
    private readonly MockFileKeyValueStore _mockStore;
    private readonly Mock<ILogger<RoomRepository>> _loggerMock;

    public RoomRepositoryTests()
    {
        _mockStore = new MockFileKeyValueStore();
        _loggerMock = new Mock<ILogger<RoomRepository>>();
        _repo = new RoomRepository(_mockStore, _loggerMock.Object);
    }

    // ─── CreateAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_ValidRoom_ReturnsNonEmptyId()
    {
        var room = new RoomDto(null, "101", 2);
        var id = await _repo.CreateAsync(room);
        Assert.NotEmpty(id);
    }

    [Fact]
    public async Task CreateAsync_DuplicateName_ThrowsInvalidOperationException()
    {
        var room = new RoomDto(null, "101", 2);
        await _repo.CreateAsync(room);
        await Assert.ThrowsAsync<InvalidOperationException>(() => _repo.CreateAsync(room));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task CreateAsync_EmptyName_ThrowsArgumentException(string? name)
    {
        var room = new RoomDto(null, name!, 2);
        await Assert.ThrowsAsync<ArgumentException>(() => _repo.CreateAsync(room));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(101)]
    public async Task CreateAsync_InvalidCapacity_ThrowsArgumentException(int capacity)
    {
        var room = new RoomDto(null, "Room-1", capacity);
        await Assert.ThrowsAsync<ArgumentException>(() => _repo.CreateAsync(room));
    }

    [Theory]
    [InlineData("S")]
    [InlineData("N")]
    [InlineData(null)]
    public async Task CreateAsync_ValidHasBeds_Succeeds(string? hasBeds)
    {
        var room = new RoomDto(null, $"Room-hasbeds-{hasBeds ?? "null"}", 2, HasBeds: hasBeds);
        var id = await _repo.CreateAsync(room);
        Assert.NotEmpty(id);
    }

    [Fact]
    public async Task CreateAsync_InvalidHasBeds_ThrowsArgumentException()
    {
        var room = new RoomDto(null, "Room-X", 2, HasBeds: "Y");
        await Assert.ThrowsAsync<ArgumentException>(() => _repo.CreateAsync(room));
    }

    // ─── GetByIdAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_ExistingRoom_ReturnsRoom()
    {
        var room = new RoomDto(null, "Suite-A", 4, FloorNumber: "3");
        var id = await _repo.CreateAsync(room);

        var found = await _repo.GetByIdAsync(id);

        Assert.NotNull(found);
        Assert.Equal("Suite-A", found!.Name);
        Assert.Equal(4, found.Capacity);
        Assert.Equal("3", found.FloorNumber);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistentId_ReturnsNull()
    {
        var result = await _repo.GetByIdAsync("nonexistent-id");
        Assert.Null(result);
    }

    // ─── GetByNameAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task GetByNameAsync_ExistingName_ReturnsRoom()
    {
        var room = new RoomDto(null, "Room-42", 3);
        await _repo.CreateAsync(room);

        var found = await _repo.GetByNameAsync("Room-42");

        Assert.NotNull(found);
        Assert.Equal("Room-42", found!.Name);
        Assert.Equal(3, found.Capacity);
    }

    [Fact]
    public async Task GetByNameAsync_NonExistentName_ReturnsNull()
    {
        var result = await _repo.GetByNameAsync("Nonexistent Room");
        Assert.Null(result);
    }

    // ─── UpdateAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_ExistingRoom_PersistsChanges()
    {
        var room = new RoomDto(null, "Room-U", 2);
        var id = await _repo.CreateAsync(room);

        var updated = new RoomDto(id, "Room-U", 5, FloorNumber: "2");
        await _repo.UpdateAsync(id, updated);

        var found = await _repo.GetByIdAsync(id);
        Assert.Equal(5, found!.Capacity);
        Assert.Equal("2", found.FloorNumber);
    }

    [Fact]
    public async Task UpdateAsync_NonExistentId_ThrowsKeyNotFoundException()
    {
        var room = new RoomDto("fake-id", "Room-X", 2);
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _repo.UpdateAsync("fake-id", room));
    }

    [Fact]
    public async Task UpdateAsync_InvalidCapacity_ThrowsArgumentException()
    {
        var room = new RoomDto(null, "Room-V", 2);
        var id = await _repo.CreateAsync(room);

        var badUpdate = new RoomDto(id, "Room-V", 0);
        await Assert.ThrowsAsync<ArgumentException>(() => _repo.UpdateAsync(id, badUpdate));
    }

    // ─── DeleteAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_ExistingRoom_RemovesIt()
    {
        var room = new RoomDto(null, "Room-D", 1);
        var id = await _repo.CreateAsync(room);

        await _repo.DeleteAsync(id);

        var found = await _repo.GetByIdAsync(id);
        Assert.Null(found);
    }

    [Fact]
    public async Task DeleteAsync_NonExistentId_ThrowsKeyNotFoundException()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _repo.DeleteAsync("nonexistent"));
    }

    // ─── ListAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task ListAsync_NoRooms_ReturnsEmptyList()
    {
        var rooms = await _repo.ListAsync();
        Assert.Empty(rooms);
    }

    [Fact]
    public async Task ListAsync_MultipleRooms_ReturnsAll()
    {
        await _repo.CreateAsync(new RoomDto(null, "Room-L1", 1));
        await _repo.CreateAsync(new RoomDto(null, "Room-L2", 2));
        await _repo.CreateAsync(new RoomDto(null, "Room-L3", 3));

        var rooms = await _repo.ListAsync();

        Assert.Equal(3, rooms.Count);
        Assert.Contains(rooms, r => r.Name == "Room-L1");
        Assert.Contains(rooms, r => r.Name == "Room-L2");
        Assert.Contains(rooms, r => r.Name == "Room-L3");
    }

    // ─── Boundary values ────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_MinCapacity_Succeeds()
    {
        var room = new RoomDto(null, "Room-Min", 1);
        var id = await _repo.CreateAsync(room);
        Assert.NotEmpty(id);
    }

    [Fact]
    public async Task CreateAsync_MaxCapacity_Succeeds()
    {
        var room = new RoomDto(null, "Room-Max", 100);
        var id = await _repo.CreateAsync(room);
        Assert.NotEmpty(id);
    }

    [Fact]
    public async Task CreateAsync_AllOptionalFields_RoundTrips()
    {
        var room = new RoomDto(
            null, "Room-Full", 3,
            FloorNumber: "2",
            HouseNumber: "B",
            Priority: 5,
            SecondaryPriority: 2,
            HasBeds: "S",
            NeighboringRooms: "Room-101,Room-103",
            Comments: "Corner room with extra light"
        );
        var id = await _repo.CreateAsync(room);
        var found = await _repo.GetByIdAsync(id);

        Assert.NotNull(found);
        Assert.Equal("2", found!.FloorNumber);
        Assert.Equal("B", found.HouseNumber);
        Assert.Equal(5, found.Priority);
        Assert.Equal(2, found.SecondaryPriority);
        Assert.Equal("S", found.HasBeds);
        Assert.Equal("Room-101,Room-103", found.NeighboringRooms);
        Assert.Equal("Corner room with extra light", found.Comments);
    }
}
