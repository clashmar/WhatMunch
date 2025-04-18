using Microsoft.Extensions.Logging;
using Moq;
using WhatMunch_MAUI.Models.Dtos;
using WhatMunch_MAUI.Resources.Localization;
using WhatMunch_MAUI.Services;
using WhatMunch_MAUI.Utility.Exceptions;
using WhatMunch_MAUI.ViewModels;
using WhatMunch_MAUI.Views;

namespace WhatMunch_MAUI.Tests.Unit.ViewModels
{
    public class DashboardViewModelTests
    {
        private readonly Mock<ISearchService> _searchServiceMock;
        private readonly Mock<IShellService> _shellServiceMock;
        private readonly Mock<ILogger<DashboardViewModel>> _loggerMock;
        private readonly DashboardViewModel _viewModel;

        public DashboardViewModelTests()
        {
            _searchServiceMock = new();
            _shellServiceMock = new();
            _loggerMock = new();

            _viewModel = new DashboardViewModel(
                _searchServiceMock.Object,
                _shellServiceMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task HandleSearch_ShouldNavigateToSearchResults_WhenPlacesAreFound()
        {
            // Arrange
            var mockResponse = new TextSearchResponseDto
            {
                Places = [new() { Id = "1" }],
                NextPageToken = "nextPageToken"
            };

            _searchServiceMock
                .Setup(s => s.GetSearchResponseAsync(null))
                .ReturnsAsync(mockResponse);

            // Act
            await _viewModel.HandleSearchCommand.ExecuteAsync(null);

            // Assert
            _shellServiceMock.Verify(s => s.GoToAsync(
                It.Is<string>(route => route.Contains(nameof(SearchResultsPage))),
                It.Is<Dictionary<string, object>>(parameters =>
                    parameters.ContainsKey("Places") &&
                    parameters.ContainsKey("NextPageToken"))), Times.Once);
        }

        [Fact]
        public async Task HandleSearch_ShouldDisplayError_WhenNoPlacesAreFound()
        {
            // Arrange
            var mockResponse = new TextSearchResponseDto
            {
                Places = [],
                NextPageToken = null
            };

            _searchServiceMock
                .Setup(s => s.GetSearchResponseAsync(null))
                .ReturnsAsync(mockResponse);

            // Act
            await _viewModel.HandleSearchCommand.ExecuteAsync(null);

            // Assert
            _shellServiceMock.Verify(s => s.DisplayError(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task HandleSearch_ShouldDisplayConnectivityError_WhenConnectivityExceptionIsThrown()
        {
            // Arrange
            _searchServiceMock
                .Setup(s => s.GetSearchResponseAsync(null))
                .ThrowsAsync(new ConnectivityException());

            // Act
            await _viewModel.HandleSearchCommand.ExecuteAsync(null);

            // Assert
            _shellServiceMock.Verify(s => s.DisplayError(AppResources.ErrorInternetConnection), Times.Once);
        }
    }
}
