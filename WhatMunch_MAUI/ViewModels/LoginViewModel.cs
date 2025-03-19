using WhatMunch_MAUI.Extensions;
using WhatMunch_MAUI.Services;
using WhatMunch_MAUI.Views;

namespace WhatMunch_MAUI.ViewModels
{
    public partial class LoginViewModel(ILoginService loginService, IConnectivity connectivity) : BaseViewModel
    {
        [ObservableProperty]
        public LoginModel _loginModel = new();

        private readonly ILoginService _loginService = loginService;

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
                    await Shell.Current.DisplayAlert("Internet Error", "Please check your internet connection.", "Ok");
                    return;
                }

                IsBusy = true;
                await _loginService.LoginUserAsync(LoginModel.ToDto());
                await Shell.Current.DisplayAlert("Success", "Login was successful.", "Ok");
            }
            catch (Exception)
            {
                await Shell.Current.DisplayAlert("Hmm", "Something went wrong.", "Ok");
                throw;
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
            await Shell.Current.GoToAsync($"{nameof(RegistrationPage)}");
        }
    }
}
