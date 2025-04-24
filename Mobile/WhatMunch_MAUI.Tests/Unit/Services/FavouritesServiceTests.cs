using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;
using WhatMunch_MAUI.Data;
using WhatMunch_MAUI.Data.SQLite;
using WhatMunch_MAUI.MockData;
using WhatMunch_MAUI.Models;
using WhatMunch_MAUI.Models.Dtos;
using WhatMunch_MAUI.Services;
using WhatMunch_MAUI.Utility;

namespace WhatMunch_MAUI.Tests.Unit.Services
{
    public class FavouritesServiceTests
    {
        private readonly Mock<ILocalDatabase> _localDatabaseMock;
        private readonly Mock<ILogger<FavouritesService>> _loggerMock;
        private readonly Mock<ISecureStorage> _secureStorageMock;
        private readonly Mock<ILocationService> _locationServiceMock;
        private readonly Mock<IGooglePlacesService> _googlePlacesServiceMock;
        private readonly FavouritesService _favouritesService;

        public FavouritesServiceTests()
        {
            _localDatabaseMock = new();
            _loggerMock = new();
            _secureStorageMock = new();
            _locationServiceMock = new();
            _googlePlacesServiceMock = new();

            _favouritesService = new FavouritesService(
                _localDatabaseMock.Object,
                _loggerMock.Object,
                _secureStorageMock.Object,
                _locationServiceMock.Object,
                _googlePlacesServiceMock.Object

            );
        }

        private readonly PlaceDto _placeDto = new()
        {
            Id = MockPlace.ID,
        };

        private const string USERNAME = "username";

        [Fact]
        public async Task GetUserFavouritesAsync_ReturnsFavourites_WhenUsernameExists()
        {
            // Arrange
            var username = "username";
            var placeJson = JsonSerializer.Serialize(_placeDto);
            var placeDbEntries = new List<PlaceDbEntry>
            {
                new() { PlaceId = MockPlace.ID, UserId = USERNAME, PlaceJson = placeJson }
            };

            _secureStorageMock
                .Setup(s => s.GetAsync(Constants.USERNAME_KEY))
                .ReturnsAsync(USERNAME);

            _localDatabaseMock
                .Setup(db => db.GetUserPlacesAsync(username))
                .ReturnsAsync(placeDbEntries);

            _locationServiceMock
                .Setup(l => l.GetLastSearchLocation())
                .ReturnsAsync(new Location());

            // Act
            var result = await _favouritesService.GetUserFavouritesAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Single(result.Data);
            Assert.Equal(MockPlace.ID, result.Data.First().Id);
        }

        [Fact]
        public async Task SaveUserFavouriteAsync_CallsCorrectMethod()
        {
            // Arrange
            _secureStorageMock.Setup(s => s.GetAsync(Constants.USERNAME_KEY))
                .ReturnsAsync(USERNAME);

            // Act
            await _favouritesService.SaveUserFavouriteAsync(_placeDto);

            // Assert
            _localDatabaseMock.Verify(db => db.SavePlaceAsync(It.IsAny<PlaceDbEntry>()), Times.Once);
        }

        [Fact]
        public async Task DeleteUserFavouriteAsync_CallsCorrectMethod()
        {
            // Act
            await _favouritesService.DeleteUserFavouriteAsync(_placeDto);

            // Assert
            _localDatabaseMock.Verify(db => db.DeletePlaceAsync(_placeDto.Id), Times.Once);
        }

        [Fact]
        public async Task DeleteAllUserFavouritesAsync_CallsCorrectMethod()
        {
            // Act
            await _favouritesService.DeleteAllUserFavouritesAsync();

            // Assert
            _localDatabaseMock.Verify(db => db.DeleteAllPlacesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreatePlaceDbEntryAsync_CreatesCorrectPlaceDbEntry()
        {
            // Arrange
            _secureStorageMock.Setup(s => s.GetAsync(Constants.USERNAME_KEY))
                .ReturnsAsync(USERNAME);

            var method = typeof(FavouritesService).GetMethod("CreatePlaceDbEntryAsync",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Act
            var result = await (Task<PlaceDbEntry>)method?.Invoke(_favouritesService, [_placeDto])!;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(USERNAME, result.UserId);
            Assert.Equal(_placeDto.Id, result.PlaceId);
            Assert.Equal(JsonSerializer.Serialize(_placeDto), result.PlaceJson);
            Assert.True((DateTime.UtcNow - result.SavedAt).TotalSeconds < 5); 
        }

        [Fact]
        public async Task UpdateFavouritesAsync_UpdatesOutdatedFavorites()
        {
            // Arrange
            var outdatedPlace = new PlaceDto
            {
                Id = MockPlace.ID,
                LastUpdatedUtc = DateTime.UtcNow.AddDays(-2)
            };

            var updatedPlace = new PlaceDto
            {
                Id = MockPlace.ID,
                LastUpdatedUtc = DateTime.UtcNow
            };

            _secureStorageMock
                .Setup(s => s.GetAsync(Constants.USERNAME_KEY))
                .ReturnsAsync(USERNAME);

            _locationServiceMock
                .Setup(l => l.GetLocationWithTimeoutAsync())
                .ReturnsAsync(new Location());

            _googlePlacesServiceMock
                .Setup(g => g.GetPlaceDetailsAsync(outdatedPlace.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<PlaceDto>.Success(updatedPlace));

            _localDatabaseMock
                .Setup(db => db.SavePlaceAsync(It.IsAny<PlaceDbEntry>()))
                .Verifiable();

            List<PlaceDto> favourites = [outdatedPlace];

            // Act
            var result = await _favouritesService.UpdateFavouritesAsync(favourites);

            // Assert
            Assert.Single(result);
            Assert.Equal(updatedPlace.Id, result.First().Id);
            _localDatabaseMock.Verify(db => db.SavePlaceAsync(It.IsAny<PlaceDbEntry>()), Times.Once);
        }

        [Fact]
        public async Task UpdateFavouritesAsync_DoesNotUpdateRecentlyUpdatedFavorites()
        {
            // Arrange
            var recentPlace = new PlaceDto
            {
                Id = MockPlace.ID,
                LastUpdatedUtc = DateTime.UtcNow.AddHours(-12)
            };

            _locationServiceMock
                .Setup(l => l.GetLocationWithTimeoutAsync())
                .ReturnsAsync(new Location());

            List<PlaceDto> favourites = [recentPlace];

            // Act
            var result = await _favouritesService.UpdateFavouritesAsync(favourites);

            // Assert
            Assert.Single(result);
            Assert.Equal(recentPlace.Id, result.First().Id);
            _googlePlacesServiceMock.Verify(g => g.GetPlaceDetailsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UpdateFavouritesAsync_HandlesExceptionGracefully()
        {
            // Arrange
            var place = new PlaceDto
            {
                Id = MockPlace.ID,
                LastUpdatedUtc = DateTime.UtcNow.AddDays(-2)
            };

            _locationServiceMock
                .Setup(l => l.GetLocationWithTimeoutAsync())
                .ThrowsAsync(new Exception("Location service error"));

            List<PlaceDto> favourites = [place];

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _favouritesService.UpdateFavouritesAsync(favourites));
        }
    }
}
