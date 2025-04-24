using System.Text.Json.Serialization;
using WhatMunch_MAUI.Resources.Localization;
using WhatMunch_MAUI.Extensions;

namespace WhatMunch_MAUI.Models.Dtos
{
    public partial class PlaceDto : ObservableObject
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("primaryType")]
        public string PrimaryType { get; set; } = string.Empty;

        [JsonPropertyName("displayName")]
        public DisplayName? DisplayName { get; set; }

        [JsonPropertyName("primaryTypeDisplayName")]
        public PrimaryTypeDisplayName? PrimaryTypeDisplayName { get; set; } 

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

        [JsonPropertyName("internationalPhoneNumber")]
        public string InternationalPhoneNumber { get; set; } = AppResources.NotAvailable;

        [JsonPropertyName("websiteUri")]
        public string WebsiteUri { get; set; } = AppResources.NotAvailable;

        [JsonPropertyName("shortFormattedAddress")]
        public string ShortFormattedAddress { get; set; } = AppResources.NotAvailable;

        [JsonPropertyName("location")]
        public PlaceLocation? Location { get; set; }

        [JsonPropertyName("reviews")]
        public List<Review> Reviews { get; set; } = [];

        [JsonPropertyName("isFavourite")]
        [ObservableProperty]
        public bool _isFavourite;

        [JsonPropertyName("generativeSummary")]
        public GenerativeSummary? GenerativeSummary { get; set; }

        [JsonPropertyName("reviewSummary")]
        public ReviewSummary? ReviewSummary { get; set; }

        [JsonIgnore]
        public double Distance { get; set; }

        [JsonIgnore]
        public string DisplayNameText => DisplayName?.Text.ToDisplayNameText() ?? AppResources.NotAvailable;

        [JsonPropertyName("lastUpdatedUtc")]
        public DateTime LastUpdatedUtc { get; set; } = DateTime.UtcNow;
    }

    public class DisplayName
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } = AppResources.NotAvailable;

        [JsonPropertyName("languageCode")]
        public string LanguageCode { get; set; } = string.Empty;
    }

    public class PrimaryTypeDisplayName
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } = AppResources.NotAvailable;
    }

    public class RegularOpeningHours
    {
        [JsonPropertyName("openNow")]
        public bool OpenNow { get; set; }

        [JsonPropertyName("weekdayDescriptions")]
        public List<string> WeekdayDescriptions { get; set; } = [];
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
    public class PlaceLocation
    {
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }
    }

    public class Review
    {
        [JsonPropertyName("relativePublishTimeDescription")]
        public string RelativePublishTimeDescription { get; set; } = string.Empty;

        [JsonPropertyName("rating")]
        public int Rating { get; set; }

        [JsonPropertyName("text")]
        public ReviewText? Text { get; set; } 
    }

    public class ReviewText
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;

        [JsonPropertyName("languageCode")]
        public string LanguageCode { get; set; } = string.Empty;
    }

    public class GenerativeSummary
    {
        [JsonPropertyName("overview")]
        public Overview? Overview { get; set; }
    }
    public class ReviewSummary
    {
        [JsonPropertyName("text")]
        public Overview? Text { get; set; }
    }

    public class Overview
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;
    }
}
