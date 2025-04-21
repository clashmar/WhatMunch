using System.Text.Json.Serialization;

namespace WhatMunch_MAUI.Models.Dtos
{
    public class LoginResponseDto
    {
        [JsonPropertyName("refresh")]
        public string RefreshToken { get; set; } = string.Empty;

        [JsonPropertyName("access")]
        public string AccessToken { get; set; } = string.Empty;
    }
}
