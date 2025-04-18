﻿using System.Text;
using WhatMunch_MAUI.Data;
using WhatMunch_MAUI.Extensions;
using WhatMunch_MAUI.Models.Dtos;
using WhatMunch_MAUI.Utility;

namespace WhatMunch_MAUI.Services
{
    public interface ILoginService
    {
        Task<Result<LoginResponseDto>> LoginUserAsync(LoginRequestDto requestDto);
    }

    public class LoginService(
        IHttpClientFactory clientFactory, 
        ITokenService tokenService, 
        ISecureStorage secureStorage) : ILoginService
    {
        public async Task<Result<LoginResponseDto>> LoginUserAsync(LoginRequestDto requestDto)
        {
            try
            {
                var client = clientFactory.CreateClient("WhatMunch").UpdateLanguageHeaders();
                var json = JsonSerializer.Serialize(requestDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("token/", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var deserializedData = JsonSerializer.Deserialize<LoginResponseDto>(responseContent);
                    
                    if(deserializedData != null)
                    {
                        await tokenService.SaveAccessTokenAsync(deserializedData.AccessToken);
                        await tokenService.SaveRefreshTokenAsync(deserializedData.RefreshToken);
                        await secureStorage.SetAsync(Constants.USERNAME_KEY, requestDto.Username);
                        return Result<LoginResponseDto>.Success(deserializedData);
                    }

                    return Result<LoginResponseDto>.Failure("Invalid server response.");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var error = JsonSerializer.Deserialize<ErrorMessageDto>(errorContent);
                    return Result<LoginResponseDto>.Failure($"Login failed: {error!.ErrorMessage}.");
                }
            }
            catch (HttpRequestException)
            {
                return Result<LoginResponseDto>.Failure("Failed to connect to the server. Please check your internet connection.");
            }
            catch (Exception)
            {
                return Result<LoginResponseDto>.Failure("An unexpected error occurred. Please try again later.");
            }
        }
    }
}
