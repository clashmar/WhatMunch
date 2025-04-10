﻿using Microsoft.Extensions.Logging;
using Moq;
using RichardSzalay.MockHttp;
using WhatMunch_MAUI.Models;
using WhatMunch_MAUI.Models.Places;
using WhatMunch_MAUI.Services;

namespace WhatMunch_MAUI.Tests.Unit
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
            var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MockData", "mockPlace.json");
            var mockJson = File.ReadAllText(jsonPath);

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
            Assert.Equal("La Mar Cocina Peruana San Francisco", result.Data.Places.First()?.DisplayName?.Text);
            Assert.Equal(4.5, result.Data.Places.First()?.Rating);
            Assert.Equal(4338, result.Data.Places.First()?.UserRatingCount);
            Assert.Equal(PriceLevel.PRICE_LEVEL_MODERATE, result.Data.Places.First()?.PriceLevel);
            Assert.True(result.Data.Places.First()?.RegularOpeningHours.OpenNow);
            Assert.True(result.Data.Places.First()?.GoodForChildren);
            Assert.True(result.Data.Places.First()?.AllowsDogs);
            Assert.False(string.IsNullOrEmpty(result.Data.Places.First()?.Photos.First()?.GoogleMapsUri));
            Assert.Contains("restaurant", result.Data.Places.First()?.Types!);
        }
    }
}
