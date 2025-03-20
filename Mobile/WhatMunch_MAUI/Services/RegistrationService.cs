using System.Text;
using WhatMunch_MAUI.Dtos;
using WhatMunch_MAUI.Utility;

namespace WhatMunch_MAUI.Services
{
    public interface IRegistrationService
    {
        Task<HttpResult<string>> RegisterUserAsync(RegistrationRequestDto requestDto);
    }

    public class RegistrationService(IHttpClientFactory clientFactory) : IRegistrationService
    {
        private readonly IHttpClientFactory _clientFactory = clientFactory;

        public async Task<HttpResult<string>> RegisterUserAsync(RegistrationRequestDto requestDto)
        {
            try
            {
                var client = _clientFactory.CreateClient("WhatMunch");
                var json = JsonSerializer.Serialize(requestDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("auth/register/", content);

                if(response.IsSuccessStatusCode)
                {
                    return HttpResult<string>.Success("Registration successful.");
                    //Login and get token
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var error = JsonSerializer.Deserialize<ErrorMessageDto>(errorContent);
                    return HttpResult<string>.Failure($"Registration failed: {error!.ErrorMessage}.");
                }

            }
            catch (HttpRequestException)
            {
                return HttpResult<string>.Failure("Failed to connect to the server. Please check your internet connection.");
            }
            catch (Exception)
            {
                return HttpResult<string>.Failure("An unexpected error occurred. Please try again later.");
            }
        }
    }
}
