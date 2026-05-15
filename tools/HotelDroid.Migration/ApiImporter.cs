using System.Net.Http.Json;
using System.Text.Json;

namespace HotelDroid.Migration;

/// <summary>
/// Posts a migration package directly to the HotelDroid API import endpoints.
/// </summary>
public class ApiImporter
{
    private readonly HttpClient _http;

    public ApiImporter(string apiBaseUrl)
    {
        _http = new HttpClient { BaseAddress = new Uri(apiBaseUrl.TrimEnd('/') + "/") };
    }

    public async Task<bool> ImportAsync(string zipPath, bool verbose)
    {
        Console.WriteLine("  → Validating package with API...");
        using var zipStream = File.OpenRead(zipPath);
        using var content = new MultipartFormDataContent();
        content.Add(new StreamContent(zipStream), "file", Path.GetFileName(zipPath));

        var validateResponse = await _http.PostAsync("api/import/validate", content);
        var validateBody = await validateResponse.Content.ReadAsStringAsync();

        if (!validateResponse.IsSuccessStatusCode)
        {
            Console.WriteLine($"  ✗ Validation failed ({validateResponse.StatusCode}): {validateBody}");
            return false;
        }

        if (verbose) Console.WriteLine($"  Validation response: {validateBody}");

        // Extract packageId from validation response
        using var doc = JsonDocument.Parse(validateBody);
        if (!doc.RootElement.TryGetProperty("packageId", out var pkgIdEl))
        {
            Console.WriteLine("  ✗ Validation response missing packageId");
            return false;
        }
        var packageId = pkgIdEl.GetString()!;
        Console.WriteLine($"  ✓ Package validated. ID: {packageId}");

        Console.WriteLine("  → Executing import...");
        var executeResponse = await _http.PostAsJsonAsync("api/import/execute", new { packageId });
        var executeBody = await executeResponse.Content.ReadAsStringAsync();

        if (!executeResponse.IsSuccessStatusCode)
        {
            Console.WriteLine($"  ✗ Import failed ({executeResponse.StatusCode}): {executeBody}");
            return false;
        }

        if (verbose) Console.WriteLine($"  Import response: {executeBody}");
        Console.WriteLine("  ✓ Import complete");
        return true;
    }
}
