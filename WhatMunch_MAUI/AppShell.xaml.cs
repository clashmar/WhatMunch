using WhatMunch_MAUI.Pages;
using WhatMunch_MAUI.Services;

namespace WhatMunch_MAUI
{
    public partial class AppShell : Shell
    {
        private readonly IAuthService _authService;

        public AppShell(IAuthService authService)
        {
            InitializeComponent();
            _authService = authService;
            CheckAuthentication();
        }

        private async void CheckAuthentication()
        {
            bool isAuthenticated = await _authService.IsUserAuthenticated();

            if(!isAuthenticated)
            {
                await GoToAsync($"{nameof(LoginPage)}");
            }
        }
    }
}
