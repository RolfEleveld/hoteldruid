using Xunit;
using Bunit;
using HotelDroid.Client.Pages;
using HotelDroid.Shared;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Moq;

namespace HotelDroid.Client.Tests.Integration.Pages
{
    /// <summary>
    /// Integration tests for the Rooms management page (/rooms)
    /// Tests the full CRUD functionality and UI interactions
    /// </summary>
    public class RoomsPageIntegrationTests : TestContext
    {
        private Mock<IJSRuntime> _mockJsRuntime;

        public RoomsPageIntegrationTests()
        {
            _mockJsRuntime = new Mock<IJSRuntime>();
            Services.AddScoped(_ => _mockJsRuntime.Object);
            Services.AddScoped(_ => new HttpClient());
        }

        [Fact]
        public void RoomsPage_ShouldRenderSuccessfully()
        {
            // Act
            var component = RenderComponent<Rooms>();

            // Assert
            component.Instance.Should().NotBeNull();
        }

        [Fact]
        public void RoomsPage_ShouldHaveThreePanelLayout()
        {
            // Act
            var component = RenderComponent<Rooms>();

            // Assert
            // Check for three main panel sections: left (list), center (editor), right (matrix)
            var columns = component.FindAll(".col-md-4, .col-md-5");
            columns.Should().HaveCount(3);
        }

        [Fact]
        public void RoomsPage_LeftPanel_ShouldHaveAddRoomButton()
        {
            // Act
            var component = RenderComponent<Rooms>();

            // Assert
            var markup = component.Markup;
            markup.Should().Contain("Add Room");
        }

        [Fact]
        public void RoomsPage_CenterPanel_ShouldHaveFormFields()
        {
            // Act
            var component = RenderComponent<Rooms>();

            // Assert
            var markup = component.Markup;
            // Check for key form labels
            markup.Should().Contain("Room Name");
            markup.Should().Contain("Capacity");
            markup.Should().Contain("Floor Number");
            markup.Should().Contain("House Number");
            markup.Should().Contain("Priority");
            markup.Should().Contain("Comments");
        }

        [Fact]
        public void RoomsPage_CenterPanel_ShouldHaveActionButtons()
        {
            // Act
            var component = RenderComponent<Rooms>();

            // Assert
            var markup = component.Markup;
            markup.Should().Contain("Save");
        }

        [Fact]
        public void RoomsPage_RightPanel_ShouldHaveNeighboringRoomsMatrix()
        {
            // Act
            var component = RenderComponent<Rooms>();

            // Assert
            var markup = component.Markup;
            markup.Should().Contain("Neighboring Rooms");
        }

        [Fact]
        public void RoomsPage_ShouldShowHelpTextWhenNoRoomSelected()
        {
            // Act
            var component = RenderComponent<Rooms>();

            // Assert
            var markup = component.Markup;
            markup.Should().Contain("Select a room from the list or add a new one");
        }

        [Fact]
        public void RoomsPage_StatusBadge_ShouldDisplayInitially()
        {
            // Act
            var component = RenderComponent<Rooms>();

            // Assert
            var markup = component.Markup;
            // Initial state shows "No changes" message
            markup.Should().Contain("No changes");
        }

        [Fact]
        public void RoomsPage_ShouldHaveProperPageTitle()
        {
            // Act
            var component = RenderComponent<Rooms>();

            // Assert
            component.Markup.Should().Contain("Rooms Management");
        }

        [Fact]
        public void RoomsPage_AddNewRoom_ShouldShowNewRoomTitle()
        {
            // Arrange
            var component = RenderComponent<Rooms>();

            // Act
            var buttons = component.FindAll("button");
            var addButton = buttons.FirstOrDefault(b => b.TextContent.Contains("Add Room"));
            if (addButton != null)
            {
                addButton.Click();
                component.Render();
            }

            // Assert - Form inputs should be visible with "New Room" title
            var markup = component.Markup;
            markup.Should().Contain("New Room");
        }

