using Microsoft.Extensions.Logging;
using WhatMunch_MAUI.Dtos;
using WhatMunch_MAUI.Resources.Localization;
using WhatMunch_MAUI.Services;
using WhatMunch_MAUI.Utility;
using WhatMunch_MAUI.Views;

namespace WhatMunch_MAUI.ViewModels
{
    public partial class DashboardViewModel : BaseViewModel
    {
        private readonly ITokenService _tokenService;
        private readonly IGooglePlacesService _googlePlacesService;
        private readonly IShellService _shellService;
        private readonly IConnectivity _connectivity;
        private readonly ILogger<DashboardViewModel> _logger;

        public DashboardViewModel(
            ITokenService tokenService,
            IGooglePlacesService googlePlacesService,
            IShellService shellService,
            IConnectivity connectivity,
            ILogger<DashboardViewModel> logger)
        {
            _tokenService = tokenService;
            _googlePlacesService = googlePlacesService;
            _shellService = shellService;
            _connectivity = connectivity;
            _logger = logger;
        }

        [RelayCommand]
        private async Task HandleSearch()
        {
            if (_connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                _logger.LogWarning("No internet connection.");
                await _shellService.DisplayAlert(AppResources.Error, AppResources.ErrorInternetConnection, AppResources.Ok);
                return; 
            }

            try
            {
                var result = await _googlePlacesService.GetNearbySearchResults();
                //Result<NearbySearchResponseDto> result = Result<NearbySearchResponseDto>.Success(MockData.MockDtos.MockSearchResponse);

                if (result.IsSuccess && result.Value is not null)
                {
                    await _shellService.GoToAsync($"{nameof(SearchResultsPage)}",
                        new Dictionary<string, object>
                            {
                                { "Places", result.Value.places }
                            });
                }
                else
                {
                    await _shellService.DisplayAlert(
                        AppResources.Error, 
                        result.ErrorMessage ?? AppResources.ErrorUnexpected, 
                        AppResources.Ok);
                }
            }
            catch (Exception ex)
            {
                var exception = ex;
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
