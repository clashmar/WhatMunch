using WhatMunch_MAUI.Dtos;
using WhatMunch_MAUI.Models.Places;

namespace WhatMunch_MAUI.ViewModels
{
    [QueryProperty(nameof(NearbySearchResponseDto), nameof(Places))]
    public partial class SearchResultsViewModel : BaseViewModel
    {
        public readonly ObservableCollection<Place> Places = [];

        public SearchResultsViewModel()
        {
            
        }

        public void ResetViewModel()
        {

        } 
    }
}
