namespace WhatMunch_MAUI.Views;

public partial class SearchResultsPage : ContentPage
{
    private readonly SearchResultsViewModel _viewModel;
    public SearchResultsPage(SearchResultsViewModel viewModel)
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