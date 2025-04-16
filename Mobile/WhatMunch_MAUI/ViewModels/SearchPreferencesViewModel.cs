using Microsoft.Extensions.Logging;
using WhatMunch_MAUI.Services;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using WhatMunch_MAUI.Resources.Localization;

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

        [ObservableProperty]
        private double _searchRadiusMinimum = 400;

        [RelayCommand]
        public async Task HandleSavePreferences()
        {
            try
            {
                await _searchPreferencesService.SavePreferencesAsync(Preferences);
                await DisplayToast(AppResources.UpdatedPreferences);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while saving preferences");
                await DisplayToast(AppResources.Error);
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

        private async Task DisplayToast(string text)
        {
            CancellationTokenSource cancellationTokenSource = new();
            ToastDuration duration = ToastDuration.Short;
            double fontSize = 14;
            var toast = Toast.Make(text, duration, fontSize);
            await toast.Show(cancellationTokenSource.Token);
        }

        public override void ResetViewModel()
        {
            throw new NotImplementedException();
        }
    }
}
