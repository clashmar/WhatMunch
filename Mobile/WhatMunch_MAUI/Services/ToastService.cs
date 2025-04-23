using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace WhatMunch_MAUI.Services
{
    public interface IToastService
    {
        Task DisplayToast(string text);
    }
    public class ToastService : IToastService
    {
        public async Task DisplayToast(string text)
        {
            CancellationTokenSource cancellationTokenSource = new();
            ToastDuration duration = ToastDuration.Short;
            double fontSize = 14;
            var toast = Toast.Make(text, duration, fontSize);
            await toast.Show(cancellationTokenSource.Token);
        }
    }
}
