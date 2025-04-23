using System.Windows.Input;

namespace WhatMunch_MAUI.Views.Controls;
public partial class SocialButton : ContentView
{
    public static readonly BindableProperty IconProperty =
        BindableProperty.Create(nameof(Icon), typeof(string), typeof(SocialButton), string.Empty);

    public string Icon
    {
        get => (string)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(SocialButton), string.Empty);

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly BindableProperty LoginCommandProperty =
        BindableProperty.Create(nameof(LoginCommand), typeof(ICommand), typeof(SocialButton));

    public ICommand LoginCommand
    {
        get => (ICommand)GetValue(LoginCommandProperty);
        set => SetValue(LoginCommandProperty, value);
    }

    public SocialButton()
	{
		InitializeComponent();
	}
}