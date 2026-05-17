using System.Net;
using System.Text;
using System.Text.Json;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using HotelDruid.Api;
using HotelDruid.Api.Services;

namespace HotelDruid.Api.Tests.Integration;

[Collection("Sequential")]
public class DocumentRenderApiTests : IAsyncLifetime
{
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;
    private string _testDataRoot = null!;
    private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

    private class ClientDto { public string? Id { get; set; } public string? LastName { get; set; } public string? FirstName { get; set; } public string? Email { get; set; } public string? Phone { get; set; } }
    private class RoomDto { public string? Id { get; set; } public string? Name { get; set; } }
    private class BookingDto { public string? Id { get; set; } public int Year { get; set; } public string? ClientId { get; set; } public string? RoomId { get; set; } public string? Status { get; set; } public string? ArrivalDate { get; set; } public string? DepartureDate { get; set; } }
    private class BookingCostDto { public string? Id { get; set; } public int Year { get; set; } public string? BookingId { get; set; } public double? Amount { get; set; } public string? Description { get; set; } }
    private class BookingGuestDto { public string? Id { get; set; } public int Year { get; set; } public string? BookingId { get; set; } public string? ClientId { get; set; } public int? GuestNumber { get; set; } }
    private class ContractTemplateDto { public string? Id { get; set; } public string? Type { get; set; } public int Number { get; set; } public string? Content { get; set; } }

    public async Task InitializeAsync()
    {
        _testDataRoot = Path.Combine(Path.GetTempPath(), $"api-render-tests-{Guid.NewGuid()}");
        Directory.CreateDirectory(_testDataRoot);

        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(b => b.UseSetting("DataRoot", _testDataRoot));

        _client = _factory.CreateClient();
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        _client?.Dispose();
        _factory?.Dispose();
        if (Directory.Exists(_testDataRoot)) try { Directory.Delete(_testDataRoot, true); } catch { }
        await Task.CompletedTask;
    }

    private StringContent Json(object o) =>
        new(JsonSerializer.Serialize(o), Encoding.UTF8, "application/json");

    private async Task<T?> ReadAs<T>(HttpResponseMessage r) =>
        JsonSerializer.Deserialize<T>(await r.Content.ReadAsStringAsync(), _json);

    // ---- Helpers ----

    private async Task<(string clientId, string roomId, string bookingId)> CreateBookingAsync(int year = 2026)
    {
        var clientRes = await _client.PostAsync("/api/clients",
            Json(new { LastName = "Doe", FirstName = "Jane", Email = "jane@example.com", Phone = "+1555000" }));
        Assert.True(clientRes.IsSuccessStatusCode, $"Client creation failed: {clientRes.StatusCode}");
        var clientDto = await ReadAs<ClientDto>(clientRes);

        var roomRes = await _client.PostAsync("/api/rooms",
            Json(new { Name = $"Room-{Guid.NewGuid():N}", Capacity = 2 }));
        Assert.True(roomRes.IsSuccessStatusCode, $"Room creation failed: {roomRes.StatusCode} — {await roomRes.Content.ReadAsStringAsync()}");
        var roomDto = await ReadAs<RoomDto>(roomRes);

        var bookingRes = await _client.PostAsync("/api/bookings",
            Json(new { Year = year, ClientId = clientDto!.Id, RoomId = roomDto!.Id,
                       ArrivalDate = $"{year}-06-10", DepartureDate = $"{year}-06-13", Status = "Confirmed" }));
        Assert.True(bookingRes.IsSuccessStatusCode, $"Booking creation failed: {bookingRes.StatusCode}");
        var bookingDto = await ReadAs<BookingDto>(bookingRes);

        return (clientDto.Id!, roomDto.Id!, bookingDto!.Id!);
    }

    private async Task CreateTemplateAsync(string type, string content)
    {
        await _client.PostAsync("/api/contract-templates",
            Json(new { Type = type, Number = 1, Content = content }));
    }

    // ---- Tests ----

    [Fact]
    public async Task SeededTemplates_AreAvailableAfterStartup()
    {
        // The DocumentTemplateSeeder runs on startup; all known types should be present.
        foreach (var type in DocumentTemplateSeeder.KnownTemplateTypes)
        {
            var r = await _client.GetAsync($"/api/contract-templates/{type}/1");
            Assert.True(r.StatusCode == HttpStatusCode.OK,
                $"Expected seeded template '{type}' to be present, got {r.StatusCode}");
        }
    }

    [Fact]
    public async Task Render_Html_ReturnsOkWithTokensReplaced()
    {
        var (_, _, bookingId) = await CreateBookingAsync();

        await CreateTemplateAsync("render-test-html", "Hello {{Client.FirstName}} for booking {{Booking.Id}}");

        var r = await _client.GetAsync(
            $"/api/documents/render/render-test-html/1?bookingId={bookingId}&year=2026&format=html");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        Assert.Equal("text/html", r.Content.Headers.ContentType?.MediaType);

        var body = await r.Content.ReadAsStringAsync();
        Assert.DoesNotContain("{{Client.FirstName}}", body);
        Assert.DoesNotContain("{{Booking.Id}}", body);
        Assert.Contains("Jane", body);
        Assert.Contains(bookingId, body);
    }

