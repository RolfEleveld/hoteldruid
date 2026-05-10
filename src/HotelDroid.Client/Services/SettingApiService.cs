using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace HotelDroid.Client.Services
{
    public interface ISettingApiService
    {
        Task<List<SettingDto>> GetAllAsync();
        Task<List<SettingDto>> GetByUserAsync(int userId);
        Task<SettingDto?> GetByUserAndKeyAsync(int userId, string key);
        Task UpsertAsync(SettingDto dto);
        Task UpdateAsync(int userId, string key, SettingDto dto);
        Task DeleteAsync(int userId, string key);
    }

    public record SettingDto(
        string? Key,
        int? UserId,
        string? StringValue,
        int? NumericValue
    );

    public class SettingApiService : ISettingApiService
    {
        private readonly HttpClient _httpClient;
        private const string ApiBaseUrl = "/api";

        public SettingApiService(HttpClient httpClient) => _httpClient = httpClient;

        public async Task<List<SettingDto>> GetAllAsync()
        {
            try
            {
                var resp = await _httpClient.GetAsync($"{ApiBaseUrl}/settings");
                resp.EnsureSuccessStatusCode();
                return System.Text.Json.JsonSerializer.Deserialize<List<SettingDto>>(await resp.Content.ReadAsStringAsync()) ?? new();
            }
            catch { return new List<SettingDto>(); }
        }

        public async Task<List<SettingDto>> GetByUserAsync(int userId)
        {
            try
            {
                var resp = await _httpClient.GetAsync($"{ApiBaseUrl}/settings?userId={userId}");
                resp.EnsureSuccessStatusCode();
                return System.Text.Json.JsonSerializer.Deserialize<List<SettingDto>>(await resp.Content.ReadAsStringAsync()) ?? new();
            }
            catch { return new List<SettingDto>(); }
        }

        public async Task<SettingDto?> GetByUserAndKeyAsync(int userId, string key)
        {
            try
            {
                var resp = await _httpClient.GetAsync($"{ApiBaseUrl}/settings/{userId}/{key}");
                resp.EnsureSuccessStatusCode();
                return System.Text.Json.JsonSerializer.Deserialize<SettingDto>(await resp.Content.ReadAsStringAsync());
            }
            catch { return null; }
        }

        public async Task UpsertAsync(SettingDto dto)
        {
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(dto), System.Text.Encoding.UTF8, "application/json");
            var resp = await _httpClient.PostAsync($"{ApiBaseUrl}/settings", content);
            resp.EnsureSuccessStatusCode();
        }

        public async Task UpdateAsync(int userId, string key, SettingDto dto)
        {
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(dto), System.Text.Encoding.UTF8, "application/json");
            var resp = await _httpClient.PutAsync($"{ApiBaseUrl}/settings/{userId}/{key}", content);
            resp.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(int userId, string key)
        {
            var resp = await _httpClient.DeleteAsync($"{ApiBaseUrl}/settings/{userId}/{key}");
            resp.EnsureSuccessStatusCode();
        }
    }
}
