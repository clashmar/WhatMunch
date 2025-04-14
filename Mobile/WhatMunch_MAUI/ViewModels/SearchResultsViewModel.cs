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
        private readonly ILogger<SearchResultsViewModel> _logger;

        public SearchResultsViewModel(
            ISearchService searchService, 
            IShellService shellService,
            ILogger<SearchResultsViewModel> logger)
        {
            _searchService = searchService;
            _shellService = shellService;
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
                    PageList.Add([.. Places]);
                    NextPageToken = result.Data;
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
                    PageList.Add([..Places]);
                    HasPreviousPage = true;
                    currentPageIndex += 1; // Must be set before page token
                    NextPageToken = result.Data;
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
        private async Task GoToPlaceDetails(PlaceDto place)
        {
            if (place is null) return;

            await _shellService.GoToAsync($"{nameof(PlaceDetailsPage)}",
                        new Dictionary<string, object>
                        {
                                { "Place", place.ToModel() }
                            });
        }

        private async Task<Result<string?>> Search(string? pageToken = null)
        {
            try
            {
                IsBusy = true;

                var response = await _searchService.GetSearchResponseAsync(pageToken);

                if (response.Places.Count > 0)
                {
                    var places = response.Places.ToObservableCollection();

                    if(Places.Count != 0) Places.Clear();

                    foreach(var place in places)
                    {
                        Places.Add(place);
                    }

                    return Result<string?>.Success(response.NextPageToken);
                }
                else
                {
                    await _shellService.DisplayError(AppResources.NoPlacesFound);

                    return Result<string?>.Failure();
                }
            }
            catch (ConnectivityException)
            {
                await _shellService.DisplayError(AppResources.ErrorInternetConnection);
                return Result<string?>.Failure();
            }
            catch (HttpRequestException ex)
            {
                await _shellService.DisplayError(ex.Message ?? AppResources.ErrorUnexpected);

                return Result<string?>.Failure();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while executing search");
                await _shellService.DisplayError(AppResources.ErrorUnexpected);
                return Result<string?>.Failure();
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
            catch (Exception)
            {
                throw;
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
