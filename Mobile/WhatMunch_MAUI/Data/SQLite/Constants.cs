using SQLite;

namespace WhatMunch_MAUI.Data.SQLite
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
    }
}
