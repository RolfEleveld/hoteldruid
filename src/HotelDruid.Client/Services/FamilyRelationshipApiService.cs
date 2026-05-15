using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace HotelDruid.Client.Services
{
    public interface IFamilyRelationshipApiService
    {
        Task<List<FamilyRelationshipDto>> GetAllAsync();
        Task<FamilyRelationshipDto?> GetByIdAsync(string id);
        Task<string> CreateAsync(FamilyRelationshipDto dto);
        Task UpdateAsync(string id, FamilyRelationshipDto dto);
        Task DeleteAsync(string id);
    }

    public record FamilyRelationshipDto(
        string Id,
        string? Name,
        string? Code,
        string? Code2,
        string? Code3,
        string? CreatedAt
    );

    public class FamilyRelationshipApiService : IFamilyRelationshipApiService
    {
        private readonly HttpClient _httpClient;
        private const string ApiBaseUrl = "/api";

        public FamilyRelationshipApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<FamilyRelationshipDto>> GetAllAsync()
        {
            try
            {
                var resp = await _httpClient.GetAsync($"{ApiBaseUrl}/family-relationships");
                resp.EnsureSuccessStatusCode();
                var content = await resp.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<List<FamilyRelationshipDto>>(content) ?? new();
            }
            catch
            {
                return new List<FamilyRelationshipDto>();
            }
        }

        public async Task<FamilyRelationshipDto?> GetByIdAsync(string id)
        {
            try
            {
                var resp = await _httpClient.GetAsync($"{ApiBaseUrl}/family-relationships/{id}");
                resp.EnsureSuccessStatusCode();
                var content = await resp.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<FamilyRelationshipDto>(content);
            }
            catch
            {
                return null;
            }
        }

        public async Task<string> CreateAsync(FamilyRelationshipDto dto)
        {
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(dto), System.Text.Encoding.UTF8, "application/json");
            var resp = await _httpClient.PostAsync($"{ApiBaseUrl}/family-relationships", content);
            resp.EnsureSuccessStatusCode();
            var body = await resp.Content.ReadAsStringAsync();
            var created = System.Text.Json.JsonSerializer.Deserialize<FamilyRelationshipDto>(body);
            return created?.Id ?? string.Empty;
        }

        public async Task UpdateAsync(string id, FamilyRelationshipDto dto)
        {
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(dto), System.Text.Encoding.UTF8, "application/json");
            var resp = await _httpClient.PutAsync($"{ApiBaseUrl}/family-relationships/{id}", content);
            resp.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(string id)
        {
            var resp = await _httpClient.DeleteAsync($"{ApiBaseUrl}/family-relationships/{id}");
            resp.EnsureSuccessStatusCode();
        }
    }
}

