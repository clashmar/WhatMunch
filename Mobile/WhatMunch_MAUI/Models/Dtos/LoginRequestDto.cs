using System.Text.Json.Serialization;

namespace WhatMunch_MAUI.Models.Dtos
{
    public class LoginRequestDto
    {
        [JsonPropertyName("username")]
        public string Username { get; set; } = "";

        [JsonPropertyName("password")]
        public string Password { get; set; } = "";
    }
}
