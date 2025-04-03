using System.Text.Json.Serialization;
using WhatMunch_MAUI.Models.Places;

namespace WhatMunch_MAUI.Models
{
    public enum RankPreference
    {
        DISTANCE,
        POPULARITY
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
        [ObservableProperty]
        public RankPreference _rankPreference = RankPreference.DISTANCE;

        [JsonIgnore]
        public static SearchPreferencesModel Default => new();
    }
}
