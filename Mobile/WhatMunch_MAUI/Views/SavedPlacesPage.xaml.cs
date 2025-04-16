namespace WhatMunch_MAUI.Views;

public partial class SavedPlacesPage : ContentPage
{

    private readonly SavedPlacesViewModel _viewModel;
    public SavedPlacesPage(SavedPlacesViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
	}

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        _viewModel.LoadFavouritesAsync();
    }
}