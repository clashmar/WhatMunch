using Microsoft.Extensions.Logging;
using WhatMunch_MAUI.Services;

namespace WhatMunch_MAUI.ViewModels
{
    public partial class SearchPreferencesViewModel : BaseViewModel
    {
        private readonly IShellService _shellService;
        private readonly ISearchPreferencesService _searchPreferencesService;
        private readonly ILogger<SearchPreferencesViewModel> _logger;

        public SearchPreferencesViewModel(
            IShellService shellService, 
            ISearchPreferencesService searchPreferencesService, 
            ILogger<SearchPreferencesViewModel> logger)
        {
            _shellService = shellService;
            _searchPreferencesService = searchPreferencesService;
            _logger = logger;
        }

        [ObservableProperty]
        private SearchPreferencesModel _preferences = SearchPreferencesModel.Default;



        [RelayCommand]
        public async Task HandleSavePreferences()
        {
            try
            {
                await _searchPreferencesService.SavePreferencesAsync(Preferences);
            }
            catch (Exception)
            {
                // toast alert
            }
            
        }

        public async void LoadPreferencesAsync()
        {
            try
            {
                IsBusy = true;
                Preferences = await _searchPreferencesService.GetPreferencesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load search preferences");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
