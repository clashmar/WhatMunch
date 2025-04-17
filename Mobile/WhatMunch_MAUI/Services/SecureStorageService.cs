
using Microsoft.Extensions.Logging;

namespace WhatMunch_MAUI.Services
{
    public interface ISecureStorageService
    {
        Task SaveUsernameAsync(string username);
        Task<string?> GetUsernameAsync();
    }
    public class SecureStorageService(ILogger<SecureStorageService> logger) : ISecureStorageService
    {
        private const string USERNAME_KEY = "username_key";
        

        public async Task SaveUsernameAsync(string username)
        {
            try
            {
                await SecureStorage.Default.SetAsync(USERNAME_KEY, username);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to get save username in secure storage");
                throw;
            }
        }

        public async Task<string?> GetUsernameAsync()
        {
            try
            {
                return await SecureStorage.Default.GetAsync(USERNAME_KEY);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to get get username from secure storage");
                throw;
            }
        }
    }
}
