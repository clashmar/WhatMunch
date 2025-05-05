using System.Windows.Input;

namespace WhatMunch_MAUI.Views.Controls;

public partial class FloatingEntry : ContentView
{
    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(FloatingEntry), string.Empty, BindingMode.TwoWay);

    public static readonly BindableProperty PlaceholderProperty =
        BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(FloatingEntry), string.Empty);

    public static readonly BindableProperty LabelProperty =
        BindableProperty.Create(nameof(Label), typeof(string), typeof(FloatingEntry), string.Empty);

    public static readonly BindableProperty IsPasswordProperty =
        BindableProperty.Create(nameof(IsPassword), typeof(bool), typeof(FloatingEntry), false);

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public string Placeholder
    {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    public string Label
    {
        get => (string)GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public bool IsPassword
    {
        get => (bool)GetValue(IsPasswordProperty);
        set => SetValue(IsPasswordProperty, value);
    }

    public static readonly BindableProperty CompletedCommandProperty =
    BindableProperty.Create(nameof(CompletedCommand), typeof(ICommand), typeof(FloatingEntry));

    public ICommand CompletedCommand
    {
        get => (ICommand)GetValue(CompletedCommandProperty);
        set => SetValue(CompletedCommandProperty, value);
    }

    public FloatingEntry()
	{
		InitializeComponent();

        EntryControl.Completed += OnEntryCompleted;
    }

    private void OnEntryCompleted(object? sender, EventArgs e)
    {
        if (CompletedCommand?.CanExecute(null) == true)
        {
            CompletedCommand.Execute(null);
        }
    }

    protected override void OnParentSet()
    {
        base.OnParentSet();
        if (Parent == null)
        {
            EntryControl.Completed -= OnEntryCompleted;
        }
    }
}