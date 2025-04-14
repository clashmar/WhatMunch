using System.Globalization;
using WhatMunch_MAUI.Models.Dtos;
using WhatMunch_MAUI.Models.Fonts;
using WhatMunch_MAUI.Resources.Localization;
using WhatMunch_MAUI.Secrets;

namespace WhatMunch_MAUI.Extensions
{
    public static class ModelExtensions
    {
        private const string EmptyPhoto = "empty_photo.svg";

        public static RegistrationRequestDto ToDto(this RegistrationModel registrationModel)
        {
            ArgumentNullException.ThrowIfNull(registrationModel);

            return new RegistrationRequestDto()
            {
                Email = registrationModel.Email,
                Username = registrationModel.Username,
                Password = registrationModel.Password
            };
        }

        public static LoginRequestDto ToDto(this LoginModel loginModel)
        {
            ArgumentNullException.ThrowIfNull(loginModel);

            return new LoginRequestDto()
            {
                Username = loginModel.Username,
                Password = loginModel.Password
            };
        }

        public static string ToDisplayNameText(this string displayName)
        {
            if (string.IsNullOrEmpty(displayName))
                return AppResources.NotAvailable;

            return displayName.Length > 26 ? $"{displayName[..26]}...": displayName;
        }

        public static PlaceModel ToModel(this PlaceDto placeDto)
        {
            ArgumentNullException.ThrowIfNull(placeDto);

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
                Photos = placeDto.Photos?.ToDisplayPhotos() ?? [EmptyPhoto],
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
            ArgumentNullException.ThrowIfNull(types);
            ArgumentNullException.ThrowIfNull(placeDto);

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
                displayAttributes.Add(new DisplayAttribute(FaSolid.ShoePrints, GetDistanceDescription(placeDto.Distance)));
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
                if (type == "establishment"
                    || type == "point_of_interest"
                    || type == "food_store"
                    || type == "store"
                    || type == "food")
                    continue;

                var attribute = CultureInfo.CurrentCulture.TextInfo
                    .ToTitleCase(type.Replace("_", " ")
                    .ToLower());

                if (attribute == AppResources.VegetarianRestaurant)
                {
                    displayAttributes.Add(new(FaSolid.Leaf, AppResources.Vegetarian));
                }
                else if (attribute == AppResources.VeganRestaurant)
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

        public static PriceLevelDisplay ToDollarDisplay(this PriceLevel priceLevel)
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

            return new PriceLevelDisplay(number, remainder);
        }

        public static List<string> ToDisplayPhotos(this List<PlacePhoto> photos)
        {
            if (photos == null || photos.Count == 0) return [EmptyPhoto];

            return photos.Select(photo =>
                string.IsNullOrEmpty(photo.Name)
                    ? EmptyPhoto
                    : $"https://places.googleapis.com/v1/{photo.Name}/media?maxWidthPx=600&key={ApiKeys.GOOGLE_MAPS_API_KEY}"
            ).ToList();
        }

        private static string GetDistanceDescription(double distance)
        {
            return distance switch
            {
                <= 0.4 => AppResources._5MinWalk,
                <= 0.8 => AppResources._10MinWalk,
                <= 1.2 => AppResources._15MinWalk,
                <= 1.6 => AppResources._20MinWalk,
                _ => AppResources._30MinWalk
            };
        }
    }
}
