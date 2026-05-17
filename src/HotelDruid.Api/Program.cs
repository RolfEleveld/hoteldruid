using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;
using HotelDruid.Api.Services;
using HotelDruid.Api.Services.ExportImport;
using HotelDruid.Api.Models;
using HotelDruid.Shared;
using HotelDruid.Shared.Configuration;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.StaticFiles;

var builder = WebApplication.CreateBuilder(args);

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
    // Read dataRoot lazily so tests can override it via configuration
    var config = sp.GetRequiredService<IConfiguration>();
    var dataRoot = Environment.GetEnvironmentVariable("HOTELDRUID_DATAROOT") 
        ?? config["DataRoot"]
        ?? Path.Combine(Path.GetTempPath(), "hoteldruid");
    var logger = sp.GetRequiredService<ILogger<FileKeyValueStore>>();
    
    var baseStore = new FileKeyValueStore(dataRoot, logger);
    
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
            invalidationRootPath: dataRoot,
            versionCheckIntervalMilliseconds: versionCheckIntervalMs);
    }
    
    return baseStore;
});

// Register a simple file-backed store for development (legacy support)
builder.Services.AddSingleton<FileKvStore>();

// Register Phase 1B repositories
builder.Services.AddSingleton<IRoomRepository, RoomRepository>();
builder.Services.AddSingleton<ISystemConfigurationStore, SystemConfigurationStore>();
builder.Services.AddSingleton<ILedgerRepository>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var dataRoot = Environment.GetEnvironmentVariable("HOTELDRUID_DATAROOT")
        ?? config["DataRoot"]
        ?? Path.Combine(Path.GetTempPath(), "hoteldruid");
    var logger = sp.GetRequiredService<ILogger<LedgerRepository>>();
    return new LedgerRepository(dataRoot, logger);
});
builder.Services.AddSingleton<IBookingTransactionRepository>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var dataRoot = Environment.GetEnvironmentVariable("HOTELDRUID_DATAROOT")
        ?? config["DataRoot"]
        ?? Path.Combine(Path.GetTempPath(), "hoteldruid");
    var logger = sp.GetRequiredService<ILogger<BookingTransactionRepository>>();
    return new BookingTransactionRepository(dataRoot, logger);
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
builder.Services.AddSingleton<IEndpointQueryCache>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var logger = sp.GetRequiredService<ILogger<EndpointQueryCache>>();
    var enabled = config.GetValue<bool>("EndpointQueryCacheEnabled", true);
    var ttlSeconds = config.GetValue<int>("EndpointQueryCacheTtlSeconds", 30);
    return new EndpointQueryCache(logger, enabled, ttlSeconds);
});

var app = builder.Build();

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
    ContentTypeProvider = staticContentTypeProvider
};

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
    ContentTypeProvider = staticContentTypeProvider
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

app.MapGet("/api/status", () => Results.Ok(new { ActiveYear = "2026", User = "admin", Version = "HotelDruid 3.0.7" }));

app.MapGet("/api/system/configuration", async (ISystemConfigurationStore configurationStore) =>
{
    var config = await configurationStore.GetAsync();
    if (config is null)
        return Results.NotFound();

    return Results.Ok(config);
});

app.MapPut("/api/system/configuration", async (SystemConfiguration config, ISystemConfigurationStore configurationStore) =>
{
    if (string.IsNullOrWhiteSpace(config.Id))
        config.Id = "system";

    await configurationStore.SaveAsync(config);
    return Results.Ok(config);
});

app.MapDelete("/api/system/configuration", async (ISystemConfigurationStore configurationStore) =>
{
    await configurationStore.DeleteAsync();
    return Results.NoContent();
});

// --- Bookings endpoints are defined in Layer 4 section below ---

// --- Room API endpoints ---

