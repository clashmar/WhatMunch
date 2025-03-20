using System.Text;
using WhatMunch_MAUI.Dtos;

namespace WhatMunch_MAUI.Services
{
    public interface IRegistrationService
    {
        Task RegisterUserAsync(RegistrationRequestDto requestDto);
    }

    public class RegistrationService(IHttpClientFactory clientFactory, IAuthService authService) : IRegistrationService
    {
        private readonly IHttpClientFactory _clientFactory = clientFactory;
        private readonly IAuthService _authService = authService;

        public async Task RegisterUserAsync(RegistrationRequestDto requestDto)
        {
            try
            {
                var client = _clientFactory.CreateClient("WhatMunch");
                var json = JsonSerializer.Serialize(requestDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("auth/register/", content);

                if(response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    //Login and get token
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Registration failed: {response.StatusCode}. {errorContent}");
                }

            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException("Failed to connect to the server. Please try again later.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occured.", ex);
            }
        }
    }
}
