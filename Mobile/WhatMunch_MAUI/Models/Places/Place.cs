using System.Text.Json.Serialization;

namespace WhatMunch_MAUI.Models.Places
{
    public enum PriceLevel
    {
        PRICE_LEVEL_FREE,
        PRICE_LEVEL_INEXPENSIVE,
        PRICE_LEVEL_MODERATE,
        PRICE_LEVEL_EXPENSIVE,
        PRICE_LEVEL_VERY_EXPENSIVE
    }

    public class Place
    {
        [JsonPropertyName("displayName")]
        public DisplayName? DisplayName { get; set; }

        [JsonPropertyName("primaryType")]
        public string PrimaryType { get; set; } = string.Empty;

        [JsonPropertyName("types")]
        public List<string> Types { get; set; } = [];

        [JsonPropertyName("rating")]
        public double Rating { get; set; }

        [JsonPropertyName("userRatingCount")]
        public int UserRatingCount { get; set; }

        [JsonPropertyName("priceLevel")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PriceLevel PriceLevel { get; set; }

        [JsonPropertyName("regularOpeningHours")]
        public RegularOpeningHours? RegularOpeningHours { get; set; }

        [JsonPropertyName("photos")]
        public List<PlacePhoto> Photos { get; set; } = [];

        [JsonPropertyName("goodForChildren")]
        public bool GoodForChildren { get; set; }

        [JsonPropertyName("allowsDogs")]
        public bool AllowsDogs { get; set; }
    }
}
