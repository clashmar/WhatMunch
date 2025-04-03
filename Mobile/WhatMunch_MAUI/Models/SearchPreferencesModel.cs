using Newtonsoft.Json;

namespace WhatMunch_MAUI.Models
{
    public partial class SearchPreferencesModel : ObservableValidator
    {
        [JsonProperty("minRating")]
        [ObservableProperty]
        public int _minRating;

        [JsonProperty("maxPriceLevel")]
        [ObservableProperty]
        public int _maxPriceLevel;

        [JsonProperty("isVegetarian")]
        [ObservableProperty]
        public bool _isVegetarian;

        [JsonProperty("isVegan")]
        [ObservableProperty]
        public bool _isVegan;

        [JsonProperty("isChildFriendly")]
        [ObservableProperty]
        public bool _isChildFriendly;

        [JsonProperty("isDogFriendly")]
        [ObservableProperty]
        public bool _isDogFriendly;
    }
}
