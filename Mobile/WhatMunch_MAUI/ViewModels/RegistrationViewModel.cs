using Microsoft.Extensions.Logging;
using WhatMunch_MAUI.Extensions;
using WhatMunch_MAUI.Resources.Localization;
using WhatMunch_MAUI.Services;
using WhatMunch_MAUI.Utility;
using WhatMunch_MAUI.Views;

namespace WhatMunch_MAUI.ViewModels
{
    public partial class RegistrationViewModel(
        IAccountService accountService, 
        IConnectivity connectivity, 
        IShellService shellService,
        IToastService toastService,
        ILogger<RegistrationViewModel> logger) : BaseViewModel
    {
        [ObservableProperty]
        public RegistrationModel _registrationModel = new();

        [ObservableProperty]
        public double _errorOpacity = 0;

        [RelayCommand]
        public async Task HandleRegistrationAsync()
        {
            ErrorOpacity = 1.0;
            if (!RegistrationModel.IsValid()) return;
            await ExecuteRegistrationAsync(() => accountService.RegisterUserAsync(RegistrationModel.ToDto()));
        }

        [RelayCommand]
        public async Task HandleSocialRegistrationAsync()
        {
            await ExecuteRegistrationAsync(() => accountService.LoginSocialUserAsync());
        }
         
        [RelayCommand]
        public async Task GoToLoginPageAsync()
        {
            await shellService.GoToAsync($"{nameof(LoginPage)}");
        }

        [RelayCommand]
        protected async Task ExecuteRegistrationAsync(Func<Task<Result>> registrationFunction)
        {
            if (IsBusy) return;

            try
            {
                if (connectivity.NetworkAccess != NetworkAccess.Internet)
                {
                    await shellService.DisplayError(AppResources.ErrorInternetConnection);
                    return;
                }

                IsBusy = true;
                var result = await registrationFunction.Invoke();

                if(result.IsSuccess)
                {
                    await toastService.DisplayToast(AppResources.RegistrationSuccessful);
                    await shellService.GoToAsync($"//MainTabs/DashboardPage");
                }
                else
                {
                    await shellService.DisplayError(result.ErrorMessage ?? "Invalid server response.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error during registration.");
                await shellService.DisplayError(AppResources.ErrorUnexpected);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public override void ResetViewModel()
        {
            RegistrationModel = new RegistrationModel();
            IsBusy = false;
            ErrorOpacity = 0;
        }
    }
}
