using Microsoft.Extensions.Logging;
using System.Text;
using WhatMunch_MAUI.Models.Dtos;
using WhatMunch_MAUI.Resources.Localization;
using WhatMunch_MAUI.Secrets;
using WhatMunch_MAUI.Utility;

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
            "places.displayName," +
            "places.photos," +
            "places.primaryType," +
            "places.rating," +
            "places.userRatingCount," +
            "places.types," +
            "places.regularOpeningHours," +
            "places.goodForChildren," +
            "places.allowsDogs," +
            "places.priceLevel," +
            "nextPageToken";

        public async Task<Result<TextSearchResponseDto>> GetNearbySearchResultsAsync(SearchPreferencesModel preferences, string? pageToken = null)
        {
            //Mock data for development
            var mockDeserializedData = JsonSerializer.Deserialize<TextSearchResponseDto>(MockJsonContent());
            if (mockDeserializedData is TextSearchResponseDto mockResponseDto)
            {
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
            catch (InvalidOperationException ex)
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
            catch (InvalidOperationException ex)
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
                        ""displayName"": { ""text"": ""La Mar Cocina San Francisco"", ""languageCode"": ""en"" },
                        ""primaryType"": ""restaurant"",
                        ""types"": [""seafood_restaurant"", ""restaurant""],
                        ""rating"": 4.5,
                        ""userRatingCount"": 4338,
                        ""priceLevel"": ""PRICE_LEVEL_MODERATE"",
                        ""regularOpeningHours"": { ""openNow"": true },
                        ""photos"": [
                            { ""name"": ""places/photo1"", ""widthPx"": 4800, ""heightPx"": 3600, ""googleMapsUri"": ""https://www.google.com/maps/place/Vegitalian/@52.0929051,5.1193188,2218a,13.1y/data=!3m7!1e2!3m5!1sAF1QipMdiBncYi5UR2CT3r0uyHCQCc-tZnS8ukFNWiI!2e10!3e12!7i4800!8i3200!4m7!3m6!1s0x47c66f296c94daf7:0xe8979e3183f143b3!8m2!3d52.0929051!4d5.1193188!10e5!16s%2Fg%2F11kdlnx5cj?entry=ttu&g_ep=EgoyMDI1MDQwMi4xIKXMDSoASAFQAw%3D%3D"" }
                        ],
                        ""goodForChildren"": true,
                        ""allowsDogs"": false
                    },
                    {
                        ""displayName"": { ""text"": ""Nobu SF"", ""languageCode"": ""en"" },
                        ""primaryType"": ""restaurant"",
                        ""types"": [""japanese_restaurant"", ""restaurant""],
                        ""rating"": 4.7,
                        ""userRatingCount"": 3254,
                        ""priceLevel"": ""PRICE_LEVEL_EXPENSIVE"",
                        ""regularOpeningHours"": { ""openNow"": false },
                        ""photos"": [
                            { ""name"": ""places/photo2"", ""widthPx"": 3800, ""heightPx"": 2600, ""googleMapsUri"": ""https://www.google.com/maps/place/Vegitalian/@52.0929051,5.1193188,2218a,13.1y/data=!3m7!1e2!3m5!1sAF1QipMdiBncYi5UR2CT3r0uyHCQCc-tZnS8ukFNWiI!2e10!3e12!7i4800!8i3200!4m7!3m6!1s0x47c66f296c94daf7:0xe8979e3183f143b3!8m2!3d52.0929051!4d5.1193188!10e5!16s%2Fg%2F11kdlnx5cj?entry=ttu&g_ep=EgoyMDI1MDQwMi4xIKXMDSoASAFQAw%3D%3D"" }
                        ],
                        ""goodForChildren"": false,
                        ""allowsDogs"": false
                    },
                    {
                        ""displayName"": { ""text"": ""Pizzeria Delfina"", ""languageCode"": ""en"" },
                        ""primaryType"": ""restaurant"",
                        ""types"": [""pizza_restaurant"", ""restaurant"", ""food""],
                        ""rating"": 4.4,
                        ""userRatingCount"": 2311,
                        ""priceLevel"": ""PRICE_LEVEL_MODERATE"",
                        ""regularOpeningHours"": { ""openNow"": true },
                        ""photos"": [
                            { ""name"": ""places/photo3"", ""widthPx"": 4000, ""heightPx"": 3000, ""googleMapsUri"": ""https://www.google.com/maps/place//data=!3m4!1e2!3m2!1sCIHM0ogKEICAgIDfhaO7_gE!2e10!4m2!3m1!1s0x8085806737ca1051:0xaa881a41cd0c4037"" }
                        ],
                        ""goodForChildren"": true,
                        ""allowsDogs"": true
                    },
                    {
                        ""displayName"": { ""text"": ""The French Laundry"", ""languageCode"": ""en"" },
                        ""primaryType"": ""cafe"",
                        ""types"": [""french_restaurant"", ""restaurant"", ""cafe""],
                        ""rating"": 4.9,
                        ""userRatingCount"": 1245,
                        ""priceLevel"": ""PRICE_LEVEL_EXPENSIVE"",
                        ""regularOpeningHours"": { ""openNow"": false },
                        ""photos"": [
                            { ""name"": ""places/photo4"", ""widthPx"": 4500, ""heightPx"": 3400, ""googleMapsUri"": ""https://www.google.com/maps/place//data=!3m4!1e2!3m2!1sCIHM0ogKEICAgIDfhaO7_gE!2e10!4m2!3m1!1s0x8085806737ca1051:0xaa881a41cd0c4037"" }
                        ],
                        ""goodForChildren"": false,
                        ""allowsDogs"": false
                    },
                    {
                        ""displayName"": { ""text"": ""Mission Chinese Food"", ""languageCode"": ""en"" },
                        ""primaryType"": ""restaurant"",
                        ""types"": [""chinese_restaurant"", ""restaurant""],
                        ""rating"": 4.3,
                        ""userRatingCount"": 2891,
                        ""priceLevel"": ""PRICE_LEVEL_INEXPENSIVE"",
                        ""regularOpeningHours"": { ""openNow"": true },
                        ""photos"": [
                            { ""name"": ""places/photo5"", ""widthPx"": 4200, ""heightPx"": 3100, ""googleMapsUri"": ""https://www.google.com/maps/place//data=!3m4!1e2!3m2!1sCIHM0ogKEICAgIDfhaO7_gE!2e10!4m2!3m1!1s0x8085806737ca1051:0xaa881a41cd0c4037"" }
                        ],
                        ""goodForChildren"": true,
                        ""allowsDogs"": false
                    },
                    {
                        ""displayName"": { ""text"": ""Benu"", ""languageCode"": ""en"" },
                        ""primaryType"": ""restaurant"",
                        ""types"": [""fine_dining_restaurant"", ""restaurant""],
                        ""rating"": 4.8,
                        ""userRatingCount"": 897,
                        ""priceLevel"": ""PRICE_LEVEL_VERY_EXPENSIVE"",
                        ""regularOpeningHours"": { ""openNow"": false },
                        ""photos"": [
                            { ""name"": ""places/photo6"", ""widthPx"": 4600, ""heightPx"": 3500, ""googleMapsUri"": ""https://www.google.com/maps/place//data=!3m4!1e2!3m2!1sCIHM0ogKEICAgIDfhaO7_gE!2e10!4m2!3m1!1s0x8085806737ca1051:0xaa881a41cd0c4037"" }
                        ],
                        ""goodForChildren"": false,
                        ""allowsDogs"": false
                    }
                ]
            }";
        }
    }
}
