using Microsoft.Extensions.Logging;
using WhatMunch_MAUI.Extensions;
using WhatMunch_MAUI.Resources.Localization;
using WhatMunch_MAUI.Services;
using WhatMunch_MAUI.Utility;
using WhatMunch_MAUI.Views;

namespace WhatMunch_MAUI.ViewModels
{
    public partial class LoginViewModel(
        IAccountService loginService, 
        IConnectivity connectivity, 
        IShellService shellService,
        ILogger<LoginViewModel> logger,
        IToastService toastService) : BaseViewModel
    {
        [ObservableProperty]
        public LoginModel _loginModel = new();

        [ObservableProperty]
        public double _errorOpacity = 0;

        [RelayCommand]
        public async Task HandleUsernameLoginAsync()
        {
            ErrorOpacity = 1.0;
            if (!LoginModel.IsValid()) return;
            await ExecuteLoginAsync(() => loginService.LoginUserAsync(LoginModel.ToDto()));
        }

        [RelayCommand]
        public async Task HandleSocialLoginAsync()
        {
            await ExecuteLoginAsync(() => loginService.LoginSocialUserAsync());
        }

        protected async Task ExecuteLoginAsync(Func<Task<Result>> loginFunction)
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

                var result = await loginFunction.Invoke();

                if(result.IsSuccess)
                {
                    await toastService.DisplayToast(AppResources.LoginSuccessful);
                    await shellService.GoToAsync($"//MainTabs/DashboardPage");
                }
                else
                {
                    await shellService.DisplayError(result.ErrorMessage ?? AppResources.ErrorUnexpected);
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during login.");
                await shellService.DisplayError(AppResources.ErrorUnexpected);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public override void ResetViewModel()
        {
            LoginModel = new LoginModel();
            IsBusy = false;
            ErrorOpacity = 0;
        }

        [RelayCommand]
        async Task GoToRegistrationPageAsync()
        {
            await shellService.GoToAsync($"{nameof(RegistrationPage)}");
        }
    }
}
