using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HotelDroid.Client.Services
{
    public interface IBookingCostApiService
    {
        Task<List<BookingCostClientDto>> ListAsync(int? year = null);
        Task<BookingCostClientDto?> GetAsync(int year, string id);
        Task<string> CreateAsync(BookingCostClientDto dto);
        Task UpdateAsync(int year, string id, BookingCostClientDto dto);
        Task DeleteAsync(int year, string id);
    }

    public record BookingCostClientDto(
        string? Id = null,
        int Year = 0,
        string? BookingId = null,
        string? TariffId = null,
        double? Amount = null,
        string? Description = null
    );

    public class BookingCostApiService : IBookingCostApiService
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

        public BookingCostApiService(HttpClient httpClient) { _httpClient = httpClient; }

        public async Task<List<BookingCostClientDto>> ListAsync(int? year = null)
        {
            try
            {
                var url = year.HasValue ? $"/api/booking-costs?year={year}" : "/api/booking-costs";
                var resp = await _httpClient.GetAsync(url);
                resp.EnsureSuccessStatusCode();
                return JsonSerializer.Deserialize<List<BookingCostClientDto>>(await resp.Content.ReadAsStringAsync(), _json) ?? new();
            }
            catch { return new List<BookingCostClientDto>(); }
        }

        public async Task<BookingCostClientDto?> GetAsync(int year, string id)
        {
            try
            {
                var resp = await _httpClient.GetAsync($"/api/booking-costs/{year}/{id}");
                resp.EnsureSuccessStatusCode();
                return JsonSerializer.Deserialize<BookingCostClientDto>(await resp.Content.ReadAsStringAsync(), _json);
            }
            catch { return null; }
        }

        public async Task<string> CreateAsync(BookingCostClientDto dto)
        {
            var content = new StringContent(JsonSerializer.Serialize(dto, _json), Encoding.UTF8, "application/json");
            var resp = await _httpClient.PostAsync("/api/booking-costs", content);
            resp.EnsureSuccessStatusCode();
            var created = JsonSerializer.Deserialize<BookingCostClientDto>(await resp.Content.ReadAsStringAsync(), _json);
            return created?.Id ?? string.Empty;
        }

        public async Task UpdateAsync(int year, string id, BookingCostClientDto dto)
        {
            var content = new StringContent(JsonSerializer.Serialize(dto, _json), Encoding.UTF8, "application/json");
            var resp = await _httpClient.PutAsync($"/api/booking-costs/{year}/{id}", content);
            resp.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(int year, string id)
        {
            var resp = await _httpClient.DeleteAsync($"/api/booking-costs/{year}/{id}");
            resp.EnsureSuccessStatusCode();
        }
    }
}
