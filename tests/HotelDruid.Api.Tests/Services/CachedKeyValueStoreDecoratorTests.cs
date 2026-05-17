using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HotelDruid.Api.Services.Tests;

/// <summary>
/// Functional tests for CachedKeyValueStoreDecorator.
/// Verifies cache correctness: invalidation on mutations, consistency, etc.
/// </summary>
public class CachedKeyValueStoreDecoratorTests : IAsyncLifetime
{
    private string _tempDataRoot = null!;
    private FileKeyValueStore _innerStore = null!;
    private CachedKeyValueStoreDecorator _cachedStore = null!;
    private Mock<ILogger<FileKeyValueStore>> _mockLogger1 = null!;
    private Mock<ILogger<CachedKeyValueStoreDecorator>> _mockLogger2 = null!;

    public async Task InitializeAsync()
    {
        _tempDataRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempDataRoot);
        
        _mockLogger1 = new Mock<ILogger<FileKeyValueStore>>();
        _mockLogger2 = new Mock<ILogger<CachedKeyValueStoreDecorator>>();

        _innerStore = new FileKeyValueStore(_tempDataRoot, _mockLogger1.Object);
        _cachedStore = new CachedKeyValueStoreDecorator(_innerStore, _mockLogger2.Object);
        
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        _innerStore?.Dispose();
        
        if (Directory.Exists(_tempDataRoot))
        {
            Directory.Delete(_tempDataRoot, recursive: true);
        }
        
