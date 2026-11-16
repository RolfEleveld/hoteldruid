using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using HotelDruid.Api.Services;
using HotelDruid.Api.Services.ExportImport;
using HotelDruid.Api.Models;
using HotelDruid.Shared;
using HotelDruid.Shared.Configuration;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.StaticFiles;

var builder = WebApplication.CreateBuilder(args);

var dataRootFromEnvironment = Environment.GetEnvironmentVariable("HOTELDRUID_DATAROOT");
var dataRootFromConfiguration = builder.Configuration["DataRoot"];
var resolvedDataRoot = dataRootFromEnvironment
    ?? dataRootFromConfiguration
    ?? Path.Combine(Path.GetTempPath(), "hoteldruid");
var resolvedDataRootSource = dataRootFromEnvironment is not null
    ? "environment:HOTELDRUID_DATAROOT"
    : dataRootFromConfiguration is not null
        ? "configuration:DataRoot"
        : "default:TempPath/hoteldruid";

// Optional: bind Kestrel to a LocalMachine certificate when a thumbprint is provided
var certThumbprint = Environment.GetEnvironmentVariable("ASPNETCORE_Kestrel__Certificates__Default__Thumbprint");
certThumbprint = certThumbprint?.Replace(" ", "", StringComparison.OrdinalIgnoreCase);
var hasLocalCert = false;
if (!string.IsNullOrEmpty(certThumbprint))
{
    try
    {
        X509Certificate2? cert = null;
        // Try LocalMachine first
        using (var store = new X509Store(StoreName.My, StoreLocation.LocalMachine))
        {
            store.Open(OpenFlags.ReadOnly);
            cert = store.Certificates.Find(X509FindType.FindByThumbprint, certThumbprint, false).OfType<X509Certificate2>().FirstOrDefault();
            store.Close();
        }

        // If not found or lacks private key, try CurrentUser store as a fallback
        if (cert is null || !cert.HasPrivateKey)
        {
            if (cert is not null && !cert.HasPrivateKey)
            {
                Console.WriteLine($"Certificate {certThumbprint} found in LocalMachine\\My but has no private key.");
            }
            using (var store = new X509Store(StoreName.My, StoreLocation.CurrentUser))
            {
                store.Open(OpenFlags.ReadOnly);
                var certCu = store.Certificates.Find(X509FindType.FindByThumbprint, certThumbprint, false).OfType<X509Certificate2>().FirstOrDefault();
                store.Close();
                if (certCu is not null && certCu.HasPrivateKey)
                {
                    cert = certCu;
                    Console.WriteLine($"Certificate {certThumbprint} found in CurrentUser\\My and will be used.");
                }
            }
        }

        if (cert is not null && cert.HasPrivateKey)
        {
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenLocalhost(5000);                                                    // HTTP
                options.ListenLocalhost(5001, listenOptions => listenOptions.UseHttps(cert));     // HTTPS
            });
            Console.WriteLine($"Kestrel: HTTP -> http://localhost:5000   HTTPS -> https://localhost:5001");
            hasLocalCert = true;
        }
        else
        {
            Console.WriteLine($"Certificate with thumbprint {certThumbprint} not found or has no private key in LocalMachine\\My or CurrentUser\\My.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error loading certificate {certThumbprint}: {ex.Message}");
    }
}

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddLogging();
var hotelLogDirectory = Environment.GetEnvironmentVariable("HOTELDRUID_LOGDIR");
if (!string.IsNullOrWhiteSpace(hotelLogDirectory))
{
    builder.Logging.ClearProviders();
    builder.Logging.AddProvider(new HotelDruid.Api.Services.DailyFileLoggerProvider(hotelLogDirectory));
}

// Configure JSON serialization for API responses
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = null; // Use PascalCase (no transformation)
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

// Register file-based stores
builder.Services.AddSingleton<ILogger>(sp => sp.GetRequiredService<ILoggerFactory>().CreateLogger("HotelDruid.Api"));
builder.Services.AddSingleton<IKeyValueStore>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var logger = sp.GetRequiredService<ILogger<FileKeyValueStore>>();
    
    var baseStore = new FileKeyValueStore(resolvedDataRoot, logger);
    
    // Optional: wrap with caching decorator for performance
    // Enable via configuration: "CacheKeyValueStore": true or HOTELDRUID_CACHE env var
    var cacheEnvVar = Environment.GetEnvironmentVariable("HOTELDRUID_CACHE");
    var enableCache = cacheEnvVar == "true" || (cacheEnvVar == null && config.GetValue<bool>("CacheKeyValueStore", false));
    
    if (enableCache)
    {
        var cacheLogger = sp.GetRequiredService<ILogger<CachedKeyValueStoreDecorator>>();
        var maxCollections = config.GetValue<int>("CacheMaxCollections", 20);
        var cacheExpirationMinutes = config.GetValue<int>("CacheExpirationMinutes", 5);
        var versionCheckIntervalMs = config.GetValue<int>("CacheVersionCheckIntervalMs", 250);

        return new CachedKeyValueStoreDecorator(
            baseStore,
            cacheLogger,
            maxCollections,
            cacheExpirationMinutes,
                invalidationRootPath: resolvedDataRoot,
            versionCheckIntervalMilliseconds: versionCheckIntervalMs);
    }
    
    return baseStore;
});

// Register a simple file-backed store for development (legacy support)
builder.Services.AddSingleton<FileKvStore>();

// Register Phase 1B repositories
builder.Services.AddSingleton<IRoomRepository, RoomRepository>();
builder.Services.AddSingleton<ISystemConfigurationStore, SystemConfigurationStore>();
builder.Services.AddSingleton<IAccountChart, AccountChartService>();
builder.Services.AddSingleton<IAssumptionRuleEngine, AssumptionRuleEngine>();
builder.Services.AddSingleton<ITariffResolutionService, TariffResolutionService>();
builder.Services.AddSingleton<ILedgerRepository>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<LedgerRepository>>();
    return new LedgerRepository(resolvedDataRoot, logger);
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IFinancialPostingService, FinancialPostingService>();
builder.Services.AddSingleton<IBookingLedgerIndexRepository>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<BookingLedgerIndexRepository>>();
    return new BookingLedgerIndexRepository(resolvedDataRoot, logger);
});

// Register export/import services
builder.Services.AddScoped<ICanonicalMapper>(sp =>
    new CanonicalMapper(sp.GetRequiredService<ILogger<CanonicalMapper>>())
);
builder.Services.AddScoped<IPackageBuilder, PackageBuilder>();
builder.Services.AddScoped<IZipBuilder, ZipBuilder>();
builder.Services.AddScoped<IApiDistributor, ApiDistributor>();
builder.Services.AddScoped<IExportService, ExportService>();
builder.Services.AddScoped<IImportService, ImportService>();
builder.Services.AddSingleton<CacheWarmupState>();
builder.Services.AddSingleton<ICacheWarmupState>(sp => sp.GetRequiredService<CacheWarmupState>());
builder.Services.AddHostedService<CacheWarmupHostedService>();
builder.Services.AddSingleton<IDocumentRenderService, DocumentRenderService>();
builder.Services.AddSingleton<DispatchService>();
builder.Services.AddHostedService<DocumentTemplateSeeder>();
builder.Services.AddHttpClient<IIdentityFrontProbe, IdentityFrontProbe>((sp, client) =>
{
    var seconds = sp.GetRequiredService<IConfiguration>().GetValue("Identity:ProbeTimeoutSeconds", 2);
    client.Timeout = TimeSpan.FromSeconds(Math.Clamp(seconds, 1, 10));
});
builder.Services.AddSingleton<IEndpointQueryCache>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var logger = sp.GetRequiredService<ILogger<EndpointQueryCache>>();
    var enabled = config.GetValue<bool>("EndpointQueryCacheEnabled", true);
    var ttlSeconds = config.GetValue<int>("EndpointQueryCacheTtlSeconds", 30);
    return new EndpointQueryCache(logger, enabled, ttlSeconds);
});

var app = builder.Build();

var configuredMinCacheHours = builder.Configuration.GetValue<int>("StaticAssetCaching:MinHours", 8);
var configuredMaxCacheHours = builder.Configuration.GetValue<int>("StaticAssetCaching:MaxHours", 20);
configuredMinCacheHours = Math.Clamp(configuredMinCacheHours, 1, 23);
configuredMaxCacheHours = Math.Clamp(configuredMaxCacheHours, 1, 23);
if (configuredMaxCacheHours < configuredMinCacheHours)
{
    (configuredMinCacheHours, configuredMaxCacheHours) = (configuredMaxCacheHours, configuredMinCacheHours);
}

var randomizedStaticAssetCacheHours = Random.Shared.Next(configuredMinCacheHours, configuredMaxCacheHours + 1);
var randomizedStaticAssetMaxAge = TimeSpan.FromHours(randomizedStaticAssetCacheHours);
var fingerprintedAssetMaxAge = TimeSpan.FromDays(30);

void PrepareStaticResponse(StaticFileResponseContext context)
{
    var headers = context.Context.Response.Headers;
    var path = context.Context.Request.Path.Value ?? string.Empty;

    if (path.Equals("/", StringComparison.OrdinalIgnoreCase) ||
        path.EndsWith("/index.html", StringComparison.OrdinalIgnoreCase) ||
        path.Equals("/index.html", StringComparison.OrdinalIgnoreCase))
    {
        headers.CacheControl = "no-cache, no-store, must-revalidate";
        headers.Pragma = "no-cache";
        headers.Expires = "0";
        return;
    }

    var fileName = Path.GetFileName(path);
    var isFingerprintedAsset = Regex.IsMatch(fileName, @"\.[a-z0-9]{8,}\.", RegexOptions.IgnoreCase);

    if (isFingerprintedAsset)
    {
        headers.CacheControl = $"public, max-age={(int)fingerprintedAssetMaxAge.TotalSeconds}, immutable";
        return;
    }

    headers.CacheControl = $"public, max-age={(int)randomizedStaticAssetMaxAge.TotalSeconds}, must-revalidate";
}

Console.WriteLine($"Static asset browser cache max-age for non-fingerprinted assets: {randomizedStaticAssetCacheHours}h (configured range {configuredMinCacheHours}-{configuredMaxCacheHours}h)");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

if (hasLocalCert)
{
    app.UseHttpsRedirection();
}

// .NET 9/10 Blazor WASM publishes fingerprinted assets (e.g. dotnet.abc123.js).
// UseStaticFiles() cannot resolve these tokens - only MapStaticAssets() can,
// Allow static serving of .dat (ICU) and compressed assets that Blazor emits.
var staticContentTypeProvider = new FileExtensionContentTypeProvider();
staticContentTypeProvider.Mappings[".dat"] = "application/octet-stream";
staticContentTypeProvider.Mappings[".br"]  = "application/octet-stream";

var staticFileOptions = new StaticFileOptions
{
    ContentTypeProvider = staticContentTypeProvider,
    OnPrepareResponse = PrepareStaticResponse
};

const string ActorTraceContextItem = "ActorTraceId";
const string ActorDisplayContextItem = "ActorDisplayName";

static string HashTokenMaterial(string material)
{
    var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(material));
    return Convert.ToHexString(bytes).ToLowerInvariant();
}

static string ResolveTraceId(HttpContext context)
{
    var user = context.User;
    var subject = user.FindFirst("sub")?.Value;
    if (!string.IsNullOrWhiteSpace(subject))
    {
        return $"subject_{HashTokenMaterial(subject)[..24]}";
    }

    var browserFingerprint = $"{context.Connection.RemoteIpAddress}|{context.Request.Headers.UserAgent}";
    return $"browser_{HashTokenMaterial(browserFingerprint)[..24]}";
}

static string ResolveActorDisplayName(HttpContext context)
    => RequestActor.KnownName(context) ?? "admin";

app.UseDefaultFiles();
app.UseStaticFiles(staticFileOptions);

// Also serve static files from the content root. Some publish flows place the
// Blazor client files at the app content root (not under wwwroot). Serve them
// directly so index.html and _framework resources are available regardless
// of whether they were placed in the publish root or in wwwroot.
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(app.Environment.ContentRootPath),
    RequestPath = string.Empty,
    ContentTypeProvider = staticContentTypeProvider,
    OnPrepareResponse = PrepareStaticResponse
});

app.Use(async (context, next) =>
{
    if (!context.Request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase))
    {
        await next();
        return;
    }

    context.Items[ActorTraceContextItem] = ResolveTraceId(context);
    context.Items[ActorDisplayContextItem] = ResolveActorDisplayName(context);

    await next();
});

// Basic root & health endpoints for quick validation
app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }));
app.MapGet("/health/warmup", (ICacheWarmupState warmup) => Results.Ok(new
{
    warmup.IsEnabled,
    warmup.IsInProgress,
    warmup.IsCompleted,
    warmup.StartedAtUtc,
    warmup.CompletedAtUtc,
    warmup.CollectionsProcessed,
    warmup.DocumentsWarmed,
    warmup.LastError
}));

// --- Mock API endpoints for early Blazor development ---

app.MapGet("/api/status", (HttpContext httpContext) =>
{
    var buildNumber = Environment.GetEnvironmentVariable("BUILD_BUILDNUMBER")
        ?? Environment.GetEnvironmentVariable("GITHUB_RUN_NUMBER")
        ?? Environment.GetEnvironmentVariable("CI_PIPELINE_IID")
        ?? Environment.GetEnvironmentVariable("HOTELDRUID_BUILD_NUMBER")
        ?? "local";

    return Results.Ok(new
    {
        ActiveYear = DateTime.UtcNow.Year.ToString(),
        User = httpContext.Items.TryGetValue(ActorDisplayContextItem, out var actor) ? actor?.ToString() ?? "admin" : "admin",
        Authenticated = RequestActor.IsAuthenticated(httpContext),
        Version = "HotelDruid 3.0.7",
        VersionLabel = "HotelDruid 3.0.7. Blazor",
        BuildNumber = buildNumber
    });
});

app.MapGet("/api/system/configuration", async (ISystemConfigurationStore configurationStore) =>
{
    var config = await configurationStore.GetAsync();
    if (config is null)
        return Results.NotFound();

    return Results.Ok(config);
});

app.MapGet("/api/system/storage", () => Results.Ok(new
{
    DataRoot = resolvedDataRoot,
    Source = resolvedDataRootSource,
    EnvironmentVariable = "HOTELDRUID_DATAROOT",
    ConfigurationKey = "DataRoot"
}));

app.MapPut("/api/system/configuration", async (HttpContext httpContext, SystemConfiguration config, ISystemConfigurationStore configurationStore, ILedgerRepository ledger) =>
{
    if (string.IsNullOrWhiteSpace(config.Id))
        config.Id = "system";

    config.UpdatedUtc = DateTime.UtcNow;
    config.Settings ??= new Dictionary<string, string>();
    config.Settings["LastUpdatedBy"] = httpContext.Items.TryGetValue(ActorDisplayContextItem, out var actor) ? actor?.ToString() ?? "admin" : "admin";
    config.Settings["LastUpdatedTraceId"] = httpContext.Items.TryGetValue(ActorTraceContextItem, out var trace) ? trace?.ToString() ?? "unknown" : "unknown";

    if (!AccountChart.TryResolve(config.AccountingStandard, config.AccountCodes, out var chart, out var chartError))
        return Results.BadRequest(new { error = chartError });

    var previous = await configurationStore.GetAsync();
    config.AccountingStandard = chart.StandardId;
    config.AccountCodes = new Dictionary<string, string>(chart.Codes);

    await configurationStore.SaveAsync(config);
    if (!ChartAdoptionLog.AlreadyRecorded(previous, chart))
        await ChartAdoptionLog.WriteAsync(ledger, previous, chart, RequestActor.KnownName(httpContext));
    return Results.Ok(config);
});

app.MapDelete("/api/system/configuration", async (ISystemConfigurationStore configurationStore) =>
{
    await configurationStore.DeleteAsync();
    return Results.NoContent();
});

app.MapGet("/api/system/setup/status", async (
    HttpContext httpContext,
    ISystemConfigurationStore configurationStore,
    bool? configure,
    bool? adminMode) =>
{
    var config = await configurationStore.GetAsync();
    var remoteIp = httpContext.Connection.RemoteIpAddress;
    var isLocalRequest = remoteIp is null || IPAddress.IsLoopback(remoteIp);
    var isAdmin = adminMode == true || isLocalRequest;
    var configurationMissing = config is null;
    var explicitConfigure = configure == true;
    var requiresSetup = isAdmin && (configurationMissing || explicitConfigure);

    return Results.Ok(new
    {
        IsAdmin = isAdmin,
        IsLocalRequest = isLocalRequest,
        ConfigurationMissing = configurationMissing,
        ExplicitConfigureRequested = explicitConfigure,
        RequiresSetup = requiresSetup,
        Reason = explicitConfigure
            ? "explicit-configure"
            : configurationMissing
                ? "missing-configuration"
                : "configured"
    });
});

app.MapGet("/api/system/identity-front", async (IIdentityFrontProbe probe, CancellationToken cancellationToken) =>
    Results.Ok(await probe.GetStatusAsync(cancellationToken)))
    .WithName("GetIdentityFront");

// --- Bookings endpoints are defined in Layer 4 section below ---

// --- Room API endpoints ---

