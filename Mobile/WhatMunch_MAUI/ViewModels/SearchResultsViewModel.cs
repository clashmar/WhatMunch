using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using WhatMunch_MAUI.Extensions;
using WhatMunch_MAUI.Models.Dtos;
using WhatMunch_MAUI.Resources.Localization;
using WhatMunch_MAUI.Services;
using WhatMunch_MAUI.Utility;
using WhatMunch_MAUI.Utility.Exceptions;
using WhatMunch_MAUI.Views;

namespace WhatMunch_MAUI.ViewModels
{
    public partial class SearchResultsViewModel : BaseViewModel, IQueryAttributable
    {
        private readonly ISearchService _searchService;
        private readonly IShellService _shellService;
        private readonly IFavouritesService _favouritesService;
        private readonly ILogger<SearchResultsViewModel> _logger;
        private readonly IDjangoApiService _djangoApiService;

        public SearchResultsViewModel(
            ISearchService searchService, 
            IShellService shellService,
            IFavouritesService favouritesService,
            ILogger<SearchResultsViewModel> logger,
            IDjangoApiService djangoApiService)
        {
            _searchService = searchService;
            _shellService = shellService;
            _favouritesService = favouritesService;
            _logger = logger;
            _djangoApiService = djangoApiService;
            IsActive = true;
        }

        [ObservableProperty]
        private ObservableCollection<PlaceDto> _places = [];
        public List<ObservableCollection<PlaceDto>> PageList { get; set; } = []; 
        public int CurrentPageIndex { get; set; } = 0;

        [ObservableProperty]
        private bool _hasPreviousPage;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasNextPage))]
        private string? _nextPageToken;
        public bool HasNextPage => !string.IsNullOrEmpty(NextPageToken) | PageList.ElementAtOrDefault(CurrentPageIndex + 1) is not null;

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            Places = query["Places"] as ObservableCollection<PlaceDto> ?? [];
            NextPageToken = query["NextPageToken"] as string;
        }

        //Called from code behind 
        public void InitializePageList()
        {
            if (PageList.Count == 0 && Places.Count > 0)
            {
                PageList.Add([..Places]);
            }

            Places = PageList[CurrentPageIndex];
        }

        [RelayCommand]
        private async Task HandleRefresh()
        {
            if (IsBusy) return;

            try
            {
                IsRefreshing = true;

                var result = await Search();

                if (result.IsSuccess)
                {
                    ResetPagination();
                    var data = result.Data!;
                    Places = data.Places.ToObservableCollection();
                    PageList.Add(data.Places.ToObservableCollection());
                    NextPageToken = data.NextPageToken;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while refreshing search results");
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        private async Task HandleNext()
        {
            if (IsBusy) return;
            if (!HasNextPage) return;

            if(PageList.ElementAtOrDefault(CurrentPageIndex + 1) is not null)
            {
                Places = PageList[CurrentPageIndex + 1];
                HasPreviousPage = true;
                CurrentPageIndex++;
            }
            else
            {
                var result = await Search(NextPageToken);

                if (result.IsSuccess)
                {
                    var data = result.Data!;
                    var newPage = data.Places.ToObservableCollection();
                    Places = newPage;
                    PageList.Add(newPage);
                    HasPreviousPage = true;
                    CurrentPageIndex++; // Must be set before page token
                    NextPageToken = data.NextPageToken;
                }
            }
        }

        [RelayCommand]
        private void HandlePrevious()
        {
            if(IsBusy) return;
            if (CurrentPageIndex < 1 || PageList.ElementAtOrDefault(CurrentPageIndex - 1) is null) return;
            Places = PageList[CurrentPageIndex - 1];
            CurrentPageIndex -= 1;
            if(CurrentPageIndex < 1) HasPreviousPage = false;
            OnPropertyChanged(nameof(HasNextPage));
        }

        [RelayCommand]
        public async Task GoToPlaceDetails(PlaceDto place)
        {
            if (place is null) return;
            IsBusy = true;

            try
            {
                string apiKey = await _djangoApiService.GetGoogleMapsApiKeyAsync();

                await _shellService.GoToAsync($"{nameof(PlaceDetailsPage)}",
                        new Dictionary<string, object>
                        {
                            { "Place", place.ToPlaceModel(apiKey) }
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

        private async Task<Result<TextSearchResponseDto>> Search(string? pageToken = null)
        {
            try
            {
                IsBusy = true;
                var response = await _searchService.GetSearchResponseAsync(pageToken);

                if (response.Places.Count > 0)
                {
                    return Result<TextSearchResponseDto>.Success(response);
                }
                else
                {
                    await _shellService.DisplayError(AppResources.NoPlacesFound);
                    return Result<TextSearchResponseDto>.Failure();
                }
            }
            catch (ConnectivityException)
            {
                await _shellService.DisplayError(AppResources.ErrorInternetConnection);
                return Result<TextSearchResponseDto>.Failure();
            }
            catch (HttpRequestException ex)
            {
                await _shellService.DisplayError(ex.Message ?? AppResources.ErrorUnexpected);

                return Result<TextSearchResponseDto>.Failure();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while executing search");
                await _shellService.DisplayError(AppResources.ErrorUnexpected);
                return Result<TextSearchResponseDto>.Failure();
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task GoBackAsync()
        {
            try
            {
                ResetViewModel();
                await _shellService.GoToAsync("..");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while trying to go back");
            }
        }

        [RelayCommand]
        public async Task ToggleFavouriteAsync(PlaceDto place)
        {
            try
            {
                place.IsFavourite = !place.IsFavourite;
                WeakReferenceMessenger.Default.Send(new FavouritesChangedMessage(string.Empty));
                if (place.IsFavourite)
                {
                    await _favouritesService.SaveUserFavouriteAsync(place);
                }
                else
                {
                    await _favouritesService.DeleteUserFavouriteAsync(place);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while trying to add place: {Place}", place.Id);
                await _shellService.DisplayError(AppResources.ErrorUnexpected);
            }
        }

        protected override void OnActivated()
        {
            WeakReferenceMessenger.Default.Register<SearchResultsViewModel, FavouriteDeletedMessage>(this, (r, m) => {
                var place = PageList
                    .SelectMany(page => page)
                    .FirstOrDefault(place => place.Id == m.Value);

                if (place is not null) 
                    place.IsFavourite = false;
            });

            WeakReferenceMessenger.Default.Register<SearchResultsViewModel, AllFavouritesDeletedMessage>(this, (r, m) => {
                PageList.ForEach(page => page.ToList().ForEach(place => place.IsFavourite = false));
            });
        }

        public void ResetPagination()
        {
            PageList.Clear();
            NextPageToken = null;
            HasPreviousPage = false;
            CurrentPageIndex = 0;
        }

        public override void ResetViewModel()
        {
            ResetPagination();
        } 
    }
}
