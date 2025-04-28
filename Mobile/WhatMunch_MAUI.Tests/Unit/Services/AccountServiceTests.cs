using Microsoft.Extensions.Logging;
using Moq;
using RichardSzalay.MockHttp;
using System.Net;
using System.Text.Json;
using WhatMunch_MAUI.Models.Dtos;
using WhatMunch_MAUI.Services;

namespace WhatMunch_MAUI.Tests.Unit.Services
{
    public class AccountServiceTests
    {
        private readonly Mock<IHttpClientFactory> _clientFactoryMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<ISecureStorage> _secureStorageMock;
        private readonly Mock<ILogger<AccountService>> _loggerMock;
        private readonly Mock<IWebAuthenticator> _webAuthenticatorMock;
        private readonly Mock<IShellService> _shellServiceMock;
        private readonly MockHttpMessageHandler _handlerMock;
        private readonly AccountService _accountService;

        public AccountServiceTests()
        {
            _clientFactoryMock = new();
            _tokenServiceMock = new();
            _secureStorageMock = new();
            _loggerMock = new();
            _webAuthenticatorMock = new();
            _shellServiceMock = new();
            _handlerMock = new();

            _accountService = new AccountService(
                _clientFactoryMock.Object,
                _tokenServiceMock.Object,
                _secureStorageMock.Object,
                _loggerMock.Object,
                _webAuthenticatorMock.Object,
                _shellServiceMock.Object
            );
        }

        private readonly RegistrationRequestDto _registrationRequest = new()
        {
            Email = "test@example.com",
            Username = "testuser",
            Password = "password123"
        };

        private readonly LoginRequestDto _loginRequest = new()
        {
            Username = "testuser",
            Password = "password123"
        };

        private readonly string _errorMessage = "Invalid data.";

        [Fact]
        public async Task RegisterUserAsync_ShouldReturnSuccess_WhenRegistrationSucceeds()
        {
            // Arrange
            _handlerMock.When(
                $"http://10.0.2.2:8000/api/*")
                .Respond(HttpStatusCode.OK);

            _clientFactoryMock.Setup(x => x.CreateClient("WhatMunch"))
                .Returns(new HttpClient(_handlerMock)
                {
                    BaseAddress = new Uri("http://10.0.2.2:8000/api/")
                });

            // Act
            var result = await _accountService.RegisterUserAsync(_registrationRequest);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldReturnFailure_WhenRegistrationFails()
        {
            // Arrange
            var errorResponse = new ErrorMessageDto { Detail = _errorMessage };
            var jsonError = JsonSerializer.Serialize(errorResponse);

            _handlerMock.When(
                $"http://10.0.2.2:8000/api/*")
                .Respond(HttpStatusCode.BadRequest,"application/json", jsonError);

            _clientFactoryMock.Setup(x => x.CreateClient("WhatMunch"))
                .Returns(new HttpClient(_handlerMock)
                {
                    BaseAddress = new Uri("http://10.0.2.2:8000/api/")
                });

            // Act
            var result = await _accountService.RegisterUserAsync(_registrationRequest);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains(_errorMessage, result.ErrorMessage);
        }

        [Fact]
        public async Task LoginUserAsync_ShouldReturnSuccess_WhenLoginSucceeds()
        {
            // Arrange
            var loginResponse = new LoginResponseDto
            {
                AccessToken = "access-token",
                RefreshToken = "refresh-token"
            };
            var jsonResponse = JsonSerializer.Serialize(loginResponse);

            _handlerMock.When(
                $"http://10.0.2.2:8000/api/*")
                .Respond(HttpStatusCode.OK, "application/json", jsonResponse);

            _clientFactoryMock.Setup(x => x.CreateClient("WhatMunch"))
                .Returns(new HttpClient(_handlerMock)
                {
                    BaseAddress = new Uri("http://10.0.2.2:8000/api/")
                });

            // Act
            var result = await _accountService.LoginUserAsync(_loginRequest);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task LoginUserAsync_ShouldReturnFailure_WhenLoginFails()
        {
            // Arrange
            var errorResponse = new ErrorMessageDto { Detail = _errorMessage };
            var jsonError = JsonSerializer.Serialize(errorResponse);

            _handlerMock.When(
                $"http://10.0.2.2:8000/api/*")
                .Respond(HttpStatusCode.NotFound, "application/json", jsonError);

            _clientFactoryMock.Setup(x => x.CreateClient("WhatMunch"))
                .Returns(new HttpClient(_handlerMock)
                {
                    BaseAddress = new Uri("http://10.0.2.2:8000/api/")
                });

            // Act
            var result = await _accountService.LoginUserAsync(_loginRequest);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains(_errorMessage, result.ErrorMessage);
        }

        [Fact]
        public async Task LoginSocialUserAsync_ShouldReturnSuccess_WhenSocialLoginSucceeds()
        {
            // Arrange
            WebAuthenticatorResult authResult = new()
            {
                Properties = 
                { 
                    { "access_token", "access-token" },
                    { "refresh_token", "refresh-token" },
                    { "email", "testuser@example.com" } 
                }
            };

            _webAuthenticatorMock
                .Setup(m => m.AuthenticateAsync(It.IsAny<WebAuthenticatorOptions>()))
                .ReturnsAsync(authResult);

            // Act
            var result = await _accountService.LoginSocialUserAsync();

            // Assert
            Assert.True(result.IsSuccess);
        }
    }
}
