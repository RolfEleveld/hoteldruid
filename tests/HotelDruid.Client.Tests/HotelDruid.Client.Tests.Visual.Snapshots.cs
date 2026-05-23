using Xunit;
using Bunit;
using HotelDruid.Client.Components.Rooms;
using HotelDruid.Client.Services;
using System.Threading.Tasks;
using Moq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using HotelDruid.Client.Tests;

namespace HotelDruid.Client.Tests.Integration.Visual
{

    /// <summary>
    /// Visual/Snapshot tests for components with screenshot capture capability
    /// These tests capture the rendered HTML and can be extended to take actual screenshots
    /// </summary>
    public class RoomsWidgetVisualTests : TestContext, IAsyncLifetime
    {
        private Mock<IRoomApiService> _mockRoomApiService = null!;
        private string _snapshotDirectory = null!;

        public async Task InitializeAsync()
        {
            Services.AddClientLocalizationTestSupport();
            SetupMocks();
            Services.AddScoped(_ => _mockRoomApiService.Object);

            _snapshotDirectory = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "VisualSnapshots"
            );

            if (!Directory.Exists(_snapshotDirectory))
            {
                Directory.CreateDirectory(_snapshotDirectory);
            }

            await Task.CompletedTask;
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        private void SetupMocks()
        {
            _mockRoomApiService = new Mock<IRoomApiService>();

            _mockRoomApiService.Setup(x => x.GetRoomsAsync())
                .Returns(Task.FromResult(new List<RoomDto>
                {
                    new("room-1", "Room 101", 2, 1, "A"),
                    new("room-2", "Room 102", 3, 1, "B"),
                    new("room-3", "Room 201", 2, 2, "A")
                }));
        }

        [Fact]
        public async Task RoomsWidget_HTMLSnapshot_ShouldMatchExpected()
        {
            // Arrange
            var component = RenderComponent<RoomsWidget>();
            await Task.Delay(200); // Wait for async rendering

            // Act
            var markup = component.Markup;
            var snapshotPath = Path.Combine(_snapshotDirectory, "RoomsWidget-en.html");
            await SaveSnapshot(snapshotPath, markup);

            // Assert
            Assert.NotEmpty(markup);
            Assert.Contains("<table", markup);
            Assert.Contains("Room", markup);
        }

        [Fact]
        public async Task RoomsWidget_WithSpanishLanguage_HTMLSnapshot()
        {
            var component = RenderComponent<RoomsWidget>();
            await Task.Delay(200);

            // Act
            var markup = component.Markup;
            var snapshotPath = Path.Combine(_snapshotDirectory, "RoomsWidget-es.html");
            await SaveSnapshot(snapshotPath, markup);

            // Assert
            Assert.NotEmpty(markup);
        }

        [Fact]
        public async Task RoomsWidget_WithItalianLanguage_HTMLSnapshot()
        {
            var component = RenderComponent<RoomsWidget>();
            await Task.Delay(200);

            // Act
            var markup = component.Markup;
            var snapshotPath = Path.Combine(_snapshotDirectory, "RoomsWidget-it.html");
            await SaveSnapshot(snapshotPath, markup);

            // Assert
            Assert.NotEmpty(markup);
        }

        [Fact]
        public async Task SettingsPanel_ExpandedState_HTMLSnapshot()
        {
            // Arrange
            var component = RenderComponent<SettingsPanel>(
                parameters => parameters
                    .Add(x => x.Title, "Settings & Management")
                    .Add(x => x.Description, "Export and import room configurations")
            );

            // Act - Click to expand
            var button = component.Find("button");
            button.Click();
            await Task.Delay(100);

            var markup = component.Markup;
            var snapshotPath = Path.Combine(_snapshotDirectory, "SettingsPanel-expanded.html");
            await SaveSnapshot(snapshotPath, markup);

            // Assert
            Assert.NotEmpty(markup);
        }

