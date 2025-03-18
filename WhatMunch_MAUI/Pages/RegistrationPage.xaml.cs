namespace WhatMunch_MAUI.Pages;

public partial class RegistrationPage : ContentPage
{
	public RegistrationPage(RegistrationViewModel viewModel)
	{
		InitializeComponent();
        Shell.SetNavBarIsVisible(this, false);
		BindingContext = viewModel;
    }
}