namespace HotelDruid.Shared;

public record PerformanceYearDto(
    int Year,
    decimal RoomRevenue,
    decimal OtherIncome,
    decimal OperatingExpense,
    int OccupiedNights,
    int SellableNights,
    int MaintenanceNights,
    decimal Occupancy,
    decimal Adr,
    decimal RevPar);

public record RoomProductivityDto(
    string RoomName,
    int OccupiedNights,
    int SellableNights,
    int MaintenanceNights,
    decimal RoomRevenue,
    decimal Occupancy,
    decimal Adr);

public record RoomYearDto(
    string RoomName,
    int Year,
    int OccupiedNights,
    int SellableNights,
    int MaintenanceNights,
    decimal RoomRevenue,
    decimal Occupancy,
    decimal Adr,
    decimal RevPar,
    decimal? OccupancyChange,
    decimal? AdrChange,
    decimal? RevParChange);

/// <summary>
/// One thing the operator can do. Numbers are raw rates or money; the page turns them into a sentence.
/// Detail is a weekday (0–6), a month (1–12), or a guest capacity, depending on Code.
/// </summary>
public record PerformanceActionDto(
    string Code,
    string? RoomName,
    int Year,
    int CompareYear,
    decimal Primary,
    decimal Secondary,
    decimal Benchmark,
    decimal BenchmarkSecondary,
    string? Detail);

public record PerformanceReportDto(
    string HotelName,
    DateOnly From,
    DateOnly To,
    int RoomCount,
    decimal Occupancy,
    decimal RoomRevenue,
    decimal OtherIncome,
    decimal OperatingExpense,
    int OccupiedNights,
    int SellableNights,
    int MaintenanceNights,
    int BookingCount,
    decimal Adr,
    decimal RevPar,
    long ComputedInMilliseconds,
    IReadOnlyList<PerformanceYearDto> Years,
    IReadOnlyList<RoomProductivityDto> Rooms,
    IReadOnlyList<RoomYearDto> RoomYears,
    IReadOnlyList<PerformanceActionDto> Actions);
