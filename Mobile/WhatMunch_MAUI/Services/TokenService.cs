using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;

namespace WhatMunch_MAUI.Services
{
    public interface ITokenService
    {
        Task SaveAccessTokenAsync(string token);
        Task SaveRefreshTokenAsync(string token);
        Task<string?> GetAccessTokenAsync();
        Task<string?> GetRefreshTokenAsync();
        Task LogoutAsync();
        Task<bool> IsUserAuthenticated();
        Task UpdateHeaders(HttpClient httpClient);
    }

    public class TokenService(
        ISecureStorage secureStorage,
        IHttpClientFactory clientFactory, 
        ILogger<TokenService> logger) : ITokenService
    {
        public readonly string _accessTokenKey = "jwt_token";
        private readonly string _refreshTokenKey = "jwtRefreshToken";

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

        public async Task LogoutAsync()
        {
            try
            {
                var client = clientFactory.CreateClient("WhatMunch");
                await UpdateHeaders(client);
                var response = await client.PostAsync("auth/logout/", null);

                //if (!response.IsSuccessStatusCode) throw new Exception("Could not logout of django.");

                secureStorage.Remove(_accessTokenKey);
                secureStorage.Remove(_refreshTokenKey);
            }
            catch (Exception  ex)
            {
                logger.LogError(ex, "Unexpected error while logging out.");
                throw;
            }
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
