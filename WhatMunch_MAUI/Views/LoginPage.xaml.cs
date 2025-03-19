namespace WhatMunch_MAUI.Views;

public partial class LoginPage : ContentPage
{
	public LoginPage(LoginViewModel viewModel)
	{
		InitializeComponent();
		Shell.SetNavBarIsVisible(this, false);
		BindingContext = viewModel;
	}

	protected override bool OnBackButtonPressed() => true;
}