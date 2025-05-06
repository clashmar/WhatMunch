using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Platform;
using WhatMunch_MAUI.Data.SQLite;
using WhatMunch_MAUI.Services;
using WhatMunch_MAUI.Utility;
namespace WhatMunch_MAUI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.UseSentry(options =>
			{
				options.Dsn = "https://68445c1a58c3c42af10108e1f3945008@o4509246843453440.ingest.de.sentry.io/4509246859903056";
            })
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				fonts.AddFont("fa-solid-900.ttf", "FaSolid");
			});



		builder.Services.AddHttpClient("WhatMunch", client =>
		{
			client.Timeout = TimeSpan.FromSeconds(10);
			client.BaseAddress = new Uri("http://10.0.2.2:8080/api/");
		});
		builder.Services.AddHttpClient("GooglePlaces", client =>
		{
			client.Timeout = TimeSpan.FromSeconds(10);
			client.BaseAddress = new Uri("https://places.googleapis.com/");
		});

#if DEBUG
		builder.Logging.AddDebug();
#endif
		builder.Services.AddSingleton<AppShell>();

		builder.Services
			.AddLogging()
			.AddSingleton<IConnectivity>(Connectivity.Current)
			.AddSingleton<IGeolocation>(Geolocation.Default)
			.AddSingleton<ILauncher>(Launcher.Default)
			.AddSingleton<ISecureStorage>(SecureStorage.Default)
			.AddSingleton<IWebAuthenticator>(WebAuthenticator.Default)
			.AddSingleton<ILocalDatabase, LocalDatabase>()
			.AddSingleton<IMainThread, MainThreadWrapper>();

		builder.Services
			.AddSingleton<ITokenService, TokenService>()
			.AddSingleton<IDjangoApiService, DjangoApiService>()
			.AddSingleton<IShellService, ShellService>()
			.AddSingleton<ISearchService, SearchService>()
			.AddSingleton<IPermissionsService, PermissionsService>()
			.AddSingleton<ILocationService, LocationService>()
			.AddSingleton<ISearchPreferencesService, SearchPreferencesService>()
			.AddSingleton<IAccountService, AccountService>()
			.AddSingleton<IGooglePlacesService, GooglePlacesService>()
			.AddSingleton<IFavouritesService, FavouritesService>()
			.AddSingleton<IToastService, ToastService>();

		builder.Services
			.AddSingleton<LoginViewModel>()
			.AddSingleton<RegistrationViewModel>()
			.AddSingleton<DashboardViewModel>()
			.AddSingleton<SearchResultsViewModel>()
			.AddSingleton<SearchPreferencesViewModel>()
			.AddTransient<PlaceDetailsViewModel>()
			.AddTransient<SavedPlacesViewModel>();

		Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("Placeholder", (h, v) =>
		{
#if ANDROID
			h.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Colors.Transparent.ToPlatform());
#endif

#if IOS
			h.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#endif
		});

		return builder.Build();
	}
}
