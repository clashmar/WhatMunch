using System.Globalization;
using WhatMunch_MAUI.Models.Fonts;

namespace WhatMunch_MAUI.Converters
{
    public class IntToStarsConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int rating && rating >= 0 && rating <= 5)
            {
                return new string(FaSolid.Star[0], rating);
            }

            return string.Empty;
        }
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return 0;
        }
    }

    public class IntToEmptyStarsConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int rating && rating >= 0 && rating <= 5)
            {
                return new string(FaSolid.Star[0], 5 - rating);
            }

            return string.Empty;
        }
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return 0;
        }
    }
}
