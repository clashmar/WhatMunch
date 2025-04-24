using WhatMunch_MAUI.Models.Dtos;

namespace WhatMunch_MAUI.Extensions
{
    public static class SearchExtensions
    {
        public static IEnumerable<PlaceDto> AddDistances(this IEnumerable<PlaceDto> places, Location? location = null)
        {
            ArgumentNullException.ThrowIfNull(places);
            if (location is null) return places;

            return places
                .Select(p =>
                {
                    if (p.Location is not null)
                    {
                        p.Distance = location.CalculateDistance(
                            p.Location.Latitude, 
                            p.Location.Longitude, 
                            DistanceUnits.Kilometers);
                    }
                    return p;
                })
                .ToList();
        }

        // TODO: Unit test AddDistance
        public static PlaceDto AddDistance(this PlaceDto place, Location? location = null)
        {
            ArgumentNullException.ThrowIfNull(place);
            if (location is null) return place;

            if(place.Location is not null)
            {
                place.Distance = location.CalculateDistance(
                            place.Location.Latitude,
                            place.Location.Longitude,
                            DistanceUnits.Kilometers);
            }
            return place;
        }

        public static IEnumerable<PlaceDto> FilterDistances(this IEnumerable<PlaceDto> places)
        {
            ArgumentNullException.ThrowIfNull(places);

            return places.Where(p => p.Distance < 3.0);
        }

        public static IEnumerable<PlaceDto> CheckIsFavourite(this IEnumerable<PlaceDto> places, IEnumerable<PlaceDto> favourites)
        {
            ArgumentNullException.ThrowIfNull(places);
            ArgumentNullException.ThrowIfNull(favourites);

            var favouriteIds = favourites.Select(f => f.Id).ToHashSet();

            foreach (var place in places)
            {
                place.IsFavourite = favouriteIds.Contains(place.Id);
            }

            return places;
        }
    }
}
