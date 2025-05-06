using System.Windows.Input;
using WhatMunch_MAUI.Models.Dtos;

namespace WhatMunch_MAUI.Views.Templates;

public partial class PlacesCollectionTemplate : ContentView
{
    public static readonly BindableProperty PlacesProperty =
        BindableProperty.Create(nameof(Places), typeof(IEnumerable<PlaceDto>), typeof(PlacesCollectionTemplate));

    public IEnumerable<PlaceDto> Places
    {
        get => (IEnumerable<PlaceDto>)GetValue(PlacesProperty);
        set => SetValue(PlacesProperty, value);
    }

    public static readonly BindableProperty HandleFavouriteCommandProperty =
        BindableProperty.Create(nameof(HandleFavouriteCommand), typeof(ICommand), typeof(PlacesCollectionTemplate));

    public ICommand HandleFavouriteCommand
    {
        get => (ICommand)GetValue(HandleFavouriteCommandProperty);
        set => SetValue(HandleFavouriteCommandProperty, value);
    }

    public static readonly BindableProperty GoToPlaceDetailsCommandProperty =
        BindableProperty.Create(nameof(GoToPlaceDetailsCommand), typeof(ICommand), typeof(PlacesCollectionTemplate));

    public ICommand GoToPlaceDetailsCommand
    {
        get => (ICommand)GetValue(GoToPlaceDetailsCommandProperty);
        set => SetValue(GoToPlaceDetailsCommandProperty, value);
    }

    public static readonly BindableProperty PageNameProperty =
    BindableProperty.Create(nameof(PageName), typeof(string), typeof(PlacesCollectionTemplate), default(string));

    public string PageName
    {
        get => (string)GetValue(PageNameProperty);
        set => SetValue(PageNameProperty, value);
    }

    public PlacesCollectionTemplate()
    {
        InitializeComponent();
    }

    private async void OnCardTapped(object? sender, TappedEventArgs e)
    {
        if (sender is Border border)
        {
            try
            {
                await border.FadeTo(0.6, 100);
                await border.FadeTo(1.0, 100);

                GoToPlaceDetailsCommand.Execute(e.Parameter);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
    }
}