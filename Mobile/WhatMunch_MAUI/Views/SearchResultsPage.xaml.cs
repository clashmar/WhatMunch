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

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (_viewModel.PageList.Count == 0 && _viewModel.Places.Count > 0)
        {
            _viewModel.PageList.Add([.. _viewModel.Places]);
        }

        _viewModel.ShouldReset = true;
    }

    protected override void OnDisappearing()
    {
        if (_viewModel.ShouldReset)
        {
            _viewModel.ResetViewModel();
        }

        base.OnDisappearing();
    }
}