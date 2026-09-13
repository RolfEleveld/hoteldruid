using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace HotelDruid.Client.Services
{
    public interface ITariffApiService
    {
        Task<List<TariffDto>> GetTariffsAsync();
        Task<TariffDto?> GetTariffAsync(string id);
        Task<string> CreateTariffAsync(TariffDto dto);
        Task UpdateTariffAsync(string id, TariffDto dto);
        Task DeleteTariffAsync(string id);
        Task<TariffResolutionDto?> ResolveTariffAsync(string roomId, DateOnly arrivalDate, DateOnly departureDate, int guestCount);
    }

    /// <summary>Result of resolving which tariff applies to a room for a given stay and guest count.</summary>
    public record TariffResolutionDto(
        string TariffId,
        string TariffName,
        int Nights,
        double BasePricePerNight,
        double AdjustedPricePerNight,
        double TotalPrice,
        string? AppliedSeasonName,
        string ApplicabilityMode
    );

    /// <summary>
    /// Room-rate tariff (rack rate, weekend rate, week/month rate, etc.).
    /// TargetType is reserved for future extension to amenities/services.
    /// </summary>
    public record TariffDto(
        string? Id,
        string Name,
        string? Description,
        string ApplicabilityMode,
        int? WeekdaysMask,
        int? MinNights,
        double PricePerNight,
        double? PricePerPerson,
        int Priority,
        bool IsActive,
        string TargetType,
        DateTime? CreatedAt,
        int? MinGuestCount = null
    );

    public class TariffApiService : ITariffApiService
    {
        private readonly HttpClient _httpClient;
        private const string ApiBaseUrl = "/api";

        public TariffApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<TariffDto>> GetTariffsAsync()
        {
            try
            {
                var resp = await _httpClient.GetAsync($"{ApiBaseUrl}/tariffs");
                resp.EnsureSuccessStatusCode();
                var content = await resp.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<List<TariffDto>>(content) ?? new();
            }
            catch
            {
                return new List<TariffDto>();
            }
        }

        public async Task<TariffDto?> GetTariffAsync(string id)
        {
            try
            {
                var resp = await _httpClient.GetAsync($"{ApiBaseUrl}/tariffs/{id}");
                resp.EnsureSuccessStatusCode();
                var content = await resp.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<TariffDto>(content);
            }
            catch
            {
                return null;
            }
        }

        public async Task<string> CreateTariffAsync(TariffDto dto)
        {
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(dto), System.Text.Encoding.UTF8, "application/json");
            var resp = await _httpClient.PostAsync($"{ApiBaseUrl}/tariffs", content);
            resp.EnsureSuccessStatusCode();
            var body = await resp.Content.ReadAsStringAsync();
            var created = System.Text.Json.JsonSerializer.Deserialize<TariffDto>(body);
            return created?.Id ?? string.Empty;
        }

        public async Task UpdateTariffAsync(string id, TariffDto dto)
        {
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(dto), System.Text.Encoding.UTF8, "application/json");
            var resp = await _httpClient.PutAsync($"{ApiBaseUrl}/tariffs/{id}", content);
            resp.EnsureSuccessStatusCode();
        }

        public async Task DeleteTariffAsync(string id)
        {
            var resp = await _httpClient.DeleteAsync($"{ApiBaseUrl}/tariffs/{id}");
            resp.EnsureSuccessStatusCode();
        }

        public async Task<TariffResolutionDto?> ResolveTariffAsync(string roomId, DateOnly arrivalDate, DateOnly departureDate, int guestCount)
        {
            try
            {
                var url = $"{ApiBaseUrl}/tariffs/resolve?roomId={Uri.EscapeDataString(roomId)}&arrivalDate={arrivalDate:yyyy-MM-dd}&departureDate={departureDate:yyyy-MM-dd}&guestCount={guestCount}";
                var resp = await _httpClient.GetAsync(url);
                if (!resp.IsSuccessStatusCode)
                {
                    return null;
                }
                var content = await resp.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<TariffResolutionDto>(content);
            }
            catch
            {
                return null;
            }
        }
    }
}
