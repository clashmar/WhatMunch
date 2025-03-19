using WhatMunch_MAUI.Views;

namespace WhatMunch_MAUI.ViewModels
{
    public partial class RegistrationViewModel : BaseViewModel
    {
        [ObservableProperty]
        public RegistrationModel registrationModel;

        public RegistrationViewModel()
        {
            registrationModel = new RegistrationModel();
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
            ErrorOpacity = 1.0;

            if(!RegistrationModel.IsValid())
            {
                Debug.WriteLine("Form has errors!");
                return;
            }

            Debug.WriteLine("Registration successful!");
        }
        public void ResetViewModel()
        {
            RegistrationModel = new RegistrationModel();
            ErrorOpacity = 0;
        }
    }
}
