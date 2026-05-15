using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace HotelDruid.Client.Services
{
    public interface IContractTemplateApiService
    {
        Task<List<ContractTemplateApiDto>> GetContractTemplatesAsync(string? type = null);
        Task<ContractTemplateApiDto?> GetContractTemplateAsync(string type, int number);
        Task<ContractTemplateApiDto?> CreateContractTemplateAsync(ContractTemplateApiDto ct);
        Task<ContractTemplateApiDto?> UpdateContractTemplateAsync(string type, int number, ContractTemplateApiDto ct);
        Task DeleteContractTemplateAsync(string type, int number);
    }

    public record ContractTemplateApiDto(
        string? Id,
        string? Type,
        int Number,
        string? Content
    );

    public class ContractTemplateApiService : IContractTemplateApiService
    {
        private readonly HttpClient _httpClient;
        private const string ApiBaseUrl = "/api";

        public ContractTemplateApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ContractTemplateApiDto>> GetContractTemplatesAsync(string? type = null)
        {
            try
            {
                var query = type != null ? $"?type={Uri.EscapeDataString(type)}" : "";
                var resp = await _httpClient.GetAsync($"{ApiBaseUrl}/contract-templates{query}");
                resp.EnsureSuccessStatusCode();
                return System.Text.Json.JsonSerializer.Deserialize<List<ContractTemplateApiDto>>(await resp.Content.ReadAsStringAsync()) ?? new();
            }
            catch { return new List<ContractTemplateApiDto>(); }
        }

        public async Task<ContractTemplateApiDto?> GetContractTemplateAsync(string type, int number)
        {
            try
            {
                var resp = await _httpClient.GetAsync($"{ApiBaseUrl}/contract-templates/{Uri.EscapeDataString(type)}/{number}");
                resp.EnsureSuccessStatusCode();
                return System.Text.Json.JsonSerializer.Deserialize<ContractTemplateApiDto>(await resp.Content.ReadAsStringAsync());
            }
            catch { return null; }
        }

        public async Task<ContractTemplateApiDto?> CreateContractTemplateAsync(ContractTemplateApiDto ct)
        {
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(ct), System.Text.Encoding.UTF8, "application/json");
            var resp = await _httpClient.PostAsync($"{ApiBaseUrl}/contract-templates", content);
            resp.EnsureSuccessStatusCode();
            return System.Text.Json.JsonSerializer.Deserialize<ContractTemplateApiDto>(await resp.Content.ReadAsStringAsync());
        }

        public async Task<ContractTemplateApiDto?> UpdateContractTemplateAsync(string type, int number, ContractTemplateApiDto ct)
        {
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(ct), System.Text.Encoding.UTF8, "application/json");
            var resp = await _httpClient.PutAsync($"{ApiBaseUrl}/contract-templates/{Uri.EscapeDataString(type)}/{number}", content);
            resp.EnsureSuccessStatusCode();
            return System.Text.Json.JsonSerializer.Deserialize<ContractTemplateApiDto>(await resp.Content.ReadAsStringAsync());
        }

        public async Task DeleteContractTemplateAsync(string type, int number)
        {
            var resp = await _httpClient.DeleteAsync($"{ApiBaseUrl}/contract-templates/{Uri.EscapeDataString(type)}/{number}");
            resp.EnsureSuccessStatusCode();
        }
    }
}

