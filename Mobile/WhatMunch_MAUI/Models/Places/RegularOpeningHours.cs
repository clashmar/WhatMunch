using System.Text.Json.Serialization;

namespace WhatMunch_MAUI.Models.Places
{
    public class RegularOpeningHours
    {
        [JsonPropertyName("openNow")]
        public bool OpenNow { get; set; }
    }
}
