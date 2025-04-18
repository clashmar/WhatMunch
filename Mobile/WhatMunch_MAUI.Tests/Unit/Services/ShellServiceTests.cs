using Moq;
using WhatMunch_MAUI.Resources.Localization;
using WhatMunch_MAUI.Services;

namespace WhatMunch_MAUI.Tests.Unit.Services
{
    public class ShellServiceTests
    {
        private readonly ShellService _shellService;
        private readonly Mock<Func<string, Task>> _mockGoToAsyncMethod;
        private readonly Mock<Func<string, Dictionary<string, object>, Task>> _mockGoToAsyncWithParamsMethod;
        private readonly Mock<Func<string, string, string, Task>> _mockDisplayAlertMethod;
        private readonly Mock<Func<string, Task<bool>>> _mockCheckUserPromptMethod;

        public ShellServiceTests()
        {
            _mockGoToAsyncMethod = new();
            _mockGoToAsyncWithParamsMethod = new();
            _mockDisplayAlertMethod = new();
            _mockCheckUserPromptMethod = new();

            _shellService = new ShellService
            {
                GoToAsyncMethod = _mockGoToAsyncMethod.Object,
                GoToAsyncWithParamsMethod = _mockGoToAsyncWithParamsMethod.Object,
                DisplayAlertMethod = _mockDisplayAlertMethod.Object,
                CheckUserPromptMethod = _mockCheckUserPromptMethod.Object
            };
        }

        private const string ROUTE = "route";
        private const string MESSAGE = "message";

        [Fact]
        public async Task GoToAsync_Should_Invoke_GoToAsyncMethod()
        {
            // Act
            await _shellService.GoToAsync(ROUTE);

            // Assert
            _mockGoToAsyncMethod.Verify(m => m(ROUTE), Times.Once);
        }

        [Fact]
        public async Task GoToAsync_WithParams_Should_Invoke_GoToAsyncWithParamsMethod()
        {
            // Arrange
            var parameters = new Dictionary<string, object> { { "key", "value" } };

            // Act
            await _shellService.GoToAsync(ROUTE, parameters);

            // Assert
            _mockGoToAsyncWithParamsMethod.Verify(m => m(ROUTE, parameters), Times.Once);
        }

        [Fact]
        public async Task DisplayAlert_Should_Invoke_DisplayAlertMethod()
        {
            // Arrange
            var title = "Test Title";
            var accept = "OK";

            // Act
            await _shellService.DisplayAlert(title, MESSAGE, accept);

            // Assert
            _mockDisplayAlertMethod.Verify(m => m(title, MESSAGE, accept), Times.Once);
        }

        [Fact]
        public async Task DisplayError_Should_Invoke_DisplayAlertMethod_With_ErrorTitle()
        {
            // Act
            await _shellService.DisplayError(MESSAGE);

            // Assert
            _mockDisplayAlertMethod.Verify(m => m(AppResources.Error, MESSAGE, AppResources.Ok), Times.Once);
        }

        [Fact]
        public async Task CheckUserPrompt_Should_Invoke_CheckUserPromptMethod()
        {
            // Arrange
            _mockCheckUserPromptMethod.Setup(m => m(MESSAGE)).ReturnsAsync(true);

            // Act
            var result = await _shellService.CheckUserPrompt(MESSAGE);

            // Assert
            Assert.True(result);
            _mockCheckUserPromptMethod.Verify(m => m(MESSAGE), Times.Once);
        }
    }
}
