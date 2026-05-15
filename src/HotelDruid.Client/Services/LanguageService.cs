using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace HotelDruid.Client.Services
{
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
        private readonly HttpClient _httpClient;
        private Dictionary<string, object> _currentLanguageData = new();
        private string _currentLanguage = "en";
        private Dictionary<string, Dictionary<string, object>> _languageCache = new();
        
        public List<LanguageOption> AvailableLanguages { get; private set; } = new();
        public string CurrentLanguage => _currentLanguage;
        public event Action? LanguageChanged;

        public LanguageService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Initialize the language service and load available languages
        /// </summary>
        public async Task InitializeAsync()
        {
            try
            {
                // Load English as default
                await LoadLanguageAsync("en");
                
                // Load all available languages
                foreach (var langFile in new[] { "en", "es", "it" })
                {
                    try
                    {
                        await LoadLanguageAsync(langFile);
                    }
                    catch { /* Continue if language fails to load */ }
                }

                // Set up available languages
                AvailableLanguages = new List<LanguageOption>
                {
                    new() { Code = "en", Name = "English" },
                    new() { Code = "es", Name = "Español" },
                    new() { Code = "it", Name = "Italiano" }
                };

                // Try to restore saved language preference
                var savedLanguage = await GetStoredLanguageAsync();
                if (!string.IsNullOrEmpty(savedLanguage) && _languageCache.ContainsKey(savedLanguage))
                {
                    await SetLanguageAsync(savedLanguage);
                }
                else
                {
                    // Set to browser language preference if available
                    var browserLang = await GetBrowserLanguageAsync();
                    if (_languageCache.ContainsKey(browserLang))
                    {
                        await SetLanguageAsync(browserLang);
                    }
                }
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
            if (languageCode == _currentLanguage) return;

            if (!_languageCache.ContainsKey(languageCode))
            {
                await LoadLanguageAsync(languageCode);
            }

            _currentLanguage = languageCode;
            _currentLanguageData = _languageCache.ContainsKey(languageCode)
                ? _languageCache[languageCode]
                : new();

            // Persist language preference
            await SaveLanguagePreferenceAsync(languageCode);
            
            LanguageChanged?.Invoke();
        }

        /// <summary>
        /// Get localized text for a given key
        /// </summary>
        public string GetText(string key, string defaultValue = "")
        {
            var keys = key.Split('.');
            object? current = _currentLanguageData;

            foreach (var k in keys)
            {
                if (current is Dictionary<string, object> dict && dict.TryGetValue(k, out var value))
                {
                    current = value;
                }
                else if (current is JsonElement jsonElem &&
                         jsonElem.ValueKind == JsonValueKind.Object &&
                         jsonElem.TryGetProperty(k, out var jsonProp))
                {
                    current = jsonProp;
                }
                else
                {
                    return defaultValue;
                }
            }

            if (current is JsonElement leaf)
                return leaf.ValueKind == JsonValueKind.String ? leaf.GetString() ?? defaultValue : leaf.ToString();

            return current?.ToString() ?? defaultValue;
        }

        /// <summary>
        /// Load a language file from resources
        /// </summary>
        private async Task LoadLanguageAsync(string languageCode)
        {
            try
            {
                var filePath = $"Resources/Lang/{languageCode}.json";
                var response = await _httpClient.GetAsync(filePath);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<Dictionary<string, object>>(content);
                    
                    if (data != null)
                    {
                        _languageCache[languageCode] = data;
                        
                        if (_currentLanguage == languageCode)
                        {
                            _currentLanguageData = data;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error loading language {languageCode}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Get language preference from localStorage
        /// </summary>
        private async Task<string> GetStoredLanguageAsync()
        {
            // This would use JS interop in a real implementation
            // For now, return empty string
            return await Task.FromResult(string.Empty);
        }

        /// <summary>
        /// Save language preference to localStorage
        /// </summary>
        private async Task SaveLanguagePreferenceAsync(string languageCode)
        {
            // This would use JS interop in a real implementation
            await Task.CompletedTask;
        }

        /// <summary>
        /// Get browser language preference
        /// </summary>
        private async Task<string> GetBrowserLanguageAsync()
        {
            // This would use JS interop in a real implementation
            // For now, return default
            return await Task.FromResult("en");
        }
    }
}

