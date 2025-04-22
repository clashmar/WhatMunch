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
        private readonly Mock<IAccountService> _loginServiceMock;
        private readonly Mock<IConnectivity> _connectivityMock;
        private readonly Mock<IShellService> _shellServiceMock;
        private readonly LoginViewModel _viewModel;

        public LoginViewModelTests()
        {
            _loginServiceMock = new();
            _connectivityMock = new();
            _shellServiceMock = new();
            _viewModel = new LoginViewModel(_loginServiceMock.Object, _connectivityMock.Object, _shellServiceMock.Object);
        }

        [Fact]
        public async Task HandleLoginAsync_WithValidModel_NavigatesSuccessfully()
        {
            // Arrange
            _connectivityMock.Setup(c => c.NetworkAccess).Returns(NetworkAccess.Internet);
            var loginModel = new LoginModel { Username = "testuser", Password = "Password1" };
            _viewModel.LoginModel = loginModel;

            _loginServiceMock.Setup(s => s.LoginUserAsync(It.IsAny<LoginRequestDto>()))
                .ReturnsAsync(Result<LoginResponseDto>.Success(new LoginResponseDto()));

            // Act
            await _viewModel.HandleLoginCommand.ExecuteAsync(null);

            // Assert
            _loginServiceMock.Verify(s => s.LoginUserAsync(It.Is<LoginRequestDto>(
                dto => dto.Username == loginModel.Username && dto.Password == loginModel.Password)),
                Times.Once);
            _shellServiceMock.Verify(n => n.DisplayAlert(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _shellServiceMock.Verify(n => n.GoToAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task HandleLoginAsync_WithNoInternet_DoesNotCallLoginService()
        {
            // Arrange
            _connectivityMock.Setup(c => c.NetworkAccess).Returns(NetworkAccess.None);
            var loginModel = new LoginModel { Username = "testuser", Password = "Password1" };
            _viewModel.LoginModel = loginModel;

            // Act
            await _viewModel.HandleLoginCommand.ExecuteAsync(null);

            // Assert
            _loginServiceMock.Verify(s => s.LoginUserAsync(It.IsAny<LoginRequestDto>()), Times.Never);
            _shellServiceMock.Verify(n => n.DisplayAlert(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task HandleLoginAsync_WithInvalidModel_DoesNotCallLoginService()
        {
            // Arrange
            _connectivityMock.Setup(c => c.NetworkAccess).Returns(NetworkAccess.Internet);
            _viewModel.LoginModel = new LoginModel { Username = "", Password = "" };

            // Act
            await _viewModel.HandleLoginCommand.ExecuteAsync(null);

            // Assert
            _loginServiceMock.Verify(s => s.LoginUserAsync(It.IsAny<LoginRequestDto>()), Times.Never);
            _shellServiceMock.Verify(n => n.DisplayAlert(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task HandleLoginAsync_SetsIsBusyCorrectly()
        {
            // Arrange
            _connectivityMock.Setup(c => c.NetworkAccess).Returns(NetworkAccess.Internet);
            var loginModel = new LoginModel { Username = "testuser", Password = "Password1" };
            _viewModel.LoginModel = loginModel;

            _loginServiceMock.Setup(s => s.LoginUserAsync(It.IsAny<LoginRequestDto>()))
                .Returns(async () =>
                {
                    await Task.Delay(100);
                    return Result<LoginResponseDto>.Success(new LoginResponseDto());
                });

            // Act
            var loginTask = _viewModel.HandleLoginCommand.ExecuteAsync(null);

            // Assert
            Assert.True(_viewModel.IsBusy);

            await loginTask;

            Assert.False(_viewModel.IsBusy);
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