using System.Text.Json.Serialization;

namespace WhatMunch_MAUI.Dtos
{
    public class SignInResponseDto
    {
        [JsonPropertyName("refresh")]
        public string RefreshToken { get; set; } = "";

        [JsonPropertyName("access")]
        public string AccessToken { get; set; } = "";
    }
}
