using Microsoft.JSInterop;

namespace HotelDruid.Client.Services;

public interface IActiveYearService
{
    int CurrentYear { get; }
    Task InitializeAsync();
    Task SetActiveYearAsync(int year, bool persistPreference = true);
}

public sealed class ActiveYearService : IActiveYearService
{
    private const string StorageKey = "hoteldruid.ui.activeYear";
    private readonly IJSRuntime _jsRuntime;

    public int CurrentYear { get; private set; } = DateTime.Now.Year;

    public ActiveYearService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task InitializeAsync()
    {
        try
        {
            var storedYear = await _jsRuntime.InvokeAsync<int?>("hotelDruidCulture.getStoredYear", StorageKey);
            if (storedYear is int year && year is >= 1900 and <= 2200)
            {
                CurrentYear = year;
                return;
            }
        }
        catch (JSException)
        {
            // Fall back to the current machine year if browser storage is unavailable.
        }

        CurrentYear = DateTime.Now.Year;
    }

    public async Task SetActiveYearAsync(int year, bool persistPreference = true)
    {
        if (year is < 1900 or > 2200)
        {
            return;
        }

        CurrentYear = year;

        if (!persistPreference)
        {
            return;
        }

        try
        {
            await _jsRuntime.InvokeVoidAsync("hotelDruidCulture.setStoredYear", StorageKey, year);
        }
        catch (JSException)
        {
            // Ignore client storage failures.
        }
    }
}