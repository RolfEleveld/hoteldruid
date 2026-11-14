using System.Security.Claims;

namespace HotelDruid.Api.Services;

/// <summary>
/// The person to record on a journal. A signed-in token wins.
/// A desk-name header is used only when the request is anonymous.
/// An unknown caller stays unnamed. The word admin is not invented.
/// </summary>
public static class RequestActor
{
    public static string? KnownName(HttpContext? context)
    {
        if (context is null)
            return null;

        var signedIn = SignedInName(context.User);
        if (!string.IsNullOrWhiteSpace(signedIn))
            return signedIn;

        if (context.Request.Headers.TryGetValue(HotelDruid.Shared.DeskUserNames.HeaderName, out var header))
            return HotelDruid.Shared.DeskUserNames.Sanitize(header.ToString());

        return null;
    }

    public static bool IsAuthenticated(HttpContext context)
        => !string.IsNullOrWhiteSpace(SignedInName(context.User));

    private static string? SignedInName(ClaimsPrincipal? user)
    {
        if (user?.Identity?.IsAuthenticated != true)
            return null;

        var preferredUserName = user.FindFirst("preferred_username")?.Value;
        if (!string.IsNullOrWhiteSpace(preferredUserName))
            return preferredUserName.Trim();

        var nameClaim = user.FindFirst(ClaimTypes.Name)?.Value ?? user.FindFirst("name")?.Value;
        if (!string.IsNullOrWhiteSpace(nameClaim))
            return nameClaim.Trim();

        var subject = user.FindFirst("sub")?.Value;
        return string.IsNullOrWhiteSpace(subject) ? null : subject.Trim();
    }
}
