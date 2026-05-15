using Xunit;
using HotelDroid.Client.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace HotelDroid.Client.Tests.Unit.Services
{
    /// <summary>
    /// Unit tests for LanguageService
    /// </summary>
    public class LanguageServiceTests : IAsyncLifetime
    {
        private LanguageService _languageService = null!;
        private HttpClientMock _httpClientMock = null!;

        public async Task InitializeAsync()
        {
            _httpClientMock = new HttpClientMock();
            _languageService = new LanguageService(_httpClientMock.GetHttpClient());
            await _languageService.InitializeAsync();
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        [Fact]
        public async Task InitializeAsync_ShouldLoadAvailableLanguages()
        {
            // Arrange
            var expectedLanguages = new[] { "en", "es", "it" };

            // Act
            var availableLanguages = _languageService.AvailableLanguages;

            // Assert
            Assert.NotNull(availableLanguages);
            Assert.Equal(3, availableLanguages.Count);
            Assert.All(expectedLanguages, lang =>
                Assert.Contains(availableLanguages, al => al.Code == lang)
            );
        }

        [Fact]
        public async Task SetLanguageAsync_ShouldChangeCurrentLanguage()
        {
            // Arrange
            var targetLanguage = "es";

            // Act
            await _languageService.SetLanguageAsync(targetLanguage);

            // Assert
            Assert.Equal(targetLanguage, _languageService.CurrentLanguage);
        }

        [Theory]
        [InlineData("en")]
        [InlineData("es")]
        [InlineData("it")]
        public async Task SetLanguageAsync_ShouldSupportMultipleLanguages(string language)
        {
            // Act
            await _languageService.SetLanguageAsync(language);

            // Assert
            Assert.Equal(language, _languageService.CurrentLanguage);
        }

        [Fact]
        public void GetText_ShouldReturnLocalizedString()
        {
            // Arrange
            var key = "common.export";
            var defaultValue = "Export";

            // Act
            var result = _languageService.GetText(key, defaultValue);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void GetText_ShouldReturnDefaultValueIfKeyNotFound()
        {
            // Arrange
            var key = "nonexistent.key.path";
            var defaultValue = "Default Text";

            // Act
            var result = _languageService.GetText(key, defaultValue);

            // Assert
            Assert.Equal(defaultValue, result);
        }

        [Fact]
        public void GetText_ShouldHandleNestedKeys()
        {
            // Arrange
            var nestedKey = "rooms.title";

            // Act
            var result = _languageService.GetText(nestedKey);

            // Assert
            Assert.NotNull(result);
            // Should return "Top Rooms" or similar for English
        }

        [Fact]
        public async Task LanguageChanged_EventShouldRaise()
        {
            // Arrange
            var eventRaised = false;
            _languageService.LanguageChanged += () => eventRaised = true;

            // Act
            await _languageService.SetLanguageAsync("es");

            // Assert
            Assert.True(eventRaised);
        }

        [Fact]
        public async Task SetLanguageAsync_WithSameLanguage_ShouldNotRaiseEvent()
        {
            // Arrange
            var eventCount = 0;
            _languageService.LanguageChanged += () => eventCount++;
            await _languageService.SetLanguageAsync("en");

            // Act
            await _languageService.SetLanguageAsync("en");

            // Assert
            Assert.Equal(0, eventCount); // Event should not be raised if language is same
        }

        [Fact]
        public async Task GetText_AfterLanguageChange_ShouldReturnCorrectTranslation()
        {
            // Arrange
            var key = "common.export";
            var englishValue = _languageService.GetText(key);

            // Act
            await _languageService.SetLanguageAsync("es");
            var spanishValue = _languageService.GetText(key);

            // Assert
            Assert.NotEqual(englishValue, spanishValue);
        }
    }

    /// <summary>
    /// Mock HttpClient for testing
    /// </summary>
    public class HttpClientMock : IDisposable
    {
        private readonly HttpMessageHandlerMock _handler;
        private readonly HttpClient _httpClient;

        public HttpClientMock()
        {
            _handler = new HttpMessageHandlerMock();
            _httpClient = new HttpClient(_handler) { BaseAddress = new Uri("http://localhost/") };
        }

        public HttpClient GetHttpClient() => _httpClient;

        public void Dispose()
        {
            _httpClient?.Dispose();
            _handler?.Dispose();
        }
    }

    /// <summary>
    /// Mock HTTP message handler for testing HTTP calls
    /// </summary>
    public class HttpMessageHandlerMock : HttpMessageHandler
    {
        private readonly Dictionary<string, string> _responses = new()
        {
            { "Resources/Lang/en.json", LoadEmbeddedResource("en.json") },
            { "Resources/Lang/es.json", LoadEmbeddedResource("es.json") },
            { "Resources/Lang/it.json", LoadEmbeddedResource("it.json") }
        };

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var uri = request.RequestUri?.ToString() ?? string.Empty;
            var matchedKey = _responses.Keys.FirstOrDefault(k => uri.EndsWith(k));
            
            if (matchedKey != null && _responses.TryGetValue(matchedKey, out var content))
            {
                return Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new StringContent(content, System.Text.Encoding.UTF8, "application/json")
                });
            }

            return Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.NotFound));
        }

        private static string LoadEmbeddedResource(string fileName)
        {
            // Return sample JSON for testing
            return fileName switch
            {
                "en.json" => @"{""language"":{""name"":""English"",""code"":""en""},""common"":{""export"":""Export"",""import"":""Import""},""rooms"":{""title"":""Top Rooms""}}",
                "es.json" => @"{""language"":{""name"":""Español"",""code"":""es""},""common"":{""export"":""Exportar"",""import"":""Importar""},""rooms"":{""title"":""Habitaciones Principales""}}",
                "it.json" => @"{""language"":{""name"":""Italiano"",""code"":""it""},""common"":{""export"":""Esporta"",""import"":""Importa""},""rooms"":{""title"":""Camere Principali""}}",
                _ => "{}"
            };
        }
    }
}
