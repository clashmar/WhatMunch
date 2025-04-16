using Microsoft.Extensions.Logging;
using Moq;
using WhatMunch_MAUI.Data.SQLite;
using WhatMunch_MAUI.MockData;
using WhatMunch_MAUI.Models;
using WhatMunch_MAUI.Tests.TestClasses;

namespace WhatMunch_MAUI.Tests.Unit.Services
{
    public class LocalDatabaseTests
    {
        private readonly Mock<ILogger<LocalDatabase>> _loggerMock;
        private readonly MockLocalDatabase _localDatabase;

        private const string PLACE_ID = "123";
        private const string USERNAME = "username";
        private readonly string JSON = MockPlace.GetMockPlaceJson();

        public LocalDatabaseTests()
        {
            _loggerMock = new();
            _localDatabase = new MockLocalDatabase(_loggerMock.Object, ":memory:");
        }

        private async Task SetupDatabaseAsync()
        {
            await _localDatabase.Init();

            var place = new PlaceDbEntry
            {
                PlaceId = PLACE_ID,
                UserId = USERNAME,
                PlaceJson = MockPlace.GetMockPlaceJson(),
            };

            await _localDatabase.SavePlaceAsync(place);
        }

        [Fact]
        private async Task SavePlaceAsync_ShouldSaveAndRetrieveEntry()
        {
            // Arrange
            await SetupDatabaseAsync();

            // Act
            var results = await _localDatabase.GetUserPlacesAsync(USERNAME);

            // Assert
            Assert.Single(results);
            Assert.Equal(PLACE_ID, results.FirstOrDefault()?.PlaceId);
            Assert.Equal(USERNAME, results.FirstOrDefault()?.UserId);
            Assert.Equal(JSON, results.FirstOrDefault()?.PlaceJson);
        }

        [Fact]
        public async Task GetUserPlacesAsync_ShouldReturnEmptyList_WhenNoMatches()
        {
            // Arrange
            await SetupDatabaseAsync();

            // Act
            var results = await _localDatabase.GetUserPlacesAsync("nonexistentuser");

            // Assert
            Assert.Empty(results);
        }

        [Fact]
        public async Task DeletePlaceAsync_ShouldRemoveCorrectEntry()
        {
            // Arrange
            await SetupDatabaseAsync();

            // Act
            await _localDatabase.DeletePlaceAsync(PLACE_ID);
            var results = await _localDatabase.GetUserPlacesAsync(USERNAME);

            // Assert
            Assert.Empty(results);
        }

        [Fact]
        public async Task DeleteAllPlacesAsync_ShouldRemoveAllEntries()
        {
            // Arrange
            await SetupDatabaseAsync();

            // Act
            await _localDatabase.DeleteAllPlacesAsync();
            var results = await _localDatabase.GetUserPlacesAsync(USERNAME);

            // Assert
            Assert.Empty(results);
        }
    }
}
