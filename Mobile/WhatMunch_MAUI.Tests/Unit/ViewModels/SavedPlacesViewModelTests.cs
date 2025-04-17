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
        private readonly SavedPlacesViewModel _viewModel;

        public SavedPlacesViewModelTests()
        {
            _favouritesServiceMock = new();
            _shellServiceMock = new();
            _loggerMock = new();

            _viewModel = new SavedPlacesViewModel(
                _favouritesServiceMock.Object,
                _shellServiceMock.Object,
                _loggerMock.Object
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

            // Act
            await _viewModel.LoadFavouritesAsync();

            // Assert
            Assert.Empty(_viewModel.Favourites);
        }

        [Fact]
        public async Task GoToPlaceDetails_ShouldNavigateToPlaceDetails_WhenPlaceIsValid()
        {
            // Arrange
            var place = new PlaceDto { Id = "1", PrimaryType = "Restaurant" };

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
