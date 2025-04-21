using Microsoft.Extensions.Logging;
using WhatMunch_MAUI.Extensions;
using WhatMunch_MAUI.Resources.Localization;
using WhatMunch_MAUI.Services;
using WhatMunch_MAUI.Views;

namespace WhatMunch_MAUI.ViewModels
{
    public partial class LoginViewModel(
        ILoginService loginService, 
        IConnectivity connectivity, 
        IShellService shellService,
        ILogger<LoginViewModel> logger) : BaseViewModel
    {
        [ObservableProperty]
        public LoginModel _loginModel = new();

        [ObservableProperty]
        public double _errorOpacity = 0;

        [RelayCommand]
        async Task HandleLoginAsync()
        {
            ErrorOpacity = 1.0;

            if (IsBusy) return;

            if (!LoginModel.IsValid())
            {
                return;
            }

            try
            {
                if (connectivity.NetworkAccess != NetworkAccess.Internet)
                {
                    await shellService.DisplayError(AppResources.ErrorInternetConnection);
                    return;
                }

                IsBusy = true;
                var result = await loginService.LoginUserAsync(LoginModel.ToDto());

                if(result.IsSuccess)
                {
                    await shellService.DisplayAlert("Success", "Login was successful.", "Ok");
                    await shellService.GoToAsync($"//MainTabs/DashboardPage");
                }
                else
                {
                    await shellService.DisplayAlert("Login Failed", result.ErrorMessage ?? "Invalid server response.", "Ok");
                }

            }
            catch (Exception)
            {
                await shellService.DisplayAlert("Hmm", "Something went wrong.", "Ok");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task HandleGoogleLoginAsync()
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
                var result = await loginService.SocialLoginAsync();

                if (result.IsSuccess)
                {
                    await shellService.DisplayAlert("Success", "Login was successful.", "Ok");
                    await shellService.GoToAsync($"//MainTabs/DashboardPage");
                }
                else
                {
                    await shellService.DisplayError(result.ErrorMessage ?? "Invalid server response.");
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while logging in with social account.");
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
