using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace HotelDruid.Client.Services
{
    public interface IRegionApiService
    {
        Task<List<RegionDto>> GetAllAsync();
        Task<RegionDto?> GetByIdAsync(string id);
        Task<string> CreateAsync(RegionDto dto);
        Task UpdateAsync(string id, RegionDto dto);
        Task DeleteAsync(string id);
    }

    public record RegionDto(
        string Id,
        string? Name,
        string? Code,
        string? Code2,
        string? Code3,
        string? CreatedAt
    );

    public class RegionApiService : IRegionApiService
    {
        private readonly HttpClient _httpClient;
        private const string ApiBaseUrl = "/api";

        public RegionApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<RegionDto>> GetAllAsync()
        {
            try
            {
                var resp = await _httpClient.GetAsync($"{ApiBaseUrl}/regions");
                resp.EnsureSuccessStatusCode();
                var content = await resp.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<List<RegionDto>>(content) ?? new();
            }
            catch
            {
                return new List<RegionDto>();
            }
        }

        public async Task<RegionDto?> GetByIdAsync(string id)
        {
            try
            {
                var resp = await _httpClient.GetAsync($"{ApiBaseUrl}/regions/{id}");
                resp.EnsureSuccessStatusCode();
                var content = await resp.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<RegionDto>(content);
            }
            catch
            {
                return null;
            }
        }

        public async Task<string> CreateAsync(RegionDto dto)
        {
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(dto), System.Text.Encoding.UTF8, "application/json");
            var resp = await _httpClient.PostAsync($"{ApiBaseUrl}/regions", content);
            resp.EnsureSuccessStatusCode();
            var body = await resp.Content.ReadAsStringAsync();
            var created = System.Text.Json.JsonSerializer.Deserialize<RegionDto>(body);
            return created?.Id ?? string.Empty;
        }

        public async Task UpdateAsync(string id, RegionDto dto)
        {
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(dto), System.Text.Encoding.UTF8, "application/json");
            var resp = await _httpClient.PutAsync($"{ApiBaseUrl}/regions/{id}", content);
            resp.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(string id)
        {
            var resp = await _httpClient.DeleteAsync($"{ApiBaseUrl}/regions/{id}");
            resp.EnsureSuccessStatusCode();
        }
    }
}

