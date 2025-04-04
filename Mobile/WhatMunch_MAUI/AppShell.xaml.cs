using WhatMunch_MAUI.Views;
using WhatMunch_MAUI.Services;

namespace WhatMunch_MAUI
{
    public partial class AppShell : Shell
    {
        private readonly ITokenService _tokenService;

        public AppShell(ITokenService tokenService)
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(RegistrationPage), typeof(RegistrationPage));
            Routing.RegisterRoute(nameof(DashboardPage), typeof(DashboardPage));
            Routing.RegisterRoute(nameof(SearchResultsPage), typeof(SearchResultsPage));
            Routing.RegisterRoute(nameof(SearchPreferencesPage), typeof(SearchPreferencesPage));
            _tokenService = tokenService; 
            CheckAuthentication();
        }

        private async void CheckAuthentication()
        {
            try
            {
                bool isAuthenticated = await _tokenService.IsUserAuthenticated();

                if (!isAuthenticated)
                {
                    await GoToAsync($"{nameof(DashboardPage)}");
                    //await GoToAsync($"{nameof(LoginPage)}");
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
