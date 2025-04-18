using Microsoft.Extensions.Logging;
using Moq;
using WhatMunch_MAUI.Models.Dtos;
using WhatMunch_MAUI.Services;
using WhatMunch_MAUI.ViewModels;
using WhatMunch_MAUI.Views;

namespace WhatMunch_MAUI.Tests.Unit.ViewModels
{
    public class SearchResultsViewModelTests
    {
        private readonly Mock<ISearchService> _searchServiceMock;
        private readonly Mock<IShellService> _shellServiceMock;
        private readonly Mock<IFavouritesService> _favouritesServiceMock;
        private readonly Mock<ILogger<SearchResultsViewModel>> _loggerMock;
        private readonly SearchResultsViewModel _viewModel;

        public SearchResultsViewModelTests()
        {
            _searchServiceMock = new();
            _shellServiceMock = new();
            _favouritesServiceMock = new();
            _loggerMock = new();

            _viewModel = new SearchResultsViewModel(
                _searchServiceMock.Object,
                _shellServiceMock.Object,
                _favouritesServiceMock.Object,
                _loggerMock.Object
            );
        }

        private const string TOKEN = "token";

        [Fact]
        public void InitializePageList_ShouldAddPlacesToPageList_WhenPlacesAreNotEmpty()
        {
            // Arrange
            _viewModel.Places =
            [
                new() { Id = "1", PrimaryType = "Restaurant" }
            ];

            // Act
            _viewModel.InitializePageList();

            // Assert
            Assert.Single(_viewModel.PageList);
            Assert.Equal(_viewModel.Places, _viewModel.PageList[0]);
        }

        [Fact]
        public async Task HandleRefresh_ShouldUpdatePlacesAndPageList_WhenSearchIsSuccessful()
        {
            // Arrange
            var mockResponse = new TextSearchResponseDto
            {
                Places = [new() { Id = "1" }, new() { Id = "2" }],
                NextPageToken = TOKEN
            };

            _searchServiceMock
                .Setup(s => s.GetSearchResponseAsync(null))
                .ReturnsAsync(mockResponse);

            // Act
            await _viewModel.HandleRefreshCommand.ExecuteAsync(null);

            // Assert
            Assert.Equal(mockResponse.Places.Count, _viewModel.Places.Count);
            Assert.Single(_viewModel.PageList);
            Assert.Equal(mockResponse.NextPageToken, _viewModel.NextPageToken);
        }

        [Fact]
        public async Task HandleNext_ShouldLoadNextPage_WhenNextPageExists()
        {
            // Arrange
            _viewModel.PageList.Add(
            [
                new PlaceDto { Id = "1", PrimaryType = "Restaurant" }
            ]);

            var mockResponse = new TextSearchResponseDto
            {
                Places = [new() { Id = "2" }, new() { Id = "2" }],
                NextPageToken = TOKEN
            };

            _viewModel.NextPageToken = TOKEN;

            _searchServiceMock
                .Setup(s => s.GetSearchResponseAsync(TOKEN))
                .ReturnsAsync(mockResponse);

            // Act
            await _viewModel.HandleNextCommand.ExecuteAsync(null);

            // Assert
            Assert.Equal(mockResponse.Places.Count, _viewModel.Places.Count);
            Assert.Equal(2, _viewModel.PageList.Count);
            Assert.Equal(mockResponse.NextPageToken, _viewModel.NextPageToken);
        }

        [Fact]
        public void HandlePrevious_ShouldNavigateToPreviousPage_WhenPreviousPageExists()
        {
            // Arrange
            _viewModel.PageList.Add(
            [
                new PlaceDto { Id = "1" }
            ]);

            _viewModel.PageList.Add(
            [
                new PlaceDto { Id = "2" }
            ]);

            _viewModel.Places = _viewModel.PageList[1];
            _viewModel.HasPreviousPage = true;
            _viewModel.CurrentPageIndex = 1;

            // Act
            _viewModel.HandlePreviousCommand.Execute(null);

            // Assert
            Assert.Equivalent(_viewModel.PageList[0], _viewModel.Places);
            Assert.False(_viewModel.HasPreviousPage);
        }

        [Fact]
        public async Task GoToPlaceDetails_ShouldNavigateToPlaceDetailsPage_WhenPlaceIsNotNull()
        {
            // Arrange
            var place = new PlaceDto { Id = "1" };

            // Act
            await _viewModel.GoToPlaceDetailsCommand.ExecuteAsync(place);

            // Assert
            _shellServiceMock.Verify(s => s.GoToAsync(
                It.Is<string>(route => route.Contains($"{nameof(PlaceDetailsPage)}")),
                It.IsAny<Dictionary<string, object>>()), Times.Once);
        }

        [Fact]
        public async Task ToggleFavouriteAsync_ShouldSaveOrDeleteFavorite_WhenPlaceIsToggled()
        {
            // Arrange
            var place = new PlaceDto { Id = "1", IsFavourite = false };

            // Act
            await _viewModel.ToggleFavouriteCommand.ExecuteAsync(place);

            // Assert
            Assert.True(place.IsFavourite);
            _favouritesServiceMock.Verify(f => f.SaveUserFavouriteAsync(place), Times.Once);

            // Act (toggle back)
            await _viewModel.ToggleFavouriteCommand.ExecuteAsync(place);

            // Assert
            Assert.False(place.IsFavourite);
            _favouritesServiceMock.Verify(f => f.DeleteUserFavouriteAsync(place), Times.Once);
        }

        [Fact]
        public async Task GoBackAsync_ShouldNavigateBack()
        {
            // Act
            await _viewModel.GoBackCommand.ExecuteAsync(null);

            // Assert
            _shellServiceMock.Verify(s => s.GoToAsync(".."), Times.Once);
        }
    }
}
