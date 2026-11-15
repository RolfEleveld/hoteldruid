namespace HotelDruid.Client.Reports;

public sealed record ReportLink(string Key, string TitleKey, string BlurbKey);

/// <summary>The fixed set of desk and manager sheets. The landing widget and the reports page share this list.</summary>
public static class ReportCatalog
{
    public static readonly IReadOnlyList<ReportLink> All =
    [
        new("arrivals", "reports.arrivals.title", "reports.arrivals.blurb"),
        new("in-house", "reports.inhouse.title", "reports.inhouse.blurb"),
        new("departures", "reports.departures.title", "reports.departures.blurb"),
        new("room-status", "reports.rooms.title", "reports.rooms.blurb"),
        new("cashier", "reports.cashier.title", "reports.cashier.blurb"),
        new("flash", "reports.flash.title", "reports.flash.blurb"),
        new("forecast", "reports.forecast.title", "reports.forecast.blurb"),
        new("balances", "reports.balances.title", "reports.balances.blurb"),
        new("performance", "reports.performance.title", "reports.performance.blurb"),
        new("tax", "reports.tax.title", "reports.tax.blurb"),
        new("statistics", "reports.statistics.title", "reports.statistics.blurb")
    ];
}
