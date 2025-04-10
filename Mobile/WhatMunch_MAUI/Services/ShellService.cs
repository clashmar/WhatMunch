﻿namespace WhatMunch_MAUI.Services
{
    public interface IShellService
    {
        Task GoToAsync(string route);
        Task DisplayAlert(string title, string message, string accept);

        Task GoToAsync(string route, Dictionary<string, object> navigationParameter);
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

        public async Task DisplayAlert(string title, string message, string accept)
        {
            await Shell.Current.DisplayAlert(title, message, accept);
        }
    }
}
