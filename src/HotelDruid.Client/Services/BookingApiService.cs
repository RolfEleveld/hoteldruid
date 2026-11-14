using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using HotelDruid.Shared;

namespace HotelDruid.Client.Services
{
    public sealed record BookingCheckInRequest(DateOnly ActualArrivalDate, int ActualGuestCount, string? Status = null);
    public sealed record BookingCheckOutRequest(DateOnly ActualDepartureDate, string? Status = null);
    public sealed record BookingNoShowRequest(string Notes, string? Status = null);

    internal sealed class BookingApiErrorResponse
    {
        public string? Error { get; set; }
    }

    public interface IBookingApiService
    {
        Task<List<BookingDto>> ListAsync(int? year = null, bool includeDeleted = false, DateOnly? from = null, DateOnly? to = null, string? clientId = null);
        Task<List<BookingDto>> ListStaysAsync(int? year = null);
        Task<List<BookingDto>> ListReservationsAsync(int? year = null);
        Task<List<BookingDto>> ListArrivalsAsync(int? year = null, int? lookAheadDays = null);
        Task<List<BookingDto>> ListDeparturesAsync(int? year = null);
        Task<BookingDto?> GetAsync(int year, string id);
        Task<BookingDto?> GetByIdAsync(string id);
        Task<string> CreateAsync(BookingDto dto);
        Task UpdateAsync(int year, string id, BookingDto dto);
        Task DeleteAsync(int year, string id);
        Task CancelAsync(int year, string id);
        Task<BookingDto?> CheckInAsync(int year, string id, BookingCheckInRequest request);
        Task<BookingDto?> CheckOutAsync(int year, string id, BookingCheckOutRequest request);
        Task<BookingDto?> MarkNoShowAsync(int year, string id, BookingNoShowRequest request);
    }

    public class BookingApiService : IBookingApiService
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

        public BookingApiService(HttpClient httpClient) { _httpClient = httpClient; }

        public async Task<List<BookingDto>> ListAsync(int? year = null, bool includeDeleted = false, DateOnly? from = null, DateOnly? to = null, string? clientId = null)
        {
            try
            {
                var query = new List<string>();
                if (from.HasValue && to.HasValue)
                {
                    var start = from.Value <= to.Value ? from.Value : to.Value;
                    var end = from.Value <= to.Value ? to.Value : from.Value;
                    query.Add($"from={start:yyyy-MM-dd}");
                    query.Add($"to={end:yyyy-MM-dd}");
                }
                else if (year.HasValue)
                {
                    query.Add($"year={year.Value}");
                }

                if (includeDeleted)
                {
                    query.Add("includeDeleted=true");
                }

                if (!string.IsNullOrWhiteSpace(clientId))
                {
                    query.Add($"clientId={Uri.EscapeDataString(clientId)}");
                }

                var url = query.Count == 0 ? "/api/bookings" : $"/api/bookings?{string.Join("&", query)}";
                var resp = await _httpClient.GetAsync(url);
                resp.EnsureSuccessStatusCode();
                return JsonSerializer.Deserialize<List<BookingDto>>(await resp.Content.ReadAsStringAsync(), _json) ?? new();
            }
            catch { return new List<BookingDto>(); }
        }

        public async Task<List<BookingDto>> ListStaysAsync(int? year = null)
        {
            try
            {
                var url = year.HasValue ? $"/api/bookings/stays?year={year}" : "/api/bookings/stays";
                var resp = await _httpClient.GetAsync(url);
                resp.EnsureSuccessStatusCode();
                return JsonSerializer.Deserialize<List<BookingDto>>(await resp.Content.ReadAsStringAsync(), _json) ?? new();
            }
            catch { return new List<BookingDto>(); }
        }

        public async Task<List<BookingDto>> ListReservationsAsync(int? year = null)
        {
            try
            {
                var url = year.HasValue ? $"/api/bookings/reservations?year={year}" : "/api/bookings/reservations";
                var resp = await _httpClient.GetAsync(url);
                resp.EnsureSuccessStatusCode();
                return JsonSerializer.Deserialize<List<BookingDto>>(await resp.Content.ReadAsStringAsync(), _json) ?? new();
            }
            catch { return new List<BookingDto>(); }
        }

