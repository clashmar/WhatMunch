using System.Text.Json.Serialization;

namespace WhatMunch_MAUI.Models.Dtos
{
    public class TextSearchResponseDto
    {
        [JsonPropertyName("places")]
        public List<PlaceDto> Places { get; set; } = [];

        [JsonPropertyName("nextPageToken")]
        public string? NextPageToken { get; set; }
    }
}
