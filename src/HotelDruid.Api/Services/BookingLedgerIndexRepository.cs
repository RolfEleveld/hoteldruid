using System.Text.Json;

namespace HotelDruid.Api.Services;

/// <summary>
/// File-backed implementation of IBookingLedgerIndexRepository.
/// Stores a simple JSON array of ledger entry IDs under the booking folder.
/// </summary>
public class BookingLedgerIndexRepository : IBookingLedgerIndexRepository
{
    private readonly string _dataRoot;
    private readonly ILogger<BookingLedgerIndexRepository> _logger;
    private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

    public BookingLedgerIndexRepository(string dataRoot, ILogger<BookingLedgerIndexRepository> logger)
    {
        _dataRoot = dataRoot;
        _logger = logger;
    }

    private string GetIndexPath(string bookingId)
        => Path.Combine(_dataRoot, "bookings", bookingId, "_ledger_index.json");

    public async Task AppendEntryIdAsync(string bookingId, string ledgerEntryId)
    {
        if (string.IsNullOrWhiteSpace(bookingId) || string.IsNullOrWhiteSpace(ledgerEntryId))
            return;

        var path = GetIndexPath(bookingId);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);

        List<string> ids;
        if (File.Exists(path))
        {
            try
            {
                var json = await File.ReadAllTextAsync(path);
                ids = JsonSerializer.Deserialize<List<string>>(json, _json) ?? new();
            }
            catch
            {
                ids = new();
            }
        }
        else
        {
            ids = new();
        }

        if (!ids.Contains(ledgerEntryId))
        {
            ids.Add(ledgerEntryId);
            var json = JsonSerializer.Serialize(ids, _json);
            await File.WriteAllTextAsync(path, json);
        }
    }

    public async Task<List<string>> GetEntryIdsAsync(string bookingId)
    {
        var path = GetIndexPath(bookingId);
        if (!File.Exists(path))
            return new();

        try
        {
            var json = await File.ReadAllTextAsync(path);
            return JsonSerializer.Deserialize<List<string>>(json, _json) ?? new();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to read ledger index for booking {BookingId}", bookingId);
            return new();
        }
    }
}