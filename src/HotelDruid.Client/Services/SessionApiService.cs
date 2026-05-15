using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace HotelDruid.Client.Services
{
    public interface ISessionApiService
    {
        Task<List<SessionApiDto>> GetSessionsAsync(int? userId = null);
        Task<SessionApiDto?> GetSessionAsync(string sessionId);
        Task<SessionApiDto?> CreateSessionAsync(SessionApiDto session);
        Task<SessionApiDto?> TouchSessionAsync(string sessionId);
        Task DeleteSessionAsync(string sessionId);
        Task DeleteSessionsForUserAsync(int userId);
    }

    public record SessionApiDto(
        string? SessionId,
        int? UserId,
        string? IpAddress,
        string? ConnectionType,
        string? LastAccess
    );

    public class SessionApiService : ISessionApiService
    {
        private readonly HttpClient _httpClient;
        private const string ApiBaseUrl = "/api";

        public SessionApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<SessionApiDto>> GetSessionsAsync(int? userId = null)
        {
            try
            {
                var query = userId.HasValue ? $"?userId={userId}" : "";
                var resp = await _httpClient.GetAsync($"{ApiBaseUrl}/sessions{query}");
                resp.EnsureSuccessStatusCode();
                return System.Text.Json.JsonSerializer.Deserialize<List<SessionApiDto>>(await resp.Content.ReadAsStringAsync()) ?? new();
            }
            catch { return new List<SessionApiDto>(); }
        }

        public async Task<SessionApiDto?> GetSessionAsync(string sessionId)
        {
            try
            {
                var resp = await _httpClient.GetAsync($"{ApiBaseUrl}/sessions/{sessionId}");
                resp.EnsureSuccessStatusCode();
                return System.Text.Json.JsonSerializer.Deserialize<SessionApiDto>(await resp.Content.ReadAsStringAsync());
            }
            catch { return null; }
        }

        public async Task<SessionApiDto?> CreateSessionAsync(SessionApiDto session)
        {
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(session), System.Text.Encoding.UTF8, "application/json");
            var resp = await _httpClient.PostAsync($"{ApiBaseUrl}/sessions", content);
            resp.EnsureSuccessStatusCode();
            return System.Text.Json.JsonSerializer.Deserialize<SessionApiDto>(await resp.Content.ReadAsStringAsync());
        }

        public async Task<SessionApiDto?> TouchSessionAsync(string sessionId)
        {
            var resp = await _httpClient.PutAsync($"{ApiBaseUrl}/sessions/{sessionId}/touch", null);
            resp.EnsureSuccessStatusCode();
            return System.Text.Json.JsonSerializer.Deserialize<SessionApiDto>(await resp.Content.ReadAsStringAsync());
        }

        public async Task DeleteSessionAsync(string sessionId)
        {
            var resp = await _httpClient.DeleteAsync($"{ApiBaseUrl}/sessions/{sessionId}");
            resp.EnsureSuccessStatusCode();
        }

        public async Task DeleteSessionsForUserAsync(int userId)
        {
            var resp = await _httpClient.DeleteAsync($"{ApiBaseUrl}/sessions?userId={userId}");
            resp.EnsureSuccessStatusCode();
        }
    }
}

