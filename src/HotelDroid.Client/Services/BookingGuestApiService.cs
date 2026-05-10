using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HotelDroid.Client.Services
{
    public interface IBookingGuestApiService
    {
        Task<List<BookingGuestClientDto>> ListAsync(int? year = null);
        Task<BookingGuestClientDto?> GetAsync(int year, string id);
        Task<string> CreateAsync(BookingGuestClientDto dto);
        Task UpdateAsync(int year, string id, BookingGuestClientDto dto);
        Task DeleteAsync(int year, string id);
    }

    public record BookingGuestClientDto(
        string? Id = null,
        int Year = 0,
        string? BookingId = null,
        string? ClientId = null,
        int? GuestNumber = null
    );

    public class BookingGuestApiService : IBookingGuestApiService
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

        public BookingGuestApiService(HttpClient httpClient) { _httpClient = httpClient; }

        public async Task<List<BookingGuestClientDto>> ListAsync(int? year = null)
        {
            try
            {
                var url = year.HasValue ? $"/api/booking-guests?year={year}" : "/api/booking-guests";
                var resp = await _httpClient.GetAsync(url);
                resp.EnsureSuccessStatusCode();
                return JsonSerializer.Deserialize<List<BookingGuestClientDto>>(await resp.Content.ReadAsStringAsync(), _json) ?? new();
            }
            catch { return new List<BookingGuestClientDto>(); }
        }

        public async Task<BookingGuestClientDto?> GetAsync(int year, string id)
        {
            try
            {
                var resp = await _httpClient.GetAsync($"/api/booking-guests/{year}/{id}");
                resp.EnsureSuccessStatusCode();
                return JsonSerializer.Deserialize<BookingGuestClientDto>(await resp.Content.ReadAsStringAsync(), _json);
            }
            catch { return null; }
        }

        public async Task<string> CreateAsync(BookingGuestClientDto dto)
        {
            var content = new StringContent(JsonSerializer.Serialize(dto, _json), Encoding.UTF8, "application/json");
            var resp = await _httpClient.PostAsync("/api/booking-guests", content);
            resp.EnsureSuccessStatusCode();
            var created = JsonSerializer.Deserialize<BookingGuestClientDto>(await resp.Content.ReadAsStringAsync(), _json);
            return created?.Id ?? string.Empty;
        }

        public async Task UpdateAsync(int year, string id, BookingGuestClientDto dto)
        {
            var content = new StringContent(JsonSerializer.Serialize(dto, _json), Encoding.UTF8, "application/json");
            var resp = await _httpClient.PutAsync($"/api/booking-guests/{year}/{id}", content);
            resp.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(int year, string id)
        {
            var resp = await _httpClient.DeleteAsync($"/api/booking-guests/{year}/{id}");
            resp.EnsureSuccessStatusCode();
        }
    }
}
