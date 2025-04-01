using System.Text.Json.Serialization;

namespace WhatMunch_MAUI.Models.Places
{
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
