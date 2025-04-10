﻿using Microsoft.Extensions.Logging;
using Moq;
using WhatMunch_MAUI.Models;
using WhatMunch_MAUI.Models.Dtos;
using WhatMunch_MAUI.Models.Places;
using WhatMunch_MAUI.Services;
using WhatMunch_MAUI.Utility;

namespace WhatMunch_MAUI.Tests.Unit
{
    public class SearchServiceTests
    {
        private readonly Mock<IShellService> _shellServiceMock;
        private readonly Mock<ILogger<SearchService>> _loggerMock;
        private readonly Mock<IGooglePlacesService> _googlePlacesServiceMock;
        private readonly Mock<IConnectivity> _connectivityMock;
        private readonly Mock<ISearchPreferencesService> _searchPreferencesServiceMock;
        private readonly SearchService _service;

        public SearchServiceTests()
        {
            _shellServiceMock = new();
            _loggerMock = new();
            _googlePlacesServiceMock = new();
            _connectivityMock = new();
            _searchPreferencesServiceMock = new();

            _service = new(
                _shellServiceMock.Object,
                _loggerMock.Object,
                _googlePlacesServiceMock.Object,
                _connectivityMock.Object,
                _searchPreferencesServiceMock.Object);
        }

        [Fact]
        public async Task GetSearchResponseAsync_ReturnsCorrectlData()
        {
            // Arrange
            _connectivityMock.Setup(c => c.NetworkAccess).Returns(NetworkAccess.Internet);

            var testPreferences = new SearchPreferencesModel()
            {
                MinRating = 3,
                MaxPriceLevel = PriceLevel.PRICE_LEVEL_MODERATE,
                IsChildFriendly = true,
                IsDogFriendly = true
            };
            _searchPreferencesServiceMock.Setup(c => c.GetPreferencesAsync()).ReturnsAsync(testPreferences);

            var searchResult = Result<TextSearchResponseDto>.Success(new TextSearchResponseDto() { Places = MockPlacesList });
            _googlePlacesServiceMock.Setup(c => c.GetNearbySearchResultsAsync(testPreferences)).ReturnsAsync(searchResult);

            // Act
            var result = await _service.GetSearchResponseAsync();

            // Assert
            Assert.Equivalent(result.Places, MockPlacesList);
        }

        private readonly List<PlaceDto> MockPlacesList = [
        new PlaceDto()
        {
            DisplayName = new DisplayName { Text = "Central Park Coffee", LanguageCode = "en" },
            PrimaryType = "cafe",
            Types = ["cafe", "coffee_shop"],
            Rating = 4.5,
            UserRatingCount = 530,
            PriceLevel = PriceLevel.PRICE_LEVEL_INEXPENSIVE,
            RegularOpeningHours = new RegularOpeningHours { OpenNow = true },
            GoodForChildren = true,
            AllowsDogs = true
        },
        new PlaceDto()
        {
            DisplayName = new DisplayName { Text = "Sushi Express", LanguageCode = "ja" },
            PrimaryType = "sushi_restaurant",
            Types = ["sushi_restaurant", "restaurant"],
            Rating = 4.3,
            UserRatingCount = 178,
            PriceLevel = PriceLevel.PRICE_LEVEL_VERY_EXPENSIVE,
            RegularOpeningHours = new RegularOpeningHours { OpenNow = true },
            GoodForChildren = true,
            AllowsDogs = true
        }];
    }
}
