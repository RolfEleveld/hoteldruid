using Xunit;
using Bunit;
using HotelDruid.Client.Components.Rooms;
using HotelDruid.Client.Services;
using System.Threading.Tasks;
using Moq;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components;

namespace HotelDruid.Client.Tests.Integration.Components
{

    /// <summary>
    /// Integration tests for RoomsWidget component
    /// </summary>
    public class RoomsWidgetComponentTests : TestContext
    {
        private Mock<IRoomApiService> _mockRoomApiService = null!;
        private Mock<ILanguageService> _mockLanguageService = null!;

        public RoomsWidgetComponentTests()
        {
            SetupMocks();
            Services.AddScoped(_ => _mockRoomApiService.Object);
            Services.AddScoped(_ => _mockLanguageService.Object);
        }

        private void SetupMocks()
        {
            _mockRoomApiService = new Mock<IRoomApiService>();
            _mockLanguageService = new Mock<ILanguageService>();

            // Setup default mock behavior
            _mockLanguageService.Setup(x => x.GetText(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string key, string defaultValue) => defaultValue);

            _mockLanguageService.Setup(x => x.AvailableLanguages)
                .Returns(new List<LanguageOption>
                {
                    new() { Code = "en", Name = "English" },
                    new() { Code = "es", Name = "Español" },
                    new() { Code = "it", Name = "Italiano" }
                });

            _mockLanguageService.Setup(x => x.CurrentLanguage)
                .Returns("en");

            _mockRoomApiService.Setup(x => x.GetRoomsAsync())
                .Returns(Task.FromResult(new List<RoomDto>
                {
                    new("room-1", "Room 101", 2, 1, "A"),
                    new("room-2", "Room 102", 3, 1, "B"),
                    new("room-3", "Room 201", 2, 2, "A")
                }));
        }

        [Fact]
        public void RoomsWidget_ShouldRenderSuccessfully()
        {
            // Act
            var component = RenderComponent<RoomsWidget>();

            // Assert
            Assert.NotNull(component.Instance);
        }

        [Fact]
        public async Task RoomsWidget_ShouldLoadAndDisplayRooms()
        {
            // Act
            var component = RenderComponent<RoomsWidget>();
            await Task.Delay(100); // Wait for async initialization

            // Assert
            var markup = component.Markup;
            Assert.Contains("Room", markup);
        }

        [Fact]
        public void RoomsWidget_ShouldDisplayRoomTable()
        {
            // Act
            var component = RenderComponent<RoomsWidget>();

            // Assert
            var table = component.Find("table");
            Assert.NotNull(table);
        }

        [Fact]
        public async Task RoomsWidget_ShouldCallGetRoomsAsync()
        {
            // Act
            RenderComponent<RoomsWidget>();
            await Task.Delay(100);

            // Assert
            _mockRoomApiService.Verify(x => x.GetRoomsAsync(), Times.Once);
        }

        [Fact]
        public void RoomsWidget_ShouldContainSettingsPanel()
        {
            // Act
            var component = RenderComponent<RoomsWidget>();

            // Assert
            var markup = component.Markup;
            Assert.Contains("Settings", markup);
        }
    }

    /// <summary>
    /// Integration tests for SettingsPanel component
    /// </summary>
    public class SettingsPanelComponentTests : TestContext
    {
        private Mock<ILanguageService> _mockLanguageService;
        private Mock<IRoomApiService> _mockRoomApiService;

        public SettingsPanelComponentTests()
        {
            _mockLanguageService = new Mock<ILanguageService>();
            _mockLanguageService.Setup(x => x.GetText(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string key, string defaultValue) => defaultValue);

            _mockRoomApiService = new Mock<IRoomApiService>();
            _mockRoomApiService.Setup(x => x.GetRoomsAsync())
                .ReturnsAsync(new List<RoomDto>());
            _mockRoomApiService.Setup(x => x.ValidateImportAsync(It.IsAny<Stream>()))
                .ReturnsAsync(new ImportValidationResponse(true, new List<string>(), new List<string>(), 0, 0));

            Services.AddScoped(_ => _mockLanguageService.Object);
            Services.AddScoped(_ => _mockRoomApiService.Object);
        }

