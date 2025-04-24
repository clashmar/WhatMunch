using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.Extensions.Logging;
using WhatMunch_MAUI.Extensions;
using WhatMunch_MAUI.Models.Dtos;
using WhatMunch_MAUI.Resources.Localization;
using WhatMunch_MAUI.Services;
using WhatMunch_MAUI.Views;

namespace WhatMunch_MAUI.ViewModels
{
    public partial class SavedPlacesViewModel(
        IFavouritesService favouritesService,
        IShellService shellService,
        ILogger<SavedPlacesViewModel> logger) : BaseViewModel
    {
        [ObservableProperty]
        private ObservableCollection<PlaceDto> _favourites = [];

        public bool ShouldNotLoad { get; set; } 

        public async Task LoadFavouritesAsync()
        {
            if (IsBusy || ShouldNotLoad) return;
            IsBusy = true;

            try
            {
                var result = await favouritesService.GetUserFavouritesAsync();

                if (result.IsSuccess && result.Data is not null)
                {
                    Favourites = result.Data.ToObservableCollection();
                    IsBusy = false;

                    // TODO: Update tests for GetUserFavouritesAsync
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            var updatedList = await favouritesService.UpdateFavouritesAsync(result.Data);
                            MainThread.BeginInvokeOnMainThread(() =>
                            {
                                Favourites = updatedList.ToObservableCollection();
                            });
                        }
                        catch (Exception ex)
                        {
                            logger.LogWarning(ex, "Background refresh of favourites failed.");
                        }
                    });
                }
                else
                {
                    Favourites.Clear();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to load favorites.");
                await shellService.DisplayError(AppResources.ErrorUnexpected);
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
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                await shellService.GoToAsync($"{nameof(PlaceDetailsPage)}",
                        new Dictionary<string, object>
                        {
                            { "Place", place.ToPlaceModel() }
                        });

                ShouldNotLoad = true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unexpected error occurred while navigating to {place}", place);
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task DeleteFavouriteAsync(PlaceDto place)
        {
            if (place is null) return;

            try
            {
                bool isConfirmed = await shellService.CheckUserPrompt(AppResources.DeleteFavourite);
                if (!isConfirmed) return;

                await favouritesService.DeleteUserFavouriteAsync(place);
                Favourites.Remove(place);
                WeakReferenceMessenger.Default.Send(new FavouriteDeletedMessage(place.Id));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unexpected error occurred while trying to add place: {place.Id}", place.Id);
                await shellService.DisplayError(AppResources.ErrorUnexpected);
            }
        }

        [RelayCommand]
        public async Task DeleteAllFavouritesAsync()
        {
            try
            {
                bool isConfirmed = await shellService.CheckUserPrompt(AppResources.DeleteAll);
                if (!isConfirmed) return;

                await favouritesService.DeleteAllUserFavouritesAsync();
                Favourites.Clear();
                WeakReferenceMessenger.Default.Send(new AllFavouritesDeletedMessage(string.Empty));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unexpected error occurred while trying to delete favourites");
                await shellService.DisplayError(AppResources.ErrorUnexpected);
            }
        }

        protected override void OnActivated()
        {
            WeakReferenceMessenger.Default.Register<SavedPlacesViewModel, FavouritesChangedMessage>(this, (r, m) => {
                ShouldNotLoad = false;
            });
        }

        public override void ResetViewModel()
        {
            throw new NotImplementedException();
        }
    }

    public sealed class FavouriteDeletedMessage(string value) : ValueChangedMessage<string>(value)
    {
    }

    public sealed class AllFavouritesDeletedMessage(string value) : ValueChangedMessage<string>(value)
    {
    }
}
