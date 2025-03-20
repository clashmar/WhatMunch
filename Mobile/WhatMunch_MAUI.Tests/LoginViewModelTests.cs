using Moq;
using WhatMunch_MAUI.Dtos;
using WhatMunch_MAUI.Extensions;
using WhatMunch_MAUI.Models;
using WhatMunch_MAUI.Services;
using WhatMunch_MAUI.ViewModels;

namespace WhatMunch_MAUI.Tests
{
    public class LoginViewModelTests
    {
        private readonly Mock<ILoginService> _mockLoginService;
        private readonly Mock<IConnectivity> _mockConnectivity;
        private readonly LoginViewModel _viewModel;

        public LoginViewModelTests()
        {
            _mockLoginService = new Mock<ILoginService>();
            _mockConnectivity = new Mock<IConnectivity>();
            _viewModel = new LoginViewModel(_mockLoginService.Object, _mockConnectivity.Object);
        }

        [Fact]
        public async Task HandleLoginAsync_WithValidModel_CallsLoginService()
        {
            // Arrange
            _mockConnectivity.Setup(c => c.NetworkAccess).Returns(NetworkAccess.Internet);
            var loginModel = new LoginModel { Username = "testuser", Password = "password" };
            _viewModel.LoginModel = loginModel;

            // Act
            await _viewModel.HandleLoginCommand.ExecuteAsync(null);

            // Assert
            _mockLoginService.Verify(s => s.LoginUserAsync(It.Is<LoginRequestDto>(
                dto => dto.Username == loginModel.Username && dto.Password == loginModel.Password)),
                Times.Once);
        }
    }
}