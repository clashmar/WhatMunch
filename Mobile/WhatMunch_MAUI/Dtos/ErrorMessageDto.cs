using System.Text.Json.Serialization;

namespace WhatMunch_MAUI.Dtos
{
    public class ErrorMessageDto
    {
        [JsonPropertyName("detail")]
        public string ErrorMessage { get; set; } = "";
    }
}
