using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace HotelDruid.Client.Services
{
    public interface IInventoryApiService
    {
        Task<List<InventoryDto>> GetInventoryAsync();
        Task<InventoryDto?> GetInventoryItemAsync(string id);
        Task<string> CreateInventoryAsync(InventoryDto dto);
        Task UpdateInventoryAsync(string id, InventoryDto dto);
        Task DeleteInventoryAsync(string id);
    }

    public record InventoryDto(
        string? Id,
        string? AssetId,
        string? RoomId,
        string? WarehouseId,
        int Quantity,
        int? MinQuantityDefault,
        bool? RequiredOnCheckin,
        DateTime? CreatedAt
    );

    public class InventoryApiService : IInventoryApiService
    {
        private readonly HttpClient _httpClient;
        private const string ApiBaseUrl = "/api";

        public InventoryApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<InventoryDto>> GetInventoryAsync()
        {
            try
            {
                var resp = await _httpClient.GetAsync($"{ApiBaseUrl}/inventory");
                resp.EnsureSuccessStatusCode();
                var content = await resp.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<List<InventoryDto>>(content) ?? new();
            }
            catch
            {
                return new List<InventoryDto>();
            }
        }

        public async Task<InventoryDto?> GetInventoryItemAsync(string id)
        {
            try
            {
                var resp = await _httpClient.GetAsync($"{ApiBaseUrl}/inventory/{id}");
                resp.EnsureSuccessStatusCode();
                var content = await resp.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<InventoryDto>(content);
            }
            catch
            {
                return null;
            }
        }

        public async Task<string> CreateInventoryAsync(InventoryDto dto)
        {
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(dto), System.Text.Encoding.UTF8, "application/json");
            var resp = await _httpClient.PostAsync($"{ApiBaseUrl}/inventory", content);
            resp.EnsureSuccessStatusCode();
            var body = await resp.Content.ReadAsStringAsync();
            var created = System.Text.Json.JsonSerializer.Deserialize<InventoryDto>(body);
            return created?.Id ?? string.Empty;
        }

        public async Task UpdateInventoryAsync(string id, InventoryDto dto)
        {
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(dto), System.Text.Encoding.UTF8, "application/json");
            var resp = await _httpClient.PutAsync($"{ApiBaseUrl}/inventory/{id}", content);
            resp.EnsureSuccessStatusCode();
        }

        public async Task DeleteInventoryAsync(string id)
        {
            var resp = await _httpClient.DeleteAsync($"{ApiBaseUrl}/inventory/{id}");
            resp.EnsureSuccessStatusCode();
        }
    }
}

