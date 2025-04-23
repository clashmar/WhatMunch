using Microsoft.Extensions.Logging;
using Moq;
using WhatMunch_MAUI.Models;
using WhatMunch_MAUI.Services;
using WhatMunch_MAUI.ViewModels;

namespace WhatMunch_MAUI.Tests.Unit.ViewModels
{
    public class SearchPreferencesViewModelTests
    {
        private readonly Mock<ISearchPreferencesService> _searchPreferencesServiceMock;
        private readonly Mock<ILogger<SearchPreferencesViewModel>> _loggerMock;
        private readonly Mock<IToastService> _toastServiceMock;
        private readonly SearchPreferencesViewModel _viewModel;

        public SearchPreferencesViewModelTests()
        {
            _searchPreferencesServiceMock = new();
            _loggerMock = new();
            _toastServiceMock = new();
            _viewModel = new(_searchPreferencesServiceMock.Object, _loggerMock.Object, _toastServiceMock.Object);
        }

        [Fact]
        public async Task HandleSavePreferences_SavesPreferencesSuccessfully()
        {
            // Arrange
            var preferences = SearchPreferencesModel.Default;
            _viewModel.Preferences = preferences;

            _searchPreferencesServiceMock
                .Setup(s => s.SavePreferencesAsync(preferences))
                .Returns(Task.CompletedTask);

            // Act
            await _viewModel.HandleSavePreferences();

            // Assert
            _searchPreferencesServiceMock.Verify(s => s.SavePreferencesAsync(preferences), Times.Once);
        }

        [Fact]
        public async Task LoadPreferencesAsync_LoadsPreferencesSuccessfully()
        {
            // Arrange
            var preferences = SearchPreferencesModel.Default;

            _searchPreferencesServiceMock
                .Setup(s => s.GetPreferencesAsync())
                .ReturnsAsync(preferences);

            // Act
            await _viewModel.LoadPreferencesAsync();

            // Assert
            Assert.Equal(preferences, _viewModel.Preferences);
            Assert.False(_viewModel.IsBusy);
        }
    }
}
