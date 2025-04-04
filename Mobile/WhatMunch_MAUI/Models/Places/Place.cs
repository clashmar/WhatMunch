using System.Text.Json.Serialization;

namespace WhatMunch_MAUI.Models.Places
{
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

    public class DisplayName
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;

        [JsonPropertyName("languageCode")]
        public string LanguageCode { get; set; } = string.Empty;
    }

    public class RegularOpeningHours
    {
        [JsonPropertyName("openNow")]
        public bool OpenNow { get; set; }
    }
    public class PlacePhoto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("widthPx")]
        public int WidthPx { get; set; }

        [JsonPropertyName("heightPx")]
        public int HeightPx { get; set; }

        [JsonPropertyName("googleMapsUri")]
        public string GoogleMapsUri { get; set; } = string.Empty;
    }
}
