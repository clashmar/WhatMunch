using WhatMunch_MAUI.Models.Dtos;

namespace WhatMunch_MAUI.Models
{
    public partial class PlaceModel : ObservableObject
    {
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
        private PriceLevel _priceLevel;

        [ObservableProperty]
        private bool _openNow;

        [ObservableProperty]
        private List<PlacePhoto> _photos = [];

        [ObservableProperty]
        private bool _goodForChildren;

        [ObservableProperty]
        private bool _allowsDogs;

        [ObservableProperty]
        private string? _mainPhoto;
    }
}
