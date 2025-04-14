using System.Globalization;

namespace WhatMunch_MAUI.Converters
{
    public class KilometersToLocalizedDistanceConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not double km)
                return "";

            if (km < 1.0)
            {
                double meters = km * 1000;
                return string.Format(culture, "{0:N0}m", meters);
            }
            else
            {
                return string.Format(culture, "{0:N2}km", km);
            }
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
