using System.Globalization;
using WhatMunch_MAUI.Models.Fonts;

namespace WhatMunch_MAUI.Converters
{
    public class PrimaryTypeToIconConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string type)
            {
                return type switch
                {
                    "american_restaurant" => FaSolid.Hotdog[0],
                    "asian_restaurant" or "thai_restaurant" or "sushi_restaurant" => FaSolid.BowlRice[0],
                    "bakery" => FaSolid.BreadSlice[0],
                    "brunch_restaurant" or "breakfast_restaurant" => FaSolid.Bacon[0],
                    "cafe" or "coffee_shop" => FaSolid.MugHot[0],
                    "meal_takeaway" => FaSolid.DrumstickBite[0],
                    "mexican_restaurant" => FaSolid.PepperHot[0],
                    "pub" => FaSolid.BeerMugEmpty[0],
                    "ramen_restaurant" => FaSolid.BowlFood[0],
                    "wine_bar" or "bar" => FaSolid.WineGlass[0],
                    "fast_food_restaurant" => FaSolid.Burger[0],
                    "seafood_restaurant" => FaSolid.Shrimp[0],
                    "vegan_restaurant" or "vegetarian_restaurant" => FaSolid.Seedling[0],
                    "italian_restaurant" or "pizza_restaurant" => FaSolid.PizzaSlice[0],
                    "dessert_restaurant " or "dessert_shop" => FaSolid.IceCream[0],
                    "candy_store" => FaSolid.CandyCane[0],
                    _ => FaSolid.Utensils[0]
                };
            }

            return string.Empty;
        }
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return 0;
        }
    }
    public class PrimaryTypeToColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not string type)
                return Colors.Transparent;

            bool isDark = Application.Current?.RequestedTheme == AppTheme.Dark;

            return type switch
            {
                "american_restaurant" => GetColor("Tomato", isDark),
                "asian_restaurant" or "thai_restaurant" or "sushi_restaurant" => GetColor("SandyBrown", isDark),
                "bakery" => GetColor("NaplesYellow", isDark),
                "brunch_restaurant" or "breakfast_restaurant" => GetColor("Tomato", isDark),
                "cafe" or "coffee_shop" => GetColor("TrueBrown", isDark),
                "meal_takeaway" => GetColor("Tomato", isDark),
                "mexican_restaurant" => GetColor("Tomato", isDark),
                "pub" => GetColor("NaplesYellow", isDark),
                "ramen_restaurant" => GetColor("SandyBrown", isDark),
                "wine_bar" or "bar" => GetColor("Tomato", isDark),
                "fast_food_restaurant" => GetColor("Tomato", isDark),
                "seafood_restaurant" => GetColor("YaleBlue", isDark),
                "vegan_restaurant" or "vegetarian_restaurant" => GetColor("OliveGreen", isDark),
                "italian_restaurant" or "pizza_restaurant" => GetColor("Tomato", isDark),
                "dessert_restaurant" or "dessert_shop" => GetColor("NaplesYellow", isDark),
                "candy_store" => GetColor("Tomato", isDark),
                _ => GetColor("YaleBlue", isDark)
            };
        }

        private static Color GetColor(string baseKey, bool isDark)
        {
            string key = isDark ? $"{baseKey}Dark" : baseKey;
            return Application.Current?.Resources[key] as Color ?? Colors.Black;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
