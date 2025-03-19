using WhatMunch_MAUI.Extensions;
using WhatMunch_MAUI.Services;
using WhatMunch_MAUI.Views;

namespace WhatMunch_MAUI.ViewModels
{
    public partial class RegistrationViewModel(IRegistrationService registrationService, IConnectivity connectivity) : BaseViewModel
    {
        [ObservableProperty]
        public RegistrationModel _registrationModel = new();

        private readonly IRegistrationService _registrationService = registrationService;

        private readonly IConnectivity _connectivity = connectivity;

        [ObservableProperty]
        public double _errorOpacity = 0;

        [RelayCommand]
        async Task GoToLoginPageAsync()
        {
            await Shell.Current.GoToAsync($"{nameof(LoginPage)}");
        }

        [RelayCommand]
        async Task HandleRegistrationAsync()
        {
            if (IsBusy) return;

            if (!RegistrationModel.IsValid())
            {
                ErrorOpacity = 1.0;
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
                await _registrationService.RegisterUserAsync(RegistrationModel.ToDto());
                await Shell.Current.DisplayAlert("Success", "Registration was successful.", "Ok");
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
            RegistrationModel = new RegistrationModel();
            IsBusy = false;
            ErrorOpacity = 0;
        }
    }
}
