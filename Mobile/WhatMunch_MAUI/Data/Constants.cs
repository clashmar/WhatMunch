using SQLite;
using WhatMunch_MAUI.Views;

namespace WhatMunch_MAUI.Data
{
    public static class Constants
    {
        public const string DatabaseFilename = "LocalDatabase.db3";

        public const SQLiteOpenFlags Flags =
            SQLiteOpenFlags.ReadWrite |
            SQLiteOpenFlags.Create |
            SQLiteOpenFlags.SharedCache;

        public static string DatabasePath =>
            Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);

        public const string SEARCH_RESULTS_PAGE = nameof(SearchResultsPage);
        public const string SAVED_PLACES_PAGE = nameof(SavedPlacesPage);

        public const string USERNAME_KEY = "username_key";
    }
}
