using WhatMunch_MAUI.Resources.Localization;

namespace WhatMunch_MAUI.Services
{
    public interface IShellService
    {
        Task GoToAsync(string route);
        Task DisplayAlert(string title, string message, string accept);
        Task GoToAsync(string route, Dictionary<string, object> navigationParameter);
        Task GoToAsync(string route, bool animate, Dictionary<string, object> navigationParameter);
        Task DisplayError(string message);
    }
    public class ShellService : IShellService
    {
        public async Task GoToAsync(string route)
        {
            await Shell.Current.GoToAsync(route);
        }

        public async Task GoToAsync(string route, Dictionary<string, object> navigationParameter)
        {
            await Shell.Current.GoToAsync(route, navigationParameter);
        }

        public async Task GoToAsync(string route, bool animate, Dictionary<string, object> navigationParameter)
        {
            await Shell.Current.GoToAsync(route, animate, navigationParameter);
        }

        public async Task DisplayAlert(string title, string message, string accept)
        {
            await Shell.Current.DisplayAlert(title, message, accept);
        }

        public async Task DisplayError(string message)
        {
            await DisplayAlert(AppResources.Error, message, AppResources.Ok);
        }
    }
}
