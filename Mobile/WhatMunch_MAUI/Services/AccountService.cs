using Microsoft.Extensions.Logging;
using System.Text;
using WhatMunch_MAUI.Data;
using WhatMunch_MAUI.Extensions;
using WhatMunch_MAUI.Models.Dtos;
using WhatMunch_MAUI.Resources.Localization;
using WhatMunch_MAUI.Utility;

namespace WhatMunch_MAUI.Services
{
    public interface IAccountService
    {
        Task<Result> RegisterUserAsync(RegistrationRequestDto requestDto);
        Task<Result> LoginUserAsync(LoginRequestDto requestDto);
        Task<Result> LoginSocialUserAsync();
    }

    public class AccountService(
        IHttpClientFactory clientFactory, 
        ITokenService tokenService, 
        ISecureStorage secureStorage,
        ILogger<AccountService> logger,
        IWebAuthenticator webAuthenticator) : IAccountService
    {
        public async Task<Result> RegisterUserAsync(RegistrationRequestDto requestDto)
        {
            try
            {
                var client = clientFactory.CreateClient("WhatMunch").UpdateLanguageHeaders();
                var json = JsonSerializer.Serialize(requestDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("auth/register/", content);

                if (response.IsSuccessStatusCode)
                {
                    await LoginUserAsync(requestDto.ToLoginRequestDto());
                    return Result.Success();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var error = JsonSerializer.Deserialize<ErrorMessageDto>(errorContent);
                    logger.LogError("Registration failed.");
                    return Result.Failure($"{AppResources.RegistrationFailed} {error!.ErrorMessage}.");
                }
            }
            catch (HttpRequestException ex)
            {
                logger.LogError(ex, "Account Service could not connect to the server during registration.");
                return Result.Failure(AppResources.ErrorServerConnection);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error during registration.");
                return Result.Failure(AppResources.ErrorUnexpected);
            }
        }

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

                    return Result.Failure(AppResources.InvalidServerResponse);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var error = JsonSerializer.Deserialize<ErrorMessageDto>(errorContent);
                    return Result.Failure($"{AppResources.LoginFailed} {error!.ErrorMessage}.");
                }
            }
            catch (HttpRequestException ex)
            {
                logger.LogError(ex, "Account Service could not connect to the server during login.");
                return Result.Failure(AppResources.ErrorServerConnection);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error during login.");
                return Result.Failure(AppResources.ErrorUnexpected);
            }
        }

        public async Task<Result> LoginSocialUserAsync()
        {
            try
            {
                var authResult = await webAuthenticator.AuthenticateAsync(
                        new WebAuthenticatorOptions()
                        {
                            // TODO: parameterize social provider
                            Url = new Uri("https://a4e2-217-123-90-227.ngrok-free.app/accounts/google/login/"),
                            CallbackUrl = new Uri("whatmunch://oauth-redirect"),
                            PrefersEphemeralWebBrowserSession = true,

                        });
                
                if (authResult is not null)
                {
                    string accessToken = authResult.AccessToken;
                    string refreshToken = authResult.RefreshToken;
                    string username = authResult.Properties["email"];

                    if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken)) return Result.Failure(AppResources.InvalidServerResponse);
                    if (string.IsNullOrEmpty(username)) return Result.Failure(AppResources.InvalidServerResponse);

                    await HandleLoginDetails(accessToken, refreshToken, username);

                    return Result.Success();
                }

                logger.LogError("WebAuthenticatorResult was null.");
                return Result.Failure(AppResources.ErrorUnexpected);
            }
            catch (TaskCanceledException ex)
            {
                logger.LogError(ex, "Login cancelled.");
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error during social login.");
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
