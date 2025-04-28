using Microsoft.Extensions.Logging;
using WhatMunch_MAUI.Extensions;
using WhatMunch_MAUI.Resources.Localization;
using WhatMunch_MAUI.Services;
using WhatMunch_MAUI.Utility.Exceptions;
using WhatMunch_MAUI.Views;

namespace WhatMunch_MAUI.ViewModels
{
    public partial class DashboardViewModel(
        ISearchService searchService,
        IShellService shellService,
        ILogger<DashboardViewModel> logger) : BaseViewModel
    {
        [RelayCommand]
        private async Task HandleSearch()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var response = await searchService.GetSearchResponseAsync();

                if (response.Places.Count > 0)
                {
                    var places = response.Places
                        .ToObservableCollection();

                    await shellService.GoToAsync($"{nameof(SearchResultsPage)}",
                        new Dictionary<string, object>
                            {
                                { "Places", places },
                                { "NextPageToken", response.NextPageToken ?? string.Empty }
                            });
                }
                else
                {
                    await shellService.DisplayError(AppResources.NoPlacesFound);
                }
            }
            catch (ConnectivityException)
            {
                await shellService.DisplayError(AppResources.ErrorInternetConnection);
            }
            catch (HttpRequestException ex)
            {
                await shellService.DisplayError(ex.Message ?? AppResources.ErrorUnexpected);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error: {Message}", ex.Message);
                await shellService.DisplayError(AppResources.ErrorUnexpected);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public override void ResetViewModel()
        {
            throw new NotImplementedException();
        }
    }
}
