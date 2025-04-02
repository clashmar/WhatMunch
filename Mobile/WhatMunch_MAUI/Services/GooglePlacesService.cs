using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json.Serialization;
using WhatMunch_MAUI.Dtos;
using WhatMunch_MAUI.Resources.Localization;
using WhatMunch_MAUI.Secrets;
using WhatMunch_MAUI.Utility;

namespace WhatMunch_MAUI.Services
{
    public interface IGooglePlacesService
    {
        Task<Result<NearbySearchResponseDto>> GetNearbySearchResults();
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

        private const string FIELD_MASK = "" +
            "places.displayName," +
            "places.photos," +
            "places.primaryType," +
            "places.rating," +
            "places.userRatingCount," +
            "places.types," +
            "places.regularOpeningHours";

        public async Task<Result<NearbySearchResponseDto>> GetNearbySearchResults()
        {
            // Mock data for development
            //var mockDeserializedData = JsonSerializer.Deserialize<NearbySearchResponseDto>(MockJsonContent());
            //if (mockDeserializedData is NearbySearchResponseDto mockResponseDto)
            //{
            //    return Result<NearbySearchResponseDto>.Success(mockResponseDto);
            //}
            //else
            //{
            //    _logger.LogError("Failed to deserialize mock response");
            //    return Result<NearbySearchResponseDto>.Failure("Failed to deserialize mock response");
            //}

            try
            {
                Task<string> jsonContent = CreateNearbySearchJsonAsync();

                var client = _clientFactory.CreateClient("GooglePlaces");
                client.DefaultRequestHeaders.Add("X-Goog-Api-Key", _apiKey);
                client.DefaultRequestHeaders.Add("X-Goog-FieldMask", FIELD_MASK);

                StringContent stringContent = new(await jsonContent, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync("v1/places:searchNearby", stringContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var deserializedData = JsonSerializer.Deserialize<NearbySearchResponseDto>(responseContent);

                    if (deserializedData is NearbySearchResponseDto searchResponseDto)
                    {
                        return Result<NearbySearchResponseDto>.Success(searchResponseDto);
                    }
                    else
                    {
                        _logger.LogError("Failed to deserialize nearby search response");
                        return Result<NearbySearchResponseDto>.Failure(AppResources.ErrorUnexpected);
                    }
                }
                else
                {
                    _logger.LogWarning("Nearby search returned {response.StatusCode}", response.StatusCode);
                    return Result<NearbySearchResponseDto>.Failure($"{AppResources.SearchReturned}: {response.StatusCode}");
                }
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Location services are unavailable");
                return Result<NearbySearchResponseDto>.Failure(AppResources.ErrorLocationServices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred in GooglePlacesService");
                return Result<NearbySearchResponseDto>.Failure(AppResources.ErrorUnexpected);
            }
        }

        private async Task<string> CreateNearbySearchJsonAsync()
        {
            try
            {
                var location = await _locationService.GetLocationWithTimeout();

                var request = new NearbySearchRequest
                {
                    LocationRestriction = new LocationRestriction
                    {
                        Circle = new Circle
                        {
                            Center = new Center
                            {
                                Latitude = location.Latitude,
                                Longitude = location.Longitude,
                            },
                            Radius = 500.0
                        }
                    }
                };
                return JsonSerializer.Serialize(request);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while creating search object");
                return null;
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
                        ""types"": [""seafood_restaurant"", ""restaurant"", ""food""],
                        ""rating"": 4.5,
                        ""userRatingCount"": 4338,
                        ""regularOpeningHours"": { ""openNow"": true },
                        ""photos"": [
                            { ""name"": ""places/photo1"", ""widthPx"": 4800, ""heightPx"": 3600, ""googleMapsUri"": ""https://www.google.com/maps/place//data=!3m4!1e2!3m2!1sCIHM0ogKEICAgIDfhaO7_gE!2e10!4m2!3m1!1s0x8085806737ca1051:0xaa881a41cd0c4037"" }
                        ]
                    },
                    {
                        ""displayName"": { ""text"": ""Nobu SF"", ""languageCode"": ""en"" },
                        ""primaryType"": ""restaurant"",
                        ""types"": [""japanese_restaurant"", ""restaurant"", ""food""],
                        ""rating"": 4.7,
                        ""userRatingCount"": 3254,
                        ""regularOpeningHours"": { ""openNow"": false },
                        ""photos"": [
                            { ""name"": ""places/photo2"", ""widthPx"": 3800, ""heightPx"": 2600, ""googleMapsUri"": ""https://www.google.com/maps/place//data=!3m4!1e2!3m2!1sCIHM0ogKEICAgIDfhaO7_gE!2e10!4m2!3m1!1s0x8085806737ca1051:0xaa881a41cd0c4037"" }
                        ]
                    },
                    {
                        ""displayName"": { ""text"": ""Pizzeria Delfina"", ""languageCode"": ""en"" },
                        ""primaryType"": ""restaurant"",
                        ""types"": [""pizza"", ""restaurant"", ""food""],
                        ""rating"": 4.4,
                        ""userRatingCount"": 2311,
                        ""regularOpeningHours"": { ""openNow"": true },
                        ""photos"": [
                            { ""name"": ""places/photo3"", ""widthPx"": 4000, ""heightPx"": 3000, ""googleMapsUri"": ""https://www.google.com/maps/place//data=!3m4!1e2!3m2!1sCIHM0ogKEICAgIDfhaO7_gE!2e10!4m2!3m1!1s0x8085806737ca1051:0xaa881a41cd0c4037"" }
                        ]
                    },
                    {
                        ""displayName"": { ""text"": ""The French Laundry"", ""languageCode"": ""en"" },
                        ""primaryType"": ""restaurant"",
                        ""types"": [""french_restaurant"", ""restaurant"", ""food""],
                        ""rating"": 4.9,
                        ""userRatingCount"": 1245,
                        ""regularOpeningHours"": { ""openNow"": false },
                        ""photos"": [
                            { ""name"": ""places/photo4"", ""widthPx"": 4500, ""heightPx"": 3400, ""googleMapsUri"": ""https://www.google.com/maps/place//data=!3m4!1e2!3m2!1sCIHM0ogKEICAgIDfhaO7_gE!2e10!4m2!3m1!1s0x8085806737ca1051:0xaa881a41cd0c4037"" }
                        ]
                    },
                    {
                        ""displayName"": { ""text"": ""Mission Chinese Food"", ""languageCode"": ""en"" },
                        ""primaryType"": ""restaurant"",
                        ""types"": [""chinese_restaurant"", ""restaurant"", ""food""],
                        ""rating"": 4.3,
                        ""userRatingCount"": 2891,
                        ""regularOpeningHours"": { ""openNow"": true },
                        ""photos"": [
                            { ""name"": ""places/photo5"", ""widthPx"": 4200, ""heightPx"": 3100, ""googleMapsUri"": ""https://www.google.com/maps/place//data=!3m4!1e2!3m2!1sCIHM0ogKEICAgIDfhaO7_gE!2e10!4m2!3m1!1s0x8085806737ca1051:0xaa881a41cd0c4037"" }
                        ]
                    },
                    {
                        ""displayName"": { ""text"": ""Benu"", ""languageCode"": ""en"" },
                        ""primaryType"": ""restaurant"",
                        ""types"": [""fusion_restaurant"", ""restaurant"", ""food""],
                        ""rating"": 4.8,
                        ""userRatingCount"": 897,
                        ""regularOpeningHours"": { ""openNow"": false },
                        ""photos"": [
                            { ""name"": ""places/photo6"", ""widthPx"": 4600, ""heightPx"": 3500, ""googleMapsUri"": ""https://www.google.com/maps/place//data=!3m4!1e2!3m2!1sCIHM0ogKEICAgIDfhaO7_gE!2e10!4m2!3m1!1s0x8085806737ca1051:0xaa881a41cd0c4037"" }
                        ]
                    }
                ]
            }";
        }
    }

    public class NearbySearchRequest
    {
        [JsonPropertyName("includedTypes")]
        public string[] IncludedTypes { get; set; } = ["restaurant"];

        [JsonPropertyName("maxResultCount")]
        public int MaxResultCount { get; set; } = 10;

        [JsonPropertyName("locationRestriction")]
        public required LocationRestriction LocationRestriction { get; set; }
    }

    public class LocationRestriction
    {
        [JsonPropertyName("circle")]
        public required Circle Circle { get; set; }
    }

    public class Circle
    {
        [JsonPropertyName("center")]
        public required Center Center { get; set; }

        [JsonPropertyName("radius")]
        public double Radius { get; set; } = 800;
    }

    public class Center
    {
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; } = 37.7937;

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; } = -122.3965;
    }
}
