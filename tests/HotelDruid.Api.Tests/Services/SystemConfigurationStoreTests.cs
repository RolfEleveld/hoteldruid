using System.Threading.Tasks;
using HotelDruid.Api.Services;
using HotelDruid.Shared.Configuration;
using Xunit;

namespace HotelDruid.Api.Tests.Services;

public class SystemConfigurationStoreTests
{
    [Fact]
    public async Task Can_Save_And_Get_Configuration()
    {
        var store = new SystemConfigurationStore(new MockFileKeyValueStore());
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
        var store = new SystemConfigurationStore(new MockFileKeyValueStore());
        var loaded = await store.GetAsync();
        Assert.Null(loaded);
    }

    [Fact]
    public async Task Can_Delete_Configuration()
    {
        var store = new SystemConfigurationStore(new MockFileKeyValueStore());
        var config = new SystemConfiguration { DefaultCurrency = "EUR" };
        await store.SaveAsync(config);
        await store.DeleteAsync();
        var loaded = await store.GetAsync();
        Assert.Null(loaded);
    }
}
