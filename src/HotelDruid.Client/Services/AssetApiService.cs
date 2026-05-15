using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace HotelDruid.Client.Services
{
    public interface IAssetApiService
    {
        Task<List<AssetDto>> GetAssetsAsync();
        Task<AssetDto?> GetAssetAsync(string id);
        Task<string> CreateAssetAsync(AssetDto asset);
        Task UpdateAssetAsync(string id, AssetDto asset);
        Task DeleteAssetAsync(string id);
    }

    public record AssetDto(
        string Id,
        string Name,
        string? Code,
        string? Description,
        string? CreatedAt
    );

    public class AssetApiService : IAssetApiService
    {
        private readonly HttpClient _httpClient;
        private const string ApiBaseUrl = "/api";

        public AssetApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<AssetDto>> GetAssetsAsync()
        {
            try
            {
                var resp = await _httpClient.GetAsync($"{ApiBaseUrl}/assets");
                resp.EnsureSuccessStatusCode();
                var content = await resp.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<List<AssetDto>>(content) ?? new();
            }
            catch
            {
                return new List<AssetDto>();
            }
        }

        public async Task<AssetDto?> GetAssetAsync(string id)
        {
            try
            {
                var resp = await _httpClient.GetAsync($"{ApiBaseUrl}/assets/{id}");
                resp.EnsureSuccessStatusCode();
                var content = await resp.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<AssetDto>(content);
            }
            catch
            {
                return null;
            }
        }

        public async Task<string> CreateAssetAsync(AssetDto asset)
        {
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(asset), System.Text.Encoding.UTF8, "application/json");
            var resp = await _httpClient.PostAsync($"{ApiBaseUrl}/assets", content);
            resp.EnsureSuccessStatusCode();
            var body = await resp.Content.ReadAsStringAsync();
            var created = System.Text.Json.JsonSerializer.Deserialize<AssetDto>(body);
            return created?.Id ?? string.Empty;
        }

        public async Task UpdateAssetAsync(string id, AssetDto asset)
        {
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(asset), System.Text.Encoding.UTF8, "application/json");
            var resp = await _httpClient.PutAsync($"{ApiBaseUrl}/assets/{id}", content);
            resp.EnsureSuccessStatusCode();
        }

        public async Task DeleteAssetAsync(string id)
        {
            var resp = await _httpClient.DeleteAsync($"{ApiBaseUrl}/assets/{id}");
            resp.EnsureSuccessStatusCode();
        }
    }
}

