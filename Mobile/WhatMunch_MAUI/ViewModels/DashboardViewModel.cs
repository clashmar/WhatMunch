using WhatMunch_MAUI.Resources.Localization;
using WhatMunch_MAUI.Services;
using WhatMunch_MAUI.Views;

namespace WhatMunch_MAUI.ViewModels
{
    public partial class DashboardViewModel : BaseViewModel
    {
        private readonly ITokenService _tokenService;
        private readonly IGooglePlacesService _googlePlacesService;
        private readonly IShellService _shellService;

        public DashboardViewModel(
            ITokenService tokenService,
            IGooglePlacesService googlePlacesService,
            IShellService shellService)
        {
            _tokenService = tokenService;
            _googlePlacesService = googlePlacesService;
            _shellService = shellService;
        }

        [RelayCommand]
        private async Task HandleSearch()
        {
            try
            {
                var result = await _googlePlacesService.GetNearbySearchResults();

                if(result.IsSuccess)
                {
                    await Shell.Current.GoToAsync($"{nameof(LoginPage)}");
                }
                else
                {
                    await _shellService.DisplayAlert(AppResources.Error, result.ErrorMessage ?? "", AppResources.Ok);
                }
            }
            catch (Exception)
            {
                await _shellService.DisplayAlert(AppResources.Error, AppResources.ErrorUnexpected, AppResources.Ok);
            }
        }

        [RelayCommand]
        private async Task HandleLogout()
        {
            try
            {
                _tokenService.Logout();
                await Shell.Current.GoToAsync($"{nameof(LoginPage)}");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
