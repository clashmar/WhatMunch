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
        private string? _title;

        public abstract void ResetViewModel();
    }
}
