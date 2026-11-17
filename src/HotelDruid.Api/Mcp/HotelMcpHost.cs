using System.Net.Security;
using ModelContextProtocol.Server;

namespace HotelDruid.Mcp;

/// <summary>
/// Lets a test point the desk tools at the in-memory hotel instead of the public URL.
/// The running hotel leaves this empty and calls itself on its own HTTPS port.
/// </summary>
public sealed class HotelHttpSlot
{
    public HttpClient? Override { get; set; }
}

public static class HotelMcpHost
{
    public const string Instructions = """
        You are the desk of this hotel. Search before you create a guest or a stay.
        Speak rooms by name. The booking year is the arrival year.
        Source is who sent the stay. External reference is their number.
        An outside booking is an inbox request until it is confirmed.
        The hotel queues messages and does not send them.
        A signed night means the day was seen, not that it was paid. Do not sign a night unless the person named that date and asked to close it.
        """;

    public static void Add(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<HotelHttpSlot>();
        services.AddSingleton(sp =>
        {
            var slot = sp.GetRequiredService<HotelHttpSlot>();
            var http = slot.Override ?? CreateLoopback(configuration);
            return new HotelClient(http);
        });

        var assembly = typeof(HotelMcpHost).Assembly;
        services.AddMcpServer(options => options.ServerInstructions = Instructions)
            .WithHttpTransport()
            .WithToolsFromAssembly(assembly)
            .WithResourcesFromAssembly(assembly)
            .WithPromptsFromAssembly(assembly);
    }

    private static HttpClient CreateLoopback(IConfiguration configuration)
    {
        var api = configuration["HotelDruid:Api"]
            ?? Environment.GetEnvironmentVariable("HOTELDRUID_API")
            ?? "https://127.0.0.1:5001";
        return new HttpClient(new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, _, _, errors) =>
            {
                if (errors == SslPolicyErrors.None)
                    return true;
                var host = message.RequestUri?.Host;
                return host is "localhost" or "127.0.0.1";
            }
        })
        {
            BaseAddress = new Uri(api.TrimEnd('/') + "/"),
            Timeout = TimeSpan.FromSeconds(60)
        };
    }
}
