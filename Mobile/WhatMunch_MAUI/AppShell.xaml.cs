using WhatMunch_MAUI.Resources.Localization;
using WhatMunch_MAUI.Services;
using WhatMunch_MAUI.Views;

namespace WhatMunch_MAUI
{
    public partial class AppShell : Shell
    {
        private readonly ITokenService _tokenService;
        public AppShell(ITokenService tokenService)
        {
            InitializeComponent(); 
            _tokenService = tokenService;
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(RegistrationPage), typeof(RegistrationPage));
            Routing.RegisterRoute(nameof(SearchResultsPage), typeof(SearchResultsPage));
            Routing.RegisterRoute(nameof(PlaceDetailsPage), typeof(PlaceDetailsPage));
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
                await _tokenService.LogoutAsync();
                await GoToAsync($"{nameof(LoginPage)}");
            }
            catch (Exception ex)
            {
                await DisplayAlert(AppResources.Error, ex.Message, AppResources.Ok);
                throw;
            }
        }
    }
}
