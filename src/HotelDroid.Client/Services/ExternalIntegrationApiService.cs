using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace HotelDroid.Client.Services
{
    public interface IExternalIntegrationApiService
    {
        Task<List<ExternalIntegrationApiDto>> GetExternalIntegrationsAsync(int? year = null, string? name = null);
        Task<ExternalIntegrationApiDto?> GetExternalIntegrationAsync(string id);
        Task<ExternalIntegrationApiDto?> CreateExternalIntegrationAsync(ExternalIntegrationApiDto ei);
        Task<ExternalIntegrationApiDto?> UpdateExternalIntegrationAsync(string id, ExternalIntegrationApiDto ei);
        Task DeleteExternalIntegrationAsync(string id);
    }

    public record ExternalIntegrationApiDto(
        string? Id,
        string? IntegrationName,
        string? IdType,
        int? LocalId,
        string? RemoteId1,
        string? RemoteId2,
        int? Year,
        string? CreatedAt
    );

    public class ExternalIntegrationApiService : IExternalIntegrationApiService
    {
        private readonly HttpClient _httpClient;
        private const string ApiBaseUrl = "/api";

        public ExternalIntegrationApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ExternalIntegrationApiDto>> GetExternalIntegrationsAsync(int? year = null, string? name = null)
        {
            try
            {
                var query = "";
                if (year.HasValue) query += $"?year={year}";
                if (name != null) query += (query == "" ? "?" : "&") + $"name={Uri.EscapeDataString(name)}";
                var resp = await _httpClient.GetAsync($"{ApiBaseUrl}/external-integrations{query}");
                resp.EnsureSuccessStatusCode();
                return System.Text.Json.JsonSerializer.Deserialize<List<ExternalIntegrationApiDto>>(await resp.Content.ReadAsStringAsync()) ?? new();
            }
            catch { return new List<ExternalIntegrationApiDto>(); }
        }

        public async Task<ExternalIntegrationApiDto?> GetExternalIntegrationAsync(string id)
        {
            try
            {
                var resp = await _httpClient.GetAsync($"{ApiBaseUrl}/external-integrations/{id}");
                resp.EnsureSuccessStatusCode();
                return System.Text.Json.JsonSerializer.Deserialize<ExternalIntegrationApiDto>(await resp.Content.ReadAsStringAsync());
            }
            catch { return null; }
        }

        public async Task<ExternalIntegrationApiDto?> CreateExternalIntegrationAsync(ExternalIntegrationApiDto ei)
        {
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(ei), System.Text.Encoding.UTF8, "application/json");
            var resp = await _httpClient.PostAsync($"{ApiBaseUrl}/external-integrations", content);
            resp.EnsureSuccessStatusCode();
            return System.Text.Json.JsonSerializer.Deserialize<ExternalIntegrationApiDto>(await resp.Content.ReadAsStringAsync());
        }

        public async Task<ExternalIntegrationApiDto?> UpdateExternalIntegrationAsync(string id, ExternalIntegrationApiDto ei)
        {
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(ei), System.Text.Encoding.UTF8, "application/json");
            var resp = await _httpClient.PutAsync($"{ApiBaseUrl}/external-integrations/{id}", content);
            resp.EnsureSuccessStatusCode();
            return System.Text.Json.JsonSerializer.Deserialize<ExternalIntegrationApiDto>(await resp.Content.ReadAsStringAsync());
        }

        public async Task DeleteExternalIntegrationAsync(string id)
        {
            var resp = await _httpClient.DeleteAsync($"{ApiBaseUrl}/external-integrations/{id}");
            resp.EnsureSuccessStatusCode();
        }
    }
}
