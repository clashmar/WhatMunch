using WhatMunch_MAUI.Extensions;
using WhatMunch_MAUI.Services;
using WhatMunch_MAUI.Views;

namespace WhatMunch_MAUI.ViewModels
{
    public partial class RegistrationViewModel : BaseViewModel
    {
        [ObservableProperty]
        public RegistrationModel _registrationModel;

        private readonly RegistrationService _registrationService;

        public RegistrationViewModel(RegistrationService registrationService)
        {
            _registrationModel = new RegistrationModel();
            _registrationService = registrationService;
        }

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
            if(!RegistrationModel.IsValid())
            {
                ErrorOpacity = 1.0;
                return;
            }

            var response = await _registrationService.RegisterUserAsync(RegistrationModel.ToDto());
        }

        public void ResetViewModel()
        {
            RegistrationModel = new RegistrationModel();
            ErrorOpacity = 0;
        }
    }
}
