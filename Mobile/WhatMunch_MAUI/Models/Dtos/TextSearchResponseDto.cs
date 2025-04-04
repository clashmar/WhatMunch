using System.Text.Json.Serialization;
using WhatMunch_MAUI.Models.Places;

namespace WhatMunch_MAUI.Models.Dtos
{
    public class TextSearchResponseDto
    {
        [JsonPropertyName("places")]
        public List<Place> Places { get; set; } = [];

        [JsonPropertyName("nextPageToken")]
        public string? NextPageToken { get; set; }
    }
}