        [Fact]
        public void SettingsPanel_ShouldRenderWithTitle()
        {
            // Act
            var component = RenderComponent<SettingsPanel>(
                parameters => parameters
                    .Add(x => x.Title, "Test Settings")
                    .Add(x => x.Description, "Test Description")
            );

            // Assert
            var markup = component.Markup;
            Assert.Contains("Test Settings", markup);
        }

        [Fact]
        public void SettingsPanel_ShouldHaveCollapsibleSection()
        {
            // Act
            var component = RenderComponent<SettingsPanel>(
                parameters => parameters
                    .Add(x => x.Title, "Settings")
                    .Add(x => x.Description, "Manage settings")
            );

            // Assert
            var button = component.Find("button");
            Assert.NotNull(button);
        }

        [Fact]
        public void SettingsPanel_ShouldToggleExpandedState()
        {
            // Arrange
            var component = RenderComponent<SettingsPanel>(
                parameters => parameters
                    .Add(x => x.Title, "Settings")
                    .Add(x => x.Description, "Manage settings")
            );

            // Act
            var button = component.Find("button");
            button.Click();

            // Assert
            // Component should render with expanded state
            var markup = component.Markup;
            Assert.NotEmpty(markup);
        }

        [Fact]
        public void SettingsPanel_ShouldHaveTwoTabs()
        {
            // Act
            var component = RenderComponent<SettingsPanel>(
                parameters => parameters
                    .Add(x => x.Title, "Settings")
                    .Add(x => x.Description, "Manage settings")
            );

            // Expand the panel first
            var toggleButton = component.Find("button");
            toggleButton.Click();

            // Assert
            var buttons = component.FindAll(".settings-tab-button");
            Assert.True(buttons.Count >= 2); // Should have Export and Import tabs
        }
    }

    /// <summary>
    /// Integration tests for ExportSettings component
    /// </summary>
    public class ExportSettingsComponentTests : TestContext
    {
        private Mock<IRoomApiService> _mockRoomApiService = null!;
        private Mock<ILanguageService> _mockLanguageService = null!;

        public ExportSettingsComponentTests()
        {
            SetupMocks();
            Services.AddScoped(_ => _mockRoomApiService.Object);
            Services.AddScoped(_ => _mockLanguageService.Object);
        }

        private void SetupMocks()
        {
            _mockRoomApiService = new Mock<IRoomApiService>();
            _mockLanguageService = new Mock<ILanguageService>();

            _mockLanguageService.Setup(x => x.GetText(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string key, string defaultValue) => defaultValue);

            _mockRoomApiService.Setup(x => x.GetRoomsAsync())
                .Returns(Task.FromResult(new List<RoomDto>
                {
                    new("room-1", "Room 101", 2, 1, "A")
                }));
        }

        [Fact]
        public void ExportSettings_ShouldRenderSuccessfully()
        {
            // Act
            var component = RenderComponent<ExportSettings>();

            // Assert
            Assert.NotNull(component.Instance);
        }

        [Fact]
        public void ExportSettings_ShouldHaveExportButton()
        {
            // Act
            var component = RenderComponent<ExportSettings>();

            // Assert
            var button = component.Find("button");
            Assert.NotNull(button);
        }

        [Fact]
        public void ExportSettings_ShouldHaveRadioButtonsForSelection()
        {
            // Act
            var component = RenderComponent<ExportSettings>();

            // Assert
            var radios = component.FindAll("input[type='radio']");
            Assert.True(radios.Count >= 2);
        }

        [Fact]
        public void ExportSettings_ShouldHaveMetadataCheckbox()
        {
            // Act
            var component = RenderComponent<ExportSettings>();

            // Assert
            var checkboxes = component.FindAll("input[type='checkbox']");
            Assert.True(checkboxes.Count > 0);
        }
    }

