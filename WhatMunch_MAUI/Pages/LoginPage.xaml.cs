namespace WhatMunch_MAUI.Pages;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
		Shell.SetNavBarIsVisible(this, false);
	}

	protected override bool OnBackButtonPressed() => true;
}