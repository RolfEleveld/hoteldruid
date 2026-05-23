using System.Globalization;
using Microsoft.JSInterop;

namespace HotelDruid.Client.Services
{
    public interface IClientCultureService
    {
        CultureInfo CurrentCulture { get; }
        IReadOnlyList<CultureInfo> SupportedCultures { get; }
        Task InitializeAsync();
        Task SetCultureAsync(string cultureName, bool persistPreference = true);
    }

    public sealed class ClientCultureService : IClientCultureService
    {
        private const string StorageKey = "hoteldruid.ui.culture";
        private readonly IJSRuntime _jsRuntime;
        private static readonly IReadOnlyList<CultureInfo> SupportedCultureList = new[]
        {
            CultureInfo.GetCultureInfo("en"),
            CultureInfo.GetCultureInfo("es"),
            CultureInfo.GetCultureInfo("it")
        };

        public CultureInfo CurrentCulture { get; private set; } = CultureInfo.GetCultureInfo("en");
        public IReadOnlyList<CultureInfo> SupportedCultures => SupportedCultureList;

        public ClientCultureService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task InitializeAsync()
        {
            var preferredLanguages = new List<string>();

            try
            {
                var storedCulture = await _jsRuntime.InvokeAsync<string?>("hotelDruidCulture.getStoredCulture", StorageKey);
                if (!string.IsNullOrWhiteSpace(storedCulture))
                {
                    preferredLanguages.Add(storedCulture);
                }

                var browserLanguages = await _jsRuntime.InvokeAsync<string[]>("hotelDruidCulture.getBrowserLanguages");
                if (browserLanguages is not null)
                {
                    preferredLanguages.AddRange(browserLanguages);
                }
            }
            catch (JSException)
            {
                // Fall back to English if browser APIs are unavailable.
            }

            var selectedCulture = ResolveBestMatch(preferredLanguages);
            await ApplyCultureAsync(selectedCulture, persistPreference: false);
        }

        public async Task SetCultureAsync(string cultureName, bool persistPreference = true)
        {
            var selectedCulture = ResolveBestMatch(new[] { cultureName });
            await ApplyCultureAsync(selectedCulture, persistPreference);
        }

        private async Task ApplyCultureAsync(CultureInfo culture, bool persistPreference)
        {
            CurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;

            try
            {
                await _jsRuntime.InvokeVoidAsync("hotelDruidCulture.setDocumentLanguage", culture.Name);
                if (persistPreference)
                {
                    await _jsRuntime.InvokeVoidAsync("hotelDruidCulture.setStoredCulture", StorageKey, culture.Name);
                }
            }
            catch (JSException)
            {
                // Ignore client storage/document update failures.
            }
        }

        private static CultureInfo ResolveBestMatch(IEnumerable<string> preferredLanguages)
        {
            var supportedByName = SupportedCultureList.ToDictionary(c => c.Name, StringComparer.OrdinalIgnoreCase);
            var supportedByNeutral = SupportedCultureList.ToDictionary(c => c.TwoLetterISOLanguageName, StringComparer.OrdinalIgnoreCase);

            foreach (var requested in preferredLanguages.Where(x => !string.IsNullOrWhiteSpace(x)))
            {
                var normalized = requested.Trim();
                if (supportedByName.TryGetValue(normalized, out var exact))
                {
                    return exact;
                }

                try
                {
                    var culture = CultureInfo.GetCultureInfo(normalized);
                    if (supportedByNeutral.TryGetValue(culture.TwoLetterISOLanguageName, out var neutral))
                    {
                        return neutral;
                    }
                }
                catch (CultureNotFoundException)
                {
                    var separatorIndex = normalized.IndexOf('-');
                    var language = separatorIndex > 0 ? normalized[..separatorIndex] : normalized;
                    if (supportedByNeutral.TryGetValue(language, out var neutral))
                    {
                        return neutral;
                    }
                }
            }

            return supportedByNeutral["en"];
        }
    }
}

