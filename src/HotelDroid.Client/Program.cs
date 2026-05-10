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
// Reference data services
builder.Services.AddScoped<HotelDroid.Client.Services.INationApiService, HotelDroid.Client.Services.NationApiService>();
builder.Services.AddScoped<HotelDroid.Client.Services.IRegionApiService, HotelDroid.Client.Services.RegionApiService>();
builder.Services.AddScoped<HotelDroid.Client.Services.ICityApiService, HotelDroid.Client.Services.CityApiService>();
builder.Services.AddScoped<HotelDroid.Client.Services.IIdentityDocumentTypeApiService, HotelDroid.Client.Services.IdentityDocumentTypeApiService>();
builder.Services.AddScoped<HotelDroid.Client.Services.IFamilyRelationshipApiService, HotelDroid.Client.Services.FamilyRelationshipApiService>();
// Layer 1: Configuration entity services
builder.Services.AddScoped<HotelDroid.Client.Services.ICashRegisterApiService, HotelDroid.Client.Services.CashRegisterApiService>();
builder.Services.AddScoped<HotelDroid.Client.Services.IUserGroupApiService, HotelDroid.Client.Services.UserGroupApiService>();
builder.Services.AddScoped<HotelDroid.Client.Services.IUserApiService, HotelDroid.Client.Services.UserApiService>();
builder.Services.AddScoped<HotelDroid.Client.Services.ISettingApiService, HotelDroid.Client.Services.SettingApiService>();

// Layer 2: Guest and User management services
builder.Services.AddScoped<HotelDroid.Client.Services.IClientApiService, HotelDroid.Client.Services.ClientApiService>();
builder.Services.AddScoped<HotelDroid.Client.Services.IClientDataApiService, HotelDroid.Client.Services.ClientDataApiService>();
builder.Services.AddScoped<HotelDroid.Client.Services.IUserPrivilegeApiService, HotelDroid.Client.Services.UserPrivilegeApiService>();
builder.Services.AddScoped<HotelDroid.Client.Services.IUserRelationApiService, HotelDroid.Client.Services.UserRelationApiService>();
builder.Services.AddScoped<HotelDroid.Client.Services.IGroupMembershipApiService, HotelDroid.Client.Services.GroupMembershipApiService>();
builder.Services.AddScoped<HotelDroid.Client.Services.IYearApiService, HotelDroid.Client.Services.YearApiService>();
builder.Services.AddScoped<HotelDroid.Client.Services.IPeriodApiService, HotelDroid.Client.Services.PeriodApiService>();
builder.Services.AddScoped<HotelDroid.Client.Services.ITariffApiService, HotelDroid.Client.Services.TariffApiService>();
builder.Services.AddScoped<HotelDroid.Client.Services.IAssignmentRuleApiService, HotelDroid.Client.Services.AssignmentRuleApiService>();

// Layer 4: Transactional entity services
builder.Services.AddScoped<HotelDroid.Client.Services.IBookingApiService, HotelDroid.Client.Services.BookingApiService>();
builder.Services.AddScoped<HotelDroid.Client.Services.IBookingCostApiService, HotelDroid.Client.Services.BookingCostApiService>();
builder.Services.AddScoped<HotelDroid.Client.Services.IBookingGuestApiService, HotelDroid.Client.Services.BookingGuestApiService>();
builder.Services.AddScoped<HotelDroid.Client.Services.ICancelledBookingApiService, HotelDroid.Client.Services.CancelledBookingApiService>();
builder.Services.AddScoped<HotelDroid.Client.Services.IExpenseApiService, HotelDroid.Client.Services.ExpenseApiService>();
builder.Services.AddScoped<HotelDroid.Client.Services.IMoneyHistoryApiService, HotelDroid.Client.Services.MoneyHistoryApiService>();

await builder.Build().RunAsync();
