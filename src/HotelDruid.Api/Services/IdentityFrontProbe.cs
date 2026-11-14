using System.Text.Json;

namespace HotelDruid.Api.Services;

public interface IIdentityFrontProbe
{
    Task<IdentityFrontStatus> GetStatusAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Whether Keycloak is configured as the identity front and whether this API can reach it.
/// The hotel application does not store a second login directory.
/// </summary>
public sealed record IdentityFrontStatus(
    bool Configured,
    bool Reachable,
    bool IsFront,
    string? Provider,
    string? Issuer,
    string? Realm,
    string? AdminConsoleUrl);

public sealed class IdentityFrontProbe : IIdentityFrontProbe
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _http;

    public IdentityFrontProbe(IConfiguration configuration, HttpClient http)
    {
        _configuration = configuration;
        _http = http;
    }

    public async Task<IdentityFrontStatus> GetStatusAsync(CancellationToken cancellationToken = default)
    {
        var provider = _configuration["Identity:Provider"]?.Trim();
        var issuer = TryAbsoluteHttp(_configuration["Identity:Issuer"]);
        var admin = TryAbsoluteHttp(_configuration["Identity:AdminConsoleUrl"]);
        var isKeycloak = string.Equals(provider, "keycloak", StringComparison.OrdinalIgnoreCase);
        var configured = isKeycloak && issuer is not null;
        var reportedProvider = string.IsNullOrWhiteSpace(provider) ? null : provider;

        if (!configured)
            return new IdentityFrontStatus(false, false, false, reportedProvider, issuer?.ToString(), null, admin?.ToString());

        var realm = RealmFromIssuer(issuer!);
        try
        {
            var discovery = new Uri(issuer!.ToString().TrimEnd('/') + "/.well-known/openid-configuration");
            using var response = await _http.GetAsync(discovery, cancellationToken);
            if (!response.IsSuccessStatusCode)
                return new IdentityFrontStatus(true, false, false, "keycloak", issuer!.ToString(), realm, admin?.ToString());

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
            var reachable = document.RootElement.TryGetProperty("issuer", out var issuerElement)
                && issuerElement.ValueKind == JsonValueKind.String
                && !string.IsNullOrWhiteSpace(issuerElement.GetString());
            return new IdentityFrontStatus(true, reachable, reachable, "keycloak", issuer!.ToString(), realm, admin?.ToString());
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
        {
            return new IdentityFrontStatus(true, false, false, "keycloak", issuer!.ToString(), realm, admin?.ToString());
        }
    }

    private static Uri? TryAbsoluteHttp(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;
        if (!Uri.TryCreate(value.Trim().TrimEnd('/'), UriKind.Absolute, out var uri))
            return null;
        if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
            return null;
        return uri;
    }

    private static string? RealmFromIssuer(Uri issuer)
    {
        var parts = issuer.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        for (var i = 0; i < parts.Length - 1; i++)
        {
            if (string.Equals(parts[i], "realms", StringComparison.OrdinalIgnoreCase))
                return Uri.UnescapeDataString(parts[i + 1]);
        }

        return null;
    }
}
