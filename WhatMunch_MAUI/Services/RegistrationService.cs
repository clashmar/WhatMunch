using System.Text;
using WhatMunch_MAUI.Dtos;

namespace WhatMunch_MAUI.Services
{
    public interface IRegistrationService
    {
        Task<HttpResponseMessage> RegisterUserAsync(RegistrationRequestDto requestDto);
    }

    public class RegistrationService(IHttpClientFactory ClientFactory) : IRegistrationService
    {
        private readonly IHttpClientFactory _clientFactory = ClientFactory;

        public async Task<HttpResponseMessage> RegisterUserAsync(RegistrationRequestDto requestDto)
        {
            try
            {
                var client = _clientFactory.CreateClient("WhatMunch");
                var json = JsonSerializer.Serialize(requestDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("register", content);
                return response;
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
