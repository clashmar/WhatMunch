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
		client.BaseAddress = new Uri("hello"));

        builder.Services.AddSingleton<IAuthService, AuthService>();
		builder.Services.AddSingleton<IRegistrationService, RegistrationService>();

        builder.Services.AddSingleton<AppShell>();

        builder.Services.AddSingleton<LoginViewModel>();
        builder.Services.AddSingleton<RegistrationViewModel>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
