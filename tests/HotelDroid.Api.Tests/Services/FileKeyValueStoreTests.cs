using Xunit;
using Microsoft.Extensions.Logging;
using Moq;

namespace HotelDroid.Api.Services.Tests;

/// <summary>
/// Unit tests for FileKeyValueStore.
/// Tests concurrency, atomicity, CRUD, index management.
/// </summary>
public class FileKeyValueStoreTests : IAsyncLifetime
{
    private string _tempDataRoot = null!;
    private FileKeyValueStore _store = null!;
    private Mock<ILogger<FileKeyValueStore>> _mockLogger = null!;

    // Test model
    private class TestDocument
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }
    }

    public async Task InitializeAsync()
    {
        _tempDataRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempDataRoot);
        
        _mockLogger = new Mock<ILogger<FileKeyValueStore>>();
        _store = new FileKeyValueStore(_tempDataRoot, _mockLogger.Object);
        
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        _store?.Dispose();
        
        if (Directory.Exists(_tempDataRoot))
        {
            Directory.Delete(_tempDataRoot, recursive: true);
        }
        
        await Task.CompletedTask;
    }

    [Fact]
    public async Task CreateAsync_GeneratesUniqueId()
    {
        // Arrange
        var doc1 = new TestDocument { Name = "Doc1", Value = 1 };
        var doc2 = new TestDocument { Name = "Doc2", Value = 2 };

        // Act
        var id1 = await _store.CreateAsync("test", "Doc1", doc1);
        var id2 = await _store.CreateAsync("test", "Doc2", doc2);

        // Assert
        Assert.NotEqual(id1, id2);
        Assert.Equal(26, id1.Length);  // Base32 GUID should be 26 chars
        Assert.Equal(26, id2.Length);
    }

    [Fact]
    public async Task CreateAsync_ThrowsWhenNameExists()
    {
        // Arrange
        var doc = new TestDocument { Name = "Room1", Value = 1 };
        await _store.CreateAsync("rooms", "Room1", doc);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _store.CreateAsync("rooms", "Room1", doc));
    }

    [Fact]
    public async Task GetAsync_ReturnsDocument()
    {
        // Arrange
        var original = new TestDocument { Name = "TestDoc", Value = 42 };
        var id = await _store.CreateAsync("test", "TestDoc", original);

        // Act
        var retrieved = await _store.GetAsync<TestDocument>("test", id);

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal("TestDoc", retrieved!.Name);
        Assert.Equal(42, retrieved.Value);
    }

    [Fact]
    public async Task GetAsync_ReturnsNullWhenNotFound()
    {
        // Act
        var result = await _store.GetAsync<TestDocument>("test", "nonexistent");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByNameAsync_ReturnsDocument()
    {
        // Arrange
        var original = new TestDocument { Name = "Room1", Value = 100 };
        await _store.CreateAsync("rooms", "Room1", original);

        // Act
        var retrieved = await _store.GetByNameAsync<TestDocument>("rooms", "Room1");

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal("Room1", retrieved!.Name);
        Assert.Equal(100, retrieved.Value);
    }

    [Fact]
    public async Task UpdateAsync_ModifiesDocument()
    {
        // Arrange
        var original = new TestDocument { Name = "Room1", Value = 50 };
        var id = await _store.CreateAsync("rooms", "Room1", original);
        
        var updated = new TestDocument { Id = id, Name = "Room1", Value = 75 };

        // Act
        await _store.UpdateAsync("rooms", id, updated);
        var retrieved = await _store.GetAsync<TestDocument>("rooms", id);

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal(75, retrieved!.Value);
    }

    [Fact]
    public async Task UpdateAsync_ThrowsWhenNotFound()
    {
        // Arrange
        var doc = new TestDocument { Name = "Room1", Value = 50 };

        // Act & Assert
        await Assert.ThrowsAsync<FileNotFoundException>(
            () => _store.UpdateAsync("rooms", "nonexistent", doc));
    }

    [Fact]
    public async Task DeleteAsync_RemovesDocument()
    {
        // Arrange
        var doc = new TestDocument { Name = "Room1", Value = 50 };
        var id = await _store.CreateAsync("rooms", "Room1", doc);

        // Act
        await _store.DeleteAsync("rooms", id);
        var retrieved = await _store.GetAsync<TestDocument>("rooms", id);

        // Assert
        Assert.Null(retrieved);
    }

    [Fact]
    public async Task DeleteAsync_RemovesFromIndex()
    {
        // Arrange
        var doc = new TestDocument { Name = "Room1", Value = 50 };
        var id = await _store.CreateAsync("rooms", "Room1", doc);

        // Act
        await _store.DeleteAsync("rooms", id);
        var retrieved = await _store.GetByNameAsync<TestDocument>("rooms", "Room1");

        // Assert
        Assert.Null(retrieved);
    }

    [Fact]
    public async Task ListAsync_ReturnsAllDocuments()
    {
        // Arrange
        for (int i = 1; i <= 5; i++)
        {
            var doc = new TestDocument { Name = $"Room{i}", Value = i };
            await _store.CreateAsync("rooms", $"Room{i}", doc);
        }

        // Act
        var list = await _store.ListAsync<TestDocument>("rooms");

        // Assert
        Assert.Equal(5, list.Count);
        Assert.All(list, doc => Assert.NotNull(doc.Name));
    }

    [Fact]
    public async Task ListAsync_ReturnsEmptyWhenCollectionNotFound()
    {
        // Act
        var list = await _store.ListAsync<TestDocument>("nonexistent");

        // Assert
        Assert.Empty(list);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsTrueForExistingDocument()
    {
        // Arrange
        var doc = new TestDocument { Name = "Room1", Value = 50 };
        var id = await _store.CreateAsync("rooms", "Room1", doc);

        // Act
        var exists = await _store.ExistsAsync("rooms", id);

        // Assert
        Assert.True(exists);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsFalseForMissingDocument()
    {
        // Act
        var exists = await _store.ExistsAsync("rooms", "nonexistent");

        // Assert
        Assert.False(exists);
    }

    [Fact]
    public async Task RebuildIndexAsync_ReconstructsIndexFromFiles()
    {
        // Arrange - create documents directly
        var collectionPath = Path.Combine(_tempDataRoot, "collections", "rooms");
        Directory.CreateDirectory(collectionPath);
        
        var id1 = IdGenerator.GenerateId();
        var id2 = IdGenerator.GenerateId();
        
        var doc1 = new TestDocument { Id = id1, Name = "Room1", Value = 1 };
        var doc2 = new TestDocument { Id = id2, Name = "Room2", Value = 2 };
        
        // Use same JSON options as FileKeyValueStore for consistency
        var jsonOptions = new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
        };
        
        await File.WriteAllTextAsync(
            Path.Combine(collectionPath, $"{id1}.json"),
            System.Text.Json.JsonSerializer.Serialize(doc1, jsonOptions));
        await File.WriteAllTextAsync(
            Path.Combine(collectionPath, $"{id2}.json"),
            System.Text.Json.JsonSerializer.Serialize(doc2, jsonOptions));

        // Act
        var count = await _store.RebuildIndexAsync("rooms");

        // Assert
        Assert.Equal(2, count);
        
        var index = await _store.GetIndexAsync("rooms");
        Assert.Equal(2, index.Count);
        Assert.True(index.ContainsKey("Room1"));
        Assert.True(index.ContainsKey("Room2"));
    }

    [Fact]
    public async Task GetIndexAsync_ReturnsIndexMapping()
    {
        // Arrange
        var doc1 = new TestDocument { Name = "Room1", Value = 1 };
        var doc2 = new TestDocument { Name = "Room2", Value = 2 };
        
        var id1 = await _store.CreateAsync("rooms", "Room1", doc1);
        var id2 = await _store.CreateAsync("rooms", "Room2", doc2);

        // Act
        var index = await _store.GetIndexAsync("rooms");

        // Assert
        Assert.Equal(2, index.Count);
        Assert.Equal(id1, index["Room1"]);
        Assert.Equal(id2, index["Room2"]);
    }

    [Fact]
    public async Task DeleteCollectionAsync_RemovesEntireCollection()
    {
        // Arrange
        for (int i = 1; i <= 3; i++)
        {
            var doc = new TestDocument { Name = $"Room{i}", Value = i };
            await _store.CreateAsync("rooms", $"Room{i}", doc);
        }

        // Act
        await _store.DeleteCollectionAsync("rooms");
        var list = await _store.ListAsync<TestDocument>("rooms");

        // Assert
        Assert.Empty(list);
    }

    [Fact]
    public async Task ConcurrentWrites_SerializeCorrectly()
    {
        // Arrange
        const int concurrentCount = 10;
        var tasks = new List<Task<string>>();

        // Act - Create multiple documents concurrently
        for (int i = 0; i < concurrentCount; i++)
        {
            var doc = new TestDocument { Name = $"Room{i}", Value = i };
            tasks.Add(_store.CreateAsync("rooms", $"Room{i}", doc));
        }

        var ids = await Task.WhenAll(tasks);

        // Assert - All should be unique and exist
        Assert.Equal(concurrentCount, ids.Length);
        Assert.Equal(concurrentCount, ids.Distinct().Count());  // All unique
        
        var list = await _store.ListAsync<TestDocument>("rooms");
        Assert.Equal(concurrentCount, list.Count);
    }

    [Fact]
    public async Task ConcurrentUpdates_PreventCorruption()
    {
        // Arrange
        var doc = new TestDocument { Name = "Room1", Value = 0 };
        var id = await _store.CreateAsync("rooms", "Room1", doc);

        // Act - Concurrent updates
        var tasks = new List<Task>();
        for (int i = 1; i <= 5; i++)
        {
            int value = i;  // Capture the value to avoid closure bug
            tasks.Add(Task.Run(async () =>
            {
                var updated = new TestDocument { Id = id, Name = "Room1", Value = value };
                await _store.UpdateAsync("rooms", id, updated);
            }));
        }

        await Task.WhenAll(tasks);
        var final = await _store.GetAsync<TestDocument>("rooms", id);

        // Assert - Final value should be one of the updates (no corruption)
        Assert.NotNull(final);
        Assert.True(final!.Value >= 1 && final.Value <= 5);
    }

    [Fact]
    public void IdGenerator_GeneratesValidIds()
    {
        // Act
        var id1 = IdGenerator.GenerateId();
        var id2 = IdGenerator.GenerateId();

        // Assert
        Assert.NotEqual(id1, id2);
        Assert.True(IdGenerator.IsValidId(id1));
        Assert.True(IdGenerator.IsValidId(id2));
        Assert.Equal(26, id1.Length);
        Assert.Equal(26, id2.Length);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("../../etc/passwd")]
    [InlineData("rooms/../../etc/passwd")]
    public async Task ValidateCollectionName_RejectsInvalid(string collectionName)
    {
        // Arrange
        var doc = new TestDocument { Name = "Test", Value = 1 };

        // Act & Assert
        if (collectionName == null)
        {
            await Assert.ThrowsAsync<ArgumentException>(
                () => _store.CreateAsync(collectionName!, "Test", doc));
        }
        else if (string.IsNullOrWhiteSpace(collectionName))
        {
            await Assert.ThrowsAsync<ArgumentException>(
                () => _store.CreateAsync(collectionName, "Test", doc));
        }
        else
        {
            await Assert.ThrowsAsync<ArgumentException>(
                () => _store.CreateAsync(collectionName, "Test", doc));
        }
    }
}
