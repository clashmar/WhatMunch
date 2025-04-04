using Microsoft.Extensions.Logging;
using WhatMunch_MAUI.Extensions;
using WhatMunch_MAUI.Resources.Localization;
using WhatMunch_MAUI.Services;
using WhatMunch_MAUI.Utility.Exceptions;
using WhatMunch_MAUI.Views;

namespace WhatMunch_MAUI.ViewModels
{
    public partial class DashboardViewModel : BaseViewModel
    {
        private readonly ITokenService _tokenService;
        private readonly ISearchService _searchService;
        private readonly IShellService _shellService;
        private readonly ILogger<DashboardViewModel> _logger;

        public DashboardViewModel(
            ITokenService tokenService,
            ISearchService searchService,
            IShellService shellService,
            ILogger<DashboardViewModel> logger)
        {
            _tokenService = tokenService;
            _searchService = searchService;
            _shellService = shellService;
            _logger = logger;
        }

        [RelayCommand]
        private async Task HandleSearch()
        {
            try
            {
                var response = await _searchService.GetSearchResponseAsync();

                // Loading indicator

                if (response.Places.Count > 0)
                {
                    var places = response.Places.ToObservableCollection();

                    await _shellService.GoToAsync($"{nameof(SearchResultsPage)}",
                        new Dictionary<string, object>
                            {
                                { "Places", places },
                                { "PageToken", response.NextPageToken ?? string.Empty }
                            });
                }
                else
                {
                    await _shellService.DisplayAlert(
                        AppResources.Error,
                        AppResources.NoPlacesFound,
                        AppResources.Ok);
                }
            }
            catch (ConnectivityException)
            {
                await _shellService.DisplayAlert(AppResources.Error, AppResources.ErrorInternetConnection, AppResources.Ok);
            }
            catch (HttpRequestException ex)
            {
                await _shellService.DisplayAlert(
                        AppResources.Error,
                        ex.Message ?? AppResources.ErrorUnexpected,
                        AppResources.Ok);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while executing search");
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

        [RelayCommand]
        private async Task HandleSetPreferences()
        {
            try
            {
                _tokenService.Logout();
                await Shell.Current.GoToAsync($"{nameof(SearchPreferencesPage)}");
            }
            catch (Exception)
            {
                throw;
            }
        }

        [RelayCommand]
        private async Task HandleTestToken()
        {
            try
            {
                string? token = await _tokenService.GetAccessTokenAsync();
                Debug.WriteLine(token);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
