using System.Globalization;
using HotelDruid.Client.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace HotelDruid.Client.Tests;

internal static class TestLocalizationSupport
{
    public static IServiceCollection AddClientLocalizationTestSupport(this IServiceCollection services)
    {
        services.AddSingleton(typeof(IStringLocalizer<>), typeof(TestStringLocalizer<>));
        services.AddScoped<IClientCultureService, TestClientCultureService>();
        services.AddScoped<IActiveYearService, TestActiveYearService>();
        return services;
    }
}

internal sealed class TestStringLocalizer<T> : IStringLocalizer<T>
{
    public LocalizedString this[string name] => new(name, name, resourceNotFound: false);

    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            if (arguments is { Length: > 0 })
            {
                var first = arguments[0]?.ToString();
                if (!string.IsNullOrWhiteSpace(first))
                {
                    return new LocalizedString(name, first, resourceNotFound: false);
                }
            }

            return new LocalizedString(name, name, resourceNotFound: false);
        }
    }

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        return Enumerable.Empty<LocalizedString>();
    }

    public IStringLocalizer WithCulture(CultureInfo culture)
    {
        return this;
    }
}

internal sealed class TestClientCultureService : IClientCultureService
{
    private static readonly IReadOnlyList<CultureInfo> Cultures = new[]
    {
        CultureInfo.GetCultureInfo("en"),
        CultureInfo.GetCultureInfo("es"),
        CultureInfo.GetCultureInfo("it")
    };

    public CultureInfo CurrentCulture { get; private set; } = CultureInfo.GetCultureInfo("en");

    public IReadOnlyList<CultureInfo> SupportedCultures => Cultures;

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public Task SetCultureAsync(string cultureName, bool persistPreference = true)
    {
        CurrentCulture = Cultures.FirstOrDefault(c => c.Name.Equals(cultureName, StringComparison.OrdinalIgnoreCase))
            ?? CultureInfo.GetCultureInfo("en");
        return Task.CompletedTask;
    }
}

internal sealed class TestActiveYearService : IActiveYearService
{
    public int CurrentYear { get; private set; } = DateTime.Now.Year;

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public Task SetActiveYearAsync(int year, bool persistPreference = true)
    {
        CurrentYear = year;
        return Task.CompletedTask;
    }
}
