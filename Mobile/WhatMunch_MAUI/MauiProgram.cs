using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using WhatMunch_MAUI.Services;
namespace WhatMunch_MAUI;
public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
            .UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				fonts.AddFont("fa-regular-400.ttf", "FaRegular");
				fonts.AddFont("fa-solid-900.ttf", "FaSolid");
            });

        builder.Services.AddHttpClient("WhatMunch", client =>
        {
            client.Timeout = TimeSpan.FromSeconds(10);
            client.BaseAddress = new Uri("http://10.0.2.2:8000/api/");
        });
        builder.Services.AddHttpClient("GooglePlaces", client =>
        {
            client.Timeout = TimeSpan.FromSeconds(10);
            client.BaseAddress = new Uri("https://places.googleapis.com/");
        });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services
            .AddLogging()
            .AddSingleton<IConnectivity>(Connectivity.Current)
            .AddSingleton<IGeolocation>(Geolocation.Default);

        builder.Services
            .AddSingleton<ITokenService, TokenService>()
            .AddSingleton<IShellService, ShellService>()
            .AddSingleton<ISearchService, SearchService>()
            .AddSingleton<IPermissionsService, PermissionsService>()
            .AddSingleton<ILocationService, LocationService>()
            .AddSingleton<ISearchPreferencesService, SearchPreferencesService>()
            .AddSingleton<IRegistrationService, RegistrationService>()
            .AddSingleton<ILoginService, LoginService>()
            .AddSingleton<IGooglePlacesService, GooglePlacesService>();

        builder.Services
            .AddSingleton<LoginViewModel>()
            .AddSingleton<RegistrationViewModel>()
            .AddSingleton<DashboardViewModel>()
            .AddSingleton<SearchResultsViewModel>()
            .AddSingleton<SearchPreferencesViewModel>()
            .AddTransient<PlaceDetailsViewModel>();

        builder.Services.AddSingleton<AppShell>();

        return builder.Build();
	}
}
