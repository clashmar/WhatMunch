namespace WhatMunch_MAUI.Views;

public partial class LoginPage : ContentPage
{
	private readonly LoginViewModel ViewModel;

	public LoginPage(LoginViewModel viewModel)
	{
		InitializeComponent();
		Shell.SetNavBarIsVisible(this, false);
		ViewModel = viewModel;
		BindingContext = ViewModel;
	}

	protected override bool OnBackButtonPressed() => true;

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        ViewModel.ResetViewModel();
    }
}