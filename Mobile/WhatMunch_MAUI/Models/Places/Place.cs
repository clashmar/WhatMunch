using System.Text.Json.Serialization;

namespace WhatMunch_MAUI.Models.Places
{
    public class Place
    {
        [JsonPropertyName("displayName")]
        public required DisplayName DisplayName { get; set; }

        [JsonPropertyName("primaryType")]
        public string PrimaryType { get; set; } = string.Empty;

        [JsonPropertyName("types")]
        public List<string> Types { get; set; } = [];

        [JsonPropertyName("rating")]
        public double Rating { get; set; }

        [JsonPropertyName("userRatingCount")]
        public int UserRatingCount { get; set; }

        [JsonPropertyName("regularOpeningHours")]
        public required RegularOpeningHours RegularOpeningHours { get; set; }

        [JsonPropertyName("photos")]
        public List<PlacePhoto> Photos { get; set; } = [];

    }
}
