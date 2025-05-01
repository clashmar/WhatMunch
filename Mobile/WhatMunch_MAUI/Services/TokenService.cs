using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;

namespace WhatMunch_MAUI.Services
{
    public interface ITokenService
    {
        Task SaveAccessTokenAsync(string token);
        Task SaveRefreshTokenAsync(string token);
        Task<string?> GetAccessTokenAsync();
        Task<string?> GetRefreshTokenAsync();
        Task<bool> IsUserAuthenticated();
        Task UpdateHeaders(HttpClient httpClient);
        void RemoveTokensFromStorage();
    }

    // TODO: Add Unit tests
    public class TokenService(
        ISecureStorage secureStorage,
        ILogger<TokenService> logger) : ITokenService
    {
        public const string AccessTokenKey = "jwt_token";
        private const string RefreshTokenKey = "jwt_refresh_token";

        
        public async Task SaveAccessTokenAsync(string token)
        {
            logger.LogInformation("Saving access token to secure storage.");
            await secureStorage.SetAsync(AccessTokenKey, token);
        }

        public async Task SaveRefreshTokenAsync(string token)
        {
            logger.LogInformation("Saving refresh token to secure storage.");
            await secureStorage.SetAsync(RefreshTokenKey, token);
        }

        public async Task<string?> GetAccessTokenAsync()
        {
            logger.LogInformation("Retrieving access token from secure storage.");
            return await secureStorage.GetAsync(AccessTokenKey);
        }

        public async Task<string?> GetRefreshTokenAsync()
        {
            logger.LogInformation("Retrieving refresh token from secure storage.");
            return await secureStorage.GetAsync(RefreshTokenKey);
        }

        public async Task<bool> IsUserAuthenticated()
        {
            var token = await GetAccessTokenAsync();
            return !string.IsNullOrEmpty(token);
        }

        public async Task UpdateHeaders(HttpClient httpClient)
        {
            var token = await GetAccessTokenAsync();

            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
        public void RemoveTokensFromStorage()
        {
            logger.LogInformation("Removing tokens from secure storage.");
            secureStorage.Remove(AccessTokenKey);
            secureStorage.Remove(RefreshTokenKey);
        }
    }
}