HashSet<string> ParseNeighborList(string? raw)
{
    var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    if (string.IsNullOrWhiteSpace(raw))
        return result;

    foreach (var value in raw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
    {
        if (!string.IsNullOrWhiteSpace(value))
            result.Add(value);
    }

    return result;
}

string? SerializeNeighborList(HashSet<string> neighbors)
{
    if (neighbors.Count == 0)
        return null;

    return string.Join(",", neighbors.OrderBy(x => x, StringComparer.OrdinalIgnoreCase));
}

async Task<string?> ValidateRoomNeighboringAsync(RoomDto request, IKeyValueStore store)
{
    if (!string.IsNullOrWhiteSpace(request.HasBeds)
        && !string.Equals(request.HasBeds, "S", StringComparison.OrdinalIgnoreCase)
        && !string.Equals(request.HasBeds, "N", StringComparison.OrdinalIgnoreCase))
    {
        return "HasBeds must be 'S', 'N', or null";
    }

    var neighbors = ParseNeighborList(request.NeighboringRooms);
    if (neighbors.Contains(request.Name) || (!string.IsNullOrWhiteSpace(request.Id) && neighbors.Contains(request.Id)))
        return "A room cannot be marked as neighboring itself";

    return null;
}

static bool SameRoomLabel(string? left, string? right)
    => string.Equals((left ?? "").Trim(), (right ?? "").Trim(), StringComparison.OrdinalIgnoreCase);

static bool TokenMatchesRoom(string token, string id, RoomStorageModel room)
    => SameRoomLabel(token, id) || SameRoomLabel(token, room.Name);

async Task<List<(string Id, RoomStorageModel Room)>> LoadStoredRoomsAsync(IKeyValueStore store)
{
    await store.UseDocumentIdsAsIndexAsync("rooms");
    var index = await store.GetIndexAsync("rooms");
    var rooms = new List<(string Id, RoomStorageModel Room)>();
    foreach (var id in index.Values.Distinct(StringComparer.OrdinalIgnoreCase))
    {
        if (string.IsNullOrWhiteSpace(id))
            continue;
        var room = await store.GetAsync<RoomStorageModel>("rooms", id);
        if (room is not null)
            rooms.Add((id, room));
    }

    return rooms;
}

RoomDto ToRoomDto(string id, RoomStorageModel room) => new(
    id, room.Name ?? "", room.Capacity ?? 0, room.FloorNumber, room.HouseNumber,
    room.Priority, room.SecondaryPriority, room.HasBeds, room.NeighboringRooms, room.Comments,
    NormalizeRoomCondition(room.Condition));

string? NormalizeRoomCondition(string? value)
{
    if (string.IsNullOrWhiteSpace(value))
        return null;
    var condition = value.Trim().ToLowerInvariant();
    return condition is "clean" or "dirty" or "blocked" or "maintenance" ? condition : null;
}

static string? BlankToNull(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

bool SameRoomPlace(RoomStorageModel room, string? name, string? house)
    => SameRoomLabel(room.Name, name) && SameRoomLabel(room.HouseNumber, house);

async Task SyncNeighborReciprocityAsync(string roomId, string? neighboringRooms, IKeyValueStore store)
{
    var rooms = await LoadStoredRoomsAsync(store);
    var self = rooms.FirstOrDefault(room => SameRoomLabel(room.Id, roomId));
    if (self.Room is null)
        return;

    var desired = ParseNeighborList(neighboringRooms);
    desired.Remove(self.Id);
    if (!string.IsNullOrWhiteSpace(self.Room.Name))
        desired.Remove(self.Room.Name);

    foreach (var other in rooms)
    {
        if (SameRoomLabel(other.Id, self.Id))
            continue;

        var current = ParseNeighborList(other.Room.NeighboringRooms);
        var shouldContain = desired.Any(token => TokenMatchesRoom(token, other.Id, other.Room));
        var contains = current.Any(token => TokenMatchesRoom(token, self.Id, self.Room));
        if (shouldContain == contains)
            continue;

        if (shouldContain)
            current.Add(self.Id);
        else
        {
            current.Remove(self.Id);
            if (!string.IsNullOrWhiteSpace(self.Room.Name))
                current.Remove(self.Room.Name);
        }

        other.Room.NeighboringRooms = SerializeNeighborList(current);
        await store.UpdateAsync("rooms", other.Id, other.Room);
    }
}

async Task RenameRoomInNeighborListsAsync(string previousRoomName, string newRoomName, IKeyValueStore store)
{
    if (SameRoomLabel(previousRoomName, newRoomName))
        return;

    foreach (var room in await LoadStoredRoomsAsync(store))
    {
        var current = ParseNeighborList(room.Room.NeighboringRooms);
        if (!current.Remove(previousRoomName))
            continue;

        current.Add(newRoomName);
        room.Room.NeighboringRooms = SerializeNeighborList(current);
        await store.UpdateAsync("rooms", room.Id, room.Room);
    }
}

async Task RemoveRoomFromNeighborListsAsync(string roomId, string roomName, IKeyValueStore store)
{
    foreach (var room in await LoadStoredRoomsAsync(store))
    {
        if (SameRoomLabel(room.Id, roomId))
            continue;

        var current = ParseNeighborList(room.Room.NeighboringRooms);
        var removed = current.Remove(roomId);
        if (!string.IsNullOrWhiteSpace(roomName))
            removed |= current.Remove(roomName);
        if (!removed)
            continue;

        room.Room.NeighboringRooms = SerializeNeighborList(current);
        await store.UpdateAsync("rooms", room.Id, room.Room);
    }
}

app.MapPost("/api/rooms", async (RoomDto request, IKeyValueStore store, IEndpointQueryCache queryCache) =>
{
    if (string.IsNullOrWhiteSpace(request.Name))
        return Results.BadRequest(new { error = "Room name (ID) is required" });
    if (request.Capacity <= 0)
        return Results.BadRequest(new { error = "Capacity must be greater than 0" });

    var neighboringValidation = await ValidateRoomNeighboringAsync(request, store);
    if (neighboringValidation is not null)
        return Results.BadRequest(new { error = neighboringValidation });

    try
    {
        var normalizedNeighbors = SerializeNeighborList(ParseNeighborList(request.NeighboringRooms));
        var storage = new RoomStorageModel
        {
            Name = request.Name,
            Capacity = request.Capacity,
            FloorNumber = request.FloorNumber,
            HouseNumber = request.HouseNumber,
            Priority = request.Priority,
            SecondaryPriority = request.SecondaryPriority,
            HasBeds = request.HasBeds,
            NeighboringRooms = normalizedNeighbors,
            Comments = request.Comments,
            Condition = NormalizeRoomCondition(request.Condition)
        };

        var existing = await LoadStoredRoomsAsync(store);
        if (existing.Any(room => SameRoomPlace(room.Room, request.Name, request.HouseNumber)))
            return Results.Conflict(new { error = "A room with this name already exists in this building" });

        var id = await store.CreateAsync("rooms", request.Name, storage, indexByDocumentId: true);
        await SyncNeighborReciprocityAsync(id, storage.NeighboringRooms, store);
        queryCache.InvalidateTag("rooms");
        return Results.Created($"/api/rooms/{id}", new RoomDto(
            id, request.Name, request.Capacity, request.FloorNumber, request.HouseNumber,
            request.Priority, request.SecondaryPriority, request.HasBeds, normalizedNeighbors, request.Comments,
            NormalizeRoomCondition(request.Condition)));
    }
    catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
    {
        return Results.Conflict(new { error = ex.Message });
    }
});

app.MapGet("/api/rooms/{id}", async (string id, IKeyValueStore store) =>
{
    var room = await store.GetAsync<RoomStorageModel>("rooms", id);
    if (room is null)
        return Results.NotFound();

    return Results.Ok(ToRoomDto(id, room));
});

app.MapGet("/api/rooms", async (IKeyValueStore store, IEndpointQueryCache queryCache, string? name) =>
{
    if (!string.IsNullOrEmpty(name))
    {
        var roomDto = await queryCache.GetOrCreateAsync($"rooms:name:{name}", new[] { "rooms" }, async () =>
        {
            var matches = (await LoadStoredRoomsAsync(store))
                .Where(room => SameRoomLabel(room.Room.Name, name))
                .Select(room => ToRoomDto(room.Id, room.Room))
                .ToList();
            return matches.Count == 0 ? null : matches[0];
        });

        if (roomDto is null)
            return Results.NotFound();

        return Results.Ok(roomDto);
    }

    var result = await queryCache.GetOrCreateAsync($"rooms:list", new[] { "rooms" }, async () =>
    {
        var rooms = await LoadStoredRoomsAsync(store);
        return rooms.Select(room => ToRoomDto(room.Id, room.Room)).ToList();
    });

    return Results.Ok(result);
});

app.MapPut("/api/rooms/{id}", async (string id, RoomDto request, IKeyValueStore store, IEndpointQueryCache queryCache) =>
{
    var existingRoom = await store.GetAsync<RoomStorageModel>("rooms", id);
    if (existingRoom is null)
        return Results.NotFound();
    
    if (string.IsNullOrWhiteSpace(request.Name))
        return Results.BadRequest(new { error = "Room name is required" });
    if (request.Capacity <= 0)
        return Results.BadRequest(new { error = "Capacity must be greater than 0" });

    var neighboringValidation = await ValidateRoomNeighboringAsync(request, store);
    if (neighboringValidation is not null)
        return Results.BadRequest(new { error = neighboringValidation });

    var previousRoomName = existingRoom.Name ?? request.Name;
    var others = await LoadStoredRoomsAsync(store);
    if (others.Any(room => !SameRoomLabel(room.Id, id) && SameRoomPlace(room.Room, request.Name, request.HouseNumber)))
        return Results.Conflict(new { error = "A room with this name already exists in this building" });

    var normalizedNeighbors = SerializeNeighborList(ParseNeighborList(request.NeighboringRooms));

    var storage = new RoomStorageModel
    {
        Name = request.Name,
        Capacity = request.Capacity,
        FloorNumber = request.FloorNumber,
        HouseNumber = request.HouseNumber,
        Priority = request.Priority,
        SecondaryPriority = request.SecondaryPriority,
        HasBeds = request.HasBeds,
        NeighboringRooms = normalizedNeighbors,
        Comments = request.Comments,
        Condition = NormalizeRoomCondition(request.Condition)
    };

    await store.UpdateAsync("rooms", id, storage);
    if (!string.Equals(previousRoomName, request.Name, StringComparison.OrdinalIgnoreCase))
        await RenameRoomInNeighborListsAsync(previousRoomName, request.Name, store);
    await SyncNeighborReciprocityAsync(id, normalizedNeighbors, store);
    queryCache.InvalidateTag("rooms");
    return Results.Ok(new RoomDto(
        id, request.Name, request.Capacity, request.FloorNumber, request.HouseNumber,
        request.Priority, request.SecondaryPriority, request.HasBeds, normalizedNeighbors, request.Comments,
        NormalizeRoomCondition(request.Condition)));
});

app.MapDelete("/api/rooms/{id}", async (string id, IKeyValueStore store, IEndpointQueryCache queryCache) =>
{
    var room = await store.GetAsync<RoomStorageModel>("rooms", id);
    if (room is null)
        return Results.NotFound();

    var roomName = room.Name ?? string.Empty;
    var today = DateOnly.FromDateTime(DateTime.UtcNow);
    var bookings = await store.ListAsync<BookingStorageModel>("bookings");
    var hasFutureBookings = bookings.Any(b =>
        !string.IsNullOrWhiteSpace(b.RoomId)
        && (string.Equals(b.RoomId, id, StringComparison.OrdinalIgnoreCase)
            || (!string.IsNullOrWhiteSpace(roomName) && string.Equals(b.RoomId, roomName, StringComparison.OrdinalIgnoreCase)))
        && (!b.DepartureDate.HasValue || b.DepartureDate.Value >= today));

    if (hasFutureBookings)
        return Results.Conflict(new { error = "Room contains future bookings and cannot be deleted" });

    var inventoryIndex = await store.GetIndexAsync("inventory");
    foreach (var inventoryEntry in inventoryIndex)
    {
        var inventoryItem = await store.GetAsync<InventoryStorageModel>("inventory", inventoryEntry.Value);
        if (inventoryItem is null || string.IsNullOrWhiteSpace(inventoryItem.RoomId))
            continue;

        if (string.Equals(inventoryItem.RoomId, id, StringComparison.OrdinalIgnoreCase)
            || (!string.IsNullOrWhiteSpace(roomName) && string.Equals(inventoryItem.RoomId, roomName, StringComparison.OrdinalIgnoreCase)))
        {
            await store.DeleteAsync("inventory", inventoryEntry.Value);
        }
    }

    await store.DeleteAsync("rooms", id);
    if (!string.IsNullOrWhiteSpace(roomName))
        await RemoveRoomFromNeighborListsAsync(id, roomName, store);
    queryCache.InvalidateTag("rooms");
    queryCache.InvalidateTag("bookings");
    queryCache.InvalidateTag("inventory");
    return Results.NoContent();
});

// --- Assets API (parallel to Rooms) ---

app.MapPost("/api/assets", async (AssetDto request, IKeyValueStore store) =>
{
    if (string.IsNullOrWhiteSpace(request.Name))
        return Results.BadRequest(new { error = "Asset name is required" });

    try
    {
        var storage = new AssetStorageModel
        {
            Name = request.Name,
            Code = request.Code,
            Description = request.Description,
            CreatedAt = request.CreatedAt ?? DateTime.UtcNow
        };

        var id = await store.CreateAsync("assets", request.Name, storage);
        return Results.Created($"/api/assets/{id}", new AssetDto(id, request.Name, request.Code, request.Description, storage.CreatedAt));
    }
    catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
    {
        return Results.Conflict(new { error = ex.Message });
    }
}).WithName("CreateAsset");

app.MapGet("/api/assets/{id}", async (string id, IKeyValueStore store) =>
{
    var asset = await store.GetAsync<AssetStorageModel>("assets", id);
    if (asset is null)
        return Results.NotFound();

    return Results.Ok(new AssetDto(id, asset.Name ?? "", asset.Code, asset.Description, asset.CreatedAt));
}).WithName("GetAsset");

app.MapGet("/api/assets", async (IKeyValueStore store, string? name) =>
{
    if (!string.IsNullOrEmpty(name))
    {
        var asset = await store.GetByNameAsync<AssetStorageModel>("assets", name);
        if (asset is null)
            return Results.NotFound();

        var index = await store.GetIndexAsync("assets");
        var id = index.TryGetValue(name, out var assetId) ? assetId : "";
        return Results.Ok(new AssetDto(id, asset.Name ?? "", asset.Code, asset.Description, asset.CreatedAt));
    }

    var assets = await store.ListAsync<AssetStorageModel>("assets");
    var index2 = await store.GetIndexAsync("assets");

    var result = new List<AssetDto>();
    foreach (var a in assets)
    {
        var assetName = a.Name ?? "";
        var assetId = index2.TryGetValue(assetName, out var id) ? id : "";
        result.Add(new AssetDto(assetId, a.Name ?? "", a.Code, a.Description, a.CreatedAt));
    }

    return Results.Ok(result);
}).WithName("ListAssets");

app.MapPut("/api/assets/{id}", async (string id, AssetDto request, IKeyValueStore store) =>
{
    var exists = await store.ExistsAsync("assets", id);
    if (!exists)
        return Results.NotFound();

    if (string.IsNullOrWhiteSpace(request.Name))
        return Results.BadRequest(new { error = "Asset name is required" });

    var storage = new AssetStorageModel
    {
        Name = request.Name,
        Code = request.Code,
        Description = request.Description,
        CreatedAt = request.CreatedAt ?? DateTime.UtcNow
    };

    await store.UpdateAsync("assets", id, storage);
    return Results.Ok(new AssetDto(id, request.Name, request.Code, request.Description, storage.CreatedAt));
}).WithName("UpdateAsset");

app.MapDelete("/api/assets/{id}", async (string id, IKeyValueStore store) =>
{
    var exists = await store.ExistsAsync("assets", id);
    if (!exists)
        return Results.NotFound();

    await store.DeleteAsync("assets", id);
    return Results.NoContent();
}).WithName("DeleteAsset");

// --- Warehouses (magazzini) ---

app.MapPost("/api/warehouses", async (WarehouseDto request, IKeyValueStore store) =>
{
    if (string.IsNullOrWhiteSpace(request.Name))
        return Results.BadRequest(new { error = "Warehouse name is required" });

    var storage = new WarehouseStorageModel
    {
        Name = request.Name,
        Code = request.Code,
        Description = request.Description,
        FloorNumber = request.FloorNumber,
        HouseNumber = request.HouseNumber,
        CreatedAt = request.CreatedAt ?? DateTime.UtcNow
    };

    var id = await store.CreateAsync("warehouses", request.Name, storage);
    return Results.Created($"/api/warehouses/{id}", new WarehouseDto(id, request.Name, request.Code, request.Description, request.FloorNumber, request.HouseNumber, storage.CreatedAt));
}).WithName("CreateWarehouse");

app.MapGet("/api/warehouses/{id}", async (string id, IKeyValueStore store) =>
{
    var w = await store.GetAsync<WarehouseStorageModel>("warehouses", id);
    if (w is null) return Results.NotFound();
    return Results.Ok(new WarehouseDto(id, w.Name, w.Code, w.Description, w.FloorNumber, w.HouseNumber, w.CreatedAt));
}).WithName("GetWarehouse");

app.MapGet("/api/warehouses", async (IKeyValueStore store, string? name) =>
{
    if (!string.IsNullOrEmpty(name))
    {
        var w = await store.GetByNameAsync<WarehouseStorageModel>("warehouses", name);
        if (w is null) return Results.NotFound();
        var index = await store.GetIndexAsync("warehouses");
        var id = index.TryGetValue(name, out var wid) ? wid : "";
        return Results.Ok(new WarehouseDto(id, w.Name, w.Code, w.Description, w.FloorNumber, w.HouseNumber, w.CreatedAt));
    }

    var all = await store.ListAsync<WarehouseStorageModel>("warehouses");
    var idx = await store.GetIndexAsync("warehouses");
    var list = new List<WarehouseDto>();
    foreach (var w in all)
    {
        var name2 = w.Name ?? "";
        var id2 = idx.TryGetValue(name2, out var wid) ? wid : "";
        list.Add(new WarehouseDto(id2, w.Name, w.Code, w.Description, w.FloorNumber, w.HouseNumber, w.CreatedAt));
    }
    return Results.Ok(list);
}).WithName("ListWarehouses");

app.MapPut("/api/warehouses/{id}", async (string id, WarehouseDto request, IKeyValueStore store) =>
{
    var exists = await store.ExistsAsync("warehouses", id);
    if (!exists) return Results.NotFound();
    if (string.IsNullOrWhiteSpace(request.Name)) return Results.BadRequest(new { error = "Warehouse name is required" });

    var storage = new WarehouseStorageModel
    {
        Name = request.Name,
        Code = request.Code,
        Description = request.Description,
        FloorNumber = request.FloorNumber,
        HouseNumber = request.HouseNumber,
        CreatedAt = request.CreatedAt ?? DateTime.UtcNow
    };

    await store.UpdateAsync("warehouses", id, storage);
    return Results.Ok(new WarehouseDto(id, request.Name, request.Code, request.Description, request.FloorNumber, request.HouseNumber, storage.CreatedAt));
}).WithName("UpdateWarehouse");

app.MapDelete("/api/warehouses/{id}", async (string id, IKeyValueStore store) =>
{
    var exists = await store.ExistsAsync("warehouses", id);
    if (!exists) return Results.NotFound();
    await store.DeleteAsync("warehouses", id);
    return Results.NoContent();
}).WithName("DeleteWarehouse");

// --- Inventory relations (relinventario) ---

app.MapPost("/api/inventory", async (InventoryDto request, IKeyValueStore store) =>
{
    if (string.IsNullOrWhiteSpace(request.AssetId))
        return Results.BadRequest(new { error = "AssetId is required" });
    if (string.IsNullOrWhiteSpace(request.RoomId) && string.IsNullOrWhiteSpace(request.WarehouseId))
        return Results.BadRequest(new { error = "Either RoomId or WarehouseId must be provided" });

    var storage = new InventoryStorageModel
    {
        AssetId = request.AssetId,
        RoomId = request.RoomId,
        WarehouseId = request.WarehouseId,
        Quantity = request.Quantity,
        MinQuantityDefault = request.MinQuantityDefault,
        RequiredOnCheckin = request.RequiredOnCheckin,
        CreatedAt = request.CreatedAt ?? DateTime.UtcNow
    };

    var id = await store.CreateAsync("inventory", Guid.NewGuid().ToString("N"), storage);
    return Results.Created($"/api/inventory/{id}", new InventoryDto(id, storage.AssetId, storage.RoomId, storage.WarehouseId, storage.Quantity, storage.MinQuantityDefault, storage.RequiredOnCheckin, storage.CreatedAt));
}).WithName("CreateInventory");

app.MapGet("/api/inventory/{id}", async (string id, IKeyValueStore store) =>
{
    var inv = await store.GetAsync<InventoryStorageModel>("inventory", id);
    if (inv is null) return Results.NotFound();
    return Results.Ok(new InventoryDto(id, inv.AssetId, inv.RoomId, inv.WarehouseId, inv.Quantity, inv.MinQuantityDefault, inv.RequiredOnCheckin, inv.CreatedAt));
}).WithName("GetInventory");

app.MapGet("/api/inventory", async (IKeyValueStore store, string? assetId, string? roomId, string? warehouseId) =>
{
    // Inventory rows must be materialized by entry id because storage does not embed id in payload.
    var idx = await store.GetIndexAsync("inventory");
    var list = new List<InventoryDto>();

    foreach (var entry in idx)
    {
        var id = entry.Value;
        var item = await store.GetAsync<InventoryStorageModel>("inventory", id);
        if (item is null)
            continue;

        if (!string.IsNullOrEmpty(assetId) && !string.Equals(item.AssetId, assetId, StringComparison.Ordinal))
            continue;
        if (!string.IsNullOrEmpty(roomId) && !string.Equals(item.RoomId, roomId, StringComparison.Ordinal))
            continue;
        if (!string.IsNullOrEmpty(warehouseId) && !string.Equals(item.WarehouseId, warehouseId, StringComparison.Ordinal))
            continue;

        list.Add(new InventoryDto(id, item.AssetId, item.RoomId, item.WarehouseId, item.Quantity, item.MinQuantityDefault, item.RequiredOnCheckin, item.CreatedAt));
    }

    return Results.Ok(list);
}).WithName("ListInventory");

app.MapPut("/api/inventory/{id}", async (string id, InventoryDto request, IKeyValueStore store) =>
{
    var exists = await store.ExistsAsync("inventory", id);
    if (!exists) return Results.NotFound();

    var storage = new InventoryStorageModel
    {
        AssetId = request.AssetId,
        RoomId = request.RoomId,
        WarehouseId = request.WarehouseId,
        Quantity = request.Quantity,
        MinQuantityDefault = request.MinQuantityDefault,
        RequiredOnCheckin = request.RequiredOnCheckin,
        CreatedAt = request.CreatedAt ?? DateTime.UtcNow
    };

    await store.UpdateAsync("inventory", id, storage);
    return Results.Ok(new InventoryDto(id, storage.AssetId, storage.RoomId, storage.WarehouseId, storage.Quantity, storage.MinQuantityDefault, storage.RequiredOnCheckin, storage.CreatedAt));
}).WithName("UpdateInventory");

app.MapDelete("/api/inventory/{id}", async (string id, IKeyValueStore store) =>
{
    var exists = await store.ExistsAsync("inventory", id);
    if (!exists) return Results.NotFound();
    await store.DeleteAsync("inventory", id);
    return Results.NoContent();
}).WithName("DeleteInventory");

// --- Room Tariffs (rate plans: rack rate, weekend, week, month, seasonal, etc.) ---

TariffDto ToTariffDto(string id, TariffStorageModel s) => new(
    id, s.Name ?? "", s.Description, s.ApplicabilityMode ?? "AnyDay", s.WeekdaysMask, s.MinNights,
    s.PricePerNight, s.PricePerPerson, s.Priority, s.IsActive, s.TargetType ?? "Room", s.CreatedAt, s.MinGuestCount);

app.MapPost("/api/tariffs", async (TariffDto request, IKeyValueStore store) =>
{
    if (string.IsNullOrWhiteSpace(request.Name))
        return Results.BadRequest(new { error = "Tariff name is required" });

    var storage = new TariffStorageModel
    {
        Name = request.Name,
        Description = request.Description,
        ApplicabilityMode = string.IsNullOrWhiteSpace(request.ApplicabilityMode) ? "AnyDay" : request.ApplicabilityMode,
        WeekdaysMask = request.WeekdaysMask,
        MinNights = request.MinNights,
        MinGuestCount = request.MinGuestCount,
        PricePerNight = request.PricePerNight,
        PricePerPerson = request.PricePerPerson,
        Priority = request.Priority,
        IsActive = request.IsActive,
        TargetType = string.IsNullOrWhiteSpace(request.TargetType) ? "Room" : request.TargetType,
        CreatedAt = request.CreatedAt ?? DateTime.UtcNow
    };

    var id = await store.CreateAsync("tariffs", Guid.NewGuid().ToString("N"), storage);
    return Results.Created($"/api/tariffs/{id}", ToTariffDto(id, storage));
}).WithName("CreateTariff");

app.MapGet("/api/tariffs/{id}", async (string id, IKeyValueStore store) =>
{
    var t = await store.GetAsync<TariffStorageModel>("tariffs", id);
    if (t is null) return Results.NotFound();
    return Results.Ok(ToTariffDto(id, t));
}).WithName("GetTariff");

app.MapGet("/api/tariffs", async (IKeyValueStore store) =>
{
    var all = await store.ListAsync<TariffStorageModel>("tariffs");
    var idx = await store.GetIndexAsync("tariffs");
    var list = new List<TariffDto>();
    foreach (var kvp in idx)
    {
        var t = await store.GetAsync<TariffStorageModel>("tariffs", kvp.Value);
        if (t is not null) list.Add(ToTariffDto(kvp.Value, t));
    }
    return Results.Ok(list.OrderByDescending(t => t.Priority).ThenBy(t => t.Name, StringComparer.OrdinalIgnoreCase));
}).WithName("ListTariffs");

app.MapPut("/api/tariffs/{id}", async (string id, TariffDto request, IKeyValueStore store) =>
{
    var exists = await store.ExistsAsync("tariffs", id);
    if (!exists) return Results.NotFound();
    if (string.IsNullOrWhiteSpace(request.Name)) return Results.BadRequest(new { error = "Tariff name is required" });

    var storage = new TariffStorageModel
    {
        Name = request.Name,
        Description = request.Description,
        ApplicabilityMode = string.IsNullOrWhiteSpace(request.ApplicabilityMode) ? "AnyDay" : request.ApplicabilityMode,
        WeekdaysMask = request.WeekdaysMask,
        MinNights = request.MinNights,
        MinGuestCount = request.MinGuestCount,
        PricePerNight = request.PricePerNight,
        PricePerPerson = request.PricePerPerson,
        Priority = request.Priority,
        IsActive = request.IsActive,
        TargetType = string.IsNullOrWhiteSpace(request.TargetType) ? "Room" : request.TargetType,
        CreatedAt = request.CreatedAt ?? DateTime.UtcNow
    };

    await store.UpdateAsync("tariffs", id, storage);
    return Results.Ok(ToTariffDto(id, storage));
}).WithName("UpdateTariff");

app.MapDelete("/api/tariffs/{id}", async (string id, IKeyValueStore store) =>
{
    var exists = await store.ExistsAsync("tariffs", id);
    if (!exists) return Results.NotFound();
    await store.DeleteAsync("tariffs", id);

    // Clean up assignments and seasons that reference this tariff.
    var assignmentIdx = await store.GetIndexAsync("tariff_assignments");
    foreach (var kvp in assignmentIdx)
    {
        var a = await store.GetAsync<TariffAssignmentStorageModel>("tariff_assignments", kvp.Value);
        if (a is not null && string.Equals(a.TariffId, id, StringComparison.OrdinalIgnoreCase))
        {
            await store.DeleteAsync("tariff_assignments", kvp.Value);
        }
    }
    var seasonIdx = await store.GetIndexAsync("tariff_seasons");
    foreach (var kvp in seasonIdx)
    {
        var s = await store.GetAsync<TariffSeasonStorageModel>("tariff_seasons", kvp.Value);
        if (s is not null && string.Equals(s.TariffId, id, StringComparison.OrdinalIgnoreCase))
        {
            await store.DeleteAsync("tariff_seasons", kvp.Value);
        }
    }

    return Results.NoContent();
}).WithName("DeleteTariff");

// --- Tariff Assignments (many-to-many: a tariff can apply to many rooms, a room can have many tariffs) ---

TariffAssignmentDto ToTariffAssignmentDto(string id, TariffAssignmentStorageModel s) => new(
    id, s.TariffId, s.TargetType ?? "Room", s.TargetId, s.CreatedAt);

app.MapPost("/api/tariff-assignments", async (TariffAssignmentDto request, IKeyValueStore store) =>
{
    if (string.IsNullOrWhiteSpace(request.TariffId) || string.IsNullOrWhiteSpace(request.TargetId))
        return Results.BadRequest(new { error = "TariffId and TargetId are required" });
    var targetType = string.IsNullOrWhiteSpace(request.TargetType) ? "Room" : request.TargetType;

    var storage = new TariffAssignmentStorageModel
    {
        TariffId = request.TariffId,
        TargetType = targetType,
        TargetId = request.TargetId,
        CreatedAt = request.CreatedAt ?? DateTime.UtcNow
    };

    var id = await store.CreateAsync("tariff_assignments", Guid.NewGuid().ToString("N"), storage);
    return Results.Created($"/api/tariff-assignments/{id}", ToTariffAssignmentDto(id, storage));
}).WithName("CreateTariffAssignment");

app.MapGet("/api/tariff-assignments", async (IKeyValueStore store, string? tariffId, string? targetId) =>
{
    var idx = await store.GetIndexAsync("tariff_assignments");
    var list = new List<TariffAssignmentDto>();
    foreach (var kvp in idx)
    {
        var a = await store.GetAsync<TariffAssignmentStorageModel>("tariff_assignments", kvp.Value);
        if (a is null) continue;
        if (tariffId is not null && !string.Equals(a.TariffId, tariffId, StringComparison.OrdinalIgnoreCase)) continue;
        if (targetId is not null && !string.Equals(a.TargetId, targetId, StringComparison.OrdinalIgnoreCase)) continue;
        list.Add(ToTariffAssignmentDto(kvp.Value, a));
    }
    return Results.Ok(list);
}).WithName("ListTariffAssignments");

app.MapDelete("/api/tariff-assignments/{id}", async (string id, IKeyValueStore store) =>
{
    var exists = await store.ExistsAsync("tariff_assignments", id);
    if (!exists) return Results.NotFound();
    await store.DeleteAsync("tariff_assignments", id);
    return Results.NoContent();
}).WithName("DeleteTariffAssignment");

// --- Tariff Seasons (date-range price adjustments layered on top of a tariff's base price) ---

TariffSeasonDto ToTariffSeasonDto(string id, TariffSeasonStorageModel s) => new(
    id, s.TariffId, s.Name, s.StartDate, s.EndDate, s.AdjustmentType ?? "PercentageAdjustment", s.Value, s.CreatedAt);

app.MapPost("/api/tariff-seasons", async (TariffSeasonDto request, IKeyValueStore store) =>
{
    if (string.IsNullOrWhiteSpace(request.TariffId))
        return Results.BadRequest(new { error = "TariffId is required" });

    var storage = new TariffSeasonStorageModel
    {
        TariffId = request.TariffId,
        Name = request.Name,
        StartDate = request.StartDate,
        EndDate = request.EndDate,
        AdjustmentType = string.IsNullOrWhiteSpace(request.AdjustmentType) ? "PercentageAdjustment" : request.AdjustmentType,
        Value = request.Value,
        CreatedAt = request.CreatedAt ?? DateTime.UtcNow
    };

    var id = await store.CreateAsync("tariff_seasons", Guid.NewGuid().ToString("N"), storage);
    return Results.Created($"/api/tariff-seasons/{id}", ToTariffSeasonDto(id, storage));
}).WithName("CreateTariffSeason");

app.MapGet("/api/tariff-seasons", async (IKeyValueStore store, string? tariffId) =>
{
    var idx = await store.GetIndexAsync("tariff_seasons");
    var list = new List<TariffSeasonDto>();
    foreach (var kvp in idx)
    {
        var s = await store.GetAsync<TariffSeasonStorageModel>("tariff_seasons", kvp.Value);
        if (s is null) continue;
        if (tariffId is not null && !string.Equals(s.TariffId, tariffId, StringComparison.OrdinalIgnoreCase)) continue;
        list.Add(ToTariffSeasonDto(kvp.Value, s));
    }
    return Results.Ok(list.OrderBy(s => s.StartDate));
}).WithName("ListTariffSeasons");

app.MapPut("/api/tariff-seasons/{id}", async (string id, TariffSeasonDto request, IKeyValueStore store) =>
{
    var exists = await store.ExistsAsync("tariff_seasons", id);
    if (!exists) return Results.NotFound();

    var storage = new TariffSeasonStorageModel
    {
        TariffId = request.TariffId,
        Name = request.Name,
        StartDate = request.StartDate,
        EndDate = request.EndDate,
        AdjustmentType = string.IsNullOrWhiteSpace(request.AdjustmentType) ? "PercentageAdjustment" : request.AdjustmentType,
        Value = request.Value,
        CreatedAt = request.CreatedAt ?? DateTime.UtcNow
    };

    await store.UpdateAsync("tariff_seasons", id, storage);
    return Results.Ok(ToTariffSeasonDto(id, storage));
}).WithName("UpdateTariffSeason");

app.MapDelete("/api/tariff-seasons/{id}", async (string id, IKeyValueStore store) =>
{
    var exists = await store.ExistsAsync("tariff_seasons", id);
    if (!exists) return Results.NotFound();
    await store.DeleteAsync("tariff_seasons", id);
    return Results.NoContent();
}).WithName("DeleteTariffSeason");

app.MapGet("/api/tariffs/resolve", async (string roomId, DateOnly arrivalDate, DateOnly departureDate, int? guestCount, ITariffResolutionService resolver) =>
{
    var result = await resolver.ResolveAsync(roomId, arrivalDate, departureDate, guestCount ?? 1);
    return result is null ? Results.NotFound() : Results.Ok(result);
}).WithName("ResolveTariff");



app.MapPost("/api/export/create", async (ExportRequest? request, IExportService exportService) =>
{
    request ??= new ExportRequest();
    var exportId = await exportService.CreateExportPackageAsync(request);
    var status = await exportService.GetExportStatusAsync(exportId);
    return Results.Ok(new ExportResponse(
        ExportId: exportId,
        Status: status.Status,
        StatusUrl: $"/api/export/{exportId}/status",
        EstimatedSeconds: 5
    ));
})
.WithName("CreateExport")
;

app.MapGet("/api/export/{exportId}/status", async (string exportId, IExportService exportService) =>
{
    try
    {
        var status = await exportService.GetExportStatusAsync(exportId);
        return Results.Ok(status);
    }
    catch (KeyNotFoundException)
    {
        return Results.NotFound(new { error = $"Export {exportId} not found" });
    }
})
.WithName("GetExportStatus")
;

app.MapGet("/api/export/{exportId}/download", async (string exportId, IExportService exportService) =>
{
    try
    {
        var (stream, fileName) = await exportService.DownloadExportAsync(exportId);
        return Results.File(stream, "application/zip", fileName);
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("DownloadExport")
;

app.MapGet("/api/export/list", async (IExportService exportService, int limit = 20, int offset = 0) =>
{
    var exports = await exportService.ListExportsAsync(limit, offset);
    return Results.Ok(new { exports = exports, total = exports.Count });
})
.WithName("ListExports")
;

app.MapPost("/api/import/validate", async (IFormFile file, IImportService importService) =>
{
    if (file == null || file.Length == 0)
        return Results.BadRequest(new { error = "File is required" });

    try
    {
        var validation = await importService.ValidatePackageAsync(file);
        return Results.Ok(validation);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.WithName("ValidateImport")
.DisableAntiforgery()
;

app.MapGet("/api/import/{packageId}/preview", async (string packageId, IImportService importService) =>
{
    try
    {
        var preview = await importService.GetImportPreviewAsync(packageId);
        return Results.Ok(preview);
    }
    catch (KeyNotFoundException)
    {
        return Results.NotFound(new { error = $"Package {packageId} not found" });
    }
})
.WithName("GetImportPreview")
;

app.MapPost("/api/import/{packageId}/execute", async (string packageId, ImportExecuteRequest request, IImportService importService) =>
{
    try
    {
        var result = await importService.ExecuteImportAsync(packageId, request);
        return Results.Ok(result);
    }
    catch (KeyNotFoundException)
    {
        return Results.NotFound(new { error = $"Package {packageId} not found" });
    }
})
.WithName("ExecuteImport")
;

app.MapGet("/api/import/{importId}/status", async (string importId, IImportService importService) =>
{
    try
    {
        var status = await importService.GetImportStatusAsync(importId);
        return Results.Ok(status);
    }
    catch (KeyNotFoundException)
    {
        return Results.NotFound(new { error = $"Import {importId} not found" });
    }
})
.WithName("GetImportStatus")
;

// --- Cash Registers (casse) ---

app.MapPost("/api/cash-registers", async (CashRegisterDto request, IKeyValueStore store) =>
{
    if (string.IsNullOrWhiteSpace(request.Name))
        return Results.BadRequest(new { error = "Cash register name is required" });

    try
    {
        var storage = new CashRegisterStorageModel
        {
            Name = request.Name,
            Status = request.Status ?? "open",
            Code = request.Code,
            Description = request.Description,
            CreatedAt = request.CreatedAt ?? DateTime.UtcNow
        };
        var id = await store.CreateAsync("cash_registers", request.Name, storage);
        return Results.Created($"/api/cash-registers/{id}", new CashRegisterDto(id, storage.Name, storage.Status, storage.Code, storage.Description, storage.CreatedAt));
    }
    catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
    {
        return Results.Conflict(new { error = ex.Message });
    }
}).WithName("CreateCashRegister");

app.MapGet("/api/cash-registers/{id}", async (string id, IKeyValueStore store) =>
{
    var cr = await store.GetAsync<CashRegisterStorageModel>("cash_registers", id);
    if (cr is null) return Results.NotFound();
    return Results.Ok(new CashRegisterDto(id, cr.Name, cr.Status, cr.Code, cr.Description, cr.CreatedAt));
}).WithName("GetCashRegister");

app.MapGet("/api/cash-registers", async (IKeyValueStore store, string? name) =>
{
    if (!string.IsNullOrEmpty(name))
    {
        var cr = await store.GetByNameAsync<CashRegisterStorageModel>("cash_registers", name);
        if (cr is null) return Results.NotFound();
        var index = await store.GetIndexAsync("cash_registers");
        var id = index.TryGetValue(name, out var crid) ? crid : "";
        return Results.Ok(new CashRegisterDto(id, cr.Name, cr.Status, cr.Code, cr.Description, cr.CreatedAt));
    }
    var all = await store.ListAsync<CashRegisterStorageModel>("cash_registers");
    var idx = await store.GetIndexAsync("cash_registers");
    var list = new List<CashRegisterDto>();
    foreach (var cr in all)
    {
        var crName = cr.Name ?? "";
        var crid2 = idx.TryGetValue(crName, out var i) ? i : "";
        list.Add(new CashRegisterDto(crid2, cr.Name, cr.Status, cr.Code, cr.Description, cr.CreatedAt));
    }
    return Results.Ok(list);
}).WithName("ListCashRegisters");

app.MapPut("/api/cash-registers/{id}", async (string id, CashRegisterDto request, IKeyValueStore store) =>
{
    var exists = await store.ExistsAsync("cash_registers", id);
    if (!exists) return Results.NotFound();
    if (string.IsNullOrWhiteSpace(request.Name)) return Results.BadRequest(new { error = "Cash register name is required" });

    var storage = new CashRegisterStorageModel
    {
        Name = request.Name,
        Status = request.Status ?? "open",
        Code = request.Code,
        Description = request.Description,
        CreatedAt = request.CreatedAt ?? DateTime.UtcNow
    };
    await store.UpdateAsync("cash_registers", id, storage);
    return Results.Ok(new CashRegisterDto(id, storage.Name, storage.Status, storage.Code, storage.Description, storage.CreatedAt));
}).WithName("UpdateCashRegister");

app.MapDelete("/api/cash-registers/{id}", async (string id, IKeyValueStore store) =>
{
    var exists = await store.ExistsAsync("cash_registers", id);
    if (!exists) return Results.NotFound();
    await store.DeleteAsync("cash_registers", id);
    return Results.NoContent();
}).WithName("DeleteCashRegister");

// --- User Groups (gruppi) ---

app.MapPost("/api/user-groups", async (UserGroupDto request, IKeyValueStore store) =>
{
    if (string.IsNullOrWhiteSpace(request.Name))
        return Results.BadRequest(new { error = "User group name is required" });

    try
    {
        var storage = new UserGroupStorageModel { Name = request.Name };
        var id = await store.CreateAsync("user_groups", request.Name, storage);
        return Results.Created($"/api/user-groups/{id}", new UserGroupDto(id, storage.Name));
    }
    catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
    {
        return Results.Conflict(new { error = ex.Message });
    }
}).WithName("CreateUserGroup");

app.MapGet("/api/user-groups/{id}", async (string id, IKeyValueStore store) =>
{
    var ug = await store.GetAsync<UserGroupStorageModel>("user_groups", id);
    if (ug is null) return Results.NotFound();
    return Results.Ok(new UserGroupDto(id, ug.Name));
}).WithName("GetUserGroup");

app.MapGet("/api/user-groups", async (IKeyValueStore store, string? name) =>
{
    if (!string.IsNullOrEmpty(name))
    {
        var ug = await store.GetByNameAsync<UserGroupStorageModel>("user_groups", name);
        if (ug is null) return Results.NotFound();
        var index = await store.GetIndexAsync("user_groups");
        var id = index.TryGetValue(name, out var ugid) ? ugid : "";
        return Results.Ok(new UserGroupDto(id, ug.Name));
    }
    var all = await store.ListAsync<UserGroupStorageModel>("user_groups");
    var idx = await store.GetIndexAsync("user_groups");
    var list = new List<UserGroupDto>();
    foreach (var ug in all) { var ugName = ug.Name ?? ""; var ugid2 = idx.TryGetValue(ugName, out var i) ? i : ""; list.Add(new UserGroupDto(ugid2, ug.Name)); }
    return Results.Ok(list);
}).WithName("ListUserGroups");

app.MapPut("/api/user-groups/{id}", async (string id, UserGroupDto request, IKeyValueStore store) =>
{
    var exists = await store.ExistsAsync("user_groups", id);
    if (!exists) return Results.NotFound();
    if (string.IsNullOrWhiteSpace(request.Name)) return Results.BadRequest(new { error = "User group name is required" });
    var storage = new UserGroupStorageModel { Name = request.Name };
    await store.UpdateAsync("user_groups", id, storage);
    return Results.Ok(new UserGroupDto(id, request.Name));
}).WithName("UpdateUserGroup");

app.MapDelete("/api/user-groups/{id}", async (string id, IKeyValueStore store) =>
{
    var exists = await store.ExistsAsync("user_groups", id);
    if (!exists) return Results.NotFound();
    await store.DeleteAsync("user_groups", id);
    return Results.NoContent();
}).WithName("DeleteUserGroup");

// --- Users (utenti) ---
// SECURITY: Password and Salt are NEVER returned in any UserDto response.
// The default admin user (id=1, username='admin') is seeded by PHP hoteldruid on install.

app.MapPost("/api/users", async (UserWriteDto request, IKeyValueStore store) =>
{
    if (string.IsNullOrWhiteSpace(request.Username))
        return Results.BadRequest(new { error = "Username is required" });

    try
    {
        var saltBytes = System.Security.Cryptography.RandomNumberGenerator.GetBytes(16);
        var salt = Convert.ToBase64String(saltBytes);
        var passwordHash = string.IsNullOrEmpty(request.Password)
            ? string.Empty
            : Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(
                System.Text.Encoding.UTF8.GetBytes((request.Password) + salt)));

        var storage = new UserStorageModel
        {
            Username = request.Username,
            PasswordHash = passwordHash,
            Salt = salt,
            PasswordType = request.PasswordType ?? "n",
            CreatedAt = DateTime.UtcNow
        };
        var id = await store.CreateAsync("users", request.Username, storage);
        return Results.Created($"/api/users/{id}", new UserDto(id, storage.Username, storage.PasswordType, storage.CreatedAt));
    }
    catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
    {
        return Results.Conflict(new { error = ex.Message });
    }
}).WithName("CreateUser");

app.MapGet("/api/users/{id}", async (string id, IKeyValueStore store) =>
{
    var u = await store.GetAsync<UserStorageModel>("users", id);
    if (u is null) return Results.NotFound();
    return Results.Ok(new UserDto(id, u.Username, u.PasswordType, u.CreatedAt));
}).WithName("GetUser");

app.MapGet("/api/users", async (IKeyValueStore store, string? username) =>
{
    if (!string.IsNullOrEmpty(username))
    {
        var u = await store.GetByNameAsync<UserStorageModel>("users", username);
        if (u is null) return Results.NotFound();
        var index = await store.GetIndexAsync("users");
        var id = index.TryGetValue(username, out var uid) ? uid : "";
        return Results.Ok(new UserDto(id, u.Username, u.PasswordType, u.CreatedAt));
    }
    var all = await store.ListAsync<UserStorageModel>("users");
    var idx = await store.GetIndexAsync("users");
    var list = new List<UserDto>();
    foreach (var u in all) { var uname = u.Username ?? ""; var uid2 = idx.TryGetValue(uname, out var i) ? i : ""; list.Add(new UserDto(uid2, u.Username, u.PasswordType, u.CreatedAt)); }
    return Results.Ok(list);
}).WithName("ListUsers");

app.MapPut("/api/users/{id}", async (string id, UserWriteDto request, IKeyValueStore store) =>
{
    var exists = await store.ExistsAsync("users", id);
    if (!exists) return Results.NotFound();
    if (string.IsNullOrWhiteSpace(request.Username)) return Results.BadRequest(new { error = "Username is required" });

    var existing = await store.GetAsync<UserStorageModel>("users", id);
    var storage = new UserStorageModel
    {
        Username = request.Username,
        PasswordHash = existing?.PasswordHash,
        Salt = existing?.Salt,
        PasswordType = request.PasswordType ?? existing?.PasswordType ?? "n",
        CreatedAt = existing?.CreatedAt ?? DateTime.UtcNow
    };
    await store.UpdateAsync("users", id, storage);
    return Results.Ok(new UserDto(id, storage.Username, storage.PasswordType, storage.CreatedAt));
}).WithName("UpdateUser");

app.MapDelete("/api/users/{id}", async (string id, IKeyValueStore store) =>
{
    var exists = await store.ExistsAsync("users", id);
    if (!exists) return Results.NotFound();
    await store.DeleteAsync("users", id);
    return Results.NoContent();
}).WithName("DeleteUser");

app.MapPost("/api/users/{id}/change-password", async (string id, Microsoft.AspNetCore.Http.HttpContext ctx, IKeyValueStore store) =>
{
    var exists = await store.ExistsAsync("users", id);
    if (!exists) return Results.NotFound();

    var body = await System.Text.Json.JsonDocument.ParseAsync(ctx.Request.Body);
    if (!body.RootElement.TryGetProperty("Password", out var pwdProp) &&
        !body.RootElement.TryGetProperty("password", out pwdProp))
        return Results.BadRequest(new { error = "Password is required" });

    var newPassword = pwdProp.GetString();
    if (string.IsNullOrEmpty(newPassword))
        return Results.BadRequest(new { error = "Password cannot be empty" });

    var existing = await store.GetAsync<UserStorageModel>("users", id);
    if (existing is null) return Results.NotFound();

    var salt = existing.Salt ?? Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(16));
    var hash = Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(
        System.Text.Encoding.UTF8.GetBytes(newPassword + salt)));

    existing.PasswordHash = hash;
    existing.Salt = salt;
    await store.UpdateAsync("users", id, existing);
    return Results.Ok(new { message = "Password updated" });
}).WithName("ChangeUserPassword");

// --- Settings (personalizza) ---
// Key format in store: "{userId}_{key}". Settings are scoped per user.

app.MapPost("/api/settings", async (SettingDto request, IKeyValueStore store) =>
{
    if (string.IsNullOrWhiteSpace(request.Key))
        return Results.BadRequest(new { error = "Setting key is required" });

    var compositeKey = $"{request.UserId}_{request.Key}";
    var storage = new SettingStorageModel
    {
        Key = request.Key,
        UserId = request.UserId,
        StringValue = request.StringValue,
        NumericValue = request.NumericValue
    };

    // Upsert: update if exists, create otherwise
    var idx = await store.GetIndexAsync("settings");
    if (idx.TryGetValue(compositeKey, out var existingId))
    {
        await store.UpdateAsync("settings", existingId, storage);
        return Results.Ok(new SettingDto(storage.Key, storage.UserId, storage.StringValue, storage.NumericValue));
    }
    else
    {
        await store.CreateAsync("settings", compositeKey, storage);
        return Results.Created($"/api/settings/{request.UserId}/{request.Key}",
            new SettingDto(storage.Key, storage.UserId, storage.StringValue, storage.NumericValue));
    }
}).WithName("UpsertSetting");

app.MapGet("/api/settings/{userId:int}/{key}", async (int userId, string key, IKeyValueStore store) =>
{
    var compositeKey = $"{userId}_{key}";
    var s = await store.GetByNameAsync<SettingStorageModel>("settings", compositeKey);
    if (s is null) return Results.NotFound();
    return Results.Ok(new SettingDto(s.Key, s.UserId, s.StringValue, s.NumericValue));
}).WithName("GetSetting");

app.MapGet("/api/settings", async (IKeyValueStore store, int? userId) =>
{
    var all = await store.ListAsync<SettingStorageModel>("settings");
    var filtered = userId.HasValue ? all.Where(s => s.UserId == userId.Value) : all;
    return Results.Ok(filtered.Select(s => new SettingDto(s.Key, s.UserId, s.StringValue, s.NumericValue)).ToList());
}).WithName("ListSettings");

app.MapPut("/api/settings/{userId:int}/{key}", async (int userId, string key, SettingDto request, IKeyValueStore store) =>
{
    var compositeKey = $"{userId}_{key}";
    var idx = await store.GetIndexAsync("settings");
    if (!idx.TryGetValue(compositeKey, out var id)) return Results.NotFound();

    var storage = new SettingStorageModel
    {
        Key = key,
        UserId = userId,
        StringValue = request.StringValue,
        NumericValue = request.NumericValue
    };
    await store.UpdateAsync("settings", id, storage);
    return Results.Ok(new SettingDto(key, userId, storage.StringValue, storage.NumericValue));
}).WithName("UpdateSetting");

app.MapDelete("/api/settings/{userId:int}/{key}", async (int userId, string key, IKeyValueStore store) =>
{
    var compositeKey = $"{userId}_{key}";
    var idx = await store.GetIndexAsync("settings");
    if (!idx.TryGetValue(compositeKey, out var id)) return Results.NotFound();
    await store.DeleteAsync("settings", id);
    return Results.NoContent();
}).WithName("DeleteSetting");

// SPA fallback: route unmatched requests to index.html for Blazor client-side routing

// --- Clients (clienti) ---

ClientFullDto ToClientFullDto(string id, ClientStorageModel s)
{
    var phones = GetCanonicalContactList(s.Phones, s.Phone, s.Phone2, s.Phone3);
    var emails = GetCanonicalContactList(s.Emails, s.Email, s.Email2, s.Email3);

    return new(
        id, s.LastName, s.FirstName, s.Nickname, s.Gender, s.Title, s.Language,
        s.DateOfBirth, s.BirthCity, s.BirthRegion, s.BirthNation,
        s.DocumentNumber, s.DocumentExpiry, s.DocumentType, s.DocumentCity, s.DocumentRegion, s.DocumentNation,
        s.Nationality, s.Nation, s.Region, s.City, s.Street, s.StreetNumber, s.PostalCode,
        s.Phone ?? phones.ElementAtOrDefault(0), s.Phone2 ?? phones.ElementAtOrDefault(1), s.Phone3 ?? phones.ElementAtOrDefault(2),
        s.Email ?? emails.ElementAtOrDefault(0), s.Email2 ?? emails.ElementAtOrDefault(1), s.Email3 ?? emails.ElementAtOrDefault(2),
        s.TaxCode, s.VatNumber, s.Notes, s.MaxOrderNumber, s.CompanionIds, s.DocumentsSent,
        s.CreatedAt, s.HostCreated, s.CreatedBy,
        phones, emails, s.SecondNationality);
}

static List<string> GetCanonicalContactList(IEnumerable<string?>? values, params string?[] legacyValues)
{
    var list = values?
        .Where(value => !string.IsNullOrWhiteSpace(value))
        .Select(value => value!.Trim())
        .ToList() ?? new List<string>();

    if (list.Any())
        return list;

    return legacyValues
        .Where(value => !string.IsNullOrWhiteSpace(value))
        .Select(value => value!.Trim())
        .ToList();
}

app.MapPost("/api/clients", async (ClientFullDto request, IKeyValueStore store) =>
{
    if (string.IsNullOrWhiteSpace(request.LastName))
        return Results.BadRequest(new { error = "LastName is required" });
    var phones = GetCanonicalContactList(request.Phones, request.Phone, request.Phone2, request.Phone3);
    var emails = GetCanonicalContactList(request.Emails, request.Email, request.Email2, request.Email3);

    var storage = new ClientStorageModel
    {
        LastName = request.LastName, FirstName = request.FirstName, Nickname = request.Nickname,
        Gender = request.Gender, Title = request.Title, Language = request.Language,
        DateOfBirth = request.DateOfBirth, BirthCity = request.BirthCity, BirthRegion = request.BirthRegion,
        BirthNation = request.BirthNation, DocumentNumber = request.DocumentNumber,
        DocumentExpiry = request.DocumentExpiry, DocumentType = request.DocumentType,
        DocumentCity = request.DocumentCity, DocumentRegion = request.DocumentRegion,
        DocumentNation = request.DocumentNation, Nationality = request.Nationality,
        Nation = request.Nation, Region = request.Region, City = request.City,
        Street = request.Street, StreetNumber = request.StreetNumber, PostalCode = request.PostalCode,
        Phones = phones, Emails = emails,
        Phone = request.Phone ?? phones.ElementAtOrDefault(0), Phone2 = request.Phone2 ?? phones.ElementAtOrDefault(1), Phone3 = request.Phone3 ?? phones.ElementAtOrDefault(2),
        Email = request.Email ?? emails.ElementAtOrDefault(0), Email2 = request.Email2 ?? emails.ElementAtOrDefault(1), Email3 = request.Email3 ?? emails.ElementAtOrDefault(2),
        TaxCode = request.TaxCode, VatNumber = request.VatNumber, Notes = request.Notes,
        MaxOrderNumber = request.MaxOrderNumber, CompanionIds = request.CompanionIds,
        DocumentsSent = request.DocumentsSent, SecondNationality = request.SecondNationality,
        CreatedAt = request.CreatedAt ?? DateTime.UtcNow,
        HostCreated = request.HostCreated, CreatedBy = request.CreatedBy
    };
    var key = $"{request.LastName}_{DateTime.UtcNow.Ticks}";
    var id = await store.CreateAsync("clients", key, storage);
    return Results.Created($"/api/clients/{id}", ToClientFullDto(id, storage));
}).WithName("CreateClient");

app.MapGet("/api/clients/{id}", async (string id, IKeyValueStore store) =>
{
    var c = await store.GetAsync<ClientStorageModel>("clients", id);
    if (c is null) return Results.NotFound();
    return Results.Ok(ToClientFullDto(id, c));
}).WithName("GetClient");

app.MapGet("/api/clients", async (IKeyValueStore store, string? lastName, DateOnly? from, DateOnly? to) =>
{
    if (from.HasValue && to.HasValue && to.Value < from.Value)
    {
        (from, to) = (to, from);
    }

    if (from.HasValue && to.HasValue)
    {
        var guestIds = await CollectDeskGuestIdsAsync(store, from.Value, to.Value);
        var matched = new List<ClientFullDto>();
        foreach (var guestId in guestIds)
        {
            var guest = await store.GetAsync<ClientStorageModel>("clients", guestId);
            if (guest is null || !ClientMatchesLastName(guest, lastName)) continue;
            matched.Add(ToClientFullDto(guestId, guest));
        }

        return Results.Ok(matched);
    }

    var idx = await store.GetIndexAsync("clients");
    var result = new List<ClientFullDto>();
    foreach (var kvp in idx)
    {
        var c = await store.GetAsync<ClientStorageModel>("clients", kvp.Value);
        if (c is null || !ClientMatchesLastName(c, lastName)) continue;
        result.Add(ToClientFullDto(kvp.Value, c));
    }
    return Results.Ok(result);
}).WithName("ListClients");

static bool ClientMatchesLastName(ClientStorageModel client, string? lastName)
    => string.IsNullOrEmpty(lastName)
        || (client.LastName is not null && client.LastName.Contains(lastName, StringComparison.OrdinalIgnoreCase));

static async Task<HashSet<string>> CollectDeskGuestIdsAsync(IKeyValueStore store, DateOnly from, DateOnly to)
{
    var guestIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    var idx = await store.GetIndexAsync("bookings");
    foreach (var kvp in idx)
    {
        if (FilingYearOutsideRange(kvp.Key, from.Year - 1, to.Year))
        {
            continue;
        }

        var booking = await store.GetAsync<BookingStorageModel>("bookings", kvp.Value);
        if (booking is null || booking.DeletedAt.HasValue || string.IsNullOrWhiteSpace(booking.ClientId))
        {
            continue;
        }

        if (!GuestIsOnDesk(booking, from, to))
        {
            continue;
        }

        guestIds.Add(booking.ClientId);
    }

    return guestIds;
}

static bool GuestIsOnDesk(BookingStorageModel booking, DateOnly from, DateOnly to)
{
    var status = booking.Status;
    if (!string.IsNullOrWhiteSpace(status)
        && (status.Contains("cancel", StringComparison.OrdinalIgnoreCase)
            || status.Contains("void", StringComparison.OrdinalIgnoreCase)
            || status.Contains("deleted", StringComparison.OrdinalIgnoreCase)))
    {
        return false;
    }

    if (DateFallsInRange(booking.ActualArrivalDate, from, to) || DateFallsInRange(booking.ActualDepartureDate, from, to))
    {
        return true;
    }

    return StayOverlapsRange(booking, from, to);
}

static bool DateFallsInRange(DateOnly? day, DateOnly from, DateOnly to)
    => day.HasValue && day.Value >= from && day.Value <= to;

app.MapPut("/api/clients/{id}", async (string id, ClientFullDto request, IKeyValueStore store) =>
{
    var exists = await store.ExistsAsync("clients", id);
    if (!exists) return Results.NotFound();
    if (string.IsNullOrWhiteSpace(request.LastName)) return Results.BadRequest(new { error = "LastName is required" });
    var existing = await store.GetAsync<ClientStorageModel>("clients", id);
    if (existing is null) return Results.NotFound();
    var phones = GetCanonicalContactList(request.Phones, request.Phone, request.Phone2, request.Phone3);
    var emails = GetCanonicalContactList(request.Emails, request.Email, request.Email2, request.Email3);

    var storage = new ClientStorageModel
    {
        LastName = request.LastName, FirstName = request.FirstName, Nickname = request.Nickname,
        Gender = request.Gender, Title = request.Title, Language = request.Language,
        DateOfBirth = request.DateOfBirth, BirthCity = request.BirthCity, BirthRegion = request.BirthRegion,
        BirthNation = request.BirthNation, DocumentNumber = request.DocumentNumber,
        DocumentExpiry = request.DocumentExpiry, DocumentType = request.DocumentType,
        DocumentCity = request.DocumentCity, DocumentRegion = request.DocumentRegion,
        DocumentNation = request.DocumentNation, Nationality = request.Nationality,
        Nation = request.Nation, Region = request.Region, City = request.City,
        Street = request.Street, StreetNumber = request.StreetNumber, PostalCode = request.PostalCode,
        Phones = phones, Emails = emails,
        Phone = request.Phone ?? phones.ElementAtOrDefault(0), Phone2 = request.Phone2 ?? phones.ElementAtOrDefault(1), Phone3 = request.Phone3 ?? phones.ElementAtOrDefault(2),
        Email = request.Email ?? emails.ElementAtOrDefault(0), Email2 = request.Email2 ?? emails.ElementAtOrDefault(1), Email3 = request.Email3 ?? emails.ElementAtOrDefault(2),
        TaxCode = request.TaxCode, VatNumber = request.VatNumber, Notes = request.Notes,
        MaxOrderNumber = request.MaxOrderNumber, CompanionIds = request.CompanionIds,
        DocumentsSent = request.DocumentsSent,
        SecondNationality = request.SecondNationality,
        CreatedAt = request.CreatedAt ?? existing.CreatedAt ?? DateTime.UtcNow,
        HostCreated = request.HostCreated ?? existing.HostCreated,
        CreatedBy = request.CreatedBy ?? existing.CreatedBy
    };
    await store.UpdateAsync("clients", id, storage);
    return Results.Ok(ToClientFullDto(id, storage));
}).WithName("UpdateClient");

app.MapDelete("/api/clients/{id}", async (string id, IKeyValueStore store) =>
{
    var exists = await store.ExistsAsync("clients", id);
    if (!exists) return Results.NotFound();
    await store.DeleteAsync("clients", id);
    return Results.NoContent();
}).WithName("DeleteClient");

// --- User Privileges (privilegi) ---

UserPrivilegeDto ToUserPrivilegeDto(string id, UserPrivilegeStorageModel s) => new(
    id, s.UserId, s.Year, s.AllowedRules, s.AllowedTariffs, s.AllowedExtraCosts, s.AllowedContracts,
    s.AllowedCashRegisters, s.PaymentCashRegister, s.BookingInsertPriv, s.BookingModifyPriv,
    s.PersonalSettingsPriv, s.ClientInsertPriv, s.ClientPrefix, s.CostInsertPriv,
    s.ViewTablePriv, s.TariffInsertPriv, s.RuleInsertPriv, s.MessagePriv, s.InventoryPriv);

app.MapPost("/api/user-privileges", async (UserPrivilegeDto request, IKeyValueStore store) =>
{
    if (request.UserId is null)
        return Results.BadRequest(new { error = "UserId is required" });
    var key = $"{request.UserId}_{request.Year}";
    var storage = new UserPrivilegeStorageModel
    {
        UserId = request.UserId, Year = request.Year, AllowedRules = request.AllowedRules,
        AllowedTariffs = request.AllowedTariffs, AllowedExtraCosts = request.AllowedExtraCosts,
        AllowedContracts = request.AllowedContracts, AllowedCashRegisters = request.AllowedCashRegisters,
        PaymentCashRegister = request.PaymentCashRegister, BookingInsertPriv = request.BookingInsertPriv,
        BookingModifyPriv = request.BookingModifyPriv, PersonalSettingsPriv = request.PersonalSettingsPriv,
        ClientInsertPriv = request.ClientInsertPriv, ClientPrefix = request.ClientPrefix,
        CostInsertPriv = request.CostInsertPriv, ViewTablePriv = request.ViewTablePriv,
        TariffInsertPriv = request.TariffInsertPriv, RuleInsertPriv = request.RuleInsertPriv,
        MessagePriv = request.MessagePriv, InventoryPriv = request.InventoryPriv
    };
    var id = await store.CreateAsync("user_privileges", key, storage);
    return Results.Created($"/api/user-privileges/{request.UserId}/{request.Year}", ToUserPrivilegeDto(id, storage));
}).WithName("CreateUserPrivilege");

app.MapGet("/api/user-privileges/{userId:int}/{year:int}", async (int userId, int year, IKeyValueStore store) =>
{
    var key = $"{userId}_{year}";
    var idx = await store.GetIndexAsync("user_privileges");
    if (!idx.TryGetValue(key, out var id)) return Results.NotFound();
    var p = await store.GetAsync<UserPrivilegeStorageModel>("user_privileges", id);
    if (p is null) return Results.NotFound();
    return Results.Ok(ToUserPrivilegeDto(id, p));
}).WithName("GetUserPrivilege");

app.MapGet("/api/user-privileges", async (IKeyValueStore store, int? userId, int? year) =>
{
    var all = await store.ListAsync<UserPrivilegeStorageModel>("user_privileges");
    var idx = await store.GetIndexAsync("user_privileges");
    var filtered = all.Where(p => (!userId.HasValue || p.UserId == userId.Value) && (!year.HasValue || p.Year == year.Value)).ToList();
    var nameToId = idx.ToDictionary(kv => kv.Value, kv => kv.Key);
    var result = new List<UserPrivilegeDto>();
    foreach (var p in filtered)
    {
        var key = $"{p.UserId}_{p.Year}";
        var pid = idx.TryGetValue(key, out var i) ? i : "";
        result.Add(ToUserPrivilegeDto(pid, p));
    }
    return Results.Ok(result);
}).WithName("ListUserPrivileges");

app.MapPut("/api/user-privileges/{userId:int}/{year:int}", async (int userId, int year, UserPrivilegeDto request, IKeyValueStore store) =>
{
    var key = $"{userId}_{year}";
    var idx = await store.GetIndexAsync("user_privileges");
    if (!idx.TryGetValue(key, out var id)) return Results.NotFound();
    var storage = new UserPrivilegeStorageModel
    {
        UserId = userId, Year = year, AllowedRules = request.AllowedRules,
        AllowedTariffs = request.AllowedTariffs, AllowedExtraCosts = request.AllowedExtraCosts,
        AllowedContracts = request.AllowedContracts, AllowedCashRegisters = request.AllowedCashRegisters,
        PaymentCashRegister = request.PaymentCashRegister, BookingInsertPriv = request.BookingInsertPriv,
        BookingModifyPriv = request.BookingModifyPriv, PersonalSettingsPriv = request.PersonalSettingsPriv,
        ClientInsertPriv = request.ClientInsertPriv, ClientPrefix = request.ClientPrefix,
        CostInsertPriv = request.CostInsertPriv, ViewTablePriv = request.ViewTablePriv,
        TariffInsertPriv = request.TariffInsertPriv, RuleInsertPriv = request.RuleInsertPriv,
        MessagePriv = request.MessagePriv, InventoryPriv = request.InventoryPriv
    };
    await store.UpdateAsync("user_privileges", id, storage);
    return Results.Ok(ToUserPrivilegeDto(id, storage));
}).WithName("UpdateUserPrivilege");

app.MapDelete("/api/user-privileges/{userId:int}/{year:int}", async (int userId, int year, IKeyValueStore store) =>
{
    var key = $"{userId}_{year}";
    var idx = await store.GetIndexAsync("user_privileges");
    if (!idx.TryGetValue(key, out var id)) return Results.NotFound();
    await store.DeleteAsync("user_privileges", id);
    return Results.NoContent();
}).WithName("DeleteUserPrivilege");

// --- User Relations (relutenti) ---

UserRelationDto ToUserRelationDto(string id, UserRelationStorageModel s) => new(
    id, s.UserId, s.NationId, s.RegionId, s.CityId, s.DocumentTypeId,
    s.FamilyRelationshipId, s.SuperiorId, s.IsDefault, s.CreatedAt, s.HostCreated, s.CreatedBy);

app.MapPost("/api/user-relations", async (UserRelationDto request, IKeyValueStore store) =>
{
    if (request.UserId is null)
        return Results.BadRequest(new { error = "UserId is required" });
    var key = $"{request.UserId}";
    var storage = new UserRelationStorageModel
    {
        UserId = request.UserId, NationId = request.NationId, RegionId = request.RegionId,
        CityId = request.CityId, DocumentTypeId = request.DocumentTypeId,
        FamilyRelationshipId = request.FamilyRelationshipId, SuperiorId = request.SuperiorId,
        IsDefault = request.IsDefault, CreatedAt = request.CreatedAt ?? DateTime.UtcNow,
        HostCreated = request.HostCreated, CreatedBy = request.CreatedBy
    };
    var id = await store.CreateAsync("user_relations", key, storage);
    return Results.Created($"/api/user-relations/{request.UserId}", ToUserRelationDto(id, storage));
}).WithName("CreateUserRelation");

app.MapGet("/api/user-relations/{userId:int}", async (int userId, IKeyValueStore store) =>
{
    var key = $"{userId}";
    var idx = await store.GetIndexAsync("user_relations");
    if (!idx.TryGetValue(key, out var id)) return Results.NotFound();
    var r = await store.GetAsync<UserRelationStorageModel>("user_relations", id);
    if (r is null) return Results.NotFound();
    return Results.Ok(ToUserRelationDto(id, r));
}).WithName("GetUserRelation");

app.MapPut("/api/user-relations/{userId:int}", async (int userId, UserRelationDto request, IKeyValueStore store) =>
{
    var key = $"{userId}";
    var idx = await store.GetIndexAsync("user_relations");
    if (!idx.TryGetValue(key, out var id)) return Results.NotFound();
    var storage = new UserRelationStorageModel
    {
        UserId = userId, NationId = request.NationId, RegionId = request.RegionId,
        CityId = request.CityId, DocumentTypeId = request.DocumentTypeId,
        FamilyRelationshipId = request.FamilyRelationshipId, SuperiorId = request.SuperiorId,
        IsDefault = request.IsDefault, CreatedAt = request.CreatedAt ?? DateTime.UtcNow,
        HostCreated = request.HostCreated, CreatedBy = request.CreatedBy
    };
    await store.UpdateAsync("user_relations", id, storage);
    return Results.Ok(ToUserRelationDto(id, storage));
}).WithName("UpdateUserRelation");

app.MapDelete("/api/user-relations/{userId:int}", async (int userId, IKeyValueStore store) =>
{
    var key = $"{userId}";
    var idx = await store.GetIndexAsync("user_relations");
    if (!idx.TryGetValue(key, out var id)) return Results.NotFound();
    await store.DeleteAsync("user_relations", id);
    return Results.NoContent();
}).WithName("DeleteUserRelation");

// --- Group Memberships (relgruppi) ---

GroupMembershipDto ToGroupMembershipDto(string id, GroupMembershipStorageModel s) => new(
    id, s.UserId, s.GroupId, s.CreatedAt, s.HostCreated, s.CreatedBy);

app.MapPost("/api/group-memberships", async (GroupMembershipDto request, IKeyValueStore store) =>
{
    if (request.UserId is null || request.GroupId is null)
        return Results.BadRequest(new { error = "UserId and GroupId are required" });
    var key = $"{request.UserId}_{request.GroupId}";
    var storage = new GroupMembershipStorageModel
    {
        UserId = request.UserId, GroupId = request.GroupId,
        CreatedAt = request.CreatedAt ?? DateTime.UtcNow,
        HostCreated = request.HostCreated, CreatedBy = request.CreatedBy
    };
    var id = await store.CreateAsync("group_memberships", key, storage);
    return Results.Created($"/api/group-memberships/{request.UserId}/{request.GroupId}", ToGroupMembershipDto(id, storage));
}).WithName("CreateGroupMembership");

app.MapGet("/api/group-memberships", async (IKeyValueStore store, int? userId) =>
{
    var idx = await store.GetIndexAsync("group_memberships");
    var result = new List<GroupMembershipDto>();
    foreach (var kvp in idx)
    {
        var m = await store.GetAsync<GroupMembershipStorageModel>("group_memberships", kvp.Value);
        if (m is null) continue;
        if (userId.HasValue && m.UserId != userId.Value) continue;
        result.Add(ToGroupMembershipDto(kvp.Value, m));
    }
    return Results.Ok(result);
}).WithName("ListGroupMemberships");

app.MapDelete("/api/group-memberships/{userId:int}/{groupId:int}", async (int userId, int groupId, IKeyValueStore store) =>
{
    var key = $"{userId}_{groupId}";
    var idx = await store.GetIndexAsync("group_memberships");
    if (!idx.TryGetValue(key, out var id)) return Results.NotFound();
    await store.DeleteAsync("group_memberships", id);
    return Results.NoContent();
}).WithName("DeleteGroupMembership");

// --- Layer 3: Years (anni) ---

app.MapGet("/api/years", async (IKeyValueStore store) =>
{
    var idx = await store.GetIndexAsync("years");
    var result = new List<YearDto>();
    foreach (var kvp in idx)
    {
        if (!int.TryParse(kvp.Key, out var yr)) continue;
        var ym = await store.GetAsync<YearStorageModel>("years", kvp.Value);
        if (ym is null) continue;
        result.Add(new YearDto(yr, ym.PeriodType));
    }
    return Results.Ok(result.OrderBy(y => y.Year));
}).WithName("ListYears");

app.MapGet("/api/years/{year:int}", async (int year, IKeyValueStore store) =>
{
    var idx = await store.GetIndexAsync("years");
    if (!idx.TryGetValue($"{year}", out var id)) return Results.NotFound();
    var ym = await store.GetAsync<YearStorageModel>("years", id);
    if (ym is null) return Results.NotFound();
    return Results.Ok(new YearDto(year, ym.PeriodType));
}).WithName("GetYear");

app.MapPost("/api/years", async (YearDto request, IKeyValueStore store) =>
{
    var key = $"{request.Year}";
    var idx = await store.GetIndexAsync("years");
    if (idx.ContainsKey(key)) return Results.Conflict(new { error = "Year already exists" });
    var storage = new YearStorageModel { PeriodType = request.PeriodType ?? "variable" };
    await store.CreateAsync("years", key, storage);
    return Results.Created($"/api/years/{request.Year}", new YearDto(request.Year, storage.PeriodType));
}).WithName("CreateYear");

app.MapPut("/api/years/{year:int}", async (int year, YearDto request, IKeyValueStore store) =>
{
    var key = $"{year}";
    var idx = await store.GetIndexAsync("years");
    if (!idx.TryGetValue(key, out var id)) return Results.NotFound();
    var storage = new YearStorageModel { PeriodType = request.PeriodType ?? "variable" };
    await store.UpdateAsync("years", id, storage);
    return Results.Ok(new YearDto(year, storage.PeriodType));
}).WithName("UpdateYear");

app.MapDelete("/api/years/{year:int}", async (int year, IKeyValueStore store) =>
{
    var key = $"{year}";
    var idx = await store.GetIndexAsync("years");
    if (!idx.TryGetValue(key, out var id)) return Results.NotFound();
    await store.DeleteAsync("years", id);
    return Results.NoContent();
}).WithName("DeleteYear");

// --- Layer 3: Periods (periodi{year}) ---

PeriodDto ToPeriodDto(string id, PeriodStorageModel s) => new(
    id, s.Year, s.StartDate, s.EndDate,
    s.Tariff1, s.Tariff1PerPerson, s.Tariff2, s.Tariff2PerPerson,
    s.Tariff3, s.Tariff3PerPerson, s.Tariff4, s.Tariff4PerPerson,
    s.Tariff5, s.Tariff5PerPerson, s.Tariff6, s.Tariff6PerPerson,
    s.Tariff7, s.Tariff7PerPerson, s.Tariff8, s.Tariff8PerPerson,
    s.Tariff9, s.Tariff9PerPerson, s.Tariff10, s.Tariff10PerPerson,
    s.Tariff11, s.Tariff11PerPerson, s.Tariff12, s.Tariff12PerPerson);

app.MapGet("/api/periods", async (IKeyValueStore store, int? year) =>
{
    var idx = await store.GetIndexAsync("periods");
    var prefix = year.HasValue ? $"{year}_" : null;
    var result = new List<PeriodDto>();
    foreach (var kvp in idx)
    {
        if (prefix != null && !kvp.Key.StartsWith(prefix)) continue;
        var p = await store.GetAsync<PeriodStorageModel>("periods", kvp.Value);
        if (p is null) continue;
        result.Add(ToPeriodDto(kvp.Value, p));
    }

    return Results.Ok(result);
}).WithName("ListPeriods");

app.MapGet("/api/periods/{year:int}/{id}", async (int year, string id, IKeyValueStore store) =>
{
    var p = await store.GetAsync<PeriodStorageModel>("periods", id);
    if (p is null || p.Year != year) return Results.NotFound();
    return Results.Ok(ToPeriodDto(id, p));
}).WithName("GetPeriod");

app.MapPost("/api/periods", async (PeriodDto request, IKeyValueStore store) =>
{
    var guid = Guid.NewGuid().ToString("N");
    var key = $"{request.Year}_{guid}";
    var storage = new PeriodStorageModel
    {
        Year = request.Year, StartDate = request.StartDate, EndDate = request.EndDate,
        Tariff1 = request.Tariff1, Tariff1PerPerson = request.Tariff1PerPerson,
        Tariff2 = request.Tariff2, Tariff2PerPerson = request.Tariff2PerPerson,
        Tariff3 = request.Tariff3, Tariff3PerPerson = request.Tariff3PerPerson,
        Tariff4 = request.Tariff4, Tariff4PerPerson = request.Tariff4PerPerson,
        Tariff5 = request.Tariff5, Tariff5PerPerson = request.Tariff5PerPerson,
        Tariff6 = request.Tariff6, Tariff6PerPerson = request.Tariff6PerPerson,
        Tariff7 = request.Tariff7, Tariff7PerPerson = request.Tariff7PerPerson,
        Tariff8 = request.Tariff8, Tariff8PerPerson = request.Tariff8PerPerson,
        Tariff9 = request.Tariff9, Tariff9PerPerson = request.Tariff9PerPerson,
        Tariff10 = request.Tariff10, Tariff10PerPerson = request.Tariff10PerPerson,
        Tariff11 = request.Tariff11, Tariff11PerPerson = request.Tariff11PerPerson,
        Tariff12 = request.Tariff12, Tariff12PerPerson = request.Tariff12PerPerson
    };
    var id = await store.CreateAsync("periods", key, storage);
    return Results.Created($"/api/periods/{request.Year}/{id}", ToPeriodDto(id, storage));
}).WithName("CreatePeriod");

app.MapPut("/api/periods/{year:int}/{id}", async (int year, string id, PeriodDto request, IKeyValueStore store) =>
{
    var p = await store.GetAsync<PeriodStorageModel>("periods", id);
    if (p is null || p.Year != year) return Results.NotFound();
    var storage = new PeriodStorageModel
    {
        Year = year, StartDate = request.StartDate, EndDate = request.EndDate,
        Tariff1 = request.Tariff1, Tariff1PerPerson = request.Tariff1PerPerson,
        Tariff2 = request.Tariff2, Tariff2PerPerson = request.Tariff2PerPerson,
        Tariff3 = request.Tariff3, Tariff3PerPerson = request.Tariff3PerPerson,
        Tariff4 = request.Tariff4, Tariff4PerPerson = request.Tariff4PerPerson,
        Tariff5 = request.Tariff5, Tariff5PerPerson = request.Tariff5PerPerson,
        Tariff6 = request.Tariff6, Tariff6PerPerson = request.Tariff6PerPerson,
        Tariff7 = request.Tariff7, Tariff7PerPerson = request.Tariff7PerPerson,
        Tariff8 = request.Tariff8, Tariff8PerPerson = request.Tariff8PerPerson,
        Tariff9 = request.Tariff9, Tariff9PerPerson = request.Tariff9PerPerson,
        Tariff10 = request.Tariff10, Tariff10PerPerson = request.Tariff10PerPerson,
        Tariff11 = request.Tariff11, Tariff11PerPerson = request.Tariff11PerPerson,
        Tariff12 = request.Tariff12, Tariff12PerPerson = request.Tariff12PerPerson
    };
    await store.UpdateAsync("periods", id, storage);
    return Results.Ok(ToPeriodDto(id, storage));
}).WithName("UpdatePeriod");

app.MapDelete("/api/periods/{year:int}/{id}", async (int year, string id, IKeyValueStore store) =>
{
    var p = await store.GetAsync<PeriodStorageModel>("periods", id);
    if (p is null || p.Year != year) return Results.NotFound();
    await store.DeleteAsync("periods", id);
    return Results.NoContent();
}).WithName("DeletePeriod");

// --- Layer 3: Extra Costs (ntariffe{year}) ---

ExtraCostDto ToExtraCostDto(string id, ExtraCostStorageModel s) => new(
    id, s.Year, s.ExtraCostName, s.CostType, s.BaseValue, s.PercentageValue, s.TaxPercentage, s.Category, s.NumberLimit, s.ValidFrom, s.ValidUntil);

app.MapGet("/api/extra-costs", async (IKeyValueStore store, int? year) =>
{
    var idx = await store.GetIndexAsync("extra_costs");
    var prefix = year.HasValue ? $"{year}_" : null;
    var result = new List<ExtraCostDto>();
    foreach (var kvp in idx)
    {
        if (prefix != null && !kvp.Key.StartsWith(prefix)) continue;
        var t = await store.GetAsync<ExtraCostStorageModel>("extra_costs", kvp.Value);
        if (t is null) continue;
        result.Add(ToExtraCostDto(kvp.Value, t));
    }

    return Results.Ok(result);
}).WithName("ListExtraCosts");

app.MapGet("/api/extra-costs/{year:int}/{id}", async (int year, string id, IKeyValueStore store) =>
{
    var t = await store.GetAsync<ExtraCostStorageModel>("extra_costs", id);
    if (t is null || t.Year != year) return Results.NotFound();
    return Results.Ok(ToExtraCostDto(id, t));
}).WithName("GetExtraCost");

app.MapPost("/api/extra-costs", async (ExtraCostDto request, IKeyValueStore store) =>
{
    var guid = Guid.NewGuid().ToString("N");
    var key = $"{request.Year}_{guid}";
    var storage = new ExtraCostStorageModel
    {
        Year = request.Year, ExtraCostName = request.ExtraCostName, CostType = request.CostType,
        BaseValue = request.BaseValue, PercentageValue = request.PercentageValue,
        TaxPercentage = request.TaxPercentage, Category = request.Category, NumberLimit = request.NumberLimit,
        ValidFrom = request.ValidFrom, ValidUntil = request.ValidUntil
    };
    var id = await store.CreateAsync("extra_costs", key, storage);
    return Results.Created($"/api/extra-costs/{request.Year}/{id}", ToExtraCostDto(id, storage));
}).WithName("CreateExtraCost");

app.MapPut("/api/extra-costs/{year:int}/{id}", async (int year, string id, ExtraCostDto request, IKeyValueStore store) =>
{
    var t = await store.GetAsync<ExtraCostStorageModel>("extra_costs", id);
    if (t is null || t.Year != year) return Results.NotFound();
    var storage = new ExtraCostStorageModel
    {
        Year = year, ExtraCostName = request.ExtraCostName, CostType = request.CostType,
        BaseValue = request.BaseValue, PercentageValue = request.PercentageValue,
        TaxPercentage = request.TaxPercentage, Category = request.Category, NumberLimit = request.NumberLimit,
        ValidFrom = request.ValidFrom, ValidUntil = request.ValidUntil
    };
    await store.UpdateAsync("extra_costs", id, storage);
    return Results.Ok(ToExtraCostDto(id, storage));
}).WithName("UpdateExtraCost");

app.MapDelete("/api/extra-costs/{year:int}/{id}", async (int year, string id, IKeyValueStore store) =>
{
    var t = await store.GetAsync<ExtraCostStorageModel>("extra_costs", id);
    if (t is null || t.Year != year) return Results.NotFound();
    await store.DeleteAsync("extra_costs", id);
    return Results.NoContent();
}).WithName("DeleteExtraCost");

// --- Layer 3: Assignment Rules (regole{year}) ---

AssignmentRuleDto ToAssignmentRuleDto(string id, AssignmentRuleStorageModel s) => new(
    id, s.Year, s.RoomOrAgency, s.ClosedTariff, s.TariffPerRoom, s.StartPeriodId, s.EndPeriodId, s.Reason1);

app.MapGet("/api/assignment-rules", async (IKeyValueStore store, int? year) =>
{
    var idx = await store.GetIndexAsync("assignment_rules");
    var prefix = year.HasValue ? $"{year}_" : null;
    var result = new List<AssignmentRuleDto>();
    foreach (var kvp in idx)
    {
        if (prefix != null && !kvp.Key.StartsWith(prefix)) continue;
        var r = await store.GetAsync<AssignmentRuleStorageModel>("assignment_rules", kvp.Value);
        if (r is null) continue;
        result.Add(ToAssignmentRuleDto(kvp.Value, r));
    }
    return Results.Ok(result);
}).WithName("ListAssignmentRules");

app.MapGet("/api/assignment-rules/{year:int}/{id}", async (int year, string id, IKeyValueStore store) =>
{
    var r = await store.GetAsync<AssignmentRuleStorageModel>("assignment_rules", id);
    if (r is null || r.Year != year) return Results.NotFound();
    return Results.Ok(ToAssignmentRuleDto(id, r));
}).WithName("GetAssignmentRule");

app.MapPost("/api/assignment-rules", async (AssignmentRuleDto request, IKeyValueStore store) =>
{
    var guid = Guid.NewGuid().ToString("N");
    var key = $"{request.Year}_{guid}";
    var storage = new AssignmentRuleStorageModel
    {
        Year = request.Year, RoomOrAgency = request.RoomOrAgency, ClosedTariff = request.ClosedTariff,
        TariffPerRoom = request.TariffPerRoom, StartPeriodId = request.StartPeriodId,
        EndPeriodId = request.EndPeriodId, Reason1 = request.Reason1
    };
    var id = await store.CreateAsync("assignment_rules", key, storage);
    return Results.Created($"/api/assignment-rules/{request.Year}/{id}", ToAssignmentRuleDto(id, storage));
}).WithName("CreateAssignmentRule");

app.MapPut("/api/assignment-rules/{year:int}/{id}", async (int year, string id, AssignmentRuleDto request, IKeyValueStore store) =>
{
    var r = await store.GetAsync<AssignmentRuleStorageModel>("assignment_rules", id);
    if (r is null || r.Year != year) return Results.NotFound();
    var storage = new AssignmentRuleStorageModel
    {
        Year = year, RoomOrAgency = request.RoomOrAgency, ClosedTariff = request.ClosedTariff,
        TariffPerRoom = request.TariffPerRoom, StartPeriodId = request.StartPeriodId,
        EndPeriodId = request.EndPeriodId, Reason1 = request.Reason1
    };
    await store.UpdateAsync("assignment_rules", id, storage);
    return Results.Ok(ToAssignmentRuleDto(id, storage));
}).WithName("UpdateAssignmentRule");

app.MapDelete("/api/assignment-rules/{year:int}/{id}", async (int year, string id, IKeyValueStore store) =>
{
    var r = await store.GetAsync<AssignmentRuleStorageModel>("assignment_rules", id);
    if (r is null || r.Year != year) return Results.NotFound();
    await store.DeleteAsync("assignment_rules", id);
    return Results.NoContent();
}).WithName("DeleteAssignmentRule");

app.MapFallbackToFile("index.html");

// --- Layer 4: Bookings (prenotazioni{year}) ---

BookingDto ToBookingDto(string id, BookingStorageModel s) => new(
    id,
    s.Year,
    s.ClientId,
    s.RoomId,
    s.ArrivalDate,
    s.DepartureDate,
    s.Status,
    s.Notes,
    s.ActualArrivalDate,
    s.ActualGuestCount,
    s.PrepaidAmount,
    s.PrepaidMethod,
    s.PrepaidReference,
    s.ActualDepartureDate,
    s.ExpectedGuestCount,
    s.TariffId,
    s.PreAuthReference,
    s.PreAuthMethod,
    s.SettlementDate,
    s.DeletedAt,
    s.TariffTotalPrice,
    s.Source,
    s.ExternalReference,
    s.GroupCode,
    s.RoomFixed,
    s.ContinuedFromId,
    s.ContinuedToId,
    s.GroupName,
    s.EntryTime,
    s.PrintedInvoiceNumber,
    s.IssuedInvoiceNumber,
    s.GuestFileStatus,
    s.GuestFileReceipt);

async Task ResolveAndApplyTariffAsync(BookingStorageModel storage, ITariffResolutionService resolver)
{
    if (string.IsNullOrWhiteSpace(storage.RoomId) || !storage.ArrivalDate.HasValue || !storage.DepartureDate.HasValue)
    {
        return;
    }

    var guestCount = storage.ActualGuestCount ?? storage.ExpectedGuestCount ?? 1;
    var result = await resolver.ResolveAsync(storage.RoomId, storage.ArrivalDate.Value, storage.DepartureDate.Value, guestCount);
    storage.TariffId = result?.TariffId;
    storage.TariffTotalPrice = result?.TotalPrice;
}

string? ValidateBookingRequest(BookingDto request, int expectedYear)
{
    if (expectedYear <= 0)
        return "Year is required";
    if (string.IsNullOrWhiteSpace(request.ClientId))
        return "ClientId is required";
    if (!request.ArrivalDate.HasValue)
        return "ArrivalDate is required";
    if (!request.DepartureDate.HasValue)
        return "DepartureDate is required";
    if (request.DepartureDate.Value <= request.ArrivalDate.Value)
        return "DepartureDate must be after ArrivalDate";
    if (request.PrepaidAmount.HasValue && request.PrepaidAmount.Value < 0)
        return "PrepaidAmount must be greater than or equal to zero";
    if (request.ActualArrivalDate.HasValue && !request.ActualGuestCount.HasValue)
        return "ActualGuestCount is required when ActualArrivalDate is provided";
    if (request.ActualGuestCount.HasValue && !request.ActualArrivalDate.HasValue)
        return "ActualArrivalDate is required when ActualGuestCount is provided";
    if (request.ActualGuestCount.HasValue && request.ActualGuestCount.Value <= 0)
        return "ActualGuestCount must be greater than zero";
    if (request.ActualArrivalDate.HasValue && request.DepartureDate.HasValue && request.ActualArrivalDate.Value >= request.DepartureDate.Value)
        return "ActualArrivalDate must be before DepartureDate";
    if (request.ActualDepartureDate.HasValue && !request.ActualArrivalDate.HasValue)
        return "ActualArrivalDate is required when ActualDepartureDate is provided";
    if (request.ActualDepartureDate.HasValue && request.DepartureDate.HasValue && request.ActualDepartureDate.Value > request.DepartureDate.Value)
        return "ActualDepartureDate cannot be after DepartureDate";
    if (request.ActualDepartureDate.HasValue && request.ActualArrivalDate.HasValue && request.ActualDepartureDate.Value < request.ActualArrivalDate.Value)
        return "ActualDepartureDate must be on or after ActualArrivalDate";

    return null;
}

bool IsNonBlockingBookingStatus(string? status)
{
    if (string.IsNullOrWhiteSpace(status))
    {
        return false;
    }

    return status.Contains("cancel", StringComparison.OrdinalIgnoreCase)
           || status.Contains("void", StringComparison.OrdinalIgnoreCase)
           || status.Contains("deleted", StringComparison.OrdinalIgnoreCase);
}

bool IsCurrentStay(BookingStorageModel booking, DateOnly today)
{
    if (booking.DeletedAt.HasValue
        || string.IsNullOrWhiteSpace(booking.RoomId)
        || IsNonBlockingBookingStatus(booking.Status)
        || !booking.ActualArrivalDate.HasValue
        || booking.ActualDepartureDate.HasValue
        || !booking.DepartureDate.HasValue
        || booking.DepartureDate.Value <= booking.ActualArrivalDate.Value
        || !booking.ActualGuestCount.HasValue
        || booking.ActualGuestCount.Value <= 0)
    {
        return false;
    }

    return booking.ActualArrivalDate.Value <= today && today < booking.DepartureDate.Value;
}

bool IsReservation(BookingStorageModel booking)
{
    if (booking.DeletedAt.HasValue
        || IsNonBlockingBookingStatus(booking.Status)
        || booking.ActualArrivalDate.HasValue
        || booking.ActualDepartureDate.HasValue
        || !booking.ArrivalDate.HasValue
        || !booking.DepartureDate.HasValue)
    {
        return false;
    }

    return booking.DepartureDate.Value > booking.ArrivalDate.Value;
}

bool IsPendingArrival(BookingStorageModel booking, DateOnly today, int lookAheadDays)
{
    if (!IsReservation(booking))
    {
        return false;
    }

    var latestArrival = today.AddDays(Math.Max(0, lookAheadDays));
    return today <= booking.ArrivalDate!.Value && booking.ArrivalDate.Value <= latestArrival;
}

bool IsPendingDeparture(BookingStorageModel booking, DateOnly today)
{
    if (booking.DeletedAt.HasValue
        || string.IsNullOrWhiteSpace(booking.RoomId)
        || IsNonBlockingBookingStatus(booking.Status)
        || !booking.ActualArrivalDate.HasValue
        || booking.ActualDepartureDate.HasValue
        || !booking.DepartureDate.HasValue
        || booking.DepartureDate.Value <= booking.ActualArrivalDate.Value)
    {
        return false;
    }

    return booking.DepartureDate.Value == today;
}

app.MapGet("/api/bookings", async (IKeyValueStore store, IEndpointQueryCache queryCache, int? year, bool? includeDeleted, DateOnly? from, DateOnly? to, string? clientId, string? groupCode) =>
{
    var showDeleted = includeDeleted == true;
    if (from.HasValue && to.HasValue && to.Value < from.Value)
    {
        (from, to) = (to, from);
    }

    var hasRange = from.HasValue && to.HasValue;
    var guestId = string.IsNullOrWhiteSpace(clientId) ? null : clientId.Trim();
    var group = string.IsNullOrWhiteSpace(groupCode) ? null : groupCode.Trim();
    var key = hasRange
        ? $"bookings:list:from:{from:yyyy-MM-dd}:to:{to:yyyy-MM-dd}:deleted:{showDeleted}:client:{guestId}:group:{group}"
        : guestId is not null
            ? $"bookings:list:client:{guestId}:deleted:{showDeleted}:group:{group}"
            : group is not null
                ? $"bookings:list:group:{group}:deleted:{showDeleted}"
                : year.HasValue
                    ? $"bookings:list:year:{year.Value}:deleted:{showDeleted}"
                    : $"bookings:list:all:deleted:{showDeleted}";
    var result = await queryCache.GetOrCreateAsync(key, new[] { "bookings" }, async () =>
    {
        var idx = await store.GetIndexAsync("bookings");
        var prefix = !hasRange && guestId is null && group is null && year.HasValue ? $"{year}_" : null;
        var earliestArrivalYear = hasRange ? from!.Value.Year - 1 : (int?)null;
        var latestArrivalYear = hasRange ? to!.Value.Year : (int?)null;
        var computed = new List<BookingDto>();
        foreach (var kvp in idx)
        {
            if (prefix != null && !kvp.Key.StartsWith(prefix)) continue;
            if (earliestArrivalYear.HasValue
                && latestArrivalYear.HasValue
                && FilingYearOutsideRange(kvp.Key, earliestArrivalYear.Value, latestArrivalYear.Value))
            {
                continue;
            }

            var b = await store.GetAsync<BookingStorageModel>("bookings", kvp.Value);
            if (b is null || (!showDeleted && b.DeletedAt.HasValue)) continue;
            if (guestId is not null && !string.Equals(b.ClientId, guestId, StringComparison.OrdinalIgnoreCase)) continue;
            if (group is not null && !string.Equals(b.GroupCode, group, StringComparison.OrdinalIgnoreCase)) continue;
            if (hasRange && !StayOverlapsRange(b, from!.Value, to!.Value)) continue;
            computed.Add(ToBookingDto(kvp.Value, b));
        }

        return computed;
    });

    return Results.Ok(result);
}).WithName("ListBookings");

app.MapGet("/api/bookings/stays", async (IKeyValueStore store, IEndpointQueryCache queryCache, int? year) =>
{
    var key = year.HasValue ? $"bookings:stays:year:{year.Value}" : "bookings:stays:all";
    var result = await queryCache.GetOrCreateAsync(key, new[] { "bookings" }, async () =>
    {
        var idx = await store.GetIndexAsync("bookings");
        var prefix = year.HasValue ? $"{year}_" : null;
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var computed = new List<BookingDto>();
        foreach (var kvp in idx)
        {
            if (prefix != null && !kvp.Key.StartsWith(prefix))
            {
                continue;
            }

            var booking = await store.GetAsync<BookingStorageModel>("bookings", kvp.Value);
            if (booking is null || !IsCurrentStay(booking, today))
            {
                continue;
            }

            computed.Add(ToBookingDto(kvp.Value, booking));
        }

        return computed;
    });

    return Results.Ok(result);
}).WithName("ListBookingStays");

app.MapGet("/api/bookings/reservations", async (IKeyValueStore store, IEndpointQueryCache queryCache, int? year) =>
{
    var key = year.HasValue ? $"bookings:reservations:year:{year.Value}" : "bookings:reservations:all";
    var result = await queryCache.GetOrCreateAsync(key, new[] { "bookings" }, async () =>
    {
        var idx = await store.GetIndexAsync("bookings");
        var prefix = year.HasValue ? $"{year}_" : null;
        var computed = new List<BookingDto>();
        foreach (var kvp in idx)
        {
            if (prefix != null && !kvp.Key.StartsWith(prefix))
            {
                continue;
            }

            var booking = await store.GetAsync<BookingStorageModel>("bookings", kvp.Value);
            if (booking is null || !IsReservation(booking))
            {
                continue;
            }

            computed.Add(ToBookingDto(kvp.Value, booking));
        }

        return computed;
    });

    return Results.Ok(result);
}).WithName("ListBookingReservations");

app.MapGet("/api/bookings/arrivals", async (IKeyValueStore store, IEndpointQueryCache queryCache, int? year, int? lookAheadDays) =>
{
    var effectiveLookAheadDays = Math.Max(0, lookAheadDays ?? 0);
    var key = year.HasValue
        ? $"bookings:arrivals:year:{year.Value}:lookahead:{effectiveLookAheadDays}"
        : $"bookings:arrivals:all:lookahead:{effectiveLookAheadDays}";

    var result = await queryCache.GetOrCreateAsync(key, new[] { "bookings" }, async () =>
    {
        var idx = await store.GetIndexAsync("bookings");
        var prefix = year.HasValue ? $"{year}_" : null;
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var computed = new List<BookingDto>();
        foreach (var kvp in idx)
        {
            if (prefix != null && !kvp.Key.StartsWith(prefix))
            {
                continue;
            }

            var booking = await store.GetAsync<BookingStorageModel>("bookings", kvp.Value);
            if (booking is null || !IsPendingArrival(booking, today, effectiveLookAheadDays))
            {
                continue;
            }

            computed.Add(ToBookingDto(kvp.Value, booking));
        }

        return computed;
    });

    return Results.Ok(result);
}).WithName("ListBookingArrivals");

app.MapGet("/api/bookings/departures", async (IKeyValueStore store, IEndpointQueryCache queryCache, int? year) =>
{
    var key = year.HasValue ? $"bookings:departures:year:{year.Value}" : "bookings:departures:all";
    var result = await queryCache.GetOrCreateAsync(key, new[] { "bookings" }, async () =>
    {
        var idx = await store.GetIndexAsync("bookings");
        var prefix = year.HasValue ? $"{year}_" : null;
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var computed = new List<BookingDto>();
        foreach (var kvp in idx)
        {
            if (prefix != null && !kvp.Key.StartsWith(prefix))
            {
                continue;
            }

            var booking = await store.GetAsync<BookingStorageModel>("bookings", kvp.Value);
            if (booking is null || !IsPendingDeparture(booking, today))
            {
                continue;
            }

            computed.Add(ToBookingDto(kvp.Value, booking));
        }

        return computed;
    });

    return Results.Ok(result);
}).WithName("ListBookingDepartures");

app.MapGet("/api/bookings/by-id/{id}", async (string id, IKeyValueStore store) =>
{
    var booking = await store.GetAsync<BookingStorageModel>("bookings", id);
    if (booking is null) return Results.NotFound();
    return Results.Ok(ToBookingDto(id, booking));
}).WithName("GetBookingById");

app.MapGet("/api/bookings/{year:int}/{id}", async (int year, string id, IKeyValueStore store) =>
{
    var b = await store.GetAsync<BookingStorageModel>("bookings", id);
    if (b is null || b.Year != year) return Results.NotFound();
    return Results.Ok(ToBookingDto(id, b));
}).WithName("GetBooking");

static bool FilingYearOutsideRange(string key, int earliestYear, int latestYear)
{
    var split = key.IndexOf('_');
    var token = split > 0 ? key[..split] : key;
    if (!int.TryParse(token, out var year))
    {
        return false;
    }

    return year < earliestYear || year > latestYear;
}

static bool StayOverlapsRange(BookingStorageModel booking, DateOnly from, DateOnly to)
{
    var arrival = booking.ArrivalDate ?? booking.ActualArrivalDate;
    if (arrival is null)
    {
        return booking.Year >= from.Year && booking.Year <= to.Year;
    }

    var departure = booking.DepartureDate ?? booking.ActualDepartureDate ?? arrival.Value;
    return arrival.Value <= to && departure >= from;
}

app.MapPost("/api/bookings", async (BookingDto request, IKeyValueStore store, IEndpointQueryCache queryCache, ITariffResolutionService resolver, IFinancialPostingService ledger, IAccountChart accountChart, ISystemConfigurationStore configurationStore) =>
{
    var validationError = ValidateBookingRequest(request, request.Year);
    if (validationError is not null)
        return Results.BadRequest(new { error = validationError });

    var guid = Guid.NewGuid().ToString("N");
    var key = $"{request.Year}_{guid}";
    var storage = new BookingStorageModel
    {
        Year = request.Year,
        ClientId = request.ClientId,
        RoomId = request.RoomId,
        ArrivalDate = request.ArrivalDate,
        DepartureDate = request.DepartureDate,
        Status = request.Status,
        Notes = request.Notes,
        ActualArrivalDate = request.ActualArrivalDate,
        ActualDepartureDate = request.ActualDepartureDate,
        ActualGuestCount = request.ActualGuestCount,
        PrepaidAmount = request.PrepaidAmount,
        PrepaidMethod = request.PrepaidMethod,
        PrepaidReference = request.PrepaidReference,
        ExpectedGuestCount = request.ExpectedGuestCount,
        PreAuthReference = request.PreAuthReference,
        PreAuthMethod = request.PreAuthMethod,
        Source = BlankToNull(request.Source),
        ExternalReference = BlankToNull(request.ExternalReference),
        GroupCode = BlankToNull(request.GroupCode),
        GroupName = BlankToNull(request.GroupName),
        RoomFixed = request.RoomFixed,
        ContinuedFromId = BlankToNull(request.ContinuedFromId),
        ContinuedToId = BlankToNull(request.ContinuedToId),
        EntryTime = request.EntryTime
    };
    await ResolveAndApplyTariffAsync(storage, resolver);
    var id = await store.CreateAsync("bookings", key, storage);

    decimal stayTotal = Convert.ToDecimal(storage.TariffTotalPrice ?? 0);
    decimal extraTotal = 0m;
    var costIndex = await store.GetIndexAsync("booking_costs");
    foreach (var entry in costIndex)
    {
        var cost = await store.GetAsync<BookingCostStorageModel>("booking_costs", entry.Value);
        if (cost is not null && !cost.DeletedAt.HasValue && string.Equals(cost.BookingId, id, StringComparison.OrdinalIgnoreCase))
            extraTotal += Convert.ToDecimal(cost.Amount ?? 0);
    }

    var when = DateTime.UtcNow;
    var chart = await accountChart.CurrentAsync();
    var tax = await CurrentTaxAsync(configurationStore);
    if (stayTotal > 0)
        await ledger.PostGuestChargeAsync(when, "booking_stay_charge", "Stay charge on booking creation", stayTotal, tax.Stay, tax.Inclusive, chart.GuestLedger, "asset", chart.RoomRevenue, "revenue", chart.OutputTax, bookingId: id, sourceType: "booking", sourceId: id);
    if (extraTotal > 0)
        await ledger.PostGuestChargeAsync(when, "booking_extra_charge", "Extras on booking creation", extraTotal, tax.Product ?? tax.Stay, tax.Inclusive, chart.GuestLedger, "asset", chart.OtherIncome, "revenue", chart.OutputTax, bookingId: id, sourceType: "booking", sourceId: id);
    var stayNights = storage.ArrivalDate.HasValue && storage.DepartureDate.HasValue
        ? storage.DepartureDate.Value.DayNumber - storage.ArrivalDate.Value.DayNumber
        : 0;
    var cityTax = StayTax.CityTax(stayNights, storage.ActualGuestCount ?? storage.ExpectedGuestCount ?? 1, tax.CityBasis, tax.CityAmount);
    if (cityTax > 0)
        await ledger.PostBalancedAsync(when, "booking_city_tax", "City tax", cityTax, chart.GuestLedger, "asset", chart.CityTax, "liability", bookingId: id, sourceType: "booking", sourceId: id);

    var prepaid = Convert.ToDecimal(request.PrepaidAmount ?? 0);
    if (prepaid > 0)
        await ledger.PostBalancedAsync(when, "booking_prepaid", "Prepaid amount on booking creation", prepaid, chart.Cash, "asset", chart.GuestDeposits, "liability", bookingId: id, sourceType: "booking", sourceId: id);

    queryCache.InvalidateTag("bookings");
    return Results.Created($"/api/bookings/{request.Year}/{id}", ToBookingDto(id, storage));
}).WithName("CreateBooking");

app.MapPut("/api/bookings/{year:int}/{id}", async (int year, string id, BookingDto request, IKeyValueStore store, IEndpointQueryCache queryCache, ITariffResolutionService resolver, IFinancialPostingService ledger, ILedgerRepository ledgerReader, IAccountChart accountChart) =>
{
    var b = await store.GetAsync<BookingStorageModel>("bookings", id);
    if (b is null || b.Year != year) return Results.NotFound();

    var validationError = ValidateBookingRequest(request, year);
    if (validationError is not null)
        return Results.BadRequest(new { error = validationError });

    var storage = new BookingStorageModel
    {
        Year = year,
        ClientId = request.ClientId,
        RoomId = request.RoomId,
        ArrivalDate = request.ArrivalDate,
        DepartureDate = request.DepartureDate,
        Status = request.Status,
        Notes = request.Notes,
        ActualArrivalDate = request.ActualArrivalDate,
        ActualDepartureDate = request.ActualDepartureDate,
        ActualGuestCount = request.ActualGuestCount,
        PrepaidAmount = request.PrepaidAmount,
        PrepaidMethod = request.PrepaidMethod,
        PrepaidReference = request.PrepaidReference,
        ExpectedGuestCount = request.ExpectedGuestCount,
        PreAuthReference = request.PreAuthReference,
        PreAuthMethod = request.PreAuthMethod,
        Source = BlankToNull(request.Source),
        ExternalReference = BlankToNull(request.ExternalReference),
        GroupCode = BlankToNull(request.GroupCode),
        GroupName = BlankToNull(request.GroupName),
        RoomFixed = request.RoomFixed,
        ContinuedFromId = b.ContinuedFromId,
        ContinuedToId = b.ContinuedToId,
        DeletedAt = b.DeletedAt,
        EntryTime = request.EntryTime ?? b.EntryTime,
        PrintedInvoiceNumber = b.PrintedInvoiceNumber,
        IssuedInvoiceNumber = b.IssuedInvoiceNumber,
        GuestFileStatus = request.GuestFileStatus ?? b.GuestFileStatus,
        GuestFileReceipt = request.GuestFileReceipt ?? b.GuestFileReceipt
    };
    await ResolveAndApplyTariffAsync(storage, resolver);
    await store.UpdateAsync("bookings", id, storage);

    var chart = await accountChart.CurrentAsync();
    var oldPrepaid = Convert.ToDecimal(b.PrepaidAmount ?? 0);
    var newPrepaid = Convert.ToDecimal(request.PrepaidAmount ?? 0);
    var prepaidDelta = newPrepaid - oldPrepaid;
    if (prepaidDelta != 0)
    {
        var whenUpd = DateTime.UtcNow;
        var desc = prepaidDelta > 0 ? "Prepaid amount increased" : "Prepaid amount decreased";
        await ledger.PostBalancedAsync(whenUpd, "booking_prepaid", desc, Math.Abs(prepaidDelta),
            prepaidDelta > 0 ? chart.Cash : chart.GuestDeposits,
            prepaidDelta > 0 ? "asset" : "liability",
            prepaidDelta > 0 ? chart.GuestDeposits : chart.Cash,
            prepaidDelta > 0 ? "liability" : "asset",
            bookingId: id, sourceType: "booking", sourceId: id);
    }

    if (b.ActualDepartureDate == null && request.ActualDepartureDate.HasValue)
    {
        var openLines = await ledgerReader.GetEntriesForBookingAsync(id, DateTime.MinValue, DateTime.MaxValue);
        var held = openLines.Where(line => line.AccountCode == chart.GuestDeposits).Sum(line => line.Credit - line.Debit);
        if (held > 0)
        {
            await ledger.PostBalancedAsync(DateTime.UtcNow, "booking_deposit_application", "Deposit applied on checkout", held,
                chart.GuestDeposits, "liability", chart.GuestLedger, "asset", bookingId: id, sourceType: "booking", sourceId: id);
        }
    }

    queryCache.InvalidateTag("bookings");
    return Results.Ok(ToBookingDto(id, storage));
}).WithName("UpdateBooking");

app.MapGet("/api/stay-groups", async (IKeyValueStore store, IEndpointQueryCache queryCache) =>
{
    var result = await queryCache.GetOrCreateAsync("stay-groups:list", new[] { "bookings" }, async () =>
    {
        var named = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var (_, id) in await store.GetIndexAsync("stay_groups"))
        {
            var group = await store.GetAsync<StayGroupStorageModel>("stay_groups", id);
            if (group is null || string.IsNullOrWhiteSpace(group.Name))
                continue;
            named[id] = group.Name.Trim();
        }

        var stats = new Dictionary<string, (DateOnly? First, DateOnly? Last, int Count)>(StringComparer.OrdinalIgnoreCase);
        foreach (var (_, id) in await store.GetIndexAsync("bookings"))
        {
            var booking = await store.GetAsync<BookingStorageModel>("bookings", id);
            if (booking is null || booking.DeletedAt.HasValue || string.IsNullOrWhiteSpace(booking.GroupCode))
                continue;
            var code = booking.GroupCode.Trim();
            stats.TryGetValue(code, out var current);
            var first = current.First;
            var last = current.Last;
            if (booking.ArrivalDate.HasValue && (first is null || booking.ArrivalDate.Value < first))
                first = booking.ArrivalDate;
            if (booking.DepartureDate.HasValue && (last is null || booking.DepartureDate.Value > last))
                last = booking.DepartureDate;
            stats[code] = (first, last, current.Count + 1);
        }

        var rows = new List<StayGroupDto>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var (id, name) in named)
        {
            stats.TryGetValue(id, out var span);
            rows.Add(new StayGroupDto(id, name, span.First, span.Last, span.Count));
            seen.Add(id);
        }

        foreach (var (code, span) in stats)
        {
            if (!seen.Add(code))
                continue;
            rows.Add(new StayGroupDto(code, code, span.First, span.Last, span.Count));
        }

        return rows
            .OrderByDescending(row => row.FirstArrival ?? DateOnly.MinValue)
            .ThenBy(row => row.Name)
            .ToList();
    });
    return Results.Ok(result);
}).WithName("ListStayGroups");

app.MapPost("/api/stay-groups", async (StayGroupRequest request, IKeyValueStore store, IEndpointQueryCache queryCache) =>
{
    var name = request.Name?.Trim();
    if (string.IsNullOrWhiteSpace(name))
        return Results.BadRequest(new { error = "A group needs a name." });
    if (name.Length > 80)
        return Results.BadRequest(new { error = "The group name is too long." });

    var id = await store.CreateAsync("stay_groups", name, new StayGroupStorageModel { Name = name }, indexByDocumentId: true);
    queryCache.InvalidateTag("bookings");
    return Results.Created($"/api/stay-groups/{id}", new StayGroupDto(id, name, null, null, 0));
}).WithName("CreateStayGroup");

app.MapPost("/api/bookings/groups/{code}/confirm", async (string code, IKeyValueStore store, IEndpointQueryCache queryCache) =>
{
    if (string.IsNullOrWhiteSpace(code))
        return Results.BadRequest(new { error = "Group code is required" });

    var index = await store.GetIndexAsync("bookings");
    var members = new List<(string Id, BookingStorageModel Booking)>();
    foreach (var (_, id) in index)
    {
        var booking = await store.GetAsync<BookingStorageModel>("bookings", id);
        if (booking is null || booking.DeletedAt.HasValue || !SameGroup(booking.GroupCode, code) || ClosedBooking(booking.Status))
            continue;
        members.Add((id, booking));
    }

    if (members.Count == 0)
        return Results.NotFound(new { error = "No bookings in this group" });

    if (members.Any(member => string.IsNullOrWhiteSpace(member.Booking.RoomId)))
        return Results.Conflict(new { error = "Every room in the group must be assigned before the group is confirmed." });

    foreach (var (id, booking) in members)
    {
        if (string.IsNullOrWhiteSpace(booking.Status) || booking.Status.Equals("Incoming", StringComparison.OrdinalIgnoreCase))
        {
            booking.Status = "Confirmed";
            await store.UpdateAsync("bookings", id, booking);
        }
    }

    queryCache.InvalidateTag("bookings");
    return Results.Ok(new { confirmed = members.Count });
});

app.MapPost("/api/bookings/{year:int}/{id}/move", async (
    int year,
    string id,
    BookingMoveRequest request,
    IKeyValueStore store,
    IEndpointQueryCache queryCache,
    IFinancialPostingService ledger,
    ILedgerRepository ledgerReader,
    IAccountChart accountChart,
    ISystemConfigurationStore configurationStore) =>
{
    var origin = await store.GetAsync<BookingStorageModel>("bookings", id);
    if (origin is null || origin.Year != year)
        return Results.NotFound();
    if (origin.ArrivalDate is null || origin.DepartureDate is null || string.IsNullOrWhiteSpace(origin.RoomId))
        return Results.BadRequest(new { error = "The stay needs a room, an arrival, and a departure before it can move." });
    if (!string.IsNullOrWhiteSpace(origin.ContinuedToId))
        return Results.Conflict(new { error = "This stay has already moved." });
    if (origin.ActualDepartureDate.HasValue || ClosedBooking(origin.Status) || string.Equals(origin.Status, "CheckedOut", StringComparison.OrdinalIgnoreCase) || string.Equals(origin.Status, "Maintenance", StringComparison.OrdinalIgnoreCase))
        return Results.BadRequest(new { error = "This stay cannot move." });
    if (string.IsNullOrWhiteSpace(request.RoomId))
        return Results.BadRequest(new { error = "Choose the room the guest moves to." });
    if (string.Equals(request.RoomId, origin.RoomId, StringComparison.OrdinalIgnoreCase))
        return Results.BadRequest(new { error = "Choose a different room." });

    var from = request.FromDate;
    if (from <= origin.ArrivalDate.Value || from >= origin.DepartureDate.Value)
        return Results.BadRequest(new { error = "The move night has to leave at least one night in each room." });

    var destination = await store.GetAsync<RoomStorageModel>("rooms", request.RoomId);
    if (destination is null)
        return Results.BadRequest(new { error = "The room was not found." });
    if (await RoomTakenAsync(store, request.RoomId, from, origin.DepartureDate.Value, id))
        return Results.Conflict(new { error = "That room is not free for the nights after the move." });

    var allNights = origin.DepartureDate.Value.DayNumber - origin.ArrivalDate.Value.DayNumber;
    var keptNights = from.DayNumber - origin.ArrivalDate.Value.DayNumber;
    var movedNights = allNights - keptNights;
    var price = Convert.ToDecimal(origin.TariffTotalPrice ?? 0);
    var keptPrice = StayTax.Portion(price, keptNights, allNights);
    var movedPrice = price - keptPrice;
    var oldRoom = await store.GetAsync<RoomStorageModel>("rooms", origin.RoomId);
    var oldRoomName = string.IsNullOrWhiteSpace(oldRoom?.Name) ? origin.RoomId : oldRoom.Name;
    var newRoomName = string.IsNullOrWhiteSpace(destination.Name) ? request.RoomId : destination.Name;
    var reason = string.IsNullOrWhiteSpace(request.Reason) ? "Relocate" : request.Reason.Trim();
    var extraNote = string.IsNullOrWhiteSpace(request.Note) ? string.Empty : " " + request.Note.Trim();
    var movingNow = origin.ActualArrivalDate.HasValue && !origin.ActualDepartureDate.HasValue && from <= DateOnly.FromDateTime(DateTime.UtcNow);

    var prepaid = Convert.ToDecimal(origin.PrepaidAmount ?? 0);
    var keepPrepaid = Math.Min(prepaid, Math.Max(keptPrice, 0));
    var carryPrepaid = prepaid - keepPrepaid;

    var continued = new BookingStorageModel
    {
        Year = from.Year,
        ClientId = origin.ClientId,
        RoomId = request.RoomId,
        ArrivalDate = from,
        DepartureDate = origin.DepartureDate,
        Status = movingNow ? "CheckedIn" : "Confirmed",
        ActualArrivalDate = movingNow ? from : null,
        ActualGuestCount = movingNow ? origin.ActualGuestCount : null,
        ExpectedGuestCount = origin.ExpectedGuestCount,
        TariffId = origin.TariffId,
        TariffTotalPrice = (double)movedPrice,
        Source = origin.Source,
        ExternalReference = origin.ExternalReference,
        GroupCode = origin.GroupCode,
        GroupName = origin.GroupName,
        RoomFixed = origin.RoomFixed,
        ContinuedFromId = id,
        PrepaidAmount = (double)carryPrepaid,
        Notes = $"Continued from {oldRoomName} on {from:yyyy-MM-dd}. {reason}.{extraNote}".Trim()
    };
    var newId = await store.CreateAsync("bookings", $"{from.Year}_{Guid.NewGuid():N}", continued);

    var oldNote = string.IsNullOrWhiteSpace(origin.Notes) ? string.Empty : origin.Notes.Trim() + " ";
    origin.Notes = $"{oldNote}Moved to {newRoomName} on {from:yyyy-MM-dd}. The stay continues. {reason}.{extraNote}".Trim();
    origin.DepartureDate = from;
    origin.TariffTotalPrice = (double)keptPrice;
    origin.ContinuedToId = newId;
    origin.PrepaidAmount = (double)keepPrepaid;
    if (movingNow)
    {
        origin.Status = "CheckedOut";
        origin.ActualDepartureDate = from;
    }
    await store.UpdateAsync("bookings", id, origin);
    await CopyStayGuestsAsync(store, id, newId, from.Year);

    var chart = await accountChart.CurrentAsync();
    var tax = await CurrentTaxAsync(configurationStore);
    var when = DateTime.UtcNow;
    var posted = await ledgerReader.GetEntriesForBookingAsync(id, DateTime.MinValue, DateTime.MaxValue);
    if (movedPrice > 0)
    {
        var stayLines = posted.Where(line => string.Equals(line.EntryType, "booking_stay_charge", StringComparison.OrdinalIgnoreCase)).ToList();
        var postedTax = stayLines.Where(line => line.AccountCode == chart.OutputTax).Sum(line => line.Credit - line.Debit);
        var postedGross = stayLines.Where(line => line.AccountCode == chart.GuestLedger).Sum(line => line.Debit - line.Credit);
        if (postedGross > 0)
        {
            var taxPart = postedGross == 0 ? 0 : StayTax.Round(postedTax * movedPrice / postedGross);
            if (taxPart < 0) taxPart = 0;
            if (taxPart > movedPrice) taxPart = movedPrice;
            var netPart = movedPrice - taxPart;
            var moveText = $"Guest moved from {oldRoomName} to {newRoomName}. Payment carried forward.";
            if (taxPart == 0)
                await ledger.PostBalancedAsync(when, "booking_stay_charge", moveText, movedPrice, chart.RoomRevenue, "revenue", chart.GuestLedger, "asset", bookingId: id, sourceType: "booking", sourceId: id);
            else if (netPart == 0)
                await ledger.PostBalancedAsync(when, "booking_stay_charge", moveText, taxPart, chart.OutputTax, "liability", chart.GuestLedger, "asset", bookingId: id, sourceType: "booking", sourceId: id);
            else
                await ledger.RelieveGuestChargeAsync(when, "booking_stay_charge", moveText, netPart, taxPart / netPart * 100m, false, chart.GuestLedger, "asset", chart.RoomRevenue, "revenue", chart.OutputTax, bookingId: id, sourceType: "booking", sourceId: id);
        }

        await ledger.PostGuestChargeAsync(when, "booking_stay_charge", $"Continued from {oldRoomName}. Payment carried forward.", movedPrice, tax.Stay, tax.Inclusive, chart.GuestLedger, "asset", chart.RoomRevenue, "revenue", chart.OutputTax, bookingId: newId, sourceType: "booking", sourceId: newId);
    }

    var cityLines = posted.Where(line => string.Equals(line.EntryType, "booking_city_tax", StringComparison.OrdinalIgnoreCase)).ToList();
    var cityGross = cityLines.Where(line => line.AccountCode == chart.CityTax).Sum(line => line.Credit - line.Debit);
    if (cityGross > 0 && movedNights > 0)
    {
        var cityMoved = cityGross - StayTax.Portion(cityGross, keptNights, allNights);
        if (cityMoved > 0)
        {
            await ledger.PostBalancedAsync(when, "booking_city_tax", $"City tax moved to {newRoomName}", cityMoved, chart.CityTax, "liability", chart.GuestLedger, "asset", bookingId: id, sourceType: "booking", sourceId: id);
            await ledger.PostBalancedAsync(when, "booking_city_tax", $"City tax continued from {oldRoomName}", cityMoved, chart.GuestLedger, "asset", chart.CityTax, "liability", bookingId: newId, sourceType: "booking", sourceId: newId);
        }
    }

    if (carryPrepaid > 0)
    {
        var deposits = (await ledgerReader.GetEntriesForBookingAsync(id, DateTime.MinValue, DateTime.MaxValue))
            .Where(line => line.AccountCode == chart.GuestDeposits)
            .Sum(line => line.Credit - line.Debit);
        var depositCarry = Math.Min(carryPrepaid, Math.Max(deposits, 0));
        if (depositCarry > 0)
            await ledger.PostTransferAsync(when, "booking_prepaid", $"Deposit carried from {oldRoomName} to {newRoomName}", depositCarry, chart.GuestDeposits, "liability", id, newId, "booking", newId);
    }

    if (movingNow)
    {
        var held = (await ledgerReader.GetEntriesForBookingAsync(id, DateTime.MinValue, DateTime.MaxValue))
            .Where(line => line.AccountCode == chart.GuestDeposits)
            .Sum(line => line.Credit - line.Debit);
        if (held > 0)
            await ledger.PostBalancedAsync(when, "booking_deposit_application", "Deposit applied on move", held, chart.GuestDeposits, "liability", chart.GuestLedger, "asset", bookingId: id, sourceType: "booking", sourceId: id);
    }

    var due = (await ledgerReader.GetEntriesForBookingAsync(id, DateTime.MinValue, DateTime.MaxValue))
        .Where(line => line.AccountCode == chart.GuestLedger)
        .Sum(line => line.Debit - line.Credit);
    if (due < 0)
        await ledger.PostTransferAsync(when, "booking_payment", $"Payment carried from {oldRoomName} to {newRoomName}", -due, chart.GuestLedger, "asset", id, newId, "booking", newId);

    queryCache.InvalidateTag("bookings");
    return Results.Ok(ToBookingDto(newId, continued));
}).WithName("MoveBooking");

static async Task<bool> RoomTakenAsync(IKeyValueStore store, string roomId, DateOnly from, DateOnly to, string exceptId)
{
    var index = await store.GetIndexAsync("bookings");
    foreach (var (_, bookingId) in index)
    {
        if (string.Equals(bookingId, exceptId, StringComparison.OrdinalIgnoreCase))
            continue;
        var booking = await store.GetAsync<BookingStorageModel>("bookings", bookingId);
        if (booking is null || booking.DeletedAt.HasValue || !string.Equals(booking.RoomId, roomId, StringComparison.OrdinalIgnoreCase))
            continue;
        if (ClosedBooking(booking.Status) || string.Equals(booking.Status, "NoShow", StringComparison.OrdinalIgnoreCase))
            continue;
        var arrival = booking.ArrivalDate ?? booking.ActualArrivalDate;
        var departure = booking.DepartureDate ?? booking.ActualDepartureDate;
        if (arrival is null || departure is null)
            continue;
        if (arrival.Value < to && departure.Value > from)
            return true;
    }

    return false;
}

static async Task CopyStayGuestsAsync(IKeyValueStore store, string fromId, string toId, int year)
{
    var index = await store.GetIndexAsync("booking_guests");
    foreach (var (_, guestId) in index)
    {
        var guest = await store.GetAsync<BookingGuestStorageModel>("booking_guests", guestId);
        if (guest is null || !string.Equals(guest.BookingId, fromId, StringComparison.OrdinalIgnoreCase))
            continue;
        await store.CreateAsync("booking_guests", $"{year}_{Guid.NewGuid():N}", new BookingGuestStorageModel
        {
            Year = year,
            BookingId = toId,
            ClientId = guest.ClientId,
            GuestNumber = guest.GuestNumber
        });
    }
}

static bool SameGroup(string? stored, string code) =>
    !string.IsNullOrWhiteSpace(stored) && stored.Equals(code, StringComparison.OrdinalIgnoreCase);

static bool ClosedBooking(string? status) =>
    status is not null && (
        status.Equals("Cancelled", StringComparison.OrdinalIgnoreCase)
        || status.Equals("Void", StringComparison.OrdinalIgnoreCase)
        || status.Equals("Deleted", StringComparison.OrdinalIgnoreCase));

app.MapDelete("/api/bookings/{year:int}/{id}", async (int year, string id, IKeyValueStore store, IEndpointQueryCache queryCache, ILedgerRepository ledger) =>
{
    var b = await store.GetAsync<BookingStorageModel>("bookings", id);
    if (b is null || b.Year != year) return Results.NotFound();
    if (b.DeletedAt.HasValue)
        return Results.NoContent();

    var cited = await ledger.AnyMatchingAsync(line => string.Equals(line.BookingId, id, StringComparison.OrdinalIgnoreCase));
    if (cited)
    {
        b.DeletedAt = DateTime.UtcNow;
        await store.UpdateAsync("bookings", id, b);
    }
    else
    {
        await store.DeleteAsync("bookings", id);
    }

    queryCache.InvalidateTag("bookings");
    return Results.NoContent();
}).WithName("DeleteBooking");

app.MapPost("/api/bookings/{year:int}/{id}/cancel", async (int year, string id, IKeyValueStore store, IEndpointQueryCache queryCache, ILedgerRepository ledger) =>
{
    var b = await store.GetAsync<BookingStorageModel>("bookings", id);
    if (b is null || b.Year != year) return Results.NotFound();
    var cancelGuid = Guid.NewGuid().ToString("N");
    var cancelKey = $"{year}_{cancelGuid}";
    var cancelled = new CancelledBookingStorageModel { Year = year, ClientId = b.ClientId, RoomId = b.RoomId, ArrivalDate = b.ArrivalDate, DepartureDate = b.DepartureDate, Status = b.Status, CancelledAt = DateTime.UtcNow, Notes = b.Notes, BookingId = id };
    await store.CreateAsync("cancelled_bookings", cancelKey, cancelled);
    var cited = await ledger.AnyMatchingAsync(line => string.Equals(line.BookingId, id, StringComparison.OrdinalIgnoreCase));
    if (cited)
    {
        b.Status = "Cancelled";
        b.DeletedAt = DateTime.UtcNow;
        await store.UpdateAsync("bookings", id, b);
    }
    else
    {
        await store.DeleteAsync("bookings", id);
    }

    queryCache.InvalidateTag("bookings");
    queryCache.InvalidateTag("cancelled_bookings");
    return Results.NoContent();
}).WithName("CancelBooking");

app.MapPost("/api/bookings/{year:int}/{id}/check-in", async (int year, string id, BookingCheckInRequest request, IKeyValueStore store, IEndpointQueryCache queryCache, ITariffResolutionService resolver, ISystemConfigurationStore configurationStore) =>
{
    var booking = await store.GetAsync<BookingStorageModel>("bookings", id);
    if (booking is null || booking.Year != year)
        return Results.NotFound();

    if (!request.ActualArrivalDate.HasValue)
        return Results.BadRequest(new { error = "ActualArrivalDate is required" });
    if (!request.ActualGuestCount.HasValue || request.ActualGuestCount.Value <= 0)
        return Results.BadRequest(new { error = "ActualGuestCount must be greater than zero" });
    if (!booking.DepartureDate.HasValue || request.ActualArrivalDate.Value >= booking.DepartureDate.Value)
        return Results.BadRequest(new { error = "ActualArrivalDate must be before DepartureDate" });
    if (booking.ActualDepartureDate.HasValue)
        return Results.BadRequest(new { error = "Cannot check in a booking that is already checked out" });

    booking.ActualArrivalDate = request.ActualArrivalDate.Value;
    booking.ActualGuestCount = request.ActualGuestCount.Value;
    booking.Status = string.IsNullOrWhiteSpace(request.Status) ? "CheckedIn" : request.Status;

    // Business rule (persisted in SystemConfiguration.Settings["TariffAutoCorrectOnCheckIn"], enabled by default):
    // when the guest turns out to be more people than expected at booking time, re-resolve the tariff
    // so higher-occupancy per-person pricing is applied instead of keeping the originally quoted rate.
    if (booking.ActualGuestCount.Value > (booking.ExpectedGuestCount ?? 0))
    {
        var config = await configurationStore.GetAsync();
        var autoCorrectEnabled = !(config?.Settings is not null
            && config.Settings.TryGetValue("TariffAutoCorrectOnCheckIn", out var flag)
            && bool.TryParse(flag, out var parsedFlag)
            && !parsedFlag);

        if (autoCorrectEnabled)
        {
            await ResolveAndApplyTariffAsync(booking, resolver);
        }
    }

    await store.UpdateAsync("bookings", id, booking);
    queryCache.InvalidateTag("bookings");
    return Results.Ok(ToBookingDto(id, booking));
}).WithName("CheckInBooking");

app.MapPost("/api/bookings/{year:int}/{id}/check-out", async (int year, string id, BookingCheckOutRequest request, IKeyValueStore store, IEndpointQueryCache queryCache) =>
{
    var booking = await store.GetAsync<BookingStorageModel>("bookings", id);
    if (booking is null || booking.Year != year)
        return Results.NotFound();

    if (!request.ActualDepartureDate.HasValue)
        return Results.BadRequest(new { error = "ActualDepartureDate is required" });
    if (!booking.ActualArrivalDate.HasValue)
        return Results.BadRequest(new { error = "ActualArrivalDate is required when checking out" });
    if (request.ActualDepartureDate.Value < booking.ActualArrivalDate.Value)
        return Results.BadRequest(new { error = "ActualDepartureDate must be on or after ActualArrivalDate" });
    if (booking.DepartureDate.HasValue && request.ActualDepartureDate.Value > booking.DepartureDate.Value)
        return Results.BadRequest(new { error = "ActualDepartureDate cannot be after DepartureDate" });

    booking.ActualDepartureDate = request.ActualDepartureDate.Value;
    booking.Status = string.IsNullOrWhiteSpace(request.Status) ? "CheckedOut" : request.Status;

    await store.UpdateAsync("bookings", id, booking);
    queryCache.InvalidateTag("bookings");
    return Results.Ok(ToBookingDto(id, booking));
}).WithName("CheckOutBooking");

app.MapPost("/api/bookings/{year:int}/{id}/no-show", async (int year, string id, BookingNoShowRequest request, IKeyValueStore store, IEndpointQueryCache queryCache) =>
{
    var booking = await store.GetAsync<BookingStorageModel>("bookings", id);
    if (booking is null || booking.Year != year)
        return Results.NotFound();

    if (booking.ActualArrivalDate.HasValue)
        return Results.BadRequest(new { error = "Cannot mark no-show after check-in" });
    if (string.IsNullOrWhiteSpace(request.Notes))
        return Results.BadRequest(new { error = "Notes are required for no-show" });

    booking.Status = string.IsNullOrWhiteSpace(request.Status) ? "NoShow" : request.Status;
    booking.Notes = request.Notes;

    await store.UpdateAsync("bookings", id, booking);
    queryCache.InvalidateTag("bookings");
    return Results.Ok(ToBookingDto(id, booking));
}).WithName("MarkBookingNoShow");

// --- Layer 4: BookingCosts (costiprenotazione{year}) ---

BookingCostDto ToBookingCostDto(string id, BookingCostStorageModel s) => new(id, s.Year, s.BookingId, s.TariffId, s.Amount, s.Description);

app.MapGet("/api/booking-costs", async (IKeyValueStore store, int? year) =>
{
    var idx = await store.GetIndexAsync("booking_costs");
    var prefix = year.HasValue ? $"{year}_" : null;
    var result = new List<BookingCostDto>();
    foreach (var kvp in idx)
    {
        if (prefix != null && !kvp.Key.StartsWith(prefix)) continue;
        var c = await store.GetAsync<BookingCostStorageModel>("booking_costs", kvp.Value);
        if (c is null || c.DeletedAt.HasValue) continue;
        result.Add(ToBookingCostDto(kvp.Value, c));
    }
    return Results.Ok(result);
}).WithName("ListBookingCosts");

app.MapGet("/api/booking-costs/{year:int}/{id}", async (int year, string id, IKeyValueStore store) =>
{
    var c = await store.GetAsync<BookingCostStorageModel>("booking_costs", id);
    if (c is null || c.Year != year) return Results.NotFound();
    return Results.Ok(ToBookingCostDto(id, c));
}).WithName("GetBookingCost");

app.MapPost("/api/booking-costs", async (BookingCostDto request, IKeyValueStore store, IFinancialPostingService ledger, IAccountChart accountChart, ISystemConfigurationStore configurationStore) =>
{
    var guid = Guid.NewGuid().ToString("N");
    var key = $"{request.Year}_{guid}";
    var storage = new BookingCostStorageModel { Year = request.Year, BookingId = request.BookingId, TariffId = request.TariffId, Amount = request.Amount, Description = request.Description };
    var id = await store.CreateAsync("booking_costs", key, storage);
    var sold = Convert.ToDecimal(request.Amount ?? 0);
    if (sold > 0)
    {
        var tax = await CurrentTaxAsync(configurationStore);
        await PostExtraCostDeltaAsync(ledger, await accountChart.CurrentAsync(), tax, request.BookingId, sold, request.Description ?? "Extra cost", id, tax.Product ?? tax.Stay);
    }
    return Results.Created($"/api/booking-costs/{request.Year}/{id}", ToBookingCostDto(id, storage));
}).WithName("CreateBookingCost");

app.MapPut("/api/booking-costs/{year:int}/{id}", async (int year, string id, BookingCostDto request, IKeyValueStore store, IFinancialPostingService ledger, IAccountChart accountChart, ISystemConfigurationStore configurationStore) =>
{
    var c = await store.GetAsync<BookingCostStorageModel>("booking_costs", id);
    if (c is null || c.Year != year || c.DeletedAt.HasValue) return Results.NotFound();
    var oldAmount = Convert.ToDecimal(c.Amount ?? 0);
    var newAmount = Convert.ToDecimal(request.Amount ?? 0);
    var storage = new BookingCostStorageModel { Year = year, BookingId = request.BookingId, TariffId = request.TariffId, Amount = request.Amount, Description = request.Description };
    await store.UpdateAsync("booking_costs", id, storage);
    var description = request.Description ?? c.Description ?? "Extra cost";
    var chart = await accountChart.CurrentAsync();
    var tax = await CurrentTaxAsync(configurationStore);
    var rate = tax.Product ?? tax.Stay;
    if (string.Equals(c.BookingId, request.BookingId, StringComparison.OrdinalIgnoreCase))
        await PostExtraCostDeltaAsync(ledger, chart, tax, request.BookingId, newAmount - oldAmount, description, id, rate);
    else
    {
        await PostExtraCostDeltaAsync(ledger, chart, tax, c.BookingId, -oldAmount, description, id, rate);
        await PostExtraCostDeltaAsync(ledger, chart, tax, request.BookingId, newAmount, description, id, rate);
    }
    return Results.Ok(ToBookingCostDto(id, storage));
}).WithName("UpdateBookingCost");

app.MapDelete("/api/booking-costs/{year:int}/{id}", async (int year, string id, IKeyValueStore store, ILedgerRepository ledger, IFinancialPostingService posting, IAccountChart accountChart, ISystemConfigurationStore configurationStore) =>
{
    var c = await store.GetAsync<BookingCostStorageModel>("booking_costs", id);
    if (c is null || c.Year != year) return Results.NotFound();
    if (c.DeletedAt.HasValue)
        return Results.NoContent();

    var removedTax = await CurrentTaxAsync(configurationStore);
    await PostExtraCostDeltaAsync(posting, await accountChart.CurrentAsync(), removedTax, c.BookingId, -Convert.ToDecimal(c.Amount ?? 0), c.Description ?? "Extra cost removed", id, removedTax.Product ?? removedTax.Stay);
    var cited = await ledger.AnyMatchingAsync(line =>
        string.Equals(line.SourceType, "booking-cost", StringComparison.OrdinalIgnoreCase)
        && string.Equals(line.SourceId, id, StringComparison.OrdinalIgnoreCase));
    if (cited)
    {
        c.DeletedAt = DateTime.UtcNow;
        await store.UpdateAsync("booking_costs", id, c);
    }
    else
    {
        await store.DeleteAsync("booking_costs", id);
    }
    return Results.NoContent();
}).WithName("DeleteBookingCost");

static bool IsPosFolioStay(BookingStorageModel booking)
{
    if (booking.DeletedAt.HasValue
        || !booking.ActualArrivalDate.HasValue
        || booking.ActualDepartureDate.HasValue
        || !booking.DepartureDate.HasValue
        || booking.DepartureDate.Value <= booking.ActualArrivalDate.Value)
        return false;

    var status = booking.Status ?? "";
    return !status.Contains("cancel", StringComparison.OrdinalIgnoreCase)
        && !status.Contains("void", StringComparison.OrdinalIgnoreCase)
        && !status.Contains("deleted", StringComparison.OrdinalIgnoreCase)
        && !status.Contains("no-show", StringComparison.OrdinalIgnoreCase)
        && !status.Contains("noshow", StringComparison.OrdinalIgnoreCase);
}

static async Task<(string? Id, bool Missing)> ResolvePosRegisterAsync(IKeyValueStore store, string? requested)
{
    if (!string.IsNullOrWhiteSpace(requested))
    {
        var found = await store.GetAsync<CashRegisterStorageModel>("cash_registers", requested);
        return found is null ? (null, true) : (requested, false);
    }

    var index = await store.GetIndexAsync("cash_registers");
    string? first = null;
    string? main = null;
    foreach (var pair in index)
    {
        first ??= pair.Value;
        var register = await store.GetAsync<CashRegisterStorageModel>("cash_registers", pair.Value);
        if (register is null) continue;
        var name = register.Name ?? "";
        if (string.Equals(register.Code, "1", StringComparison.OrdinalIgnoreCase)
            || name.Contains("main", StringComparison.OrdinalIgnoreCase)
            || name.Contains("princip", StringComparison.OrdinalIgnoreCase))
            main = pair.Value;
    }

    return (main ?? first, false);
}

static async Task<string> PosDefaultCurrencyAsync(ISystemConfigurationStore configurationStore)
{
    var config = await configurationStore.GetAsync();
    return config?.DefaultCurrency
        ?? config?.Settings?.GetValueOrDefault("Currency")
        ?? "EUR";
}

static async Task<decimal> PosRegisterTapeAsync(IKeyValueStore store, string? registerId)
{
    var index = await store.GetIndexAsync("pos_sales");
    decimal tape = 0;
    foreach (var pair in index)
    {
        var sale = await store.GetAsync<PosSaleStorageModel>("pos_sales", pair.Value);
        if (sale is null || !string.Equals(sale.Settlement, "cash", StringComparison.OrdinalIgnoreCase))
            continue;
        if (!string.Equals(sale.CashRegisterId ?? "", registerId ?? "", StringComparison.OrdinalIgnoreCase))
            continue;
        tape += Convert.ToDecimal(sale.Amount ?? 0);
    }

    return tape;
}

static async Task AdjustArticleQuantityAsync(IKeyValueStore store, string inventoryId, int delta)
{
    var row = await store.GetAsync<InventoryStorageModel>("inventory", inventoryId);
    if (row is null || string.IsNullOrWhiteSpace(row.WarehouseId))
        return;
    row.Quantity += delta;
    await store.UpdateAsync("inventory", inventoryId, row);
}

static async Task<TaxPolicy> CurrentTaxAsync(ISystemConfigurationStore store)
{
    var config = await store.GetAsync();
    return TaxPolicy.From(config?.Settings);
}

static async Task<decimal?> ExtraRateAsync(IKeyValueStore store, TaxPolicy tax, string? extraCostId)
{
    if (string.IsNullOrWhiteSpace(extraCostId))
        return tax.Product ?? tax.Stay;
    var extra = await store.GetAsync<ExtraCostStorageModel>("extra_costs", extraCostId);
    return tax.RateFor(extra?.Category, extra?.TaxPercentage);
}

static async Task PostExtraCostDeltaAsync(IFinancialPostingService ledger, AccountChart chart, TaxPolicy tax, string? bookingId, decimal delta, string description, string sourceId, decimal? rate)
{
    if (string.IsNullOrWhiteSpace(bookingId) || delta == 0)
        return;
    var when = DateTime.UtcNow;
    if (delta > 0)
    {
        await ledger.PostGuestChargeAsync(when, "booking_extra_charge", description, delta, rate, tax.Inclusive, chart.GuestLedger, "asset", chart.OtherIncome, "revenue", chart.OutputTax, bookingId: bookingId, sourceType: "booking-cost", sourceId: sourceId);
        return;
    }

    var amount = decimal.Abs(delta);
    var taxed = await ledger.SourceHasOutputTaxAsync(bookingId, sourceId, chart.OutputTax);
    if (taxed)
        await ledger.RelieveGuestChargeAsync(when, "booking_extra_reversal", description, amount, rate, tax.Inclusive, chart.GuestLedger, "asset", chart.OtherIncome, "revenue", chart.OutputTax, bookingId: bookingId, sourceType: "booking-cost", sourceId: sourceId);
    else
        await ledger.PostBalancedAsync(when, "booking_extra_reversal", description, amount, chart.OtherIncome, "revenue", chart.GuestLedger, "asset", bookingId: bookingId, sourceType: "booking-cost", sourceId: sourceId);
}

// --- Layer 4: BookingGuests (ospiti{year}) ---

BookingGuestDto ToBookingGuestDto(string id, BookingGuestStorageModel s) => new(id, s.Year, s.BookingId, s.ClientId, s.GuestNumber, s.CoveredByLead, s.CardState);

app.MapGet("/api/booking-guests", async (IKeyValueStore store, int? year) =>
{
    var idx = await store.GetIndexAsync("booking_guests");
    var prefix = year.HasValue ? $"{year}_" : null;
    var result = new List<BookingGuestDto>();
    foreach (var kvp in idx)
    {
        if (prefix != null && !kvp.Key.StartsWith(prefix)) continue;
        var g = await store.GetAsync<BookingGuestStorageModel>("booking_guests", kvp.Value);
        if (g is null) continue;
        result.Add(ToBookingGuestDto(kvp.Value, g));
    }
    return Results.Ok(result);
}).WithName("ListBookingGuests");

app.MapGet("/api/booking-guests/{year:int}/{id}", async (int year, string id, IKeyValueStore store) =>
{
    var g = await store.GetAsync<BookingGuestStorageModel>("booking_guests", id);
    if (g is null || g.Year != year) return Results.NotFound();
    return Results.Ok(ToBookingGuestDto(id, g));
}).WithName("GetBookingGuest");

app.MapPost("/api/booking-guests", async (BookingGuestDto request, IKeyValueStore store) =>
{
    var guid = Guid.NewGuid().ToString("N");
    var key = $"{request.Year}_{guid}";
    var storage = new BookingGuestStorageModel { Year = request.Year, BookingId = request.BookingId, ClientId = request.ClientId, GuestNumber = request.GuestNumber, CoveredByLead = request.CoveredByLead, CardState = request.CardState };
    var id = await store.CreateAsync("booking_guests", key, storage);
    return Results.Created($"/api/booking-guests/{request.Year}/{id}", ToBookingGuestDto(id, storage));
}).WithName("CreateBookingGuest");

app.MapPut("/api/booking-guests/{year:int}/{id}", async (int year, string id, BookingGuestDto request, IKeyValueStore store) =>
{
    var g = await store.GetAsync<BookingGuestStorageModel>("booking_guests", id);
    if (g is null || g.Year != year) return Results.NotFound();
    var storage = new BookingGuestStorageModel { Year = year, BookingId = request.BookingId, ClientId = request.ClientId, GuestNumber = request.GuestNumber, CoveredByLead = request.CoveredByLead, CardState = request.CardState };
    await store.UpdateAsync("booking_guests", id, storage);
    return Results.Ok(ToBookingGuestDto(id, storage));
}).WithName("UpdateBookingGuest");

app.MapDelete("/api/booking-guests/{year:int}/{id}", async (int year, string id, IKeyValueStore store) =>
{
    var g = await store.GetAsync<BookingGuestStorageModel>("booking_guests", id);
    if (g is null || g.Year != year) return Results.NotFound();
    await store.DeleteAsync("booking_guests", id);
    return Results.NoContent();
}).WithName("DeleteBookingGuest");

// --- Layer 4: CancelledBookings (prenotazioniannullate{year}) - read-only + DELETE ---

CancelledBookingDto ToCancelledBookingDto(string id, CancelledBookingStorageModel s) => new(id, s.Year, s.ClientId, s.RoomId, s.ArrivalDate, s.DepartureDate, s.Status, s.CancelledAt, s.Notes, s.BookingId);

app.MapGet("/api/cancelled-bookings", async (IKeyValueStore store, int? year) =>
{
    var idx = await store.GetIndexAsync("cancelled_bookings");
    var prefix = year.HasValue ? $"{year}_" : null;
    var result = new List<CancelledBookingDto>();
    foreach (var kvp in idx)
    {
        if (prefix != null && !kvp.Key.StartsWith(prefix)) continue;
        var cb = await store.GetAsync<CancelledBookingStorageModel>("cancelled_bookings", kvp.Value);
        if (cb is null) continue;
        result.Add(ToCancelledBookingDto(kvp.Value, cb));
    }
    return Results.Ok(result);
}).WithName("ListCancelledBookings");

app.MapGet("/api/cancelled-bookings/{year:int}/{id}", async (int year, string id, IKeyValueStore store) =>
{
    var cb = await store.GetAsync<CancelledBookingStorageModel>("cancelled_bookings", id);
    if (cb is null || cb.Year != year) return Results.NotFound();
    return Results.Ok(ToCancelledBookingDto(id, cb));
}).WithName("GetCancelledBooking");

app.MapDelete("/api/cancelled-bookings/{year:int}/{id}", async (int year, string id, IKeyValueStore store) =>
{
    var cb = await store.GetAsync<CancelledBookingStorageModel>("cancelled_bookings", id);
    if (cb is null || cb.Year != year) return Results.NotFound();
    await store.DeleteAsync("cancelled_bookings", id);
    return Results.NoContent();
}).WithName("DeleteCancelledBooking");

// --- Layer 4: Expenses (spese{year}) ---

ExpenseDto ToExpenseDto(string id, ExpenseStorageModel s) => new(id, s.Year, s.CashRegisterId, s.Amount, s.Description, s.Date, s.DeletedAt);

app.MapGet("/api/expenses", async (IKeyValueStore store, int? year) =>
{
    var idx = await store.GetIndexAsync("expenses");
    var prefix = year.HasValue ? $"{year}_" : null;
    var result = new List<ExpenseDto>();
    foreach (var kvp in idx)
    {
        if (prefix != null && !kvp.Key.StartsWith(prefix)) continue;
        var e = await store.GetAsync<ExpenseStorageModel>("expenses", kvp.Value);
        if (e is null || e.DeletedAt.HasValue) continue;
        result.Add(ToExpenseDto(kvp.Value, e));
    }
    return Results.Ok(result);
}).WithName("ListExpenses");

app.MapGet("/api/expenses/{year:int}/{id}", async (int year, string id, IKeyValueStore store) =>
{
    var e = await store.GetAsync<ExpenseStorageModel>("expenses", id);
    if (e is null || e.Year != year) return Results.NotFound();
    return Results.Ok(ToExpenseDto(id, e));
}).WithName("GetExpense");

app.MapPost("/api/expenses", async (ExpenseDto request, IKeyValueStore store, IFinancialPostingService ledger, IAccountChart accountChart) =>
{
    var guid = Guid.NewGuid().ToString("N");
    var key = $"{request.Year}_{guid}";
    var storage = new ExpenseStorageModel { Year = request.Year, CashRegisterId = request.CashRegisterId, Amount = request.Amount, Description = request.Description, Date = request.Date };
    var id = await store.CreateAsync("expenses", key, storage);
    var chart = await accountChart.CurrentAsync();
    await ledger.PostBalancedAsync(request.Date?.ToDateTime(TimeOnly.MinValue) ?? DateTime.UtcNow, "hotel_purchase", request.Description ?? "Hotel purchase", Convert.ToDecimal(request.Amount ?? 0), chart.OperatingExpense, "expense", chart.Cash, "asset", cashRegisterId: request.CashRegisterId, sourceType: "expense", sourceId: id);
    return Results.Created($"/api/expenses/{request.Year}/{id}", ToExpenseDto(id, storage));
}).WithName("CreateExpense");

app.MapPut("/api/expenses/{year:int}/{id}", async (int year, string id, ExpenseDto request, IKeyValueStore store) =>
{
    var e = await store.GetAsync<ExpenseStorageModel>("expenses", id);
    if (e is null || e.Year != year) return Results.NotFound();
    var storage = new ExpenseStorageModel { Year = year, CashRegisterId = request.CashRegisterId, Amount = request.Amount, Description = request.Description, Date = request.Date, DeletedAt = e.DeletedAt };
    await store.UpdateAsync("expenses", id, storage);
    return Results.Ok(ToExpenseDto(id, storage));
}).WithName("UpdateExpense");

app.MapDelete("/api/expenses/{year:int}/{id}", async (int year, string id, IKeyValueStore store, ILedgerRepository ledger) =>
{
    var e = await store.GetAsync<ExpenseStorageModel>("expenses", id);
    if (e is null || e.Year != year) return Results.NotFound();
    if (e.DeletedAt.HasValue)
        return Results.NoContent();

    var cited = await ledger.AnyMatchingAsync(line =>
        string.Equals(line.SourceType, "expense", StringComparison.OrdinalIgnoreCase)
        && string.Equals(line.SourceId, id, StringComparison.OrdinalIgnoreCase));
    if (cited)
    {
        e.DeletedAt = DateTime.UtcNow;
        await store.UpdateAsync("expenses", id, e);
    }
    else
    {
        await store.DeleteAsync("expenses", id);
    }

    return Results.NoContent();
}).WithName("DeleteExpense");

// --- Point of sale: folio charge while the guest is in house, or a walk-up cash sale ---

PosSaleDto ToPosSaleDto(string id, PosSaleStorageModel s) => new(id, s.Year, s.Date, s.Description, s.Amount, s.Settlement, s.BookingId, s.ClientId, s.CashRegisterId, s.PaymentMethod, s.Currency, s.Person, s.RunningCashTotal, s.ExtraCostId, s.BookingCostId, s.EntryType, s.DebitAccount, s.CreditAccount, s.ReversesId, s.ReversedById, s.ArticleId, s.Units);

app.MapGet("/api/pos-sales", async (IKeyValueStore store, int? year) =>
{
    var idx = await store.GetIndexAsync("pos_sales");
    var result = new List<PosSaleDto>();
    foreach (var kvp in idx)
    {
        var sale = await store.GetAsync<PosSaleStorageModel>("pos_sales", kvp.Value);
        if (sale is null || (year.HasValue && sale.Year != year.Value)) continue;
        result.Add(ToPosSaleDto(kvp.Value, sale));
    }
    return Results.Ok(result.OrderByDescending(s => s.Date).ThenByDescending(s => s.Id).ToList());
}).WithName("ListPosSales");

app.MapGet("/api/pos-sales/{year:int}/{id}", async (int year, string id, IKeyValueStore store) =>
{
    var sale = await store.GetAsync<PosSaleStorageModel>("pos_sales", id);
    if (sale is null || sale.Year != year) return Results.NotFound();
    return Results.Ok(ToPosSaleDto(id, sale));
}).WithName("GetPosSale");

app.MapPost("/api/pos-sales", async (PosSaleDto request, IKeyValueStore store, IFinancialPostingService ledger, ISystemConfigurationStore configurationStore, IAccountChart accountChart) =>
{
    var amount = Convert.ToDecimal(request.Amount ?? 0);
    if (amount <= 0)
        return Results.BadRequest(new { error = "Amount must be greater than zero." });
    if (string.IsNullOrWhiteSpace(request.Description))
        return Results.BadRequest(new { error = "Description is required." });
    var units = request.Units ?? 1;
    if (units < 1)
        return Results.BadRequest(new { error = "Article units must be greater than zero." });

    var year = request.Year > 0 ? request.Year : DateTime.UtcNow.Year;
    var chart = await accountChart.CurrentAsync();
    var when = request.Date?.ToDateTime(TimeOnly.MinValue) ?? DateTime.UtcNow;
    var description = request.Description.Trim();
    var onStay = !string.IsNullOrWhiteSpace(request.BookingId);

    BookingStorageModel? stay = null;
    if (onStay)
    {
        stay = await store.GetAsync<BookingStorageModel>("bookings", request.BookingId!);
        if (stay is null)
            return Results.NotFound(new { error = "Booking was not found." });
        if (!IsPosFolioStay(stay))
            return Results.BadRequest(new { error = "A stay sale is available after arrival and until checkout." });
    }

    string? articleId = null;
    var commitWarehouse = false;
    if (!string.IsNullOrWhiteSpace(request.ArticleId))
    {
        var article = await store.GetAsync<InventoryStorageModel>("inventory", request.ArticleId);
        if (article is null)
            return Results.BadRequest(new { error = "Article was not found." });
        articleId = request.ArticleId;
        if (!string.IsNullOrWhiteSpace(article.WarehouseId))
        {
            if (article.Quantity < units)
                return Results.BadRequest(new { error = "The warehouse does not hold enough of this article." });
            commitWarehouse = true;
        }
        else if (!string.IsNullOrWhiteSpace(article.RoomId))
        {
            if (stay is null)
                return Results.BadRequest(new { error = "A room article can be sold on a checked-in stay." });
            if (!string.Equals(article.RoomId, stay.RoomId, StringComparison.OrdinalIgnoreCase) || article.Quantity < units)
                return Results.BadRequest(new { error = "The room does not hold enough of this article." });
        }
        else
            return Results.BadRequest(new { error = "Article was not found." });
    }

    string? bookingId = null;
    string? clientId = null;
    string? registerId = null;
    string? bookingCostId = null;
    string? currency = null;
    double? running = null;
    string settlement;
    string entryType;
    string debit;
    string credit;
    var saleTax = await CurrentTaxAsync(configurationStore);
    var saleRate = await ExtraRateAsync(store, saleTax, request.ExtraCostId);
    var (guestPaid, _, _) = StayTax.Split(amount, saleRate, saleTax.Inclusive);

    if (onStay)
    {
        settlement = "folio";
        bookingId = request.BookingId;
        clientId = stay!.ClientId;
        entryType = "booking_extra_charge";
        debit = chart.GuestLedger;
        credit = chart.OtherIncome;
        if (stay.Year > 0) year = stay.Year;

        var cost = new BookingCostStorageModel { Year = year, BookingId = bookingId, Amount = (double)amount, Description = description };
        bookingCostId = await store.CreateAsync("booking_costs", $"{year}_{Guid.NewGuid():N}", cost);
        await PostExtraCostDeltaAsync(ledger, chart, saleTax, bookingId, amount, description, bookingCostId, saleRate);
    }
    else
    {
        var register = await ResolvePosRegisterAsync(store, request.CashRegisterId);
        if (register.Missing)
            return Results.BadRequest(new { error = "Cash register was not found." });

        settlement = "cash";
        entryType = "pos_cash_sale";
        debit = chart.Cash;
        credit = chart.OtherIncome;
        registerId = register.Id;
        currency = string.IsNullOrWhiteSpace(request.Currency) ? await PosDefaultCurrencyAsync(configurationStore) : request.Currency.Trim();
        running = (double)(await PosRegisterTapeAsync(store, registerId) + guestPaid);
        amount = guestPaid;
    }

    var sale = new PosSaleStorageModel
    {
        Year = year,
        Date = request.Date ?? DateOnly.FromDateTime(when),
        Description = description,
        Amount = (double)amount,
        Settlement = settlement,
        BookingId = bookingId,
        ClientId = clientId,
        CashRegisterId = registerId,
        PaymentMethod = onStay ? null : request.PaymentMethod,
        Currency = currency,
        Person = onStay ? null : request.Person,
        RunningCashTotal = running,
        ExtraCostId = request.ExtraCostId,
        BookingCostId = bookingCostId,
        EntryType = entryType,
        DebitAccount = debit,
        CreditAccount = credit,
        ArticleId = articleId,
        Units = articleId is null ? null : units
    };
    var id = await store.CreateAsync("pos_sales", $"{year}_{Guid.NewGuid():N}", sale);
    if (!onStay)
    {
        await ledger.PostGuestChargeAsync(when, entryType, description, request.Amount is null ? amount : Convert.ToDecimal(request.Amount.Value), saleRate, saleTax.Inclusive, debit, "asset", credit, "revenue", chart.OutputTax, cashRegisterId: registerId, payerName: request.Person ?? description, sourceType: "pos-sale", sourceId: id);
    }

    if (commitWarehouse)
        await AdjustArticleQuantityAsync(store, articleId!, -units);

    return Results.Created($"/api/pos-sales/{year}/{id}", ToPosSaleDto(id, sale));
}).WithName("CreatePosSale");

app.MapDelete("/api/pos-sales/{year:int}/{id}", (int year, string id) =>
{
    _ = year;
    _ = id;
    return Results.StatusCode(StatusCodes.Status405MethodNotAllowed);
}).WithName("DeletePosSale");

app.MapPost("/api/pos-sales/{year:int}/{id}/reverse", async (int year, string id, IKeyValueStore store, IFinancialPostingService ledger, IAccountChart accountChart, ISystemConfigurationStore configurationStore) =>
{
    var original = await store.GetAsync<PosSaleStorageModel>("pos_sales", id);
    if (original is null || original.Year != year) return Results.NotFound();
    if (!string.IsNullOrWhiteSpace(original.ReversedById))
        return Results.Conflict(new { error = "This sale is already reversed." });
    if (!string.IsNullOrWhiteSpace(original.ReversesId))
        return Results.Conflict(new { error = "Reverse the original sale." });

    var amount = Convert.ToDecimal(original.Amount ?? 0);
    if (amount == 0)
        return Results.BadRequest(new { error = "A zero amount cannot be reversed." });

    var chart = await accountChart.CurrentAsync();
    var onStay = string.Equals(original.Settlement, "folio", StringComparison.OrdinalIgnoreCase);
    var description = string.IsNullOrWhiteSpace(original.Description) ? "Reversal" : $"Reversal of {original.Description}";
    var reversal = new PosSaleStorageModel
    {
        Year = year,
        Date = DateOnly.FromDateTime(DateTime.UtcNow),
        Description = description,
        Amount = -(double)amount,
        Settlement = original.Settlement,
        BookingId = original.BookingId,
        ClientId = original.ClientId,
        CashRegisterId = original.CashRegisterId,
        PaymentMethod = original.PaymentMethod,
        Currency = original.Currency,
        Person = original.Person,
        ExtraCostId = original.ExtraCostId,
        BookingCostId = original.BookingCostId,
        EntryType = onStay ? "booking_extra_reversal" : "pos_cash_reversal",
        DebitAccount = original.CreditAccount,
        CreditAccount = original.DebitAccount,
        ReversesId = id,
        ArticleId = original.ArticleId,
        Units = original.Units is null ? null : -original.Units
    };
    if (!onStay)
        reversal.RunningCashTotal = (double)(await PosRegisterTapeAsync(store, original.CashRegisterId) - amount);

    var reversalId = await store.CreateAsync("pos_sales", $"{year}_{Guid.NewGuid():N}", reversal);
    original.ReversedById = reversalId;
    await store.UpdateAsync("pos_sales", id, original);

    if (onStay)
    {
        var cost = string.IsNullOrWhiteSpace(original.BookingCostId)
            ? null
            : await store.GetAsync<BookingCostStorageModel>("booking_costs", original.BookingCostId);
        if (cost is not null && !cost.DeletedAt.HasValue)
        {
            var outstanding = Convert.ToDecimal(cost.Amount ?? 0);
            if (outstanding != 0)
            {
                var reverseTax = await CurrentTaxAsync(configurationStore);
                var reverseRate = await ExtraRateAsync(store, reverseTax, original.ExtraCostId);
                await PostExtraCostDeltaAsync(ledger, chart, reverseTax, original.BookingId, -outstanding, description, original.BookingCostId!, reverseRate);
            }
            cost.DeletedAt = DateTime.UtcNow;
            await store.UpdateAsync("booking_costs", original.BookingCostId!, cost);
        }
        else
        {
            await ledger.PostBalancedAsync(DateTime.UtcNow, "booking_extra_reversal", description, amount, chart.OtherIncome, "revenue", chart.GuestLedger, "asset", bookingId: original.BookingId, sourceType: "pos-sale", sourceId: reversalId);
        }
    }
    else
    {
        var saleDay = original.Date?.ToDateTime(TimeOnly.MinValue) ?? DateTime.UtcNow;
        var flipped = await ledger.FlipSourceOnDateAsync(saleDay, id, "pos_cash_reversal", description, original.CashRegisterId, original.Person ?? description, "pos-sale", reversalId);
        if (!flipped && saleDay.Date != DateTime.UtcNow.Date)
            flipped = await ledger.FlipSourceOnDateAsync(DateTime.UtcNow, id, "pos_cash_reversal", description, original.CashRegisterId, original.Person ?? description, "pos-sale", reversalId);
        if (!flipped)
            await ledger.PostBalancedAsync(DateTime.UtcNow, "pos_cash_reversal", description, amount, chart.OtherIncome, "revenue", chart.Cash, "asset", cashRegisterId: original.CashRegisterId, payerName: original.Person ?? description, sourceType: "pos-sale", sourceId: reversalId);
    }

    if (!string.IsNullOrWhiteSpace(original.ArticleId) && (original.Units ?? 0) > 0)
        await AdjustArticleQuantityAsync(store, original.ArticleId, original.Units ?? 1);

    return Results.Created($"/api/pos-sales/{year}/{reversalId}", ToPosSaleDto(reversalId, reversal));
}).WithName("ReversePosSale");

// --- Layer 4: MoneyHistory (movimenticassa{year}) - append-only, no PUT ---

MoneyHistoryDto ToMoneyHistoryDto(string id, MoneyHistoryStorageModel s) => new(id, s.Year, s.CashRegisterId, s.Amount, s.Type, s.Description, s.Date, s.ReversesId, s.ReversedById, s.Person, s.PaymentMethod, s.Currency, s.RunningCashTotal);

static bool MoneyHistoryIsOutflow(double? amount, string? type)
{
    var value = Convert.ToDecimal(amount ?? 0);
    return value < 0
        || string.Equals(type, "expense", StringComparison.OrdinalIgnoreCase)
        || string.Equals(type, "s", StringComparison.OrdinalIgnoreCase);
}

app.MapGet("/api/money-history", async (IKeyValueStore store, int? year) =>
{
    var idx = await store.GetIndexAsync("money_history");
    var prefix = year.HasValue ? $"{year}_" : null;
    var result = new List<MoneyHistoryDto>();
    foreach (var kvp in idx)
    {
        if (prefix != null && !kvp.Key.StartsWith(prefix)) continue;
        var m = await store.GetAsync<MoneyHistoryStorageModel>("money_history", kvp.Value);
        if (m is null) continue;
        result.Add(ToMoneyHistoryDto(kvp.Value, m));
    }
    return Results.Ok(result);
}).WithName("ListMoneyHistory");

app.MapGet("/api/money-history/{year:int}/{id}", async (int year, string id, IKeyValueStore store) =>
{
    var m = await store.GetAsync<MoneyHistoryStorageModel>("money_history", id);
    if (m is null || m.Year != year) return Results.NotFound();
    return Results.Ok(ToMoneyHistoryDto(id, m));
}).WithName("GetMoneyHistory");

app.MapPost("/api/money-history", async (MoneyHistoryDto request, IKeyValueStore store, IFinancialPostingService ledger, IAccountChart accountChart) =>
{
    var guid = Guid.NewGuid().ToString("N");
    var key = $"{request.Year}_{guid}";
    var storage = new MoneyHistoryStorageModel { Year = request.Year, CashRegisterId = request.CashRegisterId, Amount = request.Amount, Type = request.Type, Description = request.Description, Date = request.Date, Person = request.Person, PaymentMethod = request.PaymentMethod, Currency = request.Currency, RunningCashTotal = request.RunningCashTotal };
    var id = await store.CreateAsync("money_history", key, storage);
    var amount = Convert.ToDecimal(request.Amount ?? 0);
    var chart = await accountChart.CurrentAsync();
    var isOutflow = MoneyHistoryIsOutflow(request.Amount, request.Type);
    if (isOutflow)
    {
        await ledger.PostBalancedAsync(request.Date?.ToDateTime(TimeOnly.MinValue) ?? DateTime.UtcNow, "petty_cash_expense", request.Description ?? "Petty cash expense", Math.Abs(amount), chart.OperatingExpense, "expense", chart.Cash, "asset", cashRegisterId: request.CashRegisterId, payerName: request.Person ?? request.Description, sourceType: "money-history", sourceId: id);
    }
    else
    {
        await ledger.PostBalancedAsync(request.Date?.ToDateTime(TimeOnly.MinValue) ?? DateTime.UtcNow, "petty_cash_income", request.Description ?? "Petty cash income", Math.Abs(amount), chart.Cash, "asset", chart.OtherIncome, "revenue", cashRegisterId: request.CashRegisterId, payerName: request.Person ?? request.Description, sourceType: "money-history", sourceId: id);
    }
    return Results.Created($"/api/money-history/{request.Year}/{id}", ToMoneyHistoryDto(id, storage));
}).WithName("CreateMoneyHistory");

app.MapDelete("/api/money-history/{year:int}/{id}", (int year, string id) =>
{
    _ = year;
    _ = id;
    return Results.StatusCode(StatusCodes.Status405MethodNotAllowed);
}).WithName("DeleteMoneyHistory");

app.MapPost("/api/money-history/{year:int}/{id}/reverse", async (int year, string id, IKeyValueStore store, IFinancialPostingService ledger, IAccountChart accountChart) =>
{
    var original = await store.GetAsync<MoneyHistoryStorageModel>("money_history", id);
    if (original is null || original.Year != year) return Results.NotFound();
    if (!string.IsNullOrWhiteSpace(original.ReversedById))
        return Results.Conflict(new { error = "This entry is already reversed." });

    if (string.Equals(original.Type, "register", StringComparison.OrdinalIgnoreCase))
        return Results.Conflict(new { error = "This row is the cash-register copy of a booking payment. Reverse that payment instead." });

    var amount = Convert.ToDecimal(original.Amount ?? 0);
    if (amount == 0)
        return Results.BadRequest(new { error = "A zero amount cannot be reversed." });

    var guid = Guid.NewGuid().ToString("N");
    var key = $"{year}_{guid}";
    var reversal = new MoneyHistoryStorageModel
    {
        Year = year,
        CashRegisterId = original.CashRegisterId,
        Amount = -(original.Amount ?? 0),
        Type = "reversal",
        Description = string.IsNullOrWhiteSpace(original.Description) ? "Reversal" : $"Reversal of {original.Description}",
        Date = DateOnly.FromDateTime(DateTime.UtcNow),
        ReversesId = id,
        Person = original.Person,
        PaymentMethod = original.PaymentMethod,
        Currency = original.Currency
    };
    var reversalId = await store.CreateAsync("money_history", key, reversal);
    original.ReversedById = reversalId;
    await store.UpdateAsync("money_history", id, original);

    var magnitude = Math.Abs(amount);
    var when = DateTime.UtcNow;
    var chart = await accountChart.CurrentAsync();
    if (MoneyHistoryIsOutflow(original.Amount, original.Type))
    {
        await ledger.PostBalancedAsync(when, "petty_cash_reversal", reversal.Description, magnitude, chart.Cash, "asset", chart.OperatingExpense, "expense", cashRegisterId: original.CashRegisterId, payerName: reversal.Description, sourceType: "money-history", sourceId: reversalId);
    }
    else
    {
        await ledger.PostBalancedAsync(when, "petty_cash_reversal", reversal.Description, magnitude, chart.OtherIncome, "revenue", chart.Cash, "asset", cashRegisterId: original.CashRegisterId, payerName: reversal.Description, sourceType: "money-history", sourceId: reversalId);
    }

    return Results.Created($"/api/money-history/{year}/{reversalId}", ToMoneyHistoryDto(reversalId, reversal));
}).WithName("ReverseMoneyHistory");

app.MapPost("/api/retention/clear-deleted", async (IKeyValueStore store, ILedgerRepository ledger, IEndpointQueryCache queryCache) =>
{
    var closedThroughYear = DateTime.UtcNow.Year - 1;
    var removed = 0;

    var bookings = await store.GetIndexAsync("bookings");
    foreach (var kvp in bookings)
    {
        var booking = await store.GetAsync<BookingStorageModel>("bookings", kvp.Value);
        if (booking is null || !booking.DeletedAt.HasValue || booking.Year > closedThroughYear)
            continue;
        var cited = await ledger.AnyMatchingAsync(line => string.Equals(line.BookingId, kvp.Value, StringComparison.OrdinalIgnoreCase));
        if (cited)
            continue;
        await store.DeleteAsync("bookings", kvp.Value);
        removed++;
    }

    var expenses = await store.GetIndexAsync("expenses");
    foreach (var kvp in expenses)
    {
        var expense = await store.GetAsync<ExpenseStorageModel>("expenses", kvp.Value);
        if (expense is null || !expense.DeletedAt.HasValue || expense.Year > closedThroughYear)
            continue;
        var cited = await ledger.AnyMatchingAsync(line =>
            string.Equals(line.SourceType, "expense", StringComparison.OrdinalIgnoreCase)
            && string.Equals(line.SourceId, kvp.Value, StringComparison.OrdinalIgnoreCase));
        if (cited)
            continue;
        await store.DeleteAsync("expenses", kvp.Value);
        removed++;
    }

    queryCache.InvalidateTag("bookings");
    return Results.Ok(new { removed, closedThroughYear });
}).WithName("ClearDeletedRecords");

// --- Layer 5: Messages (messaggi) ---

MessageDto ToMessageDto(string id, MessageStorageModel s) => new(id, s.MessageType, s.Status, s.Sender, s.Body, s.RecipientUserIds, s.CreatedAt, s.SeenAt);

app.MapGet("/api/messages", async (IKeyValueStore store, string? userId, string? status) =>
{
    var idx = await store.GetIndexAsync("messages");
    var result = new List<MessageDto>();
    foreach (var kvp in idx)
    {
        var m = await store.GetAsync<MessageStorageModel>("messages", kvp.Value);
        if (m is null) continue;
        if (userId != null && (m.RecipientUserIds == null || !m.RecipientUserIds.Contains(userId))) continue;
        if (status != null && !string.Equals(m.Status, status, StringComparison.OrdinalIgnoreCase)) continue;
        result.Add(ToMessageDto(kvp.Value, m));
    }
    return Results.Ok(result);
}).WithName("ListMessages");

app.MapGet("/api/messages/{id}", async (string id, IKeyValueStore store) =>
{
    var m = await store.GetAsync<MessageStorageModel>("messages", id);
    if (m is null) return Results.NotFound();
    return Results.Ok(ToMessageDto(id, m));
}).WithName("GetMessage");

app.MapPost("/api/messages", async (MessageDto request, IKeyValueStore store) =>
{
    if (string.IsNullOrWhiteSpace(request.Sender) || string.IsNullOrWhiteSpace(request.Body))
        return Results.BadRequest("Sender and Body are required.");
    var name = Guid.NewGuid().ToString("N");
    var storage = new MessageStorageModel
    {
        MessageType = request.MessageType ?? "internal",
        Status = request.Status ?? "unread",
        Sender = request.Sender,
        Body = request.Body,
        RecipientUserIds = request.RecipientUserIds,
        CreatedAt = request.CreatedAt ?? DateTime.UtcNow
    };
    var id = await store.CreateAsync("messages", name, storage);
    return Results.Created($"/api/messages/{id}", ToMessageDto(id, storage));
}).WithName("CreateMessage");

app.MapPut("/api/messages/{id}/read", async (string id, IKeyValueStore store) =>
{
    var m = await store.GetAsync<MessageStorageModel>("messages", id);
    if (m is null) return Results.NotFound();
    m.Status = "read";
    m.SeenAt = DateTime.UtcNow;
    await store.UpdateAsync("messages", id, m);
    return Results.Ok(ToMessageDto(id, m));
}).WithName("MarkMessageRead");

app.MapPut("/api/messages/{id}/archive", async (string id, IKeyValueStore store) =>
{
    var m = await store.GetAsync<MessageStorageModel>("messages", id);
    if (m is null) return Results.NotFound();
    m.Status = "archived";
    await store.UpdateAsync("messages", id, m);
    return Results.Ok(ToMessageDto(id, m));
}).WithName("ArchiveMessage");

app.MapDelete("/api/messages/{id}", async (string id, IKeyValueStore store) =>
{
    var m = await store.GetAsync<MessageStorageModel>("messages", id);
    if (m is null) return Results.NotFound();
    await store.DeleteAsync("messages", id);
    return Results.NoContent();
}).WithName("DeleteMessage");

// --- Layer 5: ContractTemplates (contratti) ---

static string SanitizeContractType(string type) =>
    System.Text.RegularExpressions.Regex.Replace(type, @"[^a-zA-Z0-9_]", "_");

ContractTemplateDto ToContractTemplateDto(string id, ContractTemplateStorageModel s) => new(id, s.Type, s.Number, s.Content);

app.MapGet("/api/contract-templates", async (IKeyValueStore store, string? type) =>
{
    var idx = await store.GetIndexAsync("contract_templates");
    var result = new List<ContractTemplateDto>();
    foreach (var kvp in idx)
    {
        var ct = await store.GetAsync<ContractTemplateStorageModel>("contract_templates", kvp.Value);
        if (ct is null) continue;
        if (type != null && !string.Equals(ct.Type, type, StringComparison.OrdinalIgnoreCase)) continue;
        result.Add(ToContractTemplateDto(kvp.Value, ct));
    }
    return Results.Ok(result);
}).WithName("ListContractTemplates");

app.MapGet("/api/contract-templates/{type}/{number:int}", async (string type, int number, IKeyValueStore store) =>
{
    var sanitized = SanitizeContractType(type);
    var key = $"{sanitized}_{number}";
    var ctIdx = await store.GetIndexAsync("contract_templates");
    if (!ctIdx.TryGetValue(key, out var ctId)) return Results.NotFound();
    var ct = await store.GetAsync<ContractTemplateStorageModel>("contract_templates", ctId);
    if (ct is null) return Results.NotFound();
    return Results.Ok(ToContractTemplateDto(key, ct));
}).WithName("GetContractTemplate");

app.MapPost("/api/contract-templates", async (ContractTemplateDto request, IKeyValueStore store) =>
{
    if (string.IsNullOrWhiteSpace(request.Type) || string.IsNullOrWhiteSpace(request.Content))
        return Results.BadRequest("Type and Content are required.");
    var sanitized = SanitizeContractType(request.Type);
    var idx = await store.GetIndexAsync("contract_templates");
    var existingForType = idx.Keys.Where(k => k.StartsWith($"{sanitized}_")).ToList();
    var maxNum = existingForType.Count > 0
        ? existingForType.Select(k => { var parts = k.Split('_'); return int.TryParse(parts.Last(), out var n) ? n : 0; }).Max()
        : 0;
    var number = request.Number > 0 ? request.Number : maxNum + 1;
    var key = $"{sanitized}_{number}";
    var storage = new ContractTemplateStorageModel { Type = request.Type, Number = number, Content = request.Content };
    await store.CreateAsync("contract_templates", key, storage);
    return Results.Created($"/api/contract-templates/{request.Type}/{number}", ToContractTemplateDto(key, storage));
}).WithName("CreateContractTemplate");

app.MapPut("/api/contract-templates/{type}/{number:int}", async (string type, int number, ContractTemplateDto request, IKeyValueStore store) =>
{
    var sanitized = SanitizeContractType(type);
    var key = $"{sanitized}_{number}";
    var ctPutIdx = await store.GetIndexAsync("contract_templates");
    if (!ctPutIdx.TryGetValue(key, out var ctPutId)) return Results.NotFound();
    var ct = await store.GetAsync<ContractTemplateStorageModel>("contract_templates", ctPutId);
    if (ct is null) return Results.NotFound();
    ct.Type = request.Type ?? ct.Type;
    ct.Content = request.Content ?? ct.Content;
    await store.UpdateAsync("contract_templates", ctPutId, ct);
    return Results.Ok(ToContractTemplateDto(key, ct));
}).WithName("UpdateContractTemplate");

app.MapDelete("/api/contract-templates/{type}/{number:int}", async (string type, int number, IKeyValueStore store) =>
{
    var sanitized = SanitizeContractType(type);
    var key = $"{sanitized}_{number}";
    var ctDelIdx = await store.GetIndexAsync("contract_templates");
    if (!ctDelIdx.TryGetValue(key, out var ctDelId)) return Results.NotFound();
    await store.DeleteAsync("contract_templates", ctDelId);
    return Results.NoContent();
}).WithName("DeleteContractTemplate");

// --- Booking finance (ledger projection, not a second book) ---

app.MapGet("/api/bookings/{year:int}/{id}/transactions", async (int year, string id, IKeyValueStore store, ILedgerRepository ledger, IBookingLedgerIndexRepository indexRepo) =>
{
    var booking = await store.GetAsync<BookingStorageModel>("bookings", id);
    if (booking is null || booking.Year != year) return Results.NotFound();

    var from = booking.ActualArrivalDate?.ToDateTime(TimeOnly.MinValue)
               ?? booking.ArrivalDate?.ToDateTime(TimeOnly.MinValue)
               ?? DateTime.MinValue;
    var to = (booking.ActualDepartureDate?.ToDateTime(TimeOnly.MinValue)
              ?? booking.DepartureDate?.ToDateTime(TimeOnly.MinValue)
              ?? DateTime.MaxValue).AddDays(1);
    var lines = await BookingTransactionProjection.LoadAsync(ledger, indexRepo, id, from, to);
    return Results.Ok(BookingTransactionProjection.Project(lines));
}).WithName("ListBookingTransactions");

app.MapPost("/api/bookings/{year:int}/{id}/transactions", async (int year, string id, BookingTransactionWriteDto request, IKeyValueStore store, IFinancialPostingService posting, ILedgerRepository ledger, IBookingLedgerIndexRepository indexRepo, IAccountChart accountChart) =>
{
    if (request is null || string.IsNullOrWhiteSpace(request.Type))
        return Results.BadRequest(new { error = "Transaction type is required" });
    if (request.Amount == 0)
        return Results.BadRequest(new { error = "Transaction amount must not be zero" });

    var booking = await store.GetAsync<BookingStorageModel>("bookings", id);
    if (booking is null || booking.Year != year) return Results.NotFound();

    var chart = await accountChart.CurrentAsync();
    var when = request.Timestamp ?? DateTime.UtcNow;
    var appliedTo = string.IsNullOrWhiteSpace(request.AppliedTo) ? "stay_balance" : request.AppliedTo.Trim();
    var isDeposit = string.Equals(request.Type, "deposit", StringComparison.OrdinalIgnoreCase) || string.Equals(appliedTo, "deposit", StringComparison.OrdinalIgnoreCase);
    var isRefund = string.Equals(request.Type, "refund", StringComparison.OrdinalIgnoreCase);
    var sourceId = Guid.NewGuid().ToString("N");

    LedgerEntry createdEntry;
    if (isRefund)
        createdEntry = await posting.PostBalancedAsync(when, "booking_refund", request.Description ?? "Booking refund", request.Amount, isDeposit ? chart.GuestDeposits : chart.GuestLedger, isDeposit ? "liability" : "asset", chart.Cash, "asset", bookingId: id, sourceType: "booking-transaction", sourceId: sourceId, createdBy: request.CreatedBy, appliedTo: appliedTo);
    else if (isDeposit)
        createdEntry = await posting.PostBalancedAsync(when, "booking_deposit", request.Description ?? "Booking deposit", request.Amount, chart.Cash, "asset", chart.GuestDeposits, "liability", bookingId: id, sourceType: "booking-transaction", sourceId: sourceId, createdBy: request.CreatedBy, appliedTo: appliedTo);
    else
        createdEntry = await posting.PostBalancedAsync(when, "booking_payment", request.Description ?? "Booking payment", request.Amount, chart.Cash, "asset", chart.GuestLedger, "asset", bookingId: id, sourceType: "booking-transaction", sourceId: sourceId, createdBy: request.CreatedBy, appliedTo: appliedTo);

    var lines = await BookingTransactionProjection.LoadAsync(ledger, indexRepo, id, DateTime.MinValue, DateTime.MaxValue);
    var dto = BookingTransactionProjection.ProjectJournal(lines, createdEntry.JournalId)
        ?? new BookingTransactionDto(id, 0, when, request.Type, Math.Abs(request.Amount), request.Description, appliedTo, request.CreatedBy);
    return Results.Created($"/api/bookings/{year}/{id}/transactions/{dto.Sequence}", dto);
}).WithName("AppendBookingTransaction");

app.MapGet("/api/bookings/{year:int}/{id}/finance", async (
    int year,
    string id,
    IKeyValueStore store,
    ILedgerRepository ledger,
    IBookingLedgerIndexRepository indexRepo,
    ITariffResolutionService resolver,
    ISystemConfigurationStore configurationStore) =>
{
    var booking = await store.GetAsync<BookingStorageModel>("bookings", id);
    if (booking is null || booking.Year != year) return Results.NotFound();

    var config = await configurationStore.GetAsync() ?? new SystemConfiguration();
    var currency = config.DefaultCurrency
        ?? config.Settings?.GetValueOrDefault("Currency")
        ?? "EUR";

    string? tariffName = null;
    var nights = 0;
    if (booking.ArrivalDate.HasValue && booking.DepartureDate.HasValue)
        nights = Math.Max(0, booking.DepartureDate.Value.DayNumber - booking.ArrivalDate.Value.DayNumber);

    if (!string.IsNullOrWhiteSpace(booking.RoomId) && booking.ArrivalDate.HasValue && booking.DepartureDate.HasValue)
    {
        var guestCount = booking.ActualGuestCount ?? booking.ExpectedGuestCount ?? 1;
        var resolved = await resolver.ResolveAsync(booking.RoomId, booking.ArrivalDate.Value, booking.DepartureDate.Value, guestCount);
        tariffName = resolved?.TariffName;
    }

    var lines = await BookingTransactionProjection.LoadAsync(ledger, indexRepo, id, DateTime.MinValue, DateTime.MaxValue);
    var totals = BookingLedgerTotals.From(lines, AccountChart.ResolveOrUsali(config.AccountingStandard, config.AccountCodes));
    var prepaid = Convert.ToDecimal(booking.PrepaidAmount ?? 0);
    return Results.Ok(new BookingFinanceSummaryDto(
        id,
        year,
        totals.StayTotal,
        totals.ExtraTotal,
        totals.ChargesTotal,
        prepaid,
        totals.PaymentsTotal,
        totals.RefundsTotal,
        totals.DepositsHeld,
        totals.TotalDue,
        currency,
        booking.TariffId,
        tariffName,
        nights));
}).WithName("GetBookingFinance");

static LedgerLineDto ToLedgerLineDto(LedgerEntry entry) => new(
    entry.Id,
    entry.JournalId,
    entry.Timestamp,
    entry.EntryType,
    entry.BookingId,
    entry.AccountCode,
    entry.AccountGroup,
    entry.Debit,
    entry.Credit,
    entry.CashRegisterId,
    entry.PayerName,
    entry.Notes,
    entry.SourceType,
    entry.SourceId,
    entry.CreatedBy,
    entry.PrintedInvoiceNumber);

app.MapGet("/api/reports/performance", async (IKeyValueStore store, ILedgerRepository ledger) =>
    Results.Ok(await PerformanceReportBuilder.BuildAsync(store, ledger)))
.WithName("PerformanceReport");

app.MapGet("/api/reports/desk/{key}", async (string key, DateOnly? date, IKeyValueStore store, ILedgerRepository ledger) =>
{
    var report = await DeskReportBuilder.BuildAsync(key, date ?? DateOnly.FromDateTime(DateTime.Now), store, ledger);
    return report is null ? Results.NotFound() : Results.Ok(report);
}).WithName("DeskReport");

app.MapGet("/api/ledger/lines", async (DateOnly from, DateOnly to, string? bookingId, bool? unassigned, ILedgerRepository ledger) =>
{
    if (to < from) return Results.BadRequest(new { error = "to must be on or after from" });
    var start = from.ToDateTime(TimeOnly.MinValue);
    var end = to.ToDateTime(TimeOnly.MinValue);
    List<LedgerEntry> entries;
    if (unassigned == true)
        entries = await ledger.GetEntriesWithoutBookingAsync(start, end);
    else if (!string.IsNullOrWhiteSpace(bookingId))
        entries = await ledger.GetEntriesForBookingAsync(bookingId, start, end);
    else
    {
        entries = [];
        for (var date = start; date <= end; date = date.AddDays(1))
            entries.AddRange(await ledger.GetConsolidatedAsync(date));
    }
    return Results.Ok(entries.Select(ToLedgerLineDto).ToList());
}).WithName("ListLedgerLines");

app.MapGet("/api/ledger/accounts", async (IAccountChart accountChart) =>
    Results.Ok((await accountChart.CurrentAsync()).Accounts)).WithName("ListChartOfAccounts");

app.MapGet("/api/night-audits", async (int year, int month, IKeyValueStore store) =>
{
    if (month is < 1 or > 12)
        return Results.BadRequest(new { error = "Month is required" });

    var index = await store.GetIndexAsync("night_audits");
    var prefix = $"{year:0000}-{month:00}-";
    var rows = new List<NightAuditDto>();
    foreach (var (name, id) in index)
    {
        if (!name.StartsWith(prefix, StringComparison.Ordinal))
            continue;
        var stored = await store.GetAsync<NightAuditStorageModel>("night_audits", id);
        if (stored is not null)
            rows.Add(ToNightAuditDto(stored));
    }

    return Results.Ok(rows.OrderByDescending(row => row.BusinessDate).ToList());
});

app.MapGet("/api/night-audits/day", async (DateOnly date, IKeyValueStore store, ILedgerRepository ledger, ISystemConfigurationStore config) =>
{
    var stored = await FindNightAuditAsync(store, date);
    if (stored is not null)
        return Results.Ok(ToNightAuditDto(stored));

    var live = await LiveNightAuditAsync(date, store, ledger, config);
    return Results.Ok(live);
});

app.MapPost("/api/night-audits/day", async (DateOnly date, NightAuditSignRequest request, HttpContext httpContext, IKeyValueStore store, ILedgerRepository ledger, IAccountChart accountChart, ISystemConfigurationStore config) =>
{
    if (date > DateOnly.FromDateTime(DateTime.UtcNow))
        return Results.BadRequest(new { error = "A future day cannot be signed" });
    if (!request.CashAgrees || !request.BalancesReviewed || !request.RoomsSeen)
        return Results.BadRequest(new { error = "Cash, balances, and the rooms have to be marked as seen" });

    var name = RequestActor.KnownName(httpContext);
    if (string.IsNullOrWhiteSpace(name))
        return Results.BadRequest(new { error = "A desk name is required before the day can be signed" });
    if (await FindNightAuditAsync(store, date) is not null)
        return Results.Conflict(new { error = "This day is already signed" });

    var live = await LiveNightAuditAsync(date, store, ledger, config);
    var signedAt = DateTime.UtcNow;
    var stored = new NightAuditStorageModel
    {
        BusinessDate = date,
        SignedAt = signedAt,
        SignedBy = name,
        CashAgrees = true,
        BalancesReviewed = true,
        RoomsSeen = true,
        Note = string.IsNullOrWhiteSpace(request.Note) ? null : request.Note.Trim(),
        CashTaken = live.CashTaken,
        RoomRevenue = live.RoomRevenue,
        OtherIncome = live.OtherIncome,
        StillDue = live.StillDue,
        Occupancy = live.Occupancy,
        Sold = live.Sold,
        GuestFileGaps = live.GuestFileGaps
    };

    try
    {
        await store.CreateAsync("night_audits", date.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture), stored);
    }
    catch (InvalidOperationException)
    {
        return Results.Conflict(new { error = "This day is already signed" });
    }

    var chart = await accountChart.CurrentAsync();
    await ledger.AppendEntryAsync(date.ToDateTime(TimeOnly.MinValue), new LedgerEntry
    {
        Timestamp = signedAt,
        EntryType = "night_audit",
        AccountCode = chart.Cash,
        AccountGroup = "asset",
        Debit = 0,
        Credit = 0,
        Amount = 0,
        SourceType = "night-audit",
        SourceId = date.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
        CreatedBy = name,
        Notes = $"Night audit {date:yyyy-MM-dd} signed by {name}. Seen and accepted. Open balances stay open.",
        Status = "posted"
    });

    return Results.Ok(ToNightAuditDto(stored));
});

static NightAuditDto ToNightAuditDto(NightAuditStorageModel stored) => new(
    stored.BusinessDate,
    true,
    stored.SignedAt,
    stored.SignedBy,
    stored.CashAgrees,
    stored.BalancesReviewed,
    stored.RoomsSeen,
    stored.Note,
    stored.CashTaken,
    stored.RoomRevenue,
    stored.OtherIncome,
    stored.StillDue,
    stored.Occupancy,
    stored.Sold,
    stored.GuestFileGaps);

static async Task<NightAuditStorageModel?> FindNightAuditAsync(IKeyValueStore store, DateOnly date)
{
    var index = await store.GetIndexAsync("night_audits");
    var key = date.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
    if (!index.TryGetValue(key, out var id))
        return null;
    return await store.GetAsync<NightAuditStorageModel>("night_audits", id);
}

static async Task<NightAuditDto> LiveNightAuditAsync(DateOnly date, IKeyValueStore store, ILedgerRepository ledger, ISystemConfigurationStore config)
{
    var flash = await DeskReportBuilder.BuildAsync("flash", date, store, ledger);
    var guestFile = await GuestFileAudit.CollectAsync(date, store, config);
    return new NightAuditDto(
        date,
        false,
        null,
        null,
        false,
        false,
        false,
        null,
        SheetValue(flash, "reports.total.taken"),
        SheetValue(flash, "reports.col.revenue"),
        SheetValue(flash, "reports.col.other"),
        SheetValue(flash, "reports.total.due"),
        SheetValue(flash, "reports.col.occupancy"),
        SheetValue(flash, "reports.col.sold"),
        guestFile.Gaps.ToList());
}

static string SheetValue(PaperReportDto? report, string label)
{
    var row = report?.Sections.FirstOrDefault()?.Rows.FirstOrDefault(item => item.Cells.Count > 0 && item.Cells[0] == label);
    return row is null || row.Cells.Count < 2 ? "" : row.Cells[1];
}

app.MapGet("/api/guest-files", async (DateOnly date, IKeyValueStore store, ISystemConfigurationStore config) =>
    Results.Ok(await GuestFileAudit.CollectAsync(date, store, config))).WithName("GuestFileReport");

app.MapGet("/api/dispatch", async (string? status, string? channel, string? direction, DispatchService dispatch) =>
    Results.Ok(await dispatch.ListAsync(status, channel, direction))).WithName("ListDispatch");

app.MapPost("/api/dispatch", async (DispatchMessageDto request, DispatchService dispatch) =>
{
    var topic = string.Equals(request.Direction, "in", StringComparison.OrdinalIgnoreCase)
        ? DispatchTopics.IncomingReceived
        : DispatchTopics.OutgoingReady;
    var stored = new DispatchMessageStorage
    {
        Direction = request.Direction,
        Channel = request.Channel,
        To = request.To,
        From = request.From,
        RequestedAt = request.RequestedAt ?? DateTime.UtcNow,
        DeliverBy = request.DeliverBy,
        Subject = request.Subject,
        Body = request.Body,
        Attachments = string.Equals(request.Channel, "email", StringComparison.OrdinalIgnoreCase) ? null : request.Attachments,
        BookingId = request.BookingId,
        Year = request.Year,
        InvoiceNumber = request.InvoiceNumber,
        Kind = request.Kind,
        Eml = request.Eml,
        Status = request.Status
    };
    var created = await dispatch.EnqueueAsync(stored, topic);
    return Results.Created($"/api/dispatch/{created.Id}", created);
}).WithName("CreateDispatch");

app.MapPost("/api/dispatch/from-document", async (DispatchDocumentRequest request, IKeyValueStore store, IDocumentRenderService renderer, DispatchService dispatch) =>
{
    var sanitized = SanitizeContractType(request.DocumentType);
    var key = $"{sanitized}_{request.TemplateNumber}";
    var templateIdx = await store.GetIndexAsync("contract_templates");
    if (!templateIdx.TryGetValue(key, out var templateGuid)) return Results.NotFound();
    var template = await store.GetAsync<ContractTemplateStorageModel>("contract_templates", templateGuid);
    if (template is null) return Results.NotFound();
    var context = await renderer.BuildContextAsync(request.BookingId, request.Year, request.DocumentType);
    if (context is null) return Results.NotFound();
    var html = renderer.RenderHtml(template.Content ?? string.Empty, context);
    var (eml, _, _) = renderer.ApplyFormat(html, context, DocumentFormat.Eml);
    var queued = await dispatch.EnqueueDocumentAsync(context, html, eml, request.DocumentType);
    return Results.Created($"/api/dispatch/{queued.Id}", queued);
}).WithName("QueueDocument");

app.MapPost("/api/dispatch/{id}/relayed", async (string id, DispatchRelayRequest request, DispatchService dispatch) =>
{
    var updated = await dispatch.RelayAsync(id, request.RelayedAt);
    return updated is null ? Results.NotFound() : Results.Ok(updated);
}).WithName("RelayDispatch");

app.MapPost("/api/dispatch/{id}/accept", async (string id, DispatchService dispatch) =>
{
    var (message, error) = await dispatch.AcceptAsync(id);
    if (message is null) return Results.NotFound();
    if (error == "waiting") return Results.BadRequest(new { error = "The message needs a name, dates, and a room before it can become a booking." });
    return Results.Ok(message);
}).WithName("AcceptDispatch");

app.MapPost("/api/dispatch/process", async (DispatchService dispatch) =>
    Results.Ok(new { processed = await dispatch.ProcessReadyAsync() })).WithName("ProcessDispatch");

app.MapGet("/api/dispatch/events", async (long? after, DispatchService dispatch) =>
    Results.Ok(await dispatch.EventsAsync(after ?? 0))).WithName("DispatchEvents");

app.MapGet("/api/role-confirmations", async (IKeyValueStore store) =>
{
    var index = await store.GetIndexAsync("role_confirmations");
    var rows = new List<RoleConfirmation>();
    foreach (var entry in index)
    {
        var row = await store.GetAsync<RoleConfirmation>("role_confirmations", entry.Value);
        if (row is not null)
            rows.Add(row);
    }
    return Results.Ok(rows.OrderBy(row => row.UserId).ToList());
}).WithName("ListRoleConfirmations");

// --- Document Rendering ---

app.MapGet("/api/documents/render/{type}/{number:int}",
    async (string type, int number, string bookingId, int year, string? format,
           IKeyValueStore store, IDocumentRenderService renderer) =>
    {
        var sanitized = SanitizeContractType(type);
        var key = $"{sanitized}_{number}";
        var templateIdx = await store.GetIndexAsync("contract_templates");
        if (!templateIdx.TryGetValue(key, out var templateGuid)) return Results.NotFound();
        var template = await store.GetAsync<ContractTemplateStorageModel>("contract_templates", templateGuid);
        if (template is null) return Results.NotFound();

        var context = await renderer.BuildContextAsync(bookingId, year, type);
        if (context is null) return Results.NotFound();

        var html = renderer.RenderHtml(template.Content ?? string.Empty, context);
        var docFormat = (format?.ToLowerInvariant()) switch
        {
            "print" => DocumentFormat.Print,
            "doc"   => DocumentFormat.Doc,
            "eml"   => DocumentFormat.Eml,
            _       => DocumentFormat.Html
        };
        var (content, contentType, filename) = renderer.ApplyFormat(html, context, docFormat);

        var bytes = System.Text.Encoding.UTF8.GetBytes(content);
        // Print and plain HTML must render in the new tab so window.print() can run.
        // A filename on Results.File forces Content-Disposition: attachment, which
        // downloads the page instead of displaying it.
        if (docFormat is DocumentFormat.Print or DocumentFormat.Html)
            return Results.File(bytes, contentType);

        return Results.File(bytes, contentType, filename);
    }).WithName("RenderDocument");

app.MapPost("/api/documents/open/{type}/{number:int}",
    async (string type, int number, string bookingId, int year, IKeyValueStore store, IDocumentRenderService renderer) =>
    {
        var sanitized = SanitizeContractType(type);
        var key = $"{sanitized}_{number}";
        var templateIdx = await store.GetIndexAsync("contract_templates");
        if (!templateIdx.TryGetValue(key, out var templateGuid)) return Results.NotFound();
        var template = await store.GetAsync<ContractTemplateStorageModel>("contract_templates", templateGuid);
        if (template is null) return Results.NotFound();

        var context = await renderer.BuildContextAsync(bookingId, year, type);
        if (context is null) return Results.NotFound();

        var html = renderer.RenderHtml(template.Content ?? string.Empty, context);
        var (content, _, filename) = renderer.ApplyFormat(html, context, DocumentFormat.Eml);
        var safe = new string((filename ?? "message.eml").Where(ch => char.IsLetterOrDigit(ch) || ch is '.' or '-' or '_').ToArray());
        if (string.IsNullOrWhiteSpace(safe))
            safe = "message.eml";

        var directory = Path.Combine(Path.GetTempPath(), "hoteldruid-mail");
        Directory.CreateDirectory(directory);
        var path = Path.Combine(directory, safe);
        await File.WriteAllTextAsync(path, content);
        try
        {
            Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            return Results.Json(new { error = ex.Message }, statusCode: StatusCodes.Status503ServiceUnavailable);
        }

        return Results.Ok(new { opened = true });
    }).WithName("OpenDocument");

app.Run();

record BookingCheckInRequest(DateOnly? ActualArrivalDate, int? ActualGuestCount, string? Status);
record BookingCheckOutRequest(DateOnly? ActualDepartureDate, string? Status);
record BookingNoShowRequest(string? Notes, string? Status);