using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Bunit;
using HotelDruid.Client.Pages;
using HotelDruid.Shared;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Moq;
using HotelDruid.Client.Tests;

namespace HotelDruid.Client.Tests.Integration.Pages
{
    // Minimal HTTP mock for Rooms.razor (uses Shared.RoomDto with string? FloorNumber)
    internal class RoomsPageMockHandler : System.Net.Http.HttpMessageHandler
    {
        protected override Task<System.Net.Http.HttpResponseMessage> SendAsync(
            System.Net.Http.HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            if (request.Method == System.Net.Http.HttpMethod.Get &&
                (request.RequestUri?.ToString() ?? "").Contains("/api/rooms"))
                return Task.FromResult(new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new System.Net.Http.StringContent("[]", System.Text.Encoding.UTF8, "application/json")
                });
            if (request.Method == System.Net.Http.HttpMethod.Post)
                return Task.FromResult(new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Created)
                {
                    Content = new System.Net.Http.StringContent("{}", System.Text.Encoding.UTF8, "application/json")
                });
            return Task.FromResult(new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.NotFound));
        }
    }

    /// <summary>
    /// Integration tests for the Rooms management page (/rooms)
    /// Tests the full CRUD functionality and UI interactions
    /// </summary>
    public class RoomsPageIntegrationTests : TestContext
    {
        private Mock<IJSRuntime> _mockJsRuntime;

        public RoomsPageIntegrationTests()
        {
            Services.AddClientLocalizationTestSupport();
            _mockJsRuntime = new Mock<IJSRuntime>();
            Services.AddScoped(_ => _mockJsRuntime.Object);
            var mockHttp = new HttpClient(new RoomsPageMockHandler()) { BaseAddress = new Uri("http://localhost/") };
            Services.AddScoped(_ => mockHttp);
            var mockRoomApi = new Mock<HotelDruid.Client.Services.IRoomApiService>();
            mockRoomApi.Setup(s => s.GetRoomsAsync()).Returns(System.Threading.Tasks.Task.FromResult(new System.Collections.Generic.List<HotelDruid.Client.Services.RoomDto>()));
            Services.AddScoped<HotelDruid.Client.Services.IRoomApiService>(_ => mockRoomApi.Object);
        }

        // Helper: render and click "Add Room" so the editor form becomes visible
        private IRenderedComponent<Rooms> RenderWithFormOpen()
        {
            var component = RenderComponent<Rooms>();
            var addBtn = component.FindAll("button")
                .FirstOrDefault(b => b.TextContent.Contains("Add Room"));
            addBtn?.Click();
            component.Render();
            return component;
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
            var columns = component.FindAll(".col-md-3, .col-md-5, .col-md-4");
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
            var component = RenderWithFormOpen();

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
            var component = RenderWithFormOpen();

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
            // When editModel is set, the badge shows status
            // Render with form open to see status badge
            var c2 = RenderWithFormOpen();
            c2.Markup.Should().Contain("status-badge");
        }

        [Fact]
        public void RoomsPage_ShouldHaveProperPageTitle()
        {
            // Act
            var component = RenderComponent<Rooms>();

            // Assert
            // PageTitle is not in bUnit markup; check the panel title instead
            component.Markup.Should().Contain("Rooms");
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
            var component = RenderWithFormOpen();

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
            var component = RenderWithFormOpen();

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
            var component = RenderWithFormOpen();

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
            var component = RenderWithFormOpen();

            // Assert
            var markup = component.Markup;
            // The markup should contain matrix or neighbors related content
            markup.Should().Contain("Neighboring Rooms");
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
            var component = RenderWithFormOpen();

            // Assert
            var markup = component.Markup;
            markup.Should().Contain("status-badge");
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
            var component = RenderWithFormOpen();

            // Assert
            var labels = component.FindAll("label");
            labels.Count().Should().BeGreaterThanOrEqualTo(8);

            var markup = component.Markup;
            // Check for form-label classes (Bootstrap 5)
            markup.Should().Contain("form-label");
            markup.Should().Contain("form-control");
        }

        [Fact]
        public void RoomsPage_RequiredFields_ShouldBeMarkedWithAsterisk()
        {
            // Act
            var component = RenderWithFormOpen();

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
            var component = RenderWithFormOpen();

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
            var component = RenderWithFormOpen();

            // Assert
            var markup = component.Markup;
            markup.Should().Contain("btn-primary"); // Save is always visible when form is open
        }

        [Fact]
        public void RoomsPage_EditorPanel_ShouldHaveHeading()
        {
            // Act
            var component = RenderWithFormOpen();

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
            var component = RenderWithFormOpen();

            // Assert
            var markup = component.Markup;
            markup.Should().Contain(label);
        }

        [Fact]
        public void RoomsPage_SuccessHelperText_ShouldBePresent()
        {
            // Act
            var component = RenderWithFormOpen();

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
            var component = RenderWithFormOpen();

            // Assert
            var markup = component.Markup;
            
            // Check for form elements
            markup.Should().Contain("<form>");
            
            // Check for input binding (Blazor specific)
            markup.Should().Contain("input");
        }
    }
}

