using WhatMunch_MAUI.Extensions;
using WhatMunch_MAUI.Models;
using WhatMunch_MAUI.Models.Dtos;
using WhatMunch_MAUI.Models.Fonts;
using WhatMunch_MAUI.Resources.Localization;

namespace WhatMunch_MAUI.Tests.Unit
{
    public class ModelExtensionsTests
    {
        [Fact]
        public void ToDto_RegistrationModel_ValidInput_ReturnsDto()
        {
            // Arrange
            var registrationModel = new RegistrationModel
            {
                Email = "test@example.com",
                Username = "testuser",
                Password = "Password123"
            };

            // Act
            var result = registrationModel.ToDto();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(registrationModel.Email, result.Email);
            Assert.Equal(registrationModel.Username, result.Username);
            Assert.Equal(registrationModel.Password, result.Password);
        }

        [Fact]
        public void ToDto_RegistrationModel_NullInput_ThrowsArgumentNullException()
        {
            // Arrange
            RegistrationModel registrationModel = null!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => registrationModel.ToDto());
        }

        [Fact]
        public void ToDto_LoginModel_ValidInput_ReturnsDto()
        {
            // Arrange
            var loginModel = new LoginModel
            {
                Username = "testuser",
                Password = "Password123"
            };

            // Act
            var result = loginModel.ToDto();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(loginModel.Username, result.Username);
            Assert.Equal(loginModel.Password, result.Password);
        }

        [Fact]
        public void ToDto_LoginModel_NullInput_ThrowsArgumentNullException()
        {
            // Arrange
            LoginModel loginModel = null!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => loginModel.ToDto());
        }

        [Fact]
        public void ToModel_PlaceDto_ValidInput_ReturnsModel()
        {
            // Arrange
            var placeDto = new PlaceDto
            {
                Id = "1",
                DisplayName = new DisplayName { Text = "Test Place" },
                PrimaryTypeDisplayName = new PrimaryTypeDisplayName { Text = "Restaurant" },
                Types = ["food", "establishment" ],
                Rating = 4.5,
                UserRatingCount = 100,
                PriceLevel = PriceLevel.PRICE_LEVEL_MODERATE,
                RegularOpeningHours = new RegularOpeningHours
                {
                    OpenNow = true,
                    WeekdayDescriptions = ["Mon-Fri: 9AM-5PM"]
                },
                Photos = [new PlacePhoto { Name = "photo1" }],
                GoodForChildren = true,
                AllowsDogs = false,
                InternationalPhoneNumber = "+123456789",
                WebsiteUri = "http://example.com",
                ShortFormattedAddress = "123 Test St",
                Reviews = [new Review { Rating = 5 }],
                Distance = 0.5
            };

            // Act
            var result = placeDto.ToModel();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(placeDto.Id, result.Id);
            Assert.Equal(placeDto.DisplayName.Text, result.DisplayName);
            Assert.Equal(placeDto.PrimaryTypeDisplayName.Text, result.PrimaryType);
            Assert.Equal(placeDto.Rating, result.Rating);
            Assert.Equal(placeDto.UserRatingCount, result.UserRatingCount);
            Assert.Equal(placeDto.RegularOpeningHours.OpenNow, result.OpenNow);
            Assert.Equal(placeDto.RegularOpeningHours.WeekdayDescriptions, result.OpeningTimes);
            Assert.NotEmpty(result.Photos);
            Assert.Equal(placeDto.GoodForChildren, result.GoodForChildren);
            Assert.Equal(placeDto.AllowsDogs, result.AllowsDogs);
            Assert.Equal(placeDto.InternationalPhoneNumber, result.InternationalPhoneNumber);
            Assert.Equal(placeDto.WebsiteUri, result.Website);
            Assert.Equal(placeDto.ShortFormattedAddress, result.Address);
            Assert.Equal(placeDto.Reviews, result.Reviews);
        }

        [Fact]
        public void ToModel_PlaceDto_NullInput_ThrowsArgumentNullException()
        {
            // Arrange
            PlaceDto placeDto = null!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => placeDto.ToModel());
        }

        [Fact]
        public void ToDisplayAttributes_ValidInput_ReturnsAttributes()
        {
            // Arrange
            List<string> types = ["food", "dog_friendly"];
            PlaceDto placeDto = new()
            {
                RegularOpeningHours = new RegularOpeningHours { OpenNow = true },
                Distance = 0.5,
                AllowsDogs = true,
                GoodForChildren = false
            };

            // Act
            var result = types.ToDisplayAttributes(placeDto);

            // Assert
            Assert.NotNull(result);
            Assert.Contains(result, attr => attr.Text == AppResources.OpenNow);
            Assert.Contains(result, attr => attr.Text == AppResources.DogFriendly);
        }

        [Fact]
        public void ToStars_ValidRating_ReturnsStars()
        {
            // Arrange
            double rating = 4.5;
            string expectedStars = new string(FaSolid.Star[0], 4) + FaSolid.StarHalf;

            // Act
            var result = rating.ToStars();

            // Assert
            Assert.Equal(expectedStars, result);
        }

        [Fact]
        public void ToDollarDisplay_ValidPriceLevel_ReturnsDollarDisplay()
        {
            // Arrange
            var priceLevel = PriceLevel.PRICE_LEVEL_EXPENSIVE;

            // Act
            var priceLevelDisplay = priceLevel.ToDollarDisplay();

            // Assert
            Assert.Equal("$$$", priceLevelDisplay.Number);
            Assert.Equal("$", priceLevelDisplay.Remainder);
        }

        [Fact]
        public void ToDisplayPhotos_ValidPhotos_ReturnsPhotoUrls()
        {
            // Arrange
            List<PlacePhoto> photos =
            [
                new() { Name = "photo1" },
                new() { Name = "photo2" }
            ];

            // Act
            var result = photos.ToDisplayPhotos();

            // Assert
            Assert.NotNull(result);
            Assert.All(result, url => Assert.StartsWith("https://places.googleapis.com/v1/", url));
        }

        [Fact]
        public void ToDisplayPhotos_EmptyPhotos_ReturnsDefaultPhoto()
        {
            // Arrange
            var photos = new List<PlacePhoto>();

            // Act
            var result = photos.ToDisplayPhotos();

            // Assert
            Assert.Single(result);
            Assert.Equal("empty_photo.svg", result[0]);
        }
    }
}
