using System.Text.Json.Serialization;

namespace WhatMunch_MAUI.Models.Dtos
{
    public class LoginResponseDto
    {
        [JsonPropertyName("refresh")]
        public string RefreshToken { get; set; } = "";

        [JsonPropertyName("access")]
        public string AccessToken { get; set; } = "";
    }
}
