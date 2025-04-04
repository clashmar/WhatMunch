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

	private void OnMaxPriceChanged(object sender, ValueChangedEventArgs e)
	{
        if(sender is Slider slider)
        {
            var snapped = Math.Round(e.NewValue);
            if (snapped != e.NewValue)
            {
                slider.Value = snapped;
            }
        }
    }

    private void OnMinRatingChanged(object sender, ValueChangedEventArgs e)
    {
        if (sender is Slider slider)
        {
            double snapped = Math.Round(e.NewValue * 2) / 2.0;
            if (slider.Value != snapped)
            {
                slider.Value = snapped;
            }
        }
    }
}