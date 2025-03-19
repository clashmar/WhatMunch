using WhatMunch_MAUI.Services;
using WhatMunch_MAUI.Views;

namespace WhatMunch_MAUI.ViewModels
{
    public partial class DashboardViewModel(IAuthService authService) : BaseViewModel
    {
        private readonly IAuthService _authservice = authService;

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
