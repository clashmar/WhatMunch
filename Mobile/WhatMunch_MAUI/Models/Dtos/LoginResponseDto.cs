using System.Text.Json.Serialization;

namespace WhatMunch_MAUI.Models.Dtos
{
    public class LoginResponseDto(string accessToken, string refreshToken)
    {
        [JsonPropertyName("refresh")]
        public string RefreshToken { get; set; } = refreshToken;

        [JsonPropertyName("access")]
        public string AccessToken { get; set; } = accessToken;
    }
}
