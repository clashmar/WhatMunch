using Microsoft.Extensions.Logging;
using WhatMunch_MAUI.Services;
using WhatMunch_MAUI.Resources.Localization;

namespace WhatMunch_MAUI.ViewModels
{
    public partial class SearchPreferencesViewModel(
        ISearchPreferencesService searchPreferencesService,
        ILogger<SearchPreferencesViewModel> logger,
        IToastService toastService) : BaseViewModel
    {
        [ObservableProperty]
        private SearchPreferencesModel _preferences = SearchPreferencesModel.Default;

        [ObservableProperty]
        private double _searchRadiusMinimum = 400;

        [RelayCommand]
        public async Task HandleSavePreferences()
        {
            try
            {
                await searchPreferencesService.SavePreferencesAsync(Preferences);
                await toastService.DisplayToast(AppResources.UpdatedPreferences);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error while saving preferences");
                await toastService.DisplayToast(AppResources.Error);
            }
        }
        public async Task LoadPreferencesAsync()
        {
            try
            {
                IsBusy = true;
                Preferences = await searchPreferencesService.GetPreferencesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to load search preferences");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public override void ResetViewModel()
        {
            throw new NotImplementedException();
        }
    }
}
