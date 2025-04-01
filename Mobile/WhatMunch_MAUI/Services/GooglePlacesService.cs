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
        private readonly string _apiKey;

        public GooglePlacesService(
            IHttpClientFactory clientFactory,
            ILogger<GooglePlacesService> logger)
        {
            _clientFactory = clientFactory;
            _logger = logger;
            _apiKey = ApiKeys.GOOGLE_MAPS_API_KEY;
        }

        private const string FIELD_MASK = "places.displayName,places.photos,places.primaryType,places.rating,places.userRatingCount,places.types,places.regularOpeningHours";

        public async Task<Result<NearbySearchResponseDto>> GetNearbySearchResults()
        {
            try
            {
                var client = _clientFactory.CreateClient("GooglePlaces");

                client.DefaultRequestHeaders.Add("X-Goog-Api-Key", _apiKey);
                client.DefaultRequestHeaders.Add("X-Goog-FieldMask", FIELD_MASK);

                var jsonContent = CreateNearbySearchJson();
                var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
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
            catch (Exception)
            {
                _logger.LogError("An unexpected error occurred in GooglePlacesService");
                return Result<NearbySearchResponseDto>.Failure(AppResources.ErrorUnexpected);
            }
        }

        private static string CreateNearbySearchJson()
        {
            var request = new NearbySearchRequest
            {
                LocationRestriction = new LocationRestriction
                {
                    Circle = new Circle
                    {
                        Center = new Center
                        {
                            Latitude = 37.7937,
                            Longitude = -122.3965
                        },
                        Radius = 500.0
                    }
                }
            };
            return JsonSerializer.Serialize(request);
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
