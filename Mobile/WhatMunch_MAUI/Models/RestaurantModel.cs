namespace WhatMunch_MAUI.Models
{
    public class Restaurant
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public float Rating { get; set; }
        public int PriceLevel { get; set; }
        public string? Address { get; set; }
        public Photo[] Photos { get; set; } = [];
        public OpeningHours? OpeningHours { get; set; }
        public Geometry? Geometry { get; set; }
        public string[] CuisineTypes { get; set; } = [];
    }

    public class OpeningHours
    {
        public bool OpenNow { get; set; }
    }

    public class Geometry
    {
        public Location? Location { get; set; }
    }

    public class Location
    {
        public float Lat { get; set; }
        public float Lng { get; set; }
    }

    public class Photo
    {
        public string Source { get; set; } = "";
        public int Height { get; set; }
        public int Width { get; set; }
    }
}
