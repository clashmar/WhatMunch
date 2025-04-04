using System.Text.Json.Serialization;
using WhatMunch_MAUI.Models.Places;

namespace WhatMunch_MAUI.Models.Dtos
{
    public class TextSearchRequestDto
    {
        [JsonPropertyName("textQuery")]
        public string TextQuery { get; set; } = "Food";

        [JsonPropertyName("includedType")]
        public string? IncludedType { get; set; }

        [JsonPropertyName("locationRestriction")]
        public required LocationRestriction LocationRestriction { get; set; }

        [JsonPropertyName("minRating")]
        public double MinRating { get; set; } = 0;
 
        [JsonPropertyName("openNow")]
        public bool OpenNow { get; set; } = true;

        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; } = 20;

        [JsonPropertyName("pageToken")]
        public string? PageToken { get; set; }

        [JsonPropertyName("priceLevels")]
        public PriceLevel[]? PriceLevels { get; set; }

        [JsonPropertyName("rankPreference")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public required RankPreference RankPreference { get; set; } = RankPreference.DISTANCE;
    }

    public class LocationRestriction
    {
        [JsonPropertyName("circle")]
        public required Circle Circle { get; set; } 
    }

    public class Circle
    {
        [JsonPropertyName("center")]
        public required Center Center { get; set; }

        [JsonPropertyName("radius")]
        public double Radius { get; set; } = 500;
    }
    public class Center
    {
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; } = 37.7937;

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; } = -122.3965;
    }
}
