using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HotelDroid.Client.Services
{
    public interface IExpenseApiService
    {
        Task<List<ExpenseClientDto>> ListAsync(int? year = null);
        Task<ExpenseClientDto?> GetAsync(int year, string id);
        Task<string> CreateAsync(ExpenseClientDto dto);
        Task UpdateAsync(int year, string id, ExpenseClientDto dto);
        Task DeleteAsync(int year, string id);
    }

    public record ExpenseClientDto(
        string? Id = null,
        int Year = 0,
        string? CashRegisterId = null,
        double? Amount = null,
        string? Description = null,
        string? Date = null
    );

    public class ExpenseApiService : IExpenseApiService
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

        public ExpenseApiService(HttpClient httpClient) { _httpClient = httpClient; }

        public async Task<List<ExpenseClientDto>> ListAsync(int? year = null)
        {
            try
            {
                var url = year.HasValue ? $"/api/expenses?year={year}" : "/api/expenses";
                var resp = await _httpClient.GetAsync(url);
                resp.EnsureSuccessStatusCode();
                return JsonSerializer.Deserialize<List<ExpenseClientDto>>(await resp.Content.ReadAsStringAsync(), _json) ?? new();
            }
            catch { return new List<ExpenseClientDto>(); }
        }

        public async Task<ExpenseClientDto?> GetAsync(int year, string id)
        {
            try
            {
                var resp = await _httpClient.GetAsync($"/api/expenses/{year}/{id}");
                resp.EnsureSuccessStatusCode();
                return JsonSerializer.Deserialize<ExpenseClientDto>(await resp.Content.ReadAsStringAsync(), _json);
            }
            catch { return null; }
        }

        public async Task<string> CreateAsync(ExpenseClientDto dto)
        {
            var content = new StringContent(JsonSerializer.Serialize(dto, _json), Encoding.UTF8, "application/json");
            var resp = await _httpClient.PostAsync("/api/expenses", content);
            resp.EnsureSuccessStatusCode();
            var created = JsonSerializer.Deserialize<ExpenseClientDto>(await resp.Content.ReadAsStringAsync(), _json);
            return created?.Id ?? string.Empty;
        }

        public async Task UpdateAsync(int year, string id, ExpenseClientDto dto)
        {
            var content = new StringContent(JsonSerializer.Serialize(dto, _json), Encoding.UTF8, "application/json");
            var resp = await _httpClient.PutAsync($"/api/expenses/{year}/{id}", content);
            resp.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(int year, string id)
        {
            var resp = await _httpClient.DeleteAsync($"/api/expenses/{year}/{id}");
            resp.EnsureSuccessStatusCode();
        }
    }
}