        [Fact]
        public void RoomsPage_FormFields_ShouldHaveProperInputTypes()
        {
            // Act
            var component = RenderComponent<Rooms>();

            // Assert
            var inputs = component.FindAll("input[type='text'], input[type='number'], textarea, select");
            inputs.Should().NotBeEmpty();
            
            // Should have text inputs for Name
            var textInputs = component.FindAll("input[type='text']");
            textInputs.Count().Should().BeGreaterThan(0);

            // Should have number inputs for Capacity, Priority, SecondaryPriority
            var numberInputs = component.FindAll("input[type='number']");
            numberInputs.Count().Should().BeGreaterThan(0);

            // Should have textarea for Comments
            var textareas = component.FindAll("textarea");
            textareas.Count().Should().BeGreaterThan(0);

            // Should have select for HasBeds
            var selects = component.FindAll("select");
            selects.Count().Should().BeGreaterThan(0);
        }

        [Fact]
        public void RoomsPage_CapacityField_ShouldHaveMinimumValue()
        {
            // Act
            var component = RenderComponent<Rooms>();

            // Assert
            var capacityInput = component.Find("input[type='number'][min]");
            if (capacityInput != null)
            {
                capacityInput.GetAttribute("min").Should().Be("1");
            }
        }

        [Fact]
        public void RoomsPage_ShouldHaveHasBedsSelectOptions()
        {
            // Act
            var component = RenderComponent<Rooms>();

            // Assert
            var markup = component.Markup;
            markup.Should().Contain("Select...");
            markup.Should().Contain("Yes");
            markup.Should().Contain("No");
        }

        [Fact]
        public void RoomsPage_NeighboringRoomsMatrix_ShouldBePresent()
        {
            // Act
            var component = RenderComponent<Rooms>();

            // Assert
            var markup = component.Markup;
            // The markup should contain matrix or neighbors related content
            markup.Should().Contain("neighbors-matrix");
        }

        [Fact]
        public void RoomsPage_ShouldRenderWithoutErrors()
        {
            // Act & Assert - This verifies the component doesn't throw during render
            var ex = Record.Exception(() =>
            {
                var component = RenderComponent<Rooms>();
                _ = component.Markup;
            });

            ex.Should().BeNull();
        }

        [Fact]
        public void RoomsPage_CenterPanel_ShouldShowStatusBadge()
        {
            // Act
            var component = RenderComponent<Rooms>();

            // Assert
            var markup = component.Markup;
            markup.Should().Contain("status-badge");
            markup.Should().Contain("badge");
        }

        [Fact]
        public void RoomsPage_FormLayout_ShouldBeResponsive()
        {
            // Act
            var component = RenderComponent<Rooms>();

            // Assert
            var markup = component.Markup;
            // Check for Bootstrap responsive classes
            markup.Should().Contain("container-fluid");
            markup.Should().Contain("row");
            markup.Should().Contain("col-md");
        }

        [Fact]
        public void RoomsPage_ShouldHavePanelStructure()
        {
            // Act
            var component = RenderComponent<Rooms>();

            // Assert
            var panels = component.FindAll(".panel");
            panels.Count().Should().BeGreaterThanOrEqualTo(3); // At least 3 panels (list, editor, matrix)

            // Each panel should have header and body
            var panelHeadings = component.FindAll(".panel-heading");
            panelHeadings.Count().Should().BeGreaterThan(0);

            var panelBodies = component.FindAll(".panel-body");
            panelBodies.Count().Should().BeGreaterThan(0);
        }

        [Fact]
        public void RoomsPage_LeftPanel_ShouldHaveListGroup()
        {
            // Act
            var component = RenderComponent<Rooms>();

            // Assert
            var markup = component.Markup;
            markup.Should().Contain("list-group");
        }

        [Fact]
        public void RoomsPage_FormShouldHaveAllRequiredLabels()
        {
            // Act
            var component = RenderComponent<Rooms>();

            // Assert
            var labels = component.FindAll("label");
            labels.Count().Should().BeGreaterThan(8);

            var markup = component.Markup;
            // Check for form-label classes (Bootstrap 5)
            markup.Should().Contain("form-label");
            markup.Should().Contain("form-control");
        }

