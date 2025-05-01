using Microsoft.Extensions.Logging;
using System.Text;
using WhatMunch_MAUI.Extensions;
using WhatMunch_MAUI.MockData;
using WhatMunch_MAUI.Models.Dtos;
using WhatMunch_MAUI.Resources.Localization;
using WhatMunch_MAUI.Utility;
using WhatMunch_MAUI.Utility.Exceptions;

namespace WhatMunch_MAUI.Services
{
    public interface IGooglePlacesService
    {
        Task<Result<TextSearchResponseDto>> GetNearbySearchResultsAsync(SearchPreferencesModel preferences, string? pageToken = null);
        Task<Result<PlaceDto>> GetPlaceDetailsAsync(string placeId, CancellationToken ct);
    }

    public class GooglePlacesService(
        IHttpClientFactory clientFactory,
        ILogger<GooglePlacesService> logger,
        ILocationService locationService,
        IDjangoApiService djangoApiService) : IGooglePlacesService
    {

        // Specify what the api returns
        private readonly string PLACES_FIELD_MASK = "places." + DETAILS_FIELD_MASK.Replace(",", ",places.") + ",nextPageToken";

        private static readonly string DETAILS_FIELD_MASK = string.Join(",",
            "id",
            "displayName",
            "photos",
            "primaryType",
            "primaryTypeDisplayName",
            "rating",
            "userRatingCount",
            "types",
            "regularOpeningHours",
            "goodForChildren",
            "allowsDogs",
            "priceLevel",
            "websiteUri",
            "internationalPhoneNumber",
            "shortFormattedAddress",
            "location",
            "reviews",
            "generativeSummary",
            "reviewSummary"
        );

        public async Task<Result<TextSearchResponseDto>> GetNearbySearchResultsAsync(SearchPreferencesModel preferences, string? pageToken = null)
        {
            preferences ??= SearchPreferencesModel.Default;

            return await ExecuteRequestAsync(async () =>
            {
                //Mock data for development
                //var mockDeserializedData = JsonSerializer.Deserialize<TextSearchResponseDto>(MockPlace.GetMockPlaceJson());
                //if (mockDeserializedData is TextSearchResponseDto mockResponseDto)
                //{
                //    mockResponseDto.SearchLocation = await locationService.GetLastSearchLocation();
                //    return Result<TextSearchResponseDto>.Success(mockResponseDto);
                //}
                //else
                //{
                //    logger.LogError("Failed to deserialize mock response");
                //    return Result<TextSearchResponseDto>.Failure("Failed to deserialize mock response");
                //}

                preferences ??= SearchPreferencesModel.Default;

                var apiKeyTask = djangoApiService.GetGoogleMapsApiKeyAsync();
                var jsonTask = CreateNearbySearchJsonAsync(preferences, pageToken);

                await Task.WhenAll(apiKeyTask, jsonTask);

                var apiKey = apiKeyTask.Result;
                var jsonContent = jsonTask.Result;

                using var client = clientFactory.CreateClient("GooglePlaces").UpdateLanguageHeaders();
                client.DefaultRequestHeaders.Add("X-Goog-Api-Key", apiKey);
                client.DefaultRequestHeaders.Add("X-Goog-FieldMask", PLACES_FIELD_MASK);

                using var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                using var response = await client.PostAsync("v1/places:searchText", stringContent);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var deserializedData = JsonSerializer.Deserialize<TextSearchResponseDto>(responseContent);

                if (deserializedData is null)
                {
                    logger.LogError("Failed to deserialize nearby search response");
                    return Result<TextSearchResponseDto>.Failure(AppResources.ErrorUnexpected);
                }

                deserializedData.SearchLocation = await locationService.GetLastSearchLocation();
                return Result<TextSearchResponseDto>.Success(deserializedData);
            });
        }

        public async Task<Result<PlaceDto>> GetPlaceDetailsAsync(string placeId, CancellationToken ct)
        {
            return await ExecuteRequestAsync(async () =>
            {
                var apiKey = await djangoApiService.GetGoogleMapsApiKeyAsync();

                var client = clientFactory.CreateClient("GooglePlaces").UpdateLanguageHeaders();
                client.DefaultRequestHeaders.Add("X-Goog-Api-Key", apiKey);
                client.DefaultRequestHeaders.Add("X-Goog-FieldMask", DETAILS_FIELD_MASK);

                var response = await client.GetAsync($"v1/places/{placeId}");
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var deserializedData = JsonSerializer.Deserialize<PlaceDto>(responseContent);

                if (deserializedData is null)
                {
                    logger.LogError("Failed to deserialize place details.");
                    return Result<PlaceDto>.Failure(AppResources.ErrorUnexpected);
                }
                return Result<PlaceDto>.Success(deserializedData);
            });
        }

        private async Task<Result<T>> ExecuteRequestAsync<T>(Func<Task<Result<T>>> requestFunc)
        {
            try
            {
                return await requestFunc.Invoke();
            }
            catch (HttpRequestException ex)
            {
                logger.LogError(ex, "HTTP request failed: {Message}", ex.Message);
                return Result<T>.Failure(AppResources.ErrorUnexpected);
            }
            catch (LocationException ex)
            {
                logger.LogError(ex, "Location service error: {Message}", ex.Message);
                return Result<T>.Failure(AppResources.ErrorLocationServices);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error: {Message}", ex.Message);
                return Result<T>.Failure(AppResources.ErrorUnexpected);
            }
        }

        private async Task<string> CreateNearbySearchJsonAsync(SearchPreferencesModel preferences, string? pageToken = null)
        {
            preferences ??= SearchPreferencesModel.Default;

            try
            {
                var location = await locationService.GetLocationWithTimeoutAsync();

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
                logger.LogError(ex, "An error occurred while getting the geolocation of this device");
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unexpected error occurred while creating search object");
                throw;
            }
        }
    }
}
