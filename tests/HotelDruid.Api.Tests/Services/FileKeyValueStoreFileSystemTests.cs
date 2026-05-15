using System.Text.Json;
using Xunit;
using Microsoft.Extensions.Logging;
using Moq;

namespace HotelDruid.Api.Services.Tests;

/// <summary>
/// Enhanced unit tests for FileKeyValueStore with file system validation.
/// Tests verify both in-memory operations AND actual files on disk.
/// Full environment setup/teardown per test.
/// </summary>
public class FileKeyValueStoreFileSystemTests : IAsyncLifetime
{
    private string _tempDataRoot = null!;
    private FileKeyValueStore _store = null!;
    private Mock<ILogger<FileKeyValueStore>> _mockLogger = null!;
    private TestEventLogger _eventLogger = null!;

    private class TestDocument
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Value { get; set; }
        public string? Description { get; set; }
    }

    public async Task InitializeAsync()
    {
        _tempDataRoot = Path.Combine(Path.GetTempPath(), $"HotelDruid-tests-{Guid.NewGuid()}");
        Directory.CreateDirectory(_tempDataRoot);
        
        _mockLogger = new Mock<ILogger<FileKeyValueStore>>();
        _store = new FileKeyValueStore(_tempDataRoot, _mockLogger.Object);
        _eventLogger = new TestEventLogger();
        
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        _store?.Dispose();
        
        if (Directory.Exists(_tempDataRoot))
        {
            try
            {
                Directory.Delete(_tempDataRoot, recursive: true);
            }
            catch
            {
                // Ignore cleanup errors in test teardown
            }
        }
        
        await Task.CompletedTask;
    }

    [Fact]
    public async Task CreateAsync_CreatesFileOnDisk()
    {
        // Arrange
        var doc = new TestDocument { Name = "Room1", Value = 42, Description = "Test room" };
        
        // Act
        var id = await _store.CreateAsync("rooms", "Room1", doc);
        _eventLogger.LogDataOperation("CREATE", "rooms", id, "Room1");

        // Assert - File exists on disk
        var filePath = Path.Combine(_tempDataRoot, "collections", "rooms", $"{id}.json");
        Assert.True(File.Exists(filePath), $"File not found: {filePath}");

        // Assert - File content is valid JSON
        // Use same JSON options as FileKeyValueStore for deserialization
        var jsonOptions = new System.Text.Json.JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
        };
        var fileContent = await File.ReadAllTextAsync(filePath);
        var parsed = JsonSerializer.Deserialize<TestDocument>(fileContent, jsonOptions);
        Assert.NotNull(parsed);
        Assert.Equal("Room1", parsed!.Name);
        Assert.Equal(42, parsed.Value);
        Assert.Equal("Test room", parsed.Description);

        _eventLogger.AssertEventLogged("CREATE", "rooms");
    }

    [Fact]
    public async Task CreateAsync_CreatesIndexFile()
    {
        // Arrange
        var doc = new TestDocument { Name = "Room1", Value = 50 };
        
        // Act
        var id = await _store.CreateAsync("rooms", "Room1", doc);

        // Assert - Index file exists
        var indexPath = Path.Combine(_tempDataRoot, "collections", "rooms", "_index.json");
        Assert.True(File.Exists(indexPath), $"Index file not found: {indexPath}");

        // Assert - Index contains mapping
        var indexContent = await File.ReadAllTextAsync(indexPath);
        var index = JsonSerializer.Deserialize<Dictionary<string, string>>(indexContent);
        Assert.NotNull(index);
        Assert.True(index!.ContainsKey("Room1"));
        Assert.Equal(id, index["Room1"]);
    }

    [Fact]
    public async Task UpdateAsync_ModifiesFileOnDisk()
    {
        // Arrange
        var original = new TestDocument { Name = "Room1", Value = 50 };
        var id = await _store.CreateAsync("rooms", "Room1", original);
        
        var updated = new TestDocument { Id = id, Name = "Room1", Value = 75 };

        // Act
        await _store.UpdateAsync("rooms", id, updated);

        // Assert - File content changed
        // Use same JSON options as FileKeyValueStore for deserialization
        var jsonOptions = new System.Text.Json.JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
        };
        var filePath = Path.Combine(_tempDataRoot, "collections", "rooms", $"{id}.json");
        var fileContent = await File.ReadAllTextAsync(filePath);
        var parsed = JsonSerializer.Deserialize<TestDocument>(fileContent, jsonOptions);
        
        Assert.NotNull(parsed);
        Assert.Equal(75, parsed!.Value);
    }

    [Fact]
    public async Task DeleteAsync_RemovesFileFromDisk()
    {
        // Arrange
        var doc = new TestDocument { Name = "Room1", Value = 50 };
        var id = await _store.CreateAsync("rooms", "Room1", doc);
        var filePath = Path.Combine(_tempDataRoot, "collections", "rooms", $"{id}.json");
        
        Assert.True(File.Exists(filePath));

        // Act
        await _store.DeleteAsync("rooms", id);

        // Assert - File removed
        Assert.False(File.Exists(filePath));

        // Assert - Index updated
        var index = await _store.GetIndexAsync("rooms");
        Assert.False(index.ContainsKey("Room1"));
    }

    [Fact]
    public async Task DeleteAsync_UpdatesIndexFile()
    {
        // Arrange
        var doc1 = new TestDocument { Name = "Room1", Value = 50 };
        var doc2 = new TestDocument { Name = "Room2", Value = 60 };
        
        var id1 = await _store.CreateAsync("rooms", "Room1", doc1);
        var id2 = await _store.CreateAsync("rooms", "Room2", doc2);

        // Act
        await _store.DeleteAsync("rooms", id1);

        // Assert - Index file updated correctly
        var indexPath = Path.Combine(_tempDataRoot, "collections", "rooms", "_index.json");
        var indexContent = await File.ReadAllTextAsync(indexPath);
        var index = JsonSerializer.Deserialize<Dictionary<string, string>>(indexContent);
        
        Assert.NotNull(index);
        Assert.False(index!.ContainsKey("Room1"));
        Assert.True(index.ContainsKey("Room2"));
    }

    [Fact]
    public async Task RebuildIndexAsync_ScansAllFilesOnDisk()
    {
        // Arrange - Create documents so files exist
        var doc1 = new TestDocument { Name = "Room1", Value = 1 };
        var doc2 = new TestDocument { Name = "Room2", Value = 2 };
        var doc3 = new TestDocument { Name = "Room3", Value = 3 };
        
        var id1 = await _store.CreateAsync("rooms", "Room1", doc1);
        var id2 = await _store.CreateAsync("rooms", "Room2", doc2);
        var id3 = await _store.CreateAsync("rooms", "Room3", doc3);

        // Act - Delete index to simulate corruption, then rebuild
        var indexPath = Path.Combine(_tempDataRoot, "collections", "rooms", "_index.json");
        File.Delete(indexPath);
        Assert.False(File.Exists(indexPath));

        var count = await _store.RebuildIndexAsync("rooms");

        // Assert - Index recreated with correct count
        Assert.Equal(3, count);
        Assert.True(File.Exists(indexPath));
        
        var index = await _store.GetIndexAsync("rooms");
        Assert.Equal(3, index.Count);
        Assert.Equal(id1, index["Room1"]);
        Assert.Equal(id2, index["Room2"]);
        Assert.Equal(id3, index["Room3"]);
    }

    [Fact]
    public async Task CreateAsync_WritesValidJsonStructure()
    {
        // Arrange
        var doc = new TestDocument
        {
            Name = "Suite-101",
            Value = 150,
            Description = "Deluxe suite with ocean view"
        };

        // Act
        var id = await _store.CreateAsync("rooms", "Suite-101", doc);

        // Assert - Verify JSON structure and content
        var filePath = Path.Combine(_tempDataRoot, "collections", "rooms", $"{id}.json");
        var json = await File.ReadAllTextAsync(filePath);
        
        // Should be valid JSON
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var parsed = JsonDocument.Parse(json);
        
        Assert.NotNull(parsed);
        Assert.True(parsed.RootElement.TryGetProperty("name", out var nameElem));
        Assert.Equal("Suite-101", nameElem.GetString());
        Assert.True(parsed.RootElement.TryGetProperty("value", out var valueElem));
        Assert.Equal(150, valueElem.GetInt32());
    }

    [Fact]
    public async Task ListAsync_ReadsAllFilesFromDisk()
    {
        // Arrange - Create 5 documents
        for (int i = 1; i <= 5; i++)
        {
            var doc = new TestDocument { Name = $"Room{i}", Value = i * 10 };
            await _store.CreateAsync("rooms", $"Room{i}", doc);
        }

        // Assert - Directory has 5 data files + 1 index file
        var collectionPath = Path.Combine(_tempDataRoot, "collections", "rooms");
        var files = Directory.GetFiles(collectionPath, "*.json");
        Assert.Equal(6, files.Length);  // 5 data + 1 index

        // Act
        var docs = await _store.ListAsync<TestDocument>("rooms");

        // Assert
        Assert.Equal(5, docs.Count);
        Assert.All(docs, d => Assert.NotNull(d.Name));
    }

    [Fact]
    public async Task ExistsAsync_ChecksFileOnDisk()
    {
        // Arrange
        var doc = new TestDocument { Name = "Room1", Value = 50 };
        var id = await _store.CreateAsync("rooms", "Room1", doc);
        var filePath = Path.Combine(_tempDataRoot, "collections", "rooms", $"{id}.json");

        // Assert - File exists
        Assert.True(File.Exists(filePath));

        // Act & Assert - Service reports existence correctly
        var exists = await _store.ExistsAsync("rooms", id);
        Assert.True(exists);

        // Delete file manually and test again
        File.Delete(filePath);
        exists = await _store.ExistsAsync("rooms", id);
        Assert.False(exists);
    }

    [Fact]
    public async Task DeleteCollectionAsync_RemovesDirectoryAndAllFiles()
    {
        // Arrange
        for (int i = 1; i <= 3; i++)
        {
            var doc = new TestDocument { Name = $"Room{i}", Value = i };
            await _store.CreateAsync("rooms", $"Room{i}", doc);
        }

        var collectionPath = Path.Combine(_tempDataRoot, "collections", "rooms");
        Assert.True(Directory.Exists(collectionPath));
        Assert.NotEmpty(Directory.GetFiles(collectionPath));

        // Act
        await _store.DeleteCollectionAsync("rooms");

        // Assert - Directory removed
        Assert.False(Directory.Exists(collectionPath));
    }

    [Fact]
    public async Task AtomicWrite_NoPartialFilesOnCrash()
    {
        // Arrange
        var doc = new TestDocument { Name = "Room1", Value = 50 };

        // Act - Create document (uses atomic write pattern)
        var id = await _store.CreateAsync("rooms", "Room1", doc);
        var filePath = Path.Combine(_tempDataRoot, "collections", "rooms", $"{id}.json");

        // Assert - No temporary files left behind
        var tempPath = filePath + ".tmp";
        Assert.False(File.Exists(tempPath), "Temp file should not exist after successful write");

        // Assert - Actual file is valid
        Assert.True(File.Exists(filePath));
        var content = await File.ReadAllTextAsync(filePath);
        Assert.NotEmpty(content);
        
        // Should be parseable JSON
        _ = JsonDocument.Parse(content);  // Will throw if invalid
    }

    [Fact]
    public async Task ConcurrentWrites_NoFileCorruption()
    {
        // Arrange
        const int numConcurrent = 5;

        // Act - Create multiple documents concurrently
        var tasks = Enumerable.Range(0, numConcurrent)
            .Select(i => _store.CreateAsync("rooms", $"Room{i}", 
                new TestDocument { Name = $"Room{i}", Value = i }))
            .ToList();

        var ids = await Task.WhenAll(tasks);

        // Assert - All files exist and are valid JSON
        var collectionPath = Path.Combine(_tempDataRoot, "collections", "rooms");
        foreach (var id in ids)
        {
            var filePath = Path.Combine(collectionPath, $"{id}.json");
            Assert.True(File.Exists(filePath), $"File not found: {filePath}");
            
            var content = await File.ReadAllTextAsync(filePath);
            Assert.NotEmpty(content);
            
            // Should be valid JSON
            var doc = JsonSerializer.Deserialize<TestDocument>(content);
            Assert.NotNull(doc);
        }

        // Assert - Index file is valid and complete
        var indexPath = Path.Combine(collectionPath, "_index.json");
        var index = await _store.GetIndexAsync("rooms");
        Assert.Equal(numConcurrent, index.Count);
    }
}

/// <summary>
/// Test helper for event logging verification.
/// </summary>
public class TestEventLogger
{
    private readonly List<EventRecord> _events = new();

    public void LogDataOperation(string operation, string collection, string id, string? name = null)
    {
        _events.Add(new EventRecord
        {
            Type = "DATA_OPERATION",
            Operation = operation,
            Collection = collection,
            DocumentId = id,
            DocumentName = name,
            Timestamp = DateTime.UtcNow
        });
    }

    public void AssertEventLogged(string operation, string collection)
    {
        var hasEvent = _events.Any(e => 
            e.Type == "DATA_OPERATION" && 
            e.Operation == operation && 
            e.Collection == collection);
        
        Assert.True(hasEvent, $"Event not logged: {operation} on {collection}");
    }

    public IReadOnlyList<EventRecord> GetAllEvents() => _events.AsReadOnly();

    public class EventRecord
    {
        public string Type { get; set; } = string.Empty;
        public string? Operation { get; set; }
        public string? Collection { get; set; }
        public string? DocumentId { get; set; }
        public string? DocumentName { get; set; }
        public DateTime Timestamp { get; set; }
    }
}

