using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HotelDroid.Client.Services
{
    public interface IClientDataApiService
    {
        Task<List<ClientDataDto>> GetByClientAsync(string clientId);
        Task<string> CreateAsync(ClientDataDto dto);
        Task UpdateAsync(string id, ClientDataDto dto);
        Task DeleteAsync(string id);
    }

    public record ClientDataDto(
        string? Id,
        string? ClientId,
        int? Number = null,
        string? Type = null,
        string? Text1 = null,
        string? Text2 = null,
        string? Text3 = null,
        string? Text4 = null,
        string? Text5 = null,
        string? Text6 = null,
        string? Text7 = null,
        string? Text8 = null,
        string? CreatedAt = null
    );

    public class ClientDataApiService : IClientDataApiService
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

        public ClientDataApiService(HttpClient httpClient) { _httpClient = httpClient; }

        public async Task<List<ClientDataDto>> GetByClientAsync(string clientId)
        {
            try
            {
                var resp = await _httpClient.GetAsync($"/api/client-data?clientId={Uri.EscapeDataString(clientId)}");
                resp.EnsureSuccessStatusCode();
                return JsonSerializer.Deserialize<List<ClientDataDto>>(await resp.Content.ReadAsStringAsync(), _json) ?? new();
            }
            catch { return new List<ClientDataDto>(); }
        }

        public async Task<string> CreateAsync(ClientDataDto dto)
        {
            var content = new StringContent(JsonSerializer.Serialize(dto, _json), Encoding.UTF8, "application/json");
            var resp = await _httpClient.PostAsync("/api/client-data", content);
            resp.EnsureSuccessStatusCode();
            var created = JsonSerializer.Deserialize<ClientDataDto>(await resp.Content.ReadAsStringAsync(), _json);
            return created?.Id ?? string.Empty;
        }

        public async Task UpdateAsync(string id, ClientDataDto dto)
        {
            var content = new StringContent(JsonSerializer.Serialize(dto, _json), Encoding.UTF8, "application/json");
            var resp = await _httpClient.PutAsync($"/api/client-data/{id}", content);
            resp.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(string id)
        {
            var resp = await _httpClient.DeleteAsync($"/api/client-data/{id}");
            resp.EnsureSuccessStatusCode();
        }
    }
}