app.MapPost("/api/rooms", async (RoomDto request, IKeyValueStore store, IEndpointQueryCache queryCache) =>
{
    if (string.IsNullOrWhiteSpace(request.Name))
        return Results.BadRequest(new { error = "Room name (ID) is required" });
    if (request.Capacity <= 0)
        return Results.BadRequest(new { error = "Capacity must be greater than 0" });

    try
    {
        var storage = new RoomStorageModel
        {
            Name = request.Name,
            Capacity = request.Capacity,
            FloorNumber = request.FloorNumber,
            HouseNumber = request.HouseNumber,
            Priority = request.Priority,
            SecondaryPriority = request.SecondaryPriority,
            HasBeds = request.HasBeds,
            NeighboringRooms = request.NeighboringRooms,
            Comments = request.Comments
        };

        var id = await store.CreateAsync("rooms", request.Name, storage);
        queryCache.InvalidateTag("rooms");
        return Results.Created($"/api/rooms/{id}", new RoomDto(
            id, request.Name, request.Capacity, request.FloorNumber, request.HouseNumber,
            request.Priority, request.SecondaryPriority, request.HasBeds, request.NeighboringRooms, request.Comments));
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

    return Results.Ok(new RoomDto(
        id, room.Name ?? "", room.Capacity ?? 0, room.FloorNumber, room.HouseNumber,
        room.Priority, room.SecondaryPriority, room.HasBeds, room.NeighboringRooms, room.Comments));
});

app.MapGet("/api/rooms", async (IKeyValueStore store, IEndpointQueryCache queryCache, string? name) =>
{
    if (!string.IsNullOrEmpty(name))
    {
        var roomDto = await queryCache.GetOrCreateAsync($"rooms:name:{name}", new[] { "rooms" }, async () =>
        {
            var room = await store.GetByNameAsync<RoomStorageModel>("rooms", name);
            if (room is null)
                return null;

            var index = await store.GetIndexAsync("rooms");
            var id = index.TryGetValue(name, out var roomId) ? roomId : "";

            return new RoomDto(
                id, room.Name ?? "", room.Capacity ?? 0, room.FloorNumber, room.HouseNumber,
                room.Priority, room.SecondaryPriority, room.HasBeds, room.NeighboringRooms, room.Comments);
        });

        if (roomDto is null)
            return Results.NotFound();

        return Results.Ok(roomDto);
    }

    var result = await queryCache.GetOrCreateAsync($"rooms:list", new[] { "rooms" }, async () =>
    {
        var rooms = await store.ListAsync<RoomStorageModel>("rooms");
        var index2 = await store.GetIndexAsync("rooms");

        var computed = new List<RoomDto>();
        foreach (var room in rooms)
        {
            var roomName = room.Name ?? "";
            var roomId = index2.TryGetValue(roomName, out var id) ? id : "";
            computed.Add(new RoomDto(
                roomId, room.Name ?? "", room.Capacity ?? 0, room.FloorNumber, room.HouseNumber,
                room.Priority, room.SecondaryPriority, room.HasBeds, room.NeighboringRooms, room.Comments));
        }

        return computed;
    });

    return Results.Ok(result);
});

app.MapPut("/api/rooms/{id}", async (string id, RoomDto request, IKeyValueStore store, IEndpointQueryCache queryCache) =>
{
    var exists = await store.ExistsAsync("rooms", id);
    if (!exists)
        return Results.NotFound();
    
    if (string.IsNullOrWhiteSpace(request.Name))
        return Results.BadRequest(new { error = "Room name is required" });
    if (request.Capacity <= 0)
        return Results.BadRequest(new { error = "Capacity must be greater than 0" });

    var storage = new RoomStorageModel
    {
        Name = request.Name,
        Capacity = request.Capacity,
        FloorNumber = request.FloorNumber,
        HouseNumber = request.HouseNumber,
        Priority = request.Priority,
        SecondaryPriority = request.SecondaryPriority,
        HasBeds = request.HasBeds,
        NeighboringRooms = request.NeighboringRooms,
        Comments = request.Comments
    };

    await store.UpdateAsync("rooms", id, storage);
    queryCache.InvalidateTag("rooms");
    return Results.Ok(new RoomDto(
        id, request.Name, request.Capacity, request.FloorNumber, request.HouseNumber,
        request.Priority, request.SecondaryPriority, request.HasBeds, request.NeighboringRooms, request.Comments));
});

app.MapDelete("/api/rooms/{id}", async (string id, IKeyValueStore store, IEndpointQueryCache queryCache) =>
{
    var exists = await store.ExistsAsync("rooms", id);
    if (!exists)
        return Results.NotFound();

    await store.DeleteAsync("rooms", id);
    queryCache.InvalidateTag("rooms");
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
    // Basic filtering implemented client-side in this prototype by listing and filtering
    var all = await store.ListAsync<InventoryStorageModel>("inventory");
    var filtered = all.AsEnumerable();
    if (!string.IsNullOrEmpty(assetId)) filtered = filtered.Where(x => x.AssetId == assetId);
    if (!string.IsNullOrEmpty(roomId)) filtered = filtered.Where(x => x.RoomId == roomId);
    if (!string.IsNullOrEmpty(warehouseId)) filtered = filtered.Where(x => x.WarehouseId == warehouseId);

    var idx = await store.GetIndexAsync("inventory");
    var list = new List<InventoryDto>();
    foreach (var item in filtered)
    {
        // try to find id by AssetId or by searching idx (best-effort)
        var id = idx.FirstOrDefault(kvp => kvp.Value == item.AssetId).Key ?? "";
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

// --- Export/Import API endpoints ---

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

// --- Nations (nazioni) ---

app.MapPost("/api/nations", async (NationDto request, IKeyValueStore store) =>
{
    if (string.IsNullOrWhiteSpace(request.Name))
        return Results.BadRequest(new { error = "Nation name is required" });

    try
    {
        var storage = new NationStorageModel { Name = request.Name, Code = request.Code, Code2 = request.Code2, Code3 = request.Code3, CreatedAt = request.CreatedAt ?? DateTime.UtcNow };
        var id = await store.CreateAsync("nations", request.Name, storage);
        return Results.Created($"/api/nations/{id}", new NationDto(id, request.Name, request.Code, request.Code2, request.Code3, storage.CreatedAt));
    }
    catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
    {
        return Results.Conflict(new { error = ex.Message });
    }
}).WithName("CreateNation");

app.MapGet("/api/nations/{id}", async (string id, IKeyValueStore store) =>
{
    var n = await store.GetAsync<NationStorageModel>("nations", id);
    if (n is null) return Results.NotFound();
    return Results.Ok(new NationDto(id, n.Name, n.Code, n.Code2, n.Code3, n.CreatedAt));
}).WithName("GetNation");

app.MapGet("/api/nations", async (IKeyValueStore store, string? name) =>
{
    if (!string.IsNullOrEmpty(name))
    {
        var n = await store.GetByNameAsync<NationStorageModel>("nations", name);
        if (n is null) return Results.NotFound();
        var index = await store.GetIndexAsync("nations");
        var id = index.TryGetValue(name, out var nid) ? nid : "";
        return Results.Ok(new NationDto(id, n.Name, n.Code, n.Code2, n.Code3, n.CreatedAt));
    }
    var all = await store.ListAsync<NationStorageModel>("nations");
    var idx = await store.GetIndexAsync("nations");
    var list = new List<NationDto>();
    foreach (var n in all) { var nname = n.Name ?? ""; var nid = idx.TryGetValue(nname, out var i) ? i : ""; list.Add(new NationDto(nid, n.Name, n.Code, n.Code2, n.Code3, n.CreatedAt)); }
    return Results.Ok(list);
}).WithName("ListNations");

app.MapPut("/api/nations/{id}", async (string id, NationDto request, IKeyValueStore store) =>
{
    var exists = await store.ExistsAsync("nations", id);
    if (!exists) return Results.NotFound();
    if (string.IsNullOrWhiteSpace(request.Name)) return Results.BadRequest(new { error = "Nation name is required" });
    var storage = new NationStorageModel { Name = request.Name, Code = request.Code, Code2 = request.Code2, Code3 = request.Code3, CreatedAt = request.CreatedAt ?? DateTime.UtcNow };
    await store.UpdateAsync("nations", id, storage);
    return Results.Ok(new NationDto(id, request.Name, request.Code, request.Code2, request.Code3, storage.CreatedAt));
}).WithName("UpdateNation");

app.MapDelete("/api/nations/{id}", async (string id, IKeyValueStore store) =>
{
    var exists = await store.ExistsAsync("nations", id);
    if (!exists) return Results.NotFound();
    await store.DeleteAsync("nations", id);
    return Results.NoContent();
}).WithName("DeleteNation");

// --- Regions (regioni) ---

app.MapPost("/api/regions", async (RegionDto request, IKeyValueStore store) =>
{
    if (string.IsNullOrWhiteSpace(request.Name))
        return Results.BadRequest(new { error = "Region name is required" });

    try
    {
        var storage = new RegionStorageModel { Name = request.Name, Code = request.Code, Code2 = request.Code2, Code3 = request.Code3, CreatedAt = request.CreatedAt ?? DateTime.UtcNow };
        var id = await store.CreateAsync("regions", request.Name, storage);
        return Results.Created($"/api/regions/{id}", new RegionDto(id, request.Name, request.Code, request.Code2, request.Code3, storage.CreatedAt));
    }
    catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
    {
        return Results.Conflict(new { error = ex.Message });
    }
}).WithName("CreateRegion");

app.MapGet("/api/regions/{id}", async (string id, IKeyValueStore store) =>
{
    var r = await store.GetAsync<RegionStorageModel>("regions", id);
    if (r is null) return Results.NotFound();
    return Results.Ok(new RegionDto(id, r.Name, r.Code, r.Code2, r.Code3, r.CreatedAt));
}).WithName("GetRegion");

app.MapGet("/api/regions", async (IKeyValueStore store, string? name) =>
{
    if (!string.IsNullOrEmpty(name))
    {
        var r = await store.GetByNameAsync<RegionStorageModel>("regions", name);
        if (r is null) return Results.NotFound();
        var index = await store.GetIndexAsync("regions");
        var id = index.TryGetValue(name, out var rid) ? rid : "";
        return Results.Ok(new RegionDto(id, r.Name, r.Code, r.Code2, r.Code3, r.CreatedAt));
    }
    var all = await store.ListAsync<RegionStorageModel>("regions");
    var idx = await store.GetIndexAsync("regions");
    var list = new List<RegionDto>();
    foreach (var r in all) { var rname = r.Name ?? ""; var rid = idx.TryGetValue(rname, out var i) ? i : ""; list.Add(new RegionDto(rid, r.Name, r.Code, r.Code2, r.Code3, r.CreatedAt)); }
    return Results.Ok(list);
}).WithName("ListRegions");

app.MapPut("/api/regions/{id}", async (string id, RegionDto request, IKeyValueStore store) =>
{
    var exists = await store.ExistsAsync("regions", id);
    if (!exists) return Results.NotFound();
    if (string.IsNullOrWhiteSpace(request.Name)) return Results.BadRequest(new { error = "Region name is required" });
    var storage = new RegionStorageModel { Name = request.Name, Code = request.Code, Code2 = request.Code2, Code3 = request.Code3, CreatedAt = request.CreatedAt ?? DateTime.UtcNow };
    await store.UpdateAsync("regions", id, storage);
    return Results.Ok(new RegionDto(id, request.Name, request.Code, request.Code2, request.Code3, storage.CreatedAt));
}).WithName("UpdateRegion");

app.MapDelete("/api/regions/{id}", async (string id, IKeyValueStore store) =>
{
    var exists = await store.ExistsAsync("regions", id);
    if (!exists) return Results.NotFound();
    await store.DeleteAsync("regions", id);
    return Results.NoContent();
}).WithName("DeleteRegion");

// --- Cities (citta) ---

app.MapPost("/api/cities", async (CityDto request, IKeyValueStore store) =>
{
    if (string.IsNullOrWhiteSpace(request.Name))
        return Results.BadRequest(new { error = "City name is required" });

    try
    {
        var storage = new CityStorageModel { Name = request.Name, Code = request.Code, Code2 = request.Code2, Code3 = request.Code3, CreatedAt = request.CreatedAt ?? DateTime.UtcNow };
        var id = await store.CreateAsync("cities", request.Name, storage);
        return Results.Created($"/api/cities/{id}", new CityDto(id, request.Name, request.Code, request.Code2, request.Code3, storage.CreatedAt));
    }
    catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
    {
        return Results.Conflict(new { error = ex.Message });
    }
}).WithName("CreateCity");

app.MapGet("/api/cities/{id}", async (string id, IKeyValueStore store) =>
{
    var c = await store.GetAsync<CityStorageModel>("cities", id);
    if (c is null) return Results.NotFound();
    return Results.Ok(new CityDto(id, c.Name, c.Code, c.Code2, c.Code3, c.CreatedAt));
}).WithName("GetCity");

app.MapGet("/api/cities", async (IKeyValueStore store, string? name) =>
{
    if (!string.IsNullOrEmpty(name))
    {
        var c = await store.GetByNameAsync<CityStorageModel>("cities", name);
        if (c is null) return Results.NotFound();
        var index = await store.GetIndexAsync("cities");
        var id = index.TryGetValue(name, out var cid) ? cid : "";
        return Results.Ok(new CityDto(id, c.Name, c.Code, c.Code2, c.Code3, c.CreatedAt));
    }
    var all = await store.ListAsync<CityStorageModel>("cities");
    var idx = await store.GetIndexAsync("cities");
    var list = new List<CityDto>();
    foreach (var c in all) { var cname = c.Name ?? ""; var cid = idx.TryGetValue(cname, out var i) ? i : ""; list.Add(new CityDto(cid, c.Name, c.Code, c.Code2, c.Code3, c.CreatedAt)); }
    return Results.Ok(list);
}).WithName("ListCities");

app.MapPut("/api/cities/{id}", async (string id, CityDto request, IKeyValueStore store) =>
{
    var exists = await store.ExistsAsync("cities", id);
    if (!exists) return Results.NotFound();
    if (string.IsNullOrWhiteSpace(request.Name)) return Results.BadRequest(new { error = "City name is required" });
    var storage = new CityStorageModel { Name = request.Name, Code = request.Code, Code2 = request.Code2, Code3 = request.Code3, CreatedAt = request.CreatedAt ?? DateTime.UtcNow };
    await store.UpdateAsync("cities", id, storage);
    return Results.Ok(new CityDto(id, request.Name, request.Code, request.Code2, request.Code3, storage.CreatedAt));
}).WithName("UpdateCity");

app.MapDelete("/api/cities/{id}", async (string id, IKeyValueStore store) =>
{
    var exists = await store.ExistsAsync("cities", id);
    if (!exists) return Results.NotFound();
    await store.DeleteAsync("cities", id);
    return Results.NoContent();
}).WithName("DeleteCity");

// --- Identity Document Types (documentiid) ---

app.MapPost("/api/identity-document-types", async (IdentityDocumentTypeDto request, IKeyValueStore store) =>
{
    if (string.IsNullOrWhiteSpace(request.Name))
        return Results.BadRequest(new { error = "Identity document type name is required" });

    try
    {
        var storage = new IdentityDocumentTypeStorageModel { Name = request.Name, Code = request.Code, Code2 = request.Code2, Code3 = request.Code3, CreatedAt = request.CreatedAt ?? DateTime.UtcNow };
        var id = await store.CreateAsync("identity_document_types", request.Name, storage);
        return Results.Created($"/api/identity-document-types/{id}", new IdentityDocumentTypeDto(id, request.Name, request.Code, request.Code2, request.Code3, storage.CreatedAt));
    }
    catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
    {
        return Results.Conflict(new { error = ex.Message });
    }
}).WithName("CreateIdentityDocumentType");

app.MapGet("/api/identity-document-types/{id}", async (string id, IKeyValueStore store) =>
{
    var d = await store.GetAsync<IdentityDocumentTypeStorageModel>("identity_document_types", id);
    if (d is null) return Results.NotFound();
    return Results.Ok(new IdentityDocumentTypeDto(id, d.Name, d.Code, d.Code2, d.Code3, d.CreatedAt));
}).WithName("GetIdentityDocumentType");

app.MapGet("/api/identity-document-types", async (IKeyValueStore store, string? name) =>
{
    if (!string.IsNullOrEmpty(name))
    {
        var d = await store.GetByNameAsync<IdentityDocumentTypeStorageModel>("identity_document_types", name);
        if (d is null) return Results.NotFound();
        var index = await store.GetIndexAsync("identity_document_types");
        var id = index.TryGetValue(name, out var did) ? did : "";
        return Results.Ok(new IdentityDocumentTypeDto(id, d.Name, d.Code, d.Code2, d.Code3, d.CreatedAt));
    }
    var all = await store.ListAsync<IdentityDocumentTypeStorageModel>("identity_document_types");
    var idx = await store.GetIndexAsync("identity_document_types");
    var list = new List<IdentityDocumentTypeDto>();
    foreach (var d in all) { var dname = d.Name ?? ""; var did = idx.TryGetValue(dname, out var i) ? i : ""; list.Add(new IdentityDocumentTypeDto(did, d.Name, d.Code, d.Code2, d.Code3, d.CreatedAt)); }
    return Results.Ok(list);
}).WithName("ListIdentityDocumentTypes");

app.MapPut("/api/identity-document-types/{id}", async (string id, IdentityDocumentTypeDto request, IKeyValueStore store) =>
{
    var exists = await store.ExistsAsync("identity_document_types", id);
    if (!exists) return Results.NotFound();
    if (string.IsNullOrWhiteSpace(request.Name)) return Results.BadRequest(new { error = "Identity document type name is required" });
    var storage = new IdentityDocumentTypeStorageModel { Name = request.Name, Code = request.Code, Code2 = request.Code2, Code3 = request.Code3, CreatedAt = request.CreatedAt ?? DateTime.UtcNow };
    await store.UpdateAsync("identity_document_types", id, storage);
    return Results.Ok(new IdentityDocumentTypeDto(id, request.Name, request.Code, request.Code2, request.Code3, storage.CreatedAt));
}).WithName("UpdateIdentityDocumentType");

app.MapDelete("/api/identity-document-types/{id}", async (string id, IKeyValueStore store) =>
{
    var exists = await store.ExistsAsync("identity_document_types", id);
    if (!exists) return Results.NotFound();
    await store.DeleteAsync("identity_document_types", id);
    return Results.NoContent();
}).WithName("DeleteIdentityDocumentType");

// --- Family Relationships (parentele) ---

app.MapPost("/api/family-relationships", async (FamilyRelationshipDto request, IKeyValueStore store) =>
{
    if (string.IsNullOrWhiteSpace(request.Name))
        return Results.BadRequest(new { error = "Family relationship name is required" });

    try
    {
        var storage = new FamilyRelationshipStorageModel { Name = request.Name, Code = request.Code, Code2 = request.Code2, Code3 = request.Code3, CreatedAt = request.CreatedAt ?? DateTime.UtcNow };
        var id = await store.CreateAsync("family_relationships", request.Name, storage);
        return Results.Created($"/api/family-relationships/{id}", new FamilyRelationshipDto(id, request.Name, request.Code, request.Code2, request.Code3, storage.CreatedAt));
    }
    catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
    {
        return Results.Conflict(new { error = ex.Message });
    }
}).WithName("CreateFamilyRelationship");

app.MapGet("/api/family-relationships/{id}", async (string id, IKeyValueStore store) =>
{
    var f = await store.GetAsync<FamilyRelationshipStorageModel>("family_relationships", id);
    if (f is null) return Results.NotFound();
    return Results.Ok(new FamilyRelationshipDto(id, f.Name, f.Code, f.Code2, f.Code3, f.CreatedAt));
}).WithName("GetFamilyRelationship");

app.MapGet("/api/family-relationships", async (IKeyValueStore store, string? name) =>
{
    if (!string.IsNullOrEmpty(name))
    {
        var f = await store.GetByNameAsync<FamilyRelationshipStorageModel>("family_relationships", name);
        if (f is null) return Results.NotFound();
        var index = await store.GetIndexAsync("family_relationships");
        var id = index.TryGetValue(name, out var fid) ? fid : "";
        return Results.Ok(new FamilyRelationshipDto(id, f.Name, f.Code, f.Code2, f.Code3, f.CreatedAt));
    }
    var all = await store.ListAsync<FamilyRelationshipStorageModel>("family_relationships");
    var idx = await store.GetIndexAsync("family_relationships");
    var list = new List<FamilyRelationshipDto>();
    foreach (var f in all) { var fname = f.Name ?? ""; var fid = idx.TryGetValue(fname, out var i) ? i : ""; list.Add(new FamilyRelationshipDto(fid, f.Name, f.Code, f.Code2, f.Code3, f.CreatedAt)); }
    return Results.Ok(list);
}).WithName("ListFamilyRelationships");

app.MapPut("/api/family-relationships/{id}", async (string id, FamilyRelationshipDto request, IKeyValueStore store) =>
{
    var exists = await store.ExistsAsync("family_relationships", id);
    if (!exists) return Results.NotFound();
    if (string.IsNullOrWhiteSpace(request.Name)) return Results.BadRequest(new { error = "Family relationship name is required" });
    var storage = new FamilyRelationshipStorageModel { Name = request.Name, Code = request.Code, Code2 = request.Code2, Code3 = request.Code3, CreatedAt = request.CreatedAt ?? DateTime.UtcNow };
    await store.UpdateAsync("family_relationships", id, storage);
    return Results.Ok(new FamilyRelationshipDto(id, request.Name, request.Code, request.Code2, request.Code3, storage.CreatedAt));
}).WithName("UpdateFamilyRelationship");

app.MapDelete("/api/family-relationships/{id}", async (string id, IKeyValueStore store) =>
{
    var exists = await store.ExistsAsync("family_relationships", id);
    if (!exists) return Results.NotFound();
    await store.DeleteAsync("family_relationships", id);
    return Results.NoContent();
}).WithName("DeleteFamilyRelationship");

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

ClientFullDto ToClientFullDto(string id, ClientStorageModel s) => new(
    id, s.LastName, s.FirstName, s.Nickname, s.Gender, s.Title, s.Language,
    s.DateOfBirth, s.BirthCity, s.BirthRegion, s.BirthNation,
    s.DocumentNumber, s.DocumentExpiry, s.DocumentType, s.DocumentCity, s.DocumentRegion, s.DocumentNation,
    s.Nationality, s.Nation, s.Region, s.City, s.Street, s.StreetNumber, s.PostalCode,
    s.Phone, s.Phone2, s.Phone3, s.Fax, s.Email, s.Email2, s.Email3,
    s.TaxCode, s.VatNumber, s.Notes, s.MaxOrderNumber, s.CompanionIds, s.DocumentsSent,
    s.CreatedAt, s.HostCreated, s.CreatedBy);

app.MapPost("/api/clients", async (ClientFullDto request, IKeyValueStore store) =>
{
    if (string.IsNullOrWhiteSpace(request.LastName))
        return Results.BadRequest(new { error = "LastName is required" });
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
        Phone = request.Phone, Phone2 = request.Phone2, Phone3 = request.Phone3, Fax = request.Fax,
        Email = request.Email, Email2 = request.Email2, Email3 = request.Email3,
        TaxCode = request.TaxCode, VatNumber = request.VatNumber, Notes = request.Notes,
        MaxOrderNumber = request.MaxOrderNumber, CompanionIds = request.CompanionIds,
        DocumentsSent = request.DocumentsSent, CreatedAt = request.CreatedAt ?? DateTime.UtcNow,
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

app.MapGet("/api/clients", async (IKeyValueStore store, string? lastName) =>
{
    var idx = await store.GetIndexAsync("clients");
    var result = new List<ClientFullDto>();
    foreach (var kvp in idx)
    {
        var c = await store.GetAsync<ClientStorageModel>("clients", kvp.Value);
        if (c is null) continue;
        if (!string.IsNullOrEmpty(lastName) && (c.LastName == null || !c.LastName.Contains(lastName, StringComparison.OrdinalIgnoreCase))) continue;
        result.Add(ToClientFullDto(kvp.Value, c));
    }
    return Results.Ok(result);
}).WithName("ListClients");

app.MapPut("/api/clients/{id}", async (string id, ClientFullDto request, IKeyValueStore store) =>
{
    var exists = await store.ExistsAsync("clients", id);
    if (!exists) return Results.NotFound();
    if (string.IsNullOrWhiteSpace(request.LastName)) return Results.BadRequest(new { error = "LastName is required" });
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
        Phone = request.Phone, Phone2 = request.Phone2, Phone3 = request.Phone3, Fax = request.Fax,
        Email = request.Email, Email2 = request.Email2, Email3 = request.Email3,
        TaxCode = request.TaxCode, VatNumber = request.VatNumber, Notes = request.Notes,
        MaxOrderNumber = request.MaxOrderNumber, CompanionIds = request.CompanionIds,
        DocumentsSent = request.DocumentsSent, CreatedAt = request.CreatedAt ?? DateTime.UtcNow,
        HostCreated = request.HostCreated, CreatedBy = request.CreatedBy
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

// --- Client Data (relclienti) ---

ClientDataDto ToClientDataDto(string id, ClientDataStorageModel s) => new(
    id, s.ClientId, s.Number, s.Type, s.Text1, s.Text2, s.Text3, s.Text4,
    s.Text5, s.Text6, s.Text7, s.Text8, s.CreatedAt, s.HostCreated, s.CreatedBy);

app.MapPost("/api/client-data", async (ClientDataDto request, IKeyValueStore store) =>
{
    if (string.IsNullOrWhiteSpace(request.ClientId))
        return Results.BadRequest(new { error = "ClientId is required" });
    var storage = new ClientDataStorageModel
    {
        ClientId = request.ClientId, Number = request.Number, Type = request.Type,
        Text1 = request.Text1, Text2 = request.Text2, Text3 = request.Text3, Text4 = request.Text4,
        Text5 = request.Text5, Text6 = request.Text6, Text7 = request.Text7, Text8 = request.Text8,
        CreatedAt = request.CreatedAt ?? DateTime.UtcNow, HostCreated = request.HostCreated, CreatedBy = request.CreatedBy
    };
    var key = $"{request.ClientId}_{request.Number}_{DateTime.UtcNow.Ticks}";
    var id = await store.CreateAsync("client_data", key, storage);
    return Results.Created($"/api/client-data/{id}", ToClientDataDto(id, storage));
}).WithName("CreateClientData");

app.MapGet("/api/client-data", async (IKeyValueStore store, string? clientId) =>
{
    var idx = await store.GetIndexAsync("client_data");
    var result = new List<ClientDataDto>();
    foreach (var kvp in idx)
    {
        var d = await store.GetAsync<ClientDataStorageModel>("client_data", kvp.Value);
        if (d is null) continue;
        if (!string.IsNullOrEmpty(clientId) && d.ClientId != clientId) continue;
        result.Add(ToClientDataDto(kvp.Value, d));
    }
    return Results.Ok(result);
}).WithName("ListClientData");

app.MapPut("/api/client-data/{id}", async (string id, ClientDataDto request, IKeyValueStore store) =>
{
    var exists = await store.ExistsAsync("client_data", id);
    if (!exists) return Results.NotFound();
    var storage = new ClientDataStorageModel
    {
        ClientId = request.ClientId, Number = request.Number, Type = request.Type,
        Text1 = request.Text1, Text2 = request.Text2, Text3 = request.Text3, Text4 = request.Text4,
        Text5 = request.Text5, Text6 = request.Text6, Text7 = request.Text7, Text8 = request.Text8,
        CreatedAt = request.CreatedAt ?? DateTime.UtcNow, HostCreated = request.HostCreated, CreatedBy = request.CreatedBy
    };
    await store.UpdateAsync("client_data", id, storage);
    return Results.Ok(ToClientDataDto(id, storage));
}).WithName("UpdateClientData");

app.MapDelete("/api/client-data/{id}", async (string id, IKeyValueStore store) =>
{
    var exists = await store.ExistsAsync("client_data", id);
    if (!exists) return Results.NotFound();
    await store.DeleteAsync("client_data", id);
    return Results.NoContent();
}).WithName("DeleteClientData");

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

// --- Layer 3: Tariffs (ntariffe{year}) ---

TariffDto ToTariffDto(string id, TariffStorageModel s) => new(
    id, s.Year, s.ExtraCostName, s.CostType, s.BaseValue, s.PercentageValue, s.TaxPercentage, s.Category, s.NumberLimit);

app.MapGet("/api/tariffs", async (IKeyValueStore store, int? year) =>
{
    var idx = await store.GetIndexAsync("tariffs");
    var prefix = year.HasValue ? $"{year}_" : null;
    var result = new List<TariffDto>();
    foreach (var kvp in idx)
    {
        if (prefix != null && !kvp.Key.StartsWith(prefix)) continue;
        var t = await store.GetAsync<TariffStorageModel>("tariffs", kvp.Value);
        if (t is null) continue;
        result.Add(ToTariffDto(kvp.Value, t));
    }
    return Results.Ok(result);
}).WithName("ListTariffs");

app.MapGet("/api/tariffs/{year:int}/{id}", async (int year, string id, IKeyValueStore store) =>
{
    var t = await store.GetAsync<TariffStorageModel>("tariffs", id);
    if (t is null || t.Year != year) return Results.NotFound();
    return Results.Ok(ToTariffDto(id, t));
}).WithName("GetTariff");

app.MapPost("/api/tariffs", async (TariffDto request, IKeyValueStore store) =>
{
    var guid = Guid.NewGuid().ToString("N");
    var key = $"{request.Year}_{guid}";
    var storage = new TariffStorageModel
    {
        Year = request.Year, ExtraCostName = request.ExtraCostName, CostType = request.CostType,
        BaseValue = request.BaseValue, PercentageValue = request.PercentageValue,
        TaxPercentage = request.TaxPercentage, Category = request.Category, NumberLimit = request.NumberLimit
    };
    var id = await store.CreateAsync("tariffs", key, storage);
    return Results.Created($"/api/tariffs/{request.Year}/{id}", ToTariffDto(id, storage));
}).WithName("CreateTariff");

app.MapPut("/api/tariffs/{year:int}/{id}", async (int year, string id, TariffDto request, IKeyValueStore store) =>
{
    var t = await store.GetAsync<TariffStorageModel>("tariffs", id);
    if (t is null || t.Year != year) return Results.NotFound();
    var storage = new TariffStorageModel
    {
        Year = year, ExtraCostName = request.ExtraCostName, CostType = request.CostType,
        BaseValue = request.BaseValue, PercentageValue = request.PercentageValue,
        TaxPercentage = request.TaxPercentage, Category = request.Category, NumberLimit = request.NumberLimit
    };
    await store.UpdateAsync("tariffs", id, storage);
    return Results.Ok(ToTariffDto(id, storage));
}).WithName("UpdateTariff");

app.MapDelete("/api/tariffs/{year:int}/{id}", async (int year, string id, IKeyValueStore store) =>
{
    var t = await store.GetAsync<TariffStorageModel>("tariffs", id);
    if (t is null || t.Year != year) return Results.NotFound();
    await store.DeleteAsync("tariffs", id);
    return Results.NoContent();
}).WithName("DeleteTariff");

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

BookingDto ToBookingDto(string id, BookingStorageModel s) => new(id, s.Year, s.ClientId, s.RoomId, s.ArrivalDate, s.DepartureDate, s.Status, s.Notes);

app.MapGet("/api/bookings", async (IKeyValueStore store, IEndpointQueryCache queryCache, int? year) =>
{
    var key = year.HasValue ? $"bookings:list:year:{year.Value}" : "bookings:list:all";
    var result = await queryCache.GetOrCreateAsync(key, new[] { "bookings" }, async () =>
    {
        var idx = await store.GetIndexAsync("bookings");
        var prefix = year.HasValue ? $"{year}_" : null;
        var computed = new List<BookingDto>();
        foreach (var kvp in idx)
        {
            if (prefix != null && !kvp.Key.StartsWith(prefix)) continue;
            var b = await store.GetAsync<BookingStorageModel>("bookings", kvp.Value);
            if (b is null) continue;
            computed.Add(ToBookingDto(kvp.Value, b));
        }

        return computed;
    });

    return Results.Ok(result);
}).WithName("ListBookings");

app.MapGet("/api/bookings/{year:int}/{id}", async (int year, string id, IKeyValueStore store) =>
{
    var b = await store.GetAsync<BookingStorageModel>("bookings", id);
    if (b is null || b.Year != year) return Results.NotFound();
    return Results.Ok(ToBookingDto(id, b));
}).WithName("GetBooking");

app.MapPost("/api/bookings", async (BookingDto request, IKeyValueStore store, IEndpointQueryCache queryCache) =>
{
    var guid = Guid.NewGuid().ToString("N");
    var key = $"{request.Year}_{guid}";
    var storage = new BookingStorageModel { Year = request.Year, ClientId = request.ClientId, RoomId = request.RoomId, ArrivalDate = request.ArrivalDate, DepartureDate = request.DepartureDate, Status = request.Status, Notes = request.Notes };
    var id = await store.CreateAsync("bookings", key, storage);
    queryCache.InvalidateTag("bookings");
    return Results.Created($"/api/bookings/{request.Year}/{id}", ToBookingDto(id, storage));
}).WithName("CreateBooking");

app.MapPut("/api/bookings/{year:int}/{id}", async (int year, string id, BookingDto request, IKeyValueStore store, IEndpointQueryCache queryCache) =>
{
    var b = await store.GetAsync<BookingStorageModel>("bookings", id);
    if (b is null || b.Year != year) return Results.NotFound();
    var storage = new BookingStorageModel { Year = year, ClientId = request.ClientId, RoomId = request.RoomId, ArrivalDate = request.ArrivalDate, DepartureDate = request.DepartureDate, Status = request.Status, Notes = request.Notes };
    await store.UpdateAsync("bookings", id, storage);
    queryCache.InvalidateTag("bookings");
    return Results.Ok(ToBookingDto(id, storage));
}).WithName("UpdateBooking");

app.MapDelete("/api/bookings/{year:int}/{id}", async (int year, string id, IKeyValueStore store, IEndpointQueryCache queryCache) =>
{
    var b = await store.GetAsync<BookingStorageModel>("bookings", id);
    if (b is null || b.Year != year) return Results.NotFound();
    await store.DeleteAsync("bookings", id);
    queryCache.InvalidateTag("bookings");
    return Results.NoContent();
}).WithName("DeleteBooking");

app.MapPost("/api/bookings/{year:int}/{id}/cancel", async (int year, string id, IKeyValueStore store, IEndpointQueryCache queryCache) =>
{
    var b = await store.GetAsync<BookingStorageModel>("bookings", id);
    if (b is null || b.Year != year) return Results.NotFound();
    var cancelGuid = Guid.NewGuid().ToString("N");
    var cancelKey = $"{year}_{cancelGuid}";
    var cancelled = new CancelledBookingStorageModel { Year = year, ClientId = b.ClientId, RoomId = b.RoomId, ArrivalDate = b.ArrivalDate, DepartureDate = b.DepartureDate, Status = b.Status, CancelledAt = DateTime.UtcNow, Notes = b.Notes };
    await store.CreateAsync("cancelled_bookings", cancelKey, cancelled);
    await store.DeleteAsync("bookings", id);
    queryCache.InvalidateTag("bookings");
    queryCache.InvalidateTag("cancelled_bookings");
    return Results.NoContent();
}).WithName("CancelBooking");

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
        if (c is null) continue;
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

app.MapPost("/api/booking-costs", async (BookingCostDto request, IKeyValueStore store) =>
{
    var guid = Guid.NewGuid().ToString("N");
    var key = $"{request.Year}_{guid}";
    var storage = new BookingCostStorageModel { Year = request.Year, BookingId = request.BookingId, TariffId = request.TariffId, Amount = request.Amount, Description = request.Description };
    var id = await store.CreateAsync("booking_costs", key, storage);
    return Results.Created($"/api/booking-costs/{request.Year}/{id}", ToBookingCostDto(id, storage));
}).WithName("CreateBookingCost");

app.MapPut("/api/booking-costs/{year:int}/{id}", async (int year, string id, BookingCostDto request, IKeyValueStore store) =>
{
    var c = await store.GetAsync<BookingCostStorageModel>("booking_costs", id);
    if (c is null || c.Year != year) return Results.NotFound();
    var storage = new BookingCostStorageModel { Year = year, BookingId = request.BookingId, TariffId = request.TariffId, Amount = request.Amount, Description = request.Description };
    await store.UpdateAsync("booking_costs", id, storage);
    return Results.Ok(ToBookingCostDto(id, storage));
}).WithName("UpdateBookingCost");

app.MapDelete("/api/booking-costs/{year:int}/{id}", async (int year, string id, IKeyValueStore store) =>
{
    var c = await store.GetAsync<BookingCostStorageModel>("booking_costs", id);
    if (c is null || c.Year != year) return Results.NotFound();
    await store.DeleteAsync("booking_costs", id);
    return Results.NoContent();
}).WithName("DeleteBookingCost");

// --- Layer 4: BookingGuests (ospiti{year}) ---

BookingGuestDto ToBookingGuestDto(string id, BookingGuestStorageModel s) => new(id, s.Year, s.BookingId, s.ClientId, s.GuestNumber);

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
    var storage = new BookingGuestStorageModel { Year = request.Year, BookingId = request.BookingId, ClientId = request.ClientId, GuestNumber = request.GuestNumber };
    var id = await store.CreateAsync("booking_guests", key, storage);
    return Results.Created($"/api/booking-guests/{request.Year}/{id}", ToBookingGuestDto(id, storage));
}).WithName("CreateBookingGuest");

app.MapPut("/api/booking-guests/{year:int}/{id}", async (int year, string id, BookingGuestDto request, IKeyValueStore store) =>
{
    var g = await store.GetAsync<BookingGuestStorageModel>("booking_guests", id);
    if (g is null || g.Year != year) return Results.NotFound();
    var storage = new BookingGuestStorageModel { Year = year, BookingId = request.BookingId, ClientId = request.ClientId, GuestNumber = request.GuestNumber };
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

CancelledBookingDto ToCancelledBookingDto(string id, CancelledBookingStorageModel s) => new(id, s.Year, s.ClientId, s.RoomId, s.ArrivalDate, s.DepartureDate, s.Status, s.CancelledAt, s.Notes);

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

ExpenseDto ToExpenseDto(string id, ExpenseStorageModel s) => new(id, s.Year, s.CashRegisterId, s.Amount, s.Description, s.Date);

app.MapGet("/api/expenses", async (IKeyValueStore store, int? year) =>
{
    var idx = await store.GetIndexAsync("expenses");
    var prefix = year.HasValue ? $"{year}_" : null;
    var result = new List<ExpenseDto>();
    foreach (var kvp in idx)
    {
        if (prefix != null && !kvp.Key.StartsWith(prefix)) continue;
        var e = await store.GetAsync<ExpenseStorageModel>("expenses", kvp.Value);
        if (e is null) continue;
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

app.MapPost("/api/expenses", async (ExpenseDto request, IKeyValueStore store) =>
{
    var guid = Guid.NewGuid().ToString("N");
    var key = $"{request.Year}_{guid}";
    var storage = new ExpenseStorageModel { Year = request.Year, CashRegisterId = request.CashRegisterId, Amount = request.Amount, Description = request.Description, Date = request.Date };
    var id = await store.CreateAsync("expenses", key, storage);
    return Results.Created($"/api/expenses/{request.Year}/{id}", ToExpenseDto(id, storage));
}).WithName("CreateExpense");

app.MapPut("/api/expenses/{year:int}/{id}", async (int year, string id, ExpenseDto request, IKeyValueStore store) =>
{
    var e = await store.GetAsync<ExpenseStorageModel>("expenses", id);
    if (e is null || e.Year != year) return Results.NotFound();
    var storage = new ExpenseStorageModel { Year = year, CashRegisterId = request.CashRegisterId, Amount = request.Amount, Description = request.Description, Date = request.Date };
    await store.UpdateAsync("expenses", id, storage);
    return Results.Ok(ToExpenseDto(id, storage));
}).WithName("UpdateExpense");

app.MapDelete("/api/expenses/{year:int}/{id}", async (int year, string id, IKeyValueStore store) =>
{
    var e = await store.GetAsync<ExpenseStorageModel>("expenses", id);
    if (e is null || e.Year != year) return Results.NotFound();
    await store.DeleteAsync("expenses", id);
    return Results.NoContent();
}).WithName("DeleteExpense");

// --- Layer 4: MoneyHistory (movimenticassa{year}) - append-only, no PUT ---

MoneyHistoryDto ToMoneyHistoryDto(string id, MoneyHistoryStorageModel s) => new(id, s.Year, s.CashRegisterId, s.Amount, s.Type, s.Description, s.Date);

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

app.MapPost("/api/money-history", async (MoneyHistoryDto request, IKeyValueStore store) =>
{
    var guid = Guid.NewGuid().ToString("N");
    var key = $"{request.Year}_{guid}";
    var storage = new MoneyHistoryStorageModel { Year = request.Year, CashRegisterId = request.CashRegisterId, Amount = request.Amount, Type = request.Type, Description = request.Description, Date = request.Date };
    var id = await store.CreateAsync("money_history", key, storage);
    return Results.Created($"/api/money-history/{request.Year}/{id}", ToMoneyHistoryDto(id, storage));
}).WithName("CreateMoneyHistory");

app.MapDelete("/api/money-history/{year:int}/{id}", async (int year, string id, IKeyValueStore store) =>
{
    var m = await store.GetAsync<MoneyHistoryStorageModel>("money_history", id);
    if (m is null || m.Year != year) return Results.NotFound();
    await store.DeleteAsync("money_history", id);
    return Results.NoContent();
}).WithName("DeleteMoneyHistory");

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

// --- Layer 5: ExternalIntegrations (interconnessioni) ---

ExternalIntegrationDto ToExternalIntegrationDto(string id, ExternalIntegrationStorageModel s) => new(id, s.IntegrationName, s.IdType, s.LocalId, s.RemoteId1, s.RemoteId2, s.Year, s.CreatedAt);

app.MapGet("/api/external-integrations", async (IKeyValueStore store, int? year, string? name) =>
{
    var idx = await store.GetIndexAsync("external_integrations");
    var result = new List<ExternalIntegrationDto>();
    foreach (var kvp in idx)
    {
        var ei = await store.GetAsync<ExternalIntegrationStorageModel>("external_integrations", kvp.Value);
        if (ei is null) continue;
        if (year.HasValue && ei.Year != year) continue;
        if (name != null && (ei.IntegrationName == null || !ei.IntegrationName.Contains(name, StringComparison.OrdinalIgnoreCase))) continue;
        result.Add(ToExternalIntegrationDto(kvp.Value, ei));
    }
    return Results.Ok(result);
}).WithName("ListExternalIntegrations");

app.MapGet("/api/external-integrations/{id}", async (string id, IKeyValueStore store) =>
{
    var ei = await store.GetAsync<ExternalIntegrationStorageModel>("external_integrations", id);
    if (ei is null) return Results.NotFound();
    return Results.Ok(ToExternalIntegrationDto(id, ei));
}).WithName("GetExternalIntegration");

app.MapPost("/api/external-integrations", async (ExternalIntegrationDto request, IKeyValueStore store) =>
{
    if (string.IsNullOrWhiteSpace(request.IntegrationName))
        return Results.BadRequest("IntegrationName is required.");
    var name = Guid.NewGuid().ToString("N");
    var storage = new ExternalIntegrationStorageModel
    {
        IntegrationName = request.IntegrationName,
        IdType = request.IdType,
        LocalId = request.LocalId,
        RemoteId1 = request.RemoteId1,
        RemoteId2 = request.RemoteId2,
        Year = request.Year,
        CreatedAt = request.CreatedAt ?? DateTime.UtcNow
    };
    var id = await store.CreateAsync("external_integrations", name, storage);
    return Results.Created($"/api/external-integrations/{id}", ToExternalIntegrationDto(id, storage));
}).WithName("CreateExternalIntegration");

app.MapPut("/api/external-integrations/{id}", async (string id, ExternalIntegrationDto request, IKeyValueStore store) =>
{
    var ei = await store.GetAsync<ExternalIntegrationStorageModel>("external_integrations", id);
    if (ei is null) return Results.NotFound();
    ei.IntegrationName = request.IntegrationName ?? ei.IntegrationName;
    ei.IdType = request.IdType ?? ei.IdType;
    ei.LocalId = request.LocalId ?? ei.LocalId;
    ei.RemoteId1 = request.RemoteId1 ?? ei.RemoteId1;
    ei.RemoteId2 = request.RemoteId2 ?? ei.RemoteId2;
    ei.Year = request.Year ?? ei.Year;
    await store.UpdateAsync("external_integrations", id, ei);
    return Results.Ok(ToExternalIntegrationDto(id, ei));
}).WithName("UpdateExternalIntegration");

app.MapDelete("/api/external-integrations/{id}", async (string id, IKeyValueStore store) =>
{
    var ei = await store.GetAsync<ExternalIntegrationStorageModel>("external_integrations", id);
    if (ei is null) return Results.NotFound();
    await store.DeleteAsync("external_integrations", id);
    return Results.NoContent();
}).WithName("DeleteExternalIntegration");

// --- Layer 5: Sessions (sessioni) ---

static string SanitizeSessionId(string sessionId) =>
    System.Text.RegularExpressions.Regex.Replace(sessionId, @"[^a-zA-Z0-9_]", "_");

SessionDto ToSessionDto(string id, SessionStorageModel s) => new(id, s.UserId, s.IpAddress, s.ConnectionType, s.LastAccess);

app.MapGet("/api/sessions", async (IKeyValueStore store, int? userId) =>
{
    var idx = await store.GetIndexAsync("sessions");
    var result = new List<SessionDto>();
    foreach (var kvp in idx)
    {
        var s = await store.GetAsync<SessionStorageModel>("sessions", kvp.Value);
        if (s is null) continue;
        if (userId.HasValue && s.UserId != userId) continue;
        result.Add(ToSessionDto(kvp.Value, s));
    }
    return Results.Ok(result);
}).WithName("ListSessions");

app.MapGet("/api/sessions/{sessionId}", async (string sessionId, IKeyValueStore store) =>
{
    var key = SanitizeSessionId(sessionId);
    var sessIdx = await store.GetIndexAsync("sessions");
    if (!sessIdx.TryGetValue(key, out var sessId)) return Results.NotFound();
    var s = await store.GetAsync<SessionStorageModel>("sessions", sessId);
    if (s is null) return Results.NotFound();
    return Results.Ok(ToSessionDto(key, s));
}).WithName("GetSession");

app.MapPost("/api/sessions", async (SessionDto request, IKeyValueStore store) =>
{
    if (string.IsNullOrWhiteSpace(request.SessionId) || !request.UserId.HasValue)
        return Results.BadRequest("SessionId and UserId are required.");
    var key = SanitizeSessionId(request.SessionId);
    var storage = new SessionStorageModel
    {
        UserId = request.UserId,
        IpAddress = request.IpAddress,
        ConnectionType = request.ConnectionType,
        LastAccess = request.LastAccess ?? DateTime.UtcNow
    };
    await store.CreateAsync("sessions", key, storage);
    return Results.Created($"/api/sessions/{key}", ToSessionDto(key, storage));
}).WithName("CreateSession");

app.MapPut("/api/sessions/{sessionId}/touch", async (string sessionId, IKeyValueStore store) =>
{
    var key = SanitizeSessionId(sessionId);
    var touchIdx = await store.GetIndexAsync("sessions");
    if (!touchIdx.TryGetValue(key, out var touchId)) return Results.NotFound();
    var s = await store.GetAsync<SessionStorageModel>("sessions", touchId);
    if (s is null) return Results.NotFound();
    s.LastAccess = DateTime.UtcNow;
    await store.UpdateAsync("sessions", touchId, s);
    return Results.Ok(ToSessionDto(key, s));
}).WithName("TouchSession");

app.MapDelete("/api/sessions/{sessionId}", async (string sessionId, IKeyValueStore store) =>
{
    var key = SanitizeSessionId(sessionId);
    var delSessIdx = await store.GetIndexAsync("sessions");
    if (!delSessIdx.TryGetValue(key, out var delSessId)) return Results.NotFound();
    await store.DeleteAsync("sessions", delSessId);
    return Results.NoContent();
}).WithName("DeleteSession");

app.MapDelete("/api/sessions", async (IKeyValueStore store, int? userId) =>
{
    if (!userId.HasValue) return Results.BadRequest("userId query parameter is required.");
    var idx = await store.GetIndexAsync("sessions");
    foreach (var kvp in idx)
    {
        var s = await store.GetAsync<SessionStorageModel>("sessions", kvp.Value);
        if (s?.UserId == userId) await store.DeleteAsync("sessions", kvp.Value);
    }
    return Results.NoContent();
}).WithName("DeleteSessionsForUser");

app.Run();