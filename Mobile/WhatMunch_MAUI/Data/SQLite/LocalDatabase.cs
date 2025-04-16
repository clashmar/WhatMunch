using Microsoft.Extensions.Logging;
using SQLite;

namespace WhatMunch_MAUI.Data.SQLite
{
    public interface ILocalDatabase
    {
        Task Init();
        Task<List<PlaceDbEntry>> GetUserPlacesAsync(string userId);
        Task<int> SavePlaceAsync(PlaceDbEntry place);
        Task<int> DeletePlaceAsync(int id);
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

        public async Task<int> SavePlaceAsync(PlaceDbEntry place)
        {
            ArgumentNullException.ThrowIfNull(place, nameof(place));

            try
            {
                await Init();
                await _database!.InsertOrReplaceAsync(place);

                var result = await _database!.Table<PlaceDbEntry>()
                    .Where(p => p.PlaceId == place.PlaceId)
                    .FirstOrDefaultAsync();

                return result.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while saving place: {place.PlaceId}", place.PlaceId);
                throw new InvalidOperationException("Unexpected error while saving place: ", ex);
            }
        }

        public async Task<int> DeletePlaceAsync(int id)
        {
            try
            {
                await Init();
                return await _database!.DeleteAsync<PlaceDbEntry>(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting entry: {id}", id);
                throw new InvalidOperationException("Unexpected error while deleting place: ", ex);
            }
        }
    }
}
