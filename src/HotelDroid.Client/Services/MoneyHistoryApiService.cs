using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HotelDroid.Client.Services
{
    public interface IMoneyHistoryApiService
    {
        Task<List<MoneyHistoryClientDto>> ListAsync(int? year = null);
        Task<MoneyHistoryClientDto?> GetAsync(int year, string id);
        Task<string> CreateAsync(MoneyHistoryClientDto dto);
        Task DeleteAsync(int year, string id);
    }

    public record MoneyHistoryClientDto(
        string? Id = null,
        int Year = 0,
        string? CashRegisterId = null,
        double? Amount = null,
        string? Type = null,
        string? Description = null,
        string? Date = null
    );

    public class MoneyHistoryApiService : IMoneyHistoryApiService
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

        public MoneyHistoryApiService(HttpClient httpClient) { _httpClient = httpClient; }

        public async Task<List<MoneyHistoryClientDto>> ListAsync(int? year = null)
        {
            try
            {
                var url = year.HasValue ? $"/api/money-history?year={year}" : "/api/money-history";
                var resp = await _httpClient.GetAsync(url);
                resp.EnsureSuccessStatusCode();
                return JsonSerializer.Deserialize<List<MoneyHistoryClientDto>>(await resp.Content.ReadAsStringAsync(), _json) ?? new();
            }
            catch { return new List<MoneyHistoryClientDto>(); }
        }

        public async Task<MoneyHistoryClientDto?> GetAsync(int year, string id)
        {
            try
            {
                var resp = await _httpClient.GetAsync($"/api/money-history/{year}/{id}");
                resp.EnsureSuccessStatusCode();
                return JsonSerializer.Deserialize<MoneyHistoryClientDto>(await resp.Content.ReadAsStringAsync(), _json);
            }
            catch { return null; }
        }

        public async Task<string> CreateAsync(MoneyHistoryClientDto dto)
        {
            var content = new StringContent(JsonSerializer.Serialize(dto, _json), Encoding.UTF8, "application/json");
            var resp = await _httpClient.PostAsync("/api/money-history", content);
            resp.EnsureSuccessStatusCode();
            var created = JsonSerializer.Deserialize<MoneyHistoryClientDto>(await resp.Content.ReadAsStringAsync(), _json);
            return created?.Id ?? string.Empty;
        }

        public async Task DeleteAsync(int year, string id)
        {
            var resp = await _httpClient.DeleteAsync($"/api/money-history/{year}/{id}");
            resp.EnsureSuccessStatusCode();
        }
    }
}
