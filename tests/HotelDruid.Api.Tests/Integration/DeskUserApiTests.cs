using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using HotelDruid.Api;
using HotelDruid.Shared;
using Microsoft.AspNetCore.Mvc.Testing;

namespace HotelDruid.Api.Tests.Integration;

[Collection("Sequential")]
public class DeskUserApiTests : IAsyncLifetime
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;
    private string _testDataRoot = null!;

    public Task InitializeAsync()
    {
        _testDataRoot = Path.Combine(Path.GetTempPath(), $"api-desk-{Guid.NewGuid()}");
        Directory.CreateDirectory(_testDataRoot);
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => builder.UseSetting("DataRoot", _testDataRoot));
        _client = _factory.CreateClient();
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        _client.Dispose();
        _factory.Dispose();
        if (Directory.Exists(_testDataRoot))
        {
            try { Directory.Delete(_testDataRoot, recursive: true); } catch { }
        }

        return Task.CompletedTask;
    }

    [Fact]
    public async Task DeskName_IsWrittenOnTheLedger_AndABadNameIsNot()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var stamp = today.ToString("yyyy-MM-dd");
        using var named = _factory.CreateClient();
        named.DefaultRequestHeaders.TryAddWithoutValidation(DeskUserNames.HeaderName, "Maria Rossi");

        var created = await named.PostAsJsonAsync("/api/money-history", new
        {
            year = today.Year,
            cashRegisterId = "CR1",
            amount = -2.0,
            type = "expense",
            description = "Named coffee",
            date = stamp
        }, JsonOptions);
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);

        var lines = await named.GetFromJsonAsync<List<LedgerLine>>(
            $"/api/ledger/lines?from={stamp}&to={stamp}&unassigned=true",
            JsonOptions) ?? [];
        var coffee = lines.Where(line => line.Description == "Named coffee").ToList();
        Assert.NotEmpty(coffee);
        Assert.All(coffee, line => Assert.Equal("Maria Rossi", line.CreatedBy));

        var unnamed = await _client.PostAsJsonAsync("/api/money-history", new
        {
            year = today.Year,
            cashRegisterId = "CR1",
            amount = -1.0,
            type = "expense",
            description = "Unnamed coffee",
            date = stamp
        }, JsonOptions);
        Assert.Equal(HttpStatusCode.Created, unnamed.StatusCode);

        using var rejected = _factory.CreateClient();
        rejected.DefaultRequestHeaders.TryAddWithoutValidation(DeskUserNames.HeaderName, "<script>");
        var bad = await rejected.PostAsJsonAsync("/api/money-history", new
        {
            year = today.Year,
            cashRegisterId = "CR1",
            amount = -1.0,
            type = "expense",
            description = "Rejected name",
            date = stamp
        }, JsonOptions);
        Assert.Equal(HttpStatusCode.Created, bad.StatusCode);

        lines = await _client.GetFromJsonAsync<List<LedgerLine>>(
            $"/api/ledger/lines?from={stamp}&to={stamp}&unassigned=true",
            JsonOptions) ?? [];
        Assert.All(lines.Where(line => line.Description is "Unnamed coffee" or "Rejected name"), line => Assert.True(string.IsNullOrWhiteSpace(line.CreatedBy)));
    }

    private sealed class LedgerLine
    {
        public string? Description { get; set; }
        public string? CreatedBy { get; set; }
    }
}
