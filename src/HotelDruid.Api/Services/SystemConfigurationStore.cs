using System.Threading.Tasks;
using HotelDruid.Shared.Configuration;

namespace HotelDruid.Api.Services
{
    public class SystemConfigurationStore : ISystemConfigurationStore
    {
        private readonly IKeyValueStore _store;
        private const string ConfigKey = "system-configuration";

        public SystemConfigurationStore(IKeyValueStore store)
        {
            _store = store;
        }

        public async Task<SystemConfiguration?> GetAsync()
        {
            return await _store.GetAsync<SystemConfiguration>(ConfigKey);
        }

        public async Task SaveAsync(SystemConfiguration config)
        {
            config.UpdatedUtc = System.DateTime.UtcNow;
            await _store.SetAsync(ConfigKey, config);
        }

        public async Task DeleteAsync()
        {
            await _store.DeleteAsync(ConfigKey);
        }
    }
}
