using System.Globalization;
using System.Text.Json;
using System.Threading.Tasks;
using System.Reflection;
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

    /// <summary>
    /// Interface for language localization service
    /// </summary>
    public interface ILanguageService
    {
        Task InitializeAsync();
        Task SetLanguageAsync(string languageCode);
        string GetText(string key, string defaultValue = "");
        string CurrentLanguage { get; }
        List<LanguageOption> AvailableLanguages { get; }
        event Action? LanguageChanged;
    }

    /// <summary>
    /// Language option for UI selection
    /// </summary>
    public class LanguageOption
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// Language service implementation with multi-language support
    /// </summary>
    public class LanguageService : ILanguageService
    {
        private readonly IClientCultureService _cultureService;
        private JsonElement _currentLanguageData;
        private JsonElement _fallbackLanguageData;
        private string _currentLanguage = "en";
        private readonly Dictionary<string, JsonElement> _languageCache = new();
        
        public List<LanguageOption> AvailableLanguages { get; private set; } = new();
        public string CurrentLanguage => _currentLanguage;
        public event Action? LanguageChanged;

        public LanguageService(IClientCultureService cultureService)
        {
            _cultureService = cultureService;
        }

        /// <summary>
        /// Initialize the language service and load available languages
        /// </summary>
        public async Task InitializeAsync()
        {
            try
            {
                AvailableLanguages = new List<LanguageOption>
                {
                    new() { Code = "en", Name = "English" },
                    new() { Code = "es", Name = "Español" },
                    new() { Code = "it", Name = "Italiano" }
                };

                await _cultureService.InitializeAsync();
                await EnsureLanguageLoadedAsync("en");
                _fallbackLanguageData = _languageCache["en"];

                var selectedLanguage = NormalizeLanguageCode(_cultureService.CurrentCulture.Name);
                await EnsureLanguageLoadedAsync(selectedLanguage);
                SetCurrentLanguage(selectedLanguage);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error initializing language service: {ex.Message}");
            }
        }

        /// <summary>
        /// Set the current language
        /// </summary>
        public async Task SetLanguageAsync(string languageCode)
        {
            languageCode = NormalizeLanguageCode(languageCode);
            if (languageCode == _currentLanguage)
            {
                return;
            }

            await EnsureLanguageLoadedAsync(languageCode);
            await _cultureService.SetCultureAsync(languageCode);
            SetCurrentLanguage(languageCode);
            
            LanguageChanged?.Invoke();
        }

        /// <summary>
        /// Get localized text for a given key
        /// </summary>
        public string GetText(string key, string defaultValue = "")
        {
            var current = TryGetValue(_currentLanguageData, key);
            if (current is not null)
            {
                return current.Value.Value;
            }

            var fallback = TryGetValue(_fallbackLanguageData, key);
            return fallback?.Value ?? defaultValue;
        }

        private async Task EnsureLanguageLoadedAsync(string languageCode)
        {
            languageCode = NormalizeLanguageCode(languageCode);
            if (_languageCache.ContainsKey(languageCode))
            {
                return;
            }

            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = $"HotelDruid.Client.Resources.Lang.{languageCode}.json";
                await using var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream is null)
                {
                    return;
                }

                using var reader = new StreamReader(stream);
                var content = await reader.ReadToEndAsync();
                using var document = JsonDocument.Parse(content);
                _languageCache[languageCode] = document.RootElement.Clone();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error loading language {languageCode}: {ex.Message}");
                throw;
            }
        }

        private void SetCurrentLanguage(string languageCode)
        {
            _currentLanguage = languageCode;
            if (_languageCache.TryGetValue(languageCode, out var data))
            {
                _currentLanguageData = data;
            }
            else
            {
                _currentLanguageData = _fallbackLanguageData;
            }
        }

        private static string NormalizeLanguageCode(string? cultureName)
        {
            if (string.IsNullOrWhiteSpace(cultureName))
            {
                return "en";
            }

            try
            {
                return CultureInfo.GetCultureInfo(cultureName).TwoLetterISOLanguageName;
            }
            catch (CultureNotFoundException)
            {
                var separatorIndex = cultureName.IndexOf('-');
                if (separatorIndex > 0)
                {
                    return cultureName[..separatorIndex].ToLowerInvariant();
                }

                return cultureName.ToLowerInvariant();
            }
        }

        private static KeyValuePair<string, string>? TryGetValue(JsonElement root, string key)
        {
            if (root.ValueKind != JsonValueKind.Object)
            {
                return null;
            }

            var current = root;
            var parts = key.Split('.');

            foreach (var part in parts)
            {
                if (current.ValueKind != JsonValueKind.Object || !current.TryGetProperty(part, out current))
                {
                    return null;
                }
            }

            return current.ValueKind switch
            {
                JsonValueKind.String => new KeyValuePair<string, string>(key, current.GetString() ?? string.Empty),
                JsonValueKind.Number => new KeyValuePair<string, string>(key, current.ToString()),
                JsonValueKind.True => new KeyValuePair<string, string>(key, bool.TrueString),
                JsonValueKind.False => new KeyValuePair<string, string>(key, bool.FalseString),
                _ => new KeyValuePair<string, string>(key, current.ToString())
            };
        }
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

