using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace HotelDroid.Client.Services
{
    public interface IMessageApiService
    {
        Task<List<MessageApiDto>> GetMessagesAsync(string? userId = null, string? status = null);
        Task<MessageApiDto?> GetMessageAsync(string id);
        Task<MessageApiDto?> CreateMessageAsync(MessageApiDto msg);
        Task<MessageApiDto?> MarkReadAsync(string id);
        Task<MessageApiDto?> ArchiveAsync(string id);
        Task DeleteMessageAsync(string id);
    }

    public record MessageApiDto(
        string? Id,
        string? MessageType,
        string? Status,
        string? Sender,
        string? Body,
        string? RecipientUserIds,
        string? CreatedAt,
        string? SeenAt
    );

    public class MessageApiService : IMessageApiService
    {
        private readonly HttpClient _httpClient;
        private const string ApiBaseUrl = "/api";

        public MessageApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<MessageApiDto>> GetMessagesAsync(string? userId = null, string? status = null)
        {
            try
            {
                var query = "";
                if (userId != null) query += $"?userId={Uri.EscapeDataString(userId)}";
                if (status != null) query += (query == "" ? "?" : "&") + $"status={Uri.EscapeDataString(status)}";
                var resp = await _httpClient.GetAsync($"{ApiBaseUrl}/messages{query}");
                resp.EnsureSuccessStatusCode();
                return System.Text.Json.JsonSerializer.Deserialize<List<MessageApiDto>>(await resp.Content.ReadAsStringAsync()) ?? new();
            }
            catch { return new List<MessageApiDto>(); }
        }

        public async Task<MessageApiDto?> GetMessageAsync(string id)
        {
            try
            {
                var resp = await _httpClient.GetAsync($"{ApiBaseUrl}/messages/{id}");
                resp.EnsureSuccessStatusCode();
                return System.Text.Json.JsonSerializer.Deserialize<MessageApiDto>(await resp.Content.ReadAsStringAsync());
            }
            catch { return null; }
        }

        public async Task<MessageApiDto?> CreateMessageAsync(MessageApiDto msg)
        {
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(msg), System.Text.Encoding.UTF8, "application/json");
            var resp = await _httpClient.PostAsync($"{ApiBaseUrl}/messages", content);
            resp.EnsureSuccessStatusCode();
            return System.Text.Json.JsonSerializer.Deserialize<MessageApiDto>(await resp.Content.ReadAsStringAsync());
        }

        public async Task<MessageApiDto?> MarkReadAsync(string id)
        {
            var resp = await _httpClient.PutAsync($"{ApiBaseUrl}/messages/{id}/read", null);
            resp.EnsureSuccessStatusCode();
            return System.Text.Json.JsonSerializer.Deserialize<MessageApiDto>(await resp.Content.ReadAsStringAsync());
        }

        public async Task<MessageApiDto?> ArchiveAsync(string id)
        {
            var resp = await _httpClient.PutAsync($"{ApiBaseUrl}/messages/{id}/archive", null);
            resp.EnsureSuccessStatusCode();
            return System.Text.Json.JsonSerializer.Deserialize<MessageApiDto>(await resp.Content.ReadAsStringAsync());
        }

        public async Task DeleteMessageAsync(string id)
        {
            var resp = await _httpClient.DeleteAsync($"{ApiBaseUrl}/messages/{id}");
            resp.EnsureSuccessStatusCode();
        }
    }
}
