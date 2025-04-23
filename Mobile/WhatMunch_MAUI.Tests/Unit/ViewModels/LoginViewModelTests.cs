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
    public class LoginViewModelTests
    {
        private readonly Mock<IAccountService> _accountServiceMock;
        private readonly Mock<IConnectivity> _connectivityMock;
        private readonly Mock<IShellService> _shellServiceMock;
        private readonly Mock<ILogger<LoginViewModel>> _loggerMock;
        private readonly Mock<IToastService> _toastServiceMock;
        private readonly LoginViewModel _viewModel;

        public LoginViewModelTests()
        {
            _accountServiceMock = new();
            _connectivityMock = new();
            _shellServiceMock = new();
            _loggerMock = new();
            _toastServiceMock = new();
            _viewModel = new LoginViewModel(
                _accountServiceMock.Object, 
                _connectivityMock.Object, 
                _shellServiceMock.Object,
                _loggerMock.Object,
                _toastServiceMock.Object);
        }

        private readonly LoginModel _loginModel = new()
        {
            Username = "testuser",
            Password = "Password1"
        };

        private void SetupMocks()
        {
            _connectivityMock.Setup(c => c.NetworkAccess).Returns(NetworkAccess.Internet);
            _viewModel.LoginModel = _loginModel;
        }

        [Fact]
        public async Task HandleUsernameLoginAsync_WithValidModel_NavigatesSuccessfully()
        {
            // Arrange
            SetupMocks();

            _accountServiceMock.Setup(s => s.LoginUserAsync(It.IsAny<LoginRequestDto>()))
                .ReturnsAsync(Result.Success());

            // Act
            await _viewModel.HandleUsernameLoginCommand.ExecuteAsync(null);

            // Assert
            _accountServiceMock.Verify(s => s.LoginUserAsync(It.Is<LoginRequestDto>(
                dto => dto.Username == _loginModel.Username && dto.Password == _loginModel.Password)),
                Times.Once);
            _toastServiceMock.Verify(n => n.DisplayToast(It.IsAny<string>()), Times.Once);
            _shellServiceMock.Verify(n => n.GoToAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task HandleLoginAsync_WithNoInternet_DoesNotCallLoginService()
        {
            // Arrange
            _connectivityMock.Setup(c => c.NetworkAccess).Returns(NetworkAccess.None);
            _viewModel.LoginModel = _loginModel;

            // Act
            await _viewModel.HandleUsernameLoginCommand.ExecuteAsync(null);

            // Assert
            _accountServiceMock.Verify(s => s.LoginUserAsync(It.IsAny<LoginRequestDto>()), Times.Never);
            _shellServiceMock.Verify(n => n.DisplayError(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task HandleLoginAsync_WithInvalidModel_DoesNotCallLoginService()
        {
            // Arrange
            _connectivityMock.Setup(c => c.NetworkAccess).Returns(NetworkAccess.Internet);
            _viewModel.LoginModel = new LoginModel { Username = "", Password = "" };

            // Act
            await _viewModel.HandleUsernameLoginCommand.ExecuteAsync(null);

            // Assert
            _accountServiceMock.Verify(s => s.LoginUserAsync(It.IsAny<LoginRequestDto>()), Times.Never);
        }

        [Fact]
        public async Task HandleLoginAsync_SetsIsBusyCorrectly()
        {
            // Arrange
            SetupMocks();

            _accountServiceMock.Setup(s => s.LoginUserAsync(It.IsAny<LoginRequestDto>()))
                .Returns(async () =>
                {
                    await Task.Delay(100);
                    return Result.Success();
                });

            // Act
            var loginTask = _viewModel.HandleUsernameLoginCommand.ExecuteAsync(null);

            // Assert
            Assert.True(_viewModel.IsBusy);

            await loginTask;

            Assert.False(_viewModel.IsBusy);
        }

        [Fact]
        public async Task HandleSocialLoginAsync_WithSuccessfulLogin_NavigatesSuccessfully()
        {
            // Arrange
            _connectivityMock.Setup(c => c.NetworkAccess).Returns(NetworkAccess.Internet);

            _accountServiceMock.Setup(s => s.LoginSocialUserAsync())
                .ReturnsAsync(Result.Success());

            // Act
            await _viewModel.HandleSocialLoginCommand.ExecuteAsync(null);

            // Assert
            _accountServiceMock.Verify(s => s.LoginSocialUserAsync(), Times.Once);
            _toastServiceMock.Verify(n => n.DisplayToast(It.IsAny<string>()), Times.Once);
            _shellServiceMock.Verify(n => n.GoToAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void ResetViewModel_ResetsProperties()
        {
            // Arrange
            _viewModel.LoginModel = new LoginModel { Username = "testuser", Password = "Password1" };
            _viewModel.IsBusy = true;
            _viewModel.ErrorOpacity = 1.0;

            // Act
            _viewModel.ResetViewModel();

            // Assert
            Assert.False(_viewModel.IsBusy);
            Assert.Equal(0, _viewModel.ErrorOpacity);
            Assert.NotNull(_viewModel.LoginModel);
            Assert.Equal("", _viewModel.LoginModel.Username);
            Assert.Equal("", _viewModel.LoginModel.Password);
        }

        [Fact]
        public async Task GoToRegistrationPageAsync_NavigatesToRegistrationPage()
        {
            // Act
            await _viewModel.GoToRegistrationPageCommand.ExecuteAsync(null);

            // Assert
            _shellServiceMock.Verify(n => n.GoToAsync(nameof(RegistrationPage)), Times.Once);
        }
    }
}