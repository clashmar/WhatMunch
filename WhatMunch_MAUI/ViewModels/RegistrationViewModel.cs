using WhatMunch_MAUI.Pages;

namespace WhatMunch_MAUI.ViewModels
{
    public partial class RegistrationViewModel : BaseViewModel
    {
        [RelayCommand]
        async Task GoToLoginPageAsync()
        {
            await Shell.Current.GoToAsync($"{nameof(LoginPage)}");
        }

        [RelayCommand]
        async Task HandleRegistrationAsync()
        {
            Debug.WriteLine("Submit");
        }
    }
}
