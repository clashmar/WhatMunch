using System.Globalization;
using WhatMunch_MAUI.Resources.Localization;

namespace WhatMunch_MAUI.Converters
{
    public class SearchRadiusToStringConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is double searchRadius)
            {
                if(0 < searchRadius && searchRadius <= 400.0)
                {
                    return AppResources._5MinWalk;
                }
                else if(400.0 < searchRadius && searchRadius <= 800.0)
                {
                    return AppResources._10MinWalk;
                }
                else if(800.0 < searchRadius && searchRadius <= 1200.0)
                {
                    return AppResources._15MinWalk;
                }
                else if(1200 < searchRadius)
                {
                    return AppResources._20MinWalk;
                }
            }
            return "?";
        }
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return 800.0;
        }
    }
}
