namespace HotelDruid.Api.Services;

public interface IEndpointQueryCache
{
    Task<T> GetOrCreateAsync<T>(string key, IReadOnlyCollection<string> tags, Func<Task<T>> factory);
    void InvalidateTag(string tag);
}
