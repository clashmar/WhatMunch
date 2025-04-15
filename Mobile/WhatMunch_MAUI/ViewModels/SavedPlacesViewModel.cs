using Microsoft.Extensions.Logging;
using WhatMunch_MAUI.Extensions;
using WhatMunch_MAUI.Models.Dtos;
using WhatMunch_MAUI.Services;
using WhatMunch_MAUI.Views;

namespace WhatMunch_MAUI.ViewModels
{
    public partial class SavedPlacesViewModel : BaseViewModel
    {
        private readonly IFavouritesService _favouritesService;
        private readonly IShellService _shellService;
        private readonly ILogger<SavedPlacesViewModel> _logger;

        public SavedPlacesViewModel(
            IFavouritesService favouritesService,
            IShellService shellService,
            ILogger<SavedPlacesViewModel> logger)
        {
            _favouritesService = favouritesService;
            _shellService = shellService;
            _logger = logger;
        }

        [ObservableProperty]
        private ObservableCollection<PlaceDto?> _favourites = [];

        public async void LoadFavouritesAsync()
        {
            try
            {
                var result = await _favouritesService.GetUserFavouritesAsync();

                if (result.IsSuccess && result.Data is not null)
                {
                    Favourites = result.Data.ToObservableCollection();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [RelayCommand]
        public async Task GoToPlaceDetails(PlaceDto place)
        {
            if (place is null) return;
            IsBusy = true;

            try
            {
                await _shellService.GoToAsync($"{nameof(PlaceDetailsPage)}",
                        new Dictionary<string, object>
                        {
                            { "Place", place.ToModel() }
                        });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while navigating to {place}", place);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public override void ResetViewModel()
        {
            throw new NotImplementedException();
        }
    }
}
