using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace HotelDruid.Client.Services
{
    public interface IIdentityDocumentTypeApiService
    {
        Task<List<IdentityDocumentTypeDto>> GetAllAsync();
        Task<IdentityDocumentTypeDto?> GetByIdAsync(string id);
        Task<string> CreateAsync(IdentityDocumentTypeDto dto);
        Task UpdateAsync(string id, IdentityDocumentTypeDto dto);
        Task DeleteAsync(string id);
    }

    public record IdentityDocumentTypeDto(
        string Id,
        string? Name,
        string? Code,
        string? Code2,
        string? Code3,
        string? CreatedAt
    );

    public class IdentityDocumentTypeApiService : IIdentityDocumentTypeApiService
    {
        private readonly HttpClient _httpClient;
        private const string ApiBaseUrl = "/api";

        public IdentityDocumentTypeApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<IdentityDocumentTypeDto>> GetAllAsync()
        {
            try
            {
                var resp = await _httpClient.GetAsync($"{ApiBaseUrl}/identity-document-types");
                resp.EnsureSuccessStatusCode();
                var content = await resp.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<List<IdentityDocumentTypeDto>>(content) ?? new();
            }
            catch
            {
                return new List<IdentityDocumentTypeDto>();
            }
        }

        public async Task<IdentityDocumentTypeDto?> GetByIdAsync(string id)
        {
            try
            {
                var resp = await _httpClient.GetAsync($"{ApiBaseUrl}/identity-document-types/{id}");
                resp.EnsureSuccessStatusCode();
                var content = await resp.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<IdentityDocumentTypeDto>(content);
            }
            catch
            {
                return null;
            }
        }

        public async Task<string> CreateAsync(IdentityDocumentTypeDto dto)
        {
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(dto), System.Text.Encoding.UTF8, "application/json");
            var resp = await _httpClient.PostAsync($"{ApiBaseUrl}/identity-document-types", content);
            resp.EnsureSuccessStatusCode();
            var body = await resp.Content.ReadAsStringAsync();
            var created = System.Text.Json.JsonSerializer.Deserialize<IdentityDocumentTypeDto>(body);
            return created?.Id ?? string.Empty;
        }

        public async Task UpdateAsync(string id, IdentityDocumentTypeDto dto)
        {
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(dto), System.Text.Encoding.UTF8, "application/json");
            var resp = await _httpClient.PutAsync($"{ApiBaseUrl}/identity-document-types/{id}", content);
            resp.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(string id)
        {
            var resp = await _httpClient.DeleteAsync($"{ApiBaseUrl}/identity-document-types/{id}");
            resp.EnsureSuccessStatusCode();
        }
    }
}

