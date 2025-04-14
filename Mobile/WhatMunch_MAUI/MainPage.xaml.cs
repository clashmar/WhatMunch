using WhatMunch_MAUI.Services;
using WhatMunch_MAUI.Views;

namespace WhatMunch_MAUI
{
    public partial class MainPage : ContentPage
    {
        private readonly ITokenService _tokenService;
        private readonly IShellService _shellService;

        public MainPage(ITokenService tokenService, IShellService shellService)
        {
            InitializeComponent();
            _tokenService = tokenService;
            _shellService = shellService;
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
                    await _shellService.GoToAsync("//MainTabs/DashboardPage");
                }
                else
                {
                    await _shellService.GoToAsync($"//MainTabs/{nameof(DashboardPage)}");
                }
            }
            catch (Exception)
            {
                // TODO: Handle AuthCheck errors in MainPage.xaml.cs
                throw;
            }
        }
    }
}
