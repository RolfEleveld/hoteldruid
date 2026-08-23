using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace HotelDruid.Client.Services
{
    public interface ITariffAssignmentApiService
    {
        Task<List<TariffAssignmentDto>> GetAssignmentsAsync(string? tariffId = null, string? targetId = null);
        Task<string> CreateAssignmentAsync(TariffAssignmentDto dto);
        Task DeleteAssignmentAsync(string id);
    }

    /// <summary>Join entity: a tariff can apply to many targets (rooms today; amenities/services later), and a target can have many tariffs.</summary>
    public record TariffAssignmentDto(string? Id, string? TariffId, string TargetType, string? TargetId, DateTime? CreatedAt);

    public class TariffAssignmentApiService : ITariffAssignmentApiService
    {
        private readonly HttpClient _httpClient;
        private const string ApiBaseUrl = "/api";

        public TariffAssignmentApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<TariffAssignmentDto>> GetAssignmentsAsync(string? tariffId = null, string? targetId = null)
        {
            try
            {
                var query = new List<string>();
                if (!string.IsNullOrWhiteSpace(tariffId)) query.Add($"tariffId={Uri.EscapeDataString(tariffId)}");
                if (!string.IsNullOrWhiteSpace(targetId)) query.Add($"targetId={Uri.EscapeDataString(targetId)}");
                var url = $"{ApiBaseUrl}/tariff-assignments" + (query.Count > 0 ? "?" + string.Join("&", query) : "");
                var resp = await _httpClient.GetAsync(url);
                resp.EnsureSuccessStatusCode();
                var content = await resp.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<List<TariffAssignmentDto>>(content) ?? new();
            }
            catch
            {
                return new List<TariffAssignmentDto>();
            }
        }

        public async Task<string> CreateAssignmentAsync(TariffAssignmentDto dto)
        {
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(dto), System.Text.Encoding.UTF8, "application/json");
            var resp = await _httpClient.PostAsync($"{ApiBaseUrl}/tariff-assignments", content);
            resp.EnsureSuccessStatusCode();
            var body = await resp.Content.ReadAsStringAsync();
            var created = System.Text.Json.JsonSerializer.Deserialize<TariffAssignmentDto>(body);
            return created?.Id ?? string.Empty;
        }

        public async Task DeleteAssignmentAsync(string id)
        {
            var resp = await _httpClient.DeleteAsync($"{ApiBaseUrl}/tariff-assignments/{id}");
            resp.EnsureSuccessStatusCode();
        }
    }
}