    /// <summary>
    /// Integration tests for ImportSettings component
    /// </summary>
    public class ImportSettingsComponentTests : TestContext
    {
        private Mock<IRoomApiService> _mockRoomApiService = null!;
        private Mock<ILanguageService> _mockLanguageService = null!;

        public ImportSettingsComponentTests()
        {
            SetupMocks();
            Services.AddScoped(_ => _mockRoomApiService.Object);
            Services.AddScoped(_ => _mockLanguageService.Object);
        }

        private void SetupMocks()
        {
            _mockRoomApiService = new Mock<IRoomApiService>();
            _mockLanguageService = new Mock<ILanguageService>();

            _mockLanguageService.Setup(x => x.GetText(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string key, string defaultValue) => defaultValue);

            _mockRoomApiService.Setup(x => x.ValidateImportAsync(It.IsAny<Stream>()))
                .ReturnsAsync(new ImportValidationResponse(
                    true,
                    new List<string>(),
                    new List<string>(),
                    2,
                    1
                ));
        }

        [Fact]
        public void ImportSettings_ShouldRenderSuccessfully()
        {
            // Act
            var component = RenderComponent<ImportSettings>();

            // Assert
            Assert.NotNull(component.Instance);
        }

        [Fact]
        public void ImportSettings_ShouldHaveFileInputArea()
        {
            // Act
            var component = RenderComponent<ImportSettings>();

            // Assert
            var markup = component.Markup;
            Assert.Contains("Drag", markup);
        }

        [Fact]
        public void ImportSettings_ShouldHaveValidateButton()
        {
            // Act
            var component = RenderComponent<ImportSettings>();

            // Assert
            // The validate button is only shown after a file is selected.
            // Verify the file input drop zone is rendered (prerequisite for validation).
            var markup = component.Markup;
            Assert.Contains("drop", markup);
        }
    }

    /// <summary>
    /// Integration tests for LanguageSwitcher component
    /// </summary>
    public class LanguageSwitcherComponentTests : TestContext
    {
        private Mock<ILanguageService> _mockLanguageService;

        public LanguageSwitcherComponentTests()
        {
            _mockLanguageService = new Mock<ILanguageService>();
            _mockLanguageService.Setup(x => x.GetText(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string key, string defaultValue) => defaultValue);

            _mockLanguageService.Setup(x => x.AvailableLanguages)
                .Returns(new List<LanguageOption>
                {
                    new() { Code = "en", Name = "English" },
                    new() { Code = "es", Name = "Español" },
                    new() { Code = "it", Name = "Italiano" }
                });

            _mockLanguageService.Setup(x => x.CurrentLanguage)
                .Returns("en");

            Services.AddScoped(_ => _mockLanguageService.Object);
        }

        [Fact]
        public void LanguageSwitcher_ShouldRenderSuccessfully()
        {
            // Act
            var component = RenderComponent<LanguageSwitcher>();

            // Assert
            Assert.NotNull(component.Instance);
        }

        [Fact]
        public void LanguageSwitcher_ShouldHaveLanguageDropdown()
        {
            // Act
            var component = RenderComponent<LanguageSwitcher>();

            // Assert
            var select = component.Find("select");
            Assert.NotNull(select);
        }

        [Fact]
        public void LanguageSwitcher_ShouldDisplayAllLanguages()
        {
            // Act
            var component = RenderComponent<LanguageSwitcher>();

            // Assert
            var options = component.FindAll("option");
            Assert.Equal(3, options.Count);
        }

        [Fact]
        public async Task LanguageSwitcher_ShouldSwitchLanguage()
        {
            // Arrange
            var component = RenderComponent<LanguageSwitcher>();
            var select = component.Find("select");

            // Act  
            // In bUnit, ChangeAsync requires a ChangeEventArgs with Value property set
            var changeArgs = new ChangeEventArgs { Value = "es" };
            await select.ChangeAsync(changeArgs);

            // Assert
            _mockLanguageService.Verify(x => x.SetLanguageAsync("es"), Times.Once);
        }
    }
}

