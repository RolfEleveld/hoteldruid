using HotelDruid.Shared;

namespace HotelDruid.Api.Tests.Services;

public class AccountChartTests
{
    [Fact]
    public void MissingChoice_IsUsali()
    {
        Assert.True(AccountChart.TryResolve(null, null, out var chart, out var error));
        Assert.Null(error);
        Assert.Equal(AccountChart.UsaliId, chart.StandardId);
        Assert.Equal("1000", chart.Cash);
        Assert.Equal("2400", chart.GuestDeposits);
        Assert.Equal("6100", chart.OperatingExpense);
    }

    [Fact]
    public void Custom_UsesTheRegisteredNumbers()
    {
        var codes = new Dictionary<string, string>
        {
            [AccountRoles.Cash] = "1000",
            [AccountRoles.GuestLedger] = "1100",
            [AccountRoles.GuestDeposits] = "2030",
            [AccountRoles.RoomRevenue] = "3000",
            [AccountRoles.OtherIncome] = "3200",
            [AccountRoles.OperatingExpense] = "6000"
        };

        Assert.True(AccountChart.TryResolve(AccountChart.CustomId, codes, out var chart, out var error));
        Assert.Null(error);
        Assert.Equal("1100", chart.GuestLedger);
        Assert.Equal("6000", chart.OperatingExpense);
        Assert.Equal(AccountRoles.Cash, chart.Accounts[0].Role);
    }

    [Fact]
    public void Custom_RejectsADuplicateNumber()
    {
        var codes = new Dictionary<string, string>
        {
            [AccountRoles.Cash] = "1000",
            [AccountRoles.GuestLedger] = "1000",
            [AccountRoles.GuestDeposits] = "2030",
            [AccountRoles.RoomRevenue] = "3000",
            [AccountRoles.OtherIncome] = "3200",
            [AccountRoles.OperatingExpense] = "6000"
        };

        Assert.False(AccountChart.TryResolve("custom", codes, out _, out var error));
        Assert.Equal("duplicate-code", error);
    }

    [Fact]
    public void PublishedCharts_UseTheirAccountNumbers()
    {
        Assert.Equal("1100", AccountChart.SwissKmu.GuestLedger);
        Assert.Equal("2030", AccountChart.SwissKmu.GuestDeposits);
        Assert.Equal("3400", AccountChart.SwissKmu.RoomRevenue);
        Assert.Equal("3600", AccountChart.SwissKmu.OtherIncome);
        Assert.Equal("6700", AccountChart.SwissKmu.OperatingExpense);

        Assert.Equal("101.01", AccountChart.MexicoSat.Cash);
        Assert.Equal("105.01", AccountChart.MexicoSat.GuestLedger);
        Assert.Equal("206.01", AccountChart.MexicoSat.GuestDeposits);
        Assert.Equal("401.01", AccountChart.MexicoSat.RoomRevenue);
        Assert.Equal("403.01", AccountChart.MexicoSat.OtherIncome);
        Assert.Equal("601.84", AccountChart.MexicoSat.OperatingExpense);

        Assert.Equal("570", AccountChart.SpainPgc.Cash);
        Assert.Equal("430", AccountChart.SpainPgc.GuestLedger);
        Assert.Equal("438", AccountChart.SpainPgc.GuestDeposits);
        Assert.Equal("705", AccountChart.SpainPgc.RoomRevenue);
        Assert.Equal("759", AccountChart.SpainPgc.OtherIncome);
        Assert.Equal("629", AccountChart.SpainPgc.OperatingExpense);
    }

    [Fact]
    public void UnknownStandard_IsRejected()
    {
        Assert.False(AccountChart.TryResolve("not-a-standard", null, out _, out var error));
        Assert.Equal("unknown-standard", error);
    }
}
