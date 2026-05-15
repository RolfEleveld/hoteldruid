using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace HotelDruid.Client.Services
{
    public interface IUserApiService
    {
        Task<List<UserDto>> GetAllAsync();
        Task<UserDto?> GetByIdAsync(string id);
        Task<string> CreateAsync(UserWriteDto dto);
        Task UpdateAsync(string id, UserWriteDto dto);
        Task DeleteAsync(string id);
        Task ChangePasswordAsync(string id, string newPassword);
    }

    /// <summary>Read DTO — never contains password or salt.</summary>
    public record UserDto(
        string Id,
        string? Username,
        string? PasswordType,
        string? CreatedAt
    );

    /// <summary>Write DTO — used for create/update when password is needed.</summary>
    public record UserWriteDto(
        string? Username,
        string? Password,
        string? PasswordType
    );

    public class UserApiService : IUserApiService
    {
        private readonly HttpClient _httpClient;
        private const string ApiBaseUrl = "/api";

        public UserApiService(HttpClient httpClient) => _httpClient = httpClient;

        public async Task<List<UserDto>> GetAllAsync()
        {
            try
            {
                var resp = await _httpClient.GetAsync($"{ApiBaseUrl}/users");
                resp.EnsureSuccessStatusCode();
                return System.Text.Json.JsonSerializer.Deserialize<List<UserDto>>(await resp.Content.ReadAsStringAsync()) ?? new();
            }
            catch { return new List<UserDto>(); }
        }

        public async Task<UserDto?> GetByIdAsync(string id)
        {
            try
            {
                var resp = await _httpClient.GetAsync($"{ApiBaseUrl}/users/{id}");
                resp.EnsureSuccessStatusCode();
                return System.Text.Json.JsonSerializer.Deserialize<UserDto>(await resp.Content.ReadAsStringAsync());
            }
            catch { return null; }
        }

        public async Task<string> CreateAsync(UserWriteDto dto)
        {
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(dto), System.Text.Encoding.UTF8, "application/json");
            var resp = await _httpClient.PostAsync($"{ApiBaseUrl}/users", content);
            resp.EnsureSuccessStatusCode();
            var created = System.Text.Json.JsonSerializer.Deserialize<UserDto>(await resp.Content.ReadAsStringAsync());
            return created?.Id ?? string.Empty;
        }

        public async Task UpdateAsync(string id, UserWriteDto dto)
        {
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(dto), System.Text.Encoding.UTF8, "application/json");
            var resp = await _httpClient.PutAsync($"{ApiBaseUrl}/users/{id}", content);
            resp.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(string id)
        {
            var resp = await _httpClient.DeleteAsync($"{ApiBaseUrl}/users/{id}");
            resp.EnsureSuccessStatusCode();
        }

        public async Task ChangePasswordAsync(string id, string newPassword)
        {
            var body = System.Text.Json.JsonSerializer.Serialize(new { Password = newPassword });
            var content = new StringContent(body, System.Text.Encoding.UTF8, "application/json");
            var resp = await _httpClient.PostAsync($"{ApiBaseUrl}/users/{id}/change-password", content);
            resp.EnsureSuccessStatusCode();
        }
    }
}

