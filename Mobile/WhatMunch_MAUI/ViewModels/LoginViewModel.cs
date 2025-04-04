using WhatMunch_MAUI.Extensions;
using WhatMunch_MAUI.Services;
using WhatMunch_MAUI.Views;

namespace WhatMunch_MAUI.ViewModels
{
    public partial class LoginViewModel(ILoginService loginService, IConnectivity connectivity, IShellService shellService) : BaseViewModel
    {
        [ObservableProperty]
        public LoginModel _loginModel = new();

        private readonly ILoginService _loginService = loginService;
        private readonly IShellService _shellService = shellService;
        private readonly IConnectivity _connectivity = connectivity;

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
                if (_connectivity.NetworkAccess != NetworkAccess.Internet)
                {
                    await _shellService.DisplayAlert("Internet Error", "Please check your internet connection.", "Ok");
                    return;
                }

                IsBusy = true;
                var result = await _loginService.LoginUserAsync(LoginModel.ToDto());

                if(result.IsSuccess)
                {
                    await _shellService.DisplayAlert("Success", "Login was successful.", "Ok");
                    await _shellService.GoToAsync($"{nameof(DashboardPage)}");
                }
                else
                {
                    await _shellService.DisplayAlert("Login Failed", result.ErrorMessage ?? "Invalid server response.", "Ok");
                }

            }
            catch (Exception)
            {
                await _shellService.DisplayAlert("Hmm", "Something went wrong.", "Ok");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void ResetViewModel()
        {
            LoginModel = new LoginModel();
            IsBusy = false;
            ErrorOpacity = 0;
        }

        [RelayCommand]
        async Task GoToRegistrationPageAsync()
        {
            await _shellService.GoToAsync($"{nameof(RegistrationPage)}");
        }
    }
}
