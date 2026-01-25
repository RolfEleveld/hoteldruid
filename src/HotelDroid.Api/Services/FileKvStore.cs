using System.Text.Json;
using HotelDroid.Api.Models;

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

    public Task<List<ClientDto>> GetClientsAsync()
    {
        lock (_lock)
        {
            var path = FileFor("clients");
            if (!File.Exists(path)) return Task.FromResult(new List<ClientDto>());
            var txt = File.ReadAllText(path);
            var list = JsonSerializer.Deserialize<List<ClientDto>>(txt, _jsonOptions) ?? new List<ClientDto>();
            return Task.FromResult(list);
        }
    }

    public async Task<ClientDto?> GetClientAsync(int id)
    {
        var list = await GetClientsAsync();
        return list.FirstOrDefault(x => x.Id == id);
    }

    public Task<ClientDto> AddClientAsync(ClientDto client)
    {
        lock (_lock)
        {
            var path = FileFor("clients");
            var list = File.Exists(path)
                ? JsonSerializer.Deserialize<List<ClientDto>>(File.ReadAllText(path), _jsonOptions) ?? new List<ClientDto>()
                : new List<ClientDto>();

            var next = list.Any() ? list.Max(x => x.Id) + 1 : 1;
            var toAdd = client with { Id = next };
            list.Add(toAdd);
            File.WriteAllText(path, JsonSerializer.Serialize(list, _jsonOptions));
            return Task.FromResult(toAdd);
        }
    }

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

    public async Task<BookingDto?> GetBookingAsync(int id)
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

            var next = list.Any() ? list.Max(x => x.Id) + 1 : 1;
            var toAdd = booking with { Id = next };
            list.Add(toAdd);
            File.WriteAllText(path, JsonSerializer.Serialize(list, _jsonOptions));
            return Task.FromResult(toAdd);
        }
    }
}
