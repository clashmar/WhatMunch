using WhatMunch_MAUI.Models.Dtos;
using WhatMunch_MAUI.Resources.Localization;

namespace WhatMunch_MAUI.Models
{
    public partial class PlaceModel
    {
        public string Id { get; set; } = string.Empty;
     
        public string? DisplayName { get; set; }
        
        public string PrimaryType { get; set; } = string.Empty;

        public List<DisplayAttribute> Attributes { get; set; } = [];

        public double Rating { get; set; }

        public int UserRatingCount { get; set; }
        
        public PriceLevelDisplay? PriceLevel { get; set; }
        
        public bool OpenNow { get; set; }
        
        public List<string> OpeningTimes { get; set; } = [];
        
        public List<string> Photos { get; set; } = [];

        public bool GoodForChildren { get; set; }
        
        public bool AllowsDogs { get; set; }

        public string RatingSummary { get; set; }  = string.Empty;

        public string Stars { get; set; } = string.Empty;

        public string InternationalPhoneNumber { get; set; } = AppResources.NotAvailable;

        public string Website { get; set; } = AppResources.NotAvailable;
        
        public string Address { get; set; } = AppResources.NotAvailable;

        public List<Review> Reviews { get; set; } = [];
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
