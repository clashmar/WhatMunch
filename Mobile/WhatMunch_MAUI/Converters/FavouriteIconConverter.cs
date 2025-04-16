using System.Globalization;
using WhatMunch_MAUI.Models.Dtos;

namespace WhatMunch_MAUI.Converters
{
    class FavouriteIconConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var place = values[0] as PlaceDto;
            var mode = values[1] as string;

            if (place is not null && !string.IsNullOrEmpty(mode))
            {
                return mode switch
                {
                    "saved" => "delete",
                    "search" => place.IsFavourite ? "liked" : "like",
                    _ => "liked"
                };
            }

            return "liked";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
