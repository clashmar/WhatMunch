namespace WhatMunch_MAUI.Views;

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