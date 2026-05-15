namespace HotelDruid.Api.Services;

/// <summary>
/// Generic key-value store interface for managing collections of JSON documents.
/// Supports CRUD operations on collections with automatic ID generation and index management.
/// </summary>
public interface IKeyValueStore
{
    /// <summary>
    /// Get a document by ID from a collection.
    /// </summary>
    /// <param name="collectionName">Name of the collection (e.g., "rooms", "guests")</param>
    /// <param name="id">GUID-base32 ID of the document</param>
    /// <returns>Deserialized document or null if not found</returns>
    Task<T?> GetAsync<T>(string collectionName, string id) where T : class;

    /// <summary>
    /// Get a document by human-readable name (via index lookup).
    /// </summary>
    /// <param name="collectionName">Name of the collection</param>
    /// <param name="name">Human-readable name (e.g., "Room-1")</param>
    /// <returns>Deserialized document or null if not found</returns>
    Task<T?> GetByNameAsync<T>(string collectionName, string name) where T : class;

    /// <summary>
    /// List all documents in a collection.
    /// </summary>
    /// <param name="collectionName">Name of the collection</param>
    /// <returns>List of all documents, empty list if none</returns>
    Task<List<T>> ListAsync<T>(string collectionName) where T : class;

    /// <summary>
    /// Create a new document with auto-generated ID.
    /// Updates the index with the provided name mapping.
    /// </summary>
    /// <param name="collectionName">Name of the collection</param>
    /// <param name="name">Human-readable name for the index</param>
    /// <param name="document">Document object to store</param>
    /// <returns>Generated GUID-base32 ID</returns>
    /// <exception cref="InvalidOperationException">If name already exists in index</exception>
    Task<string> CreateAsync<T>(string collectionName, string name, T document) where T : class;

    /// <summary>
    /// Update an existing document by ID.
    /// </summary>
    /// <param name="collectionName">Name of the collection</param>
    /// <param name="id">GUID-base32 ID of the document</param>
    /// <param name="document">Updated document object</param>
    /// <exception cref="FileNotFoundException">If document doesn't exist</exception>
    Task UpdateAsync<T>(string collectionName, string id, T document) where T : class;

    /// <summary>
    /// Delete a document by ID.
    /// </summary>
    /// <param name="collectionName">Name of the collection</param>
    /// <param name="id">GUID-base32 ID of the document</param>
    /// <exception cref="FileNotFoundException">If document doesn't exist</exception>
    Task DeleteAsync(string collectionName, string id);

    /// <summary>
    /// Check if a document exists by ID.
    /// </summary>
    /// <param name="collectionName">Name of the collection</param>
    /// <param name="id">GUID-base32 ID of the document</param>
    /// <returns>True if document exists, false otherwise</returns>
    Task<bool> ExistsAsync(string collectionName, string id);

    /// <summary>
    /// Delete all documents in a collection and remove the collection directory.
    /// </summary>
    /// <param name="collectionName">Name of the collection</param>
    Task DeleteCollectionAsync(string collectionName);

    /// <summary>
    /// Rebuild the index for a collection by scanning all data files.
    /// </summary>
    /// <param name="collectionName">Name of the collection</param>
    /// <returns>Number of entries in rebuilt index</returns>
    Task<int> RebuildIndexAsync(string collectionName);

    /// <summary>
    /// Get the index for a collection (name → GUID mapping).
    /// </summary>
    /// <param name="collectionName">Name of the collection</param>
    /// <returns>Dictionary mapping names to GUIDs, or empty if collection doesn't exist</returns>
    Task<Dictionary<string, string>> GetIndexAsync(string collectionName);
}

