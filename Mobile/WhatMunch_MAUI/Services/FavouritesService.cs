using Microsoft.Extensions.Logging;
using WhatMunch_MAUI.Data.SQLite;
using WhatMunch_MAUI.Extensions;
using WhatMunch_MAUI.Models.Dtos;
using WhatMunch_MAUI.Utility;

namespace WhatMunch_MAUI.Services
{
    public interface IFavouritesService
    {
        Task<Result<List<PlaceDto>>> GetUserFavouritesAsync();
        Task SaveUserFavouriteAsync(PlaceDto placeDto);
        Task DeleteUserFavouriteAsync(PlaceDto placeDto);
        Task DeleteAllUserFavouritesAsync();
    }
    public class FavouritesService(
        ILocalDatabase localDatabase,
        ILogger<FavouritesService> logger,
        ISecureStorageService secureStorageService,
        ILocationService locationService) : IFavouritesService
    {
        public async Task<Result<List<PlaceDto>>> GetUserFavouritesAsync()
        {
            try
            {
                // TODO: check open now/remove open now/update dto
                var locationTask = locationService.GetLastSearchLocation();
                string? username = await secureStorageService.GetUsernameAsync();

                if (string.IsNullOrEmpty(username))
                {
                    logger.LogWarning("No username found in secure storage.");
                    return Result<List<PlaceDto>>.Failure("Username not found.");
                    // TODO: Handle redirect to login if no username can be found
                }

                var favourites = await localDatabase.GetUserPlacesAsync(username);

                if (favourites is not null && favourites.Count > 0)
                {
                    var result = favourites
                        .Select(f => DeserializePlace(f.PlaceJson))
                        .AddDistances(await locationTask)
                        .OrderBy(f => f.Distance)
                        .ToList();

                    return Result<List<PlaceDto>>.Success(result);
                }

                // TODO: make another call to the backend and update localdatabase
                return Result<List<PlaceDto>>.Failure("No favorites found.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while retrieving user favorites.");
                return Result<List<PlaceDto>>.Failure(ex.Message);
            }
        }

        public async Task SaveUserFavouriteAsync(PlaceDto placeDto) 
        {
            try
            {
                var placeDbEntry = await CreatePlaceDbEntryAsync(placeDto);
                await localDatabase.SavePlaceAsync(placeDbEntry);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while saving to favourites");
                throw;
            }
        }

        public async Task DeleteUserFavouriteAsync(PlaceDto placeDto)
        {
            try
            {
                await localDatabase.DeletePlaceAsync(placeDto.Id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while deleting favourite.");
                throw;
            }
        }

        public async Task DeleteAllUserFavouritesAsync()
        {
            try
            {
                await localDatabase.DeleteAllPlacesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while deleting all favourites.");
                throw;
            }
        }

        private async Task<PlaceDbEntry> CreatePlaceDbEntryAsync(PlaceDto placeDto)
        {
            var username = await secureStorageService.GetUsernameAsync() 
                ?? throw new InvalidOperationException("Username is not available from secure storage");

            var placeJson = JsonSerializer.Serialize(placeDto);

            var result = new PlaceDbEntry()
            {
                UserId = username,
                PlaceId = placeDto.Id,
                PlaceJson = placeJson,
                SavedAt = DateTime.UtcNow
            };

            return result;
        }

        private static PlaceDto DeserializePlace(string placeJson)
        {
            return JsonSerializer.Deserialize<PlaceDto>(placeJson)
                ?? throw new InvalidOperationException("Failed to deserialize PlaceDto.");
        }
    }
}