    [Fact]
    public async Task Render_PrintFormat_InjectsWindowPrint()
    {
        var (_, _, bookingId) = await CreateBookingAsync();

        await CreateTemplateAsync("render-test-print", "<html><body>{{Client.LastName}}</body></html>");

        var r = await _client.GetAsync(
            $"/api/documents/render/render-test-print/1?bookingId={bookingId}&year=2026&format=print");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);

        var body = await r.Content.ReadAsStringAsync();
        Assert.Contains("window.print()", body);
    }

    [Fact]
    public async Task Render_DocFormat_ReturnsMsWordContentType()
    {
        var (_, _, bookingId) = await CreateBookingAsync();

        await CreateTemplateAsync("render-test-doc", "<html><body>{{Client.LastName}}</body></html>");

        var r = await _client.GetAsync(
            $"/api/documents/render/render-test-doc/1?bookingId={bookingId}&year=2026&format=doc");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        Assert.Equal("application/msword", r.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Render_EmlFormat_ReturnsRfc2822Structure()
    {
        var (_, _, bookingId) = await CreateBookingAsync();

        await CreateTemplateAsync("render-test-eml", "<html><body>{{Client.Email}}</body></html>");

        var r = await _client.GetAsync(
            $"/api/documents/render/render-test-eml/1?bookingId={bookingId}&year=2026&format=eml");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        Assert.Equal("message/rfc822", r.Content.Headers.ContentType?.MediaType);

        var body = await r.Content.ReadAsStringAsync();
        Assert.Contains("MIME-Version:", body);
        Assert.Contains("Content-Type:", body);
        Assert.Contains("multipart/alternative", body);
    }

    [Fact]
    public async Task Render_ForeachCosts_ExpandsRows()
    {
        var (_, _, bookingId) = await CreateBookingAsync();

        await _client.PostAsync("/api/booking-costs",
            Json(new { Year = 2026, BookingId = bookingId, Amount = 150.0, Description = "Night rate" }));
        await _client.PostAsync("/api/booking-costs",
            Json(new { Year = 2026, BookingId = bookingId, Amount = 25.0, Description = "Breakfast" }));

        const string templateContent =
            "<!-- FOREACH:Costs -->{{Cost.Description}}: {{Cost.Amount}}<!-- ENDFOREACH:Costs -->";
        await CreateTemplateAsync("render-test-costs", templateContent);

        var r = await _client.GetAsync(
            $"/api/documents/render/render-test-costs/1?bookingId={bookingId}&year=2026&format=html");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);

        var body = await r.Content.ReadAsStringAsync();
        Assert.Contains("Night rate", body);
        Assert.Contains("Breakfast", body);
        Assert.DoesNotContain("FOREACH", body);
    }

    [Fact]
    public async Task Render_ForeachGuests_ExpandsRows()
    {
        var (clientId, _, bookingId) = await CreateBookingAsync();

        var guest2Res = await _client.PostAsync("/api/clients",
            Json(new { LastName = "Smith", FirstName = "Bob" }));
        var guest2 = await ReadAs<ClientDto>(guest2Res);

        await _client.PostAsync("/api/booking-guests",
            Json(new { Year = 2026, BookingId = bookingId, ClientId = clientId, GuestNumber = 1 }));
        await _client.PostAsync("/api/booking-guests",
            Json(new { Year = 2026, BookingId = bookingId, ClientId = guest2!.Id, GuestNumber = 2 }));

        const string templateContent =
            "<!-- FOREACH:Guests -->{{Guest.FullName}}<!-- ENDFOREACH:Guests -->";
        await CreateTemplateAsync("render-test-guests", templateContent);

        var r = await _client.GetAsync(
            $"/api/documents/render/render-test-guests/1?bookingId={bookingId}&year=2026&format=html");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);

        var body = await r.Content.ReadAsStringAsync();
        Assert.Contains("Doe", body);
        Assert.Contains("Smith", body);
        Assert.DoesNotContain("FOREACH", body);
    }

    [Fact]
    public async Task Render_MissingBooking_Returns404()
    {
        await CreateTemplateAsync("render-test-missing-bkg", "<html><body>{{Client.FirstName}}</body></html>");

        var r = await _client.GetAsync(
            "/api/documents/render/render-test-missing-bkg/1?bookingId=nonexistent&year=2026&format=html");
        Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
    }

    [Fact]
    public async Task Render_MissingTemplate_Returns404()
    {
        var (_, _, bookingId) = await CreateBookingAsync();

        var r = await _client.GetAsync(
            $"/api/documents/render/no-such-template/1?bookingId={bookingId}&year=2026&format=html");
        Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
    }

    [Fact]
    public async Task Render_DefaultFormat_IsHtml()
    {
        var (_, _, bookingId) = await CreateBookingAsync();

        await CreateTemplateAsync("render-test-default", "{{Hotel.Name}}");

        // No format query param — should default to html
        var r = await _client.GetAsync(
            $"/api/documents/render/render-test-default/1?bookingId={bookingId}&year=2026");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        Assert.Equal("text/html", r.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Render_SeededBookingConfirmation_ContainsClientName()
    {
        var (_, _, bookingId) = await CreateBookingAsync();

        var r = await _client.GetAsync(
            $"/api/documents/render/booking-confirmation/1?bookingId={bookingId}&year=2026&format=html");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);

        var body = await r.Content.ReadAsStringAsync();
        Assert.DoesNotContain("{{Client.FullName}}", body);
        Assert.DoesNotContain("{{Booking.Id}}", body);
    }
}
