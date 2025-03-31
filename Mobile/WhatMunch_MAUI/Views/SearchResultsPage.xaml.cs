namespace WhatMunch_MAUI.Views;

public partial class SearchResultsPage : ContentPage
{
    private readonly SearchResultsViewModel ViewModel;
    public SearchResultsPage(SearchResultsViewModel viewModel)
	{
		InitializeComponent();
        ViewModel = viewModel;
        BindingContext = ViewModel;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        ViewModel.ResetViewModel();
    }
}