using Microsoft.Extensions.Logging;

namespace WhatMunch_MAUI.Services
{
    public interface ISearchPreferencesService
    {
        Task SavePreferencesAsync(SearchPreferencesModel preferencesModel);
        Task<SearchPreferencesModel> GetPreferencesAsync();
    }
    public class SearchPreferencesService(ISecureStorage secureStorage, ILogger<SearchPreferencesService> logger) : ISearchPreferencesService
    {
        private const string PREFERENCES_KEY = "search_preferences";

        public async Task SavePreferencesAsync(SearchPreferencesModel preferences)
        {
            try
            {
                string json = JsonSerializer.Serialize(preferences);
                await secureStorage.SetAsync(PREFERENCES_KEY, json);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to save search preferences.");
                throw;
            }
        }
        public async Task<SearchPreferencesModel> GetPreferencesAsync()
        {
            try
            {
                string? json = await secureStorage.GetAsync(PREFERENCES_KEY);

                if(string.IsNullOrEmpty(json))
                {
                    return SearchPreferencesModel.Default;
                }
                else
                {
                    return JsonSerializer.Deserialize<SearchPreferencesModel>(json) ?? SearchPreferencesModel.Default;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to get search preferences.");
                return SearchPreferencesModel.Default;
            }
        }
    }
}
