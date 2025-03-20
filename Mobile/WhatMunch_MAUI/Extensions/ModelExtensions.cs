using WhatMunch_MAUI.Dtos;

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
    }
}
