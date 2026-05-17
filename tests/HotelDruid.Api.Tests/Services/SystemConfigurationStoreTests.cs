using System.Threading.Tasks;
using HotelDruid.Api.Services;
using HotelDruid.Shared.Configuration;
using Xunit;

public class SystemConfigurationStoreTests
{
    private class InMemoryKeyValueStore : IKeyValueStore
    {
        private readonly Dictionary<string, object> _dict = new();
        public Task<T?> GetAsync<T>(string key) where T : class
        {
            if (_dict.TryGetValue(key, out var value))
                return Task.FromResult(value as T);
            return Task.FromResult<T?>(null);
        }
        public Task SetAsync<T>(string key, T value) where T : class
        {
            _dict[key] = value;
            return Task.CompletedTask;
        }
        public Task DeleteAsync(string key)
        {
            _dict.Remove(key);
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task Can_Save_And_Get_Configuration()
    {
        var store = new SystemConfigurationStore(new InMemoryKeyValueStore());
        var config = new SystemConfiguration { DefaultCurrency = "USD", DefaultYear = 2026 };
        await store.SaveAsync(config);
        var loaded = await store.GetAsync();
        Assert.NotNull(loaded);
        Assert.Equal("USD", loaded.DefaultCurrency);
        Assert.Equal(2026, loaded.DefaultYear);
    }

    [Fact]
    public async Task Returns_Null_If_Not_Exists()
    {
        var store = new SystemConfigurationStore(new InMemoryKeyValueStore());
        var loaded = await store.GetAsync();
        Assert.Null(loaded);
    }

    [Fact]
    public async Task Can_Delete_Configuration()
    {
        var store = new SystemConfigurationStore(new InMemoryKeyValueStore());
        var config = new SystemConfiguration { DefaultCurrency = "EUR" };
        await store.SaveAsync(config);
        await store.DeleteAsync();
        var loaded = await store.GetAsync();
        Assert.Null(loaded);
    }
}
