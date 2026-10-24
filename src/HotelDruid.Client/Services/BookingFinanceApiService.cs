using System.Net.Http.Json;
using System.Text.Json;
using HotelDruid.Shared;

namespace HotelDruid.Client.Services;

public interface IBookingFinanceApiService
{
    Task<BookingFinanceSummaryDto?> GetSummaryAsync(int year, string bookingId);
    Task<List<BookingTransactionDto>> ListTransactionsAsync(int year, string bookingId);
    Task<BookingTransactionDto?> AppendTransactionAsync(int year, string bookingId, BookingTransactionWriteDto request);
    Task<List<LedgerLineDto>> ListLedgerLinesAsync(DateOnly from, DateOnly to, string? bookingId = null, bool unassigned = false);
    Task<List<ChartAccountDto>> ListChartOfAccountsAsync();
    string GetDocumentUrl(int year, string bookingId, string documentType, string format = "print");
}

public class BookingFinanceApiService : IBookingFinanceApiService
{
    private readonly HttpClient _httpClient;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public BookingFinanceApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<BookingFinanceSummaryDto?> GetSummaryAsync(int year, string bookingId)
    {
        var response = await _httpClient.GetAsync($"/api/bookings/{year}/{Uri.EscapeDataString(bookingId)}/finance");
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<BookingFinanceSummaryDto>(JsonOptions);
    }

    public async Task<List<BookingTransactionDto>> ListTransactionsAsync(int year, string bookingId)
    {
        var response = await _httpClient.GetAsync($"/api/bookings/{year}/{Uri.EscapeDataString(bookingId)}/transactions");
        if (!response.IsSuccessStatusCode) return [];
        return await response.Content.ReadFromJsonAsync<List<BookingTransactionDto>>(JsonOptions) ?? [];
    }

    public async Task<BookingTransactionDto?> AppendTransactionAsync(int year, string bookingId, BookingTransactionWriteDto request)
    {
        var response = await _httpClient.PostAsJsonAsync($"/api/bookings/{year}/{Uri.EscapeDataString(bookingId)}/transactions", request, JsonOptions);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<BookingTransactionDto>(JsonOptions);
    }

    public async Task<List<LedgerLineDto>> ListLedgerLinesAsync(DateOnly from, DateOnly to, string? bookingId = null, bool unassigned = false)
    {
        var query = $"/api/ledger/lines?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}";
        if (!string.IsNullOrWhiteSpace(bookingId))
            query += $"&bookingId={Uri.EscapeDataString(bookingId)}";
        if (unassigned)
            query += "&unassigned=true";
        var response = await _httpClient.GetAsync(query);
        if (!response.IsSuccessStatusCode) return [];
        return await response.Content.ReadFromJsonAsync<List<LedgerLineDto>>(JsonOptions) ?? [];
    }

    public async Task<List<ChartAccountDto>> ListChartOfAccountsAsync()
    {
        var response = await _httpClient.GetAsync("/api/ledger/accounts");
        if (!response.IsSuccessStatusCode) return [];
        return await response.Content.ReadFromJsonAsync<List<ChartAccountDto>>(JsonOptions) ?? [];
    }

    public string GetDocumentUrl(int year, string bookingId, string documentType, string format = "print")
    {
        var type = Uri.EscapeDataString(documentType);
        var id = Uri.EscapeDataString(bookingId);
        var requestedFormat = Uri.EscapeDataString(format);
        return $"/api/documents/render/{type}/1?bookingId={id}&year={year}&format={requestedFormat}";
    }
}
