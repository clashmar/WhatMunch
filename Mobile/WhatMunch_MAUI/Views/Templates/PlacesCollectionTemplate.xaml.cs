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

    public static readonly BindableProperty AddFavouriteCommandProperty =
        BindableProperty.Create(nameof(AddFavouriteCommand), typeof(ICommand), typeof(PlacesCollectionTemplate));

    public ICommand AddFavouriteCommand
    {
        get => (ICommand)GetValue(AddFavouriteCommandProperty);
        set => SetValue(AddFavouriteCommandProperty, value);
    }

    public static readonly BindableProperty GoToPlaceDetailsCommandProperty =
        BindableProperty.Create(nameof(GoToPlaceDetailsCommand), typeof(ICommand), typeof(PlacesCollectionTemplate));

    public ICommand GoToPlaceDetailsCommand
    {
        get => (ICommand)GetValue(GoToPlaceDetailsCommandProperty);
        set => SetValue(GoToPlaceDetailsCommandProperty, value);
    }

    public PlacesCollectionTemplate()
    {
        InitializeComponent();
    }
}