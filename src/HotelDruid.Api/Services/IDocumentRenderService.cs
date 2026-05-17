namespace HotelDruid.Api.Services;

/// <summary>
/// Supported document output formats.
/// </summary>
public enum DocumentFormat
{
    /// <summary>HTML body returned as text/html.</summary>
    Html,
    /// <summary>HTML with window.print() injected — triggers browser print dialog.</summary>
    Print,
    /// <summary>HTML served as application/msword so Word/LibreOffice opens it.</summary>
    Doc,
    /// <summary>RFC 2822 EML file (HTML MIME part) — opens in Outlook/Thunderbird.</summary>
    Eml
}

/// <summary>
/// Assembles the document context for a booking and renders a template
/// in the requested output format.
/// </summary>
public interface IDocumentRenderService
{
    /// <summary>
    /// Build context from storage for the given booking.
    /// Returns null when the booking is not found.
    /// </summary>
    Task<DocumentContext?> BuildContextAsync(string bookingId, int year, string templateType);

    /// <summary>
    /// Render a template's HTML content using a pre-built context.
    /// </summary>
    string RenderHtml(string templateContent, DocumentContext context);

    /// <summary>
    /// Format rendered HTML into the target output format.
    /// Returns (content, contentType, filename).
    /// </summary>
    (string content, string contentType, string filename) ApplyFormat(
        string renderedHtml, DocumentContext context, DocumentFormat format);
}
