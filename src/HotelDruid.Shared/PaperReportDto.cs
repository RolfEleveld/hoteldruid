namespace HotelDruid.Shared;

/// <summary>One printable desk or manager sheet. Cells are plain values; the page translates keys and formats money.</summary>
public record PaperReportDto(
    string Key,
    string HotelName,
    string Currency,
    DateOnly From,
    DateOnly To,
    IReadOnlyList<PaperSectionDto> Sections);

public record PaperSectionDto(
    string HeadingKey,
    IReadOnlyList<string> Columns,
    IReadOnlyList<string> Kinds,
    IReadOnlyList<PaperRowDto> Rows,
    IReadOnlyList<PaperTotalDto> Totals);

public record PaperRowDto(IReadOnlyList<string> Cells);

public record PaperTotalDto(string LabelKey, string Value, string Kind);
