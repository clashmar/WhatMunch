using System.Text;
using WhatMunch_MAUI.Dtos;

namespace WhatMunch_MAUI.Services
{
    public interface ILoginService
    {
        Task LoginUserAsync(SignInRequestDto requestDto);
    }

    public class LoginService(IHttpClientFactory clientFactory, IAuthService authService) : ILoginService
    {
        private readonly IHttpClientFactory _clientFactory = clientFactory;
        private readonly IAuthService _authService = authService;

        public async Task LoginUserAsync(SignInRequestDto requestDto)
        {
            try
            {
                var client = _clientFactory.CreateClient("WhatMunch");
                var json = JsonSerializer.Serialize(requestDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("token/", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var deserializedData = JsonSerializer.Deserialize<SignInResponseDto>(responseContent);
                    
                    if(deserializedData != null)
                    {
                        var accessToken = deserializedData.AccessToken;
                        await _authService.SaveAccessTokenAsync(accessToken);
                        var refreshToken = deserializedData.RefreshToken;
                        await _authService.SaveRefreshTokenAsync(refreshToken);
                    }
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
