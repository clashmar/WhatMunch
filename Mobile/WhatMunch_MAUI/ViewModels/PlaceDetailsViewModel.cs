using Microsoft.Extensions.Logging;
using System;
using WhatMunch_MAUI.Resources.Localization;

namespace WhatMunch_MAUI.ViewModels
{
    [QueryProperty(nameof(Place), "Place")]
    public partial class PlaceDetailsViewModel : BaseViewModel
    {
        private readonly ILogger<PlaceDetailsViewModel> _logger;

        public PlaceDetailsViewModel(ILogger<PlaceDetailsViewModel> logger)
        {
            _logger = logger;
        }

        [ObservableProperty]
        private PlaceModel? _place;

        [RelayCommand]
        private async Task GoToWebsite(string url)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(url) || url.SequenceEqual(AppResources.NotAvailable))
                    return;

                if (!url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                    url = $"https://{url}";

                Uri uri = new(url);
                await Launcher.Default.OpenAsync(uri);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while accessing website.");
            }
        }

        [RelayCommand]
        private async Task GoToPhone(string number)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(number) || number.SequenceEqual(AppResources.NotAvailable))
                    return;

                var phoneUri = $"tel:{number}";
                await Launcher.Default.OpenAsync(phoneUri);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while accessing phone.");
            }
        }

        [RelayCommand]
        private async Task GoToMap()
        {
            try
            {
                string uri = $"https://www.google.com/maps/place/?q=place_id:{Place?.Id}";
                await Launcher.Default.OpenAsync(uri);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while accessing map.");
            }
        }

        public override void ResetViewModel()
        {

        }
    }
}
