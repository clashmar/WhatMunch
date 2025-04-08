using System.Globalization;
using WhatMunch_MAUI.Models.Dtos;
using WhatMunch_MAUI.Secrets;
using WhatMunch_MAUI.Models.Fonts;

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
            string reference = placeDto.Photos.FirstOrDefault()?.Name ?? string.Empty;
            string key = ApiKeys.GOOGLE_MAPS_API_KEY;
            string mainPhoto = $"https://places.googleapis.com/v1/{reference}/media?maxWidthPx=600&key={key}";

            string ratingSummary = $"{placeDto.Rating} ({placeDto.UserRatingCount})";

            return new PlaceModel()
            {
                DisplayName = placeDto.DisplayName?.Text ?? string.Empty,
                PrimaryType = placeDto.PrimaryType,
                Types = placeDto.Types.ToDisplayTypes(),
                Rating = placeDto.Rating,
                UserRatingCount = placeDto.UserRatingCount,
                PriceLevel = placeDto.PriceLevel,
                OpenNow = placeDto.RegularOpeningHours!.OpenNow,
                Photos = placeDto.Photos,
                GoodForChildren = placeDto.GoodForChildren,
                AllowsDogs = placeDto.AllowsDogs,
                MainPhoto = mainPhoto,
                Stars = placeDto.Rating.ToStars(),
                RatingSummary = ratingSummary,
            };
        }

        public static List<string> ToDisplayTypes(this List<string> types)
        {
            List<string> displayTypes = [];

            foreach (var type in types)
            {
                var cleaned = type.Replace("_", " ");

                var display = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(cleaned.ToLower());

                displayTypes.Add(display);
            }

            return displayTypes;
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

            // Calculate the empty stars
            //int emptyStars = 5 - (fullStars + (hasHalfStar ? 1 : 0));
            //stars += new string('☆', emptyStars);

            return stars;
        }
    }
}
