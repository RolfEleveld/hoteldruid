using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace HotelDruid.Client.Services
{
    public interface ICityApiService
    {
        Task<List<CityDto>> GetAllAsync();
        Task<CityDto?> GetByIdAsync(string id);
        Task<string> CreateAsync(CityDto dto);
        Task UpdateAsync(string id, CityDto dto);
        Task DeleteAsync(string id);
    }

    public record CityDto(
        string Id,
        string? Name,
        string? Code,
        string? Code2,
        string? Code3,
        string? CreatedAt
    );

    public class CityApiService : ICityApiService
    {
        private readonly HttpClient _httpClient;
        private const string ApiBaseUrl = "/api";

        public CityApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<CityDto>> GetAllAsync()
        {
            try
            {
                var resp = await _httpClient.GetAsync($"{ApiBaseUrl}/cities");
                resp.EnsureSuccessStatusCode();
                var content = await resp.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<List<CityDto>>(content) ?? new();
            }
            catch
            {
                return new List<CityDto>();
            }
        }

        public async Task<CityDto?> GetByIdAsync(string id)
        {
            try
            {
                var resp = await _httpClient.GetAsync($"{ApiBaseUrl}/cities/{id}");
                resp.EnsureSuccessStatusCode();
                var content = await resp.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<CityDto>(content);
            }
            catch
            {
                return null;
            }
        }

        public async Task<string> CreateAsync(CityDto dto)
        {
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(dto), System.Text.Encoding.UTF8, "application/json");
            var resp = await _httpClient.PostAsync($"{ApiBaseUrl}/cities", content);
            resp.EnsureSuccessStatusCode();
            var body = await resp.Content.ReadAsStringAsync();
            var created = System.Text.Json.JsonSerializer.Deserialize<CityDto>(body);
            return created?.Id ?? string.Empty;
        }

        public async Task UpdateAsync(string id, CityDto dto)
        {
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(dto), System.Text.Encoding.UTF8, "application/json");
            var resp = await _httpClient.PutAsync($"{ApiBaseUrl}/cities/{id}", content);
            resp.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(string id)
        {
            var resp = await _httpClient.DeleteAsync($"{ApiBaseUrl}/cities/{id}");
            resp.EnsureSuccessStatusCode();
        }
    }
}

