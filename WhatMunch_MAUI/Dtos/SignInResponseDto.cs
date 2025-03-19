using System.Text.Json.Serialization;

namespace WhatMunch_MAUI.Dtos
{
    public class SignInResponseDto
    {
        [JsonPropertyName("refresh")]
        public string Refresh { get; set; } = "";

        [JsonPropertyName("access")]
        public string Access { get; set; } = "";
    }
}
