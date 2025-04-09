using Microsoft.Extensions.Logging;
using System.Text;
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
        private const string FIELD_MASK = "" +
            "places.id," +
            "places.displayName," +
            "places.photos," +
            "places.primaryTypeDisplayName," +
            "places.rating," +
            "places.userRatingCount," +
            "places.types," +
            "places.regularOpeningHours," +
            "places.goodForChildren," +
            "places.allowsDogs," +
            "places.priceLevel," +
            "places.websiteUri," +
            "places.internationalPhoneNumber," +
            "places.shortFormattedAddress," +
            "places.location," +
            "nextPageToken";

        public async Task<Result<TextSearchResponseDto>> GetNearbySearchResultsAsync(SearchPreferencesModel preferences, string? pageToken = null)
        {
            //Mock data for development
            var mockDeserializedData = JsonSerializer.Deserialize<TextSearchResponseDto>(MockJsonContent());
            if (mockDeserializedData is TextSearchResponseDto mockResponseDto)
            {
                mockResponseDto.SearchLocation = await _locationService.GetLastSearchLocation();
                return Result<TextSearchResponseDto>.Success(mockResponseDto);
            }
            else
            {
                _logger.LogError("Failed to deserialize mock response");
                return Result<TextSearchResponseDto>.Failure("Failed to deserialize mock response");
            }

            try
            {
                Task<string> jsonContent = CreateNearbySearchJsonAsync(preferences, pageToken);

                var client = _clientFactory.CreateClient("GooglePlaces");
                client.DefaultRequestHeaders.Add("X-Goog-Api-Key", _apiKey);
                client.DefaultRequestHeaders.Add("X-Goog-FieldMask", FIELD_MASK);

                StringContent stringContent = new(await jsonContent, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync("v1/places:searchText", stringContent);

                if (response.IsSuccessStatusCode)
                {
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
                else
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Nearby search returned {response.StatusCode}", response.StatusCode);
                    return Result<TextSearchResponseDto>.Failure($"{response.StatusCode}: {responseContent}");
                }
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
            try
            {
                var location = await _locationService.GetLocationWithTimeoutAsync();

                string textQuery = "";

                if (preferences.IsVegetarian) textQuery += "Vegetarian ";
                if (preferences.IsVegan) textQuery += "Vegan ";
                if (preferences.IsChildFriendly) textQuery += "Child Friendly ";

                textQuery += "Place to eat";

                if (preferences.IsDogFriendly) textQuery += " that Allows Dogs";

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

        private static string MockJsonContent()
        {
            return @"
            {
            ""places"": [
                {
                    ""id"": ""ChIJxUpFWk9vxkcRwNu9kxkQoM8"",
                    ""types"": [
                        ""restaurant"",
                        ""point_of_interest"",
                        ""vegan_restaurant"",
                        ""vegetarian_restaurant"",
                        ""food"",
                        ""establishment""
                    ],
                    ""rating"": 4.9,
                    ""websiteUri"": ""https://www.pampalini.nl/"",
                    ""regularOpeningHours"": {
                        ""openNow"": true,
                        ""weekdayDescriptions"": [
                            ""Monday: Closed"",
                            ""Tuesday: Closed"",
                            ""Wednesday: 10:00 AM – 4:30 PM"",
                            ""Thursday: 10:00 AM – 4:30 PM"",
                            ""Friday: 10:00 AM – 4:30 PM"",
                            ""Saturday: 10:00 AM – 4:30 PM"",
                            ""Sunday: 10:00 AM – 4:30 PM""
                        ]
                    },
                    ""userRatingCount"": 1765,
                    ""displayName"": {
                        ""text"": ""Pampalini Lunchroom & Coffee - Utrecht 2014"",
                        ""languageCode"": ""en""
                    },
                    ""primaryTypeDisplayName"": {
                        ""text"": ""Restaurant""
                    },
                    ""shortFormattedAddress"": ""Wittevrouwenstraat 14, Utrecht"",
                    ""goodForChildren"": true,
                    ""allowsDogs"": true,
                    ""location"": {
                            ""latitude"": 52.092992300000006,
                            ""longitude"": 5.1221492
                            }
                    }
                ],
                ""nextPageToken"": ""AeeoHcI7Xnd8tU32jESwMvgnhAo6QAJfBz6liaHIUALeeGfQ-8NM763uoABKPHSXrxo6MwR6GPkQI3BuamzPLyfNC1ssp5P6JBXwRmUADDsokhrcRQ""
            }";
        }
    }
}
