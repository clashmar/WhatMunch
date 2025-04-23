using Microsoft.Extensions.Logging;
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
        private readonly Mock<IAccountService> _accountServiceMock;
        private readonly Mock<IConnectivity> _connectivityMock;
        private readonly Mock<IShellService> _shellServiceMock;
        private readonly Mock<IToastService> _toastServiceMock;
        private readonly Mock<ILogger<RegistrationViewModel>> _loggerMock;
        private readonly RegistrationViewModel _viewModel;

        public RegistrationViewModelTests()
        {
            _accountServiceMock = new();
            _connectivityMock = new();
            _shellServiceMock = new();
            _toastServiceMock = new();
            _loggerMock = new();
            _viewModel = new RegistrationViewModel(
                _accountServiceMock.Object,
                _connectivityMock.Object, 
                _shellServiceMock.Object,
                _toastServiceMock.Object,
                _loggerMock.Object);
        }

        private readonly RegistrationModel registrationModel = new()
        {
            Email = "user@email.com",
            Username = "testuser",
            Password = "Password1",
            ConfirmPassword = "Password1"
        };

        private void SetupMocks()
        {
            _connectivityMock.Setup(c => c.NetworkAccess).Returns(NetworkAccess.Internet);
            _viewModel.RegistrationModel = registrationModel;
            _accountServiceMock.Setup(s => s.RegisterUserAsync(It.IsAny<RegistrationRequestDto>()))
                .ReturnsAsync(Result.Success());
        }

        [Fact]
        public async Task HandleRegistrationAsync_WithValidModel_NavigatesSuccessfully()
        {
            // Arrange
            SetupMocks();

            // Act
            await _viewModel.HandleRegistrationCommand.ExecuteAsync(null);

            // Assert
            _accountServiceMock.Verify(s => s.RegisterUserAsync(It.Is<RegistrationRequestDto>(
                dto => dto.Username == registrationModel.Username && dto.Password == registrationModel.Password)),
                Times.Once);
            _toastServiceMock.Verify(n => n.DisplayToast(It.IsAny<string>()), Times.Once);
            _shellServiceMock.Verify(n => n.GoToAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task HandleRegistrationAsync_WithNoInternet_DoesNotCallRegistrationService()
        {
            // Arrange
            _connectivityMock.Setup(c => c.NetworkAccess).Returns(NetworkAccess.None);
            _viewModel.RegistrationModel = registrationModel;

            // Act
            await _viewModel.HandleRegistrationCommand.ExecuteAsync(null);

            // Assert
            _accountServiceMock.Verify(s => s.RegisterUserAsync(It.IsAny<RegistrationRequestDto>()), Times.Never);
            _shellServiceMock.Verify(n => n.DisplayError(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task HandleRegistrationAsync_WithInvalidModel_DoesNotCallRegistrationService()
        {
            // Arrange
            _connectivityMock.Setup(c => c.NetworkAccess).Returns(NetworkAccess.Internet);
            _viewModel.RegistrationModel = new RegistrationModel { Email = "", Username = "", Password = "", ConfirmPassword= "" };

            // Act
            await _viewModel.HandleRegistrationCommand.ExecuteAsync(null);

            // Assert
            _accountServiceMock.Verify(s => s.RegisterUserAsync(It.IsAny<RegistrationRequestDto>()), Times.Never);
        }

        [Fact]
        public async Task HandleRegistrationAsync_WhenExceptionOccurs_DisplaysErrorAlert()
        {
            // Arrange
            SetupMocks();

            _accountServiceMock.Setup(s => s.RegisterUserAsync(It.IsAny<RegistrationRequestDto>()))
                .ThrowsAsync(new Exception(It.IsAny<string>()));

            // Act
            await _viewModel.HandleRegistrationCommand.ExecuteAsync(null);

            // Assert
            _shellServiceMock.Verify(n => n.DisplayError(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task HandleRegistrationAsync_SetsIsBusyCorrectly()
        {
            // Arrange
            SetupMocks();

            _accountServiceMock.Setup(s => s.RegisterUserAsync(It.IsAny<RegistrationRequestDto>()))
                .Returns(async () =>
                {
                    await Task.Delay(100);
                    return Result.Success();
                });

            // Act
            var registrationTask = _viewModel.HandleRegistrationCommand.ExecuteAsync(null);

            // Assert
            Assert.True(_viewModel.IsBusy);

            await registrationTask;

            Assert.False(_viewModel.IsBusy); 
        }

        [Fact]
        public async Task HandleSocialRegistrationAsync_WithSuccessfulRegistration_NavigatesSuccessfully()
        {
            // Arrange
            _connectivityMock.Setup(c => c.NetworkAccess).Returns(NetworkAccess.Internet);
            _accountServiceMock.Setup(s => s.LoginSocialUserAsync())
                .ReturnsAsync(Result.Success());

            // Act
            await _viewModel.HandleSocialRegistrationCommand.ExecuteAsync(null);

            // Assert
            _accountServiceMock.Verify(s => s.LoginSocialUserAsync(), Times.Once);
            _toastServiceMock.Verify(n => n.DisplayToast(It.IsAny<string>()), Times.Once);
            _shellServiceMock.Verify(n => n.GoToAsync(It.IsAny<string>()), Times.Once);
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
            _shellServiceMock.Verify(n => n.GoToAsync($"{nameof(LoginPage)}"), Times.Once);
        }
    }
}