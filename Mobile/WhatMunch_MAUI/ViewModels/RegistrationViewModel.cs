using WhatMunch_MAUI.Extensions;
using WhatMunch_MAUI.Services;
using WhatMunch_MAUI.Views;

namespace WhatMunch_MAUI.ViewModels
{
    public partial class RegistrationViewModel(IRegistrationService registrationService, IConnectivity connectivity, IShellService shellService) : BaseViewModel
    {
        [ObservableProperty]
        public RegistrationModel _registrationModel = new();

        private readonly IRegistrationService _registrationService = registrationService;
        private readonly IShellService _shellService = shellService;
        private readonly IConnectivity _connectivity = connectivity;

        [ObservableProperty]
        public double _errorOpacity = 0;

        [RelayCommand]
        async Task GoToLoginPageAsync()
        {
            await _shellService.GoToAsync($"{nameof(LoginPage)}");
        }

        [RelayCommand]
        async Task HandleRegistrationAsync()
        {
            ErrorOpacity = 1.0;

            if (IsBusy) return;

            if (!RegistrationModel.IsValid())
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
                var result = await _registrationService.RegisterUserAsync(RegistrationModel.ToDto());

                if(result.IsSuccess)
                {
                    await _shellService.DisplayAlert("Success", "Registration was successful.", "Ok");
                    await _shellService.GoToAsync($"{nameof(LoginPage)}");
                }
                else
                {
                    await _shellService.DisplayAlert("Registration Failed", result.ErrorMessage ?? "Invalid server response.", "Ok");
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

        public override void ResetViewModel()
        {
            RegistrationModel = new RegistrationModel();
            IsBusy = false;
            ErrorOpacity = 0;
        }
    }
}
