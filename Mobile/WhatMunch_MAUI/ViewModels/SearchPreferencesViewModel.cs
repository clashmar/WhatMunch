using Microsoft.Extensions.Logging;
using WhatMunch_MAUI.Services;
using WhatMunch_MAUI.Resources.Localization;
using WhatMunch_MAUI.Views;

namespace WhatMunch_MAUI.ViewModels
{
    public partial class SearchPreferencesViewModel(
        ISearchPreferencesService searchPreferencesService,
        ILogger<SearchPreferencesViewModel> logger,
        IToastService toastService,
        IAccountService accountService,
        IShellService shellService) : BaseViewModel
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
                logger.LogError(ex, "Unexpected error: {Message}", ex.Message);
                await toastService.DisplayToast(AppResources.ErrorUnexpected);
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
                logger.LogError(ex, "Unexpected error: {Message}", ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        // TODO: Unit test HandleDeleteAccountAsync
        public async Task HandleDeleteAccountAsync()
        {
            try
            {
                IsBusy = true;
                bool isConfirmed = await shellService.CheckUserPrompt(AppResources.DeleteAccount);
                if(!isConfirmed) return;

                var result = await accountService.DeleteUserAccountAsync();
                if(result.IsSuccess)
                {
                    await toastService.DisplayToast(AppResources.AccountWasDeleted);
                    await shellService.GoToAsync(nameof(RegistrationPage));
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error: {Message}", ex.Message);
                await toastService.DisplayToast(AppResources.ErrorUnexpected);
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
