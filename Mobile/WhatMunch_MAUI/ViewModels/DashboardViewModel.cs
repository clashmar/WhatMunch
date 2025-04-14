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
        private readonly ISearchService _searchService;
        private readonly IShellService _shellService;
        private readonly ILogger<DashboardViewModel> _logger;

        public DashboardViewModel(
            ISearchService searchService,
            IShellService shellService,
            ILogger<DashboardViewModel> logger)
        {
            _searchService = searchService;
            _shellService = shellService;
            _logger = logger;
        }

        [RelayCommand]
        private async Task HandleSearch()
        {
            if (IsBusy) return;
            IsBusy = true;

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
                                { "NextPageToken", response.NextPageToken ?? string.Empty }
                            });
                }
                else
                {
                    await _shellService.DisplayError(AppResources.NoPlacesFound);
                }
            }
            catch (ConnectivityException)
            {
                await _shellService.DisplayError(AppResources.ErrorInternetConnection);
            }
            catch (HttpRequestException ex)
            {
                await _shellService.DisplayError(ex.Message ?? AppResources.ErrorUnexpected);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while executing search");
                await _shellService.DisplayError(AppResources.ErrorUnexpected);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public override void ResetViewModel()
        {
            // TODO: Implement ResetViewModel in Dashboard
            throw new NotImplementedException();
        }
    }
}
