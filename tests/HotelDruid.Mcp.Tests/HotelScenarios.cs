using System.Net.Http.Json;
using System.Text.Json;
using HotelDruid.Mcp;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Client;

namespace HotelDruid.Mcp.Tests;

[CollectionDefinition("mcp")]
public sealed class McpCollection : ICollectionFixture<HotelFixture>;

public sealed class HotelFixture : IAsyncLifetime
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), "hoteldruid-mcp-" + Guid.NewGuid().ToString("N"));
    private string? _previousRoot;
    private WebApplicationFactory<Program> _hotel = null!;

    public HttpClient Http { get; private set; } = null!;
    public HotelClient Client { get; private set; } = null!;
    public HotelHttpSlot Slot => _hotel.Services.GetRequiredService<HotelHttpSlot>();

    public HttpClient CreateClient() => _hotel.CreateClient();

    public async Task InitializeAsync()
    {
        Directory.CreateDirectory(_root);
        _previousRoot = Environment.GetEnvironmentVariable("HOTELDRUID_DATAROOT");
        Environment.SetEnvironmentVariable("HOTELDRUID_DATAROOT", _root);
        _hotel = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => builder.UseSetting("DataRoot", _root));
        Http = _hotel.CreateClient();
        Client = new HotelClient(Http);
        await Post("api/rooms", new { Name = "901", Capacity = 2, HasBeds = "S" });
        await Post("api/rooms", new { Name = "902", Capacity = 2, HasBeds = "S" });
        await Post("api/rooms", new { Name = "903", Capacity = 2, HasBeds = "S" });
        await Post("api/cash-registers", new { Name = "Desk" });
    }

    public Task DisposeAsync()
    {
        Http.Dispose();
        _hotel.Dispose();
        Environment.SetEnvironmentVariable("HOTELDRUID_DATAROOT", _previousRoot);
        try
        {
            if (Directory.Exists(_root))
                Directory.Delete(_root, recursive: true);
        }
        catch (IOException)
        {
        }

        return Task.CompletedTask;
    }

    private async Task Post(string path, object body)
    {
        var response = await Http.PostAsJsonAsync(path, body, HotelClient.Write);
        var text = await response.Content.ReadAsStringAsync();
        Assert.True(response.IsSuccessStatusCode, path + " " + text);
    }
}

[Collection("mcp")]
public sealed class HotelScenarios(HotelFixture hotel)
{

