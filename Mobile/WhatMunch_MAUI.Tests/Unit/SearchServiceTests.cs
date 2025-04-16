using Microsoft.Extensions.Logging;
using Moq;
using WhatMunch_MAUI.Models;
using WhatMunch_MAUI.Models.Dtos;
using WhatMunch_MAUI.Services;
using WhatMunch_MAUI.Utility;

namespace WhatMunch_MAUI.Tests.Unit
{
    public class SearchServiceTests
    {
        private readonly Mock<ILogger<SearchService>> _loggerMock;
        private readonly Mock<IGooglePlacesService> _googlePlacesServiceMock;
        private readonly Mock<IConnectivity> _connectivityMock;
        private readonly Mock<ISearchPreferencesService> _searchPreferencesServiceMock;
        private readonly Mock<IFavouritesService> _favouritesServiceMock;
        private readonly SearchService _service;

        public SearchServiceTests()
        {
            _loggerMock = new();
            _googlePlacesServiceMock = new();
            _connectivityMock = new();
            _searchPreferencesServiceMock = new();
            _favouritesServiceMock = new();

            _service = new(
                _loggerMock.Object,
                _googlePlacesServiceMock.Object,
                _connectivityMock.Object,
                _searchPreferencesServiceMock.Object,
                _favouritesServiceMock.Object);
        }

        [Fact]
        public async Task GetSearchResponseAsync_ReturnsCorrectlData()
        {
            // Arrange
            _connectivityMock.Setup(c => c.NetworkAccess).Returns(NetworkAccess.Internet);
            string nextPageToken = "token";

            var testPreferences = new SearchPreferencesModel()
            {
                MinRating = 3,
                MaxPriceLevel = PriceLevel.PRICE_LEVEL_MODERATE,
                IsChildFriendly = true,
                IsDogFriendly = true
            };
            var favouritesResult = Result<List<PlaceDto>>.Success(_mockFavouritesList);
            _favouritesServiceMock.Setup(m => m.GetUserFavouritesAsync()).ReturnsAsync((favouritesResult));
            _searchPreferencesServiceMock.Setup(m => m.GetPreferencesAsync()).ReturnsAsync(testPreferences);

            var searchResult = Result<TextSearchResponseDto>.Success(new TextSearchResponseDto() { Places = _mockPlacesList });
            _googlePlacesServiceMock.Setup(c => c.GetNearbySearchResultsAsync(testPreferences, nextPageToken)).ReturnsAsync(searchResult);

            // Act
            var result = await _service.GetSearchResponseAsync(nextPageToken);

            // Assert
            Assert.Equivalent(result.Places, _mockPlacesList);
        }

        private readonly List<PlaceDto> _mockFavouritesList = []; 

        private readonly List<PlaceDto> _mockPlacesList = [
        new PlaceDto()
        {
            DisplayName = new DisplayName { Text = "Central Park Coffee", LanguageCode = "en" },
            PrimaryTypeDisplayName = new PrimaryTypeDisplayName() { Text = "cafe" },
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
            PrimaryTypeDisplayName = new PrimaryTypeDisplayName() { Text = "sushi_restaurant" },
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
