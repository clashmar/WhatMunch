using System.Text.Json.Serialization;

namespace WhatMunch_MAUI.Dtos
{
    public class SignInRequestDto
    {
        [JsonPropertyName("username")]
        public string Username { get; set; } = "";

        [JsonPropertyName("password")]
        public string Password { get; set; } = "";
    }
}
