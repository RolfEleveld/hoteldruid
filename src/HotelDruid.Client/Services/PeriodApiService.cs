using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HotelDruid.Client.Services
{
    public interface IPeriodApiService
    {
        Task<List<PeriodDto>> ListAsync(int? year = null);
        Task<PeriodDto?> GetAsync(int year, string id);
        Task<string> CreateAsync(PeriodDto dto);
        Task UpdateAsync(int year, string id, PeriodDto dto);
        Task DeleteAsync(int year, string id);
    }

    public record PeriodDto(
        string? Id = null,
        int Year = 0,
        string? StartDate = null,
        string? EndDate = null,
        double? Tariff1 = null,
        double? Tariff1PerPerson = null,
        double? Tariff2 = null,
        double? Tariff2PerPerson = null,
        double? Tariff3 = null,
        double? Tariff3PerPerson = null,
        double? Tariff4 = null,
        double? Tariff4PerPerson = null,
        double? Tariff5 = null,
        double? Tariff5PerPerson = null,
        double? Tariff6 = null,
        double? Tariff6PerPerson = null,
        double? Tariff7 = null,
        double? Tariff7PerPerson = null,
        double? Tariff8 = null,
        double? Tariff8PerPerson = null,
        double? Tariff9 = null,
        double? Tariff9PerPerson = null,
        double? Tariff10 = null,
        double? Tariff10PerPerson = null,
        double? Tariff11 = null,
        double? Tariff11PerPerson = null,
        double? Tariff12 = null,
        double? Tariff12PerPerson = null
    );

    public class PeriodApiService : IPeriodApiService
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

        public PeriodApiService(HttpClient httpClient) { _httpClient = httpClient; }

        public async Task<List<PeriodDto>> ListAsync(int? year = null)
        {
            try
            {
                var url = year.HasValue ? $"/api/periods?year={year}" : "/api/periods";
                var resp = await _httpClient.GetAsync(url);
                resp.EnsureSuccessStatusCode();
                return JsonSerializer.Deserialize<List<PeriodDto>>(await resp.Content.ReadAsStringAsync(), _json) ?? new();
            }
            catch { return new List<PeriodDto>(); }
        }

        public async Task<PeriodDto?> GetAsync(int year, string id)
        {
            try
            {
                var resp = await _httpClient.GetAsync($"/api/periods/{year}/{id}");
                resp.EnsureSuccessStatusCode();
                return JsonSerializer.Deserialize<PeriodDto>(await resp.Content.ReadAsStringAsync(), _json);
            }
            catch { return null; }
        }

        public async Task<string> CreateAsync(PeriodDto dto)
        {
            var content = new StringContent(JsonSerializer.Serialize(dto, _json), Encoding.UTF8, "application/json");
            var resp = await _httpClient.PostAsync("/api/periods", content);
            resp.EnsureSuccessStatusCode();
            var created = JsonSerializer.Deserialize<PeriodDto>(await resp.Content.ReadAsStringAsync(), _json);
            return created?.Id ?? string.Empty;
        }

        public async Task UpdateAsync(int year, string id, PeriodDto dto)
        {
            var content = new StringContent(JsonSerializer.Serialize(dto, _json), Encoding.UTF8, "application/json");
            var resp = await _httpClient.PutAsync($"/api/periods/{year}/{id}", content);
            resp.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(int year, string id)
        {
            var resp = await _httpClient.DeleteAsync($"/api/periods/{year}/{id}");
            resp.EnsureSuccessStatusCode();
        }
    }
}

