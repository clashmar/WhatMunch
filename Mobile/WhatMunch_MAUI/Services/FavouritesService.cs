using Microsoft.Extensions.Logging;
using WhatMunch_MAUI.Data.SQLite;
using WhatMunch_MAUI.Models.Dtos;
using WhatMunch_MAUI.Utility;

namespace WhatMunch_MAUI.Services
{
    public interface IFavouritesService
    {
        Task<Result<List<PlaceDto?>>> GetUserFavourites();
    }
    public class FavouritesService : IFavouritesService
    {
        private readonly ILocalDatabase _localDatabase;
        private readonly ILogger<FavouritesService> _logger;
        public FavouritesService(ILocalDatabase localDatabase, ILogger<FavouritesService> logger)
        {
            _localDatabase = localDatabase;
            _logger = logger;
        }

        public async Task<Result<List<PlaceDto?>>> GetUserFavourites()
        {
            try
            {
                List<PlaceDbEntry> localFavourites = await _localDatabase.GetUserPlacesAsync("userId");

                if (localFavourites is not null && localFavourites.Count > 0)
                {
                    var result = localFavourites
                        .Select(f => JsonSerializer.Deserialize<PlaceDto>(f.PlaceJson))
                        .ToList();

                    return Result< List<PlaceDto?>>.Success(result);
                }

                // TODO: make another call to the backend and update localdatabase
                return Result<List<PlaceDto?>>.Failure("Failure");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while trying to get user favouries");
                return Result<List<PlaceDto?>>.Failure(ex.Message);
            }
        }
    }
}
