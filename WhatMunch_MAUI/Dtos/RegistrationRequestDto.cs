using System.Text.Json.Serialization;

namespace WhatMunch_MAUI.Dtos
{
    public class RegistrationRequestDto
    {
        [JsonPropertyName("email")]
        public string Email { get; set; } = "";

        [JsonPropertyName("username")]
        public string Username { get; set; } = "";

        [JsonPropertyName("password")]
        public string Password { get; set; } = "";
    }
}
