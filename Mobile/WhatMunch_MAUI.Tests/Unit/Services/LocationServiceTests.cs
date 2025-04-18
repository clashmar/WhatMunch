using Microsoft.Extensions.Logging;
using Moq;
using WhatMunch_MAUI.Services;
using WhatMunch_MAUI.Utility.Exceptions;

namespace WhatMunch_MAUI.Tests.Unit.Services
{
    
    public class LocationServiceTests
    {
        private readonly Mock<IGeolocation> _mockGeolocation;
        private readonly Mock<IPermissionsService> _mockPermissionsService;
        private readonly Mock<ILogger<LocationService>> _mockLogger;
        private readonly LocationService _locationService;

        public LocationServiceTests()
        {
            _mockGeolocation = new();
            _mockPermissionsService = new();
            _mockLogger = new();

            _locationService = new LocationService(
                _mockGeolocation.Object,
                _mockPermissionsService.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task GetLocationWithTimeoutAsync_ReturnsLocation_WhenPermissionGranted()
        {
            // Arrange
            var expectedLocation = new Location(37.7749, -122.4194);

            _mockPermissionsService
                .Setup(p => p.CheckAndRequestLocationPermissionAsync())
                .ReturnsAsync(true);

            _mockGeolocation
                .Setup(g => g.GetLastKnownLocationAsync())
                .ReturnsAsync((Location?)null);

            _locationService.GetLocationAsyncMethod = (geo, req) =>
            {
                return Task.FromResult<Location?>(expectedLocation);
            };

            // Act
            var result = await _locationService.GetLocationWithTimeoutAsync();

            // Assert
            Assert.Equal(expectedLocation, result);
        }

        [Fact]
        public async Task GetLocationWithTimeoutAsync_ThrowsException_WhenPermissionDenied()
        {
            // Arrange
            _mockPermissionsService
                .Setup(p => p.CheckAndRequestLocationPermissionAsync())
                .ReturnsAsync(false);

            // Act 
            await Assert.ThrowsAsync<LocationException>(() => _locationService.GetLocationWithTimeoutAsync());
        }

        [Fact]
        public async Task GetLocationWithTimeoutAsync_ThrowsException_WhenLocationUnavailable()
        {
            // Arrange
            _mockPermissionsService
                .Setup(p => p.CheckAndRequestLocationPermissionAsync())
                .ReturnsAsync(true);

            _mockGeolocation
                .Setup(g => g.GetLastKnownLocationAsync())
                .ReturnsAsync((Location?)null);

            _locationService.GetLocationAsyncMethod = (geo, req) =>
            {
                return Task.FromResult<Location?>(null);
            };

            // Act & Assert
            await Assert.ThrowsAsync<LocationException>(() => _locationService.GetLocationWithTimeoutAsync());
        }

        [Fact]
        public async Task GetLastSearchLocation_ReturnsLastSearchLocation_WhenAvailable()
        {
            // Arrange
            var expectedLocation = new Location(37.7749, -122.4194); 

            _mockPermissionsService
                .Setup(p => p.CheckAndRequestLocationPermissionAsync())
                .ReturnsAsync(true);

            _mockGeolocation
                .Setup(g => g.GetLastKnownLocationAsync())
                .ReturnsAsync(expectedLocation);

            // Act
            var result = await _locationService.GetLastSearchLocation();

            // Assert
            Assert.Equal(expectedLocation, result);
        }

        [Fact]
        public async Task GetLastSearchLocation_CallsGetLocationWithTimeoutAsync_WhenLastSearchLocationIsNull()
        {
            // Arrange
            var expectedLocation = new Location(37.7749, -122.4194);

            _mockPermissionsService
                .Setup(p => p.CheckAndRequestLocationPermissionAsync())
                .ReturnsAsync(true);

            _mockGeolocation
                .Setup(g => g.GetLastKnownLocationAsync())
                .ReturnsAsync((Location?)null);

            _locationService.GetLocationAsyncMethod = (geo, req) =>
            {
                return Task.FromResult<Location?>(expectedLocation);
            };

            // Act
            var result = await _locationService.GetLastSearchLocation();

            // Assert
            Assert.Equal(expectedLocation, result);
        }
    }
}
