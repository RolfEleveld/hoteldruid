using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HotelDruid.Client.Services
{
    public interface IGroupMembershipApiService
    {
        Task<List<GroupMembershipDto>> GetByUserAsync(int userId);
        Task<string> CreateAsync(GroupMembershipDto dto);
        Task DeleteAsync(int userId, int groupId);
    }

    public record GroupMembershipDto(
        string? Id,
        int? UserId,
        int? GroupId,
        string? CreatedAt = null
    );

    public class GroupMembershipApiService : IGroupMembershipApiService
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

        public GroupMembershipApiService(HttpClient httpClient) { _httpClient = httpClient; }

        public async Task<List<GroupMembershipDto>> GetByUserAsync(int userId)
        {
            try
            {
                var resp = await _httpClient.GetAsync($"/api/group-memberships?userId={userId}");
                resp.EnsureSuccessStatusCode();
                return JsonSerializer.Deserialize<List<GroupMembershipDto>>(await resp.Content.ReadAsStringAsync(), _json) ?? new();
            }
            catch { return new List<GroupMembershipDto>(); }
        }

        public async Task<string> CreateAsync(GroupMembershipDto dto)
        {
            var content = new StringContent(JsonSerializer.Serialize(dto, _json), Encoding.UTF8, "application/json");
            var resp = await _httpClient.PostAsync("/api/group-memberships", content);
            resp.EnsureSuccessStatusCode();
            var created = JsonSerializer.Deserialize<GroupMembershipDto>(await resp.Content.ReadAsStringAsync(), _json);
            return created?.Id ?? string.Empty;
        }

        public async Task DeleteAsync(int userId, int groupId)
        {
            var resp = await _httpClient.DeleteAsync($"/api/group-memberships/{userId}/{groupId}");
            resp.EnsureSuccessStatusCode();
        }
    }
}

