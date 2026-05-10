using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HotelDroid.Client.Services
{
    public interface ITariffApiService
    {
        Task<List<TariffDto>> ListAsync(int? year = null);
        Task<TariffDto?> GetAsync(int year, string id);
        Task<string> CreateAsync(TariffDto dto);
        Task UpdateAsync(int year, string id, TariffDto dto);
        Task DeleteAsync(int year, string id);
    }

    public record TariffDto(
        string? Id = null,
        int Year = 0,
        string? ExtraCostName = null,
        string? CostType = null,
        double? BaseValue = null,
        double? PercentageValue = null,
        double? TaxPercentage = null,
        string? Category = null,
        int? NumberLimit = null
    );

    public class TariffApiService : ITariffApiService
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

        public TariffApiService(HttpClient httpClient) { _httpClient = httpClient; }

        public async Task<List<TariffDto>> ListAsync(int? year = null)
        {
            try
            {
                var url = year.HasValue ? $"/api/tariffs?year={year}" : "/api/tariffs";
                var resp = await _httpClient.GetAsync(url);
                resp.EnsureSuccessStatusCode();
                return JsonSerializer.Deserialize<List<TariffDto>>(await resp.Content.ReadAsStringAsync(), _json) ?? new();
            }
            catch { return new List<TariffDto>(); }
        }

        public async Task<TariffDto?> GetAsync(int year, string id)
        {
            try
            {
                var resp = await _httpClient.GetAsync($"/api/tariffs/{year}/{id}");
                resp.EnsureSuccessStatusCode();
                return JsonSerializer.Deserialize<TariffDto>(await resp.Content.ReadAsStringAsync(), _json);
            }
            catch { return null; }
        }

        public async Task<string> CreateAsync(TariffDto dto)
        {
            var content = new StringContent(JsonSerializer.Serialize(dto, _json), Encoding.UTF8, "application/json");
            var resp = await _httpClient.PostAsync("/api/tariffs", content);
            resp.EnsureSuccessStatusCode();
            var created = JsonSerializer.Deserialize<TariffDto>(await resp.Content.ReadAsStringAsync(), _json);
            return created?.Id ?? string.Empty;
        }

        public async Task UpdateAsync(int year, string id, TariffDto dto)
        {
            var content = new StringContent(JsonSerializer.Serialize(dto, _json), Encoding.UTF8, "application/json");
            var resp = await _httpClient.PutAsync($"/api/tariffs/{year}/{id}", content);
            resp.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(int year, string id)
        {
            var resp = await _httpClient.DeleteAsync($"/api/tariffs/{year}/{id}");
            resp.EnsureSuccessStatusCode();
        }
    }
}
