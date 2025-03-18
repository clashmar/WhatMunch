namespace WhatMunch_MAUI.Services
{
    public interface IAuthService
    {
        Task SaveTokenAsync(string token);
        Task<string?> GetTokenAsync();
        void Logout();
        Task<bool> IsUserAuthenticated();
    }

    public class AuthService : IAuthService
    {
        private string _tokenKey = "jwt_token";

        public async Task SaveTokenAsync(string token)
        {
            await SecureStorage.SetAsync(_tokenKey, token);
        }

        public async Task<string?> GetTokenAsync()
        {
            return await SecureStorage.GetAsync(_tokenKey);
        }

        public void Logout()
        {
            SecureStorage.Remove(_tokenKey);
        }

        public async Task<bool> IsUserAuthenticated()
        {
            var token = await GetTokenAsync();
            return !string.IsNullOrEmpty(token);
        }
    }
}
