using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HotelDroid.Client.Services
{
    public interface IBookingApiService
    {
        Task<List<BookingClientDto>> ListAsync(int? year = null);
        Task<BookingClientDto?> GetAsync(int year, string id);
        Task<string> CreateAsync(BookingClientDto dto);
        Task UpdateAsync(int year, string id, BookingClientDto dto);
        Task DeleteAsync(int year, string id);
        Task CancelAsync(int year, string id);
    }

    public record BookingClientDto(
        string? Id = null,
        int Year = 0,
        string? ClientId = null,
        string? RoomId = null,
        string? ArrivalDate = null,
        string? DepartureDate = null,
        string? Status = null,
        string? Notes = null
    );

    public class BookingApiService : IBookingApiService
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

        public BookingApiService(HttpClient httpClient) { _httpClient = httpClient; }

        public async Task<List<BookingClientDto>> ListAsync(int? year = null)
        {
            try
            {
                var url = year.HasValue ? $"/api/bookings?year={year}" : "/api/bookings";
                var resp = await _httpClient.GetAsync(url);
                resp.EnsureSuccessStatusCode();
                return JsonSerializer.Deserialize<List<BookingClientDto>>(await resp.Content.ReadAsStringAsync(), _json) ?? new();
            }
            catch { return new List<BookingClientDto>(); }
        }

        public async Task<BookingClientDto?> GetAsync(int year, string id)
        {
            try
            {
                var resp = await _httpClient.GetAsync($"/api/bookings/{year}/{id}");
                resp.EnsureSuccessStatusCode();
                return JsonSerializer.Deserialize<BookingClientDto>(await resp.Content.ReadAsStringAsync(), _json);
            }
            catch { return null; }
        }

        public async Task<string> CreateAsync(BookingClientDto dto)
        {
            var content = new StringContent(JsonSerializer.Serialize(dto, _json), Encoding.UTF8, "application/json");
            var resp = await _httpClient.PostAsync("/api/bookings", content);
            resp.EnsureSuccessStatusCode();
            var created = JsonSerializer.Deserialize<BookingClientDto>(await resp.Content.ReadAsStringAsync(), _json);
            return created?.Id ?? string.Empty;
        }

        public async Task UpdateAsync(int year, string id, BookingClientDto dto)
        {
            var content = new StringContent(JsonSerializer.Serialize(dto, _json), Encoding.UTF8, "application/json");
            var resp = await _httpClient.PutAsync($"/api/bookings/{year}/{id}", content);
            resp.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(int year, string id)
        {
            var resp = await _httpClient.DeleteAsync($"/api/bookings/{year}/{id}");
            resp.EnsureSuccessStatusCode();
        }

        public async Task CancelAsync(int year, string id)
        {
            var resp = await _httpClient.PostAsync($"/api/bookings/{year}/{id}/cancel", null);
            resp.EnsureSuccessStatusCode();
        }
    }
}