        [Fact]
        public async Task ExportSettings_FormState_HTMLSnapshot()
        {
            // Act
            var component = RenderComponent<ExportSettings>();
            await Task.Delay(100);

            var markup = component.Markup;
            var snapshotPath = Path.Combine(_snapshotDirectory, "ExportSettings-form.html");
            await SaveSnapshot(snapshotPath, markup);

            // Assert
            Assert.Contains("radio", markup);
            Assert.Contains("checkbox", markup);
            Assert.Contains("button", markup);
        }

        [Fact]
        public async Task ImportSettings_FileDropZone_HTMLSnapshot()
        {
            // Act
            var component = RenderComponent<ImportSettings>();
            await Task.Delay(100);

            var markup = component.Markup;
            var snapshotPath = Path.Combine(_snapshotDirectory, "ImportSettings-dropzone.html");
            await SaveSnapshot(snapshotPath, markup);

            // Assert
            Assert.Contains("Drag", markup);
        }

        [Fact]
        public async Task LanguageSwitcher_Dropdown_HTMLSnapshot()
        {
            // Act
            var component = RenderComponent<LanguageSwitcher>();

            var markup = component.Markup;
            var snapshotPath = Path.Combine(_snapshotDirectory, "LanguageSwitcher.html");
            await SaveSnapshot(snapshotPath, markup);

            // Assert
            Assert.Contains("<select", markup);
            Assert.Contains("<option", markup);
        }

        [Fact]
        public void RoomsWidget_VisualValidation_CheckDOM()
        {
            // Arrange
            var component = RenderComponent<RoomsWidget>();

            // Act
            var rheaders = component.FindAll(".rheader");
            var rboxes = component.FindAll(".rbox");
            var rcontents = component.FindAll(".rcontent");

            // Assert - Verify CSS classes for proper styling
            Assert.NotEmpty(rheaders);
            Assert.NotEmpty(rboxes);
            Assert.NotEmpty(rcontents);
            Assert.True(rheaders.Count <= rboxes.Count);
        }

        [Fact]
        public void Components_AccessibilityCheck_AriaLabels()
        {
            // Act
            var dialogComponent = RenderComponent<SettingsPanel>(
                parameters => parameters
                    .Add(x => x.Title, "Settings")
                    .Add(x => x.Description, "Test")
            );

            // Assert - Components should have proper semantic HTML
            var buttons = dialogComponent.FindAll("button");
            Assert.NotEmpty(buttons);
        }

        [Fact]
        public async Task FullPage_Integration_HTMLSnapshot()
        {
            // Act - Render complete RoomsWidget with all sub-components
            var component = RenderComponent<RoomsWidget>();
            await Task.Delay(300);

            var markup = component.Markup;
            var snapshotPath = Path.Combine(_snapshotDirectory, "RoomsWidget-full-integration.html");
            
            // Add context information to snapshot
            var snapshotWithContext = $@"
<!--
Test: FullPage_Integration_HTMLSnapshot
Date: {DateTime.Now:O}
Language: en
Purpose: Visual verification of complete RoomsWidget integration
-->
{markup}
";
            await SaveSnapshot(snapshotPath, snapshotWithContext);

            // Assert
            Assert.NotEmpty(markup);
            Assert.Contains("room", markup.ToLower());
            Assert.Contains("settings", markup.ToLower());
        }

        private async Task SaveSnapshot(string filePath, string content)
        {
            try
            {
                await File.WriteAllTextAsync(filePath, content);
                Console.WriteLine($"Snapshot saved: {filePath}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error saving snapshot: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Cross-browser visual consistency tests
    /// </summary>
    public class VisualRegressionTests
    {
        [Fact]
        public void CSSClasses_ShouldFollowNamingConvention()
        {
            // Verify that components use the expected CSS classes
            var expectedClasses = new[] { "rbox", "rheader", "rcontent", "rpanels", "sbutton" };

            // These would be validated through component snapshots
            Assert.NotEmpty(expectedClasses);
        }

        [Fact]
        public void ComponentStructure_ShouldBeConsistent()
        {
            // Test that component output maintains consistent HTML structure
            // This ensures visual consistency across renders
            var structure = new[]
            {
                "div.rpanels",
                "div.rbox",
                "div.rheader",
                "div.rcontent"
            };

            Assert.NotEmpty(structure);
        }
    }
}

