using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HotelDruid.Client.Services
{
    public interface IPosSaleApiService
    {
        Task<List<PosSaleClientDto>> ListAsync(int? year = null);
        Task<PosSaleClientDto> CreateAsync(PosSaleClientDto dto);
        Task<PosSaleClientDto> ReverseAsync(int year, string id);
    }

    public record PosSaleClientDto(
        string? Id = null,
        int Year = 0,
        DateOnly? Date = null,
        string? Description = null,
        double? Amount = null,
        string? Settlement = null,
        string? BookingId = null,
        string? ClientId = null,
        string? CashRegisterId = null,
        string? PaymentMethod = null,
        string? Currency = null,
        string? Person = null,
        double? RunningCashTotal = null,
        string? ExtraCostId = null,
        string? BookingCostId = null,
        string? EntryType = null,
        string? DebitAccount = null,
        string? CreditAccount = null,
        string? ReversesId = null,
        string? ReversedById = null
    );

    public class PosSaleApiService : IPosSaleApiService
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

        public PosSaleApiService(HttpClient httpClient) { _httpClient = httpClient; }

        public async Task<List<PosSaleClientDto>> ListAsync(int? year = null)
        {
            var url = year.HasValue ? $"/api/pos-sales?year={year}" : "/api/pos-sales";
            var resp = await _httpClient.GetAsync(url);
            resp.EnsureSuccessStatusCode();
            return JsonSerializer.Deserialize<List<PosSaleClientDto>>(await resp.Content.ReadAsStringAsync(), _json) ?? new();
        }

        public async Task<PosSaleClientDto> CreateAsync(PosSaleClientDto dto)
        {
            var content = new StringContent(JsonSerializer.Serialize(dto, _json), Encoding.UTF8, "application/json");
            var resp = await _httpClient.PostAsync("/api/pos-sales", content);
            if (!resp.IsSuccessStatusCode)
                throw new InvalidOperationException(await ReadError(resp));
            return JsonSerializer.Deserialize<PosSaleClientDto>(await resp.Content.ReadAsStringAsync(), _json)
                ?? throw new InvalidOperationException("Save failed.");
        }

        public async Task<PosSaleClientDto> ReverseAsync(int year, string id)
        {
            var resp = await _httpClient.PostAsync($"/api/pos-sales/{year}/{id}/reverse", new StringContent("{}", Encoding.UTF8, "application/json"));
            if (!resp.IsSuccessStatusCode)
                throw new InvalidOperationException(await ReadError(resp));
            return JsonSerializer.Deserialize<PosSaleClientDto>(await resp.Content.ReadAsStringAsync(), _json)
                ?? throw new InvalidOperationException("Reverse failed.");
        }

        private static async Task<string> ReadError(HttpResponseMessage resp)
        {
            try
            {
                using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
                if (doc.RootElement.TryGetProperty("error", out var error))
                    return error.GetString() ?? "Save failed.";
            }
            catch { }
            return "Save failed.";
        }
    }
}
