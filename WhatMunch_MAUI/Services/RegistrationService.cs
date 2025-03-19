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
                    throw new Exception("Error"); // Handle properly
                }

            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP Error: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error: {ex.Message}");
                throw;
            }
        }
    }
}
