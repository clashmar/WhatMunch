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

                if (response.Places.Count > 0)
                {
                    var places = response.Places
                        .ToObservableCollection();

                    await _shellService.GoToAsync($"{nameof(SearchResultsPage)}",
                        new Dictionary<string, object>
                            {
                                { "Places", places },
                                { "NextPageToken", response.NextPageToken ?? string.Empty },
                                { "ShouldReset", false }
                            });
                }
                else
                {
                    await DisplayErrorAlertAsync(AppResources.NoPlacesFound);
                }
            }
            catch (ConnectivityException)
            {
                await DisplayErrorAlertAsync(AppResources.ErrorInternetConnection);
            }
            catch (HttpRequestException ex)
            {
                await DisplayErrorAlertAsync(ex.Message ?? AppResources.ErrorUnexpected);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while executing search");
                await DisplayErrorAlertAsync(AppResources.ErrorUnexpected);
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

        private async Task DisplayErrorAlertAsync(string message)
        {
            await _shellService.DisplayAlert(AppResources.Error, message, AppResources.Ok);
        }

        public override void ResetViewModel()
        {
            // TODO: Implement ResetViewModel in Dashboard
            throw new NotImplementedException();
        }
    }
}
