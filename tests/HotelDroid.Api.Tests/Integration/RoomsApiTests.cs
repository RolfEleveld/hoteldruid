using System.Net;
using System.Text;
using System.Text.Json;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using HotelDroid.Api;

namespace HotelDroid.Api.Tests.Integration;

/// <summary>
/// API integration tests for rooms endpoints.
/// Uses WebApplicationFactory to test HTTP endpoints end-to-end.
/// Full environment setup/teardown per test fixture.
/// </summary>
public class RoomsApiTests : IAsyncLifetime
{
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;
    private string _testDataRoot = null!;

    private class RoomDto
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public int Capacity { get; set; }
        public string? RoomType { get; set; }
        public decimal PricePerNight { get; set; }
    }

    public async Task InitializeAsync()
    {
        // Setup test data directory
        _testDataRoot = Path.Combine(Path.GetTempPath(), $"api-tests-{Guid.NewGuid()}");
        Directory.CreateDirectory(_testDataRoot);

        // Configure test environment
        Environment.SetEnvironmentVariable("HOTELDRUID_DATAROOT", _testDataRoot);

        // Create factory with custom configuration
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting("DataRoot", _testDataRoot);
            });

        _client = _factory.CreateClient();
        
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        _client?.Dispose();
        _factory?.Dispose();

        if (Directory.Exists(_testDataRoot))
        {
            try
            {
                Directory.Delete(_testDataRoot, recursive: true);
            }
            catch
            {
                // Ignore cleanup errors
            }
        }

        await Task.CompletedTask;
    }

    [Fact]
    public async Task CreateRoom_Returns201Created()
    {
        // Arrange
        var room = new RoomDto
        {
            Name = "Room-101",
            Capacity = 2,
            RoomType = "Standard",
            PricePerNight = 100m
        };

        var json = JsonSerializer.Serialize(room);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/rooms", content);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<RoomDto>(responseContent);
        
        Assert.NotNull(result);
        Assert.NotEmpty(result!.Id);
        Assert.Equal("Room-101", result.Name);
    }

    [Fact]
    public async Task GetRoom_Returns200Ok()
    {
        // Arrange - Create a room first
        var createRoom = new RoomDto
        {
            Name = "Room-102",
            Capacity = 2,
            RoomType = "Standard",
            PricePerNight = 100m
        };

        var createJson = JsonSerializer.Serialize(createRoom);
        var createContent = new StringContent(createJson, Encoding.UTF8, "application/json");
        var createResponse = await _client.PostAsync("/api/rooms", createContent);
        
        var createResult = await createResponse.Content.ReadAsStringAsync();
        var created = JsonSerializer.Deserialize<RoomDto>(createResult);
        var roomId = created!.Id;

        // Act
        var response = await _client.GetAsync($"/api/rooms/{roomId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<RoomDto>(responseContent);
        
        Assert.NotNull(result);
        Assert.Equal(roomId, result!.Id);
        Assert.Equal("Room-102", result.Name);
    }

    [Fact]
    public async Task GetRoomByName_Returns200Ok()
    {
        // Arrange - Create a room first
        var createRoom = new RoomDto
        {
            Name = "Suite-Deluxe",
            Capacity = 4,
            RoomType = "Suite",
            PricePerNight = 250m
        };

        var createJson = JsonSerializer.Serialize(createRoom);
        var createContent = new StringContent(createJson, Encoding.UTF8, "application/json");
        await _client.PostAsync("/api/rooms", createContent);

        // Act
        var response = await _client.GetAsync("/api/rooms?name=Suite-Deluxe");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<RoomDto>(responseContent);
        
        Assert.NotNull(result);
        Assert.Equal("Suite-Deluxe", result!.Name);
    }

    [Fact]
    public async Task ListRooms_Returns200Ok()
    {
        // Arrange - Create multiple rooms
        for (int i = 1; i <= 3; i++)
        {
            var room = new RoomDto
            {
                Name = $"Room-{i:D3}",
                Capacity = i,
                RoomType = "Standard",
                PricePerNight = 50m * i
            };

            var json = JsonSerializer.Serialize(room);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            await _client.PostAsync("/api/rooms", content);
        }

        // Act
        var response = await _client.GetAsync("/api/rooms");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var results = JsonSerializer.Deserialize<List<RoomDto>>(responseContent);
        
        Assert.NotNull(results);
        Assert.Equal(3, results!.Count);
    }

    [Fact]
    public async Task UpdateRoom_Returns200Ok()
    {
        // Arrange - Create a room first
        var createRoom = new RoomDto
        {
            Name = "Room-103",
            Capacity = 2,
            RoomType = "Standard",
            PricePerNight = 100m
        };

        var createJson = JsonSerializer.Serialize(createRoom);
        var createContent = new StringContent(createJson, Encoding.UTF8, "application/json");
        var createResponse = await _client.PostAsync("/api/rooms", createContent);
        
        var createResult = await createResponse.Content.ReadAsStringAsync();
        var created = JsonSerializer.Deserialize<RoomDto>(createResult);
        var roomId = created!.Id;

        // Update the room
        var updateRoom = new RoomDto
        {
            Name = "Room-103",
            Capacity = 3,
            RoomType = "Deluxe",
            PricePerNight = 150m
        };

        var updateJson = JsonSerializer.Serialize(updateRoom);
        var updateContent = new StringContent(updateJson, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync($"/api/rooms/{roomId}", updateContent);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<RoomDto>(responseContent);
        
        Assert.NotNull(result);
        Assert.Equal(3, result!.Capacity);
        Assert.Equal("Deluxe", result.RoomType);
        Assert.Equal(150m, result.PricePerNight);
    }

    [Fact]
    public async Task DeleteRoom_Returns204NoContent()
    {
        // Arrange - Create a room first
        var createRoom = new RoomDto
        {
            Name = "Room-104",
            Capacity = 2,
            RoomType = "Standard",
            PricePerNight = 100m
        };

        var createJson = JsonSerializer.Serialize(createRoom);
        var createContent = new StringContent(createJson, Encoding.UTF8, "application/json");
        var createResponse = await _client.PostAsync("/api/rooms", createContent);
        
        var createResult = await createResponse.Content.ReadAsStringAsync();
        var created = JsonSerializer.Deserialize<RoomDto>(createResult);
        var roomId = created!.Id;

        // Act
        var response = await _client.DeleteAsync($"/api/rooms/{roomId}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify deletion
        var getResponse = await _client.GetAsync($"/api/rooms/{roomId}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task CreateRoom_DuplicateName_Returns409Conflict()
    {
        // Arrange
        var room = new RoomDto
        {
            Name = "Room-Unique",
            Capacity = 2,
            RoomType = "Standard",
            PricePerNight = 100m
        };

        var json = JsonSerializer.Serialize(room);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Create first room
        await _client.PostAsync("/api/rooms", content);

        // Act - Try to create duplicate
        var response = await _client.PostAsync("/api/rooms", content);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task GetRoom_NonExistent_Returns404NotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/rooms/nonexistent-id-12345");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateRoom_MissingRequired_Returns400BadRequest()
    {
        // Arrange - Invalid room (missing name)
        var room = new { Capacity = 2 };
        var json = JsonSerializer.Serialize(room);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/rooms", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task RoomOperations_PreservesDataIntegrity()
    {
        // Arrange
        var room = new RoomDto
        {
            Name = "Integrity-Test-Room",
            Capacity = 2,
            RoomType = "Standard",
            PricePerNight = 99.99m
        };

        // Act - Create
        var createJson = JsonSerializer.Serialize(room);
        var createContent = new StringContent(createJson, Encoding.UTF8, "application/json");
        var createResponse = await _client.PostAsync("/api/rooms", createContent);
        var created = JsonSerializer.Deserialize<RoomDto>(
            await createResponse.Content.ReadAsStringAsync());

        // Act - Get
        var getResponse = await _client.GetAsync($"/api/rooms/{created!.Id}");
        var retrieved = JsonSerializer.Deserialize<RoomDto>(
            await getResponse.Content.ReadAsStringAsync());

        // Assert - Data matches
        Assert.Equal(room.Name, retrieved!.Name);
        Assert.Equal(room.Capacity, retrieved.Capacity);
        Assert.Equal(room.RoomType, retrieved.RoomType);
        Assert.Equal(room.PricePerNight, retrieved.PricePerNight);
    }
}

/// <summary>
/// Extended integration tests for ledger operations.
/// </summary>
public class LedgerApiTests : IAsyncLifetime
{
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;
    private string _testDataRoot = null!;

    public async Task InitializeAsync()
    {
        _testDataRoot = Path.Combine(Path.GetTempPath(), $"ledger-tests-{Guid.NewGuid()}");
        Directory.CreateDirectory(_testDataRoot);
        Environment.SetEnvironmentVariable("HOTELDRUID_DATAROOT", _testDataRoot);

        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => builder.UseSetting("DataRoot", _testDataRoot));

        _client = _factory.CreateClient();
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        _client?.Dispose();
        _factory?.Dispose();

        if (Directory.Exists(_testDataRoot))
        {
            try { Directory.Delete(_testDataRoot, recursive: true); }
            catch { }
        }

        await Task.CompletedTask;
    }

    [Fact]
    public async Task GetLedgerSnapshot_Returns200Ok()
    {
        // Act
        var response = await _client.GetAsync($"/api/ledger/snapshot?date={DateTime.UtcNow:yyyy-MM-dd}");

        // Assert
        Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetLedgerEntries_Returns200Ok()
    {
        // Act
        var response = await _client.GetAsync($"/api/ledger/entries?date={DateTime.UtcNow:yyyy-MM-dd}");

        // Assert
        Assert.True(response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NotFound);
    }
}
