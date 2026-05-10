using System.Text.Json;
using HotelDroid.Api.Models;
using HotelDroid.Shared;

namespace HotelDroid.Api.Services;

public class FileKvStore
{
    private readonly string _dataPath;
    private readonly object _lock = new();
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true };

    public FileKvStore(Microsoft.Extensions.Hosting.IHostEnvironment env)
    {
        _dataPath = Path.Combine(env.ContentRootPath ?? Directory.GetCurrentDirectory(), "data");
        Directory.CreateDirectory(_dataPath);
    }

    private string FileFor(string collection) => Path.Combine(_dataPath, collection + ".json");

    public Task<List<BookingDto>> GetBookingsAsync()
    {
        lock (_lock)
        {
            var path = FileFor("bookings");
            if (!File.Exists(path)) return Task.FromResult(new List<BookingDto>());
            var txt = File.ReadAllText(path);
            var list = JsonSerializer.Deserialize<List<BookingDto>>(txt, _jsonOptions) ?? new List<BookingDto>();
            return Task.FromResult(list);
        }
    }

    public async Task<BookingDto?> GetBookingAsync(string id)
    {
        var list = await GetBookingsAsync();
        return list.FirstOrDefault(x => x.Id == id);
    }

    public Task<BookingDto> AddBookingAsync(BookingDto booking)
    {
        lock (_lock)
        {
            var path = FileFor("bookings");
            var list = File.Exists(path)
                ? JsonSerializer.Deserialize<List<BookingDto>>(File.ReadAllText(path), _jsonOptions) ?? new List<BookingDto>()
                : new List<BookingDto>();

            var toAdd = booking with { Id = Guid.NewGuid().ToString("N") };
            list.Add(toAdd);
            File.WriteAllText(path, JsonSerializer.Serialize(list, _jsonOptions));
            return Task.FromResult(toAdd);
        }
    }
}
