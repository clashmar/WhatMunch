using System.Globalization;

namespace WhatMunch_MAUI.Converters
{
    public class EnumToIntConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is Enum enumValue)
            {
                return System.Convert.ToInt32(enumValue);
            }
            return 0;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is double doubleValue)
            {
                int intValue = (int)Math.Round(doubleValue);
                return Enum.ToObject(typeof(PriceLevel), intValue);
            }
            return PriceLevel.PRICE_LEVEL_INEXPENSIVE;
        }
    }
}
