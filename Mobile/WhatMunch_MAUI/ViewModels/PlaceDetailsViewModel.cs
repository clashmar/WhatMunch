using System;

namespace WhatMunch_MAUI.ViewModels
{
    [QueryProperty(nameof(Place), "Place")]
    public partial class PlaceDetailsViewModel : BaseViewModel
    {
        [ObservableProperty]
        private PlaceModel? _place;

        [RelayCommand]
        private async void GoToWebsite(string url)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(url))
                    return;

                if (!url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                    url = $"https://{url}";

                Uri uri = new(url);
                await Launcher.Default.OpenAsync(uri);
            }
            catch (Exception)
            {
                
            }
        }

        [RelayCommand]
        private async void GoToPhone(string number)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(number))
                {
                    var phoneUri = $"tel:{number}";
                    await Launcher.Default.OpenAsync(phoneUri);
                }
            }
            catch (Exception)
            {

            }
        }

        public override void ResetViewModel()
        {

        }
    }
}
