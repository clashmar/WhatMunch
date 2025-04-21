using Microsoft.Extensions.Logging;
using System.Text;
using WhatMunch_MAUI.Data;
using WhatMunch_MAUI.Extensions;
using WhatMunch_MAUI.Models.Dtos;
using WhatMunch_MAUI.Utility;

namespace WhatMunch_MAUI.Services
{
    public interface ILoginService
    {
        Task<Result> LoginUserAsync(LoginRequestDto requestDto);
        Task<Result> LoginSocialUserAsync();
    }

    public class LoginService(
        IHttpClientFactory clientFactory, 
        ITokenService tokenService, 
        ISecureStorage secureStorage,
        ILogger<LoginService> logger) : ILoginService
    {
        public async Task<Result> LoginUserAsync(LoginRequestDto requestDto)
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
                    
                    if(deserializedData is not null)
                    {
                        await HandleLoginDetails(deserializedData.AccessToken, deserializedData.RefreshToken, requestDto.Username);
                        return Result.Success();
                    }

                    return Result.Failure("Invalid server response.");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var error = JsonSerializer.Deserialize<ErrorMessageDto>(errorContent);
                    return Result.Failure($"Login failed: {error!.ErrorMessage}.");
                }
            }
            catch (HttpRequestException)
            {
                return Result.Failure("Failed to connect to the server. Please check your internet connection.");
            }
            catch (Exception)
            {
                return Result.Failure("An unexpected error occurred. Please try again later.");
            }
        }

        public async Task<Result> LoginSocialUserAsync()
        {
            try
            {
                WebAuthenticatorResult authResult = await WebAuthenticator.Default.AuthenticateAsync(
                    new WebAuthenticatorOptions()
                    {
                        // TODO: parameterize social provider
                        Url = new Uri("https://28b7-217-123-90-227.ngrok-free.app/accounts/google/login/"),
                        CallbackUrl = new Uri("whatmunch://oauth-redirect"),
                        PrefersEphemeralWebBrowserSession = true,

                    });
                
                if (authResult is not null)
                {
                    string accessToken = authResult.AccessToken;
                    string refreshToken = authResult.RefreshToken;
                    string username = authResult.Properties["email"];

                    if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken)) return Result.Failure("Could not fetch tokens on login.");
                    if (string.IsNullOrEmpty(username)) return Result.Failure("Could not fetch username on login.");

                    await HandleLoginDetails(accessToken, refreshToken, username);

                    return Result.Success();
                }
                    
                return Result.Failure("WebAuthenticatorResult was null.");
            }
            catch (TaskCanceledException ex)
            {
                logger.LogError(ex, "Login cancelled.");
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while logging in with social account.");
                throw;
            }
        }

        private async Task HandleLoginDetails(string accessToken, string refreshToken, string username)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(accessToken, nameof(accessToken));
            ArgumentException.ThrowIfNullOrWhiteSpace(refreshToken, nameof(refreshToken));
            ArgumentException.ThrowIfNullOrWhiteSpace(username, nameof(username));

            try
            {
                await tokenService.SaveAccessTokenAsync(accessToken);
                await tokenService.SaveRefreshTokenAsync(refreshToken);
                await secureStorage.SetAsync(Constants.USERNAME_KEY, username);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while saving login details.");
                throw;
            }
        }
    }
}