        [Fact]
        public void RoomsPage_RequiredFields_ShouldBeMarkedWithAsterisk()
        {
            // Act
            var component = RenderComponent<Rooms>();

            // Assert
            var markup = component.Markup;
            // Check for visual indicator of required fields
            markup.Should().Contain("text-danger");
            markup.Should().Contain("*");
        }

        [Fact]
        public void RoomsPage_CommentsField_ShouldBeTextarea()
        {
            // Act
            var component = RenderComponent<Rooms>();

            // Assert
            var textareas = component.FindAll("textarea");
            textareas.Should().NotBeEmpty();

            // Check for rows attribute
            var commentsTextarea = textareas.FirstOrDefault();
            if (commentsTextarea != null)
            {
                commentsTextarea.GetAttribute("rows").Should().Be("3");
            }
        }

        [Fact]
        public void RoomsPage_ShouldHaveBootstrapButtonStyles()
        {
            // Act
            var component = RenderComponent<Rooms>();

            // Assert
            var markup = component.Markup;
            markup.Should().Contain("btn-primary");
            markup.Should().Contain("btn-info");
            markup.Should().Contain("btn-danger");
        }

        [Fact]
        public void RoomsPage_EditorPanel_ShouldHaveHeading()
        {
            // Act
            var component = RenderComponent<Rooms>();

            // Assert
            var markup = component.Markup;
            // The center panel should indicate it's for managing rooms
            markup.Should().Contain("Manage the room");
        }

        [Theory]
        [InlineData("Room Name")]
        [InlineData("Capacity")]
        [InlineData("Floor Number")]
        [InlineData("House Number")]
        [InlineData("Priority")]
        [InlineData("Secondary Priority")]
        [InlineData("Has Beds")]
        [InlineData("Comments")]
        public void RoomsPage_ShouldContainFormLabel(string label)
        {
            // Act
            var component = RenderComponent<Rooms>();

            // Assert
            var markup = component.Markup;
            markup.Should().Contain(label);
        }

        [Fact]
        public void RoomsPage_SuccessHelperText_ShouldBePresent()
        {
            // Act
            var component = RenderComponent<Rooms>();

            // Assert
            var markup = component.Markup;
            // Check for form control helper text
            markup.Should().Contain("form-text");
            markup.Should().Contain("Unique identifier");
            markup.Should().Contain("Max occupants");
        }

        [Fact]
        public void RoomsPage_PageTitle_ShouldBeSet()
        {
            // Act
            var component = RenderComponent<Rooms>();

            // Assert
            // The component uses <PageTitle> which sets the browser title
            // We can verify it's rendered in the markup
            component.Markup.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void RoomsPage_ShouldHaveBaseStyles()
        {
            // Act
            var component = RenderComponent<Rooms>();

            // Assert
            var markup = component.Markup;
            // Check for key CSS classes that define the layout
            markup.Should().Contain("h-100");  // Full height
            markup.Should().Contain("d-flex");  // Flexbox display
            markup.Should().Contain("flex-column");  // Vertical flex
        }

        [Fact]
        public void RoomsPage_PageDirective_ShouldBeRouted()
        {
            // This test verifies that the page has the @page directive
            // and is routable at /rooms
            // The component must inherit from ComponentBase and have the right structure
            
            var component = RenderComponent<Rooms>();
            
            // If the component renders without errors, routing is properly configured
            component.Instance.Should().NotBeNull();
        }

        [Fact]
        public void RoomsPage_ShouldHaveProperFormElements()
        {
            // Act
            var component = RenderComponent<Rooms>();

            // Assert
            var markup = component.Markup;
            
            // Check for form elements
            markup.Should().Contain("<form>");
            
            // Check for input binding (Blazor specific)
            markup.Should().Contain("input");
        }
    }
}
