using WhatMunch_MAUI.Models.Dtos;

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
            return new PlaceModel()
            {
                DisplayName = placeDto.DisplayName?.Text ?? string.Empty,
                PrimaryType = placeDto.PrimaryType,
                Types = placeDto.Types,
                Rating = placeDto.Rating,
                UserRatingCount = placeDto.UserRatingCount,
                PriceLevel = placeDto.PriceLevel,
                OpenNow = placeDto.RegularOpeningHours!.OpenNow,
                Photos = placeDto.Photos,
                GoodForChildren = placeDto.GoodForChildren,
                AllowsDogs = placeDto.AllowsDogs,
                MainPhoto = placeDto.Photos.FirstOrDefault()?.GoogleMapsUri
            };
        }
    }
}
