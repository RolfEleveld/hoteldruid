using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HotelDruid.Client.Services
{
    public interface IExtraCostApiService
    {
        Task<List<ExtraCostDto>> ListAsync(int? year = null);
        Task<ExtraCostDto?> GetAsync(int year, string id);
        Task<string> CreateAsync(ExtraCostDto dto);
        Task UpdateAsync(int year, string id, ExtraCostDto dto);
        Task DeleteAsync(int year, string id);
    }

    public record ExtraCostDto(
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

    public class ExtraCostApiService : IExtraCostApiService
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

        public ExtraCostApiService(HttpClient httpClient) { _httpClient = httpClient; }

        public async Task<List<ExtraCostDto>> ListAsync(int? year = null)
        {
            try
            {
                var url = year.HasValue ? $"/api/extra-costs?year={year}" : "/api/extra-costs";
                var resp = await _httpClient.GetAsync(url);
                resp.EnsureSuccessStatusCode();
                return JsonSerializer.Deserialize<List<ExtraCostDto>>(await resp.Content.ReadAsStringAsync(), _json) ?? new();
            }
            catch { return new List<ExtraCostDto>(); }
        }

        public async Task<ExtraCostDto?> GetAsync(int year, string id)
        {
            try
            {
                var resp = await _httpClient.GetAsync($"/api/extra-costs/{year}/{id}");
                resp.EnsureSuccessStatusCode();
                return JsonSerializer.Deserialize<ExtraCostDto>(await resp.Content.ReadAsStringAsync(), _json);
            }
            catch { return null; }
        }

        public async Task<string> CreateAsync(ExtraCostDto dto)
        {
            var content = new StringContent(JsonSerializer.Serialize(dto, _json), Encoding.UTF8, "application/json");
            var resp = await _httpClient.PostAsync("/api/extra-costs", content);
            resp.EnsureSuccessStatusCode();
            var created = JsonSerializer.Deserialize<ExtraCostDto>(await resp.Content.ReadAsStringAsync(), _json);
            return created?.Id ?? string.Empty;
        }

        public async Task UpdateAsync(int year, string id, ExtraCostDto dto)
        {
            var content = new StringContent(JsonSerializer.Serialize(dto, _json), Encoding.UTF8, "application/json");
            var resp = await _httpClient.PutAsync($"/api/extra-costs/{year}/{id}", content);
            resp.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(int year, string id)
        {
            var resp = await _httpClient.DeleteAsync($"/api/extra-costs/{year}/{id}");
            resp.EnsureSuccessStatusCode();
        }
    }
}
