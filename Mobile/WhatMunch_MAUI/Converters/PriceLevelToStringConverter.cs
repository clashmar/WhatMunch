using System.Globalization;

namespace WhatMunch_MAUI.Converters
{
    public class PriceLevelToStringConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is PriceLevel priceLevel)
            {
                return priceLevel switch
                {
                    PriceLevel.PRICE_LEVEL_INEXPENSIVE => "Inexpensive $",
                    PriceLevel.PRICE_LEVEL_MODERATE => "Moderate $$",
                    PriceLevel.PRICE_LEVEL_EXPENSIVE => "Expensive $$$",
                    PriceLevel.PRICE_LEVEL_VERY_EXPENSIVE => "Very Expensive $$$$",
                    _ => "?"
                };
            }
            return "?";
        }
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string priceLevel)
            {
                return priceLevel switch
                {
                    "Inexpensive $" => PriceLevel.PRICE_LEVEL_INEXPENSIVE,
                    "Moderate $$" => PriceLevel.PRICE_LEVEL_MODERATE,
                    "Expensive $$$" => PriceLevel.PRICE_LEVEL_EXPENSIVE,
                    "Very Expensive $$$$" => PriceLevel.PRICE_LEVEL_VERY_EXPENSIVE,
                    _ => PriceLevel.PRICE_LEVEL_VERY_EXPENSIVE
                };
            }
            return PriceLevel.PRICE_LEVEL_VERY_EXPENSIVE;
        }
    }
}
