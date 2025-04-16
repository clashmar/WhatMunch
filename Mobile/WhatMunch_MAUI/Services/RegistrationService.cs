using System.Text;
using WhatMunch_MAUI.Extensions;
using WhatMunch_MAUI.Models.Dtos;
using WhatMunch_MAUI.Utility;

namespace WhatMunch_MAUI.Services
{
    public interface IRegistrationService
    {
        Task<Result<string>> RegisterUserAsync(RegistrationRequestDto requestDto);
    }

    public class RegistrationService(IHttpClientFactory clientFactory) : IRegistrationService
    {
        private readonly IHttpClientFactory _clientFactory = clientFactory;

        public async Task<Result<string>> RegisterUserAsync(RegistrationRequestDto requestDto)
        {
            try
            {
                var client = _clientFactory.CreateClient("WhatMunch").UpdateLanguageHeaders();
                var json = JsonSerializer.Serialize(requestDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("auth/register/", content);

                if(response.IsSuccessStatusCode)
                {
                    return Result<string>.Success("Registration successful.");
                    // TODO: Auto login and get token after registration
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var error = JsonSerializer.Deserialize<ErrorMessageDto>(errorContent);
                    return Result<string>.Failure($"Registration failed: {error!.ErrorMessage}.");
                }

            }
            catch (HttpRequestException)
            {
                return Result<string>.Failure("Failed to connect to the server. Please check your internet connection.");
            }
            catch (Exception)
            {
                return Result<string>.Failure("An unexpected error occurred. Please try again later.");
            }
        }
    }
}
