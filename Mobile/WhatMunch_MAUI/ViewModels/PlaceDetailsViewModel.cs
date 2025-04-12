using Microsoft.Extensions.Logging;
using WhatMunch_MAUI.Resources.Localization;
using WhatMunch_MAUI.Services;
using WhatMunch_MAUI.Views;

namespace WhatMunch_MAUI.ViewModels
{
    [QueryProperty(nameof(Place), "Place")]
    public partial class PlaceDetailsViewModel : BaseViewModel
    {
        private readonly ILauncher _launcher;
        private readonly IShellService _shellService;
        private readonly ILogger<PlaceDetailsViewModel> _logger;

        public PlaceDetailsViewModel(ILauncher launcher, IShellService shellService, ILogger<PlaceDetailsViewModel> logger)
        {
            _launcher = launcher;
            _shellService = shellService;
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
                    await _launcher.OpenAsync(uri);
                }
                else
                {
                    _logger.LogWarning("Invalid URL: {url}", url);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error opening website: {url}", url);
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

                var phoneUri = new Uri($"tel:{number}");
                await _launcher.OpenAsync(phoneUri);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error opening phone number: {number}", number);
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

                var uri = new Uri($"https://www.google.com/maps/place/?q=place_id:{Place?.Id}");
                await _launcher.OpenAsync(uri);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error opening map for Place ID: {Place?.Id}", Place?.Id);
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
