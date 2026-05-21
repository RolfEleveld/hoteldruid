using Xunit;
using HotelDruid.Client.Services;
using System.Threading.Tasks;
using System.Globalization;

namespace HotelDruid.Client.Tests.Unit.Services
{
    /// <summary>
    /// Unit tests for LanguageService
    /// </summary>
    public class LanguageServiceTests : IAsyncLifetime
    {
        private LanguageService _languageService = null!;
        private FakeClientCultureService _cultureService = null!;

        public async Task InitializeAsync()
        {
            _cultureService = new FakeClientCultureService();
            _languageService = new LanguageService(_cultureService);
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

    internal sealed class FakeClientCultureService : IClientCultureService
    {
        private static readonly IReadOnlyList<CultureInfo> Cultures = new[]
        {
            CultureInfo.GetCultureInfo("en"),
            CultureInfo.GetCultureInfo("es"),
            CultureInfo.GetCultureInfo("it")
        };

        public CultureInfo CurrentCulture { get; private set; } = CultureInfo.GetCultureInfo("en");
        public IReadOnlyList<CultureInfo> SupportedCultures => Cultures;

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public Task SetCultureAsync(string cultureName, bool persistPreference = true)
        {
            CurrentCulture = CultureInfo.GetCultureInfo(cultureName);
            return Task.CompletedTask;
        }
    }
}

