namespace WhatMunch_MAUI.Views;

public partial class SearchPreferencesPage : ContentPage
{
	private readonly SearchPreferencesViewModel _viewModel;
	public SearchPreferencesPage(SearchPreferencesViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = _viewModel;
	}

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
		_viewModel.LoadPreferencesAsync();
    }
}