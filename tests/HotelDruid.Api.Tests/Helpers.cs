using Microsoft.Extensions.Logging;
using HotelDruid.Api.Services;
using Xunit.Abstractions;

namespace HotelDruid.Api.Tests;

/// <summary>
/// Helper methods for unit tests
/// </summary>
public static class LoggingHelper
{
    public static ILogger<T> CreateLogger<T>()
    {
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddDebug();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        return loggerFactory.CreateLogger<T>();
    }

    public static ILogger<T> CreateLogger<T>(ITestOutputHelper testOutput)
    {
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddDebug();
            builder.SetMinimumLevel(LogLevel.Debug);
        });

        return loggerFactory.CreateLogger<T>();
    }
}

/// <summary>
/// Mock implementations for unit tests
/// </summary>
public class MockFileKeyValueStore : IKeyValueStore
{
    // Collection name -> Dictionary of {ID -> JSON}
    private readonly Dictionary<string, Dictionary<string, string>> _collections = new();
    // Collection name -> Dictionary of {Name -> ID} (index)
    private readonly Dictionary<string, Dictionary<string, string>> _indexes = new();

    public async Task<T?> GetAsync<T>(string collectionName, string id) where T : class
    {
        if (_collections.TryGetValue(collectionName, out var collection))
        {
            if (collection.TryGetValue(id, out var value))
            {
                return System.Text.Json.JsonSerializer.Deserialize<T>(value);
            }
        }
        return null;
    }

    public async Task<T?> GetByNameAsync<T>(string collectionName, string name) where T : class
    {
        if (_indexes.TryGetValue(collectionName, out var index))
        {
            if (index.TryGetValue(name, out var id))
            {
                return await GetAsync<T>(collectionName, id);
            }
        }
        return null;
    }

    public async Task<List<T>> ListAsync<T>(string collectionName) where T : class
    {
        var result = new List<T>();
        if (_collections.TryGetValue(collectionName, out var collection))
        {
            foreach (var value in collection.Values)
            {
                var item = System.Text.Json.JsonSerializer.Deserialize<T>(value);
                if (item != null)
                    result.Add(item);
            }
        }
        return result;
    }

    public async Task<string> CreateAsync<T>(string collectionName, string name, T document) where T : class
    {
        if (!_collections.ContainsKey(collectionName))
        {
            _collections[collectionName] = new();
            _indexes[collectionName] = new();
        }

        // Check if name already exists
        if (_indexes[collectionName].ContainsKey(name))
        {
            throw new InvalidOperationException($"Name '{name}' already exists in collection '{collectionName}'");
        }

        var id = Guid.NewGuid().ToString();
        var json = System.Text.Json.JsonSerializer.Serialize(document);
        _collections[collectionName][id] = json;
        _indexes[collectionName][name] = id;  // Add to index
        return id;
    }

    public async Task UpdateAsync<T>(string collectionName, string id, T document) where T : class
    {
        if (_collections.TryGetValue(collectionName, out var collection))
        {
            var json = System.Text.Json.JsonSerializer.Serialize(document);
            collection[id] = json;
        }
    }

    public async Task DeleteAsync(string collectionName, string id)
    {
        if (_collections.TryGetValue(collectionName, out var collection))
        {
            collection.Remove(id);
            // Also remove from index
            if (_indexes.TryGetValue(collectionName, out var index))
            {
                var nameToRemove = index.FirstOrDefault(kvp => kvp.Value == id).Key;
                if (nameToRemove != null)
                {
                    index.Remove(nameToRemove);
                }
            }
        }
    }

    public async Task<bool> ExistsAsync(string collectionName, string id)
    {
        return _collections.TryGetValue(collectionName, out var collection) && collection.ContainsKey(id);
    }

    public async Task DeleteCollectionAsync(string collectionName)
    {
        _collections.Remove(collectionName);
        _indexes.Remove(collectionName);
    }

    public async Task<int> RebuildIndexAsync(string collectionName)
    {
        return _collections.TryGetValue(collectionName, out var collection) ? collection.Count : 0;
    }

    public async Task<Dictionary<string, string>> GetIndexAsync(string collectionName)
    {
        return _indexes.TryGetValue(collectionName, out var index) 
            ? new Dictionary<string, string>(index) 
            : new Dictionary<string, string>();
    }
}

