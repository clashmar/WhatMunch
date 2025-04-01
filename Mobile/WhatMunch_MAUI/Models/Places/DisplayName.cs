using System.Text.Json.Serialization;

namespace WhatMunch_MAUI.Models.Places
{
    public class DisplayName
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;

        [JsonPropertyName("languageCode")]
        public string LanguageCode { get; set; } = string.Empty;
    }
}
