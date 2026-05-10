using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace HotelDroid.Client.Services
{
    public interface ICancelledBookingApiService
    {
        Task<List<CancelledBookingClientDto>> ListAsync(int? year = null);
        Task<CancelledBookingClientDto?> GetAsync(int year, string id);
        Task DeleteAsync(int year, string id);
    }

    public record CancelledBookingClientDto(
        string? Id = null,
        int Year = 0,
        string? ClientId = null,
        string? RoomId = null,
        string? ArrivalDate = null,
        string? DepartureDate = null,
        string? Status = null,
        string? CancelledAt = null,
        string? Notes = null
    );

    public class CancelledBookingApiService : ICancelledBookingApiService
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

        public CancelledBookingApiService(HttpClient httpClient) { _httpClient = httpClient; }

        public async Task<List<CancelledBookingClientDto>> ListAsync(int? year = null)
        {
            try
            {
                var url = year.HasValue ? $"/api/cancelled-bookings?year={year}" : "/api/cancelled-bookings";
                var resp = await _httpClient.GetAsync(url);
                resp.EnsureSuccessStatusCode();
                return JsonSerializer.Deserialize<List<CancelledBookingClientDto>>(await resp.Content.ReadAsStringAsync(), _json) ?? new();
            }
            catch { return new List<CancelledBookingClientDto>(); }
        }

        public async Task<CancelledBookingClientDto?> GetAsync(int year, string id)
        {
            try
            {
                var resp = await _httpClient.GetAsync($"/api/cancelled-bookings/{year}/{id}");
                resp.EnsureSuccessStatusCode();
                return JsonSerializer.Deserialize<CancelledBookingClientDto>(await resp.Content.ReadAsStringAsync(), _json);
            }
            catch { return null; }
        }

        public async Task DeleteAsync(int year, string id)
        {
            var resp = await _httpClient.DeleteAsync($"/api/cancelled-bookings/{year}/{id}");
            resp.EnsureSuccessStatusCode();
        }
    }
}
