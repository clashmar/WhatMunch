namespace WhatMunch_MAUI.Views;

public partial class SavedPlacesPage : ContentPage
{

    private readonly SavedPlacesViewModel _viewModel;
    public SavedPlacesPage(SavedPlacesViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.IsActive = true;
	}

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        await _viewModel.LoadFavouritesAsync();
        _viewModel.ShouldNotLoad = false;
    }
}