    [Fact]
    public async Task Desk_channel_money_and_admin_scenarios()
    {
        var discovery = new DiscoveryTools(hotel.Client);
        var reservations = new ReservationTools(hotel.Client);
        var guests = new GuestTools(hotel.Client);
        var channel = new ChannelTools(hotel.Client);
        var finance = new FinanceTools(hotel.Client);
        var admin = new AdminTools(hotel.Client);

        var free = await discovery.FindAvailability("2027-03-16", "2027-03-18", 2, "901");
        Assert.True(free.Ok, free.Summary);
        Assert.Contains("901", free.Summary + free.Data?.ToJsonString());

        var reserved = await reservations.Create("Adler", "Nora", room: "901", arrival: "2027-03-16", departure: "2027-03-18", guests: 2, source: "phone", email: "nora.adler@example.com");
        Assert.True(reserved.Ok, reserved.Summary);
        var bookingId = reserved.Data!["booking"]!["id"]!.GetValue<string>();

        var found = await discovery.Search("Adler");
        Assert.True(found.Ok, found.Summary);
        Assert.Contains("Adler", found.Data!.ToJsonString());

        var saved = await guests.Save(lastName: "Adler", firstName: "Nora", guestId: reserved.Data["booking"]!["guestId"]!.GetValue<string>(), nationality: "CH", documentType: "Passport", documentNumber: "X123", city: "Lugano");
        Assert.True(saved.Ok, saved.Summary);

        var attached = await guests.Attach(bookingId, lastName: "Adler", firstName: "Lars", guestNumber: 2);
        Assert.True(attached.Ok, attached.Summary);

        var stay = await reservations.Read(bookingId);
        Assert.True(stay.Ok, stay.Summary);
        Assert.Equal("phone", stay.Data!["booking"]!["source"]!.GetValue<string>());
        Assert.Equal("901", stay.Data["booking"]!["room"]!.GetValue<string>());

        var paid = await finance.Pay(bookingId, 40m, "payment", "Card on the phone booking", "Nora");
        Assert.True(paid.Ok, paid.Summary);
        Assert.Contains("40", paid.Data!.ToJsonString());

        var invoice = await finance.Issue(bookingId, "invoice");
        Assert.True(invoice.Ok, invoice.Summary);
        Assert.StartsWith("2027-", invoice.Data!["invoiceNumber"]!.GetValue<string>());

        var request = await channel.Submit("Berg", "2027-06-01", "2027-06-03", "902", "Ida", "ida.berg@example.com", 1, "booking.com", externalReference: "BDC-900");
        Assert.True(request.Ok, request.Summary);
        var messageId = request.Data!["message"]!["id"]!.GetValue<string>();

        var confirmed = await channel.Confirm(messageId);
        Assert.True(confirmed.Ok, confirmed.Summary);
        var channelBooking = confirmed.Data!["message"]!["bookingId"]!.GetValue<string>();
        var channelStay = await reservations.Read(channelBooking);
        Assert.Equal("dispatch", channelStay.Data!["booking"]!["source"]!.GetValue<string>());
        Assert.Equal(messageId, channelStay.Data["booking"]!["externalReference"]!.GetValue<string>());

        var numbered = await reservations.Change(channelBooking, externalReference: "BDC-900", source: "booking.com");
        Assert.True(numbered.Ok, numbered.Summary);
        var numberedStay = await reservations.Read(channelBooking);
        Assert.Equal("BDC-900", numberedStay.Data!["booking"]!["externalReference"]!.GetValue<string>());
        Assert.Equal("booking.com", numberedStay.Data["booking"]!["source"]!.GetValue<string>());

        var waiting = await channel.List("out", "ready");
        Assert.True(waiting.Ok, waiting.Summary);
        Assert.True(waiting.Data!["total"]!.GetValue<int>() >= 1);

        var purchase = await finance.Expense(18.5m, "MCP scenario linen", "2027-03-16");
        Assert.True(purchase.Ok, purchase.Summary);
        var drawer = await finance.Drawer(5m, "in", "MCP scenario float", "2027-03-16");
        Assert.True(drawer.Ok, drawer.Summary);
        var walkUp = await finance.Sell("MCP scenario coffee", 4m);
        Assert.True(walkUp.Ok, walkUp.Summary);

        var ledger = await finance.Ledger("2027-03-16", "2027-03-16");
        Assert.True(ledger.Ok, ledger.Summary);
        Assert.Contains("hotel_purchase", ledger.Data!.ToJsonString());

        var desk = await discovery.ReadDesk("2027-03-16");
        Assert.True(desk.Ok, desk.Summary);
        Assert.Contains("Adler", desk.Data!.ToJsonString(), StringComparison.OrdinalIgnoreCase);

        var report = await admin.Report("arrivals", "2027-03-16");
        Assert.True(report.Ok, report.Summary);

        var gaps = await admin.Registration("2027-03-16");
        Assert.True(gaps.Ok, gaps.Summary);

        var profile = await admin.ReadHotel();
        Assert.True(profile.Ok, profile.Summary);

        var night = await admin.Night("2026-01-15");
        Assert.True(night.Ok, night.Summary);
        Assert.False(night.Data!["Signed"]!.GetValue<bool>());
        var refused = await admin.Sign("2026-01-15", confirm: false, cashAgrees: true, balancesReviewed: true, roomsSeen: true);
        Assert.False(refused.Ok);
        var signed = await admin.Sign("2026-01-15", confirm: true, cashAgrees: true, balancesReviewed: true, roomsSeen: true, person: "MCP scenario", note: "Seen in the scenario.");
        Assert.True(signed.Ok, signed.Summary);

        var automated = await admin.Automation(true);
        Assert.True(automated.Ok, automated.Summary);
        await admin.Automation(false);

        var toMove = await reservations.Create("Keller", "Otto", room: "901", arrival: "2027-04-01", departure: "2027-04-04", guests: 1, source: "desk");
        Assert.True(toMove.Ok, toMove.Summary);
        var moved = await reservations.Move(toMove.Data!["booking"]!["id"]!.GetValue<string>(), "2027-04-02", "903", "Quieter room");
        Assert.True(moved.Ok, moved.Summary);

        var toCancel = await reservations.Create("Frei", "Una", room: "903", arrival: "2027-07-01", departure: "2027-07-03", guests: 1, source: "email");
        Assert.True(toCancel.Ok, toCancel.Summary);
        var cancelled = await reservations.Cancel(toCancel.Data!["booking"]!["id"]!.GetValue<string>());
        Assert.True(cancelled.Ok, cancelled.Summary);

        var toMiss = await reservations.Create("Nacht", "Pia", room: "903", arrival: "2027-08-01", departure: "2027-08-03", guests: 1, source: "phone");
        Assert.True(toMiss.Ok, toMiss.Summary);
        var missed = await reservations.NoShow(toMiss.Data!["booking"]!["id"]!.GetValue<string>(), "Did not arrive");
        Assert.True(missed.Ok, missed.Summary);

        var toStay = await reservations.Create("Haus", "Eva", room: "902", arrival: "2027-05-01", departure: "2027-05-03", guests: 1, source: "desk");
        Assert.True(toStay.Ok, toStay.Summary);
        var stayId = toStay.Data!["booking"]!["id"]!.GetValue<string>();
        var arrived = await reservations.CheckIn(stayId, "2027-05-01", 1);
        Assert.True(arrived.Ok, arrived.Summary);
        var left = await reservations.CheckOut(stayId, "2027-05-03");
        Assert.True(left.Ok, left.Summary);
    }