        await Task.CompletedTask;
    }

    private class TestDocument
    {
        public string? Name { get; set; }
        public int Value { get; set; }
    }

    [Fact]
    public async Task GetAsync_CachesDocument()
    {
        // Arrange
        var doc = new TestDocument { Name = "Test", Value = 42 };
        var id = await _cachedStore.CreateAsync("test", "Test", doc);

        // Act: First call (cache miss)
        var result1 = await _cachedStore.GetAsync<TestDocument>("test", id);

        // Act: Second call (cache hit)
        var result2 = await _cachedStore.GetAsync<TestDocument>("test", id);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(result1.Value, result2.Value);
    }

    [Fact]
    public async Task GetIndexAsync_CachesIndex()
    {
        // Arrange
        await _cachedStore.CreateAsync("test", "Doc1", new TestDocument { Value = 1 });
        await _cachedStore.CreateAsync("test", "Doc2", new TestDocument { Value = 2 });

        // Act: First call (cache miss)
        var idx1 = await _cachedStore.GetIndexAsync("test");

        // Act: Second call (cache hit)
        var idx2 = await _cachedStore.GetIndexAsync("test");

        // Assert
        Assert.Equal(2, idx1.Count);
        Assert.Equal(2, idx2.Count);
        Assert.Equal(idx1, idx2);
    }

    [Fact]
    public async Task CreateAsync_InvalidatesCollectionCache()
    {
        // Arrange
        await _cachedStore.CreateAsync("test", "Doc1", new TestDocument { Value = 1 });
        var idx1 = await _cachedStore.GetIndexAsync("test");
        Assert.Single(idx1);

        // Act: Create another document (should invalidate cache)
        await _cachedStore.CreateAsync("test", "Doc2", new TestDocument { Value = 2 });
        var idx2 = await _cachedStore.GetIndexAsync("test");

        // Assert: New index is loaded from disk
        Assert.Equal(2, idx2.Count);
    }

    [Fact]
    public async Task UpdateAsync_InvalidatesCollectionCache()
    {
        // Arrange
        var doc = new TestDocument { Name = "Original", Value = 1 };
        var id = await _cachedStore.CreateAsync("test", "Test", doc);
        
        // Load and cache
        var cached1 = await _cachedStore.GetAsync<TestDocument>("test", id);
        Assert.Equal(1, cached1!.Value);

        // Act: Update document (should invalidate cache)
        var updated = new TestDocument { Name = "Updated", Value = 2 };
        await _cachedStore.UpdateAsync("test", id, updated);

        // Act: Retrieve again (should be fresh from disk)
        var cached2 = await _cachedStore.GetAsync<TestDocument>("test", id);

        // Assert
        Assert.Equal(2, cached2!.Value);
    }

    [Fact]
    public async Task DeleteAsync_InvalidatesCollectionCache()
    {
        // Arrange
        var id = await _cachedStore.CreateAsync("test", "Test", new TestDocument { Value = 1 });
        
        // Load and cache
        var doc1 = await _cachedStore.GetAsync<TestDocument>("test", id);
        Assert.NotNull(doc1);

        // Act: Delete document (should invalidate cache)
        await _cachedStore.DeleteAsync("test", id);

        // Act: Try to retrieve (should return null)
        var doc2 = await _cachedStore.GetAsync<TestDocument>("test", id);

        // Assert
        Assert.Null(doc2);
    }

    [Fact]
    public async Task DeleteCollectionAsync_InvalidatesCollectionCache()
    {
        // Arrange
        await _cachedStore.CreateAsync("test", "Doc1", new TestDocument { Value = 1 });
        var idx1 = await _cachedStore.GetIndexAsync("test");
        Assert.Single(idx1);

        // Act: Delete entire collection (should invalidate cache)
        await _cachedStore.DeleteCollectionAsync("test");
        var idx2 = await _cachedStore.GetIndexAsync("test");

        // Assert: Index is empty
        Assert.Empty(idx2);
    }

    [Fact]
    public async Task RebuildIndexAsync_InvalidatesCollectionCache()
    {
        // Arrange
        await _cachedStore.CreateAsync("test", "Doc1", new TestDocument { Name = "Doc1", Value = 1 });
        var idx1 = await _cachedStore.GetIndexAsync("test");
        Assert.Single(idx1);

        // Act: Rebuild index (should invalidate cache)
        var count = await _cachedStore.RebuildIndexAsync("test");
        var idx2 = await _cachedStore.GetIndexAsync("test");

        // Assert
        Assert.Equal(1, count);
        Assert.Single(idx2);
    }

    [Fact]
    public async Task GetByNameAsync_UsesCache()
    {
        // Arrange
        await _cachedStore.CreateAsync("test", "TestDoc", new TestDocument { Value = 42 });

        // Act: First call (cache miss on index)
        var doc1 = await _cachedStore.GetByNameAsync<TestDocument>("test", "TestDoc");

        // Act: Second call (cache hit on both index and document)
        var doc2 = await _cachedStore.GetByNameAsync<TestDocument>("test", "TestDoc");

        // Assert
        Assert.NotNull(doc1);
        Assert.NotNull(doc2);
        Assert.Equal(doc1.Value, doc2.Value);
    }

    [Fact]
    public async Task ExistsAsync_ChecksCache()
    {
        // Arrange
        var doc = new TestDocument { Value = 1 };
        var id = await _cachedStore.CreateAsync("test", "Test", doc);

        // Act: Load into cache
        _ = await _cachedStore.GetAsync<TestDocument>("test", id);

        // Act: Check existence (should find in cache)
        var exists = await _cachedStore.ExistsAsync("test", id);

        // Assert
        Assert.True(exists);
    }

    [Fact]
    public async Task ListAsync_UsesCachedIndexAndDocuments()
    {
        // Arrange
        for (int i = 1; i <= 5; i++)
        {
            await _cachedStore.CreateAsync("test", $"Doc{i}", new TestDocument { Value = i });
        }

        // Act: First list (all cache misses)
        var list1 = await _cachedStore.ListAsync<TestDocument>("test");

        // Act: Second list (all cache hits)
        var list2 = await _cachedStore.ListAsync<TestDocument>("test");

        // Assert
        Assert.Equal(5, list1.Count);
        Assert.Equal(5, list2.Count);
        Assert.Equal(list1.OrderBy(x => x.Value).Select(x => x.Value),
                     list2.OrderBy(x => x.Value).Select(x => x.Value));
    }

    [Fact]
    public async Task CacheConsistency_AfterMultipleMutations()
    {
        // Arrange
        var id1 = await _cachedStore.CreateAsync("test", "Doc1", new TestDocument { Value = 1 });
        var id2 = await _cachedStore.CreateAsync("test", "Doc2", new TestDocument { Value = 2 });
        
        // Load all into cache
        var list1 = await _cachedStore.ListAsync<TestDocument>("test");
        Assert.Equal(2, list1.Count);

        // Act: Update first document
        await _cachedStore.UpdateAsync("test", id1, new TestDocument { Value = 10 });
        var updated = await _cachedStore.GetAsync<TestDocument>("test", id1);

        // Act: Delete second document
        await _cachedStore.DeleteAsync("test", id2);
        var deleted = await _cachedStore.GetAsync<TestDocument>("test", id2);

        // Act: Create third document
        var id3 = await _cachedStore.CreateAsync("test", "Doc3", new TestDocument { Value = 3 });
        var created = await _cachedStore.GetAsync<TestDocument>("test", id3);

        // Act: Final list
        var list2 = await _cachedStore.ListAsync<TestDocument>("test");

        // Assert
        Assert.NotNull(updated);
        Assert.Equal(10, updated.Value);
        Assert.Null(deleted);
        Assert.NotNull(created);
        Assert.Equal(2, list2.Count);
    }

    [Fact]
    public async Task CrossInstance_Update_InvalidatesOtherInstanceCache()
    {
        // Arrange: two independent store+cache instances sharing the same data root
        var loggerStoreA = new Mock<ILogger<FileKeyValueStore>>();
        var loggerStoreB = new Mock<ILogger<FileKeyValueStore>>();
        var loggerCacheA = new Mock<ILogger<CachedKeyValueStoreDecorator>>();
        var loggerCacheB = new Mock<ILogger<CachedKeyValueStoreDecorator>>();

        using var storeA = new FileKeyValueStore(_tempDataRoot, loggerStoreA.Object);
        using var storeB = new FileKeyValueStore(_tempDataRoot, loggerStoreB.Object);

        var cacheA = new CachedKeyValueStoreDecorator(
            storeA,
            loggerCacheA.Object,
            invalidationRootPath: _tempDataRoot,
            versionCheckIntervalMilliseconds: 0);
        var cacheB = new CachedKeyValueStoreDecorator(
            storeB,
            loggerCacheB.Object,
            invalidationRootPath: _tempDataRoot,
            versionCheckIntervalMilliseconds: 0);

        var id = await cacheA.CreateAsync("test", "SharedDoc", new TestDocument { Value = 1 });

        // Prime cacheB with stale value
        var initial = await cacheB.GetAsync<TestDocument>("test", id);
        Assert.NotNull(initial);
        Assert.Equal(1, initial!.Value);

        // Act: mutate from cacheA and read from cacheB
        await cacheA.UpdateAsync("test", id, new TestDocument { Value = 2 });
        var refreshed = await cacheB.GetAsync<TestDocument>("test", id);

        // Assert: cacheB should observe updated value after cross-instance invalidation
        Assert.NotNull(refreshed);
        Assert.Equal(2, refreshed!.Value);
    }

    [Fact]
    public async Task CrossInstance_Create_InvalidatesOtherInstanceIndexCache()
    {
        // Arrange
        var loggerStoreA = new Mock<ILogger<FileKeyValueStore>>();
        var loggerStoreB = new Mock<ILogger<FileKeyValueStore>>();
        var loggerCacheA = new Mock<ILogger<CachedKeyValueStoreDecorator>>();
        var loggerCacheB = new Mock<ILogger<CachedKeyValueStoreDecorator>>();

        using var storeA = new FileKeyValueStore(_tempDataRoot, loggerStoreA.Object);
        using var storeB = new FileKeyValueStore(_tempDataRoot, loggerStoreB.Object);

        var cacheA = new CachedKeyValueStoreDecorator(
            storeA,
            loggerCacheA.Object,
            invalidationRootPath: _tempDataRoot,
            versionCheckIntervalMilliseconds: 0);
        var cacheB = new CachedKeyValueStoreDecorator(
            storeB,
            loggerCacheB.Object,
            invalidationRootPath: _tempDataRoot,
            versionCheckIntervalMilliseconds: 0);

        await cacheA.CreateAsync("test", "Doc1", new TestDocument { Name = "Doc1", Value = 1 });

        // Prime cacheB index
        var indexBefore = await cacheB.GetIndexAsync("test");
        Assert.Single(indexBefore);

        // Act: create from other instance and read index from cacheB
        await cacheA.CreateAsync("test", "Doc2", new TestDocument { Name = "Doc2", Value = 2 });
        var indexAfter = await cacheB.GetIndexAsync("test");

        // Assert
        Assert.Equal(2, indexAfter.Count);
        Assert.Contains("Doc2", indexAfter.Keys);
    }

    [Fact]
    public async Task CrossInstance_Delete_InvalidatesOtherInstanceDocumentCache()
    {
        // Arrange
        var loggerStoreA = new Mock<ILogger<FileKeyValueStore>>();
        var loggerStoreB = new Mock<ILogger<FileKeyValueStore>>();
        var loggerCacheA = new Mock<ILogger<CachedKeyValueStoreDecorator>>();
        var loggerCacheB = new Mock<ILogger<CachedKeyValueStoreDecorator>>();

        using var storeA = new FileKeyValueStore(_tempDataRoot, loggerStoreA.Object);
        using var storeB = new FileKeyValueStore(_tempDataRoot, loggerStoreB.Object);

        var cacheA = new CachedKeyValueStoreDecorator(
            storeA,
            loggerCacheA.Object,
            invalidationRootPath: _tempDataRoot,
            versionCheckIntervalMilliseconds: 0);
        var cacheB = new CachedKeyValueStoreDecorator(
            storeB,
            loggerCacheB.Object,
            invalidationRootPath: _tempDataRoot,
            versionCheckIntervalMilliseconds: 0);

        var id = await cacheA.CreateAsync("test", "Doc1", new TestDocument { Name = "Doc1", Value = 1 });

        // Prime cacheB with document value
        var beforeDelete = await cacheB.GetAsync<TestDocument>("test", id);
        Assert.NotNull(beforeDelete);

        // Act
        await cacheA.DeleteAsync("test", id);
        var afterDelete = await cacheB.GetAsync<TestDocument>("test", id);

        // Assert
        Assert.Null(afterDelete);
    }

    [Fact]
    public async Task Cache_HandlesMissingDocument()
    {
        // Act
        var doc = await _cachedStore.GetAsync<TestDocument>("test", "nonexistent");

        // Assert
        Assert.Null(doc);
    }

    [Fact]
    public async Task Cache_HandlesMissingName()
    {
        // Act
        var doc = await _cachedStore.GetByNameAsync<TestDocument>("test", "nonexistent");

        // Assert
        Assert.Null(doc);
    }
}
