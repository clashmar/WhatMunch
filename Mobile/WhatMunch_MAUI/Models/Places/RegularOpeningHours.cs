using System.Text.Json.Serialization;

namespace WhatMunch_MAUI.Models.Places
{
    public class RegularOpeningHours
    {
        [JsonPropertyName("openNow")]
        public required bool OpenNow { get; set; }
    }
}
