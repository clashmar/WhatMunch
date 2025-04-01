using WhatMunch_MAUI.Dtos;
using WhatMunch_MAUI.Models.Places;

namespace WhatMunch_MAUI.ViewModels
{
    [QueryProperty("Places", nameof(Places))]
    public partial class SearchResultsViewModel : BaseViewModel
    {
        [ObservableProperty]
        public ObservableCollection<Place>? _places;

        public SearchResultsViewModel()
        {
            
        }

        public void ResetViewModel()
        {
            Places.Clear();
        } 
    }
}
