using System.Text.Json.Serialization;
using WhatMunch_MAUI.Models.Places;

namespace WhatMunch_MAUI.Dtos
{
    public class NearbySearchResponseDto
    {
        [JsonPropertyName("places")]
        public List<Place> Places { get; set; } = [];
    }
}
