using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;
using HotelDroid.Api.Services;
using HotelDroid.Api.Services.ExportImport;
using HotelDroid.Api.Models;
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
                options.ListenLocalhost(5001, listenOptions => listenOptions.UseHttps(cert));
            });
            Console.WriteLine($"Kestrel will bind HTTPS on localhost:5001 using certificate {certThumbprint}");
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
builder.Services.AddSingleton<ILogger>(sp => sp.GetRequiredService<ILoggerFactory>().CreateLogger("HotelDroid.Api"));
builder.Services.AddSingleton<IKeyValueStore>(sp =>
{
    // Read dataRoot lazily so tests can override it via configuration
    var config = sp.GetRequiredService<IConfiguration>();
    var dataRoot = Environment.GetEnvironmentVariable("HOTELDRUID_DATAROOT") 
        ?? config["DataRoot"]
        ?? Path.Combine(Path.GetTempPath(), "hoteldruid");
    var logger = sp.GetRequiredService<ILogger<FileKeyValueStore>>();
    return new FileKeyValueStore(dataRoot, logger);
});

// Register a simple file-backed store for development (legacy support)
builder.Services.AddSingleton<FileKvStore>();

// Register export/import services
builder.Services.AddScoped<ICanonicalMapper>(sp =>
    new CanonicalMapper(sp.GetRequiredService<ILogger<CanonicalMapper>>())
);
builder.Services.AddScoped<IPackageBuilder, PackageBuilder>();
builder.Services.AddScoped<IZipBuilder, ZipBuilder>();
builder.Services.AddScoped<IApiDistributor, ApiDistributor>();
builder.Services.AddScoped<IExportService, ExportService>();
builder.Services.AddScoped<IImportService, ImportService>();

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

// Allow static serving of .dat (ICU) and compressed assets that Blazor emits.
var staticContentTypeProvider = new FileExtensionContentTypeProvider();
staticContentTypeProvider.Mappings[".dat"] = "application/octet-stream";
staticContentTypeProvider.Mappings[".br"] = "application/octet-stream";

// Serve static files (useful when Blazor client is copied to api/wwwroot)
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
app.MapGet("/", () => Results.Text("HotelDroid API running", "text/plain"));
app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }));

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

// --- Mock API endpoints for early Blazor development ---

app.MapGet("/api/status", () => Results.Ok(new { ActiveYear = "2026", User = "admin", Version = "HotelDruid 3.0.7" }));

app.MapGet("/api/clients", async (FileKvStore store) => Results.Ok(await store.GetClientsAsync()));
app.MapGet("/api/clients/{id:int}", async (int id, FileKvStore store) =>
{
    var c = await store.GetClientAsync(id);
    return c is null ? Results.NotFound() : Results.Ok(c);
});
app.MapPost("/api/clients", async (ClientDto client, FileKvStore store) =>
{
    var added = await store.AddClientAsync(client);
    return Results.Created($"/api/clients/{added.Id}", added);
});

app.MapGet("/api/bookings", async (FileKvStore store) => Results.Ok(await store.GetBookingsAsync()));
app.MapGet("/api/bookings/{id:int}", async (int id, FileKvStore store) =>
{
    var b = await store.GetBookingAsync(id);
    return b is null ? Results.NotFound() : Results.Ok(b);
});
app.MapPost("/api/bookings", async (BookingDto booking, FileKvStore store) =>
{
    var added = await store.AddBookingAsync(booking);
    return Results.Created($"/api/bookings/{added.Id}", added);
});

// --- Room API endpoints ---

app.MapPost("/api/rooms", async (RoomDto request, IKeyValueStore store) =>
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

app.MapGet("/api/rooms", async (IKeyValueStore store, string? name) =>
{
    if (!string.IsNullOrEmpty(name))
    {
        // Get by name
        var room = await store.GetByNameAsync<RoomStorageModel>("rooms", name);
        if (room is null)
            return Results.NotFound();

        // Get the ID from the index
        var index = await store.GetIndexAsync("rooms");
        var id = index.TryGetValue(name, out var roomId) ? roomId : "";
        
        return Results.Ok(new RoomDto(
            id, room.Name ?? "", room.Capacity ?? 0, room.FloorNumber, room.HouseNumber,
            room.Priority, room.SecondaryPriority, room.HasBeds, room.NeighboringRooms, room.Comments));
    }

    // List all
    var rooms = await store.ListAsync<RoomStorageModel>("rooms");
    var index2 = await store.GetIndexAsync("rooms");
    
    var result = new List<RoomDto>();
    foreach (var room in rooms)
    {
        var roomName = room.Name ?? "";
        var roomId = index2.TryGetValue(roomName, out var id) ? id : "";
        result.Add(new RoomDto(
            roomId, room.Name ?? "", room.Capacity ?? 0, room.FloorNumber, room.HouseNumber,
            room.Priority, room.SecondaryPriority, room.HasBeds, room.NeighboringRooms, room.Comments));
    }
    
    return Results.Ok(result);
});

app.MapPut("/api/rooms/{id}", async (string id, RoomDto request, IKeyValueStore store) =>
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
    return Results.Ok(new RoomDto(
        id, request.Name, request.Capacity, request.FloorNumber, request.HouseNumber,
        request.Priority, request.SecondaryPriority, request.HasBeds, request.NeighboringRooms, request.Comments));
});

app.MapDelete("/api/rooms/{id}", async (string id, IKeyValueStore store) =>
{
    var exists = await store.ExistsAsync("rooms", id);
    if (!exists)
        return Results.NotFound();

    await store.DeleteAsync("rooms", id);
    return Results.NoContent();
});

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
.WithOpenApi();

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
.WithOpenApi();

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
.WithOpenApi();

app.MapGet("/api/export/list", async (IExportService exportService, int limit = 20, int offset = 0) =>
{
    var exports = await exportService.ListExportsAsync(limit, offset);
    return Results.Ok(new { exports = exports, total = exports.Count });
})
.WithName("ListExports")
.WithOpenApi();

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
.WithOpenApi();

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
.WithOpenApi();

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
.WithOpenApi();

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
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
