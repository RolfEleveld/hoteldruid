// Service registration extensions for Rooms Widget feature
using HotelDroid.Client.Services;

namespace HotelDroid.Client.Configuration
{
    /// <summary>
    /// Extension methods for registering Rooms Widget services
    /// </summary>
    public static class RoomsWidgetServiceCollectionExtensions
    {
        /// <summary>
        /// Add Rooms Widget services to the dependency injection container
        /// </summary>
        public static IServiceCollection AddRoomsWidgetServices(this IServiceCollection services)
        {
            // Register language service as singleton for application-wide access
            services.AddSingleton<ILanguageService, LanguageService>();

            // Register room API service as scoped
            services.AddScoped<IRoomApiService, RoomApiService>();

            // Register HttpClient for use in services
            // Note: Ensure HttpClient is configured in Program.cs like:
            // builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            
            return services;
        }
    }
}
