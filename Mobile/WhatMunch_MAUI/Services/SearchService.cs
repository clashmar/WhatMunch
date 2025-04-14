using Microsoft.Extensions.Logging;
using WhatMunch_MAUI.Models.Dtos;
using WhatMunch_MAUI.Utility.Exceptions;
using WhatMunch_MAUI.Extensions;

namespace WhatMunch_MAUI.Services
{
    public interface ISearchService
    {
        Task<TextSearchResponseDto> GetSearchResponseAsync(string? pageToken = null);
    }

    public class SearchService : ISearchService
    {
        private readonly ILogger<SearchService> _logger;
        private readonly IGooglePlacesService _googlePlacesService;
        private readonly IConnectivity _connectivity;
        private readonly ISearchPreferencesService _searchPreferencesService;

        public SearchService(
            ILogger<SearchService> logger, 
            IGooglePlacesService googlePlacesService, 
            IConnectivity connectivity,
            ISearchPreferencesService searchPreferencesService)
        {
            _logger = logger;
            _googlePlacesService = googlePlacesService;
            _connectivity = connectivity;
            _searchPreferencesService = searchPreferencesService;
        }

        public async Task<TextSearchResponseDto> GetSearchResponseAsync(string? pageToken = null)
        {
            if (_connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                _logger.LogWarning("No internet connection.");
                throw new ConnectivityException();
            }

            try
            {
                var preferences = await _searchPreferencesService.GetPreferencesAsync();
                var result = await _googlePlacesService.GetNearbySearchResultsAsync(preferences, pageToken);

                if (result.IsSuccess && result.Data is not null)
                {
                    var dto = result.Data;
                    dto.Places = dto.Places
                        .AddDistances(dto.SearchLocation)
                        .FilterDistances();
                    return dto;
                }
                else
                {
                    _logger.LogError("Search service error: {result.ErrorMessage}", result.ErrorMessage);
                    throw new HttpRequestException(result.ErrorMessage);
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed while fetching search results.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while executing search");
                throw;
            }
        }
    }
}
