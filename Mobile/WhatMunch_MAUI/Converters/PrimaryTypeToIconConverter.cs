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
}
