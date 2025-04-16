using Microsoft.Extensions.Logging;
using SQLite;

namespace WhatMunch_MAUI.Data.SQLite
{
    public interface ILocalDatabase
    {
        Task Init();
        Task<List<PlaceDbEntry>> GetUserPlacesAsync(string userId);
        Task SavePlaceAsync(PlaceDbEntry place);
        Task DeletePlaceAsync(string id);
        Task DeleteAllPlacesAsync();
    }
    public class LocalDatabase: ILocalDatabase
    {
        private readonly ILogger<LocalDatabase> _logger;
        public LocalDatabase(ILogger<LocalDatabase> logger)
        {
            _logger = logger;
        }
        private SQLiteAsyncConnection? _database;

        public async Task Init()
        {
            if (_database is not null)
                return;

            _database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
            await _database.CreateTableAsync<PlaceDbEntry>();
        }

        public async Task<List<PlaceDbEntry>> GetUserPlacesAsync(string userId)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(userId, nameof(userId));

            try
            {
                await Init();
                return await _database!.Table<PlaceDbEntry>()
                    .Where(p => p.UserId == userId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while getting places for user: {userId}", userId);
                throw new InvalidOperationException("Unexpected error while getting places: ", ex);
            }
            
        }

        public async Task SavePlaceAsync(PlaceDbEntry place)
        {
            ArgumentNullException.ThrowIfNull(place, nameof(place));

            try
            {
                await Init();
                await _database!.InsertOrReplaceAsync(place);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while saving place: {place.PlaceId}", place.PlaceId);
                throw new InvalidOperationException("Unexpected error while saving place: ", ex);
            }
        }

        public async Task DeletePlaceAsync(string placeId)
        {
            try
            {
                await Init();
                await _database!.DeleteAsync<PlaceDbEntry>(placeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting entry: {id}", placeId);
                throw new InvalidOperationException("Unexpected error while deleting place: ", ex);
            }
        }

        public async Task DeleteAllPlacesAsync()
        {
            try
            {
                await Init();
                await _database!.DeleteAllAsync<PlaceDbEntry>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting all places");
                throw new InvalidOperationException("Unexpected error while deleting all places", ex);
            }
        }
    }
}
