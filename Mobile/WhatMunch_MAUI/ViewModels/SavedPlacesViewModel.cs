﻿using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.Extensions.Logging;
using WhatMunch_MAUI.Extensions;
using WhatMunch_MAUI.Models.Dtos;
using WhatMunch_MAUI.Resources.Localization;
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
        private ObservableCollection<PlaceDto> _favourites = [];

        public async void LoadFavouritesAsync()
        {
            IsBusy = true;

            try
            {
                var result = await _favouritesService.GetUserFavouritesAsync();

                if (result.IsSuccess && result.Data is not null)
                {
                    Favourites = result.Data.ToObservableCollection();
                }
                else
                {
                    Favourites.Clear();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                IsBusy = false;
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

        [RelayCommand]
        private async Task DeleteFavouriteAsync(PlaceDto place)
        {
            try
            {
                bool isConfirmed = await _shellService.CheckUserPrompt(AppResources.DeleteFavourite);
                if (!isConfirmed) return;

                await _favouritesService.DeleteUserFavouriteAsync(place);
                Favourites.Remove(place);
                WeakReferenceMessenger.Default.Send(new FavouriteDeletedMessage(place.Id));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while trying to add place: {place.Id}", place.Id);
                await _shellService.DisplayError(AppResources.ErrorUnexpected);
            }
        }

        [RelayCommand]
        private async Task DeleteAllFavouritesAsync()
        {
            try
            {
                bool isConfirmed = await _shellService.CheckUserPrompt(AppResources.DeleteAll);
                if (!isConfirmed) return;

                await _favouritesService.DeleteAllUserFavouritesAsync();
                Favourites.Clear();
                WeakReferenceMessenger.Default.Send(new AllFavouritesDeletedMessage(string.Empty));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while trying to delete favourites");
                await _shellService.DisplayError(AppResources.ErrorUnexpected);
            }
        }


        public override void ResetViewModel()
        {
            throw new NotImplementedException();
        }
    }

    public sealed class FavouriteDeletedMessage : ValueChangedMessage<string>
    {
        public FavouriteDeletedMessage(string value) : base(value)
        {
        }
    }

    public sealed class AllFavouritesDeletedMessage : ValueChangedMessage<string>
    {
        public AllFavouritesDeletedMessage(string value) : base(value)
        {
        }
    }
}