    [Fact]
    public async Task Mcp_client_lists_tools_and_reads_a_guest()
    {
        var toolHttp = hotel.CreateClient();
        var mcpHttp = hotel.CreateClient();
        hotel.Slot.Override = toolHttp;
        var transport = new HttpClientTransport(
            new HttpClientTransportOptions
            {
                Endpoint = new Uri(mcpHttp.BaseAddress!, "mcp"),
                Name = "hoteldruid-test"
            },
            mcpHttp,
            LoggerFactory.Create(_ => { }),
            ownsHttpClient: false);
        await using var mcp = await McpClient.CreateAsync(transport);
        var tools = await mcp.ListToolsAsync();
        Assert.Contains(tools, tool => tool.Name == "search_hotel");
        Assert.Contains(tools, tool => tool.Name == "confirm_channel_request");
        Assert.Contains(tools, tool => tool.Name == "issue_document");
        Assert.Contains(tools, tool => tool.Name == "sign_night");

        var prompts = await mcp.ListPromptsAsync();
        Assert.Contains(prompts, prompt => prompt.Name == "accept_an_ota_booking");

        var resources = await mcp.ListResourcesAsync();
        Assert.Contains(resources, resource => resource.Uri == "hotel://guide");

        var result = await mcp.CallToolAsync("search_hotel", new Dictionary<string, object?> { ["query"] = "901" });
        var text = result.StructuredContent is { ValueKind: not JsonValueKind.Undefined and not JsonValueKind.Null } structured
            ? structured.GetRawText()
            : string.Join('\n', result.Content.Select(block => block.ToString()));
        Assert.True(result.IsError != true, text);
        Assert.Contains("901", text);
    }
}

public sealed class LiveHotelProbe
{
    [Fact]
    public async Task Against_the_running_hotel()
    {
        if (!string.Equals(Environment.GetEnvironmentVariable("HOTELDRUID_MCP_LIVE"), "1", StringComparison.Ordinal))
            return;

        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, _, _, errors) =>
                errors == System.Net.Security.SslPolicyErrors.None
                || message.RequestUri?.Host is "localhost" or "127.0.0.1"
        };
        using var http = new HttpClient(handler) { BaseAddress = new Uri("https://localhost:5001/"), Timeout = TimeSpan.FromSeconds(60) };
        var client = new HotelClient(http);
        var discovery = new DiscoveryTools(client);
        var reservations = new ReservationTools(client);
        var channel = new ChannelTools(client);
        var finance = new FinanceTools(client);

        var known = await discovery.Search("de Vries");
        Assert.True(known.Ok, known.Summary);
        Assert.Contains("de Vries", known.Data!.ToJsonString());

        var free = await discovery.FindAvailability("2027-03-10", "2027-03-12", 1);
        Assert.True(free.Ok, free.Summary);
        var room = free.Data!["free"]![0]!["name"]!.GetValue<string>();
        var reserved = await reservations.Create("Quill", "Mara", room: room, arrival: "2027-03-10", departure: "2027-03-12", guests: 1, source: "phone", email: "mara.quill@example.com", notes: "MCP live scenario");
        Assert.True(reserved.Ok, reserved.Summary);
        var bookingId = reserved.Data!["booking"]!["id"]!.GetValue<string>();
        var paid = await finance.Pay(bookingId, 25m, "payment", "MCP live scenario card", "Mara Quill");
        Assert.True(paid.Ok, paid.Summary);
        var invoice = await finance.Issue(bookingId, "invoice");
        Assert.True(invoice.Ok, invoice.Summary);

        var later = await discovery.FindAvailability("2027-09-08", "2027-09-10", 1);
        Assert.True(later.Ok, later.Summary);
        var channelRoom = later.Data!["free"]!.AsArray().Select(item => item!["name"]!.GetValue<string>()).First(name => !string.Equals(name, room, StringComparison.OrdinalIgnoreCase));
        var request = await channel.Submit("Quill", "2027-09-08", "2027-09-10", channelRoom, "Mara", "mara.quill@example.com", 1, "booking.com", externalReference: "BDC-LIVE-1");
        Assert.True(request.Ok, request.Summary);
        var confirmed = await channel.Confirm(request.Data!["message"]!["id"]!.GetValue<string>());
        Assert.True(confirmed.Ok, confirmed.Summary);
        var channelBooking = confirmed.Data!["message"]!["bookingId"]!.GetValue<string>();
        var numbered = await reservations.Change(channelBooking, source: "booking.com", externalReference: "BDC-LIVE-1");
        Assert.True(numbered.Ok, numbered.Summary);

        var lines = new[]
        {
            "phone stay " + bookingId + " room " + room,
            "invoice " + invoice.Data!["invoiceNumber"],
            "channel stay " + channelBooking + " room " + channelRoom,
            "payment " + paid.Summary
        };
        Directory.CreateDirectory(@"c:\Users\RolfEleveld\git\BaJoMeRo\hotelDruid\artifacts");
        await File.WriteAllLinesAsync(@"c:\Users\RolfEleveld\git\BaJoMeRo\hotelDruid\artifacts\mcp-live-run.txt", lines);
    }
}
