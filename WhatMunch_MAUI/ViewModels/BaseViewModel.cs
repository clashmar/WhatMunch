namespace WhatMunch_MAUI.ViewModels
{
    public partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        bool isBusy;
        bool IsNotBusy => !IsBusy;

        [ObservableProperty]
        bool isRefreshing;

        [ObservableProperty]
        string? title;
    }
}
