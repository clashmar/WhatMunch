namespace WhatMunch_MAUI.ViewModels
{
    public abstract partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        private bool _isBusy;

        public bool IsNotBusy => !IsBusy;

        [ObservableProperty]
        private bool _isRefreshing;

        [ObservableProperty]
        string? title;

        public abstract void ResetViewModel();
    }
}
