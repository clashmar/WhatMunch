using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;
using WhatMunch_MAUI.Models;
using WhatMunch_MAUI.Services;

namespace WhatMunch_MAUI.Tests.Unit.Services
{
    public class SearchPreferencesServiceTests
    {
        private readonly Mock<ISecureStorage> _secureStorageMock;
        private readonly Mock<ILogger<SearchPreferencesService>> _loggerMock;
        private readonly SearchPreferencesService _service;

        public SearchPreferencesServiceTests()
        {
            _secureStorageMock = new();
            _loggerMock = new();
            _service = new(_secureStorageMock.Object, _loggerMock.Object);
        }

        private const string PREFERENCES_KEY = "search_preferences";

        private readonly SearchPreferencesModel _preferences = new()
        {
            MinRating = 4.5,
            MaxPriceLevel = PriceLevel.PRICE_LEVEL_MODERATE,
            SearchRadius = 1000,
            IsVegetarian = true,
            IsVegan = false,
            IsChildFriendly = true,
            IsDogFriendly = false,
            RankPreference = RankPreference.DISTANCE
        };

        [Fact]
        public async Task SavePreferencesAsync_ShouldSavePreferences()
        {
            // Arrange
            var json = JsonSerializer.Serialize(_preferences);

            // Act
            await _service.SavePreferencesAsync(_preferences);

            // Assert
            _secureStorageMock.Verify(s => s.SetAsync(PREFERENCES_KEY, json), Times.Once);
        }

        [Fact]
        public async Task GetPreferencesAsync_ShouldReturnPreferences_WhenPreferencesExist()
        {
            // Arrange
            var json = JsonSerializer.Serialize(_preferences);

            _secureStorageMock.Setup(s => s.GetAsync(PREFERENCES_KEY)).ReturnsAsync(json);

            // Act
            var result = await _service.GetPreferencesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_preferences.MinRating, result.MinRating);
            Assert.Equal(_preferences.MaxPriceLevel, result.MaxPriceLevel);
            Assert.Equal(_preferences.SearchRadius, result.SearchRadius);
            Assert.Equal(_preferences.IsVegetarian, result.IsVegetarian);
            Assert.Equal(_preferences.IsVegan, result.IsVegan);
            Assert.Equal(_preferences.IsChildFriendly, result.IsChildFriendly);
            Assert.Equal(_preferences.IsDogFriendly, result.IsDogFriendly);
            Assert.Equal(_preferences.RankPreference, result.RankPreference);
        }

        [Fact]
        public async Task GetPreferencesAsync_ShouldReturnDefault_WhenPreferencesDoNotExist()
        {
            // Arrange
            _secureStorageMock.Setup(s => s.GetAsync(PREFERENCES_KEY)).ReturnsAsync((string?)null);

            // Act
            var result = await _service.GetPreferencesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equivalent(SearchPreferencesModel.Default, result);
        }

        [Fact]
        public async Task GetPreferencesAsync_ShouldReturnDefault_WhenDeserializationFails()
        {
            // Arrange
            _secureStorageMock.Setup(s => s.GetAsync(PREFERENCES_KEY)).ReturnsAsync("invalid_json");

            // Act
            var result = await _service.GetPreferencesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equivalent(SearchPreferencesModel.Default, result);
        }
    }
}
