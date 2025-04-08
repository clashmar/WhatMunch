namespace WhatMunch_MAUI.ViewModels
{
    [QueryProperty(nameof(Place), "Place")]
    public partial class PlaceDetailsViewModel : BaseViewModel
    {
        [ObservableProperty]
        private PlaceModel? _place;

        public override void ResetViewModel()
        {

        }
    }
}
