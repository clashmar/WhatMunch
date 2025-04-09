using System.Globalization;
using WhatMunch_MAUI.Models.Dtos;
using WhatMunch_MAUI.Models.Fonts;
using WhatMunch_MAUI.Resources.Localization;
using WhatMunch_MAUI.Secrets;

namespace WhatMunch_MAUI.Extensions
{
    public static class ModelExtensions
    {
        public static RegistrationRequestDto ToDto(this RegistrationModel registrationModel)
        {
            return new RegistrationRequestDto()
            {
                Email = registrationModel.Email,
                Username = registrationModel.Username,
                Password = registrationModel.Password
            };
        }

        public static LoginRequestDto ToDto(this LoginModel loginModel)
        {
            return new LoginRequestDto()
            {
                Username = loginModel.Username,
                Password = loginModel.Password
            };
        }

        public static PlaceModel ToModel(this PlaceDto placeDto)
        {
            string ratingSummary = $"{placeDto.Rating} ({placeDto.UserRatingCount})";

            return new PlaceModel()
            {
                Id = placeDto.Id,
                DisplayName = placeDto.DisplayName?.Text ?? AppResources.NotAvailable,
                PrimaryType = placeDto.PrimaryTypeDisplayName?.Text ?? AppResources.NotAvailable,
                Attributes = placeDto.Types.ToDisplayAttributes(placeDto),
                Rating = placeDto.Rating,
                UserRatingCount = placeDto.UserRatingCount,
                PriceLevel = placeDto.PriceLevel.ToDollarDisplay(),
                OpenNow = placeDto.RegularOpeningHours!.OpenNow,
                OpeningTimes = placeDto.RegularOpeningHours.WeekdayDescriptions ?? [],
                Photos = placeDto.Photos?.ToDisplayPhotos() ?? ["empty_photo.svg"],
                GoodForChildren = placeDto.GoodForChildren,
                AllowsDogs = placeDto.AllowsDogs,
                Stars = placeDto.Rating.ToStars(),
                RatingSummary = ratingSummary,
                InternationalPhoneNumber = placeDto.InternationalPhoneNumber,
                Website = placeDto.WebsiteUri,
                Address = placeDto.ShortFormattedAddress,
                Reviews = placeDto.Reviews,
            };
        }

        public static List<DisplayAttribute> ToDisplayAttributes(this List<string> types, PlaceDto placeDto)
        {
            List<DisplayAttribute> displayAttributes = [];

            if (placeDto.RegularOpeningHours?.OpenNow ?? false)
            {
                displayAttributes.Add(new(FaSolid.DoorOpen, AppResources.OpenNow));
            }
            else
            {
                displayAttributes.Add(new(FaSolid.DoorClosed, AppResources.Closed));
            }

            if (placeDto.Distance > 0)
            {
                string distance;

                if (placeDto.Distance <= 0.4)
                {
                    distance = AppResources._5MinWalk;
                }
                else if (placeDto.Distance <= 0.8)
                {
                    distance = AppResources._10MinWalk;
                }
                else if (placeDto.Distance <= 1.2)
                {
                    distance = AppResources._15MinWalk;
                }
                else if (placeDto.Distance <= 1.6)
                {
                    distance = AppResources._20MinWalk;
                }
                else
                {
                    distance = AppResources._30MinWalk;
                }

                displayAttributes.Add(new(FaSolid.ShoePrints, distance));
            }

            if (placeDto.AllowsDogs)
            {
                displayAttributes.Add(new(FaSolid.Dog, AppResources.DogFriendly));
            }

            if (placeDto.GoodForChildren)
            {
                displayAttributes.Add(new(FaSolid.Child, AppResources.ChildFriendly));
            }

            foreach (var type in types)
            {
                if (type == AppResources.establishment 
                    || type == AppResources.point_of_interest
                    || type == AppResources.food) 
                    continue;

                var cleaned = type.Replace("_", " ");

                var attribute = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(cleaned.ToLower());

                if (attribute == AppResources.VegetarianRestaurant)
                {
                    displayAttributes.Add(new(FaSolid.Leaf, AppResources.Vegetarian));
                }
                else if(attribute == AppResources.VeganRestaurant)
                {
                    displayAttributes.Add(new(FaSolid.Leaf, AppResources.Vegan));
                }
                else
                {
                    displayAttributes.Add(new(null, attribute));
                }
            }

            return displayAttributes;
        }

        public static string ToStars(this double rating)
        {
            int fullStars = (int)Math.Floor(rating);
            bool hasHalfStar = (rating - fullStars) >= 0.5;

            string stars = new(FaSolid.Star[0], fullStars);

            if (hasHalfStar)
            {
                stars += FaSolid.StarHalf;
            }
            return stars;
        }

        public static (string number, string remainder) ToDollarDisplay(this PriceLevel priceLevel)
        {
            string number = priceLevel switch
            {
                PriceLevel.PRICE_LEVEL_INEXPENSIVE => new(FaSolid.DollarSign[0], 1),
                PriceLevel.PRICE_LEVEL_MODERATE => new(FaSolid.DollarSign[0], 2),
                PriceLevel.PRICE_LEVEL_EXPENSIVE => new(FaSolid.DollarSign[0], 3),
                PriceLevel.PRICE_LEVEL_VERY_EXPENSIVE => new(FaSolid.DollarSign[0], 4),
                _ => string.Empty
            };

            string remainder = new(FaSolid.DollarSign[0], 4 - number.Length);

            return (number, remainder);
        }

        public static List<string> ToDisplayPhotos(this List<PlacePhoto> photos)
        {
            if (photos is null || photos.Count < 1) return ["empty_photo.svg"];

            List<string> displayPhotos = [];

            foreach (var photo in photos)
            {
                string? reference = photo.Name;
                string displayPhoto = string.IsNullOrEmpty(reference)
                    ? "empty_photo.svg"
                    : $"https://places.googleapis.com/v1/{reference}/media?maxWidthPx=600&key={ApiKeys.GOOGLE_MAPS_API_KEY}";

                displayPhotos.Add(displayPhoto);
            }
            return displayPhotos.Count > 0 ? displayPhotos : ["empty_photo.svg"];
        }
    }
}
