using System.Net;
using System.Text;
using System.Text.Json;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using HotelDroid.Api;

namespace HotelDroid.Api.Tests.Integration;

[Collection("Sequential")]
public class Layer3ApiTests : IAsyncLifetime
{
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;
    private string _testDataRoot = null!;
    private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

    private class YearDto { public int Year { get; set; } public string? PeriodType { get; set; } }
    private class PeriodDto { public string? Id { get; set; } public int Year { get; set; } public string? StartDate { get; set; } public string? EndDate { get; set; } public double? Tariff1 { get; set; } }
    private class TariffDto { public string? Id { get; set; } public int Year { get; set; } public string? ExtraCostName { get; set; } public string? CostType { get; set; } public double? BaseValue { get; set; } }
    private class AssignmentRuleDto { public string? Id { get; set; } public int Year { get; set; } public string? RoomOrAgency { get; set; } public string? ClosedTariff { get; set; } public int? StartPeriodId { get; set; } }

    public async Task InitializeAsync()
    {
        _testDataRoot = Path.Combine(Path.GetTempPath(), $"api-layer3-tests-{Guid.NewGuid()}");
        Directory.CreateDirectory(_testDataRoot);
        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(b => b.UseSetting("DataRoot", _testDataRoot));
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

    private StringContent Json(object o) => new(JsonSerializer.Serialize(o), Encoding.UTF8, "application/json");
    private async Task<T?> ReadAs<T>(HttpResponseMessage r) => JsonSerializer.Deserialize<T>(await r.Content.ReadAsStringAsync(), _json);

    // ========== Years ==========

    [Fact]
    public async Task Year_Create_ReturnsCreated()
    {
        var r = await _client.PostAsync("/api/years", Json(new { Year = 2024, PeriodType = "fixed" }));
        Assert.Equal(HttpStatusCode.Created, r.StatusCode);
        var dto = await ReadAs<YearDto>(r);
        Assert.Equal(2024, dto?.Year);
        Assert.Equal("fixed", dto?.PeriodType);
    }

    [Fact]
    public async Task Year_GetByYear_ReturnsOk()
    {
        await _client.PostAsync("/api/years", Json(new { Year = 2025, PeriodType = "variable" }));
        var r = await _client.GetAsync("/api/years/2025");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var dto = await ReadAs<YearDto>(r);
        Assert.Equal(2025, dto?.Year);
        Assert.Equal("variable", dto?.PeriodType);
    }

    [Fact]
    public async Task Year_List_ContainsCreated()
    {
        await _client.PostAsync("/api/years", Json(new { Year = 2026, PeriodType = "fixed" }));
        var r = await _client.GetAsync("/api/years");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var list = await ReadAs<List<YearDto>>(r);
        Assert.Contains(list!, x => x.Year == 2026);
    }

    [Fact]
    public async Task Year_Update_ChangesFields()
    {
        await _client.PostAsync("/api/years", Json(new { Year = 2027, PeriodType = "fixed" }));
        var r = await _client.PutAsync("/api/years/2027", Json(new { Year = 2027, PeriodType = "variable" }));
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var dto = await ReadAs<YearDto>(r);
        Assert.Equal("variable", dto?.PeriodType);
    }

    [Fact]
    public async Task Year_Delete_ReturnsNoContent()
    {
        await _client.PostAsync("/api/years", Json(new { Year = 2028, PeriodType = "fixed" }));
        var r = await _client.DeleteAsync("/api/years/2028");
        Assert.Equal(HttpStatusCode.NoContent, r.StatusCode);
        var get = await _client.GetAsync("/api/years/2028");
        Assert.Equal(HttpStatusCode.NotFound, get.StatusCode);
    }

    // ========== Periods ==========

    [Fact]
    public async Task Period_Create_ReturnsCreated()
    {
        var r = await _client.PostAsync("/api/periods", Json(new { Year = 2024, StartDate = "2024-01-01", EndDate = "2024-03-31", Tariff1 = 95.5 }));
        Assert.Equal(HttpStatusCode.Created, r.StatusCode);
        var dto = await ReadAs<PeriodDto>(r);
        Assert.NotNull(dto?.Id);
        Assert.Equal(2024, dto.Year);
        Assert.Equal(95.5, dto.Tariff1);
    }

    [Fact]
    public async Task Period_GetByYearAndId_ReturnsOk()
    {
        var create = await _client.PostAsync("/api/periods", Json(new { Year = 2024, StartDate = "2024-04-01", EndDate = "2024-06-30" }));
        var created = await ReadAs<PeriodDto>(create);
        var r = await _client.GetAsync($"/api/periods/2024/{created!.Id}");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var dto = await ReadAs<PeriodDto>(r);
        Assert.Equal(2024, dto?.Year);
    }

    [Fact]
    public async Task Period_List_FilteredByYear()
    {
        await _client.PostAsync("/api/periods", Json(new { Year = 2030, StartDate = "2030-01-01" }));
        await _client.PostAsync("/api/periods", Json(new { Year = 2031, StartDate = "2031-01-01" }));
        var r = await _client.GetAsync("/api/periods?year=2030");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var list = await ReadAs<List<PeriodDto>>(r);
        Assert.All(list!, p => Assert.Equal(2030, p.Year));
    }

    [Fact]
    public async Task Period_Update_ChangesFields()
    {
        var create = await _client.PostAsync("/api/periods", Json(new { Year = 2024, Tariff1 = 100.0 }));
        var created = await ReadAs<PeriodDto>(create);
        var r = await _client.PutAsync($"/api/periods/2024/{created!.Id}", Json(new { Year = 2024, Tariff1 = 150.0 }));
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var dto = await ReadAs<PeriodDto>(r);
        Assert.Equal(150.0, dto?.Tariff1);
    }

    [Fact]
    public async Task Period_Delete_ReturnsNoContent()
    {
        var create = await _client.PostAsync("/api/periods", Json(new { Year = 2024, StartDate = "2024-07-01" }));
        var created = await ReadAs<PeriodDto>(create);
        var r = await _client.DeleteAsync($"/api/periods/2024/{created!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, r.StatusCode);
    }

    // ========== Tariffs ==========

    [Fact]
    public async Task Tariff_Create_ReturnsCreated()
    {
        var r = await _client.PostAsync("/api/tariffs", Json(new { Year = 2024, ExtraCostName = "Breakfast", CostType = "fixed", BaseValue = 15.0 }));
        Assert.Equal(HttpStatusCode.Created, r.StatusCode);
        var dto = await ReadAs<TariffDto>(r);
        Assert.NotNull(dto?.Id);
        Assert.Equal("Breakfast", dto.ExtraCostName);
        Assert.Equal(15.0, dto.BaseValue);
    }

    [Fact]
    public async Task Tariff_GetByYearAndId_ReturnsOk()
    {
        var create = await _client.PostAsync("/api/tariffs", Json(new { Year = 2024, ExtraCostName = "Parking", CostType = "per_night" }));
        var created = await ReadAs<TariffDto>(create);
        var r = await _client.GetAsync($"/api/tariffs/2024/{created!.Id}");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var dto = await ReadAs<TariffDto>(r);
        Assert.Equal("Parking", dto?.ExtraCostName);
    }

    [Fact]
    public async Task Tariff_List_FilteredByYear()
    {
        await _client.PostAsync("/api/tariffs", Json(new { Year = 2040, ExtraCostName = "Spa" }));
        await _client.PostAsync("/api/tariffs", Json(new { Year = 2041, ExtraCostName = "Gym" }));
        var r = await _client.GetAsync("/api/tariffs?year=2040");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var list = await ReadAs<List<TariffDto>>(r);
        Assert.All(list!, t => Assert.Equal(2040, t.Year));
    }

    [Fact]
    public async Task Tariff_Delete_ReturnsNoContent()
    {
        var create = await _client.PostAsync("/api/tariffs", Json(new { Year = 2024, ExtraCostName = "ToDelete" }));
        var created = await ReadAs<TariffDto>(create);
        var r = await _client.DeleteAsync($"/api/tariffs/2024/{created!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, r.StatusCode);
    }

    // ========== Assignment Rules ==========

    [Fact]
    public async Task AssignmentRule_Create_ReturnsCreated()
    {
        var r = await _client.PostAsync("/api/assignment-rules", Json(new { Year = 2024, RoomOrAgency = "Room101", ClosedTariff = "T1", StartPeriodId = 1 }));
        Assert.Equal(HttpStatusCode.Created, r.StatusCode);
        var dto = await ReadAs<AssignmentRuleDto>(r);
        Assert.NotNull(dto?.Id);
        Assert.Equal("Room101", dto.RoomOrAgency);
        Assert.Equal(1, dto.StartPeriodId);
    }

    [Fact]
    public async Task AssignmentRule_GetByYearAndId_ReturnsOk()
    {
        var create = await _client.PostAsync("/api/assignment-rules", Json(new { Year = 2024, RoomOrAgency = "Agency1" }));
        var created = await ReadAs<AssignmentRuleDto>(create);
        var r = await _client.GetAsync($"/api/assignment-rules/2024/{created!.Id}");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var dto = await ReadAs<AssignmentRuleDto>(r);
        Assert.Equal("Agency1", dto?.RoomOrAgency);
    }

    [Fact]
    public async Task AssignmentRule_List_FilteredByYear()
    {
        await _client.PostAsync("/api/assignment-rules", Json(new { Year = 2050, RoomOrAgency = "RoomA" }));
        await _client.PostAsync("/api/assignment-rules", Json(new { Year = 2051, RoomOrAgency = "RoomB" }));
        var r = await _client.GetAsync("/api/assignment-rules?year=2050");
        Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        var list = await ReadAs<List<AssignmentRuleDto>>(r);
        Assert.All(list!, ar => Assert.Equal(2050, ar.Year));
    }

    [Fact]
    public async Task AssignmentRule_Delete_ReturnsNoContent()
    {
        var create = await _client.PostAsync("/api/assignment-rules", Json(new { Year = 2024, RoomOrAgency = "ToDelete" }));
        var created = await ReadAs<AssignmentRuleDto>(create);
        var r = await _client.DeleteAsync($"/api/assignment-rules/2024/{created!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, r.StatusCode);
    }
}
