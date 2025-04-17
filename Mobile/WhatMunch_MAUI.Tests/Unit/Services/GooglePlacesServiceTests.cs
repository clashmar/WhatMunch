using Microsoft.Extensions.Logging;
using Moq;
using RichardSzalay.MockHttp;
using WhatMunch_MAUI.MockData;
using WhatMunch_MAUI.Models;
using WhatMunch_MAUI.Services;

namespace WhatMunch_MAUI.Tests.Unit.Services
{
    public class GooglePlacesServiceTests
    {
        private readonly Mock<IHttpClientFactory> _clientFactoryMock;
        private readonly Mock<ILogger<GooglePlacesService>> _loggerMock;
        private readonly Mock<ILocationService> _locationServiceMock;
        private readonly MockHttpMessageHandler _handlerMock;
        private readonly GooglePlacesService _service;

        public GooglePlacesServiceTests()
        {
            _clientFactoryMock = new();
            _loggerMock = new();
            _locationServiceMock = new();
            _handlerMock = new();
            _service = new(_clientFactoryMock.Object, _loggerMock.Object, _locationServiceMock.Object);
        }

        [Fact]
        public async Task GetNearbySearchResults_DeserializesCorrectly_AndReturnsSuccess()
        {
            // Arrange
            var mockJson = MockPlace.GetMockPlaceJson();


            _locationServiceMock.Setup(m => m.GetLocationWithTimeoutAsync())
                .ReturnsAsync(new Location(37.7749, -122.4194));

            _handlerMock.When(
                $"https://places.googleapis.com/*")
                .Respond("application/json", mockJson);

            _clientFactoryMock.Setup(x => x.CreateClient("GooglePlaces"))
                .Returns(new HttpClient(_handlerMock)
                {
                    BaseAddress = new Uri("https://places.googleapis.com/")
                });

            // Act
            var result = await _service.GetNearbySearchResultsAsync(SearchPreferencesModel.Default);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.NotEmpty(result.Data.Places);

            var place = result.Data.Places.First();

            // Basic info
            Assert.Equal("Pampalini Lunchroom & Coffee - Utrecht 2014", place.DisplayName?.Text);
            Assert.Equal(4.9, place.Rating);
            Assert.Equal(1765, place.UserRatingCount);
            Assert.Equal("Wittevrouwenstraat 14, Utrecht", place.ShortFormattedAddress);
            Assert.Equal("https://www.pampalini.nl/", place.WebsiteUri);
            Assert.Equal(PriceLevel.PRICE_LEVEL_MODERATE, place.PriceLevel);

            // Types
            Assert.Equal("cafe", place.PrimaryType);
            Assert.Contains("restaurant", place.Types);
            Assert.Contains("food", place.Types);

            // Boolean flags
            Assert.True(place.GoodForChildren);
            Assert.True(place.AllowsDogs);

            // Location
            Assert.Equal(52.092992300000006, place.Location?.Latitude);
            Assert.Equal(5.1221492, place.Location?.Longitude);

            // Opening hours
            Assert.NotNull(place.RegularOpeningHours);
            Assert.True(place.RegularOpeningHours.OpenNow);
            Assert.Equal(7, place.RegularOpeningHours.WeekdayDescriptions.Count);
            Assert.Contains("Monday: Closed", place.RegularOpeningHours.WeekdayDescriptions);

            // Reviews
            Assert.NotNull(place.Reviews);
            Assert.Equal(5, place.Reviews.Count);
            Assert.Contains(place.Reviews, r => r.Text?.Text.Contains("chicken panini") == true);
            Assert.Contains(place.Reviews, r => r.Text?.Text.Contains("wrap") == true);

            // Next Page Token
            Assert.Equal("AeeoHcI7Xnd8tU32jESwMvgnhAo6QAJfBz6liaHIUALeeGfQ-8NM763uoABKPHSXrxo6MwR6GPkQI3BuamzPLyfNC1ssp5P6JBXwRmUADDsokhrcRQ", result.Data.NextPageToken);
        }
    }
}
