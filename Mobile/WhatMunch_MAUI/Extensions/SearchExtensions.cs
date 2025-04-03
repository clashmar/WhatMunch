using WhatMunch_MAUI.Models.Places;

namespace WhatMunch_MAUI.Extensions
{
    public static class SearchExtensions
    {
        public static List<Place> FilterPreferences(this List<Place> places, SearchPreferencesModel preferences)
        {
            places = places
                .Where(p => p.Rating >= preferences.MinRating)
                .Where(p => p.PriceLevel <= preferences.MaxPriceLevel)
                .ToList();

            if (preferences.IsDogFriendly)
            {
                places = places.Where(p => p.AllowsDogs == true).ToList();
            }

            if (preferences.IsChildFriendly)
            {
                places = places.Where(p => p.GoodForChildren == true).ToList();
            }

            return places;
        }
    }
}
