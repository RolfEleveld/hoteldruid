using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HotelDroid.Client.Services
{
    public interface IYearApiService
    {
        Task<List<YearDto>> ListAsync();
        Task<YearDto?> GetAsync(int year);
        Task<int> CreateAsync(YearDto dto);
        Task UpdateAsync(int year, YearDto dto);
        Task DeleteAsync(int year);
    }

    public record YearDto(int Year, string? PeriodType = null);

    public class YearApiService : IYearApiService
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

        public YearApiService(HttpClient httpClient) { _httpClient = httpClient; }

        public async Task<List<YearDto>> ListAsync()
        {
            try
            {
                var resp = await _httpClient.GetAsync("/api/years");
                resp.EnsureSuccessStatusCode();
                return JsonSerializer.Deserialize<List<YearDto>>(await resp.Content.ReadAsStringAsync(), _json) ?? new();
            }
            catch { return new List<YearDto>(); }
        }

        public async Task<YearDto?> GetAsync(int year)
        {
            try
            {
                var resp = await _httpClient.GetAsync($"/api/years/{year}");
                resp.EnsureSuccessStatusCode();
                return JsonSerializer.Deserialize<YearDto>(await resp.Content.ReadAsStringAsync(), _json);
            }
            catch { return null; }
        }

        public async Task<int> CreateAsync(YearDto dto)
        {
            var content = new StringContent(JsonSerializer.Serialize(dto, _json), Encoding.UTF8, "application/json");
            var resp = await _httpClient.PostAsync("/api/years", content);
            resp.EnsureSuccessStatusCode();
            var created = JsonSerializer.Deserialize<YearDto>(await resp.Content.ReadAsStringAsync(), _json);
            return created?.Year ?? 0;
        }

        public async Task UpdateAsync(int year, YearDto dto)
        {
            var content = new StringContent(JsonSerializer.Serialize(dto, _json), Encoding.UTF8, "application/json");
            var resp = await _httpClient.PutAsync($"/api/years/{year}", content);
            resp.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(int year)
        {
            var resp = await _httpClient.DeleteAsync($"/api/years/{year}");
            resp.EnsureSuccessStatusCode();
        }
    }
}
