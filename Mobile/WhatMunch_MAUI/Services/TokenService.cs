using System.Net.Http.Headers;

namespace WhatMunch_MAUI.Services
{
    public interface ITokenService
    {
        Task SaveAccessTokenAsync(string token);
        Task SaveRefreshTokenAsync(string token);
        Task<string?> GetAccessTokenAsync();
        Task<string?> GetRefreshTokenAsync();
        void Logout();
        Task<bool> IsUserAuthenticated();
        Task UpdateHeaders(HttpClient httpClient);
    }

    public class TokenService : ITokenService
    {
        public readonly string _accessTokenKey = "jwt_token";
        private readonly string _refreshTokenKey = "jwtRefreshToken";

        public async Task SaveAccessTokenAsync(string token)
        {
            await SecureStorage.SetAsync(_accessTokenKey, token);
        }

        public async Task SaveRefreshTokenAsync(string token)
        {
            await SecureStorage.SetAsync(_refreshTokenKey, token);
        }

        public async Task<string?> GetAccessTokenAsync()
        {
            return await SecureStorage.GetAsync(_accessTokenKey);
        }

        public async Task<string?> GetRefreshTokenAsync()
        {
            return await SecureStorage.GetAsync(_refreshTokenKey);
        }

        public void Logout()
        {
            SecureStorage.Remove(_accessTokenKey);
            SecureStorage.Remove(_refreshTokenKey);
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
    }
}
