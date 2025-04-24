using WhatMunch_MAUI.Extensions;
using WhatMunch_MAUI.Models.Dtos;

namespace WhatMunch_MAUI.Tests.Unit.Extensions
{
    public class SearchExtensionTests
    {
        private readonly List<PlaceDto> _places = [
                new() { Id = "1", Location = new PlaceLocation { Latitude = 10, Longitude = 20 } },
                new() { Id = "2", Location = new PlaceLocation { Latitude = 20, Longitude = 30 } },
                new() { Id = "3", Location = new PlaceLocation { Latitude = 30, Longitude = 40 } }
            ];

        [Fact]
        public void AddDistances_ShouldNotModifyPlaces_WhenLocationIsNull()
        {
            // Act
            var result = _places.AddDistances(null).ToList();

            // Assert
            Assert.Equal(_places, result);
            Assert.All(result, place => Assert.Equal(0, place.Distance));
        }

        [Fact]
        public void AddDistances_ShouldCalculateDistances_WhenLocationIsProvided()
        {
            // Arrange
            var location = new Location(0, 0);

            // Act
            var result = _places.AddDistances(location).ToList();

            // Assert
            Assert.All(result, place => Assert.True(place.Distance > 0));
        }

        [Fact]
        public void AddDistance_ShouldReturnSamePlace_WhenLocationIsNull()
        {
            // Arrange
            PlaceDto place = new()
            {
                Location = new() { Latitude = 10, Longitude = 20 }
            };

            // Act
            var result = place.AddDistance(null);

            // Assert
            Assert.Equal(place, result);
        }

        [Fact]
        public void AddDistance_ShouldCalculateDistance_WhenLocationIsProvided()
        {
            // Arrange
            var location = new Location(0, 0);
            PlaceDto place = new()
            {
                Location = new() { Latitude = 10, Longitude = 20 }
            };

            // Act
            var result = place.AddDistance(location);

            // Assert
            Assert.NotNull(result);
            Assert.True(0 < result.Distance);
        }

        [Fact]
        public void FilterDistances_ShouldReturnPlacesWithinDistance()
        {
            // Arrange
            var places = new List<PlaceDto>
            {
                new() { Id = "1", Distance = 2.5 },
                new() { Id = "2", Distance = 3.5 },
                new() { Id = "3", Distance = 1.0 }
            };

            // Act
            var result = places.FilterDistances().ToList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.All(result, place => Assert.True(place.Distance < 3.0));
        }

        [Fact]
        public void CheckIsFavourite_ShouldMarkFavoritesCorrectly()
        {
            // Arrange
            var favourites = new List<PlaceDto>
            {
                new() { Id = "2" },
                new() { Id = "3" }
            };

            // Act
            var result = _places.CheckIsFavourite(favourites).ToList();

            // Assert
            Assert.False(result[0].IsFavourite);
            Assert.True(result[1].IsFavourite);
            Assert.True(result[2].IsFavourite);
        }

        [Fact]
        public void CheckIsFavourite_ShouldHandleEmptyFavourites()
        {
            // Arrange
            List<PlaceDto> favourites = [];

            // Act
            var result = _places.CheckIsFavourite(favourites).ToList();

            // Assert
            Assert.All(result, place => Assert.False(place.IsFavourite));
        }
    }
}
