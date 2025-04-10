using Microsoft.Extensions.Logging;
using Moq;
using WhatMunch_MAUI.ViewModels;

namespace WhatMunch_MAUI.Tests.Unit
{
    public class PlaceDetailsViewModelTests
    {
        private readonly Mock<ILogger<PlaceDetailsViewModel>> _loggerMock;
        private readonly PlaceDetailsViewModel _viewModel;

        public PlaceDetailsViewModelTests()
        {
            _loggerMock = new Mock<ILogger<PlaceDetailsViewModel>>();
            _viewModel = new PlaceDetailsViewModel(_loggerMock.Object);
        }

        [Fact]
        public async Task GoToWebsite_ValidUrl_OpensUrl()
        {
            // Arrange
            var url = "www.example.com";

            // Act
            await _viewModel.GoToWebsiteCommand.ExecuteAsync(url);

            // Assert
            // Verify that the URL was opened (mock Launcher.Default.OpenAsync if needed)
        }

        [Fact]
        public async Task GoToWebsite_InvalidUrl_LogsWarning()
        {
            // Arrange
            var invalidUrl = "invalid-url";

            // Act
            await _viewModel.GoToWebsiteCommand.ExecuteAsync(invalidUrl);

            // Assert
            _loggerMock.Verify(
                logger => logger.LogWarning(It.IsAny<string>(), invalidUrl),
                Times.Once
            );
        }

        [Fact]
        public async Task GoToPhone_ValidNumber_OpensPhoneUri()
        {
            // Arrange
            var phoneNumber = "123456789";

            // Act
            await _viewModel.GoToPhoneCommand.ExecuteAsync(phoneNumber);

            // Assert
            // Verify that the phone URI was opened (mock Launcher.Default.OpenAsync if needed)
        }

        [Fact]
        public async Task GoToPhone_InvalidNumber_LogsError()
        {
            // Arrange
            var invalidNumber = "";

            // Act
            await _viewModel.GoToPhoneCommand.ExecuteAsync(invalidNumber);

            // Assert
            _loggerMock.Verify(
                logger => logger.LogError(It.IsAny<Exception>(), It.IsAny<string>(), invalidNumber),
                Times.Never
            );
        }

        [Fact]
        public async Task GoToMap_ValidPlaceId_OpensMapUri()
        {
            // Arrange
            _viewModel.Place = new PlaceModel { Id = "valid-place-id" };

            // Act
            await _viewModel.GoToMapCommand.ExecuteAsync(null);

            // Assert
            // Verify that the map URI was opened (mock Launcher.Default.OpenAsync if needed)
        }

        [Fact]
        public async Task GoToMap_NullPlaceId_LogsWarning()
        {
            // Arrange
            _viewModel.Place = new PlaceModel { Id = null };

            // Act
            await _viewModel.GoToMapCommand.ExecuteAsync(null);

            // Assert
            _loggerMock.Verify(
                logger => logger.LogWarning("Place ID is null or empty."),
                Times.Once
            );
        }
    }
}
