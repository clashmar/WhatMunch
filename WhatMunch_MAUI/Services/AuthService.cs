using System.Net.Http.Headers;

namespace WhatMunch_MAUI.Services
{
    public interface IAuthService
    {
        Task SaveAccessTokenAsync(string token);
        Task SaveRefreshTokenAsync(string token);
        Task<string?> GetAccessTokenAsync();
        Task<string?> GetRefreshTokenAsync();
        void Logout();
        Task<bool> IsUserAuthenticated();
        Task UpdateHeaders(HttpClient httpClient);
    }

    public class AuthService : IAuthService
    {
        private readonly string _tokenKey = "jwt_token";

        public async Task SaveAccessTokenAsync(string token)
        {
            await SecureStorage.SetAsync(_tokenKey, token);
        }

        public async Task SaveRefreshTokenAsync(string token)
        {
            await SecureStorage.SetAsync(_tokenKey, token);
        }

        public async Task<string?> GetAccessTokenAsync()
        {
            return await SecureStorage.GetAsync(_tokenKey);
        }

        public async Task<string?> GetRefreshTokenAsync()
        {
            return await SecureStorage.GetAsync(_tokenKey);
        }

        public void Logout()
        {
            SecureStorage.Remove(_tokenKey);
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
