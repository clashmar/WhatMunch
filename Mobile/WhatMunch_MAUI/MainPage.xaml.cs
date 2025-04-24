using Microsoft.Extensions.Logging;
using WhatMunch_MAUI.Services;
using WhatMunch_MAUI.Views;

namespace WhatMunch_MAUI
{
    public partial class MainPage : ContentPage
    {
        private readonly ITokenService _tokenService;
        private readonly IShellService _shellService;
        private readonly ILogger<MainPage> _logger;

        public MainPage(
            ITokenService tokenService, 
            IShellService shellService, 
            ILogger<MainPage> logger)
        {
            InitializeComponent();
            _tokenService = tokenService;
            _shellService = shellService;
            _logger = logger;
            Shell.SetNavBarIsVisible(this, false);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await CheckAuthentication();
        }

        private async Task CheckAuthentication()
        {
            try
            {
                bool isAuthenticated = await _tokenService.IsUserAuthenticated();

                if (!isAuthenticated)
                {
                    await _shellService.GoToAsync($"{nameof(LoginPage)}");
                }
                else
                {
                    await _shellService.GoToAsync($"//MainTabs/DashboardPage");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during authentication check.");
                throw;
            }
        }
    }
}
