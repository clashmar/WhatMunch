using Microsoft.Extensions.Logging;
using System.Text;
using WhatMunch_MAUI.MockData;
using WhatMunch_MAUI.Models.Dtos;
using WhatMunch_MAUI.Resources.Localization;
using WhatMunch_MAUI.Secrets;
using WhatMunch_MAUI.Utility;
using WhatMunch_MAUI.Utility.Exceptions;

namespace WhatMunch_MAUI.Services
{
    public interface IGooglePlacesService
    {
        Task<Result<TextSearchResponseDto>> GetNearbySearchResultsAsync(SearchPreferencesModel preferences, string? pageToken = null);
    }

    public class GooglePlacesService : IGooglePlacesService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<GooglePlacesService> _logger;
        private readonly ILocationService _locationService;
        private readonly string _apiKey;

        public GooglePlacesService(
            IHttpClientFactory clientFactory,
            ILogger<GooglePlacesService> logger,
            ILocationService locationService)
        {
            _clientFactory = clientFactory;
            _logger = logger;
            _locationService = locationService;
            _apiKey = ApiKeys.GOOGLE_MAPS_API_KEY;
        }

        // Specify what the api returns
        private static readonly string FIELD_MASK = string.Join(",",
            "places.id",
            "places.displayName",
            "places.photos",
            "places.primaryType",
            "places.primaryTypeDisplayName",
            "places.rating",
            "places.userRatingCount",
            "places.types",
            "places.regularOpeningHours",
            "places.goodForChildren",
            "places.allowsDogs",
            "places.priceLevel",
            "places.websiteUri",
            "places.internationalPhoneNumber",
            "places.shortFormattedAddress",
            "places.location",
            "places.reviews",
            "nextPageToken"
        );

        public async Task<Result<TextSearchResponseDto>> GetNearbySearchResultsAsync(SearchPreferencesModel preferences, string? pageToken = null)
        {
            if (preferences is null) preferences = SearchPreferencesModel.Default;

            //Mock data for development
            //var mockDeserializedData = JsonSerializer.Deserialize<TextSearchResponseDto>(MockPlace.GetMockPlaceJson());
            //if (mockDeserializedData is TextSearchResponseDto mockResponseDto)
            //{
            //    mockResponseDto.SearchLocation = await _locationService.GetLastSearchLocation();
            //    return Result<TextSearchResponseDto>.Success(mockResponseDto);
            //}
            //else
            //{
            //    _logger.LogError("Failed to deserialize mock response");
            //    return Result<TextSearchResponseDto>.Failure("Failed to deserialize mock response");
            //}

            try
            {
                Task<string> jsonContent = CreateNearbySearchJsonAsync(preferences, pageToken);

                var client = _clientFactory.CreateClient("GooglePlaces");
                client.DefaultRequestHeaders.Add("X-Goog-Api-Key", _apiKey);
                client.DefaultRequestHeaders.Add("X-Goog-FieldMask", FIELD_MASK);

                StringContent stringContent = new(await jsonContent, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync("v1/places:searchText", stringContent);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var deserializedData = JsonSerializer.Deserialize<TextSearchResponseDto>(responseContent);

                if (deserializedData is TextSearchResponseDto searchResponseDto)
                {
                    searchResponseDto.SearchLocation = await _locationService.GetLastSearchLocation();
                    return Result<TextSearchResponseDto>.Success(searchResponseDto);
                }
                else
                {
                    _logger.LogError("Failed to deserialize nearby search response");
                    return Result<TextSearchResponseDto>.Failure(AppResources.ErrorUnexpected);
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed with status code {StatusCode}", ex.StatusCode);
                return Result<TextSearchResponseDto>.Failure(AppResources.ErrorUnexpected);
            }
            catch (LocationException ex)
            {
                _logger.LogError(ex, "Location services are unavailable");
                return Result<TextSearchResponseDto>.Failure(AppResources.ErrorLocationServices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred in GooglePlacesService");
                return Result<TextSearchResponseDto>.Failure(AppResources.ErrorUnexpected);
            }
        }
        private async Task<string> CreateNearbySearchJsonAsync(SearchPreferencesModel preferences, string? pageToken = null)
        {
            if (preferences is null) preferences = SearchPreferencesModel.Default;

            try
            {
                var location = await _locationService.GetLocationWithTimeoutAsync();

                var textQueryBuilder = new StringBuilder();

                if (preferences.IsVegetarian) textQueryBuilder.Append("Vegetarian ");
                if (preferences.IsVegan) textQueryBuilder.Append("Vegan ");
                if (preferences.IsChildFriendly) textQueryBuilder.Append("Child Friendly ");
                textQueryBuilder.Append("Place to eat");
                if (preferences.IsDogFriendly) textQueryBuilder.Append(" that Allows Dogs");

                string textQuery = textQueryBuilder.ToString();

                var request = new TextSearchRequestDto
                {
                    TextQuery = textQuery,
                    LocationBias = new LocationBias
                    {
                        Circle = new Circle
                        {
                            Center = new Center
                            {
                                Latitude = location.Latitude,
                                Longitude = location.Longitude,
                            },
                            Radius = preferences.SearchRadius
                        }
                    },
                    MinRating = preferences.MinRating,
                    OpenNow = true,
                    PageToken = pageToken,
                    PriceLevels = preferences.GetPriceLevels(),
                    RankPreference = preferences.RankPreference,
                };
                return JsonSerializer.Serialize(request);
            }
            catch (LocationException ex)
            {
                _logger.LogError(ex, "An error occurred while getting the geolocation of this device");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while creating search object");
                throw;
            }
        }
    }
}
