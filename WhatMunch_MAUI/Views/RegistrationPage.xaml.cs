namespace WhatMunch_MAUI.Views;

public partial class RegistrationPage : ContentPage
{
    readonly RegistrationViewModel ViewModel;
	public RegistrationPage(RegistrationViewModel viewModel)
	{
		InitializeComponent();
        Shell.SetNavBarIsVisible(this, false);
        ViewModel = viewModel;
		BindingContext = ViewModel;
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        ViewModel.ResetViewModel();
    }
}