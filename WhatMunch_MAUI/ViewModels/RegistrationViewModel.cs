using WhatMunch_MAUI.Pages;

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
        public bool _clickedSubmit;

        [RelayCommand]
        async Task GoToLoginPageAsync()
        {
            await Shell.Current.GoToAsync($"{nameof(LoginPage)}");
        }

        [RelayCommand]
        async Task HandleRegistrationAsync()
        {
            ClickedSubmit = true;

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
            ClickedSubmit = false;
        }
    }
}
