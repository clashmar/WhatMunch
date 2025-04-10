using System.Globalization;

namespace WhatMunch_MAUI.Converters
{
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value != null && !string.IsNullOrEmpty(value.ToString());
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value!;
        }
    }
}
