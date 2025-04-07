using WhatMunch_MAUI.Models.Dtos;

namespace WhatMunch_MAUI.Extensions
{
    public static class SearchExtensions
    {
        public static List<PlaceDto> FilterPreferences(this List<PlaceDto> places, SearchPreferencesModel preferences)
        {
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
