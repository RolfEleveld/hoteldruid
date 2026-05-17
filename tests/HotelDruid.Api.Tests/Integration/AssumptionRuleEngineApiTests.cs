using System.Net;
using System.Net.Http.Json;
using HotelDruid.Api;
using HotelDruid.Shared.Configuration;
using Microsoft.AspNetCore.Mvc.Testing;

namespace HotelDruid.Api.Tests.Integration;

[Collection("Sequential")]
public class AssumptionRuleEngineApiTests : IAsyncLifetime
{
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;
    private string _testDataRoot = null!;

    public Task InitializeAsync()
    {
        _testDataRoot = Path.Combine(Path.GetTempPath(), $"api-assumption-tests-{Guid.NewGuid()}");
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
    public async Task MissingTariffsYear_ReturnsEmptyThenSelfHealsFromFallbackYear()
    {
        var configResponse = await _client.PutAsJsonAsync("/api/system/configuration", new SystemConfiguration
        {
            Id = "system",
            DefaultYear = 2026,
            Settings = new Dictionary<string, string>
            {
                ["TariffFallbackSourceYear"] = "2026"
            }
        });
        Assert.Equal(HttpStatusCode.OK, configResponse.StatusCode);

        var createTariffResponse = await _client.PostAsJsonAsync("/api/tariffs", new
        {
            Id = (string?)null,
            Year = 2026,
            ExtraCostName = "Breakfast",
            CostType = "fixed",
            BaseValue = 10.0,
            PercentageValue = (double?)null,
            TaxPercentage = 10.0,
            Category = "meal",
            NumberLimit = (int?)null
        });
        Assert.Equal(HttpStatusCode.Created, createTariffResponse.StatusCode);

        var firstRead = await _client.GetFromJsonAsync<List<TariffDto>>("/api/tariffs?year=2027");
        Assert.NotNull(firstRead);
        Assert.Empty(firstRead!);

        List<TariffDto> healed = new();
        for (var i = 0; i < 20; i++)
        {
            healed = await _client.GetFromJsonAsync<List<TariffDto>>("/api/tariffs?year=2027") ?? new();
            if (healed.Count > 0)
            {
                break;
            }

            await Task.Delay(50);
        }

        Assert.NotEmpty(healed);
        Assert.All(healed, t => Assert.Equal(2027, t.Year));
    }

    [Fact]
    public async Task MissingPeriodsYear_ReturnsEmptyThenSelfHealsFromFallbackYear()
    {
        var configResponse = await _client.PutAsJsonAsync("/api/system/configuration", new SystemConfiguration
        {
            Id = "system",
            DefaultYear = 2026,
            Settings = new Dictionary<string, string>
            {
                ["AvailabilityFallbackSourceYear"] = "2026"
            }
        });
        Assert.Equal(HttpStatusCode.OK, configResponse.StatusCode);

        var createPeriodResponse = await _client.PostAsJsonAsync("/api/periods", new
        {
            Id = (string?)null,
            Year = 2026,
            StartDate = DateOnly.Parse("2026-01-01"),
            EndDate = DateOnly.Parse("2026-01-31"),
            Tariff1 = 100.0,
            Tariff1PerPerson = (double?)null,
            Tariff2 = (double?)null,
            Tariff2PerPerson = (double?)null,
            Tariff3 = (double?)null,
            Tariff3PerPerson = (double?)null,
            Tariff4 = (double?)null,
            Tariff4PerPerson = (double?)null,
            Tariff5 = (double?)null,
            Tariff5PerPerson = (double?)null,
            Tariff6 = (double?)null,
            Tariff6PerPerson = (double?)null,
            Tariff7 = (double?)null,
            Tariff7PerPerson = (double?)null,
            Tariff8 = (double?)null,
            Tariff8PerPerson = (double?)null,
            Tariff9 = (double?)null,
            Tariff9PerPerson = (double?)null,
            Tariff10 = (double?)null,
            Tariff10PerPerson = (double?)null,
            Tariff11 = (double?)null,
            Tariff11PerPerson = (double?)null,
            Tariff12 = (double?)null,
            Tariff12PerPerson = (double?)null
        });
        Assert.Equal(HttpStatusCode.Created, createPeriodResponse.StatusCode);

        var firstRead = await _client.GetFromJsonAsync<List<PeriodDto>>("/api/periods?year=2027");
        Assert.NotNull(firstRead);
        Assert.Empty(firstRead!);

        List<PeriodDto> healed = new();
        for (var i = 0; i < 20; i++)
        {
            healed = await _client.GetFromJsonAsync<List<PeriodDto>>("/api/periods?year=2027") ?? new();
            if (healed.Count > 0)
            {
                break;
            }

            await Task.Delay(50);
        }

        Assert.NotEmpty(healed);
        Assert.All(healed, p => Assert.Equal(2027, p.Year));
    }

    [Fact]
    public async Task MissingRoomStillReturns404()
    {
        var response = await _client.GetAsync("/api/rooms/non-existing-room-id");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private sealed class TariffDto
    {
        public string? Id { get; set; }
        public int Year { get; set; }
        public string? ExtraCostName { get; set; }
        public string? CostType { get; set; }
        public double? BaseValue { get; set; }
    }

    private sealed class PeriodDto
    {
        public string? Id { get; set; }
        public int Year { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public double? Tariff1 { get; set; }
    }
}
