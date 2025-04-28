using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;
using WhatMunch_MAUI.Utility;

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

    public class TokenService(
        ISecureStorage secureStorage,
        ILogger<TokenService> logger) : ITokenService
    {
        public readonly string _accessTokenKey = "jwt_token";
        private readonly string _refreshTokenKey = "jwtRefreshToken";

        // TODO: Add logging
        public async Task SaveAccessTokenAsync(string token)
        {
            await secureStorage.SetAsync(_accessTokenKey, token);
        }

        public async Task SaveRefreshTokenAsync(string token)
        {
            await secureStorage.SetAsync(_refreshTokenKey, token);
        }

        public async Task<string?> GetAccessTokenAsync()
        {
            return await secureStorage.GetAsync(_accessTokenKey);
        }

        public async Task<string?> GetRefreshTokenAsync()
        {
            return await secureStorage.GetAsync(_refreshTokenKey);
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
            secureStorage.Remove(_accessTokenKey);
            secureStorage.Remove(_refreshTokenKey);
        }
    }
}
