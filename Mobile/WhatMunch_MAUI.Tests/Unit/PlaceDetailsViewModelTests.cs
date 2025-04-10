using Microsoft.Extensions.Logging;
using Moq;
using System;
using WhatMunch_MAUI.Models;
using WhatMunch_MAUI.ViewModels;

namespace WhatMunch_MAUI.Tests.Unit
{
    public class PlaceDetailsViewModelTests
    {
        private readonly Mock<ILauncher> _launcherMock;
        private readonly Mock<ILogger<PlaceDetailsViewModel>> _loggerMock;
        private readonly PlaceDetailsViewModel _viewModel;

        private const string VALID_PHONE_NUMBER = "123456789";
        private const string InvalidPhoneNumber = "";

        public PlaceDetailsViewModelTests()
        {
            _launcherMock = new();
            _loggerMock = new();
            _viewModel = new PlaceDetailsViewModel(_launcherMock.Object, _loggerMock.Object);
        }

        [Theory]
        [InlineData("https://www.example.com")]
        [InlineData("https://example.com")]
        [InlineData("www.example.com")]
        public async Task GoToWebsite_ValidUrl_OpensUrl(string url)
        {
            // Arrange
            url = url.StartsWith("http", StringComparison.OrdinalIgnoreCase) ? url : $"https://{url}";
            var uri = new Uri(url);

            _launcherMock
                .Setup(l => l.OpenAsync(It.IsAny<Uri>()))
                .ReturnsAsync(true);

            // Act
            await _viewModel.GoToWebsiteCommand.ExecuteAsync(url);

            // Assert
            _launcherMock.Verify(
                l => l.OpenAsync(uri),
                Times.Once
            );
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task GoToWebsite_NoUrl_Returns(string? url)
        {
            // Act
            await _viewModel.GoToWebsiteCommand.ExecuteAsync(url);

            // Assert
            _launcherMock.Verify(
                l => l.OpenAsync(It.IsAny<Uri>()),
                Times.Never
            );
        }

        [Fact]
        public async Task GoToPhone_ValidNumber_OpensPhoneUri()
        {
            // Arrange
            var uri = new Uri($"tel:{VALID_PHONE_NUMBER}");
            _launcherMock
                .Setup(launcher => launcher.OpenAsync(It.IsAny<Uri>()))
                .ReturnsAsync(true);

            // Act
            await _viewModel.GoToPhoneCommand.ExecuteAsync(VALID_PHONE_NUMBER);

            // Assert
            _launcherMock.Verify(
                launcher => launcher.OpenAsync(uri),
                Times.Once
            );
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task GoToPhone_NpNumber_Returns(string? number)
        {
            // Act
            await _viewModel.GoToPhoneCommand.ExecuteAsync(number);

            // Assert
            _launcherMock.Verify(
                l => l.OpenAsync(It.IsAny<Uri>()),
                Times.Never
            );
        }

        [Fact]
        public async Task GoToMap_ValidPlaceId_OpensMapUri()
        {
            // Arrange
            var placeId = "valid-place-id";
            _viewModel.Place = new PlaceModel { Id = placeId };
            var uri = new Uri($"https://www.google.com/maps/place/?q=place_id:{placeId}");
            _launcherMock
                .Setup(l => l.OpenAsync(It.IsAny<Uri>()))
                .ReturnsAsync(true);

            // Act
            await _viewModel.GoToMapCommand.ExecuteAsync(null);

            // Assert
            _launcherMock.Verify(
                l => l.OpenAsync(uri),
                Times.Once
            );
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task GoToMap_NoPlaceId_Returns(string? id)
        {
            // Arrange
            _viewModel.Place = new PlaceModel { Id = id! };

            // Act
            await _viewModel.GoToMapCommand.ExecuteAsync(null);

            // Assert
            _launcherMock.Verify(
                l => l.OpenAsync(It.IsAny<Uri>()),
                Times.Never
            );
        }
    }
}
