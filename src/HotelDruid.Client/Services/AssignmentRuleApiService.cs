using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HotelDruid.Client.Services
{
    public interface IAssignmentRuleApiService
    {
        Task<List<AssignmentRuleDto>> ListAsync(int? year = null);
        Task<AssignmentRuleDto?> GetAsync(int year, string id);
        Task<string> CreateAsync(AssignmentRuleDto dto);
        Task UpdateAsync(int year, string id, AssignmentRuleDto dto);
        Task DeleteAsync(int year, string id);
    }

    public record AssignmentRuleDto(
        string? Id = null,
        int Year = 0,
        string? RoomOrAgency = null,
        string? ClosedTariff = null,
        string? TariffPerRoom = null,
        int? StartPeriodId = null,
        int? EndPeriodId = null,
        string? Reason1 = null
    );

    public class AssignmentRuleApiService : IAssignmentRuleApiService
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

        public AssignmentRuleApiService(HttpClient httpClient) { _httpClient = httpClient; }

        public async Task<List<AssignmentRuleDto>> ListAsync(int? year = null)
        {
            try
            {
                var url = year.HasValue ? $"/api/assignment-rules?year={year}" : "/api/assignment-rules";
                var resp = await _httpClient.GetAsync(url);
                resp.EnsureSuccessStatusCode();
                return JsonSerializer.Deserialize<List<AssignmentRuleDto>>(await resp.Content.ReadAsStringAsync(), _json) ?? new();
            }
            catch { return new List<AssignmentRuleDto>(); }
        }

        public async Task<AssignmentRuleDto?> GetAsync(int year, string id)
        {
            try
            {
                var resp = await _httpClient.GetAsync($"/api/assignment-rules/{year}/{id}");
                resp.EnsureSuccessStatusCode();
                return JsonSerializer.Deserialize<AssignmentRuleDto>(await resp.Content.ReadAsStringAsync(), _json);
            }
            catch { return null; }
        }

        public async Task<string> CreateAsync(AssignmentRuleDto dto)
        {
            var content = new StringContent(JsonSerializer.Serialize(dto, _json), Encoding.UTF8, "application/json");
            var resp = await _httpClient.PostAsync("/api/assignment-rules", content);
            resp.EnsureSuccessStatusCode();
            var created = JsonSerializer.Deserialize<AssignmentRuleDto>(await resp.Content.ReadAsStringAsync(), _json);
            return created?.Id ?? string.Empty;
        }

        public async Task UpdateAsync(int year, string id, AssignmentRuleDto dto)
        {
            var content = new StringContent(JsonSerializer.Serialize(dto, _json), Encoding.UTF8, "application/json");
            var resp = await _httpClient.PutAsync($"/api/assignment-rules/{year}/{id}", content);
            resp.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(int year, string id)
        {
            var resp = await _httpClient.DeleteAsync($"/api/assignment-rules/{year}/{id}");
            resp.EnsureSuccessStatusCode();
        }
    }
}

