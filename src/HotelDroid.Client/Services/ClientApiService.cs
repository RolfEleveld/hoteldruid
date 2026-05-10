using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HotelDroid.Client.Services
{
    public interface IClientApiService
    {
        Task<List<ClientDto>> GetAllAsync(string? lastName = null);
        Task<ClientDto?> GetByIdAsync(string id);
        Task<string> CreateAsync(ClientDto dto);
        Task UpdateAsync(string id, ClientDto dto);
        Task DeleteAsync(string id);
    }

    public record ClientDto(
        string? Id,
        string? LastName,
        string? FirstName = null,
        string? Nickname = null,
        string? Gender = null,
        string? Title = null,
        string? Language = null,
        string? DateOfBirth = null,
        string? BirthCity = null,
        string? BirthRegion = null,
        string? BirthNation = null,
        string? DocumentNumber = null,
        string? DocumentExpiry = null,
        string? DocumentType = null,
        string? DocumentCity = null,
        string? DocumentRegion = null,
        string? DocumentNation = null,
        string? Nationality = null,
        string? Nation = null,
        string? Region = null,
        string? City = null,
        string? Street = null,
        string? StreetNumber = null,
        string? PostalCode = null,
        string? Phone = null,
        string? Phone2 = null,
        string? Phone3 = null,
        string? Fax = null,
        string? Email = null,
        string? Email2 = null,
        string? Email3 = null,
        string? TaxCode = null,
        string? VatNumber = null,
        string? Notes = null,
        int? MaxOrderNumber = null,
        string? CompanionIds = null,
        string? DocumentsSent = null,
        string? CreatedAt = null
    );

    public class ClientApiService : IClientApiService
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

        public ClientApiService(HttpClient httpClient) { _httpClient = httpClient; }

        public async Task<List<ClientDto>> GetAllAsync(string? lastName = null)
        {
            try
            {
                var url = string.IsNullOrEmpty(lastName) ? "/api/clients" : $"/api/clients?lastName={Uri.EscapeDataString(lastName)}";
                var resp = await _httpClient.GetAsync(url);
                resp.EnsureSuccessStatusCode();
                return JsonSerializer.Deserialize<List<ClientDto>>(await resp.Content.ReadAsStringAsync(), _json) ?? new();
            }
            catch { return new List<ClientDto>(); }
        }

        public async Task<ClientDto?> GetByIdAsync(string id)
        {
            try
            {
                var resp = await _httpClient.GetAsync($"/api/clients/{id}");
                resp.EnsureSuccessStatusCode();
                return JsonSerializer.Deserialize<ClientDto>(await resp.Content.ReadAsStringAsync(), _json);
            }
            catch { return null; }
        }

        public async Task<string> CreateAsync(ClientDto dto)
        {
            var content = new StringContent(JsonSerializer.Serialize(dto, _json), Encoding.UTF8, "application/json");
            var resp = await _httpClient.PostAsync("/api/clients", content);
            resp.EnsureSuccessStatusCode();
            var created = JsonSerializer.Deserialize<ClientDto>(await resp.Content.ReadAsStringAsync(), _json);
            return created?.Id ?? string.Empty;
        }

        public async Task UpdateAsync(string id, ClientDto dto)
        {
            var content = new StringContent(JsonSerializer.Serialize(dto, _json), Encoding.UTF8, "application/json");
            var resp = await _httpClient.PutAsync($"/api/clients/{id}", content);
            resp.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(string id)
        {
            var resp = await _httpClient.DeleteAsync($"/api/clients/{id}");
            resp.EnsureSuccessStatusCode();
        }
    }
}
