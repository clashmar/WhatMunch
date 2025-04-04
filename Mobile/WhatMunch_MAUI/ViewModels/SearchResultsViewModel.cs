using WhatMunch_MAUI.Models.Places;

namespace WhatMunch_MAUI.ViewModels
{
    [QueryProperty("Places", nameof(Places))]
    [QueryProperty("PageToken", nameof(PageToken))]
    public partial class SearchResultsViewModel : BaseViewModel
    {
        [ObservableProperty]
        public ObservableCollection<Place>? _places;

        public string? PageToken;

        public SearchResultsViewModel()
        {
            
        }

        public void ResetViewModel()
        {
            Places?.Clear();
        } 
    }
}
