using WhatMunch_MAUI.Resources.Localization;

namespace WhatMunch_MAUI.Models
{
    public partial class PlaceModel : ObservableObject
    {
        [ObservableProperty]
        private string _id = string.Empty;

        [ObservableProperty]
        private string? _displayName;

        [ObservableProperty]
        private string _primaryType = string.Empty;

        [ObservableProperty]
        private List<string> _types = [];

        [ObservableProperty]
        private double _rating;

        [ObservableProperty]
        private int _userRatingCount;

        [ObservableProperty]
        private (string number, string remainder) _priceLevel;

        [ObservableProperty]
        private bool _openNow;

        [ObservableProperty]
        private List<string> _openingTimes = [];

        [ObservableProperty]
        private List<string> _photos = [];

        [ObservableProperty]
        private bool _goodForChildren;

        [ObservableProperty]
        private bool _allowsDogs;

        [ObservableProperty]
        private string _ratingSummary = string.Empty;

        [ObservableProperty]
        private string _stars = string.Empty;

        [ObservableProperty]
        private string _internationalPhoneNumber = AppResources.NotAvailable;

        [ObservableProperty]
        private string _website = AppResources.NotAvailable;

        [ObservableProperty]
        private string _address = AppResources.NotAvailable;
    }
}
