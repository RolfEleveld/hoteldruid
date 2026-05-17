using System.Threading.Tasks;
using HotelDruid.Shared.Configuration;

namespace HotelDruid.Api.Services
{
    public class SystemConfigurationStore : ISystemConfigurationStore
    {
        private readonly IKeyValueStore _store;
        private const string Collection = "system_configuration";
        private const string ConfigName = "system";

        public SystemConfigurationStore(IKeyValueStore store)
        {
            _store = store;
        }

        public async Task<SystemConfiguration?> GetAsync()
        {
            var id = await TryGetInternalIdAsync();
            if (id is null)
                return null;

            return await _store.GetAsync<SystemConfiguration>(Collection, id);
        }

        public async Task SaveAsync(SystemConfiguration config)
        {
            config.Id = ConfigName;
            config.UpdatedUtc = System.DateTime.UtcNow;
            var existingId = await TryGetInternalIdAsync();
            if (!string.IsNullOrWhiteSpace(existingId))
            {
                await _store.UpdateAsync(Collection, existingId, config);
                return;
            }

            await _store.CreateAsync(Collection, ConfigName, config);
        }

        public async Task DeleteAsync()
        {
            var existingId = await TryGetInternalIdAsync();
            if (!string.IsNullOrWhiteSpace(existingId))
            {
                await _store.DeleteAsync(Collection, existingId);
            }
        }

        private async Task<string?> TryGetInternalIdAsync()
        {
            var index = await _store.GetIndexAsync(Collection);
            if (!index.TryGetValue(ConfigName, out var id) || string.IsNullOrWhiteSpace(id))
                return null;

            return id;
        }
    }
}
