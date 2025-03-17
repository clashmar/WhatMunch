using WhatMunch_MAUI.Services;

namespace WhatMunch_MAUI
{
    public partial class AppShell : Shell
    {
        Auth

        public AppShell()
        {
            InitializeComponent();
        }

        private async void CheckAuthentication()
        {
            bool isAuthenticated = await AuthService
        }
    }
}
