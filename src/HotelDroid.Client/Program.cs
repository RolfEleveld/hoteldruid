using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using HotelDroid.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
// Register Asset API client service
builder.Services.AddScoped<HotelDroid.Client.Services.IAssetApiService, HotelDroid.Client.Services.AssetApiService>();
// Warehouses and Inventory services
builder.Services.AddScoped<HotelDroid.Client.Services.IWarehouseApiService, HotelDroid.Client.Services.WarehouseApiService>();
builder.Services.AddScoped<HotelDroid.Client.Services.IInventoryApiService, HotelDroid.Client.Services.InventoryApiService>();

await builder.Build().RunAsync();
