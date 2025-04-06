using CommunityToolkit.Maui.Core.Extensions;
using Microsoft.Extensions.Logging;
using WhatMunch_MAUI.Models.Places;
using WhatMunch_MAUI.Resources.Localization;
using WhatMunch_MAUI.Services;
using WhatMunch_MAUI.Utility;
using WhatMunch_MAUI.Utility.Exceptions;

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
        public ObservableCollection<Place> _places = [];

        public List<ObservableCollection<Place>> PageList = []; // First place added in code behind

        private int currentPageIndex = 0;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(NextButtonColumnSpan))]
        public bool _hasPreviousPage;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasNextPage))]
        private string? _nextPageToken;
        public bool HasNextPage => !string.IsNullOrEmpty(NextPageToken) | PageList.ElementAtOrDefault(currentPageIndex + 1) is not null;
        public int NextButtonColumnSpan => HasPreviousPage ? 1 : 2;

        

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
                    NextPageToken = result.Data;
                    HasPreviousPage = false;
                    currentPageIndex = 0;
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
        private async Task HandleSearchNext()
        {
            if (!HasNextPage) return;

            IsBusy = true;
            if(PageList.ElementAtOrDefault(currentPageIndex + 1) is not null)
            {
                Places.Clear();

                foreach (var place in PageList[currentPageIndex + 1])
                {
                    Places.Add(place);
                }

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
            IsBusy = false;
        }

        [RelayCommand]
        private void HandleSearchPrevious()
        {
            IsBusy = true;
            if (currentPageIndex < 1 | PageList.ElementAtOrDefault(currentPageIndex - 1) is null) return;

            Places.Clear();

            foreach(var place in PageList[currentPageIndex - 1])
            {
                Places.Add(place);
            }

            currentPageIndex -= 1;

            if(currentPageIndex < 1) HasPreviousPage = false;
            OnPropertyChanged(nameof(HasNextPage));
            IsBusy = false;
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
                    await _shellService.DisplayAlert(
                        AppResources.Error,
                        AppResources.NoPlacesFound,
                        AppResources.Ok);

                    return Result<string?>.Failure();
                }
            }
            catch (ConnectivityException)
            {
                await _shellService.DisplayAlert(AppResources.Error, AppResources.ErrorInternetConnection, AppResources.Ok);
                return Result<string?>.Failure();
            }
            catch (HttpRequestException ex)
            {
                await _shellService.DisplayAlert(
                        AppResources.Error,
                        ex.Message ?? AppResources.ErrorUnexpected,
                        AppResources.Ok);

                return Result<string?>.Failure();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while executing search");
                await _shellService.DisplayAlert(AppResources.Error, AppResources.ErrorUnexpected, AppResources.Ok);
                return Result<string?>.Failure();
            }
            finally
            {
                IsBusy = false;
            }
        }
        public void ResetPagination()
        {
            NextPageToken = null;
            HasPreviousPage = false;
            currentPageIndex = 0;
        }

        public void ResetViewModel()
        {
            Places?.Clear();
            ResetPagination();
        } 
    }
}
