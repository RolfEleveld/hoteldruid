using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HotelDruid.Client.Services
{
    public interface IUserPrivilegeApiService
    {
        Task<List<UserPrivilegeDto>> GetByUserAsync(int userId, int? year = null);
        Task<UserPrivilegeDto?> GetByUserAndYearAsync(int userId, int year);
        Task<string> CreateAsync(UserPrivilegeDto dto);
        Task UpdateAsync(int userId, int year, UserPrivilegeDto dto);
        Task DeleteAsync(int userId, int year);
    }

    public record UserPrivilegeDto(
        string? Id,
        int? UserId,
        int? Year,
        string? AllowedRules = null,
        string? AllowedTariffs = null,
        string? AllowedExtraCosts = null,
        string? AllowedContracts = null,
        string? AllowedCashRegisters = null,
        string? PaymentCashRegister = null,
        string? BookingInsertPriv = null,
        string? BookingModifyPriv = null,
        string? PersonalSettingsPriv = null,
        string? ClientInsertPriv = null,
        string? ClientPrefix = null,
        string? CostInsertPriv = null,
        string? ViewTablePriv = null,
        string? TariffInsertPriv = null,
        string? RuleInsertPriv = null,
        string? MessagePriv = null,
        string? InventoryPriv = null
    );

    public class UserPrivilegeApiService : IUserPrivilegeApiService
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

        public UserPrivilegeApiService(HttpClient httpClient) { _httpClient = httpClient; }

        public async Task<List<UserPrivilegeDto>> GetByUserAsync(int userId, int? year = null)
        {
            try
            {
                var url = year.HasValue ? $"/api/user-privileges?userId={userId}&year={year}" : $"/api/user-privileges?userId={userId}";
                var resp = await _httpClient.GetAsync(url);
                resp.EnsureSuccessStatusCode();
                return JsonSerializer.Deserialize<List<UserPrivilegeDto>>(await resp.Content.ReadAsStringAsync(), _json) ?? new();
            }
            catch { return new List<UserPrivilegeDto>(); }
        }

        public async Task<UserPrivilegeDto?> GetByUserAndYearAsync(int userId, int year)
        {
            try
            {
                var resp = await _httpClient.GetAsync($"/api/user-privileges/{userId}/{year}");
                resp.EnsureSuccessStatusCode();
                return JsonSerializer.Deserialize<UserPrivilegeDto>(await resp.Content.ReadAsStringAsync(), _json);
            }
            catch { return null; }
        }

        public async Task<string> CreateAsync(UserPrivilegeDto dto)
        {
            var content = new StringContent(JsonSerializer.Serialize(dto, _json), Encoding.UTF8, "application/json");
            var resp = await _httpClient.PostAsync("/api/user-privileges", content);
            resp.EnsureSuccessStatusCode();
            var created = JsonSerializer.Deserialize<UserPrivilegeDto>(await resp.Content.ReadAsStringAsync(), _json);
            return created?.Id ?? string.Empty;
        }

        public async Task UpdateAsync(int userId, int year, UserPrivilegeDto dto)
        {
            var content = new StringContent(JsonSerializer.Serialize(dto, _json), Encoding.UTF8, "application/json");
            var resp = await _httpClient.PutAsync($"/api/user-privileges/{userId}/{year}", content);
            resp.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(int userId, int year)
        {
            var resp = await _httpClient.DeleteAsync($"/api/user-privileges/{userId}/{year}");
            resp.EnsureSuccessStatusCode();
        }
    }
}

