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
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
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
            .AddSingleton<IGeolocation>(Geolocation.Default)
            .AddSingleton<ITokenService, TokenService>()
            .AddSingleton<IShellService, ShellService>()
            .AddSingleton<IPermissionsService, PermissionsService>()
            .AddSingleton<ILocationService, LocationService>()
            .AddSingleton<IRegistrationService, RegistrationService>()
            .AddSingleton<ILoginService, LoginService>()
            .AddSingleton<IGooglePlacesService, GooglePlacesService>()
            .AddSingleton<AppShell>()
            .AddSingleton<LoginViewModel>()
            .AddSingleton<RegistrationViewModel>()
            .AddSingleton<DashboardViewModel>()
            .AddSingleton<SearchResultsViewModel>();

		return builder.Build();
	}
}
