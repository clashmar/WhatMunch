﻿using System.Text;
using WhatMunch_MAUI.Dtos;

namespace WhatMunch_MAUI.Services
{
    public interface ILoginService
    {
        Task LoginUserAsync(LoginRequestDto requestDto);
    }

    public class LoginService(IHttpClientFactory clientFactory, IAuthService authService) : ILoginService
    {
        private readonly IHttpClientFactory _clientFactory = clientFactory;
        private readonly IAuthService _authService = authService;

        public async Task LoginUserAsync(LoginRequestDto requestDto)
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
                    var deserializedData = JsonSerializer.Deserialize<LoginResponseDto>(responseContent);
                    
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
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Login failed: {response.StatusCode}. {errorContent}");
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
