using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using HotelDruid.Shared;

namespace HotelDruid.Client.Services
{
    public interface IBookingApiService
    {
        Task<List<BookingDto>> ListAsync(int? year = null);
        Task<BookingDto?> GetAsync(int year, string id);
        Task<string> CreateAsync(BookingDto dto);
        Task UpdateAsync(int year, string id, BookingDto dto);
        Task DeleteAsync(int year, string id);
        Task CancelAsync(int year, string id);
    }

    public class BookingApiService : IBookingApiService
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

        public BookingApiService(HttpClient httpClient) { _httpClient = httpClient; }

        public async Task<List<BookingDto>> ListAsync(int? year = null)
        {
            try
            {
                var url = year.HasValue ? $"/api/bookings?year={year}" : "/api/bookings";
                var resp = await _httpClient.GetAsync(url);
                resp.EnsureSuccessStatusCode();
                return JsonSerializer.Deserialize<List<BookingDto>>(await resp.Content.ReadAsStringAsync(), _json) ?? new();
            }
            catch { return new List<BookingDto>(); }
        }

        public async Task<BookingDto?> GetAsync(int year, string id)
        {
            try
            {
                var resp = await _httpClient.GetAsync($"/api/bookings/{year}/{id}");
                resp.EnsureSuccessStatusCode();
                return JsonSerializer.Deserialize<BookingDto>(await resp.Content.ReadAsStringAsync(), _json);
            }
            catch { return null; }
        }

        public async Task<string> CreateAsync(BookingDto dto)
        {
            var content = new StringContent(JsonSerializer.Serialize(dto, _json), Encoding.UTF8, "application/json");
            var resp = await _httpClient.PostAsync("/api/bookings", content);
            resp.EnsureSuccessStatusCode();
            var created = JsonSerializer.Deserialize<BookingDto>(await resp.Content.ReadAsStringAsync(), _json);
            return created?.Id ?? string.Empty;
        }

        public async Task UpdateAsync(int year, string id, BookingDto dto)
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

