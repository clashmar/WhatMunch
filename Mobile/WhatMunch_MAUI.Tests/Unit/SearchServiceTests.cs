using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.ObjectModel;
using WhatMunch_MAUI.Dtos;
using WhatMunch_MAUI.Models;
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
        public async Task GetFilteredSearchResults_ReturnsCorrectlyFilteredList()
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

            var searchResult = Result<NearbySearchResponseDto>.Success(new NearbySearchResponseDto() { Places = MockPlacesList });
            _googlePlacesServiceMock.Setup(c => c.GetNearbySearchResultsAsync(testPreferences)).ReturnsAsync(searchResult);

            // Act
            var result = await _service.GetFilteredSearchResults();

            // Assert
            Assert.Equivalent(result, MockPlacesCollection);
        }

        private readonly ObservableCollection<Place> MockPlacesCollection = [
        new Place()
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
        },];

        private readonly List<Place> MockPlacesList = [
        new Place()
        {
            DisplayName = new DisplayName { Text = "Joe's Diner", LanguageCode = "en" },
            PrimaryType = "restaurant",
            Types = ["diner", "restaurant"],
            Rating = 2.2,
            UserRatingCount = 120,
            PriceLevel = PriceLevel.PRICE_LEVEL_INEXPENSIVE,
            RegularOpeningHours = new RegularOpeningHours { OpenNow = true },
            GoodForChildren = true,
            AllowsDogs = true
        },
        new Place()
        {
            DisplayName = new DisplayName { Text = "Bella Italia", LanguageCode = "en" },
            PrimaryType = "italian_restaurant",
            Types = ["italian_restaurant", "restaurant"],
            Rating = 4.7,
            UserRatingCount = 315,
            PriceLevel = PriceLevel.PRICE_LEVEL_MODERATE,
            RegularOpeningHours = new RegularOpeningHours { OpenNow = true },
            GoodForChildren = true,
            AllowsDogs = false
        },
        new Place()
        {
            DisplayName = new DisplayName { Text = "The Vegan Spot", LanguageCode = "en" },
            PrimaryType = "vegan_restaurant",
            Types = ["vegan_restaurant", "restaurant"],
            Rating = 4.8,
            UserRatingCount = 210,
            PriceLevel = PriceLevel.PRICE_LEVEL_MODERATE,
            RegularOpeningHours = new RegularOpeningHours { OpenNow = false },
            GoodForChildren = false,
            AllowsDogs = true
        },
        new Place()
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
        new Place()
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
        },
        new Place()
        {
            DisplayName = new DisplayName { Text = "BBQ Heaven", LanguageCode = "en" },
            PrimaryType = "bbq_restaurant",
            Types = ["bbq_restaurant", "restaurant"],
            Rating = 4.6,
            UserRatingCount = 400,
            PriceLevel = PriceLevel.PRICE_LEVEL_EXPENSIVE,
            RegularOpeningHours = new RegularOpeningHours { OpenNow = true },
            GoodForChildren = true,
            AllowsDogs = true
        }];
    }
}
