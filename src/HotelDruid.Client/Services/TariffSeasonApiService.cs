using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace HotelDruid.Client.Services
{
    public interface ITariffSeasonApiService
    {
        Task<List<TariffSeasonDto>> GetSeasonsAsync(string? tariffId = null);
        Task<string> CreateSeasonAsync(TariffSeasonDto dto);
        Task UpdateSeasonAsync(string id, TariffSeasonDto dto);
        Task DeleteSeasonAsync(string id);
    }

    /// <summary>Date-range price adjustment layered on top of a tariff's base price (e.g. "High Season" +20%).</summary>
    public record TariffSeasonDto(string? Id, string? TariffId, string? Name, DateOnly StartDate, DateOnly EndDate, string AdjustmentType, double Value, DateTime? CreatedAt);

    public class TariffSeasonApiService : ITariffSeasonApiService
    {
        private readonly HttpClient _httpClient;
        private const string ApiBaseUrl = "/api";

        public TariffSeasonApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<TariffSeasonDto>> GetSeasonsAsync(string? tariffId = null)
        {
            try
            {
                var url = $"{ApiBaseUrl}/tariff-seasons" + (string.IsNullOrWhiteSpace(tariffId) ? "" : $"?tariffId={Uri.EscapeDataString(tariffId)}");
                var resp = await _httpClient.GetAsync(url);
                resp.EnsureSuccessStatusCode();
                var content = await resp.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<List<TariffSeasonDto>>(content) ?? new();
            }
            catch
            {
                return new List<TariffSeasonDto>();
            }
        }

        public async Task<string> CreateSeasonAsync(TariffSeasonDto dto)
        {
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(dto), System.Text.Encoding.UTF8, "application/json");
            var resp = await _httpClient.PostAsync($"{ApiBaseUrl}/tariff-seasons", content);
            resp.EnsureSuccessStatusCode();
            var body = await resp.Content.ReadAsStringAsync();
            var created = System.Text.Json.JsonSerializer.Deserialize<TariffSeasonDto>(body);
            return created?.Id ?? string.Empty;
        }

        public async Task UpdateSeasonAsync(string id, TariffSeasonDto dto)
        {
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(dto), System.Text.Encoding.UTF8, "application/json");
            var resp = await _httpClient.PutAsync($"{ApiBaseUrl}/tariff-seasons/{id}", content);
            resp.EnsureSuccessStatusCode();
        }

        public async Task DeleteSeasonAsync(string id)
        {
            var resp = await _httpClient.DeleteAsync($"{ApiBaseUrl}/tariff-seasons/{id}");
            resp.EnsureSuccessStatusCode();
        }
    }
}
