using System.Net.Http.Json;
using System.Text.Json;
using HotelDruid.Api;
using HotelDruid.Api.Models;
using HotelDruid.Shared;
using Microsoft.AspNetCore.Mvc.Testing;

namespace HotelDruid.Api.Tests.Integration;

[Collection("Sequential")]
public class PerformanceScenarioTests : IAsyncLifetime
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);
    private readonly List<string> _roots = [];

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync()
    {
        foreach (var root in _roots)
        {
            try
            {
                if (Directory.Exists(root))
                    Directory.Delete(root, recursive: true);
            }
            catch
            {
                // The test host can still be releasing files.
            }
        }

        return Task.CompletedTask;
    }

    [Fact]
    public async Task Export_RoundTripsRevenue_ForAGeneratedHotel()
    {
        var source = NewRoot();
        var written = ScenarioWriter.Write(source, new ScenarioRequest(
            new DateOnly(2024, 6, 1),
            new DateOnly(2024, 7, 20),
            20,
            0.67,
            20261114));
        Assert.InRange(written.Occupancy, 0.60, 0.75);
        Assert.True(written.RoomRevenue > 0);
        Assert.True(written.BabyCotStays > 0);
        Assert.True(written.AdjacentStays > 0);
        Assert.True(written.MaintenanceNights > 0);

        await using var sourceFactory = Factory(source);
        var sourceClient = sourceFactory.CreateClient();
        var before = await sourceClient.GetFromJsonAsync<PerformanceReportDto>("/api/reports/performance", Json);
        Assert.NotNull(before);
        Assert.Equal(20, before!.RoomCount);
        Assert.Equal(written.RoomRevenue, before.RoomRevenue);
        Assert.Equal(written.OtherIncome, before.OtherIncome);

        var created = await sourceClient.PostAsJsonAsync("/api/export/create", new { });
        created.EnsureSuccessStatusCode();
        var export = await created.Content.ReadFromJsonAsync<ExportCreated>(Json);
        Assert.NotNull(export);
        await WaitUntil(async () =>
        {
            var status = await sourceClient.GetFromJsonAsync<ExportStatusResponse>($"/api/export/{export!.ExportId}/status", Json);
            return status?.Status is "completed" or "failed" ? status.Status : null;
        });
        var zip = await sourceClient.GetByteArrayAsync($"/api/export/{export!.ExportId}/download");
        Assert.True(zip.Length > 1000);

        var destination = NewRoot();
        await using var destinationFactory = Factory(destination);
        var destinationClient = destinationFactory.CreateClient();
        using var form = new MultipartFormDataContent();
        using var file = new ByteArrayContent(zip);
        form.Add(file, "file", "scenario.zip");
        var validated = await destinationClient.PostAsync("/api/import/validate", form);
        if (!validated.IsSuccessStatusCode)
            throw new Exception(await validated.Content.ReadAsStringAsync());
        var validation = await validated.Content.ReadFromJsonAsync<ImportValidationResponse>(Json);
        Assert.NotNull(validation);
        Assert.True(validation!.Valid, string.Join("; ", validation.Errors));

        var started = await destinationClient.PostAsJsonAsync($"/api/import/{validation.PackageId}/execute", new ImportExecuteRequest(validation.PackageId));
        started.EnsureSuccessStatusCode();
        var job = await started.Content.ReadFromJsonAsync<ImportExecuteResponse>(Json);
        var final = await WaitUntil(async () =>
        {
            var status = await destinationClient.GetFromJsonAsync<ImportStatusResponse>($"/api/import/{job!.ImportId}/status", Json);
            return status?.Status is "completed" or "failed" ? status : null;
        });
        Assert.Equal("completed", final!.Status);
        Assert.True(final.Errors.Count == 0, string.Join("; ", final.Errors));

        var after = await destinationClient.GetFromJsonAsync<PerformanceReportDto>("/api/reports/performance", Json);
        Assert.NotNull(after);
        Assert.Equal(before.RoomRevenue, after!.RoomRevenue);
        Assert.Equal(before.OtherIncome, after.OtherIncome);
        Assert.Equal(before.OperatingExpense, after.OperatingExpense);
        Assert.Equal(before.OccupiedNights, after.OccupiedNights);
        Assert.Equal(before.SellableNights, after.SellableNights);
    }

    private string NewRoot()
    {
        var root = Path.Combine(Path.GetTempPath(), "hoteldruid-scenario-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        _roots.Add(root);
        return root;
    }

    private static WebApplicationFactory<Program> Factory(string dataRoot) =>
        new WebApplicationFactory<Program>().WithWebHostBuilder(builder => builder.UseSetting("DataRoot", dataRoot));

    private static async Task<T> WaitUntil<T>(Func<Task<T?>> read) where T : class
    {
        var deadline = DateTime.UtcNow.AddMinutes(3);
        while (DateTime.UtcNow < deadline)
        {
            var value = await read();
            if (value is not null)
                return value;
            await Task.Delay(200);
        }

        throw new TimeoutException("The export or import did not finish.");
    }

    private sealed record ExportCreated(string ExportId, string Status, string StatusUrl, int EstimatedSeconds);
}
