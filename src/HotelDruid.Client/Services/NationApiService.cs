using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace HotelDruid.Client.Services
{
    public interface INationApiService
    {
        Task<List<NationDto>> GetAllAsync();
        Task<NationDto?> GetByIdAsync(string id);
        Task<string> CreateAsync(NationDto dto);
        Task UpdateAsync(string id, NationDto dto);
        Task DeleteAsync(string id);
    }

    public record NationDto(
        string Id,
        string? Name,
        string? Code,
        string? Code2,
        string? Code3,
        string? CreatedAt
    );

    public class NationApiService : INationApiService
    {
        private readonly HttpClient _httpClient;
        private const string ApiBaseUrl = "/api";

        public NationApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<NationDto>> GetAllAsync()
        {
            try
            {
                var resp = await _httpClient.GetAsync($"{ApiBaseUrl}/nations");
                resp.EnsureSuccessStatusCode();
                var content = await resp.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<List<NationDto>>(content) ?? new();
            }
            catch
            {
                return new List<NationDto>();
            }
        }

        public async Task<NationDto?> GetByIdAsync(string id)
        {
            try
            {
                var resp = await _httpClient.GetAsync($"{ApiBaseUrl}/nations/{id}");
                resp.EnsureSuccessStatusCode();
                var content = await resp.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<NationDto>(content);
            }
            catch
            {
                return null;
            }
        }

        public async Task<string> CreateAsync(NationDto dto)
        {
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(dto), System.Text.Encoding.UTF8, "application/json");
            var resp = await _httpClient.PostAsync($"{ApiBaseUrl}/nations", content);
            resp.EnsureSuccessStatusCode();
            var body = await resp.Content.ReadAsStringAsync();
            var created = System.Text.Json.JsonSerializer.Deserialize<NationDto>(body);
            return created?.Id ?? string.Empty;
        }

        public async Task UpdateAsync(string id, NationDto dto)
        {
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(dto), System.Text.Encoding.UTF8, "application/json");
            var resp = await _httpClient.PutAsync($"{ApiBaseUrl}/nations/{id}", content);
            resp.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(string id)
        {
            var resp = await _httpClient.DeleteAsync($"{ApiBaseUrl}/nations/{id}");
            resp.EnsureSuccessStatusCode();
        }
    }
}

