namespace WhatMunch_MAUI.Services
{
    public interface IShellService
    {
        Task GoToAsync(string route);
        Task DisplayAlert(string title, string message, string accept);
    }
    public class ShellService : IShellService
    {
        public async Task GoToAsync(string route)
        {
            await Shell.Current.GoToAsync(route);
        }

        public async Task DisplayAlert(string title, string message, string accept)
        {
            await Shell.Current.DisplayAlert(title, message, accept);
        }
    }
}
