using Microsoft.JSInterop;
using HotelDruid.Shared;

namespace HotelDruid.Client.Services;

public interface IDeskUserService
{
    string? Name { get; }
    bool FromIdentity { get; }
    event Action? Changed;
    Task InitializeAsync();
    Task ApplyIdentityAsync(bool authenticated, string? identityName);
    Task SetNameAsync(string? name);
    void ApplyHeader(HttpClient http);
}

public sealed class DeskUserService : IDeskUserService
{
    private readonly IJSRuntime _jsRuntime;
    private HttpClient? _http;

    public DeskUserService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public string? Name { get; private set; }

    public bool FromIdentity { get; private set; }

    public event Action? Changed;

    public async Task InitializeAsync()
    {
        try
        {
            var stored = await _jsRuntime.InvokeAsync<string?>("hotelDruidCulture.getStoredCulture", DeskUserNames.StorageKey);
            Name = DeskUserNames.Sanitize(stored);
        }
        catch (JSException)
        {
            Name = null;
        }
    }

    public Task ApplyIdentityAsync(bool authenticated, string? identityName)
    {
        if (authenticated && !string.IsNullOrWhiteSpace(identityName))
        {
            FromIdentity = true;
            Name = identityName.Trim();
            ApplyHeader(_http);
            Changed?.Invoke();
            return Task.CompletedTask;
        }

        FromIdentity = false;
        ApplyHeader(_http);
        Changed?.Invoke();
        return Task.CompletedTask;
    }

    public async Task SetNameAsync(string? name)
    {
        if (FromIdentity)
            return;

        Name = DeskUserNames.Sanitize(name);
        try
        {
            if (Name is null)
                await _jsRuntime.InvokeVoidAsync("hotelDruidCulture.setStoredCulture", DeskUserNames.StorageKey, string.Empty);
            else
                await _jsRuntime.InvokeVoidAsync("hotelDruidCulture.setStoredCulture", DeskUserNames.StorageKey, Name);
        }
        catch (JSException)
        {
            // The name still applies to this session when storage is blocked.
        }

        ApplyHeader(_http);
        Changed?.Invoke();
    }

    public void Attach(HttpClient http)
    {
        _http = http;
        ApplyHeader(http);
    }

    public void ApplyHeader(HttpClient? http)
    {
        if (http is null)
            return;

        http.DefaultRequestHeaders.Remove(DeskUserNames.HeaderName);
        if (!FromIdentity && !string.IsNullOrWhiteSpace(Name))
            http.DefaultRequestHeaders.TryAddWithoutValidation(DeskUserNames.HeaderName, Name);
    }
}
