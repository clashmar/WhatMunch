using Microsoft.Extensions.Logging;
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
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                if (string.IsNullOrWhiteSpace(url) || url == AppResources.NotAvailable)
                    return;

                url = url.StartsWith("http", StringComparison.OrdinalIgnoreCase) ? url : $"https://{url}";

                if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
                {
                    await Launcher.Default.OpenAsync(uri);
                }
                else
                {
                    _logger.LogWarning("Invalid URL: {Url}", url);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error opening website: {Url}", url);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task GoToPhone(string number)
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                if (string.IsNullOrWhiteSpace(number) || number == AppResources.NotAvailable)
                    return;

                var phoneUri = $"tel:{number}";
                await Launcher.Default.OpenAsync(phoneUri);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error opening phone number: {Number}", number);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task GoToMap()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                if (string.IsNullOrWhiteSpace(Place?.Id))
                {
                    _logger.LogWarning("Place ID is null or empty.");
                    return;
                }

                string uri = $"https://www.google.com/maps/place/?q=place_id:{Place?.Id}";
                await Launcher.Default.OpenAsync(uri);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error opening map for Place ID: {PlaceId}", Place?.Id);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public override void ResetViewModel()
        {

        }
    }
}
