using Microsoft.Extensions.Logging;
using System.Text;
using WhatMunch_MAUI.Data;
using WhatMunch_MAUI.Extensions;
using WhatMunch_MAUI.Models.Dtos;
using WhatMunch_MAUI.Resources.Localization;
using WhatMunch_MAUI.Utility;
using WhatMunch_MAUI.Views;

namespace WhatMunch_MAUI.Services
{
    public interface IAccountService
    {
        Task<Result> RegisterUserAsync(RegistrationRequestDto requestDto);
        Task<Result> LoginUserAsync(LoginRequestDto requestDto);
        Task<Result> LoginSocialUserAsync();
        Task<Result> RefreshAccessTokenAsync();
        Task LogoutAsync();
        Task<Result> DeleteUserAccountAsync();
    }

    public class AccountService(
        IHttpClientFactory clientFactory, 
        ITokenService tokenService, 
        ISecureStorage secureStorage,
        ILogger<AccountService> logger,
        IWebAuthenticator webAuthenticator,
        IShellService shellService) : IAccountService
    {
        public async Task<Result> RegisterUserAsync(RegistrationRequestDto requestDto)
        {
            return await ExecuteRequestAsync(async () =>
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
            });
        }

        public async Task<Result> LoginUserAsync(LoginRequestDto requestDto)
        {
            return await ExecuteRequestAsync(async () =>
            {
                var client = clientFactory.CreateClient("WhatMunch").UpdateLanguageHeaders();
                var json = JsonSerializer.Serialize(requestDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("token/", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var deserializedData = JsonSerializer.Deserialize<LoginResponseDto>(responseContent);

                    if (deserializedData is not null)
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
            });
        }

        public async Task<Result> LoginSocialUserAsync()
        {
            return await ExecuteRequestAsync(async () =>
            {
                var authResult = await webAuthenticator.AuthenticateAsync(
                        new WebAuthenticatorOptions()
                        {
                            Url = new Uri("https://2260-217-123-90-227.ngrok-free.app/accounts/google/login/"),
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
            });
        }

        // TODO: Unit test RefreshAccessTokenAsync
        public async Task<Result> RefreshAccessTokenAsync()
        {
            var refreshToken = await tokenService.GetRefreshTokenAsync();
            if (string.IsNullOrEmpty(refreshToken))
                return Result.Failure("Could not get refresh token.");

            return await ExecuteRequestAsync(async () =>
            {
                var client = clientFactory.CreateClient("WhatMunch");
                var json = JsonSerializer.Serialize(refreshToken);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("token/refresh/", content);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<Dictionary<string, string>>(responseContent);

                    if (data != null && data.TryGetValue("access", out var newAccessToken))
                    {
                        await tokenService.SaveAccessTokenAsync(newAccessToken);
                        return Result.Success();
                    }
                }
                else
                {
                    logger.LogWarning("Refresh token invalid or expired.");
                    await shellService.DisplayAlert(AppResources.SessionExpired, AppResources.PleaseLoginAgain, AppResources.Ok);
                    await LogoutAsync();
                }
                return Result.Failure("Could not refresh access token. Refresh token may be expired.");
            });
        }

        // TODO: Unit test LogoutAsync
        public async Task LogoutAsync()
        {
            try
            {
                var client = clientFactory.CreateClient("WhatMunch");
                await tokenService.UpdateHeaders(client);
                var response = await client.PostAsync("auth/logout/", null);

                if (!response.IsSuccessStatusCode) logger.LogWarning("Could not log out from server.");

                tokenService.RemoveTokensFromStorage();
                await shellService.GoToAsync($"{nameof(LoginPage)}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error while logging out.");
                throw;
            }
        }

        // TODO: Unit test DeleteUserAccountAsync
        public async Task<Result> DeleteUserAccountAsync()
        {
            return await ExecuteRequestAsync(async () =>
            {
                var client = clientFactory.CreateClient("WhatMunch");

                var response = await client.ExecuteRequestWithRefreshAsync(
                    c => c.PostAsync("auth/delete-account/", null),
                    this,
                    tokenService,
                    clientFactory);

                //var response = await client.PostAsync("auth/delete-account/", null);

                response.EnsureSuccessStatusCode();
                tokenService.RemoveTokensFromStorage();
                await shellService.GoToAsync($"{nameof(RegistrationPage)}");
                return Result.Success();
            });
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

        private async Task<Result> ExecuteRequestAsync(Func<Task<Result>> requestFunc)
        {
            try
            {
                return await requestFunc.Invoke();
            }
            catch (TaskCanceledException ex)
            {
                logger.LogError(ex, "Login cancelled.");
                throw;
            }
            catch (HttpRequestException ex)
            {
                logger.LogError(ex, "HTTP request failed: {Message}", ex.Message);
                return Result.Failure(AppResources.ErrorServerConnection);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error: {Message}", ex.Message);
                return Result.Failure(AppResources.ErrorUnexpected);
            }
        }
    }
}
