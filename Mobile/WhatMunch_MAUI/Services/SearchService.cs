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

    public class SearchService(
        ILogger<SearchService> logger,
        IGooglePlacesService googlePlacesService,
        IConnectivity connectivity,
        ISearchPreferencesService searchPreferencesService,
        IFavouritesService favouritesService) : ISearchService
    {
        public async Task<TextSearchResponseDto> GetSearchResponseAsync(string? pageToken = null)
        {
            if (connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                logger.LogWarning("No internet connection.");
                throw new ConnectivityException();
            }

            try
            {
                var preferencesTask = searchPreferencesService.GetPreferencesAsync();
                var favouritesTask = favouritesService.GetUserFavouritesAsync();

                await Task.WhenAll(preferencesTask, favouritesTask);

                var preferences = preferencesTask.Result;
                var favouritesResult = favouritesTask.Result;

                if (preferences is null)
                {
                    logger.LogError("Failed to retrieve search preferences.");
                    throw new InvalidOperationException("Search preferences are null.");
                }

                var result = await googlePlacesService.GetNearbySearchResultsAsync(preferences, pageToken);

                if (result.IsSuccess && result.Data is not null)
                {
                    var favourites = favouritesResult.Data ?? [];
                    return ProcessSearchResults(result.Data, favourites);
                }
                else
                {
                    logger.LogError("Search service error: {result.ErrorMessage}", result.ErrorMessage);
                    throw new HttpRequestException(result.ErrorMessage);
                }
            }
            catch (HttpRequestException ex)
            {
                logger.LogError(ex, "HTTP request failed while fetching search results.");
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error: {Message}", ex.Message);
                throw;
            }
        }

        protected static TextSearchResponseDto ProcessSearchResults(
            TextSearchResponseDto dto, 
            List<PlaceDto> favourites)
        {
            dto.Places = dto.Places
                .AddDistances(dto.SearchLocation)
                .FilterDistances()
                .CheckIsFavourite(favourites)
                .ToList();

            return dto;
        }
    }
}
