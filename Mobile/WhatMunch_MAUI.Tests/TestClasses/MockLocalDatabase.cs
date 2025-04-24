using Microsoft.Extensions.Logging;
using SQLite;
using WhatMunch_MAUI.Data;
using WhatMunch_MAUI.Data.SQLite;
using WhatMunch_MAUI.Models;

namespace WhatMunch_MAUI.Tests.TestClasses
{
    public class MockLocalDatabase(ILogger<LocalDatabase> logger, string dbPath) : LocalDatabase(logger)
    {
        public override async Task Init()
        {
            if (_database is not null)
                return;

            _database = new SQLiteAsyncConnection(dbPath, Constants.Flags);
            await _database.CreateTableAsync<PlaceDbEntry>();
        }
    }
}