        public async Task<List<BookingDto>> ListArrivalsAsync(int? year = null, int? lookAheadDays = null)
        {
            try
            {
                var query = new List<string>();
                if (year.HasValue)
                {
                    query.Add($"year={year.Value}");
                }

                if (lookAheadDays.HasValue)
                {
                    query.Add($"lookAheadDays={Math.Max(0, lookAheadDays.Value)}");
                }

                var url = query.Count > 0 ? $"/api/bookings/arrivals?{string.Join("&", query)}" : "/api/bookings/arrivals";
                var resp = await _httpClient.GetAsync(url);
                resp.EnsureSuccessStatusCode();
                return JsonSerializer.Deserialize<List<BookingDto>>(await resp.Content.ReadAsStringAsync(), _json) ?? new();
            }
            catch { return new List<BookingDto>(); }
        }

        public async Task<List<BookingDto>> ListDeparturesAsync(int? year = null)
        {
            try
            {
                var url = year.HasValue ? $"/api/bookings/departures?year={year}" : "/api/bookings/departures";
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

        public async Task<BookingDto?> GetByIdAsync(string id)
        {
            try
            {
                var resp = await _httpClient.GetAsync($"/api/bookings/by-id/{Uri.EscapeDataString(id)}");
                resp.EnsureSuccessStatusCode();
                return JsonSerializer.Deserialize<BookingDto>(await resp.Content.ReadAsStringAsync(), _json);
            }
            catch { return null; }
        }

        public async Task<string> CreateAsync(BookingDto dto)
        {
            var content = new StringContent(JsonSerializer.Serialize(dto, _json), Encoding.UTF8, "application/json");
            var resp = await _httpClient.PostAsync("/api/bookings", content);
            await EnsureSuccessOrThrowAsync(resp);
            var created = JsonSerializer.Deserialize<BookingDto>(await resp.Content.ReadAsStringAsync(), _json);
            return created?.Id ?? string.Empty;
        }

        public async Task UpdateAsync(int year, string id, BookingDto dto)
        {
            var content = new StringContent(JsonSerializer.Serialize(dto, _json), Encoding.UTF8, "application/json");
            var resp = await _httpClient.PutAsync($"/api/bookings/{year}/{id}", content);
            await EnsureSuccessOrThrowAsync(resp);
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

        public async Task<BookingDto?> CheckInAsync(int year, string id, BookingCheckInRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync($"/api/bookings/{year}/{id}/check-in", request, _json);
            await EnsureSuccessOrThrowAsync(response);
            return JsonSerializer.Deserialize<BookingDto>(await response.Content.ReadAsStringAsync(), _json);
        }

        public async Task<BookingDto?> CheckOutAsync(int year, string id, BookingCheckOutRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync($"/api/bookings/{year}/{id}/check-out", request, _json);
            await EnsureSuccessOrThrowAsync(response);
            return JsonSerializer.Deserialize<BookingDto>(await response.Content.ReadAsStringAsync(), _json);
        }

        public async Task<BookingDto?> MarkNoShowAsync(int year, string id, BookingNoShowRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync($"/api/bookings/{year}/{id}/no-show", request, _json);
            await EnsureSuccessOrThrowAsync(response);
            return JsonSerializer.Deserialize<BookingDto>(await response.Content.ReadAsStringAsync(), _json);
        }

        private static async Task EnsureSuccessOrThrowAsync(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return;
            }

            string? errorMessage = null;

            try
            {
                var payload = JsonSerializer.Deserialize<BookingApiErrorResponse>(await response.Content.ReadAsStringAsync(), _json);
                errorMessage = payload?.Error;
            }
            catch
            {
            }

            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                throw new InvalidOperationException(errorMessage);
            }

            response.EnsureSuccessStatusCode();
        }
    }
}

