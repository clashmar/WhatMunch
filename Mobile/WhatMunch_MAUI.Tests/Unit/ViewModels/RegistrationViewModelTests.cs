using Moq;
using WhatMunch_MAUI.Models;
using WhatMunch_MAUI.Models.Dtos;
using WhatMunch_MAUI.Services;
using WhatMunch_MAUI.Utility;
using WhatMunch_MAUI.ViewModels;
using WhatMunch_MAUI.Views;

namespace WhatMunch_MAUI.Tests.Unit.ViewModels
{
    public class RegistrationViewModelTests
    {
        private readonly Mock<IRegistrationService> _mockRegistrationService;
        private readonly Mock<IConnectivity> _mockConnectivity;
        private readonly Mock<IShellService> _mockShellService;
        private readonly RegistrationViewModel _viewModel;

        public RegistrationViewModelTests()
        {
            _mockRegistrationService = new Mock<IRegistrationService>();
            _mockConnectivity = new Mock<IConnectivity>();
            _mockShellService = new Mock<IShellService>();
            _viewModel = new RegistrationViewModel(_mockRegistrationService.Object, _mockConnectivity.Object, _mockShellService.Object);
        }

        [Fact]
        public async Task HandleRegistrationAsync_WithValidModel_NavigatesSuccessfully()
        {
            // Arrange
            _mockConnectivity.Setup(c => c.NetworkAccess).Returns(NetworkAccess.Internet);
            var registrationModel = new RegistrationModel { Email = "user@email.com", Username = "testuser", Password = "Password1", ConfirmPassword = "Password1" };
            _viewModel.RegistrationModel = registrationModel;

            _mockRegistrationService.Setup(s => s.RegisterUserAsync(It.IsAny<RegistrationRequestDto>()))
                .ReturnsAsync(Result<string>.Success("Success"));

            // Act
            await _viewModel.HandleRegistrationCommand.ExecuteAsync(null);

            // Assert
            _mockRegistrationService.Verify(s => s.RegisterUserAsync(It.Is<RegistrationRequestDto>(
                dto => dto.Username == registrationModel.Username && dto.Password == registrationModel.Password)),
                Times.Once);
            _mockShellService.Verify(n => n.DisplayAlert(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _mockShellService.Verify(n => n.GoToAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task HandleRegistrationAsync_WithNoInternet_DoesNotCallRegistrationService()
        {
            // Arrange
            _mockConnectivity.Setup(c => c.NetworkAccess).Returns(NetworkAccess.None);
            var registrationModel = new RegistrationModel { Email = "user@email.com", Username = "testuser", Password = "Password1", ConfirmPassword = "Password1" };
            _viewModel.RegistrationModel = registrationModel;

            // Act
            await _viewModel.HandleRegistrationCommand.ExecuteAsync(null);

            // Assert
            _mockRegistrationService.Verify(s => s.RegisterUserAsync(It.IsAny<RegistrationRequestDto>()), Times.Never);
            _mockShellService.Verify(n => n.DisplayAlert(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task HandleRegistrationAsync_WithInvalidModel_DoesNotCallRegistrationService()
        {
            // Arrange
            _mockConnectivity.Setup(c => c.NetworkAccess).Returns(NetworkAccess.Internet);
            _viewModel.RegistrationModel = new RegistrationModel { Email = "", Username = "", Password = "", ConfirmPassword= "" };

            // Act
            await _viewModel.HandleRegistrationCommand.ExecuteAsync(null);

            // Assert
            _mockRegistrationService.Verify(s => s.RegisterUserAsync(It.IsAny<RegistrationRequestDto>()), Times.Never);
            _mockShellService.Verify(n => n.DisplayAlert(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task HandleRegistrationAsync_WhenExceptionOccurs_DisplaysErrorAlert()
        {
            // Arrange
            _mockConnectivity.Setup(c => c.NetworkAccess).Returns(NetworkAccess.Internet);
            var registrationModel = new RegistrationModel { Email = "user@email.com", Username = "testuser", Password = "Password1", ConfirmPassword = "Password1" };
            _viewModel.RegistrationModel = registrationModel;

            _mockRegistrationService.Setup(s => s.RegisterUserAsync(It.IsAny<RegistrationRequestDto>()))
                .ThrowsAsync(new Exception(It.IsAny<string>()));

            // Act
            await _viewModel.HandleRegistrationCommand.ExecuteAsync(null);

            // Assert
            _mockShellService.Verify(n => n.DisplayAlert(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task HandleRegistrationAsync_SetsIsBusyCorrectly()
        {
            // Arrange
            _mockConnectivity.Setup(c => c.NetworkAccess).Returns(NetworkAccess.Internet);
            var registrationModel = new RegistrationModel {Email = "user@email.com", Username = "testuser", Password = "Password1", ConfirmPassword = "Password1" };
            _viewModel.RegistrationModel = registrationModel;

            _mockRegistrationService.Setup(s => s.RegisterUserAsync(It.IsAny<RegistrationRequestDto>()))
                .Returns(async () =>
                {
                    await Task.Delay(100);
                    return Result<string>.Success("Success");
                });

            // Act
            var registrationTask = _viewModel.HandleRegistrationCommand.ExecuteAsync(null);

            // Assert
            Assert.True(_viewModel.IsBusy);

            await registrationTask;

            Assert.False(_viewModel.IsBusy); 
        }

        [Fact]
        public void ResetViewModel_ResetsProperties()
        {
            // Arrange
            _viewModel.RegistrationModel = new RegistrationModel { Email = "user@email.com", Username = "testuser", Password = "Password1", ConfirmPassword = "Password1" };
            _viewModel.IsBusy = true;
            _viewModel.ErrorOpacity = 1.0;

            // Act
            _viewModel.ResetViewModel();

            // Assert
            Assert.False(_viewModel.IsBusy);
            Assert.Equal(0, _viewModel.ErrorOpacity);
            Assert.NotNull(_viewModel.RegistrationModel);
            Assert.Equal("", _viewModel.RegistrationModel.Username);
            Assert.Equal("", _viewModel.RegistrationModel.Password);
        }

        [Fact]
        public async Task GoToLoginPageAsync_NavigatesToLoginPage()
        {
            // Act
            await _viewModel.GoToLoginPageCommand.ExecuteAsync(null);

            // Assert
            _mockShellService.Verify(n => n.GoToAsync($"{nameof(LoginPage)}"), Times.Once);
        }
    }
}