using WhatMunch_MAUI.Models.Dtos;

namespace WhatMunch_MAUI.Extensions
{
    public static class SearchExtensions
    {
        public static List<PlaceDto> AddDistances(this List<PlaceDto> places, Location? location = null)
        {
            ArgumentNullException.ThrowIfNull(places);
            if (location is null) return places;

            return places
                .Select(p =>
                {
                    if (p.Location is not null)
                    {
                        p.Distance = location.CalculateDistance(p.Location.Latitude, p.Location.Longitude, DistanceUnits.Kilometers);
                    }
                    return p;
                })
                .ToList();
        }
        public static List<PlaceDto> FilterDistances(this List<PlaceDto> places)
        {
            ArgumentNullException.ThrowIfNull(places);

            return places.Where(p => p.Distance < 3.0).ToList();
        }
    }
}
