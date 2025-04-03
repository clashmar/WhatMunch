using Newtonsoft.Json;

namespace WhatMunch_MAUI.Models
{
    public enum RankPreference
    {
        DISTANCE,
        POPULARITY
    }

    public enum PriceLevel
    {
        PRICE_LEVEL_FREE,
        PRICE_LEVEL_INEXPENSIVE,
        PRICE_LEVEL_MODERATE,
        PRICE_LEVEL_EXPENSIVE,
        PRICE_LEVEL_VERY_EXPENSIVE
    }

    public partial class SearchPreferencesModel : ObservableValidator
    {
        [JsonProperty("minRating")]
        [ObservableProperty]
        public int _minRating = 0;

        [JsonProperty("maxPriceLevel")]
        [ObservableProperty]
        public PriceLevel _maxPriceLevel = PriceLevel.PRICE_LEVEL_VERY_EXPENSIVE;

        [JsonProperty("isVegetarian")]
        [ObservableProperty]
        public bool _isVegetarian = false;

        [JsonProperty("isVegan")]
        [ObservableProperty]
        public bool _isVegan = false;

        [JsonProperty("isChildFriendly")]
        [ObservableProperty]
        public bool _isChildFriendly = false;

        [JsonProperty("isDogFriendly")]
        [ObservableProperty]
        public bool _isDogFriendly = false;

        [JsonProperty("rankPreference")]
        [ObservableProperty]
        public RankPreference _rankPreference = RankPreference.DISTANCE;

        [JsonIgnore]
        public static SearchPreferencesModel Default => new();
    }
}
