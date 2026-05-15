using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace HotelDruid.Client.Services
{
    public interface IWarehouseApiService
    {
        Task<List<WarehouseDto>> GetWarehousesAsync();
        Task<WarehouseDto?> GetWarehouseAsync(string id);
        Task<string> CreateWarehouseAsync(WarehouseDto dto);
        Task UpdateWarehouseAsync(string id, WarehouseDto dto);
        Task DeleteWarehouseAsync(string id);
    }

    public record WarehouseDto(
        string? Id,
        string? Name,
        string? Code,
        string? Description,
        string? FloorNumber,
        string? HouseNumber,
        DateTime? CreatedAt
    );

    public class WarehouseApiService : IWarehouseApiService
    {
        private readonly HttpClient _httpClient;
        private const string ApiBaseUrl = "/api";

        public WarehouseApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<WarehouseDto>> GetWarehousesAsync()
        {
            try
            {
                var resp = await _httpClient.GetAsync($"{ApiBaseUrl}/warehouses");
                resp.EnsureSuccessStatusCode();
                var content = await resp.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<List<WarehouseDto>>(content) ?? new();
            }
            catch
            {
                return new List<WarehouseDto>();
            }
        }

        public async Task<WarehouseDto?> GetWarehouseAsync(string id)
        {
            try
            {
                var resp = await _httpClient.GetAsync($"{ApiBaseUrl}/warehouses/{id}");
                resp.EnsureSuccessStatusCode();
                var content = await resp.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<WarehouseDto>(content);
            }
            catch
            {
                return null;
            }
        }

        public async Task<string> CreateWarehouseAsync(WarehouseDto dto)
        {
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(dto), System.Text.Encoding.UTF8, "application/json");
            var resp = await _httpClient.PostAsync($"{ApiBaseUrl}/warehouses", content);
            resp.EnsureSuccessStatusCode();
            var body = await resp.Content.ReadAsStringAsync();
            var created = System.Text.Json.JsonSerializer.Deserialize<WarehouseDto>(body);
            return created?.Id ?? string.Empty;
        }

        public async Task UpdateWarehouseAsync(string id, WarehouseDto dto)
        {
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(dto), System.Text.Encoding.UTF8, "application/json");
            var resp = await _httpClient.PutAsync($"{ApiBaseUrl}/warehouses/{id}", content);
            resp.EnsureSuccessStatusCode();
        }

        public async Task DeleteWarehouseAsync(string id)
        {
            var resp = await _httpClient.DeleteAsync($"{ApiBaseUrl}/warehouses/{id}");
            resp.EnsureSuccessStatusCode();
        }
    }
}

