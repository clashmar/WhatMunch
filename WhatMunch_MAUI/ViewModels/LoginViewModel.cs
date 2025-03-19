using WhatMunch_MAUI.Views;

namespace WhatMunch_MAUI.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {

        [RelayCommand]
        async Task HandleLoginAsync()
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

        [RelayCommand]
        async Task GoToRegistrationPageAsync()
        {
            await Shell.Current.GoToAsync($"{nameof(RegistrationPage)}");
        }
    }
}
