using Microsoft.Extensions.Logging;

namespace WhatMunch_MAUI.Services
{
    public interface ISearchPreferencesService
    {
        Task SavePreferencesAsync(SearchPreferencesModel preferencesModel);
        Task<SearchPreferencesModel> GetPreferencesAsync();
    }
    public class SearchPreferencesService : ISearchPreferencesService
    {
        private readonly ILogger<SearchPreferencesService> _logger;

        public SearchPreferencesService(ILogger<SearchPreferencesService> logger)
        {
            _logger = logger;
        }

        private const string PREFERENCES_KEY = "search_preferences";

        public async Task SavePreferencesAsync(SearchPreferencesModel preferences)
        {
            try
            {
                string json = JsonSerializer.Serialize(preferences);
                await SecureStorage.Default.SetAsync("SEARCH_PREFERENCES", json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save search preferences");
                throw;
            }
        }
        public async Task<SearchPreferencesModel> GetPreferencesAsync()
        {
            try
            {
                string? json = await SecureStorage.Default.GetAsync(PREFERENCES_KEY);

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
                _logger.LogError(ex, "Failed to get search preferences");
                return SearchPreferencesModel.Default;
            }
        }
    }
}
