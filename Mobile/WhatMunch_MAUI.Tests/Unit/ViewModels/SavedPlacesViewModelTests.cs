
using Microsoft.Extensions.Logging;
using Moq;
using WhatMunch_MAUI.Models.Dtos;
using WhatMunch_MAUI.Services;
using WhatMunch_MAUI.Utility;
using WhatMunch_MAUI.ViewModels;
using WhatMunch_MAUI.Views;

namespace WhatMunch_MAUI.Tests.Unit.ViewModels
{
    public class SavedPlacesViewModelTests
    {
        private readonly Mock<IFavouritesService> _favouritesServiceMock;
        private readonly Mock<IShellService> _shellServiceMock;
        private readonly Mock<ILogger<SavedPlacesViewModel>> _loggerMock;
        private readonly Mock<IMainThread> _mainThreadMock;
        private readonly Mock<IDjangoApiService> _djangoApiService;
        private readonly SavedPlacesViewModel _viewModel;

        public SavedPlacesViewModelTests()
        {
            _favouritesServiceMock = new();
            _shellServiceMock = new();
            _loggerMock = new();
            _mainThreadMock = new();
            _djangoApiService = new();

            _viewModel = new SavedPlacesViewModel(
                _favouritesServiceMock.Object,
                _shellServiceMock.Object,
                _loggerMock.Object,
                _mainThreadMock.Object,
                _djangoApiService.Object
            );
        }

        [Fact]
        public async Task LoadFavouritesAsync_ShouldLoadFavourites_WhenServiceReturnsData()
        {
            // Arrange
            var favourites = new List<PlaceDto>
            {
                new() { Id = "1", PrimaryType = "Restaurant" },
                new() { Id = "2", PrimaryType = "Restaurant" }
            };

            _favouritesServiceMock
                .Setup(s => s.GetUserFavouritesAsync())
                .ReturnsAsync(Result<List<PlaceDto>>.Success(favourites));

            _djangoApiService.Setup(m => m.GetGoogleMapsApiKeyAsync())
                .ReturnsAsync("apiKey");

            // Act
            await _viewModel.LoadFavouritesAsync();

            // Assert
            Assert.Equal(2, _viewModel.Favourites.Count);
            Assert.Equal("Restaurant", _viewModel.Favourites[0].PrimaryType);
        }

        [Fact]
        public async Task LoadFavouritesAsync_ShouldClearFavourites_WhenServiceReturnsFailure()
        {
            // Arrange
            _favouritesServiceMock
                .Setup(s => s.GetUserFavouritesAsync())
                .ReturnsAsync(Result<List<PlaceDto>>.Failure());

            _djangoApiService.Setup(m => m.GetGoogleMapsApiKeyAsync())
                .ReturnsAsync("apiKey");

            // Act
            await _viewModel.LoadFavouritesAsync();

            // Assert
            Assert.Empty(_viewModel.Favourites);
        }

        [Fact]
        public async Task LoadFavouritesAsync_ShouldLoadAndUpdateFavourites_WhenServiceReturnsData()
        {
            // Arrange
            List<PlaceDto> initialFavourites =
            [
                new() { Id = "1", PrimaryType = "Restaurant" },
                new() { Id = "2", PrimaryType = "Restaurant" }
            ];

            List<PlaceDto> updatedFavourites =
            [
                new() { Id = "1", PrimaryType = "Restaurant" },
                new() { Id = "2", PrimaryType = "Restaurant" },
                new() { Id = "3", PrimaryType = "Cafe" }
            ];

            _favouritesServiceMock
                .Setup(s => s.GetUserFavouritesAsync())
                .ReturnsAsync(Result<List<PlaceDto>>.Success(initialFavourites));

            _mainThreadMock
                .Setup(s => s.BeginInvokeOnMainThread(It.IsAny<Action>()))
                .Callback<Action>(action => action.Invoke());

            _favouritesServiceMock
                .Setup(s => s.UpdateFavouritesAsync(initialFavourites, It.IsAny<CancellationToken>()))
                .ReturnsAsync(updatedFavourites);

            // Act
            await _viewModel.LoadFavouritesAsync();
            await Task.Delay(1000); 

            // Assert
            Assert.Equal(3, _viewModel.Favourites.Count);
            Assert.Equal("Cafe", _viewModel.Favourites[2].PrimaryType);

            _favouritesServiceMock.Verify(s => s.UpdateFavouritesAsync(initialFavourites, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GoToPlaceDetails_ShouldNavigateToPlaceDetails_WhenPlaceIsValid()
        {
            // Arrange
            var place = new PlaceDto { Id = "1", PrimaryType = "Restaurant" };

            _djangoApiService.Setup(m => m.GetGoogleMapsApiKeyAsync())
                .ReturnsAsync("apiKey");

            // Act
            await _viewModel.GoToPlaceDetails(place);

            // Assert
            _shellServiceMock.Verify(s => s.GoToAsync(
                It.Is<string>(route => route.Contains(nameof(PlaceDetailsPage))),
                It.IsAny<Dictionary<string, object>>()
            ), Times.Once);
        }

        [Fact]
        public async Task DeleteFavouriteAsync_ShouldRemovePlace_WhenConfirmed()
        {
            // Arrange
            var place = new PlaceDto { Id = "1", PrimaryType = "Restaurant" };

            _viewModel.Favourites = [place];

            _shellServiceMock
                .Setup(s => s.CheckUserPrompt(It.IsAny<string>()))
                .ReturnsAsync(true);

            // Act
            await _viewModel.DeleteFavouriteAsync(place);

            // Assert
            Assert.Empty(_viewModel.Favourites);
            _favouritesServiceMock.Verify(s => s.DeleteUserFavouriteAsync(place), Times.Once);
        }

        [Fact]
        public async Task DeleteAllFavouritesAsync_ShouldClearAllFavourites_WhenConfirmed()
        {
            // Arrange
            var place = new PlaceDto { Id = "1", PrimaryType = "Restaurant" };
            _viewModel.Favourites = [place];

            _shellServiceMock
                .Setup(s => s.CheckUserPrompt(It.IsAny<string>()))
                .ReturnsAsync(true);

            // Act
            await _viewModel.DeleteAllFavouritesAsync();

            // Assert
            Assert.Empty(_viewModel.Favourites);
            _favouritesServiceMock.Verify(s => s.DeleteAllUserFavouritesAsync(), Times.Once);
        }
    }
}
