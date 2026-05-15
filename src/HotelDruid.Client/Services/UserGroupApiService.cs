using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace HotelDruid.Client.Services
{
    public interface IUserGroupApiService
    {
        Task<List<UserGroupDto>> GetAllAsync();
        Task<UserGroupDto?> GetByIdAsync(string id);
        Task<string> CreateAsync(UserGroupDto dto);
        Task UpdateAsync(string id, UserGroupDto dto);
        Task DeleteAsync(string id);
    }

    public record UserGroupDto(
        string Id,
        string? Name
    );

    public class UserGroupApiService : IUserGroupApiService
    {
        private readonly HttpClient _httpClient;
        private const string ApiBaseUrl = "/api";

        public UserGroupApiService(HttpClient httpClient) => _httpClient = httpClient;

        public async Task<List<UserGroupDto>> GetAllAsync()
        {
            try
            {
                var resp = await _httpClient.GetAsync($"{ApiBaseUrl}/user-groups");
                resp.EnsureSuccessStatusCode();
                return System.Text.Json.JsonSerializer.Deserialize<List<UserGroupDto>>(await resp.Content.ReadAsStringAsync()) ?? new();
            }
            catch { return new List<UserGroupDto>(); }
        }

        public async Task<UserGroupDto?> GetByIdAsync(string id)
        {
            try
            {
                var resp = await _httpClient.GetAsync($"{ApiBaseUrl}/user-groups/{id}");
                resp.EnsureSuccessStatusCode();
                return System.Text.Json.JsonSerializer.Deserialize<UserGroupDto>(await resp.Content.ReadAsStringAsync());
            }
            catch { return null; }
        }

        public async Task<string> CreateAsync(UserGroupDto dto)
        {
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(dto), System.Text.Encoding.UTF8, "application/json");
            var resp = await _httpClient.PostAsync($"{ApiBaseUrl}/user-groups", content);
            resp.EnsureSuccessStatusCode();
            var created = System.Text.Json.JsonSerializer.Deserialize<UserGroupDto>(await resp.Content.ReadAsStringAsync());
            return created?.Id ?? string.Empty;
        }

        public async Task UpdateAsync(string id, UserGroupDto dto)
        {
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(dto), System.Text.Encoding.UTF8, "application/json");
            var resp = await _httpClient.PutAsync($"{ApiBaseUrl}/user-groups/{id}", content);
            resp.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(string id)
        {
            var resp = await _httpClient.DeleteAsync($"{ApiBaseUrl}/user-groups/{id}");
            resp.EnsureSuccessStatusCode();
        }
    }
}

