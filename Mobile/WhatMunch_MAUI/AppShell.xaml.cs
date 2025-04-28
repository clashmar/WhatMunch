using Microsoft.Extensions.Logging;
using WhatMunch_MAUI.Resources.Localization;
using WhatMunch_MAUI.Services;
using WhatMunch_MAUI.Views;

namespace WhatMunch_MAUI
{
    public partial class AppShell : Shell
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<AppShell> _logger;
        public AppShell(IAccountService accountService, ILogger<AppShell> logger)
        {
            InitializeComponent();
            _accountService = accountService;
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(RegistrationPage), typeof(RegistrationPage));
            Routing.RegisterRoute(nameof(SearchResultsPage), typeof(SearchResultsPage));
            Routing.RegisterRoute(nameof(PlaceDetailsPage), typeof(PlaceDetailsPage));
            _logger = logger;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            bool isConfirmed = await DisplayAlert(AppResources.Logout, AppResources.AreYouSure, AppResources.Yes, AppResources.No);
            if (!isConfirmed) return;

            try
            {
                await _accountService.LogoutAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while logging out.");
                await DisplayAlert(AppResources.Error, AppResources.ErrorUnexpected, AppResources.Ok);
                throw;
            }
        }
    }
}
