using System.Net.Http.Json;
using HotelDruid.Shared;
using HotelDruid.Shared.Configuration;

namespace HotelDruid.Client.Services;

public sealed class HotelClock
{
    private readonly HttpClient _http;
    private Task? _load;
    private string? _zone;

    public static string? ZoneId { get; private set; }

    public static DateOnly BusinessToday => HotelTime.Today(ZoneId);

    public HotelClock(HttpClient http) => _http = http;

    public async Task EnsureAsync()
    {
        _load ??= LoadAsync();
        await _load;
    }

    public DateOnly Today => HotelTime.Today(_zone);

    public DateTime Now => HotelTime.Now(_zone);

    public DateTime Display(DateTime stored) => HotelTime.Display(stored, _zone);

    public string Time(DateTime stored) => Display(stored).ToString("yyyy-MM-dd HH:mm");

    public string When(DateTime? stored) => stored is null ? "" : Display(stored.Value).ToString("g");

    private async Task LoadAsync()
    {
        try
        {
            var config = await _http.GetFromJsonAsync<SystemConfiguration>("/api/system/configuration");
            _zone = HotelTime.Resolve(config?.TimeZoneId);
            ZoneId = _zone;
        }
        catch
        {
            _zone = HotelTime.Resolve(null);
            ZoneId = _zone;
        }
    }
}
