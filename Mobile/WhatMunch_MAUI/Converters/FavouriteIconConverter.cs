using System.Globalization;
using WhatMunch_MAUI.Data;
using WhatMunch_MAUI.Models.Dtos;
using WhatMunch_MAUI.Views;

namespace WhatMunch_MAUI.Converters
{
    class FavouriteIconConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var place = values[0] as PlaceDto;
            var page = values[1] as string;

            if (place is not null && !string.IsNullOrEmpty(page))
            {
                return page switch
                {
                    Constants.SAVED_PLACES_PAGE => "delete",
                    Constants.SEARCH_RESULTS_PAGE => place.IsFavourite ? "liked" : "like",
                    _ => "liked"
                };
            }

            return "liked";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
