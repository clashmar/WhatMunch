namespace WhatMunch_MAUI.Views;

public partial class PlaceDetailsPage : ContentPage
{
	private readonly PlaceDetailsViewModel _viewModel;
	public PlaceDetailsPage(PlaceDetailsViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.ResetViewModel();
    }
}