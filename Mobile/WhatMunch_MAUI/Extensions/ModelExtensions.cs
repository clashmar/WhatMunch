using System.Globalization;
using WhatMunch_MAUI.Models.Dtos;
using WhatMunch_MAUI.Models.Fonts;
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
            string? reference = placeDto.Photos?.FirstOrDefault()?.Name;
            string mainPhoto = string.IsNullOrEmpty(reference)
                ? "empty_photo.svg"
                : $"https://places.googleapis.com/v1/{reference}/media?maxWidthPx=600&key={ApiKeys.GOOGLE_MAPS_API_KEY}";

            string ratingSummary = $"{placeDto.Rating} ({placeDto.UserRatingCount})";

            return new PlaceModel()
            {
                DisplayName = placeDto.DisplayName?.Text ?? string.Empty,
                PrimaryType = placeDto.PrimaryTypeDisplayName?.Text ?? string.Empty,
                Types = placeDto.Types.ToDisplayTypes(),
                Rating = placeDto.Rating,
                UserRatingCount = placeDto.UserRatingCount,
                PriceLevel = placeDto.PriceLevel,
                OpenNow = placeDto.RegularOpeningHours!.OpenNow,
                OpeningTimes = placeDto.RegularOpeningHours.WeekdayDescriptions ?? [],
                Photos = placeDto.Photos?.ToDisplayPhotos() ?? ["empty_photo.svg"],
                GoodForChildren = placeDto.GoodForChildren,
                AllowsDogs = placeDto.AllowsDogs,
                MainPhoto = mainPhoto,
                Stars = placeDto.Rating.ToStars(),
                RatingSummary = ratingSummary,
                InternationalPhoneNumber = placeDto.InternationalPhoneNumber,
                Website = placeDto.WebsiteUri,
                Address = placeDto.ShortFormattedAddress,
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
            return stars;
        }

        public static List<string> ToDisplayPhotos(this List<PlacePhoto> photos)
        {
            if (photos is null || photos.Count < 1) return [];

            List<string> displayPhotos = [];

            foreach (var photo in photos)
            {
                string? reference = photo.Name;
                string displayPhoto = string.IsNullOrEmpty(reference)
                    ? "empty_photo.svg"
                    : $"https://places.googleapis.com/v1/{reference}/media?maxWidthPx=600&key={ApiKeys.GOOGLE_MAPS_API_KEY}";

                displayPhotos.Add(displayPhoto);
            }
            return displayPhotos;
        }
    }
}
