using System.Diagnostics.CodeAnalysis;
using WhatMunch_MAUI.Resources.Localization;

namespace WhatMunch_MAUI.Services
{
    public interface IShellService
    {
        Task GoToAsync(string route);
        Task DisplayAlert(string title, string message, string accept);
        Task GoToAsync(string route, Dictionary<string, object> navigationParameter);
        Task DisplayError(string message);
        Task<bool> CheckUserPrompt(string message);
    }
    public class ShellService : IShellService
    {
        [ExcludeFromCodeCoverage]
        public Func<string, Task> GoToAsyncMethod { get; set; }
            = (route) => Shell.Current.GoToAsync(route);

        public async Task GoToAsync(string route)
        {
            ArgumentException.ThrowIfNullOrEmpty(route);
            await GoToAsyncMethod.Invoke(route);
        }

        [ExcludeFromCodeCoverage]
        public Func<string, Dictionary<string, object>, Task> GoToAsyncWithParamsMethod { get; set; }
            = (route, navigationParameters) => Shell.Current.GoToAsync(route, navigationParameters);

        public async Task GoToAsync(string route, Dictionary<string, object> navigationParameters)
        {
            ArgumentException.ThrowIfNullOrEmpty(route);
            await GoToAsyncWithParamsMethod.Invoke(route, navigationParameters);
        }

        [ExcludeFromCodeCoverage]
        public Func<string, string, string, Task> DisplayAlertMethod { get; set; }
            = (title, message, accept) => Shell.Current.DisplayAlert(title, message, accept);

        public async Task DisplayAlert(string title, string message, string accept)
        {
            await DisplayAlertMethod.Invoke(title, message, accept);
        }

        public async Task DisplayError(string message)
        {
            await DisplayAlert(AppResources.Error, message, AppResources.Ok);
        }

        [ExcludeFromCodeCoverage]
        public Func<string, Task<bool>> CheckUserPromptMethod { get; set; }
            = (message) => Shell.Current.DisplayAlert(message, AppResources.AreYouSure, AppResources.Yes, AppResources.No);

        public async Task<bool> CheckUserPrompt(string message)
        {
            return await CheckUserPromptMethod.Invoke(message);
        }
    }
}
