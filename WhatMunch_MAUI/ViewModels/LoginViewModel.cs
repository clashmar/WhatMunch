using WhatMunch_MAUI.Pages;

namespace WhatMunch_MAUI.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        [RelayCommand]
        async Task GoToRegistrationPageAsync()
        {
            await Shell.Current.GoToAsync($"{nameof(RegistrationPage)}");
        }
    }
}
