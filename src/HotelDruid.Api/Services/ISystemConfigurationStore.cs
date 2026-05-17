using System.Threading.Tasks;
using HotelDruid.Shared.Configuration;

namespace HotelDruid.Api.Services
{
    public interface ISystemConfigurationStore
    {
        Task<SystemConfiguration?> GetAsync();
        Task SaveAsync(SystemConfiguration config);
        Task DeleteAsync();
    }
}
