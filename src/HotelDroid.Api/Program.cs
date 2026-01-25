using System.Security.Cryptography.X509Certificates;
using HotelDroid.Api.Services;
using HotelDroid.Api.Models;
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
// Register a simple file-backed store for development
builder.Services.AddSingleton<FileKvStore>();

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


app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
