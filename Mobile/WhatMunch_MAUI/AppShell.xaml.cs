using WhatMunch_MAUI.Views;
using WhatMunch_MAUI.Services;
using WhatMunch_MAUI.Pages;

namespace WhatMunch_MAUI
{
    public partial class AppShell : Shell
    {
        private readonly IAuthService _authService;

        public AppShell(IAuthService authService)
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(RegistrationPage), typeof(RegistrationPage));
            Routing.RegisterRoute(nameof(DashboardPage), typeof(DashboardPage));
            _authService = authService;
            CheckAuthentication();
        }

        private async void CheckAuthentication()
        {
            try
            {
                bool isAuthenticated = await _authService.IsUserAuthenticated();

                if (!isAuthenticated)
                {
                    await GoToAsync($"{nameof(LoginPage)}");
                }
                else
                {
                    await GoToAsync($"{nameof(DashboardPage)}");
                }
            }
            catch (Exception)
            {

                throw;
            }
            
        }
    }
}
