using CommunityToolkit.Maui.Core.Extensions;
using Microsoft.Extensions.Logging;
using WhatMunch_MAUI.Models.Dtos;
using WhatMunch_MAUI.Services;

namespace WhatMunch_MAUI.ViewModels
{
    public partial class SavedPlacesViewModel : BaseViewModel
    {
        private readonly IFavouritesService _favouritesService;
        private readonly ILogger<SavedPlacesViewModel> _logger;

        public SavedPlacesViewModel(
            IFavouritesService favouritesService,
            ILogger<SavedPlacesViewModel> logger)
        {
            _favouritesService = favouritesService;
            _logger = logger;
        }

        [ObservableProperty]
        private ObservableCollection<PlaceDto> _favourites = [];

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


        public override void ResetViewModel()
        {
            throw new NotImplementedException();
        }
    }
}
