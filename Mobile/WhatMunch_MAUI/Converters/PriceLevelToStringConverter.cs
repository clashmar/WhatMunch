using System.Globalization;
using WhatMunch_MAUI.Resources.Localization;

namespace WhatMunch_MAUI.Converters
{
    public class PriceLevelToStringConverter : IValueConverter
    {
        private readonly string _inexpensive = $"{AppResources.Inexpensive} $";
        private readonly string _moderate = $"{AppResources.Moderate} $$";
        private readonly string _expensive = $"{AppResources.Expensive} $$$";
        private readonly string _veryExpensive = $"{AppResources.VeryExpensive} $$$$";

        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is PriceLevel priceLevel)
            {
                return priceLevel switch
                {
                    PriceLevel.PRICE_LEVEL_INEXPENSIVE => _inexpensive,
                    PriceLevel.PRICE_LEVEL_MODERATE => _moderate,
                    PriceLevel.PRICE_LEVEL_EXPENSIVE => _expensive,
                    PriceLevel.PRICE_LEVEL_VERY_EXPENSIVE => _veryExpensive,
                    _ => "?"
                };
            }
            return "?";
        }
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string priceLevel)
            {
                if (priceLevel == _inexpensive)
                    return PriceLevel.PRICE_LEVEL_INEXPENSIVE;
                else if (priceLevel == _moderate)
                    return PriceLevel.PRICE_LEVEL_MODERATE;
                else if (priceLevel == _expensive)
                    return PriceLevel.PRICE_LEVEL_EXPENSIVE;
                else if (priceLevel == _veryExpensive)
                    return PriceLevel.PRICE_LEVEL_VERY_EXPENSIVE;
                else
                    return PriceLevel.PRICE_LEVEL_VERY_EXPENSIVE;
            }
            return PriceLevel.PRICE_LEVEL_VERY_EXPENSIVE;
        }
    }

    public class PriceLevelToDollarSignConverter : IValueConverter
    {
        private readonly string _inexpensive = "$";
        private readonly string _moderate = "$$";
        private readonly string _expensive = "$$$";
        private readonly string _veryExpensive = "$$$$";

        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is PriceLevel priceLevel)
            {
                return priceLevel switch
                {
                    PriceLevel.PRICE_LEVEL_INEXPENSIVE => _inexpensive,
                    PriceLevel.PRICE_LEVEL_MODERATE => _moderate,
                    PriceLevel.PRICE_LEVEL_EXPENSIVE => _expensive,
                    PriceLevel.PRICE_LEVEL_VERY_EXPENSIVE => _veryExpensive,
                    _ => "?"
                };
            }
            return "?";
        }
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string priceLevel)
            {
                if (priceLevel == _inexpensive)
                    return PriceLevel.PRICE_LEVEL_INEXPENSIVE;
                else if (priceLevel == _moderate)
                    return PriceLevel.PRICE_LEVEL_MODERATE;
                else if (priceLevel == _expensive)
                    return PriceLevel.PRICE_LEVEL_EXPENSIVE;
                else if (priceLevel == _veryExpensive)
                    return PriceLevel.PRICE_LEVEL_VERY_EXPENSIVE;
                else
                    return PriceLevel.PRICE_LEVEL_VERY_EXPENSIVE;
            }
            return PriceLevel.PRICE_LEVEL_VERY_EXPENSIVE;
        }
    }
}
