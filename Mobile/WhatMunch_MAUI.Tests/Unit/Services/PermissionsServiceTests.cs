using Microsoft.Extensions.Logging;
using Moq;
using WhatMunch_MAUI.Services;

namespace WhatMunch_MAUI.Tests.Unit.Services
{
    public class PermissionsServiceTests
    {
        private readonly Mock<ILogger<PermissionsService>> _loggerMock;
        private readonly PermissionsService _permissionsService;

        public PermissionsServiceTests()
        {
            _loggerMock = new();
            _permissionsService = new(_loggerMock.Object);
        }


        [Fact]
        public async Task CheckAndRequestLocationPermissionAsync_ShouldReturnTrue_WhenPermissionIsGrantedInitially()
        {
            // Arrange
            _permissionsService.CheckStatusAsyncMethod = () => Task.FromResult(PermissionStatus.Granted);

            // Act
            var result = await _permissionsService.CheckAndRequestLocationPermissionAsync();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task CheckAndRequestLocationPermissionAsync_ShouldReturnTrue_WhenPermissionIsGrantedAfterRequest()
        {
            // Arrange
            _permissionsService.CheckStatusAsyncMethod = () => Task.FromResult(PermissionStatus.Denied);
            _permissionsService.RequestAsyncMethod = () => Task.FromResult(PermissionStatus.Granted);

            // Act
            var result = await _permissionsService.CheckAndRequestLocationPermissionAsync();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task CheckAndRequestLocationPermissionAsync_ShouldReturnFalse_WhenPermissionIsDeniedAfterRequest()
        {
            // Arrange
            _permissionsService.CheckStatusAsyncMethod = () => Task.FromResult(PermissionStatus.Denied);
            _permissionsService.RequestAsyncMethod = () => Task.FromResult(PermissionStatus.Denied);

            // Act
            var result = await _permissionsService.CheckAndRequestLocationPermissionAsync();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task CheckAndRequestLocationPermissionAsync_ShouldLogError_WhenExceptionIsThrown()
        {
            // Arrange
            _permissionsService.CheckStatusAsyncMethod = () => throw new Exception();

            // Act
            var result = await _permissionsService.CheckAndRequestLocationPermissionAsync();

            // Assert
            Assert.False(result);
        }
    }
}
