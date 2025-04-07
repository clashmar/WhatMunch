using WhatMunch_MAUI.Models.Places;

namespace WhatMunch_MAUI.ViewModels
{
    [QueryProperty(nameof(Place), "Place")]
    public partial class PlaceDetailsViewModel : BaseViewModel
    {
        [ObservableProperty]
        private Place? _place;

        public override void ResetViewModel()
        {

        }
    }
}
