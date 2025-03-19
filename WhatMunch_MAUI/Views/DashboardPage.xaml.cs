namespace WhatMunch_MAUI.Pages;

public partial class DashboardPage : ContentPage
{
	private readonly DashboardViewModel ViewModel;
	public DashboardPage(DashboardViewModel viewModel)
	{
		InitializeComponent();
		ViewModel = viewModel;
		BindingContext = ViewModel;
	}
}