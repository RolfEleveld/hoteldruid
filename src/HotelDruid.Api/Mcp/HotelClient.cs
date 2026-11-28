using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using HotelDruid.Shared;

namespace HotelDruid.Mcp;

public sealed class HotelCallException(int status, string body) : Exception(Trim(body))
{
    public int Status { get; } = status;

    private static string Trim(string body)
    {
        var text = string.IsNullOrWhiteSpace(body) ? "The hotel returned an empty error." : body.Trim();
        return text.Length <= 600 ? text : text[..600];
    }
}

public sealed class HotelClient(HttpClient http)
{
    public static readonly JsonSerializerOptions Write = new()
    {
        PropertyNamingPolicy = null,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    public async Task<JsonElement?> SendAsync(HttpMethod method, string path, object? body, CancellationToken cancellationToken, IReadOnlyDictionary<string, string>? headers = null)
    {
        using var request = new HttpRequestMessage(method, path.TrimStart('/'));
        if (headers is not null)
        {
            foreach (var header in headers)
                request.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }
        if (body is JsonNode node)
            request.Content = new StringContent(node.ToJsonString(), Encoding.UTF8, "application/json");
        else if (body is not null)
            request.Content = JsonContent.Create(body, options: Write);

        using var response = await http.SendAsync(request, cancellationToken);
        var text = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
            throw new HotelCallException((int)response.StatusCode, string.IsNullOrWhiteSpace(text) ? response.ReasonPhrase ?? "Request failed." : text);
        if (string.IsNullOrWhiteSpace(text))
            return null;
        using var document = JsonDocument.Parse(text);
        return document.RootElement.Clone();
    }

    public Task<JsonElement?> GetAsync(string path, CancellationToken cancellationToken)
        => SendAsync(HttpMethod.Get, path, null, cancellationToken);

    public async Task<DateOnly> TodayAsync(CancellationToken cancellationToken)
    {
        var body = await GetAsync("api/system/configuration", cancellationToken);
        var zone = body is null ? null : JsonText(body.Value, "TimeZoneId");
        return HotelTime.Today(zone);
    }

    public Task<JsonElement?> PostAsync(string path, object body, CancellationToken cancellationToken, IReadOnlyDictionary<string, string>? headers = null)
        => SendAsync(HttpMethod.Post, path, body, cancellationToken, headers);

    public Task<JsonElement?> PutAsync(string path, object body, CancellationToken cancellationToken)
        => SendAsync(HttpMethod.Put, path, body, cancellationToken);

    public async Task<IReadOnlyList<JsonElement>> ArrayAsync(string path, CancellationToken cancellationToken)
    {
        var body = await GetAsync(path, cancellationToken);
        if (body is null || body.Value.ValueKind != JsonValueKind.Array)
            return [];
        return body.Value.EnumerateArray().Select(item => item.Clone()).ToList();
    }

    public async Task<(string Id, string Name, int Capacity, string? Condition)?> RoomAsync(string room, CancellationToken cancellationToken)
    {
        foreach (var item in await ArrayAsync("api/rooms", cancellationToken))
        {
            var id = JsonText(item, "Id");
            var name = JsonText(item, "Name");
            if (string.Equals(id, room, StringComparison.OrdinalIgnoreCase) || string.Equals(name, room, StringComparison.OrdinalIgnoreCase))
                return (id ?? room, name ?? id ?? room, JsonInt(item, "Capacity"), JsonText(item, "Condition"));
        }

        return null;
    }

    public async Task<JsonElement?> BookingAsync(string bookingId, CancellationToken cancellationToken)
    {
        try
        {
            return await GetAsync($"api/bookings/by-id/{Uri.EscapeDataString(bookingId)}", cancellationToken);
        }
        catch (HotelCallException ex) when (ex.Status == 404)
        {
            return null;
        }
    }

    public async Task<string?> RegisterIdAsync(string? register, CancellationToken cancellationToken)
    {
        var rows = await ArrayAsync("api/cash-registers", cancellationToken);
        if (rows.Count == 0)
            return null;
        if (string.IsNullOrWhiteSpace(register))
            return JsonText(rows[0], "Id");
        foreach (var row in rows)
        {
            var id = JsonText(row, "Id");
            var name = JsonText(row, "Name");
            if (string.Equals(id, register, StringComparison.OrdinalIgnoreCase) || string.Equals(name, register, StringComparison.OrdinalIgnoreCase))
                return id;
        }

        return null;
    }

    public static string? JsonText(JsonElement element, string name)
        => element.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.String ? value.GetString() : null;

    public static int JsonInt(JsonElement element, string name)
        => element.TryGetProperty(name, out var value) && value.TryGetInt32(out var number) ? number : 0;

    public static bool JsonBool(JsonElement element, string name)
        => element.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.True;

    public static string? JsonRaw(JsonElement element, string name)
        => element.TryGetProperty(name, out var value) && value.ValueKind is not JsonValueKind.Null and not JsonValueKind.Undefined
            ? value.ToString()
            : null;

    public static DateOnly? Date(string? text)
        => DateOnly.TryParse(text, out var date) ? date : null;

    public static bool Closed(string? status)
        => status is not null && (status.Contains("cancel", StringComparison.OrdinalIgnoreCase)
            || status.Contains("void", StringComparison.OrdinalIgnoreCase)
            || status.Contains("deleted", StringComparison.OrdinalIgnoreCase)
            || status.Contains("no-show", StringComparison.OrdinalIgnoreCase)
            || status.Contains("noshow", StringComparison.OrdinalIgnoreCase));
}

public sealed record HotelReply(bool Ok, string Summary, JsonNode? Data = null, string? Next = null);

public static class HotelCall
{
    public static async Task<HotelReply> Run(Func<Task<HotelReply>> action)
    {
        try
        {
            return await action();
        }
        catch (HotelCallException ex)
        {
            return new HotelReply(false, ex.Message, new JsonObject { ["status"] = ex.Status }, "Correct the arguments from the error. Do not repeat the same call.");
        }
        catch (HttpRequestException ex)
        {
            return new HotelReply(false, "The hotel API is not reachable. " + ex.Message, null, "Start the hotel, then try the read again.");
        }
    }
}
