using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace HotelDroid.Client.Services
{
    public interface ICashRegisterApiService
    {
        Task<List<CashRegisterDto>> GetAllAsync();
        Task<CashRegisterDto?> GetByIdAsync(string id);
        Task<string> CreateAsync(CashRegisterDto dto);
        Task UpdateAsync(string id, CashRegisterDto dto);
        Task DeleteAsync(string id);
    }

    public record CashRegisterDto(
        string Id,
        string? Name,
        string? Status,
        string? Code,
        string? Description,
        string? CreatedAt
    );

    public class CashRegisterApiService : ICashRegisterApiService
    {
        private readonly HttpClient _httpClient;
        private const string ApiBaseUrl = "/api";

        public CashRegisterApiService(HttpClient httpClient) => _httpClient = httpClient;

        public async Task<List<CashRegisterDto>> GetAllAsync()
        {
            try
            {
                var resp = await _httpClient.GetAsync($"{ApiBaseUrl}/cash-registers");
                resp.EnsureSuccessStatusCode();
                return System.Text.Json.JsonSerializer.Deserialize<List<CashRegisterDto>>(await resp.Content.ReadAsStringAsync()) ?? new();
            }
            catch { return new List<CashRegisterDto>(); }
        }

        public async Task<CashRegisterDto?> GetByIdAsync(string id)
        {
            try
            {
                var resp = await _httpClient.GetAsync($"{ApiBaseUrl}/cash-registers/{id}");
                resp.EnsureSuccessStatusCode();
                return System.Text.Json.JsonSerializer.Deserialize<CashRegisterDto>(await resp.Content.ReadAsStringAsync());
            }
            catch { return null; }
        }

        public async Task<string> CreateAsync(CashRegisterDto dto)
        {
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(dto), System.Text.Encoding.UTF8, "application/json");
            var resp = await _httpClient.PostAsync($"{ApiBaseUrl}/cash-registers", content);
            resp.EnsureSuccessStatusCode();
            var created = System.Text.Json.JsonSerializer.Deserialize<CashRegisterDto>(await resp.Content.ReadAsStringAsync());
            return created?.Id ?? string.Empty;
        }

        public async Task UpdateAsync(string id, CashRegisterDto dto)
        {
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(dto), System.Text.Encoding.UTF8, "application/json");
            var resp = await _httpClient.PutAsync($"{ApiBaseUrl}/cash-registers/{id}", content);
            resp.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(string id)
        {
            var resp = await _httpClient.DeleteAsync($"{ApiBaseUrl}/cash-registers/{id}");
            resp.EnsureSuccessStatusCode();
        }
    }
}
