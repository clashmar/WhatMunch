using SQLite;

namespace WhatMunch_MAUI.Data.SQLite
{
    public interface ILocalDatabase
    {
        Task Init();
        Task<List<PlaceDbEntry>> GetUserPlacesAsync(string userId);
        Task<int> SavePlaceAsync(PlaceDbEntry place);
        Task<int> DeletePlaceAsync(string id);
    }
    public class LocalDatabase: ILocalDatabase
    {
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
            await Init();
            return await _database!.Table<PlaceDbEntry>()
                .Where(p => p.UserId == userId)
                .ToListAsync();
        }

        public async Task<int> SavePlaceAsync(PlaceDbEntry place)
        {
            ArgumentNullException.ThrowIfNull(place, nameof(place));
            return await _database!.InsertOrReplaceAsync(place);
        }

        public async Task<int> DeletePlaceAsync(string id)
        {
            ArgumentNullException.ThrowIfNull(id, nameof(id));
            return await _database!.DeleteAsync<PlaceDbEntry>(id);
        }
    }
}
