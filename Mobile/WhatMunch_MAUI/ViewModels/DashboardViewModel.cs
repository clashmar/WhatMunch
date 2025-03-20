using WhatMunch_MAUI.Services;
using WhatMunch_MAUI.Views;

namespace WhatMunch_MAUI.ViewModels
{
    public partial class DashboardViewModel(ITokenService authService) : BaseViewModel
    {
        private readonly ITokenService _authservice = authService;

        [RelayCommand]
        async Task HandleLogout()
        {
            try
            {
                _authservice.Logout();
                await Shell.Current.GoToAsync($"{nameof(LoginPage)}");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
