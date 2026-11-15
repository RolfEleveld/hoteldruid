using HotelDruid.Api.Services;
using HotelDruid.Shared;
using Xunit;

namespace HotelDruid.Api.Tests.Unit;

public class DeskReportBuilderTests
{
    private static readonly DateOnly Day = new(2026, 11, 15);

    [Fact]
    public void Arrivals_UsesTheGuestNameAndTheAmountStillDue()
    {
        var report = DeskReportBuilder.Build("arrivals", Day, Sample());
        var row = Assert.Single(report.Sections[0].Rows);
        Assert.Equal("101", row.Cells[0]);
        Assert.Equal("Bauer, Felix", row.Cells[1]);
        Assert.Equal("150.00", row.Cells[5]);
    }

    [Fact]
    public void RoomStatus_AsksForADepartureCheckAndTheArrivalPreparation()
    {
        var report = DeskReportBuilder.Build("room-status", Day, Sample());
        var room = report.Sections[0].Rows[0];
        Assert.Equal("101", room.Cells[0]);
        Assert.Equal("reports.work.turn", room.Cells[3]);
        Assert.Contains("Bauer, Felix", room.Cells[2]);
        Assert.Contains("Rossi, Anna", room.Cells[2]);
        Assert.Equal("", room.Cells[4]);
    }

    [Fact]
    public void Cashier_NetsThePostedCash()
    {
        var report = DeskReportBuilder.Build("cashier", Day, Sample());
        Assert.Equal("60.00", report.Sections[0].Totals[0].Value);
        Assert.Equal("reports.count.opening", report.Sections[1].Rows[0].Cells[0]);
        Assert.Equal("check", report.Sections[1].Kinds[1]);
    }

    [Fact]
    public void Forecast_CountsTheNightAndTheQuotedRoomCharge()
    {
        var report = DeskReportBuilder.Build("forecast", Day, Sample());
        Assert.Equal(14, report.Sections[0].Rows.Count);
        Assert.Equal("50.0%", report.Sections[0].Rows[0].Cells[4]);
        Assert.Equal("100.00", report.Sections[0].Rows[0].Cells[5]);
    }

    [Fact]
    public void Tax_UsesTheLedgerDateAndTheRateInsideThePrice()
    {
        var report = DeskReportBuilder.Build("tax", Day, Sample());
        Assert.Equal(new DateOnly(2026, 11, 1), report.From);
        Assert.Equal(new DateOnly(2026, 11, 30), report.To);
        Assert.Equal("110.00", report.Sections[0].Totals[0].Value);
        Assert.Equal("10.00", report.Sections[0].Totals[1].Value);
    }

    private static DeskFacts Sample() => new(
        "Belvedere",
        "EUR",
        10m,
        AccountChart.Usali,
        [new DeskRoom("r1", "101", "A", "1"), new DeskRoom("r2", "102", "A", "1")],
        [
            new DeskStay("b1", "r1", "c1", "Bauer, Felix", Day, Day.AddDays(2), "Confirmed", 2, 200m, 50m, "Late", "DE", "Passport", "X1", new DateTime(1980, 1, 2)),
            new DeskStay("b2", "r1", "c2", "Rossi, Anna", Day.AddDays(-1), Day, "CheckedIn", 1, 80m, 80m, null, "IT", null, null, null),
            new DeskStay("b3", "r2", "c3", "Gone, Guest", Day, Day.AddDays(1), "Cancelled", 1, 90m, 0m, null, null, null, null, null)
        ],
        [
            new DeskLine(Day, "booking_payment", "1000", "asset", 80m, 0m, "reg", null, "Card", "b1"),
            new DeskLine(Day, "booking_refund", "1000", "asset", 0m, 20m, "reg", null, "Cash", "b2"),
            new DeskLine(new DateOnly(2026, 11, 3), "booking_stay_charge", "4100", "revenue", 0m, 110m, null, null, "Stay", "b1")
        ],
        new Dictionary<string, string> { ["reg"] = "Front desk" });
}
