using Microsoft.Extensions.Logging;
using WhatMunch_MAUI.Extensions;
using WhatMunch_MAUI.Models.Places;
using WhatMunch_MAUI.Resources.Localization;

namespace WhatMunch_MAUI.Services
{
    public interface ISearchService
    {
        Task<ObservableCollection<Place>> GetFilteredSearchResults();
    }

    public class SearchService : ISearchService
    {
        private readonly IShellService _shellService;
        private readonly ILogger<SearchService> _logger;
        private readonly IGooglePlacesService _googlePlacesService;
        private readonly IConnectivity _connectivity;

        public SearchService(
            IShellService shellService, 
            ILogger<SearchService> logger, 
            IGooglePlacesService googlePlacesService, 
            IConnectivity connectivity)
        {
            _shellService = shellService;
            _logger = logger;
            _googlePlacesService = googlePlacesService;
            _connectivity = connectivity;
        }

        public async Task<ObservableCollection<Place>> GetFilteredSearchResults()
        {
            if (_connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                _logger.LogWarning("No internet connection.");
                await _shellService.DisplayAlert(AppResources.Error, AppResources.ErrorInternetConnection, AppResources.Ok);
                return [];
            }

            try
            {
                var result = await _googlePlacesService.GetNearbySearchResults();

                if (result.IsSuccess && result.Data is not null)
                {
                    // Filter based on user prefs
                    return result.Data.Places.ToObservableCollection<Place>();
                }
                else
                {
                    await _shellService.DisplayAlert(
                        AppResources.Error,
                        result.ErrorMessage ?? AppResources.ErrorUnexpected,
                        AppResources.Ok);

                    return [];
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while executing search");
                throw;
            }
        }
    }
}
