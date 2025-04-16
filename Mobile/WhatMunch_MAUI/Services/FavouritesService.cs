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
        Task<int> SaveUserFavouriteAsync(PlaceDto placeDto);
        Task DeleteUserFavouriteAsync(PlaceDto placeDto);
    }
    public class FavouritesService : IFavouritesService
    {
        private readonly ILocalDatabase _localDatabase;
        private readonly ILogger<FavouritesService> _logger;
        private readonly ISecureStorageService _secureStorageService;
        private readonly ILocationService _locationService;
        public FavouritesService(
            ILocalDatabase localDatabase, 
            ILogger<FavouritesService> logger, 
            ISecureStorageService secureStorageService,
            ILocationService locationService) 
        {
            _localDatabase = localDatabase;
            _logger = logger;
            _secureStorageService = secureStorageService;
            _locationService = locationService;
        }

        public async Task<Result<List<PlaceDto>>> GetUserFavouritesAsync()
        {
            try
            {
                // TODO: check open now/remove open now/update dto
                var locationTask = _locationService.GetLastSearchLocation();

                string? username = await _secureStorageService.GetUsernameAsync();
                if (string.IsNullOrEmpty(username))
                {
                    _logger.LogWarning("No username found in secure storage.");
                    return Result<List<PlaceDto>>.Failure();
                    // TODO: Handle redirect to login if no username can be found
                }

                var favourites = await _localDatabase.GetUserPlacesAsync(username);

                if (favourites is not null && favourites.Count > 0)
                {
                    var result = favourites
                        .Select(f =>
                        {
                            var place = JsonSerializer.Deserialize<PlaceDto>(f.PlaceJson) ?? throw new Exception("Deserialization failed");
                            place.DbId = f.Id;
                            return place;
                        })
                        .ToList()
                        .AddDistances(await locationTask) ?? [];

                    // TODO: Order by distance
                    
                    return Result<List<PlaceDto>>.Success(result);
                }

                // TODO: make another call to the backend and update localdatabase
                return Result<List<PlaceDto>>.Failure();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while trying to get user favourites");
                return Result<List<PlaceDto>>.Failure(ex.Message);
            }
        }

        public async Task<int> SaveUserFavouriteAsync(PlaceDto placeDto) 
        {
            try
            {
                var placeDbEntry = await CreatePlaceDbEntryAsync(placeDto);
                return await _localDatabase.SavePlaceAsync(placeDbEntry);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while trying to save to favourites");
                throw;
            }
        }

        public async Task DeleteUserFavouriteAsync(PlaceDto placeDto)
        {
            try
            {
                await _localDatabase.DeletePlaceAsync(placeDto.DbId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while trying to save to favourites");
                throw;
            }
        }

        private async Task<PlaceDbEntry> CreatePlaceDbEntryAsync(PlaceDto placeDto)
        {
            var username = await _secureStorageService.GetUsernameAsync() 
                ?? throw new InvalidOperationException("Username is not available from secure storage");

            var placeJson = JsonSerializer.Serialize(placeDto);

            var result = new PlaceDbEntry()
            {
                UserId = username,
                PlaceId = placeDto.Id,
                PlaceJson = placeJson,
            };

            return result;
        }
    }
}
