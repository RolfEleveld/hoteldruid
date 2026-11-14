using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using HotelDruid.Api;
using HotelDruid.Shared;
using HotelDruid.Shared.Configuration;
using Microsoft.AspNetCore.Mvc.Testing;

namespace HotelDruid.Api.Tests.Integration;

[Collection("Sequential")]
public class AccountingStandardApiTests : IAsyncLifetime
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;
    private string _testDataRoot = null!;

    public Task InitializeAsync()
    {
        _testDataRoot = Path.Combine(Path.GetTempPath(), $"api-chart-{Guid.NewGuid()}");
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
    public async Task DefaultChart_IsUsali_UntilSetupChoosesAnother()
    {
        var response = await _client.GetAsync("/api/ledger/accounts");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var accounts = await response.Content.ReadFromJsonAsync<List<ChartAccount>>(JsonOptions) ?? [];
        Assert.Equal("1000", accounts.Single(account => account.Role == AccountRoles.Cash).Code);
        Assert.Equal("4100", accounts.Single(account => account.Role == AccountRoles.RoomRevenue).Code);
    }

    [Fact]
    public async Task CustomChart_PostsTheRegisteredNumbers()
    {
        var save = await _client.PutAsJsonAsync("/api/system/configuration", new SystemConfiguration
        {
            Id = "system",
            DefaultCurrency = "CHF",
            DefaultYear = 2026,
            AccountingStandard = AccountChart.CustomId,
            AccountCodes = new Dictionary<string, string>
            {
                [AccountRoles.Cash] = "1000",
                [AccountRoles.GuestLedger] = "1100",
                [AccountRoles.GuestDeposits] = "2030",
                [AccountRoles.RoomRevenue] = "3000",
                [AccountRoles.OtherIncome] = "3200",
                [AccountRoles.OperatingExpense] = "6000"
            }
        });
        Assert.Equal(HttpStatusCode.OK, save.StatusCode);

        var created = await _client.PostAsJsonAsync("/api/money-history", new
        {
            year = 2026,
            cashRegisterId = "CR1",
            amount = -4.0,
            type = "expense",
            description = "Swiss coffee",
            date = "2026-05-15"
        }, JsonOptions);
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);

        var lines = await _client.GetFromJsonAsync<List<LedgerLine>>(
            "/api/ledger/lines?from=2026-05-15&to=2026-05-15&unassigned=true",
            JsonOptions) ?? [];
        var coffee = lines.Where(line => line.Description == "Swiss coffee").ToList();
        Assert.Contains(coffee, line => line.AccountCode == "6000" && line.Debit == 4m);
        Assert.Contains(coffee, line => line.AccountCode == "1000" && line.Credit == 4m);
        Assert.DoesNotContain(coffee, line => line.AccountCode == "6100");
    }

    [Fact]
    public async Task Setup_WritesTheChartIntoTheLedger_AndARepeatSaveDoesNot()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var spain = await _client.PutAsJsonAsync("/api/system/configuration", new SystemConfiguration
        {
            Id = "system",
            DefaultCurrency = "EUR",
            DefaultYear = today.Year,
            AccountingStandard = AccountChart.SpainPgcId
        });
        Assert.Equal(HttpStatusCode.OK, spain.StatusCode);

        var first = await LinesOn(today);
        var adoption = first.Where(line => line.EntryType == "chart_of_accounts").ToList();
        Assert.Equal(6, adoption.Count);
        Assert.Single(adoption.Select(line => line.JournalId).Distinct());
        Assert.Contains(adoption, line => line.AccountCode == "570" && line.Debit == 0 && line.Credit == 0);
        Assert.Contains(adoption, line => line.AccountCode == "705");
        Assert.Contains(adoption, line => line.Description!.Contains("from this entry forward", StringComparison.Ordinal));
        Assert.Contains(adoption, line => line.Description!.Contains("Spain (PGC)", StringComparison.Ordinal));

        var repeat = await _client.PutAsJsonAsync("/api/system/configuration", new SystemConfiguration
        {
            Id = "system",
            DefaultCurrency = "EUR",
            DefaultYear = today.Year,
            AccountingStandard = AccountChart.SpainPgcId
        });
        Assert.Equal(HttpStatusCode.OK, repeat.StatusCode);
        var second = await LinesOn(today);
        Assert.Equal(adoption.Count, second.Count(line => line.EntryType == "chart_of_accounts"));

        var usali = await _client.PutAsJsonAsync("/api/system/configuration", new SystemConfiguration
        {
            Id = "system",
            DefaultCurrency = "EUR",
            DefaultYear = today.Year,
            AccountingStandard = AccountChart.UsaliId
        });
        Assert.Equal(HttpStatusCode.OK, usali.StatusCode);
        var changed = (await LinesOn(today)).Where(line => line.EntryType == "chart_of_accounts" && line.Description!.Contains("changed from", StringComparison.Ordinal)).ToList();
        Assert.Equal(6, changed.Count);
        Assert.Contains(changed, line => line.AccountCode == "1000");
        Assert.Contains(changed, line => line.Description!.Contains("570", StringComparison.Ordinal));

        var created = await _client.PostAsJsonAsync("/api/money-history", new
        {
            year = today.Year,
            cashRegisterId = "CR1",
            amount = -3.0,
            type = "expense",
            description = "After the change",
            date = today.ToString("yyyy-MM-dd")
        }, JsonOptions);
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        var coffee = (await LinesOn(today)).Where(line => line.Description == "After the change").ToList();
        Assert.Contains(coffee, line => line.AccountCode == "6100" && line.Debit == 3m);
        Assert.Contains(coffee, line => line.AccountCode == "1000" && line.Credit == 3m);
        Assert.DoesNotContain(coffee, line => line.AccountCode == "629");
    }

    private async Task<List<LedgerLine>> LinesOn(DateOnly day)
    {
        var stamp = day.ToString("yyyy-MM-dd");
        return await _client.GetFromJsonAsync<List<LedgerLine>>(
            $"/api/ledger/lines?from={stamp}&to={stamp}&unassigned=true",
            JsonOptions) ?? [];
    }

    [Fact]
    public async Task CustomChart_RejectsDuplicateNumbers()
    {
        var save = await _client.PutAsJsonAsync("/api/system/configuration", new SystemConfiguration
        {
            Id = "system",
            AccountingStandard = "custom",
            AccountCodes = new Dictionary<string, string>
            {
                [AccountRoles.Cash] = "1000",
                [AccountRoles.GuestLedger] = "1000",
                [AccountRoles.GuestDeposits] = "2030",
                [AccountRoles.RoomRevenue] = "3000",
                [AccountRoles.OtherIncome] = "3200",
                [AccountRoles.OperatingExpense] = "6000"
            }
        });
        Assert.Equal(HttpStatusCode.BadRequest, save.StatusCode);
    }

    private sealed class ChartAccount
    {
        public string? Code { get; set; }
        public string? Role { get; set; }
    }

    private sealed class LedgerLine
    {
        public string? JournalId { get; set; }
        public string? EntryType { get; set; }
        public string? AccountCode { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string? Description { get; set; }
    }
}
