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
    [QueryProperty(nameof(Places), "Places")]
    [QueryProperty(nameof(NextPageToken), "NextPageToken")]
    public partial class SearchResultsViewModel : BaseViewModel
    {
        private readonly ISearchService _searchService;
        private readonly IShellService _shellService;
        private readonly IFavouritesService _favouritesService;
        private readonly ILogger<SearchResultsViewModel> _logger;

        public SearchResultsViewModel(
            ISearchService searchService, 
            IShellService shellService,
            IFavouritesService favouritesService,
            ILogger<SearchResultsViewModel> logger)
        {
            _searchService = searchService;
            _shellService = shellService;
            _favouritesService = favouritesService;
            _logger = logger;
        }

        [ObservableProperty]
        private ObservableCollection<PlaceDto> _places = [];
        public List<ObservableCollection<PlaceDto>> PageList { get; set; } = []; 

        private int currentPageIndex = 0;

        [ObservableProperty]
        private bool _hasPreviousPage;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasNextPage))]
        private string? _nextPageToken;
        public bool HasNextPage => !string.IsNullOrEmpty(NextPageToken) | PageList.ElementAtOrDefault(currentPageIndex + 1) is not null;

        //Called from code behind 
        public void InitializePageList()
        {
            if (PageList.Count == 0 && Places.Count > 0)
            {
                PageList.Add([..Places]);
            }
        }

        [RelayCommand]
        private async Task HandleRefresh()
        {
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
            if (!HasNextPage) return;

            if(PageList.ElementAtOrDefault(currentPageIndex + 1) is not null)
            {
                Places = PageList[currentPageIndex + 1];
                HasPreviousPage = true;
                currentPageIndex += 1;
            }
            else
            {
                var result = await Search(NextPageToken);

                if (result.IsSuccess)
                {
                    var data = result.Data!;
                    Places = data.Places.ToObservableCollection();
                    PageList.Add(data.Places.ToObservableCollection());
                    HasPreviousPage = true;
                    currentPageIndex += 1; // Must be set before page token
                    NextPageToken = data.NextPageToken;
                }
            }
        }

        [RelayCommand]
        private void HandlePrevious()
        {
            if (currentPageIndex < 1 || PageList.ElementAtOrDefault(currentPageIndex - 1) is null) return;
            Places = PageList[currentPageIndex - 1];
            currentPageIndex -= 1;
            if(currentPageIndex < 1) HasPreviousPage = false;
            OnPropertyChanged(nameof(HasNextPage));
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
        private async Task AddFavouriteAsync(PlaceDto place)
        {
            try
            {
                place.IsFavourite = !place.IsFavourite;
                await _favouritesService.SaveUserFavouriteAsync(place);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while trying to add place: {place.Id}", place.Id);
                await _shellService.DisplayError(AppResources.ErrorUnexpected);
            }
        }

        public void ResetPagination()
        {
            PageList.Clear();
            NextPageToken = null;
            HasPreviousPage = false;
            currentPageIndex = 0;
        }

        public override void ResetViewModel()
        {
            ResetPagination();
        } 
    }
}
