using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HotelDroid.Client.Services
{
    public interface IUserRelationApiService
    {
        Task<UserRelationDto?> GetByUserAsync(int userId);
        Task<string> CreateAsync(UserRelationDto dto);
        Task UpdateAsync(int userId, UserRelationDto dto);
        Task DeleteAsync(int userId);
    }

    public record UserRelationDto(
        string? Id,
        int? UserId,
        int? NationId = null,
        int? RegionId = null,
        int? CityId = null,
        int? DocumentTypeId = null,
        int? FamilyRelationshipId = null,
        int? SuperiorId = null,
        int? IsDefault = null,
        string? CreatedAt = null
    );

    public class UserRelationApiService : IUserRelationApiService
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

        public UserRelationApiService(HttpClient httpClient) { _httpClient = httpClient; }

        public async Task<UserRelationDto?> GetByUserAsync(int userId)
        {
            try
            {
                var resp = await _httpClient.GetAsync($"/api/user-relations/{userId}");
                resp.EnsureSuccessStatusCode();
                return JsonSerializer.Deserialize<UserRelationDto>(await resp.Content.ReadAsStringAsync(), _json);
            }
            catch { return null; }
        }

        public async Task<string> CreateAsync(UserRelationDto dto)
        {
            var content = new StringContent(JsonSerializer.Serialize(dto, _json), Encoding.UTF8, "application/json");
            var resp = await _httpClient.PostAsync("/api/user-relations", content);
            resp.EnsureSuccessStatusCode();
            var created = JsonSerializer.Deserialize<UserRelationDto>(await resp.Content.ReadAsStringAsync(), _json);
            return created?.Id ?? string.Empty;
        }

        public async Task UpdateAsync(int userId, UserRelationDto dto)
        {
            var content = new StringContent(JsonSerializer.Serialize(dto, _json), Encoding.UTF8, "application/json");
            var resp = await _httpClient.PutAsync($"/api/user-relations/{userId}", content);
            resp.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(int userId)
        {
            var resp = await _httpClient.DeleteAsync($"/api/user-relations/{userId}");
            resp.EnsureSuccessStatusCode();
        }
    }
}
