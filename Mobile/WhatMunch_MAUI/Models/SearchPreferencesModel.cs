using System.Text.Json.Serialization;

namespace WhatMunch_MAUI.Models
{
    public enum PriceLevel
    {
        PRICE_LEVEL_INEXPENSIVE,
        PRICE_LEVEL_MODERATE,
        PRICE_LEVEL_EXPENSIVE,
        PRICE_LEVEL_VERY_EXPENSIVE
    }

    public enum RankPreference
    {
        DISTANCE,
        RELEVANCE
    }

    public partial class SearchPreferencesModel : ObservableValidator
    {
        [JsonPropertyName("minRating")]
        [ObservableProperty]
        public int _minRating = 0;

        [JsonPropertyName("maxPriceLevel")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        [ObservableProperty]
        public PriceLevel _maxPriceLevel = PriceLevel.PRICE_LEVEL_VERY_EXPENSIVE;

        [JsonPropertyName("radius")]
        [ObservableProperty]
        public double _searchRadius = 800;

        [JsonPropertyName("isVegetarian")]
        [ObservableProperty]
        public bool _isVegetarian = false;

        [JsonPropertyName("isVegan")]
        [ObservableProperty]
        public bool _isVegan = false;

        [JsonPropertyName("isChildFriendly")]
        [ObservableProperty]
        public bool _isChildFriendly = false;

        [JsonPropertyName("isDogFriendly")]
        [ObservableProperty]
        public bool _isDogFriendly = false;

        [JsonPropertyName("rankPreference")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        [ObservableProperty]
        public RankPreference _rankPreference = RankPreference.DISTANCE;

        [JsonIgnore]
        public static SearchPreferencesModel Default => new();

        public string[] GetPriceLevels()
        {
            return Enumerable.Range((int)PriceLevel.PRICE_LEVEL_INEXPENSIVE, (int)MaxPriceLevel + 1)
                             .Select(p => ((PriceLevel)p).ToString())
                             .ToArray();
        }
    }
}
