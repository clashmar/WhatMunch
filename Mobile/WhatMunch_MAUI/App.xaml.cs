using Microsoft.Maui.Controls.PlatformConfiguration;

namespace WhatMunch_MAUI
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            AppShell appShell = IPlatformApplication.Current?.Services.GetService<AppShell>()!;

            if (appShell == null)
                throw new Exception("AppShell is not registered in the DI container!");

            return new Window(appShell);
        }

        
    }
}