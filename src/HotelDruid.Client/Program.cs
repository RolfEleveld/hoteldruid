using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using HotelDruid.Client;
using HotelDruid.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddLocalization();
builder.Services.AddScoped<IDeskUserService, DeskUserService>();
builder.Services.AddScoped(sp =>
{
    var http = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
    if (sp.GetRequiredService<IDeskUserService>() is DeskUserService desk)
        desk.Attach(http);
    return http;
});
builder.Services.AddScoped<IClientCultureService, ClientCultureService>();
builder.Services.AddScoped<IActiveYearService, ActiveYearService>();
// Register Asset API client service
builder.Services.AddScoped<HotelDruid.Client.Services.IAssetApiService, HotelDruid.Client.Services.AssetApiService>();
// Warehouses and Inventory services
builder.Services.AddScoped<HotelDruid.Client.Services.IRoomApiService, HotelDruid.Client.Services.RoomApiService>();
builder.Services.AddScoped<HotelDruid.Client.Services.IWarehouseApiService, HotelDruid.Client.Services.WarehouseApiService>();
builder.Services.AddScoped<HotelDruid.Client.Services.ITariffApiService, HotelDruid.Client.Services.TariffApiService>();
builder.Services.AddScoped<HotelDruid.Client.Services.ITariffAssignmentApiService, HotelDruid.Client.Services.TariffAssignmentApiService>();
builder.Services.AddScoped<HotelDruid.Client.Services.ITariffSeasonApiService, HotelDruid.Client.Services.TariffSeasonApiService>();
builder.Services.AddScoped<HotelDruid.Client.Services.IInventoryApiService, HotelDruid.Client.Services.InventoryApiService>();
// Layer 1: Configuration entity services
builder.Services.AddScoped<HotelDruid.Client.Services.ICashRegisterApiService, HotelDruid.Client.Services.CashRegisterApiService>();
builder.Services.AddScoped<HotelDruid.Client.Services.IUserGroupApiService, HotelDruid.Client.Services.UserGroupApiService>();
builder.Services.AddScoped<HotelDruid.Client.Services.IUserApiService, HotelDruid.Client.Services.UserApiService>();
builder.Services.AddScoped<HotelDruid.Client.Services.ISettingApiService, HotelDruid.Client.Services.SettingApiService>();

// Layer 2: Guest and User management services
builder.Services.AddScoped<HotelDruid.Client.Services.IClientApiService, HotelDruid.Client.Services.ClientApiService>();
builder.Services.AddScoped<HotelDruid.Client.Services.IUserPrivilegeApiService, HotelDruid.Client.Services.UserPrivilegeApiService>();
builder.Services.AddScoped<HotelDruid.Client.Services.IUserRelationApiService, HotelDruid.Client.Services.UserRelationApiService>();
builder.Services.AddScoped<HotelDruid.Client.Services.IGroupMembershipApiService, HotelDruid.Client.Services.GroupMembershipApiService>();
builder.Services.AddScoped<HotelDruid.Client.Services.IYearApiService, HotelDruid.Client.Services.YearApiService>();
builder.Services.AddScoped<HotelDruid.Client.Services.IExtraCostApiService, HotelDruid.Client.Services.ExtraCostApiService>();
builder.Services.AddScoped<HotelDruid.Client.Services.IAssignmentRuleApiService, HotelDruid.Client.Services.AssignmentRuleApiService>();

// Layer 4: Transactional entity services
builder.Services.AddScoped<HotelDruid.Client.Services.IBookingApiService, HotelDruid.Client.Services.BookingApiService>();
builder.Services.AddScoped<HotelDruid.Client.Services.IBookingFinanceApiService, HotelDruid.Client.Services.BookingFinanceApiService>();
builder.Services.AddScoped<HotelDruid.Client.Services.IBookingCostApiService, HotelDruid.Client.Services.BookingCostApiService>();
builder.Services.AddScoped<HotelDruid.Client.Services.IBookingGuestApiService, HotelDruid.Client.Services.BookingGuestApiService>();
builder.Services.AddScoped<HotelDruid.Client.Services.ICancelledBookingApiService, HotelDruid.Client.Services.CancelledBookingApiService>();
builder.Services.AddScoped<HotelDruid.Client.Services.IExpenseApiService, HotelDruid.Client.Services.ExpenseApiService>();
builder.Services.AddScoped<HotelDruid.Client.Services.IMoneyHistoryApiService, HotelDruid.Client.Services.MoneyHistoryApiService>();
builder.Services.AddScoped<HotelDruid.Client.Services.IPosSaleApiService, HotelDruid.Client.Services.PosSaleApiService>();
builder.Services.AddScoped<HotelDruid.Client.Services.IMessageApiService, HotelDruid.Client.Services.MessageApiService>();
builder.Services.AddScoped<HotelDruid.Client.Services.IContractTemplateApiService, HotelDruid.Client.Services.ContractTemplateApiService>();

var host = builder.Build();
var cultureService = host.Services.GetRequiredService<IClientCultureService>();
await cultureService.InitializeAsync();
var deskUser = host.Services.GetRequiredService<IDeskUserService>();
await deskUser.InitializeAsync();
var activeYearService = host.Services.GetRequiredService<IActiveYearService>();
await activeYearService.InitializeAsync();

await host.RunAsync();

