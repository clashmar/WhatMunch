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
        

        builder.Services.AddSingleton<IConnectivity>(Connectivity.Current);

        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddSingleton<IShellService, ShellService>();
		builder.Services.AddSingleton<IRegistrationService, RegistrationService>();
		builder.Services.AddSingleton<ILoginService, LoginService>();

        builder.Services.AddSingleton<AppShell>();

        builder.Services.AddSingleton<LoginViewModel>();
        builder.Services.AddSingleton<RegistrationViewModel>();
        builder.Services.AddSingleton<DashboardViewModel>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
