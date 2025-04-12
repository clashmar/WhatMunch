using WhatMunch_MAUI.Models.Dtos;
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
        private List<DisplayAttribute> _attributes = [];

        [ObservableProperty]
        private double _rating;

        [ObservableProperty]
        private int _userRatingCount;

        [ObservableProperty]
        private PriceLevelDisplay? _priceLevel;

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

        [ObservableProperty]
        private List<Review> _reviews = [];
    }

    public class DisplayAttribute(string? icon, string text)
    {
        public string? Icon { get; set; } = icon;
        public string Text { get; set; } = text;
    }

    public class PriceLevelDisplay(string number, string remainder)
    {
        public string Number { get; set; } = number;
        public string Remainder { get; set; } = remainder;
    }
}
